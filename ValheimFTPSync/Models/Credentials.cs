using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    internal class Credentials
    {
        public Credentials(string? name, string? password)
        {
            Name = name;
            Password = password;
        }
        public string? Name { get; set; }
        public string? Password { get; set; }
    }
}
