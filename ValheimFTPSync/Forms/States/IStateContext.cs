using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ValheimFTPSync.Forms.States
{
    internal interface IStateContext
    {
        void TransientToState(IState state);

        void BlockUI();
    }
}
