using System.ComponentModel;
using System.Globalization;

namespace ValheimFTPSync
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void ChangeLanguage(string langCode)
        {
            // 1. Меняем культуру
            CultureInfo culture = new CultureInfo(langCode);
            CultureInfo.CurrentUICulture = culture;

            // 2. Создаем менеджер ресурсов для текущей формы
            ComponentResourceManager rm = new ComponentResourceManager(typeof(MainForm));

            // 3. Обновляем строки для каждого элемента управления на форме
            foreach (Control ctrl in this.Controls)
            {
                rm.ApplyResources(ctrl, ctrl.Name);
            }

            // 4. Не забываем обновить заголовок самой формы
            rm.ApplyResources(this, "$this");

            // Обновляем подсказки текстбокса.
            //appPathTextBox_LanguageUpdate(oldUICulture, culture);

        }

        private void LanguageButton_Click(object sender, EventArgs e)
        {
            if (sender is Control control && control.Tag is string locale)
            {
                ChangeLanguage(locale);
            }
        }
    }
}
