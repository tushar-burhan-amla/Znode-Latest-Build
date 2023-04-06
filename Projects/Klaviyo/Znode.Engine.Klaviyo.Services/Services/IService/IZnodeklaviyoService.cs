using klaviyo.net;

using Znode.Engine.klaviyo.Models;

namespace Znode.Engine.Klaviyo.Services
{
    public interface IZnodeklaviyoService
    {
        #region Klaviyo
        /// <summary>
        /// To send user data to Klaviyo. 
        /// </summary>
        /// <param name="identify">User details for send to klaviyo</param>
        /// <param name="publicKey">Klaviyo public API key</param>
        /// <returns>Returns status 1:Success and 0:Fails data push to Klaviyo</returns>
        SubmitStatus KlaviyoIdentify(IdentifyModel identify, string publicKey);

        /// <summary>
        /// To send order details data to Klaviyo. 
        /// </summary>
        /// <param name="orderDetails">Order details for send to klaviyo</param>
        /// <param name="publicKey">Klaviyo public API key</param>
        /// <returns>Returns status 1:Success and 0:Fails data push to Klaviyo</returns>
        SubmitStatus KlaviyoOrder(OrderDetailsModel klaviyoProductDetailModel, string publicKey);
        #endregion
    }
}
