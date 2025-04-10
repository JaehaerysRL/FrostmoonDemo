using UnityEngine;
using Unity.Entities;

public class TilePrefabConfigAuthoring : MonoBehaviour
{
    public GameObject tundra;
    public GameObject permafrost;
    public GameObject frozenLake;
    public GameObject shrub;

    class Baker : Baker<TilePrefabConfigAuthoring>
    {
        public override void Bake(TilePrefabConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new TilePrefabConfig
            {
                TundraPrefab = GetEntity(authoring.tundra, TransformUsageFlags.Renderable),
                PermafrostPrefab = GetEntity(authoring.permafrost, TransformUsageFlags.Renderable),
                FrozenLakePrefab = GetEntity(authoring.frozenLake, TransformUsageFlags.Renderable),
                ShrubPrefab = GetEntity(authoring.shrub, TransformUsageFlags.Renderable)
            });
        }
    }
}
