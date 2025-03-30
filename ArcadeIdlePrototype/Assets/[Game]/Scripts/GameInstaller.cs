using System.Collections.Generic;
using PoolSystem;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    [SerializeField] private GameSettings _gameSettings;

    private void Awake()
    {
        Application.targetFrameRate = 120;

        RegisterServices();
    }

    private void Start() => InitializeGame();

    private void RegisterServices()
    {
        //Settings
        ServiceLocator.Register(_gameSettings.ItemSettings);

        //Pools
        var pools = RegisterPools();

        //Systems

        InitializePools(pools);
    }

    private List<IPool> RegisterPools()
    {
        var pools = new List<IPool>();

        RegisterAndAddToList(new Item.Pool(_gameSettings.ItemSettings.PoolSettings));
        // RegisterAndAddToList(new Item.Pool(_gameSettings.ItemPoolSettings));

        return pools;

        void RegisterAndAddToList<T>(T pool) where T : class, IPool
        {
            ServiceLocator.Register<T>(pool);
            pools.Add(pool);
        }
    }

    private void InitializePools(List<IPool> pools)
    {
        foreach (var pool in pools)
            pool.Initialize();
    }

    private void InitializeGame()
    {
    }
}