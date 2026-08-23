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

        /// <summary>
        /// Загружает локальные файлы обратно на FTP с прогрессом и поддержкой отмены.
        /// </summary>
     /*   public async Task UploadFilesBack(string pathTempDirectory, IProgress<(float, string, float)> progress, CancellationToken token)
        {
            List<string> TempDirectory = Directory.GetDirectories(pathTempDirectory, "*", SearchOption.AllDirectories).ToList();
            List<string> filesInTempDirectory = Directory.GetFiles(pathTempDirectory, "", SearchOption.AllDirectories).ToList();
            //FTPLISTS
            //List<string> filesAll = DirectoryModel.GetFilesInDirectoryRecursive(ListFiles);
            //List<string> directories = DirectoryModel.GetDirectoryRecursive(ListFiles);
            var directories = TempDirectory.Select(x => Path.GetRelativePath(pathTempDirectory, x)).Select(x => new Uri(Uri, x).OriginalString).ToList();
            var filesAll = filesInTempDirectory.Select(x => Path.GetRelativePath(pathTempDirectory, x)).Select(x => new Uri(Uri, x).OriginalString).ToList();

            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            foreach (var path in directories)
            {
                CreateDirectoryFtp(path);
            }

            int countFiles = 0;
            string fileName = "";
            Progress<float> progressOneFileUploading = new Progress<float>(prog =>
            {
                progress.Report((((prog + countFiles) / filesAll.Count), fileName, prog));
            });

            foreach (string file in filesAll)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                fileName = Path.GetFileName(file);
                Uri fileNameUri = new Uri(file);
                var tempPath = Path.Combine(pathTempDirectory, Uri.MakeRelativeUri(fileNameUri).ToString());
                if (!File.Exists(tempPath))
                    throw new Exception("Не найден файл " + tempPath);
                await Task.Run(() => UploadFileFtp(tempPath, fileNameUri, progressOneFileUploading));
                countFiles++;
            }

        }*/

        /// <summary>
        /// Удаляет все файлы и каталоги в целевой папке FTP.
        /// </summary>
/*        public async Task DeleteFilesFTP(IProgress<(float, string, float)> progress, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();
            dirModel = LoadDirectoryModel(Uri.OriginalString);
            List<FileModel> filesAll = dirModel.GetFilesInDirectoryRecursive();
            List<DirectoryModel> dirAll = dirModel.GetDirectoryRecursive();
            DateTimeChanged.Invoke(filesAll.Select(x => x.DateTimeChangedFile).Max());
            int count = 0;
            foreach (var file in filesAll)
            {
                FtpWebRequest ftpWeb = FtpWebRequest.Create(file.FilePath) as FtpWebRequest;
                ftpWeb.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)await ftpWeb.GetResponseAsync();
                if (!response.StatusDescription.Contains("250") && response.StatusCode != FtpStatusCode.FileActionOK)
                {
                    throw new Exception("Ошибка при удалении файла " + file.FilePath);
                }
                response.Close();

                progress.Report((++count / (float)(filesAll.Count + dirAll.Count), "Удалено: " + file.FileName, 1));
                if (ct.IsCancellationRequested)
                    ct.ThrowIfCancellationRequested();
            }
            foreach (var dir in dirAll)
            {
                FtpWebRequest ftpWeb = FtpWebRequest.Create(dir.PathDirectory) as FtpWebRequest;
                ftpWeb.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse response = (FtpWebResponse)await ftpWeb.GetResponseAsync();
                if (!response.StatusDescription.Contains("250") && response.StatusCode != FtpStatusCode.FileActionOK)
                {
                    throw new Exception("Ошибка при удалении директории " + dir.NameDirectory);
                }
                response.Close();
                progress.Report((++count / (float)(filesAll.Count + dirAll.Count), "Удалено: " + dir.NameDirectory, 1));
                if (ct.IsCancellationRequested)
                    ct.ThrowIfCancellationRequested();
            }
        }
*/        /// <summary>
        /// Удаляет все файлы и каталоги во временной локальной папке.
        /// </summary>

        public void DownloadFiles(IReadOnlyCollection<FtpFileModel> fileModels, string targetDirectoryPath, IProgress<OperationProgress>? progress, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach ((int index, FtpFileModel file) in fileModels.Index())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var localFilePath = Path.Combine(targetDirectoryPath, file.RelativePath).Replace("/", "\\");
                LocalFileService.CreateDirectory(Path.GetDirectoryName(localFilePath));
                using FileStream fileStream = LocalFileService.CreateFileStream(localFilePath, Operation.Download);

                IProgress<TransferProgress> transferProgres = new Progress<TransferProgress>(tp =>
                        progress?.Report(new OperationProgress(file.RelativePath, tp, index, fileModels.Count, Operation.Download)
                        ));
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
                using FileStream fileStream = LocalFileService.CreateFileStream(localFilePath, Operation.Download, async: true);

                IProgress<TransferProgress> transferProgres = new Progress<TransferProgress>(tp =>
                        progress?.Report(new OperationProgress(file.RelativePath, tp, index, fileModels.Count, Operation.Download)
                        ));
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
                using FileStream fileStream = LocalFileService.CreateFileStream(localFilePath, Operation.Upload);

                IProgress<TransferProgress> transferProgres = new Progress<TransferProgress>(tp =>
                        progress?.Report(new OperationProgress(file.RelativePath, tp, index, fileModels.Count, Operation.Upload)
                        ));

                FtpClient.UploadFile(file.RelativePath, fileStream, transferProgres, cancellationToken);
            }
        }

        public async Task UploadFilesAsync(IReadOnlyCollection<FtpFileModel> fileModels, string sourceDirectoryPath, IProgress<OperationProgress>? progress, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach ((int index, FtpFileModel file) in fileModels.Index())
            {//сделать создание директориии.
                cancellationToken.ThrowIfCancellationRequested();
                var localFilePath = Path.Combine(sourceDirectoryPath, file.RelativePath).Replace("/", "\\");

                if (!File.Exists(localFilePath))
                {
                    Logger.Error($"File not found: {localFilePath}");
                    return;
                }
                using FileStream fileStream = LocalFileService.CreateFileStream(localFilePath, Operation.Upload, async: true);

                var treeDirectory = Path.GetDirectoryName(file.RelativePath);
                if (!string.IsNullOrEmpty(treeDirectory))
                    await MakeDirectoryRecursiveAsync(treeDirectory);


                IProgress<TransferProgress> transferProgres = new Progress<TransferProgress>(tp =>
                        progress?.Report(new OperationProgress(file.RelativePath, tp, index, fileModels.Count, Operation.Upload)
                        ));

                await FtpClient.UploadFileAsync(file.RelativePath, fileStream, transferProgres, cancellationToken).ConfigureAwait(false);
            }
        }

        public FtpDirectoryModel GetDirectoryInfo(string relativePath, CancellationToken cancellationToken = default)
        {
            FtpDirectoryModel dir = new FtpDirectoryModel(relativePath);
            IEnumerable<string> listDirectory = FtpClient.ListDirectory(relativePath, cancellationToken);
            IEnumerable<string> dataFilesDetails = FtpClient.ListDirectoryDetails(relativePath, cancellationToken);
            var dictionaryDetails = dataFilesDetails.ToDictionary(x => x.Split(' ', 9, StringSplitOptions.RemoveEmptyEntries).Last(), x => x);
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
            var dictionaryDetails = dataFilesDetails.ToDictionary(x => x.Split(' ', 9, StringSplitOptions.RemoveEmptyEntries).Last(), x => x);
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
            IEnumerable<string> dataFilesDetails = FtpClient.ListDirectoryDetails(relativePath, cancellationToken);
            var dictionaryDetails = dataFilesDetails.ToDictionary(x => x.Split(' ', 9, StringSplitOptions.RemoveEmptyEntries).Last(), x => x);
            var detail = dictionaryDetails[Path.GetFileName(relativePath)];
            return detail.StartsWith('d');
        }
        private async Task<bool> IsDirectoryAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            var rootDirectory = Path.GetDirectoryName(relativePath) ?? string.Empty;
            IEnumerable<string> dataFilesDetails = await FtpClient.ListDirectoryDetailsAsync(rootDirectory, cancellationToken).ConfigureAwait(false);
            var dictionaryDetails = dataFilesDetails.ToDictionary(x => x.Split(' ', 9, StringSplitOptions.RemoveEmptyEntries).Last(), x => x);
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
            while (recursivePath.TryPop(out parentRelativePath) && !string.IsNullOrEmpty(parentRelativePath))
            {
                if (!IsDirectory(parentRelativePath))
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
            bool isDirectory = true;
            while (recursivePath.TryPop(out parentRelativePath) && !string.IsNullOrEmpty(parentRelativePath))
            {
                if (isDirectory)
                    isDirectory = await IsDirectoryAsync(parentRelativePath).ConfigureAwait(false);
                if (!isDirectory)
                    await FtpClient.MakeDirectoryAsync(parentRelativePath, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
