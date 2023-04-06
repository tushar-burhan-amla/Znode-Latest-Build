using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface ITradeCentricAgent
    {
        /// <summary>
        /// This method return store URL so that user can access store if token is valid.
        /// </summary>
        /// <param name="tradeCentricSessionRequestViewModel">tradeCentricSessionRequestViewModel</param>
        /// <returns>return url with token in query string</returns>
        string GetUserStoreSessionUrl(TradeCentricSessionRequestViewModel tradeCentricSessionRequestViewModel);

        /// <summary>
        /// Validate login based on token key.
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>return login model if token is valid</returns>
        LoginViewModel ValidateLogin(string token);

        /// <summary>
        /// Save tradecentric user details.
        /// </summary>
        /// <param name="tradeCentricUserModel">tradeCentricUserModel</param>
        /// <returns>Result</returns>
        bool SaveTradeCentricUser(TradeCentricUserModel tradeCentricUserModel);

        /// <summary>
        /// To transfer cart to the Tradecentric Portal
        /// </summary>
        /// <param name="cartViewModel">cartViewModel</param>
        /// <returns>Transfer cart response</returns>
        string TransferCart(CartViewModel cartViewModel);

        /// <summary>
        /// Save tradecentric user details.
        /// </summary>
        /// <param name="tradeCentricOrderViewModel">tradeCentricOrderViewModel</param>
        /// <returns>Returns OrdersViewModel if order is successfully placed</returns>
        OrdersViewModel SubmitOrder(TradeCentricOrderViewModel tradeCentricOrderViewModel);

        /// <summary>
        /// This method is used to load cart of trade centric user
        /// </summary>
        /// <param name="operation">operation</param>
        /// <returns></returns>
        void LoadUserCart(string operation);
    }
}