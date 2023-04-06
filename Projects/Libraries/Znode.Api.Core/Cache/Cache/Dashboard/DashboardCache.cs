using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class DashboardCache : BaseCache, IDashboardCache
    {
        #region private variables
        private readonly IDashboardService _service;
        #endregion

        #region Constructor
        public DashboardCache(IDashboardService dashboardService)
        {
            _service = dashboardService;
        }
        #endregion

        #region public methods
        //Gets all the top brands data
        public string GetDashboardTopBrands(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardTopItemsListModel list = _service.GetDashboardTopBrands(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardListResponse response = new DashboardListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Gets all the top categories data
        public string GetDashboardTopCategories(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardTopItemsListModel list = _service.GetDashboardTopCategories(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardListResponse response = new DashboardListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Gets all the top products data
        public string GetDashboardTopProducts(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardTopItemsListModel list = _service.GetDashboardTopProducts(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardListResponse response = new DashboardListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Gets all the top searches data
        public string GetDashboardTopSearches(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardTopItemsListModel list = _service.GetDashboardTopSearches(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardListResponse response = new DashboardListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //gets the total sales, orders, new customers and avg orders data 
        public string GetDashboardSalesDetails(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardTopItemsListModel list = _service.GetDashboardSalesDetails(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardListResponse response = new DashboardListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
        
        // Gets dashboard low inventory product count
        public string GetDashboardLowInventoryProductCount(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardTopItemsListModel list = _service.GetDashboardLowInventoryProductCount(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardListResponse response = new DashboardListResponse { TopItems = list.TopItemsList };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Gets list of Quotes
        public string GetDashboardQuotes(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardItemsListModel list = _service.GetDashboardQuotes(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardItemsListResponse response = new DashboardItemsListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
               
        //Gets the list of Orders
        public string GetDashboardOrders(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardItemsListModel list = _service.GetDashboardOrders(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardItemsListResponse response = new DashboardItemsListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Gets the list of Returns
        public string GetDashboardReturns(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardItemsListModel list = _service.GetDashboardReturns(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardItemsListResponse response = new DashboardItemsListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Gets the list of top accounts
        public string GetDashboardTopAccounts(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardItemsListModel list = _service.GetDashboardTopAccounts(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardItemsListResponse response = new DashboardItemsListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Gets the dasboard details
        public string GetDashboardDetails(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardItemsListModel list = _service.GetDashboardDetails(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardItemsListResponse response = new DashboardItemsListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //gets the total sales, orders, new customers and avg orders data 
        public string GetDashboardSalesCountDetails(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardTopItemsListModel list = _service.GetDashboardSalesCountDetails(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardCountDetailsResponse response = new DashboardCountDetailsResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Gets the sale details
        public string GetDashboardSaleDetails(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardItemsListModel list = _service.GetDashboardSalesInfo(Filters, Sorts, Page);
                if (list?.TopItemsList.Count > 0)
                {
                    DashboardItemsListResponse response = new DashboardItemsListResponse { TopItems = list.TopItemsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Gets account and store list
        public string GetAccountAndStoreList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DashboardDropDownListModel list = _service.GetAccountAndStoreList(Filters, Sorts, Page);
                if (list?.DashboardDropDownList.Count > 0)
                {
                    DashboardDropDownListResponse response = new DashboardDropDownListResponse { DashboardDropDown = list.DashboardDropDownList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
        #endregion
    }
}
