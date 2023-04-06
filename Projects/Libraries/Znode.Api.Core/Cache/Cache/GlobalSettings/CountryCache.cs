using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class CountryCache : BaseCache, ICountryCache
    {
        #region Private Variables
        private readonly ICountryService _service;
        #endregion

        #region Constructor
        public CountryCache(ICountryService countryService)
        {
            _service = countryService;
        }
        #endregion

        #region Public Methods

        //Get a list of all countries.
        public virtual string GetCountries(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CountryListModel list = _service.GetCountries(Expands, Filters, Sorts, Page);
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

        //Get country as per filter passed.
        public virtual string GetCountry(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CountryModel country = _service.GetCountry(Filters);
                if (!Equals(country, null))
                {
                    CountryResponse response = new CountryResponse { Country = country };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}