using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Znode.Multifront.PaymentApplication.Data.Maps;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Data
{
    public class PaymentMethodsService : BaseService
    {
        #region Private Variables
        private readonly IZnodePaymentRepository<ZnodePaymentMethod> _paymentMethodRepository;
        private readonly IZnodePaymentRepository<ZnodePaymentAddress> _paymentAddressRepository;
        #endregion

        #region Constructor
        public PaymentMethodsService()
        {
            _paymentMethodRepository = new ZnodePaymentRepository<ZnodePaymentMethod>();
            _paymentAddressRepository = new ZnodePaymentRepository<ZnodePaymentAddress>();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Add Payment Methods 
        /// </summary>
        /// <param name="paymentModel">Payment Model</param>
        /// <returns>payment guid </returns>
        public string AddPaymentMethods(PaymentModel paymentModel)
        {
            try
            {
                ZnodePaymentMethod paymentMethods = _paymentMethodRepository.Insert(PaymentMethodMap.ToEntity(paymentModel));
                return paymentMethods.PaymentGUID.ToString();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                string fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                string exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                string data = $"PaymentGUID : {paymentModel.PaymentToken}";
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, exceptionMessage, data);
                Logging.LogMessage(exceptionMessage, Logging.Components.Payment.ToString(), TraceLevel.Error);

            }
            return null;
        }

        /// <summary>
        /// Get Payment methods by payment settings id and Customer GUID
        /// </summary>
        /// <param name="paymentSettingId">int payment settings id</param>
        /// <param name="customersGUID">string Customers GUID</param>
        /// <returns>All the information of PaymentMethod in the form of ZnodePaymentMethod List type Model</returns>
        public List<PaymentMethodCCDetailsModel> GetPaymentMethods(int paymentSettingId, string customersGUID)
        {
            var paymentMethods = GetSavedCreditCardList(paymentSettingId, customersGUID);
            return PaymentMethodMap.ToPaymentMethodCCDetailsModel(paymentMethods);
        }

        /// <summary>
        /// Get Payment methods by payment settings id and Customer GUID
        /// </summary>
        /// <param name="paymentSettingId">int payment settings id</param>
        /// <param name="customersGUID">string Customers GUID</param>
        /// <returns>All the information of PaymentMethod in the form of ZnodePaymentMethod List type Model</returns>
        public List<PaymentMethodCCDetailsModel> GetPaymentMethodsACH(int paymentSettingId, string customersGUID)
        {
            var paymentMethods = GetSavedACHAccountList(paymentSettingId, customersGUID);
            return PaymentMethodMap.ToPaymentMethodACHDetailsModel(paymentMethods);
        }

        /// <summary>
        /// Get Payment methods by payment settings id and Customer GUID
        /// </summary>
        /// <param name="paymentSettingId">payment settings id</param>
        /// <param name="CustomersGUID">Customers GUID</param>
        /// <returns>paymentMethod form ZnodePaymentMethod</returns>
        public ZnodePaymentMethod GetPaymentMethod(int paymentSettingId, string CustomersGUID, bool skipCardTypeCheck = false)
        {
            string getActiveCardTypes = skipCardTypeCheck ? string.Empty : GetAvailableCardTypes(paymentSettingId);

            var paymentMethods = (from item in _paymentMethodRepository.Table
                                  where (item.PaymentSettingID == paymentSettingId
                                  && item.CustomersGUID == new Guid(CustomersGUID)
                                  && skipCardTypeCheck ? item.IsSaveCreditCard.Equals(true) :
                                  getActiveCardTypes.ToLower().Contains(item.CardType.ToLower()) && item.IsSaveCreditCard.Equals(true))
                                  select item);

            if (paymentMethods.Any())
            {
                ZnodePaymentMethod paymentMethod = paymentMethods.FirstOrDefault();
                paymentMethod.Token = DecryptPaymentToken(paymentMethod.Token);
                return paymentMethod;
            }
            return null;
        }

        /// <summary>
        /// Get Payment methods by payment settings id and Customer GUID
        /// </summary>
        /// <param name="paymentSettingId">payment settings id</param>
        /// <param name="CustomersGUID">Customers GUID</param>
        /// <returns>paymentMethod form ZnodePaymentMethod</returns>
        public ZnodePaymentMethod GetPaymentMethodForACH(int paymentSettingId, string CustomersGUID)
        {
            //string getActiveCardTypes = GetAvailableCardTypes(paymentSettingId);

            var paymentMethods = (from item in _paymentMethodRepository.Table
                                  where (item.PaymentSettingID == paymentSettingId
                                  && item.CustomersGUID == new Guid(CustomersGUID)
                                  && item.IsSaveCreditCard.Equals(true))
                                  select item);

            if (paymentMethods.Any())
            {
                ZnodePaymentMethod paymentMethod = paymentMethods.FirstOrDefault();
                paymentMethod.Token = DecryptPaymentToken(paymentMethod.Token);
                return paymentMethod;
            }
            return null;
        }


        /// <summary>
        ///  Get Payment methods by paymentSettingId ,CustomersGUID and paymentToken
        /// </summary>
        /// <param name="paymentSettingId">Id of paymentSetting</param>
        /// <param name="CustomersGUID">Customers GUID</param>
        /// <param name="paymentToken">payment Token</param>
        /// <returns>ZnodePaymentMethod</returns>
        public ZnodePaymentMethod GetPaymentMethod(int paymentSettingId, string CustomersGUID, string paymentToken)
        {
            var paymentMethods = from item in _paymentMethodRepository.Table
                                 where (item.PaymentSettingID == paymentSettingId
                                 && item.CustomersGUID == new Guid(CustomersGUID)
                                 && item.PaymentGUID == new Guid(paymentToken)
                                 && item.IsSaveCreditCard.Equals(true))
                                 select item;

            if (paymentMethods.Any())
            {
                ZnodePaymentMethod paymentMethod = paymentMethods.FirstOrDefault();
                paymentMethod.Token = DecryptPaymentToken(paymentMethod.Token);
                return paymentMethod;
            }
            return null;
        }

        /// <summary>
        ///  Get Payment methods by paymentSettingId ,CustomersGUID and paymentToken based on CustomersGUID, cardnumber,  ExpiryMonth, ExpiryYear 
        /// </summary>
        /// <param name="paymentSettingId">Id of paymentSetting</param>
        /// <param name="CustomersGUID">Customers GUID</param>
        /// <param name="cardnumber">Card number</param>
        /// <param name="ExpMonth">Card expiry month</param>
        /// <param name="ExpYear">Card expiry year</param>
        /// <returns>ZnodePaymentMethod</returns>
        public ZnodePaymentMethod GetPaymentMethod(int paymentSettingId, string CustomersGUID, string cardnumber, int? ExpMonth, int? ExpYear)
        {
            var paymentMethods = from item in _paymentMethodRepository.Table
                                 where (item.PaymentSettingID == paymentSettingId
                                 && item.CustomersGUID == new Guid(CustomersGUID)
                                 && item.CreditCardLastFourDigit == cardnumber
                                 && item.CreditCardExpMonth == ExpMonth
                                 && item.CreditCardExpYear == ExpYear
                                 && item.IsSaveCreditCard.Equals(true))
                                 select item;

            if (paymentMethods.Any())
            {
                ZnodePaymentMethod paymentMethod = paymentMethods.FirstOrDefault();
                paymentMethod.Token = DecryptPaymentToken(paymentMethod.Token);
                return paymentMethod;
            }
            return null;
        }

        /// <summary>
        /// Get all Saved Payment Credit Card Details
        /// </summary>
        /// <param name="paymentSettingId">ID of paymentSetting</param>
        /// <param name="customersGUID">customers GUID</param>
        /// <returns>List of PaymentMethodCCDetailsModel</returns>
        public List<PaymentMethodCCDetailsModel> GetPaymentCreditCardDetails(int paymentSettingId, string customerGUID)
        {
            string getActiveCardTypes = GetAvailableCardTypes(paymentSettingId);

            return (from address in _paymentAddressRepository.Table
                    join paymentMethod in _paymentMethodRepository.Table
                        on address.CreditCardAddressId equals paymentMethod.CreditCardAddressId
                    where (paymentMethod.PaymentSettingID == paymentSettingId)
                    && (paymentMethod.CustomersGUID.Value == new Guid(customerGUID))
                    && paymentMethod.IsSaveCreditCard.Equals(true)
                    && getActiveCardTypes.ToLower().Contains(paymentMethod.CardType.ToLower())
                    select new PaymentMethodCCDetailsModel
                    {
                        CreditCardLastFourDigit = paymentMethod.CreditCardLastFourDigit,
                        PaymentGUID = paymentMethod.PaymentGUID,
                        CardHolderFirstName = address.CardHolderFirstName,
                        CardHolderLastName = address.CardHolderLastName,
                        AddressLine1 = address.AddressLine1,
                        AddressLine2 = address.AddressLine2,
                        City = address.City,
                        State = address.State,
                        Country = address.Country,
                        ZipCode = address.ZipCode,
                        CreditCardImageUrl = paymentMethod.CreditCardImageUrl,
                        CardType = paymentMethod.CardType
                    }).ToList();
        }

        /// <summary>
        /// Delete PaymentMethod present across customerProfileId and CustomerPaymentProfileId
        /// </summary>
        /// <param name="customerProfileId">string customerProfileId</param>
        /// <param name="CustomerPaymentProfileId">string CustomerPaymentProfileId</param>
        /// <returns>Delete status True/False</returns>
        public bool DeletePaymentMethods(string customerProfileId, string CustomerPaymentProfileId)
        {
            CustomerPaymentProfileId = EncryptPaymentToken(CustomerPaymentProfileId);
            try
            {
                ZnodePaymentMethod paymentMethod = _paymentMethodRepository.Table.FirstOrDefault(e => e.CustomerProfileId == customerProfileId && e.Token == CustomerPaymentProfileId);

                if (!Equals(paymentMethod, null))
                {
                    _paymentAddressRepository.Delete($"{ZnodePaymentAddressEnum.CreditCardAddressId}{Operators.Equals}{paymentMethod.CreditCardAddressId}");

                    return _paymentMethodRepository.Delete(paymentMethod);
                }
                return false;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                return false;
            }
        }       
        /// <summary>
        /// Get saved credit card count by settings id and Customer GUID
        /// </summary>
        /// <param name="paymentSettingId">int payment settings id</param>
        /// <param name="customersGUID">string Customers GUID</param>
        /// <returns>Saved credit card count</returns>
        public int GetSaveCreditCardCount(int paymentSettingId, string customersGUID)
        => GetSavedCreditCardList(paymentSettingId, customersGUID).Count();

        /// <summary>
        /// Delete saved credit card Details.
        /// </summary>
        /// <param name="PaymentGUID">PaymentGUID</param>
        /// <returns>bool</returns>
        public BooleanModel DeleteSavedCreditCard(string PaymentGUID)
            => new BooleanModel { IsSuccess = _paymentMethodRepository.Delete((from item in _paymentMethodRepository.Table where item.PaymentGUID.Equals(new Guid(PaymentGUID)) select item).FirstOrDefault()) };

        /// <summary>
        /// Get all saved card details by customers GUID
        /// </summary>
        /// <param name="customersGUID">customers GUID</param>
        /// <returns>List of PaymentMethodCCDetailsModel</returns>
        public List<PaymentMethodCCDetailsModel> GetSavedCardDetailsByCustomerGUID(string customerGUID)
        {
            return (from address in _paymentAddressRepository.Table
                    join paymentMethod in _paymentMethodRepository.Table
                        on address.CreditCardAddressId equals paymentMethod.CreditCardAddressId
                    where (paymentMethod.CustomersGUID.Value == new Guid(customerGUID))
                    && paymentMethod.IsSaveCreditCard.Equals(true)
                    select new PaymentMethodCCDetailsModel
                    {
                        CreditCardLastFourDigit = paymentMethod.CreditCardLastFourDigit,
                        PaymentGUID = paymentMethod.PaymentGUID,
                        CardHolderFirstName = address.CardHolderFirstName,
                        CardHolderLastName = address.CardHolderLastName,
                        AddressLine1 = address.AddressLine1,
                        AddressLine2 = address.AddressLine2,
                        City = address.City,
                        State = address.State,
                        Country = address.Country,
                        ZipCode = address.ZipCode,
                        CreditCardImageUrl = paymentMethod.CreditCardImageUrl,
                        CardType = paymentMethod.CardType
                    }).ToList();
        }

        public string GetSavedCardDetailsByPaymentGuid (System.Guid paymentGUID, out string cardNumber)
        {
            ZnodePaymentMethod paymentModel =_paymentMethodRepository.Table.FirstOrDefault(x => x.PaymentGUID == paymentGUID);
            cardNumber = paymentModel.CreditCardLastFourDigit;
            return paymentModel.Token;
        }

        /// <summary>
        ///  Get Token by CustomersGUID and paymentToken
        /// </summary>
        /// <param name="CustomersGUID">Customers GUID</param>
        /// <param name="paymentToken">payment Token</param>
        /// <returns>string</returns>
        public string GetPaymentACHToken(string customersGUID, string paymentToken)
        {
            var paymentMethods = from item in _paymentMethodRepository.Table
                                 where (item.CustomersGUID == new Guid(customersGUID)
                                 && item.PaymentGUID == new Guid(paymentToken)
                                 && item.IsSaveCreditCard.Equals(true))
                                 select item;

            if (paymentMethods.Any())
            {
                ZnodePaymentMethod paymentMethod = paymentMethods.FirstOrDefault();
                return DecryptPaymentToken(paymentMethod.Token);
            }
            return null;
        }
        #endregion

        #region Private Methods
        private List<ZnodePaymentMethod> GetSavedCreditCardList(int paymentSettingId, string customersGUID)
        {
            string getActiveCardTypes = GetAvailableCardTypes(paymentSettingId);
            List<ZnodePaymentMethod> paymentMethods = (from item in _paymentMethodRepository.Table
                                                       where item.PaymentSettingID.Value.Equals(paymentSettingId)
                                                       && (item.CustomersGUID.Value).Equals(new Guid(customersGUID))
                                                       && item.IsSaveCreditCard.Equals(true)
                                                       
                                                       select item).ToList();
            return paymentMethods;
        }

        protected virtual List<ZnodePaymentMethod> GetSavedACHAccountList(int paymentSettingId, string customersGUID)
        {
            List<ZnodePaymentMethod> paymentMethods = (from item in _paymentMethodRepository.Table
                                                       where item.PaymentSettingID.Value.Equals(paymentSettingId)
                                                       && (item.CustomersGUID.Value).Equals(new Guid(customersGUID))
                                                       && item.IsSaveCreditCard.Equals(true)
                                                       select item).ToList();
            return paymentMethods;
        }
        /// <summary>
        /// This method will return all the available credit card types  
        /// </summary>
        /// <param name="paymentSettingsId"></param>
        /// <returns></returns>
        private string GetAvailableCardTypes(int paymentSettingsId)
        {
            string activeCardTypes = string.Empty;

            PaymentSettingsService repository = new PaymentSettingsService();
            ZNodePaymentSetting paymentSetting = repository.GetPaymentSetting(paymentSettingsId);

            if (!Equals(paymentSetting, null))
            {
                if (paymentSetting.EnableAmex.Value)
                    activeCardTypes = string.Concat(activeCardTypes, ",", "AMEX");

                if (paymentSetting.EnableDiscover.Value)
                    activeCardTypes = string.Concat(activeCardTypes, ",", "DISCOVER");

                if (paymentSetting.EnableMasterCard.Value)
                    activeCardTypes = string.Concat(activeCardTypes, ",", "MASTERCARD");

                if (paymentSetting.EnableVisa.Value)
                    activeCardTypes = string.Concat(activeCardTypes, ",", "VISA");

                if (activeCardTypes.StartsWith(","))
                    activeCardTypes = activeCardTypes.Substring(1);
                else if (activeCardTypes.EndsWith(","))
                    activeCardTypes = activeCardTypes.Substring(0, activeCardTypes.Length - 2);
            }
            return activeCardTypes;
        }
        #endregion
    }
}