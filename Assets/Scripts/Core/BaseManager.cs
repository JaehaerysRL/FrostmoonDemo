using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.Entities;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
public interface IMgr
{
    int InitPriority { get; } // 初始化优先级
    void OnInit();
    void OnDestroy();
}

public abstract class BaseManager<T> : IMgr where T : BaseManager<T>, new()
{
    #region Singleton
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    #endregion

    #region Lifecycle
    public virtual int InitPriority => 0; // 默认初始化优先级为0
    protected bool _isInitialized = false;

    public virtual void OnInit()
    {
        if (_isInitialized) return;

        Initialize();
        _isInitialized = true;
    }

    public virtual void OnDestroy()
    {
        if (!_isInitialized) return;

        Cleanup();
        _isInitialized = false;
    }

    protected virtual void Initialize() { }
    protected virtual void Cleanup() { }

    #endregion

    #region Addressables
    private readonly HashSet<AsyncOperationHandle> _asyncOperationHandles = new HashSet<AsyncOperationHandle>();
    protected async UniTask LoadAddressableAsync(string address, System.Action<AsyncOperationHandle> onComplete = null)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(address);
        _asyncOperationHandles.Add(handle);
        await handle.Task;
        onComplete?.Invoke(handle);
    }
    protected void UnloadAddressable(AsyncOperationHandle handle)
    {
        if (_asyncOperationHandles.Contains(handle))
        {
            Addressables.Release(handle);
            _asyncOperationHandles.Remove(handle);
        }
    }
    protected void UnloadAllAddressables()
    {
        foreach (var handle in _asyncOperationHandles)
        {
            Addressables.Release(handle);
        }
        _asyncOperationHandles.Clear();
    }

    #endregion

    #region ECS
    protected World CurrentWorld => World.DefaultGameObjectInjectionWorld;
    protected EntityManager EntityManager => CurrentWorld.EntityManager;

    #endregion
}