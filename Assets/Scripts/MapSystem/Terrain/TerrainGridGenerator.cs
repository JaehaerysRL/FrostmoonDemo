using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.Mathematics;
using System.Collections.Generic;

public class TerrainGridGenerator : MonoBehaviour
{
    [Header("基本信息")]
    public readonly int mapRadius = GlobalConst.MapRadius;
    public readonly int gridSize = GlobalConst.GridPixelSize;

    [Header("地形参数")]
    public int BiomeCount = 4;
    public int FrozenLakeCount = 1;
    public float FrozenLakeRadius = 5f;
    public float NoiseScale = 0.1f;
    public float MaxNoiseHeight = 0.2f;
    public float BaseHeightMultiplier = 1f;

    [Header("地形预制体")]
    public List<TerrainPrefabEntry> terrainPrefabs;

    private Dictionary<TerrainType, AssetReference> prefabMap = new();
    private List<int2> BiomeCenters = new();
    private List<int2> FrozenLakeCenters = new();

    void Start()
    {
        foreach (var entry in terrainPrefabs)
        {
            prefabMap[entry.terrainType] = entry.prefabReference;
        }

        InitClusters();
        GenerateTerrain();
    }

    void InitClusters()
    {
        BiomeCenters.Clear();
        FrozenLakeCenters.Clear();

        float safeMargin = mapRadius * 0.7f;
        int maxTries = 20;

        while (BiomeCenters.Count < BiomeCount && maxTries-- > 0)
        {
            float angle = UnityEngine.Random.Range(0f, 360f);
            float radius = UnityEngine.Random.Range(safeMargin * 0.3f, safeMargin);
            Vector2 offset = new Vector2(math.cos(math.radians(angle)), math.sin(math.radians(angle))) * radius;

            int2 candidate = new int2((int)math.round(offset.x), (int)math.round(offset.y));

            bool tooClose = false;
            foreach (var center in BiomeCenters)
            {
                if (math.distance(center, candidate) < 3f)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose) BiomeCenters.Add(candidate);
        }

        for (int i = 0; i < FrozenLakeCount; i++)
        {
            Vector2 offset = UnityEngine.Random.insideUnitCircle * mapRadius * 0.4f;
            int2 lakeCenter = new int2((int)math.round(offset.x), (int)math.round(offset.y));
            FrozenLakeCenters.Add(lakeCenter);
        }
    }

    void GenerateTerrain()
    {
        for (int x = -mapRadius; x <= mapRadius; x++)
        {
            for (int y = -mapRadius; y <= mapRadius; y++)
            {
                int2 gridPos = new int2(x, y);
                TerrainType type = CalculateBiomeType(gridPos);
                float lakeEffect = CalculateLakeInfluence(gridPos);
                float height = CalculateFinalHeight(type, gridPos, lakeEffect) * BaseHeightMultiplier;

                if (lakeEffect > 0.9f)
                    type = TerrainType.Frozenlake;

                if (prefabMap.TryGetValue(type, out var prefabRef))
                {
                    Vector2 worldPos = new Vector2(x * gridSize, y * gridSize);
                    LoadAndPlace(prefabRef, worldPos, height);
                }
            }
        }
    }

    void LoadAndPlace(AssetReference prefabRef, Vector2 worldPos, float height)
    {
        prefabRef.InstantiateAsync().Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var go = handle.Result;
                go.transform.SetParent(transform, false);
                go.transform.position = new Vector3(worldPos.x, worldPos.y, -height); // 用Z轴表示高度，视觉上有层次
            }
        };
    }

    TerrainType CalculateBiomeType(int2 pos)
    {
        float minDist = float.MaxValue;
        int index = 0;
        for (int i = 0; i < BiomeCenters.Count; i++)
        {
            float d = math.distance(pos, BiomeCenters[i]);
            if (d < minDist)
            {
                minDist = d;
                index = i;
            }
        }
        return (TerrainType)(index % System.Enum.GetValues(typeof(TerrainType)).Length);
    }

    float CalculateLakeInfluence(int2 pos)
    {
        float minDist = float.MaxValue;
        foreach (var center in FrozenLakeCenters)
        {
            float d = math.distance(pos, center);
            minDist = math.min(d, minDist);
        }

        return math.saturate(1 - (minDist / FrozenLakeRadius));
    }

    float CalculateFinalHeight(TerrainType type, int2 pos, float lakeEffect)
    {
        float baseHeight = type switch
        {
            TerrainType.Tundra => 0.5f,
            TerrainType.Permafrost => 0.3f,
            TerrainType.Frozenlake => 0f,
            TerrainType.Shrub => 0.2f,
            _ => 0.1f
        };

        float2 coord = new float2(pos.x * NoiseScale, pos.y * NoiseScale);
        float noiseVal = noise.snoise(coord) * MaxNoiseHeight;
        float smoothLake = lakeEffect * lakeEffect * (3f - 2f * lakeEffect);
        float final = (baseHeight + noiseVal) * (1 - smoothLake);
        if (type == TerrainType.Frozenlake) final = 0;

        return math.max(final, 0);
    }
}

[System.Serializable]
public class TerrainPrefabEntry
{
    public TerrainType terrainType;
    public AssetReference prefabReference;
}
