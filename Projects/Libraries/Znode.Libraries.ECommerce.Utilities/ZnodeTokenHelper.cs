using System;
using System.Text;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeEncryptionLibrary;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class ZnodeTokenHelper
    {
        public static string GenerateTokenKey(string key)
        {
            try
            {
                string authValueDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(key));
                string[] array = authValueDecoded.Split('|');
                long ticks = DateTime.Now.AddMinutes(ZnodeApiSettings.TokenExpirationTime).Ticks;
                string DomainName = array[0];
                string webApiKey = string.IsNullOrEmpty(array[1]) ? string.Empty : array[1];
                string randomnumber =
                   string.Join("|", new string[]
                   {  DomainName ,webApiKey,
                KeyGenerator.GetUniqueKey(),
                Convert.ToString(ticks),
                   });

                return EncryptText(randomnumber);
            }
            catch
            {
                throw;
            }
        }

        //To validate the token expiration.
        public static bool ValidateToken(string tick, string webApiKey, string domainName)
        {
            bool validToken = false;
            // Token issued time
            if (!string.IsNullOrEmpty(tick))
            {
                long ticks = long.Parse(tick);

                DateTime IssuedOn = new DateTime(ticks);
                string domainConfig = GetConfiguredDomainKey(domainName);

                if (string.Equals(webApiKey, domainConfig) && (DateTime.Now < IssuedOn))
                {
                    validToken = true;
                }
            }
            return validToken;
        }

        //To validate the domainName with database.
        public static string GetConfiguredDomainKey(string domainName)
        {
            var domainConfig = ZnodeConfigManager.GetDomainConfig(domainName);
            return !Equals(domainConfig, null) ? domainConfig.ApiKey : string.Empty;
        }

        public static bool CheckAuthHeader(string domainName, string domainKey)
        {
            // If either domain name or domain key are empty, get out
            if (String.IsNullOrEmpty(domainName) || String.IsNullOrEmpty(domainKey)) return false;

            // Get the configured key for the domain
            string configuredDomainKey = GetConfiguredDomainKey(domainName);

            // Now compare the two
            return String.Compare(domainKey, configuredDomainKey, StringComparison.InvariantCulture) == 0;
        }

        public static string[] GetAuthHeader(string authValue)
        {
            // If auth value doesn't exist, get out
            if (String.IsNullOrEmpty(authValue)) return null;

            // Strip off the "Basic "
            authValue = authValue.Remove(0, 6);

            // Decode it; if empty then get out
            string authValueDecoded = DecodeBase64(authValue);
            if (String.IsNullOrEmpty(authValueDecoded)) return null;

            // Now split it to get the domain info (index 0 = domain name, index 1 = domain key)
            return authValueDecoded.Split('|');
        }

        public static string DecodeBase64(string encodedValue) => Encoding.UTF8.GetString(Convert.FromBase64String(encodedValue));

        #region Impersonation 
        public static string GenerateCSRToken(int adminUserId, int userId, string userName)
        {
            try
            {
                long ticks = DateTime.Now.AddMinutes(2).Ticks;
                string randomnumber =
                    string.Join("|", new string[]
                    {  adminUserId.ToString(),
                       userId.ToString(),
                       userName,                      
                       KeyGenerator.GetUniqueKey(),
                       Convert.ToString(ticks),
                    });

                return EncryptText(randomnumber);
            }
            catch
            {
                throw;
            }
        }

        public static bool ValidateCSRToken(string encodedKey ,out string decriptedKey)
        {
            bool validToken = false;
            decriptedKey = "";
            if (!string.IsNullOrEmpty(encodedKey))
            {
                string key = DecryptText(encodedKey);

                string[] parts = key.Split(new char[] { '|' });
                // Token issued time

                long ticks = long.Parse(parts[4]);

                DateTime IssuedOn = new DateTime(ticks);
                if ((DateTime.Now < IssuedOn))
                {
                    validToken = true;
                    decriptedKey = key;
                }
            }
            return validToken;
        }
        #endregion
    }
}
