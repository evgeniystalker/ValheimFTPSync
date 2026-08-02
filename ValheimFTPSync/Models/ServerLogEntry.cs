using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    public class ServerLogEntry
    {
        public DateTime DateTimeStartServer { get; set; }
        public string World { get; set; } = string.Empty;
        public StatusCodeServer StatusCode { get; set; }
        public string Ip { get; set; } = string.Empty;
        public string NameServer { get; set; } = string.Empty;
        public string NameUser { get; set; } = string.Empty;
    }
}
