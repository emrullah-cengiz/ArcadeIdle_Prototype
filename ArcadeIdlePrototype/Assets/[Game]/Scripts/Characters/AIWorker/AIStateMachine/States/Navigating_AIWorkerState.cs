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