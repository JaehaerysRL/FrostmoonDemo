using Unity.Entities;

public struct Heightmap : IComponentData
{
    public float BaseHeight;   // 原始地形高度
    public float FinalHeight;  // 最终高度（= BaseHeight + Snow）
}
