using System.Linq;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Data
{
    public class GatewayService : BaseService
    {
        #region Private Variables
        private readonly IZnodePaymentRepository<ZNodePaymentGateway> _gatewayRepository;
        #endregion

        #region Constructor
        public GatewayService()
        {
            _gatewayRepository = new ZnodePaymentRepository<ZNodePaymentGateway>();
        }
        #endregion


        /// <summary>
        /// Get Gateway types
        /// </summary>
        /// <returns>PaymentGateway List Model</returns>
        public PaymentGatewayListModel GetAll()
        => new PaymentGatewayListModel { PaymentGateways = _gatewayRepository.Table?.ToModel<PaymentGatewayModel>()?.ToList() };

        /// <summary>
        ///  Get Gateway class name
        /// </summary>
        /// <param name="gateTypeId">ID to get a getway class name</param>
        /// <returns>Class name</returns>
        public string GetGatewayClassNameById(int gatewayTypeId)
        => _gatewayRepository.Table.Where(x => x.PaymentGatewayId == gatewayTypeId).Select(x=>x.ClassName).FirstOrDefault();

        /// <summary>
        ///  Get Gateway Code
        /// </summary>
        /// <param name="gatewayCode">Id to get gatewaycode by class name</param>
        /// <returns>GatewayCode</returns>
        public string GetGatewayClassNameByCode(string gatewayCode)
        => _gatewayRepository.Table.Where(x => x.GatewayCode == gatewayCode).Select(x => x.ClassName)?.FirstOrDefault();

        /// <summary>
        /// Get Gateway ACH types
        /// </summary>
        /// <returns>PaymentGateway List Model</returns>
        public PaymentGatewayListModel GetACHGateways()
        => new PaymentGatewayListModel { PaymentGateways = _gatewayRepository.Table?.Where(x => x.IsACHEnabled)?.ToModel<PaymentGatewayModel>()?.ToList() };

    }
}