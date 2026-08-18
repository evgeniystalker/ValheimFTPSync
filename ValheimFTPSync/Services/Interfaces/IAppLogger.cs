using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Services.Interfaces
{
    internal interface IAppLogger
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message, Exception? ex = null);
        void Debug(string message);
    }
}
