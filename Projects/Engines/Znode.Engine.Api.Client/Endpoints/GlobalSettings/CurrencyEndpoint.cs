namespace Znode.Engine.Api.Client.Endpoints
{
    public class CurrencyEndpoint : BaseEndpoint
    {
        //Get currency list endpoint
        public static string GetCurrencyList() => $"{ApiRoot}/currency/list";

        //Get currency endpoint
        public static string GetCurrency() => $"{ApiRoot}/currency";

        //Update currency endpoint
        public static string UpdateCurrency() => $"{ApiRoot}/currency/update";

        //Get culture list endpoint
        public static string GetCultureList() => $"{ApiRoot}/currency/getculturelist";

        //Get culture endpoint
        public static string GetCultureCode() => $"{ApiRoot}/currency/getculturecode";
        
        public static string GetCurrencyCultureList() => $"{ApiRoot}/currency/getcurrencyculturelist";
    }
}
