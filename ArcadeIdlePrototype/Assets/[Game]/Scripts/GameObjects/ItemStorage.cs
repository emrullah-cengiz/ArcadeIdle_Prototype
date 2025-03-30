using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public enum StorageType { None, Collectable, Storable }

/// <summary>
/// Base class for item storage, managing item stacking and retrieval.
/// </summary>
public class ItemStorage : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] protected StorageType _storageType;
    [SerializeField] private ItemType _itemType;
    [SerializeField] private int _capacity;

    [SerializeField] [InfoBox("Enables machine-driven transfers on char interactions")]
    private bool _allowMachineTransfersOnCharacterInteraction = true;

    [SerializeField, MinValue(1)] private Vector2Int _layoutSize;
    [SerializeField] private Vector3 _cellSize;
    [SerializeField] private Transform _itemsContainer;

    [FormerlySerializedAs("_tweenItemOnTaken")] [SerializeField]
    private bool _tweenItemOnAdded;

    #endregion

    #region Properties

    public bool IsMachineTransfersEnabled { get; private set; } = true;
    public StorageType Type => _storageType;
    public virtual ItemType? ItemType => _itemType;
    public bool HasItem => Items.Count > 0;
    public bool HasSpace => !IsFull;
    public bool IsFull => Items.Count >= _capacity;

    #endregion

    #region Events

    public event Action OnItemPushed;
    public event Action OnEmpty;
    public event Action OnHasSpace;
    public event Action OnHasItem;

    #endregion

    protected List<Item> Items = new();

    protected virtual void Awake() => Items = new List<Item>();

    public async UniTask Push(Item item)
    {
        Items.Add(item);

        item.transform.SetParent(_itemsContainer, worldPositionStays: true);

        if (_tweenItemOnAdded)
            item.MoveCurved(transform.position + GetItemPosition(Items.Count - 1)).Forget();
        await UniTask.WaitForSeconds(.2f);

        OnItemPushed?.Invoke();

        if (Items.Count == 1)
            OnHasItem?.Invoke();
    }

    public virtual Item Pop(bool removeItem = true)
    {
        if (!HasItem)
            return null;

        var item = Items[^1];
        if (removeItem)
            Items.RemoveAt(Items.Count - 1);

        if (Items.Count == 0)
            OnEmpty?.Invoke();
        else if (Items.Count == _capacity - 1)
            OnHasSpace?.Invoke();

        return item;
    }

    // public Item Peek() => Pop(removeItem: false);

    public void OnCharacterInteract(bool s)
    {
        if (!_allowMachineTransfersOnCharacterInteraction)
            IsMachineTransfersEnabled = !s;
    }

    protected Vector3 GetItemPosition(int index)
    {
        int itemsPerLayer = _layoutSize.x * _layoutSize.y;
        int layer = index / itemsPerLayer;
        int indexInLayer = index % itemsPerLayer;

        int row = indexInLayer / _layoutSize.x;
        int col = indexInLayer % _layoutSize.x;

        var position = col * _cellSize.x * Vector3.right
                       + row * _cellSize.z * Vector3.back
                       + layer * _cellSize.y * Vector3.up;

        return position;
    }

#if UNITY_EDITOR
    [Button]
    void DestroyItems()
    {
        foreach (var item in Items)
            Destroy(item.gameObject);

        Items.Clear();
    }
#endif
}