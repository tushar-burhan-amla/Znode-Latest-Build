using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class CityCache : BaseCache, ICityCache
    {
        #region Private Variables
        private readonly ICityService _service;
        #endregion

        #region Constructor
        public CityCache(ICityService cityService)
        {
            _service = cityService;
        }
        #endregion

        #region Public Methods

        //Get a list of all cities.
        public virtual string GetCityList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CityListModel list = _service.GetCityList(Filters, Sorts, Page);
                if (list?.Cities?.Count > 0)
                {
                    //Create response.
                    CityListResponse response = new CityListResponse { Cities = list.Cities };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}