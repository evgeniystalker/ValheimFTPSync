using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface ISyncService
    {
        Task SyncAsync(IFtpClientBase ftpClient, string localPath, IProgress<OperationProgress>? progress, CancellationToken cancellationToken);
    }
}
