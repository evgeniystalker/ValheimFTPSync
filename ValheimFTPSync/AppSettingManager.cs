using System.Collections;
using System.Configuration;

namespace ValheimFTPSync
{
    public class AppSettingManager
    {
        public string AppFolderPath { get; set; } = string.Empty;
        public string ValheimExePath => string.IsNullOrWhiteSpace(AppFolderPath) ? string.Empty : Path.Combine(AppFolderPath, "valheim.exe");

        public bool Nographics { get; set; } = true;
        public bool Batchmode { get; set; } = true;
        public string Name { get; set; } = "My server";
        public int Port { get; set; } = 2456;
        public string World { get; set; } = "Dedicated";
        public string Password { get; set; } = "secret";
        public bool Crossplay { get; set; } = true;
        public string Savedir { get; set; } = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

        private static string[] intParams = new string[] { "-port" };
        private static string[] stringParams = new string[] { "-name", "-world", "-password", "-savedir" };
        private static string[] boolParams = new string[] { "-nographics", "-batchmode", "-crossplay" };

        // Метод ЗАГРУЗКИ (вызывается 1 раз при старте программы)
        public void Load()
        {
            AppFolderPath = Properties.Settings.Default.AppFolderPath;

            Nographics = true;
            Batchmode = true; ;
            Name = "My server";
            Port = 2456;
            World = "Dedicated";
            Password = "secret";
            Crossplay = true;
            Savedir = string.Empty;
            // Загрузка остальных полей...
        }

        // Метод СОХРАНЕНИЯ (вызывается при закрытии окна настроек)
        public void Save()
        {
            Properties.Settings.Default.AppFolderPath = AppFolderPath;

            List<string> settings = new List<string>()
                {
                    this.Name,
                    this.World,
                    this.Password,
                    this.Port.ToString(),
                    this.Nographics.ToString(),
                    this.Batchmode.ToString(),
                    this.Crossplay.ToString()
                };
            Properties.Settings.Default.AppSetting = (System.Collections.Specialized.StringCollection)(IList)settings;
            Properties.Settings.Default.Save(); // Физически пишет в файл на диск
        }

        /// <summary>
        /// Возвращает полный список поддерживаемых ключей параметров.
        /// </summary>
        public static string[] GetNameAllParams()
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
            if (name.Contains('-'))
                name = name.Substring(1);

            name = char.ToUpper(name[0]) + name.Substring(1);
            var value = typeof(AppSettingManager).GetProperty(name)?.GetValue(this);
            return value is null ? default : (T)value;
            //return (T?)typeof(AppSettingManager).GetProperty(name)?.GetValue(this) ?? default;
        }

        /// <summary>
        /// Формирует строку параметров командной строки.
        /// </summary>
        public override string ToString()
        {
            List<string> comandLine = new List<string>();
            foreach (string name in intParams)
            {
                comandLine.Add(name + " " + GetParamValue<int>(name));
            }
            foreach (string name in stringParams)
            {
                comandLine.Add(name + " " + GetParamValue<string>(name));
            }
            foreach (string name in boolParams)
            {
                comandLine.Add(name);
            }
            return string.Join(" ", comandLine);
        }
    }
}
