namespace ValheimFTPSync
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            serverAppPathTextBox = new TextBox();
            serverAppPathBrowseButton = new Button();
            russiaLanguageButton = new Button();
            usLanguageButton = new Button();
            kzLanguageButton = new Button();
            serverArgsButton = new Button();
            startServerButton = new Button();
            serverAppPathBrowserDialog = new FolderBrowserDialog();
            journalButton = new Button();
            ftpUrlTextBox = new TextBox();
            connectStatusPictureBox = new PictureBox();
            ftpSyncProgressBar = new CustomControls.CustomProgressBar();
            loggerRichTextBox = new RichTextBox();
            consoleButton = new Button();
            ftpUserNameTextBox = new TextBox();
            ftpPasswordTextBox = new TextBox();
            rememberPassCheckBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)connectStatusPictureBox).BeginInit();
            SuspendLayout();
            // 
            // serverAppPathTextBox
            // 
            resources.ApplyResources(serverAppPathTextBox, "serverAppPathTextBox");
            serverAppPathTextBox.Name = "serverAppPathTextBox";
            serverAppPathTextBox.TextChanged += serverAppPathTextBox_TextChanged;
            // 
            // serverAppPathBrowseButton
            // 
            resources.ApplyResources(serverAppPathBrowseButton, "serverAppPathBrowseButton");
            serverAppPathBrowseButton.Name = "serverAppPathBrowseButton";
            serverAppPathBrowseButton.UseVisualStyleBackColor = true;
            serverAppPathBrowseButton.Click += serverAppPathBrowseButton_Click;
            // 
            // russiaLanguageButton
            // 
            resources.ApplyResources(russiaLanguageButton, "russiaLanguageButton");
            russiaLanguageButton.BackgroundImage = Properties.Resources.russia;
            russiaLanguageButton.Name = "russiaLanguageButton";
            russiaLanguageButton.Tag = "ru-RU";
            russiaLanguageButton.UseVisualStyleBackColor = true;
            russiaLanguageButton.Click += LanguageButton_Click;
            // 
            // usLanguageButton
            // 
            resources.ApplyResources(usLanguageButton, "usLanguageButton");
            usLanguageButton.BackgroundImage = Properties.Resources.us;
            usLanguageButton.Name = "usLanguageButton";
            usLanguageButton.Tag = "en-US";
            usLanguageButton.UseVisualStyleBackColor = true;
            usLanguageButton.Click += LanguageButton_Click;
            // 
            // kzLanguageButton
            // 
            resources.ApplyResources(kzLanguageButton, "kzLanguageButton");
            kzLanguageButton.BackgroundImage = Properties.Resources.kz;
            kzLanguageButton.Name = "kzLanguageButton";
            kzLanguageButton.Tag = "kk-KZ";
            kzLanguageButton.UseVisualStyleBackColor = true;
            kzLanguageButton.Click += LanguageButton_Click;
            // 
            // serverArgsButton
            // 
            resources.ApplyResources(serverArgsButton, "serverArgsButton");
            serverArgsButton.BackgroundImage = Properties.Resources.settings;
            serverArgsButton.Name = "serverArgsButton";
            serverArgsButton.UseVisualStyleBackColor = true;
            serverArgsButton.Click += serverArgsButton_Click;
            // 
            // startServerButton
            // 
            resources.ApplyResources(startServerButton, "startServerButton");
            startServerButton.BackgroundImage = Properties.Resources.start_button;
            startServerButton.Name = "startServerButton";
            startServerButton.UseVisualStyleBackColor = true;
            startServerButton.Click += startServerButton_Click;
            // 
            // serverAppPathBrowserDialog
            // 
            resources.ApplyResources(serverAppPathBrowserDialog, "serverAppPathBrowserDialog");
            // 
            // journalButton
            // 
            resources.ApplyResources(journalButton, "journalButton");
            journalButton.BackgroundImage = Properties.Resources.journal;
            journalButton.Name = "journalButton";
            journalButton.UseVisualStyleBackColor = true;
            journalButton.Click += journalButton_Click;
            // 
            // ftpUrlTextBox
            // 
            resources.ApplyResources(ftpUrlTextBox, "ftpUrlTextBox");
            ftpUrlTextBox.Name = "ftpUrlTextBox";
            ftpUrlTextBox.TextChanged += FtpUrlTextBox_TextChanged;
            // 
            // connectStatusPictureBox
            // 
            resources.ApplyResources(connectStatusPictureBox, "connectStatusPictureBox");
            connectStatusPictureBox.BackColor = Color.Transparent;
            connectStatusPictureBox.Image = Properties.Resources.cloud_stop;
            connectStatusPictureBox.Name = "connectStatusPictureBox";
            connectStatusPictureBox.TabStop = false;
            // 
            // ftpSyncProgressBar
            // 
            resources.ApplyResources(ftpSyncProgressBar, "ftpSyncProgressBar");
            ftpSyncProgressBar.BackColor = Color.FromArgb(100, 170, 255, 200);
            ftpSyncProgressBar.ForeColor = Color.DarkSlateGray;
            ftpSyncProgressBar.Name = "ftpSyncProgressBar";
            ftpSyncProgressBar.ProgressColor = Color.LightGreen;
            ftpSyncProgressBar.Value = 20;
            // 
            // loggerRichTextBox
            // 
            resources.ApplyResources(loggerRichTextBox, "loggerRichTextBox");
            loggerRichTextBox.Name = "loggerRichTextBox";
            loggerRichTextBox.ReadOnly = true;
            // 
            // consoleButton
            // 
            resources.ApplyResources(consoleButton, "consoleButton");
            consoleButton.BackgroundImage = Properties.Resources.consoleRight;
            consoleButton.Name = "consoleButton";
            consoleButton.UseVisualStyleBackColor = true;
            consoleButton.Click += consoleButton_Click;
            // 
            // ftpUserNameTextBox
            // 
            resources.ApplyResources(ftpUserNameTextBox, "ftpUserNameTextBox");
            ftpUserNameTextBox.Name = "ftpUserNameTextBox";
            ftpUserNameTextBox.TextChanged += FtpUrlTextBox_TextChanged;
            // 
            // ftpPasswordTextBox
            // 
            resources.ApplyResources(ftpPasswordTextBox, "ftpPasswordTextBox");
            ftpPasswordTextBox.Name = "ftpPasswordTextBox";
            ftpPasswordTextBox.UseSystemPasswordChar = true;
            ftpPasswordTextBox.TextChanged += FtpUrlTextBox_TextChanged;
            // 
            // rememberPassCheckBox
            // 
            resources.ApplyResources(rememberPassCheckBox, "rememberPassCheckBox");
            rememberPassCheckBox.BackColor = Color.Transparent;
            rememberPassCheckBox.ForeColor = SystemColors.ActiveCaption;
            rememberPassCheckBox.Name = "rememberPassCheckBox";
            rememberPassCheckBox.UseVisualStyleBackColor = false;
            rememberPassCheckBox.CheckedChanged += rememberPassCheckBox_CheckedChanged;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.background;
            Controls.Add(rememberPassCheckBox);
            Controls.Add(ftpPasswordTextBox);
            Controls.Add(ftpUserNameTextBox);
            Controls.Add(consoleButton);
            Controls.Add(ftpSyncProgressBar);
            Controls.Add(connectStatusPictureBox);
            Controls.Add(ftpUrlTextBox);
            Controls.Add(journalButton);
            Controls.Add(startServerButton);
            Controls.Add(serverArgsButton);
            Controls.Add(kzLanguageButton);
            Controls.Add(usLanguageButton);
            Controls.Add(russiaLanguageButton);
            Controls.Add(serverAppPathBrowseButton);
            Controls.Add(serverAppPathTextBox);
            Controls.Add(loggerRichTextBox);
            Icon = Properties.Resources.IconValheim;
            Name = "MainForm";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)connectStatusPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox serverAppPathTextBox;
        private Button serverAppPathBrowseButton;
        private Button russiaLanguageButton;
        private Button usLanguageButton;
        private Button kzLanguageButton;
        private Button serverArgsButton;
        private Button startServerButton;
        private FolderBrowserDialog serverAppPathBrowserDialog;
        private Button journalButton;
        private TextBox ftpUrlTextBox;
        private PictureBox connectStatusPictureBox;
        private CustomControls.CustomProgressBar ftpSyncProgressBar;
        private RichTextBox loggerRichTextBox;
        private Button consoleButton;
        private TextBox ftpUserNameTextBox;
        private TextBox ftpPasswordTextBox;
        private CheckBox rememberPassCheckBox;
    }
}
