using System;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class PaymentTokenService : BaseService, IPaymentTokenService
    {

        /// <summary>
        /// Delete expired token from payment api.
        /// </summary>
        /// <param></param>
        /// <returns>Return bool</returns>
        public virtual bool DeletePaymentToken()
        {
            IPaymentHelper paymentHelper = GetService<IPaymentHelper>();
            try
            {
                return paymentHelper.DeletePaymentToken();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Payment.ToString(), TraceLevel.Error);
                return false;
            }
        }
    }
}
