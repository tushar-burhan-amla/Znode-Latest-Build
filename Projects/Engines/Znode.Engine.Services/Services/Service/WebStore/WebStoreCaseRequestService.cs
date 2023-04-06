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
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class WebStoreCaseRequestService : BaseService, IWebStoreCaseRequestService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeCaseRequest> _caseRequestRepository;
        private readonly IZnodeRepository<ZnodeCaseType> _caseTypeRepository;
        private readonly IZnodeRepository<ZnodeCasePriority> _casePriorityRepository;
        private readonly IZnodeRepository<ZnodeCaseStatu> _caseStatusRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodeCaseRequestHistory> _caseRequestHistoryRepository;
        private readonly IEmailTemplateSharedService _emailTemplateSharedService;
        #endregion

        #region Constructor
        public WebStoreCaseRequestService()
        {
            _caseRequestRepository = new ZnodeRepository<ZnodeCaseRequest>();
            _caseTypeRepository = new ZnodeRepository<ZnodeCaseType>();
            _casePriorityRepository = new ZnodeRepository<ZnodeCasePriority>();
            _caseStatusRepository = new ZnodeRepository<ZnodeCaseStatu>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _caseRequestHistoryRepository = new ZnodeRepository<ZnodeCaseRequestHistory>();
            _emailTemplateSharedService = GetService<IEmailTemplateSharedService>();
        }
        #endregion

        #region Public Methods

        //Create contact us.
        public virtual WebStoreCaseRequestModel CreateContactUs(WebStoreCaseRequestModel caseRequestModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            if (IsNull(caseRequestModel))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ErrorCaseRequestModelNull);

            //Create new CaseRequest and return it.
            ZnodeCaseRequest caseRequest = _caseRequestRepository.Insert(caseRequestModel.ToEntity<ZnodeCaseRequest>());//insert log
            if (Equals(caseRequestModel.CaseOrigin, ZnodeConstant.ContactUsForm))
            {
                SendContactUsEmail(GetStoreName(caseRequestModel), caseRequestModel.FirstName, caseRequestModel.LastName, caseRequestModel.CompanyName, caseRequestModel.PhoneNumber, caseRequestModel.Description, caseRequestModel.EmailId, caseRequestModel.PortalId, caseRequestModel.LocaleId);
                var onContactUsInit = new ZnodeEventNotifier<WebStoreCaseRequestModel>(caseRequestModel, EventConstant.OnContactUs);
            }
            else if (Equals(caseRequestModel.CaseOrigin, ZnodeConstant.CustomerFeedbackForm))
            {
                //Split message data.
                string[] message = caseRequestModel.Description.Split(new String[] { ":", "<br/>" }, StringSplitOptions.None);

                SendCustomerFeedbackEmail(GetStoreName(caseRequestModel), message, caseRequestModel.FirstName, caseRequestModel.LastName, caseRequestModel.EmailId, caseRequestModel.PortalId, caseRequestModel.LocaleId, caseRequestModel.Description);
                var onCustomerFeedbackNotificationlInit = new ZnodeEventNotifier<WebStoreCaseRequestModel>(caseRequestModel, EventConstant.OnCustomerFeedbackNotification);
            }

            if (caseRequest?.CaseRequestId > 0)
                return caseRequest.ToModel<WebStoreCaseRequestModel>();

            ZnodeLogging.LogMessage("CaseRequestId, PortalId and LocaleId properties of caseRequestModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { CaseRequestId = caseRequestModel?.CaseRequestId, PortalId = caseRequestModel?.PortalId, LocaleId = caseRequestModel?.LocaleId });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return caseRequestModel;
        }

        // Get the list of case request.
        public virtual WebStoreCaseRequestListModel GetCaseRequests(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            int IsCaseHistory = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == FilterKeys.IsCaseHistory.ToString().ToLower())?.FirstOrDefault()?.Item3);
            filters.RemoveAll(x => x.Item1.Equals(FilterKeys.IsCaseHistory.ToLower(), StringComparison.InvariantCultureIgnoreCase));

            //Bind the Filter conditions for the authorized portal access.
            BindUserPortalFilter(ref filters);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to set SP parameters to get caseRequestList: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<WebStoreCaseRequestModel> objStoredProc = new ZnodeViewRepository<WebStoreCaseRequestModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@IsCaseHistory", IsCaseHistory, ParameterDirection.Input, DbType.Int32);

            IList<WebStoreCaseRequestModel> caseRequestList = objStoredProc.ExecuteStoredProcedureList("Znode_GetCaseRequest @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT,@IsCaseHistory", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("caseRequestList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, caseRequestList?.Count);

            WebStoreCaseRequestListModel listModel = new WebStoreCaseRequestListModel { CaseRequestList = caseRequestList?.ToList() };
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Create Case Request.
        public virtual WebStoreCaseRequestModel CreateCaseRequest(WebStoreCaseRequestModel webStoreCaseRequestModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            if (IsNull(webStoreCaseRequestModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorCaseRequestModelNull);
            ZnodeLogging.LogMessage("Input parameters CaseRequestId, PortalId and LocaleId properties of caseRequestModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { CaseRequestId = webStoreCaseRequestModel?.CaseRequestId, PortalId = webStoreCaseRequestModel?.PortalId, LocaleId = webStoreCaseRequestModel?.LocaleId });
            //Create new case request and return it.
            ZnodeCaseRequest caseRequest = _caseRequestRepository.Insert(webStoreCaseRequestModel.ToEntity<ZnodeCaseRequest>());

            ZnodeLogging.LogMessage((caseRequest?.CaseRequestId < 0) ? Admin_Resources.ErrorInsertCaseRequest : Admin_Resources.SuccessInsertCaseRequest, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            if (IsNotNull(caseRequest))
                return caseRequest.ToModel<WebStoreCaseRequestModel>();
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return webStoreCaseRequestModel;
        }

        //Get case request by GetCaseRequest.
        public virtual WebStoreCaseRequestModel GetCaseRequest(int caseRequestId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter caseRequestId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, caseRequestId);

            if (caseRequestId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeCaseRequestEnum.CaseRequestId.ToString(), FilterOperators.Equals, caseRequestId.ToString()));

                //Bind the Filter, sorts & Paging details.
                PageListModel pageListModel = new PageListModel(filters, null, null);
                ZnodeLogging.LogMessage("pageListModel generated to set SP parameters to get caseRequestDetails: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

                IZnodeViewRepository<WebStoreCaseRequestModel> objStoredProc = new ZnodeViewRepository<WebStoreCaseRequestModel>();
                //SP parameters
                objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
                WebStoreCaseRequestModel caseRequestDetails = objStoredProc.ExecuteStoredProcedureList("Znode_GetCaseRequest @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT", 4, out pageListModel.TotalRowCount)?.FirstOrDefault();
                ZnodeLogging.LogMessage("CaseRequestId, PortalId and LocaleId properties of caseRequestModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { CaseRequestId = caseRequestDetails?.CaseRequestId, PortalId = caseRequestDetails?.PortalId, LocaleId = caseRequestDetails?.LocaleId });
                return caseRequestDetails?.CaseRequestId > 0 ? caseRequestDetails : new WebStoreCaseRequestModel();
            }
            return new WebStoreCaseRequestModel();
        }

        //Update Case Request.
        public virtual bool UpdateCaseRequest(WebStoreCaseRequestModel webStoreCaseRequestModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            if (IsNull(webStoreCaseRequestModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorCaseRequestModelNull);
            if (webStoreCaseRequestModel.CaseRequestId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorCaseRequestIdLessThanOne);
            ZnodeLogging.LogMessage("Input parameters CaseRequestId, PortalId and LocaleId properties of caseRequestModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { CaseRequestId = webStoreCaseRequestModel?.CaseRequestId, PortalId = webStoreCaseRequestModel?.PortalId, LocaleId = webStoreCaseRequestModel?.LocaleId });

            //Update Case Request.
            bool isCaseRequestUpdated = _caseRequestRepository.Update(webStoreCaseRequestModel.ToEntity<ZnodeCaseRequest>());
            ZnodeLogging.LogMessage(isCaseRequestUpdated ? Admin_Resources.SuccessCaseRequestUpdate : Admin_Resources.ErrorCaseRequestUpdate, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return isCaseRequestUpdated;
        }

        //Create Case Request.
        public virtual bool ReplyCustomer(WebStoreCaseRequestModel webStoreCaseRequestModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            if (IsNull(webStoreCaseRequestModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorCaseRequestModelNull);
            ZnodeLogging.LogMessage("Input parameters CaseRequestId, PortalId and LocaleId properties of caseRequestModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { CaseRequestId = webStoreCaseRequestModel?.CaseRequestId, PortalId = webStoreCaseRequestModel?.PortalId, LocaleId = webStoreCaseRequestModel?.LocaleId });

            //Send email to customer.
            bool isMailSend = SendEmailToCustomer(GetStoreName(webStoreCaseRequestModel), webStoreCaseRequestModel.FirstName, webStoreCaseRequestModel.EmailId, webStoreCaseRequestModel.EmailMessage, webStoreCaseRequestModel.EmailSubject, webStoreCaseRequestModel.AttachedPath, webStoreCaseRequestModel.PortalId);
            if (isMailSend)
                _caseRequestHistoryRepository.Insert(webStoreCaseRequestModel.ToEntity<ZnodeCaseRequestHistory>());
            var onServiceRequestMessageInit = new ZnodeEventNotifier<WebStoreCaseRequestModel>(webStoreCaseRequestModel, EventConstant.OnServiceRequestMessage);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return isMailSend;
        }

        // Get the list of case type.
        public virtual CaseTypeListModel GetCaseTypeList()
        {
            CaseTypeListModel listModel = new CaseTypeListModel();
            IList<ZnodeCaseType> caseTypes = _caseTypeRepository.GetEntityList(string.Empty);
            listModel.CaseTypes = caseTypes?.Count > 0 ? caseTypes.ToModel<CaseTypeModel>().ToList() : new List<CaseTypeModel>();
            ZnodeLogging.LogMessage("Case type list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, listModel?.CaseTypes?.Count);
            return listModel;
        }

        // Get the list of case status.
        public virtual CaseStatusListModel GetCaseStatusList()
        {
            CaseStatusListModel listModel = new CaseStatusListModel();
            IList<ZnodeCaseStatu> statuses = _caseStatusRepository.GetEntityList(string.Empty);
            listModel.CaseStatuses = statuses?.Count > 0 ? statuses.ToModel<CaseStatusModel>().ToList() : new List<CaseStatusModel>();
            ZnodeLogging.LogMessage("Case status list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, listModel?.CaseStatuses?.Count);
            return listModel;
        }

        // Get the list of case priority.
        public virtual CasePriorityListModel GetCasePriorityList()
        {
            CasePriorityListModel listModel = new CasePriorityListModel();
            IList<ZnodeCasePriority> priorities = _casePriorityRepository.GetEntityList(string.Empty);
            listModel.CasePriorities = priorities?.Count > 0 ? priorities.ToModel<CasePriorityModel>().ToList() : new List<CasePriorityModel>();
            ZnodeLogging.LogMessage("Case priority list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, listModel?.CasePriorities?.Count);
            return listModel;
        }
        #endregion

        #region Private Methods
        //This method will send email to customer.
        private bool SendEmailToCustomer(string storeName, string userName, string email, string emailMessage, string emailSubject, string attachedPath, int portalId)
        {
            ZnodeLogging.LogMessage("Input parameters storeName, userName, email,emailMessage, emailSubject, attachedPath, portalId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { storeName, userName, email, emailMessage, emailSubject, attachedPath, portalId });

            //Get Email template by passing code as 'ServiceRequestMessage'
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.ServiceRequestMessage, (portalId > 0) ? portalId : PortalId);
            ZnodeLogging.LogMessage("EmailTemplateMapperId and EmailTemplateId of emailTemplateMapperModel returned from method GetEmailTemplateByCode", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { EmailTemplateMapperId = emailTemplateMapperModel?.EmailTemplateMapperId, EmailTemplateId = emailTemplateMapperModel?.EmailTemplateId });

            if (IsNull(emailTemplateMapperModel))
                return false;

            if (IsNull(storeName))
                storeName = ZnodeConfigManager.SiteConfig.StoreName;
            string senderEmail = ZnodeConfigManager.SiteConfig.AdminEmail;

            string subject = $"{storeName} - {emailSubject}";

            string messageText = emailTemplateMapperModel.Descriptions;
            messageText = ReplaceTokenWithMessageText("#FirstName#", userName, messageText);
            messageText = ReplaceTokenWithMessageText("#EmailMessage#", emailMessage, messageText);
            messageText = HelperUtility.ReplaceTokenWithMessageText("#StoreLogo#", GetCustomPortalDetails((portalId > 0) ? portalId : PortalId)?.StoreLogo, messageText);

            //These method is used to set null to unwanted keys.
            messageText = _emailTemplateSharedService.ReplaceTemplateTokens(messageText, new Dictionary<string, string>());
            ZnodeLogging.LogMessage("messageText generated by _emailTemplateSharedService.ReplaceTemplateTokens", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, messageText);

            try
            {           
                ZnodeEmail.SendEmail(email, senderEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, portalId, string.Empty), subject, messageText, true, attachedPath);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginCreateSuccess, userName, string.Empty, null, ex.Message, null);
                return false;
            }
        }

        //This method will send email to site admin for contact us form.
        private bool SendContactUsEmail(string storeName, string firstName, string lastName, string companyName, string phoneNumber, string comments, string email, int portalId, int localeId)
        {
            ZnodeLogging.LogMessage("Input parameters storeName, firstName, lastName, companyName, phoneNumber, comments, email, portalId, localeId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { storeName, firstName, lastName, companyName, phoneNumber, comments, email, portalId, localeId});

            //Get Email template by passing code as 'ContactUs'
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.ContactUs, (portalId > 0) ? portalId : PortalId, localeId);
            ZnodeLogging.LogMessage("EmailTemplateMapperId and EmailTemplateId of emailTemplateMapperModel returned from method GetEmailTemplateByCode", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { EmailTemplateMapperId = emailTemplateMapperModel?.EmailTemplateMapperId, EmailTemplateId = emailTemplateMapperModel?.EmailTemplateId });

            if (IsNull(emailTemplateMapperModel))
                return false;

            if (IsNull(storeName))
                storeName = ZnodeConfigManager.SiteConfig.StoreName;

            string messageText = emailTemplateMapperModel.Descriptions;
            messageText = ReplaceTokenWithMessageText("#FirstName#", firstName, messageText);
            messageText = ReplaceTokenWithMessageText("#LastName#", lastName, messageText);
            messageText = ReplaceTokenWithMessageText("#CompanyName#", IsNull(companyName) ? string.Empty : companyName, messageText);
            messageText = ReplaceTokenWithMessageText("#PhoneNumber#", IsNull(phoneNumber) ? string.Empty : phoneNumber, messageText);
            messageText = ReplaceTokenWithMessageText("#Comments#", IsNull(comments) ? string.Empty : comments, messageText);
            messageText = HelperUtility.ReplaceTokenWithMessageText("#StoreLogo#", GetCustomPortalDetails((portalId > 0) ? portalId : PortalId)?.StoreLogo, messageText);

            //These method is used to set null to unwanted keys.
            messageText = _emailTemplateSharedService.ReplaceTemplateTokens(messageText, new Dictionary<string, string>());
            ZnodeLogging.LogMessage("messageText generated by _emailTemplateSharedService.ReplaceTemplateTokens", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, messageText);
            try
            {              
                ZnodeEmail.SendEmail(ZnodeConfigManager.SiteConfig.AdminEmail, email, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, portalId, string.Empty), emailTemplateMapperModel.Subject, messageText, true);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginCreateSuccess, firstName, string.Empty, null, ex.Message, null);
                return false;
            }
        }

        //This method will send email to site admin for customer feedback.
        private bool SendCustomerFeedbackEmail(string storeName, string[] message, string firstName, string lastName, string email, int portalId, int localeId, string comments)
        {
            ZnodeLogging.LogMessage("Input parameters storeName, message, firstName, lastName, email, portalId, localeId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { storeName, message, firstName, lastName, email, portalId, localeId });

            //Get Email template by passing code as 'ContactUs'
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.CustomerFeedbackNotification, (portalId > 0) ? portalId : PortalId, localeId);
            ZnodeLogging.LogMessage("EmailTemplateMapperId and EmailTemplateId of emailTemplateMapperModel returned from method GetEmailTemplateByCode", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { EmailTemplateMapperId = emailTemplateMapperModel?.EmailTemplateMapperId, EmailTemplateId = emailTemplateMapperModel?.EmailTemplateId });

            if (IsNull(emailTemplateMapperModel))
                return false;

            if (IsNull(storeName))
                storeName = ZnodeConfigManager.SiteConfig.StoreName;

            string messageText = emailTemplateMapperModel.Descriptions;
            messageText = ReplaceTokenWithMessageText("#Message#", message[1], messageText);
            messageText = ReplaceTokenWithMessageText("#FirstName#", IsNull(firstName) ? string.Empty : firstName, messageText);
            messageText = ReplaceTokenWithMessageText("#LastName#", IsNull(lastName) ? string.Empty : lastName, messageText);
            messageText = ReplaceTokenWithMessageText("#City#", message[3], messageText);
            messageText = ReplaceTokenWithMessageText("#State#", message[5], messageText);
            messageText = ReplaceTokenWithMessageText("#EmailAddress#", email, messageText);
            messageText = ReplaceTokenWithMessageText("#ShareFeedback#", message[6], messageText);
            messageText = ReplaceTokenWithMessageText("#Comments#", IsNull(comments) ? string.Empty : comments, messageText);
            messageText = HelperUtility.ReplaceTokenWithMessageText("#StoreLogo#", GetCustomPortalDetails((portalId > 0) ? portalId : PortalId)?.StoreLogo, messageText);

            //These method is used to set null to unwanted keys.
            messageText = _emailTemplateSharedService.ReplaceTemplateTokens(messageText, new Dictionary<string, string>());
            ZnodeLogging.LogMessage("messageText generated by _emailTemplateSharedService.ReplaceTemplateTokens", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, messageText);

            try
            {
                ZnodeEmail.SendEmail(ZnodeConfigManager.SiteConfig.AdminEmail, email, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, portalId, string.Empty), emailTemplateMapperModel.Subject, messageText, true);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.LoginCreateSuccess, firstName, string.Empty, null, ex.Message, null);
                return false;
            }
        }

        //Get store name on the basis of portalId
        private string GetStoreName(WebStoreCaseRequestModel webStoreCaseRequestModel)
        => _portalRepository.GetById(webStoreCaseRequestModel.PortalId)?.StoreName;

        #endregion
    }
}
