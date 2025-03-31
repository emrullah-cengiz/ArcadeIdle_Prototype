using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private CharacterMachineInteractionController _machineInteractionController;
    public ItemStorage InteractingStorage => _machineInteractionController.InteractingStorage;
}