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
            stopServerButton = new Button();
            journalButton = new Button();
            ftpUrlTextBox = new TextBox();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
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
            russiaLanguageButton.Name = "russiaLanguageButton";
            russiaLanguageButton.Tag = "ru-RU";
            russiaLanguageButton.UseVisualStyleBackColor = true;
            russiaLanguageButton.Click += LanguageButton_Click;
            // 
            // usLanguageButton
            // 
            resources.ApplyResources(usLanguageButton, "usLanguageButton");
            usLanguageButton.Name = "usLanguageButton";
            usLanguageButton.Tag = "en-US";
            usLanguageButton.UseVisualStyleBackColor = true;
            usLanguageButton.Click += LanguageButton_Click;
            // 
            // kzLanguageButton
            // 
            resources.ApplyResources(kzLanguageButton, "kzLanguageButton");
            kzLanguageButton.Name = "kzLanguageButton";
            kzLanguageButton.Tag = "kk-KZ";
            kzLanguageButton.UseVisualStyleBackColor = true;
            kzLanguageButton.Click += LanguageButton_Click;
            // 
            // serverArgsButton
            // 
            resources.ApplyResources(serverArgsButton, "serverArgsButton");
            serverArgsButton.Name = "serverArgsButton";
            serverArgsButton.UseVisualStyleBackColor = true;
            serverArgsButton.Click += serverArgsButton_Click;
            // 
            // startServerButton
            // 
            resources.ApplyResources(startServerButton, "startServerButton");
            startServerButton.Name = "startServerButton";
            startServerButton.UseVisualStyleBackColor = true;
            startServerButton.Click += startServerButton_Click;
            // 
            // serverAppPathBrowserDialog
            // 
            resources.ApplyResources(serverAppPathBrowserDialog, "serverAppPathBrowserDialog");
            // 
            // stopServerButton
            // 
            resources.ApplyResources(stopServerButton, "stopServerButton");
            stopServerButton.Name = "stopServerButton";
            stopServerButton.UseVisualStyleBackColor = true;
            // 
            // journalButton
            // 
            resources.ApplyResources(journalButton, "journalButton");
            journalButton.Name = "journalButton";
            journalButton.UseVisualStyleBackColor = true;
            journalButton.Click += journalButton_Click;
            // 
            // ftpUrlTextBox
            // 
            resources.ApplyResources(ftpUrlTextBox, "ftpUrlTextBox");
            ftpUrlTextBox.Name = "ftpUrlTextBox";
            ftpUrlTextBox.TextChanged += ftpUrlTextBox_TextChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.cloud_stop;
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pictureBox1);
            Controls.Add(ftpUrlTextBox);
            Controls.Add(journalButton);
            Controls.Add(stopServerButton);
            Controls.Add(startServerButton);
            Controls.Add(serverArgsButton);
            Controls.Add(kzLanguageButton);
            Controls.Add(usLanguageButton);
            Controls.Add(russiaLanguageButton);
            Controls.Add(serverAppPathBrowseButton);
            Controls.Add(serverAppPathTextBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            LocationChanged += MainForm_LocationChanged;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
        private Button stopServerButton;
        private Button journalButton;
        private TextBox ftpUrlTextBox;
        private PictureBox pictureBox1;
    }
}
