using Unity.Entities;
using UnityEngine;

public class SnowLayerAuthoring : MonoBehaviour
{
    public float initialThickness = 0f;
    public float accumulationRate = 0.01f;
    public float meltingRate = 0.002f;

    class Baker : Baker<SnowLayerAuthoring>
    {
        public override void Bake(SnowLayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SnowLayer
            {
                Thickness = authoring.initialThickness,
                AccumulationRate = authoring.accumulationRate,
                MeltingRate = authoring.meltingRate
            });
        }
    }
}