using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class ThemeCache : BaseCache, IThemeCache
    {
        #region Private Variable
        private readonly IThemeService _service;
        #endregion

        #region Constructor
        public ThemeCache(IThemeService themeService)
        {
            _service = themeService;
        }
        #endregion

        #region Public Methods

        #region Theme Configuration
        //Gets theme List.
        public virtual string GetThemes(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ThemeListModel list = _service.GetThemes(Filters, Sorts, Page);
                if (list?.Themes?.Count > 0)
                {
                    ThemeListResponse response = new ThemeListResponse { Themes = list.Themes };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get theme by themeId.
        public virtual string GetTheme(int themeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                ThemeModel theme = _service.GetTheme(themeId);
                if (IsNotNull(theme))
                {
                    ThemeResponse response = new ThemeResponse { Theme = theme };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Associate Store

        //Get associated store list for cms theme.
        public virtual string GetAssociatedStoreList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PricePortalListModel list = _service.GetAssociatedStoreList(Expands, Filters, Sorts, Page);
                if (list?.PricePortalList?.Count > 0)
                {
                    PricePortalListResponse response = new PricePortalListResponse { PricePortalList = list.PricePortalList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of unassociated stores.
        public virtual string GetUnAssociatedStoreList(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PortalListModel list = _service.GetUnAssociatedStoreList(Filters, Sorts, Page);
                if (list?.PortalList?.Count > 0)
                {
                    //Create Response
                    PortalListResponse response = new PortalListResponse { PortalList = list.PortalList };

                    //Apply Pagination Parameters
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region CMS Widgets     
        //Get area list for theme.
        public virtual string GetAreas(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSAreaListModel list = _service.GetAreas(Filters, Sorts, Page);
                if (list?.CMSAreas?.Count > 0)
                {
                    CMSAreaListResponse response = new CMSAreaListResponse { CMSAreas = list.CMSAreas };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
        #endregion
        #endregion
    }
}