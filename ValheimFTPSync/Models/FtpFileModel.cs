using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    internal class FtpFileModel
    {
            public string FileName;
            public string RelativePath;
            public DateTime DateTimeChangedFile;
            public long Length;
            public FtpFileModel(string fileName, string filePath, DateTime dateOfChanged, long lenght)
            {
                FileName = fileName;
                RelativePath = filePath;
                DateTimeChangedFile = dateOfChanged;
                Length = lenght;
            }
    }
}
