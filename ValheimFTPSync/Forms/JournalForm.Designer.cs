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
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)serverLogEntryBindingSource).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = SystemColors.GradientInactiveCaption;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] { dateTimeStartServerDataGridViewTextBoxColumn, worldDataGridViewTextBoxColumn, statusCodeDataGridViewTextBoxColumn, ipDataGridViewTextBoxColumn, nameServerDataGridViewTextBoxColumn, nameUserDataGridViewTextBoxColumn });
            dataGridView.DataSource = serverLogEntryBindingSource;
            dataGridView.Location = new Point(12, 12);
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersVisible = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new Size(776, 304);
            dataGridView.TabIndex = 0;
            // 
            // dateTimeStartServerDataGridViewTextBoxColumn
            // 
            dateTimeStartServerDataGridViewTextBoxColumn.DataPropertyName = "DateTimeStartServer";
            dateTimeStartServerDataGridViewTextBoxColumn.HeaderText = "DateTimeStartServer";
            dateTimeStartServerDataGridViewTextBoxColumn.Name = "dateTimeStartServerDataGridViewTextBoxColumn";
            dateTimeStartServerDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // worldDataGridViewTextBoxColumn
            // 
            worldDataGridViewTextBoxColumn.DataPropertyName = "World";
            worldDataGridViewTextBoxColumn.HeaderText = "World";
            worldDataGridViewTextBoxColumn.Name = "worldDataGridViewTextBoxColumn";
            worldDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // statusCodeDataGridViewTextBoxColumn
            // 
            statusCodeDataGridViewTextBoxColumn.DataPropertyName = "StatusCode";
            statusCodeDataGridViewTextBoxColumn.HeaderText = "StatusCode";
            statusCodeDataGridViewTextBoxColumn.Name = "statusCodeDataGridViewTextBoxColumn";
            statusCodeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ipDataGridViewTextBoxColumn
            // 
            ipDataGridViewTextBoxColumn.DataPropertyName = "Ip";
            ipDataGridViewTextBoxColumn.HeaderText = "Ip";
            ipDataGridViewTextBoxColumn.Name = "ipDataGridViewTextBoxColumn";
            ipDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nameServerDataGridViewTextBoxColumn
            // 
            nameServerDataGridViewTextBoxColumn.DataPropertyName = "NameServer";
            nameServerDataGridViewTextBoxColumn.HeaderText = "NameServer";
            nameServerDataGridViewTextBoxColumn.Name = "nameServerDataGridViewTextBoxColumn";
            nameServerDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nameUserDataGridViewTextBoxColumn
            // 
            nameUserDataGridViewTextBoxColumn.DataPropertyName = "NameUser";
            nameUserDataGridViewTextBoxColumn.HeaderText = "NameUser";
            nameUserDataGridViewTextBoxColumn.Name = "nameUserDataGridViewTextBoxColumn";
            nameUserDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // serverLogEntryBindingSource
            // 
            serverLogEntryBindingSource.DataSource = typeof(Models.ServerLogEntry);
            // 
            // JournalForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 328);
            Controls.Add(dataGridView);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "JournalForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Journal Games";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)serverLogEntryBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn dateTimeStartServerDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn worldDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn statusCodeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn ipDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn nameServerDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn nameUserDataGridViewTextBoxColumn;
        private BindingSource serverLogEntryBindingSource;
    }
}