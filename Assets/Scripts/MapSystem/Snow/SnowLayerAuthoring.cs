using Unity.Entities;
using UnityEngine;

public class SnowLayerAuthoring : MonoBehaviour
{
    public float maxThickness = 1.2f;

    class Baker : Baker<SnowLayerAuthoring>
    {
        public override void Bake(SnowLayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new SnowLayer
            {
                Thickness = 0f,
                MaxThickness = authoring.maxThickness,
                VarianceSeed = UnityEngine.Random.Range(0f, 1000f)
            });
        }
    }
}
