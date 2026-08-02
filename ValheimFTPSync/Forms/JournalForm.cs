using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ValheimFTPSync.Models;

namespace ValheimFTPSync
{
    public partial class JournalForm : Form
    {
        public JournalForm(BindingList<ServerLogEntry> logEntry)
        {
            InitializeComponent();
            serverLogEntryBindingSource.DataSource = logEntry;
            this.DoubleBuffered = true;
        }
    }
}
