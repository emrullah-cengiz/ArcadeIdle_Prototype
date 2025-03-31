using System;
using GAME.Utilities.StateMachine;
using UnityEngine;

public enum AIWorkerState { WaitingForAction, Navigating, TransferringStack }

public class AIWorkerStateController : MonoBehaviour
{
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