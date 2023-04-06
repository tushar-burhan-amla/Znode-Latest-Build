using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IECertClient : IBaseClient
    {

        /// <summary>
        /// Get the list of all orders.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>ECertificateListModel.</returns>
        ECertificateListModel GetAvailableECertList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Adds eCertificate to the wallet.
        /// </summary>
        /// <param name="eCertificateModel"></param>
        /// <returns>inserted eCertificateModel</returns>
        ECertificateModel AddECertToBalance(ECertificateModel eCertificateModel);
    }
}
