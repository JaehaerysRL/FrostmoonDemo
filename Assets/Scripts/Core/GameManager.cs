using Unity.Entities;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const string LOG_TAG = "GameManager";
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SmartLog.Info(LOG_TAG, "GameManager started.");
        // 初始化 ECS World & Systems
        var world = World.DefaultGameObjectInjectionWorld ?? new World("Default World");
        World.DefaultGameObjectInjectionWorld = world;

        // 获取或创建 SimulationSystemGroup
        var systemGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();

        // 注册子系统
        systemGroup.AddSystemToUpdateList(world.CreateSystem<InputSystem>());
        systemGroup.AddSystemToUpdateList(world.CreateSystem<TurnSystem>());
        systemGroup.AddSystemToUpdateList(world.CreateSystem<CombatSystem>());
        systemGroup.AddSystemToUpdateList(world.CreateSystem<AISystem>());

        // 初始化系统组
        DefaultWorldInitialization.Initialize("Default World", false);
        SmartLog.Info(LOG_TAG, "ECS World and Systems initialized.");
    }

    public void Init()
    {
        SmartLog.Info(LOG_TAG, "GameManager initialized.");
    }
}