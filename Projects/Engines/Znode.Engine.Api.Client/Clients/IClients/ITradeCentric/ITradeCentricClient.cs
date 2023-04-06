using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ITradeCentricClient : IBaseClient
    {
        /// <summary>
        ///Get tradecentric user details.
        /// </summary>
        /// <param name="userId">userId</param>       
        /// <returns>Tradecentric user details</returns>
        TradeCentricUserModel GetTradeCentricUser(int userId);

        /// <summary>
        ///Save tradecentric user details.
        /// </summary>
        /// <param name="tradeCentricUserModel">tradeCentricUserModel</param>
        /// <returns>Result</returns>
        bool SaveTradeCentricUser(TradeCentricUserModel tradeCentricUserModel);

        /// <summary>
        /// Transfer cart.
        /// </summary>
        /// <param name="tradeCentricUserModel">tradeCentricUserModel</param>
        /// <returns>redirection url</returns>
        string TransferCart(TradeCentricUserModel tradeCentricUserModel);

    }
}
