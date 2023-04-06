using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.Abstract.Client.Endpoints
{
    public abstract class BaseEndpoint
	{
        public static string ApiRoot => ZnodeAdminSettings.ZnodeApiRootUri;

        //Payment Api URl
        public static string PaymentApiRoot => ZnodeAdminSettings.PaymentApplicationUrl;

        public static string ListByQuery(string query) => $"{ApiRoot}{query}";

        public static string GenerateToken(string key) => $"{ApiRoot}/token/generatetoken/{key}";

        public static string GenerateToken() => $"{ApiRoot}/token/generatetoken";
    }
}
