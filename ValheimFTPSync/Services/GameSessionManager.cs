using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Configuration;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal sealed class GameSessionManager
    {
        public GameSessionManager(IAppLogger logger, IFtpFactory ftpFactory, IUserInteractive userInteractive, AppGlobalSettingManager appSetting)
        {
            _logger = logger;
            _ftpFactory = ftpFactory;
            _userInteractive = userInteractive;
            _appSetting = appSetting;
        }
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly IAppLogger _logger;
        private readonly IFtpFactory _ftpFactory;
        private readonly IUserInteractive _userInteractive;
        private readonly AppGlobalSettingManager _appSetting;


        public bool IsRunning => _cancellationTokenSource is not null;

        public async Task RunAsync()
        {
            if (IsRunning)
            {
                _logger.Warning("Server already running!");
                return;
            }
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                _logger.Info("SyncService creating...");

                var appSetting = _appSetting.AppSettingManager;
                var fileLocalService = _ftpFactory.CreateLocalFileService();
                ISyncService? syncService = _ftpFactory.CreateSyncService(_userInteractive, fileLocalService, appSetting.FtpUrl, appSetting.FtpUserName, appSetting.FtpPassword);
                if (syncService == null) { _logger.Error("Error on create syncService."); return; }


                _logger.Info("Sync running...");
                // Здесь будет работа игровой сессии.
                await Task.Delay(10000, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) when (_cancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(3000);
                _logger.Info("Operation canceled.");
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}

