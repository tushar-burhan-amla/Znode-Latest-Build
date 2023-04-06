using System;
using System.Text;
using Znode.Multifront.PaymentFramework.Bussiness;

namespace Znode.Multifront.PaymentApplication.Helpers
{
    public class EncryptionHelper
    {
        //Function to encrypt data
        public static string Encrypt(string data) => ZnodeEncryption.Encrypt(data);

        //Function to decrypt data
        public static string Decrypt(string data) => ZnodeEncryption.Decrypt(data);

        //Function to encrypt data
        public static string EncryptToken(string data) => ZnodeEncryption.EncryptPaymentToken(data);

        //Function to decrypt data
        public static string DecryptToken(string data) => ZnodeEncryption.DecryptPaymentToken(data);

        //Function to encode data using base64 encryption
        public static string EncodeBase64(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

        //Function to decode data using base64 decryption
        public static string DecodeBase64(string encodedValue) => Encoding.UTF8.GetString(Convert.FromBase64String(encodedValue));
    }
}
