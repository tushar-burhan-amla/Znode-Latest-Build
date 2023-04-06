using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class CSSClient : BaseClient, ICSSClient
    {
        #region Public Methods
        public virtual CSSListModel GetCSSs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CSSEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CSSListResponse response = GetResourceFromEndpoint<CSSListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CSSListModel list = new CSSListModel { CSSs = response?.CSSs };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        public virtual CSSModel GetCSS(int cssId)
        {
            string endpoint = CSSEndpoint.Get(cssId);

            ApiStatus status = new ApiStatus();
            CSSResponse response = GetResourceFromEndpoint<CSSResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.CSS;
        }

        public virtual CSSModel CreateCSS(CSSModel model)
        {
            string endpoint = CSSEndpoint.Create();

            ApiStatus status = new ApiStatus();
            CSSResponse response = PostResourceToEndpoint<CSSResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CSS;
        }

        public virtual CSSModel UpdateCSS(CSSModel model)
        {
            string endpoint = CSSEndpoint.Update();

            ApiStatus status = new ApiStatus();
            CSSResponse response = PutResourceToEndpoint<CSSResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.CSS;
        }

        public virtual bool DeleteCSS(string cssIds)
        {
            string endpoint = CSSEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse deleted = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel { Ids = cssIds }), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return deleted.IsSuccess;
        }

        public virtual CSSListModel GetCssListByThemeId(int cmsThemeId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CSSEndpoint.GetCssListByThemeId(cmsThemeId);
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CSSListResponse response = GetResourceFromEndpoint<CSSListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CSSListModel list = new CSSListModel { CSSs = response?.CSSs, CMSThemeName = response?.CMSThemeName };
            list.MapPagingDataFromResponse(response);

            return list;
        }
        #endregion
    }
}
