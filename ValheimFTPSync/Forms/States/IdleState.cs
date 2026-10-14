namespace ValheimFTPSync.Forms.States
{
    internal class IdleState : IState
    {
        public void EnterState(IStateContext context)
        {
            context.IdleUI();
        }

        public void ExitState(IStateContext context)
        {

        }
    }
}