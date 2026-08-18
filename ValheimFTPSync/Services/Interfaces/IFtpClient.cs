using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpClient
    {
        void AppendFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
        void DeleteFile(string absolutePath,  CancellationToken cancellationToken = default);
        void DownloadFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
        DateTime GetDateTimeStamp(string absolutePath, CancellationToken cancellationToken = default);
        long GetFileSize(string absolutePath, CancellationToken cancellationToken = default);
        IEnumerable<string> ListDirectory(string absolutePath, CancellationToken cancellationToken = default);
        IEnumerable<string> ListDirectoryDetails(string absolutePath, CancellationToken cancellationToken = default);
        void MakeDirectory(string absolutePath, CancellationToken cancellationToken = default);
        void RemoveDirectory(string absolutePath, CancellationToken cancellationToken = default);
        void Rename(string absolutePath, string name, CancellationToken cancellationToken = default);
        void UploadFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);

        public event EventHandler<TransferProgress> ProgressChanged;

    }
}
