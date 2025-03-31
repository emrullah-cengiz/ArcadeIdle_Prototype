using Cysharp.Threading.Tasks;

public class SpawnerMachine : Machine
{
    // private void OnEnable() => _producedItemStorage.OnEmpty += OnProductStorageEmpty;
    // private void OnProductStorageEmpty() => Execute().Forget();

    public override MachineType MachineType => MachineType.Spawner;

    protected override void Start()
    {
        base.Start();
        ExecutionTimer().Forget();
    }

    private async UniTask ExecutionTimer()
    {
        while (enabled)
        {
            await UniTask.WaitForSeconds(_transferTimerCooldown);

            Execute().Forget();
        }
    }

    protected override bool ExecutionCondition() =>
        ProductStorage.HasSpace &&
        ProductStorage.IsMachineTransfersEnabled;

    protected override async UniTask Execute()
    {
        if (IsWorking || !ExecutionCondition())
            return;

        SetWorking(true);

        do
        {
            var item = _itemSpawner.Spawn(ProductStorage.ItemType!.Value, new ItemData(transform.position));

            await ProductStorage.Push(item);
        } while (ExecutionCondition());

        SetWorking(false);
    }
}