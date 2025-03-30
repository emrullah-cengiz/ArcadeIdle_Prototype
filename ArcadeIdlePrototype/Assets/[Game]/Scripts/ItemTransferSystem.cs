using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ItemTransferSystem : MonoBehaviour
{
    private CancellationTokenSource _cancellationTokenSource;

    public void TryToStartTransfer(ItemStorage storage1, ItemStorage storage2)
    {
        var from = storage1.Type == StorageType.Collectable ? storage1 : storage2;
        var to = storage1.Type == StorageType.Depositable ? storage1 : storage2;

        TryToStartDirectTransfer(from, to);
    }

    public void TryToStartDirectTransfer(ItemStorage from, ItemStorage to)
    {
        if (!IsValidForTransfer(from, to))
            return;

        StopTransfer();
        _cancellationTokenSource = new CancellationTokenSource();

        TransferItems(from, to, _cancellationTokenSource).Forget();
    }

    public void StopTransfer()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private static async UniTask TransferItems(ItemStorage from, ItemStorage to, CancellationTokenSource cts)
    {
        while (IsValidForTransfer(from, to))
            await to.Push(from.Pop(), cts);
    }

    private static bool IsValidForTransfer(ItemStorage from, ItemStorage to) =>
        from.HasItem && !to.IsFull;
}