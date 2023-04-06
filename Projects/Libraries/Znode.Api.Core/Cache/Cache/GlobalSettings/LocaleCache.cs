using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class LocaleCache : BaseCache, ILocaleCache
    {
        #region private Data Member
        private readonly ILocaleService _service;
        #endregion

        #region Public Constructor
        public LocaleCache(ILocaleService localeService)
        {
            _service = localeService;
        }
        #endregion

        #region Public Methods
        //Get Locale List
        public virtual string GetLocaleList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Menu list
                LocaleListModel list = _service.GetLocaleList(Expands, Filters, Sorts, Page);
                if (list?.Locales?.Count > 0)
                {
                    //Create response.
                    LocaleListResponse response = new LocaleListResponse { Locales = list.Locales };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Locale.
        public virtual string GetLocale(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                LocaleModel locale = _service.GetLocale(Filters);
                if (!Equals(locale, null))
                {
                    LocaleResponse response = new LocaleResponse { Locale = locale };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}