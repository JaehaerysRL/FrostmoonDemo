using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct SnowAccumulationSystem : ISystem
{
    private float randomOffset;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        randomOffset = UnityEngine.Random.Range(0f, 1000f); // 每次进游戏不同随机性
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float time = (float)SystemAPI.Time.ElapsedTime;

        foreach (var (snow, heightmap, ceil) in
                 SystemAPI.Query<RefRW<SnowLayer>, RefRW<Heightmap>, RefRW<TerrainCell>>())
        {
            // 简单噪声模拟环境变化（雪强度）
            float noise = Unity.Mathematics.noise.snoise(new float2(time * 0.05f + randomOffset, 0));
            float dynamicRate = math.saturate(0.5f + 0.5f * noise); // 0 ~ 1 波动

            float netChange = (snow.ValueRO.AccumulationRate * dynamicRate
                              - snow.ValueRO.MeltingRate) * deltaTime;

            snow.ValueRW.Thickness = math.clamp(snow.ValueRO.Thickness + netChange, 0f, 2f);

            // 更新最终高度
            heightmap.ValueRW.FinalHeight = heightmap.ValueRO.BaseHeight + snow.ValueRW.Thickness;
            ceil.ValueRW.Height = heightmap.ValueRW.BaseHeight;
        }
    }
}
