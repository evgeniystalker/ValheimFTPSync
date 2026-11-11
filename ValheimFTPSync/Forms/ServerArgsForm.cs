using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using ValheimFTPSync.Configuration;
using ValheimFTPSync.Properties;
using ValheimFTPSync.Services;

namespace ValheimFTPSync
{
    public partial class ServerArgsForm : Form
    {
        private readonly GameSessionValidator _validator;

        private ServerArgsSettingsManager SettingManager { get; set; }

        internal ServerArgsForm(ServerArgsSettingsManager appSetting, GameSessionValidator validator)
        {
            InitializeComponent();
            SettingManager = appSetting;
            SettingManager.Initialize();
            UpdateUI();
            _validator = validator;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            var error = _validator.ValidateServerArgs(textBoxName.Text, textBoxPassword.Text);
            if (error is not null)
            {
                DialogResult = DialogResult.None;
                MessageBox.Show(this, error, Messages.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxPassword.Focus();
                return;
            }
            SettingManager.Name = textBoxName.Text;
            SettingManager.Port = int.TryParse(textBoxPort.Text, out int port) ? port : port;
            SettingManager.World = textBoxWorld.Text;
            SettingManager.Password = textBoxPassword.Text;
            SettingManager.Nographics = checkBoxNoGraphics.Checked;
            SettingManager.Batchmode = checkBoxBatchMode.Checked;
            SettingManager.Crossplay = checkBoxCrossPlay.Checked;
            SettingManager.Save();
        }
        private void UpdateUI()
        {
            textBoxName.Text = SettingManager.Name;
            textBoxPort.Text = SettingManager.Port.ToString();
            textBoxWorld.Text = SettingManager.World;
            textBoxPassword.Text = SettingManager.Password;
            checkBoxNoGraphics.Checked = SettingManager.Nographics;
            checkBoxBatchMode.Checked = SettingManager.Batchmode;
            checkBoxCrossPlay.Checked = SettingManager.Crossplay;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxPassword_Enter(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = false;
        }

        private void textBoxPassword_Leave(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = true;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            SettingManager.Default();
            UpdateUI();
        }
    }
}
