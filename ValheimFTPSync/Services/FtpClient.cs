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
    internal class FtpClient : IFtpClient
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
        private NetworkCredential? Credentials { get; }


        /// <summary>
        /// Инициализация клиента из строки Uri.
        /// </summary>
        public FtpClient(string uri, NetworkCredential? credit = null) : this(new Uri(uri), credit) { }

        /// <summary>
        /// Инициализация клиента из Uri.
        /// </summary>
        public FtpClient(Uri uri, NetworkCredential? credit = null)
        {
            FtpUri = uri;
            Credentials = credit;
            if (string.IsNullOrEmpty(FtpUri.UserInfo) && credit is not null)
            {
                var builder = new UriBuilder(FtpUri);
                builder.UserName = credit.UserName; builder.Password = credit.Password;
                FtpUri = builder.Uri;
            }
            else if (!string.IsNullOrEmpty(FtpUri.UserInfo) && credit is null)
            {
                var userSplit = FtpUri.UserInfo.Split(':');
                Credentials = new NetworkCredential(userSplit.ElementAtOrDefault(0), userSplit.ElementAtOrDefault(1));
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

        private DateTime GetDate(string url)
        {
            FtpWebRequest ftpWeb = FtpWebRequest.Create(url) as FtpWebRequest;
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

        private long GetFileSize(string url)
        {
            FtpWebRequest ftpWeb = FtpWebRequest.Create(url) as FtpWebRequest;
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

        public void AppendFile(string uri, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string uri, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(string uri, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void DownloadFile(string uri, Stream stream, CancellationToken ct = default)
        {

#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(uri) as FtpWebRequest;
#pragma warning restore SYSLIB0014 // Тип или член устарел
            if (ftpWeb == null)
                throw new InvalidOperationException("FtpWebRequest cannot be created");
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

        public Task DownloadFileAsync(string uri, Stream stream, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTimestamp()
        {
            throw new NotImplementedException();
        }

        public float GetSizeFile()
        {
            throw new NotImplementedException();
        }

        public IList<string> ListDirectory()
        {
            return ListDirectory("/");
        }

        public IList<string> ListDirectory(string directoryPath)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, directoryPath)) as FtpWebRequest;
#pragma warning restore SYSLIB0014 // Тип или член устарел
            if (ftpWeb == null)
                throw new InvalidOperationException("It is not possible to create an Ftp connection. Invalid link.");

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

        public IList<string> ListDirectoryDetails(string directoryPath)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, directoryPath)) as FtpWebRequest;
#pragma warning restore SYSLIB0014 // Тип или член устарел
            if (ftpWeb == null)
                throw new InvalidOperationException("It is not possible to create an Ftp connection. Invalid link.");

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

        public async Task<IList<string>> ListDirectoryAsync(string directoryPath)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, directoryPath)) as FtpWebRequest;
#pragma warning restore SYSLIB0014 // Тип или член устарел
            if (ftpWeb == null)
                throw new InvalidOperationException("It is not possible to create an Ftp connection. Invalid link.");

            ftpWeb.Method = WebRequestMethods.Ftp.ListDirectory;

            using FtpWebResponse? response = await ftpWeb.GetResponseAsync() as FtpWebResponse;
            if (response == null)
                throw new InvalidOperationException("There is no response from the connection.");
            if (!response.WelcomeMessage?.Contains("230") ?? false && response.StatusCode != FtpStatusCode.OpeningData)
                throw new InvalidOperationException("Response status is invalid");

            using StreamReader sr = new StreamReader(response.GetResponseStream());
            string dataFiles = sr.ReadToEnd();
            return dataFiles.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public async Task<IList<string>> ListDirectoryDetailsAsync()
        {
            return await ListDirectoryDetailsAsync("/");
        }

        public async Task<IList<string>> ListDirectoryDetailsAsync(string directoryPath)
        {
#pragma warning disable SYSLIB0014 // Тип или член устарел
            FtpWebRequest? ftpWeb = FtpWebRequest.Create(new Uri(FtpUri, directoryPath)) as FtpWebRequest;
#pragma warning restore SYSLIB0014 // Тип или член устарел
            if (ftpWeb == null)
                throw new InvalidOperationException("It is not possible to create an Ftp connection. Invalid link.");

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

        public void MakeDirectory(string directoryPath, CancellationToken ct = default)
        {
            try
            {
#pragma warning disable SYSLIB0014 // Тип или член устарел
                FtpWebRequest? ftpWeb = FtpWebRequest.Create(directoryPath) as FtpWebRequest;
#pragma warning restore SYSLIB0014 // Тип или член устарел
                if (ftpWeb == null)
                    throw new InvalidOperationException("FtpWebRequest cannot be created.");
                ftpWeb.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpWeb.GetResponse();
            }
            catch (WebException wEx)
            {
                if (!wEx.Message.Contains("550"))
                    throw;
            }
        }

        public Task MakeDirectoryAsync(string DirectoryPath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void RemoveDirectory(string DirectoryPath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveDirectoryAsync(string DirectoryPath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void Rename(string uri, string name, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task RenameAsync(string uri, string name, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void UploadFile(string uri, Stream stream, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UploadFileAsync(string uri, Stream stream, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
