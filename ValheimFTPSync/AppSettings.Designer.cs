namespace ValheimFTPSync
{
    partial class AppSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppSettings));
            checkBoxCrossPlay = new CheckBox();
            checkBoxBatchMode = new CheckBox();
            labelPassword = new Label();
            labelWorld = new Label();
            textBoxPassword = new TextBox();
            textBoxWorld = new TextBox();
            labelPort = new Label();
            textBoxPort = new TextBox();
            textBoxName = new TextBox();
            checkBoxNoGraphics = new CheckBox();
            labelName = new Label();
            buttonCancel = new Button();
            buttonOk = new Button();
            SuspendLayout();
            // 
            // checkBoxCrossPlay
            // 
            checkBoxCrossPlay.AutoSize = true;
            checkBoxCrossPlay.CheckAlign = ContentAlignment.MiddleRight;
            checkBoxCrossPlay.Checked = true;
            checkBoxCrossPlay.CheckState = CheckState.Checked;
            checkBoxCrossPlay.Location = new Point(206, 99);
            checkBoxCrossPlay.Name = "checkBoxCrossPlay";
            checkBoxCrossPlay.Size = new Size(80, 19);
            checkBoxCrossPlay.TabIndex = 22;
            checkBoxCrossPlay.Text = "-crossplay";
            checkBoxCrossPlay.UseVisualStyleBackColor = true;
            // 
            // checkBoxBatchMode
            // 
            checkBoxBatchMode.AutoSize = true;
            checkBoxBatchMode.CheckAlign = ContentAlignment.MiddleRight;
            checkBoxBatchMode.Checked = true;
            checkBoxBatchMode.CheckState = CheckState.Checked;
            checkBoxBatchMode.Location = new Point(108, 99);
            checkBoxBatchMode.Name = "checkBoxBatchMode";
            checkBoxBatchMode.Size = new Size(92, 19);
            checkBoxBatchMode.TabIndex = 21;
            checkBoxBatchMode.Text = "-batchmode";
            checkBoxBatchMode.UseVisualStyleBackColor = true;
            // 
            // labelPassword
            // 
            labelPassword.AutoSize = true;
            labelPassword.Location = new Point(12, 73);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(62, 15);
            labelPassword.TabIndex = 18;
            labelPassword.Text = "-password";
            // 
            // labelWorld
            // 
            labelWorld.AutoSize = true;
            labelWorld.Location = new Point(12, 44);
            labelWorld.Name = "labelWorld";
            labelWorld.Size = new Size(42, 15);
            labelWorld.TabIndex = 19;
            labelWorld.Text = "-world";
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(80, 70);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(206, 23);
            textBoxPassword.TabIndex = 15;
            textBoxPassword.UseSystemPasswordChar = true;
            textBoxPassword.Enter += textBoxPassword_Enter;
            textBoxPassword.Leave += textBoxPassword_Leave;
            // 
            // textBoxWorld
            // 
            textBoxWorld.Location = new Point(60, 41);
            textBoxWorld.Name = "textBoxWorld";
            textBoxWorld.Size = new Size(226, 23);
            textBoxWorld.TabIndex = 16;
            // 
            // labelPort
            // 
            labelPort.AutoSize = true;
            labelPort.Location = new Point(202, 15);
            labelPort.Name = "labelPort";
            labelPort.Size = new Size(34, 15);
            labelPort.TabIndex = 20;
            labelPort.Text = "-port";
            // 
            // textBoxPort
            // 
            textBoxPort.Location = new Point(242, 12);
            textBoxPort.Name = "textBoxPort";
            textBoxPort.Size = new Size(44, 23);
            textBoxPort.TabIndex = 17;
            // 
            // textBoxName
            // 
            textBoxName.Location = new Point(60, 12);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(136, 23);
            textBoxName.TabIndex = 14;
            // 
            // checkBoxNoGraphics
            // 
            checkBoxNoGraphics.AutoSize = true;
            checkBoxNoGraphics.CheckAlign = ContentAlignment.MiddleRight;
            checkBoxNoGraphics.Checked = true;
            checkBoxNoGraphics.CheckState = CheckState.Checked;
            checkBoxNoGraphics.Location = new Point(12, 99);
            checkBoxNoGraphics.Name = "checkBoxNoGraphics";
            checkBoxNoGraphics.Size = new Size(90, 19);
            checkBoxNoGraphics.TabIndex = 13;
            checkBoxNoGraphics.Text = "-nographics";
            checkBoxNoGraphics.UseVisualStyleBackColor = true;
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Location = new Point(12, 15);
            labelName.Name = "labelName";
            labelName.Size = new Size(42, 15);
            labelName.TabIndex = 12;
            labelName.Text = "-name";
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(211, 129);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 11;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonOk
            // 
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.Location = new Point(121, 129);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(75, 23);
            buttonOk.TabIndex = 10;
            buttonOk.Text = "OK";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += buttonOk_Click;
            // 
            // AppSettings
            // 
            AcceptButton = buttonOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(304, 166);
            ControlBox = false;
            Controls.Add(checkBoxCrossPlay);
            Controls.Add(checkBoxBatchMode);
            Controls.Add(labelPassword);
            Controls.Add(labelWorld);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxWorld);
            Controls.Add(labelPort);
            Controls.Add(textBoxPort);
            Controls.Add(textBoxName);
            Controls.Add(checkBoxNoGraphics);
            Controls.Add(labelName);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOk);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "AppSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Настройки запуска приложения сервера.";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CheckBox checkBoxCrossPlay;
        private CheckBox checkBoxBatchMode;
        private Label labelPassword;
        private Label labelWorld;
        private TextBox textBoxPassword;
        private TextBox textBoxWorld;
        private Label labelPort;
        private TextBox textBoxPort;
        private TextBox textBoxName;
        private CheckBox checkBoxNoGraphics;
        private Label labelName;
        private Button buttonCancel;
        private Button buttonOk;
    }
}