using klaviyo.net;

namespace Znode.Libraries.Klaviyo
{
    public interface IZnodeklaviyoService
    {
        #region Klaviyo
        /// <summary>
        /// To send user data to Klaviyo. 
        /// </summary>
        /// <param name="identify">User details for send to klaviyo</param>
        /// <param name="publicKey">Klaviyo public API key</param>
        /// <returns>Returns staus 1:Success and 0:Fails data push to Kalviyo</returns>
        SubmitStatus KlaviyoIdentify(IdentifyModel identify, string publicKey);

        /// <summary>
        /// To send order details data to Klaviyo. 
        /// </summary>
        /// <param name="orderDetails">Order details for send to kalviyo</param>
        /// <param name="publicKey">Klaviyo public API key</param>
        /// <returns>Returns staus 1:Success and 0:Fails data push to Kalviyo</returns>
        SubmitStatus KlaviyoOrder(OrderDetailsModel klaviyoProductDetailModel, string publicKey);
        #endregion
    }
}
