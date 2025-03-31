using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public enum MachineType { Spawner, Transformer, Trash }

public abstract class Machine : MonoBehaviour
{
    protected static readonly int IS_WORKING_PARAM = Animator.StringToHash("IsWorking");

    public abstract MachineType MachineType { get; }

    [SerializeField] public ItemStorage ProductStorage;
    [SerializeField, PropertyOrder(-1)] public ItemStorage RawMaterialStorage;

    [SerializeField] protected ItemTransferSystem _transferSystem;

    [SerializeField] protected Animator _animator;

    protected ItemSpawner _itemSpawner;

    protected bool IsWorking;

    protected abstract UniTask Execute();

    protected virtual void Start()
    {
        _itemSpawner = ServiceLocator.Resolve<ItemSpawner>();
    }

    protected void SetWorking(bool s)
    {
        IsWorking = s;
        if (_animator)
            _animator.SetBool(IS_WORKING_PARAM, s);
    }

    protected abstract bool ExecutionCondition();

#if UNITY_EDITOR
    [Button]
    private void TriggerExecute() => Execute();
#endif
}