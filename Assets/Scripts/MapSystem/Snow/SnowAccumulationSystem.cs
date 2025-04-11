using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct SnowAccumulationSystem : ISystem
{
    private uint randomSeed; // 使用 Burst 兼容的随机种子

    // 注意：OnCreate 不标记 BurstCompile，允许使用 UnityEngine.Random
    public void OnCreate(ref SystemState state)
    {
        // 仅在非 Burst 环境下初始化随机种子
        randomSeed = (uint)UnityEngine.Random.Range(1, int.MaxValue);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float time = (float)SystemAPI.Time.ElapsedTime;

        // 创建基于种子的随机数生成器 (Burst 兼容)
        var random = new Unity.Mathematics.Random(randomSeed);

        foreach (var (snow, heightmap, ceil) in
                 SystemAPI.Query<RefRW<SnowLayer>, RefRW<Heightmap>, RefRW<TerrainCell>>())
        {
            // 使用随机数生成器生成偏移量
            float noise = Unity.Mathematics.noise.snoise(new float2(
                time * 0.05f + random.NextFloat(0f, 1000f), // 每帧不同随机偏移
                0
            ));
            
            float dynamicRate = math.saturate(0.5f + 0.5f * noise);
            float netChange = (snow.ValueRO.AccumulationRate * dynamicRate - snow.ValueRO.MeltingRate) * deltaTime;

            snow.ValueRW.Thickness = math.clamp(snow.ValueRO.Thickness + netChange, 0f, 2f);
            heightmap.ValueRW.FinalHeight = heightmap.ValueRO.BaseHeight + snow.ValueRW.Thickness;
            ceil.ValueRW.Height = heightmap.ValueRW.BaseHeight;
        }

        // 更新种子以确保下一帧的随机性不同
        randomSeed = random.NextUInt();
    }
}