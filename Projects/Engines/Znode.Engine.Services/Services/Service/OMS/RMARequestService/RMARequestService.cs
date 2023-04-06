using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class RMARequestService : BaseService, IRMARequestService
    {
        private readonly IZnodeRepository<ZnodeRmaRequest> _rmaRequestRepository;
        private readonly IZnodeRepository<ZnodeRmaRequestItem> _rmaRequestItemRepository;
        private readonly IZnodeRepository<ZnodeGiftCard> _giftCardRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodeRmaConfiguration> _rmaConfigurationRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _omsOrderDetailsRepository;
        private readonly IZnodeRepository<View_GetRMASearchRequest> _viewGetRMASearchRequestRepository;
        private readonly IZnodeRepository<ZnodeRmaRequestStatu> _rmaRequestStatusRepository;
        private readonly IEmailTemplateSharedService _emailTemplateSharedService;

        public RMARequestService()
        {
            _rmaRequestRepository = new ZnodeRepository<ZnodeRmaRequest>();
            _rmaRequestItemRepository = new ZnodeRepository<ZnodeRmaRequestItem>();
            _giftCardRepository = new ZnodeRepository<ZnodeGiftCard>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _rmaConfigurationRepository = new ZnodeRepository<ZnodeRmaConfiguration>();
            _omsOrderDetailsRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
            _viewGetRMASearchRequestRepository = new ZnodeRepository<View_GetRMASearchRequest>();
            _rmaRequestStatusRepository = new ZnodeRepository<ZnodeRmaRequestStatu>();
            _emailTemplateSharedService = GetService<IEmailTemplateSharedService>();
        }

        //Get RMA Request.
        public virtual RMARequestModel GetRMARequest(int rmaRequestId)
        {
            ZnodeLogging.LogMessage("GetRMARequest method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (rmaRequestId < 1)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorRMARequestIdLessThanOne);

            ZnodeRmaRequest rmaRequest = _rmaRequestRepository.Table.Where(w => w.RmaRequestId == rmaRequestId)?.FirstOrDefault();

            RMARequestModel requestModel = IsNotNull(rmaRequest) ? rmaRequest.ToModel<RMARequestModel>() : new RMARequestModel();

            if(IsNotNull(rmaRequest))
                requestModel.RequestCode = GetRMARequestStatusCodeById(rmaRequest.RmaRequestStatusId.GetValueOrDefault());
            ZnodeLogging.LogMessage("GetRMARequest method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return requestModel;
        }

        //Get Order RMA Flag to check order has already been applied for RMA or not.
        public virtual bool GetOrderRMAFlag(int omsOrderDetailsId)
        {
            ZnodeLogging.LogMessage("GetOrderRMAFlag method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (omsOrderDetailsId < 1)
                return false;

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@OmsOrderDetailsId", omsOrderDetailsId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("SP parameter values:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { omsOrderDetailsId });
            int status = 0;

            IList<View_ReturnBoolean> result = objStoredProc.ExecuteStoredProcedureList("Znode_GetOrderRMAFlag @OmsOrderDetailsId,  @Status OUT", 1, out status);
            ZnodeLogging.LogMessage("Result list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, result?.Count());
            ZnodeLogging.LogMessage("GetOrderRMAFlag method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return result.FirstOrDefault().Status.Value;
        }

        //Update RMA Request.
        public virtual RMARequestModel UpdateRMARequest(int rmaRequestId, RMARequestModel rmaRequestModel)
        {
            ZnodeLogging.LogMessage("UpdateRMARequest method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (rmaRequestId < 1)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorRMARequestIdLessThanOne);

            if (IsNull(rmaRequestModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorRMARequestModelNull);

            ZnodeRmaRequest rmaRequest = _rmaRequestRepository.Table.FirstOrDefault(w => w.RmaRequestId == rmaRequestId);

            if (IsNotNull(rmaRequest))
            {
                rmaRequestModel.RmaRequestId = rmaRequest.RmaRequestId;
                rmaRequestModel.RequestDate = rmaRequest.RequestDate;
                //Get RMA Request Status Code.
                ZnodeLogging.LogMessage("Parameter for getting RMARequestStatusIdByCode", ZnodeLogging.Components.OMS.ToString(),TraceLevel.Verbose, rmaRequestModel.RequestCode);
                rmaRequestModel.RmaRequestStatusId = GetRMARequestStatusIdByCode(rmaRequestModel.RequestCode);

                ZnodeRmaRequest rmaUpdateRequest = rmaRequestModel.ToEntity<ZnodeRmaRequest>();

                bool status = _rmaRequestRepository.Update(rmaUpdateRequest);

                ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessRMARequestUpdate : Admin_Resources.ErrorRMARequestUpdate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                return status ? rmaUpdateRequest.ToModel<RMARequestModel>() : new RMARequestModel();

            }
            ZnodeLogging.LogMessage("UpdateRMARequest method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return new RMARequestModel();
        }

        //Get RMA Request List.
        public virtual RMARequestListModel GetRMARequestList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("GetRMARequestList method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Bind the Filter conditions for the authorized portal access.
            BindUserPortalFilter(ref filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            RMARequestListModel listModel;
            List<RMARequestModel> rmaRequestList = _viewGetRMASearchRequestRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToModel<RMARequestModel>()?.ToList();
            ZnodeLogging.LogMessage("RmaRequest list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, rmaRequestList?.Count());
            listModel = new RMARequestListModel { RMARequests = rmaRequestList.ToList() };
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("GetRMARequestList method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get RMA Gift Card Details
        public virtual IssuedGiftCardListModel GetRMAGiftCardDetails(int rmaRequestId)
        {
            ZnodeLogging.LogMessage("GetRMAGiftCardDetails method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            IssuedGiftCardListModel giftCardListModel = new IssuedGiftCardListModel();

            List<Nullable<int>> giftCardIds = _rmaRequestItemRepository.Table.Where(w => w.RmaRequestId == rmaRequestId).Select(s => s.GiftCardId).ToList();
            ZnodeLogging.LogMessage("Gift Card Ids list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, giftCardIds?.Count());
            if (giftCardIds?.Count > 0)
                giftCardListModel.IssuedGiftCardModels = _giftCardRepository.Table.Where(w => giftCardIds.Contains(w.GiftCardId))?.Distinct()?.ToModel<IssuedGiftCardModel>()?.ToList();
            ZnodeLogging.LogMessage("GetRMAGiftCardDetails method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return giftCardListModel ?? new IssuedGiftCardListModel();
        }

        //Create RMA Request.
        public virtual RMARequestModel CreateRMARequest(RMARequestModel rmaRequestModel)
        {
            ZnodeLogging.LogMessage("CreateRMARequest method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(rmaRequestModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorRMARequestModelNull);

            if (IsNull(rmaRequestModel.RMARequestItems) || rmaRequestModel.RMARequestItems.Count < 1)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorRMARequestItemsModelNull);

            rmaRequestModel.RequestDate = GetDateTime();

            //Get RMA Request Status Code.
            rmaRequestModel.RmaRequestStatusId = (_rmaRequestStatusRepository.Table.Where(w => w.RequestCode == rmaRequestModel.RequestCode).Select(y => y.RmaRequestStatusId)?.FirstOrDefault()).GetValueOrDefault();
            
            ZnodeRmaRequest znodeRmaRequest = _rmaRequestRepository.Insert(rmaRequestModel.ToEntity<ZnodeRmaRequest>());
            ZnodeLogging.LogMessage("Inserted RmaRequestStatus with id ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, rmaRequestModel?.RmaRequestStatusId);
            if (znodeRmaRequest.RmaRequestId > 0 && HelperUtility.IsNotNull(rmaRequestModel.RMARequestItems) && rmaRequestModel.RMARequestItems.Count > 0)
            {
                rmaRequestModel.RmaRequestId = znodeRmaRequest.RmaRequestId;

                foreach (RMARequestItemModel items in rmaRequestModel.RMARequestItems)
                {
                    items.RMARequestId = znodeRmaRequest.RmaRequestId;

                    _rmaRequestItemRepository.Insert(items.ToEntity<ZnodeRmaRequestItem>());
                }
                ZnodeLogging.LogMessage("Inserted RmaRequestStatus with id ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, rmaRequestModel?.RmaRequestStatusId);
            }
            else
                rmaRequestModel.RmaRequestId = 0;

            ZnodeLogging.LogMessage(rmaRequestModel.RmaRequestId > 0 ? Admin_Resources.SuccessRMARequestCreate : Admin_Resources.ErrorRMARequestCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            //Send RMA Status Mail to customer if EmailRMAReport is true.
            if (rmaRequestModel.RmaRequestId > 0 && rmaRequestModel.EmailRMAReport)
                ZnodeLogging.LogMessage("Parameter for SendRMAStatusMail", ZnodeLogging.Components.OMS.ToString(),TraceLevel.Verbose,rmaRequestModel.RmaRequestId);
            SendRMAStatusMail(rmaRequestModel.RmaRequestId);
            ZnodeLogging.LogMessage("CreateRMARequest method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return rmaRequestModel;
        }

        // Send RMA status request mail.
        public virtual bool SendRMAStatusMail(int rmaRequestId)
        {
            ZnodeLogging.LogMessage("SendRMAStatusMail method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (rmaRequestId < 1)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorRMARequestIdLessThanOne);

            RMARequestReportModel rmaRequestReportModel;

            IZnodeViewRepository<RMARequestReportModel> objStoredProc = new ZnodeViewRepository<RMARequestReportModel>();
            ZnodeLogging.LogMessage("SP parameter values: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, rmaRequestId);
            //SP parameters
            objStoredProc.SetParameter("@RMARequestId", rmaRequestId, ParameterDirection.Input, DbType.Int32);

            List<RMARequestReportModel> rmaRequestReport = objStoredProc.ExecuteStoredProcedureList("Znode_GetOmsOrderLineItemRMARequestReport @RMARequestId").ToList();
            ZnodeLogging.LogMessage("rmaRequestReport list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, rmaRequestReport?.Count());
            rmaRequestReportModel = rmaRequestReport?.FirstOrDefault() ?? new RMARequestReportModel();

            if (IsNotNull(rmaRequestReportModel))
                return SendRMARequestStatusMail(rmaRequestReportModel);
            ZnodeLogging.LogMessage("SendRMAStatusMail method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return false;
        }

        // Send RMA request status mail to user.
        private bool SendRMARequestStatusMail(RMARequestReportModel rmaRequestReportModel)
        {
            if (rmaRequestReportModel.IsEmailNotification && !string.IsNullOrEmpty(rmaRequestReportModel.CustomerNotification))
            {
                //Get Email template by passing code as 'RMARequestStatus'
                ZnodeLogging.LogMessage("Parameter for getting emailTemplateMapperModel", ZnodeLogging.Components.OMS.ToString(),TraceLevel.Verbose, new object[] { "RMARequestStatus", PortalId });
                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.RMARequestStatus, PortalId);

                if (IsNull(emailTemplateMapperModel))
                    return false;

                string storeName = ZnodeConfigManager.SiteConfig.StoreName;
                string senderEmail = ZnodeConfigManager.SiteConfig.AdminEmail;
                string subject = $"{storeName} - {emailTemplateMapperModel.Subject}";
                string messageText = emailTemplateMapperModel.Descriptions;
                messageText = ReplaceTokenWithMessageText("#CustomerName#", rmaRequestReportModel.CustomerName, messageText);
                messageText = ReplaceTokenWithMessageText("#CustomerNotification#", rmaRequestReportModel.CustomerNotification, messageText);
                messageText = ReplaceTokenWithMessageText("#StoreName#", storeName, messageText);
                messageText = ReplaceTokenWithMessageText("#DepartmentName#", rmaRequestReportModel.DepartmentName, messageText);
                messageText = ReplaceTokenWithMessageText("#DepartmentAddress#", rmaRequestReportModel.DepartmentAddress, messageText);
                messageText = ReplaceTokenWithMessageText("#DepartmentEmail#", rmaRequestReportModel.DepartmentEmail, messageText);
                messageText = ReplaceTokenWithMessageText("#BillingEmailId#", rmaRequestReportModel.BillingEmailId, messageText);

                // These method is used to set null to unwanted keys.
                messageText = _emailTemplateSharedService.ReplaceTemplateTokens(messageText, new Dictionary<string, string>());
                ZnodeLogging.LogMessage("Parameter for sending mail", ZnodeLogging.Components.OMS.ToString(),TraceLevel.Verbose, new object[] { rmaRequestReportModel.BillingEmailId, senderEmail, "Method:ZnodeEmail.GetBccEmail", subject, messageText });
                return SendMail(rmaRequestReportModel.BillingEmailId, senderEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, PortalId, string.Empty), subject, messageText, true);
            }
            return false;
        }

        // Send gift card mail.
        public virtual bool SendGiftCardMail(GiftCardModel giftCardModel)
        {
            if (IsNull(giftCardModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.GiftCardModelNotNull);

            RMAConfigurationModel rmaConfiguration = _rmaConfigurationRepository.Table?.FirstOrDefault()?.ToModel<RMAConfigurationModel>();
            ZnodeOmsOrderDetail omsOrderDetails = _omsOrderDetailsRepository.Table.FirstOrDefault(w => w.OmsOrderDetailsId == giftCardModel.OMSOrderDetailsId);
            return SendGiftCardMailToUser(giftCardModel, omsOrderDetails ?? new ZnodeOmsOrderDetail(), rmaConfiguration);
        }

        // Get list of service requests according to parameters
        public virtual List<ReportServiceRequestModel> GetServiceRequestReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            DateTime beginDate;
            RemoveFilters(filters, out beginDate);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string cmdString = "ZnodeDevExpressReport_GetCaseRequest @BeginDate, @WhereClause";

            IZnodeViewRepository<ReportServiceRequestModel> objStoredProc = new ZnodeViewRepository<ReportServiceRequestModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            ZnodeLogging.LogMessage("SP parameter values: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }

        // Send gift card mail to customer.
        private bool SendGiftCardMailToUser(GiftCardModel giftCardModel, ZnodeOmsOrderDetail omsOrderDetails, RMAConfigurationModel rmaConfiguration)
        {
            //Get Email template by passing code as 'IssueGiftCard'
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.IssueGiftCard, (giftCardModel.PortalId > 0) ? giftCardModel.PortalId : PortalId);

            if (IsNull(emailTemplateMapperModel))
                return false;

            string storeName = GetStoreName(giftCardModel);
            ZnodeLogging.LogMessage("Store Name:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, storeName);

            if (IsNull(storeName))
                storeName = ZnodeConfigManager.SiteConfig.StoreName;
            string senderEmail = ZnodeConfigManager.SiteConfig.AdminEmail;

            string subject = $"{storeName} - {emailTemplateMapperModel.Subject}";
            string messageText = emailTemplateMapperModel.Descriptions;
            messageText = ReplaceTokenWithMessageText("#FirstName#", omsOrderDetails.BillingFirstName + omsOrderDetails.BillingLastName, messageText);
            messageText = ReplaceTokenWithMessageText("#GCNotification#", rmaConfiguration.GcNotification, messageText);
            messageText = ReplaceTokenWithMessageText("#CardNumber#", giftCardModel.CardNumber, messageText);
            messageText = ReplaceTokenWithMessageText("#Amount#", Convert.ToString(giftCardModel.Amount), messageText);
            messageText = ReplaceTokenWithMessageText("#ExpirationDate#", Convert.ToString(giftCardModel.ExpirationDate), messageText);
            messageText = ReplaceTokenWithMessageText("#StoreName#", storeName, messageText);
            messageText = ReplaceTokenWithMessageText("#DisplayName#", rmaConfiguration.DisplayName, messageText);
            messageText = ReplaceTokenWithMessageText("#Address#", rmaConfiguration.Address, messageText);
            messageText = ReplaceTokenWithMessageText("#Email#", rmaConfiguration.Email, messageText);

            // These method is used to set null to unwanted keys.
            messageText = EmailTemplateHelper.ReplaceTemplateTokens(messageText);
            ZnodeLogging.LogMessage("Parameter for sending mail", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { omsOrderDetails.BillingEmailId, senderEmail, "Method:ZnodeEmail.GetBccEmail", subject, messageText });
            return SendMail(omsOrderDetails.BillingEmailId, senderEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, giftCardModel.PortalId, string.Empty), subject, messageText, true);

        }

        // Get store name on the basis of portalId.
        private string GetStoreName(GiftCardModel model)
        => _portalRepository.GetById(model.PortalId)?.StoreName;

        // Send mail.
        private bool SendMail(string billingEmailId, string senderEmail, string bcc, string subject, string messageText, bool isBodyHtml = false)
        {
            try
            {
                ZnodeEmail.SendEmail(billingEmailId, senderEmail, bcc, subject, messageText, isBodyHtml);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginCreateFailed, string.Empty, string.Empty, null, ex.Message, null);
                return false;
            }
        }

        //Get RMA Request Status Id By Code.
        private int GetRMARequestStatusIdByCode(string requestStatusCode)
            => (_rmaRequestStatusRepository.Table.Where(w => w.RequestCode == requestStatusCode).Select(y => y.RmaRequestStatusId)?.FirstOrDefault()).GetValueOrDefault();

        //Get RMA Request Status Code By Id.
        private string GetRMARequestStatusCodeById(int requestStatusId)
            => _rmaRequestStatusRepository.Table.Where(w => w.RmaRequestStatusId == requestStatusId).Select(y => y.RequestCode)?.FirstOrDefault();

        //This method is used to remove the parameters from the filters and assign its values to variables.
        private void RemoveFilters(FilterCollection filters, out DateTime beginDate)
        {
            //Remove begin date from filters collection
            beginDate = Convert.ToDateTime(filters?.Find(x => string.Equals(x.FilterName, FilterKeys.BeginDate, StringComparison.CurrentCultureIgnoreCase))?.Item3);
            filters?.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.BeginDate, StringComparison.CurrentCultureIgnoreCase));

            //Remove VisibleColumns from filters collection               
            filters?.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.VisibleColumns, StringComparison.CurrentCultureIgnoreCase));

            //Update the value of store name filter
            var storeFilter = filters?.Find(x => string.Equals(x.FilterName, FilterKeys.StoresName, StringComparison.CurrentCultureIgnoreCase));
            if (IsNotNull(storeFilter))
            {
                var value = storeFilter.Item3.Replace("|", ",");
                filters?.Remove(storeFilter);
                filters.Add(new FilterTuple(FilterKeys.StoreName, storeFilter.FilterOperator, value));
            }
        }

    }
}
