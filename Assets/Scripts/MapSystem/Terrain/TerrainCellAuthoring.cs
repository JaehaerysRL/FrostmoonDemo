using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TerrainCellAuthoring : MonoBehaviour
{
    public TerrainType terrainType;
    public float height;
}

public class TerrainCellBaker : Baker<TerrainCellAuthoring>
{
    public override void Bake(TerrainCellAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new TerrainCell
        {
            GridPos = int2.zero,
            Height = authoring.height,
            Type = authoring.terrainType
        });
        AddComponent<TerrainTag>(entity);
    }
}
