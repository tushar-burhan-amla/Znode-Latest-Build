using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class CMSSearchConfigurationController : BaseController
    {
        private readonly ICMSPageSearchService _cmsPageService;
        private readonly ICMSPageSearchCache _cmsPagecache;

        public CMSSearchConfigurationController(ICMSPageSearchService cmsPageService)
        {
            _cmsPageService = cmsPageService;
            _cmsPagecache = new CMSPageSearchCache(_cmsPageService);
        }

        #region CMS index
        /// <summary>
        /// Create index for CMS pages.
        /// </summary>
        /// <param name="cmsPortalContentPageIndexModel">Cms portal content pages index model </param>
        /// <returns>The CMS portal contentpages index data</returns>
        [ResponseType(typeof(CMSPortalContentPageIndexResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage InsertCreateCmsPageIndexData([FromBody]CMSPortalContentPageIndexModel cmsPortalContentPageIndexModel)
        {
            HttpResponseMessage response;
            try
            {
                CMSPortalContentPageIndexModel cmsPageIndexData = _cmsPageService.InsertCreateCmsPageIndexDataByRevisionTypes(cmsPortalContentPageIndexModel);
                if (HelperUtility.IsNotNull(cmsPageIndexData))
                {
                    response = CreateCreatedResponse(new CMSPortalContentPageIndexResponse { CMSPortalContentPageIndex = cmsPageIndexData });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(cmsPageIndexData.CMSSearchIndexId)));
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSPortalContentPageIndexResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSPortalContentPageIndexResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of CMS page search index monitor list.
        /// </summary>
        /// <returns>List of search index monitor.</returns>
        [ResponseType(typeof(CMSSearchIndexMonitorListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCmsPageSearchIndexMonitorlist()
        {
            HttpResponseMessage response;
            try
            {
                Func<string> method = () => _cmsPagecache.GetCmsPageSearchIndexMonitorList(RouteUri, RouteTemplate);
                return CreateResponse<CMSSearchIndexMonitorListResponse>(method, ZnodeLogging.Components.Search.ToString());
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new CMSSearchIndexMonitorListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get Cms Page search index data.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(CMSPortalContentPageIndexResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCmsPageIndexdata()
        {
            HttpResponseMessage response;

            try
            {
                Func<string> method = () => _cmsPagecache.GetCmsPageIndexData(RouteUri, RouteTemplate);
                return CreateResponse<CMSPortalContentPageIndexResponse>(method, ZnodeLogging.Components.Search.ToString());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new CMSPortalContentPageIndexResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new CMSPortalContentPageIndexResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region CMS page search request

        /// <summary>
        /// Perform cms page full text keyword search.
        /// </summary>
        /// <param name="model">The model of the keyword search.</param>
        /// <returns>Return cms page from elastic search for search keyword</returns>
        [ResponseType(typeof(CMSKeywordSearchResponse))]
        [HttpPost]
        public virtual HttpResponseMessage FullTextContentPageSearch([FromBody] CMSPageSearchRequestModel model)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cmsPagecache.FullTextContentPageSearch(model, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CMSKeywordSearchResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                CMSKeywordSearchResponse keywordSearch = new CMSKeywordSearchResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(keywordSearch);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                CMSKeywordSearchResponse keywordSearch = new CMSKeywordSearchResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(keywordSearch);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion
    }
}
