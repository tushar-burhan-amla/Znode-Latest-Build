using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Api.Core.Controllers
{
    public class SearchReportController : BaseController
    {
        #region Private Variables
        private readonly ISearchReportCache _cache;
        private readonly ISearchReportService _searchReportService;
        #endregion

        #region Public Constructor

        public SearchReportController(ISearchReportService searchReportService)
        {
            _searchReportService = searchReportService;
            _cache = new SearchReportCache(_searchReportService);
        }

        #endregion

        /// <summary>
        /// Get no result search keyword list
        /// </summary>
        /// <returns>List of no result search keywords</returns>
        [ResponseType(typeof(SearchReportListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetNoResultsFoundReport()
        {
            HttpResponseMessage response;

            try
            {
                Func<string> method = () => _cache.GetNoResultsFoundReport(RouteUri, RouteTemplate);
                return CreateResponse<SearchReportListResponse>(method, ZnodeLogging.Components.Search.ToString());
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchReportListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get top search keyword list.
        /// </summary>
        /// <returns>list of top search keywords</returns>
        [ResponseType(typeof(SearchReportListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetTopKeywordsReport()
        {
            HttpResponseMessage response;

            try
            {
                Func<string> method = () => _cache.GetTopKeywordsReport(RouteUri, RouteTemplate);
                return CreateResponse<SearchReportListResponse>(method, ZnodeLogging.Components.Search.ToString());
            }

            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchReportListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Save search report data.
        /// </summary>
        /// <param name="model">Model with the search report data</param>
        /// <returns>Updated model of search report</returns>
        [ResponseType(typeof(SearchReportResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SaveSearchReportData([FromBody] SearchReportModel model)
        {
            HttpResponseMessage response;
            try
            {
                SearchReportModel report = _searchReportService.SaveSearchReportData(model);

                if (HelperUtility.IsNotNull(report))
                {
                    response = CreateCreatedResponse(new SearchReportResponse { SearchReport = report });
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SearchReportResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SearchReportResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
}
