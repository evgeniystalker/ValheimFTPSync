using System.Security;

namespace ValheimFTPSync.Configuration
{
    internal sealed record AppGlobalSettingSnapshot(AppSettingSnapshot App, ServerArgsSettingSnapshot Server);

    internal sealed record AppSettingSnapshot(string ServerAppFolderPath, string FtpUrl, string FtpUserName, string FtpPassword, bool RememberPassword, Point DisplayPostion, string ValheimExePath);

    internal sealed record ServerArgsSettingSnapshot(bool Nographics, bool Batchmode, string Name, int Port, string World, string Password, bool Crossplay, string Savedir)
    {
        private static string[] intParams = new string[] { "-port" };
        private static string[] stringParams = new string[] { "-name", "-world", "-password", "-savedir" };
        private static string[] boolParams = new string[] { "-nographics", "-batchmode", "-crossplay" };

        /// <summary>
        /// Возвращает полный список поддерживаемых ключей параметров.
        /// </summary>
        public static string[] GetNamesAllParams()
        {
            return boolParams.Concat(stringParams).Concat(intParams).ToArray();
        }

        /// <summary>
        /// Возвращает ключи параметров по типу (bool/string/int).
        /// </summary>
        public static string[] GetNamesParams<T>()
        {
            if (typeof(bool) == typeof(T))
                return boolParams;
            else if (typeof(string) == typeof(T))
                return stringParams;
            else if (typeof(int) == typeof(T))
                return intParams;
            else throw new Exception("Invalid type params");
        }

        /// <summary>
        /// Получает значение параметра по имени ключа (например, "-world").
        /// </summary>
        public T? GetParamValue<T>(string name)
        {
            if (name.StartsWith('-'))
                name = name.Substring(1);

            name = char.ToUpper(name[0]) + name.Substring(1);
            var value = typeof(ServerArgsSettingSnapshot).GetProperty(name)?.GetValue(this);
            return value is null ? default : (T)value;
            //return (T?)typeof(ServerArgsSettingSnapshot ).GetProperty(name)?.GetValue(this) ?? default;
        }

        /// <summary>
        /// Формирует строку параметров командной строки.
        /// </summary>
        public override string ToString()
        {
            var arguments = BuildArguments(true);
            return string.Join(" ", arguments);
        }

        public IReadOnlyList<string> BuildArguments(bool escaping = false)
        {
            List<string> comandLine = new List<string>();
            foreach (string name in intParams)
            {
                comandLine.Add(name);
                comandLine.Add(GetParamValue<int>(name).ToString());

            }
            foreach (string name in stringParams)
            {
                var value = GetParamValue<string>(name);
                if (!string.IsNullOrEmpty(value))
                {
                    comandLine.Add(name);
                    comandLine.Add(escaping ? $"\"{value}\"" : value);
                }
            }
            foreach (string name in boolParams)
            {
                if (GetParamValue<bool>(name))
                    comandLine.Add(name);
            }
            return comandLine;
        }
    }
}