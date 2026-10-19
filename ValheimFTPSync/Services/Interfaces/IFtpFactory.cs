using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpFactory
    {
        IFtpClientBase? CreateFtpClient(string uri, ICredentials? credentials = null);
        IFtpClientBase? CreateFtpClient(string uri, string? username, string? password);
        IFtpService? CreateFtpService(IFtpClientBase ftpCLient, ILocalFileService localFileService);
        ISyncService? CreateSyncService(IFtpService ftpService, IUserInteractive userInteractive, ILocalFileService localFileService);
        ISyncService? CreateSyncService(IUserInteractive userInteractive, ILocalFileService localFileService, string uri, ICredentials? credentials = null);
        ISyncService? CreateSyncService(IUserInteractive userInteractive, ILocalFileService localFileService, string uri, string? username, string? password);
        ILocalFileService CreateLocalFileService();
    }
}
