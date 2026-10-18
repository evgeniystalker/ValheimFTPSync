using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ValheimFTPSync.Models;
using ValheimFTPSync.Services;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync
{
    internal partial class JournalForm : Form
    {
        IJournalService _journalService;
        CancellationTokenSource _cancellationTokenSource;
        private readonly ComponentResourceManager _resources;
        internal JournalForm(IFtpClientAsync ftpClient)
        {
            _resources = new(typeof(JournalForm));
            InitializeComponent();
            _journalService = new JournalService(ftpClient);
            _cancellationTokenSource = new();
            this.DoubleBuffered = true;
            Shown += JournalForm_Shown;
        }

        private async void JournalForm_Shown(object? sender, EventArgs e)
        {
            loadingProgressBar.Visible = true;
            try
            {
                var entries = await _journalService.GetListAsync();

                serverLogEntryBindingSource.DataSource =
                    new BindingList<ServerLogEntry>(entries);
                loadingProgressBar.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, _resources.GetString("journalLoadError") + ex.Message, _resources.GetString("messageBoxErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void JournalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
