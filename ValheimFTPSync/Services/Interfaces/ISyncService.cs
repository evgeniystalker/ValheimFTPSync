using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface ISyncService
    {
        Task<IReadOnlyList<SyncItem>> CompareAsync(string localPath, CancellationToken cancellationToken = default);
        Task SyncAsync(IReadOnlyList<SyncItem> items, CancellationToken cancellationToken = default);
    }
}
