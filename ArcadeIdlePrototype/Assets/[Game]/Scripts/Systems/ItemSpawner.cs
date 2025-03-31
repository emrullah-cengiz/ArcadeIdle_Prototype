public class ItemSpawner
{
    private readonly Item.Pool _pool = ServiceLocator.Resolve<Item.Pool>();

    public Item Spawn(ItemType itemType, ItemData data) => _pool.Spawn(itemType, data);
    public void Despawn(Item item) => _pool.Despawn(item, item.Type);
}