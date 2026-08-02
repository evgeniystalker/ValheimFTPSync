using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Security
{
    internal interface IEncryptionService
    {
        string Encrypt(string clearText);
        string Decrypt(string encryptedText);
    }
}
