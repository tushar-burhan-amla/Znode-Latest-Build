using System;
using System.Linq;
using System.Collections.Specialized;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;
using System.Collections.Generic;


namespace Znode.Multifront.PaymentApplication.Data
{
    public class PaymentSettingsService : BaseService
    {
        #region Private Variables
        private readonly IZnodePaymentRepository<ZNodePaymentSetting> _paymentSettingRepository;
        private readonly IZnodePaymentRepository<ZNodePaymentSettingCredential> _paymentSettingCredentialRepository;
        private readonly IZnodePaymentRepository<ZnodeTransaction> _transactionsRepository;
        private readonly IZnodePaymentRepository<ZNodePaymentGateway> _paymentGateway;
        private readonly IZnodePaymentRepository<ZNodePaymentType> _paymentTypeRepository;
        #endregion

        #region Constructor
        public PaymentSettingsService()
        {
            _paymentSettingRepository = new ZnodePaymentRepository<ZNodePaymentSetting>();
            _paymentSettingCredentialRepository = new ZnodePaymentRepository<ZNodePaymentSettingCredential>();
            _transactionsRepository = new ZnodePaymentRepository<ZnodeTransaction>();
            _paymentGateway = new ZnodePaymentRepository<ZNodePaymentGateway>();
            _paymentTypeRepository = new ZnodePaymentRepository<ZNodePaymentType>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add the payment settings in payment api db.
        /// </summary>
        /// <param name="paymentSetting">ZNodePaymentSetting settings</param>
        /// <returns>Payment Settings Model</returns>
        public PaymentSettingsModel AddPaymentSetting(PaymentSettingsModel paymentSetting)
        {
            if (Equals(paymentSetting, null)) return null;

            SetPaymentTypeAndGatewayIdByCode(paymentSetting);

            ZNodePaymentSetting paymentSettingEntity = paymentSetting.ToEntity<ZNodePaymentSetting>();
            _paymentSettingRepository.Insert(paymentSettingEntity);

            UpdatePaymentechURLs(paymentSetting.PaymentGatewayId.GetValueOrDefault(), paymentSetting.TestMode);

            paymentSetting.PaymentSettingId = paymentSettingEntity.PaymentSettingId;
            PaymentSettingCredentialsService paymentSettingCredentialsRepository = new PaymentSettingCredentialsService();
            paymentSettingCredentialsRepository.AddPaymentSettingCredential(paymentSetting.ToEntity<ZNodePaymentSettingCredential>());

            return paymentSettingEntity?.ToModel<PaymentSettingsModel>();
        }

        /// <summary>
        /// Update the payment settings in payment api db.
        /// </summary>
        /// <param name="paymentSetting">ZNodePaymentSetting settings</param>
        /// <returns>Returns true if updated successfully else return false</returns>
        public bool UpdatePaymentSetting(PaymentSettingsModel paymentSetting)
        {
            if (Equals(paymentSetting, null)) return false;

            if (!string.IsNullOrEmpty(paymentSetting.PaymentCode))
                paymentSetting.PaymentSettingId = GetPaymentSettingIdByCode(paymentSetting.PaymentCode);

            SetPaymentTypeAndGatewayIdByCode(paymentSetting);

            bool updateResponse = _paymentSettingRepository.Update(paymentSetting.ToEntity<ZNodePaymentSetting>());

            UpdatePaymentechURLs(paymentSetting.PaymentGatewayId.GetValueOrDefault(), paymentSetting.TestMode);

            bool updateCredentialsResponse = new PaymentSettingCredentialsService().UpdatePaymentSettingCredential(paymentSetting?.ToEntity<ZNodePaymentSettingCredential>());
            return updateCredentialsResponse || updateResponse;
        }

        /// <summary>
        /// Delete the payment settings based on the payment settings id.
        /// </summary>
        /// <param name="paymentSettingId">int payment settings id</param>
        /// <returns>True if record deleted</returns>
        public bool DeletePaymentSetting(string paymentSettingId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZNodePaymentSettingCredentialEnum.PaymentSettingId.ToString(), EntityFilterOperators.In, paymentSettingId));

            //Delete Payment Setting Credentials
            _paymentSettingCredentialRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);

            //Remove Filters once Delete is complete
            filters.RemoveAll(x => x.FilterName.ToLower() == ZNodePaymentSettingCredentialEnum.PaymentSettingId.ToString().ToLower());

            //Add Payment Setting Filters
            filters.Add(new FilterTuple(ZNodePaymentSettingEnum.PaymentSettingId.ToString(), EntityFilterOperators.In, paymentSettingId));

            return _paymentSettingRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        /// <summary>
        /// Get Payment settings by payment settings id
        /// </summary>
        /// <param name="paymentSettingId">int payment settings id</param>
        /// <returns>All the information of PaymentSettings in the form of ZNodePaymentSetting entity</returns>
        public ZNodePaymentSetting GetPaymentSetting(int paymentSettingId)
          => (from paymentSettings in _paymentSettingRepository.Table
              where paymentSettings.PaymentSettingId == paymentSettingId
              select paymentSettings).FirstOrDefault();


        /// <summary>
        /// Get Payment Code by payment settings id
        /// </summary>
        /// <param name="paymentSettingId">int payment settings id</param>
        /// <returns>Payment code</returns>
        public string GetPaymentCodeAndTypes(int paymentSettingId)
        {
            var codeType = (from paymentSettings in _paymentSettingRepository.Table 
             join paymentType in _paymentTypeRepository.Table  on paymentSettings.PaymentTypeId equals paymentType.PaymentTypeId
             where paymentSettings.PaymentSettingId == paymentSettingId
             select new { paymentCode = paymentSettings.PaymentCode, paymentType = paymentType.Code }).FirstOrDefault();

            if(string.Equals(codeType.paymentType, "paypalexpress", StringComparison.InvariantCultureIgnoreCase))
            {
                return codeType.paymentCode;
            }
            return string.Empty;
        }


        /// <summary>
        /// Get All Payment Settings
        /// </summary>
        /// <returns>Payment Setting ListModel</returns>
        public PaymentSettingListModel GetPaymentSetting(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            //Set Paging Parameters
            int pagingStart = 0;
            int pagingLength = 0;
            int totalCount = 0;
            HelperMethods.SetPaging(page, out pagingStart, out pagingLength);

            //Get Sort Clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            List<ZNodePaymentSetting> paymentSetting = _paymentSettingRepository.GetPagedList(whereClauseModel.WhereClause, orderBy, whereClauseModel.FilterValues, pagingStart, pagingLength, out totalCount)?.ToList();

            PaymentSettingListModel listModel = new PaymentSettingListModel();

            listModel.PaymentSettings = paymentSetting?.Count > 0 ? paymentSetting.ToModel<PaymentSettingsModel>().ToList() : new List<PaymentSettingsModel>();

            listModel.PageIndex = pagingStart;
            listModel.PageSize = pagingLength;
            listModel.TotalResults = totalCount;

            return listModel;
        }

        /// <summary>
        /// Get Payment settings along with PaymentSetting Credentials by paymentCode
        /// </summary>
        /// <param name="paymentCode">string paymentCode</param>
        /// <returns>All the information of PaymentSettings in the form of ZNodePaymentSetting model</returns>
        public PaymentSettingsModel GetPaymentSettingWithCredentials(string paymentCode)
        {
            PaymentSettingsModel paymentSettingDetails = null;
            

            paymentSettingDetails = (from paymentsetting in _paymentSettingRepository.Table
                                     join paymentCredential in _paymentSettingCredentialRepository.Table on paymentsetting.PaymentSettingId equals paymentCredential.PaymentSettingId
                                     join paymentType in _paymentTypeRepository.Table on paymentsetting.PaymentTypeId equals paymentType.PaymentTypeId
                                     join paymentGateway in _paymentGateway.Table on paymentsetting.PaymentGatewayId equals paymentGateway.PaymentGatewayId into tempMap
                                     from paymentGateway in tempMap.DefaultIfEmpty()
                                     where paymentsetting.TestMode == paymentCredential.TestMode && paymentsetting.PaymentCode.ToLower() == paymentCode.ToLower()
                                     select new PaymentSettingsModel
                                     {
                                         PaymentSettingId = paymentsetting.PaymentSettingId,
                                         TestMode = paymentCredential.TestMode,
                                         Partner = paymentCredential.Partner,
                                         GatewayPassword = paymentCredential.GatewayPassword,
                                         GatewayUsername = paymentCredential.GatewayUsername,
                                         Vendor = paymentCredential.Vendor,
                                         TransactionKey = paymentCredential.TransactionKey,
                                         IsActive = paymentsetting.IsActive,
                                         DisplayOrder = paymentsetting.DisplayOrder,
                                         EnableAmex = paymentsetting.EnableAmex,
                                         EnableDiscover = paymentsetting.EnableDiscover,
                                         EnableMasterCard = paymentsetting.EnableMasterCard,
                                         EnableRecurringPayments = paymentsetting.EnableRecurringPayments,
                                         EnableVault = paymentsetting.EnableVault,
                                         EnableVisa = paymentsetting.EnableVisa,
                                         PaymentGatewayId = paymentsetting.PaymentGatewayId,
                                         IsRMACompatible = paymentsetting.IsRMACompatible,
                                         PreAuthorize = paymentsetting.PreAuthorize,
                                         PaymentTypeId = paymentsetting.PaymentTypeId,
                                         PaymentCode = paymentsetting.PaymentCode,
                                         GatewayCode = paymentGateway.GatewayCode,
                                         PaymentTypeCode = paymentType.BehaviorType,
                                         Custom1 = paymentCredential.Custom1,
                                         Custom2 = paymentCredential.Custom2,
                                         Custom3 = paymentCredential.Custom3,
                                         Custom4 = paymentCredential.Custom4,
                                         Custom5 = paymentCredential.Custom5
                                     }).FirstOrDefault();

            if (!Equals(paymentSettingDetails, null))
                return DecryptPaymentSettings(paymentSettingDetails);
            return null;
        }

        /// <summary>
        /// Get Payment settings along with PaymentSetting Credentials by payment settings id
        /// </summary>
        /// <param name="paymentSettingId">int payment settings id</param>
        /// <returns>All the information of PaymentSettings in the form of ZNodePaymentSetting model</returns>       
        [Obsolete("This method is deprecated, please use 'GetPaymentSettingWithCredentials(string paymentCode)' instead.")]
        public PaymentSettingsModel GetPaymentSettingWithCredentials(int paymentSettingId)
        {
            PaymentSettingsModel paymentSettingDetails = null;
            ZNodePaymentSetting znodePaymentSetting = _paymentSettingRepository.Table.FirstOrDefault(x => x.PaymentSettingId == paymentSettingId);

            //1 and 3 are Creditcard and Paypal Express respectively for which credentials have to be saved
            if (znodePaymentSetting?.PaymentTypeId == 1 || znodePaymentSetting?.PaymentTypeId == 3)
            {
                paymentSettingDetails = (from paymentsetting in _paymentSettingRepository.Table
                                         join paymentCredential in _paymentSettingCredentialRepository.Table on paymentsetting.PaymentSettingId equals paymentCredential.PaymentSettingId
                                         join paymentGateway in _paymentGateway.Table on paymentsetting.PaymentGatewayId equals paymentGateway.PaymentGatewayId
                                         where paymentsetting.TestMode == paymentCredential.TestMode && paymentsetting.PaymentSettingId == paymentSettingId
                                         select new PaymentSettingsModel
                                         {
                                             PaymentSettingId = paymentsetting.PaymentSettingId,
                                             TestMode = paymentCredential.TestMode,
                                             Partner = paymentCredential.Partner,
                                             GatewayPassword = paymentCredential.GatewayPassword,
                                             GatewayUsername = paymentCredential.GatewayUsername,
                                             Vendor = paymentCredential.Vendor,
                                             TransactionKey = paymentCredential.TransactionKey,
                                             IsActive = paymentsetting.IsActive,
                                             DisplayOrder = paymentsetting.DisplayOrder,
                                             EnableAmex = paymentsetting.EnableAmex,
                                             EnableDiscover = paymentsetting.EnableDiscover,
                                             EnableMasterCard = paymentsetting.EnableMasterCard,
                                             EnableRecurringPayments = paymentsetting.EnableRecurringPayments,
                                             EnableVault = paymentsetting.EnableVault,
                                             EnableVisa = paymentsetting.EnableVisa,
                                             PaymentGatewayId = paymentsetting.PaymentGatewayId,
                                             IsRMACompatible = paymentsetting.IsRMACompatible,
                                             PreAuthorize = paymentsetting.PreAuthorize,
                                             PaymentTypeId = paymentsetting.PaymentTypeId,
                                             PaymentCode = paymentsetting.PaymentCode,
                                             Custom1 = paymentCredential.Custom1,
                                             Custom2 = paymentCredential.Custom2,
                                             Custom3 = paymentCredential.Custom3,
                                             Custom4 = paymentCredential.Custom4,
                                             Custom5 = paymentCredential.Custom5,
                                             GatewayCode=paymentGateway.GatewayCode
                                         }).FirstOrDefault();
            }
            else
            {
                paymentSettingDetails = Equals(znodePaymentSetting, null) ? null :
                                         new PaymentSettingsModel
                                         {
                                             PaymentSettingId = znodePaymentSetting.PaymentSettingId,
                                             PaymentTypeId = znodePaymentSetting.PaymentTypeId,
                                             IsActive = znodePaymentSetting.IsActive,
                                             PaymentGatewayId = znodePaymentSetting.PaymentGatewayId,
                                             DisplayOrder = znodePaymentSetting.DisplayOrder,
                                             EnableAmex = znodePaymentSetting.EnableAmex,
                                             EnableDiscover = znodePaymentSetting.EnableDiscover,
                                             EnableMasterCard = znodePaymentSetting.EnableMasterCard,
                                             EnableRecurringPayments = znodePaymentSetting.EnableRecurringPayments,
                                             EnableVault = znodePaymentSetting.EnableVault,
                                             EnableVisa = znodePaymentSetting.EnableVisa,
                                             IsRMACompatible = znodePaymentSetting.IsRMACompatible,
                                             PreAuthorize = znodePaymentSetting.PreAuthorize,
                                             TestMode = znodePaymentSetting.TestMode,
                                             EnablePODocUpload = znodePaymentSetting.EnablePODocUpload,
                                             IsPODocRequired = znodePaymentSetting.IsPODocRequired,
                                             PaymentCode = znodePaymentSetting.PaymentCode
                                         };
            }
            if (!Equals(paymentSettingDetails, null))
                return DecryptPaymentSettings(paymentSettingDetails);
            return null;
        }


        /// <summary>
        /// Get Payment settings along with PaymentSetting Credentials by payment settings and profile id
        /// </summary>
        /// <param name="gatewayTypeId">int gatewayTypeId</param>
        /// <param name="profileId">nullable int profile id</param>
        /// <param name="paymentSettingId">nullable int payment setting id</param>
        /// <returns>PaymentSettingsModel</returns>
        public PaymentSettingsModel GetPaymentSettingWithCredentials(int gatewayTypeId, int? profileId, int paymentSettingId)
        {
            var znodePaymentSetting = (from paymentSetting in _paymentSettingRepository.Table
                                       join paymentCredential in _paymentSettingCredentialRepository.Table on paymentSetting.PaymentSettingId equals paymentCredential.PaymentSettingId
                                       where paymentSetting.TestMode == paymentCredential.TestMode
                                       && paymentSetting.PaymentGatewayId == gatewayTypeId && paymentSetting.IsActive == true && paymentSetting.PaymentSettingId == paymentSettingId
                                       select new PaymentSettingsModel
                                       {
                                           PaymentSettingId = paymentSetting.PaymentSettingId,
                                           TestMode = paymentCredential.TestMode,
                                           Partner = paymentCredential.Partner,
                                           GatewayPassword = paymentCredential.GatewayPassword,
                                           GatewayUsername = paymentCredential.GatewayUsername,
                                           Vendor = paymentCredential.Vendor,
                                           TransactionKey = paymentCredential.TransactionKey,
                                           IsActive = paymentSetting.IsActive,
                                           DisplayOrder = paymentSetting.DisplayOrder,
                                           EnableAmex = paymentSetting.EnableAmex,
                                           EnableDiscover = paymentSetting.EnableDiscover,
                                           EnableMasterCard = paymentSetting.EnableMasterCard,
                                           EnableRecurringPayments = paymentSetting.EnableRecurringPayments,
                                           EnableVault = paymentSetting.EnableVault,
                                           EnableVisa = paymentSetting.EnableVisa,
                                           PaymentGatewayId = paymentSetting.PaymentGatewayId,
                                           IsRMACompatible = paymentSetting.IsRMACompatible,
                                           PreAuthorize = paymentSetting.PreAuthorize,
                                           PaymentTypeId = paymentSetting.PaymentTypeId,
                                           Custom1 = paymentCredential.Custom1,
                                           Custom2 = paymentCredential.Custom2,
                                           Custom3 = paymentCredential.Custom3,
                                           Custom4 = paymentCredential.Custom4,
                                           Custom5 = paymentCredential.Custom5
                                       }).FirstOrDefault();
            if (!Equals(znodePaymentSetting, null))
                return DecryptPaymentSettings(znodePaymentSetting);
            return null;
        }

        /// <summary>
        /// Get payment setting with credentials by using paymentCode & gatewayCode
        /// </summary>
        /// <param name="paymentCode">string paymentCode</param>
        /// <param name="gatewayCode">string gatewayCode</param>
        /// <returns>PaymentSettingsModel</returns>
        public PaymentSettingsModel GetPaymentSettingWithCredentials(string paymentCode, string gatewayCode)
        {
            var znodePaymentSetting = (from paymentSetting in _paymentSettingRepository.Table
                                       join paymentCredential in _paymentSettingCredentialRepository.Table on paymentSetting.PaymentSettingId equals paymentCredential.PaymentSettingId
                                       join paymentGateway in _paymentGateway.Table on paymentSetting.PaymentGatewayId equals paymentGateway.PaymentGatewayId
                                       where paymentSetting.TestMode == paymentCredential.TestMode
                                       && paymentSetting.PaymentCode.ToLower() == paymentCode.ToLower() && paymentSetting.IsActive == true
                                       && paymentGateway.GatewayCode.ToLower() == gatewayCode.ToLower()
                                       select new PaymentSettingsModel
                                       {
                                           PaymentSettingId = paymentSetting.PaymentSettingId,
                                           TestMode = paymentCredential.TestMode,
                                           Partner = paymentCredential.Partner,
                                           GatewayPassword = paymentCredential.GatewayPassword,
                                           GatewayUsername = paymentCredential.GatewayUsername,
                                           Vendor = paymentCredential.Vendor,
                                           TransactionKey = paymentCredential.TransactionKey,
                                           IsActive = paymentSetting.IsActive,
                                           DisplayOrder = paymentSetting.DisplayOrder,
                                           EnableAmex = paymentSetting.EnableAmex,
                                           EnableDiscover = paymentSetting.EnableDiscover,
                                           EnableMasterCard = paymentSetting.EnableMasterCard,
                                           EnableRecurringPayments = paymentSetting.EnableRecurringPayments,
                                           EnableVault = paymentSetting.EnableVault,
                                           EnableVisa = paymentSetting.EnableVisa,
                                           PaymentGatewayId = paymentSetting.PaymentGatewayId,
                                           IsRMACompatible = paymentSetting.IsRMACompatible,
                                           PreAuthorize = paymentSetting.PreAuthorize,
                                           PaymentTypeId = paymentSetting.PaymentTypeId,
                                           PaymentCode = paymentSetting.PaymentCode,
                                           Custom1 = paymentCredential.Custom1,
                                           Custom2 = paymentCredential.Custom2,
                                           Custom3 = paymentCredential.Custom3,
                                           Custom4 = paymentCredential.Custom4,
                                           Custom5 = paymentCredential.Custom5
                                       }).FirstOrDefault();
            if (!Equals(znodePaymentSetting, null))
                return DecryptPaymentSettings(znodePaymentSetting);
            return null;
        }


        /// <summary>
        /// Check Payment Setting is used or not
        /// </summary>
        /// <param name="paymentSettingId">>Payment Setting ID</param>
        /// <returns>true if Payment Setting used or not else return false</returns>
        public bool IsPaymentSettingUsed(string paymentSettingIds)
        {
            if (String.IsNullOrEmpty(paymentSettingIds))
                throw new Exception("payment Setting Ids Can not be null");

            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeTransactionEnum.PaymentSettingId.ToString(), EntityFilterOperators.In, paymentSettingIds);

            //hardcoded sort is specified as the primary key of table is GUID and Exception occurs if  sort by GUID
            return _transactionsRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause, $"{ZnodeTransactionEnum.PaymentSettingId} {"asc"}", null, null)?.Count > 0;
        }

        /// <summary>
        /// Get payment settings along with credentials by paymentCode
        /// </summary>
        /// <param name="paymentCode"></param>
        /// <returns>returns Payment Settings Model</returns>
        public PaymentSettingsModel GetPaymentSettingsByPaymentCode(string paymentCode)
        {
            if (!string.IsNullOrEmpty(paymentCode))
                return GetPaymentSettingWithCredentials(paymentCode);

            return null;
        }

        /// <summary>
        /// to check paymentCode is in use
        /// </summary>
        /// <param name="paymentCode">Comma separated paymentCodes</param>
        /// <returns>retrun true if record exist in transaction table else false</returns>
        public bool IsPaymentCodeUsed(string paymentCode)
        {
            if (String.IsNullOrEmpty(paymentCode))
                throw new Exception("payment Code Can not be null");

            string[] paymentCodeList = paymentCode.ToLower().Split(',');

            List<Int32> paymenttransaction = (from _setting in _paymentSettingRepository.Table
                                              join _transaction in _transactionsRepository.Table on _setting.PaymentSettingId equals _transaction.PaymentSettingId
                                              where paymentCodeList.Contains(_setting.PaymentCode.ToLower())
                                              select (_setting.PaymentSettingId)).ToList();

            return paymenttransaction?.Count > 0;
        }

        /// <summary>
        /// Delete the payment settings based on the paymentCode.
        /// </summary>
        /// <param name="paymentCode">string paymentCode</param>
        /// <returns>True if record deleted</returns>
        public bool DeletePaymentSettingByPaymentCode(string paymentCode)
        {
            if (String.IsNullOrEmpty(paymentCode))
                throw new Exception("payment Code Can not be null");

            string[] paymentCodeList = paymentCode.ToLower().Split(',');

            var paymentSetting = _paymentSettingRepository.Table.Where(x => paymentCodeList.Contains(x.PaymentCode.ToLower())).Select(x => x.PaymentSettingId).ToList();

            string paymentSettingIds = string.Join(",", paymentSetting);

            return DeletePaymentSetting(paymentSettingIds);
        }


        /// <summary>
        /// To get payment setting Id by payment code
        /// </summary>
        /// <param name="paymentCode">string paymentCode</param>
        /// <returns>int PaymentSettingId</returns>
        public int GetPaymentSettingIdByCode(string paymentCode)
        => _paymentSettingRepository.Table.Where(x => x.PaymentCode.ToLower().Trim() == paymentCode.ToLower().Trim())?.Select(x => x.PaymentSettingId)?.FirstOrDefault() ?? 0;

        #endregion

        #region Private Methods
        // Decrypt the Payment settings important fields for calling
        private PaymentSettingsModel DecryptPaymentSettings(PaymentSettingsModel paymentSetting)
        {
            paymentSetting.GatewayPassword = Decrypt(paymentSetting.GatewayPassword);
            paymentSetting.GatewayUsername = Decrypt(paymentSetting.GatewayUsername);
            paymentSetting.TransactionKey = Decrypt(paymentSetting.TransactionKey);
            paymentSetting.Vendor = Decrypt(paymentSetting.Vendor);
            paymentSetting.Partner = Decrypt(paymentSetting.Partner);
            return paymentSetting;
        }

        // Update the Paymentech URLs depending upon test/prod mode 
        private void UpdatePaymentechURLs(int paymentGatewayId, bool isTestMode)
        {
            ZNodePaymentGateway gatewayDetails = _paymentGateway.GetById(paymentGatewayId);
            if (gatewayDetails?.GatewayName?.ToLower().Contains(Convert.ToString(GatewayType.PAYMENTECH).ToLower()) ?? false)
                PaymentechHelper.ReplacePaymentechURLs(isTestMode);
        }

        /// <summary>
        /// Get update refund transaction id.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public string GetAmazonUpdateTransactionId(string transactionId)
            => Convert.ToString(_transactionsRepository.Table.Where(w => w.TransactionId == transactionId)?.Select(x => x.RefundTransactionId)?.FirstOrDefault() ?? transactionId);

        //Set payment type Id and gateway Id by paymentcode and gatewaycode
        private void SetPaymentTypeAndGatewayIdByCode(PaymentSettingsModel paymentSetting)
        {
            if (paymentSetting != null && !string.IsNullOrEmpty(paymentSetting.PaymentTypeCode))
            {
                paymentSetting.PaymentTypeId = _paymentTypeRepository.Table.Where(x => x.Code.ToLower() == paymentSetting.PaymentTypeCode.ToLower()).Select(x=>x.PaymentTypeId).FirstOrDefault();

                if (string.Equals(paymentSetting.PaymentTypeCode, "paypalexpress", StringComparison.InvariantCultureIgnoreCase))
                    paymentSetting.PaymentGatewayId = _paymentGateway.Table.Where(x => x.GatewayCode.ToLower() == paymentSetting.PaymentTypeCode.ToLower()).FirstOrDefault()?.PaymentGatewayId;
                
                if (!string.IsNullOrEmpty(paymentSetting.GatewayCode))
                    paymentSetting.PaymentGatewayId = _paymentGateway.Table.Where(x => x.GatewayCode.ToLower() == paymentSetting.GatewayCode.ToLower()).Select(x => x.PaymentGatewayId).FirstOrDefault();
            }
        }

        #endregion
    }
}
