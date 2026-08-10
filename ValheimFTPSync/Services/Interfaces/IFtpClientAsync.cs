using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpClientAsync
    {
        Task AppendFileAsync(string absolutePath, CancellationToken ct = default);
        Task DeleteFileAsync(string absolutePath, CancellationToken ct = default);
        Task DownloadFileAsync(string absolutePath, Stream stream, CancellationToken ct = default);
        Task<DateTime> GetDateTimeStampAsync(string absolutePath, CancellationToken ct = default);
        Task<long> GetFileSizeAsync(string absolutePath, CancellationToken ct = default);
        Task<IList<string>> ListDirectoryAsync(string absolutePath);
        Task<IList<string>> ListDirectoryDetailsAsync(string absolutePath);
        Task MakeDirectoryAsync(string absolutePath, CancellationToken ct = default);
        Task RemoveDirectoryAsync(string absolutePath, CancellationToken ct = default);
        Task RenameAsync(string absolutePath, string name, CancellationToken ct = default);
        Task UploadFileAsync(string absolutePath, Stream stream, CancellationToken ct = default);

        public event EventHandler<ProgressEventArgs> ProgressChanged;

    }
}
