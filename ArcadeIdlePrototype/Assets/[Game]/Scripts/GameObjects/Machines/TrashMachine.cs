using Cysharp.Threading.Tasks;

public class TrashMachine : Machine
{
    public override MachineType MachineType => MachineType.Trash;

    private void OnEnable() => RawMaterialStorage.OnItemPushed += OnItemPushed;
    private void OnItemPushed() => Execute().Forget();

    protected override bool ExecutionCondition() => true;

    protected override async UniTask Execute()
    {
        if (IsWorking || !ExecutionCondition())
            return;

        SetWorking(true);

        _itemSpawner.Despawn(RawMaterialStorage.Pop());

        SetWorking(false);
    }
}