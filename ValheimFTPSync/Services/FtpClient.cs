using System;
using System.Collections.Generic;
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
        public bool TryConnect()
        {
            if (this.ListDirectory() != null)
            {
                //  Logger?.Info("Проверка успешна!");
                return true;
            }
            else
            {
                //   Logger?.Warning("Ошибка подключения к ftp...");
                return false;
            }
        }
        internal async Task<bool> TryConnectAsync()
        {
            return await this.ListDirectoryAsync() != null;
        }

        public DateTime GetDateTimeStamp(string absolutePath, CancellationToken cancellationToken)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.GetDateTimestamp, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            //if (!response.WelcomeMessage.Contains("230") && response.StatusCode != FtpStatusCode.FileStatus)
            //    throw new Exception("Неверный код ответа при запросе даты");
            return DateTime.SpecifyKind(response.LastModified.ToUniversalTime(), DateTimeKind.Local).ToUniversalTime();//на моём ftp почему то время смещается два раза...
        }

        public async Task<DateTime> GetDateTimeStampAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.GetDateTimestamp, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            return DateTime.SpecifyKind(response.LastModified.ToUniversalTime(), DateTimeKind.Local).ToUniversalTime();//на моём ftp почему то время смещается два раза...
        }

        public long GetFileSize(string absolutePath, CancellationToken cancellationToken = default)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest ftpWeb = FtpWebRequest.Create(absolutePath) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел
            ftpWeb.Method = WebRequestMethods.Ftp.GetFileSize;
            try
            {
                using (FtpWebResponse response = ftpWeb.GetResponse() as FtpWebResponse)
                {
                    if (!response.WelcomeMessage.Contains("230") && response.StatusCode != FtpStatusCode.FileStatus)
                        throw new Exception("Неверный код ответа при запросе даты");

                    return response.ContentLength;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<long> GetFileSizeAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void AppendFile(string absolutePath, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AppendFileAsync(string absolutePath, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string absolutePath, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void DownloadFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {

#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(absolutePath) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел
            ftpWeb.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpWeb.UseBinary = true;
            ftpWeb.ConnectionGroupName = "DownloadFTP";
            using FtpWebResponse? response = ftpWeb.GetResponse() as FtpWebResponse;
            if (response?.StatusDescription?.Contains("150") ?? false && response.StatusCode == FtpStatusCode.OpeningData)
            {
                using Stream streamResponse = response.GetResponseStream();
                //using FileStream writer = new FileStream(, FileMode.Create);

                // Получаем информацию о файле для сохранения даты создания
                DateTime? lastModified = null;
                if (response.LastModified != DateTime.MinValue)
                {
                    lastModified = response.LastModified;
                }


                var lenghtBytes = response.ContentLength;
                byte[] buffer = new byte[4096];

                int oldPrecent = 0;
                int numOfBytesRead = streamResponse.Read(buffer, 0, buffer.Length);
                long countBytes = numOfBytesRead;

                while (numOfBytesRead != 0)
                {
                    //writer.Write(buffer, 0, numOfBytesRead);
                    numOfBytesRead = streamResponse.Read(buffer, 0, buffer.Length);
                    int precent = (int)((countBytes += numOfBytesRead) * 100 / lenghtBytes);

                    if (precent != oldPrecent)
                    {
                        oldPrecent = precent;
                        ProgressChanged?.Invoke(this, new TransferProgress(precent, countBytes, lenghtBytes, Operation.Download));
                        //progressOneFile.Report(precent / 100f);
                    }

                }
                //writer.Flush();
                //writer.Close();
                //UpdateLocalFileAttrributeDateTime(tempPath, file.DateTimeChangedFile);
            }
        }

        public Task DownloadFileAsync(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IList<string> ListDirectory()
        {
            return ListDirectory("/");
        }

        public IList<string> ListDirectory(string absolutePath, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectory, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public IList<string> ListDirectoryDetails()
        {
            return ListDirectoryDetails("/");
        }

        public IList<string> ListDirectoryDetails(string absolutePath, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectoryDetails, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public async Task<IList<string>> ListDirectoryAsync()
        {
            return await ListDirectoryAsync("/");
        }

        public async Task<IList<string>> ListDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectory, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = await sr.ReadToEndAsync();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public async Task<IList<string>> ListDirectoryDetailsAsync()
        {
            return await ListDirectoryDetailsAsync("/");
        }

        public async Task<IList<string>> ListDirectoryDetailsAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.ListDirectoryDetails, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = await sr.ReadToEndAsync();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public void MakeDirectory(string absolutePath, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.MakeDirectory, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task MakeDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.MakeDirectory, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public void RemoveDirectory(string absolutePath, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.RemoveDirectory, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            /*Поэтому

Если у тебя после удаления:

response.WelcomeMessage

показывает что-то вроде:

250 File deleted successfully

я бы не использовал это как доказательство успешного удаления.

Посмотри именно:

response.StatusCode
response.StatusDescription
response.WelcomeMessage

и сравни их после DeleteFile.

Если покажешь, что конкретно выводится в этих трёх свойствах после удаления, я смогу сказать, почему твой конкретный FtpWebRequest кладёт 250 именно туда.*/
            if (!response.StatusDescription?.Contains("250") ?? false && response.StatusCode != FtpStatusCode.FileActionOK)
            {
                throw new Exception($"Error delete file \"{absolutePath}\".");
            }
        }

        public async Task RemoveDirectoryAsync(string absolutePath, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.RemoveDirectory, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
            if (!response.StatusDescription?.Contains("250") ?? false && response.StatusCode != FtpStatusCode.FileActionOK)
            {
                throw new Exception($"Error delete file \"{absolutePath}\".");
            }
        }

        public void Rename(string absolutePath, string name, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.Rename, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = request.GetResponse() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public async Task RenameAsync(string absolutePath, string name, CancellationToken cancellationToken = default)
        {
            FtpWebRequest request = CreateRequest(WebRequestMethods.Ftp.Rename, absolutePath, false);
            using var registration = cancellationToken.Register(request.Abort);
            using FtpWebResponse response = await request.GetResponseAsync() as FtpWebResponse ?? throw new InvalidOperationException("There is no response from the connection.");
        }

        public void UploadFile(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UploadFileAsync(string absolutePath, Stream stream, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private FtpWebRequest CreateRequest(string method, string? path = null, bool keepAlive = true)
        {
            var uri = path is null ? BaseFtpUri : new Uri(BaseFtpUri, path);

#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest request = FtpWebRequest.Create(uri) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел

            request.Method = method;
            if (Credentials is not null)
                request.Credentials = Credentials;

            request.UseBinary = true;
            request.KeepAlive = keepAlive;
            return request;
        }
    }

}
