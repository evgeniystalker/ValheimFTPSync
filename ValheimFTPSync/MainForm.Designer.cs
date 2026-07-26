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
            appPathTextBox = new TextBox();
            browseAppButton = new Button();
            russiaLanguageButton = new Button();
            usLanguageButton = new Button();
            SuspendLayout();
            // 
            // appPathTextBox
            // 
            resources.ApplyResources(appPathTextBox, "appPathTextBox");
            appPathTextBox.Name = "appPathTextBox";
            // 
            // browseAppButton
            // 
            resources.ApplyResources(browseAppButton, "browseAppButton");
            browseAppButton.Name = "browseAppButton";
            browseAppButton.UseVisualStyleBackColor = true;
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
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(usLanguageButton);
            Controls.Add(russiaLanguageButton);
            Controls.Add(browseAppButton);
            Controls.Add(appPathTextBox);
            Name = "MainForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox appPathTextBox;
        private Button browseAppButton;
        private Button russiaLanguageButton;
        private Button usLanguageButton;
    }
}
