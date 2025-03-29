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

        if (!IsValidForTransfer(from, to))
            return;

        StopTransfer();
        _cancellationTokenSource = new CancellationTokenSource();

        TransferItems(from, to, _cancellationTokenSource.Token).Forget();
    }

    public void StopTransfer()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private static async UniTask TransferItems(ItemStorage from, ItemStorage to, CancellationToken token)
    {
        while (IsValidForTransfer(from, to))
            await to.Add(from.Get(), cancellationToken: token);
    }

    private static bool IsValidForTransfer(ItemStorage from, ItemStorage to) =>
        from.ItemType != to.ItemType && from.HasItem && !to.IsFull;
}