using Newtonsoft.Json;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Api.Cache
{
    public class RMARequestCache : BaseCache, IRMARequestCache
    {
        #region Private Variables
        private readonly IRMARequestService _service;
        #endregion

        #region Constructor
        public RMARequestCache(IRMARequestService rmaRequestService)
        {
            _service = rmaRequestService;
        }
        #endregion

        #region Public Methods
        public virtual string GetRMARequests(string routeUri, string routeTemplate)
        {
            var data = GetFromCache(routeUri);
            if (HelperUtility.IsNull(data))
            {
                RMARequestListModel list = _service.GetRMARequestList(Expands, Filters, Sorts, Page);
                if (IsNotNull(list.RMARequests))
                {
                    RMARequestListResponse response = new RMARequestListResponse { RMARequestList = list.RMARequests };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        public virtual string GetRMARequest(int rmaRequestId, string routeUri, string routeTemplate)
        {
            var data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                RMARequestModel rmaRequest = _service.GetRMARequest(rmaRequestId);
                if (IsNotNull(rmaRequest))
                {
                    RMARequestResponse response = new RMARequestResponse { RMARequest = rmaRequest };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        public virtual string GetOrderRMAFlag(int omsOrderDetailsId, string routeUri, string routeTemplate)
        {
            string data = string.Empty;
            bool isEnabled = _service.GetOrderRMAFlag(omsOrderDetailsId);
            TrueFalseResponse response = new TrueFalseResponse { IsSuccess = isEnabled };
            data = JsonConvert.SerializeObject(response);
            return data;

        }

        //Get Issue gift card details
        public virtual string GetIssuedGiftCards(int rmaRequestId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (IsNull(data))
            {
                IssuedGiftCardListModel issuedGiftCardModels = _service.GetRMAGiftCardDetails(rmaRequestId);
                if (IsNotNull(issuedGiftCardModels))
                {
                    IssuedGiftCardListResponse response = new IssuedGiftCardListResponse { IssuedGiftCards = issuedGiftCardModels.IssuedGiftCardModels };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        // Get list of service requests according to parameters
        public virtual string GetServiceRequestReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportServiceRequestListResponse response = new ReportServiceRequestListResponse() { ServiceRequestList = _service.GetServiceRequestReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }
        #endregion
    }
}