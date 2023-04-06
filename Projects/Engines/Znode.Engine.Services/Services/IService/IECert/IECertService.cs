using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IECertService
    {
        /// <summary>
        /// Get available eCertificate balance.
        /// </summary>
        /// <param name="eCertTotalModel">Object of ECertTotalModel to get the total</param>
        /// <returns></returns>
        ECertTotalBalanceModel GetAvailableECertBalance(ECertTotalModel eCertTotalModel);


        /// <summary>
        /// Get list of certificates associated to logged in user.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <returns>ECertificateListModel</returns>
        ECertificateListModel GetWalletECertificatesList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Add eCertificate balance to the wallet.
        /// </summary>
        /// <param name="eCertificateModel">eCertificate that needs to be inserted</param>
        /// <returns>Inserted eCertificate</returns>
        ECertificateModel AddECertificateToWallet(ECertificateModel eCertificateModel);

    }
}
