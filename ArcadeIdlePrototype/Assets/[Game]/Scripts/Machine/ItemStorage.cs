using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum StorageType { Collectable, Depositable }

public class ItemStorage : MonoBehaviour
{
    [SerializeField] private StorageType _storageType;
    [SerializeField] private ItemType _itemType;
    [SerializeField] private int _capacity;

    protected Stack<Item> Items;

    public StorageType Type => _storageType;
    public virtual ItemType? ItemType => _itemType;

    public bool HasItem => Items.Count > 0;
    public bool IsFull => Items.Count >= _capacity;

    private void Start() => Items = new Stack<Item>();

    public async UniTask Add(Item item, CancellationToken cancellationToken)
    {
        if (IsFull)
            return;

        Items.Push(item);

        ///TODO:Tween
        await UniTask.Yield(cancellationToken);
    }

    public Item Get() => Items.Count > 0 ? Items.Pop() : null;
}