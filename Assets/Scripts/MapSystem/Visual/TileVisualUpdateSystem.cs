using Unity.Entities;
using UnityEngine;
using Unity.Collections;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(SnowVisualUpdateSystem))] 
public partial class TileVisualUpdateSystem : SystemBase
{
    private MaterialPropertyBlock block;

    protected override void OnCreate()
    {
        block = new MaterialPropertyBlock();
    }

    protected override void OnUpdate()
    {
        foreach (var visual in 
            SystemAPI.Query<RefRO<TileVisualComponent>>())
        {
            var renderer = EntityManager.GetComponentObject<SpriteRenderer>(visual.ValueRO.MeshEntity);
            renderer.GetPropertyBlock(block);
            block.SetFloat("_SnowBlend", visual.ValueRO.SnowBlend);
            block.SetFloat("_HeightOffset", visual.ValueRO.HeightOffset);
            renderer.SetPropertyBlock(block);
        }
    }
}
