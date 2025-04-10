using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class SnowVisualUpdateSystem : SystemBase
{
    EntityQuery m_Query;

    protected override void OnCreate()
    {
        base.OnCreate();
        // 只查询包含 SnowLayer 和 Heightmap 的 Entity
        m_Query = GetEntityQuery(ComponentType.ReadOnly<SnowLayer>(), ComponentType.ReadOnly<Heightmap>());
    }

    protected override void OnUpdate()
    {
        var entityManager = EntityManager;
        var entities = m_Query.ToEntityArray(Allocator.Temp);
        var snowArray = m_Query.ToComponentDataArray<SnowLayer>(Allocator.Temp);
        var heightArray = m_Query.ToComponentDataArray<Heightmap>(Allocator.Temp);

        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            if (!entityManager.HasComponent<TileVisualBinding>(entity)) continue;

            var snow = snowArray[i];
            var heightmap = heightArray[i];

            var binding = entityManager.GetComponentObject<TileVisualBinding>(entity);
            if (binding == null || binding.image == null) continue;

            float snowBlend = math.saturate(snow.Thickness / 1.5f);
            float heightOffset = heightmap.FinalHeight;

            binding.ApplyVisual(snowBlend, heightOffset);
        }

        entities.Dispose();
        snowArray.Dispose();
        heightArray.Dispose();
    }
}
