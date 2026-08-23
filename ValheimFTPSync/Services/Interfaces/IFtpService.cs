using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpService
    {
        void DownloadFiles(IReadOnlyCollection<FtpFileModel> fileModels, string localPath, IProgress<OperationProgress>? progress = null, CancellationToken cancellationToken = default);
        Task DownloadFilesAsync(IReadOnlyCollection<FtpFileModel> fileModels, string localPath, IProgress<OperationProgress>? progress = null, CancellationToken cancellationToken = default);
        void UploadFiles(IReadOnlyCollection<FtpFileModel> fileModels, string localPath, IProgress<OperationProgress>? progress = null, CancellationToken cancellationToken = default);
        Task UploadFilesAsync(IReadOnlyCollection<FtpFileModel> fileModels, string localPath, IProgress<OperationProgress>? progress = null, CancellationToken cancellationToken = default);
        FtpDirectoryModel GetDirectoryInfo(string relativePath, CancellationToken cancellationToken = default);
        Task<FtpDirectoryModel> GetDirectoryInfoAsync(string relativePath, CancellationToken cancellationToken = default);
        void MakeDirectoryRecursive(string relativePath, CancellationToken cancellationToken = default);
        Task MakeDirectoryRecursiveAsync(string relativePath, CancellationToken cancellationToken = default);
        bool TryConnect(CancellationToken cancellationToken = default);
        Task<bool> TryConnectAsync(CancellationToken cancellationToken = default);
    }
}
