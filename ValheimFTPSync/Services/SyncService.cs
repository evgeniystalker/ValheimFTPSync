using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValheimFTPSync.Extensions;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class SyncService : ISyncService
    {
        private readonly IFtpService _ftpService;
        private readonly ILocalFileService _localFileService;
        private readonly IUserInteractive _userInteractive;
        private readonly IAppLogger _logger;

        public SyncService(IFtpService ftpService, ILocalFileService localFileService, IUserInteractive userInteractive, IAppLogger logger)
        {
            _ftpService = ftpService;
            _localFileService = localFileService;
            _userInteractive = userInteractive;
            _logger = logger;
        }

        public async Task SyncAsync(string localPath, IProgress<OperationProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(localPath))
                _logger.Lo
            if (!Directory.Exists(localPath))
                throw new DirectoryNotFoundException($"Local directory not found: {localPath}");

            _logger.Info("Building remote file tree...");
            FtpDirectoryModel remoteRoot = await _ftpService.GetDirectoryInfoAsync(string.Empty, cancellationToken).ConfigureAwait(false);
            Dictionary<string, FtpFileModel> remoteFiles = remoteRoot
                .GetFilesInDirectoryRecursive()
                .ToDictionary(f => NormalizeKey(f.RelativePath), f => f);

            _logger.Info("Building local file tree...");
            Dictionary<string, FtpFileModel> localFiles = EnumerateLocalFiles(localPath)
                .ToDictionary(f => NormalizeKey(f.RelativePath), f => f);

            var toDownload = new List<FtpFileModel>();
            var toUpload = new List<FtpFileModel>();

            ConflictResolutionAction persistentDecision = ConflictResolutionAction.Skip;
            bool hasPersistent = false;

            // Сверяем удалённые файлы с локальными.
            foreach (FtpFileModel remote in remoteFiles.Values)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string key = NormalizeKey(remote.RelativePath);
                if (!localFiles.TryGetValue(key, out FtpFileModel local))
                {
                    toDownload.Add(remote);
                    continue;
                }

                DecideDirection(local, remote, ref persistentDecision, ref hasPersistent, out bool download, out bool upload, out bool skip);
                if (download)
                    toDownload.Add(remote);
                else if (upload)
                    toUpload.Add(local);
            }

            // Локальные файлы, которых нет на сервере.
            foreach (FtpFileModel local in localFiles.Values)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!remoteFiles.ContainsKey(NormalizeKey(local.RelativePath)))
                    toUpload.Add(local);
            }

            int total = toDownload.Count + toUpload.Count;
            _logger.Info($"Planned: {toDownload.Count} download(s), {toUpload.Count} upload(s), {remoteFiles.Count + localFiles.Count - total} skip(s).");

            if (total == 0)
            {
                _logger.Info("Already in sync.");
                progress?.Report(new OperationProgress(string.Empty, default, 0, 0, Operation.Download));
                return;
            }

            if (toDownload.Count > 0)
                await _ftpService.DownloadFilesAsync(toDownload, localPath, progress, cancellationToken).ConfigureAwait(false);

            if (toUpload.Count > 0)
                await _ftpService.UploadFilesAsync(toUpload, localPath, progress, cancellationToken).ConfigureAwait(false);

            // Выравниваем локальные времена, чтобы следующая синхронизация не считала файлы конфликтующими.
            AlignLocalTimestamps(localPath, toDownload, toUpload);
        }
    }
}
