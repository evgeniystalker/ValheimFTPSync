using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Text;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class LocalFileService : ILocalFileService
    {
        public void CreateDirectory(string? path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public FileStream CreateFileStream(string pathFile, Operation direct, bool async) => direct switch
        {
            Operation.Upload => new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, async),
            Operation.Download or Operation.Append => new FileStream(pathFile, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, async),
            _ => throw new ArgumentOutOfRangeException(nameof(direct), direct, "Неподдерживаемая операция")
        };

        public void UpdateLocalFileAttrributeDateTime(string pathFile, DateTime attrDateTime)
        {
            FileInfo fileInfo = new FileInfo(pathFile);
            fileInfo.LastWriteTimeUtc = attrDateTime;
            fileInfo.LastAccessTimeUtc = attrDateTime;
        }
    }
}
