using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class QuoteService : BaseService, IQuoteService
    {
        #region Private Variables

        protected readonly IZnodeRepository<ZnodeOmsQuote> _omsQuoteRepository;
        private readonly IZnodeRepository<ZnodeOmsQuoteType> _quoteType;
        private readonly IZnodeRepository<ZnodeShipping> _shippingRepository;
        private readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        private readonly IZnodeRepository<ZnodeShippingType> _shippingTypeRepository;
        private readonly IZnodeRepository<ZnodeAccount> _accountRepository;
        private readonly IZnodeRepository<ZnodeUser> _userRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IZnodeRepository<ZnodeOmsQuoteLineItem> _omsQuoteLineItemRepository;
        private readonly IZnodeRepository<ZnodeOmsNote> _OmsNotes;
        private readonly IZnodeRepository<ZnodeOmsTaxQuoteSummary> _omsTaxQuoteSummary;

        #endregion Private Variables

        #region Constructor

        public QuoteService()
        {
            _omsQuoteRepository = new ZnodeRepository<ZnodeOmsQuote>();
            _quoteType = new ZnodeRepository<ZnodeOmsQuoteType>();
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _shippingTypeRepository = new ZnodeRepository<ZnodeShippingType>();
            _shippingRepository = new ZnodeRepository<ZnodeShipping>();
            _accountRepository = new ZnodeRepository<ZnodeAccount>();
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _omsQuoteLineItemRepository = new ZnodeRepository<ZnodeOmsQuoteLineItem>();
            _OmsNotes = new ZnodeRepository<ZnodeOmsNote>();
            _omsTaxQuoteSummary = new ZnodeRepository<ZnodeOmsTaxQuoteSummary>();
        }

        #endregion Constructor

        #region Public Methods
        //Get quotes list.
        public virtual QuoteListModel GetQuoteList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            int userId = GetDataFromFilters(filters, ZnodeUserEnum.UserId.ToString());

            int omsQuoteTypeId = GetDataFromFilters(filters, ZnodeOmsQuoteEnum.OmsQuoteTypeId.ToString());

            int portalId = GetDataFromFilters(filters, FilterKeys.PortalId.ToString());

            //Add date time value in filter collection against filter column name Order date.
            filters = ServiceHelper.AddDateTimeValueInFilterByName(filters, Constants.FilterKeys.QuoteDate);

            //SetPageFilter if not set.
            SetPageFilter(page);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            List<QuoteModel> list = GetQuoteList(pageListModel, userId, omsQuoteTypeId);
            ZnodeLogging.LogMessage("Order list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, list?.Count);
            QuoteListModel quoteListModel = new QuoteListModel { Quotes = list?.ToList(), PortalName = GetPortalName(portalId) };
            if(list?.Count > 0)
            {
                quoteListModel.BindPageListModel(pageListModel);
            }

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return quoteListModel;
        }

        //Creates Quote
        public virtual QuoteCreateModel Create(QuoteCreateModel quoteCreateModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(quoteCreateModel))
            {
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorShoppingCartModelNull);
            }

            ZnodeLogging.LogMessage("Properties of input parameter quoteCreateModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { PortalId = quoteCreateModel?.PortalId, UserId = quoteCreateModel?.UserId });

            quoteCreateModel.QuoteNumber = !string.IsNullOrEmpty(quoteCreateModel.QuoteNumber) ? quoteCreateModel.QuoteNumber
                                                                                                          : GenerateQuoteNumber(quoteCreateModel.PortalId);
            GetUserDetails(quoteCreateModel);

            //save oms quote detail
            ZnodeOmsQuote quote = SaveQuoteDetail(quoteCreateModel);
            ZnodeLogging.LogMessage("OmsQuoteId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, quote?.OmsQuoteId);

            if (quote?.OmsQuoteId > 0)
            {
                SaveQuoteTaxSummaryDetails(quote.OmsQuoteId, quoteCreateModel.TaxSummaryList);

                //Save quote line items.
                if (SaveQuoteLineItems(quoteCreateModel, quote))
                {
                    QuoteResponseModel quoteResponseModel = GetQuoteReceipt(quote.OmsQuoteId);

                    SendQuoteReceiptEmailToUser(quoteCreateModel.UserId, quoteCreateModel.PortalId,  GetLocaleIdFromHeader(), quoteResponseModel);

                    var onQuoteRequestAcknowledgementInit = new ZnodeEventNotifier<QuoteResponseModel>(quoteResponseModel, EventConstant.OnQuoteRequestAcknowledgementToUser);

                    SendQuoteReceiptEmailToAdmin(quoteCreateModel.PortalId, GetLocaleIdFromHeader(), quoteResponseModel);

                    return new QuoteCreateModel() { OmsQuoteId = quote.OmsQuoteId };
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return new QuoteCreateModel();
        }

        //Get Order Receipt Datails. 
        public virtual QuoteResponseModel GetQuoteReceipt(int quoteId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (quoteId <= 0)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorPlaceOrder);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(Constants.FilterKeys.OmsQuoteId, FilterOperators.Equals, quoteId.ToString()));

            QuoteResponseModel quoteModel = new QuoteResponseModel();
            quoteModel = _omsQuoteRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection())?.WhereClause, new List<string> { ZnodeOmsQuoteEnum.ZnodeOmsQuoteLineItems.ToString() }).ToModel<QuoteResponseModel>();// , ZnodeOmsQuoteEnum.ZnodeOmsQuoteComments.ToString() });

            if (IsNotNull(quoteModel))
            {
                //Set quote line items details
                SetCartItemDetails(quoteModel);

                //Get quote status for quote.
                quoteModel.QuoteStatus = GetQuoteStatus(quoteModel.OmsQuoteStateId);

                quoteModel.ShippingType = GetShippingType(quoteModel.ShippingId);

                //Get shipping and billing address.
                SetShippingBillingAddress(quoteModel);

                //Check UserExpand
                GetUserDetails(quoteModel);

                //Get default currency assigned to current portal.
                if (quoteModel.PortalId > 0)
                {
                    SetPortalDefaultCurrencyCultureCode(quoteModel.PortalId, quoteModel);
                }

                //Get Quote Note Details
                quoteModel.QuoteHistoryList = GetQuoteNotes(quoteModel.OmsQuoteId);

            }
            GetQuoteTaxSummary(quoteModel.OmsQuoteId, quoteModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return quoteModel;
        }

        //Get quote details by quote id.
        public virtual QuoteResponseModel GetQuoteById(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (omsQuoteId <= 0)
                throw new ZnodeException(ErrorCodes.NotFound, "OmsQuoteId does not exist");
            //This method check the access of manage screen for sales rep user
            ValidateAccessForSalesRepUser(ZnodeConstant.Quote, omsQuoteId);
            QuoteResponseModel quoteDetail = GetQuoteDetailByQuoteId(omsQuoteId);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return quoteDetail;
        }

        //Get quote details by quote quote number.
        public virtual QuoteResponseModel GetQuoteByQuoteNumber(string quoteNumber)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(quoteNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.QuoteNumberCanNotBeEmpty);

            int? quoteId = _omsQuoteRepository?.Table?.FirstOrDefault(x => x.QuoteNumber.ToLower().Equals(quoteNumber.ToLower()))?.OmsQuoteId;

            if (IsNull(quoteId) || quoteId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.QuoteNumberIsInValidMessage);

            return GetQuoteReceipt(quoteId.GetValueOrDefault());
        }


        // Convert the quote to order
        public virtual OrderModel ConvertQuoteToOrder(ConvertQuoteToOrderModel convertToOrderModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            ZnodeOmsQuote quoteDetails = GetQuoteDetail(convertToOrderModel);


            quoteDetails.OmsOrderStateId = GetOmsQuoteStateId(ZnodeOrderStatusEnum.APPROVED.ToString());
            ShoppingCartModel shoppingCartModel = GetShoppingCartDetails(convertToOrderModel.OmsQuoteId, quoteDetails);
            shoppingCartModel.CreditCardNumber = convertToOrderModel?.PaymentDetails?.CreditCardNumber;

            SubmitOrderModel submitOrderModel = new SubmitOrderModel();
            IOrderService _orderService  = GetService<IOrderService>();
            shoppingCartModel.OrderNumber = string.IsNullOrEmpty(convertToOrderModel.PaymentDetails.OrderId)
                ? _orderService.GenerateOrderNumber(submitOrderModel, new ParameterModel() { Ids = Convert.ToString(shoppingCartModel.PortalId) })
                : convertToOrderModel.PaymentDetails.OrderId;

            shoppingCartModel.PortalName = GetPortalName(shoppingCartModel.PortalId);
            IPaymentHelper paymentHelper = GetService<IPaymentHelper>();
            //Process For Payment
            GatewayResponseModel gatewayResponseModel = paymentHelper.ProcessPayment(convertToOrderModel, shoppingCartModel);

            if (paymentHelper.IsPaypalExpressPayment(convertToOrderModel?.PaymentDetails?.PaymentType) && string.IsNullOrEmpty(convertToOrderModel.PaymentDetails.PayPalToken))
            {
                return new OrderModel { PayPalExpressResponseText = gatewayResponseModel.ResponseText, PaymentTransactionToken = gatewayResponseModel.TransactionId, OrderNumber = shoppingCartModel.OrderNumber };
            }
            else if (paymentHelper.IsAmazonPayPayment(convertToOrderModel?.PaymentDetails?.PaymentType) && !string.IsNullOrEmpty(convertToOrderModel.PaymentDetails.AmazonPayReturnUrl) && !string.IsNullOrEmpty(convertToOrderModel.PaymentDetails.AmazonPayCancelUrl))// add orderRefId as well
            {
                return new OrderModel { PaymentStatus = gatewayResponseModel.IsSuccess.ToString(), TrackingNumber = gatewayResponseModel.Token, OrderNumber = shoppingCartModel.OrderNumber };
            }

            if (gatewayResponseModel.IsSuccess)
            {
                //Get generated unique order number on basis of current date.
                submitOrderModel.OrderNumber = shoppingCartModel.OrderNumber;
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                SetPaymentResponseDetail(convertToOrderModel, shoppingCartModel, gatewayResponseModel);
                shoppingCartModel.OmsOrderStatusId = GetOmsQuoteStateId(ZnodeOrderStatusEnum.APPROVED.ToString());
                //To reset the shipping value
                SetShippingCostDetails(shoppingCartModel);

                shoppingCartModel.IsQuoteToOrder = true;
                if (IsNotNull(shoppingCartModel?.Payment?.PaymentSetting?.GatewayCode) && (string.Equals(shoppingCartModel.Payment.PaymentSetting.GatewayCode, ZnodeConstant.CyberSource, StringComparison.InvariantCultureIgnoreCase) || string.Equals(shoppingCartModel.Payment.PaymentSetting.GatewayCode, ZnodeConstant.AuthorizeNet, StringComparison.InvariantCultureIgnoreCase)))
                    shoppingCartModel.CreditCardNumber = gatewayResponseModel.CardNumber;
                OrderModel orderModel = _orderService.SaveOrder(shoppingCartModel, submitOrderModel);
                if (!paymentHelper.IsACHPayment(convertToOrderModel.PaymentDetails?.PaymentType))
                {
                    CapturePayment(convertToOrderModel, shoppingCartModel, orderModel);
                }
                SendQuoteConvertToOrderMail(convertToOrderModel.UserId, orderModel.PortalId, quoteDetails.OmsQuoteId, orderModel.OrderNumber, GetLocaleIdFromHeader());
                var onQuoteConvertedToOrderInit = new ZnodeEventNotifier<OrderModel>(orderModel, EventConstant.OnQuoteConvertedToOrder);

                return orderModel;
            }
            else
            {
                ZnodeLogging.LogMessage("Error while processing payment", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return new OrderModel();
            }
        }

        //capture payment
        public virtual void CapturePayment(ConvertQuoteToOrderModel convertToOrderModel, ShoppingCartModel shoppingCartModel, OrderModel orderModel)
        {
            IPaymentHelper paymentHelper = GetService<IPaymentHelper>();
            IOrderService _orderService = GetService<IOrderService>();
            if (!shoppingCartModel.IsGatewayPreAuthorize && paymentHelper.IsCreditCardPayment(convertToOrderModel.PaymentDetails.PaymentType))
                _orderService.UpdateOrderPaymentStatus(orderModel.OmsOrderId, ZnodeConstant.CAPTURED.ToString());

            if (orderModel.OmsOrderId > 0 && paymentHelper.IsAmazonPayPayment(convertToOrderModel?.PaymentDetails?.PaymentType) && !shoppingCartModel.IsGatewayPreAuthorize && !string.IsNullOrEmpty(convertToOrderModel?.PaymentDetails.PaymentToken))
            {
                paymentHelper.CapturePayment(convertToOrderModel?.PaymentDetails.PaymentToken);
                _orderService.UpdateOrderPaymentStatus(orderModel.OmsOrderId, ZnodeConstant.CAPTURED.ToString());
            }

            if (orderModel.OmsOrderId > 0 && paymentHelper.IsPaypalExpressPayment(convertToOrderModel?.PaymentDetails?.PaymentType) && !shoppingCartModel.IsGatewayPreAuthorize && !string.IsNullOrEmpty(convertToOrderModel?.PaymentDetails.PayPalToken))
            {
                paymentHelper.CapturePayment(shoppingCartModel?.Token);
                _orderService.UpdateOrderPaymentStatus(orderModel.OmsOrderId, ZnodeConstant.CAPTURED.ToString());
            }
        }

        //to get quote line items by omsQuoteId
        public virtual List<QuoteLineItemModel> GetQuoteLineItems(int omsQuoteId)
        {
            if (IsNull(omsQuoteId) || omsQuoteId == 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorQuoteIdNullOrZero);

            IZnodeRepository<ZnodeOmsQuoteLineItem> _quoteLineItemRepository = new ZnodeRepository<ZnodeOmsQuoteLineItem>();
            FilterDataCollection filter = new FilterDataCollection();
            filter.Add(new FilterDataTuple(ZnodeOmsQuoteLineItemEnum.OmsQuoteId.ToString(), FilterOperators.Equals, omsQuoteId.ToString()));
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter)?.WhereClause;
            return _quoteLineItemRepository.GetEntityList(whereClause)?.ToModel<QuoteLineItemModel>().ToList();
        }

        //Update existing Quote.
        public virtual BooleanModel UpdateQuote(UpdateQuoteModel model)
        {
            try
            {
                BooleanModel isQuoteUpdated = null;

                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                if (IsNull(model))
                    throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorQuoteModelNull);

                if (IsNull(model.OmsQuoteId) || model?.OmsQuoteId == 0)
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorQuoteIdNullOrZero);

                if (IsAllowedTerritories(model?.QuoteLineItems))
                    throw new ZnodeException(ErrorCodes.AllowedTerritories, Admin_Resources.AllowedTerritoriesError);

                //if there is no change in Quote data then no need to update quote
                if (!IsQuoteDataUpdated(model))
                    return new BooleanModel { IsSuccess = true };

                ZnodeLogging.LogMessage(string.Format("Update Quote process is initiated for the Quote Id: {0}", model.OmsQuoteId), ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

                int notesId = 0;
                if (!string.IsNullOrEmpty(model.AdditionalInstructions))
                {
                    OrderNotesModel notesModel = new OrderNotesModel() { Notes = model.AdditionalInstructions, OmsQuoteId = model.OmsQuoteId, CreatedBy = GetLoginUserId(), ModifiedBy = GetLoginUserId() };
                    AddQuoteNote(notesModel);
                    notesId = notesModel.OmsNotesId;
                }

                if (!string.IsNullOrEmpty(model.QuoteHistory))
                    CreateQuoteHistory(new OrderHistoryModel { OMSQuoteId = model.OmsQuoteId, Message = model.QuoteHistory, OmsNotesId = notesId, OrderAmount = BindQuoteAmount(model), CreatedBy = GetLoginUserId(), ModifiedBy = GetLoginUserId() });

                //Update Quote Changes in database
                isQuoteUpdated = UpdateQuoteDetails(model);
                if (model.QuoteLineItems?.Count > 0)
                {
                    isQuoteUpdated.IsSuccess = UpdateQuoteLineItem(model);
                }
                return isQuoteUpdated;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return new BooleanModel { IsSuccess = false , ErrorMessage = ex.ToString() };
            }
        }


        //To get quote total by quote Number
        public virtual string GetQuoteTotal(string quoteNumber)
        {
            if (string.IsNullOrEmpty(quoteNumber))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.QuoteNumberCanNotBeEmpty);

            decimal quoteTotal = _omsQuoteRepository.Table.Where(x => x.QuoteNumber.Equals(quoteNumber, StringComparison.InvariantCultureIgnoreCase)).Select(s => s.QuoteOrderTotal).FirstOrDefault().GetValueOrDefault();

            if (quoteTotal <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidQuoteID);

            return Convert.ToString(quoteTotal);
        }


        #endregion

        #region Protected Methods
        //To generate unique order number on basis of current date.
        public virtual string GenerateQuoteNumber(int portalId)
        {
            string portalName = GetPortalName(portalId);
            string orderNumber = string.Empty;

            if (!string.IsNullOrEmpty(portalName))
            {
                orderNumber = portalName.Trim().Length > 2 ? portalName.Substring(0, 2) : portalName.Substring(0, 1);
            }

            DateTime date = DateTime.Now;
            // we have removed '-fff' from the date string as order number field length not exceeds the limit.
            // This change in made for the ticket ZPD-13806
            String strDate = date.ToString("yyMMdd-HHmmss");
            string randomSuffix = GetRandomCharacters();
            orderNumber += $"-{strDate}-{randomSuffix}";

            return orderNumber.ToUpper();
        }

        #endregion

        #region protected Methods


        //Saves Quote Details
        protected virtual ZnodeOmsQuote SaveQuoteDetail(QuoteCreateModel quoteCreateModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (quoteCreateModel.FreeShipping)
                quoteCreateModel.ShippingId = _shippingRepository.Table.FirstOrDefault(x => x.ShippingCode == "FreeShipping").ShippingId;

            var quoteEntity = quoteCreateModel.ToEntity<ZnodeOmsQuote>();
            quoteEntity = ToQuoteEntity(quoteEntity, quoteCreateModel);
            ZnodeOmsQuote quote = _omsQuoteRepository.Insert(quoteEntity);

            ZnodeLogging.LogMessage(quote.OmsQuoteId > 0 ? Admin_Resources.SuccessQuoteCreated : Api_Resources.ErrorQuoteCreate, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return quote;
        }

        //Set ZnodeOmsQuote properties from QuoteCreateModel
        protected virtual ZnodeOmsQuote ToQuoteEntity(ZnodeOmsQuote quoteEntity, QuoteCreateModel quoteCreateModel)
        {
            quoteEntity.QuoteOrderTotal = quoteCreateModel.QuoteTotal;
            quoteEntity.OmsOrderStateId = GetOmsQuoteStateId(quoteCreateModel.OmsQuoteStatus);
            quoteEntity.OmsQuoteTypeId = GetQuoteTypeIdByCode(quoteCreateModel.QuoteTypeCode);
            quoteEntity.PublishStateId = quoteCreateModel.PublishStateId > 0 ? (byte)quoteCreateModel.PublishStateId : (byte)PublishStateId;
            quoteEntity.AccountId = quoteCreateModel?.AccountId;


            ICurrencyService _currencyService = GetService<ICurrencyService>();
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(FilterKeys.CurrencyId, FilterOperators.Equals, _currencyService.GetCurrencyDetail(quoteEntity.PortalId)?.CurrencyId.ToString()));
            filter.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, ZnodeConstant.TrueValue));
            filter.Add(new FilterTuple(FilterKeys.IsDefault, FilterOperators.Equals, ZnodeConstant.TrueValue));
            quoteEntity.CultureCode = _currencyService.GetCultureCode(filter)?.CultureCode;

            return quoteEntity;
        }

        //Get the quoteTypeId.
        protected int? GetQuoteTypeIdByCode(string quoteTypeCode)
        {
            int? quoteType= _quoteType?.Table?.FirstOrDefault(x => x.QuoteTypeCode.ToLower() == quoteTypeCode.ToLower())?.OmsQuoteTypeId;
            return quoteType > 0 ? quoteType : ZnodeConstant.QuoteTypeId;
        }

        //Get OmsOrderStateId based on order status.
        protected virtual int GetOmsQuoteStateId(string quoteStatus)
        {
            IZnodeRepository<ZnodeOmsOrderState> _orderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
            int quoteStateId = Convert.ToInt32(_orderStateRepository.Table.FirstOrDefault(x => string.Equals(x.OrderStateName, quoteStatus)).OmsOrderStateId);
            return quoteStateId > 0 ? quoteStateId : Convert.ToInt32(_orderStateRepository.Table.FirstOrDefault(x => string.Equals(x.OrderStateName, ZnodeConstant.SUBMITTED))?.OmsOrderStateId);
        }

        //Save quote line items.
        protected virtual bool SaveQuoteLineItems(QuoteCreateModel quoteCreateModel, ZnodeOmsQuote quote)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = quote.OmsQuoteId });

            //Save all quote line item.
            if (SaveAllQuoteCartLineItems(quote.OmsQuoteId, quoteCreateModel))
            {
                //Add Additional Notes for Quotes.
                if (!string.IsNullOrEmpty(quoteCreateModel.AdditionalInstruction))
                    AddAdditionalNotes(quote.OmsQuoteId, quoteCreateModel.AdditionalInstruction);

                // Remove all saved cart items.
                RemoveSavedCartItems(quoteCreateModel.UserId, quoteCreateModel.CookieMappingId, quoteCreateModel.PortalId);

                return true;
            }
            ZnodeLogging.LogMessage("OmsQuoteId property of QuoteCreateModel to be returned:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = quote?.OmsQuoteId });
            return false;
        }

        //Add Additional Notes for Quotes.
        protected virtual void AddAdditionalNotes(int quoteId, string additionalNotes)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteId = quoteId, additionalNotes = additionalNotes });
            if (!string.IsNullOrEmpty(additionalNotes) && quoteId > 0)
            {
                //Add additional notes for quotes.
                _OmsNotes.Insert(new ZnodeOmsNote() { OmsQuoteId = quoteId, Notes = additionalNotes });
            }
        }

        //To save SavedCartlineItem data in database
        protected virtual bool SaveAllQuoteCartLineItems(int quoteId, QuoteCreateModel quoteCreateModel)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteId = quoteId });
            int savedCartId = 0;
            if (quoteId > 0 && !Equals(quoteCreateModel, null))
            {
                //Get Product details.
                DataTable productDetails = GetProductDetails(quoteCreateModel);

                int cookieMappingId = 0;
                //Get SavedCartId
                savedCartId = GetSavedCartId(quoteCreateModel, ref cookieMappingId);

                //If the new cookie Mapping Id gets generated, then it should assign back within the requested model.
                quoteCreateModel.CookieMappingId = new ZnodeEncryption().EncryptData(cookieMappingId.ToString());

                ZnodeLogging.LogMessage("SavedCartId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, savedCartId);

                //SP call to save/update savedCartLineItem
                IZnodeViewRepository<QuoteCreateModel> objStoredProc = new ZnodeViewRepository<QuoteCreateModel>();
                objStoredProc.SetParameter("OmsQuoteId", quoteId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), quoteCreateModel.UserId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("OmsSavedCartId", savedCartId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                objStoredProc.SetTableValueParameter("@SKUPriceForQuote", productDetails, ParameterDirection.Input, SqlDbType.Structured, "dbo.SKUPriceForQuote");
                int status = 0;
                objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateQuoteLineItem @OmsQuoteId,@UserId,@OmsSavedCartId,@Status OUT,@SKUPriceForQuote", 3, out status);

                return status == 1;
            }
            return false;
        }

        // Remove all saved cart items.
        protected virtual void RemoveSavedCartItems(int userId, string cookieMapping, int portalId)
        {
            IShoppingCartService _shoppingCartService = GetService<IShoppingCartService>();
            int cookieMappingId = !string.IsNullOrEmpty(cookieMapping) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(cookieMapping)) : 0;
            ZnodeLogging.LogMessage("userId and cookieMappingId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { userId = userId, cookieMappingId = cookieMappingId });
            _shoppingCartService.RemoveSavedCartItems(userId, cookieMappingId, portalId);
        }

        //Get shipping and billing address.
        protected virtual void SetShippingBillingAddress(QuoteResponseModel quote)
        {
            ZnodeLogging.LogMessage("BillingAddressId and ShippingAddressId of input QuoteResponseModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { BillingAddressId = quote?.BillingAddressId, ShippingAddressId = quote?.ShippingAddressId });

            //Check if Shipping address id is same as Billing address id, assign billing address to shipping address. 
            if (Equals(quote?.ShippingAddressId, quote?.BillingAddressId))
            {
                quote.BillingAddressModel = _addressRepository.Table.Where(x => x.AddressId == quote.BillingAddressId)?.ToModel<AddressModel>()?.FirstOrDefault();
                quote.ShippingAddressModel = quote.BillingAddressModel;
            }
            else
            {
                //If Billing address id greater, get billing address.
                if (quote?.BillingAddressId > 0)
                {
                    quote.BillingAddressModel = _addressRepository.Table.Where(x => x.AddressId == quote.BillingAddressId)?.ToModel<AddressModel>()?.FirstOrDefault();
                }

                //If Shipping address id greater, get shipping address.
                if (quote?.ShippingAddressId > 0)
                {
                    quote.ShippingAddressModel = _addressRepository.Table.Where(x => x.AddressId == quote.ShippingAddressId)?.ToModel<AddressModel>()?.FirstOrDefault();
                }
            }
            quote.BillingAddressHtml = IsNotNull(quote.BillingAddressModel) ? GetOrderBillingAddress(quote.BillingAddressModel) : "";
            quote.ShippingAddressHtml = IsNotNull(quote.ShippingAddressModel) ? GetOrderShipmentAddress(quote.ShippingAddressModel) : "";
        }

        //Get Shopping cart details required for quote.
        protected virtual void SetCartItemDetails(QuoteResponseModel quoteModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("OmsQuoteId of QuoteResponseModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsQuoteId = quoteModel?.OmsQuoteId });

            //Map parameters of quoteModel to CartParameterModel.
            CartParameterModel cartParameterModel = ToCartParameterModel(quoteModel.UserId, quoteModel.PortalId, quoteModel.OmsQuoteId, quoteModel.ShippingId, quoteModel.LocaleId);

            IShoppingCartMap _shoppingCartMap = GetService<IShoppingCartMap>();
            IShoppingCartService _shoppingCartService = GetService<IShoppingCartService>();

            //LoadFromDatabase gives required details for Quote line items.
            quoteModel.ShoppingCartItems = _shoppingCartService.GetShoppingCart(cartParameterModel)?.ShoppingCartItems;

            quoteModel.SubTotal = Convert.ToDecimal(quoteModel?.ShoppingCartItems?.Sum(x => x.ExtendedPrice));

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Set Cart parameter model from QuoteResponseModel
        public virtual CartParameterModel ToCartParameterModel(int userId, int portalId, int omsQuoteId, int shippingId = 0, int localeId = 0, int omsOrderId = 0)
        {
            if (userId > 0 && portalId > 0 )
            {
                IZnodeOrderHelper znodeOrderHelper = GetService<IZnodeOrderHelper>();
                return new CartParameterModel
                {
                    LocaleId = localeId > 0 ? localeId : GetDefaultLocaleId(),
                    UserId = userId,
                    PortalId = portalId,
                    ShippingId = shippingId,
                    OmsQuoteId = omsQuoteId,
                    PublishedCatalogId = GetPublishCatalogId(userId, portalId),
                    ProfileId = GetProfileId(),
                    CookieId = znodeOrderHelper.GetCookieMappingId(userId, portalId),
                    OmsOrderId = omsOrderId
                };
            }
            else
                return new CartParameterModel();
        }

        //Get shipping type by shipping type id.
        protected virtual string GetShippingType(int shippingId)
        {
            string shippingType = string.Empty;
            if (shippingId > 0)
                shippingType = _shippingRepository.Table.FirstOrDefault(w => w.ShippingId == shippingId)?.Description;
            return shippingType;
        }


        //to get shipping address
        protected virtual string GetOrderBillingAddress(AddressModel quoteBilling)
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

        protected virtual string GetOrderShipmentAddress(AddressModel orderShipment)
        {
            if (IsNotNull(orderShipment))
            {
                ZnodeLogging.LogMessage("AddressId to get shipping company name", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { AddressId = orderShipment.AddressId });
                string ShippingcompanyName = _addressRepository.Table.FirstOrDefault(x => x.AddressId == orderShipment.AddressId)?.CompanyName;

                string street1 = string.IsNullOrEmpty(orderShipment.Address2) ? string.Empty : "<br />" + orderShipment.Address2;
                orderShipment.CompanyName = string.IsNullOrEmpty(orderShipment?.CompanyName) ? ShippingcompanyName : orderShipment?.CompanyName;

                return $"{orderShipment?.FirstName}{" "}{ orderShipment?.LastName}{"<br />"}" +
                         $"{(string.IsNullOrEmpty(orderShipment?.CompanyName) ? ShippingcompanyName : orderShipment.CompanyName)}{"<br />"}" +
                         $"{orderShipment.Address1}{street1}{"<br />"}{ orderShipment.CityName}{", "}" +
                         $"{(string.IsNullOrEmpty(orderShipment.StateCode) ? orderShipment.StateName : orderShipment.StateCode)}{", "}" +
                         $"{orderShipment.CountryName}{" "}{orderShipment.PostalCode}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}" +
                         $"{orderShipment.PhoneNumber}{"<br />"}";
            }
            return string.Empty;
        }

        //Get quote list by sp.
        protected virtual List<QuoteModel> GetQuoteList(PageListModel pageListModel, int userId, int? omsQuoteTypeId)
        {
            List<QuoteModel> quoteList;
            try
            {
                ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Input parameters to get order list:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { pageListModel = pageListModel?.ToDebugString() });

                if (omsQuoteTypeId < 0)
                    throw new ZnodeException(ErrorCodes.NotFound, "QuoteTypeId does not exist");

                IZnodeViewRepository<QuoteModel> objStoredProc = new ZnodeViewRepository<QuoteModel>();
                objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
                objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@OmsQuoteTypeId", omsQuoteTypeId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@SalesRepUserId", Libraries.Data.Helpers.HelperMethods.GetSalesRepUserId(), ParameterDirection.Input, DbType.Int32);

                quoteList = objStoredProc.ExecuteStoredProcedureList("Znode_GetQuoteList" + " @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT,@UserId,@OmsQuoteTypeId,@SalesRepUserId", 4, out pageListModel.TotalRowCount).ToList();

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                quoteList = new List<QuoteModel>();
            }
            return quoteList;
        }

        //Get Quote details by QuoteId.
        protected virtual QuoteResponseModel GetQuoteDetailByQuoteId(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { omsQuoteId = omsQuoteId });

            QuoteResponseModel quoteModel = null;

            if(omsQuoteId > 0)
            {
                ZnodeOmsQuote quote = null;

                IZnodeQuoteHelper _quotehelper = GetService<IZnodeQuoteHelper>();
                quote = _quotehelper.GetQuoteById(omsQuoteId);
                quote.CreatedDate = Convert.ToDateTime(quote.CreatedDate.ToTimeZoneDateTimeFormat());
                quote.QuoteExpirationDate  = Convert.ToDateTime(Convert.ToDateTime(quote.QuoteExpirationDate).ToTimeZoneDateTimeFormat());

                if (IsNotNull(quote))
                {
                    //Map quote object to QuoteResponseModel object.
                    quoteModel = quote.ToModel<QuoteResponseModel>();

                    //Get other Quote details.
                    GetQuoteDetails(quoteModel);
                }
            }

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return quoteModel;
        }

        //Get other Quote details.
        protected virtual void GetQuoteDetails(QuoteResponseModel quoteModel)
        {
            if (IsNotNull(quoteModel))
            {
                //Get quote status for quote.
                quoteModel.QuoteStatus = GetQuoteStatus(quoteModel.OmsQuoteStateId);

                //Get shipping and billing address.
                SetShippingBillingAddress(quoteModel);

                //Check UserExpand
                GetUserDetails(quoteModel);

                MapPortalData(quoteModel);

                //Set OrderNumber and OmsOrderId on the basis of Aproved status
                SetOrderDetails(quoteModel);

                //Get Shopping cart details required for quote.
                MapCartItemDataForQuote(quoteModel);

                //Get Shipping Details
                GetShippingdetails(quoteModel);

                quoteModel.SubTotal = GetQuoteSubTotal(quoteModel?.ShoppingCartItems);

                //Get Quote History Details
                MapQuoteHistory(quoteModel);

                GetTaxSummaryDetails(quoteModel);

            }
        }

        protected virtual void GetShippingdetails (QuoteResponseModel quoteModel)
        {
            quoteModel.ShippingDiscountDescription = _shippingRepository.Table.FirstOrDefault(w => w.ShippingId == quoteModel.ShippingId)?.Description;
            quoteModel.ShippingTypeId = _shippingRepository.Table.FirstOrDefault(w => w.ShippingId == quoteModel.ShippingId).ShippingTypeId;
            quoteModel.ShippingTypeClassName = _shippingTypeRepository.Table.FirstOrDefault(w => w.ShippingTypeId == quoteModel.ShippingTypeId)?.ClassName;
        }


        //Get user details by id.
        protected virtual void GetUserDetails(QuoteResponseModel quote)
        {
            ZnodeLogging.LogMessage("UserId to get user details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = quote?.UserId });

            if (IsNotNull(quote))
            {
                UserModel userDetails = GetUserNameByUserId(quote.CreatedBy);
                if (IsNotNull(userDetails))
                {
                    quote.UserName = userDetails.UserName;
                    quote.CreatedByName = userDetails.FirstName + " " + userDetails.LastName;
                }

                userDetails = GetUserDetailsByUserId(quote.UserId);

                if (IsNotNull(userDetails))
                {
                    quote.FirstName = userDetails.FirstName;
                    quote.LastName = userDetails.LastName;
                    quote.PhoneNumber = userDetails.PhoneNumber;
                    quote.Email = userDetails.Email;
                    quote.CustomerGUID = userDetails.CustomerPaymentGUID;
                }
            }
        }

        //Map Portal related data.
        protected virtual void MapPortalData(QuoteResponseModel quote)
        {
            if (IsNotNull(quote))
            {
                IZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();
                quote.StoreName = _portalRepository.Table?.FirstOrDefault(x => x.PortalId == quote.PortalId)?.StoreName;
                quote.LocaleId = GetLocaleIdFromHeader();
                quote.PublishCatalogId = GetCatalogId(quote.PortalId);
            }
        }

        //Map ShoppingCart related data.
        protected virtual void MapCartItemDataForQuote(QuoteResponseModel quoteModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNotNull(quoteModel))
            {
                IShoppingCartService _shoppingCartService = GetService<IShoppingCartService>();

                //Get shopping cart model by using omsQuoteId.
                quoteModel.ShoppingCartItems = _shoppingCartService.GetShoppingCart(new CartParameterModel
                {
                    LocaleId = GetLocaleIdFromHeader(),
                    PortalId = quoteModel.PortalId,
                    UserId = quoteModel.UserId,
                    PublishedCatalogId = quoteModel.PublishCatalogId > 0 ? quoteModel.PublishCatalogId : 0,
                    OmsQuoteId = quoteModel.OmsQuoteId
                }).ShoppingCartItems;

                ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            }
        }

        //Map Quote history from histroy as well as notes.
        protected virtual void MapQuoteHistory(QuoteResponseModel quote)
        {
            try
            {
                if(IsNotNull(quote) && quote.OmsQuoteId > 0)
                {
                    IZnodeViewRepository<OrderHistoryModel> objStoredProc = new ZnodeViewRepository<OrderHistoryModel>();
                    objStoredProc.SetParameter("@QuoteId", quote?.OmsQuoteId, ParameterDirection.Input, DbType.Int32);
                    List<OrderHistoryModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetQuoteHistory @QuoteId").ToList();
                    ZnodeLogging.LogMessage("Quote history list count and OmsQuoteId to get Quote history: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { QuoteHistoryListCount = list?.Count, OmsQuoteId = quote?.OmsQuoteId });
                    quote.QuoteHistoryList = list;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                quote.QuoteHistoryList = new List<OrderHistoryModel>();
            }
        }

        //Add Quote Notes
        protected virtual void AddQuoteNote(OrderNotesModel quoteNotesModel)
        {
            if (!string.IsNullOrEmpty(quoteNotesModel?.Notes))
            {
                IZnodeRepository<ZnodeOmsNote> _omsNoteRepository = new ZnodeRepository<ZnodeOmsNote>();
                ZnodeOmsNote notes = _omsNoteRepository.Insert(quoteNotesModel.ToEntity<ZnodeOmsNote>());
                ZnodeLogging.LogMessage("QuoteNotesModel inserted having OmsNotesId: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { OmsNotesId = quoteNotesModel?.OmsNotesId });
                quoteNotesModel.OmsNotesId = notes.OmsNotesId;
            }
        }

        // Check for allowed territories.
        protected virtual bool IsAllowedTerritories(List<QuoteLineItemModel> cartItem) => cartItem.Where(w => w.IsAllowedTerritories == false).ToList().Count > 0;

        //to check Quote data is updated
        protected virtual bool IsQuoteDataUpdated(UpdateQuoteModel model)
        {
            bool IsQuoteDataUpdated = true;
            if (IsNotNull(model))
            {
                if (IsNull(model.QuoteHistory))
                    IsQuoteDataUpdated = false;

                if (!string.IsNullOrEmpty(model.AdditionalInstructions))
                    IsQuoteDataUpdated = true;
            }
            return IsQuoteDataUpdated;
        }

        //to save Quote history in database
        protected virtual void CreateQuoteHistory(OrderHistoryModel quoteHistoryModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(quoteHistoryModel))
                throw new ZnodeException(ErrorCodes.NullModel, "Quote history model can not be null.");

            if (quoteHistoryModel.OmsNotesId == 0)
                quoteHistoryModel.OmsNotesId = null;

            if (quoteHistoryModel.OrderAmount == 0)
                quoteHistoryModel.OrderAmount = null;

            IZnodeRepository<ZnodeOmsQuoteHistory> _quoteHistoryRepository = new ZnodeRepository<ZnodeOmsQuoteHistory>();
            _quoteHistoryRepository.Insert(quoteHistoryModel.ToEntity<ZnodeOmsQuoteHistory>());
            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        //Save Quote Details for manage 
        protected virtual BooleanModel UpdateQuoteDetails(UpdateQuoteModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("OmsQuoteId for ZnodeOmsQuote model :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, model?.OmsQuoteId);
                BooleanModel isQuoteDetailsUpdated = new BooleanModel();
                if (IsNotNull(model))
                {
                    ZnodeOmsQuote quote = MapUpdatedQuoteDetails(model);
                    if (IsNotNull(quote))
                    {
                        isQuoteDetailsUpdated.IsSuccess = _omsQuoteRepository.Update(quote);
                        SaveQuoteTaxSummaryDetails(quote.OmsQuoteId, model.TaxSummaryList);
                    }
                }
                return isQuoteDetailsUpdated;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return new BooleanModel { IsSuccess = true, ErrorMessage = Admin_Resources.QuoteProcessingFailedError };
            }
        }

        //Updated Lineitem data
        protected virtual bool UpdateQuoteLineItem(UpdateQuoteModel quote)
        {
            if (IsNotNull(quote))
            {
                List<ZnodeOmsQuoteLineItem> quoteLineItems = _omsQuoteLineItemRepository.Table.Where(x => x.OmsQuoteId == quote.OmsQuoteId && x.ParentOmsQuoteLineItemId != null).ToList();

                foreach (ZnodeOmsQuoteLineItem lineItem in quoteLineItems)
                {
                    QuoteLineItemModel item = null;
                    if (lineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles)
                    {
                        item = quote.QuoteLineItems.FirstOrDefault(x => x.OmsQuoteLineItemId == lineItem.ParentOmsQuoteLineItemId);
                    }
                    else
                        item = quote.QuoteLineItems.FirstOrDefault(x => x.OmsQuoteLineItemId == lineItem.OmsQuoteLineItemId);

                    if (IsNotNull(item))
                    {
                        ZnodeOmsQuoteLineItem znodeOmsQuoteLineItem = quoteLineItems.FirstOrDefault(x => x.OmsQuoteLineItemId == lineItem.OmsQuoteLineItemId);
                        znodeOmsQuoteLineItem.Price = item.Price;
                        znodeOmsQuoteLineItem.Quantity = item.Quantity;
                        znodeOmsQuoteLineItem.ShippingCost = item.ShippingCost;
                        znodeOmsQuoteLineItem.ModifiedBy = quote.ModifiedBy;
                        znodeOmsQuoteLineItem.ModifiedDate = quote.ModifiedDate;
                        znodeOmsQuoteLineItem.IsPriceEdit = item.IsPriceEdit;

                        if (quote.IsOldQuote)
                            znodeOmsQuoteLineItem.InitialShippingCost = item.ShippingCost;

                        _omsQuoteLineItemRepository.Update(znodeOmsQuoteLineItem);
                    }
                    else if (lineItem.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.AddOns)
                    {
                        DeleteQuoteLineItemFromDataBase(lineItem, quote);
                    }
                }
                return true;
            }
            return false;
        }

        //Delete line Item from Database
        protected virtual bool DeleteQuoteLineItemFromDataBase(ZnodeOmsQuoteLineItem lineItem, UpdateQuoteModel quote)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            bool isDeleteLineItem = false;
            FilterCollection filters = new FilterCollection();

            List<int> quoteLineItem = _omsQuoteLineItemRepository.Table.Where(x => x.ParentOmsQuoteLineItemId == lineItem.OmsQuoteLineItemId || x.OmsQuoteLineItemId == lineItem.OmsQuoteLineItemId).Select(d => d.OmsQuoteLineItemId).ToList();
            ZnodeLogging.LogMessage("quoteLineItem list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteLineItemListCount = quoteLineItem?.Count });

            if (HelperUtility.IsNull(quoteLineItem) && HelperUtility.IsNull(lineItem.OmsQuoteLineItemId) && HelperUtility.IsNull(lineItem.ParentOmsQuoteLineItemId))
                throw new ZnodeException(ErrorCodes.NullModel, Api_Resources.ModelNotNull);

            //SP call for deleting line item.
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("OmsQuoteLineItemId", lineItem.OmsQuoteLineItemId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("ParentOmsQuoteLineItemId", lineItem.ParentOmsQuoteLineItemId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Boolean);
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteOmsQuoteLineItem @OmsQuoteLineItemId, @ParentOmsQuoteLineItemId, @Status OUT", 2, out status);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Api_Resources.SuccessQuoteLineItemDelete, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info, deleteResult?.Count());
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Api_Resources.ErrorQuoteLineItemDelete, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.ProcessingFailed, Api_Resources.ErrorQuoteLineItemDelete);
            }
        }

        //Bind quote amount 
        protected virtual decimal BindQuoteAmount(UpdateQuoteModel model)
        {
            decimal quoteAmount = _omsQuoteRepository.GetById(model.OmsQuoteId).QuoteOrderTotal.GetValueOrDefault() - model.QuoteTotal;

            return quoteAmount;
        }

        //This method will send email to relevant user about quote creation.          
        protected virtual void SendQuoteReceiptEmailToUser(int userId, int portalId, int localeId, QuoteResponseModel quoteResponseModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { portalId = portalId, localeId = localeId });

            if (userId > 0 && portalId > 0 && localeId > 0)
            {
                if (IsNull(quoteResponseModel))
                    return;

                UserModel userDetails = GetUserNameByUserId(userId);

                if (IsNull(userDetails))
                    return;

                string customerName = $"{userDetails?.FirstName} {userDetails?.LastName}";
                if (string.IsNullOrEmpty(customerName.Trim()))
                    customerName = userDetails?.UserName;

                PortalModel portalModel = GetCustomPortalDetails(portalId);

                if (IsNull(portalModel))
                    return;

                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.QuoteRequestAcknowledgementToUser, portalId, localeId);

                if (IsNull(emailTemplateMapperModel))
                    return;

                string subject = $"{emailTemplateMapperModel?.Subject} - {ZnodeConfigManager.SiteConfig.StoreName}";
                string messageText = emailTemplateMapperModel?.Descriptions;

                messageText = ReplaceTokenWithMessageText(ZnodeConstant.UserName, customerName, messageText);
                
                string receipt = BindEmailTemplateDetails(quoteResponseModel, portalModel, messageText);

                //Send  mail to user.
                SendEmail(customerName, userDetails?.UserName, subject, receipt, portalId, emailTemplateMapperModel.IsEnableBcc);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }
      
        //This method will send email to relevant user about quote Conversion to Order.          
        protected virtual void SendQuoteConvertToOrderMail(int userId, int portalId, int omsQuoteId, string orderNumber, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { portalId = portalId, omsQuoteId = omsQuoteId, localeId = localeId });

            if (userId > 0 && portalId > 0 && localeId > 0 && omsQuoteId > 0)
            {
                QuoteResponseModel quoteResponseModel = GetQuoteReceipt(omsQuoteId);

                if (IsNull(quoteResponseModel))
                    return;

                UserModel userDetails = GetUserNameByUserId(userId);

                if (IsNull(userDetails))
                    return;

                string customerName = $"{userDetails?.FirstName} {userDetails?.LastName}";

                if (string.IsNullOrEmpty(customerName.Trim()))
                    customerName = userDetails?.UserName;

                PortalModel portalModel = GetCustomPortalDetails(portalId);

                if (IsNull(portalModel))
                    return;

                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.QuoteConvertedToOrder, portalId, localeId);

                if (IsNull(emailTemplateMapperModel))
                    return;

                string subject = $"{emailTemplateMapperModel?.Subject} - {ZnodeConfigManager.SiteConfig.StoreName}";

                string messageText = emailTemplateMapperModel?.Descriptions;

                messageText = ReplaceTokenWithMessageText(ZnodeConstant.UserName, customerName, messageText);
                messageText = ReplaceTokenWithMessageText(ZnodeConstant.OrderNo, orderNumber, messageText);

                string receipt = BindEmailTemplateDetails(quoteResponseModel, portalModel, messageText);

                //Send  mail to user.
                SendEmail(customerName, userDetails?.UserName, subject, receipt, portalId, emailTemplateMapperModel.IsEnableBcc);

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            }
        }

        //Send Email.
        protected virtual void SendEmail(string userName, string email, string subject, string messageText, int portalId, bool isEnableBcc)
        {
            ZnodeEmail.SendEmail(portalId, email, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(isEnableBcc, portalId, string.Empty), subject, messageText, true);
        }

        //Get product details
        protected virtual DataTable GetProductDetails(QuoteCreateModel quoteCreateModel)
        {
            if (IsNotNull(quoteCreateModel) && IsNotNull(quoteCreateModel.productDetails))
            {
                return GetProductPriceDetailsForSP(quoteCreateModel.productDetails);
            }
            return new DataTable();
        }

        // Get Product PriceDetails For SP in table format
        protected virtual DataTable GetProductPriceDetailsForSP(List<ProductDetailModel> cartItem)
        {
            DataTable table = new DataTable("SKUPriceForQuote");
            table.Columns.Add(ZnodeConstant.ProductSKU, typeof(string));
            table.Columns.Add(ZnodeConstant.OmsSavedCartLineItemId, typeof(int));
            table.Columns.Add(ZnodeConstant.Price, typeof(decimal));
            table.Columns.Add(ZnodeConstant.LineItemShippingCost, typeof(decimal));
            table.Columns.Add(ZnodeConstant.InitialPrice, typeof(decimal));
            table.Columns.Add(ZnodeConstant.InitialLineItemShippingCost, typeof(decimal));
            table.Columns.Add(ZnodeConstant.IsPriceEdit, typeof(bool));
            table.Columns.Add(ZnodeConstant.CustomColumn1, typeof(string));
            table.Columns.Add(ZnodeConstant.CustomColumn2, typeof(string));
            table.Columns.Add(ZnodeConstant.CustomColumn3, typeof(string));
            table.Columns.Add(ZnodeConstant.CustomColumn4, typeof(string));
            table.Columns.Add(ZnodeConstant.CustomColumn5, typeof(string));
            foreach (ProductDetailModel item in cartItem)
                table.Rows.Add(item.SKU, item.OmsSavedcartLineItemId, item.Price, item.ShippingCost, item.InitialPrice, item.InitialShippingCost, item.IsPriceEdit, item.Custom1, item.Custom2, item.Custom3, item.Custom4, item.Custom5);
            return table;
        }

        //Map Update Details of Quote
        protected virtual ZnodeOmsQuote MapUpdatedQuoteDetails(UpdateQuoteModel model)
        {
            if (IsNotNull(model))
            {
                ZnodeOmsQuote quote = _omsQuoteRepository.Table.FirstOrDefault(w => w.OmsQuoteId == model.OmsQuoteId);
                if (IsNotNull(quote))
                {
                    quote.OmsOrderStateId = (model.OmsQuoteStateId <= 0) ? quote.OmsOrderStateId : model.OmsQuoteStateId;
                    quote.ShippingId = (model.ShippingId <= 0) ? quote.ShippingId : model.ShippingId;
                    quote.ShippingCost = (IsNull(model.ShippingCost)) ? quote.ShippingCost : model.ShippingCost;
                    quote.ShippingAddressId = (model.ShippingAddressId <= 0) ? quote.ShippingAddressId : model.ShippingAddressId;
                    quote.BillingAddressId = (model.BillingAddressId <= 0) ? quote.BillingAddressId : model.BillingAddressId;
                    quote.QuoteOrderTotal = (model.QuoteTotal <= 0) ? quote.QuoteOrderTotal : model.QuoteTotal;
                    quote.ModifiedBy = GetLoginUserId() > 0 ? GetLoginUserId() : quote.ModifiedBy;
                    quote.ModifiedDate = DateTime.Now;
                    quote.TaxCost = (IsNull(model.TaxCost)) ? quote.TaxCost : model.TaxCost;
                    quote.InHandDate = IsNull(model?.InHandDate) ? quote.InHandDate : model.InHandDate;
                    quote.QuoteExpirationDate = IsNull(model?.QuoteExpirationDate) ? quote.QuoteExpirationDate : model.QuoteExpirationDate;
                    quote.ShippingTypeId = (model.ShippingTypeId <= 0) ? quote.ShippingTypeId : model.ShippingTypeId;
                    quote.AccountNumber = string.IsNullOrEmpty(model.AccountNumber) ? quote.AccountNumber : model.AccountNumber;
                    quote.ShippingMethod = string.IsNullOrEmpty(model.ShippingMethod) ? quote.ShippingMethod : model.ShippingMethod;
                    quote.JobName = string.IsNullOrEmpty(model.JobName) ? quote.JobName : model.JobName;
                    quote.ShippingConstraintCode = string.IsNullOrEmpty(model.ShippingConstraintCode) ? quote.ShippingConstraintCode : model.ShippingConstraintCode;
                    quote.IsTaxExempt = IsNull(model.IsTaxExempt) ? quote.IsTaxExempt : model.IsTaxExempt;
                    quote.ShippingHandlingCharges = (IsNull(model.ShippingHandlingCharges)) ? quote.ShippingHandlingCharges : model.ShippingHandlingCharges;
                    quote.ImportDuty = (IsNull(model.ImportDuty)) ? quote.ImportDuty : model.ImportDuty;

                    if (model.IsOldQuote)
                        quote.IsOldQuote = false;
                }
                return quote;
            }
            return new ZnodeOmsQuote();
        }
        //Get Quote Note Details
        protected virtual List<OrderHistoryModel> GetQuoteNotes(int omsQuoteId)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { quoteId = omsQuoteId });
            try
            {
                if (omsQuoteId > 0)
                {
                    IZnodeViewRepository<OrderHistoryModel> objStoredProc = new ZnodeViewRepository<OrderHistoryModel>();
                    objStoredProc.SetParameter("@QuoteId",omsQuoteId, ParameterDirection.Input, DbType.Int32);
                    List<OrderHistoryModel> noteList = objStoredProc.ExecuteStoredProcedureList("Znode_GetOmsQuoteNotesList @QuoteId").ToList();
                    ZnodeLogging.LogMessage("Quote Notes list count and OmsQuoteId to get Quote history: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { QuoteHistoryListCount = noteList?.Count, OmsQuoteId = omsQuoteId });
                    return noteList;
                }
                return new List<OrderHistoryModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return new List<OrderHistoryModel>();
            }
        }

        //Get quote details for converting quote to an order
        protected virtual ZnodeOmsQuote GetQuoteDetail(ConvertQuoteToOrderModel convertToOrderModel)
        {
            if (IsNull(convertToOrderModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ModelNotNull);

            if (convertToOrderModel.OmsQuoteId <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorQuoteIdGreaterThanZero);

            ZnodeLogging.LogMessage("Input parameter OmsQuoteId for getting quote details :", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { convertToOrderModel?.OmsQuoteId });
            ZnodeOmsQuote quoteDetails = _omsQuoteRepository.GetById(convertToOrderModel.OmsQuoteId);
            quoteDetails.PaymentSettingId = convertToOrderModel?.PaymentDetails?.PaymentSettingId;
            convertToOrderModel.UserId = convertToOrderModel.UserId > 0 ? convertToOrderModel.UserId : quoteDetails.UserId;

            if (IsNull(quoteDetails))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.DetailsNotFound);

            if (!IsQuoteValidForConvertToOrder(quoteDetails.QuoteExpirationDate, quoteDetails.OmsOrderStateId, Convert.ToBoolean(quoteDetails.IsConvertedToOrder)))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorConvertQuoteToOrder);

            return quoteDetails;
        }

        //Set shopping cart details for converting quote to an order
        protected virtual ShoppingCartModel GetShoppingCartDetails(int quoteId, ZnodeOmsQuote quoteDetails)
        {
            CartParameterModel cartParameterModel = ToCartParameterModel(quoteDetails.UserId, quoteDetails.PortalId, quoteId, quoteDetails.ShippingId.GetValueOrDefault());

            AccountQuoteModel accountQuoteModel = new AccountQuoteModel();
            accountQuoteModel.CultureCode = quoteDetails.CultureCode;
            ICurrencyService _currencyService = GetService<ICurrencyService>();
            accountQuoteModel.CurrencyCode = _currencyService.GetCurrencyDetail(quoteDetails.PortalId)?.CurrencyCode;

            IOrderService _orderService = GetService<IOrderService>();
            ShoppingCartModel shoppingCartModel = _orderService.GetShoppingCartDetails(quoteDetails, accountQuoteModel, cartParameterModel);
            //Set IsCalculatePromotionAndCoupon to not calculate promotion and coupon discount for quote.
            shoppingCartModel.IsCalculatePromotionAndCoupon = false;
            shoppingCartModel.Shipping.AccountNumber = quoteDetails?.AccountNumber;
            shoppingCartModel.Shipping.ShippingMethod = quoteDetails?.ShippingMethod;
            return shoppingCartModel;
        }

        //This method will send email to admin user about new quote request        .          
        protected virtual void SendQuoteReceiptEmailToAdmin(int portalId, int localeId, QuoteResponseModel quoteResponseModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { portalId = portalId, localeId = localeId });

            if (portalId > 0 && localeId > 0)
            {
                PortalModel portalModel = GetCustomPortalDetails(portalId);

                if (IsNull(portalModel))
                    return;

                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.NewQuoteRequestNotificationForAdmin, portalId, localeId);

                if (IsNull(emailTemplateMapperModel))
                    return;

                string subject = $"{emailTemplateMapperModel?.Subject} - {ZnodeConfigManager.SiteConfig.StoreName}";

                string messageText = emailTemplateMapperModel?.Descriptions;

                string receipt = BindEmailTemplateDetails(quoteResponseModel, portalModel, messageText);

                //Send  mail to admin.                        
                ZnodeEmail.SendEmail(portalId, ZnodeConfigManager.SiteConfig.CustomerServiceEmail, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, portalId, string.Empty), subject, receipt, true);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
        }

        protected void SetPaymentResponseDetail(ConvertQuoteToOrderModel convertToOrderModel, ShoppingCartModel shoppingCartModel, GatewayResponseModel gatewayResponseModel)
        {
            if (!string.IsNullOrEmpty(gatewayResponseModel?.TransactionId))
            {
                shoppingCartModel.Token = gatewayResponseModel.TransactionId;
                shoppingCartModel.IsGatewayPreAuthorize = gatewayResponseModel.IsGatewayPreAuthorize;
            }
            if (convertToOrderModel.PaymentDetails.IsFromAmazonPay)
            {
                shoppingCartModel.Token = convertToOrderModel.PaymentDetails.PaymentToken;
                shoppingCartModel.CardType = "Amazon";
                shoppingCartModel.TransactionId = convertToOrderModel.PaymentDetails.PaymentToken;
            }
        }

        //Get Order Number and OrderId from OmsQuoteId.
        protected virtual void SetOrderDetails(QuoteResponseModel quoteModel)
        {
            if (quoteModel?.OmsQuoteId > 0 &&
                string.Equals(quoteModel.QuoteStatus, ZnodeOrderStatusEnum.APPROVED.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                IZnodeRepository<ZnodeOmsOrder> _orderRepository = new ZnodeRepository<ZnodeOmsOrder>();
                OrderModel order = _orderRepository.Table.Where(x => x.OMSQuoteId == quoteModel.OmsQuoteId)
                        .Select(a => new OrderModel { OmsOrderId = a.OmsOrderId, OrderNumber = a.OrderNumber }).FirstOrDefault();
                if (IsNotNull(order))
                {
                    quoteModel.OrderNumber = order.OrderNumber;
                    quoteModel.OmsOrderId = order.OmsOrderId;
                }
            }
        }
        #endregion

        #region Private Methods

        //get subtotal for cart
        private decimal GetQuoteSubTotal(List<ShoppingCartItemModel> ShoppingCartItems)
        {
            return Convert.ToDecimal(ShoppingCartItems?.Sum(x => x.ExtendedPrice));

        }

        //SetPageFilter if not set.
        private void SetPageFilter(NameValueCollection page)
        {
            if (string.IsNullOrEmpty(page.Get(ZnodeConstant.Index)))
            {
                page.Set(ZnodeConstant.Index, ZnodeConstant.DefaultIndexValue);
            }
            if (string.IsNullOrEmpty(page.Get(ZnodeConstant.TextSize)))
            {
                page.Set(ZnodeConstant.TextSize, ZnodeConstant.DefaultSizeValue);
            }
        }

        //Get saved cart id from cookieMappingId, userId and PortalId
        private int GetSavedCartId(QuoteCreateModel quoteCreateModel, ref int cookieMappingId)
        {
            IZnodeOrderHelper znodeOrderHelper = GetService<IZnodeOrderHelper>();

            int cookieId = !string.IsNullOrEmpty(quoteCreateModel.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(quoteCreateModel.CookieMappingId)) : 0;

            //Get CookieMappingId
            cookieMappingId = cookieId == 0 ? znodeOrderHelper.GetCookieMappingId(quoteCreateModel.UserId, quoteCreateModel.PortalId) : cookieId;

            return znodeOrderHelper.GetSavedCartId(ref cookieMappingId);
        }

        //Get Quote Status
        private string GetQuoteStatus(int quoteStateId)
        {
            string quoteStatus = string.Empty;
            if (quoteStateId > 0)
            {
                IZnodeRepository<ZnodeOmsOrderState> _orderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
                quoteStatus = _orderStateRepository.Table.FirstOrDefault(x => x.OmsOrderStateId == quoteStateId)?.Description;
            }
            return quoteStatus;
        }

        //Get default currency assigned to current portal.
        private void SetPortalDefaultCurrencyCultureCode(int portalId, QuoteResponseModel quote)
        {
            IZnodeRepository<ZnodePortalUnit> _portalUnitRepository = new ZnodeRepository<ZnodePortalUnit>();
            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());

            NameValueCollection expand = new NameValueCollection();
            expand.Add(ZnodePortalUnitEnum.ZnodeCurrency.ToString(), ZnodePortalUnitEnum.ZnodeCurrency.ToString());
            expand.Add(ZnodePortalUnitEnum.ZnodeCulture.ToString(), ZnodePortalUnitEnum.ZnodeCulture.ToString());

            ZnodePortalUnit portalUnit = _portalUnitRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause, GetExpands(expand));
            quote.CultureCode = portalUnit?.ZnodeCulture?.CultureCode;
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

        //Get published catalog Id
        private int GetPublishCatalogId(int userId, int portalId)
        {
            ZnodeLogging.LogMessage("UserId of input AccountQuoteModel:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId });
            if (userId > 0)
            {
                //Get accountId on basis of UserId
                int? accountId = _userRepository.Table.FirstOrDefault(x => x.UserId == userId)?.AccountId;
                ZnodeLogging.LogMessage("AccountId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, accountId);
                if (accountId > 0)
                {
                    //Get account details on basis of accountId and get catalogId
                    ZnodeAccount accountDetails = _accountRepository.GetById(accountId.GetValueOrDefault());

                    if (accountDetails?.PublishCatalogId > 0)
                    {
                        return accountDetails.PublishCatalogId.GetValueOrDefault();
                    }
                    //If account not present then looking for parent accountId and get catalogId
                    else if (accountDetails?.ParentAccountId > 0)
                    {
                        ZnodeAccount parentAccountDetails = _accountRepository.GetById(accountDetails.ParentAccountId.GetValueOrDefault());
                        if (parentAccountDetails?.PublishCatalogId > 0)
                        {
                            return parentAccountDetails.PublishCatalogId.GetValueOrDefault();
                        }
                    }
                }
            }
            //if userId is 0 then get catalogId on basis of portal
            return GetCatalogId(portalId);
        }

        //If catalog is not present for account then get catalog id on basis of portalId
        private int GetCatalogId(int portalId)
        {
            int publishCatalogId = 0;
            if (portalId > 0)
            {
                int? portalCatalogId = _portalCatalogRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.PublishCatalogId;
                ZnodeLogging.LogMessage("portalCatalogId:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, portalCatalogId);
                if (portalCatalogId > 0)
                {
                    publishCatalogId = portalCatalogId.GetValueOrDefault();
                }
            }
            return publishCatalogId;
        }

        //Get Portal name on the basis of portal id.
        private string GetPortalName(int portalId)
        {
            if (portalId > 0)
            {
                IZnodeRepository<ZnodePortal> _znodePortal = new ZnodeRepository<ZnodePortal>();
                return _znodePortal.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreName;
            }
            return string.Empty;
        }

               
        //Get data from filters.
        private int GetDataFromFilters(FilterCollection filters, string filterName)
        {
            int filterId = 0;
            if (filters.Exists(x => x.FilterName.Equals(filterName, StringComparison.InvariantCultureIgnoreCase)))
            {
                filterId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, filterName, StringComparison.CurrentCultureIgnoreCase))?.Item3);
            }
            return filterId;
        }

        //Check if the Quote is Valid For Convert To an Order
        private bool IsQuoteValidForConvertToOrder(DateTime? quoteExpirationDate, int quoteStatusId, bool isConvertedToOrder)
        {
            string quoteStatus = GetQuoteStatus(quoteStatusId);
            bool IsQuoteValid = false ;
            if (!isConvertedToOrder)
            {
                if (!(IsNotNull(quoteExpirationDate) && quoteExpirationDate == DateTime.Now))
                {
                    if (!(string.Equals(quoteStatus, ZnodeOrderStatusEnum.EXPIRED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(quoteStatus, ZnodeOrderStatusEnum.SUBMITTED.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(quoteStatus, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        IsQuoteValid = true;
                    }
                }
            }
            return IsQuoteValid;
        }

        //Get user details by id.
        protected virtual UserModel GetUserDetailsByUserId(int userId)
        {
            ZnodeLogging.LogMessage("UserId to get user details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = userId });

            UserModel userDetails = (from user in _userRepository.Table
                                     where user.UserId == userId
                                     select new UserModel
                                     {
                                         FirstName = user.FirstName,
                                         LastName = user.LastName,
                                         Email = user.Email,
                                         PhoneNumber = user.PhoneNumber,
                                         AccountId = user.AccountId,
                                         CustomerPaymentGUID = user.CustomerPaymentGUID
                                     })?.FirstOrDefault();

            return userDetails;
        }

        //Get user details by id.
        protected virtual void GetUserDetails(QuoteCreateModel quote)
        {
            ZnodeLogging.LogMessage("UserId to get user details: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { UserId = quote?.UserId });

            if (IsNotNull(quote))
            {

                UserModel userDetails = GetUserDetailsByUserId(quote.UserId);

                if (IsNotNull(userDetails))
                {
                    quote.FirstName = userDetails.FirstName;
                    quote.LastName = userDetails.LastName;
                    quote.PhoneNumber = userDetails.PhoneNumber;
                    quote.Email = userDetails.Email;
                    quote.AccountId = userDetails.AccountId.GetValueOrDefault();
                }
            }
        }

        //To reset the shipping value
        protected virtual void SetShippingCostDetails(ShoppingCartModel shoppingCartModel)
        {
            if (IsNull(shoppingCartModel))
                return;

            shoppingCartModel.CustomShippingCost = null;
        }

        protected virtual string BindEmailTemplateDetails(QuoteResponseModel quoteResponseModel, PortalModel portalModel, string messageText)
        {
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.StoreLogo, portalModel.StoreLogo, messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.StoreName, portalModel.StoreName, messageText);

            messageText = ReplaceTokenWithMessageText(ZnodeConstant.QuoteId, quoteResponseModel.QuoteNumber, messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.QuoteStatus, quoteResponseModel.QuoteStatus, messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.QuoteDate, Convert.ToString(quoteResponseModel.CreatedDate.ToTimeZoneDateTimeFormat()), messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.ExpirationDate, Convert.ToString(quoteResponseModel.QuoteExpirationDate?.ToTimeZoneDateTimeFormat()), messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.QuoteName, $"{quoteResponseModel.CreatedByName} ({quoteResponseModel.UserName})", messageText);

            messageText = ReplaceTokenWithMessageText(ZnodeConstant.CustomerServiceEmail, portalModel.CustomerServiceEmail, messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.CustomerServicePhoneNumber, portalModel.CustomerServicePhoneNumber, messageText);

            messageText = ReplaceTokenWithMessageText(ZnodeConstant.BillingAddress, System.Web.HttpUtility.HtmlDecode(quoteResponseModel.BillingAddressHtml), messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.ShippingAddress, System.Web.HttpUtility.HtmlDecode(quoteResponseModel.ShippingAddressHtml), messageText);

            messageText = ReplaceTokenWithMessageText(ZnodeConstant.LabelInHandDate, "In Hands Date", messageText);          
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.InHandDate, Convert.ToString(quoteResponseModel.InHandDate?.ToTimeZoneDateTimeFormat()), messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.LabelShippingConstraintsCode, "Shipping Constraints", messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.ShippingConstraintCode, quoteResponseModel.ShippingConstraintCode, messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.LabelShippingType, "Shipping Type", messageText);
            messageText = ReplaceTokenWithMessageText(ZnodeConstant.ShippingName, System.Web.HttpUtility.HtmlDecode(quoteResponseModel.ShippingType), messageText);

            messageText = ReplaceTokenWithMessageText(ZnodeConstant.TotalCost, GetFormatPriceWithCurrency(quoteResponseModel.QuoteTotal, "", quoteResponseModel.CultureCode), messageText);

            DataTable quotelineItemTable = BindQuoteLineItemData(quoteResponseModel.ShoppingCartItems);

            DataTable quoteAmountTable = SetQuoteAmountData(quoteResponseModel);

            string receiptHtml = GenerateHtmlResendReceiptWithParser(quotelineItemTable, quoteAmountTable, messageText);

            string receipt = EmailTemplateHelper.ReplaceTemplateTokens(receiptHtml);

            return receipt;
        }

        //to create order order line item table
        protected virtual DataTable CreateQuoteLineItemTable()
        {
            DataTable quotelineItemTable = new DataTable();
            quotelineItemTable.Columns.Add("ProductImage");
            quotelineItemTable.Columns.Add("Name");
            quotelineItemTable.Columns.Add("SKU");
            quotelineItemTable.Columns.Add("Quantity");
            quotelineItemTable.Columns.Add("Description");
            quotelineItemTable.Columns.Add("UOMDescription");
            quotelineItemTable.Columns.Add("InitialPrice");
            quotelineItemTable.Columns.Add("Price");
            quotelineItemTable.Columns.Add("ExtendedPrice");
            quotelineItemTable.Columns.Add("OmsOrderShipmentID");
            quotelineItemTable.Columns.Add("ShortDescription");
            quotelineItemTable.Columns.Add("ShippingId");
            quotelineItemTable.Columns.Add("OrderLineItemState");
            quotelineItemTable.Columns.Add("TrackingNumber");
            quotelineItemTable.Columns.Add("Custom1");
            quotelineItemTable.Columns.Add("Custom2");
            quotelineItemTable.Columns.Add("Custom3");
            quotelineItemTable.Columns.Add("Custom4");
            quotelineItemTable.Columns.Add("Custom5");
            quotelineItemTable.Columns.Add("GroupId");
            quotelineItemTable.Columns.Add("GroupingRowspan");
            quotelineItemTable.Columns.Add("GroupingDisplay");
            return quotelineItemTable;
        }

        protected virtual DataTable CreateQuoteAmountTable()
        {
            DataTable quoteAmountTable = new DataTable();
            quoteAmountTable.Columns.Add("Title");
            quoteAmountTable.Columns.Add("Amount");
            return quoteAmountTable;
        }

        //to set order amount data
        protected virtual DataTable SetQuoteAmountData(QuoteResponseModel quoteResponseModel)
        {
            // Create order amount table
            DataTable quoteAmountTable = CreateQuoteAmountTable();

            Libraries.ECommerce.ShoppingCart.IZnodeOrderReceipt znodeOrderReceipt = GetService<Libraries.ECommerce.ShoppingCart.IZnodeOrderReceipt>(new ZnodeNamedParameter("cultureCode", quoteResponseModel.CultureCode));

            znodeOrderReceipt.BuildOrderAmountTable(Admin_Resources.LabelSubTotal, quoteResponseModel.SubTotal, quoteAmountTable);
            znodeOrderReceipt.BuildOrderAmountTable(Admin_Resources.LabelShipping, quoteResponseModel.ShippingCost, quoteAmountTable);
            znodeOrderReceipt.BuildOrderAmountTable(Admin_Resources.LabelShippingHandlingCharges, quoteResponseModel.ShippingHandlingCharges, quoteAmountTable);
            znodeOrderReceipt.BuildOrderAmountTable(Admin_Resources.LabelTax, quoteResponseModel.TaxAmount, quoteAmountTable);

            return quoteAmountTable;
        }

        protected virtual DataTable BindQuoteLineItemData(List<ShoppingCartItemModel> ShoppingCartItems)
        {
            DataTable quotelineItemTable = CreateQuoteLineItemTable();

            if (ShoppingCartItems == null)
                return new DataTable();

            foreach (ShoppingCartItemModel shoppingCartItem in ShoppingCartItems)
            {
                if (IsNull(shoppingCartItem))
                    continue;

                DataRow orderlineItemDbRow = quotelineItemTable.NewRow();

                StringBuilder name = new StringBuilder();
                name.Append(shoppingCartItem.ProductName + "<br />");

                if (shoppingCartItem?.GroupProducts?.Count > 0)
                {
                    StringBuilder sku = new StringBuilder();
                    foreach (AssociatedProductModel item in shoppingCartItem?.GroupProducts)
                    {
                        sku.Append(item.Sku + "<br />");
                        name.Append(item.ProductName + "<br />");
                    }
                    orderlineItemDbRow["SKU"] = Convert.ToString(sku);
                }
                else
                {
                    orderlineItemDbRow["SKU"] = shoppingCartItem.SKU;
                }

                if (!string.IsNullOrEmpty(shoppingCartItem.DownloadableProductKey))
                {
                    List<string> keys = shoppingCartItem.DownloadableProductKey.Split(',').ToList();

                    name.Append("Product Keys :" + "<br />");

                    foreach (string key in keys)
                    {
                        name.Append(System.Web.HttpUtility.HtmlDecode(key) + "<br />");
                    }
                }

                if (shoppingCartItem.PersonaliseValuesDetail?.Count > 0)
                {
                    foreach (PersonaliseValueModel item in shoppingCartItem.PersonaliseValuesDetail)
                    {
                        if (!Equals(item.PersonalizeValue, null) && !Equals(item.PersonalizeValue, string.Empty))
                        {
                            name.Append($"{item.PersonalizeName}: {item.PersonalizeValue}" + "<br />");
                        }

                    }
                }

                orderlineItemDbRow["Name"] = Convert.ToString(name);

                if (shoppingCartItem.BundleProducts?.Count > 0 || shoppingCartItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)
                {
                    orderlineItemDbRow["Description"] = System.Web.HttpUtility.HtmlDecode(shoppingCartItem.CartDescription);
                }


                orderlineItemDbRow["UOMDescription"] = string.Empty;
                orderlineItemDbRow["Quantity"] = Convert.ToInt32(shoppingCartItem.Quantity);
                if (shoppingCartItem != null)
                {
                    string cultureCode = shoppingCartItem.CultureCode;
                    orderlineItemDbRow["Price"] = GetFormatPriceWithCurrency(shoppingCartItem.UnitPrice, shoppingCartItem.UOM, cultureCode);
                    orderlineItemDbRow["ExtendedPrice"] = GetFormatPriceWithCurrency(shoppingCartItem.ExtendedPrice, shoppingCartItem.UOM, cultureCode);

                    if (shoppingCartItem.IsPriceEdit)
                        orderlineItemDbRow["InitialPrice"] = GetFormatPriceWithCurrency(shoppingCartItem.InitialPrice, shoppingCartItem.UOM, cultureCode);
                }

                orderlineItemDbRow["GroupId"] = string.IsNullOrEmpty(shoppingCartItem.GroupId) ? Guid.NewGuid().ToString() : shoppingCartItem.GroupId;
                quotelineItemTable.Rows.Add(orderlineItemDbRow);
            }

            return quotelineItemTable;
        }

        //Get tax rates for quotes
        protected virtual void GetQuoteTaxSummary(int omsQuoteId, QuoteResponseModel quoteModel)
        {
            try
            {
                List<ZnodeOmsTaxQuoteSummary> omsTaxQuoteSummaries = _omsTaxQuoteSummary.Table.Where(x => x.OmsQuoteId == omsQuoteId).ToList();

                quoteModel.TaxSummaryList = new List<TaxSummaryModel>();

                foreach (ZnodeOmsTaxQuoteSummary omsTaxOrderSummary in omsTaxQuoteSummaries)
                {
                    quoteModel.TaxSummaryList.Add(new TaxSummaryModel()
                    {
                        OmsQuoteTaxSummaryId = omsTaxOrderSummary.OmsTaxQuoteSummaryId,
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

                    // If from manage order screen, if address getting changed then county and state may change so it is safe to delete old entries.
                    if (omsTaxOrderSummaries?.Count > 0)
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

        //Get tax summary for quotes
        protected virtual void GetTaxSummaryDetails(QuoteResponseModel quoteModel)
        {
            try
            {
                List<ZnodeOmsTaxQuoteSummary> omsTaxOrderSummaryList = new List<ZnodeOmsTaxQuoteSummary>();

                IZnodeRepository<ZnodeOmsTaxQuoteSummary> _omsTaxOrderSummaryRepository = new ZnodeRepository<ZnodeOmsTaxQuoteSummary>();

                List<ZnodeOmsTaxQuoteSummary> omsTaxOrderSummaries = _omsTaxOrderSummaryRepository.Table.Where(x => x.OmsQuoteId == quoteModel.OmsQuoteId).ToList();

                quoteModel.TaxSummaryList = new List<TaxSummaryModel>();
                foreach (ZnodeOmsTaxQuoteSummary omsTaxOrderSummary in omsTaxOrderSummaries)
                {
                    quoteModel.TaxSummaryList.Add(new TaxSummaryModel()
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

        protected virtual string GenerateHtmlResendReceiptWithParser(DataTable quotelineItemTable, DataTable quoteAmountTable, string receiptHtml)
        {
            ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(receiptHtml);
            // Parse OrderLineItem
            var filterData = quotelineItemTable.DefaultView;

            List<DataTable> group = filterData.ToTable().AsEnumerable()
              .GroupBy(r => new { Col1 = r["GroupId"] })
              .Select(g => g.CopyToDataTable()).ToList();

            receiptHelper.ParseWithGroup("LineItems" + "", group);

            receiptHelper.Parse("GrandAmountLineItems", quoteAmountTable.CreateDataReader());

            return receiptHelper?.Output;
        }

        //to get amount with currency symbol
        protected virtual string GetFormatPriceWithCurrency(decimal priceValue, string uom = "", string cultureCode = "")
        {
            return ZnodeCurrencyManager.FormatPriceWithCurrency(priceValue, cultureCode, uom);
        }
        #endregion
    }
}

