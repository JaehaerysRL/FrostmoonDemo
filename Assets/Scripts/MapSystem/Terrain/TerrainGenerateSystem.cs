using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct TerrainGenerateSystem : ISystem
{
    private EntityCommandBufferSystem _ecbSystem;

    public void OnCreate(ref SystemState state)
    {
        _ecbSystem = state.World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();
        state.RequireForUpdate<TerrainGenerationConfig>();
        state.RequireForUpdate<TilePrefabConfig>();
        state.RequireForUpdate<BiomeCenter>();
        state.RequireForUpdate<FrozenLakeCenter>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // 获取第一个包含 BiomeCenter 的实体
        EntityQuery clusterQuery = state.GetEntityQuery(
            ComponentType.ReadOnly<BiomeCenter>(),
            ComponentType.ReadOnly<FrozenLakeCenter>()
        );

        if (clusterQuery.IsEmpty) return;
        Entity clusterEntity = clusterQuery.GetSingletonEntity();
        var biomeCenters = state.EntityManager.GetBuffer<BiomeCenter>(clusterEntity);
        var frozenLakeCenters = state.EntityManager.GetBuffer<FrozenLakeCenter>(clusterEntity);

        var generationConfig = SystemAPI.GetSingleton<TerrainGenerationConfig>();
        var prefabConfig = SystemAPI.GetSingleton<TilePrefabConfig>();
        var ecb = _ecbSystem.CreateCommandBuffer();

        for (int x = -generationConfig.MapRadius; x <= generationConfig.MapRadius; x++)
        {
            for (int y = -generationConfig.MapRadius; y <= generationConfig.MapRadius; y++)
            {
                int2 gridPos = new int2(x, y);
                float2 worldPos = new float2(x * generationConfig.GridSize, y * generationConfig.GridSize);

                float lakeInfluence = CalculateLakeInfluence(gridPos, generationConfig, frozenLakeCenters);
                TerrainType type = CalculateBiomeType(gridPos, generationConfig, biomeCenters);

                if (lakeInfluence > 0.9f)
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
            }
        }

        state.Enabled = false; // 只运行一次
    }


    private static float CalculateLakeInfluence(int2 pos, TerrainGenerationConfig config,
        DynamicBuffer<FrozenLakeCenter> frozenLakeCenters)
    {
        float minDist = float.MaxValue;
        for (int i = 0; i < frozenLakeCenters.Length; i++)
        {
            float dist = math.distance(pos, frozenLakeCenters[i].Position);
            if (dist < minDist)
                minDist = dist;
        }
        return math.saturate(1 - (minDist / config.FrozenLakeRadius));
    }

    private static TerrainType CalculateBiomeType(int2 pos, TerrainGenerationConfig config, 
        DynamicBuffer<BiomeCenter> biomeCenters)
    {
        float minDist = float.MaxValue;
        int index = 0;
        for (int i = 0; i < biomeCenters.Length; i++)
        {
            float dist = math.distance(pos, biomeCenters[i].Position);
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

