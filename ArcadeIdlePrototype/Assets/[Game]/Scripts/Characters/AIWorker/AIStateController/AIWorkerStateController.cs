using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GAME.Utilities.StateMachine;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public enum AIWorkerState { WaitingForAction, Navigating, TransferringStack }

public class AIWorkerStateController : MonoBehaviour
{
    // [SerializeField] private AIWorkerStateContext _context;
    [SerializeField] private Character Character;
    [SerializeField] private CharacterItemStack CharacterStack;
    [SerializeField] private AgentController AgentController;

    private MachineManager MachineManager;
    private AIWorkerSettings AIWorkerSettings;

    private StateMachine<AIWorkerState> _stateMachine;

    public void Start()
    {
        MachineManager = ServiceLocator.Resolve<MachineManager>();
        AIWorkerSettings = ServiceLocator.Resolve<AIWorkerSettings>();

        var context = new AIWorkerStateContext()
        {
            Character = Character,
            CharacterStack = CharacterStack,
            AgentController = AgentController,
            MachineManager = MachineManager,
            AIWorkerSettings = AIWorkerSettings,
        };

        InitializeStateMachine(context);
    }

    private void InitializeStateMachine(AIWorkerStateContext context)
    {
        _stateMachine = new StateMachine<AIWorkerState>();

        _stateMachine.OnStateChanged += OnStateChanged;

        _stateMachine.AddState(AIWorkerState.WaitingForAction, new WaitingForAction_AIWorkerState(context));
        _stateMachine.AddState(AIWorkerState.Navigating, new Navigating_AIWorkerState(context));
        _stateMachine.AddState(AIWorkerState.TransferringStack, new TransferringStack_AIWorkerState(context));

        _stateMachine.SetStartState(AIWorkerState.WaitingForAction);
        _stateMachine.Init();
    }

    private void OnStateChanged(AIWorkerState state)
    {
        Debug.Log($"Agent state changing.. {_stateMachine.CurrentState} > {state}");
        // Event.OnGameStateChanged?.Invoke(state);
    }
}

[Serializable]
public class AIWorkerStateContext
{
    public MachineManager MachineManager;
    public Character Character;
    public CharacterItemStack CharacterStack;
    public AgentController AgentController;
    public AIWorkerSettings AIWorkerSettings;
}

public abstract class AIWorkerStateBase : StateBase<AIWorkerState>
{
    protected readonly AIWorkerStateContext Context;

    protected AIWorkerStateBase(AIWorkerStateContext context)
    {
        Context = context;
    }
}

public class WaitingForAction_AIWorkerState : AIWorkerStateBase
{
    public WaitingForAction_AIWorkerState(AIWorkerStateContext context) : base(context)
    {
    }

    public async override void OnEnter(params object[] @params)
    {
        while (true)
        {
            await UniTask.WaitForSeconds(0.5f);

            var targetStorage = SelectStorage(Context.CharacterStack.ItemType);

            if (targetStorage == null)
                continue;

            Debug.Log($"Target:{targetStorage.transform.parent.name}");

            ChangeState(AIWorkerState.Navigating, targetStorage);
            return;
        }
    }

    [CanBeNull]
    private ItemStorage SelectStorage(ItemType? currentStackedItemsType)
    {
        if (currentStackedItemsType.HasValue)
            return GetStorage(Context.AIWorkerSettings.ItemTransferFlow[currentStackedItemsType.Value]);

        foreach (var task in Context.AIWorkerSettings.TaskPriority)
        {
            var storage = GetStorage(task);
            if (storage != null)
                return storage;
        }

        return null;

        ItemStorage GetStorage((MachineType machineType, bool isRawStorage) task)
        {
            int minItemCount = 0;
            if (!currentStackedItemsType.HasValue)
                minItemCount = (int)(Context.CharacterStack.Capacity *
                                     Context.AIWorkerSettings.MinItemCountRatioForTaskStart);

            return Context.MachineManager.GetBy(task, minItemCount, Context.Character, true);
        }
    }
}

public class Navigating_AIWorkerState : AIWorkerStateBase
{
    private ItemStorage targetStorage;

    public Navigating_AIWorkerState(AIWorkerStateContext context) : base(context)
    {
    }

    public async override void OnEnter(params object[] @params)
    {
        targetStorage = (ItemStorage)@params[0];

        targetStorage.OnInteractWithAgent += SelectAnotherStorage;

        await Context.AgentController.GoToDestination(targetStorage.AiWaitingPoint.position);

        ChangeState(AIWorkerState.TransferringStack, targetStorage);
    }

    private void SelectAnotherStorage(Character character)
    {
        targetStorage.OnInteractWithAgent -= SelectAnotherStorage;

        if (character == Context.Character)
            return;

        ChangeState(AIWorkerState.WaitingForAction);
    }
}

public class TransferringStack_AIWorkerState : AIWorkerStateBase
{
    private ItemStorage targetStorage;

    public TransferringStack_AIWorkerState(AIWorkerStateContext context) : base(context)
    {
    }

    public override void OnEnter(params object[] @params)
    {
        targetStorage = (ItemStorage)@params[0];

        Context.CharacterStack.OnTransferEnd += OnTransferEnd;
        targetStorage.OnTransferEnd += OnTransferEnd;
    }

    private void OnTransferEnd()
    {
        Context.CharacterStack.OnTransferEnd -= OnTransferEnd;
        targetStorage.OnTransferEnd -= OnTransferEnd;

        ChangeState(AIWorkerState.WaitingForAction);
    }
}