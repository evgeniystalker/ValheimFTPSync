using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class FtpClientFactory : IFtpClientFactory
    {
        IAppLogger _appLogger;
        public FtpClientFactory(IAppLogger logger)
        {
            _appLogger = logger;
        }
        public IFtpClientBase? CreateFtpClient(string uri, ICredentials? credentials = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                _appLogger.Warning("Cannot set URI: the value is null.");
                return null;
            }
            Uri? tryUri;

            if (!Uri.TryCreate(uri, UriKind.Absolute, out tryUri) && tryUri is null)
            {
                _appLogger.Warning($"Can't create URI: \"{uri}\" is not valid.");
                return null;
            }

            if (tryUri.Scheme != Uri.UriSchemeFtp)
            {
                _appLogger.Warning($"Unsupported URI scheme: \"{tryUri.Scheme}\".");
                return null;
            }

            var ftpClient = new FtpClient(tryUri, credentials);
            return new LoggingFtpClient(ftpClient, _appLogger);
        }
    }
}
