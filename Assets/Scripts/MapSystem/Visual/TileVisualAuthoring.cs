using Unity.Entities;
using UnityEngine;

public class TileVisualAuthoring : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
}

public class TileVisualBaker : Baker<TileVisualAuthoring>
{
    public override void Bake(TileVisualAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Renderable);

        AddComponent(entity, new TileVisualComponent
        {
            MeshEntity = entity,
            SnowBlend = 0,
            HeightOffset = 0
        });
    }
}
