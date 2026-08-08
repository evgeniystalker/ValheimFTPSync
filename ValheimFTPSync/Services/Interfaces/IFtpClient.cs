using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpClient
    {

        void AppendFile();
        void DeleteFile(string filePath);
        FileStream DownloadFile();
        DateTime GetDateTimestamp();
        float GetSizeFile();

        IList<T> ListDirectory<T>();

        IList<T> ListDirectoryDetails<T>();

        void MakeDirectory(string DirectoryPath);

        void RemoveDirectory(string DirectoryPath);

        void Rename();

        void UploadFile();

        public event EventHandler<ProgressEventArgs> ProgressChanged;

    }
}
