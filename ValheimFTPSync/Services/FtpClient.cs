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
        private Uri _ftpUri;

        public event EventHandler<ProgressEventArgs>? ProgressChanged;

        private Uri FtpUri
        {
            get => _ftpUri;
            [MemberNotNull(nameof(_ftpUri))]
            set
            {
                if (!value.Scheme.Equals(Uri.UriSchemeFtp, StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"Схема ({value.Scheme}) не соответствует ftp://");
                _ftpUri = value;
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
            FtpUri = uri;
            Credentials = credit;
            if (credit is not null && !string.IsNullOrEmpty(FtpUri.UserInfo))
            {
                var builder = new UriBuilder(FtpUri);
                builder.UserName = string.Empty;
                builder.Password = string.Empty;
                FtpUri = builder.Uri;
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

        public DateTime GetDateTimeStamp(string absolutePath)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest ftpWeb = FtpWebRequest.Create(absolutePath) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел
            ftpWeb.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            try
            {
                using (FtpWebResponse response = ftpWeb.GetResponse() as FtpWebResponse)
                {
                    if (!response.WelcomeMessage.Contains("230") && response.StatusCode != FtpStatusCode.FileStatus)
                        throw new Exception("Неверный код ответа при запросе даты");

                    return DateTime.SpecifyKind(response.LastModified.ToUniversalTime(), DateTimeKind.Local).ToUniversalTime();//на моём ftp почему то время смещается два раза...
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<DateTime> GetDateTimeStampAsync(string absolutePath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public long GetFileSize(string absolutePath)
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

        public Task<long> GetFileSizeAsync(string absolutePath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void AppendFile(string absolutePath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task AppendFileAsync(string absolutePath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string absolutePath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(string absolutePath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void DownloadFile(string absolutePath, Stream stream, CancellationToken ct = default)
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
                        ProgressChanged?.Invoke(this, new ProgressEventArgs(precent, countBytes, lenghtBytes, Operation.Download));
                        //progressOneFile.Report(precent / 100f);
                    }

                }
                //writer.Flush();
                //writer.Close();
                //UpdateLocalFileAttrributeDateTime(tempPath, file.DateTimeChangedFile);
            }
        }

        public Task DownloadFileAsync(string absolutePath, Stream stream, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public IList<string> ListDirectory()
        {
            return ListDirectory("/");
        }

        public IList<string> ListDirectory(string absolutePath)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, absolutePath)) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел
            if (Credentials is not null)
                ftpWeb.Credentials = Credentials;
            ftpWeb.Method = WebRequestMethods.Ftp.ListDirectory;

            using FtpWebResponse? response = ftpWeb.GetResponse() as FtpWebResponse;
            if (response == null)
                throw new InvalidOperationException("There is no response from the connection.");
            if (!response.WelcomeMessage?.Contains("230") ?? false && response.StatusCode != FtpStatusCode.OpeningData)
                throw new InvalidOperationException("Response status is invalid");

            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public IList<string> ListDirectoryDetails()
        {
            return ListDirectoryDetails("/");
        }

        public IList<string> ListDirectoryDetails(string absolutePath)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, absolutePath)) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел

            ftpWeb.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            using FtpWebResponse? response = ftpWeb.GetResponse() as FtpWebResponse;
            if (response == null)
                throw new InvalidOperationException("There is no response from the connection.");
            if (!response.WelcomeMessage?.Contains("230") ?? false && response.StatusCode != FtpStatusCode.OpeningData)
                throw new InvalidOperationException("Response status is invalid");

            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public async Task<IList<string>> ListDirectoryAsync()
        {
            return await ListDirectoryAsync("/");
        }

        public async Task<IList<string>> ListDirectoryAsync(string absolutePath)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, absolutePath)) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел

            ftpWeb.Method = WebRequestMethods.Ftp.ListDirectory;

            if (Credentials is not null)
                ftpWeb.Credentials = Credentials;

            using FtpWebResponse? response = await ftpWeb.GetResponseAsync() as FtpWebResponse;
            if (response == null)
                throw new InvalidOperationException("There is no response from the connection.");
            if (!(response.WelcomeMessage?.Contains("230") ?? false) && response.StatusCode != FtpStatusCode.OpeningData)
                throw new WebException($"Invalid FTP server response({response.StatusCode}) for operation \"{FtpStatusCode.OpeningData}\".", WebExceptionStatus.ProtocolError);

            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public async Task<IList<string>> ListDirectoryDetailsAsync()
        {
            return await ListDirectoryDetailsAsync("/");
        }

        public async Task<IList<string>> ListDirectoryDetailsAsync(string absolutePath)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, absolutePath)) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел

            ftpWeb.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            using FtpWebResponse? response = await ftpWeb.GetResponseAsync() as FtpWebResponse;
            if (response == null)
                throw new InvalidOperationException("There is no response from the connection.");
            if (!response.WelcomeMessage?.Contains("230") ?? false && response.StatusCode != FtpStatusCode.OpeningData)
                throw new InvalidOperationException("Response status is invalid");

            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public void MakeDirectory(string absolutePath, CancellationToken ct = default)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, absolutePath)) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел
            ftpWeb.Method = WebRequestMethods.Ftp.MakeDirectory;
            using FtpWebResponse? response = ftpWeb.GetResponse() as FtpWebResponse;
            if (response is null)
                throw new WebException("Response status is invalid.");
        }

        public async Task MakeDirectoryAsync(string absolutePath, CancellationToken ct = default)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, absolutePath)) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел
            ftpWeb.Method = WebRequestMethods.Ftp.MakeDirectory;
            using FtpWebResponse? response = await ftpWeb.GetResponseAsync() as FtpWebResponse;
            if (response is null)
                throw new WebException("Response status is invalid.");
        }

        public void RemoveDirectory(string absolutePath, CancellationToken ct = default)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, absolutePath)) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел
            ftpWeb.Method = WebRequestMethods.Ftp.RemoveDirectory;
            using FtpWebResponse? response = ftpWeb.GetResponse() as FtpWebResponse;
            if (response is null)
                throw new WebException("Response status is invalid.");

            if (!response.StatusDescription?.Contains("250") ?? false && response.StatusCode != FtpStatusCode.FileActionOK)
            {
                throw new Exception($"Error delete file \"{absolutePath}\".");
            }
            response.Close();
        }

        public async Task RemoveDirectoryAsync(string absolutePath, CancellationToken ct = default)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, absolutePath)) as FtpWebRequest ?? throw new InvalidOperationException("It isn't possible to create an Ftp connection. Invalid link.");
#pragma warning restore SYSLIB0014 // Тип или член устарел
            ftpWeb.Method = WebRequestMethods.Ftp.MakeDirectory;
            using FtpWebResponse? response = await ftpWeb.GetResponseAsync() as FtpWebResponse;
            if (response is null)
                throw new WebException("Response status is invalid.");

            if (!response.StatusDescription?.Contains("250") ?? false && response.StatusCode != FtpStatusCode.FileActionOK)
            {
                throw new Exception($"Error delete file \"{absolutePath}\".");
            }
            response.Close();
        }

        public void Rename(string absolutePath, string name, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task RenameAsync(string absolutePath, string name, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void UploadFile(string absolutePath, Stream stream, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UploadFileAsync(string absolutePath, Stream stream, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

    }
}
