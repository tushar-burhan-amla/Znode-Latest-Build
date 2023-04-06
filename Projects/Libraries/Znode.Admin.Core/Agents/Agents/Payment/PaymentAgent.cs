using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class PaymentAgent : BaseAgent, IPaymentAgent
    {
        protected readonly IPaymentClient _paymentClient;
        private readonly IProfileClient _profileClient;

        public PaymentAgent(IPaymentClient paymentClient, IProfileClient profileClient)
        {
            _paymentClient = GetClient<IPaymentClient>(paymentClient);
            _profileClient = GetClient<IProfileClient>(profileClient);
        }

        #region Public Methods
        public virtual PaymentSettingListViewModel List(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(ZnodePaymentSettingEnum.DisplayOrder.ToString(), DynamicGridConstants.ASCKey);
            }

            ZnodeLogging.LogMessage("Input parameters expands, filters and sort collection:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { Expands = expands, Filters = filters, Sorts = sorts });
            PaymentSettingListModel paymentSettingList = _paymentClient.GetPaymentSettings(expands, filters, sorts, pageIndex, pageSize);
            PaymentSettingListViewModel listViewModel = new PaymentSettingListViewModel { PaymentSettings = paymentSettingList?.PaymentSettings?.ToViewModel<PaymentSettingViewModel>().ToList() };
            SetListPagingData(listViewModel, paymentSettingList);
            SetPaymentToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return paymentSettingList?.PaymentSettings?.Count > 0 ? listViewModel : new PaymentSettingListViewModel() { PaymentSettings = new List<PaymentSettingViewModel>() };
        }

        public virtual PaymentSettingViewModel GetPaymentSettingViewData(PaymentSettingViewModel paymentSettingViewModel = null, string paymentTypeCode = "")
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                paymentSettingViewModel = IsNull(paymentSettingViewModel) ? new PaymentSettingViewModel() : paymentSettingViewModel;
                paymentSettingViewModel.PaymentTypes = GetPaymentTypesListItems();
                GetPaymentGetwayList(paymentSettingViewModel, paymentTypeCode);
                if (string.IsNullOrEmpty(paymentTypeCode) && paymentSettingViewModel.PaymentTypes.Count > 0)
                {
                    paymentTypeCode = paymentSettingViewModel.PaymentTypes[0].Value;
                }
                paymentSettingViewModel.IsCallPaymentAPI = GetIsCallToPaymentAPI(paymentTypeCode);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return paymentSettingViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
        }      

        public virtual PaymentSettingViewModel AddPaymentSetting(PaymentSettingViewModel paymentSettingViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                PaymentSettingModel paymentSettingModel = paymentSettingViewModel?.ToModel<PaymentSettingModel>();

                ZnodeLogging.LogMessage("PaymentSettingModel with PaymentSettingId and PaymentApplicationSettingId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentSettingId = paymentSettingModel?.PaymentSettingId, PaymentApplicationSettingId = paymentSettingModel?.PaymentApplicationSettingId });
                if (!paymentSettingViewModel.IsActive || !_paymentClient.IsActivePaymentSettingPresentByPaymentCode(paymentSettingModel))
                {
                    //Save Payment setting in payment Application
                    if (_paymentClient.CallToPaymentAPI(paymentSettingViewModel.PaymentTypeCode))
                    {
                        if(string.Equals(paymentSettingViewModel.PaymentTypeCode, ZnodeConstant.PayPalExpress, StringComparison.CurrentCultureIgnoreCase))
                        {
                            paymentSettingModel.GatewayUsername = paymentSettingViewModel.PaypalClientId;
                            paymentSettingModel.GatewayPassword = paymentSettingViewModel.PaypalClientSecret;
                        }
                        PaymentSettingModel paymentSettingResponse = _paymentClient.CreatePaymentSetting(paymentSettingModel, true);
                        if (paymentSettingResponse?.PaymentSettingId > 0)
                        {
                            //save Payment setting in Znode payment Application
                            return (_paymentClient.CreatePaymentSetting(paymentSettingModel)?.ToViewModel<PaymentSettingViewModel>());
                        }
                        else
                            return (PaymentSettingViewModel)GetViewModelWithErrorMessage(paymentSettingViewModel, Admin_Resources.ErrorFailedToCreate);
                    }
                    else
                    {
                        //save Payment setting in Znode 
                        return (_paymentClient.CreatePaymentSetting(paymentSettingModel)?.ToViewModel<PaymentSettingViewModel>());
                    }
                }
                return (PaymentSettingViewModel)GetViewModelWithErrorMessage(paymentSettingViewModel, Admin_Resources.ErrorPaymentCodeAlreadyExist);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (PaymentSettingViewModel)GetViewModelWithErrorMessage(paymentSettingViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        public virtual PaymentSettingViewModel GetPaymentSetting(int paymentSettingId, int portalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters paymentSettingId and portalId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentSettingId = paymentSettingId, PortalId = portalId });
            try
            {
                PaymentSettingViewModel paymentSettingViewModel = null;
                PaymentSettingModel paymentSettingModel = _paymentClient.GetPaymentSetting(paymentSettingId, false, new ExpandCollection { ZnodePaymentSettingEnum.ZnodePaymentType.ToString() }, portalId);
                
                if (paymentSettingModel.IsCallToPaymentAPI)
                {
                    ZnodeLogging.LogMessage("PaymentCode to get payment setting:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentCode = paymentSettingModel?.PaymentCode });
                    paymentSettingViewModel = _paymentClient.GetPaymentSettingByPaymentCode(paymentSettingModel.PaymentCode)?.ToViewModel<PaymentSettingViewModel>();
                }

                if (HelperUtility.IsNotNull(paymentSettingViewModel) && string.Equals(paymentSettingViewModel.PaymentTypeCode, ZnodeConstant.PAYPAL_EXPRESS, StringComparison.InvariantCultureIgnoreCase))
                {
                    paymentSettingViewModel.PaypalClientId = paymentSettingViewModel.GatewayUsername;
                }
                if (IsNull(paymentSettingViewModel))
                    paymentSettingViewModel = new PaymentSettingViewModel();

                SetPaymentViewModel(paymentSettingViewModel, paymentSettingModel);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return paymentSettingViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
        }

        public virtual PaymentSettingViewModel UpdatePaymentSetting(PaymentSettingViewModel paymentSettingViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {               
                PaymentSettingModel paymentSettingModel = paymentSettingViewModel?.ToModel<PaymentSettingModel>();

                if (string.Equals(paymentSettingViewModel.PaymentTypeCode, ZnodeConstant.PayPalExpress, StringComparison.InvariantCultureIgnoreCase))
                {
                    paymentSettingModel.GatewayUsername = paymentSettingViewModel.PaypalClientId;
                    paymentSettingModel.GatewayPassword = paymentSettingViewModel.PaypalClientSecret;
                }
                if (!paymentSettingViewModel.IsActive || !_paymentClient.IsActivePaymentSettingPresentByPaymentCode(paymentSettingModel))
                {
                    int paymentsettingId = paymentSettingModel.PaymentSettingId;
                    if (_paymentClient.CallToPaymentAPI(paymentSettingModel.PaymentTypeCode))
                    {
                        //Update Payment setting in payment Application
                        PaymentSettingModel paymentSettingResponse = _paymentClient.UpdatePaymentSetting(paymentSettingModel, true);
                        if (paymentSettingResponse?.PaymentSettingId > 0)
                        {
                            paymentSettingModel.PaymentSettingId = paymentsettingId;
                            //Update Payment setting in Znode Application
                            return (_paymentClient.UpdatePaymentSetting(paymentSettingModel)?.ToViewModel<PaymentSettingViewModel>());
                        }
                        else
                            return (PaymentSettingViewModel)GetViewModelWithErrorMessage(paymentSettingViewModel, Admin_Resources.UpdateErrorMessage);
                    }
                    else
                    {
                        return (_paymentClient.UpdatePaymentSetting(paymentSettingModel)?.ToViewModel<PaymentSettingViewModel>());
                    }
                }
                return (PaymentSettingViewModel)GetViewModelWithErrorMessage(paymentSettingViewModel, Admin_Resources.ErrorPaymentCodeAlreadyExist);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (PaymentSettingViewModel)GetViewModelWithErrorMessage(paymentSettingViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        public virtual bool DeletePaymentSetting(string paymentSettingIds, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                errorMessage = Admin_Resources.PaymentDeleteErrorMessage;

                if (string.IsNullOrEmpty(paymentSettingIds)) return false;

                FilterCollection filters = new FilterCollection();

                //Replace Comma by _ as , is not allowed in filter value.
                filters.Add(ZnodePaymentSettingEnum.PaymentSettingId.ToString(), FilterOperators.In, paymentSettingIds.Replace(',', '_'));

                ZnodeLogging.LogMessage("Filters to get payment setting:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { Filters = filters });
                PaymentSettingListModel paymentList = _paymentClient.GetPaymentSettings(null, filters, null, null, null);

                //Delete Payment Setting from Admin. If deleted successfully delete from Payment Application
                if (_paymentClient.DeletePaymentSetting(paymentSettingIds))
                {
                    if (IsDeleteFromPaymentApplication(paymentList))
                        return _paymentClient.DeletePaymentSetting(GetPaymentCodeFromPaymentSetting(paymentList), true);
                    else
                        return true;
                }

                return false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = Admin_Resources.ErrorPaymentAssociated;
                        return false;
                    default:
                        errorMessage = Admin_Resources.PaymentDeleteErrorMessage;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.PaymentDeleteErrorMessage;
                return false;
            }
        }

        public virtual string GetPaymentTypeView(string paymentCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PaymentCode to get payment type view:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentCode = paymentCode });
            switch (paymentCode)
            {
                case ZnodeConstant.CreditCard:
                    return AdminConstants.CreditCardView;
                case ZnodeConstant.PurchaseOrder:
                    return AdminConstants.PurchaseOrderView;
                case ZnodeConstant.PAYPAL_EXPRESS:
                    return AdminConstants.PayPalExpressView;
                case ZnodeConstant.COD:
                    return AdminConstants.CODView;
                case ZnodeConstant.Amazon_Pay:
                    return AdminConstants.AmazonPayView;
                case ZnodeConstant.ACH:
                    return AdminConstants.ACHView;
            }
            return AdminConstants.CODView;
        }

        public virtual string GetPaymentGatewayView(string gatewayCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("GatewayCode to get payment gateway view:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { GatewayCode = gatewayCode });
            if (!String.IsNullOrEmpty(gatewayCode))
            {
                switch (gatewayCode)
                {
                    case AdminConstants.AuthorizeNet:
                        return AdminConstants.AuthorizeNetView;
                    case AdminConstants.Payflow:
                        return AdminConstants.PayflowView;
                    case AdminConstants.PaymentechOrbital:
                        return AdminConstants.PaymentechOrbitalView;
                    case AdminConstants.PayPalDirectPayment:
                        return AdminConstants.PayPalDirectPaymentView;
                    case AdminConstants.WorldPay:
                        return AdminConstants.WorldPayView;
                    case AdminConstants.CyberSource:
                        return AdminConstants.CyberSourceView;
                    case AdminConstants.Checkout2:
                        return AdminConstants.Checkout2View;
                    case AdminConstants.Stripe:
                        return AdminConstants.StripeView;
                    case AdminConstants.Braintree:
                        return AdminConstants.BraintreeView;
                    case ZnodeConstant.Amazon_Pay_Gateway:
                        return AdminConstants.AmazonPayView;
                    case ZnodeConstant.CardConnect:
                        return AdminConstants.CardConnectView;
                    case ZnodeConstant.ACHCardConnect:
                        return AdminConstants.ACHCardConnectView;
                }
                return AdminConstants.CustomGatewayView;
            }
            else
            {
                return AdminConstants.PayPalExpressView;
            }
        }

        public virtual PaymentSettingViewModel GetPaymentSettingCredentials(int paymentsettingId, bool isTestMode)
            => PaymentViewModelMap.ToPaymentSettingsViewModel(_paymentClient.GetPaymentSettingCredentials(paymentsettingId, isTestMode), new PaymentSettingViewModel(), isTestMode);

        public virtual PaymentSettingViewModel GetPaymentSettingCredentialsByPaymentCode(string paymentCode, bool isTestMode, string paymentTypeCode = "")
        {
            PaymentSettingViewModel paymentSettingViewModel = new PaymentSettingViewModel();
            paymentSettingViewModel= PaymentViewModelMap.ToPaymentSettingsViewModel(_paymentClient.GetPaymentSettingCredentialsByPaymentCode(paymentCode, isTestMode), new PaymentSettingViewModel(), isTestMode);
            paymentSettingViewModel.IsCallPaymentAPI = GetIsCallToPaymentAPI(paymentTypeCode);
            return paymentSettingViewModel;
        }        


        //Call PayNow method in Payment Application
        public virtual GatewayResponseModel ProcessPayNow(SubmitPaymentModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNotNull(model))
            {
                try
                {
                    return _paymentClient.PayNow(model);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                }
            }
            return new GatewayResponseModel { HasError = true, };
        }

        //Call PayNow method in Payment Application
        public virtual GatewayResponseModel ProcessPayPal(SubmitPaymentModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNotNull(model))
            {
                try
                {
                    return _paymentClient.PayPal(model);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                }
            }
            return new GatewayResponseModel { HasError = true, };
        }

        //Check whether the payment code already exists.
        public virtual bool CheckPaymentCodeExist(string paymentCode, int paymentSettingId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters paymentCode and paymentSettingId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentCode = paymentCode, PaymentSettingId = paymentSettingId });
            if (!string.IsNullOrEmpty(paymentCode))
            {
                paymentCode = paymentCode.Trim();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePaymentSettingEnum.PaymentCode.ToString(), FilterOperators.Contains, paymentCode));

                //Get the payment list based on the payment name filter.
                PaymentSettingListModel paymentList = _paymentClient.GetPaymentSettings(null, filters, null, null, null);
                if (paymentList?.PaymentSettings?.Count > 0)
                {
                    if (paymentSettingId > 0)
                        //Set the status in case the payment is open in edit mode.
                        paymentList.PaymentSettings.RemoveAll(x => x.PaymentSettingId == paymentSettingId);

                    return paymentList.PaymentSettings.FindIndex(x => string.Equals(x.PaymentCode, paymentCode, StringComparison.CurrentCultureIgnoreCase)) != -1;
                }
            }
            return false;
        }

        /// <summary>
        /// Check whether the payment display name already exists.
        /// </summary>
        /// <param name="paymentSettingValidationViewModel"></param>
        /// <returns>Return True False Response</returns>
        public virtual bool IsPaymentDisplayNameExists(PaymentSettingValidationViewModel paymentSettingValidationViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters paymentDisplayName and paymentSettingId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentDisplayName = paymentSettingValidationViewModel.PaymentDisplayName, PaymentSettingId = paymentSettingValidationViewModel.PaymentSettingId });
            if (!string.IsNullOrEmpty(paymentSettingValidationViewModel.PaymentDisplayName))
            {
                PaymentSettingValidationModel paymentSettingValidationModel = new PaymentSettingValidationModel();
                paymentSettingValidationModel = paymentSettingValidationViewModel?.ToModel<PaymentSettingValidationModel>();
                bool status = _paymentClient.IsPaymentDisplayNameExists(paymentSettingValidationModel);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return status;
            }
            return false;
        }


        //Parse Json string to PaymentSettingViewModel
        public PaymentSettingViewModel ParseStringToPaymentSettingViewModel(string paymentSetting)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                PaymentSettingViewModel paymentSettingViewModel = null;
                if (!string.IsNullOrEmpty(paymentSetting))
                {
                    JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                    paymentSettingViewModel = json_serializer.Deserialize<PaymentSettingViewModel>(paymentSetting);
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return paymentSettingViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
        }

        //Set PaymentSettingViewModel from paymentSettingModel
        public virtual void SetPaymentViewModel(PaymentSettingViewModel paymentSettingViewModel, PaymentSettingModel paymentSettingModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            paymentSettingViewModel.PaymentCode = paymentSettingModel.PaymentCode;
            paymentSettingViewModel.PaymentSettingId = paymentSettingModel.PaymentSettingId;
            paymentSettingViewModel.PaymentTypeId = paymentSettingModel.PaymentTypeId;
            paymentSettingViewModel.IsActive = paymentSettingModel.IsActive;
            paymentSettingViewModel.DisplayOrder = paymentSettingModel.DisplayOrder;
            paymentSettingViewModel.PaymentDisplayName = paymentSettingModel.PaymentDisplayName;
            paymentSettingViewModel.IsPoDocRequire = paymentSettingModel.IsPoDocRequire;
            paymentSettingViewModel.IsPoDocUploadEnable = paymentSettingModel.IsPoDocUploadEnable;
            paymentSettingViewModel.IsBillingAddressOptional = paymentSettingModel.IsBillingAddressOptional;
            paymentSettingViewModel.IsUsedForOfflinePayment = paymentSettingModel.IsUsedForOfflinePayment;

            paymentSettingViewModel.PaymentTypes = GetPaymentTypesListItems();
            if (HelperUtility.IsNotNull(paymentSettingModel.PaymentGatewayId))
            {
                GetPaymentGetwayList(paymentSettingViewModel, paymentSettingModel.PaymentTypeCode);
            }
            paymentSettingViewModel.PaymentTypeName = paymentSettingModel.PaymentTypeName;
            paymentSettingViewModel.IsCaptureDisable = paymentSettingModel.IsCaptureDisable;
            paymentSettingViewModel.PaymentExternalId = paymentSettingModel.PaymentExternalId;
            paymentSettingViewModel.IsApprovalRequired = paymentSettingModel.IsApprovalRequired;
            paymentSettingViewModel.IsOABRequired = paymentSettingModel.IsOABRequired;
            paymentSettingViewModel.PaymentTypeCode = paymentSettingModel.PaymentTypeCode;
            paymentSettingViewModel.IsCallPaymentAPI = paymentSettingModel.IsCallToPaymentAPI;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Get generated payment token.
        public string GetPaymentAuthToken(string userOrSessionId, bool fromAdminApp = false)
        {
            try
            {

                string token = _paymentClient.GetPaymentAuthToken(userOrSessionId, fromAdminApp);
                return token;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Payment.ToString(), TraceLevel.Error);
                return null;
            }
        }
        public virtual PaymentTokenViewModel GetPaymentGatewayToken(PaymentTokenViewModel paymentTokenViewModel)
        {
            try
            {
                //To get GatewayLoginName and GatewayTransactionKey for AuthorizeNet token
                PaymentSettingViewModel paymentSetting = GetPaymentSetting(paymentTokenViewModel.PaymentSettingId);

                if (IsNotNull(paymentSetting))
                {
                    paymentTokenViewModel.GatewayLoginName = paymentSetting.GatewayUsername;
                    paymentTokenViewModel.GatewayTransactionKey = paymentSetting.TransactionKey;
                    if (string.Equals(paymentTokenViewModel.GatewayCode, ZnodeConstant.Braintree, StringComparison.InvariantCultureIgnoreCase))
                    {
                        paymentTokenViewModel.GatewayPassword = paymentSetting.GatewayPassword;
                        paymentTokenViewModel.GatewayTestMode = paymentSetting.TestMode;
                    }
                }
                PaymentGatewayTokenModel paymentTokenModel = paymentTokenViewModel?.ToModel<PaymentGatewayTokenModel>();

                paymentTokenModel.BillingAddress = new AddressModel();
                paymentTokenModel.ShippingAddress = new AddressModel();

                if (paymentTokenViewModel.GatewayCode == ZnodeConstant.AuthorizeNet)
                {

                    var BillingAddress1 = paymentTokenViewModel.AddressListViewModel.BillingAddress;
                    var ShippingAddress1 = paymentTokenViewModel.AddressListViewModel.ShippingAddress;

                    if (paymentTokenModel.BillingAddress != null)
                    {
                        paymentTokenModel.BillingAddress.FirstName = BillingAddress1.FirstName;
                        paymentTokenModel.BillingAddress.LastName = BillingAddress1.LastName;
                        paymentTokenModel.BillingAddress.StateName = BillingAddress1.StateName;
                        paymentTokenModel.BillingAddress.CountryName = BillingAddress1.CountryName;
                        paymentTokenModel.BillingAddress.CompanyName = BillingAddress1.CompanyName;
                        paymentTokenModel.BillingAddress.Address1 = BillingAddress1.Address1;
                        paymentTokenModel.BillingAddress.CityName = BillingAddress1.CityName;
                        paymentTokenModel.BillingAddress.PostalCode = BillingAddress1.PostalCode;
                        paymentTokenModel.BillingAddress.PhoneNumber = BillingAddress1.PhoneNumber;
                        paymentTokenModel.BillingAddress.FaxNumber = BillingAddress1.FaxNumber;
                        paymentTokenModel.BillingAddress.EmailAddress = paymentTokenViewModel.AddressListViewModel.EmailAddress;
                        paymentTokenModel.BillingAddress.Address2 = BillingAddress1.Address2;
                    }

                    if (paymentTokenModel.ShippingAddress != null)
                    {
                        paymentTokenModel.ShippingAddress.FirstName = ShippingAddress1.FirstName;
                        paymentTokenModel.ShippingAddress.LastName = ShippingAddress1.LastName;
                        paymentTokenModel.ShippingAddress.StateName = ShippingAddress1.StateName;
                        paymentTokenModel.ShippingAddress.CountryName = ShippingAddress1.CountryName;
                        paymentTokenModel.ShippingAddress.CompanyName = ShippingAddress1.CompanyName;
                        paymentTokenModel.ShippingAddress.Address1 = ShippingAddress1.Address1;
                        paymentTokenModel.ShippingAddress.CityName = ShippingAddress1.CityName;
                        paymentTokenModel.ShippingAddress.PostalCode = ShippingAddress1.PostalCode;
                        paymentTokenModel.ShippingAddress.PhoneNumber = ShippingAddress1.PhoneNumber;
                        paymentTokenModel.ShippingAddress.FaxNumber = ShippingAddress1.FaxNumber;
                        paymentTokenModel.ShippingAddress.Address2 = ShippingAddress1.Address2;
                    }
                }


                paymentTokenViewModel = (_paymentClient.GetPaymentGatewayToken(paymentTokenModel))?.ToViewModel<PaymentTokenViewModel>();
                return paymentTokenViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Payment.ToString(), TraceLevel.Error);
                return null;
            }
        }

        #endregion

        #region Private Methods        
        protected List<BaseDropDownOptions> GetPaymentTypesListItems()
        {
            PaymentTypeListModel paymentTypes = GetPaymentTypes();

            List<BaseDropDownOptions> items = new List<BaseDropDownOptions>();
            if (paymentTypes?.PaymentTypes?.Count > 0)
            {
                paymentTypes.PaymentTypes.ToList().ForEach(x =>
                {
                    items.Add(new BaseDropDownOptions()
                    {
                        Id = Convert.ToString(x.PaymentTypeId),
                        Text = x.Description,
                        Value = x.Code,
                        Type = x.BehaviorType
                    });
                });
            }
            ZnodeLogging.LogMessage("PaymentTypesListItems count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentTypesListItemsCount = items?.Count });
            return items;
        }

        protected List<BaseDropDownOptions> GetpaymentGetwayListItems()
        {
            PaymentGatewayListModel paymentGateways = GetPaymentGetways();

            List<BaseDropDownOptions> items = new List<BaseDropDownOptions>();
            if (paymentGateways?.PaymentGateways?.Count > 0)
            {
                paymentGateways.PaymentGateways.OrderBy(x => x.GatewayName).ToList().ForEach(x =>
                {
                    items.Add(new BaseDropDownOptions()
                    {
                        Id = Convert.ToString(x.PaymentGatewayId),
                        Text = x.GatewayName,
                        Value = x.GatewayCode
                    });
                });
            }
            ZnodeLogging.LogMessage("PaymentGetwayListItems count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentGetwayListItemsCount = items?.Count });
            return items;
        }

        //Get PaymentGetways from payment application
        //private PaymentGatewayListModel GetPaymentGetways() => _paymentClient.GetGateways();
        private PaymentGatewayListModel GetPaymentGetways()
        {
            //Skipped paypalexpress gateway from the list 
            PaymentGatewayListModel paymentGatewayListModel =  _paymentClient.GetGateways();
            paymentGatewayListModel.PaymentGateways = paymentGatewayListModel?.PaymentGateways?.Where(x => x.GatewayCode != AdminConstants.PayPalExpressCheckout).ToList();
            
            return paymentGatewayListModel;
        }

        //Get PaymentTypes from payment application
        private PaymentTypeListModel GetPaymentTypes() => _paymentClient.GetPaymentTypes();

        //Map Profiles to Drop Down List
        private List<SelectListItem> GetProfiles()
        {
            ProfileListModel profileList = _profileClient.GetProfileList(null, null, null, null);
            List<SelectListItem> profiles = new List<SelectListItem>();
            if (profileList?.Profiles?.Count > 0)
            {
                profileList.Profiles.OrderBy(x => x.ProfileName).ToList().ForEach(x =>
                 {
                     profiles.Add(new SelectListItem()
                     {
                         Text = x.ProfileName,
                         Value = Convert.ToString(x.ProfileId)
                     });
                 });
            }
            return profiles;
        }

        //Set the Tool Menus for Account List Grid View.
        private void SetPaymentToolMenu(PaymentSettingListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeletePopup')", ControllerName = "Payment", ActionName = "Delete" });
            }
        }

        //Check to delete payment setting from payment application from paymentsettings list model
        private bool IsDeleteFromPaymentApplication(PaymentSettingListModel paymentList)
        {
            var isCallToPaymentAPI = paymentList?.PaymentSettings?.Where(x => x.IsCallToPaymentAPI).ToList() ?? null;
            ZnodeLogging.LogMessage("isCallToPaymentAPI list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { IsCallToPaymentAPIListCount = isCallToPaymentAPI?.Count });
            return isCallToPaymentAPI?.Count > 0;
        }

        //Get paymentcode from paymentsettings list model
        private string GetPaymentCodeFromPaymentSetting(PaymentSettingListModel paymentList)
        => String.Join(",", paymentList?.PaymentSettings?.Select(x => x.PaymentCode)?.ToArray());

        //Get IsCallToPaymentAPI from paymentcode
        protected bool GetIsCallToPaymentAPI(string paymentTypeCode)
        {
            if (!string.IsNullOrEmpty(paymentTypeCode))
                return _paymentClient.CallToPaymentAPI(paymentTypeCode);

            return false;
        }

        //Get PaymentGetways from payment application for ACH.
        protected virtual PaymentGatewayListModel GetPaymentGetwaysForACH() => _paymentClient.GetGatewaysForACH();

        //Get PaymentGetways list from payment application for ACH.
        protected virtual List<BaseDropDownOptions> GetpaymentGetwayListItemsForACH()
        {
            PaymentGatewayListModel paymentGateways = GetPaymentGetwaysForACH();

            List<BaseDropDownOptions> items = new List<BaseDropDownOptions>();
            if (paymentGateways?.PaymentGateways?.Count > 0)
            {
                paymentGateways.PaymentGateways.OrderBy(x => x.GatewayName).ToList().ForEach(x =>
                {
                    items.Add(new BaseDropDownOptions()
                    {
                        Id = Convert.ToString(x.PaymentGatewayId),
                        Text = x.GatewayName,
                        Value = x.GatewayCode
                    });
                });
            }
            ZnodeLogging.LogMessage("PaymentGetwayListItems count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PaymentGetwayListItemsCount = items?.Count });
            return items;
        }

        //Get PaymentGetways list from payment application.
        protected virtual void GetPaymentGetwayList(PaymentSettingViewModel paymentSettingViewModel, string paymentTypeCode)
        {
            if (HelperUtility.IsNotNull(paymentSettingViewModel))
            {
                if (string.Equals(paymentTypeCode, Admin_Resources.AutomatedClearingHouse, StringComparison.InvariantCultureIgnoreCase))
                {
                    paymentSettingViewModel.PaymentGateways = GetpaymentGetwayListItemsForACH();
                }
                else
                {
                    paymentSettingViewModel.PaymentGateways = GetpaymentGetwayListItems();
                }
            }
        }

        #endregion
    }
}
