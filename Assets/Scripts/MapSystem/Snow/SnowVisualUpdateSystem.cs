using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class SnowVisualUpdateSystem : SystemBase
{
    EntityQuery m_Query;

    protected override void OnCreate()
    {
        base.OnCreate();
        // 只查询包含 SnowLayer, Heightmap, TileVisual 的 Entity
        m_Query = GetEntityQuery(
            ComponentType.ReadOnly<SnowLayer>(),
            ComponentType.ReadOnly<Heightmap>(),
            ComponentType.ReadOnly<TileVisualComponent>()
        );
    }

    protected override void OnUpdate()
    {
        var entityManager = EntityManager;
        var entities = m_Query.ToEntityArray(Allocator.Temp);
        var snowArray = m_Query.ToComponentDataArray<SnowLayer>(Allocator.Temp);
        var heightArray = m_Query.ToComponentDataArray<Heightmap>(Allocator.Temp);
        var visualArray = m_Query.ToComponentDataArray<TileVisualComponent>(Allocator.Temp);

        for (int i = 0; i < entities.Length; i++)
        {
            var snow = snowArray[i];
            var heightmap = heightArray[i];
            var visual = visualArray[i];

            float snowBlend = math.saturate(snow.Thickness / 1.5f);
            float heightOffset = heightmap.FinalHeight;

            visual.SnowBlend = snowBlend;
            visual.HeightOffset = heightOffset;
        }

        entities.Dispose();
        snowArray.Dispose();
        heightArray.Dispose();
        visualArray.Dispose();
    }
}
