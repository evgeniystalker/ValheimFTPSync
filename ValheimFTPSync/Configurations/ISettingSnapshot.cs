using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Configuration
{
    internal interface ISettingSnapshot<out T>
    {
        T GetSnapshot();
    }
}
