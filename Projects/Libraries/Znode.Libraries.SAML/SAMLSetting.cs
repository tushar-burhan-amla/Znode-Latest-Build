using System.Configuration;

namespace Znode.Libraries.SAML
{
    public static class SAMLSetting
    {
        public static string Client
        {
            get
            {
                return ConfigurationManager.AppSettings["client"];
            }
        }

        public static string ClientReturnUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["clientReturnURL"];
            }
        }



    }
}
