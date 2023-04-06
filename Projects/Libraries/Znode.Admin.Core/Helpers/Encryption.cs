using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Znode.Engine.Admin.Helpers
{
    public class Encryption
    {
        #region Constant Variables
        public const string _EncryptionKey = "znode123";
        #endregion

        /// <summary>
        /// Encrpt the given text
        /// </summary>
        /// <param name="clearText">string value to be encrypted</param>
        /// <returns>Encrypted string of given value</returns>
        public static string Encrypt(string clearText)
        {
            if (string.IsNullOrEmpty(clearText))
            {
                return string.Empty;
            }

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encrypter = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encrypter.Key = pdb.GetBytes(32);
                encrypter.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encrypter.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        /// <summary>
        /// Decrypt the given encrypted string.
        /// </summary>
        /// <param name="cipherText">encrypted string value</param>
        /// <returns>Decrypted string value</returns>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }

            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes encrypter = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(_EncryptionKey,
                            new byte[]
                                {
                                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64,
                                    0x65, 0x76
                                });

                if (encrypter != null)
                {
                    encrypter.Key = pdb.GetBytes(32);
                    encrypter.IV = pdb.GetBytes(16);

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encrypter.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            return cipherText;
        }

        /// <summary>
        /// Encrpt the given text
        /// </summary>
        /// <param name="clearText">string value to be encrypted</param>
        /// <returns>Encrypted string of given value</returns>
        public static string EncryptPaymentToken(string cipherText)
        {
            if (!string.IsNullOrEmpty(cipherText))
            {
                string encryptedToken = EncodeBase64(cipherText);
                return Encrypt($"Basic {encryptedToken}");
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Decrypt the given encrypted string.
        /// </summary>
        /// <param name="cipherText">encrypted string value</param>
        /// <returns>Decrypted string value</returns>
        public static string DecryptPaymentToken(string cipherText)
        {
            if (!string.IsNullOrEmpty(cipherText))
            {
                string encryptedToken = Decrypt(cipherText);
                return DecodeBase64(encryptedToken.Remove(0, 6));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Encode to Base64
        /// </summary>
        /// <param name="value">string value to be encrypted</param>
        /// <returns>Encrypted string of given value</returns>
        public static string EncodeBase64(string value)
        {
            var valueAsBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueAsBytes);
        }

        /// <summary>
        /// Decode using Base64
        /// </summary>
        /// <param name="encodedValue">encrypted string value</param>
        /// <returns>Decrypted string value</returns>
        public static string DecodeBase64(string encodedValue)
        {
            var encodedValueAsBytes = Convert.FromBase64String(encodedValue);
            return Encoding.UTF8.GetString(encodedValueAsBytes);
        }
    }
}