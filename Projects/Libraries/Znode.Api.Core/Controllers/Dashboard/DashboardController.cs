using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class DashboardController : BaseController
    {
        #region private variables
        private readonly IDashboardCache _cache;
        #endregion

        #region constructor
        public DashboardController(IDashboardService service)
        {
            _cache = new DashboardCache(service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get list of dashboard top brands
        /// </summary>
        /// <returns>Returns list of dashboard top brands.</returns>
        [ResponseType(typeof(DashboardListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardTopBrandsList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardTopBrands(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of dashboard top categories
        /// </summary>
        /// <returns>Returns list of dashboard top categories.</returns>
        [ResponseType(typeof(DashboardListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardTopCategoriesList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardTopCategories(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of dashboard top products
        /// </summary>
        /// <returns>Returns list of dashboard top products.</returns>
        [ResponseType(typeof(DashboardListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardTopProductsList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardTopProducts(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of dashboard top searches
        /// </summary>
        /// <returns>Returns list of dashboard top searches.</returns>
        [ResponseType(typeof(DashboardListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardTopSearchesList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardTopSearches(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of total sales, orders, new customers and avg orders
        /// </summary>
        /// <returns>Returns total sales, orders, new customers and avg orders.</returns>
        [ResponseType(typeof(DashboardListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardSalesDetails()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardSalesDetails(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of total sales, orders, new customers and avg orders
        /// </summary>
        /// <returns>Returns total sales, orders, new customers and avg orders.</returns>
        [ResponseType(typeof(DashboardCountDetailsResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardSalesCountDetails()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardSalesCountDetails(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardCountDetailsResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardCountDetailsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets dashboard low inventory product count
        /// </summary>
        /// <returns>Returns dashboard low inventory product count.</returns>
        [ResponseType(typeof(DashboardListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardLowInventoryProductCount()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardLowInventoryProductCount(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(),TraceLevel.Error);
            }
            return response;
        }
        /// <summary>
        /// Get list of dashboard Quotes
        /// </summary>
        /// <returns>Returns list of Quotes.</returns>
        [ResponseType(typeof(DashboardItemsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardQuotes()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardQuotes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardItemsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardItemsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of orders
        /// </summary>
        /// <returns>Returns list of orders.</returns>
        [ResponseType(typeof(DashboardItemsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardOrders()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardOrders(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardItemsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardItemsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of returns
        /// </summary>
        /// <returns>Returns list of returns.</returns>
        [ResponseType(typeof(DashboardItemsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardReturns()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardReturns(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardItemsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardItemsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of top accounts
        /// </summary>
        /// <returns>Returns list of top accounts.</returns>
        [ResponseType(typeof(DashboardItemsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardTopAccounts()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardTopAccounts(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardItemsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardItemsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get dashboard details
        /// </summary>
        /// <returns>Returns dashboard details.</returns>
        [ResponseType(typeof(DashboardItemsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardDetails()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardDetails(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardItemsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardItemsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get sales details
        /// </summary>
        /// <returns>Returns sales details.</returns>
        [ResponseType(typeof(DashboardItemsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDashboardSaleDetails()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDashboardSaleDetails(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardItemsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardItemsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Get account and store list
        /// </summary>
        /// <returns>Returns account and store list.</returns>
        [ResponseType(typeof(DashboardDropDownListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountAndStoreList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAccountAndStoreList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DashboardItemsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DashboardItemsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }
            return response;
        }


        #endregion
    }
}
