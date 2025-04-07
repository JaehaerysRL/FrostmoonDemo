using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class TerrainGeneratorSystem : SystemBase
{
    [Header("地形生成参数")]
    public float LakeRadius = 60f; // 冰湖半径
    public float HeightMultiplier = 5f; // 高度乘数
    public float NoiseFrequency = 0.1f; // 噪声频率
    public float MaxNoise = 5f; // 最大噪声高度

    [ReadOnly] private NativeArray<int2> _biomeCenters;
    [ReadOnly] private NativeArray<int2> _frozenLakeCenters;

    protected override void OnCreate()
    {
        // 初始化生物群落中心点
        _biomeCenters = new NativeArray<int2>(2, Allocator.Persistent);
        _frozenLakeCenters = new NativeArray<int2>(1, Allocator.Persistent);
        var random = new Unity.Mathematics.Random(9966);
        for (int i = 0; i < _biomeCenters.Length; i++)
        {
            int2 center = new int2(
                random.NextInt(-GlobalConst.TerrainRadius, GlobalConst.TerrainRadius),
                random.NextInt(-GlobalConst.TerrainRadius, GlobalConst.TerrainRadius)
            );
            _biomeCenters[i] = center;
            if (i % GlobalConst.TerrainTypeCount == (int)TerrainType.FrozenLake)
            {
                _frozenLakeCenters[i / GlobalConst.TerrainTypeCount] = center;
            }
        }
    }

    protected override void OnDestroy()
    {
        // 释放生物群落中心点的内存
        if (_biomeCenters.IsCreated)
        {
            _biomeCenters.Dispose();
        }
        if (_frozenLakeCenters.IsCreated)
        {
            _frozenLakeCenters.Dispose();
        }
    }

    protected override void OnUpdate()
    {
        new GenerateJob {
            BiomeCenters = _biomeCenters,
            FrozenLakeCenters = _frozenLakeCenters,
            FrozenLakeRadius = LakeRadius,
            BaseHeightMultiplier = HeightMultiplier,
            NoiseScale = NoiseFrequency,
            MaxNoiseHeight = MaxNoise
        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct GenerateJob : IJobEntity
    {
        [ReadOnly] public NativeArray<int2> BiomeCenters;
        [ReadOnly] public NativeArray<int2> FrozenLakeCenters;
        public float FrozenLakeRadius;
        public float BaseHeightMultiplier;
        public float NoiseScale;
        public float MaxNoiseHeight;

        public void Execute(ref TerrainTile tile)
        {
            // 确定基础地形类型
            tile.Type = CalculateBiomeType(tile.GridPos);
            // 计算冰湖影响
            float lakeEffect = CalculateLakeInfluence(tile.GridPos);
            // 计算最终高度
            tile.Height = CalculateFinalHeight(tile.Type, tile.GridPos, lakeEffect) * BaseHeightMultiplier;
        }

        // 生物群落计算
        private TerrainType CalculateBiomeType(int2 gridPos)
        {
            float minDist = float.MaxValue;
            int biomeIndex = 0;

            for (int i = 0; i < BiomeCenters.Length; i++)
            {
                float dist = math.distance(gridPos, BiomeCenters[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                    biomeIndex = i;
                }
            }
            return (TerrainType)(biomeIndex % GlobalConst.TerrainTypeCount);
        }

        // 冰湖影响计算
        private float CalculateLakeInfluence(int2 gridPos)
        {
            float minLakeDist = float.MaxValue;

            foreach (var center in FrozenLakeCenters)
            {
                float dist = math.distance(gridPos, center);
                minLakeDist = math.min(dist, minLakeDist);
            }

            return math.saturate(1 - (minLakeDist / FrozenLakeRadius));
        }
        
      // 高度合成算法
        private float CalculateFinalHeight(TerrainType type, int2 position, float lakeEffect)
        {
            // 基础高度配置
            float baseHeight = type switch
            {
                TerrainType.Tundra => 0.5f,
                TerrainType.Permafrost => 0.3f,
                TerrainType.FrozenLake => 0f,
                TerrainType.Shrub => 0.2f,
                _ => 0f
            };

            // 噪声生成（使用柏林噪声）
            float2 noiseCoord = new float2(position.x * NoiseScale, position.y * NoiseScale);
            float noiseValue = noise.snoise(noiseCoord) * MaxNoiseHeight;
            // 冰湖影响曲线（三次平滑）
            float lakeEffectCurve = lakeEffect * lakeEffect * (3f - 2f * lakeEffect);
            // 高度混合计算
            float finalHeight = (baseHeight + noiseValue) * (1 - lakeEffectCurve);
            // 强制冰湖区域高度为0
            if (type == TerrainType.FrozenLake) finalHeight = 0;
            return math.max(finalHeight, 0);
        }
    }
}