using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Serialization.Json;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct TerrainGenerateSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TerrainGenerationConfig>();
        state.RequireForUpdate<TilePrefabConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        SmartLog.Info("TerrainGenerateSystem", "OnUpdate");
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var generationConfig = SystemAPI.GetSingleton<TerrainGenerationConfig>();
        var prefabConfig = SystemAPI.GetSingleton<TilePrefabConfig>();
        var centerEntity = entityManager.CreateEntity();
        entityManager.AddBuffer<BiomeCenter>(centerEntity);
        entityManager.AddBuffer<FrozenLakeCenter>(centerEntity);
        var biomeCenters = entityManager.GetBuffer<BiomeCenter>(centerEntity);
        var frozenLakeCenters = entityManager.GetBuffer<FrozenLakeCenter>(centerEntity);

        // 随机生成 BiomeCenter 和 FrozenLakeCenter
        var used = new NativeList<float2>(Allocator.Temp);
        var random = Unity.Mathematics.Random.CreateFromIndex(9966);
        int maxAttempts = 1000; // 安全阀值
        int currentAttempts = 0;
        while (biomeCenters.Length < generationConfig.BiomeCount)
        {
            currentAttempts++;
            // MapRadius实际是地图的XY轴长度，在范围内随机生成点
            float x = random.NextFloat(-generationConfig.MapRadius * 0.8f, generationConfig.MapRadius * 0.8f);
            float y = random.NextFloat(-generationConfig.MapRadius * 0.8f, generationConfig.MapRadius * 0.8f);
            float2 offset = new float2(x, y);

            bool tooClose = false;
            foreach (var u in used)
            {
                if (math.distance(u, offset) < 5f)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose || currentAttempts < maxAttempts)
            {
                biomeCenters.Add(new BiomeCenter { Position = (int2)math.round(offset) });
                used.Add(offset);
            }
        }
        // frozenLakeCenters是BiomeCenter的与 TerrainType.Frozenlake % System.Enum.GetValues(typeof(TerrainType)).Length同余的项，不够就补随机点
        var totalTypeCount = System.Enum.GetValues(typeof(TerrainType)).Length;
        for (int i = 0; i < generationConfig.FrozenLakeCount; i++)
        {
            if (biomeCenters.Length > (int)TerrainType.Frozenlake + i * totalTypeCount)
            {
                frozenLakeCenters.Add(new FrozenLakeCenter { Position = biomeCenters[(int)TerrainType.Frozenlake + i * totalTypeCount].Position });
            }
            else
            {
                float angle = random.NextFloat(0f, 30f);
                float r = random.NextFloat(generationConfig.MapRadius * 0.3f, generationConfig.MapRadius);
                float2 offset = new float2(math.cos(math.radians(angle)), math.sin(math.radians(angle))) * r;
                frozenLakeCenters.Add(new FrozenLakeCenter { Position = (int2)math.round(offset) });
            }
        }
        used.Dispose();
        SmartLog.Info("TerrainGenerateSystem", "Calculate center done");

        for (int x = -generationConfig.MapRadius; x <= generationConfig.MapRadius; x++)
        {
            for (int y = -generationConfig.MapRadius; y <= generationConfig.MapRadius; y++)
            {
                int2 gridPos = new int2(x, y);
                float2 worldPos = new float2(x * generationConfig.GridSize, y * generationConfig.GridSize);

                float lakeInfluence = CalculateLakeInfluence(gridPos, generationConfig, frozenLakeCenters);
                TerrainType type = CalculateBiomeType(gridPos, generationConfig, biomeCenters);

                if (lakeInfluence > 0.8f)
                    type = TerrainType.Frozenlake;

                float height = CalculateFinalHeight(type, gridPos, lakeInfluence, generationConfig);

                Entity tile = ecb.Instantiate(prefabConfig.GetPrefab(type));

                ecb.SetComponent(tile, new LocalTransform
                {
                    Position = new float3(worldPos.x, worldPos.y, 0),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                ecb.SetComponent(tile, new TileVisualComponent
                {
                    MeshEntity = tile,
                    HeightOffset = height,
                    SnowBlend = 0f
                });
                ecb.SetComponent(tile, new Heightmap
                {
                    BaseHeight = height,
                    FinalHeight = height
                });
                ecb.SetComponent(tile, new TerrainCell
                {
                    GridPos = gridPos,
                    Height = height,
                    Type = type
                }); 
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        SmartLog.Info("TerrainGenerateSystem", "Instantiate tiles done");

        state.Enabled = false; // 只运行一次
        SmartLog.Info("TerrainGenerateSystem", "OnUpdate End");
    }

    private static float CalculateLakeInfluence(int2 pos, TerrainGenerationConfig config, DynamicBuffer<FrozenLakeCenter> centers)
    {
        float minDist = float.MaxValue;
        foreach (var c in centers)
        {
            float dist = math.distance(pos, c.Position);
            if (dist < minDist)
                minDist = dist;
        }
        return math.saturate(1 - (minDist / config.FrozenLakeRadius));
    }

    private static TerrainType CalculateBiomeType(int2 pos, TerrainGenerationConfig config, DynamicBuffer<BiomeCenter> centers)
    {
        float minDist = float.MaxValue;
        int index = 0;
        for (int i = 0; i < centers.Length; i++)
        {
            float dist = math.distance(pos, centers[i].Position);
            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }
        return (TerrainType)(index % System.Enum.GetValues(typeof(TerrainType)).Length);
    }

    private static float CalculateFinalHeight(TerrainType type, int2 pos, float lakeEffect, TerrainGenerationConfig config)
    {
        float baseHeight = type switch
        {
            TerrainType.Tundra => 0.5f,
            TerrainType.Permafrost => 0.3f,
            TerrainType.Frozenlake => 0f,
            TerrainType.Shrub => 0.2f,
            _ => 0.1f
        };

        float2 coord = new float2(pos.x * config.NoiseScale, pos.y * config.NoiseScale);
        float noiseVal = noise.snoise(coord) * config.MaxNoiseHeight;
        float smoothLake = lakeEffect * lakeEffect * (3f - 2f * lakeEffect);
        float final = (baseHeight + noiseVal) * (1 - smoothLake);
        return type == TerrainType.Frozenlake ? 0 : math.max(final, 0);
    }
}
