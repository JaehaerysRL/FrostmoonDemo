using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class TerrainGridGenerator : MonoBehaviour
{
    [Header("生成设置")]
    public readonly int mapRadius = GlobalConst.MapRadius;
    public readonly int gridSize = GlobalConst.GridPixelSize;
    public float noiseScale = 0.1f;
    public RectTransform gridRoot;

    [Header("地形预制体")]
    public List<TerrainPrefabEntry> terrainPrefabs;

    private Dictionary<TerrainType, AssetReference> prefabMap = new();

    void Start()
    {
        foreach (var entry in terrainPrefabs)
        {
            prefabMap[entry.terrainType] = entry.prefabReference;
        }

        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        for (int x = -mapRadius; x <= mapRadius; x++)
        {
            for (int y = -mapRadius; y <= mapRadius; y++)
            {
                var type = GetTerrainType(x, y);
                if (prefabMap.TryGetValue(type, out var prefabRef))
                {
                    var pos = Utils.GridToWorld(x, y);
                    LoadAndPlaceCell(prefabRef, pos);
                }
            }
        }
    }

    void LoadAndPlaceCell(AssetReference prefabRef, Vector2 position)
    {
        prefabRef.InstantiateAsync(gridRoot).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var go = handle.Result;
                var rt = go.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = position;
                }
            }
        };
    }

    TerrainType GetTerrainType(int x, int y)
    {
        float nx = x * noiseScale;
        float ny = y * noiseScale;
        float noise = Mathf.PerlinNoise(nx, ny);

        if (noise < 0.3f) return TerrainType.Frozenlake;
        else if (noise < 0.5f) return TerrainType.Permafrost;
        else if (noise < 0.7f) return TerrainType.Tundra;
        else return TerrainType.Shrub;
    }
}

[System.Serializable]
public class TerrainPrefabEntry
{
    public TerrainType terrainType;
    public AssetReference prefabReference;
}