using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Forms.States
{
    internal class CancelState : IState
    {
        public void EnterState(IStateContext context)
        {
            context.CancelUI();
        }

        public void ExitState(IStateContext context)
        {
        }
    }
}
