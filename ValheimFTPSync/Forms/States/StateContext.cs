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
            CurrentState.EnterState(this);
        }

        public void TransientToState(IState state)
        {
            CurrentState.ExitState(this);
            CurrentState = state;
            CurrentState.EnterState(this);
        }

        public void BlockUI()
        {
            MainForm.EnableUI(false);
        }

        public void IdleUI()
        {
            MainForm.EnableUI(true);
        }
    }
}
