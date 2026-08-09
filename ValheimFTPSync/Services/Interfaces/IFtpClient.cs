using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpClient
    {

        void AppendFile(string uri, CancellationToken ct = default);
        void DeleteFile(string uri, CancellationToken ct = default);
        Task DeleteFileAsync(string uri, CancellationToken ct = default);
        void DownloadFile(string uri, Stream stream, CancellationToken ct = default);
        Task DownloadFileAsync(string uri, Stream stream, CancellationToken ct = default);
        DateTime GetDateTimestamp();
        float GetSizeFile();

        IList<string> ListDirectory(string directoryPath);
        Task<IList<string>> ListDirectoryAsync(string directoryPath);

        IList<string> ListDirectoryDetails(string directoryPath);
        Task<IList<string>> ListDirectoryDetailsAsync(string directoryPath);

        void MakeDirectory(string directoryPath, CancellationToken ct = default);
        Task MakeDirectoryAsync(string directoryPath, CancellationToken ct = default);

        void RemoveDirectory(string directoryPath, CancellationToken ct = default);
        Task RemoveDirectoryAsync(string directoryPath, CancellationToken ct = default);

        void Rename(string uri, string name, CancellationToken ct = default);
        Task RenameAsync(string uri, string name, CancellationToken ct = default);

        void UploadFile(string uri, Stream stream, CancellationToken ct = default);
        Task UploadFileAsync(string uri, Stream stream, CancellationToken ct = default);

        public event EventHandler<ProgressEventArgs> ProgressChanged;

    }
}
