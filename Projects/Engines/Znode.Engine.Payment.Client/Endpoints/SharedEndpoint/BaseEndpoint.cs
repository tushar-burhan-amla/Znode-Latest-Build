using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Payment.Client.Endpoints
{
    public abstract class BaseEndpoint
    {
        //Payment Api URl
        public static string PaymentApiRoot => ZnodeAdminSettings.PaymentApplicationUrl;
    }
}
