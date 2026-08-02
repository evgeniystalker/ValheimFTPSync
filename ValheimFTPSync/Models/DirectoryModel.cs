using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimFTPSync.Models
{
    internal class DirectoryModel
    {
        public string PathDirectory { get; }
        public string NameDirectory { get; }
        public List<DirectoryModel> Directories { get; set; } = new List<DirectoryModel>();
        public List<FileModel> Files { get; set; } = new List<FileModel>();

        public DirectoryModel(string path)
        {
            PathDirectory = path;
            NameDirectory = new Uri(path).Segments.Last().TrimEnd('/');
        }
        public static List<FileModel> GetFilesInDirectoryRecursive(DirectoryModel dir)
        {
            List<FileModel> files = new List<FileModel>();
            files.AddRange(dir.Files);
            foreach (var recDir in dir.Directories)
            {
                files.AddRange(GetFilesInDirectoryRecursive(recDir));
            }

            return files;
        }

        public static List<DirectoryModel> GetDirectoryRecursive(DirectoryModel dir)
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
