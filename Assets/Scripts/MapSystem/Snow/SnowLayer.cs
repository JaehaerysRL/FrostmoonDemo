using Unity.Entities;

public struct SnowLayer : IComponentData
{
    public float Thickness;        // 当前积雪厚度
    public float MaxThickness;     // 最大积雪厚度（控制上限）
    public float VarianceSeed;     // 随机种子
}
