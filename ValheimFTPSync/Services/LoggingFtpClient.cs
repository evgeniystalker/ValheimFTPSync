using System.Net;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class LoggingFtpClient : IFtpClientBase
    {
        readonly IAppLogger _logger;
        readonly IFtpClientBase _inner;

        public event EventHandler<TransferProgress> ProgressChanged
        {
            add => _inner?.ProgressChanged += value;
            remove => _inner?.ProgressChanged -= value;
        }


        public LoggingFtpClient(IFtpClientBase ftpClient, IAppLogger appLogger)
        {
            _logger = appLogger;
            _inner = ftpClient;
        }

        public T? Execute<T>(string operation, Func<T> action)
        {
            try
            {
                return action();
            }
            catch (OperationCanceledException)
            {
                _logger.Info($"Operation cancelled: {operation}");
                return default;
            }
            catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse && ftpResponse.StatusCode == FtpStatusCode.NotLoggedIn)
            {
                _logger.Warning($"Authentication error in {operation}!");
                ftpResponse.Close();
                ftpResponse.Dispose();
                return default;
            }
            catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse)
            {
                var statusCode = ftpResponse.StatusCode;
                ftpResponse.Close();
                ftpResponse.Dispose();
                _logger.Error($"Ftp error in '{operation}' (status: {statusCode}) : {ex.Message}");
                return default;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error in '{operation}': {ex.Message}");
                return default;
            }
        }

        public void Execute(string operation, Action action)
        {
            Execute<object>(operation, () => { action(); return null!; });
        }

        private async Task<T?> ExecuteAsync<T>(string operation, Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (OperationCanceledException)
            {
                _logger.Info($"Operation cancelled: {operation}");
                return default;
            }
            catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse && ftpResponse.StatusCode == FtpStatusCode.NotLoggedIn)
            {
                _logger.Warning($"Authentication error in {operation}!");
                ftpResponse.Close();
                ftpResponse.Dispose();
                return default;
            }
            catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse)
            {
                var statusCode = ftpResponse.StatusCode;
                ftpResponse.Close();
                ftpResponse.Dispose();
                _logger.Error($"Ftp error in '{operation}' (status: {statusCode}) : {ex.Message}");
                return default;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error in '{operation}': {ex.Message}");
                return default;
            }
        }

        private async Task ExecuteAsync(string operation, Func<Task> action)
        {
            await ExecuteAsync<object>(operation, async () => { await action(); return null!; });
        }

        public async Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync($"{nameof(DeleteFileAsync)}({relativePath})", () => _inner.DeleteFileAsync(relativePath, cancellationToken));
        }

        public async Task<IEnumerable<string>> ListDirectoryAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync($"{nameof(ListDirectoryAsync)}({relativePath})", () => _inner.ListDirectoryAsync(relativePath, cancellationToken)) ?? Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<string>> ListDirectoryDetailsAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync($"{nameof(ListDirectoryDetailsAsync)}({relativePath})", () => _inner.ListDirectoryDetailsAsync(relativePath, cancellationToken)) ?? Enumerable.Empty<string>(); ;
        }

        public async Task MakeDirectoryAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync($"{nameof(MakeDirectoryAsync)}({relativePath})", () => _inner.MakeDirectoryAsync(relativePath, cancellationToken));
        }

        public async Task DownloadFileAsync(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync($"{nameof(DownloadFileAsync)}({relativePath})", () => _inner.DownloadFileAsync(relativePath, stream, progress, cancellationToken));
        }

        public async Task UploadFileAsync(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync($"{nameof(UploadFileAsync)}({relativePath})", () => _inner.UploadFileAsync(relativePath, stream, progress, cancellationToken));
        }

        public async Task AppendFileAsync(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync($"{nameof(AppendFileAsync)}({relativePath})", () => _inner.AppendFileAsync(relativePath, stream, progress, cancellationToken));
        }

        public async Task<DateTime> GetDateTimeStampAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync($"{nameof(GetDateTimeStampAsync)}({relativePath})", () => _inner.GetDateTimeStampAsync(relativePath, cancellationToken));
        }

        public async Task<long> GetFileSizeAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync($"{nameof(GetFileSizeAsync)}({relativePath})", () => _inner.GetFileSizeAsync(relativePath, cancellationToken));
        }

        public async Task RemoveDirectoryAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync($"{nameof(RemoveDirectoryAsync)}({relativePath})", () => _inner.RemoveDirectoryAsync(relativePath, cancellationToken));
        }

        public async Task RenameAsync(string relativePath, string name, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync($"{nameof(RenameAsync)}({relativePath})", () => _inner.RenameAsync(relativePath, name, cancellationToken));
        }

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync($"{nameof(ConnectAsync)}", () => _inner.ConnectAsync(cancellationToken));
        }

        public void AppendFile(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(AppendFile)}({relativePath})", () => _inner.AppendFile(relativePath, stream, progress, cancellationToken));
        }

        public void DeleteFile(string relativePath, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(DeleteFile)}({relativePath})", () => _inner.DeleteFile(relativePath, cancellationToken));
        }

        public void DownloadFile(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(DownloadFile)}({relativePath})", () => _inner.DownloadFile(relativePath, stream, progress, cancellationToken));
        }

        public DateTime GetDateTimeStamp(string relativePath, CancellationToken cancellationToken = default)
        {
            return Execute($"{nameof(GetDateTimeStamp)}({relativePath})", () => _inner.GetDateTimeStamp(relativePath, cancellationToken));
        }

        public long GetFileSize(string relativePath, CancellationToken cancellationToken = default)
        {
            return Execute($"{nameof(GetFileSize)}({relativePath})", () => _inner.GetFileSize(relativePath, cancellationToken));
        }

        public IEnumerable<string> ListDirectory(string relativePath, CancellationToken cancellationToken = default)
        {
            return Execute($"{nameof(ListDirectory)}({relativePath})", () => _inner.ListDirectory(relativePath, cancellationToken)) ?? Enumerable.Empty<string>(); ;
        }

        public IEnumerable<string> ListDirectoryDetails(string relativePath, CancellationToken cancellationToken = default)
        {
            return Execute($"{nameof(ListDirectoryDetails)}({relativePath})", () => _inner.ListDirectoryDetails(relativePath, cancellationToken)) ?? Enumerable.Empty<string>(); ;
        }

        public void MakeDirectory(string relativePath, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(MakeDirectory)}({relativePath})", () => _inner.MakeDirectory(relativePath, cancellationToken));
        }

        public void RemoveDirectory(string relativePath, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(RemoveDirectory)}({relativePath})", () => _inner.RemoveDirectory(relativePath, cancellationToken));
        }

        public void Rename(string relativePath, string name, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(Rename)}({relativePath})", () => _inner.Rename(relativePath, name, cancellationToken));
        }

        public void UploadFile(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(UploadFile)}({relativePath})", () => _inner.UploadFile(relativePath, stream, progress, cancellationToken));
        }

        public bool Connect(CancellationToken cancellationToken = default)
        {
            return Execute($"{nameof(Connect)}", () => _inner.Connect(cancellationToken));
        }
    }
}
