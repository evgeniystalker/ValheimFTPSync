using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Extensions
{
    internal static class DirectoryExtensions
    {
        static List<FileModel> GetFilesInDirectoryRecursive(this DirectoryModel dir)
        {
            List<FileModel> files = new List<FileModel>();
            files.AddRange(dir.Files);
            foreach (var recDir in dir.Directories)
            {
                files.AddRange(GetFilesInDirectoryRecursive(recDir));
            }

            return files;
        }

        public static List<DirectoryModel> GetDirectoryRecursive(this DirectoryModel dir)
        {
            List<DirectoryModel> directories = new List<DirectoryModel>();
            directories.AddRange(dir.Directories);
            foreach (var recDir in dir.Directories)
            {
                directories.AddRange(GetDirectoryRecursive(recDir));
            }
            return directories;
        }
    }
}
