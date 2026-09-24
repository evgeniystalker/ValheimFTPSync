using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpFactory
    {
        IFtpClientBase? CreateFtpClient(string uri, ICredentials? credentials = null);
        IFtpService? CreateFtpService(IFtpClientBase ftpCLient, ILocalFileService localFileService);
        ISyncService? CreateSyncService(IFtpService ftpService, IUserInteractive userInteractive, ILocalFileService localFileService);
    }
}
