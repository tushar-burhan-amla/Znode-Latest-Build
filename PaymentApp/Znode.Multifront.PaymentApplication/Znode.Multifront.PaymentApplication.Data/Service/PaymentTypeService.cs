using System.Linq;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Data
{
    public class PaymentTypeService : BaseService
    {
        #region Private Variables
        private readonly IZnodePaymentRepository<ZNodePaymentType> _paymentTypeRepository;
        private readonly IZnodePaymentRepository<ZNodePaymentSetting> _paymentSettingRepository;
        #endregion

        #region Constructor

        public PaymentTypeService()
        {
            _paymentTypeRepository = new ZnodePaymentRepository<ZNodePaymentType>();
            _paymentSettingRepository = new ZnodePaymentRepository<ZNodePaymentSetting>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get payment types
        /// </summary>
        /// <returns></returns>
        public PaymentTypeListModel GetAll()
            => new PaymentTypeListModel() { PaymentTypes = _paymentTypeRepository.Table?.ToModel<PaymentTypeModel>().ToList() };

        /// <summary>
        /// Get Payment type by payment type id
        /// </summary>
        /// <param name="paymentTypeId">int Payment Type Id</param>
        /// <returns>All the information of PaymentType in the form of ZNodePaymentType entity</returns>
        public ZNodePaymentType GetPaymentType(int paymentTypeId)
          => (from paymentTypes in _paymentTypeRepository.Table
              where paymentTypes.PaymentTypeId == paymentTypeId
              select paymentTypes).FirstOrDefault();

        /// <summary>
        /// Get the payment type using the payment settings id
        /// </summary>
        /// <param name="paymentSettingId">int Payment Setting Id</param>
        /// <returns>All the information of PaymentType in the form of ZNodePaymentType entity</returns>
        public ZNodePaymentType GetPaymentTypeByPaymentSettingId(int paymentSettingId)
        => (from paymentTypes in _paymentTypeRepository.Table
            join paymentSettings in _paymentSettingRepository.Table on paymentTypes.PaymentTypeId equals paymentSettings.PaymentTypeId
            where paymentSettings.PaymentSettingId == paymentSettingId
            select paymentTypes)?.FirstOrDefault();

        #endregion
    }
}
