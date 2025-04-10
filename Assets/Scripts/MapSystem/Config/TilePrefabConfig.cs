using Unity.Entities;

public struct TilePrefabConfig : IComponentData
{
    public Entity TundraPrefab;
    public Entity PermafrostPrefab;
    public Entity FrozenLakePrefab;
    public Entity ShrubPrefab;

    public Entity GetPrefab(TerrainType type) => type switch
    {
        TerrainType.Tundra => TundraPrefab,
        TerrainType.Permafrost => PermafrostPrefab,
        TerrainType.Frozenlake => FrozenLakePrefab,
        TerrainType.Shrub => ShrubPrefab,
        _ => Entity.Null
    };
}
