using System.Collections;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace ValheimFTPSync.Configuration
{
    internal class AppSettingManager : ISettingManager
    {
        public string ServerAppFolderPath { get; set; }
        public string FtpUrl { get; set => field = value.EndsWith("/") ? value : value + "/"; }
        public string ValheimExePath => string.IsNullOrWhiteSpace(ServerAppFolderPath) ? string.Empty : Path.Combine(ServerAppFolderPath, "valheim_server.exe");

        public Point DisplayPostion { get; set; }

        public AppSettingManager()
        {
            Initialize();
        }

        public void Save()
        {
            Properties.Settings.Default.ServerAppFolderPath = ServerAppFolderPath;
            Properties.Settings.Default.FtpUrl = FtpUrl;
            Properties.Settings.Default.DisplayPostion = DisplayPostion;

            Properties.Settings.Default.Save();
        }

        [MemberNotNull(nameof(ServerAppFolderPath), nameof(FtpUrl))]
        public void Initialize()
        {
            ServerAppFolderPath = Properties.Settings.Default.ServerAppFolderPath;
            FtpUrl = Properties.Settings.Default.FtpUrl;
            DisplayPostion = Properties.Settings.Default.DisplayPostion;
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
            DisplayPostion = (Point)settings.Properties[nameof(settings.DisplayPostion)].DefaultValue;
        }
    }
}
