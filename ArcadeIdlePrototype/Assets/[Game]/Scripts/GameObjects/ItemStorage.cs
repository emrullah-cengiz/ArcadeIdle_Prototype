using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public enum StorageType { None, Collectable, Depositable }

/// <summary>
/// Base class for item storage, managing item stacking and retrieval.
/// </summary>
public class ItemStorage : MonoBehaviour
{
    [SerializeField] private StorageType _storageType;
    [SerializeField] private ItemType _itemType;
    [SerializeField] private int _capacity;

    [SerializeField, MinValue(1)] private Vector2Int _layoutSize;
    [SerializeField] private Vector3 _cellSize;
    [SerializeField] private Transform _itemsContainer;
    [SerializeField] private bool _tweenOnTransfer;

    public StorageType Type => _storageType;
    public virtual ItemType? ItemType => _itemType;
    public bool HasItem => Items.Count > 0;
    public bool IsFull => Items.Count >= _capacity;
    public event Action OnEmpty;

    private CancellationTokenSource _cancellationTokenSource;
    protected List<Item> Items;

    protected virtual void Start() => Items = new List<Item>();

    public async UniTask Push(Item item, CancellationTokenSource cancellationTokenSource)
    {
        if (IsFull)
            return;

        _cancellationTokenSource = cancellationTokenSource;

        Items.Add(item);

        item.transform.SetParent(_itemsContainer, worldPositionStays: true);

        if (_tweenOnTransfer)
            await item.transform.MoveCurved(GetItemPosition(Items.Count - 1), 10, .3f, .2f, .15f,
                                            cancellationTokenSource.Token);
        else
            await UniTask.WaitForSeconds(.2f);
    }

    public virtual Item Pop()
    {
        if (Items.Count == 0)
            return null;

        var item = Items[^1];
        Items.RemoveAt(Items.Count - 1);

        _cancellationTokenSource?.Cancel();

        if (!HasItem)
            OnEmpty?.Invoke();

        return item;
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
        _cancellationTokenSource?.Cancel();

        foreach (var item in Items)
            Destroy(item.gameObject);

        Items.Clear();
    }
#endif
}