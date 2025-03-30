using System;
using Cysharp.Threading.Tasks;

public class SpawnerMachine : Machine
{
    // private void OnEnable() => _producedItemStorage.OnEmpty += OnProductStorageEmpty;
    // private void OnProductStorageEmpty() => Execute().Forget();

    protected override void Start()
    {
        base.Start();
        ExecutionTimer().Forget();
    }

    private async UniTask ExecutionTimer()
    {
        while (enabled)
        {
            await UniTask.WaitForSeconds(1);

            Execute().Forget();
        }
    }

    protected override bool ExecutionCondition() =>
        base.ExecutionCondition() &&
        _storage.HasSpace &&
        _storage.IsMachineTransfersEnabled;

    protected override async UniTask Execute()
    {
        if (ExecutionCondition())
            return;

        SetWorking(true);

        do
        {
            var item = _itemSpawner.Spawn(_storage.ItemType!.Value, new ItemData(transform.position));

            await _storage.Push(item);
        } while (ExecutionCondition());

        SetWorking(false);
    }
}