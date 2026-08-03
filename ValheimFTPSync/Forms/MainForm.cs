using System.ComponentModel;
using System.Globalization;
using ValheimFTPSync.Configuration;
using ValheimFTPSync.Models;

namespace ValheimFTPSync
{
    public partial class MainForm : Form
    {
        private AppGlobalSettingManager SettingManager { get; set; }
        public MainForm()
        {
            InitializeComponent();
            SettingManager = new AppGlobalSettingManager();
        }

        private void ChangeLanguage(string langCode)
        {
            // 1. Меняем культуру
            CultureInfo culture = new CultureInfo(langCode);
            CultureInfo.CurrentUICulture = culture;

            // 2. Создаем менеджер ресурсов для текущей формы
            ComponentResourceManager rm = new ComponentResourceManager(typeof(MainForm));

            rm.ApplyResources(serverAppPathBrowserDialog, nameof(serverAppPathBrowserDialog));
            // 3. Обновляем строки для каждого элемента управления на форме
            foreach (Control ctrl in this.Controls)
            {
                rm.ApplyResources(ctrl, ctrl.Name);
            }

            // 4. Не забываем обновить заголовок самой формы
            rm.ApplyResources(this, "$this");

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
                    Properties.Settings.Default.Save();
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

        }

        private void journalButton_Click(object sender, EventArgs e)
        {
            JournalForm journalForm = new JournalForm(new BindingList<ServerLogEntry>() { new ServerLogEntry() { DateTimeStartServer = DateTime.Now, NameUser = "zz" }, new ServerLogEntry() { DateTimeStartServer = DateTime.Now, NameUser = "zz" } });
            journalForm.ShowDialog();
        }

        private void ftpUrlTextBox_TextChanged(object sender, EventArgs e)
        {
            SettingManager.AppSettingManager.FtpUrl = ftpUrlTextBox.Text;
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            SettingManager.AppSettingManager.DisplayPostion = this.DesktopLocation;
        }
    }
}
