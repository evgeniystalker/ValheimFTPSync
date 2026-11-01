using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Configuration
{
    internal class AppGlobalSettingManager : ISettingManager, ISettingSnapshot<AppGlobalSettingSnapshot>
    {
        public AppSettingManager AppSettingManager { get; set; } = new AppSettingManager();
        public ServerArgsSettingsManager ServerSettingsManager { get; set; } = new ServerArgsSettingsManager();

        public void Save()
        {
            AppSettingManager.Save();
            ServerSettingsManager.Save();
        }

        public void Initialize()
        {
            AppSettingManager.Initialize();
            ServerSettingsManager.Initialize();
        }

        public void Default()
        {
            AppSettingManager.Default();
            ServerSettingsManager.Default();
        }

        public void Reset()
        {
            AppSettingManager.Reset();
            ServerSettingsManager.Reset();
        }

        public AppGlobalSettingSnapshot GetSnapshot()
        {
            return new AppGlobalSettingSnapshot(AppSettingManager.GetSnapshot(), ServerSettingsManager.GetSnapshot());
        }

        public void UpgradeVersion()
        {
            AppSettingManager.UpgradeVersion();
            ServerSettingsManager.UpgradeVersion();
        }
    }
}
