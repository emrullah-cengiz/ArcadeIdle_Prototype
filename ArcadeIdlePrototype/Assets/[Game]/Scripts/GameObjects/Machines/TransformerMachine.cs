using Cysharp.Threading.Tasks;
using UnityEngine;

public class TransformerMachine : Machine
{
    public override MachineType MachineType => MachineType.Transformer;

    [SerializeField] private Transform _processPoint;
    [SerializeField] private ParticleSystem _processVFX;

    private ItemSettings _itemSettings;

    protected override void Start()
    {
        base.Start();

        _itemSettings = ServiceLocator.Resolve<ItemSettings>();

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
        RawMaterialStorage.HasItem &&
        ProductStorage.HasSpace &&
        ProductStorage.IsMachineTransfersEnabled;

    protected override async UniTask Execute()
    {
        if (IsWorking || !ExecutionCondition())
            return;

        SetWorking(true);

        await UniTask.WaitForSeconds(_delayBeforeStart);

        while (ExecutionCondition())
        {
            var rawItem = RawMaterialStorage.Pop();

            //Move to process area
            await rawItem.MoveCurved(_processPoint.position, angle: Vector3.zero, _itemSettings.TweenOptions);
            await UniTask.WaitForSeconds(_cooldownBetweenItems);

            PlayParticle();

            //spawn new item
            var newItem = _itemSpawner.Spawn(ProductStorage.ItemType!.Value, new ItemData(rawItem.transform.position));
            _itemSpawner.Despawn(rawItem);

            //push to producedItemStorage
            await ProductStorage.Push(newItem);
        }

        SetWorking(false);
    }

    private void PlayParticle()
    {
        _processVFX.Play();
    }
}