using System;
using System.Diagnostics;
using System.Web;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore
{
    public class SessionProxyHelper
    {
        // To Check whether Current login user.
        public static bool IsLoginUser() => Equals(SessionHelper.GetDataFromSession<UserViewModel>(WebStoreConstants.UserAccountKey), null) ? false : true;

        //Get user approver details.
        public static ApproverDetailsModel GetApproverDetails(int userId)
        {
            ApproverDetailsModel approverDetailsModel = SessionHelper.GetDataFromSession<ApproverDetailsModel>(WebStoreConstants.UserApproverKey);
            try
            {

                if (Equals(approverDetailsModel, null))
                {
                    approverDetailsModel = new ApproverDetailsModel();
                    AccountQuoteClient client = new AccountQuoteClient();
                    ApproverDetailsModel userApproverDetailsModel = client.UserApproverDetails(userId);
                    if (!Equals(userApproverDetailsModel, null))
                    {
                        approverDetailsModel.IsApprover = userApproverDetailsModel.IsApprover;
                        approverDetailsModel.HasApprovers = userApproverDetailsModel.HasApprovers;
                    }

                    SessionHelper.SaveDataInSession<ApproverDetailsModel>(WebStoreConstants.UserApproverKey, approverDetailsModel);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return approverDetailsModel;
        }

        //Get billing account number.
        public static string GetBillingAccountNumber(int userId)
        {
            string billingAccountNumber = SessionHelper.GetDataFromSession<string>(WebStoreConstants.BillingAccountNumber);
            try
            {
                if (Equals(billingAccountNumber, null))
                {
                    AccountQuoteClient client = new AccountQuoteClient();
                    billingAccountNumber = client.GetBillingAccountNumber(userId);
                    SessionHelper.SaveDataInSession<string>(WebStoreConstants.BillingAccountNumber, billingAccountNumber);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return billingAccountNumber;
        }
        //Save the logged in username in Session to validate the same against the Forms authentication cookie session data.
        public static void SetAuthenticatedUserName(string userName)
        {
            SessionHelper.SaveDataInSession<string>(WebStoreConstants.AuthUserName, userName);
        }
        public static void RemoveAuthenticatedUserSession()
        {
            SessionHelper.RemoveDataFromSession(WebStoreConstants.AuthUserName);
        }
        public static string GetAuthenticatedUserData()
        {
            return SessionHelper.GetDataFromSession<string>(WebStoreConstants.AuthUserName);
        }

        //Get pending payments and pending orders count for showing account menus
        public static UserDashboardPendingOrdersModel GetUserDashboardPendingOrderDetailsCount(UserViewModel userViewModel)
        {
            UserDashboardPendingOrdersModel pendingOrdersModel = SessionHelper.GetDataFromSession<UserDashboardPendingOrdersModel>(ZnodeConstant.PendingQuotesKey + userViewModel.UserId);
            try
            {
                if (Equals(pendingOrdersModel, null))
                {
                    pendingOrdersModel = new UserDashboardPendingOrdersModel();
                    AccountQuoteClient client = new AccountQuoteClient();
                    FilterCollection filters = new FilterCollection();
                    SetUserIdFilter(filters, userViewModel.UserId, userViewModel.AccountId.GetValueOrDefault());
                    UserDashboardPendingOrdersModel pendingQuotesModel = client.GetUserDashboardPendingOrderDetailsCount(filters);
                    if (!Equals(pendingQuotesModel, null))
                    {
                        pendingOrdersModel.PendingOrdersCount = pendingQuotesModel.PendingOrdersCount;
                        pendingOrdersModel.PendingPaymentsCount = pendingQuotesModel.PendingPaymentsCount;
                    }

                    SessionHelper.SaveDataInSession(ZnodeConstant.PendingQuotesKey + userViewModel.UserId, pendingOrdersModel);
                }
                else return pendingOrdersModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return pendingOrdersModel;
        }

        //Set filter for User Id.
        public static void SetUserIdFilter(FilterCollection filters, int userId, int accountId)
        {

            //If UserId or WebStoreQuote Already present in filters Remove It.
            filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());
            filters.RemoveAll(x => x.Item1 == ZnodeConstant.WebStoreQuotes);

            //Add New UserId Into filters
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            filters.Add(new FilterTuple(ZnodeConstant.WebStoreQuotes, FilterOperators.Equals, ZnodeConstant.TrueValue));

            //Checking For AccountId already Exists in Filters Or Not 
            if (filters.Exists(x => x.Item1 == ZnodeAccountEnum.AccountId.ToString()))
                //If AccountId Already present in filters Remove It.
                filters.RemoveAll(x => x.Item1 == ZnodeAccountEnum.AccountId.ToString());

            if (accountId > 0)
                //Add New AccountId Into filters
                filters.Add(new FilterTuple(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString()));
        }
    }
}