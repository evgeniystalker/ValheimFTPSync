using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IFtpClientFactory
    {
        IFtpClientBase? CreateFtpClient(string uri, ICredentials? credentials = null);
    }
}
