using Moq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System;
using System.IO;
using ValheimFTPSync.Services;
using System.Text;

namespace ValheimFTPSync.Test
{
    public class FtpClientTests
    {

        Uri realFtpUri = new Uri("ftp://192.168.1.1:21/sda1/Valheim/");
        NetworkCredential credentials = new NetworkCredential();
        public FtpClientTests()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }
        [Fact]
        public async Task GetDateTimeStampAsync_RealNetwork_ShouldReturnValidTime()
        {
            var client = new FtpClient(realFtpUri, credentials);

            // Имя реально существующего файла в папке Valheim на FTP для проверки
            // (Замените "server.cfg", если там лежит другой файл, например "world.db")
            string realFileName = "Player-prev.log";

            // 2. Act: Выполняем реальный запрос в вашу сеть
            DateTime actualTime = await client.GetDateTimeStampAsync(realFileName, CancellationToken.None);

            // 3. Assert: Проверяем базовые свойства вернувшейся даты
            // Мы не знаем точную дату, но можем проверить, что она успешно прочитана
            Assert.NotEqual(DateTime.MinValue, actualTime);
            Assert.Equal(DateTimeKind.Utc, actualTime.Kind);

            // Выводим дату в консоль теста для ручного контроля
            Console.WriteLine($"Успешно! Дата изменения файла на FTP (UTC): {actualTime}");
        }

        [Fact(Skip = "Real File")]
        public async Task UploadFile_RealNetwork_CheckDate()
        {
            var client = new FtpClient(realFtpUri, credentials);
            FileStream file = File.OpenRead("D:\\Users\\evgen\\Desktop\\Player-prev.log");
            await client.UploadFileAsync("Player-prev.log", file);

            DateTime dateTimeUploadFile = (await client.GetDateTimeStampAsync("Player-prev.log")).ToLocalTime();
            var now = DateTime.Now;
            Assert.True(dateTimeUploadFile.Hour == now.Hour && dateTimeUploadFile.Minute == now.Minute && dateTimeUploadFile.Date == now.Date);
        }
        [Fact]
        public async Task ListDirectory_RealNetwork_CheckDate()
        {
            var client = new FtpClient(realFtpUri, credentials);
            var list = await client.ListDirectoryAsync(null!, CancellationToken.None);
            Assert.NotEmpty(list);
        }

        [Fact]
        public async Task ListDirectoryDetails_RealNetwork_CheckDate()
        {
            var client = new FtpClient(realFtpUri, credentials);
            var list = await client.ListDirectoryDetailsAsync(null!, CancellationToken.None);
            Assert.NotEmpty(list);
        }
        [Fact]
        public async Task MakeDirectoryTestTest_RealNetwork_CheckDate()
        {
            var client = new FtpClient(realFtpUri, credentials);
            await client.MakeDirectoryAsync("test/test/unitest/");
            client.ListDirectory("test/test/unitest/");
        }
    }
}
