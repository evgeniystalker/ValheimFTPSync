using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpClientAsync
    {
        Task AppendFileAsync(string absolutePath, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string absolutePath, CancellationToken cancellationToken = default);
        Task DownloadFileAsync(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
        Task<DateTime> GetDateTimeStampAsync(string absolutePath, CancellationToken cancellationToken = default);
        Task<long> GetFileSizeAsync(string absolutePath, CancellationToken cancellationToken = default);
        Task<IList<string>> ListDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default);
        Task<IList<string>> ListDirectoryDetailsAsync(string absolutePath, CancellationToken cancellationToken = default);
        Task MakeDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default);
        Task RemoveDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default);
        Task RenameAsync(string absolutePath, string name, CancellationToken cancellationToken = default);
        Task UploadFileAsync(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);

        public event EventHandler<TransferProgress> ProgressChanged;

    }
}
