using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class FtpFactory : IFtpFactory
    {
        IAppLogger _appLogger;

        public FtpFactory(IAppLogger logger)
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
        public IFtpClientBase? CreateFtpClient(string uri, string? username, string? password)
        {
            return CreateFtpClient(uri, CreateCredentialsOrNull(username, password));
        }

        public IFtpService? CreateFtpService(IFtpClientBase ftpCLient, ILocalFileService localFileService)
        {
            return new FtpService(ftpCLient, _appLogger, localFileService);
        }

        public ILocalFileService CreateLocalFileService()
        {
            return new LocalFileService();
        }

        public ISyncService? CreateSyncService(IFtpService ftpService, IUserInteractive userInteractive, ILocalFileService localFileService)
        {
            return new SyncService(ftpService, localFileService, userInteractive, _appLogger);
        }
        public ISyncService? CreateSyncService(IUserInteractive userInteractive, ILocalFileService localFileService, string uri, ICredentials? credentials = null)
        {
            var client = this.CreateFtpClient(uri, credentials);
            if (client == null)
            {
                _appLogger.Error("Unable to create client.");
                return null;
            }
            var ftpService = this.CreateFtpService(client, localFileService);
            if (ftpService == null)
            {
                _appLogger.Error("Unable to create ftpService.");
                return null;
            }

            return this.CreateSyncService(ftpService, userInteractive, localFileService);
        }
        public ISyncService? CreateSyncService(IUserInteractive userInteractive, ILocalFileService localFileService, string uri, string? username, string? password)
        {
            return this.CreateSyncService(userInteractive, localFileService, uri, CreateCredentialsOrNull(username, password));
        }

        private ICredentials? CreateCredentialsOrNull(string? username, string? password)
        {
            return string.IsNullOrEmpty(username) ? null : new NetworkCredential(username, password);
        }
    }
}

