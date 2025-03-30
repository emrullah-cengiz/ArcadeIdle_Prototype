using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Machine : MonoBehaviour
{
    [SerializeField] private ItemStorage _rawMaterialStorage;
    [SerializeField] private ItemStorage _producedItemStorage;

    [SerializeField] private ItemTransferSystem _transferSystem;

    private void Start()
    {
        _producedItemStorage.OnEmpty += OnProductStorageEmpty;

        Execute().Forget();
    }

    private async UniTask Execute()
    {
        await UniTask.WaitForSeconds(0.5f);

        _transferSystem.TryToStartDirectTransfer(_rawMaterialStorage, _producedItemStorage);
    }

    private void OnProductStorageEmpty() => Execute().Forget();
}