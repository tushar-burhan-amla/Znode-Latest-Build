using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
namespace Znode.Engine.Api.Cache
{
    public class RMAConfigurationCache : BaseCache, IRMAConfigurationCache
    {
        #region Private Variable
        private readonly IRMAConfigurationService _service;
        #endregion

        #region Public Constructor
        public RMAConfigurationCache(IRMAConfigurationService Service)
        {
            _service = Service;
        }
        #endregion

        #region Public Method
        #region RMA Configuration
        //Get RMA Configuration by rma configuration id.
        public virtual string GetRMAConfiguration(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                RMAConfigurationModel rmaConfiguration = _service.GetRMAConfiguration();
                if (HelperUtility.IsNotNull(rmaConfiguration))
                    data = InsertIntoCache(routeUri, routeTemplate, new RMAConfigurationResponse { RMAConfiguration = rmaConfiguration });
                
            }
            return data;
        }
        #endregion

        #region Request Status
        //Get request status list.
        public virtual string GetRequestStatusList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                RequestStatusListModel list = _service.GetRequestStatusList(Expands, Filters, Sorts, Page);
                if (list?.RequestStatusList?.Count > 0)
                {
                    //Create response.
                    RequestStatusListResponse response = new RequestStatusListResponse { RequestStatusList = list.RequestStatusList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get request status by requestStatusId.
        public virtual string GetRequestStatus(int rmaRequestStatusId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                RequestStatusModel requestStatus = _service.GetRequestStatus(rmaRequestStatusId);
                if (HelperUtility.IsNotNull(requestStatus))
                    //Create response.
                    data = InsertIntoCache(routeUri, routeTemplate, new RequestStatusResponse { RequestStatus = requestStatus });
            }
            return data;
        }
        #endregion

        #region Return For Request.
        //Get return for return list.
        public virtual string GetReasonForReturnList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                RequestStatusListModel list = _service.GetReasonForReturnList(Expands, Filters, Sorts, Page);
                if (list?.RequestStatusList?.Count > 0)
                {
                    //Create response.
                    RequestStatusListResponse response = new RequestStatusListResponse { RequestStatusList = list.RequestStatusList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get return for return by rmaReasonForReturnId.
        public virtual string GetReasonForReturn(int rmaReasonForReturnId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                RequestStatusModel reasonForReturn = _service.GetReasonForReturn(rmaReasonForReturnId);
                if (HelperUtility.IsNotNull(reasonForReturn))
                    //Create response.
                    data = InsertIntoCache(routeUri, routeTemplate, new RequestStatusResponse { RequestStatus = reasonForReturn });
            }
            return data;
        }
        #endregion
        #endregion
    }
}