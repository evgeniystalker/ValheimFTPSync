using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using ValheimFTPSync.Configuration;
using ValheimFTPSync.Properties;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal sealed class GameSessionManager
    {
        public GameSessionManager(IAppLogger logger, IFtpFactory ftpFactory, IUserInteractive userInteractive, GameSessionValidator validator)
        {
            _logger = logger;
            _ftpFactory = ftpFactory;
            _userInteractive = userInteractive;
            _validator = validator;
        }
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly IAppLogger _logger;
        private readonly IFtpFactory _ftpFactory;
        private readonly IUserInteractive _userInteractive;
        private readonly GameSessionValidator _validator;

        public bool IsRunning => _cancellationTokenSource is not null;

        public async Task RunAsync(AppGlobalSettingSnapshot settingSnapshot)
        {
            if (IsRunning)
            {
                _logger.Warning("Server already running!");
                return;
            }
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var appSetting = settingSnapshot.App;
                var fileLocalService = _ftpFactory.CreateLocalFileService();

                var error = _validator.ValidateForLaunch(settingSnapshot);
                if (error is not null)
                {
                    _logger.Error(error);
                    return;
                }

                _logger.Info("SyncService creating...");
                ISyncService? syncService = _ftpFactory.CreateSyncService(_userInteractive, fileLocalService, appSetting.FtpUrl, appSetting.FtpUserName, appSetting.FtpPassword);
                if (syncService == null) { _logger.Error("Error on create syncService."); return; }


                _logger.Info("Sync running...");
                // Здесь будет работа игровой сессии.
                await RunServerAsync(settingSnapshot, _cancellationTokenSource.Token);

            }
            catch (OperationCanceledException) when (_cancellationTokenSource.IsCancellationRequested)
            {
                _logger.Info(Messages.OperationСanceled);
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

        private static ProcessStartInfo CreateStartInfo(AppGlobalSettingSnapshot snapshot)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = snapshot.App.ValheimExePath,
                WorkingDirectory = snapshot.App.ServerAppFolderPath,
                UseShellExecute = false
            };

            startInfo.EnvironmentVariables.Add("SteamAppId", "892970");

            foreach (var argument in snapshot.Server.BuildArguments())
            {
                startInfo.ArgumentList.Add(argument);
            }

            return startInfo;
        }

        private async Task<int> RunServerAsync(AppGlobalSettingSnapshot snapshot, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(snapshot.Server.Savedir);

            var startInfo = CreateStartInfo(snapshot);

            using var process = new Process
            {
                StartInfo = startInfo
            };

            if (!process.Start())
                throw new InvalidOperationException(Messages.ProcessFailedToStart);

            _logger.Info(Messages.ProcessStarted);

            try
            {
                await process.WaitForExitAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            { 
                if (!process.HasExited)
                {
                    bool forceStop = _userInteractive.ConfirmForceStopServer();

                    // Пока диалог был открыт, сервер мог завершиться.
                    if (forceStop && !process.HasExited)
                        process.CloseMainWindow();
                }

                // При «Да» ждём завершения после Kill.
                // При «Нет» ждём ручного Ctrl+C.
                await process.WaitForExitAsync();
            }

            _logger.Info(string.Format(Messages.ProcessExitCode, process.ExitCode));

            return process.ExitCode;
        }
    }
}

