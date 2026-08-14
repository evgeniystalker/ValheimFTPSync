using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpClient
    {
        void AppendFile(string absolutePath, IProgress<TransferProgress>? progress = null, CancellationToken ct = default);
        void DeleteFile(string absolutePath,  CancellationToken ct = default);
        void DownloadFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken ct = default);
        DateTime GetDateTimeStamp(string absolutePath);
        long GetFileSize(string absolutePath);
        IList<string> ListDirectory(string absolutePath);
        IList<string> ListDirectoryDetails(string absolutePath);
        void MakeDirectory(string absolutePath, CancellationToken ct = default);
        void RemoveDirectory(string absolutePath, CancellationToken ct = default);
        void Rename(string absolutePath, string name, CancellationToken ct = default);
        void UploadFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken ct = default);

        public event EventHandler<TransferProgress> ProgressChanged;

    }
}
