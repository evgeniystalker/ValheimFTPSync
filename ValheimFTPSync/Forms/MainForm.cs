using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Timers;
using ValheimFTPSync.Configuration;
using ValheimFTPSync.Forms.States;
using ValheimFTPSync.Models;
using ValheimFTPSync.Properties;
using ValheimFTPSync.Services;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync
{
    public partial class MainForm : Form, IUserInteractive
    {
        private AppGlobalSettingManager SettingManager { get; set; }
        private IAppLogger Logger { get; set; }
        IFtpFactory ClientFactory { get; set; }

        private readonly GameSessionManager _sessionManager;

        private IStateContext _stateContext { get; set; }

        private CancellationTokenSource? _connectionCheckCts;

        public MainForm()
        {
            InitializeComponent();
            InitializeTimer();
            SettingManager = new AppGlobalSettingManager();
            Logger = new RichTextBoxLogger(loggerRichTextBox);
            ClientFactory = new FtpFactory(Logger);
            _stateContext = new StateContext(new IdleState(), this);
            _sessionManager = new GameSessionManager(Logger, ClientFactory, (IUserInteractive)this);
        }

        private void ChangeLanguage(string langCode)
        {
            // 1. Меняем культуру
            CultureInfo culture = new CultureInfo(langCode);
            CultureInfo.CurrentUICulture = culture;

            // 2. Создаем менеджер ресурсов для текущей формы
            ResourceManager rm = new ResourceManager(typeof(MainForm));

            serverAppPathBrowserDialog.Description = rm.GetString(nameof(serverAppPathBrowserDialog) + ".Description", culture) ?? serverAppPathBrowserDialog.Description;
            // 3. Обновляем строки для каждого элемента управления на форме

            Control[] BlockedControls = { versionLabel };

            foreach (Control ctrl in this.Controls)
            {
                if (BlockedControls.Contains(ctrl))
                    continue;

                if (ctrl is TextBox textBox)
                {
                    textBox.PlaceholderText = rm.GetString(ctrl.Name + "." + nameof(textBox.PlaceholderText), culture);
                    textBox.Size = rm.GetObject(ctrl.Name + "." + nameof(textBox.Size), culture) is Size size ? size : textBox.Size;
                }
                else
                    ctrl.Text = rm.GetString(ctrl.Name + ".Text", culture);
            }

            // 4. Не забываем обновить заголовок самой формы
            this.Text = rm.GetString("$this.Text");
            // Обновляем подсказки текстбокса.
        }

        private void LanguageButton_Click(object sender, EventArgs e)
        {
            if (sender is Control control && control.Tag is string locale)
            {
                ChangeLanguage(locale);
                SettingManager.AppSettingManager.Language = locale;
            }
        }

        private void serverAppPathTextBox_TextChanged(object sender, EventArgs e)
        {
            SettingManager.AppSettingManager.ServerAppFolderPath = serverAppPathTextBox.Text;
            if (Directory.Exists(SettingManager.AppSettingManager.ServerAppFolderPath)
                && File.Exists(SettingManager.AppSettingManager.ValheimExePath)
                && SettingManager.AppSettingManager.ServerAppFolderPath != serverAppPathTextBox.Text)
            {
                // Файл найден! Сохраняем настройки
                SettingManager.AppSettingManager.Save();

            }
        }

        private void serverArgsButton_Click(object sender, EventArgs e)
        {
            ServerArgsForm settingBatForm = new ServerArgsForm(SettingManager.ServerSettingsManager);
            settingBatForm.Owner = this;
            if (settingBatForm.ShowDialog() == DialogResult.OK)
            {
                SettingManager.ServerSettingsManager.Save();
            }
        }

        private void serverAppPathBrowseButton_Click(object sender, EventArgs e)
        {
            string initDirectory = string.IsNullOrEmpty(SettingManager.AppSettingManager.ServerAppFolderPath) ? "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Valheim dedicated server" : SettingManager.AppSettingManager.ServerAppFolderPath;
            serverAppPathBrowserDialog.InitialDirectory = Path.GetDirectoryName(initDirectory) ?? string.Empty;
            if (serverAppPathBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                serverAppPathTextBox.Clear();
                serverAppPathTextBox.Text = serverAppPathBrowserDialog.SelectedPath;
                SettingManager.AppSettingManager.ServerAppFolderPath = serverAppPathBrowserDialog.SelectedPath;
                SettingManager.AppSettingManager.Save();
            }
        }

        private async void serverButton_Click(object? sender, EventArgs e)
        {
            if (_sessionManager.IsRunning)
            {
                StopSession();
                return;
            }

            try
            {
                SettingManager.Save();
                _stateContext.TransientToState(new LoadingState());

                await _sessionManager.RunAsync(SettingManager.GetSnapshot());
            }
            finally
            {
                _stateContext.TransientToState(new IdleState());
            }
        }

        private void StopSession()
        {
            _stateContext.TransientToState(new CancelState());
            _sessionManager.Stop();
        }

        private void journalButton_Click(object sender, EventArgs e)
        {
            var appSetting = SettingManager.AppSettingManager;
            var client = ClientFactory.CreateFtpClient(appSetting.FtpUrl, appSetting.FtpUserName, appSetting.FtpPassword);
            if (client != null)
            {
                JournalForm journalForm = new JournalForm(client);
                journalForm.ShowDialog();
            }
        }

        private System.Timers.Timer debounceTimer;

        [MemberNotNull(nameof(debounceTimer))]
        private void InitializeTimer()
        {
            components ??= new System.ComponentModel.Container();
            debounceTimer = new System.Timers.Timer(1000);
            components.Add(debounceTimer);
            debounceTimer.AutoReset = false;
            debounceTimer.Elapsed += FtpUrlCheckConnect;
            debounceTimer.SynchronizingObject = this;
        }

        private async void FtpUrlCheckConnect(object? sender, ElapsedEventArgs e)
        {
            _connectionCheckCts?.Cancel();

            using var cts = new CancellationTokenSource();
            _connectionCheckCts = cts;
            var appSetting = SettingManager.AppSettingManager;
            var client = ClientFactory.CreateFtpClient(appSetting.FtpUrl, appSetting.FtpUserName, appSetting.FtpPassword);
            bool isConnect = false;
            if (client != null)
            {
                FtpService ftpService = new FtpService(client, Logger, new LocalFileService());
                isConnect = await ftpService.TryConnectAsync(cts.Token);
            }

            if (!cts.IsCancellationRequested)
            {
                connectStatusPictureBox.Image = isConnect ? Properties.Resources.cloud_check : Properties.Resources.cloud_remove;
            }

            if (ReferenceEquals(_connectionCheckCts, cts))
                _connectionCheckCts = null;
        }

        private void FtpUrlTextBox_TextChanged(object sender, EventArgs e)
        {
            debounceTimer.Stop();
            debounceTimer.Start();
            switch (sender)
            {
                case TextBox textBox when textBox.Name == nameof(ftpUrlTextBox):
                    SettingManager.AppSettingManager.FtpUrl = ftpUrlTextBox.Text.Trim();
                    break;
                case TextBox textBox when textBox.Name == nameof(ftpUserNameTextBox):
                    SettingManager.AppSettingManager.FtpUserName = ftpUserNameTextBox.Text;
                    break;
                case TextBox textBox when textBox.Name == nameof(ftpPasswordTextBox):
                    SettingManager.AppSettingManager.FtpPassword = ftpPasswordTextBox.Text;
                    break;
            }

            connectStatusPictureBox.Image = Properties.Resources.cloud_load;
        }

        private void consoleButton_Click(object sender, EventArgs e)
        {
            loggerRichTextBox.Visible = !loggerRichTextBox.Visible;
            consoleButton.BackgroundImage = loggerRichTextBox.Visible ? Properties.Resources.consoleLeft : Properties.Resources.consoleRight;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (SettingManager.AppSettingManager.SettingUpgradeRequired)
                SettingManager.UpgradeVersion();
            if (!string.IsNullOrEmpty(SettingManager.AppSettingManager.Language) && SettingManager.AppSettingManager.Language != CultureInfo.CurrentUICulture.Name)
                ChangeLanguage(SettingManager.AppSettingManager.Language);


            var point = SettingManager.AppSettingManager.DisplayPostion;

            if (!point.IsEmpty && Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(point)))
                this.DesktopLocation = point;

            ftpUrlTextBox.Text = SettingManager.AppSettingManager.FtpUrl;
            serverAppPathTextBox.Text = SettingManager.AppSettingManager.ServerAppFolderPath;
            ftpUserNameTextBox.Text = SettingManager.AppSettingManager.FtpUserName;
            ftpPasswordTextBox.Text = SettingManager.AppSettingManager.FtpPassword;
            rememberPassCheckBox.Checked = SettingManager.AppSettingManager.RememberPassword;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SettingManager.AppSettingManager.DisplayPostion = this.DesktopLocation;
            SettingManager.Save();
            debounceTimer.Stop();
            debounceTimer.Elapsed -= FtpUrlCheckConnect;
        }

        private void RememberPassCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SettingManager.AppSettingManager.RememberPassword = rememberPassCheckBox.Checked;
        }
        public void EnableUI(bool enable)
        {
            ftpPasswordTextBox.Enabled = ftpUserNameTextBox.Enabled = ftpUrlTextBox.Enabled = serverAppPathTextBox.Enabled = serverAppPathBrowseButton.Enabled = enable;
            serverButton.BackgroundImage = enable ? Properties.Resources.start_button : Properties.Resources.stop_button;
        }
        public void ServerButtonEnable(bool enable)
        {
            serverButton.Enabled = enable;
        }

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            if (!AppVersion.TryParse(Application.ProductVersion, out AppVersion? appVersion))
            {
                Logger.Info(Messages.VersionNotParsed);
                versionLabel.Enabled = false;
                return;
            }
            versionLabel.Text = appVersion.Version + (appVersion.Prereliase is not null ? "-" + appVersion.Prereliase : string.Empty);

            UpdateAppService updateAppService = new(Logger);
            var latest = await updateAppService.CheckForUpdatesAsync(appVersion.Prereliase is not null);
            if (latest is null || latest <= appVersion)
            {
                Logger.Info(Messages.NewVersionNotFound);
                return;
            }

            if (IsDisposed) return;

            var answer = MessageBox.Show(this, String.Format(Messages.VersionAvailable, latest.Version + latest.Prereliase?.Insert(0, "-")), Messages.Update, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (answer == DialogResult.Yes)
                Process.Start(new ProcessStartInfo("https://github.com/evgeniystalker/ValheimFTPSync/releases/latest") { UseShellExecute = true });
        }

        ConflictResolutionAction IUserInteractive.ResolveFileConflict(string localFilePath, string remoteFilePath, uint localBytes, uint remoteBytes)
        {
            throw new NotImplementedException();
        }
    }
}
