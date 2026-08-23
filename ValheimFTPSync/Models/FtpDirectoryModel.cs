using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimFTPSync.Models
{
    internal class FtpDirectoryModel
    {
        public string PathDirectory { get; }
        public string NameDirectory { get; }
        public List<FtpDirectoryModel> Directories { get; } = new List<FtpDirectoryModel>();
        public List<FtpFileModel> Files { get; } = new List<FtpFileModel>();

        public FtpDirectoryModel(string relativePath)
        {
            PathDirectory = relativePath;
            NameDirectory = Path.GetFileName(relativePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) ?? string.Empty;
        }
    }
}
