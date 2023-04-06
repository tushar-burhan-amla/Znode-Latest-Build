namespace Znode.Engine.Api.Client.Endpoints
{
    //Configure the Endpoints used to create url's for User related API's.
    public class TradeCentricEndpoint : BaseEndpoint
    {
        //Get TradeCentric user.
        public static string GetTradeCentricUser(int userId) => $"{ApiRoot}/tradecentric/gettradecentricuser/{userId}";

        //Save TradeCentric user.
        public static string SaveTradeCentricUser() => $"{ApiRoot}/tradecentric/savetradecentricuser";

        //Transfer cart.
        public static string TransferCart() => $"{ApiRoot}/tradecentric/transfercart";

    }
}
