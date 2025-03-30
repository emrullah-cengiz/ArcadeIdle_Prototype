using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterMachineInteractionController : MonoBehaviour
{
    [SerializeField] private CharacterItemStack _characterItemStack;
    [SerializeField] private ItemTransferSystem _itemTransferSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.ITEM_STORAGE_TAG) ||
            !other.TryGetComponent<ItemStorage>(out var storage)) return;

        storage.OnCharacterInteract(true);
        _itemTransferSystem.TryToStartTransfer(storage, _characterItemStack).Forget();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.ITEM_STORAGE_TAG) ||
            !other.TryGetComponent<ItemStorage>(out var storage)) return;

        storage.OnCharacterInteract(false);
        _itemTransferSystem.StopTransfer();
    }
}