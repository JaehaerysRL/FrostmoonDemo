using Unity.Mathematics;

// 常量类
public static class GlobalConst
{
    // 地形类型数
    public const int TerrainTypeCount = 4;
    // 地图半径(格)
    public const int MapRadius = 30;
    // 尺和像素的比例
    public const int FeetToUnits = 20; // DND的尺寸转换为Unity单位的比例
    // 一格的大小,也就是移动的最小单位，5尺=1格
    public const int GridSize = 5;
    // 一格的像素
    public const int GridPixelSize = FeetToUnits * GridSize;
    // 一格的单位
    public const float GridUnitSize = 2;
}