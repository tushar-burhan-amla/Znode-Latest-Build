using System;
using System.Linq;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Data
{
    public class PaymentSettingCredentialsService : BaseService
    {

        #region Private Variables
        private readonly IZnodePaymentRepository<ZNodePaymentSetting> _paymentSettingRepository;
        private readonly IZnodePaymentRepository<ZNodePaymentSettingCredential> _paymentSettingCredentialRepository;
        #endregion

        #region Constructor
        public PaymentSettingCredentialsService()
        {
            _paymentSettingRepository = new ZnodePaymentRepository<ZNodePaymentSetting>();
            _paymentSettingCredentialRepository = new ZnodePaymentRepository<ZNodePaymentSettingCredential>();
        }
        #endregion

        /// <summary>
        /// Save Payment Setting Credentials
        /// </summary>
        /// <param name="paymentSettingCredential">ZNodePaymentSettingCredential paymentSettingCredential</param>
        /// <returns>Returns true if saved successfully else return false</returns>
        public bool AddPaymentSettingCredential(ZNodePaymentSettingCredential paymentSettingCredential)
            => _paymentSettingCredentialRepository.Insert(EncryptPaymentSettingsCredential(paymentSettingCredential)).PaymentSettingCredentialId > 0;

        /// <summary>
        /// Update Payment Setting Credentials
        /// </summary>
        /// <param name="paymentSettingCredential">ZNodePaymentSettingCredential paymentSettingCredential</param>
        /// <returns>Returns true if updated successfully else return false</returns>
        public bool UpdatePaymentSettingCredential(ZNodePaymentSettingCredential paymentSettingCredential)
        {
            if (Equals(paymentSettingCredential, null)) return false;
            ZNodePaymentSettingCredential PaymentSettingCredentialEntity = _paymentSettingCredentialRepository.Table.FirstOrDefault(payment =>
                                                       payment.PaymentSettingId == paymentSettingCredential.PaymentSettingId
                                                       && payment.TestMode.Equals(paymentSettingCredential.TestMode));
            if (!Equals(PaymentSettingCredentialEntity, null))
            {
                ZNodePaymentSettingCredential encryptedCredentials = EncryptPaymentSettingsCredential(paymentSettingCredential);
                PaymentSettingCredentialEntity.GatewayPassword = encryptedCredentials.GatewayPassword;
                PaymentSettingCredentialEntity.TransactionKey = encryptedCredentials.TransactionKey;
                PaymentSettingCredentialEntity.Vendor = encryptedCredentials.Vendor;
                PaymentSettingCredentialEntity.Partner = encryptedCredentials.Partner;
                PaymentSettingCredentialEntity.GatewayUsername = encryptedCredentials.GatewayUsername;

                return _paymentSettingCredentialRepository.Update(PaymentSettingCredentialEntity);
            }
            else
            {
                paymentSettingCredential = EncryptPaymentSettingsCredential(paymentSettingCredential);
                return _paymentSettingCredentialRepository.Insert(paymentSettingCredential).PaymentSettingCredentialId > 0;
            }
        }

        /// <summary>
        /// Get Payment Setting Credentials
        /// <param name="paymentSettingId">Id of payment Setting.</param>
        /// <param name="isTestMode">true to get Test Mode credentials else set false.</param>
        /// <returns>ZNodePayment Setting Credential</returns>
        [Obsolete("This method is deprecated, please use 'GetPaymentSettingCredentials(string paymentCode, bool isTestMode)' instead.")]
        public PaymentSettingCredentialsModel GetPaymentSettingCredentials(int paymentSettingId, bool isTestMode)
        {
            ZNodePaymentSettingCredential paymentSetting = _paymentSettingCredentialRepository.Table.FirstOrDefault(x => x.PaymentSettingId == paymentSettingId
                                                             && x.TestMode == isTestMode);
            if (!Equals(paymentSetting, null))
            {
                return DecryptPaymentSettingsCredential(paymentSetting).ToModel<PaymentSettingCredentialsModel>();
            }
            return null;
        }

        /// <summary>
        /// Get Payment Setting Credentials
        /// <param name="paymentCode">Code of payment Setting.</param>
        /// <param name="isTestMode">true to get Test Mode credentials else set false.</param>
        /// <returns>ZNodePayment Setting Credential</returns>
        public PaymentSettingCredentialsModel GetPaymentSettingCredentials(string paymentCode, bool? isTestMode = null)
        {
            int? paymentSettingId = 0;
            if (!isTestMode.HasValue)
            {
                ZNodePaymentSetting znodePaymentSetting = _paymentSettingRepository.Table.Where(x => x.PaymentCode.ToLower() == paymentCode.ToLower())?.FirstOrDefault();
                paymentSettingId = znodePaymentSetting?.PaymentSettingId != null ? znodePaymentSetting?.PaymentSettingId  : 0;
                isTestMode = znodePaymentSetting?.TestMode;
            }
            else
            {
                paymentSettingId = _paymentSettingRepository.Table.Where(x => x.PaymentCode.ToLower() == paymentCode.ToLower())?.Select(x => x.PaymentSettingId)?.FirstOrDefault() ?? 0;
            }
            ZNodePaymentSettingCredential paymentSetting = (paymentSettingId > 0) ? _paymentSettingCredentialRepository.Table.FirstOrDefault(x => x.PaymentSettingId == paymentSettingId
                                                             && x.TestMode == isTestMode) : null;
            if (!Equals(paymentSetting, null))
            {
                return DecryptPaymentSettingsCredential(paymentSetting).ToModel<PaymentSettingCredentialsModel>();
            }
            return null;
        }

        /// <summary>
        /// Encrypt the important payment fields of ZNodePaymentSetting models
        /// </summary>
        /// <param name="paymentSetting">ZNodePaymentSetting model data</param>
        /// <returns>Encrypted data in the form of ZNodePaymentSetting model</returns>
        private ZNodePaymentSettingCredential EncryptPaymentSettingsCredential(ZNodePaymentSettingCredential paymentSettingCredential)
        {
            paymentSettingCredential.GatewayPassword = Encrypt(paymentSettingCredential.GatewayPassword);
            paymentSettingCredential.GatewayUsername = Encrypt(paymentSettingCredential.GatewayUsername);
            paymentSettingCredential.TransactionKey = Encrypt(paymentSettingCredential.TransactionKey);
            paymentSettingCredential.Vendor = Encrypt(paymentSettingCredential.Vendor);
            paymentSettingCredential.Partner = Encrypt(paymentSettingCredential.Partner);
            return paymentSettingCredential;
        }

        /// <summary>
        /// Decrypt the important payment fields of ZNodePaymentSetting models
        /// </summary>
        /// <param name="paymentSetting">ZNodePaymentSetting model data</param>
        /// <returns>Decrypted data in the form of Znode PaymentSetting Credential</returns>
        private ZNodePaymentSettingCredential DecryptPaymentSettingsCredential(ZNodePaymentSettingCredential paymentSettingCredential)
        {
            paymentSettingCredential.GatewayPassword = Decrypt(paymentSettingCredential.GatewayPassword);
            paymentSettingCredential.GatewayUsername = Decrypt(paymentSettingCredential.GatewayUsername);
            paymentSettingCredential.TransactionKey = Decrypt(paymentSettingCredential.TransactionKey);
            paymentSettingCredential.Vendor = Decrypt(paymentSettingCredential.Vendor);
            paymentSettingCredential.Partner = Decrypt(paymentSettingCredential.Partner);
            return paymentSettingCredential;
        }
    }
}
