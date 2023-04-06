using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Resources;
using System.Text;
using Znode.Libraries.Observer;

namespace Znode.Engine.Services
{
    public class AccountQuoteService : BaseService, IAccountQuoteService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeOmsQuote> _omsQuoteRepository;
        private readonly IZnodeRepository<ZnodeShipping> _shippingRepository;
        private readonly IZnodeRepository<ZnodeAccount> _accountRepository;
        private readonly IZnodeRepository<ZnodeUser> _userRepository;
        private readonly IZnodeRepository<ZnodeOmsQuoteLineItem> _omsQuoteLineItemRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IZnodeRepository<View_QuoteOrderTemplateDetail> _viewQuoteOrderTemplateDetail;
        private readonly IZnodeRepository<ZnodeOmsTemplate> _templateRepository;
        private readonly IZnodeRepository<ZnodeOmsTemplateLineItem> _templateLineItemRepository;
        private readonly IZnodeRepository<ZnodeOmsNote> _omsNotesRepository;
        private readonly IZnodeRepository<ZnodeUserApprover> _userApprover;
        private readonly IZnodeRepository<ZnodeOMSQuoteApproval> _omsQuoteApproval;
        private readonly IZnodeRepository<ZnodeOmsOrderState> _omsOrderState;
        private readonly IZnodeRepository<ZnodeApproverLevel> _approverLevel;
        private readonly IUserService _userService;
        private readonly IZnodeRepository<ZnodeDomain> _domainRepository;
        private readonly IZnodeRepository<ZnodeUserPortal> _userPortalRepository;
        private readonly IZnodeRepository<ZnodeOmsQuoteComment> _omsQuoteComment;
        private readonly IZnodeRepository<ZnodePortalPaymentSetting> _portalPaymentSettingRepository;
        private readonly IZnodeRepository<ZnodePaymentSetting> _paymentSettingRepository;
        private readonly IZnodeRepository<ZnodePaymentType> _paymentType;
        private readonly IShoppingCartMap _shoppingCartMap;

        private readonly IZnodeRepository<ZnodeOmsQuoteType> _quoteType;
        private readonly IZnodeRepository<ZnodeOmsQuotePersonalizeItem> _omsQuotePersonalizeItem;
        private readonly IPublishProductHelper _helper;
        private IZnodeRepository<ZnodePortalApproval> _portalApprovalRepository;
        private IZnodeRepository<ZnodePortalApprovalType> _portalApprovalTypeRepository;
        private IZnodeRepository<ZnodePortalApprovalLevel> _portalApprovalLevelRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IZnodeRepository<ZnodeOmsQuoteOrderDiscount> _omsQuoteOrderDiscountRepository;
        private readonly IZnodeRepository<ZnodeOmsQuoteTaxOrderDetail> _omsQuoteTaxDetailRepository;
        private readonly IZnodeRepository<ZnodeOmsTemplatePersonalizeCartItem> _omsTemplatePersonalizeCartItem;
        #endregion

        #region Constructor
        public AccountQuoteService()
        {
            _omsQuoteRepository = new ZnodeRepository<ZnodeOmsQuote>();
            _shippingRepository = new ZnodeRepository<ZnodeShipping>();
            _accountRepository = new ZnodeRepository<ZnodeAccount>();
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _omsQuoteLineItemRepository = new ZnodeRepository<ZnodeOmsQuoteLineItem>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _viewQuoteOrderTemplateDetail = new ZnodeRepository<View_QuoteOrderTemplateDetail>();
            _templateRepository = new ZnodeRepository<ZnodeOmsTemplate>();
            _templateLineItemRepository = new ZnodeRepository<ZnodeOmsTemplateLineItem>();
            _omsNotesRepository = new ZnodeRepository<ZnodeOmsNote>();
            _userApprover = new ZnodeRepository<ZnodeUserApprover>();
            _omsQuoteApproval = new ZnodeRepository<ZnodeOMSQuoteApproval>();
            _omsOrderState = new ZnodeRepository<ZnodeOmsOrderState>();
            _approverLevel = new ZnodeRepository<ZnodeApproverLevel>();
            _userService = GetService<IUserService>();
            _domainRepository = new ZnodeRepository<ZnodeDomain>();
            _userPortalRepository = new ZnodeRepository<ZnodeUserPortal>();
            _omsQuoteComment = new ZnodeRepository<ZnodeOmsQuoteComment>();
            _paymentSettingRepository = new ZnodeRepository<ZnodePaymentSetting>();
            _paymentType = new ZnodeRepository<ZnodePaymentType>();
            _shoppingCartMap = GetService<IShoppingCartMap>();
            _quoteType = new ZnodeRepository<ZnodeOmsQuoteType>();
            _omsQuotePersonalizeItem = new ZnodeRepository<ZnodeOmsQuotePersonalizeItem>();
            _helper = GetService<IPublishProductHelper>();
            _portalApprovalRepository = new ZnodeRepository<ZnodePortalApproval>();
            _portalApprovalTypeRepository = new ZnodeRepository<ZnodePortalApprovalType>();
            _portalApprovalLevelRepository = new ZnodeRepository<ZnodePortalApprovalLevel>();
            _portalPaymentSettingRepository = new ZnodeRepository<ZnodePortalPaymentSetting>();
            _shoppingCartService = GetService<IShoppingCartService>();
            _omsQuoteOrderDiscountRepository = new ZnodeRepository<ZnodeOmsQuoteOrderDiscount>();
            _omsQuoteTaxDetailRepository = new ZnodeRepository<ZnodeOmsQuoteTaxOrderDetail>();
            _omsTemplatePersonalizeCartItem = new ZnodeRepository<ZnodeOmsTemplatePersonalizeCartItem>();
        }
        #endregion

        #region Public Methods
        //Get Account Quote List.
        public virtual AccountQuoteListModel GetAccountQuoteList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            int accountId = 0;
            int loginUserId = 0;
            bool isPendingPayment = false;
            bool isParentPendingOrder = false;
            //Get accountId and login user id from filter to pass as a parameter in SP.
            GetAccountIdLoginUserId(filters, ref accountId, ref loginUserId, ref isPendingPayment, ref isParentPendingOrder);
            filters.RemoveAll(x => x.Item1 == ZnodeConstant.IsParentPendingOrder.ToLower().ToString());
            //Set Authorized Portal filter based on user portal access.
            BindUserPortalFilter(ref filters);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            if (IsNotNull(pageListModel))
            {
                if (pageListModel.PagingLength == 0)
                {
                    pageListModel.PagingLength = 10;
                }

                if (pageListModel.PagingStart == 0)
                {
                    pageListModel.PagingStart = 1;
                }
            }
            ZnodeLogging.LogMessage("pageListModel for executing store procedure Znode_GetOmsQuoteList :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<AccountQuoteModel> objStoredProc = new ZnodeViewRepository<AccountQuoteModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@AccountId", accountId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", loginUserId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsPendingPayment", isPendingPayment, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@IsParentPendingOrder", isParentPendingOrder, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@SalesRepUserId", HelperMethods.GetSalesRepUserId(), ParameterDirection.Input, DbType.Int32);

            IList<AccountQuoteModel> accountQuoteList = objStoredProc.ExecuteStoredProcedureList("Znode_GetOmsQuoteList @WhereClause,@Rows,@PageNo,@Order_By,@RowsCount OUT,@AccountId,@UserId,@IsPendingPayment,@IsParentPendingOrder,@SalesRepUserId", 4, out pageListModel.TotalRowCount);

            //Bind list of entity to list model.
            AccountQuoteListModel accountQuoteListModel = new AccountQuoteListModel { AccountQuotes = accountQuoteList?.ToList() };

            //Get userId from filter.
            int? userId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            ZnodeLogging.LogMessage("UserId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, userId);
            BindAccountCustomerName(userId, Convert.ToInt32(accountId), accountQuoteListModel);

            accountQuoteListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return accountQuoteListModel;
        }

        //Get Account Quote on the basis of omsQuoteId.
        public virtual AccountQuoteModel GetAccountQuote(FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //This method check the access of manage screen for sales rep user
            ValidateAccessForSalesRepUser(ZnodeConstant.PendingPaymentStateName, GetOmsQuoteId(filters));

            bool isQuoteLineItemUpdated = false;
            int localeId = GetLocaleId(filters);
            ZnodeLogging.LogMessage("localeId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, localeId);
            AccountQuoteModel accountQuote = new AccountQuoteModel();

            //Check if filter contains IsQuoteLineItemUpdated, assign its value to isQuoteLineItemUpdated variable and remove it from filter.
            CheckFilterHasIsQuoteLineItemUpdated(filters, ref isQuoteLineItemUpdated);
            if (filters?.Count > 0)
                accountQuote = _omsQuoteRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause, GetExpands(expands))?.ToModel<AccountQuoteModel>();

            //Check whether the current level of approver has already approved or rejected the quote.
            accountQuote.IsLevelApprovedOrRejected = CheckLevelApprovedOrRejected(filters);
            if (IsNotNull(accountQuote))
            {
                accountQuote.IsQuoteLineItemUpdated = isQuoteLineItemUpdated;
                accountQuote.LocaleId = localeId;
                //Get other quote details.
                accountQuote = GetOtherDetails(accountQuote);
                accountQuote.QuoteApproverComments = GetApproverComments(accountQuote.OmsQuoteId);
                accountQuote.PaymentDisplayName = GetPaymentNameByPaymentSettingId(accountQuote.PaymentSettingId, accountQuote.PortalId);
                accountQuote.BillingAddressHtml = IsNotNull(accountQuote.BillingAddressModel) ? GetQuoteBillingAddress(accountQuote.BillingAddressModel) : "";
                accountQuote.ShippingAddressHtml = IsNotNull(accountQuote.ShippingAddressModel) ? GetQuoteShipmentAddress(accountQuote.ShippingAddressModel) : "";
                accountQuote.CreatedByName = GetUserNameByUserId(accountQuote.CreatedBy)?.UserName ?? accountQuote.UserName;
            }
            ZnodeLogging.LogMessage("Properties of AccountQuoteModel to be returned:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = accountQuote?.OmsQuoteId, PortalId = accountQuote?.PortalId, UserId = accountQuote?.UserId, AccountId = accountQuote?.AccountId });

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return accountQuote;
        }

        //Create Account Quote.
        public virtual AccountQuoteModel Create(ShoppingCartModel shoppingCartModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(shoppingCartModel))
            {
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ErrorShoppingCartModelNull);
            }
            ZnodeLogging.LogMessage("Properties of input parameter shoppingCartModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = shoppingCartModel?.OmsQuoteId, PortalId = shoppingCartModel?.PortalId, UserId = shoppingCartModel?.UserId });
            //If quote id is 0 create new quote else update quote.
            if (shoppingCartModel?.OmsQuoteId <= 0)
            {
                string approverUserIds = string.Empty;
                //Get znode oms quote.
                ZnodeOmsQuote quote = GetQuoteCartId(shoppingCartModel, out approverUserIds);
                ZnodeLogging.LogMessage("OmsQuoteId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose,quote?.OmsQuoteId);
                if (quote?.OmsQuoteId > 0)
                {
                    //Save account quote line items.
                    return SaveQuoteLineItems(shoppingCartModel, quote, approverUserIds);
                }
            }
            else
            {
                //Set order status id.
                shoppingCartModel.OmsOrderStatusId = GetOmsOrderStateId(shoppingCartModel.OrderStatus);

                int isUpdated = 0;
                IList<AccountQuoteModel> accountQuoteList = UpdateQuoteStatus(shoppingCartModel.OmsOrderStatusId, Convert.ToString(shoppingCartModel.OmsQuoteId), "Ordered", out isUpdated);

                if (accountQuoteList?.Count() > 0)
                {
                    return UpdateQuoteDetails(accountQuoteList, shoppingCartModel, isUpdated);
                }
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return new AccountQuoteModel();
            }
            return null;
        }

        //Update Account Quote.
        public virtual bool UpdateQuoteStatus(QuoteStatusModel accountQuoteModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter AccountQuoteModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { accountQuoteModel = accountQuoteModel });

            if (IsNull(accountQuoteModel))
            {
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelCanNotBeNull);
            }

            if (IsNotNull(accountQuoteModel.OrderStatus))
            {
                accountQuoteModel.OmsOrderStateId = accountQuoteModel.IsPendingPaymentStatus ? GetOmsOrderStateId(ZnodeOrderStatusEnum.REJECTED.ToString()) : GetOmsOrderStateId(accountQuoteModel.OrderStatus);
            }

            if (accountQuoteModel.IsPendingPaymentStatus)
            {
                UpdateQuoteStatusAndSendMail(accountQuoteModel);
            }
            else
            {
                UpdateQuoteStatusAndNotifyNextLevel(accountQuoteModel);
            }
            ZnodeLogging.LogMessage("AccountQuoteModel to be returned:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { accountQuoteModel = accountQuoteModel });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return accountQuoteModel.IsUpdated;
        }

        //Method to check if the current user is an approver to any other user and has approvers itself.
        public virtual ApproverDetailsModel UserApproverDetails(int userId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Input parameter UserId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId });
            ApproverDetailsModel model = new ApproverDetailsModel
            {
                IsApprover = _userApprover.Table.Any(x => x.ApproverUserId == userId),
                HasApprovers = _userApprover.Table.Any(x => x.UserId == userId)
            };
            ZnodeLogging.LogMessage("ApproverDetailsModel details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, model);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Method to check if the current user is tha final approver for the quote.
        public virtual bool IsLastApprover(int quoteId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter quoteId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteId = quoteId });
            bool isLastApprover = false;
            int userId = GetLoginUserId();
            string approverLevel = string.Empty;
            ZnodeOmsQuote znodeOmsQuoteDetails = _omsQuoteRepository.Table.FirstOrDefault(x => x.OmsQuoteId == quoteId);
            decimal quoteTotal = znodeOmsQuoteDetails.QuoteOrderTotal.Value;
            int quoteUserId = _omsQuoteApproval.Table.FirstOrDefault(x => x.OmsQuoteId == quoteId)?.UserId ?? 0;
            ZnodePortalApproval znodePortalApproval = _portalApprovalRepository.Table.FirstOrDefault(x => x.PortalId == znodeOmsQuoteDetails.PortalId);

            if (IsNotNull(znodePortalApproval))
                approverLevel = _portalApprovalLevelRepository.Table.FirstOrDefault(x => x.PortalApprovalLevelId == znodePortalApproval.PortalApprovalLevelId)?.ApprovalLevelName ?? string.Empty;

            ZnodeLogging.LogMessage("userId, quoteTotal, quoteUserId and approverLevel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId, quoteTotal = quoteTotal, quoteUserId = quoteUserId, approverLevel = approverLevel });

            List<ZnodeUserApprover> userApprovers;
            if (IsNotNull(znodePortalApproval) && string.Equals(approverLevel.ToLower(), Admin_Resources.TextSingleLevel))
                userApprovers = _userApprover.Table.Where(x => x.PortalApprovalId == znodePortalApproval.PortalApprovalId && x.IsActive == true)?.ToList();
            else
            {
                userApprovers = _userApprover.Table.Where(x => x.UserId == quoteUserId)?.ToList();
                if (userApprovers?.Count == 0 && IsNotNull(znodePortalApproval))
                    userApprovers = _userApprover.Table.Where(x => x.PortalApprovalId == znodePortalApproval.PortalApprovalId && x.IsActive == true)?.ToList();
            }
            if (userApprovers?.Count > 0)
            {
                if (userApprovers?.Count == 1)
                    return true;
                int? lastApproveOrder = userApprovers.FirstOrDefault(x => (x.FromBudgetAmount < quoteTotal || x.FromBudgetAmount == quoteTotal) && (x.ToBudgetAmount == null || (x.ToBudgetAmount > quoteTotal || x.ToBudgetAmount == quoteTotal)))?.ApproverOrder;
                int? currentApproverOrder = userApprovers.FirstOrDefault(x => x.ApproverUserId == userId)?.ApproverOrder;
                isLastApprover = Equals(lastApproveOrder, currentApproverOrder) ? true : false;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isLastApprover;
        }

        //Update Quote Line Item Quantity.
        public virtual bool UpdateQuoteLineItemQuantity(AccountQuoteLineItemModel accountQuoteLineItemModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(accountQuoteLineItemModel))
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ModelCanNotBeNull);

            if (IsNull(accountQuoteLineItemModel.OmsQuoteId))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.QuoteIdRequired);

            if (IsNull(accountQuoteLineItemModel.OmsQuoteLineItemId))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.QuoteLineItemIdRequired);

            if (IsNull(accountQuoteLineItemModel.ParentOmsQuoteLineItemId))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.ParentQuoteLineItemIdRequired);

            if (accountQuoteLineItemModel.OmsQuoteId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.InvalidQuoteID);

            if (accountQuoteLineItemModel.OmsQuoteLineItemId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.InvalidQuoteLineItemId);

            ZnodeLogging.LogMessage("Properties of input parameter accountQuoteLineItemModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = accountQuoteLineItemModel?.OmsQuoteId, OmsQuoteLineItemId = accountQuoteLineItemModel?.OmsQuoteLineItemId, ParentOmsQuoteLineItemId = accountQuoteLineItemModel?.ParentOmsQuoteLineItemId });

            bool isQuoteLineItemUpdated = false;
            AccountQuoteModel accountQuote = _omsQuoteRepository.GetById(accountQuoteLineItemModel.OmsQuoteId).ToModel<AccountQuoteModel>();
            if (IsNotNull(accountQuote))
            {
                //Check minimum and maximum product qty validation    
                if (ValidateLineItemQuantity(accountQuote, accountQuoteLineItemModel.OmsQuoteLineItemId, accountQuoteLineItemModel.Quantity))
                {
                    //If the updated quantity of the line item is zero, then delete that line item.
                    if (accountQuoteLineItemModel.Quantity.Equals(0))
                        DeleteQuoteLineItem(accountQuoteLineItemModel.OmsQuoteLineItemId, accountQuoteLineItemModel.ParentOmsQuoteLineItemId.Value, accountQuoteLineItemModel.OmsQuoteId);

                    if (UpdateQuoteQuantityByLineItemId(accountQuoteLineItemModel))
                    {
                        CalculateCartByQuote(accountQuote, null);
                        isQuoteLineItemUpdated = UpdateQuoteAmount(accountQuote);
                    }
                }
            }
            else
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidQuoteID);

            ZnodeLogging.LogMessage(isQuoteLineItemUpdated ? Admin_Resources.SuccessQuoteLineItemUpdate : Admin_Resources.ErrorQuoteLineItemUpdate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isQuoteLineItemUpdated;
        }

        //Calculate Cart by Quote
        public virtual void CalculateCartByQuote(AccountQuoteModel accountQuote, AddressModel shippingAddress = null)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Map parameters of QuoteModel to CartParameterModel.
            CartParameterModel cartParameterModel = ToCartParameterModel(accountQuote);

            //LoadFromDatabase gives required details for Quote line items.
            accountQuote.ShoppingCart = new ShoppingCartMap().ToModel(GetService<IZnodeShoppingCart>().LoadFromDatabase(cartParameterModel));

            if (IsNotNull(shippingAddress))
                accountQuote.ShippingAddressModel = shippingAddress;
            else
                GetShippingBillingAddress(accountQuote);

            //Get calculated cart
            BindDataToShoppingCart(accountQuote, cartParameterModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Delete Quote Line Item by omsQuoteLineItemId, omsParentQuoteLineItemId and omsQuoteId.
        public virtual bool DeleteQuoteLineItem(int omsQuoteLineItemId, int omsParentQuoteLineItemId, int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsQuoteLineItemId = omsQuoteLineItemId, omsParentQuoteLineItemId = omsParentQuoteLineItemId, omsQuoteId = omsQuoteId });

            if (IsNull(omsQuoteId))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.InvalidQuoteID);

            if (omsQuoteId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.QuoteIdRequired);

            if (IsNull(omsParentQuoteLineItemId))
                throw new ZnodeException(ErrorCodes.NotFound,Admin_Resources.InvalidParentQuoteLineItemId);

            if (omsParentQuoteLineItemId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ParentQuoteLineItemIdRequired);

            bool status = false;
            //Check Mimimum and maximum Qty check'

            AccountQuoteModel accountQuote = _omsQuoteRepository.GetById(omsQuoteId).ToModel<AccountQuoteModel>();
            //Check minimum and maximum product qty validation    
            if (IsNotNull(accountQuote))
            {

                if (IsValidLineItemCount(omsQuoteId))
                {
                    if (omsQuoteLineItemId.Equals(0))
                        //Delete all line cart items by omsParentQuoteLineItemId
                        status = DeleteQuoteLineItemFromDataBase(omsParentQuoteLineItemId);
                    else
                        status = ValidateAndDeleteQuoteLineItem(omsQuoteLineItemId, omsParentQuoteLineItemId);
                }
                else
                {
                    if (omsQuoteLineItemId.Equals(0))
                        throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorLastQuoteLineItem);

                    status = ValidateAndDeleteQuoteLineItem(omsQuoteLineItemId, omsParentQuoteLineItemId, true);
                }
            }
            else
            {
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidQuoteID);
            }

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessQuoteLineItemDelete : Admin_Resources.ErrorQuoteLineItemDelete,ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            CalculateCartByQuote(accountQuote, null);
            status = UpdateQuoteAmount(accountQuote);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status;
        }

        //Validate and delete according to the configurable and product
        public bool ValidateAndDeleteQuoteLineItem(int omsQuoteLineItemId, int omsParentQuoteLineItemId, bool showError = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsQuoteLineItemId = omsQuoteLineItemId, omsParentQuoteLineItemId = omsParentQuoteLineItemId, showError = showError });

            bool status;
            //Delete single item from db by child id
            ZnodeOmsQuoteLineItem znodeOmsQuoteLineItem = _omsQuoteLineItemRepository.GetById(omsQuoteLineItemId);
            if (IsNull(znodeOmsQuoteLineItem))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidQuoteLineItemId);
            else if (znodeOmsQuoteLineItem.ParentOmsQuoteLineItemId == omsParentQuoteLineItemId)
            {
                //Check for configurable product
                if (znodeOmsQuoteLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)
                {
                    int configureParentQuoteLineItemCount = _omsQuoteLineItemRepository.Table.Where(x => x.ParentOmsQuoteLineItemId == omsParentQuoteLineItemId).Select(d => d.ParentOmsQuoteLineItemId).Count();
                    if (configureParentQuoteLineItemCount == 1)
                    {
                        if (!showError)
                            //Delete parent and child line items by omsParentQuoteLineItemId
                            status = DeleteQuoteLineItemFromDataBase(omsParentQuoteLineItemId);
                        else
                            throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorLastQuoteLineItem);
                    }
                    else
                        //Delete child line cart item by omsParentQuoteLineItemId and omsQuoteLineItemId
                        status = DeleteQuoteLineItemFromDataBase(omsParentQuoteLineItemId, omsQuoteLineItemId);
                }
                else if (znodeOmsQuoteLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns ||
                    znodeOmsQuoteLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles ||
                    znodeOmsQuoteLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)
                {
                    status = DeleteQuoteLineItemFromDataBase(omsParentQuoteLineItemId);
                }
                else
                {
                    if (!showError)
                        //Delete simple product

                        status = DeleteQuoteLineItemFromDataBase(omsParentQuoteLineItemId);
                    else
                        throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorLastQuoteLineItem);
                }
            }
            else
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidParentQuoteLineItemId);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status;
        }

        //Delete quoteLineItem by parentQuoteLineItemId or quoteLineItemId
        public bool DeleteQuoteLineItemFromDataBase(int parentQuoteLineItemId, int quoteLineItemId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { parentQuoteLineItemId = parentQuoteLineItemId, quoteLineItemId = quoteLineItemId });

            bool status = false;
            if (quoteLineItemId != 0)
            {
                int? checkValidParentQuoteLineId = _omsQuoteLineItemRepository.Table.Where(x => x.OmsQuoteLineItemId == quoteLineItemId).Select(x => x.ParentOmsQuoteLineItemId).FirstOrDefault();
                ZnodeLogging.LogMessage("checkValidParentQuoteLineId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { checkValidParentQuoteLineId = checkValidParentQuoteLineId });
                if (!checkValidParentQuoteLineId.Equals(parentQuoteLineItemId))
                    throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.InvalidParentQuoteLineItemId);
            }
            FilterCollection filters = new FilterCollection();
            if (quoteLineItemId == 0)
            {
                List<int> quoteLineItem = _omsQuoteLineItemRepository.Table.Where(x => x.ParentOmsQuoteLineItemId == parentQuoteLineItemId).Select(d => d.OmsQuoteLineItemId).ToList();
                ZnodeLogging.LogMessage("quoteLineItem list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteLineItemListCount = quoteLineItem?.Count });

                if (IsNotNull(quoteLineItem))
                    filters.Add(new FilterTuple(ZnodeOmsQuoteLineItemEnum.OmsQuoteLineItemId.ToString(), ProcedureFilterOperators.In, string.Join(",", quoteLineItemId, parentQuoteLineItemId, string.Join(",", quoteLineItem.ToArray()))));
                else
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidParentQuoteLineItemId);
            }
            else
                filters.Add(new FilterTuple(ZnodeOmsQuoteLineItemEnum.OmsQuoteLineItemId.ToString(), ProcedureFilterOperators.Equals, quoteLineItemId.ToString()));

            status = _omsQuoteLineItemRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);

            if (!status)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorQuoteLineItemDelete);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return status;
        }

        //Get user approver list.
        public virtual UserApproverListModel GetUserApproverList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get quote details.
            int omsQuoteId = Convert.ToInt32(filters.Find(x => x.FilterName == ZnodeOmsQuoteEnum.OmsQuoteId.ToString().ToLower())?.Item3);
            ZnodeLogging.LogMessage("OmsQuote Id:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, omsQuoteId);
            int? userId; decimal? quoteTotal;

            GetQuoteDetails(omsQuoteId, out userId, out quoteTotal);

            //Fetch data from filters.
            bool showAllApprovers = Convert.ToBoolean(filters.Find(x => x.FilterName == ZnodeConstant.ShowAllApprovers.ToString().ToLower())?.Item3);
            int loggedInUserId = Convert.ToInt32(filters.Find(x => x.FilterName.ToLower() == ZnodeUserEnum.UserId.ToString().ToLower())?.Item3);
            bool userApprover = Convert.ToBoolean(filters.Find(x => x.FilterName == ZnodeConstant.User.ToString().ToLower())?.Item3);
            ZnodeLogging.LogMessage("LoggedInUserId and UserApprover flag:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { LoggedInUserId = loggedInUserId, userApprover = userApprover });
            //Get user access permission for the user.
            UserModel userModel = _userService.GetUserAccessPermission(loggedInUserId);

            //Get user approver details.
            int requiredUserId = (IsNotNull(userId) && !Equals(userId.GetValueOrDefault(), loggedInUserId)) ? userId.GetValueOrDefault() : loggedInUserId;

            UserApproverListModel userApprovers = GetApprovers(omsQuoteId, loggedInUserId, requiredUserId, userModel, userApprover, quoteTotal);
            ZnodeLogging.LogMessage("Properties of UserApproverListModel to be returned:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AccountId = userApprovers?.AccountId, PortalId = userApprovers?.PortalId, AccountPermissionAccessId = userApprovers?.AccountPermissionAccessId, AccountUserPermissionId = userApprovers?.AccountUserPermissionId });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            List<int> approverUserIds = userApprovers?.UserApprovers?.Select(x => x.ApproverUserId)?.ToList();
            int headerUserId = GetLoginUserId();
            if (IsNotNull(approverUserIds) && !approverUserIds.Contains(headerUserId))
            {
                if (IsNotNull(userId) && (headerUserId > 0 && userId != headerUserId))
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.HttpCode_401_AccessDeniedMsg);
            }
            return userApprovers;
        }

        //Update Quote status.
        public IList<AccountQuoteModel> UpdateQuoteStatus(int omsOrderStateId, string omsQuoteIds, string ExceptStatus, out int isUpdated)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            isUpdated = 0;
            IZnodeViewRepository<AccountQuoteModel> objStoredProc = new ZnodeViewRepository<AccountQuoteModel>();
            objStoredProc.SetParameter("@OmsQuoteId", omsQuoteIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@OmsOrderStateId", omsOrderStateId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Status", isUpdated, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@ExceptStatus", ExceptStatus, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@ModifiedBy", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);

            return objStoredProc.ExecuteStoredProcedureList("Znode_UpdateQuoteStatus @OmsQuoteId,@OmsOrderStateId,@Status OUT,@ExceptStatus,@ModifiedBy", 2, out isUpdated);
        }

        //Send mail to next level user approvers.
        public void SendMailToNextLevelApprovers(int portalId, int localeId, int quoteId, string approverIds)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.In, approverIds));
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("WhereClause generated to get approverUsersData:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { WhereClause = whereClause?.WhereClause });

            IList<ZnodeUser> approverUsersData = _userRepository.GetEntityList(whereClause.WhereClause);

            //Send mail to next level approvers.
            foreach (var item in approverUsersData)
                SendQuoteStatusEmailToApprover(item?.FirstName + " " + item?.LastName, item?.Email, Convert.ToInt32(portalId), quoteId, localeId);
        }

        //Get all the approver's comments saved against the quote.
        public List<QuoteApprovalModel> GetApproverComments(int quoteId)
        {
            ZnodeLogging.LogMessage("Input parameter QuoteId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose,new { quoteId = quoteId });
            //Get all approver's comment against the current quote.
            return (from quoteComments in _omsQuoteComment.Table
                    join user in _userRepository.Table on quoteComments.CreatedBy equals user.UserId
                    where quoteComments.OmsQuoteId == quoteId
                    select new QuoteApprovalModel
                    {
                        ApproverUserName = user.FirstName + " " + user.LastName,
                        Comments = quoteComments.Comments,
                        CommentModifiedDateTime = quoteComments.ModifiedDate,
                    }).ToList();
        }

        //Get Account Quote List.Get pending payments and pending orders count for showing account menus
        public virtual UserDashboardPendingOrdersModel GetUserDashboardPendingOrderDetailsCount(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            int accountId = 0;
            int loginUserId = 0;
            //Get accountId and login user id from filter to pass as a parameter in SP.
            GetAccountIdLoginUserIdFromFilters(filters, ref accountId, ref loginUserId);
            //Set Authorized Portal filter based on user portal access.
            BindUserPortalFilter(ref filters);
            PageListModel pageListModel = new PageListModel(filters, null, null);
            ZnodeLogging.LogMessage("pageListModel for executing store procedure Znode_GetOmsQuoteList :", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, SqlDbType.Text);
            objStoredProc.GetParameter("@AccountId", accountId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@UserId", loginUserId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@SalesRepUserId", HelperMethods.GetSalesRepUserId(), ParameterDirection.Input, SqlDbType.Int);
            DataSet dataset = objStoredProc.GetSPResultInDataSet("Znode_GetOmsQuotePendingCount");

            if (IsNull(dataset))
                return new UserDashboardPendingOrdersModel();

            UserDashboardPendingOrdersModel pendingOrderDetailsCountModel = new UserDashboardPendingOrdersModel
            {
                PendingPaymentsCount = Convert.ToInt32(dataset.Tables[0].Rows[0]["PendingPaymentCount"]),
                PendingOrdersCount = Convert.ToInt32(dataset.Tables[1].Rows[0]["ParentPendingOrderCount"])
            };

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return pendingOrderDetailsCountModel;
        }


        #region Template
        //Create template
        public virtual AccountTemplateModel CreateTemplate(AccountTemplateModel shoppingCartModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(shoppingCartModel))
            {
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ErrorShoppingCartModelNull);
            }

            //Check isExisting omsTemplateId available
            if(string.Equals(shoppingCartModel.TemplateType, ZnodeConstant.SaveForLater, StringComparison.InvariantCultureIgnoreCase))
            {
                int omsTemplateId = _templateRepository.Table.Where(x => x.UserId == shoppingCartModel.UserId && x.TemplateType == shoppingCartModel.TemplateType).Select(x => x.OmsTemplateId).FirstOrDefault();
                if (omsTemplateId > 0)
                    shoppingCartModel.OmsTemplateId = omsTemplateId;
            }

            ZnodeLogging.LogMessage("OmsTemplateId property of input parameter shoppingCartModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsTemplateId = shoppingCartModel?.OmsTemplateId });
            if (shoppingCartModel.OmsTemplateId < 1)
            {
                //Save in template.
                ZnodeOmsTemplate template = SaveInTemplate(shoppingCartModel);
                if (HelperUtility.IsNotNull(template) && template.TemplateType != ZnodeConstant.SaveForLater && template.TemplateType != ZnodeConstant.SavedCart)
                {
                    //Save all template line items.
                    SaveAllTemplateLineItems(template.OmsTemplateId, shoppingCartModel);
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return template.ToModel<AccountTemplateModel>();
                }
                else 
                {
                    //Save all template line items.
                    SaveCartLineItemsForLater(template.OmsTemplateId, shoppingCartModel);
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return template.ToModel<AccountTemplateModel>();
                }
            }
            else
            {
                //Update template.
                if (_templateRepository.Update(shoppingCartModel.ToEntity<ZnodeOmsTemplate>()))
                {
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessTemplateUpdated, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    //Save all template line items.
                    if(string.Equals(shoppingCartModel.TemplateType, ZnodeConstant.SaveForLater, StringComparison.InvariantCultureIgnoreCase))
                    {
                        SaveCartLineItemsForLater(shoppingCartModel.OmsTemplateId, shoppingCartModel);
                    }
                    if (string.Equals(shoppingCartModel.TemplateType, ZnodeConstant.SavedCart, StringComparison.InvariantCultureIgnoreCase))
                    {
                        SaveCartLineItemsForLater(shoppingCartModel.OmsTemplateId, shoppingCartModel);
                    }
                    else if (string.Equals(shoppingCartModel.TemplateType, ZnodeConstant.OrderTemplate, StringComparison.InvariantCultureIgnoreCase))
                    {
                        SaveAllTemplateLineItems(shoppingCartModel.OmsTemplateId, shoppingCartModel);
                    }
                }
                else
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorTemplateUpdate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                }
            }
            ZnodeLogging.LogMessage("Properties of shoppingCartModel to be returned:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsTemplateId = shoppingCartModel?.OmsTemplateId, PortalId = shoppingCartModel?.PortalId, PublishedCatalogId = shoppingCartModel?.PublishedCatalogId });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return shoppingCartModel;
        }

        //Edit the Existing Save Cart Product.
        public virtual bool EditSaveCart(AccountTemplateModel shoppingCartModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNotNull(shoppingCartModel))
            {
                SaveCartLineItemsForLater(shoppingCartModel.OmsTemplateId, shoppingCartModel);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(WebStore_Resources.FailedSaveForLater, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Get Template list
        public virtual AccountTemplateListModel GetTemplateList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Get UserId from filter to pass UserId parameter in SP.
            string userId = filters.Find(x => string.Equals(x.FilterName, ZnodeOmsTemplateEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue;
            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeOmsTemplateEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase));
            //Set filter, sort and paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP Znode_GetQuoteOrderTemplateDetail parametes:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<AccountTemplateModel> objStoredProc = new ZnodeViewRepository<AccountTemplateModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeOmsTemplateEnum.UserId.ToString(), userId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<AccountTemplateModel> templateList = objStoredProc.ExecuteStoredProcedureList("Znode_GetQuoteOrderTemplateDetail @WhereClause,@Rows,@PageNo,@Order_BY,@UserId,@RowsCount OUT", 5, out pageListModel.TotalRowCount);

            AccountTemplateListModel listModel = new AccountTemplateListModel { AccountTemplates = templateList?.ToList() };
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Delete template.
        public virtual bool DeleteTemplate(ParameterModel omsTemplateIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter to delete template:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsTemplateIds = omsTemplateIds?.Ids });

            if (HelperUtility.IsNull(omsTemplateIds) || string.IsNullOrEmpty(omsTemplateIds.Ids))
            {
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorTemplateIdLessThanOne);
            }

            //Generates filter clause for multiple omsTemplateIds.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeOmsTemplateLineItemEnum.OmsTemplateId.ToString(), ProcedureFilterOperators.In, omsTemplateIds.Ids));

            List<int> omsTemplateId = (omsTemplateIds.Ids).Split(',').Select(n => Convert.ToInt32(n)).ToList();
            List<int> lineItemId = _templateLineItemRepository.Table.Where(x => omsTemplateId.Contains((int)x.OmsTemplateId)).Select(x => x.OmsTemplateLineItemId).ToList();

            if (lineItemId?.Count > 0)
            {
                    List<ZnodeOmsTemplatePersonalizeCartItem> omsTemplatePersonalizeCartItem = _omsTemplatePersonalizeCartItem.Table.Where(x=> lineItemId.Contains((int)x.OmsTemplateLineItemId)).ToList();
                    if (omsTemplatePersonalizeCartItem?.Count > 0)
                    {
                        List<ZnodeOmsTemplatePersonalizeCartItem> znodeOmsTemplatePersonalizeCartItems = omsTemplatePersonalizeCartItem.ToModel<ZnodeOmsTemplatePersonalizeCartItem>()?.ToList();
                        bool IsDeletedPersonalizeCartItem = _omsTemplatePersonalizeCartItem.Delete(znodeOmsTemplatePersonalizeCartItems);
                    }
            }

            //Returns true if cart item deleted successfully else return false.
            bool IsDeleted = _templateLineItemRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessTemplateCartItemDelete : Admin_Resources.ErrorTemplateCartItemDelete, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            //Returns true if template deleted successfully else return false.
            IsDeleted = _templateRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
                ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessDeleteTemplate : Admin_Resources.ErrorDeleteTemplate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);            
            return IsDeleted;
        }

        //Delete cart items.
        public virtual bool DeleteCartItem(AccountTemplateModel accountTemplateModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(accountTemplateModel?.OmsTemplateLineItemId) || string.IsNullOrEmpty(accountTemplateModel?.OmsTemplateLineItemId.ToString()))
            {
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorTemplateLineIdLessThanOne);
            }
            ZnodeLogging.LogMessage("Input parameter shoppingCartModel properties:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsTemplateId = accountTemplateModel?.OmsTemplateId, OmsTemplateLineItemId = accountTemplateModel?.OmsTemplateLineItemId, PublishedCatalogId = accountTemplateModel?.PublishedCatalogId });

            List<int> associatedLineItemIds = new List<int>();

            List<int> omsTemplateLineItemIds = accountTemplateModel.OmsTemplateLineItemId.Split(',').Select(n => Convert.ToInt32(n)).ToList();
            if (omsTemplateLineItemIds?.Count() > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeOmsTemplateLineItemEnum.ParentOmsTemplateLineItemId.ToString(), ProcedureFilterOperators.In, string.Join(",", omsTemplateLineItemIds)));

                //Get OmsTemplateLineItemId has ParentOmsTemplateLineItemId.
                associatedLineItemIds = (_templateLineItemRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause)?.Select(x => x.OmsTemplateLineItemId))?.ToList();

                int parentOmsTemplateLineItemId = 0;
                parentOmsTemplateLineItemId = Convert.ToInt32(_templateLineItemRepository.GetById(Convert.ToInt32(accountTemplateModel.OmsTemplateLineItemId))?.ParentOmsTemplateLineItemId);

                if (accountTemplateModel.TemplateType == ZnodeConstant.SavedCart)
                {
                    int ZnodeOmsOrderLineItemRelationshipType = Convert.ToInt32(_templateLineItemRepository.GetById(Convert.ToInt32(accountTemplateModel.OmsTemplateLineItemId))?.OrderLineItemRelationshipTypeId);
                    if (parentOmsTemplateLineItemId > 0 && ZnodeOmsOrderLineItemRelationshipType != (int)ZnodeCartItemRelationshipTypeEnum.Group)
                        associatedLineItemIds.Add(parentOmsTemplateLineItemId);
                
                }
                else
                {
                    if (parentOmsTemplateLineItemId > 0)
                        associatedLineItemIds.Add(parentOmsTemplateLineItemId);

                }

                if (omsTemplateLineItemIds?.Count > 0)
                {
                    associatedLineItemIds.AddRange(omsTemplateLineItemIds);
                }
            }

            //Generates filter clause for multiple OmsTemplateLineItemId.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeOmsTemplateLineItemEnum.OmsTemplateId.ToString(), ProcedureFilterOperators.Equals, accountTemplateModel.OmsTemplateId.ToString()));

            if (associatedLineItemIds?.Count() > 0)
            {
                filter.Add(new FilterTuple(ZnodeOmsTemplateLineItemEnum.OmsTemplateLineItemId.ToString(), ProcedureFilterOperators.In, string.Join(",", associatedLineItemIds)));
            }
            else
            {
                filter.Add(new FilterTuple(ZnodeOmsTemplateLineItemEnum.OmsTemplateLineItemId.ToString(), ProcedureFilterOperators.In, accountTemplateModel.OmsTemplateLineItemId.ToString()));
            }
            int omstemplateLineItemId = Int32.Parse(accountTemplateModel.OmsTemplateLineItemId);

            List<ZnodeOmsTemplatePersonalizeCartItem> znodeOmsTemplatePersonalizeCartItemsList = (List<ZnodeOmsTemplatePersonalizeCartItem>)_omsTemplatePersonalizeCartItem.Table.Where(x => associatedLineItemIds.Contains((int)x.OmsTemplateLineItemId)).ToModel<ZnodeOmsTemplatePersonalizeCartItem>();

            if (znodeOmsTemplatePersonalizeCartItemsList.Count() > 0)
            {
                bool IsDeletedPersonalizeCartItem = _omsTemplatePersonalizeCartItem.Delete(znodeOmsTemplatePersonalizeCartItemsList);
            }

            //Returns true if cart item deleted successfully else return false.
            bool IsDeleted = _templateLineItemRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessTemplateCartItemDelete : Admin_Resources.ErrorTemplateCartItemDelete, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return IsDeleted;
        }

        //Delete all cart items from the saved cart later
        public bool DeleteAllCartitemsForLater(int omsTemplateId, bool isFromSavedCart = false )
        {
            if (omsTemplateId > 0)
            {
                bool IsDeleted;
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                //Generates filter clause for multiple omsTemplateIds.
                FilterCollection filter = new FilterCollection();
                filter.Add(new FilterTuple(ZnodeOmsTemplateLineItemEnum.OmsTemplateId.ToString(), ProcedureFilterOperators.Equals, omsTemplateId.ToString()));

                List<int> associatedLineItemIds = new List<int>();
                associatedLineItemIds = (_templateLineItemRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause)?.Select(x => x.OmsTemplateLineItemId))?.ToList();

                List<ZnodeOmsTemplatePersonalizeCartItem> omsTemplatePersonalizeCartItem = null;

                if (associatedLineItemIds?.Count > 0)
                    omsTemplatePersonalizeCartItem = _omsTemplatePersonalizeCartItem.Table.Where(x => associatedLineItemIds.Contains((int)x.OmsTemplateLineItemId)).ToList();
                if (IsNotNull(omsTemplatePersonalizeCartItem) && omsTemplatePersonalizeCartItem.Count > 0)
                {
                    List<ZnodeOmsTemplatePersonalizeCartItem> znodeOmsTemplatePersonalizeCartItems = omsTemplatePersonalizeCartItem.ToModel<ZnodeOmsTemplatePersonalizeCartItem>()?.ToList();
                    bool IsDeletedPersonalizeCartItem = _omsTemplatePersonalizeCartItem.Delete(znodeOmsTemplatePersonalizeCartItems);
                }

                if (isFromSavedCart)
                {
                    //Returns true if cart item deleted successfully else return false.
                    IsDeleted = _templateLineItemRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
                    ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessTemplateCartItemDelete : Admin_Resources.ErrorTemplateCartItemDelete, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return IsDeleted;
                }
                else {
                    //Returns true if cart item deleted successfully else return false.
                    IsDeleted = _templateLineItemRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
                    ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessTemplateCartItemDelete : Admin_Resources.ErrorTemplateCartItemDelete, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                }
                //Returns true if template deleted successfully else return false.
                IsDeleted = _templateRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
                ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessDeleteTemplate : Admin_Resources.ErrorDeleteTemplate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                return IsDeleted;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorTemplateIdLessThanOne);
            }
        }

        public virtual AccountTemplateModel GetTemplate(int templateId, NameValueCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { templateId = templateId });

            if (templateId < 1)
            {
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorTemplateIdLessThanOne);
            } 
            //Get user list.
            List<ZnodeUser> userList = _userRepository.Table?.ToList();

            ZnodeOmsTemplate orderTemplate = _templateRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(new FilterCollection() { new FilterTuple(ZnodeOmsTemplateEnum.OmsTemplateId.ToString(), FilterOperators.Equals, templateId.ToString()) }.ToFilterDataCollection()).WhereClause, GetExpands(expands));

            ZnodeLogging.LogMessage("User list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, userList?.Count);
            //Get account id on the basis of user id.

            int? userAccountId = userList?.Where(x => x.UserId == orderTemplate?.UserId)?.Select(x => x.AccountId)?.FirstOrDefault();

            //Get account id on the basis of login user id.
            int? loginUserAccountId = userList?.Where(x => x.UserId == GetLoginUserId())?.Select(x => x.AccountId)?.FirstOrDefault();

            if ((userAccountId == loginUserAccountId) || (orderTemplate.UserId == GetLoginUserId()))
            {
                if (HelperUtility.IsNotNull(orderTemplate))
                {
                    AccountTemplateModel accountTemplateModel = orderTemplate.ToModel<AccountTemplateModel>();
                    accountTemplateModel.TemplateCartItems = new List<TemplateCartItemModel>();
                    foreach (ZnodeOmsTemplateLineItem lineItem in orderTemplate.ZnodeOmsTemplateLineItems.Where(orderLineItem => orderLineItem.ParentOmsTemplateLineItemId == null))
                    {
                        TemplateCartItemModel templateCartItemModel = lineItem.ToModel<TemplateCartItemModel>();
                        List<ZnodeOmsTemplateLineItem> childItems = orderTemplate.ZnodeOmsTemplateLineItems.Where(o => o.ParentOmsTemplateLineItemId == lineItem.OmsTemplateLineItemId).ToList();
                        SetAssociateProductType(templateCartItemModel, childItems, orderTemplate.PortalId);

                        //Set product addon skus.
                        if (!string.IsNullOrEmpty(templateCartItemModel.ConfigurableProductSKUs) || templateCartItemModel?.GroupProducts?.Count > 0)
                        {
                            SetProductAddonSKUs(templateCartItemModel, orderTemplate.PortalId, filters);
                        }

                        accountTemplateModel.TemplateCartItems.Add(templateCartItemModel);
                    }
                    ZnodeLogging.LogMessage("Properties accountTemplateModel to be returned:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsTemplateId = accountTemplateModel?.OmsTemplateId, PortalId = accountTemplateModel?.PortalId, PublishedCatalogId = accountTemplateModel?.PublishedCatalogId });
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return accountTemplateModel;
                }
                else
                {
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return null;
                }
            }
            else
            {
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.UnableToMatchIds);
            }
        }

        //Get Account Quote on the basis of omsQuoteId.
        public virtual AccountTemplateModel GetAccountTemplate(int templateId, NameValueCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { templateId = templateId });

            if (templateId < 1)
            {
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorTemplateIdLessThanOne);
            }

            ZnodeOmsTemplate orderTemplate = _templateRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(new FilterCollection() { new FilterTuple(ZnodeOmsTemplateEnum.OmsTemplateId.ToString(), FilterOperators.Equals, templateId.ToString()) }.ToFilterDataCollection()).WhereClause, GetExpands(expands));
            if (HelperUtility.IsNotNull(orderTemplate))
            {
                if (orderTemplate?.TemplateType == ZnodeConstant.OrderTemplate)
                {
                    return GetTemplate(templateId, expands, filters);
                }
                //Get user list.
                List<ZnodeUser> userList = _userRepository.Table?.ToList();

                ZnodeLogging.LogMessage("User list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, userList?.Count);
                //Get account id on the basis of user id.

                int? userAccountId = userList?.Where(x => x.UserId == orderTemplate.UserId)?.Select(x => x.AccountId)?.FirstOrDefault();

                //Get account id on the basis of login user id.
                int? loginUserAccountId = userList?.Where(x => x.UserId == GetLoginUserId())?.Select(x => x.AccountId)?.FirstOrDefault();

                if ((userAccountId == loginUserAccountId) || (orderTemplate.UserId == GetLoginUserId()))
                {              
                    AccountTemplateModel accountTemplateModel = orderTemplate.ToModel<AccountTemplateModel>();
                    accountTemplateModel.TemplateCartItems = new List<TemplateCartItemModel>();

                    List<ZnodeOmsTemplateLineItem> parentOmsTemplateLineItem = orderTemplate.ZnodeOmsTemplateLineItems?.Where(x => HelperUtility.IsNull(x.OrderLineItemRelationshipTypeId))?.ToList();
                    List<ZnodeOmsTemplateLineItem> childOmsTemplateLineItem = orderTemplate.ZnodeOmsTemplateLineItems?.Where(x => HelperUtility.IsNotNull(x.OrderLineItemRelationshipTypeId))?.ToList();

                    ZnodeLogging.LogMessage("List details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { parentShoppingCartItems = parentOmsTemplateLineItem?.Count(), childShoppingCartItems = childOmsTemplateLineItem?.Count() });

                    List<TemplateCartItemModel> parentTemplateCartItem = null;
                    if (parentOmsTemplateLineItem?.Count > 0 && childOmsTemplateLineItem?.Count > 0)
                    {
                        parentTemplateCartItem = parentOmsTemplateLineItem.ToModel<TemplateCartItemModel>().ToList();
                        List<TemplateCartItemModel> childTemplateCartItem = childOmsTemplateLineItem.ToModel<TemplateCartItemModel>().ToList();

                        List<TemplateCartItemModel> childCartItems = new List<TemplateCartItemModel>();
                        List<string> publishSkus = GetPublishSKUs(filters, orderTemplate);

                        foreach (TemplateCartItemModel parentItem in parentTemplateCartItem)
                        {
                            if (IsNotNull(publishSkus) && publishSkus.Contains(parentItem.SKU.ToLower()))
                            {
                                bool isParentBundleProduct = false;
                                isParentBundleProduct = (HelperUtility.IsNull(parentItem.ParentOmsTemplateLineItemId) && orderTemplate.ZnodeOmsTemplateLineItems.Any(a => a?.ParentOmsTemplateLineItemId == parentItem.OmsTemplateLineItemId && (a?.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Bundles))));
                                parentItem.CartDescription = parentItem.Description;
                                parentItem.TemplateType = orderTemplate.TemplateType;
                                List<TemplateCartItemModel> childItems = childTemplateCartItem.Where(x => x.ParentOmsTemplateLineItemId == parentItem.OmsTemplateLineItemId && x.OrderLineItemRelationshipTypeId != 1).ToList();
                                SetAssociateProductType(parentItem, childItems, orderTemplate.PortalId);

                                if (childItems?.Count > 0 && !isParentBundleProduct)
                                {
                                    foreach (TemplateCartItemModel item in childItems)
                                    {
                                        if (IsNotNull(publishSkus) && publishSkus.Contains(item.SKU.ToLower()))
                                        {
                                            item.CartDescription = parentItem.Description;
                                            item.TemplateType = orderTemplate.TemplateType;
                                            BindChildLineItem(parentItem, item);
                                            SetPersonalizeCartItemsDetails(item);
                                            SetAssociatedAddOnProducts(orderTemplate, item);
                                        }
                                    }
                                    childCartItems.InsertRange((childCartItems.Count > 0 ? childCartItems.Count - 1 : 0), childItems);
                                }
                                else
                                {
                                    SetAssociatedAddOnProducts(orderTemplate, parentItem);
                                    SetPersonalizeCartItemsDetails(parentItem, childItems);
                                    childCartItems.Add(parentItem);
                                }
                            }
                        }
                        ZnodeLogging.LogMessage("List details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { childCartItems = childCartItems?.Count() });

                        parentTemplateCartItem = childCartItems;
                    }
                    if (parentTemplateCartItem?.Count > 0)
                        accountTemplateModel.TemplateCartItems = (parentTemplateCartItem);
                    ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return accountTemplateModel;
                }
                else
                {
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return null;
                }
            }
            else
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return null;
            }
        }

        public virtual bool AddProductToCartForSaveCart(int omsTemplateId, int userId, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { templateId = omsTemplateId, userId = userId, portalId = portalId });

            try
            {
                if (IsNotNull(omsTemplateId))
                {
                    int status = 0;
                    IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                    objStoredProc.SetParameter("OmsTemplateId", omsTemplateId, ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter("UserId", userId, ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter("PortalId", portalId, ParameterDirection.Input, DbType.Int32);
                    objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
                    objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateMoveToCartWrapper @OmsTemplateId, @UserId, @PortalId, @Status OUT", 3, out status);

                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return Convert.ToBoolean(status);
                }
                else
                {
                    ZnodeLogging.LogMessage(WebStore_Resources.FailedSaveForLater, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                ZnodeLogging.LogMessage("Insert Update Move To Cart SP execution has failed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        public virtual bool EditSaveCartName(string templateName, int templateId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { templateName = templateName, templateId = templateId });

            if (IsNotNull(templateId) && !string.IsNullOrEmpty(templateName))
            {
                ZnodeOmsTemplate model = _templateRepository.GetById(templateId);
                if (IsNotNull(model))
                {
                    model.TemplateName = templateName;
                    _templateRepository.Update(model);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                ZnodeLogging.LogMessage(WebStore_Resources.FailedEditSavedCartName, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Get the cart line items from saved cart later
        public virtual AccountTemplateModel GetCartForLater(int userId, string templateType, NameValueCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, templateType = templateType });

            int omsTemplateId = _templateRepository.Table.Where(x => x.UserId == userId && x.TemplateType == templateType).Select(x => x.OmsTemplateId).FirstOrDefault();
            ZnodeLogging.LogMessage("Execution Done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return GetAccountTemplate(omsTemplateId, expands, filters);
        }

        //Get the list of Personalized attribute order line item on the basis of quoteLineItemId.
        public Dictionary<string, object> GetPersonalizedValueQuoteLineItem(int quoteLineItemId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteLineItemId = quoteLineItemId });

            IZnodeRepository<ZnodeOmsQuotePersonalizeItem> _personalizeItemRepository = new ZnodeRepository<ZnodeOmsQuotePersonalizeItem>();
            Dictionary<string, object> personalizeItem = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> personalizeAttr in _personalizeItemRepository.Table.Where(x => x.OmsQuoteLineItemId == quoteLineItemId)?.ToDictionary(x => x.PersonalizeCode, x => x.PersonalizeValue))
            {
                personalizeItem.Add(personalizeAttr.Key, personalizeAttr.Value);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return personalizeItem;
        }

        //Update quote shipping address 
        public bool UpdateQuoteShippingAddress(UpdateQuoteShippingAddressModel updateQuoteShippingAddressModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter updateQuoteShippingAddressModel properties:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = updateQuoteShippingAddressModel?.OmsQuoteId });

            ValidateQuoteAddressModel(updateQuoteShippingAddressModel);

            AccountQuoteModel accountQuote = _omsQuoteRepository.GetById(updateQuoteShippingAddressModel.OmsQuoteId).ToModel<AccountQuoteModel>();

            if (IsNull(accountQuote))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.QuoteNotFound);

            string countryName = updateQuoteShippingAddressModel?.ShippingAddressModel?.CountryName;
            string stateName = updateQuoteShippingAddressModel?.ShippingAddressModel?.StateName;

            ZnodeLogging.LogMessage("PortalId and ShippingAddressId properties of AccountQuoteModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = accountQuote?.PortalId, ShippingAddressId = accountQuote?.ShippingAddressId });

            //Check store level address validation 
            if (accountQuote?.PortalId > 0 && IsStoreAddressValidationEnabled(accountQuote.PortalId))
            {
                BooleanModel shippingAddress = IsValidAddress(updateQuoteShippingAddressModel?.ShippingAddressModel);
                if (!shippingAddress.IsSuccess)
                    throw new ZnodeException(ErrorCodes.InvalidData, shippingAddress.ErrorMessage);
            }

            //update shipping address in database            
            if (UpdateAddress(accountQuote.ShippingAddressId, updateQuoteShippingAddressModel.ShippingAddressModel))
            {
                CalculateCartByQuote(accountQuote, updateQuoteShippingAddressModel.ShippingAddressModel);

                SetShippingAddressCountryStateName(updateQuoteShippingAddressModel.ShippingAddressModel, countryName, stateName);

                return UpdateQuoteAmount(accountQuote);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return false;
        }

        //To check store address validation enabled by portal Id
        public virtual bool IsStoreAddressValidationEnabled(int portalId)
        {
            IZnodeRepository<ZnodePortalFeature> _portalFeatureRepository = new ZnodeRepository<ZnodePortalFeature>();
            IZnodeRepository<ZnodePortalFeatureMapper> _portalFeatureMapperRepository = new ZnodeRepository<ZnodePortalFeatureMapper>();
            return ((from feature in _portalFeatureRepository.Table
                     join mapper in _portalFeatureMapperRepository.Table on feature.PortalFeatureId equals mapper.PortalFeatureId
                     where feature.PortalFeatureName.ToLower() == StoreFeature.Address_Validation.ToString().ToLower()
                     && mapper.PortalId == portalId
                     select (mapper.PortalFeatureMapperId))?.FirstOrDefault() ?? 0) > 0;
        }

        //check for valid address
        public virtual BooleanModel IsValidAddress(AddressModel address)
        {
            if (IsNotNull(address))
            {
                IShippingService _shippingService = GetService<IShippingService>();
                address.StateName = address.StateCode;
                address.CountryName = address.CountryCode;
                return _shippingService.IsShippingAddressValid(address);
            }
            return null;
        }

        //to update quote tax, shipping and total amount
        public virtual bool UpdateQuoteAmount(AccountQuoteModel model)
        {
            ZnodeLogging.LogMessage("Input parameter AccountQuoteModel properties:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = model?.OmsQuoteId });

            if (model?.OmsQuoteId > 0 && IsNotNull(model?.ShoppingCart?.Total))
            {
                ZnodeOmsQuote quote = _omsQuoteRepository.GetById(model.OmsQuoteId);
                quote.TaxCost = model.ShoppingCart.TaxCost;
                quote.ShippingCost = model.ShoppingCart.ShippingCost;
                quote.QuoteOrderTotal = model.ShoppingCart.Total;
                _omsQuoteRepository.Update(quote);
                return true;
            }
            return false;
        }

        //Update multiple quote line item quantities.
        public virtual QuoteLineItemStatusListModel UpdateQuoteLineItemQuantities(List<AccountQuoteLineItemModel> accountQuoteLineItemModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            QuoteLineItemStatusListModel listResponse = new QuoteLineItemStatusListModel();
            try
            {
                //Check model validations before updating the quantities.
                QuoteLineItemModelValidations(accountQuoteLineItemModel);

                //Get account quote details.
                AccountQuoteModel accountQuote = _omsQuoteRepository.GetById(accountQuoteLineItemModel.FirstOrDefault().OmsQuoteId).ToModel<AccountQuoteModel>();

                //Get the distinct parent line item ids for all the quote line items.
                IEnumerable<int?> parentQuoteLineItemIds = accountQuoteLineItemModel.Select(x => x.ParentOmsQuoteLineItemId).Distinct();

                foreach (int? parentQuoteLineItemId in parentQuoteLineItemIds?.ToList())
                {
                    QuoteLineItemStatusModel itemStatus = new QuoteLineItemStatusModel();
                    if (IsNotNull(parentQuoteLineItemId) || parentQuoteLineItemId > 1)
                    {
                        List<AccountQuoteLineItemModel> model = accountQuoteLineItemModel?.Where(x => x.ParentOmsQuoteLineItemId == parentQuoteLineItemId)?.ToList();
                        List<int> omsQuoteLineItemIds = model?.Select(x => x.OmsQuoteLineItemId)?.Distinct().ToList();
                        if (IsQuoteLineItemValid(parentQuoteLineItemId, omsQuoteLineItemIds) && !(model.Any(x => x.OmsQuoteLineItemId == 0)))
                        {
                            //Get the sum of quantities of one parent item to check for its min/max quantity value.
                            decimal itemsTotalQuantity = 0;
                            if (model?.Count > 0)
                                itemsTotalQuantity = (model?.Select(x => x.Quantity)?.ToList()?.Sum()).GetValueOrDefault();

                            //Get the sku of parent product.
                            string parentSku = GetProductSkuByQuoteId(accountQuote.OmsQuoteId, parentQuoteLineItemId);
                            if (!string.IsNullOrEmpty(parentSku))
                            {
                                //Validate line item quantities by checking its min/max value. 
                                if (ValidateLineItemQuantities(accountQuote, omsQuoteLineItemIds, parentQuoteLineItemId, model, parentSku, itemsTotalQuantity))
                                {
                                    List<AccountQuoteLineItemModel> itemsToUpdate = model?.Where(x => x.Quantity > 0).ToList();

                                    //If the validation qualifies, then update the quantities in the respective quote.
                                    bool quantitiesUpdated = (itemsToUpdate?.Count > 0) ? UpdateQuoteQuantityByQuoteId(itemsToUpdate, accountQuote.OmsQuoteId) : true;

                                    ZnodeLogging.LogMessage(quantitiesUpdated ? Admin_Resources.SuccessUpdateQuoteLineItem : Admin_Resources.ErrorUpdateQuoteLineItem, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                                    itemStatus.Message = quantitiesUpdated ? string.Empty : "Failed to update quantities.";
                                    itemStatus.Status = quantitiesUpdated;
                                }
                                else
                                {
                                    itemStatus.Message = "We’re sorry, but you must order at least the minimum quantity of this product.";
                                    itemStatus.Status = false;
                                }
                                itemStatus.ParentQuoteLineItemId = parentQuoteLineItemId.Value;
                                listResponse.QuoteLineItemStatusList.Add(itemStatus);
                            }
                            else
                            {
                                string message = "Parent Oms Quote Line Item Id doesnot belong to the quote.";
                                SetQuoteLineItemStatusModel(listResponse, parentQuoteLineItemId, itemStatus, message);
                            }
                        }
                        else
                        {
                            string message = "Invalid Oms Quote Line Item Id.";
                            SetQuoteLineItemStatusModel(listResponse, parentQuoteLineItemId, itemStatus, message);
                        }
                    }
                    else
                    {
                        string message = "Parent Oms Quote Line Item Id cannot be null or less than 1.";
                        SetQuoteLineItemStatusModel(listResponse, parentQuoteLineItemId, itemStatus, message);
                    }
                }
                bool isAnyParentLineItemUpdated = listResponse.QuoteLineItemStatusList?.Any(x => x.Status == true) ?? false;
                if (isAnyParentLineItemUpdated)
                {
                    //If validation qualifies and the quantities get updated, calculate cart and update the quote amount.
                    CalculateCartByQuote(accountQuote, null);
                    UpdateQuoteAmount(accountQuote);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listResponse;
        }

        //Get the value of Billing Account Number Global Attribute
        public string GetUsersAdditionalAttributes(int userId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId });
            List<GlobalAttributeValuesModel> globalAttributeList = GetGlobalLevelAttributeList(userId, ZnodeConstant.AccountUser);
            return globalAttributeList?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.BillingAccountNumber)?.AttributeValue;
        }

        #endregion

        #endregion

        #region Private Method

        // Set Products Addons Details.
        private static void SetAssociatedAddOnProducts(ZnodeOmsTemplate orderTemplate, TemplateCartItemModel item)
        {
            List<AssociatedProductModel> list = new List<AssociatedProductModel>();
            List<ZnodeOmsTemplateLineItem> addOnProducts = orderTemplate.ZnodeOmsTemplateLineItems.Where(y => y.ParentOmsTemplateLineItemId == item.OmsTemplateLineItemId && y.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns)).ToList();
            if (addOnProducts?.Count > 0)
            {
                foreach (ZnodeOmsTemplateLineItem addOn in addOnProducts)
                {
                    list.Add(new AssociatedProductModel { Sku = addOn.SKU, Quantity = addOn.Quantity.GetValueOrDefault(), OrderLineItemRelationshipTypeId = Convert.ToInt32(item.OrderLineItemRelationshipTypeId) });
                }
                item.AddOnProductSKUs = string.Join(",", addOnProducts?.Select(b => b.SKU));
                item.AssociatedAddOnProducts = list;
            }

        }

        //Bind Child line item details.
        private void BindChildLineItem(TemplateCartItemModel parentItem, ShoppingCartItemModel item)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (IsNotNull(item))
            {
                item.ConfigurableProductSKUs = (item.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable) ? item.SKU : null;
                item.BundleProductSKUs = (item.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles) ? item.SKU : null;
                item.GroupProducts = (item.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)
                                    ? new List<AssociatedProductModel> { new AssociatedProductModel { Sku = item.SKU, Quantity = item.Quantity } } : null;
                item.SKU = (item.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns) ? item.SKU : parentItem.SKU;
            }

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set personalize Cart item Details.
        private void SetPersonalizeCartItemsDetails(TemplateCartItemModel templateCartItemModel, List<TemplateCartItemModel> childItems = null)
        {
            List<int> childItemsIds = new List<int>();
            if (childItems?.Count > 0)
                childItemsIds = childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles).Select(x => x.OmsTemplateLineItemId)?.ToList();

            childItemsIds.Add(templateCartItemModel.OmsTemplateLineItemId);
            List<ZnodeOmsTemplatePersonalizeCartItem> omsTemplatePersonalizeCartItemsList = _omsTemplatePersonalizeCartItem.Table.Where(x => childItemsIds.Contains((int)x.OmsTemplateLineItemId)).ToList();
            List<PersonaliseValueModel> personaliseValuesDetail = new List<PersonaliseValueModel>();

            if (omsTemplatePersonalizeCartItemsList?.Count() > 0)
            {
                foreach (ZnodeOmsTemplatePersonalizeCartItem omsTemplatePersonalizeCartItems in omsTemplatePersonalizeCartItemsList)
                {
                    PersonaliseValueModel personaliseValue = new PersonaliseValueModel()
                    {
                        OmsSavedCartLineItemId = (int) omsTemplatePersonalizeCartItems.OmsTemplateLineItemId,
                        PersonalizeCode = omsTemplatePersonalizeCartItems.PersonalizeCode,
                        PersonalizeValue = omsTemplatePersonalizeCartItems.PersonalizeValue,
                        ThumbnailURL = omsTemplatePersonalizeCartItems.ThumbnailURL,
                        PersonalizeName = omsTemplatePersonalizeCartItems.PersonalizeName
                    };
                    personaliseValuesDetail.Add(personaliseValue);         
                }
                personaliseValuesDetail = personaliseValuesDetail.GroupBy(x => new { x.OmsSavedCartLineItemId, x.PersonalizeName }).Select(y => y.FirstOrDefault()).Distinct().ToList();
                templateCartItemModel.PersonaliseValuesDetail = personaliseValuesDetail;
            }
        }

        //To set product type data for shoppingcart line item 
        private void SetAssociateProductType(ShoppingCartItemModel lineItem, List<TemplateCartItemModel> childItems, int portalId)
        {
            if (childItems?.Count > 0)
            {
                lineItem.AddOnProductSKUs = string.Join(",", childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns).AsEnumerable().Select(b => b.SKU));
                lineItem.BundleProductSKUs = string.Join(",", childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles).AsEnumerable().Select(b => b.SKU));
                lineItem.ConfigurableProductSKUs = string.Join(",", childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable).AsEnumerable().Select(b => b.SKU));
            }
        }

        //Get accountId and login user id from filter to pass as a parameter in SP while getting quote list.
        private void GetAccountIdLoginUserId(FilterCollection filters, ref int accountId, ref int loginUserId, ref bool isPendingPayment, ref bool isParentPendingOrder)
        {
            accountId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, ZnodeAccountEnum.AccountId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue);
            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeAccountEnum.AccountId.ToString(), StringComparison.CurrentCultureIgnoreCase));

            isPendingPayment = Convert.ToBoolean(filters.Find(x => string.Equals(x.FilterName, ZnodeConstant.PendingPayment, StringComparison.CurrentCultureIgnoreCase))?.FilterValue);
            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeConstant.PendingPayment, StringComparison.CurrentCultureIgnoreCase));

            isParentPendingOrder = Convert.ToBoolean(filters.Find(x => x.FilterName == ZnodeConstant.IsParentPendingOrder.ToLower())?.Item3);
            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeConstant.IsParentPendingOrder, StringComparison.CurrentCultureIgnoreCase));

            if (filters.Any(x => string.Equals(x.FilterName, ZnodeConstant.WebStoreQuotes, StringComparison.CurrentCultureIgnoreCase)))
            {
                //Get userId from filter to pass userId parameter in SP.
                loginUserId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue);
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase));
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeConstant.WebStoreQuotes, StringComparison.CurrentCultureIgnoreCase));
            }
            ZnodeLogging.LogMessage("accountId and loginUserId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { accountId = accountId, loginUserId = loginUserId });
        }

        //Get other quote details for account quote.
        protected virtual AccountQuoteModel GetOtherDetails(AccountQuoteModel accountQuote)
        {
            ZnodeLogging.LogMessage("PortalId and AccountId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = accountQuote?.PortalId, AccountId = accountQuote?.AccountId });

            //Get order status for account quote.
            GetOrderStatus(accountQuote);

            //Get shipping and billing address.
            GetShippingBillingAddress(accountQuote);

            //Get default currency assigned to current portal.
            if (accountQuote.PortalId > 0)
            {
                GetPortalDefaultCurrencyCultureCode(accountQuote.PortalId, accountQuote);
            }
            //Get Shopping cart details required for quote.
            GetCartDetails(accountQuote);

            //Order Number if quote is convert to order
            SetOrderDetails(accountQuote);

            //get personalize attributes by OmsQuoteLineItemId
            foreach (ShoppingCartItemModel lineItem in accountQuote.ShoppingCart.ShoppingCartItems)
            {
                IZnodeOrderHelper znodeOrderHelper = GetService<IZnodeOrderHelper>();
                lineItem.PersonaliseValuesList = GetPersonalizedValueQuoteLineItem(lineItem.OmsQuoteLineItemId);
                lineItem.PersonaliseValuesDetail = znodeOrderHelper.GetPersonalizedAttributeLineItemDetails(lineItem.PersonaliseValuesList, string.Empty);
            }

            //Bind Account name.
            accountQuote.AccountName = accountQuote.AccountId > 0 ? _accountRepository.GetById(accountQuote.AccountId).Name : string.Empty;
            //Bind Store name.
            IZnodeRepository<ZnodePortal> _portal = new ZnodeRepository<ZnodePortal>();
            accountQuote.StoreName = accountQuote.PortalId > 0 ? _portal.GetById(accountQuote.PortalId).StoreName : string.Empty;
            //Get quote notes.
            accountQuote.OrderNotes = GetQuoteNoteDetails(accountQuote.OmsQuoteId);

            accountQuote.BillingAccountNumber = GetUsersAdditionalAttributes(accountQuote.UserId);
            ZnodeLogging.LogMessage("AccountName and BillingAccountNumber properties of AccountQuoteModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AccountName = accountQuote?.AccountName, BillingAccountNumber = accountQuote?.BillingAccountNumber });

            return accountQuote;
        }

        //Set Order Number and OrderId from OmsQuoteId.
        protected virtual void SetOrderDetails(AccountQuoteModel accountQuote)
        {
            if (accountQuote?.OmsQuoteId > 0)
            {
                IZnodeRepository<ZnodeOmsOrder> _orderRepository = new ZnodeRepository<ZnodeOmsOrder>();
                OrderModel order = _orderRepository.Table.Where(x => x.OMSQuoteId == accountQuote.OmsQuoteId)
                    .Select(a => new OrderModel { OmsOrderId = a.OmsOrderId, OrderNumber = a.OrderNumber }).FirstOrDefault();
                if (IsNotNull(order))
                {
                    accountQuote.OrderNumber = order.OrderNumber;
                    accountQuote.OmsOrderId = order.OmsOrderId;
                }
            }
        }

        //Bind attribute details in Quote Line item
        private void BindAttributeDetails(AccountQuoteModel accountQuote)
        {
            IZnodeOrderHelper orderHelper = GetService<IZnodeOrderHelper>();
            List<AccountQuoteLineItemModel> parentquoteLineItemModel = accountQuote?.AccountQuoteLineItemList.Where(x => x.ParentOmsQuoteLineItemId == null).ToList();
            if (IsNotNull(parentquoteLineItemModel))
            {
                foreach (AccountQuoteLineItemModel parentitem in parentquoteLineItemModel)
                {
                    if (IsNotNull(accountQuote?.AccountQuoteLineItemList))
                    {
                        foreach (AccountQuoteLineItemModel quoteLineItem in accountQuote.AccountQuoteLineItemList.Where(y => y.ParentOmsQuoteLineItemId == parentitem.OmsQuoteLineItemId))
                        {
                            quoteLineItem.GroupId = parentitem?.GroupId;
                            quoteLineItem.PersonaliseValueList = GetPersonalizedValueQuoteLineItem(parentitem.OmsQuoteLineItemId);
                            quoteLineItem.PersonaliseValuesDetail = orderHelper.GetPersonalizedAttributeLineItemDetails(quoteLineItem?.PersonaliseValueList, string.Empty);
                        }
                    }
                }
            }

            accountQuote.AccountQuoteLineItemList.ForEach(x =>
            {
                ShoppingCartItemModel lineitem = accountQuote?.ShoppingCart?.ShoppingCartItems.FirstOrDefault(y => y.GroupId == x.GroupId && y.SKU == x.SKU);
                if (IsNotNull(lineitem))
                {
                    x.Quantity = Convert.ToDecimal(ServiceHelper.ToInventoryRoundOff(lineitem.Quantity));
                    x.Attributes = GetProductAttribute(lineitem);
                    x.Price = lineitem.UnitPrice;
                }
            });
            if (IsNotNull(accountQuote?.AccountQuoteLineItemList))
            {
                foreach (var item in accountQuote?.ShoppingCart?.ShoppingCartItems.Where(a => a.ProductType == ZnodeConstant.SimpleProduct && a.ConfigurableProductSKUs == string.Empty && a.BundleProductSKUs == string.Empty && a.GroupProducts.Count() == 0))
                {
                    accountQuote.AccountQuoteLineItemList.RemoveAll(x => x.ParentOmsQuoteLineItemId == item.ParentOmsQuoteLineItemId);
                }

                foreach (var item in accountQuote?.ShoppingCart?.ShoppingCartItems.Where(a => a.ProductType == ZnodeConstant.BundleProduct && a.ParentOmsQuoteLineItemId == null))
                {
                    accountQuote.AccountQuoteLineItemList.RemoveAll(x => x.ParentOmsQuoteLineItemId == item.OmsQuoteLineItemId);
                }

                foreach (var item in accountQuote?.ShoppingCart?.ShoppingCartItems.Where(a => a.ConfigurableProductSKUs != null && a.ConfigurableProductSKUs != string.Empty))
                {
                    accountQuote.AccountQuoteLineItemList.RemoveAll(x => x.OmsQuoteLineItemId == item.ParentOmsQuoteLineItemId);
                }

                if (IsNotNull(accountQuote?.ShoppingCart?.ShoppingCartItems) && accountQuote.ShoppingCart.ShoppingCartItems.Any(a => a.GroupProducts.Count() > 0))
                {
                    List<int?> bundleProductParentId = accountQuote?.ShoppingCart?.ShoppingCartItems.Where(a => a.GroupProducts.Count > 0).Select(a => a.ParentOmsQuoteLineItemId).Distinct().ToList();
                    List<string> skus = new List<string>();
                    foreach (int ids in bundleProductParentId)
                        skus.Add(accountQuote.AccountQuoteLineItemList.Where(x => x.OmsQuoteLineItemId == ids).Select(x => x.SKU).FirstOrDefault());
                    foreach (int parentId in bundleProductParentId)
                       accountQuote.AccountQuoteLineItemList.RemoveAll(x => x.OmsQuoteLineItemId == parentId);

                    foreach(string sku in skus)
                        accountQuote.AccountQuoteLineItemList.RemoveAll(x => x.SKU == sku);

                    for (int i = 0; i < accountQuote.AccountQuoteLineItemList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(accountQuote.AccountQuoteLineItemList[i].Description))
                        {
                            string productName = accountQuote.ShoppingCart.ShoppingCartItems.Where(a => a.OmsQuoteLineItemId == accountQuote.AccountQuoteLineItemList[i].OmsQuoteLineItemId).FirstOrDefault()?.GroupProducts.FirstOrDefault()?.ProductName;
                            accountQuote.AccountQuoteLineItemList[i].Description = productName;
                        }

                    }

                }
            }
        }

        //get the product attribute based on the line item
        private List<OrderAttributeModel> GetProductAttribute(ShoppingCartItemModel lineitem)
        {
            //Count check for Personalized Attributes.
            if (lineitem != null)
            {
                List<OrderAttributeModel> orderLineItems = new List<OrderAttributeModel>();
                OrderAttributeModel model = null;
                //bind Personalized Attributes via key and value.
                foreach (var item in lineitem.ProductAttributes)
                {
                    model = new OrderAttributeModel();
                    model.AttributeCode = item.AttributeCode;
                    model.AttributeValue = item.AttributeValues;
                    model.AttributeValueCode = item.AttributeValueCode;
                    orderLineItems.Add(model);
                }
                ZnodeLogging.LogMessage("orderLineItems list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderLineItems?.Count);
                return orderLineItems;
            }
            return null;
        }

        //Save account quote line items.
        protected virtual AccountQuoteModel SaveQuoteLineItems(ShoppingCartModel shoppingCartModel, ZnodeOmsQuote quote, string approverUserIds)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { approverUserIds = approverUserIds });

            //Add Additional Notes for Quotes.
            AddAdditionalNotes(quote.OmsQuoteId, shoppingCartModel.AdditionalNotes);

            //Save all quote line item.
            SaveAllQuoteCartLineItems(quote.OmsQuoteId, approverUserIds, shoppingCartModel);

            // Remove all saved cart items.
            RemoveSavedCartItems(shoppingCartModel.UserId.GetValueOrDefault(), shoppingCartModel.CookieMappingId, shoppingCartModel.PortalId);

            AccountQuoteModel createdQuote = quote.ToModel<AccountQuoteModel>();

            createdQuote.AccountId = GetAccountId(quote.UserId);
            createdQuote.ShippingAmount = quote.ShippingCost.HasValue ?  quote.ShippingCost.Value : 0 ;

            if (shoppingCartModel.ShoppingCartItems?.Count > 0 && quote.TaxCost > 0)
            {
                SaveQuoteTax(quote.OmsQuoteId, shoppingCartModel.ShoppingCartItems, shoppingCartModel.TaxCost);
            }

            ZnodeLogging.LogMessage("AccountId property of AccountQuoteModel to be returned:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AccountId = createdQuote?.AccountId });
            return createdQuote;
        }

        //Save other quote related details on quote update.
        protected virtual AccountQuoteModel UpdateQuoteDetails(IList<AccountQuoteModel> accountQuoteList, ShoppingCartModel shoppingCartModel, int isUpdated)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter shoppingCartModel properties:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { BillingAddressId = shoppingCartModel?.BillingAddressId, OmsQuoteId = shoppingCartModel?.OmsQuoteId });

            //If shippingId or BillingAddressId or ShippingAddressId is updated, update ZnodeOmsQuote.
            if (accountQuoteList.Select(x => x.ShippingId != shoppingCartModel.ShippingId || x.ShippingAddressId != shoppingCartModel.ShippingAddressId || x.BillingAddressId != shoppingCartModel.BillingAddressId).FirstOrDefault())
            {
                UpdateQuote(shoppingCartModel);
            }

            AccountQuoteModel accountQuoteModel = accountQuoteList.Select(x => x).FirstOrDefault();

            //Update quote amount
            if (HelperUtility.IsNotNull(shoppingCartModel) && HelperUtility.IsNull(accountQuoteModel.ShoppingCart))
            {
                accountQuoteModel.ShoppingCart = shoppingCartModel;
            }
            UpdateQuoteAmount(accountQuoteModel);

            //Send quote updated mail to relevent user.
            AddAdditionalNotes(shoppingCartModel.OmsQuoteId, shoppingCartModel.AdditionalNotes);
            //Delete already existing quote line items.
            DeleteExistingQuoteLineItems(shoppingCartModel.OmsQuoteId);

            //Save/update SavedCartlineItem data in database
            SaveAllQuoteCartLineItems(shoppingCartModel.OmsQuoteId, string.Empty, shoppingCartModel);

            //Send quote updated mail to relevent user.
            SendQuoteUpdateEmail(accountQuoteModel, accountQuoteList, isUpdated);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return accountQuoteModel;
        }

        //Update shippingId / BillingAddressId / ShippingAddressId on quote update..
        protected virtual void UpdateQuote(ShoppingCartModel shoppingCartModel)
        {
            ZnodeOmsQuote znodeOmsQuote = _omsQuoteRepository.Table.FirstOrDefault(x => x.OmsQuoteId == shoppingCartModel.OmsQuoteId);
            if (HelperUtility.IsNotNull(znodeOmsQuote))
            {
                znodeOmsQuote.ShippingId = shoppingCartModel.ShippingId;
                znodeOmsQuote.ShippingAddressId = shoppingCartModel.ShippingAddressId;
                znodeOmsQuote.BillingAddressId = shoppingCartModel.BillingAddressId;

                _omsQuoteRepository.Update(znodeOmsQuote);
            }
        }

        //Add Additional Notes for Quotes.
        private void AddAdditionalNotes(int quoteId, string additionalNotes)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteId = quoteId, additionalNotes = additionalNotes});
            if (!string.IsNullOrEmpty(additionalNotes) && quoteId > 0)
            {
                //Add additional notes for quotes.
                _omsNotesRepository.Insert(new ZnodeOmsNote() { OmsQuoteId = quoteId, Notes = additionalNotes });
            }
        }

        //Delete existing quote line items.
        private void DeleteExistingQuoteLineItems(int omsQuoteId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeOmsQuoteLineItemEnum.OmsQuoteId.ToString(), ProcedureFilterOperators.Equals, omsQuoteId.ToString()));

            //Delete already existing line items.
            _omsQuoteLineItemRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        //Delete draft on the basis of quote id if all line items are deleted.
        private bool DeleteDraft(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("OmsQuoteId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, omsQuoteId);

            FilterCollection quoteFilters = new FilterCollection();
            quoteFilters.Add(new FilterTuple(ZnodeOmsQuoteLineItemEnum.OmsQuoteId.ToString(), ProcedureFilterOperators.Equals, omsQuoteId.ToString()));

            //If any quote line item is not deleted , it will delete quote line item based on quote id.
            ZnodeLogging.LogMessage(_omsQuoteLineItemRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(quoteFilters.ToFilterDataCollection()).WhereClause) ? "Quote line items deleted successfully." : "Failed to delete Quote line items.", string.Empty, TraceLevel.Info);

            bool status = _omsQuoteRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(quoteFilters.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(status ? "Quote deleted successfully." : "Failed to delete Quote.", string.Empty, TraceLevel.Info);
            return status;
        }

        //Send quote updated mail to relevent user.
        private void SendQuoteUpdateEmail(AccountQuoteModel accountQuoteModel, IList<AccountQuoteModel> accountQuoteList, int isUpdated)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //If quote status is PENDING_APPROVAL send email to relevant approver and user about quote creation.
            if (string.Equals(accountQuoteModel.Status, ZnodeConstant.PENDING_APPROVAL, StringComparison.CurrentCultureIgnoreCase))
            {
                string customername = $"{accountQuoteModel?.FirstName} {accountQuoteModel?.LastName}";
                if (string.IsNullOrEmpty(customername.Trim()))
                    customername = accountQuoteModel?.Email;
                SendPendingApprovalEmailToUser(customername, accountQuoteModel.Email, accountQuoteModel.PortalId, accountQuoteModel.OmsQuoteId, accountQuoteModel.LocaleId, string.Empty);
                var onPendingOrderStatusInit = new ZnodeEventNotifier<AccountQuoteModel>(accountQuoteModel, EventConstant.OnPendingOrderStatusNotification);
            }

            //If quote status is APPROVED or REJECTED send quote update email to relevant users. 
            if (Equals(isUpdated, 1) && (string.Equals(accountQuoteModel.Status, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(accountQuoteModel.Status, ZnodeOrderStatusEnum.REJECTED.ToString(), StringComparison.CurrentCultureIgnoreCase)))
            {
                SendQuoteStatusEmailToUser(accountQuoteModel.Status, Convert.ToInt32(accountQuoteModel.PortalId), accountQuoteList?.ToList(), accountQuoteModel.LocaleId);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //To set product type data for shoppingcart line item 
        private void SetAssociateProductType(ShoppingCartItemModel lineItem, List<ZnodeOmsTemplateLineItem> childItems, int portalId)
        {
            lineItem.AddOnProductSKUs = string.Join(",", childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns).AsEnumerable().Select(b => b.SKU));
            lineItem.BundleProductSKUs = string.Join(",", childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles).AsEnumerable().Select(b => b.SKU));
            lineItem.ConfigurableProductSKUs = string.Join(",", childItems.Where(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable).AsEnumerable().Select(b => b.SKU));
            //If OrderLineItemRelationshipTypeId is group type then get group products.
            if (childItems.Any(x => x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group))
            {
                BindGroupProducts(lineItem, childItems, portalId);
            }
        }

        //Bind group products.
        private void BindGroupProducts(ShoppingCartItemModel lineItem, List<ZnodeOmsTemplateLineItem> childItems, int portalId)
        {
            ZnodeLogging.LogMessage("PublishProductId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, lineItem?.ProductId.ToString());

            IPublishProductService _publishProduct = GetService<IPublishProductService>();
            FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.Equals, lineItem?.ProductId.ToString());
            filters.Add(FilterKeys.LocaleId, FilterOperators.Equals, GetDefaultLocaleId().ToString());
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());
            ZnodeLogging.LogMessage("Filters details:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, filters);
            //Get publish group product list and convert List<WebStoreGroupProductModel> to List<AssociatedProductModel>.
            lineItem.GroupProducts = ToAssociatedProductListModel(_publishProduct.GetGroupProducts(filters)?.GroupProducts, childItems);
            CalculateLineItemPrice(lineItem, childItems);
        }

        //To calculate unit price and extended price
        private void CalculateLineItemPrice(ShoppingCartItemModel lineItem, List<ZnodeOmsTemplateLineItem> childItems)
        {
            if (childItems?.Count > 0 && childItems.FirstOrDefault().OrderLineItemRelationshipTypeId.Equals((int)ZnodeCartItemRelationshipTypeEnum.Group))
            {
                foreach (AssociatedProductModel product in lineItem.GroupProducts)
                {
                    lineItem.ExtendedPrice += product.UnitPrice.GetValueOrDefault() * product.Quantity;
                }
            }
            else
            {
                lineItem.UnitPrice = lineItem.UnitPrice * lineItem.Quantity;
            }

            //To set externalid of line item
            lineItem.ExternalId = Guid.NewGuid().ToString();
        }

        //Convert List<WebStoreGroupProductModel> to List<AssociatedProductModel>.
        private List<AssociatedProductModel> ToAssociatedProductListModel(List<WebStoreGroupProductModel> groupProductList, List<ZnodeOmsTemplateLineItem> childItems)
        {
            childItems.RemoveAll(x => x.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Group);
            List<AssociatedProductModel> groupProducts = childItems.ToModel<AssociatedProductModel>()?.ToList();

            foreach (WebStoreGroupProductModel groupProduct in groupProductList)
            {
                if (HelperUtility.IsNotNull(groupProducts))
                {
                    AssociatedProductModel associatedProductModel = groupProducts.FirstOrDefault(x => x.Sku == groupProduct.SKU);
                    if (HelperUtility.IsNotNull(associatedProductModel))
                    {
                        associatedProductModel.ProductId = groupProduct.PublishProductId;
                        associatedProductModel.ProductName = groupProduct.Name;
                        associatedProductModel.UnitPrice = groupProduct.RetailPrice;
                        associatedProductModel.CurrencyCode = groupProduct.CurrencyCode;
                        associatedProductModel.CultureCode = groupProduct.CultureCode;
                        associatedProductModel.InStockMessage = groupProduct.InStockMessage;
                        associatedProductModel.OutOfStockMessage = groupProduct.OutOfStockMessage;
                        associatedProductModel.BackOrderMessage = groupProduct.BackOrderMessage;
                        associatedProductModel.MaximumQuantity = Convert.ToDecimal(groupProduct.Attributes?.Value(ZnodeConstant.MaximumQuantity));
                        associatedProductModel.MinimumQuantity = Convert.ToDecimal(groupProduct.Attributes?.Value(ZnodeConstant.MinimumQuantity));


                        int indexOfSelectedElement = groupProducts.IndexOf(associatedProductModel);
                        groupProducts.RemoveAll(x => x.Sku == groupProduct.SKU);
                        groupProducts.Insert(indexOfSelectedElement, associatedProductModel);
                    }
                }
            }
            ZnodeLogging.LogMessage("GroupProducts list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, groupProducts?.Count);
            return groupProducts;
        }

        //Get expands and add them to navigation properties.
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (!Equals(expands, null) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    //Add expand keys
                    if (Equals(key.ToLower(), ZnodeOmsQuoteEnum.ZnodeOmsQuoteLineItems.ToString().ToLower()))
                    {
                        SetExpands(ZnodeOmsQuoteEnum.ZnodeOmsQuoteLineItems.ToString(), navigationProperties);
                    }

                    if (Equals(key.ToLower(), ZnodeOmsTemplateEnum.ZnodeOmsTemplateLineItems.ToString().ToLower()))
                    {
                        SetExpands(ZnodeOmsTemplateEnum.ZnodeOmsTemplateLineItems.ToString(), navigationProperties);
                    }

                    if (Equals(key.ToLower(), ZnodeUserAddressEnum.ZnodeAddress.ToString().ToLower()))
                    {
                        SetExpands(ZnodeUserAddressEnum.ZnodeAddress.ToString(), navigationProperties);
                    }

                    if (Equals(key.ToLower(), ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString().ToLower()))
                    {
                        SetExpands(ZnodeOmsOrderDetailEnum.ZnodeOmsNotes.ToString(), navigationProperties);
                    }

                    if (Equals(key.ToLower(), ZnodePortalUnitEnum.ZnodeCurrency.ToString().ToLower()))
                    {
                        SetExpands(ZnodePortalUnitEnum.ZnodeCurrency.ToString(), navigationProperties);
                    }

                    if (Equals(key.ToLower(), ZnodePortalUnitEnum.ZnodeCulture.ToString().ToLower()))
                    {
                        SetExpands(ZnodePortalUnitEnum.ZnodeCulture.ToString(), navigationProperties);
                    }
                }
            }
            ZnodeLogging.LogMessage("NavigationProperties list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, navigationProperties?.Count);
            return navigationProperties;
        }

        //Bind required data to shopping cart to get calculated tax and shipping cost.
        private void BindDataToShoppingCart(AccountQuoteModel accountQuote, CartParameterModel cartParameterModel)
        {
            accountQuote.AccountId = GetAccountId(accountQuote.UserId);
            accountQuote.ShoppingCart.LocaleId = cartParameterModel.LocaleId;
            accountQuote.ShoppingCart.PublishedCatalogId = cartParameterModel.PublishedCatalogId;
            accountQuote.ShoppingCart.ShippingAddress = accountQuote.ShippingAddressModel;
            accountQuote.ShoppingCart.BillingAddress = accountQuote.BillingAddressModel;
            accountQuote.ShoppingCart.CurrencyCode = accountQuote.CurrencyCode;
            accountQuote.ShoppingCart.CultureCode = accountQuote.CultureCode;
            accountQuote.ShoppingCart.UserId = accountQuote.UserId;
            accountQuote.ShoppingCart.Shipping.ShippingId = cartParameterModel.ShippingId.GetValueOrDefault();
            accountQuote.ShoppingCart.ProfileId = cartParameterModel?.ProfileId ?? GetProfileId();
            accountQuote.ShoppingCart.IsPendingOrderRequest = string.IsNullOrEmpty(accountQuote.ShoppingCart?.QuoteTypeCode) ? true : false;


            if (HelperUtility.IsNotNull(cartParameterModel?.ShippingId))
            {
                BindShippingDetails(accountQuote, cartParameterModel);
            }

            if(accountQuote.ShoppingCart.IsPendingOrderRequest)
            {
                accountQuote.ShoppingCart.Discount = accountQuote.DiscountAmount.GetValueOrDefault();
                accountQuote.ShoppingCart.ShippingDiscount = accountQuote.ShippingDiscount.GetValueOrDefault();
                accountQuote.ShoppingCart.TaxCost = accountQuote.TaxCost;
                accountQuote.ShoppingCart.ShippingCost = accountQuote.ShippingCost.GetValueOrDefault();
                accountQuote.ShoppingCart.ShippingHandlingCharges = accountQuote.ShippingHandlingCharges.GetValueOrDefault();
                accountQuote.ShoppingCart.ImportDuty = accountQuote.ImportDuty;
            }
            else
            {
                //Gets calculated Shipping and Tax cost.
                accountQuote.ShoppingCart = _shoppingCartService.Calculate(accountQuote.ShoppingCart);
            }




            //The Shopping cost and total will bind only when both ShoppingCartItems and accountQuote have same OmsQuoteId
            if (accountQuote.ShoppingCart.ShoppingCartItems.Count > 0)
            {
                if (accountQuote.ShoppingCart.ShoppingCartItems?.FirstOrDefault().OmsQuoteId == accountQuote.OmsQuoteId)
                {

                    accountQuote.ShoppingCart.ShippingCost = accountQuote.ShippingCost.HasValue ? accountQuote.ShippingCost.Value : 0;
                    accountQuote.ShoppingCart.Total = accountQuote.QuoteOrderTotal;
                }
            }
            //If quote line item quantity is updated , then update quote total in ZnodeOmsQuote.
            if (accountQuote.IsQuoteLineItemUpdated)
            {
                accountQuote.QuoteOrderTotal = (accountQuote?.ShoppingCart?.Total).GetValueOrDefault();
                _omsQuoteRepository.Update(accountQuote.ToEntity<ZnodeOmsQuote>());
            }
            ZnodeLogging.LogMessage("AccountId of AccountQuoteModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AccountId = accountQuote?.AccountId });
        }

        //Bind shipping name and code to model.
        private void BindShippingDetails(AccountQuoteModel accountQuote, CartParameterModel cartParameterModel)
        {
            ZnodeShipping shipping = _shippingRepository.Table.FirstOrDefault(x => x.ShippingId == cartParameterModel.ShippingId);
            if (HelperUtility.IsNotNull(shipping))
            {
                accountQuote.ShoppingCart.Shipping.ShippingCountryCode = shipping.DestinationCountryCode;
                accountQuote.ShippingName = shipping.Description;
            }
        }

        //Get shipping and billing address.
        private void GetShippingBillingAddress(AccountQuoteModel accountQuote)
        {
            ZnodeLogging.LogMessage("BillingAddressId and ShippingAddressId of input AccountQuoteModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { BillingAddressId = accountQuote?.BillingAddressId, ShippingAddressId = accountQuote?.ShippingAddressId });

            IZnodeRepository<ZnodeAddress> _addressRepository = new ZnodeRepository<ZnodeAddress>();

            //Check if Shipping address id is same as Billing address id, assign billing address to shipping address. 
            if (Equals(accountQuote?.ShippingAddressId, accountQuote?.BillingAddressId))
            {
                accountQuote.BillingAddressModel = _addressRepository.Table.Where(x => x.AddressId == accountQuote.BillingAddressId)?.ToModel<AddressModel>()?.FirstOrDefault();
                accountQuote.ShippingAddressModel = accountQuote.BillingAddressModel;
                if (IsNull(accountQuote.ShippingAddressModel.EmailAddress))
                {
                    accountQuote.ShippingAddressModel.EmailAddress = accountQuote?.Email;
                }
            }
            else
            {
                //If Billing address id greater, get billing address.
                if (accountQuote?.BillingAddressId > 0)
                {
                    accountQuote.BillingAddressModel = _addressRepository.Table.Where(x => x.AddressId == accountQuote.BillingAddressId)?.ToModel<AddressModel>()?.FirstOrDefault();
                }

                //If Shipping address id greater, get shipping address.
                if (accountQuote?.ShippingAddressId > 0)
                {
                    accountQuote.ShippingAddressModel = _addressRepository.Table.Where(x => x.AddressId == accountQuote.ShippingAddressId)?.ToModel<AddressModel>()?.FirstOrDefault();
                }
            }
        }

        //Get Order Status.
        private void GetOrderStatus(AccountQuoteModel accountQuote)
        {
            IZnodeRepository<ZnodeOmsOrderState> _orderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
            if (accountQuote?.OmsOrderStateId > 0)
            {
                accountQuote.OrderStatus = _orderStateRepository.Table.Where(x => x.OmsOrderStateId == accountQuote.OmsOrderStateId)?.Select(x => x.OrderStateName).FirstOrDefault();
            }
        }

        //Get payment name by payment setting id 
        private string GetPaymentNameByPaymentSettingId(int? paymentSettingId)
        {
            ZnodeLogging.LogMessage("PaymentSettingId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, paymentSettingId);
            string PaymentDisplayName;

            PaymentDisplayName = _portalPaymentSettingRepository.Table.FirstOrDefault(x => x.PaymentSettingId == paymentSettingId)?.PaymentDisplayName;
            if (IsNull(PaymentDisplayName))
                PaymentDisplayName = (from ps in _paymentSettingRepository.Table
                                      join pt in _paymentType.Table on ps.PaymentTypeId equals pt.PaymentTypeId
                                      where ps.PaymentSettingId == paymentSettingId
                                      select ps.PaymentDisplayName).FirstOrDefault();
            ZnodeLogging.LogMessage("Payment display name:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, PaymentDisplayName);
            return PaymentDisplayName;
        }

        //Get payment name by payment setting id and portal id
        private string GetPaymentNameByPaymentSettingId(int? paymentSettingId, int portalId)
        {
            ZnodeLogging.LogMessage("PaymentSettingId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, paymentSettingId);
            string PaymentDisplayName;

            PaymentDisplayName = _portalPaymentSettingRepository.Table.FirstOrDefault(x => x.PaymentSettingId == paymentSettingId && x.PortalId == portalId)?.PaymentDisplayName;
            if (IsNull(PaymentDisplayName))
                PaymentDisplayName = (from ps in _paymentSettingRepository.Table
                                      join pt in _paymentType.Table on ps.PaymentTypeId equals pt.PaymentTypeId
                                      where ps.PaymentSettingId == paymentSettingId
                                      select ps.PaymentDisplayName).FirstOrDefault();
            ZnodeLogging.LogMessage("Payment display name:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, PaymentDisplayName);

            return PaymentDisplayName;
        }

        //Method to get catalogId on basis of userId
        private void GetCatalogId(AccountQuoteModel accountQuote)
        {
            ZnodeLogging.LogMessage("UserId of input AccountQuoteModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = accountQuote?.UserId });
            //Get accountId on basis of UserId
            int? accountId = _userRepository.Table.Where(x => x.UserId == accountQuote.UserId)?.FirstOrDefault()?.AccountId;
            ZnodeLogging.LogMessage("AccountId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, accountId);
            if (accountId > 0)
            {
                //Get account details on basis of accountId and get catalogId
                ZnodeAccount accountDetails = _accountRepository.GetById(accountId.GetValueOrDefault());

                if (accountDetails?.PublishCatalogId > 0)
                {
                    accountQuote.PublishCatalogId = accountDetails.PublishCatalogId.GetValueOrDefault();
                }
                //If account not present then looking for parent accountId and get catalogId
                else if (accountDetails?.ParentAccountId > 0)
                {
                    ZnodeAccount parentAccountDetails = _accountRepository.GetById(accountDetails.ParentAccountId.GetValueOrDefault());
                    if (parentAccountDetails?.PublishCatalogId > 0)
                    {
                        accountQuote.PublishCatalogId = parentAccountDetails.PublishCatalogId.GetValueOrDefault();
                    }
                    else
                    {
                        GetPublishCatalogId(accountQuote);
                    }
                }
                else
                {
                    GetPublishCatalogId(accountQuote);
                }
            }
            else
            {
                GetPublishCatalogId(accountQuote);
            }
        }

        //If catalog is not present for account then get catalog id on basis of portalId
        private void GetPublishCatalogId(AccountQuoteModel accountQuote)
        {
            int? portalCatalogId = _portalCatalogRepository.Table.Where(x => x.PortalId == accountQuote.PortalId)?.FirstOrDefault()?.PublishCatalogId;
            ZnodeLogging.LogMessage("portalCatalogId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, portalCatalogId);
            if (portalCatalogId > 0)
            {
                accountQuote.PublishCatalogId = portalCatalogId.GetValueOrDefault();
            }
        }

        // Remove all saved cart items.
        private void RemoveSavedCartItems(int userId, string cookieMapping,int portalId)
        {
            int cookieMappingId = !string.IsNullOrEmpty(cookieMapping) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(cookieMapping)) : 0;
            ZnodeLogging.LogMessage("userId and cookieMappingId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, cookieMappingId = cookieMappingId });
            _shoppingCartService.RemoveSavedCartItems(userId, cookieMappingId, portalId);
        }

        //To save/update SavedCartlineItem data in database
        protected virtual bool SaveAllQuoteCartLineItems(int quoteId, string approverUserId, ShoppingCartModel shoppingCart)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteId = quoteId, approverUserId = approverUserId });
            int savedCartId = 0;
            if (quoteId > 0 && !Equals(shoppingCart, null))
            {
                savedCartId = MergeShoppingCart(shoppingCart, savedCartId);

                int status = SaveQuoteLineItemInDB(quoteId, shoppingCart, savedCartId);

                if (status.Equals(1))
                {
                    List<ZnodeOmsQuoteLineItem> quoteLineItems = _omsQuoteLineItemRepository.Table?.Where(x => x.OmsQuoteId == quoteId)?.ToList();
                    List<QuoteDiscountModel> quoteDiscounts = GetQuoteDiscount(shoppingCart, quoteId);

                    if (quoteLineItems?.Count() > 0)
                    {
                        foreach (ShoppingCartItemModel lineItem in shoppingCart.ShoppingCartItems)
                        {
                            int? quoteLineItemId = quoteLineItems.FirstOrDefault(x => x.GroupId == lineItem.GroupId && x.SKU == lineItem.SKU)?.OmsQuoteLineItemId;
                            int? parentQuoteLineItemId = quoteLineItems.FirstOrDefault(x => x.GroupId == lineItem.GroupId && x.SKU == lineItem.SKU)?.ParentOmsQuoteLineItemId;

                            if (lineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Simple || !string.IsNullOrEmpty(lineItem.BundleProductSKUs))
                            {
                                HelperMethods.CurrentContext.ZnodeOmsQuoteLineItems.Where(x => x.OmsQuoteId == quoteId && x.ParentOmsQuoteLineItemId == quoteLineItemId)?.Update(y => new ZnodeOmsQuoteLineItem { Price = lineItem.UnitPrice, ShippingCost = lineItem.ShippingCost });
                            }
                            else if(lineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)
                            {
                                var OmsQuoteLineItem = HelperMethods.CurrentContext?.ZnodeOmsQuoteLineItems?.Where(x => x.OmsQuoteId == quoteId && x.ParentOmsQuoteLineItemId == quoteLineItemId);
                                var sku = string.Empty;
                                foreach (var items in lineItem.GroupProducts)
                                {
                                    if (OmsQuoteLineItem.Any(data => data.SKU == items.Sku))
                                    {
                                        sku = items.Sku;
                                        break;
                                    }
                                }
                                HelperMethods.CurrentContext?.ZnodeOmsQuoteLineItems?.Where(x => x.OmsQuoteId == quoteId && x.ParentOmsQuoteLineItemId == quoteLineItemId && x.SKU == sku)?.Update(y => new ZnodeOmsQuoteLineItem { Price = lineItem.UnitPrice, ShippingCost = lineItem.ShippingCost });
                            }
                            else
                            {
                                parentQuoteLineItemId = quoteLineItems.FirstOrDefault(x => x.GroupId == lineItem.GroupId && x.SKU == lineItem.ConfigurableProductSKUs)?.ParentOmsQuoteLineItemId;
                                HelperMethods.CurrentContext.ZnodeOmsQuoteLineItems.Where(x => x.OmsQuoteId == quoteId && x.ParentOmsQuoteLineItemId == parentQuoteLineItemId)?.Update(y => new ZnodeOmsQuoteLineItem { Price = lineItem.UnitPrice, ShippingCost = lineItem.ShippingCost });
                            }
                            quoteDiscounts = GetQuoteDiscountDetails(quoteDiscounts, lineItem, quoteId, quoteLineItemId, parentQuoteLineItemId);
                        }
                    }

                    //to save quote discount for line item
                    if (quoteDiscounts?.Count > 0)
                    {
                        SaveQuoteDiscount(quoteDiscounts);
                    }

                    SendEmailToApprover(quoteId, approverUserId, shoppingCart);
                }
                return status == 1;
            }
            return false;
        }

        //Insert into ZnodeOmsPersonalizeItem table. 
        private void SavePersonalizeItem(int OmsOrderLineItemsId, OrderLineItemModel orderLineItem)
        {
            ZnodeLogging.LogMessage("OmsOrderLineItemsId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, OmsOrderLineItemsId);

            IZnodeRepository<ZnodeOmsPersonalizeItem> _PersonalizeItemRepository = new ZnodeRepository<ZnodeOmsPersonalizeItem>();

            if (orderLineItem?.PersonaliseValueList?.Count > 0)
            {
                List<ZnodeOmsPersonalizeItem> _OmsPersonalizeItem = new List<ZnodeOmsPersonalizeItem>();
                foreach (var item in orderLineItem.PersonaliseValueList)
                {
                    ZnodeOmsPersonalizeItem model = new ZnodeOmsPersonalizeItem(); model.OmsOrderLineItemsId = OmsOrderLineItemsId; model.PersonalizeCode = item.Key; model.PersonalizeValue = item.Value?.ToString();
                    _OmsPersonalizeItem.Add(model);
                }
                _PersonalizeItemRepository.Insert(_OmsPersonalizeItem);
            }
        }

        //to save/update SavedCartlineItem data in database
        private void SaveAllTemplateLineItems(int savedTemplateId, AccountTemplateModel shoppingCart)
        {
            ZnodeLogging.LogMessage("SavedTemplateId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, savedTemplateId);
            if (savedTemplateId > 0)
            {
                List<AccountTemplateLineItemModel> accountCartLineItem = new List<AccountTemplateLineItemModel>();

                //Check if Shopping cart and cart items are null. If not then add it ro SavedCartLineItemModel.
                if (IsNotNull(shoppingCart?.TemplateCartItems))
                {
                    int sequence = 0;
                    foreach (TemplateCartItemModel cartitem in shoppingCart.TemplateCartItems)
                    {
                        sequence++;
                        cartitem.TemplateType = shoppingCart.TemplateType;
                        accountCartLineItem.Add(BindTemplateLineItemModel(savedTemplateId, cartitem, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId, sequence, cartitem.OmsTemplateLineItemId));
                    }
                }

                string savedCartLineItemXML = HelperUtility.ToXML(accountCartLineItem);

                //SP call to save/update savedCartLineItem
                IZnodeViewRepository<AccountQuoteLineItemModel> objStoredProc = new ZnodeViewRepository<AccountQuoteLineItemModel>();
                objStoredProc.SetParameter("CartLineItemXML", savedCartLineItemXML, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), shoppingCart?.UserId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                List<AccountQuoteLineItemModel> savedCartLineItemList = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateOmsQuoteTemplate @CartLineItemXML, @UserId ,@Status OUT", 2, out status).ToList();
            }
        }

        //Save cart line items for later
        private void SaveCartLineItemsForLater(int savedTemplateId, AccountTemplateModel shoppingCart)
        {
            ZnodeLogging.LogMessage("SavedTemplateId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, savedTemplateId);
            if (savedTemplateId > 0)
            {
                List<AccountTemplateLineItemModel> accountCartLineItem = new List<AccountTemplateLineItemModel>();

                //Check if Shopping cart and cart items are null. If not then add it to SavedCartLineItemModel.
                if (IsNotNull(shoppingCart?.TemplateCartItems))
                {
                    int sequence = 0;
                    foreach (TemplateCartItemModel cartitem in shoppingCart.TemplateCartItems)
                    {
                        sequence++;
                        cartitem.TemplateType = shoppingCart.TemplateType;
                        accountCartLineItem.Add(BindTemplateLineItemModel(savedTemplateId, cartitem, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId, sequence, cartitem.OmsTemplateLineItemId));
                    }
                }
                string savedCartLineItemXML = HelperUtility.ToXML(accountCartLineItem);

                //SP call to save/update savedCartLineItem
                IZnodeViewRepository<AccountQuoteLineItemModel> objStoredProc = new ZnodeViewRepository<AccountQuoteLineItemModel>();
                objStoredProc.SetParameter("CartLineItemXML", savedCartLineItemXML, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), shoppingCart?.UserId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                List<AccountQuoteLineItemModel> savedCartLineItemList = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateSaveForLaterLineItemQuantityWrapper @CartLineItemXML, @UserId ,@Status OUT", 2, out status).ToList();
            }
        }

        //Get Approver details to send mail.
        protected virtual void GetApproverDetails(ShoppingCartModel shoppingCart, int savedCartId, string approverUserIds)
        {
            if (!string.IsNullOrEmpty(approverUserIds))
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.In, approverUserIds));
                EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

                ZnodeLogging.LogMessage("WhereClause generated to get approverUsersData list:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, whereClause?.WhereClause);
                IList<ZnodeUser> approverUsersData = _userRepository.GetEntityList(whereClause.WhereClause);

                foreach (var item in approverUsersData)
                    //This method will send email to relevant approver about quote creation.
                    if (!string.Equals(shoppingCart?.OrderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase) && IsNotNull(shoppingCart))
                    {
                        string approvername = item?.FirstName + " " + item?.LastName;
                        if (string.IsNullOrEmpty(approvername.Trim()))
                        {
                            approvername = item?.Email;
                        }
                        SendQuoteStatusEmailToApprover(approvername, item?.Email, Convert.ToInt32(shoppingCart.UserDetails?.PortalId), savedCartId, shoppingCart.LocaleId);
                    }
            }
        }

        //Check if filter contains IsQuoteLineItemUpdated, assign its value to isQuoteLineItemUpdated variable and remove it from filter.
        private void CheckFilterHasIsQuoteLineItemUpdated(FilterCollection filters, ref bool isQuoteLineItemUpdated)
        {
            if (filters.Any(x => string.Equals(x.FilterName, FilterKeys.IsQuoteLineItemUpdated, StringComparison.CurrentCultureIgnoreCase)))
            {
                isQuoteLineItemUpdated = true;
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.IsQuoteLineItemUpdated, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        //To get ZnodeOmsQuote.
        protected virtual ZnodeOmsQuote GetQuoteCartId(ShoppingCartModel shoppingCartModel, out string approverUserId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            IZnodeRepository<ZnodeAccountUserOrderApproval> _accountUserOrderApproval = new ZnodeRepository<ZnodeAccountUserOrderApproval>();
            IQuoteService quoteService = GetService<IQuoteService>();
            string pendingOrderNumber = "";
            if (quoteService != null)
                pendingOrderNumber = quoteService.GenerateQuoteNumber(shoppingCartModel.PortalId);
            //Check if billing and shipping id is zero ,get and set billing and shipping id.
            CheckBillingShippingId(shoppingCartModel);
            if (shoppingCartModel.FreeShipping)
                shoppingCartModel.ShippingId = _shippingRepository.Table.FirstOrDefault(x => x.ShippingCode == "FreeShipping").ShippingId;
            ZnodeOmsQuote newCart = _omsQuoteRepository.Insert(new ZnodeOmsQuote()
            {
                PortalId = shoppingCartModel.PortalId,
                UserId = shoppingCartModel.UserId.GetValueOrDefault(),
                OmsOrderStateId = GetOmsOrderStateId(shoppingCartModel.OrderStatus),
                ShippingId = shoppingCartModel.ShippingId,
                QuoteOrderTotal = shoppingCartModel.Total,
                ShippingAddressId = shoppingCartModel.ShippingAddressId > 0 ? shoppingCartModel.ShippingAddressId : (shoppingCartModel.ShippingAddress?.AddressId).GetValueOrDefault(),
                BillingAddressId = shoppingCartModel.BillingAddressId > 0 ? shoppingCartModel.BillingAddressId : (shoppingCartModel.BillingAddress?.AddressId).GetValueOrDefault(),
                ApproverUserId = null,
                PaymentSettingId = shoppingCartModel.QuotePaymentSettingId,
                IsPendingPayment = shoppingCartModel.IsPendingPayment,
                CardType = shoppingCartModel.CardType,
                CreditCardExpMonth = shoppingCartModel.CreditCardExpMonth,
                CreditCardExpYear = shoppingCartModel.CreditCardExpYear,
                PaymentTransactionToken = shoppingCartModel.Token,
                CreditCardNumber = shoppingCartModel.CreditCardNumber,
                PoDocument = shoppingCartModel.PODocumentName,
                PurchaseOrderNumber = shoppingCartModel.PurchaseOrderNumber,
                TaxCost = shoppingCartModel.TaxCost,
                ShippingCost = shoppingCartModel.ShippingCost,
                OmsQuoteTypeId = !string.IsNullOrEmpty(shoppingCartModel.QuoteTypeCode) ? GetQuoteTypeIdByCode(shoppingCartModel.QuoteTypeCode) : (int?)null,
                PublishStateId = (byte)shoppingCartModel.PublishStateId,
                InHandDate = shoppingCartModel.InHandDate,
                ShippingConstraintCode = shoppingCartModel.ShippingConstraintCode,
                ImportDuty = shoppingCartModel.ImportDuty,
                ShippingHandlingCharges = shoppingCartModel.Shipping?.ShippingHandlingCharge,
                JobName = shoppingCartModel.JobName,
                AdditionalInstruction = shoppingCartModel.AdditionalInstructions,
                AccountNumber = shoppingCartModel?.Shipping?.AccountNumber,
                ShippingTypeId = shoppingCartModel.Shipping.ShippingTypeId,
                CultureCode = shoppingCartModel.CultureCode,
                Custom1 = shoppingCartModel.Custom1,
                Custom2 = shoppingCartModel.Custom2,
                Custom3 = shoppingCartModel.Custom3,
                Custom4 = shoppingCartModel.Custom4,
                Custom5 = shoppingCartModel.Custom5,
                QuoteNumber = pendingOrderNumber,
                ShippingDiscount = shoppingCartModel.ShippingDiscount,
                DiscountAmount= shoppingCartModel.Discount,
                SubTotal= shoppingCartModel.SubTotal,
                ShippingMethod =shoppingCartModel?.Shipping?.ShippingCode,
                FirstName = shoppingCartModel.UserDetails.FirstName,
                LastName = shoppingCartModel.UserDetails.LastName,
                PhoneNumber = shoppingCartModel.UserDetails.PhoneNumber,
                Email = shoppingCartModel.UserDetails.Email,
                AccountId = shoppingCartModel?.UserDetails?.AccountId,
            });

            SaveQuoteTaxSummaryDetails(newCart.OmsQuoteId, shoppingCartModel.TaxSummaryList);

            ZnodePortalApproval portalApproval = _portalApprovalRepository.Table.FirstOrDefault(x => x.PortalId == shoppingCartModel.PortalId);

            bool isEnableApprovalManagement = portalApproval?.EnableApprovalManagement ?? false;

            if (isEnableApprovalManagement)
            {
                List<ZnodeUserApprover> userApprover = GetUserApprover(shoppingCartModel.UserId.GetValueOrDefault(), portalApproval, shoppingCartModel.PortalPaymentGroupId, shoppingCartModel.Total);
                approverUserId = (IsNotNull(userApprover)) ? string.Join(",", userApprover.Select(x => x.ApproverUserId)) : string.Empty;

                if (IsNotNull(userApprover))
                {
                    foreach (ZnodeUserApprover approver in userApprover)
                    {
                        if (IsNotNull(approver))
                        {
                            //Get the approver of logged in user.
                            ZnodeOMSQuoteApproval omsQuoteApproval = _omsQuoteApproval.Insert(new ZnodeOMSQuoteApproval()
                            {
                                OmsQuoteId = newCart.OmsQuoteId,
                                UserId = shoppingCartModel.UserId.GetValueOrDefault(),
                                OmsOrderStateId = GetOmsOrderStateId(shoppingCartModel.OrderStatus),
                                ApproverLevelId = approver.ApproverLevelId.GetValueOrDefault(),
                                ApproverUserId = approver.ApproverUserId,
                                ApproverOrder = approver.ApproverOrder
                            });
                        }
                    }
                }
            }
            else
            {
                approverUserId = string.Empty;
                ZnodeLogging.LogMessage($"Approvers not available for the selected store {shoppingCartModel?.PortalId}.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            ZnodeLogging.LogMessage(newCart.OmsQuoteId > 0 ? Admin_Resources.SuccessAccountQuoteUpdate : Admin_Resources.ErrorAccountQuoteUpdate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return newCart;
        }

        //Get the quoteTypeId.
        private int? GetQuoteTypeIdByCode(string quoteTypeCode)
        {
            if (_quoteType.Table.FirstOrDefault() != null)
                return _quoteType.Table.FirstOrDefault(x => x.QuoteTypeCode.ToLower() == quoteTypeCode.ToLower()).OmsQuoteTypeId;
            else
                return null;
        }

        //Get the approver of logged in user.
        private List<ZnodeUserApprover> GetUserApprover(int userId, ZnodePortalApproval portalApproval, int portalPaymentGroupId, decimal? orderTotal)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, portalPaymentGroupId = portalPaymentGroupId });
            List<ZnodeUserApprover> approvers = new List<ZnodeUserApprover>();
            List<ZnodeUserApprover> userApprover = null;
            string approverType = _portalApprovalTypeRepository.Table.FirstOrDefault(x => x.PortalApprovalTypeId == portalApproval.PortalApprovalTypeId)?.ApprovalTypeName;
            string approverLevel = _portalApprovalLevelRepository.Table.FirstOrDefault(x => x.PortalApprovalLevelId == portalApproval.PortalApprovalLevelId)?.ApprovalLevelName;
            decimal? userLevelFromBudgetAmount = _userApprover?.Table?.FirstOrDefault(x =>x.UserId == userId)?.FromBudgetAmount;
            if (IsNotNull(approverType) && userId > 0)
            {
                switch (approverType.ToLower())
                {
                    case ZnodeConstant.Store:
                        if (string.Equals(approverLevel.ToLower(), Admin_Resources.TextSingleLevel))
                            userApprover = _userApprover.Table.Where(x => x.PortalApprovalId == portalApproval.PortalApprovalId && x.IsActive == true)?.ToList();
                        else if(string.Equals(approverLevel.ToLower(), Admin_Resources.TextMultiLevel) && orderTotal <= userLevelFromBudgetAmount)
                            userApprover = _userApprover.Table.Where(x => x.PortalApprovalId == portalApproval.PortalApprovalId && x.IsActive == true)?.ToList();
                        else
                            userApprover = GetMultiLevelUserApprovers(userId, portalApproval, portalPaymentGroupId, orderTotal);
                        break;

                    case ZnodeConstant.Payment:
                        if (string.Equals(approverLevel.ToLower(), Admin_Resources.TextSingleLevel))
                            userApprover = _userApprover.Table.Where(x => x.PortalApprovalId == portalApproval.PortalApprovalId && x.IsActive == true && x.PortalPaymentGroupId == portalPaymentGroupId)?.ToList();
                        else if (string.Equals(approverLevel.ToLower(), Admin_Resources.TextMultiLevel) && orderTotal <= userLevelFromBudgetAmount)
                            userApprover = _userApprover.Table.Where(x => x.PortalApprovalId == portalApproval.PortalApprovalId && x.IsActive == true)?.ToList();
                        else
                            userApprover = GetMultiLevelUserApprovers(userId, portalApproval, portalPaymentGroupId, orderTotal);
                        break;

                    default:
                        if (_userApprover.Table.Where(x => x.UserId == userId)?.ToList()?.Count > 0)
                        {
                            int minApproverDisplayOrderLevel = _userApprover.Table.Where(x => x.UserId == userId).Min(x => x.ApproverOrder);
                            userApprover = _userApprover.Table.Where(x => x.UserId == userId && x.ApproverOrder == minApproverDisplayOrderLevel)?.ToList();
                        }
                        break;
                }

            }
            ZnodeLogging.LogMessage("userApprover list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userApproverListCount = userApprover?.Count });
            return userApprover;
        }

        private List<ZnodeUserApprover> GetMultiLevelUserApprovers(int userId, ZnodePortalApproval portalApproval, int portalPaymentGroupId, decimal? orderTotal)
        {
            List<ZnodeUserApprover> approvers = _userApprover.Table.Where(x => x.UserId == userId && orderTotal >= x.FromBudgetAmount)?.ToList();
            if (IsNotNull(approvers) && approvers.Count > 0)
            {
                int minApproverDisplayOrderLevel = approvers.Min(x => x.ApproverOrder);
                return approvers.Where(x => x.ApproverOrder == minApproverDisplayOrderLevel)?.ToList();
            }
            else
            {
                return portalPaymentGroupId > 0 ? _userApprover.Table.Where(x => x.PortalApprovalId == portalApproval.PortalApprovalId && x.IsActive == true && x.PortalPaymentGroupId == portalPaymentGroupId)?.ToList()
                                                : _userApprover.Table.Where(x => x.PortalApprovalId == portalApproval.PortalApprovalId && x.IsActive == true)?.ToList();
            }
        }

        //Get OmsOrderStateId based on order status.
        protected virtual int GetOmsOrderStateId(string orderStatus)
        {
            ZnodeLogging.LogMessage("OrderStatus:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, orderStatus);
            IZnodeRepository<ZnodeOmsOrderState> _orderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
            return !string.IsNullOrEmpty(orderStatus) ? Convert.ToInt32(_orderStateRepository.Table.FirstOrDefault(x => string.Equals(x.OrderStateName, orderStatus))?.OmsOrderStateId)
                   : Convert.ToInt32(_orderStateRepository.Table.FirstOrDefault(x => string.Equals(x.OrderStateName, ZnodeConstant.PENDING_APPROVAL))?.OmsOrderStateId);
        }

        //Check if billing and shipping id is zero ,get and set billing and shipping id.
        protected virtual void CheckBillingShippingId(ShoppingCartModel shoppingCartModel)
        {
            if ((shoppingCartModel.ShippingAddressId <= 0 || shoppingCartModel.BillingAddressId <= 0) &&
                (shoppingCartModel?.ShippingAddress?.AddressId <= 0 || shoppingCartModel?.BillingAddress?.AddressId <= 0))
            {
                IZnodeRepository<ZnodeAccountAddress> _accountAddressRepository = new ZnodeRepository<ZnodeAccountAddress>();

                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeAccountAddressEnum.AccountId.ToString(), FilterOperators.Equals, shoppingCartModel.UserDetails.AccountId.ToString()));
                EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("WhereClause generated to get address list:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, whereClause);
                //expand for address.
                NameValueCollection expands = new NameValueCollection();
                expands.Add(ZnodeAccountAddressEnum.ZnodeAddress.ToString(), ZnodeAccountAddressEnum.ZnodeAddress.ToString());
                List<ZnodeAccountAddress> address = _accountAddressRepository.GetEntityList(whereClause.WhereClause, GetExpands(expands), whereClause.FilterValues)?.ToList();
                shoppingCartModel.BillingAddressId = (address.Where(x => x.ZnodeAddress.IsDefaultBilling)?.FirstOrDefault().AddressId).GetValueOrDefault();
                shoppingCartModel.ShippingAddressId = (address.Where(x => x.ZnodeAddress.IsDefaultShipping)?.FirstOrDefault().AddressId).GetValueOrDefault();
            }
        }

        //Bind Saved Cart line item model.
        private AccountQuoteLineItemModel BindQuoteCartLineItemModel(int quoteCartId, ShoppingCartItemModel shoppingCartItem, int publishCatalogId, int localeId, int itemSequence)
        {
            ZnodeLogging.LogMessage("QuoteCartId, PublishCatalogId, LocaleId, ItemSequence:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose,new object[] { quoteCartId, publishCatalogId, localeId, itemSequence });
            IZnodeOrderHelper znodeOrderHelper = GetService<IZnodeOrderHelper>();
            return new AccountQuoteLineItemModel
            {
                OmsQuoteId = quoteCartId,
                SKU = shoppingCartItem.SKU,
                Quantity = shoppingCartItem.Quantity,
                AddonProducts = !string.IsNullOrEmpty(shoppingCartItem.AddOnProductSKUs) ? shoppingCartItem.AddOnProductSKUs : znodeOrderHelper.GetAddOnProducts(shoppingCartItem.ProductId, publishCatalogId, localeId),
                BundleProducts = znodeOrderHelper.GetBundleProducts(shoppingCartItem.ProductId, shoppingCartItem.ProductType, publishCatalogId, localeId),
                ConfigurableProducts = !string.IsNullOrEmpty(shoppingCartItem.ConfigurableProductSKUs) ? shoppingCartItem.ConfigurableProductSKUs : znodeOrderHelper.GetConfigurableProducts(shoppingCartItem.ProductId, shoppingCartItem.ProductType, publishCatalogId, localeId, GetCatalogVersionForDefaultPublishState(0, localeId, publishCatalogId)),
                GroupProducts = (shoppingCartItem?.GroupProducts?.Count > 0) ? znodeOrderHelper.GetGroupProductLineItemXMLData(shoppingCartItem?.GroupProducts) : znodeOrderHelper.GetGroupProducts(shoppingCartItem.ProductId, shoppingCartItem.ProductType, publishCatalogId, localeId),
                PersonaliseValuesDetail = znodeOrderHelper.GetPersonalizedAttributeLineItemDetails(shoppingCartItem?.PersonaliseValuesList, string.Empty),
            };

        }

        //Bind Saved Cart line item model.
        private AccountTemplateLineItemModel BindTemplateLineItemModel(int quoteCartId, ShoppingCartItemModel shoppingCartItem, int publishCatalogId, int localeId, int itemSequence, int omsTemplateLineItemId)
        {
            ZnodeLogging.LogMessage("QuoteCartId, PublishCatalogId, LocaleId, ItemSequence, omsTemplateLineItemId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { quoteCartId, publishCatalogId, localeId, itemSequence, omsTemplateLineItemId });

            IZnodeOrderHelper orderHelper = GetService<IZnodeOrderHelper>();

            if (IsNotNull(shoppingCartItem))
            {
                return new AccountTemplateLineItemModel
                {
                    OmsTemplateId = quoteCartId,
                    SKU = shoppingCartItem.SKU,
                    Quantity = shoppingCartItem.Quantity,
                    AddOnQuantity = !string.IsNullOrEmpty(shoppingCartItem.AddOnProductSKUs) ? shoppingCartItem.Quantity : 0,
                    AddonProducts = !string.IsNullOrEmpty(shoppingCartItem.AddOnProductSKUs) ? shoppingCartItem.AddOnProductSKUs : orderHelper.GetAddOnProducts(shoppingCartItem.ProductId, publishCatalogId, localeId),
                    BundleProducts = (shoppingCartItem.TemplateType == ZnodeConstant.SaveForLater || shoppingCartItem.TemplateType == ZnodeConstant.SavedCart) ? shoppingCartItem.BundleProductSKUs : orderHelper.GetBundleProducts(shoppingCartItem.ProductId, shoppingCartItem.ProductType, publishCatalogId, localeId),
                    ConfigurableProducts = !string.IsNullOrEmpty(shoppingCartItem.ConfigurableProductSKUs) ? shoppingCartItem.ConfigurableProductSKUs : orderHelper.GetConfigurableProducts(shoppingCartItem.ProductId, shoppingCartItem.ProductType, publishCatalogId, localeId, GetCatalogVersionForDefaultPublishState(0, localeId, publishCatalogId)),
                    GroupProducts = (shoppingCartItem.GroupProducts?.Count > 0) ? orderHelper.GetGroupProductLineItemXMLDataForTemplate(shoppingCartItem.GroupProducts, shoppingCartItem.TemplateType) : orderHelper.GetGroupProducts(shoppingCartItem.ProductId, shoppingCartItem.ProductType, publishCatalogId, localeId),
                    OmsTemplateLineItemId = omsTemplateLineItemId,
                    ParentOmsTemplateLineItemId = shoppingCartItem.GroupProducts?.Count() > 0 ? shoppingCartItem.GroupProducts.FirstOrDefault()?.ParentOmsTemplateLineItemId : null,
                    PersonaliseValuesDetail = shoppingCartItem.PersonaliseValuesDetail,
                    Description = shoppingCartItem.Description,
                    ProductName = shoppingCartItem.ProductName
                };
            }
            else
                return new AccountTemplateLineItemModel();
        }

        //Save in template.
        private ZnodeOmsTemplate SaveInTemplate(AccountTemplateModel shoppingCartModel)
        {
            ZnodeOmsTemplate createdTemplate = _templateRepository.Insert(new ZnodeOmsTemplate() { PortalId = shoppingCartModel.PortalId, UserId = Convert.ToInt32(shoppingCartModel.UserId), TemplateName = shoppingCartModel.TemplateName, TemplateType = shoppingCartModel.TemplateType });
            ZnodeLogging.LogMessage(createdTemplate?.OmsTemplateId > 0 ? Admin_Resources.SuccessTemplateCreate : Admin_Resources.ErrorTemplateCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return createdTemplate;
        }

        //This method will send email to relevant approver about quote creation.        
        protected virtual void SendQuoteStatusEmailToApprover(string approverName, string approverEmail, int portalId, int quoteId, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { approverName = approverName, approverEmail = approverEmail, portalId = portalId, quoteId = quoteId,localeId = localeId });

            PortalModel portalModel = GetCustomPortalDetails(portalId);
            string baseUrl = GetDomains(portalId);
            baseUrl = (IsNotNull(portalModel) && portalModel.IsEnableSSL) ? $"https://{baseUrl}" : $"http://{baseUrl}";
            string quoteUrl = $"{baseUrl}/User/QuoteView?omsQuoteId={quoteId}";
            string quoteLink = $"<a href=\"{quoteUrl}\">{quoteId}</a>";

            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.PendingApproval, portalId, localeId);

            if (IsNotNull(emailTemplateMapperModel))
            {
                string subject = $"{emailTemplateMapperModel?.Subject} - {portalModel?.StoreName}";

                StringBuilder approverCommentHtml = GenerateTextForApproverComments(quoteId);

                string messageText = emailTemplateMapperModel.Descriptions;
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.QuoteApprover, approverName, messageText);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.QuoteId, quoteLink.ToString(), messageText);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.ApproverComments, approverCommentHtml.ToString(), messageText);
                messageText = ReplaceTokenWithMessageText("#StoreLogo", GetCustomPortalDetails(portalId)?.StoreLogo, messageText);

                //Send  mail to approver.
                SendEmail(approverName, approverEmail, subject, messageText, portalId, emailTemplateMapperModel.IsEnableBcc);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get domains on the basis of portalId.
        private string GetDomains(int portalId)
        {
            return _domainRepository.Table.Where(x => x.PortalId == portalId && x.ApplicationType == ZnodeConstant.WebStore && x.IsActive && x.IsDefault == true)?.Select(x => x.DomainName)?.FirstOrDefault()?.ToString();
        }

        //Get Admin Domain Details.
        private PortalModel GetAdminDomainDetails()
        {
            PortalModel model;

            IZnodeRepository<ZnodePortal> znodePortal = new ZnodeRepository<ZnodePortal>();

            model = (from portal in znodePortal.Table
                     join domain in _domainRepository.Table.Where(x => x.ApplicationType == ZnodeConstant.Admin && x.IsActive) on portal.PortalId equals domain.PortalId into pf
                     from portalDomain in pf.DefaultIfEmpty()
                     select new PortalModel
                     {
                         PortalId = portalDomain.PortalId,
                         DomainUrl = portalDomain.DomainName,
                         IsEnableSSL = portal.UseSSL,
                     }).FirstOrDefault();

            return model;
        }

        //This method will send quote update email to relevant users.        
        public virtual void SendQuoteStatusEmailToUser(string status, int portalId, List<AccountQuoteModel> accountQuoteList, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { status = status, portalId = portalId, localeId = localeId });
            PortalModel portalModel = GetCustomPortalDetails(portalId);
            string baseUrl = GetDomains(portalId);
            string onApproveRejectQuote = string.Empty;
            baseUrl = (IsNotNull(portalModel) && portalModel.IsEnableSSL) ? $"https://{baseUrl}" : $"http://{baseUrl}";

            EmailTemplateMapperModel emailTemplateMapperModel;
            if (string.Equals(status, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.StatusApproved, portalId, localeId);
                onApproveRejectQuote = EventConstant.OnPendingOrderApproved;
            }
            else
            {
                emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.StatusRejected, portalId, localeId);
                onApproveRejectQuote = EventConstant.OnPendingOrderRejected;
            }

            if (IsNotNull(emailTemplateMapperModel))
            {
                string subject = $"{emailTemplateMapperModel?.Subject} - {portalModel?.StoreName}";
                StringBuilder approverCommentHtml = GenerateTextForApproverComments(Convert.ToInt32(accountQuoteList?.FirstOrDefault()?.OmsQuoteId));
                foreach (AccountQuoteModel accountQuote in accountQuoteList)
                {
                    string quoteUrl = $"{baseUrl}/User/QuoteView?omsQuoteId={accountQuote?.OmsQuoteId.ToString()}";
                    string quoteLink = $"<a href=\"{quoteUrl}\">{accountQuote?.OmsQuoteId.ToString()}</a>";
                    string messageText = emailTemplateMapperModel.Descriptions;
                    accountQuote.UserName = $"{accountQuote?.FirstName} {accountQuote?.LastName}";
                    if (string.IsNullOrEmpty(accountQuote.UserName.Trim()))
                        accountQuote.UserName = accountQuote?.Email;
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.CustomerName, accountQuote?.UserName, messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.QuoteId, quoteLink.ToString(), messageText);
                    messageText = ReplaceTokenWithMessageText(ZnodeConstant.ApproverComments, approverCommentHtml.ToString(), messageText);
                    messageText = ReplaceTokenWithMessageText("#StoreLogo", GetCustomPortalDetails(portalId)?.StoreLogo, messageText);
                    //Send mail to users.
                    SendEmail(accountQuote?.UserName, accountQuote?.Email, subject, messageText, portalId, emailTemplateMapperModel.IsEnableBcc);
                    var onApproveRejectQuoteInit = new ZnodeEventNotifier<AccountQuoteModel>(accountQuote, onApproveRejectQuote);
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //This method creates the html for displaying comments of the approvers in all quote related emails.
        private StringBuilder GenerateTextForApproverComments(int quoteId)
        {
            //Approver Comments.
            List<QuoteApprovalModel> approverComments = GetApproverComments(quoteId);
            StringBuilder approverCommentHtml = new StringBuilder();
            if (approverComments?.Count > 0)
            {
                approverCommentHtml.Append("<h3 style='padding:5px 0;'>" + WebStore_Resources.LabelApproverComments + "</h3>");
                approverCommentHtml.Append("<table><thead style='border-bottom:1px solid #000;box-shadow: 0 1px 0 #000;'><tr>");
                approverCommentHtml.Append("<td width=15% style='font-family: Arial; font-size: 10pt;padding:5px;border-bottom:1px solid #000;'>" + WebStore_Resources.TitleDate + "</td>");
                approverCommentHtml.Append("<td width=15% style='font-family: Arial; font-size: 10pt;padding:5px;border-bottom:1px solid #000;'>" + WebStore_Resources.TextApproverName + "</td>");
                approverCommentHtml.Append("<td width=70% style='font-family: Arial; font-size: 10pt;padding:5px;border-bottom:1px solid #000;'>" + WebStore_Resources.LabelComments + "</td>");
                approverCommentHtml.Append("</tr></thead>");

                foreach (QuoteApprovalModel approverCommentsData in approverComments.AsEnumerable().Reverse())
                {
                    approverCommentHtml.Append("<tr style='border-bottom:1px solid #000;box-shadow: 0 1px 0 #000;'>");
                    approverCommentHtml.Append("<td width=15% style='font-family: Arial; font-size: 10pt;padding:5px;border-bottom:1px solid #000;'> " + approverCommentsData.CommentModifiedDateTime + "</td>");
                    approverCommentHtml.Append("<td width=15% style='font-family: Arial; font-size: 10pt;padding:5px;border-bottom:1px solid #000;'>" + approverCommentsData.ApproverUserName + "</td>");
                    approverCommentHtml.Append("<td width=70% style='font-family: Arial; font-size: 10pt;padding:5px;border-bottom:1px solid #000;'>" + approverCommentsData.Comments + "</td>");
                    approverCommentHtml.Append("</tr>");
                }
                approverCommentHtml.Append("</table>");
            }

            return approverCommentHtml;
        }

        // Send email when pending payment quote get rejected.
        private void QuoteRejectMail(int omsQuoteId, string userName, string email, int portalId, int localeId)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsQuoteId = omsQuoteId, userName = userName, email = email, portalId = portalId, localeId = localeId });
            PortalModel portalModel = GetCustomPortalDetails(portalId);
            string baseUrl = GetDomains(portalId);
            baseUrl = (IsNotNull(portalModel) && portalModel.IsEnableSSL) ? $"https://{baseUrl}" : $"http://{baseUrl}";

            EmailTemplateMapperModel emailTemplateMapperModel;
            emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.StatusRejected, portalId, localeId);

            if (IsNotNull(emailTemplateMapperModel))
            {
                string subject = $"{emailTemplateMapperModel.Subject} - {portalModel?.StoreName}";
                string quoteUrl = $"{baseUrl}/User/PendingPaymentQuoteView?omsQuoteId={omsQuoteId}";
                string quoteLink = $"<a href=\"{quoteUrl}\">{omsQuoteId}</a>";
                string messageText = emailTemplateMapperModel.Descriptions;
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.CustomerName, userName, messageText);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.QuoteId, quoteLink.ToString(), messageText);
                messageText = ReplaceTokenWithMessageText("#StoreLogo#", portalModel?.StoreLogo, messageText);
                //Send mail to users.
                if (string.IsNullOrEmpty(userName.Trim()))
                    userName = email;
                SendEmail(userName, email, subject, messageText, portalId, emailTemplateMapperModel.IsEnableBcc);
            }
        }

        //Send Email.
        protected virtual void SendEmail(string userName, string email, string subject, string messageText, int portalId, bool isEnableBcc)
        {
            ZnodeEmail.SendEmail(portalId, email, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(isEnableBcc, portalId, string.Empty), subject, messageText, true);
        }

        //Get account name and customer name.
        private void BindAccountCustomerName(int? userId, int accountId, AccountQuoteListModel accountQuoteListModel)
        {
            //If quote list count is greater, get customer name from quote list else by userId.
            if (HelperUtility.IsNotNull(userId))
            {
                accountQuoteListModel.CustomerName = accountQuoteListModel?.AccountQuotes?.Count > 0
                                          ? accountQuoteListModel?.AccountQuotes?.Select(x => x.UserName)?.FirstOrDefault()
                                          : new ZnodeRepository<ZnodeUser>().Table.Where(x => x.UserId == userId).Select(x => x.FirstName + " " + x.LastName)?.FirstOrDefault();
            }

            //Get account name from ZnodeAccount on the basis of account id.
            if (accountId > 0)
            {
                ZnodeAccount accountEntity = _accountRepository.Table.FirstOrDefault(x => x.AccountId == accountId);
                if (HelperUtility.IsNotNull(accountEntity))
                {
                    accountQuoteListModel.AccountName = accountEntity.Name;
                    accountQuoteListModel.HasParentAccounts = HelperUtility.IsNull(accountEntity.ParentAccountId);
                }
            }
        }

        //Get account id on basis of user id.
        private int GetAccountId(int userId)
        {
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId });
            return Convert.ToInt32(_userRepository.Table.FirstOrDefault(x => x.UserId == userId)?.AccountId);
        }

        //Get portal id on basis of user id. 
        private int GetPortalId(int loggedInUserId)
        {
            return Convert.ToInt32(_userPortalRepository.Table?.FirstOrDefault(x => x.UserId == loggedInUserId)?.PortalId);
        }

        //Bind Group product details in Shopping cart line items from quote line items.
        private void BindGroupProductDetails(AccountQuoteModel accountQuote)
        {
            foreach (ShoppingCartItemModel shoppingCartItem in accountQuote?.ShoppingCart?.ShoppingCartItems)
            {
                if (shoppingCartItem?.GroupProducts.Count > 0)
                {
                    foreach (AssociatedProductModel groupProduct in shoppingCartItem.GroupProducts)
                    {
                        //Get quote line item on the basis of sku to bind data.
                        AccountQuoteLineItemModel accountLineItem = accountQuote?.AccountQuoteLineItemList.Where(x => x.SKU == groupProduct.Sku)?.FirstOrDefault();

                        if (HelperUtility.IsNotNull(accountLineItem))
                        {
                            groupProduct.OmsQuoteLineItemId = accountLineItem.OmsQuoteLineItemId;
                            groupProduct.OmsQuoteId = accountLineItem.OmsQuoteId;
                            groupProduct.ParentOmsQuoteLineItemId = accountLineItem.ParentOmsQuoteLineItemId;
                            groupProduct.OrderLineItemRelationshipTypeId = accountLineItem.OrderLineItemRelationshipTypeId.GetValueOrDefault();
                            groupProduct.CustomText = accountLineItem.CustomText;
                            groupProduct.CartAddOnDetails = accountLineItem.CartAddOnDetails;
                        }
                    }
                }
            }
        }

        //Get quotenote list.
        private List<OrderNotesModel> GetQuoteNoteDetails(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsQuoteId = omsQuoteId });
            IZnodeRepository<View_GetOmsOrderNotes> _viewOmsOrderNoteList = new ZnodeRepository<View_GetOmsOrderNotes>();
            FilterCollection filters = new FilterCollection();

            if (omsQuoteId > 0)
            {
                filters.Add(new FilterTuple(ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), FilterOperators.Equals, omsQuoteId.ToString()));
            }

            if (filters.Count > 0)
            {
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                return _viewOmsOrderNoteList.GetEntityList(whereClauseModel.WhereClause)?.AsEnumerable().ToModel<OrderNotesModel>()?.ToList();
            }
            return new List<OrderNotesModel>();
        }

        //Get default currency assigned to current portal.
        private void GetPortalDefaultCurrencyCultureCode(int portalId, AccountQuoteModel accountQuoteModel)
        {
            IZnodeRepository<ZnodePortalUnit> _portalUnitRepository = new ZnodeRepository<ZnodePortalUnit>();
            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());

            NameValueCollection expand = new NameValueCollection();
            expand.Add(ZnodePortalUnitEnum.ZnodeCurrency.ToString(), ZnodePortalUnitEnum.ZnodeCurrency.ToString());
            expand.Add(ZnodePortalUnitEnum.ZnodeCulture.ToString(), ZnodePortalUnitEnum.ZnodeCulture.ToString());
            ZnodePortalUnit portalUnit = _portalUnitRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause, GetExpands(expand));
            accountQuoteModel.CurrencyCode = portalUnit?.ZnodeCurrency?.CurrencyCode;
            accountQuoteModel.CultureCode = portalUnit?.ZnodeCulture?.CultureCode;
        }

        //Get the locale id from filters.
        private static int GetLocaleId(FilterCollection filters)
        {
            int localeId = 0;
            if (filters?.Count > 0)
            {
                Int32.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeLocaleEnum.LocaleId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);
                filters.RemoveAll(x => x.FilterName.Equals(ZnodeLocaleEnum.LocaleId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            }
            return localeId;
        }
        //Get the omsQuote id from filters.
        private static int GetOmsQuoteId(FilterCollection filters)
        {
            int omsQuoteId = 0;
            if (filters?.Count > 0)
            {
                Int32.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeOmsQuoteEnum.OmsQuoteId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out omsQuoteId);
            }
            return omsQuoteId;
        }

        //Set product addon skus.
        private void SetProductAddonSKUs(TemplateCartItemModel templateCartItemModel, int portalId, FilterCollection filters)
        {
            //Get publish product based on sku.
            PublishProductModel publishProduct = GetPublishedProductBasedOnSKU(templateCartItemModel.SKU, portalId, filters);

            List<string> requiredAddOns = new List<string>();
            List<string> autoAddOns = new List<string>();

            //Get addon skus.
            GetAddonSKUs(publishProduct, requiredAddOns, autoAddOns);

            templateCartItemModel.AddOnProductSKUs = string.Join(",", requiredAddOns);
            templateCartItemModel.AutoAddonSKUs = string.Join(",", autoAddOns);
        }

        //Get publish product based on sku.
        private PublishProductModel GetPublishedProductBasedOnSKU(string sku, int portalId, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { portalId = portalId, sku= sku });

            if (filters?.Count == 0)
            {
                //Get required filters to get publish products.
                filters = GetRequiredFilters(filters, portalId);
            }

            NameValueCollection expand = new NameValueCollection();
            expand.Add(ZnodeConstant.AddOns, ZnodeConstant.AddOns);

            IPublishProductService _publishProduct = GetService<IPublishProductService>();

            return _publishProduct.GetPublishProductBySKU(new ParameterProductModel { SKU = sku }, expand, filters);
        }

        //Get required filters to get publish products.
        private FilterCollection GetRequiredFilters(FilterCollection filters, int portalId)
        {
            IZnodeRepository<ZnodePortalCatalog> _publishProductDetail = new ZnodeRepository<ZnodePortalCatalog>();

            int publishCatalogId = (_publishProductDetail.Table.FirstOrDefault(x => x.PortalId == portalId)?.PublishCatalogId).GetValueOrDefault();
            if (IsNull(filters))
                filters = new FilterCollection();

            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, publishCatalogId.ToString());
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetDefaultLocaleId().ToString());
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            return filters;
        }

        //Get addon skus.
        private void GetAddonSKUs(PublishProductModel publishProduct, List<string> requiredAddOns, List<string> autoAddOns)
        {
            foreach (WebStoreAddOnModel addon in publishProduct?.AddOns)
            {
                if (addon.IsRequired)
                {
                    requiredAddOns.Add(addon?.AddOnValues?.FirstOrDefault(x => x.IsDefault)?.SKU ?? addon?.AddOnValues?.FirstOrDefault(x => x.RetailPrice != null)?.SKU);
                }

                if (addon.IsAutoAddon)
                {
                    autoAddOns.AddRange(addon?.AddOnValues?.Select(x => x.SKU));
                }
            }
        }

        //Insert into ZnodeOmsQuoteApproval table the next level of users.
        private void InsertNextLevelUsers(int quoteId, string quoteOrderStatus, List<ZnodeUserApprover> nextLevelApprovers, int userId)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteId = quoteId, quoteOrderStatus = quoteOrderStatus, nextLevelApproversListCount = nextLevelApprovers?.Count, userId = userId });

            foreach (ZnodeUserApprover approvers in nextLevelApprovers)
            {
                if (IsNotNull(nextLevelApprovers))
                {
                    ZnodeOMSQuoteApproval omsQuoteApproval = _omsQuoteApproval.Insert(new ZnodeOMSQuoteApproval()
                    {
                        OmsQuoteId = quoteId,
                        UserId = userId,
                        OmsOrderStateId = GetOmsOrderStateId(quoteOrderStatus),
                        ApproverLevelId = approvers.ApproverLevelId.GetValueOrDefault(),
                        ApproverUserId = approvers.ApproverUserId,
                        ApproverOrder = approvers.ApproverOrder
                    });
                }
            }
            ZnodeLogging.LogMessage(quoteId > 0 ? Admin_Resources.SuccessAccountQuoteUpdate: Admin_Resources.ErrorAccountQuoteUpdate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Inserts next level approvers into the ZnodeOmsQuoteApproval table and also send a notification email to next level approvers.
        private void NotifyNextLevelApprover(QuoteStatusModel accountQuoteModel, IList<AccountQuoteModel> accountQuoteList, string quoteStatus)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            List<ZnodeUserApprover> nextLevelApprovers;

            //Insert entry into ZnodeOmsQuoteApproval table of the next level users when a level user approves the quote. Also send email notification to those users for approval.
            if (accountQuoteModel.IsUpdated && string.Equals(quoteStatus, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                int userId = accountQuoteList.FirstOrDefault().UserId;
                int approverUserId = GetLoginUserId();
                ZnodeLogging.LogMessage("userId and approverUserId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, approverUserId = approverUserId });

                List<ZnodeUserApprover> approvers = _userApprover.Table.Where(x => x.UserId == userId)?.ToList();
                ZnodeUserApprover currentApprover = approvers?.FirstOrDefault(x => x.ApproverUserId == approverUserId);
                if (IsNotNull(currentApprover))
                {
                    int currentApproverOrder = currentApprover.ApproverOrder;
                    List<ZnodeUserApprover> nextApprovers = approvers?.Where(x => x.ApproverOrder != currentApproverOrder)?.ToList();
                    if (nextApprovers?.Any(x => x.ApproverOrder > currentApproverOrder) ?? false)
                    {
                        int nextApproverOrder = nextApprovers.Where(x => x.ApproverOrder > currentApproverOrder).Min(x => x.ApproverOrder);
                        nextLevelApprovers = nextApprovers.Where(x => x.UserId == userId && x.ApproverOrder == nextApproverOrder)?.ToList();
                        decimal? quoteTotal = accountQuoteList?.FirstOrDefault()?.QuoteOrderTotal;
                        if (nextLevelApprovers?.Count > 0)
                        {
                            //If the approver level has no limit set, or the quote total fits in the budget range then
                            if (nextLevelApprovers.Any(x => (x.FromBudgetAmount < quoteTotal || x.FromBudgetAmount == quoteTotal)))
                            {
                                List<int> nextLevelApproverIds = nextLevelApprovers.Select(x => x.ApproverUserId).ToList();
                                string approverIds = string.Join(",", nextLevelApproverIds);

                                int quoteId = accountQuoteList.FirstOrDefault().OmsQuoteId;
                                int portalId = accountQuoteList.FirstOrDefault().PortalId;
                                int localeId = accountQuoteList.FirstOrDefault().LocaleId;
                                string quoteOrderStatus = accountQuoteList.FirstOrDefault().Status;
                                //Insert entry into ZnodeOmsQuoteApproval table of the next level users when a level user approves the quote.
                                InsertNextLevelUsers(quoteId, quoteOrderStatus, nextLevelApprovers, userId);
                                //Send email notification to those users for approval.
                                SendMailToNextLevelApprovers(portalId, localeId, quoteId, approverIds);
                            }
                        }
                    }
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Check whether the current level of approver has already approved or rejected the quote.
        private bool CheckLevelApprovedOrRejected(FilterCollection filters)
        {
            //Check if the current level has already approved the quote.
            int quoteId = Convert.ToInt32(filters.Find(x => x.FilterName == ZnodeOmsQuoteEnum.OmsQuoteId.ToString().ToLower())?.Item3);
            int currentApproverId = GetLoginUserId();
            int? currentApproverOrder = Convert.ToInt32(_omsQuoteApproval.Table.FirstOrDefault(x => x.OmsQuoteId == quoteId && x.ApproverUserId == currentApproverId)?.ApproverOrder);
            ZnodeLogging.LogMessage("quoteId, currentApproverId and currentApproverOrder:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteId = quoteId, currentApproverId = currentApproverId, currentApproverOrder = currentApproverOrder });

            List<ZnodeOMSQuoteApproval> omsQuoteApproval = _omsQuoteApproval.Table.Where(x => x.OmsQuoteId == quoteId && x.ApproverOrder == currentApproverOrder)?.ToList();
            List<int> approvedRejectedOrderStateId = _omsOrderState.Table.Where(x => x.OrderStateName == ZnodeOrderStatusEnum.APPROVED.ToString().ToLower() || x.OrderStateName == ZnodeOrderStatusEnum.REJECTED.ToString().ToLower())?.Select(x => x.OmsOrderStateId)?.ToList();
            return omsQuoteApproval.Where(x => x.OmsOrderStateId != null).Any(x => approvedRejectedOrderStateId.Contains(x.OmsOrderStateId.Value));
        }

        //Get quote details that include its user id and quote total.
        private void GetQuoteDetails(int omsQuoteId, out int? userId, out decimal? quoteTotal)
        {
            ZnodeOmsQuote quoteDetail = _omsQuoteRepository.Table.FirstOrDefault(x => x.OmsQuoteId == omsQuoteId);
            userId = quoteDetail?.UserId;
            quoteTotal = quoteDetail?.QuoteOrderTotal;
            ZnodeLogging.LogMessage("omsQuoteId, userId and quoteTotal:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsQuoteId = omsQuoteId, userId = userId, quoteTotal = quoteTotal });
        }

        //Get approver details.
        private UserApproverListModel GetApprovers(int omsQuoteId, int loggedInUserId, int finalUserId, UserModel userModel, bool userApproverFlag, decimal? quoteTotal)
        {
            int portalId = GetPortalId(finalUserId);
            decimal? userLevelFromBudgetAmount = _userApprover?.Table?.FirstOrDefault(x => x.UserId == finalUserId)?.FromBudgetAmount;
            int? portalApprovalLevelId = _portalApprovalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.PortalApprovalLevelId;
            string approverLevelName = _portalApprovalLevelRepository.Table.FirstOrDefault(x => x.PortalApprovalLevelId == portalApprovalLevelId)?.ApprovalLevelName ?? string.Empty;
            ZnodeLogging.LogMessage("portalId, portalApprovalLevelId and approverLevelName:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { portalId = portalId, portalApprovalLevelId = portalApprovalLevelId, approverLevelName = approverLevelName });

            if (!userApproverFlag && !string.IsNullOrEmpty(approverLevelName) && string.Equals(approverLevelName.ToLower(), Admin_Resources.TextSingleLevel))
                return GetApproversForStore(omsQuoteId, loggedInUserId, finalUserId, userModel);
            else if(!string.IsNullOrEmpty(approverLevelName) && string.Equals(approverLevelName.ToLower(), Admin_Resources.TextMultiLevel) && quoteTotal <= userLevelFromBudgetAmount)
                return GetApproversForStore(omsQuoteId, loggedInUserId, finalUserId, userModel);
            else
            {
                UserApproverListModel userApprovers = GetApproversForUser(omsQuoteId, loggedInUserId, finalUserId, userModel);
                if (userApprovers?.UserApprovers?.Count == 0)
                {
                    return GetApproversForStore(omsQuoteId, loggedInUserId, finalUserId, userModel);
                }
                FilterUserApproversByQuoteTotal(quoteTotal, userApprovers);
                return userApprovers;
            }
        }

        private UserApproverListModel GetApproversForUser(int omsQuoteId, int loggedInUserId, int finalUserId, UserModel userModel)
        {
            return new UserApproverListModel
            {
                UserApprovers = (from userApprover in _userApprover.Table
                                 join user in _userRepository.Table on userApprover.ApproverUserId equals user.UserId
                                 join approverLevel in _approverLevel.Table on userApprover.ApproverLevelId equals approverLevel.ApproverLevelId
                                 where userApprover.UserId == finalUserId
                                 select new UserApproverModel
                                 {
                                     UserApproverId = userApprover.UserApproverId,
                                     UserId = userApprover.UserId ?? 0,
                                     ToBudgetAmount = userApprover.ToBudgetAmount,
                                     FromBudgetAmount = userApprover.FromBudgetAmount,
                                     ApproverOrder = userApprover.ApproverOrder,
                                     ApproverLevelId = userApprover.ApproverLevelId.Value,
                                     ApproverUserId = userApprover.ApproverUserId,
                                     IsNoLimit = userApprover.IsNoLimit,
                                     OmsOrderState = _omsOrderState.Table.Where(x => x.OmsOrderStateId == (_omsQuoteApproval.Table.FirstOrDefault(o => o.OmsQuoteId == omsQuoteId && o.ApproverUserId == userApprover.ApproverUserId && o.UserId == finalUserId).OmsOrderStateId ?? 0)).Select(x => x.OrderStateName).FirstOrDefault(),
                                     OmsQuoteId = omsQuoteId,
                                     ApproverName = user.Email,
                                     ApproverLevelName = approverLevel.LevelName,
                                     StatusModifiedDate = _omsQuoteApproval.Table.FirstOrDefault(o => o.OmsQuoteId == omsQuoteId && o.ApproverUserId == userApprover.ApproverUserId && o.UserId == finalUserId).ModifiedDate,
                                     FullName = user.FirstName + " " + user.LastName
                                 }).Distinct().OrderBy(x => x.ApproverOrder).ToList(),
                AccountPermissionAccessId = userModel?.AccountPermissionAccessId,
                AccountUserPermissionId = userModel?.AccountUserPermissionId,
                AccountId = GetAccountId(loggedInUserId),
                PortalId = GetPortalId(loggedInUserId)
            };

        }

        private UserApproverListModel GetApproversForStore(int omsQuoteId, int loggedInUserId, int finalUserId, UserModel userModel)
        {
            return new UserApproverListModel
            {
                UserApprovers = (from userApprover in _userApprover.Table
                                 join user in _userRepository.Table on userApprover.ApproverUserId equals user.UserId
                                 join approverLevel in _approverLevel.Table on userApprover.ApproverLevelId equals approverLevel.ApproverLevelId
                                 join portalApproval in _portalApprovalRepository.Table on userApprover.PortalApprovalId equals portalApproval.PortalApprovalId
                                 join omsquoteApproval in _omsQuoteApproval.Table on userApprover.ApproverUserId equals omsquoteApproval.ApproverUserId
                                 where userApprover.UserId == null && userApprover.IsActive == true && omsquoteApproval.OmsQuoteId == omsQuoteId
                                 select new UserApproverModel
                                 {
                                     UserApproverId = userApprover.UserApproverId,
                                     UserId = userApprover.UserId ?? 0,
                                     ToBudgetAmount = userApprover.ToBudgetAmount,
                                     FromBudgetAmount = userApprover.FromBudgetAmount,
                                     ApproverOrder = userApprover.ApproverOrder,
                                     ApproverLevelId = userApprover.ApproverLevelId.Value,
                                     ApproverUserId = userApprover.ApproverUserId,
                                     IsNoLimit = userApprover.IsNoLimit,
                                     OmsOrderState = _omsOrderState.Table.FirstOrDefault(x => x.OmsOrderStateId == (_omsQuoteApproval.Table.FirstOrDefault(o => o.OmsQuoteId == omsQuoteId && o.ApproverUserId == userApprover.ApproverUserId && o.UserId == finalUserId).OmsOrderStateId ?? 0)).OrderStateName,
                                     OmsQuoteId = omsQuoteId,
                                     ApproverName = user.Email,
                                     ApproverLevelName = approverLevel.LevelName,
                                     StatusModifiedDate = _omsQuoteApproval.Table.FirstOrDefault(o => o.OmsQuoteId == omsQuoteId && o.ApproverUserId == userApprover.ApproverUserId && o.UserId == finalUserId).ModifiedDate
                                 }).Distinct().OrderBy(x => x.ApproverOrder).ToList(),
                AccountPermissionAccessId = userModel?.AccountPermissionAccessId,
                AccountUserPermissionId = userModel?.AccountUserPermissionId,
                AccountId = GetAccountId(loggedInUserId),
                PortalId = GetPortalId(loggedInUserId)
            };
        }

        //Filter the approvers on the basis of budget amount.
        private static void FilterApproversOnBudgetAmount(decimal? quoteTotal, UserApproverListModel userApprovers)
        {
            if (userApprovers?.UserApprovers?.Count > 0)
            {
                List<int> approverOrders = userApprovers.UserApprovers.Where(item => item.OmsOrderState != null && (item.OmsOrderState.ToLower() == ZnodeOrderStatusEnum.APPROVED.ToString().ToLower() || item.OmsOrderState.ToLower() == ZnodeOrderStatusEnum.REJECTED.ToString().ToLower()))?.Select(x => x.ApproverOrder).ToList();

                if (approverOrders?.Count > 0)
                {
                    approverOrders = approverOrders.Distinct().ToList();
                    foreach (int approverOrder in approverOrders)
                    {
                        userApprovers.UserApprovers.RemoveAll(item => item.OmsOrderState != null && item.OmsOrderState.ToLower() == "pending approval" && item.ApproverOrder == approverOrder);
                    }
                }

                int? maxOrder = userApprovers.UserApprovers.FirstOrDefault(x => x.IsNoLimit == true || (x.FromBudgetAmount < quoteTotal || x.FromBudgetAmount == quoteTotal) && (x.ToBudgetAmount > quoteTotal || x.ToBudgetAmount == quoteTotal))?.ApproverOrder;
                if (IsNull(maxOrder))
                {
                    if (userApprovers.UserApprovers.Any(x => x.IsNoLimit.GetValueOrDefault()))
                    {
                        int? maxApproverOrder = userApprovers.UserApprovers.FirstOrDefault(x => x.IsNoLimit.GetValueOrDefault())?.ApproverOrder;
                        decimal? maxApproveFromBudget = userApprovers.UserApprovers.FirstOrDefault(x => x.IsNoLimit.GetValueOrDefault())?.FromBudgetAmount;

                        if (quoteTotal >= maxApproveFromBudget)
                            maxOrder = maxApproverOrder.GetValueOrDefault();
                    }
                }
                //Bring all the records with and lower than maxOrder.
                if (userApprovers.UserApprovers.Count == 1)
                    userApprovers.UserApprovers = userApprovers.UserApprovers.ToList();
                else
                    userApprovers.UserApprovers = userApprovers.UserApprovers.Where(x => x.ApproverOrder <= maxOrder.GetValueOrDefault())?.ToList();
            }
        }

        //If payment status is true, update quote status without notifying the next level approvers.
        private void UpdateQuoteStatusAndSendMail(QuoteStatusModel accountQuoteModel)
        {
            int quoteId = Convert.ToInt32(accountQuoteModel.OmsQuoteIds);
            ZnodeLogging.LogMessage("quoteId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteId = quoteId });

            ZnodeOmsQuote znodeOmsQuote = _omsQuoteRepository.Table.FirstOrDefault(x => x.OmsQuoteId == quoteId);
            if (IsNotNull(znodeOmsQuote))
            {
                znodeOmsQuote.OmsOrderStateId = accountQuoteModel.OmsOrderStateId;
                accountQuoteModel.IsUpdated = _omsQuoteRepository.Update(znodeOmsQuote);
                if (accountQuoteModel.IsUpdated)
                {
                    UserModel userModel = (from user in _userRepository.Table
                                           join userPortal in _userPortalRepository.Table on user.UserId equals userPortal.UserId
                                           where user.UserId == znodeOmsQuote.UserId
                                           select new UserModel
                                           {
                                               PortalId = userPortal.PortalId
                                               ,
                                               FirstName = user.FirstName,
                                               LastName = user.LastName,
                                               Email = user.Email,
                                           }).FirstOrDefault();
                    QuoteRejectMail(quoteId, $"{userModel.FirstName} {userModel.LastName}", userModel.Email, userModel.PortalId.GetValueOrDefault(), GetDefaultLocaleId());
                }
            }
        }

        //If payment status is false, update quote status by notifying next level approvers.
        private void UpdateQuoteStatusAndNotifyNextLevel(QuoteStatusModel accountQuoteModel)
        {
            int isUpdated;
            IList<AccountQuoteModel> accountQuoteList = UpdateQuoteStatus(accountQuoteModel.OmsOrderStateId, accountQuoteModel.OmsQuoteIds, "Ordered,Draft", out isUpdated);
            accountQuoteModel.IsUpdated = isUpdated == 1;
            InsertApproverComments(accountQuoteModel);
            string quoteStatus = accountQuoteList?.Select(x => x.Status).FirstOrDefault();
            string childQuoteStatus = accountQuoteList?.Select(x => x.ChildOrderStatus).FirstOrDefault();
            if (accountQuoteModel.IsUpdated && (string.Equals(quoteStatus, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(quoteStatus, ZnodeOrderStatusEnum.REJECTED.ToString(), StringComparison.CurrentCultureIgnoreCase) || string.Equals(quoteStatus, ZnodeConstant.IN_REVIEW, StringComparison.CurrentCultureIgnoreCase)))
            {
                SendQuoteStatusEmailToUser(quoteStatus, Convert.ToInt32(accountQuoteList?.Select(x => x.PortalId).FirstOrDefault()), accountQuoteList?.ToList(), accountQuoteModel.LocaleId);
            }

            //Allow the next level approvers to approve the quote.
            NotifyNextLevelApprover(accountQuoteModel, accountQuoteList, childQuoteStatus);

            //Add additional notes for quote.
            if (!string.IsNullOrEmpty(accountQuoteModel.Notes))
                AddQuoteNote(new OrderNotesModel() { OmsQuoteId = Convert.ToInt32(accountQuoteModel.OmsQuoteIds), Notes = accountQuoteModel.Notes });
        }

        //Insert approver comments.
        private void InsertApproverComments(QuoteStatusModel accountQuoteModel)
        {
            ZnodeLogging.LogMessage("OmsQuoteIds of accountQuoteModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteIds = accountQuoteModel?.OmsQuoteIds });

            //Update comments for the approver.
            if (accountQuoteModel.IsUpdated && IsNotNull(accountQuoteModel.Comments))
            {
                int quoteId = Convert.ToInt32(accountQuoteModel.OmsQuoteIds);
                ZnodeOmsQuoteComment quoteComment = _omsQuoteComment.Insert(new ZnodeOmsQuoteComment() { OmsQuoteId = quoteId, Comments = accountQuoteModel.Comments });
                if (quoteComment?.OmsQuoteCommentId > 0)
                {
                    int approverUserId = GetLoginUserId();
                    ZnodeOMSQuoteApproval quoteApproval = _omsQuoteApproval.Table.FirstOrDefault(x => x.OmsQuoteId == quoteId && x.ApproverUserId == approverUserId);
                    if (IsNotNull(quoteApproval))
                    {
                        quoteApproval.OmsQuoteCommentId = quoteComment?.OmsQuoteCommentId;
                        bool quoteApprovalCommentUpdated = _omsQuoteApproval.Update(quoteApproval);
                        ZnodeLogging.LogMessage(quoteApprovalCommentUpdated ? "Comment has been successfully updated against this approver." : "Failed to update comments for the current user.", string.Empty, TraceLevel.Info);
                    }
                }
            }
        }

        //Add additional notes for quote.
        private void AddQuoteNote(OrderNotesModel orderNotesModel)
        {
            if (!string.IsNullOrEmpty(orderNotesModel?.Notes))
            {
                ZnodeOmsNote notes = _omsNotesRepository.Insert(orderNotesModel.ToEntity<ZnodeOmsNote>());
                orderNotesModel.OmsNotesId = notes.OmsNotesId;
                ZnodeLogging.LogMessage("OmsNotesId of orderNotesModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsNotesId = orderNotesModel?.OmsNotesId });
            }
        }

        //To update shipping address
        private bool UpdateAddress(int addressId, AddressModel addressModel)
        {
            ZnodeAddress address = addressModel.ToEntity<ZnodeAddress>();
            address.AddressId = addressId;
            address.ExternalId = string.IsNullOrEmpty(addressModel.ExternalId) ? null : addressModel.ExternalId;
            ZnodeLogging.LogMessage("AddressId and ExternalId of ZnodeAddress:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = address?.AddressId, ExternalId = address?.ExternalId });
            IZnodeRepository<ZnodeAddress> _addressRepository = new ZnodeRepository<ZnodeAddress>();
            return _addressRepository.Update(address);
        }

        //To validate quote address model
        private void ValidateQuoteAddressModel(UpdateQuoteShippingAddressModel updateShippingAddressModel)
        {
            if (IsNull(updateShippingAddressModel))
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ModelCanNotBeNull);

            else if (updateShippingAddressModel.OmsQuoteId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.InvalidQuoteID);

            else if (IsNull(updateShippingAddressModel.ShippingAddressModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorShippingAddressModelNull);

            else if (string.IsNullOrEmpty(updateShippingAddressModel.ShippingAddressModel.DisplayName))
                throw new ZnodeException(ErrorCodes.InvalidData, WebStore_Resources.RequiredDisplayName);

            else if (string.IsNullOrEmpty(updateShippingAddressModel.ShippingAddressModel.FirstName))
                throw new ZnodeException(ErrorCodes.InvalidData, WebStore_Resources.RequiredFirstName);

            else if (string.IsNullOrEmpty(updateShippingAddressModel.ShippingAddressModel.LastName))
                throw new ZnodeException(ErrorCodes.InvalidData, WebStore_Resources.RequiredLastName);

            else if (string.IsNullOrEmpty(updateShippingAddressModel.ShippingAddressModel.Address1))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.RequiredAddress1);

            else if (string.IsNullOrEmpty(updateShippingAddressModel.ShippingAddressModel.CountryCode))
                throw new ZnodeException(ErrorCodes.InvalidData, WebStore_Resources.RequiredCountryCode);

            else if (string.IsNullOrEmpty(updateShippingAddressModel.ShippingAddressModel.StateCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.RequiredState);

            else if (string.IsNullOrEmpty(updateShippingAddressModel.ShippingAddressModel.CityName))
                throw new ZnodeException(ErrorCodes.InvalidData, WebStore_Resources.RequiredCityCode);

            else if (string.IsNullOrEmpty(updateShippingAddressModel.ShippingAddressModel.PostalCode))
                throw new ZnodeException(ErrorCodes.InvalidData, WebStore_Resources.RequiredPostalCode);

            else if (string.IsNullOrEmpty(updateShippingAddressModel.ShippingAddressModel.PhoneNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, WebStore_Resources.RequiredPhoneNumber);

            else if (!IsValidCountryCode(updateShippingAddressModel.ShippingAddressModel.CountryCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.InvalidCountryCode);
        }

        //to check valid country code
        private bool IsValidCountryCode(string countryCode)
        {
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { countryCode = countryCode });
            IZnodeRepository<ZnodeCountry> _countryRepository = new ZnodeRepository<ZnodeCountry>();
            return (_countryRepository.Table.FirstOrDefault(x => x.CountryCode == countryCode.ToLower().Trim())?.CountryId ?? 0) > 0;
        }

        //to set actual country & state name to shipping address model
        private void SetShippingAddressCountryStateName(AddressModel model, string country, string state)
        {
            model.CountryName = country;
            model.StateName = state;
        }

        //Update quote quantity by line item id
        private bool UpdateQuoteQuantityByLineItemId(AccountQuoteLineItemModel accountQuoteLineItemModel)
        {
            ZnodeOmsQuoteLineItem znodeOmsQuoteLineItem = _omsQuoteLineItemRepository.GetById(accountQuoteLineItemModel.OmsQuoteLineItemId);
            if (IsNotNull(znodeOmsQuoteLineItem))
            {
                if (znodeOmsQuoteLineItem.ParentOmsQuoteLineItemId == accountQuoteLineItemModel.ParentOmsQuoteLineItemId)
                {
                    if (znodeOmsQuoteLineItem.OmsQuoteId == accountQuoteLineItemModel.OmsQuoteId)
                    {
                        //When user not updated existing qty
                        if (znodeOmsQuoteLineItem.Quantity.Equals(accountQuoteLineItemModel.Quantity))
                            return true;
                        znodeOmsQuoteLineItem.Quantity = accountQuoteLineItemModel.Quantity;
                        return _omsQuoteLineItemRepository.Update(znodeOmsQuoteLineItem);
                    }
                    else
                    {
                        throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidQuoteID);
                    }
                }
                else
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidParentQuoteLineItemId);
            }
            else
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidQuoteLineItemId);
        }

        //Checks count of quote line item cart greater than 1
        private bool IsValidLineItemCount(int omsQuoteId)
            => (_omsQuoteLineItemRepository.Table.Where(x => x.OmsQuoteId == omsQuoteId && x.ParentOmsQuoteLineItemId.Equals(null)).Select(d => d.ParentOmsQuoteLineItemId).Count()) > 1;

        //Check minimum and maximum qty validation
        protected virtual bool ValidateLineItemQuantity(AccountQuoteModel accountQuote, int OmsQuoteLineItemId, decimal Quantity = 0)
        {
            CartParameterModel cartParameterModel = ToCartParameterModel(accountQuote);
            ZnodeOmsQuoteLineItem znodeOmsQuoteLineItem = _omsQuoteLineItemRepository.GetById(OmsQuoteLineItemId);
            if (IsNotNull(znodeOmsQuoteLineItem))
            {
                //get catalog current version id by catalog id.
                int? catalogVersionId = GetCatalogVersionId(cartParameterModel.PublishedCatalogId, cartParameterModel.LocaleId);
                string parentSku = GetProductSku(znodeOmsQuoteLineItem.OmsQuoteLineItemId);
                return CheckMinMaxQuantity(Quantity, cartParameterModel, znodeOmsQuoteLineItem, catalogVersionId, parentSku);
            }
            else
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidQuoteLineItemId);

        }

        //Check minimum and maximum qty validation.
        protected virtual bool ValidateLineItemQuantities(AccountQuoteModel accountQuote, List<int> omsQuoteLineItemIds, int? parentQuoteLineItemId, List<AccountQuoteLineItemModel> accountQuoteLineItemModel, string parentSku, decimal Quantity = 0)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsQuoteLineItemIdsListCount = omsQuoteLineItemIds?.Count, parentQuoteLineItemId = parentQuoteLineItemId, parentSku = parentSku, Quantity = Quantity });

            CartParameterModel cartParameterModel = ToCartParameterModel(accountQuote);
            if (IsNotNull(omsQuoteLineItemIds))
            {
                //Get catalog current version id by catalog id.
                int? catalogVersionId = GetCatalogVersionId(cartParameterModel.PublishedCatalogId, cartParameterModel.LocaleId);

                //Check min/max quantity value.
                bool isValidMinMaxQuantity = CheckMinMaxQuantities(Quantity, cartParameterModel, catalogVersionId, parentSku, omsQuoteLineItemIds, parentQuoteLineItemId);

                //If validation qualifies then delete line items of those whose quantities are passed as 0.
                if (isValidMinMaxQuantity)
                    DeleteZeroQtyLineItems(accountQuoteLineItemModel);
                return isValidMinMaxQuantity;
            }
            else
                throw new ZnodeException(ErrorCodes.InvalidData, "Invalid Quote Line Item ID ");
        }

        //Get product sku by quote line item id.
        protected virtual string GetProductSku(int? omsQuoteLineItemId)
        => _omsQuoteLineItemRepository.Table.Where(x => x.OmsQuoteLineItemId == omsQuoteLineItemId).Select(x => x.SKU).FirstOrDefault();

        //Get product sku by parent line item id.
        protected virtual string GetProductSkuByQuoteId(int? omsQuoteId, int? parentQuoteLineItemId)
        => _omsQuoteLineItemRepository.Table.Where(x => x.OmsQuoteId == omsQuoteId && x.OmsQuoteLineItemId == parentQuoteLineItemId && x.ParentOmsQuoteLineItemId == null).Select(x => x.SKU).FirstOrDefault();

        protected virtual bool CheckMinMaxQuantity(decimal updatedQuantity, CartParameterModel cartParameterModel, ZnodeOmsQuoteLineItem znodeOmsQuoteLineItem, int? catalogVersionId, string sku)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { updatedQuantity = updatedQuantity, catalogVersionId = catalogVersionId, sku = sku });

            decimal? totalQty, checkQty, minQuantity, maxQuantity;
            GetMinMaxQty(cartParameterModel, catalogVersionId, sku, out minQuantity, out maxQuantity);

            //Get all line item except current line item.
            List<ZnodeOmsQuoteLineItem> znodeOmsQuoteLineItemList = _omsQuoteLineItemRepository.Table.Where(x => x.ParentOmsQuoteLineItemId == znodeOmsQuoteLineItem.ParentOmsQuoteLineItemId.Value && x.OmsQuoteLineItemId != znodeOmsQuoteLineItem.OmsQuoteLineItemId).ToList();
            CalculateQtySumForProduct(updatedQuantity, out totalQty, out checkQty, znodeOmsQuoteLineItemList);
            if ((checkQty >= minQuantity && checkQty <= maxQuantity))
                return true;
            else
                throw new ZnodeException(ErrorCodes.MinMaxQtyError, String.Format(Admin_Resources.QuantityLessThanMinQuantity, minQuantity));
        }

        //Calculate qty sum for a product.
        protected virtual void CalculateQtySumForProduct(decimal updatedQuantity, out decimal? totalQty, out decimal? checkQty, List<ZnodeOmsQuoteLineItem> znodeOmsQuoteLineItemList)
        {
            //Add all qty and check min and max quantity
            totalQty = (znodeOmsQuoteLineItemList?.Select(x => x.Quantity)?.ToList()?.Sum()).GetValueOrDefault();
            checkQty = totalQty + updatedQuantity;
            ZnodeLogging.LogMessage("totalQty and checkQty:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { totalQty = totalQty, checkQty = checkQty });
        }

        //Get the min and max value quantity for a product.
        protected virtual void GetMinMaxQty(CartParameterModel cartParameterModel, int? catalogVersionId, string sku, out decimal? minQuantity, out decimal? maxQuantity)
        {
            PublishedProductEntityModel publishProduct = GetService<IPublishedProductDataService>().GetPublishProductBySKU(sku, cartParameterModel.PublishedCatalogId, cartParameterModel.LocaleId, catalogVersionId)?.ToModel<PublishedProductEntityModel>();
            minQuantity = Convert.ToDecimal(publishProduct.Attributes?.Where(x => x.AttributeCode == ZnodeConstant.MinimumQuantity)?.FirstOrDefault()?.AttributeValues);
            maxQuantity = Convert.ToDecimal(publishProduct.Attributes?.Where(x => x.AttributeCode == ZnodeConstant.MaximumQuantity)?.FirstOrDefault()?.AttributeValues);
            ZnodeLogging.LogMessage("minQuantity and maxQuantity:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { minQuantity = minQuantity, maxQuantity = maxQuantity });
        }

        //Check Min/Max quantity validation for the product.
        protected virtual bool CheckMinMaxQuantities(decimal sumOfUpdatedQuantities, CartParameterModel cartParameterModel, int? catalogVersionId, string sku, List<int> omsQuoteLineItemIds, int? parentQuoteLineItemId)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { sumOfUpdatedQuantities = sumOfUpdatedQuantities, parentQuoteLineItemId = parentQuoteLineItemId, catalogVersionId = catalogVersionId, sku = sku });

            decimal? totalQty, checkQty, minQuantity, maxQuantity;
            GetMinMaxQty(cartParameterModel, catalogVersionId, sku, out minQuantity, out maxQuantity);
            //Get all line item except current line item.
            List<ZnodeOmsQuoteLineItem> znodeOmsQuoteLineItemList = _omsQuoteLineItemRepository.Table.Where(x => x.ParentOmsQuoteLineItemId == parentQuoteLineItemId && !omsQuoteLineItemIds.Contains(x.OmsQuoteLineItemId)).ToList();
            CalculateQtySumForProduct(sumOfUpdatedQuantities, out totalQty, out checkQty, znodeOmsQuoteLineItemList);

            //Check if the current product is the last product in the cart.
            IEnumerable<int?> itemsLeftInCart = _omsQuoteLineItemRepository.Table.Where(x => x.OmsQuoteId == cartParameterModel.OmsQuoteId && x.ParentOmsQuoteLineItemId != null)?.Select(x => x.ParentOmsQuoteLineItemId)?.ToList().Distinct();
            if (itemsLeftInCart.Count() == 1)
                return (checkQty >= minQuantity && checkQty <= maxQuantity) ? true : false;
            else
                return (checkQty >= minQuantity && checkQty <= maxQuantity) || checkQty == 0 ? true : false;
        }

        //Get accountId and login user id from filter to pass as a parameter in SP while getting pending payments and pending orders count.
        private void GetAccountIdLoginUserIdFromFilters(FilterCollection filters, ref int accountId, ref int loginUserId)
        {
            accountId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, ZnodeAccountEnum.AccountId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue);
            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeAccountEnum.AccountId.ToString(), StringComparison.CurrentCultureIgnoreCase));

            if (filters.Any(x => string.Equals(x.FilterName, ZnodeConstant.WebStoreQuotes, StringComparison.CurrentCultureIgnoreCase)))
            {
                //Get userId from filter to pass userId parameter in SP.
                loginUserId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue);
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeUserEnum.UserId.ToString(), StringComparison.CurrentCultureIgnoreCase));
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeConstant.WebStoreQuotes, StringComparison.CurrentCultureIgnoreCase));
            }
            ZnodeLogging.LogMessage("accountId and loginUserId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { accountId, loginUserId });
        }

        //Save tax summary for quotes
        protected virtual void SaveQuoteTaxSummaryDetails(int omsQuotesId, List<TaxSummaryModel> taxSummaryListModel)
        {
            try
            {
                if (HelperUtility.IsNotNull(taxSummaryListModel) && taxSummaryListModel?.Count > 0)
                {
                    List<ZnodeOmsTaxQuoteSummary> omsTaxOrderSummaryList = new List<ZnodeOmsTaxQuoteSummary>();
                    IZnodeRepository<ZnodeOmsTaxQuoteSummary> _omsTaxOrderSummaryRepository = new ZnodeRepository<ZnodeOmsTaxQuoteSummary>();
                    List<ZnodeOmsTaxQuoteSummary> omsTaxOrderSummaries = _omsTaxOrderSummaryRepository.Table.Where(x => x.OmsQuoteId == omsQuotesId).ToList();

                    //if address is getting changed from Admin screen, then country and state may change so it is safe to delete old entries.                    if (omsTaxOrderSummaries?.Count > 0)
                    _omsTaxOrderSummaryRepository.Delete(omsTaxOrderSummaries);

                    foreach (TaxSummaryModel transactionSummary in taxSummaryListModel)
                    {
                        omsTaxOrderSummaryList.Add(new ZnodeOmsTaxQuoteSummary()
                        {
                            Tax = transactionSummary.Tax,
                            Rate = transactionSummary.Rate,
                            TaxName = transactionSummary.TaxName,
                            TaxTypeName = transactionSummary.TaxTypeName,
                            OmsQuoteId = omsQuotesId
                        });
                    }
                    _omsTaxOrderSummaryRepository.Insert(omsTaxOrderSummaryList);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
        }

        // Set Personalize cart items details.
        private void SetPersonalizeCartItemsDetails(TemplateCartItemModel templateCartItemModel, ZnodeOmsTemplateLineItem lineItem)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            List<ZnodeOmsTemplatePersonalizeCartItem> omsTemplatePersonalizeCartItemsList = _omsTemplatePersonalizeCartItem.Table.Where(x => x.OmsTemplateLineItemId == lineItem.OmsTemplateLineItemId).ToList();
            if (IsNotNull(omsTemplatePersonalizeCartItemsList) && omsTemplatePersonalizeCartItemsList.Count > 0) {
                templateCartItemModel.PersonaliseValuesDetail = new List<PersonaliseValueModel>();

                if (omsTemplatePersonalizeCartItemsList.Count() > 0)
                {
                    foreach (var omsTemplatePersonalizeCartItems in omsTemplatePersonalizeCartItemsList)
                    {
                        var personaliseValue = new PersonaliseValueModel()
                        {
                            OmsSavedCartLineItemId = omsTemplatePersonalizeCartItems.OmsTemplatePersonalizeCartItemId,
                            PersonalizeCode = omsTemplatePersonalizeCartItems.PersonalizeCode,
                            PersonalizeValue = omsTemplatePersonalizeCartItems.PersonalizeValue,
                            ThumbnailURL = omsTemplatePersonalizeCartItems.ThumbnailURL,
                            PersonalizeName = omsTemplatePersonalizeCartItems.PersonalizeName
                        };
                        templateCartItemModel.PersonaliseValuesDetail.Add(personaliseValue);
                    }
                }
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            } 
        }

        #endregion

        #region Protected Method
        //Get Shopping cart details required for quote.
        protected virtual void GetCartDetails(AccountQuoteModel accountQuote)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("OmsQuoteId of AccountQuoteModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = accountQuote?.OmsQuoteId });

            //Map parameters of QuoteModel to CartParameterModel.
            CartParameterModel cartParameterModel = ToCartParameterModel(accountQuote);

            //LoadFromDatabase gives required details for Quote line items.
            accountQuote.ShoppingCart = _shoppingCartMap.ToModel(GetService<IZnodeShoppingCart>().LoadFromDatabase(cartParameterModel));

            // Get tax summary for Pending Order/Payment.
            GetTaxSummaryDetail(accountQuote);

            //Check product inventory of the product for all type of product in cartline item.

            //Check cart line item inventory and set insufficient flag true if it is out of stock and does not allow back ordering.
            foreach (ShoppingCartItemModel shoppingCartItem in accountQuote.ShoppingCart.ShoppingCartItems)
            {
                _shoppingCartService.CheckCartlineItemInventory(shoppingCartItem, accountQuote.PortalId, cartParameterModel);
            }

            //Bind required data to shopping cart to get calculated tax and shipping cost.
            BindDataToShoppingCart(accountQuote, cartParameterModel);

            //Bind Group product details in Shopping cart line items from quote line items.
            BindGroupProductDetails(accountQuote);
            //Bind attribute details in Quote Line item
            BindAttributeDetails(accountQuote);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Get tax summary for Pending Order/Payment.
        protected virtual void GetTaxSummaryDetail(AccountQuoteModel accountQuoteModel)
        {
            try
            {
                IZnodeRepository<ZnodeOmsTaxQuoteSummary> _omsTaxOrderSummaryRepository = new ZnodeRepository<ZnodeOmsTaxQuoteSummary>();

                List<ZnodeOmsTaxQuoteSummary> omsTaxOrderSummaries = _omsTaxOrderSummaryRepository.Table.Where(x => x.OmsQuoteId == accountQuoteModel.OmsQuoteId).ToList();

                accountQuoteModel.ShoppingCart.TaxSummaryList = new List<TaxSummaryModel>();
                foreach (ZnodeOmsTaxQuoteSummary omsTaxOrderSummary in omsTaxOrderSummaries)
                {
                    accountQuoteModel.ShoppingCart.TaxSummaryList.Add(new TaxSummaryModel()
                    {
                        OmsOrderTaxSummaryId = omsTaxOrderSummary.OmsTaxQuoteSummaryId,
                        OmsQuoteId = omsTaxOrderSummary.OmsQuoteId,
                        Tax = omsTaxOrderSummary.Tax,
                        Rate = omsTaxOrderSummary.Rate,
                        TaxName = omsTaxOrderSummary.TaxName,
                        TaxTypeName = omsTaxOrderSummary.TaxTypeName,
                    });
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
        }

        //Update quote quantities of the line item.
        protected virtual bool UpdateQuoteQuantityByQuoteId(List<AccountQuoteLineItemModel> accountQuoteLineItemModel, int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsQuoteId = omsQuoteId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeOmsQuoteLineItemEnum.OmsQuoteId.ToString(), ProcedureFilterOperators.Equals, omsQuoteId.ToString()));
            IList<ZnodeOmsQuoteLineItem> znodeOmsQuoteLineItem = _omsQuoteLineItemRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);

            //Check if the quantities passed for update is different or not, else skip the update call.
            List<ZnodeOmsQuoteLineItem> omsQuoteLineItemsToUpdate = znodeOmsQuoteLineItem.Where(x => accountQuoteLineItemModel.Any(y => y.OmsQuoteLineItemId == x.OmsQuoteLineItemId && (y.Quantity != x.Quantity))).ToList();
            if (omsQuoteLineItemsToUpdate?.Count > 0)
            {
                omsQuoteLineItemsToUpdate.ToList().ForEach(x =>
                {
                    x.Quantity = accountQuoteLineItemModel?.FirstOrDefault(y => y.OmsQuoteLineItemId == x.OmsQuoteLineItemId)?.Quantity;
                });

                return _omsQuoteLineItemRepository.BatchUpdate(omsQuoteLineItemsToUpdate);
            }
            else
                return true;
        }

        //Map parameters of QuoteModel to CartParameterModel.
        protected virtual CartParameterModel ToCartParameterModel(AccountQuoteModel accountQuote)
        {

            if (HelperUtility.IsNotNull(accountQuote))
            {
                if (accountQuote?.UserId > 0)
                {

                    GetCatalogId(accountQuote);
                }
                else
                    //if userId is 0 then get catalogId on basis of portal
                    GetPublishCatalogId(accountQuote);
                accountQuote.LocaleId = accountQuote.LocaleId > 0 ? accountQuote.LocaleId : GetDefaultLocaleId();

                return new CartParameterModel
                {
                    LocaleId = accountQuote.LocaleId,
                    UserId = accountQuote.UserId,
                    PortalId = accountQuote.PortalId,
                    ShippingId = accountQuote.ShippingId,
                    ShippingCountryCode = accountQuote.ShippingCountryCode,
                    OmsQuoteId = accountQuote.OmsQuoteId,
                    PublishedCatalogId = accountQuote.PublishCatalogId,
                    ProfileId = GetProfileId()
                };
            }
            else
                return new CartParameterModel();
        }

        //This method will send email to relevant user about quote creation.          
        protected virtual void SendPendingApprovalEmailToUser(string customerName, string email, int portalId, int quoteId, int localeId, string quoteTypeCode, bool isPendingPayment = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { customerName = customerName, portalId = portalId, quoteId = quoteId, localeId = localeId, quoteTypeCode = quoteTypeCode, isPendingPayment = isPendingPayment });

            PortalModel portalModel = GetCustomPortalDetails(portalId);
            string baseUrl = GetDomains(portalId);
            baseUrl = (IsNotNull(portalModel) && portalModel.IsEnableSSL) ? $"https://{baseUrl}" : $"http://{baseUrl}";
            string quoteUrl = $"{baseUrl}/User/QuoteView?omsQuoteId={quoteId}";
            if (isPendingPayment)
                quoteUrl = $"{baseUrl}/User/PendingPaymentQuoteView?omsQuoteId={quoteId}";
            string quoteLink = $"<a href=\"{quoteUrl}\">{quoteId}</a>";

            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.QuoteSentForApproval, portalId, localeId);

            if (IsNotNull(emailTemplateMapperModel))
            {
                string subject = $"{emailTemplateMapperModel?.Subject} - {ZnodeConfigManager.SiteConfig.StoreName}";

                string messageText = emailTemplateMapperModel.Descriptions;
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.CustomerName, customerName, messageText);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.QuoteId, quoteLink.ToString(), messageText);
                messageText = ReplaceTokenWithMessageText("#StoreLogo", GetCustomPortalDetails(portalId)?.StoreLogo, messageText);
                //Send  mail to user.
                SendEmail(customerName, email, subject, messageText, portalId, emailTemplateMapperModel.IsEnableBcc);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //This method will send email to relevant customer service about quote creation.          
        protected virtual void SendPendingPaymentApprovalEmailToUser(string customerName, string email, int portalId, int quoteId, int localeId, string quoteTypeCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            PortalModel portalModel = GetAdminDomainDetails();
            string baseUrl = portalModel?.DomainUrl;
            baseUrl = (IsNotNull(portalModel) && portalModel.IsEnableSSL) ? $"https://{baseUrl}" : $"http://{baseUrl}";
            string quoteUrl = $"{baseUrl}/Account/UpdatePendingPaymentQuote?omsQuoteId={quoteId}";
            string quoteLink = $"<a href=\"{quoteUrl}\">{quoteId}</a>";

            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.QuoteSentForApproval, portalId, localeId);

            if (IsNotNull(emailTemplateMapperModel))
            {
                string subject = $"{emailTemplateMapperModel?.Subject} - {portalModel.StoreName}";

                string messageText = emailTemplateMapperModel.Descriptions;
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.CustomerName, customerName, messageText);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.QuoteId, quoteLink.ToString(), messageText);
                messageText = ReplaceTokenWithMessageText("#StoreLogo#", portalModel.StoreLogo, messageText);
                //Send  mail to user.
                SendEmail(customerName, email, subject, messageText, portalId, emailTemplateMapperModel.IsEnableBcc);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Delete line items whose quantities are passed as zero.
        protected virtual void DeleteZeroQtyLineItems(List<AccountQuoteLineItemModel> accountQuoteLineItemModel)
        {
            //Get the line items whose quantities are set to 0.
            List<AccountQuoteLineItemModel> quoteLineItemsWithZeroQty = accountQuoteLineItemModel?.Where(x => x.Quantity == 0)?.ToList();
            if (quoteLineItemsWithZeroQty?.Count > 0)
            {
                //If the updated quantity for any of the line item is 0, then delete that line item.
                foreach (var item in quoteLineItemsWithZeroQty)
                    DeleteQuoteLineItem(item.OmsQuoteLineItemId, item.ParentOmsQuoteLineItemId.Value, item.OmsQuoteId);
            }
        }

        //Validate QuoteLineItem model values.
        protected virtual void QuoteLineItemModelValidations(List<AccountQuoteLineItemModel> accountQuoteLineItemModel)
        {
            if (IsNull(accountQuoteLineItemModel) || accountQuoteLineItemModel?.Count < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            int omsQuoteId = accountQuoteLineItemModel.Select(x => x.OmsQuoteId).FirstOrDefault();
            int quoteIdCount = accountQuoteLineItemModel.Where(x => x.OmsQuoteId == omsQuoteId).ToList().Count;
            ZnodeLogging.LogMessage("omsQuoteId and quoteIdCount:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsQuoteId = omsQuoteId, quoteIdCount = quoteIdCount });

            if (accountQuoteLineItemModel?.Count != quoteIdCount)
                throw new ZnodeException(ErrorCodes.InvalidData, "Cannot have multiple quotes to update their quantities.");

            if (IsNull(accountQuoteLineItemModel?.FirstOrDefault().OmsQuoteId) || accountQuoteLineItemModel?.FirstOrDefault().OmsQuoteId < 1)
                throw new ZnodeException(ErrorCodes.NotFound, "Quote ID is required.");
        }

        //Set quote line item status model.
        protected virtual void SetQuoteLineItemStatusModel(QuoteLineItemStatusListModel listResponse, int? parentQuoteLineItemId, QuoteLineItemStatusModel itemStatus, string message)
        {
            itemStatus.Message = message;
            itemStatus.Status = false;
            itemStatus.ParentQuoteLineItemId = parentQuoteLineItemId;
            listResponse.QuoteLineItemStatusList.Add(itemStatus);
        }

        //Check whether the oms quote line items belong to that parent line item.
        protected virtual bool IsQuoteLineItemValid(int? parentOmsQuoteLineItemId, List<int> omsQuoteLineItemIds)
        {
            bool isLineItemValid = false;
            int checkLineItemsCount = _omsQuoteLineItemRepository.Table.Where(x => x.ParentOmsQuoteLineItemId == parentOmsQuoteLineItemId && omsQuoteLineItemIds.Contains(x.OmsQuoteLineItemId)).ToList().Count;
            ZnodeLogging.LogMessage("checkLineItemsCount:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { checkLineItemsCount = checkLineItemsCount });

            if (Equals(checkLineItemsCount, omsQuoteLineItemIds?.Count))
                isLineItemValid = true;

            return isLineItemValid;
        }

        //Filtering the user approval list and showing only the required approvals within the quote total
        protected virtual void FilterUserApproversByQuoteTotal(decimal? quoteTotal, UserApproverListModel userApprovers)
        {
            List<UserApproverModel> userApproversList = new List<UserApproverModel>();
            int budgetAmountIndex = userApprovers.UserApprovers.FindIndex(x => x.ToBudgetAmount >= quoteTotal);
            int index = 0;
            if (userApprovers?.UserApprovers?.Count > 0 && budgetAmountIndex != -1)
            {
                foreach (UserApproverModel item in userApprovers.UserApprovers)
                {
                    if (index == budgetAmountIndex)
                    {
                        userApproversList.Add(item);
                        break;
                    }
                    else
                    {
                        userApproversList.Add(item);
                        index++;
                    }
                }
                userApprovers.UserApprovers = userApproversList;
            }
        }

        //Get Approver Details And Send Email
        protected virtual void SendEmailToApprover(int quoteId, string approverUserId, ShoppingCartModel shoppingCart)
        {
            //Get Approver details to send mail.
            GetApproverDetails(shoppingCart, quoteId, approverUserId);
            string customername = $"{shoppingCart?.UserDetails?.FirstName} {shoppingCart?.UserDetails?.LastName}";
            if (string.IsNullOrEmpty(customername.Trim()))
                customername = shoppingCart?.UserDetails?.UserName;

            //This method will send email to relevant user about quote creation.
            if (!string.Equals(shoppingCart.OrderStatus, ZnodeOrderStatusEnum.DRAFT.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                SendPendingApprovalEmailToUser(customername, shoppingCart?.UserDetails?.Email, Convert.ToInt32(shoppingCart?.UserDetails?.PortalId), quoteId, shoppingCart.LocaleId, shoppingCart.QuoteTypeCode, shoppingCart.IsPendingPayment);
                var onPendingOrderStatusInit = new ZnodeEventNotifier<ShoppingCartModel>(shoppingCart, EventConstant.OnPendingOrderStatusNotification);
            }
            if (shoppingCart.IsPendingPayment && !string.IsNullOrEmpty(shoppingCart.CustomerServiceEmail))
            {
                SendPendingPaymentApprovalEmailToUser(customername, shoppingCart?.CustomerServiceEmail, Convert.ToInt32(shoppingCart?.UserDetails?.PortalId), quoteId, shoppingCart.LocaleId, shoppingCart.QuoteTypeCode);
                var onPendingOrderStatusInit = new ZnodeEventNotifier<ShoppingCartModel>(shoppingCart, EventConstant.OnPendingOrderStatusNotification);
            }
        }

        // Save Quote Line Items In Database
        protected virtual int SaveQuoteLineItemInDB(int quoteId, ShoppingCartModel shoppingCart, int savedCartId)
        {
            int status = 0;

            if (IsNotNull(shoppingCart) && quoteId > 0)
            {
                //SP call to save/update savedCartLineItem
                IZnodeViewRepository<AccountQuoteLineItemModel> objStoredProc = new ZnodeViewRepository<AccountQuoteLineItemModel>();
                objStoredProc.SetParameter("OmsQuoteId", quoteId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), shoppingCart.UserId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("OmsSavedCartId", savedCartId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

                List<AccountQuoteLineItemModel> savedCartLineItemList = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateQuoteLineItem @OmsQuoteId,@UserId,@OmsSavedCartId,@Status OUT", 3, out status).ToList();
            }
            return status;
        }

        protected virtual int MergeShoppingCart(ShoppingCartModel shoppingCart, int savedCartId)
        {
            //Check if Shopping cart and cart items are null. If not then add it to SavedCartLineItemModel.
            if (IsNotNull(shoppingCart.ShoppingCartItems))
            {
                IZnodeOrderHelper znodeOrderHelper = GetService<IZnodeOrderHelper>();
                //Merge a specific shopping cart item. 
                znodeOrderHelper.MergedShoppingCart(shoppingCart);

                int cookieId = !string.IsNullOrEmpty(shoppingCart.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(shoppingCart.CookieMappingId)) : 0;
                //Get CookieMappingId
                int cookieMappingId = cookieId == 0 ? znodeOrderHelper.GetCookieMappingId(shoppingCart.UserId, shoppingCart.PortalId) : cookieId;
                //Get SavedCartId
                savedCartId = znodeOrderHelper.GetSavedCartId(ref cookieMappingId);

                //If the new cookie Mapping Id gets generated, then it should assign back within the requested model.
                shoppingCart.CookieMappingId = new ZnodeEncryption().EncryptData(cookieMappingId.ToString());
            }
            ZnodeLogging.LogMessage("SavedCartId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, savedCartId);
            return savedCartId;
        }

        //Set quote tax to ZnodeOmsQuoteTaxOrderDetail table.
        protected virtual void SaveQuoteTax(int quoteId, List<ShoppingCartItemModel> shoppingCartItem, decimal? taxRate)
        {
            if (shoppingCartItem?.Count() > 0 && quoteId > 0)
            {
                //Map quote tax details.
                ZnodeLogging.LogMessage("Input parameter to save tax order ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new Object[] { quoteId, shoppingCartItem });
                QuoteTaxDetailsModel taxQuoteDetailsModel = MapQuoteTaxDetails(quoteId, shoppingCartItem);
                ZnodeLogging.LogMessage("Output From SaveQuoteTax method", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, taxQuoteDetailsModel);
                //Insert tax details to ZnodeOmsQuoteTaxOrderDetail table.
                _omsQuoteTaxDetailRepository.Insert(taxQuoteDetailsModel.ToEntity<ZnodeOmsQuoteTaxOrderDetail>());
                //Insert tax rule details to ZnodeOmsTaxRule table.
                SaveQuoteTaxDetails(quoteId, shoppingCartItem?.FirstOrDefault()?.TaxRuleId, taxRate);
            }
        }

        //Map quote tax details.
        protected virtual QuoteTaxDetailsModel MapQuoteTaxDetails(int quoteId, List<ShoppingCartItemModel> shoppingCartItem)
        {
            if (HelperUtility.IsNotNull(shoppingCartItem) && shoppingCartItem.Count > 0)
            {
                return new QuoteTaxDetailsModel()
                {
                    SalesTax = shoppingCartItem.Sum(x => x.TaxCost),
                    ImportDuty = shoppingCartItem.Sum(x => x.ImportDuty),
                    OmsQuoteId = quoteId
                };
            }
            else
            {
                return new QuoteTaxDetailsModel();
            }
        }

        //Save tax rule for quote to ZnodeOmsQuoteTaxRule table.
        protected virtual bool SaveQuoteTaxDetails(int quoteId, int? taxRuleId, decimal? taxRate)
        {
            ZnodeLogging.LogMessage("Input parameter to save tax rule for an quote ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new Object[] { quoteId, taxRuleId });
            int status = 0;
            if (quoteId > 0 && taxRuleId > 0)
            {
                //Insert tax rule details to ZnodeOmsQuoteTaxRule table.
                IZnodeViewRepository<QuoteTaxDetailsModel> objStoredProc = new ZnodeViewRepository<QuoteTaxDetailsModel>();
                objStoredProc.SetParameter("@OmsQuoteId", quoteId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@TaxRuleId", Convert.ToInt32(taxRuleId), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@TaxRate", Convert.ToDecimal(taxRate), ParameterDirection.Input, DbType.Decimal);
                objStoredProc.SetParameter("@status", null, ParameterDirection.Output, DbType.Int32);
                objStoredProc.ExecuteStoredProcedureList("Znode_InsertQuoteTaxDetails @OmsQuoteId, @TaxRuleId, @TaxRate, @status OUT", 2, out status);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            }
            return status == 1;
        }

        //to save quote discount
        protected virtual bool SaveQuoteDiscount(List<QuoteDiscountModel> quoteDiscount)
        {
            bool status = false;
            if (quoteDiscount?.Count > 0)
            {
                IEnumerable<ZnodeOmsQuoteOrderDiscount> omsQuoteDiscounts = _omsQuoteOrderDiscountRepository.Insert(quoteDiscount.ToEntity<ZnodeOmsQuoteOrderDiscount>());
                status = omsQuoteDiscounts?.Count() > 0;
            }
            return status;
        }

        //to get all line items discount 
        protected virtual List<QuoteDiscountModel> GetQuoteDiscount(ShoppingCartModel model, int quoteId)
        {
            List<QuoteDiscountModel> quoteDiscountModel = new List<QuoteDiscountModel>();
            if (IsNotNull(model) && IsNotNull(model.OrderLevelDiscountDetails) && quoteId > 0)
            {
                foreach (OrderDiscountModel orderDiscountModel in model.OrderLevelDiscountDetails)
                {
                    quoteDiscountModel.Add(ToQuoteModel(orderDiscountModel, quoteId));
                }
            }
            return quoteDiscountModel;
        }

        protected virtual QuoteDiscountModel ToQuoteModel(OrderDiscountModel OrderDiscountModel, int quoteId)
        {
            return new QuoteDiscountModel
            {
                OmsQuoteOrderDiscountId = OrderDiscountModel.OmsOrderDiscountId,
                OmsQuoteId = quoteId,
                OmsQuoteLineItemId = OrderDiscountModel.OmsOrderLineItemId,
                OmsDiscountTypeId = OrderDiscountModel.OmsDiscountTypeId,
                DiscountCode = OrderDiscountModel.DiscountCode,
                DiscountAmount = OrderDiscountModel.DiscountAmount,
                Description = OrderDiscountModel.Description,
                PerQuantityDiscount = OrderDiscountModel.PerQuantityDiscount,
                DiscountMultiplier = OrderDiscountModel.DiscountMultiplier,
                ParentOmsQuoteLineItemId = OrderDiscountModel.ParentOmsOrderLineItemsId,
                DiscountLevelTypeId = OrderDiscountModel.DiscountLevelTypeId,
                PromotionName = OrderDiscountModel.PromotionName,
                PromotionTypeId = OrderDiscountModel.PromotionTypeId,
                DiscountAppliedSequence = OrderDiscountModel.DiscountAppliedSequence,
                PromotionMessage = OrderDiscountModel.PromotionMessage,
            };
        }

        //to get all line items discount 
        protected virtual List<QuoteDiscountModel> GetQuoteDiscountDetails(List<QuoteDiscountModel> quoteDiscounts, ShoppingCartItemModel lineItem, int quoteId, int? quoteLineItemId, int? parentQuoteLineItemId)
        {
            if (IsNotNull(lineItem.Product))
            {
                quoteDiscounts = AddDiscountItem(lineItem.Quantity, quoteDiscounts, lineItem.Product.OrdersDiscount, quoteId, quoteLineItemId, parentQuoteLineItemId);
            }
            return quoteDiscounts;
        }

        protected virtual List<QuoteDiscountModel> AddDiscountItem(decimal quantity, List<QuoteDiscountModel> orderDiscount, List<OrderDiscountModel> orderLineItem, int quoteId, int? lineItemId, int? parentOmsOrderLineItemsId)
        {
            if (IsNotNull(orderLineItem) && orderLineItem.Count > 0)
            {
                foreach (OrderDiscountModel lineItemDiscount in orderLineItem)
                {
                    QuoteDiscountModel productDiscountModel = new QuoteDiscountModel();
                    productDiscountModel = ToQuoteModel(lineItemDiscount, quoteId);
                    productDiscountModel.OmsQuoteLineItemId = lineItemId;
                    productDiscountModel.OmsQuoteId = quoteId;
                    productDiscountModel.DiscountAmount = (quantity * productDiscountModel.DiscountAmount);
                    productDiscountModel.PerQuantityDiscount = productDiscountModel.DiscountAmount.GetValueOrDefault();
                    productDiscountModel.ParentOmsQuoteLineItemId = parentOmsOrderLineItemsId;
                    productDiscountModel.DiscountMultiplier = (quantity * productDiscountModel.DiscountMultiplier);
                    orderDiscount.Add(productDiscountModel);
                }
            }
            return orderDiscount;
        }

        //Get Billing Addresses for quotes.
        protected virtual string GetQuoteBillingAddress(AddressModel quoteBilling)
        {
            if (IsNotNull(quoteBilling))
            {
                string street1 = string.IsNullOrEmpty(quoteBilling.Address2) ? string.Empty : "<br />" + quoteBilling.Address2;
                return $"{quoteBilling.FirstName}{" "}{quoteBilling.LastName}{"<br />"}{quoteBilling.CompanyName}{"<br />"}" +
                      $"{quoteBilling.Address1}{street1}{"<br />"}{quoteBilling.CityName}{", "}" +
                      $"{(string.IsNullOrEmpty(quoteBilling.StateCode) ? quoteBilling.StateName : quoteBilling.StateCode)}" +
                      $"{", "}{quoteBilling.CountryName}{" "}{quoteBilling.PostalCode}{"<br />"}" +
                      $"{Admin_Resources.LabelPhoneNumber}{" : "}{quoteBilling.PhoneNumber}";
            }
            return string.Empty;
        }

        //Get Shipping Addresses for quotes
        protected virtual string GetQuoteShipmentAddress(AddressModel quoteShipment)
        {
            if (IsNotNull(quoteShipment))
            {
                ZnodeLogging.LogMessage("AddressId to get shipping company name", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = quoteShipment.AddressId });

                string street1 = string.IsNullOrEmpty(quoteShipment.Address2) ? string.Empty : "<br />" + quoteShipment.Address2;
                quoteShipment.CompanyName = quoteShipment?.CompanyName;

                return $"{quoteShipment?.FirstName}{" "}{ quoteShipment?.LastName}{"<br />"}" +
                         $"{quoteShipment.CompanyName}{"<br />"}" +
                         $"{quoteShipment.Address1}{street1}{"<br />"}{ quoteShipment.CityName}{", "}" +
                         $"{(string.IsNullOrEmpty(quoteShipment.StateCode) ? quoteShipment.StateName : quoteShipment.StateCode)}{", "}" +
                         $"{quoteShipment.CountryName}{" "}{quoteShipment.PostalCode}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}" +
                         $"{quoteShipment.PhoneNumber}{"<br />"}{ WebStore_Resources.TitleEmail}{ " : "}" +
                         $"{quoteShipment.EmailAddress}{"<br />"}";
            }
            return string.Empty;
        }

        // Get Publish Product Skus
        private static List<string> GetPublishSKUs(FilterCollection filters, ZnodeOmsTemplate orderTemplate)
        {
            List<string> publishSkus = null;
            List<PublishedProductEntityModel> publishProduct = null;
            List<string> skus = orderTemplate?.ZnodeOmsTemplateLineItems?.Select(x => x.SKU.ToLower())?.Distinct().ToList();
            if (skus?.Count > 0)
            {
                int catalogId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, WebStoreEnum.ZnodeCatalogId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue);
                int localId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, ZnodeLocaleEnum.LocaleId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue);
                if (catalogId > 0 && localId > 0)
                    publishProduct = GetService<IPublishedProductDataService>().GetPublishProductBySKUs(skus, catalogId, localId)?.ToModel<PublishedProductEntityModel>().ToList();
                publishSkus = publishProduct?.Select(x => x.SKU.ToLower())?.Distinct().ToList();
            }
            return publishSkus;
        }
        #endregion
    }
}
