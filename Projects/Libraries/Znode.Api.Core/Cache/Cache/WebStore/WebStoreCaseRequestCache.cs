using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class WebStoreCaseRequestCache : BaseCache, IWebStoreCaseRequestCache
    {
        #region Private Variable
        private readonly IWebStoreCaseRequestService _service;
        #endregion

        #region Constructor
        public WebStoreCaseRequestCache(IWebStoreCaseRequestService caseRequestService)
        {
            _service = caseRequestService;
        }
        #endregion

        #region Public Methods

        #region Case Request.
        //Get the list of case request.
        public virtual string GetCaseRequests(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreCaseRequestListModel list = _service.GetCaseRequests(Expands,Filters, Sorts, Page);
                if (list?.CaseRequestList?.Count > 0)
                {
                    WebStoreCaseRequestListResponse response = new WebStoreCaseRequestListResponse { CaseRequests = list.CaseRequestList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get case request on the basis of caseRequestId.
        public virtual string GetCaseRequest(int caseRequestId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                WebStoreCaseRequestModel caseRequestModel = _service.GetCaseRequest(caseRequestId);
                if (HelperUtility.IsNotNull(caseRequestModel))
                {
                    WebStoreCaseRequestResponse response = new WebStoreCaseRequestResponse { CaseRequest = caseRequestModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get the list of case type.
        public virtual string GetCaseTypeList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CaseTypeListModel list = _service.GetCaseTypeList();
                if (list?.CaseTypes?.Count > 0)
                {
                    CaseTypeListResponse response = new CaseTypeListResponse { CaseTypes = list.CaseTypes };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get the list of case status.
        public virtual string GetCaseStatusList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CaseStatusListModel list = _service.GetCaseStatusList();
                if (list?.CaseStatuses?.Count > 0)
                {
                    CaseStatusListResponse response = new CaseStatusListResponse {  CaseStatus= list.CaseStatuses };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get the list of case priority.
        public virtual string GetCasePriorityList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CasePriorityListModel list = _service.GetCasePriorityList();
                if (list?.CasePriorities?.Count > 0)
                {
                    CasePriorityListResponse response = new CasePriorityListResponse { CasePriorities = list.CasePriorities };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
        #endregion

        #endregion
    }
}