using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client.Endpoints
{
    public abstract class BaseEndpoint
	{
        public static string ApiRoot => ZnodeAdminSettings.ZnodeApiRootUri;

        //Payment Api URl
        public static string PaymentApiRoot => ZnodeAdminSettings.PaymentApplicationUrl;

        public static string ListByQuery(string query) => $"{ApiRoot}{query}";		
	}
}
