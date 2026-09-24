using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    internal class FtpFileModel
    {
            public string FileName;
            public string RelativePath;
            public DateTime Modified;
            public long Size;
            public FtpFileModel(string fileName, string filePath, DateTime dateOfChanged, long lenght)
            {
                FileName = fileName;
                RelativePath = filePath;
                Modified = dateOfChanged;
                Size = lenght;
            }
    }
}
