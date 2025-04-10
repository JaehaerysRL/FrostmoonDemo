using Unity.Entities;
using Unity.Mathematics;

public enum TerrainType {
    Tundra,         // 苔原（正常）
    Permafrost,     // 冻土（正常）
    Frozenlake,        // 冰湖（困难）
    Shrub            // 灌木丛（困难）
}

public struct TerrainCell : IComponentData {
    public int2 GridPos;         // 格子坐标（单位为格）
    public TerrainType Type;     // 地形类型
    public float Height;         // 高度（单位为尺）
}
