using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using ValheimFTPSync.Configuration;

namespace ValheimFTPSync
{
    public partial class ServerArgsForm : Form
    {
        private ServerArgsSettingsManager SettingManager { get; set; }

        internal ServerArgsForm(ServerArgsSettingsManager appSetting)
        {
            InitializeComponent();
            SettingManager = appSetting;
            SettingManager.Initialize();
            UpdateUI();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
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
