using System.Collections.Generic;
using PoolSystem;
using UnityEditor.PackageManager;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameInstaller : MonoBehaviour
{
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private MachineManager _machineManager;


    private List<IPool> _pools;

    private void Awake()
    {
        Application.targetFrameRate = 120;

        RegisterServices();
        InitializePools();
    }

    private void Start() => InitializeGame();

    private void RegisterServices()
    {
        //Settings
        ServiceLocator.Register(_gameSettings.ItemSettings);
        ServiceLocator.Register(_gameSettings.AIWorkerSettings);

        //Pools
        RegisterPools();

        //Systems - Managers
        ServiceLocator.Register(_machineManager);
        ServiceLocator.Register(new ItemSpawner());
    }

    private void RegisterPools()
    {
        _pools = new List<IPool>();

        RegisterAndAddToList(new Item.Pool(_gameSettings.ItemSettings.PoolSettings));
        //RegisterAndAddToList(new Item.Pool(_gameSettings.ItemSettings.PoolSettings));

        return;

        void RegisterAndAddToList<T>(T pool) where T : class, IPool
        {
            ServiceLocator.Register<T>(pool);
            _pools.Add(pool);
        }
    }

    private void InitializePools()
    {
        foreach (var pool in _pools)
            pool.Initialize();
    }

    private void InitializeGame()
    {
    }
}