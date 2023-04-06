using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Data
{
    public class TransactionService : BaseService
    {

        #region Private Variables
        private readonly IZnodePaymentRepository<ZnodeTransaction> _transactionsRepository;
        #endregion

        #region Constructor
        public TransactionService()
        {
            _transactionsRepository = new ZnodePaymentRepository<ZnodeTransaction>();
        }
        #endregion

        /// <summary>
        /// Add payment transactions to DB
        /// </summary>
        /// <param name="paymentModel">Payment Model</param>
        /// <returns>transaction Guid</returns>
        public string AddPayment(PaymentModel paymentModel)
        {
            try
            {
                ZnodeTransaction transactions = new ZnodeTransaction();
                int paymentSettingId = GetPaymentSettingId(paymentModel);
                if (string.IsNullOrEmpty(paymentModel.CustomerProfileId))
                {
                    transactions = new ZnodeTransaction
                    {
                        GUID = Guid.NewGuid(),
                        TransactionDate = paymentModel.TransactionDate != null? paymentModel.TransactionDate : DateTime.Now,
                        TransactionId = paymentModel.TransactionId,
                        ResponseText = paymentModel.ResponseText,
                        ResponseCode = paymentModel.ResponseCode,
                        Amount = Math.Round(decimal.Parse(paymentModel.Total), 2), //decimal.Parse(paymentModel.Total),
                        PaymentSettingId = paymentSettingId,
                        CurrencyCode = paymentModel.GatewayCurrencyCode,
                        Custom1 = paymentModel.PaymentCode?.ToLower() == "amazonpay" ? paymentModel.CardDataToken : (!string.IsNullOrEmpty(paymentModel.CardDataToken) ? paymentModel.CardDataToken : paymentModel.OrderId),
                        PaymentStatusId = paymentModel.PaymentStatusId
                    };
                }
                else
                {
                    transactions = new ZnodeTransaction
                    {
                        GUID = Guid.NewGuid(),
                        CustomerProfileId = EncryptPaymentToken(paymentModel.CustomerProfileId),
                        CustomerPaymentId = (!string.IsNullOrEmpty(paymentModel.CustomerPaymentProfileId)) ? EncryptPaymentToken(paymentModel.CustomerPaymentProfileId) : string.Empty,
                        TransactionDate = paymentModel.TransactionDate != null ? paymentModel.TransactionDate : DateTime.Now,
                        TransactionId = paymentModel.TransactionId,
                        ResponseText = paymentModel.ResponseText,
                        ResponseCode = paymentModel.ResponseCode,
                        Amount = Math.Round(decimal.Parse(paymentModel.Total), 2),
                        PaymentSettingId = paymentSettingId,
                        CurrencyCode = paymentModel.GatewayCurrencyCode,
                        Custom1 = paymentModel.PaymentCode?.ToLower() == "amazonpay" ? paymentModel.CardDataToken :  paymentModel.OrderId,
                        PaymentStatusId = paymentModel.PaymentStatusId
                    };
                }

                return _transactionsRepository.Insert(transactions)?.GUID.ToString();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                var data = string.Format("TransactionId : {0}, CustomerProfileId: {1}, CustomerPaymentProfile : {2}", paymentModel.TransactionId, paymentModel.CustomerProfileId, paymentModel.CustomerPaymentProfileId);

                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, exceptionMessage, data);
                Logging.LogMessage(exceptionMessage, Logging.Components.Payment.ToString(), TraceLevel.Error);

            }
            return null;
        }

        /// <summary>
        ///  Update payment transactions to DB
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public string UpdatePayment(PaymentModel paymentModel)
        {
            try
            {
                ZnodeTransaction transactions = _transactionsRepository.Table.FirstOrDefault(payment => payment.GUID.ToString().ToLower().Equals(paymentModel.GUID.ToLower()));
                if (paymentModel.Subscriptions.Any())
                    transactions.SubscriptionId = transactions.SubscriptionId + "," + paymentModel.TransactionId;
                else
                    if (paymentModel.PaymentStatusId <= 1)
                    transactions.TransactionId = paymentModel.TransactionId;

                transactions.ResponseText = paymentModel.ResponseText;
                transactions.ResponseCode = paymentModel.ResponseCode;
                transactions.PaymentStatusId = paymentModel.PaymentStatusId;


                //If it is refund transaction then update refund date
                if (Equals(paymentModel.PaymentStatusId, 3))
                {
                    transactions.RefundTransactionDate = DateTime.Now;
                    transactions.RefundTransactionId = paymentModel.RefundTransactionId;
                    transactions.RefundAmount = (transactions.RefundAmount ?? 0.00m) + paymentModel.RefundAmount;
                }

                //If it is captured transaction then update captured date
                if (Equals(paymentModel.PaymentStatusId, 1))
                {
                    transactions.CaptureTransactionDate = DateTime.Now;
                    transactions.CaptureTransactionId = paymentModel.CaptureTransactionId;
                }
                transactions.Custom1 = paymentModel.PaymentCode?.ToLower() == "amazonpay" ? paymentModel.CardDataToken : (!string.IsNullOrEmpty(paymentModel.CardDataToken) ? paymentModel.CardDataToken : paymentModel.OrderId);
                _transactionsRepository.Update(transactions);
                return transactions.GUID.ToString();
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                return null;
            }
        }
        /// <summary>
        ///  Get payment transactions from DB
        /// </summary>
        /// <param name="transactionId">transactionId</param>
        /// <returns></returns>
        public ZnodeTransaction GetPayment(string transactionId)
        {
            try
            {
                IQueryable<ZnodeTransaction> paymentTransaction = from transaction in _transactionsRepository.Table
                                                                  where transaction.TransactionId.ToString().ToLower() == transactionId.ToLower()
                                                                  select transaction;

                if (paymentTransaction.Any())
                {
                    ZnodeTransaction znodeTransaction = paymentTransaction.FirstOrDefault();
                    znodeTransaction.CustomerProfileId = (!string.IsNullOrEmpty(znodeTransaction.CustomerProfileId)) ? DecryptPaymentToken(znodeTransaction.CustomerProfileId) : string.Empty;
                    znodeTransaction.CustomerPaymentId = (!string.IsNullOrEmpty(znodeTransaction.CustomerPaymentId)) ? DecryptPaymentToken(znodeTransaction.CustomerPaymentId) : string.Empty;
                    return znodeTransaction;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(null, ex.Message);
                return null;
            }
            return null;
        }

        /// <summary>
        /// Checks if customerProfileId and CustomerPaymentProfileId related record is presend in Transactions
        /// </summary>
        /// <param name="customerProfileId">string customerProfileId</param>
        /// <param name="CustomerPaymentProfileId">string CustomerPaymentProfileId</param>
        /// <returns>returns True if record present else returns false</returns>
        public bool IsTransactionPresent(string customerProfileId, string CustomerPaymentProfileId)
        {
            try
            {
                customerProfileId = EncryptPaymentToken(customerProfileId);
                CustomerPaymentProfileId = (!string.IsNullOrEmpty(CustomerPaymentProfileId)) ? EncryptPaymentToken(CustomerPaymentProfileId) : string.Empty;

                var paymentTransaction = from transaction in _transactionsRepository.Table
                                         where transaction.CustomerProfileId == customerProfileId &&
                                         transaction.CustomerPaymentId == CustomerPaymentProfileId
                                         select transaction;

                if (paymentTransaction.Any())
                    return true;
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(null, ex.Message);
                return false;
            }
            return false;
        }

        /// <summary>
        ///  Get payment transaction by transactionId
        /// </summary>
        /// <param name="transactionId">transactionId</param>
        /// <returns>return payment transaction</returns>
        public PaymentTransactionModel GetPaymentTransaction(string transactionId)
        {
            try
            {
                ZnodeTransaction paymentTransaction = _transactionsRepository.Table.FirstOrDefault(x => x.TransactionId.ToLower() == transactionId.ToLower()) ?? null;
                if (!Equals(paymentTransaction, null))
                {
                    return paymentTransaction?.ToModel<PaymentTransactionModel>();
                }
                return null;

            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(null, ex.Message);
                return null;
            }
        }

        //to get payment setting Id
        public int GetPaymentSettingId(PaymentModel paymentModel)
        {
            int paymentSettingId = 0;
            if (paymentModel != null)
            {
                if (!string.IsNullOrEmpty(paymentModel.PaymentCode))
                {
                    PaymentSettingsService service = new PaymentSettingsService();
                    paymentSettingId = paymentModel.PaymentApplicationSettingId > 0 ? paymentModel.PaymentApplicationSettingId : service.GetPaymentSettingIdByCode(paymentModel.PaymentCode);
                }
                else
                {
                    paymentSettingId = paymentModel.PaymentApplicationSettingId;
                }
            }
            return paymentSettingId;
        }
    }
}
