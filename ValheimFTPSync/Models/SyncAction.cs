using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    public enum SyncAction
    {
        None,

        Upload,
        Download,

        Conflict,

        DeleteLocal,
        DeleteRemote
    }
}
