using System.Diagnostics.CodeAnalysis;
using System.Net;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class FtpClient : IFtpClientBase
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
        public bool Connect(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectory);
            cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            return true;
        }

        /// <summary>
        /// Пытается асснхронно подключиться к FTP и возвращает пользовательское сообщение о результате.
        /// </summary>
        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectory);
            cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            return true;
        }

        /// <summary>
        /// Получить дату сохранения файла на FTP в UTC.
        /// </summary>
        /// <param name="relativePath">Путь к файлу.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Дата сохранения файла на FTP в UTC.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public DateTime GetDateTimeStamp(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.GetDateTimestamp, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            var rawServerDateTime = DateTime.SpecifyKind(response.LastModified.ToUniversalTime(), DateTimeKind.Unspecified);
            var verifiDateTime = new DateTimeOffset(rawServerDateTime, _dateTimeOffset).UtcDateTime;
            return verifiDateTime;
        }

        public async Task<DateTime> GetDateTimeStampAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.GetDateTimestamp, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            var rawServerDateTime = DateTime.SpecifyKind(response.LastModified.ToUniversalTime(), DateTimeKind.Unspecified);
            var verifiDateTime = new DateTimeOffset(rawServerDateTime, _dateTimeOffset).UtcDateTime;
            return verifiDateTime;
        }

        /// <summary>
        /// Получает размер файла по полному пути через протокол FTP.
        /// </summary>
        /// <param name="relativePath">Путь к файлу.</param>
        /// <param name="cancellationToken">Токен для обработки отмены операции.</param>
        /// <returns>Размер файла в байтах.</returns>
        public long GetFileSize(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.GetFileSize, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            return response.ContentLength;
        }

        /// <summary>
        /// Получает размер файла по полному пути через протокол FTP асинхронно.
        /// </summary>
        /// <param name="relativePath">Путь к файлу.</param>
        /// <param name="cancellationToken">Токен для обработки отмены операции.</param>
        /// <returns>Размер файла в байтах.</returns>
        public async Task<long> GetFileSizeAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.GetFileSize, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            return response.ContentLength;
        }

        public void AppendFile(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(relativePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Input stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.AppendFile, relativePath);
            using var registration = cancellationToken.Register(request.Abort);

            using Stream requestStream = request.GetRequestStream() ?? throw new InvalidOperationException("Request stream is null.");

            long totalBytes = stream.Length;

            byte[] buffer = new byte[4096];
            long bytesWritten = 0;
            int readBytes;

            while ((readBytes = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                requestStream.Write(buffer, 0, readBytes);
                bytesWritten += readBytes;

                var transferProgress = new TransferProgress(bytesWritten, totalBytes);
                progress?.Report(transferProgress);
                ProgressChanged?.Invoke(this, transferProgress);
            }
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("FTP request returned an invalid response.");
        }

        public async Task AppendFileAsync(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(relativePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Input stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.AppendFile, relativePath);
            using var registration = cancellationToken.Register(request.Abort);

            await using Stream requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("Request stream is null.");

            long totalBytes = stream.Length;

            byte[] buffer = new byte[4096];
            long bytesWritten = 0;
            int readBytes;

            while ((readBytes = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await requestStream.WriteAsync(buffer, 0, readBytes, cancellationToken).ConfigureAwait(false);
                bytesWritten += readBytes;

                var transferProgress = new TransferProgress(bytesWritten, totalBytes);
                progress?.Report(transferProgress);
                ProgressChanged?.Invoke(this, transferProgress);
            }
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("FTP request returned an invalid response.");
        }

        public void DeleteFile(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.DeleteFile, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.DeleteFile, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public void DownloadFile(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(relativePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Destination stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.DownloadFile, relativePath);
            request.KeepAlive = true;
            request.ConnectionGroupName = "DownloadFTP";
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using var responseStream = response.GetResponseStream() ?? throw new InvalidOperationException("Response stream is null.");
            long totalBytes = response.ContentLength;

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

        public async Task DownloadFileAsync(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(relativePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Destination stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.DownloadFile, relativePath);
            request.KeepAlive = true;
            request.ConnectionGroupName = "DownloadFTP";
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using var responseStream = response.GetResponseStream() ?? throw new InvalidOperationException("Response stream is null.");
            long totalBytes = response.ContentLength;

            const int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            long bytesWritten = 0;
            while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await stream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                bytesWritten += bytesRead;
                var transferProgress = new TransferProgress(bytesWritten, totalBytes);
                progress?.Report(transferProgress);
                ProgressChanged?.Invoke(this, transferProgress);
            }
        }

        public IEnumerable<string> ListDirectory(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectory, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(x => Uri.UnescapeDataString(_baseFtpUri.MakeRelativeUri(new Uri(_baseFtpUri, x)).OriginalString));
        }

        public IEnumerable<string> ListDirectoryDetails(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectoryDetails, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task<IEnumerable<string>> ListDirectoryAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectory, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = await sr.ReadToEndAsync().ConfigureAwait(false);
            return dataFiles.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(x => Uri.UnescapeDataString(_baseFtpUri.MakeRelativeUri(new Uri(_baseFtpUri, x)).OriginalString));
        }

        public async Task<IEnumerable<string>> ListDirectoryDetailsAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectoryDetails, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = await sr.ReadToEndAsync().ConfigureAwait(false);
            return dataFiles.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        }

        public void MakeDirectory(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.MakeDirectory, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task MakeDirectoryAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.MakeDirectory, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public void RemoveDirectory(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.RemoveDirectory, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task RemoveDirectoryAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.RemoveDirectory, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public void Rename(string relativePath, string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.Rename, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            request.RenameTo = name;
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task RenameAsync(string relativePath, string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.Rename, relativePath);
            using var registration = cancellationToken.Register(request.Abort);
            request.RenameTo = name;
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public void UploadFile(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(relativePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Input stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.UploadFile, relativePath);
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

        public async Task UploadFileAsync(string relativePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Absolute path cannot be null or empty.", nameof(relativePath));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), "Input stream cannot be null.");
            cancellationToken.ThrowIfCancellationRequested();

            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.UploadFile, relativePath);
            request.KeepAlive = true;
            request.ConnectionGroupName = "UploadFTP";

            var bytesLength = request.ContentLength = stream.Length;
            await using Stream streamRequest = await request.GetRequestStreamAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("Request stream is null.");

            byte[] buffer = new byte[4096];
            long bytesWritten = 0;
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await streamRequest.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                bytesWritten += bytesRead;
                var transfer = new TransferProgress(bytesWritten, bytesLength);
                progress?.Report(transfer);
                ProgressChanged?.Invoke(this, transfer);
            }
            using FtpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
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
