using Unity.Entities;
using UnityEngine;

public class TerrainCellAuthoring : MonoBehaviour
{
    public TerrainType terrainType;
    public float height;
}

public struct TerrainTag : IComponentData { }

public class TerrainCellBaker : Baker<TerrainCellAuthoring>
{
    public override void Bake(TerrainCellAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new TerrainCell
        {
            GridPos = Utils.WorldToGrid(authoring.transform.localPosition),
            Height = authoring.height,
            Type = authoring.terrainType
        });
        AddComponent<TerrainTag>(entity);
    }
}
