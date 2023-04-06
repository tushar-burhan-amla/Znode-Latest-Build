using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class ThemeClient : BaseClient, IThemeClient
    {
        #region Theme Configuration
        //Get theme list.
        public virtual ThemeListModel GetThemes(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ThemeEndpoint.List();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ThemeListResponse response = GetResourceFromEndpoint<ThemeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ThemeListModel list = new ThemeListModel { Themes = response?.Themes };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create theme.
        public virtual ThemeModel CreateTheme(ThemeModel model)
        {
            string endpoint = ThemeEndpoint.Create();

            ApiStatus status = new ApiStatus();
            ThemeResponse response = PostResourceToEndpoint<ThemeResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Theme;
        }

        //Get theme by cmsThemeId
        public virtual ThemeModel GetTheme(int cmsThemeId)
        {
            string endpoint = ThemeEndpoint.GetTheme(cmsThemeId);

            ApiStatus status = new ApiStatus();
            ThemeResponse response = GetResourceFromEndpoint<ThemeResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Theme;
        }

        //Update theme.
        public virtual ThemeModel UpdateTheme(ThemeModel themeModel)
        {
            string endpoint = ThemeEndpoint.UpdateTheme();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ThemeResponse response = PutResourceToEndpoint<ThemeResponse>(endpoint, JsonConvert.SerializeObject(themeModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Theme;
        }

        //Delete theme which is not associated to store.
        public virtual bool DeleteTheme(string themeId)
        {
            string endpoint = ThemeEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse deleted = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel { Ids = themeId }), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return deleted.IsSuccess;
        }
        #endregion

        #region Associate Store

        public virtual bool AssociateStore(PricePortalListModel listModel)
        {
            string endpoint = ThemeEndpoint.AssociateStore();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(listModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        public virtual bool RemoveAssociatedStores(ParameterModel cmsPortalThemeId)
        {
            string endpoint = ThemeEndpoint.RemoveAssociatedStores();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsPortalThemeId), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #endregion

        #region CMS Widgets

        //Get CMS Area list.
        public virtual CMSAreaListModel GetAreas(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ThemeEndpoint.AreaList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CMSAreaListResponse response = GetResourceFromEndpoint<CMSAreaListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CMSAreaListModel list = new CMSAreaListModel { CMSAreas = response?.CMSAreas };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        #endregion
    }
}
