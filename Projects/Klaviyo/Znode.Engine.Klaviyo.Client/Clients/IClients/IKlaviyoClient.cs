using System.Collections.Generic;
using Znode.Engine.klaviyo.Models;
using Znode.Libraries.Abstract.Client;

namespace Znode.Engine.Klaviyo.IClient
{
    public interface IKlaviyoClient : IBaseClient
    {
        /// <summary>
        /// Get Klaviyo by portalId
        /// </summary>
        /// <param name="portalId">int portalId to get Klaviyo</param>
        /// <returns>Klaviyo Model</returns>
        KlaviyoModel GetKlaviyo(int portalId);

        /// <summary>
        /// Update Klaviyo.
        /// </summary>
        /// <param name="KlaviyoModel">KlaviyoModel to update klaviyo.</param>
        /// <returns>Klaviyo Model.</returns>
        KlaviyoModel UpdateKlaviyo(KlaviyoModel klaviyoModel);

        /// <summary>
        /// Identify Klaviyo
        /// </summary>
        /// <param name="UserModel">UserModel to get data for Klaviyo.</param>
        /// <returns>Klaviyo Model.</returns>
        bool IdentifyKlaviyo(IdentifyModel userModel);

        /// <summary>
        /// Track Klaviyo.
        /// </summary>
        /// <param name="KlaviyoProductDetailModel">KlaviyoProductDetailModel to get data for klaviyo.</param>
        /// <returns>Klaviyo Model.</returns>
        bool TrackKlaviyo(KlaviyoProductDetailModel klaviyoProductDetailModel);

        /// <summary>
        /// To get email provider list.
        /// </summary>
        /// <param name="EmailProviderModel">EmailProviderModel to get data for Provider List.</param>
        /// <returns>EmailProvider Model.</returns>
        List<EmailProviderModel> GetEmailProviderList();

    }
}
