using PoolSystem;
using UnityEngine;

public class ItemData : IPoolableInitializationData
{
    public Vector3 Position;

    public ItemData(Vector3 position)
    {
        Position = position;
    }
}