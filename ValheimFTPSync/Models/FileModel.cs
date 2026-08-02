using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    internal class FileModel
    {
            public string FileName;
            public string FilePath;
            public DateTime DateTimeChangedFile;
            public long Length;
            public FileModel(string fileName, string filePath, DateTime dateOfChanged, long lenght)
            {
                FileName = fileName;
                FilePath = filePath;
                DateTimeChangedFile = dateOfChanged;
                Length = lenght;
            }
    }
}
