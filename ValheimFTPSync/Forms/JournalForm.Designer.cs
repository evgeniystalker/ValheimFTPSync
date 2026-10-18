namespace ValheimFTPSync
{
    partial class JournalForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JournalForm));
            dataGridView = new DataGridView();
            dateTimeStartServerDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            worldDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            statusCodeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            ipDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            nameServerDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            nameUserDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            serverLogEntryBindingSource = new BindingSource(components);
            loadingProgressBar = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)serverLogEntryBindingSource).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            resources.ApplyResources(dataGridView, "dataGridView");
            dataGridView.AutoGenerateColumns = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = SystemColors.GradientInactiveCaption;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] { dateTimeStartServerDataGridViewTextBoxColumn, worldDataGridViewTextBoxColumn, statusCodeDataGridViewTextBoxColumn, ipDataGridViewTextBoxColumn, nameServerDataGridViewTextBoxColumn, nameUserDataGridViewTextBoxColumn });
            dataGridView.DataSource = serverLogEntryBindingSource;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersVisible = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // 
            // dateTimeStartServerDataGridViewTextBoxColumn
            // 
            dateTimeStartServerDataGridViewTextBoxColumn.DataPropertyName = "DateTimeStartServer";
            resources.ApplyResources(dateTimeStartServerDataGridViewTextBoxColumn, "dateTimeStartServerDataGridViewTextBoxColumn");
            dateTimeStartServerDataGridViewTextBoxColumn.Name = "dateTimeStartServerDataGridViewTextBoxColumn";
            dateTimeStartServerDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // worldDataGridViewTextBoxColumn
            // 
            worldDataGridViewTextBoxColumn.DataPropertyName = "World";
            resources.ApplyResources(worldDataGridViewTextBoxColumn, "worldDataGridViewTextBoxColumn");
            worldDataGridViewTextBoxColumn.Name = "worldDataGridViewTextBoxColumn";
            worldDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // statusCodeDataGridViewTextBoxColumn
            // 
            statusCodeDataGridViewTextBoxColumn.DataPropertyName = "StatusCode";
            resources.ApplyResources(statusCodeDataGridViewTextBoxColumn, "statusCodeDataGridViewTextBoxColumn");
            statusCodeDataGridViewTextBoxColumn.Name = "statusCodeDataGridViewTextBoxColumn";
            statusCodeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ipDataGridViewTextBoxColumn
            // 
            ipDataGridViewTextBoxColumn.DataPropertyName = "Ip";
            resources.ApplyResources(ipDataGridViewTextBoxColumn, "ipDataGridViewTextBoxColumn");
            ipDataGridViewTextBoxColumn.Name = "ipDataGridViewTextBoxColumn";
            ipDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nameServerDataGridViewTextBoxColumn
            // 
            nameServerDataGridViewTextBoxColumn.DataPropertyName = "NameServer";
            resources.ApplyResources(nameServerDataGridViewTextBoxColumn, "nameServerDataGridViewTextBoxColumn");
            nameServerDataGridViewTextBoxColumn.Name = "nameServerDataGridViewTextBoxColumn";
            nameServerDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nameUserDataGridViewTextBoxColumn
            // 
            nameUserDataGridViewTextBoxColumn.DataPropertyName = "NameUser";
            resources.ApplyResources(nameUserDataGridViewTextBoxColumn, "nameUserDataGridViewTextBoxColumn");
            nameUserDataGridViewTextBoxColumn.Name = "nameUserDataGridViewTextBoxColumn";
            nameUserDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // serverLogEntryBindingSource
            // 
            serverLogEntryBindingSource.DataSource = typeof(Models.ServerLogEntry);
            // 
            // loadingProgressBar
            // 
            resources.ApplyResources(loadingProgressBar, "loadingProgressBar");
            loadingProgressBar.Name = "loadingProgressBar";
            loadingProgressBar.Style = ProgressBarStyle.Marquee;
            // 
            // JournalForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(loadingProgressBar);
            Controls.Add(dataGridView);
            Icon = Properties.Resources.IconValheim;
            Name = "JournalForm";
            FormClosing += JournalForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)serverLogEntryBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView;
        private BindingSource serverLogEntryBindingSource;
        private ProgressBar loadingProgressBar;
        private DataGridViewTextBoxColumn dateTimeStartServerDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn worldDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn statusCodeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn ipDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn nameServerDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn nameUserDataGridViewTextBoxColumn;
    }
}