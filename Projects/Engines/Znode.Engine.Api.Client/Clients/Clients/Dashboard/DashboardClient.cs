using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class DashboardClient : BaseClient, IDashboardClient
    {
        //Gets the top brands list
        public virtual async Task<DashboardTopItemsListModel> GetDashboardTopBrandsList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardTopBrandsList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardListResponse response = GetResourceFromEndpoint<DashboardListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardTopItemsListModel topItemsList = new DashboardTopItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);

            return topItemsList;
        }

        //Gets top categories list
        public virtual async Task<DashboardTopItemsListModel> GetDashboardTopCategoriesList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardTopCategoriesList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardListResponse response = GetResourceFromEndpoint<DashboardListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardTopItemsListModel topItemsList = new DashboardTopItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);

            return topItemsList;
        }

        //Gets top products list
        public virtual async Task<DashboardTopItemsListModel> GetDashboardTopProductsList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardTopProductList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardListResponse response = GetResourceFromEndpoint<DashboardListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardTopItemsListModel topItemsList = new DashboardTopItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);

            return topItemsList;
        }

        //Gets top searches list
        public virtual async Task<DashboardTopItemsListModel> GetDashboardTopSearchesList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardTopSearchesList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardListResponse response = GetResourceFromEndpoint<DashboardListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardTopItemsListModel topItemsList = new DashboardTopItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);
            return topItemsList;
        }

        //Gets total sales, total orders, total customers and average orders
        public virtual async Task<DashboardTopItemsListModel> GetDashboardSalesDetails(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardSalesDetails();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardListResponse response = GetResourceFromEndpoint<DashboardListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardTopItemsListModel topItemsList = new DashboardTopItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);

            return topItemsList;
        }

        //Gets the count of low inventory products
        public virtual async Task<DashboardTopItemsListModel> GetDashboardLowInventoryProductCount(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardLowInventoryProductsCount();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardListResponse response = GetResourceFromEndpoint<DashboardListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardTopItemsListModel lowInventoryProductModel = new DashboardTopItemsListModel { TopItemsList = response?.TopItems };
            
            return lowInventoryProductModel;
        }

        public virtual async Task<DashboardItemsListModel> GetDashboardQuotes(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardQuotes();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardItemsListResponse response = GetResourceFromEndpoint<DashboardItemsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardItemsListModel topItemsList = new DashboardItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);
            return topItemsList;
        }

        //Gets orders list
        public virtual async Task<DashboardItemsListModel> GetDashboardOrders(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardOrders();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardItemsListResponse response = GetResourceFromEndpoint<DashboardItemsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardItemsListModel topItemsList = new DashboardItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);
            return topItemsList;
        }

        //Gets returns list
        public virtual async Task<DashboardItemsListModel> GetDashboardReturns(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardReturns();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardItemsListResponse response = GetResourceFromEndpoint<DashboardItemsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardItemsListModel topItemsList = new DashboardItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);
            return topItemsList;
        }
        
        //Gets top accounts list
        public virtual async Task<DashboardItemsListModel> GetDashboardTopAccounts(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardTopAccounts();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardItemsListResponse response = GetResourceFromEndpoint<DashboardItemsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardItemsListModel topItemsList = new DashboardItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);
            return topItemsList;
        }

        //Gets total sales, total orders, count of quotes and count of returns
        public virtual async Task<DashboardItemsListModel> GetDashboardDetails(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardDetails();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardItemsListResponse response = GetResourceFromEndpoint<DashboardItemsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardItemsListModel topItemsList = new DashboardItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);

            return topItemsList;
        }
        //Gets total sales, total orders, total customers and average orders
        public virtual async Task<DashboardTopItemsListModel> GetDashboardSalesCountDetails(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardSalesCountDetails();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardCountDetailsResponse response = GetResourceFromEndpoint<DashboardCountDetailsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardTopItemsListModel topItemsList = new DashboardTopItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);

            return topItemsList;
        }

        //Gets sales details
        public virtual async Task<DashboardItemsListModel> GetDashboardSaleDetails(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetDashboardSaleDetails();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardItemsListResponse response = GetResourceFromEndpoint<DashboardItemsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardItemsListModel topItemsList = new DashboardItemsListModel { TopItemsList = response?.TopItems };
            topItemsList.MapPagingDataFromResponse(response);
            return topItemsList;
        }

        //Gets account and store details
        public virtual DashboardDropDownListModel GetAccountAndStoreList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = DashboardEndpoint.GetAccountAndStoreList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DashboardDropDownListResponse response = GetResourceFromEndpoint<DashboardDropDownListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DashboardDropDownListModel dashboardDropDownList = new DashboardDropDownListModel { DashboardDropDownList = response?.DashboardDropDown };
            dashboardDropDownList.MapPagingDataFromResponse(response);
            return dashboardDropDownList;
        }

    }
}
