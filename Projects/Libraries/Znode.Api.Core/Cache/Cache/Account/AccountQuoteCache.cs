using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class AccountQuoteCache : BaseCache, IAccountQuoteCache
    {
        private readonly IAccountQuoteService _service;

        public AccountQuoteCache(IAccountQuoteService accountQuoteService)
        {
            _service = accountQuoteService;
        }
        #region Public Methods
        //Get account quote details by omsQuoteId.
        public virtual string GetAccountQuote(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AccountQuoteModel accountQuoteModel = _service.GetAccountQuote(Filters, Expands);
                if (HelperUtility.IsNotNull(accountQuoteModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new AccountQuoteResponse { AccountQuote = accountQuoteModel });
            }
            return data;
        }

        //Get account quote list.
        public virtual string GetAccountQuoteList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AccountQuoteListModel accountQuoteListModel = _service.GetAccountQuoteList(Expands, Filters, Sorts, Page);

                //Check AccountQuoteListModel is null or not, does not check for count because list model has other properties.
                if (HelperUtility.IsNotNull(accountQuoteListModel))
                {
                    AccountQuoteListResponse response = new AccountQuoteListResponse { AccountQuotes = accountQuoteListModel };
                    response.MapPagingDataFromModel(accountQuoteListModel);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get user approver list.
        public virtual string GetUserApproverList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                UserApproverListModel userApproverListModel = _service.GetUserApproverList(Expands, Filters, Sorts, Page);

                if (HelperUtility.IsNotNull(userApproverListModel))
                {
                    UserApproverListResponse response = new UserApproverListResponse { UserApproverList = userApproverListModel };
                    response.MapPagingDataFromModel(userApproverListModel);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get template list.
        public virtual string GetTemplateList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AccountTemplateListModel accountTemplateListModel = _service.GetTemplateList(Expands, Filters, Sorts, Page);

                //Check accountTemplateListModel is null or not, does not check for count because list model has other properties.
                if (HelperUtility.IsNotNull(accountTemplateListModel))
                {
                    AccountTemplateListResponse response = new AccountTemplateListResponse { AccountTemplates = accountTemplateListModel };
                    response.MapPagingDataFromModel(accountTemplateListModel);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Method to check if the current user is an approver to any other user and has approvers itself.
        public virtual string UserApproverDetails(int userId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ApproverDetailsModel approverDetailsModel = _service.UserApproverDetails(userId);
                if (HelperUtility.IsNotNull(approverDetailsModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new ApproverDetailsResponse { ApproverDetails = approverDetailsModel });
            }
            return data;
        }

        //Method to check if the current user is an approver to any other user and has approvers itself.
        public virtual string GetUserDashboardPendingOrderDetailsCount(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                UserDashboardPendingOrdersModel pendingOrderDetailsCountModel = _service.GetUserDashboardPendingOrderDetailsCount(Filters);
                if (HelperUtility.IsNotNull(pendingOrderDetailsCountModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new UserDashboardPendingOrdersResponse { PendingOrderDetailsCount = pendingOrderDetailsCountModel });
            }
            return data;
        }

        #region Template
        //Get account quote details by omsTemplateId.
        public virtual string GetAccountTemplate(int omsTemplateId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AccountTemplateModel accountTemplateModel = _service.GetAccountTemplate(omsTemplateId, Expands, Filters);
                if (HelperUtility.IsNotNull(accountTemplateModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new AccountQuoteResponse { AccountTemplate = accountTemplateModel });
            }
            return data;
        }
        #endregion
        #endregion
    }
}