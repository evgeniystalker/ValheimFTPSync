using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Forms.States
{
    internal class StateContext : IStateContext
    {
        IState CurrentState { get; set; }
        MainForm MainForm { get; set; }

        public StateContext(IState currentState, MainForm mainForm)
        {
            CurrentState = currentState;
            MainForm = mainForm;
        }

        public void TransientToState(IState state)
        {
            state.ExitState(this);
            CurrentState = state;
            state.EnterState();
        }

        public void BlockUI()
        {
            MainForm.BlockUI();
        }
    }
}
