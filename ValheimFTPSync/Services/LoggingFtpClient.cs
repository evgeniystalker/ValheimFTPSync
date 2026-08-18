using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class LoggingFtpClient : IFtpClientAsync, IFtpClient
    {
        IAppLogger _logger;
        FtpClient _inner;

        public event EventHandler<TransferProgress> ProgressChanged
        {
            add => _inner?.ProgressChanged += value;
            remove => _inner?.ProgressChanged -= value;
        }


        public LoggingFtpClient(FtpClient ftpClient, IAppLogger appLogger)
        {
            _logger = appLogger;
            _inner = ftpClient;
        }

        public T Execute<T>(string operation, Func<T> action)
        {
            try
            {
                return action();
            }
            catch (OperationCanceledException)
            {
                _logger.Info($"Operation cancelled: {operation}");
                throw;
            }
            catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse)
            {
                var statusCode = ftpResponse.StatusCode;
                ftpResponse.Close();
                ftpResponse.Dispose();
                _logger.Error($"FTP error in '{operation}' (status: {statusCode}) : {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in '{operation}': {ex.Message}");
                throw;
            }
        }

        public void Execute(string operation, Action action)
        {
            Execute<object>(operation, () => { action(); return null!; });
        }

        private async Task<T> ExecuteAsync<T>(string operation, Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (OperationCanceledException)
            {
                _logger.Info($"Operation cancelled: {operation}");
                throw;
            }
            catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse)
            {
                var statusCode = ftpResponse.StatusCode;
                ftpResponse.Close();
                ftpResponse.Dispose();
                _logger.Error($"FTP error in '{operation}' (status: {statusCode}) : {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in '{operation}': {ex.Message}");
                throw;
            }
        }

        private async Task ExecuteAsync(string operation, Func<Task> action)
        {
            await ExecuteAsync<object>(operation, async () => { await action(); return null!; });
        }

        public async Task DeleteFileAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync(
                $"{nameof(DeleteFileAsync)}({absolutePath})",
                () => _inner.DeleteFileAsync(absolutePath, cancellationToken));
        }

        public async Task<IEnumerable<string>> ListDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(
                $"{nameof(ListDirectoryAsync)}({absolutePath})",
                () => _inner.ListDirectoryAsync(absolutePath, cancellationToken));
        }

        public async Task<IEnumerable<string>> ListDirectoryDetailsAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(
                $"{nameof(ListDirectoryDetailsAsync)}({absolutePath})",
                () => _inner.ListDirectoryDetailsAsync(absolutePath, cancellationToken));
        }

        public async Task MakeDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync(
                $"{nameof(MakeDirectoryAsync)}({absolutePath})",
                () => _inner.MakeDirectoryAsync(absolutePath, cancellationToken));
        }

        public async Task DownloadFileAsync(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync(
                $"{nameof(DownloadFileAsync)}({absolutePath})",
                () => _inner.DownloadFileAsync(absolutePath, stream, progress, cancellationToken));
        }

        public async Task UploadFileAsync(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync(
                $"{nameof(UploadFileAsync)}({absolutePath})",
                () => _inner.UploadFileAsync(absolutePath, stream, progress, cancellationToken));
        }

        public async Task AppendFileAsync(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync(
                 $"{nameof(AppendFileAsync)}({absolutePath})",
                 () => _inner.AppendFileAsync(absolutePath, stream, progress, cancellationToken));
        }

        public async Task<DateTime> GetDateTimeStampAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(
                 $"{nameof(GetDateTimeStampAsync)}({absolutePath})",
                 () => _inner.GetDateTimeStampAsync(absolutePath, cancellationToken));
        }

        public async Task<long> GetFileSizeAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(
                 $"{nameof(GetFileSizeAsync)}({absolutePath})",
                 () => _inner.GetFileSizeAsync(absolutePath, cancellationToken));
        }

        public async Task RemoveDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync(
                 $"{nameof(RemoveDirectoryAsync)}({absolutePath})",
                 () => _inner.RemoveDirectoryAsync(absolutePath, cancellationToken));
        }

        public async Task RenameAsync(string absolutePath, string name, CancellationToken cancellationToken = default)
        {
            await ExecuteAsync(
                 $"{nameof(RenameAsync)}({absolutePath})",
                 () => _inner.RenameAsync(absolutePath, name, cancellationToken));
        }

        public void AppendFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(AppendFile)}({absolutePath})", () => _inner.AppendFile(absolutePath, stream, progress, cancellationToken));
        }

        public void DeleteFile(string absolutePath, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(DeleteFile)}({absolutePath})", () => _inner.DeleteFile(absolutePath, cancellationToken));
        }

        public void DownloadFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(DownloadFile)}({absolutePath})", () => _inner.DownloadFile(absolutePath, stream, progress, cancellationToken));
        }

        public DateTime GetDateTimeStamp(string absolutePath, CancellationToken cancellationToken = default)
        {
            return Execute($"{nameof(GetDateTimeStamp)}({absolutePath})", () => _inner.GetDateTimeStamp(absolutePath, cancellationToken));
        }

        public long GetFileSize(string absolutePath, CancellationToken cancellationToken = default)
        {
            return Execute($"{nameof(GetFileSize)}({absolutePath})", () => _inner.GetFileSize(absolutePath, cancellationToken));
        }

        public IEnumerable<string> ListDirectory(string absolutePath, CancellationToken cancellationToken = default)
        {
            return Execute($"{nameof(ListDirectory)}({absolutePath})", () => _inner.ListDirectory(absolutePath, cancellationToken));
        }

        public IEnumerable<string> ListDirectoryDetails(string absolutePath, CancellationToken cancellationToken = default)
        {
            return Execute($"{nameof(ListDirectoryDetails)}({absolutePath})", () => _inner.ListDirectoryDetails(absolutePath, cancellationToken));
        }

        public void MakeDirectory(string absolutePath, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(MakeDirectory)}({absolutePath})", () => _inner.MakeDirectory(absolutePath, cancellationToken));
        }

        public void RemoveDirectory(string absolutePath, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(RemoveDirectory)}({absolutePath})", () => _inner.RemoveDirectory(absolutePath, cancellationToken));
        }

        public void Rename(string absolutePath, string name, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(Rename)}({absolutePath})", () => _inner.Rename(absolutePath, name, cancellationToken));
        }

        public void UploadFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            Execute($"{nameof(UploadFile)}({absolutePath})", () => _inner.UploadFile(absolutePath, stream, progress, cancellationToken));
        }
    }
}
