using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Machine : MonoBehaviour
{
    protected static readonly int IS_WORKING_PARAM = Animator.StringToHash("IsWorking");

    [SerializeField] protected ItemStorage _storage;

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

    protected virtual bool ExecutionCondition() => !IsWorking;

#if UNITY_EDITOR
    [Button]
    private void TriggerExecute() => Execute();
#endif
}