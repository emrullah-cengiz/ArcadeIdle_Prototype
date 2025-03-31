using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAME.Utilities.StateMachine
{
    public interface IState<TStateEnum> where TStateEnum : Enum
    {
        public StateMachine<TStateEnum> StateMachine { get; }

        public void Initialize(StateMachine<TStateEnum> stateMachine);
        void OnEnter(object[] @params = null);
        void OnExit();
        void OnUpdate();

        void ChangeState(TStateEnum state, params object[] @params);
    }

    public abstract class StateBase<TStateEnum> : IState<TStateEnum> where TStateEnum : Enum
    {
        public StateMachine<TStateEnum> StateMachine { get; private set; }

        public void Initialize(StateMachine<TStateEnum> stateMachine) =>
            StateMachine = stateMachine;

        public virtual void OnEnter(params object[] @params)
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public void ChangeState(TStateEnum state, params object[] @params) =>
            StateMachine.ChangeState(state, @params);
    }

    public class StateMachine<TStateEnum> where TStateEnum : Enum
    {
        public TStateEnum CurrentState { get; private set; }

        private TStateEnum _startState;
        private IState<TStateEnum> _currentState;
        private Dictionary<TStateEnum, IState<TStateEnum>> _states = new();

        public event Action<TStateEnum> OnStateChanged;
        public event Action OnCompleted;

        public void AddState(TStateEnum stateType, IState<TStateEnum> state)
        {
            state.Initialize(this);

            _states.TryAdd(stateType, state);
        }

        public void ChangeState(TStateEnum newStateType, params object[] @params)
        {
            _currentState?.OnExit();

            if (_states.TryGetValue(newStateType, out var state))
            {
                OnStateChanged?.Invoke(newStateType);

                CurrentState = newStateType;
                _currentState = state;

                _currentState.OnEnter(@params);
            }
            else
                Debug.LogWarning($"State {newStateType} not found!");
        }

        public void Init() => ChangeState(_startState);
        public void Update() => _currentState?.OnUpdate();
        public void SetStartState(TStateEnum state) => _startState = state;
        public void Complete() => OnCompleted?.Invoke();
    }
}