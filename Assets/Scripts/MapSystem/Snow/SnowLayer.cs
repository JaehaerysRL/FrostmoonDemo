using Unity.Entities;

public struct SnowLayer : IComponentData
{
    public float Thickness;     // 当前积雪厚度
    public float AccumulationRate; // 每秒积累速度
    public float MeltingRate;      // 每秒融化速度
}
