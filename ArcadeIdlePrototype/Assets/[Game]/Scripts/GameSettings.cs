using System;
using System.Collections.Generic;
using PoolSystem;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = nameof(GameSettings), menuName = nameof(GameSettings), order = 0)]
public class GameSettings : SerializedScriptableObject
{
    [OdinSerialize, NonSerialized] public ItemSettings ItemSettings;
    [OdinSerialize, NonSerialized] public AIWorkerSettings AIWorkerSettings;
}

[Serializable]
public class ItemSettings
{
    public Pool<Item, ItemType>.PoolSettings PoolSettings;
    [Space] public CurvedMoveOptions TweenOptions;
}

[Serializable]
public class AIWorkerSettings
{
    [Space, InfoBox("Defines the minimum ratio of items in the character stack required to start a task")]
    public float MinItemCountRatioForTaskStart = 0.3f;

    [Space] public Dictionary<ItemType, (MachineType machineType, bool isRawStorage)> ItemTransferFlow;
    [Space] public (MachineType machineType, bool isRawStorage)[] TaskPriority;
}