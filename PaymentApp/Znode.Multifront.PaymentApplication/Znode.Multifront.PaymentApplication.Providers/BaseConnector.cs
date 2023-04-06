using StructureMap;
using System.Linq;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Models;
using Znode.Multifront.PaymentFramework.Bussiness;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public class BaseConnector : ZnodePaymentBusinessBase
    {
        protected IPaymentProviders GetProvider(string gateway)
        {
            if (!string.IsNullOrEmpty(gateway))
            {
                Container container = new Container(content =>
                content.Scan(scan =>
                {
                    scan.AssemblyContainingType<IPaymentProviders>();
                    scan.AddAllTypesOf<IPaymentProviders>();
                }));
                string gateWayName = container.Model.AllInstances.FirstOrDefault(x => x.Description.Contains(gateway)).Name;
                return !string.IsNullOrEmpty(gateway) ? container.GetInstance<IPaymentProviders>(gateWayName) : null;
            }
            return null;
        }

        protected string GetGatewayClassName(int gatewayId)
            => new GatewayService().GetGatewayClassNameById(gatewayId);

        protected string GetPaymentTypeClassName(int paymentSettingsId)
            => new PaymentTypeService().GetPaymentTypeByPaymentSettingId(paymentSettingsId).Name;

        protected string GetGatewayClassNameByGatewayCode(string gatewayCode)
    => new GatewayService().GetGatewayClassNameByCode(gatewayCode);
    }
}
