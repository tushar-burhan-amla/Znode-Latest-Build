using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Znode.Multifront.PaymentApplication.Data.Maps;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Data
{
    public class PaymentAddressService : BaseService
    {
        #region Private Variables
        private readonly IZnodePaymentRepository<ZnodePaymentAddress> _paymentAddressRepository;
        #endregion

        #region Constructor
        public PaymentAddressService()
        {
            _paymentAddressRepository = new ZnodePaymentRepository<ZnodePaymentAddress>();
        }
        #endregion

        /// <summary>
        ///  Add the payment Address in payment api db.
        /// </summary>
        /// <param name="paymentModel">payment Model</param>
        /// <returns>returns CreditCardAddressId</returns>
        public string AddPaymentAddress(PaymentModel paymentModel)
        {
            try
            {
                ZnodePaymentAddress paymentAddress = _paymentAddressRepository.Insert(PaymentAddressMap.ToEntity(paymentModel));
                return paymentAddress.CreditCardAddressId.ToString();
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
                string data = $"CreditCardAddressId : {paymentModel.AddressId}";

                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, exceptionMessage, data);
                Logging.LogMessage(exceptionMessage, Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
            return null;
        }
    }
}