using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Resources;
using System.Timers;
using ValheimFTPSync.Configuration;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync
{
    public partial class MainForm : Form
    {
        private AppGlobalSettingManager SettingManager { get; set; }
        private ISyncService SyncService { get; set; }
        private IAppLogger Logger { get; set; }

        private CancellationTokenSource? CancellationTokenSource { get; set; }
        public MainForm()
        {
            InitializeComponent();
            InitializeTimer();
            SettingManager = new AppGlobalSettingManager();
            Logger = new RichTextBoxLogger(loggerRichTextBox);
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
            foreach (Control ctrl in this.Controls)
            {
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


        private void serverButton_Click(object sender, EventArgs e)
        {
            if (CancellationTokenSource == null)
                startServerButton_Click(sender, e);
            else
                CancellationTokenSource.CancelAsync();
        }

        private void startServerButton_Click(object? sender, EventArgs e)
        {
            SettingManager.Save();
            CancellationTokenSource = new CancellationTokenSource();
            RunningState(true);

        }
        private void stopServerButton_Click(object? sender, EventArgs e)
        {
            RunningState(false);
        }

        private void journalButton_Click(object sender, EventArgs e)
        {
            JournalForm journalForm = new JournalForm(new BindingList<ServerLogEntry>() { new ServerLogEntry() { DateTimeStartServer = DateTime.Now, NameUser = "zz" }, new ServerLogEntry() { DateTimeStartServer = DateTime.Now, NameUser = "zz" } });
            journalForm.ShowDialog();
        }

        private System.Timers.Timer debounceTimer;

        [MemberNotNull(nameof(debounceTimer))]
        private void InitializeTimer()
        {
            if (components is null)
                components = new System.ComponentModel.Container();
            debounceTimer = new System.Timers.Timer(1000);
            components.Add(debounceTimer);
            debounceTimer.AutoReset = false;
            debounceTimer.Elapsed += FtpUrlCheckConnect;
        }

        private async void FtpUrlCheckConnect(object? sender, ElapsedEventArgs e)
        {
            ICredentials? credentials = null;
            if (!string.IsNullOrEmpty(ftpUserNameTextBox.Text) || !string.IsNullOrEmpty(ftpPasswordTextBox.Text))
                credentials = new NetworkCredential(ftpUserNameTextBox.Text, ftpPasswordTextBox.Text);
            IFtpClientFactory clientFactory = new FtpClientFactory(Logger);
            var client = clientFactory.CreateFtpClient(ftpUrlTextBox.Text, credentials);
            FtpService ftpService = new FtpService(client, Logger, new LocalFileService());
            var isConnect = await (ftpService.TryConnectAsync() ?? Task.FromResult(false));
            Invoke(new Action(() =>
            {
                if (isConnect)
                    connectStatusPictureBox.Image = Properties.Resources.cloud_check;
                else
                    connectStatusPictureBox.Image = Properties.Resources.cloud_remove;
            }));
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

        private void rememberPassCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SettingManager.AppSettingManager.RememberPassword = rememberPassCheckBox.Checked;
        }
        public void RunningState(bool enable)
        {
            ftpPasswordTextBox.Enabled = ftpUserNameTextBox.Enabled = ftpUrlTextBox.Enabled = serverAppPathTextBox.Enabled = serverAppPathBrowseButton.Enabled = enable;
            serverButton.BackgroundImage = enable ? Properties.Resources.stop_button : Properties.Resources.start_button;
        }
    }
}
