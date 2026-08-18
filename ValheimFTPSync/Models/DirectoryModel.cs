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
    }
}
