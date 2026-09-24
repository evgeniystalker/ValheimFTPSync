using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    internal class SyncItem
    {
        public string RelativePath { get; init; } = string.Empty;

        public FileInfo? LocalFile { get; init; }

        public FtpFileModel? RemoteFile { get; init; }

        public SyncAction Action { get; set; }
    }
}
