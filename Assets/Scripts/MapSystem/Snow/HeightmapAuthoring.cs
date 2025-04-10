using Unity.Entities;
using UnityEngine;

public class HeightmapAuthoring : MonoBehaviour {
    public float baseHeight = 0f;

    class Baker : Baker<HeightmapAuthoring> {
        public override void Bake(HeightmapAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Heightmap {
                BaseHeight = authoring.baseHeight,
                FinalHeight = authoring.baseHeight
            });
        }
    }
}
