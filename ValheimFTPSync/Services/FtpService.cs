using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using ValheimFTPSync.Extensions;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services.Interfaces;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ValheimFTPSync.Services
{
    internal class FtpService : IFtpService
    {
        private IFtpClientBase FtpClient { get; set; }
        private IAppLogger Logger { get; set; }
        private ILocalFileService LocalFileService { get; set; }

        public FtpService(IFtpClientBase ftpClient, IAppLogger appLogger, ILocalFileService localFileService)
        {
            Logger = appLogger;
            FtpClient = ftpClient;
            LocalFileService = localFileService;
        }

        /// <summary>
        /// Пытается подключиться к FTP и возвращает пользовательское сообщение о результате.
        /// </summary>
        public bool TryConnect(CancellationToken cancellationToken = default)
        {
            if (FtpClient is null)
                return false;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var isSuccessful = FtpClient.Connect(cancellationToken);
                if (isSuccessful)
                    Logger.Info("Connection successful.");
                else
                {
                    Logger.Warning("Connection unsuccessful.");
                    bool isConnected = NetworkInterface.GetIsNetworkAvailable();
                    if (!isConnected)
                        Logger?.Error("Check your internet connection.");
                }
                return isSuccessful;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Пытается подключиться к FTP и возвращает пользовательское сообщение о результате.
        /// </summary>
        public async Task<bool> TryConnectAsync(CancellationToken cancellationToken = default)
        {
            if (FtpClient is null)
                return false;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var isSuccessful = await FtpClient.ConnectAsync(cancellationToken);
                if (isSuccessful)
                    Logger.Info("Connection successful.");
                else
                {
                    Logger.Warning("Connection unsuccessful.");
                    bool isConnected = NetworkInterface.GetIsNetworkAvailable();
                    if (!isConnected)
                        Logger?.Error("Check your internet connection.");
                }
                return isSuccessful;
            }
            catch
            {
                return false;
            }
        }

        public void DeleteFilesOnFtp(IReadOnlyCollection<FtpFileModel> fileModels, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach ((int index, FtpFileModel file) in fileModels.Index())
            {
                progress?.Report(new OperationProgress(file.RelativePath, default, index, fileModels.Count, Operation.Delete));
                cancellationToken.ThrowIfCancellationRequested();
                FtpClient.DeleteFile(file.RelativePath);
            }
        }

        public async Task DeleteFilesOnFtpAsync(IReadOnlyCollection<FtpFileModel> fileModels, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach ((int index, FtpFileModel file) in fileModels.Index())
            {
                progress?.Report(new OperationProgress(file.RelativePath, default, index, fileModels.Count, Operation.Delete));
                cancellationToken.ThrowIfCancellationRequested();
                await FtpClient.DeleteFileAsync(file.RelativePath).ConfigureAwait(false);
            }
        }

        public void RemoveDirectories(IReadOnlyCollection<FtpFileModel> fileModels, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach ((int index, FtpFileModel file) in fileModels.Index())
            {
                progress?.Report(new OperationProgress(file.RelativePath, default, index, fileModels.Count, Operation.Delete));
                cancellationToken.ThrowIfCancellationRequested();
                FtpClient.RemoveDirectory(file.RelativePath, cancellationToken);
            }
        }

        public async Task RemoveDirectoriesAsync(IReadOnlyCollection<FtpFileModel> fileModels, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach ((int index, FtpFileModel file) in fileModels.Index())
            {
                progress?.Report(new OperationProgress(file.RelativePath, default, index, fileModels.Count, Operation.Delete));
                cancellationToken.ThrowIfCancellationRequested();
                await FtpClient.RemoveDirectoryAsync(file.RelativePath, cancellationToken).ConfigureAwait(false);
            }
        }

        public void DownloadFiles(IReadOnlyCollection<FtpFileModel> fileModels, string targetDirectoryPath, IProgress<OperationProgress>? progress, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach ((int index, FtpFileModel file) in fileModels.Index())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var localFilePath = Path.Combine(targetDirectoryPath, file.RelativePath).Replace("/", "\\");
                LocalFileService.CreateDirectory(Path.GetDirectoryName(localFilePath));
                FileStream? fileStream = default;
                try
                {
                    fileStream = LocalFileService.CreateFileStream(localFilePath, Operation.Download);
                }
                catch (IOException ex)
                {
                    Logger.Error("Error when creating a FileStream", ex);
                    continue;
                }
                IProgress<TransferProgress> transferProgres = new Progress<TransferProgress>(tp =>
                        progress?.Report(new OperationProgress(file.RelativePath, tp, index, fileModels.Count, Operation.Download)
                        ));
                using (fileStream)
                    FtpClient.DownloadFile(file.RelativePath, fileStream, transferProgres, cancellationToken);
            }
        }

        public async Task DownloadFilesAsync(IReadOnlyCollection<FtpFileModel> fileModels, string targetDirectoryPath, IProgress<OperationProgress>? progress, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach ((int index, FtpFileModel file) in fileModels.Index())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var localFilePath = Path.Combine(targetDirectoryPath, file.RelativePath).Replace("/", "\\");
                LocalFileService.CreateDirectory(Path.GetDirectoryName(localFilePath));

                FileStream? fileStream = default;
                try
                {
                    fileStream = LocalFileService.CreateFileStream(localFilePath, Operation.Download, async: true);
                }
                catch (IOException ex)
                {
                    Logger.Error("Error when creating a FileStream", ex);
                    continue;
                }
                IProgress<TransferProgress> transferProgres = new Progress<TransferProgress>(tp =>
                    progress?.Report(new OperationProgress(file.RelativePath, tp, index, fileModels.Count, Operation.Download)
                    ));
                await using (fileStream)
                    await FtpClient.DownloadFileAsync(file.RelativePath, fileStream, transferProgres, cancellationToken).ConfigureAwait(false);
            }
        }

        public void UploadFiles(IReadOnlyCollection<FtpFileModel> fileModels, string sourceDirectoryPath, IProgress<OperationProgress>? progress, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach ((int index, FtpFileModel file) in fileModels.Index())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var localFilePath = Path.Combine(sourceDirectoryPath, file.RelativePath).Replace("/", "\\");
                FileStream? fileStream = default;
                try
                {
                    fileStream = LocalFileService.CreateFileStream(localFilePath, Operation.Upload);
                }
                catch (IOException ex)
                {
                    Logger.Error("Error when creating a FileStream", ex);
                    continue;
                }

                var treeDirectory = Path.GetDirectoryName(file.RelativePath);
                if (!string.IsNullOrEmpty(treeDirectory))
                    MakeDirectoryRecursive(treeDirectory);

                IProgress<TransferProgress> transferProgres = new Progress<TransferProgress>(tp =>
                        progress?.Report(new OperationProgress(file.RelativePath, tp, index, fileModels.Count, Operation.Upload)
                        ));

                using (fileStream)
                    FtpClient.UploadFile(file.RelativePath, fileStream, transferProgres, cancellationToken);
            }
        }

        public async Task UploadFilesAsync(IReadOnlyCollection<FtpFileModel> fileModels, string sourceDirectoryPath, IProgress<OperationProgress>? progress, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach ((int index, FtpFileModel file) in fileModels.Index())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var localFilePath = Path.Combine(sourceDirectoryPath, file.RelativePath).Replace("/", "\\");
                FileStream? fileStream = default;
                try
                {
                    fileStream = LocalFileService.CreateFileStream(localFilePath, Operation.Upload, async: true);
                }
                catch (IOException ex)
                {
                    Logger.Error("Error when creating a FileStream", ex);
                    continue;
                }

                var treeDirectory = Path.GetDirectoryName(file.RelativePath);
                if (!string.IsNullOrEmpty(treeDirectory))
                    await MakeDirectoryRecursiveAsync(treeDirectory);

                IProgress<TransferProgress> transferProgres = new Progress<TransferProgress>(tp =>
                        progress?.Report(new OperationProgress(file.RelativePath, tp, index, fileModels.Count, Operation.Upload)
                        ));

                await using (fileStream)
                    await FtpClient.UploadFileAsync(file.RelativePath, fileStream, transferProgres, cancellationToken).ConfigureAwait(false);
            }
        }

        public FtpDirectoryModel GetDirectoryInfo(string relativePath, CancellationToken cancellationToken = default)
        {
            FtpDirectoryModel dir = new FtpDirectoryModel(relativePath);
            IEnumerable<string> listDirectory = FtpClient.ListDirectory(relativePath, cancellationToken);
            IEnumerable<string> dataFilesDetails = FtpClient.ListDirectoryDetails(relativePath, cancellationToken);
            Dictionary<string, string> dictionaryDetails = ParseToDictionary(dataFilesDetails);
            if (listDirectory.Count() != dataFilesDetails.Count())
                throw new InvalidOperationException("The list of files and the list of parts do not match in terms of quantity.");
            foreach (var itemUrl in listDirectory)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var detail = dictionaryDetails[Path.GetFileName(itemUrl)];
                if (detail.StartsWith('d'))
                {
                    dir.Directories.Add(GetDirectoryInfo(itemUrl, cancellationToken));
                }
                else if (detail.StartsWith("-r"))
                {
                    var dateTimeChanged = FtpClient.GetDateTimeStamp(itemUrl, cancellationToken);
                    var fileSize = FtpClient.GetFileSize(itemUrl, cancellationToken);
                    dir.Files.Add(new FtpFileModel(Path.GetFileName(itemUrl), itemUrl, dateTimeChanged, fileSize));
                }
            }
            return dir;
        }

        public async Task<FtpDirectoryModel> GetDirectoryInfoAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            FtpDirectoryModel dir = new FtpDirectoryModel(relativePath);
            IEnumerable<string> listDirectory = await FtpClient.ListDirectoryAsync(relativePath, cancellationToken).ConfigureAwait(false);
            IEnumerable<string> dataFilesDetails = await FtpClient.ListDirectoryDetailsAsync(relativePath, cancellationToken).ConfigureAwait(false);
            Dictionary<string, string> dictionaryDetails = ParseToDictionary(dataFilesDetails);
            if (listDirectory.Count() != dataFilesDetails.Count())
                throw new InvalidOperationException("The list of files and the list of parts do not match in terms of quantity.");
            foreach (var itemUrl in listDirectory)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var detail = dictionaryDetails[Path.GetFileName(itemUrl)];
                if (detail.StartsWith('d'))
                {
                    dir.Directories.Add(await GetDirectoryInfoAsync(itemUrl, cancellationToken).ConfigureAwait(false));
                }
                else if (detail.StartsWith("-r"))
                {
                    var dateTimeChanged = await FtpClient.GetDateTimeStampAsync(itemUrl, cancellationToken).ConfigureAwait(false);
                    var fileSize = await FtpClient.GetFileSizeAsync(itemUrl, cancellationToken).ConfigureAwait(false);
                    dir.Files.Add(new FtpFileModel(Path.GetFileName(itemUrl), itemUrl, dateTimeChanged, fileSize));
                }
            }
            return dir;
        }

        private bool IsDirectory(string relativePath, CancellationToken cancellationToken = default)
        {
            var rootDirectory = Path.GetDirectoryName(relativePath) ?? string.Empty;
            IEnumerable<string> dataFilesDetails = FtpClient.ListDirectoryDetails(rootDirectory, cancellationToken);
            Dictionary<string, string> dictionaryDetails = ParseToDictionary(dataFilesDetails);
            dictionaryDetails.TryGetValue(Path.GetFileName(relativePath), out string? detail);
            return detail?.StartsWith('d') ?? false;
        }

        private static Dictionary<string, string> ParseToDictionary(IEnumerable<string> dataFilesDetails)
        {
            return dataFilesDetails.ToDictionary(x => x.Split(' ', 9, StringSplitOptions.RemoveEmptyEntries).Last(), x => x);
        }

        private async Task<bool> IsDirectoryAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            var rootDirectory = Path.GetDirectoryName(relativePath) ?? string.Empty;
            IEnumerable<string> dataFilesDetails = await FtpClient.ListDirectoryDetailsAsync(rootDirectory, cancellationToken).ConfigureAwait(false);
            Dictionary<string, string> dictionaryDetails = ParseToDictionary(dataFilesDetails);
            dictionaryDetails.TryGetValue(Path.GetFileName(relativePath), out string? detail);
            return detail?.StartsWith('d') ?? false;
        }

        public void MakeDirectoryRecursive(string relativePath, CancellationToken cancellationToken = default)
        {
            Stack<string> recursivePath = new Stack<string>();
            var parentRelativePath = relativePath;
            while (!string.IsNullOrEmpty(parentRelativePath))
            {
                recursivePath.Push(parentRelativePath);
                parentRelativePath = Path.GetDirectoryName(parentRelativePath);
            }
            bool makeDirectory = false;
            while (recursivePath.TryPop(out parentRelativePath) && !string.IsNullOrEmpty(parentRelativePath))
            {
                if (makeDirectory || (makeDirectory = !IsDirectory(parentRelativePath)))
                    FtpClient.MakeDirectory(parentRelativePath, cancellationToken);
            }
        }
        public async Task MakeDirectoryRecursiveAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            Stack<string> recursivePath = new Stack<string>();
            var parentRelativePath = relativePath;
            while (!string.IsNullOrEmpty(parentRelativePath))
            {
                recursivePath.Push(parentRelativePath);
                parentRelativePath = Path.GetDirectoryName(parentRelativePath);
            }
            bool makeDirectory = false;
            while (recursivePath.TryPop(out parentRelativePath) && !string.IsNullOrEmpty(parentRelativePath))
            {
                if (makeDirectory || (makeDirectory = !await IsDirectoryAsync(parentRelativePath).ConfigureAwait(false)))
                    await FtpClient.MakeDirectoryAsync(parentRelativePath, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
