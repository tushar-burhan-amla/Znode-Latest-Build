using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class PortalCountryCache : BaseCache, IPortalCountryCache
    {
        #region Private Variable
        private readonly IPortalCountryService _service;
        #endregion

        #region Constructor
        public PortalCountryCache(IPortalCountryService portalCountryService)
        {
            _service = portalCountryService;
        }
        #endregion

        #region Public Methods
        #region Country Association
        //Get a list of unassociated countries to store.
        public virtual string GetUnAssociatedCountryList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get country list
                CountryListModel list = _service.GetUnAssociatedCountryList(Expands, Filters, Sorts, Page);
                if (list?.Countries?.Count > 0)
                {
                    //Create response.
                    CountryListResponse response = new CountryListResponse { Countries = list.Countries };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of associated countries based on portal.
        public virtual string GetAssociatedCountryList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get country list
                CountryListModel list = _service.GetAssociatedCountryList(Expands, Filters, Sorts, Page);
                if (list?.Countries?.Count > 0)
                {
                    //Create response.
                    CountryListResponse response = new CountryListResponse { Countries = list.Countries };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
        #endregion
    }
}