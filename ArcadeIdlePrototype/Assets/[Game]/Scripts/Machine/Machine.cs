using UnityEngine;

public class Machine : MonoBehaviour
{
    [SerializeField] private ItemStorage _rawMaterialStorage;
    [SerializeField] private ItemStorage _producedItemStorage;

    [SerializeField] private ItemTransferSystem _transferSystem;

    public void Produce()
    {
        _transferSystem.TryToStartTransfer(_rawMaterialStorage, _producedItemStorage);
    }
}