using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services.Interfaces;
using static System.Net.WebRequestMethods;

namespace ValheimFTPSync.Services
{
    internal class SyncService : ISyncService
    {
        readonly string _statusServerJsonPath = "/StatusServer.json";
        public async Task SyncAsync(IFtpClientBase ftpClient, string localPath, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            if (ftpClient is null)
                throw new InvalidOperationException("FTP client is not configured.");

            //await ftpClient.DeleteFileAsync();
        }
    }

    //internal class SyncService : ISyncService
    //{
    //    private readonly IFtpClient _ftpClient;
    //    private readonly IAppLogger _logger;

    //    public SyncService(
    //        IFtpClient ftpClient,
    //        IAppLogger logger)
    //    {
    //        _ftpClient = ftpClient;
    //        _logger = logger;
    //    }

    //    public async Task SyncAsync(
    //        string localPath,
    //        CancellationToken cancellationToken = default)
    //    {
    //        // 1. Получить FTP структуру
    //        // 2. Получить локальную структуру
    //        // 3. Сравнить
    //        // 4. Определить изменения
    //        // 5. Обработать конфликты
    //        // 6. Выполнить синхронизацию
    //    }
    //}
    //        Local отсутствует, FTP есть
    //        ↓
    //     Download

    //FTP отсутствует, Local есть
    //        ↓
    //      Upload

    //Оба есть и дата одинаковая
    //        ↓
    //       Skip

    //Оба есть, FTP новее
    //        ↓
    //   Download автоматически

    //Оба есть, Local новее
    //        ↓
    //    Upload автоматически

    //Обе изменились относительно
    //последней синхронизации
    //        ↓
    //      CONFLICT
    //   SyncService
    //│
    //├── анализирует
    //├── определяет конфликт
    //│
    //└── спрашивает ISyncConflictResolver
    //                  │
    //                  ▼
    //             WinForms
    //                  │
    //         ┌────────┼─────────┐
    //         ▼        ▼         ▼
    //      Upload Download    Skip
}

