using System;
using PoolSystem;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = nameof(GameSettings), menuName = nameof(GameSettings), order = 0)]
public class GameSettings : SerializedScriptableObject
{
    [OdinSerialize, NonSerialized] public ItemSettings ItemSettings;
}

[Serializable]
public class ItemSettings
{
    public Pool<Item, ItemType>.PoolSettings PoolSettings;
}