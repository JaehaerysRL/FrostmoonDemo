using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    const string LOG_TAG = "GameManager";
    public static GameManager Instance { get; private set; }
    private List<IMgr> _managers = new();

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

        this.Init();
    }

    public void Init()
    {
        SmartLog.Info(LOG_TAG, "GameManager initialized.");

        InitECSWorld();
        InitializeManagers();
    }

    #region Initialization
    void InitECSWorld()
    {
        // 创建 ECS World 和相关系统
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

        SmartLog.Info(LOG_TAG, "ECS World initialized.");
    }

    private void InitializeManagers()
    {
        // 按注册管理器

        // 按优先级排序初始化
        _managers.Sort((x, y) => x.InitPriority.CompareTo(y.InitPriority));
        foreach (var manager in _managers)
        {
            manager.OnInit();
        }

        SmartLog.Info(LOG_TAG, "Managers initialized.");
    }

    private void RegisterManager(IMgr manager)
    {
        if (manager == null || _managers.Contains(manager))
            return;

        _managers.Add(manager);
    }

    #endregion
}