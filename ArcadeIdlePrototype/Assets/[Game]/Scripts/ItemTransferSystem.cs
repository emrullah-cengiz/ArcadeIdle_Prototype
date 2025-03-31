using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ItemTransferSystem : MonoBehaviour
{
    private CancellationTokenSource _bulkTransferCTS;

    public async UniTask TryToStartTransfer(ItemStorage storage1, ItemStorage storage2, bool singleItem = false)
    {
        var from = storage1.Type == StorageType.Collectable ? storage1 : storage2;
        var to = storage1.Type == StorageType.Storable ? storage1 : storage2;

        await TryToStartDirectTransfer(from, to, singleItem);
    }

    public async UniTask TryToStartDirectTransfer(ItemStorage from, ItemStorage to, bool singleItem = false)
    {
        if (!IsTransferValid(from, to))
            return;

        await TransferItems(from, to, singleItem);
    }

    private async UniTask TransferItems(ItemStorage from, ItemStorage to, bool singleItem = false)
    {
        _bulkTransferCTS = new CancellationTokenSource();

        while (IsTransferValid(from, to))
        {
            await to.Push(from.Pop());
            if (singleItem)
                break;

            if (_bulkTransferCTS.IsCancellationRequested)
                break;
        }

        from.OnTransferEnd?.Invoke();
        to.OnTransferEnd?.Invoke();
    }

    public void StopTransfer() => _bulkTransferCTS?.Cancel();

    public static bool IsTransferValid(ItemStorage from, ItemStorage to) =>
        from.HasItem && to.HasSpace &&

        //check for character stack (stack only same type)
        (!to.ItemType.HasValue || to.Type != StorageType.None || from.ItemType == to.ItemType) &&

        //check for storable type
        (to.Type != StorageType.Storable || from.ItemType == to.ItemType) &&

        //check for parallel transfers on character interaction
        (from.Type == StorageType.None || to.Type == StorageType.None ||
         (from.IsMachineTransfersEnabled && to.IsMachineTransfersEnabled));
}