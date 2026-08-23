using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Forms.States
{
    internal interface IState
    {
        void ExitState(IStateContext context);

        void EnterState();
    }
}
