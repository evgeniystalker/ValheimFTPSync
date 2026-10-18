using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IJournalService
    {
        Task<List<ServerLogEntry>> GetListAsync(CancellationToken cancellationToken = default);
    }
}