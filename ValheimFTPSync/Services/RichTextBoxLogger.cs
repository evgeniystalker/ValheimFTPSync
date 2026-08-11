using System;
using System.Collections.Generic;
using System.Text;
using ValheimFTPSync.Services.Interfaces;

namespace ValheimFTPSync.Services
{
    internal class RichTextBoxLogger : IAppLogger
    {
        private RichTextBox _richTextBox;

        bool HasText => _richTextBox.TextLength > 0;

        private const string _debug = "Debug: ";
        private const string _error = "Error: ";
        private const string _info = "Info: ";
        private const string _warning = "Warning: ";

        public RichTextBoxLogger(RichTextBox richTextBox)
        {
            this._richTextBox = richTextBox;
        }


        public void Debug(string message)
        {
            Log(_debug, message, Color.DarkGray);
        }

        public void Error(string message, Exception? ex = null)
        {
            Log(_error, message + Environment.NewLine + ex?.Message, Color.Red);
        }

        public void Info(string message)
        {
            Log(_info, message, RichTextBox.DefaultForeColor);
        }

        public void Warning(string message)
        {
            Log(_warning, message, Color.Orange);
        }

        private void AppendText(string logMessage, string message, Color color)
        {
            if (HasText)
            {
                _richTextBox.AppendText(Environment.NewLine);
            }
            Color oldColor;
            if (color != _richTextBox.SelectionColor)
            {
                oldColor = _richTextBox.SelectionColor;
                _richTextBox.SelectionColor = color;
                _richTextBox.AppendText(logMessage + message);
                _richTextBox.SelectionColor = oldColor;
            }
            else
            {
                _richTextBox.AppendText(logMessage + message);
            }
            _richTextBox.ScrollToCaret();
        }
        private void Log(string logMessage, string message, Color color)
        {
            if (_richTextBox.InvokeRequired)
                _richTextBox.BeginInvoke(AppendText, logMessage, message, color);
            else
                AppendText(logMessage, message, color);
        }
    }
}
