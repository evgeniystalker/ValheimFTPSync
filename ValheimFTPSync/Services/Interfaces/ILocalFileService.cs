using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface ILocalFileService
    {
        public void CreateDirectory(string? path);
        FileStream CreateFileStream(string pathFile, Operation direct, bool async = false);
        public void UpdateLocalFileAttrributeDateTime(string pathFile, DateTime attrDateTime);

    }
}
