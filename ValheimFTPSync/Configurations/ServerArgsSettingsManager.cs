using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;
using ValheimFTPSync.Security;

namespace ValheimFTPSync.Configuration
{
    internal class ServerArgsSettingsManager : ISettingManager
    {
        private DpapiEncryptionService dpapiEncryptionService { get; set; } = new DpapiEncryptionService();
        public bool Nographics { get; set; }
        public bool Batchmode { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public string World { get; set; }
        public string Password { get; set; }
        public bool Crossplay { get; set; }
        public string Savedir { get; set; }

        public ServerArgsSettingsManager()
        {
            Initialize();
        }

        [MemberNotNull(nameof(Nographics), nameof(Batchmode), nameof(Name), nameof(Port), nameof(World), nameof(Password), nameof(Crossplay), nameof(Savedir))]
        public void Initialize()
        {
            Nographics = Properties.ServerArgsSettings.Default.Nographics;
            Batchmode = Properties.ServerArgsSettings.Default.Batchmode;
            Name = Properties.ServerArgsSettings.Default.Name;
            Port = Properties.ServerArgsSettings.Default.Port;
            World =  Properties.ServerArgsSettings.Default.World;
            Password = Properties.ServerArgsSettings.Default.Password == Properties.ServerArgsSettings.Default.Properties["Password"].DefaultValue.ToString() ? Properties.ServerArgsSettings.Default.Password : dpapiEncryptionService.Decrypt(Properties.ServerArgsSettings.Default.Password);
            Crossplay = Properties.ServerArgsSettings.Default.Crossplay;
            Savedir = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            //Savedir = 
        }

        public void Save()
        {
            Properties.ServerArgsSettings.Default.Name = Name;
            Properties.ServerArgsSettings.Default.Port = Port;
            Properties.ServerArgsSettings.Default.World = World;
            Properties.ServerArgsSettings.Default.Password = dpapiEncryptionService.Encrypt(Password);
            Properties.ServerArgsSettings.Default.Nographics = Nographics;
            Properties.ServerArgsSettings.Default.Batchmode = Batchmode;
            Properties.ServerArgsSettings.Default.Crossplay = Crossplay;
            //Properties.ServerArgsSettings.Default.SaveDir =
            Properties.ServerArgsSettings.Default.Save();
        }

        public void Default()
        {
            var settings = Properties.ServerArgsSettings.Default;

            Name = (string)settings.Properties[nameof(settings.Name)].DefaultValue;
            int.TryParse((string)settings.Properties[nameof(settings.Port)].DefaultValue, out int port);
            Port = port;
            World = (string)settings.Properties[nameof(settings.World)].DefaultValue;
            Password = (string)settings.Properties[nameof(settings.Password)].DefaultValue;
            bool.TryParse((string)settings.Properties[nameof(settings.Nographics)].DefaultValue, out bool nographics);
            Nographics = nographics;
            bool.TryParse((string)settings.Properties[nameof(settings.Batchmode)].DefaultValue, out bool batchmode);
            Batchmode = batchmode;
            bool.TryParse((string)settings.Properties[nameof(settings.Crossplay)].DefaultValue, out bool crossplay);
            Crossplay = crossplay;
        }

        public void Reset()
        {
            Properties.ServerArgsSettings.Default.Reset();
            Initialize();
        }

        private static string[] intParams = new string[] { "-port" };
        private static string[] stringParams = new string[] { "-name", "-world", "-password", "-savedir" };
        private static string[] boolParams = new string[] { "-nographics", "-batchmode", "-crossplay" };

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
            var value = typeof(ServerArgsSettingsManager).GetProperty(name)?.GetValue(this);
            return value is null ? default : (T)value;
            //return (T?)typeof(ServerArgsSettingsManager ).GetProperty(name)?.GetValue(this) ?? default;
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
