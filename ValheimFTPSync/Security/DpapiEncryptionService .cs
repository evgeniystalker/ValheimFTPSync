using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ValheimFTPSync.Security
{
    internal class DpapiEncryptionService : IEncryptionService
    {
        private readonly byte[] _entropy = Encoding.UTF8.GetBytes("Valheim-Launcher-Salt-2026-StalkerSun-Edition");
        public string Encrypt(string clearText)
        {
            if (string.IsNullOrEmpty(clearText)) return string.Empty;

            byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
            byte[] encryptedBytes = ProtectedData.Protect(clearBytes, _entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText)) return string.Empty;

            try
            {
                byte[]? encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] clearBytes = ProtectedData.Unprotect(encryptedBytes, _entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(clearBytes);
            }
            catch (FormatException)
            { 
                return string.Empty; 
            }
            catch (CryptographicException)
            {
                return string.Empty;
            }
        }
    }
}
