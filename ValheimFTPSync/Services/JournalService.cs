using System.Text.Json;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class JournalService : IJournalService
    {
        const string NameServerLogFile = "StatusServer.json";

        IFtpClientAsync _ftpClient { get; set; }

        public JournalService(IFtpClientAsync ftpClient)
        {
            _ftpClient = ftpClient;
        }
        public async Task<List<ServerLogEntry>> GetListAsync(CancellationToken cancellationToken = default)
        {
            if (!await _ftpClient.ConnectAsync(cancellationToken))
                throw new Exception("Unable to connect");

            if (await CheckServerLogFileEntryAsync(cancellationToken))
            {
                using var stream = new MemoryStream();
                await _ftpClient.DownloadFileAsync(NameServerLogFile, stream, cancellationToken: cancellationToken);
                stream.Position = 0;

                return await JsonSerializer.DeserializeAsync<List<ServerLogEntry>>(stream, cancellationToken: cancellationToken) ?? new List<ServerLogEntry>();
            }
            else
                return new List<ServerLogEntry>();
        }

        public async Task<bool> CheckServerLogFileEntryAsync(CancellationToken cancellationToken = default)
        {
            var listFiles = await _ftpClient.ListDirectoryAsync("");
            return listFiles.Any(x => x == NameServerLogFile);
        }
    }
}
