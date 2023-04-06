using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class DevExpressReportClient : BaseClient, IDevExpressReportClient
    {
        //Get report categories.
        public virtual ReportCategoryListModel GetReportCategories()
        {
            //Get Endpoint.
            string endpoint = DevExpressReportEndpoint.GetReportCategories();

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportCategoryListResponse response = GetResourceFromEndpoint<ReportCategoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Reports list.
            ReportCategoryListModel list = new ReportCategoryListModel { ReportCategoryList = response?.ReportCategoryList };

            return list;
        }

        //Get report details by report category Id.
        public virtual ReportDetailListModel GetReportDetails(int reportCategoryId)
        {
            //Get Endpoint.
            string endpoint = DevExpressReportEndpoint.GetReportDetails(reportCategoryId);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportDetailListResponse response = GetResourceFromEndpoint<ReportDetailListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Reports list.
            ReportDetailListModel list = new ReportDetailListModel { ReportDetailList = response?.ReportDetailList };

            return list;
        }

        //Get report setting by report code.
        public virtual ReportSettingModel GetReportSetting(string ReportCode)
        {
            //Get Endpoint.
            string endpoint = DevExpressReportEndpoint.GetReportSetting(ReportCode);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportSettingResponse response = GetResourceFromEndpoint<ReportSettingResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Reports list.
            ReportSettingModel reportSettingModel = response?.ReportSetting;

            return reportSettingModel;
        }

        //Get orders details list for report.
        public virtual List<ReportOrderDetailsModel> GetOrdersReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetOrders(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            OrderDetailsListResponse response = GetResourceFromEndpoint<OrderDetailsListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.OrderDetailsList;
        }

        //Get order items list by OmsOrderId.
        public virtual List<ReportOrderItemsDetailsModel> GetOrdersItemsReport(int OmsOrderId)
        {
            string endpoint = DevExpressReportEndpoint.GetOrdersItems(OmsOrderId);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportOrderItemsDetailsResponse response = GetResourceFromEndpoint<ReportOrderItemsDetailsResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.OrderDetailsList;
        }

        //Get coupons details list.
        public virtual List<ReportCouponModel> GetCouponReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetCoupon(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportCouponListResponse response = GetResourceFromEndpoint<ReportCouponListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.CouponList;
        }

        //Get sales tax list.
        public virtual List<ReportSalesTaxModel> GetSalesTaxReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetSalesTax(), filters);
            //Get response.
            ApiStatus status = new ApiStatus();
            ReportSalesTaxListResponse response = GetResourceFromEndpoint<ReportSalesTaxListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.ReportSalesTaxList;
        }

        //Get affiliate orders list.
        public virtual List<ReportAffiliateOrderModel> GetAffiliateOrderReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetAffiliateOrder(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportAffiliateOrderListResponse response = GetResourceFromEndpoint<ReportAffiliateOrderListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.ReportAffiliateOrderList;
        }

        //Get orders pick list.
        public virtual List<ReportOrderPickModel> GetOrderPickListReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetOrderPickList(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportOrderPickListResponse response = GetResourceFromEndpoint<ReportOrderPickListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.ReportOrderPickList;
        }

        //Get best seller products list.
        public virtual List<ReportBestSellerProductModel> GetBestSellerProductReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetBestSellerProduct(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportBestSellerProductResponse response = GetResourceFromEndpoint<ReportBestSellerProductResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.ReportBestSellerProductList;
        }

        //Get inventory list.
        public virtual List<ReportInventoryReorderModel> GetInventoryReorderReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetInventoryReorder(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportInventoryReorderListResponse response = GetResourceFromEndpoint<ReportInventoryReorderListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.ReportInventoryReorderList;
        }

        //Get popular search list.
        public virtual List<ReportPopularSearchModel> GetPopularSearchReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetPopularSearch(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportPopularSearchListResponse response = GetResourceFromEndpoint<ReportPopularSearchListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.ReportPopularSearchList;
        }

        //Get service requests.
        public virtual List<ReportServiceRequestModel> GetServiceRequestReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetServiceRequest(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportServiceRequestListResponse response = GetResourceFromEndpoint<ReportServiceRequestListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.ServiceRequestList;
        }

        //Get vendors list.
        public virtual List<ReportVendorsModel> GetVendorsReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetVendors(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportVendorsListResponse response = GetResourceFromEndpoint<ReportVendorsListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.ReportVendorsList;
        }

        //Get users list.
        public virtual List<ReportUsersModel> GetUsersReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetUsers(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportUsersListResponse response = GetResourceFromEndpoint<ReportUsersListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.UsersList;
        }

        //Get email opt customers list.
        public virtual List<ReportEmailOptInCustomerModel> GetEmailOptInCustomerReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetEmailOptInCustomer(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportEmailOptInCustomerListResponse response = GetResourceFromEndpoint<ReportEmailOptInCustomerListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.EmailOptInCustomerList;
        }

        //Get most frequent customers list.
        public virtual List<ReportMostFrequentCustomerModel> GetMostFrequentCustomerReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetMostFrequentCustomer(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportMostFrequentCustomerListResponse response = GetResourceFromEndpoint<ReportMostFrequentCustomerListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.FrequentCustomerList;
        }

        //Get top spending customers list.
        public virtual List<ReportTopSpendingCustomersModel> GetTopSpendingCustomersReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetTopSpendingCustomers(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportTopSpendingCustomersListResponse response = GetResourceFromEndpoint<ReportTopSpendingCustomersListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.SpendingCustomersList;
        }

        //Get Abandoned Cart list.
        public virtual List<ReportAbandonedCartModel> GetAbandonedCartReport(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetAbandonedCartReport(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportAbandonedCartListResponse response = GetResourceFromEndpoint<ReportAbandonedCartListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.AbandonedCartList;
        }

        //Get list of stores with currency.
        public virtual List<ReportStoresDetailsModel> GetStoresWithCurrency(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetStoresWithCurrency(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportStoresDetailsListResponse response = GetResourceFromEndpoint<ReportStoresDetailsListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.StoresDetails;
        }

        //Get list of order status.
        public virtual List<ReportOrderStatusModel> GetOrderStatus(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetOrderStatus(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportOrderStatusListResponse response = GetResourceFromEndpoint<ReportOrderStatusListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.OrderStatusList;
        }

        //Get list of discounts type.
        public virtual List<ReportDiscountTypeModel> GetDiscountType(FilterCollection filters = null)
        {
            string endpoint = CreateEndPoint(DevExpressReportEndpoint.GetDiscountType(), filters);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportDiscountTypeListResponse response = GetResourceFromEndpoint<ReportDiscountTypeListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.DiscountTypeList;
        }

        //Save report layout.
        public virtual ReportViewModel SaveReportIntoDatabase(ReportViewModel reportModel)
        {
            //Get endpoint having api url.
            string endpoint = DevExpressReportEndpoint.SaveReportIntoDatabase();

            ApiStatus status = new ApiStatus();
            ReportViewResponse response = PostResourceToEndpoint<ReportViewResponse>(endpoint, JsonConvert.SerializeObject(reportModel), status);

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ReportView;
        }

        //Get saved report layouts.
        public virtual List<ReportViewModel> LoadSavedReportLayout(ReportViewModel reportModel)
        {
            //Get endpoint having api url.
            string endpoint = DevExpressReportEndpoint.LoadSavedReportLayout();

            ApiStatus status = new ApiStatus();
            ReportViewListResponse response = PostResourceToEndpoint<ReportViewListResponse>(endpoint, JsonConvert.SerializeObject(reportModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ReportView;
        }

        //Get report layout by report name and code.
        public virtual ReportViewModel GetReportSaveLayout(string reportCode, string reportName)
        {
            //Get Endpoint.
            string endpoint = DevExpressReportEndpoint.GetReportSaveLayout(reportCode, reportName);

            //Get response.
            ApiStatus status = new ApiStatus();
            ReportViewResponse response = GetResourceFromEndpoint<ReportViewResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.ReportView;
        }

        //Delete saved report layout by report view Id.
        public virtual bool DeleteSavedReportLayout(int reportViewId)
        {
            //Get Endpoint.
            string endpoint = DevExpressReportEndpoint.DeleteSavedReportLayout();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(reportViewId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        private string CreateEndPoint(string endpointURL, FilterCollection filters)
        {
            return endpointURL += BuildEndpointQueryString(null, filters, null, null, null);
        }
    }
}
