namespace ValheimFTPSync.Forms.States
{
    internal class LoadingState : IState
    {
        public void EnterState(IStateContext context)
        {
            context.BlockUI();
        }

        public void ExitState(IStateContext context)
        {
        }
    }
}