using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class WaitingForAction_AIWorkerState : AIWorkerStateBase
{
    public WaitingForAction_AIWorkerState(AIWorkerStateContext context) : base(context)
    {
    }

    public async override void OnEnter(params object[] @params)
    {
        while (true)
        {
            await UniTask.WaitForSeconds(0.5f);

            var targetStorage = SelectStorage(Context.CharacterStack.ItemType);

            if (targetStorage == null)
                continue;

            Debug.Log($"Target:{targetStorage.transform.parent.name}");

            ChangeState(AIWorkerState.Navigating, targetStorage);
            return;
        }
    }

    [CanBeNull]
    private ItemStorage SelectStorage(ItemType? currentStackedItemsType)
    {
        if (currentStackedItemsType.HasValue)
            return GetStorage(Context.AIWorkerSettings.ItemTransferFlow[currentStackedItemsType.Value]);

        foreach (var task in Context.AIWorkerSettings.TaskPriority)
        {
            var storage = GetStorage(task);
            if (storage != null)
                return storage;
        }

        return null;

        ItemStorage GetStorage((MachineType machineType, bool isRawStorage) task)
        {
            int minItemCount = 0;
            if (!currentStackedItemsType.HasValue)
                minItemCount = (int)(Context.CharacterStack.Capacity *
                                     Context.AIWorkerSettings.MinItemCountRatioForTaskStart);

            return Context.MachineManager.GetBy(task, minItemCount, Context.Character, true);
        }
    }
}