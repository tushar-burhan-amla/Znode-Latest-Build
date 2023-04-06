using System.Collections.Generic;

using Znode.Engine.Api.klaviyo.Cache;
using Znode.Engine.klaviyo.Models;
using Znode.Engine.klaviyo.Models.Responses;
using Znode.Engine.Klaviyo.Services;
using Znode.Libraries.Abstract.Cache;

namespace Znode.Engine.Api.Cache
{
    public class KlaviyoCache : BaseCache, IKlaviyoCache
    {
        #region Private Variable
        private readonly IKlaviyoService _service;
        #endregion

        #region Constructor
        public KlaviyoCache(IKlaviyoService klaviyoService)
        {
            _service = klaviyoService;
        }
        #endregion

        #region Public Method
        /// Get Klaviyo details 
        public virtual string GetKlaviyo(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                KlaviyoModel klaviyo = _service.GetKlaviyo(portalId);
                if (!Equals(klaviyo, null))
                {
                    KlaviyoResponse klaviyoResponse = new KlaviyoResponse { Klaviyo = klaviyo };
                    data = InsertIntoCache(routeUri, routeTemplate, klaviyoResponse);
                }
            }
            return data;
        }

        //To get Email ProvideList by PortalId.
        public virtual string GetEmailProviderList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                List<EmailProviderModel> emailProvider = _service.GetEmailProviderList();
                if (!Equals(emailProvider, null))
                {
                    KlaviyoResponse klaviyoResponse = new KlaviyoResponse { EmailProviderModelList = emailProvider };
                    data = InsertIntoCache(routeUri, routeTemplate, klaviyoResponse);
                }
            }
            return data;
        }
        #endregion
    }
}