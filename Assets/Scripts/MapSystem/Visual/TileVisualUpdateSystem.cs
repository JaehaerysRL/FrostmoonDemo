using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class TileVisualUpdateSystem : SystemBase
{
    MaterialPropertyBlock block;
    EntityQuery m_Query;

    protected override void OnCreate()
    {
        block = new MaterialPropertyBlock();
        m_Query = GetEntityQuery(
            ComponentType.ReadOnly<TileVisualComponent>()
        );
    }

    protected override void OnUpdate()
    {
        var entities = m_Query.ToEntityArray(Allocator.Temp);
        var visuals = m_Query.ToComponentDataArray<TileVisualComponent>(Allocator.Temp);

        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var visual = visuals[i];
            if (!EntityManager.HasComponent<RenderMesh>(entity)) continue;
            var renderer = EntityManager.GetComponentObject<SpriteRenderer>(visual.MeshEntity);
            if (renderer == null) continue;
            renderer.GetPropertyBlock(block);
            block.SetFloat("_SnowBlend", visual.SnowBlend);
            block.SetFloat("_HeightOffset", visual.HeightOffset);
            renderer.SetPropertyBlock(block);
        }

        entities.Dispose();
        visuals.Dispose();
    }
}
