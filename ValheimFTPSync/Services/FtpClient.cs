using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Policy;
using System.Text;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class FtpClient : IFtpClient, IFtpClientAsync
    {
        private Uri _baseFtpUri;
        private TimeSpan _dateTimeOffset = TimeSpan.FromHours(-10);
        public event EventHandler<TransferProgress>? ProgressChanged;

        private Uri BaseFtpUri
        {
            get => _baseFtpUri;
            [MemberNotNull(nameof(_baseFtpUri))]
            set
            {
                if (!value.Scheme.Equals(Uri.UriSchemeFtp, StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"Схема ({value.Scheme}) не соответствует ftp://");
                _baseFtpUri = value;
            }
        }
        private ICredentials? Credentials { get; }


        /// <summary>
        /// Инициализация клиента из строки Uri.
        /// </summary>
        public FtpClient(string uri, ICredentials? credit = null) : this(new Uri(uri), credit) { }

        /// <summary>
        /// Инициализация клиента из Uri.
        /// </summary>
        public FtpClient(Uri uri, ICredentials? credit = null)
        {
            BaseFtpUri = uri;
            Credentials = credit;
            if (credit is not null && !string.IsNullOrEmpty(BaseFtpUri.UserInfo))
            {
                var builder = new UriBuilder(BaseFtpUri);
                builder.UserName = string.Empty;
                builder.Password = string.Empty;
                BaseFtpUri = builder.Uri;
            }
        }

        /// <summary>
        /// Пытается подключиться к FTP и возвращает пользовательское сообщение о результате.
        /// </summary>
        public bool TryConnect() => this.ListDirectory() != null;

        /// <summary>
        /// Пытается асснхронно подключиться к FTP и возвращает пользовательское сообщение о результате.
        /// </summary>
        internal async Task<bool> TryConnectAsync() => await this.ListDirectoryAsync() != null;

        /// <summary>
        /// Получить дату сохранения файла на FTP в UTC.
        /// </summary>
        /// <param name="absolutePath">Путь к файлу.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Дата сохренения файла на FTP в UTC.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public DateTime GetDateTimeStamp(string absolutePath, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.GetDateTimestamp, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            var rawServerDateTime = DateTime.SpecifyKind(response.LastModified.ToUniversalTime(), DateTimeKind.Unspecified);
            var verifiDateTime = new DateTimeOffset(rawServerDateTime, _dateTimeOffset).UtcDateTime;
            return verifiDateTime;
        }

        public async Task<DateTime> GetDateTimeStampAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.GetDateTimestamp, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            var rawServerDateTime = DateTime.SpecifyKind(response.LastModified.ToUniversalTime(), DateTimeKind.Unspecified);
            var verifiDateTime = new DateTimeOffset(rawServerDateTime, _dateTimeOffset).UtcDateTime;
            return verifiDateTime;
        }

        public long GetFileSize(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.GetFileSize, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            return response.ContentLength;
        }

        public async Task<long> GetFileSizeAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.GetFileSize, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            return response.ContentLength;
        }

        public void AppendFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(absolutePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Input stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.AppendFile, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);

            using Stream requestStream = request.GetRequestStream() ?? throw new InvalidOperationException("Request stream is null.");

            long totalBytes = stream.Length;

            byte[] buffer = new byte[4096];
            long bytesWritten = 0;
            int readBytes;

            while ((readBytes = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                requestStream.Write(buffer, 0, buffer.Length);
                bytesWritten += readBytes;

                var transferProgress = new TransferProgress(bytesWritten, totalBytes);
                progress?.Report(transferProgress);
                ProgressChanged?.Invoke(this, transferProgress);
            }
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("FTP request returned an invalid response.");
        }

        public async Task AppendFileAsync(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(absolutePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Input stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.AppendFile, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);

            await using Stream requestStream = await request.GetRequestStreamAsync() ?? throw new InvalidOperationException("Request stream is null.");

            long totalBytes = stream.Length;

            byte[] buffer = new byte[4096];
            long bytesWritten = 0;
            int readBytes;

            while ((readBytes = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await requestStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
                bytesWritten += readBytes;

                var transferProgress = new TransferProgress(bytesWritten, totalBytes);
                progress?.Report(transferProgress);
                ProgressChanged?.Invoke(this, transferProgress);
            }
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("FTP request returned an invalid response.");
        }

        public void DeleteFile(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.DeleteFile, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task DeleteFileAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.DeleteFile, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public void DownloadFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(absolutePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Destination stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectory, absolutePath);
            request.KeepAlive = true;
            request.ConnectionGroupName = "DownloadFTP";
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using var responseStream = response.GetResponseStream() ?? throw new InvalidOperationException("Response stream is null.");
            long totalBytes = responseStream.Length;

            const int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            long bytesWritten = 0;
            while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                stream.Write(buffer, 0, bytesRead);
                bytesWritten += bytesRead;
                var transferProgress = new TransferProgress(bytesWritten, totalBytes);
                progress?.Report(transferProgress);
                ProgressChanged?.Invoke(this, transferProgress);
            }
        }

        public async Task DownloadFileAsync(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(absolutePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Destination stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectory, absolutePath);
            request.KeepAlive = true;
            request.ConnectionGroupName = "DownloadFTP";
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using var responseStream = response.GetResponseStream() ?? throw new InvalidOperationException("Response stream is null.");
            long totalBytes = responseStream.Length;

            const int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            long bytesWritten = 0;
            while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await stream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                bytesWritten += bytesRead;
                var transferProgress = new TransferProgress(bytesWritten, totalBytes);
                progress?.Report(transferProgress);
                ProgressChanged?.Invoke(this, transferProgress);
            }
        }

        public IEnumerable<string> ListDirectory()
        {
            return ListDirectory("/");
        }

        public IEnumerable<string> ListDirectory(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectory, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }

        public IEnumerable<string> ListDirectoryDetails()
        {
            return ListDirectoryDetails("/");
        }

        public IEnumerable<string> ListDirectoryDetails(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectoryDetails, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task<IEnumerable<string>> ListDirectoryAsync()
        {
            return await ListDirectoryAsync("/");
        }

        public async Task<IEnumerable<string>> ListDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectory, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = await sr.ReadToEndAsync();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task<IEnumerable<string>> ListDirectoryDetailsAsync()
        {
            return await ListDirectoryDetailsAsync("/");
        }

        public async Task<IEnumerable<string>> ListDirectoryDetailsAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectoryDetails, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = await sr.ReadToEndAsync();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }

        public void MakeDirectory(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.MakeDirectory, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task MakeDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.MakeDirectory, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public void RemoveDirectory(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.RemoveDirectory, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task RemoveDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.RemoveDirectory, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public void Rename(string absolutePath, string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.Rename, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task RenameAsync(string absolutePath, string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.Rename, absolutePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public void UploadFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(absolutePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Input stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.UploadFile, absolutePath);
            request.KeepAlive = true;
            request.ConnectionGroupName = "UploadFTP";

            var bytesLength = request.ContentLength = stream.Length;
            using Stream streamRequest = request.GetRequestStream() ?? throw new InvalidOperationException("Request stream is null.");

            byte[] buffer = new byte[4096];
            long bytesWritten = 0;
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                streamRequest.Write(buffer, 0, bytesRead);
                bytesWritten += bytesRead;
                var transfer = new TransferProgress(bytesWritten, bytesLength);
                progress?.Report(transfer);
                ProgressChanged?.Invoke(this, transfer);
            }
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task UploadFileAsync(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(absolutePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Input stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.UploadFile, absolutePath);
            request.KeepAlive = true;
            request.ConnectionGroupName = "UploadFTP";

            var bytesLength = request.ContentLength = stream.Length;
            await using Stream streamRequest = await request.GetRequestStreamAsync() ?? throw new InvalidOperationException("Request stream is null.");

            byte[] buffer = new byte[4096];
            long bytesWritten = 0;
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await streamRequest.WriteAsync(buffer, 0, bytesRead);
                bytesWritten += bytesRead;
                var transfer = new TransferProgress(bytesWritten, bytesLength);
                progress?.Report(transfer);
                ProgressChanged?.Invoke(this, transfer);
            }
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        private FtpWebRequest CreateRequest(string method, string? path = null)
        {
            var uri = path is null ? BaseFtpUri : new Uri(BaseFtpUri, path);

#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest request = FtpWebRequest.Create(uri) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел

            request.Method = method;
            if (Credentials is not null)
                request.Credentials = Credentials;

            request.UseBinary = true;
            return request;
        }
    }

}
