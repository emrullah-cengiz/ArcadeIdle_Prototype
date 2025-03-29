using UnityEngine;

public class CharacterMachineInteractionController : MonoBehaviour
{
    [SerializeField] private CharacterItemStack _characterItemStack;
    [SerializeField] private ItemTransferSystem _itemTransferSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.ITEM_STORAGE_TAG) &&
            other.TryGetComponent<ItemStorage>(out var storage))
            _itemTransferSystem.TryToStartTransfer(storage, _characterItemStack);
    }
}