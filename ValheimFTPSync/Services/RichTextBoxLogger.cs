using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class RichTextBoxLogger : IAppLogger
    {
        RichTextBox richTextBox;

        private const string _debug = "Debug: ";
        private const string _error = "Error: ";
        private const string _info = "Info: ";
        private const string _warning = "Warning: ";

        public RichTextBoxLogger(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
        }


        public void Debug(string message)
        {
            Log(_debug, message, Color.DarkGray);
        }

        public void Error(string message, Exception? ex = null)
        {
            Log(_error, message + Environment.NewLine + ex.Message, Color.Red);
        }

        public void Info(string message)
        {
            Log(_info, message, richTextBox.SelectionColor);
        }

        public void Warning(string message)
        {
            Log(_warning, message, Color.Orange);
        }

        private void Log(string logMessage , string message, Color color) {

            Color oldColor;
            if (color != richTextBox.SelectionColor)
            {
                oldColor = richTextBox.SelectionColor;
                richTextBox.SelectionColor = color;
                richTextBox.AppendText(logMessage + message + Environment.NewLine);
                richTextBox.SelectionColor = oldColor;
            }
            else
            {
                richTextBox.AppendText(logMessage + message + Environment.NewLine);
            }
            richTextBox.ScrollToCaret();
        }
    }
}
