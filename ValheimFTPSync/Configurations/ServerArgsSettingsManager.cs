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
    internal class ServerArgsSettingsManager : ISettingManager, ISettingSnapshot<ServerArgsSettingSnapshot>
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
            World = Properties.ServerArgsSettings.Default.World;
            Password = Properties.ServerArgsSettings.Default.Password == (string)Properties.ServerArgsSettings.Default.Properties[nameof(Password)].DefaultValue ? Properties.ServerArgsSettings.Default.Password : dpapiEncryptionService.Decrypt(Properties.ServerArgsSettings.Default.Password);
            Crossplay = Properties.ServerArgsSettings.Default.Crossplay;
            Savedir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName ?? throw new InvalidOperationException("Application product name is not specified."), "Saves");
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

        public ServerArgsSettingSnapshot GetSnapshot()
        {
            return new ServerArgsSettingSnapshot(this.Nographics, this.Batchmode, this.Name, this.Port, this.World, this.Password, this.Crossplay, this.Savedir);
        }

        public void UpgradeVersion()
        {
            Properties.ServerArgsSettings.Default.Upgrade();
            Initialize();
        }
    }
}
