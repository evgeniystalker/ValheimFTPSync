using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpClientAsync
    {
        Task AppendFileAsync(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default);
        Task DownloadFileAsync(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
        Task<DateTime> GetDateTimeStampAsync(string relativePath, CancellationToken cancellationToken = default);
        Task<long> GetFileSizeAsync(string relativePath, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> ListDirectoryAsync(string relativePath, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> ListDirectoryDetailsAsync(string relativePath, CancellationToken cancellationToken = default);
        Task MakeDirectoryAsync(string relativePath, CancellationToken cancellationToken = default);
        Task RemoveDirectoryAsync(string relativePath, CancellationToken cancellationToken = default);
        Task RenameAsync(string relativePath, string name, CancellationToken cancellationToken = default);
        Task UploadFileAsync(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
    }
}
