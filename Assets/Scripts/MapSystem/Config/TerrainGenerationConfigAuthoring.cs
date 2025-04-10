using UnityEngine;
using Unity.Entities;

public class TerrainGenerationConfigAuthoring : MonoBehaviour
{
    [Header("地图尺寸")]
    public readonly int mapRadius = GlobalConst.MapRadius;
    public readonly int gridSize = GlobalConst.GridSize;

    [Header("地形参数")]
    public int biomeCount = 12;
    public int frozenLakeCount = 3;
    public float frozenLakeRadius = 6f;
    public float noiseScale = 0.1f;
    public float maxNoiseHeight = 0.2f;
    public float baseHeightMultiplier = 1f;

    class Baker : Baker<TerrainGenerationConfigAuthoring>
    {
        public override void Bake(TerrainGenerationConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new TerrainGenerationConfig
            {
                MapRadius = authoring.mapRadius,
                GridSize = authoring.gridSize,
                FrozenLakeRadius = authoring.frozenLakeRadius,
                NoiseScale = authoring.noiseScale,
                MaxNoiseHeight = authoring.maxNoiseHeight,
                BaseHeightMultiplier = authoring.baseHeightMultiplier
            });
        }
    }
}
