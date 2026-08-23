using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ValheimFTPSync.Extensions;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Test
{
    public class FtpServiceTest
    {

        Uri _realFtpUri = new Uri("ftp://192.168.1.1:21/sda1/Valheim/");
        NetworkCredential _credentials = new NetworkCredential();
        IAppLogger _appLogger;
        public FtpServiceTest()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var mock = new Mock<IAppLogger>();
            mock.Setup(x => x.Info(It.IsAny<string>())).Callback<string>(str => Console.WriteLine(str));
            _appLogger = mock.Object;
        }

        [Fact]
        public async Task TryConnect_RealNetwork_ShouldReturnTrue()
        {
            FtpClientFactory ftpClientFactory = new FtpClientFactory(_appLogger);
            var client = ftpClientFactory.CreateFtpClient(_realFtpUri.OriginalString, _credentials);
            Assert.NotNull(client);
            FtpService ftpService = new FtpService(client, _appLogger, new LocalFileService());
            await ftpService.TryConnectAsync();
        }

        [Fact/*(Skip = "Real File")*/]
        public async Task DownloadRealFiles_RealNetwork_ShouldReturnTrue()
        {
            FtpClientFactory ftpClientFactory = new FtpClientFactory(_appLogger);
            var client = ftpClientFactory.CreateFtpClient(_realFtpUri.OriginalString, _credentials);
            Assert.NotNull(client);
            IFtpService service = new FtpService(client, _appLogger, new LocalFileService());

            var dirAsync = await service.GetDirectoryInfoAsync("", CancellationToken.None);
            var dir = service.GetDirectoryInfo("", CancellationToken.None);
            Assert.Equivalent(dirAsync, dir);
            Progress<OperationProgress> progress = new Progress<OperationProgress>(x => Console.WriteLine("[{0}] {1}: {2}/{3}:{4}% bytes. Files:{5}/{6}:{7}%", x.Operation, x.CurrentFile, x.TransferProgress.TransferredBytes, x.TransferProgress.TotalBytes, x.TransferProgress.Percent, x.CompletedFiles, x.TotalFiles, x.OverallProgress));
            var localPath = "C:\\temp";
            await service.DownloadFilesAsync(dir.GetFilesInDirectoryRecursive().ToArray(), localPath, progress);
            Assert.NotEmpty(dir.Files);
            Assert.All(dir.GetFilesInDirectoryRecursive(), x => File.Exists(Path.Combine(localPath, x.RelativePath)));
        }

        [Fact]
        public async Task UploadRealFiles_RealNetwork_ShouldReturnTrue()
        {
            FtpClientFactory ftpClientFactory = new FtpClientFactory(_appLogger);
            var client = ftpClientFactory.CreateFtpClient(_realFtpUri.OriginalString, _credentials);
            Assert.NotNull(client);
            IFtpService service = new FtpService(client, _appLogger, new LocalFileService());
            var localPath = "C:\\temp\\";
            var localPathFile = "C:\\temp\\tratra\\fwfkwfw\\аыфафафц\\фафцафф\\mysupperFile.txt";
            var files = new[] { new FtpFileModel(Path.GetFileName(localPathFile), Path.GetRelativePath(localPath, localPathFile), DateTime.MinValue, 0) };
            Progress<OperationProgress> progress = new Progress<OperationProgress>(x => Console.WriteLine("[{0}] {1}: {2}/{3}:{4}% bytes. Files:{5}/{6}:{7}%", x.Operation, x.CurrentFile, x.TransferProgress.TransferredBytes, x.TransferProgress.TotalBytes, x.TransferProgress.Percent, x.CompletedFiles, x.TotalFiles, x.OverallProgress));
            await service.UploadFilesAsync(files, localPath, progress);
        }
    }
}