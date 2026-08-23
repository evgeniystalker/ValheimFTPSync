using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Models;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IUserInteractive
    {
        /// <summary>
        /// Запрашивает у пользователя, следует ли перезаписать файл, если он существует локально.
        /// </summary>
        /// <param name="localFilePath">Полный путь к локальному файлу.</param>
        /// <param name="remoteFilePath">Полный путь к удаленному файлу.</param>
        /// <returns>Решение пользователя: Overwrite, Skip или Prompt.</returns>
        ConflictResolutionAction ResolveFileConflict(string localFilePath, string remoteFilePath, uint localBytes, uint remoteBytes);
    }
}
