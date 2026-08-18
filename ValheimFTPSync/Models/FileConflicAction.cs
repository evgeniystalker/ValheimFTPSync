using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    public enum FileConflicAction
    {
        Overwrite,
        Skip,
        OverwriteAll,
        SkipAll,
        Cancel
    }
}