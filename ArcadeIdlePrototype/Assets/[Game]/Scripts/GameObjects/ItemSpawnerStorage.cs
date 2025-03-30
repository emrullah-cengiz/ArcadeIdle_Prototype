/// <summary>
/// Specialized storage that automatically spawns a new item when one is taken.
/// </summary>
public class ItemSpawnerStorage : ItemStorage
{
    private Item.Pool _itemPool;

    protected override void Start()
    {
        base.Start();

        _itemPool = ServiceLocator.Resolve<Item.Pool>();

        SpawnOneItem();
    }

    public override Item Pop()
    {
        var item = base.Pop();

        SpawnOneItem();

        return item;
    }

    private void SpawnOneItem() =>
        Items.Add(_itemPool.Spawn(ItemType!.Value));
}