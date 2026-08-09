using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using ValheimFTPSync.Configuration;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync
{
    public partial class MainForm : Form
    {
        private AppGlobalSettingManager SettingManager { get; set; }
        private FtpService FtpService { get; set; }
        private IAppLogger Logger { get; set; }

        public MainForm()
        {
            InitializeComponent();
            SettingManager = new AppGlobalSettingManager();
            Logger = new RichTextBoxLogger(loggerRichTextBox);
            FtpService = new FtpService(Logger);
        }

        private void ChangeLanguage(string langCode)
        {
            // 1. Меняем культуру
            CultureInfo culture = new CultureInfo(langCode);
            CultureInfo.CurrentUICulture = culture;

            // 2. Создаем менеджер ресурсов для текущей формы
            ResourceManager rm = new ResourceManager(typeof(MainForm));

            serverAppPathBrowserDialog.Description = rm.GetString(nameof(serverAppPathBrowserDialog) + ".Description",culture) ?? serverAppPathBrowserDialog.Description;
            // 3. Обновляем строки для каждого элемента управления на форме
            foreach (Control ctrl in this.Controls)
            {
                if(ctrl is TextBox textBox)
                    textBox.PlaceholderText = rm.GetString(ctrl.Name + "." + nameof(textBox.PlaceholderText), culture);
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
            if (Directory.Exists(SettingManager.AppSettingManager.ServerAppFolderPath))
            {
                if (File.Exists(SettingManager.AppSettingManager.ValheimExePath))
                {
                    // Файл найден! Сохраняем настройки
                    SettingManager.AppSettingManager.Save();
                }
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

        private void startServerButton_Click(object sender, EventArgs e)
        {
            //customProgressBar.Value += 10;
        }

        private void journalButton_Click(object sender, EventArgs e)
        {
            JournalForm journalForm = new JournalForm(new BindingList<ServerLogEntry>() { new ServerLogEntry() { DateTimeStartServer = DateTime.Now, NameUser = "zz" }, new ServerLogEntry() { DateTimeStartServer = DateTime.Now, NameUser = "zz" } });
            journalForm.ShowDialog();
        }

        private async void ftpUrlTextBox_TextChanged(object sender, EventArgs e)
        {
            SettingManager.AppSettingManager.FtpUrl = ftpUrlTextBox.Text;
            connectStatusPictureBox.Image = Properties.Resources.cloud_load;
            FtpService.SetUri(ftpUrlTextBox.Text);
            var isConnect = await FtpService.TryConnectAsync();
            if (isConnect)
                connectStatusPictureBox.Image = Properties.Resources.cloud_check;
            else
                connectStatusPictureBox.Image = Properties.Resources.cloud_remove;
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            SettingManager.AppSettingManager.DisplayPostion = this.DesktopLocation;
        }
    }
}
