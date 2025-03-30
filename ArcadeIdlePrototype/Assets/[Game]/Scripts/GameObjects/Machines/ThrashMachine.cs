using System;
using Cysharp.Threading.Tasks;

public class ThrashMachine : Machine
{
    private void OnEnable() => _storage.OnItemPushed += OnItemPushed;
    private void OnItemPushed() => Execute().Forget();

    protected override async UniTask Execute()
    {
        if (ExecutionCondition())
            return;

        SetWorking(true);

        _itemSpawner.Despawn(_storage.Pop());

        SetWorking(false);
    }
}