using System.Security;

namespace ValheimFTPSync.Configuration
{
    internal sealed record AppSettingSnapshot(string ServerAppFolderPath, string FtpUrl, string FtpUserName, string FtpPassword, bool RememberPassword, Point DisplayPostion, string ValheimExePath);

    internal sealed record ServerArgsSettingSnapshot(bool Nographics, bool Batchmode, string Name, int Port, string World, string Password, bool Crossplay, string Savedir);

    internal sealed record AppGlobalSettingSnapshot(AppSettingSnapshot App, ServerArgsSettingSnapshot Server);
}