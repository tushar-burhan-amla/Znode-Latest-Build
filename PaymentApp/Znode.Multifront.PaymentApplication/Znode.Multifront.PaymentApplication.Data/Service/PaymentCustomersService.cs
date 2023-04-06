using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Znode.Multifront.PaymentApplication.Data.Maps;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Data
{
    public class PaymentCustomersService : BaseService
    {
        #region Private Variables
        private readonly IZnodePaymentRepository<ZnodePaymentCustomer> _paymentCustomerRepository;
        #endregion

        #region Constructor
        public PaymentCustomersService()
        {
            _paymentCustomerRepository = new ZnodePaymentRepository<ZnodePaymentCustomer>();
        }
        #endregion


        /// <summary>
        /// Add the payment Customers in payment api db.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns CustomersGUID</returns>
        public string AddPaymentCustomers(PaymentModel paymentModel)
        {
            try
            {
                ZnodePaymentCustomer paymentCustomers = _paymentCustomerRepository.Insert(PaymentCustomerMap.ToEntity(paymentModel));
                return paymentCustomers.CustomersGUID.ToString();
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
                string data = $"CustomersGUID : { paymentModel.CustomerGUID}";

                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, exceptionMessage, data);
                Logging.LogMessage(exceptionMessage, Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
            return null;
        }
    }
}