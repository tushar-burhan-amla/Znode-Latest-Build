using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public interface IECertAgent
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
        ECertificateListViewModel GetAvailableECertList(FilterCollectionDataModel filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Adds eCertificate to the wallet.
        /// </summary>
        /// <param name="eCertificateViewModel"></param>
        /// <returns></returns>
        ECertificateViewModel AddECertToBalance(ECertificateViewModel eCertificateViewModel);
    }
}
