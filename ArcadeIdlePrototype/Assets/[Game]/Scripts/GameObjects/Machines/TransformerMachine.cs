using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class TransformerMachine : Machine
{
    [SerializeField, PropertyOrder(-1)] protected ItemStorage _rawMaterialStorage;
    [SerializeField] private Transform _processPoint;

    protected override void Start()
    {
        base.Start();
        ExecutionTimer().Forget();
    }

    private async UniTask ExecutionTimer()
    {
        while (enabled)
        {
            await UniTask.WaitForSeconds(.5f);

            Execute().Forget();
        }
    }

    protected override bool ExecutionCondition() =>
        base.ExecutionCondition() &&
        _rawMaterialStorage.HasItem &&
        _storage.HasSpace &&
        _storage.IsMachineTransfersEnabled;

    protected override async UniTask Execute()
    {
        if (ExecutionCondition())
            return;

        SetWorking(true);

        await UniTask.WaitForSeconds(0.3f);

        while (ExecutionCondition())
        {
            var rawItem = _rawMaterialStorage.Pop();

            //Move to process area
            await rawItem.transform.MoveCurved(_processPoint.position, angle: Vector3.zero, 10, .3f, .2f, .15f);
            await UniTask.WaitForSeconds(0.1f);

            //spawn new item
            var newItem = _itemSpawner.Spawn(_storage.ItemType!.Value, new ItemData(rawItem.transform.position));
            _itemSpawner.Despawn(rawItem);

            //push to producedItemStorage
            await _storage.Push(newItem);
        }

        SetWorking(false);
    }
}