using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class CurrencyCache : BaseCache, ICurrencyCache
    {
        #region Private Variables
        private readonly ICurrencyService _service;
        #endregion

        #region Constructor
        public CurrencyCache(ICurrencyService currencyService)
        {
            _service = currencyService;
        }
        #endregion

        #region Public Methods

        //Get a list of all currencies.
        public virtual string GetCurrencies(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CurrencyListModel list = _service.GetCurrencies(Expands, Filters, Sorts, Page);
                if (list?.Currencies?.Count > 0)
                {
                    //Create response.
                    CurrencyListResponse response = new CurrencyListResponse { Currencies = list.Currencies };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetCurrencyCultureList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CurrencyListModel list = _service.CurrencyCultureList(Expands, Filters, Sorts, Page);
                if (list?.Currencies?.Count > 0)
                {
                    //Create response.
                    CurrencyListResponse response = new CurrencyListResponse { Currencies = list.Currencies };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get a list of all cultures.
        public virtual string GetCulture(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CultureListModel list = _service.GetCulture(Expands, Filters, Sorts, Page);
                if (list?.Culture?.Count > 0)
                {
                    //Create response.
                    CultureListResponse response = new CultureListResponse { Culture = list.Culture };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get catalog as per filter passed.
        public virtual string GetCurrency(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CurrencyModel currency = _service.GetCurrency(Filters);
                if (!Equals(currency, null))
                {
                    CurrencyResponse response = new CurrencyResponse { Currency = currency };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get catalog as per filter passed.
        public virtual string GetCultureCode(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CultureModel culture = _service.GetCultureCode(Filters);
                if (!Equals(culture, null))
                {
                    CultureResponse response = new CultureResponse { Culture = culture };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}