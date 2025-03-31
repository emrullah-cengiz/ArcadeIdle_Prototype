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