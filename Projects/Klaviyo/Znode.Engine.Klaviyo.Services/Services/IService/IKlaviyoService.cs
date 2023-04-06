using System.Collections.Generic;

using Znode.Engine.klaviyo.Models;

namespace Znode.Engine.Klaviyo.Services
{
    public interface IKlaviyoService
    {
        /// <summary>
        ///  Get Klaviyo by portal Id.
        /// </summary>
        /// <param name="portalId">int klaviyoId to get the klaviyo.</param>
        /// <returns>KlaviyoModel</returns>
        KlaviyoModel GetKlaviyo(int portalId);

        /// <summary>
        /// Update Klaviyo associated to portal.
        /// </summary>
        /// <param name="klaviyoModel">KlaviyoModel to update data.</param>
        /// <returns>returns true if klaviyo updated else returns false.</returns>
        bool UpdateKlaviyo(KlaviyoModel klaviyoModel);

        /// <summary>
        /// Get Klaviyo Identify Details
        /// </summary>
        /// <param name="UserModel">UserModel to update data.</param>
        /// <returns>returns true if KlaviyoModel is not null else returns false.</returns>
        bool KlaviyoIdentify(IdentifyModel userModel);

        /// <summary>
        /// Get Klaviyo Identify Details
        /// </summary>
        /// <param name="UserModel">UserModel to update data.</param>
        /// <returns>returns true if KlaviyoModel is not null else returns false.</returns>
        bool KlaviyoTrack(KlaviyoProductDetailModel klaviyoProductDetailModel);

        /// <summary>
        /// Get Email provider List Details
        /// </summary>
        /// <param name="EmailProviderModel">EmailProviderModel to get provider data.</param>
        /// <returns>EmailProviderModel</returns>
        List<EmailProviderModel> GetEmailProviderList();
    }
}
