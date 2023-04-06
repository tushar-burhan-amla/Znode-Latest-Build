using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Services
{
    public interface ITradeCentricService
    {
        /// <summary>
        /// Get the TradeCentric User.
        /// </summary>
        /// <param name="userId">User id of the TradeCentric User.</param>
        /// <returns>TradeCentricUserModel model</returns>
        TradeCentricUserModel GetTradeCentricUser(int userId);

        /// <summary>
        /// Save TradeCentric user details.
        /// </summary>
        /// <param name="tradeCentricUserModel"></param>
        /// <returns>True/False</returns>
        bool SaveTradeCentricUser(TradeCentricUserModel tradeCentricUserModel);

        /// <summary>
        /// Transfer Cart.
        /// </summary>
        /// <param name="tradeCentricUserModel">tradeCentricUserModel</param>
        /// <returns>TradeCentricCartTransferResponse model</returns>
        TradeCentricCartTransferResponse TransferTradeCentricCart(TradeCentricUserModel tradeCentricUserModel);

    }
}
