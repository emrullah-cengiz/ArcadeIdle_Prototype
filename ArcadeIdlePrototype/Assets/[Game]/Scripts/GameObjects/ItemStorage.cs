using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public enum StorageType { None, Collectable, Storable }

/// <summary>
/// Base class for item storage, managing item stacking and retrieval.
/// </summary>
public class ItemStorage : MonoBehaviour
{
    #region Serialized Fields

    [Title("References")] [SerializeField] private Transform _itemsContainer;
    [SerializeField] public Transform AiWaitingPoint;

    [Title("Base")] [SerializeField] protected StorageType _storageType;
    [SerializeField] private ItemType _itemType;
    [SerializeField] private int _capacity;

    [SerializeField, InfoBox("Enables machine-driven transfers on char interactions")]
    private bool _allowMachineTransfersOnAgentInteraction = true;

    [Title("Placement")] [SerializeField, MinValue(1)]
    private Vector2Int _layoutSize;

    [SerializeField] private Vector3 _cellSize;
    [SerializeField] private bool _tweenItemOnAdded;
    [SerializeField] private float _cooldownBetweenItems = .2f;

    #endregion

    #region Properties

    public bool IsMachineTransfersEnabled { get; private set; } = true;
    public bool IsInteractingWithAgent { get; private set; } = false;
    public StorageType Type => _storageType;
    public virtual ItemType? ItemType => _itemType;
    public int Capacity => _capacity;
    public int ItemCount => Items.Count;
    public int TotalSpace => _capacity - Items.Count;
    public bool HasItem => Items.Count > 0;
    public bool HasSpace => !IsFull;
    public bool IsFull => Items.Count >= _capacity;

    #endregion

    #region Events

    public Action OnItemPushed;
    public Action<Character> OnInteractWithAgent;
    public Action OnFullFilled;
    public Action OnEmpty;
    public Action OnTransferEnd;

    #endregion

    protected List<Item> Items = new();
    private ItemSettings _itemSettings;
    protected virtual void Awake() => Items = new List<Item>();

    private void Start()
    {
        _itemSettings = ServiceLocator.Resolve<ItemSettings>();
    }

    public async UniTask Push(Item item)
    {
        Items.Add(item);

        item.transform.SetParent(_itemsContainer, worldPositionStays: true);

        if (!HasSpace)
            OnFullFilled?.Invoke();

        if (_tweenItemOnAdded)
            item.MoveCurved(_itemsContainer.transform.position + GetItemPosition(Items.Count - 1),
                            Vector3.zero, _itemSettings.TweenOptions).Forget();

        await UniTask.WaitForSeconds(_cooldownBetweenItems);

        OnItemPushed?.Invoke();
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

        return item;
    }

    // public Item Peek() => Pop(removeItem: false);

    public void OnAgentInteract(Character character, bool s)
    {
        if (!_allowMachineTransfersOnAgentInteraction)
            IsMachineTransfersEnabled = !s;

        IsInteractingWithAgent = s;
        OnInteractWithAgent?.Invoke(character);
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

    private void OnValidate()
    {
        int i = 0;
        foreach (var item in Items)
            item.transform.position = _itemsContainer.transform.position + GetItemPosition(i++);
    }
#endif
}