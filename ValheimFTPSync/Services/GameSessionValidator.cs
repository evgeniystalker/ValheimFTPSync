using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Configuration;
using ValheimFTPSync.Properties;

namespace ValheimFTPSync.Services
{
    internal class GameSessionValidator
    {
        public string? ValidateServerArgs(string name, string password)
        {
            if (password.Length < 5)
                return "Пароль должен содержать минимум 5 символов.";

            if (string.Equals(password, name, StringComparison.OrdinalIgnoreCase))
                return "Пароль не должен совпадать с именем сервера.";

            return null;
        }

        public string? ValidateForLaunch(AppGlobalSettingSnapshot snapshot)
        {
            var error = ValidateServerArgs(snapshot.Server.Name, snapshot.Server.Password);
            if (error is not null)
                return error;

            error =  ValidateExePath(snapshot.App.ValheimExePath);
            if (error is not null)
                return error;

            return null;
        }

        public string? ValidateExePath(string exePath)
        {
            if (!File.Exists(exePath))
                return string.Format(Messages.FileNotFound, exePath);
            return null;
        }
    }
}
