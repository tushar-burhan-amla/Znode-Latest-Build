using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class UrlRedirectCache : BaseCache, IUrlRedirectCache
    {
        #region Private Variable
        private readonly IUrlRedirectService _service;
        #endregion

        #region Constructor
        public UrlRedirectCache(IUrlRedirectService urlRedirectService)
        {
            _service = urlRedirectService;
        }
        #endregion

        #region Public Methods

        public virtual string GetUrlRedirectlist(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                UrlRedirectListModel list = _service.GetUrlRedirectList(Filters, Sorts, Page);
                //if Url redirect count greater then zero
                if (list?.UrlRedirects?.Count > 0)
                {
                    UrlRedirectResponse response = new UrlRedirectResponse { UrlRedirectList = list.UrlRedirects };
                    //Map data for pagination.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        public virtual string GetUrlRedirect(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                UrlRedirectModel urlRedirect = _service.GetUrlRedirect(Filters);
                //If urlRedirect is not null.
                if (HelperUtility.IsNotNull(urlRedirect))
                {
                    UrlRedirectResponse response = new UrlRedirectResponse { UrlRedirect = urlRedirect };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}