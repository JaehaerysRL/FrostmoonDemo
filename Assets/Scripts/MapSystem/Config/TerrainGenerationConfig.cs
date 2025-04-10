using Unity.Entities;

public struct TerrainGenerationConfig : IComponentData
{
    public int MapRadius;
    public int GridSize;
    public float FrozenLakeRadius;
    public float NoiseScale;
    public float MaxNoiseHeight;
    public float BaseHeightMultiplier;
}
