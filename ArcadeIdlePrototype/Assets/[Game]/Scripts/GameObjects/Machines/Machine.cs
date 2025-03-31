using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public enum MachineType { Spawner, Transformer, Trash }

public abstract class Machine : MonoBehaviour
{
    protected static readonly int IS_WORKING_PARAM = Animator.StringToHash("IsWorking");

    public abstract MachineType MachineType { get; }

    [SerializeField] public ItemStorage RawMaterialStorage;
    [SerializeField] public ItemStorage ProductStorage;

    [SerializeField] protected Animator _animator;

    [Title("Feeling")] [SerializeField] protected float _transferTimerCooldown = .3f;
    [SerializeField] protected float _delayBeforeStart = .1f;
    [SerializeField] protected float _cooldownBetweenItems = .2f;

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