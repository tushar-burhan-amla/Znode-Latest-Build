using System.Linq;

namespace Znode.Multifront.PaymentApplication.Data
{
    public class DomainService : BaseService
    {
        /// <summary>
        /// Get Domains by Domain name
        /// </summary>
        /// <param name="domainName">domain name</param>
        /// <returns>ZNodeDomain</returns>
        public ZNodeDomain GetDomain(string domainName)
        => new ZnodePaymentRepository<ZNodeDomain>().Table.FirstOrDefault(x => x.DomainName == domainName);

    }
}