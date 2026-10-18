using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Services
{
    internal sealed class GameSessionManager
    {
        private CancellationTokenSource? _cancellationTokenSource;

        public bool IsRunning => _cancellationTokenSource is not null;

        public async Task RunAsync()
        {
            if (IsRunning)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Здесь будет работа игровой сессии.
                await Task.Delay(10000, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) when (_cancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(3000);
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

