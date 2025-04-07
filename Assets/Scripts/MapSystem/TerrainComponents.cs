using Unity.Mathematics;
using Unity.Entities;

public struct TerrainTile : IComponentData
{
    public int2 GridPos;      // 网格坐标
    public TerrainType Type;  // 地形类型枚举
    public float Height;      // 高度值（尺）
    public Entity VisualEntity; // 可视对象实体
}

public enum TerrainType
{
    Tundra, // 苔原
    Permafrost, // 冻土
    FrozenLake, // 冰湖
    Shrub, // 灌木
}