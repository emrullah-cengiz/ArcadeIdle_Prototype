using Sirenix.OdinInspector;

/// <summary>
/// Specialized storage that automatically spawns a new item when one is taken. It's works like a infinite item source
/// </summary>
[InfoBox("It's works like a infinite item source")]
public class ItemSpawnerStorage : ItemStorage
{
    private Item.Pool _itemPool;

    protected override void Awake()
    {
        base.Awake();

        _itemPool = ServiceLocator.Resolve<Item.Pool>();

        SpawnOneItem();
    }

    public override Item Pop(bool removeItem = true)
    {
        var item = base.Pop(removeItem);

        SpawnOneItem();

        return item;
    }

    private void SpawnOneItem() =>
        Items.Add(_itemPool.Spawn(ItemType!.Value));
}