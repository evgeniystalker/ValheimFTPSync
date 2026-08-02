namespace ValheimFTPSync
{
    partial class ServerArgsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerArgsForm));
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
            resetButton = new Button();
            SuspendLayout();
            // 
            // checkBoxCrossPlay
            // 
            resources.ApplyResources(checkBoxCrossPlay, "checkBoxCrossPlay");
            checkBoxCrossPlay.Checked = true;
            checkBoxCrossPlay.CheckState = CheckState.Checked;
            checkBoxCrossPlay.Name = "checkBoxCrossPlay";
            checkBoxCrossPlay.UseVisualStyleBackColor = true;
            // 
            // checkBoxBatchMode
            // 
            resources.ApplyResources(checkBoxBatchMode, "checkBoxBatchMode");
            checkBoxBatchMode.Checked = true;
            checkBoxBatchMode.CheckState = CheckState.Checked;
            checkBoxBatchMode.Name = "checkBoxBatchMode";
            checkBoxBatchMode.UseVisualStyleBackColor = true;
            // 
            // labelPassword
            // 
            resources.ApplyResources(labelPassword, "labelPassword");
            labelPassword.Name = "labelPassword";
            // 
            // labelWorld
            // 
            resources.ApplyResources(labelWorld, "labelWorld");
            labelWorld.Name = "labelWorld";
            // 
            // textBoxPassword
            // 
            resources.ApplyResources(textBoxPassword, "textBoxPassword");
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.UseSystemPasswordChar = true;
            textBoxPassword.Enter += textBoxPassword_Enter;
            textBoxPassword.Leave += textBoxPassword_Leave;
            // 
            // textBoxWorld
            // 
            resources.ApplyResources(textBoxWorld, "textBoxWorld");
            textBoxWorld.Name = "textBoxWorld";
            // 
            // labelPort
            // 
            resources.ApplyResources(labelPort, "labelPort");
            labelPort.Name = "labelPort";
            // 
            // textBoxPort
            // 
            resources.ApplyResources(textBoxPort, "textBoxPort");
            textBoxPort.Name = "textBoxPort";
            // 
            // textBoxName
            // 
            resources.ApplyResources(textBoxName, "textBoxName");
            textBoxName.Name = "textBoxName";
            // 
            // checkBoxNoGraphics
            // 
            resources.ApplyResources(checkBoxNoGraphics, "checkBoxNoGraphics");
            checkBoxNoGraphics.Checked = true;
            checkBoxNoGraphics.CheckState = CheckState.Checked;
            checkBoxNoGraphics.Name = "checkBoxNoGraphics";
            checkBoxNoGraphics.UseVisualStyleBackColor = true;
            // 
            // labelName
            // 
            resources.ApplyResources(labelName, "labelName");
            labelName.Name = "labelName";
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = DialogResult.Cancel;
            resources.ApplyResources(buttonCancel, "buttonCancel");
            buttonCancel.Name = "buttonCancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonOk
            // 
            buttonOk.DialogResult = DialogResult.OK;
            resources.ApplyResources(buttonOk, "buttonOk");
            buttonOk.Name = "buttonOk";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += buttonOk_Click;
            // 
            // resetButton
            // 
            resources.ApplyResources(resetButton, "resetButton");
            resetButton.Name = "resetButton";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += resetButton_Click;
            // 
            // ServerArgsForm
            // 
            AcceptButton = buttonOk;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ControlBox = false;
            Controls.Add(resetButton);
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
            Name = "ServerArgsForm";
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
        private Button resetButton;
    }
}