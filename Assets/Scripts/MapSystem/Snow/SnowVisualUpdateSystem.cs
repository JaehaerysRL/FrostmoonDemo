using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateBefore(typeof(TileVisualUpdateSystem))]
public partial class SnowVisualUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach (var (snow, heightmap, visual) in
                 SystemAPI.Query<RefRO<SnowLayer>, RefRO<Heightmap>, RefRW<TileVisualComponent>>())
        {
            float snowBlend = math.saturate(snow.ValueRO.Thickness / 1.5f);
            float heightOffset = math.clamp(heightmap.ValueRO.FinalHeight, 0f, 5f);

            visual.ValueRW.SnowBlend = snowBlend;
            visual.ValueRW.HeightOffset = heightOffset;
        }
    }
}
