using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterMachineInteractionController : MonoBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private CharacterItemStack _characterItemStack;
    [SerializeField] private ItemTransferSystem _itemTransferSystem;
    public ItemStorage InteractingStorage { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.ITEM_STORAGE_TAG) ||
            !other.TryGetComponent<ItemStorage>(out var storage)) return;

        storage.OnAgentInteract(_character, true);
        InteractingStorage = storage;
        _itemTransferSystem.TryToStartTransfer(storage, _characterItemStack).Forget();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.ITEM_STORAGE_TAG) ||
            !other.TryGetComponent<ItemStorage>(out var storage)) return;

        storage.OnAgentInteract(_character, false);
        InteractingStorage = null;
        _itemTransferSystem.StopTransfer();
    }
}