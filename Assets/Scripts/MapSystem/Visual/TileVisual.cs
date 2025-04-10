using Unity.Entities;

public struct TileVisualComponent : IComponentData
{
    public Entity MeshEntity; // 代表实际的显示实体
    public float SnowBlend;
    public float HeightOffset;
}
