using PoolSystem;
using UnityEngine;

public enum ItemType { RawCopper, BullionCopper }

public enum ItemState { Default, Transferring }

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType _type;
    public ItemType Type => _type;

    public ItemState State { get; private set; }

    public void ChangeState(ItemState state) => State = state;

    public class Pool : Pool<Item, ItemType>
    {
        public Pool(PoolSettings poolSettings) : base(poolSettings)
        {
        }
    }
}