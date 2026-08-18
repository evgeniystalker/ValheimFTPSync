using System.Collections;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using ValheimFTPSync.Security;

namespace ValheimFTPSync.Configuration
{
    internal class AppSettingManager : ISettingManager
    {
        private DpapiEncryptionService dpapiEncryptionService { get; set; } = new DpapiEncryptionService();
        public string ServerAppFolderPath { get; set; }
        public string FtpUrl { get; set => field = value.EndsWith("/") ? value : value + "/"; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
        public bool RememberPassword { get; set; }
        public string ValheimExePath => string.IsNullOrWhiteSpace(ServerAppFolderPath) ? string.Empty : Path.Combine(ServerAppFolderPath, "valheim_server.exe");

        public Point DisplayPostion { get; set; }

        public AppSettingManager()
        {
            Initialize();
        }

        public void Save()
        {
            Properties.Settings.Default.ServerAppFolderPath = ServerAppFolderPath;
            Properties.Settings.Default.FtpUrl = ClearUriCredentials(FtpUrl);
            Properties.Settings.Default.FtpUserName = FtpUserName;
            Properties.Settings.Default.FtpPassword = RememberPassword ? dpapiEncryptionService.Encrypt(FtpPassword) : (string)Properties.Settings.Default.Properties[nameof(FtpPassword)].DefaultValue;
            Properties.Settings.Default.RememberPassword = RememberPassword;
            Properties.Settings.Default.DisplayPostion = DisplayPostion;
            Properties.Settings.Default.Save();
        }

        [MemberNotNull(nameof(ServerAppFolderPath), nameof(FtpUrl), nameof(FtpUserName), nameof(FtpPassword))]
        public void Initialize()
        {
            ServerAppFolderPath = Properties.Settings.Default.ServerAppFolderPath;
            FtpUrl = Properties.Settings.Default.FtpUrl;
            FtpUserName = Properties.Settings.Default.FtpUserName;
            FtpPassword = Properties.Settings.Default.FtpPassword == (string)Properties.Settings.Default.Properties[nameof(FtpPassword)].DefaultValue ? Properties.Settings.Default.FtpPassword : dpapiEncryptionService.Decrypt(Properties.Settings.Default.FtpPassword);
            DisplayPostion = Properties.Settings.Default.DisplayPostion;
            RememberPassword = Properties.Settings.Default.RememberPassword;
        }

        public void Reset()
        {
            Properties.Settings.Default.Reset();
            Initialize();
        }
        public void Default()
        {
            var settings = Properties.Settings.Default;
            ServerAppFolderPath = (string)settings.Properties[nameof(settings.ServerAppFolderPath)].DefaultValue;
            FtpUrl = (string)settings.Properties[nameof(settings.FtpUrl)].DefaultValue;
            FtpUserName = (string)settings.Properties[nameof(settings.FtpUserName)].DefaultValue;
            FtpPassword = (string)settings.Properties[nameof(settings.FtpPassword)].DefaultValue;
            RememberPassword = (bool)settings.Properties[nameof(settings.RememberPassword)].DefaultValue;
            DisplayPostion = (Point)settings.Properties[nameof(settings.DisplayPostion)].DefaultValue;
        }
        public string ClearUriCredentials(string uri)
        {
            try
            {
                UriBuilder builder = new UriBuilder(uri);
                builder.Password = string.Empty;
                builder.UserName = string.Empty;
                return builder.Uri.ToString();
            }
            catch (UriFormatException)
            {
                return uri;
            }
        }
    }
}
