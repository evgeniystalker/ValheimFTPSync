using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpClient
    {
        void AppendFile(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
        void DeleteFile(string relativePath,  CancellationToken cancellationToken = default);
        void DownloadFile(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
        DateTime GetDateTimeStamp(string relativePath, CancellationToken cancellationToken = default);
        long GetFileSize(string relativePath, CancellationToken cancellationToken = default);
        IEnumerable<string> ListDirectory(string relativePath, CancellationToken cancellationToken = default);
        IEnumerable<string> ListDirectoryDetails(string relativePath, CancellationToken cancellationToken = default);
        void MakeDirectory(string relativePath, CancellationToken cancellationToken = default);
        void RemoveDirectory(string relativePath, CancellationToken cancellationToken = default);
        void Rename(string relativePath, string name, CancellationToken cancellationToken = default);
        void UploadFile(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
        bool Connect(CancellationToken cancellationToken = default);
    }
    internal interface IFtpClientBase : IFtpClient, IFtpClientAsync
    {
        public event EventHandler<TransferProgress> ProgressChanged;
    }
}
