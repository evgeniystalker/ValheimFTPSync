using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Extensions
{
    internal static class DirectoryExtensions
    {
        public static IEnumerable<FtpFileModel> GetFilesInDirectoryRecursive(this FtpDirectoryModel dir)
        {
            return dir.Files.Concat(dir.Directories.SelectMany(GetFilesInDirectoryRecursive));
        }

        public static IEnumerable<FtpDirectoryModel> GetDirectoryRecursive(this FtpDirectoryModel dir)
        {
            return dir.Directories.Concat(dir.Directories.SelectMany(GetDirectoryRecursive));
        }
    }
}
