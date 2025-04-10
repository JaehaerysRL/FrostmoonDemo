using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using System.Collections.Generic;

public class TerrainClusterBootstrapAuthoring : MonoBehaviour
{
    [Header("配置")]
    public int biomeCount = 12;
    public int frozenLakeCount = 3;
    public float radius = 6f;

    void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;

        var entity = entityManager.CreateEntity(typeof(BiomeCenter), typeof(FrozenLakeCenter));
        entityManager.AddBuffer<BiomeCenter>(entity);
        entityManager.AddBuffer<FrozenLakeCenter>(entity);

        var biomeBuffer = entityManager.GetBuffer<BiomeCenter>(entity);
        var lakeBuffer = entityManager.GetBuffer<FrozenLakeCenter>(entity);

        var used = new List<float2>();

        // 随机生成 BiomeCenter
        while (biomeBuffer.Length < biomeCount)
        {
            float angle = UnityEngine.Random.Range(0f, 30f);
            float r = UnityEngine.Random.Range(radius * 0.3f, radius);
            float2 offset = new float2(math.cos(math.radians(angle)), math.sin(math.radians(angle))) * r;

            bool tooClose = false;
            foreach (var u in used)
            {
                if (math.distance(u, offset) < 5f)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                biomeBuffer.Add(new BiomeCenter { Position = (int2)math.round(offset) });
                used.Add(offset);
            }
        }

        // 冰湖更集中分布
        for (int i = 0; i < frozenLakeCount; i++)
        {
            float2 offset = UnityEngine.Random.insideUnitCircle * (radius * 0.4f);
            lakeBuffer.Add(new FrozenLakeCenter { Position = (int2)math.round(offset) });
        }

        SmartLog.Log("TerrainClusterBootstrapAuthoring", $"生成 Biome: {biomeBuffer.Length}，FrozenLake: {lakeBuffer.Length}");
    }
}
