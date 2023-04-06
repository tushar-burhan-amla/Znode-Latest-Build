namespace Znode.Engine.Api.Client.Endpoints
{
    public class QuoteEndpoint : BaseEndpoint
    {    
        // Get quote list endpoint.
        public static string List() => $"{ApiRoot}/Quote/list";

        //Create new quote endpoint.
        public static string Create() => $"{ApiRoot}/quote/create";

        // Get Quote Receipt Details by order Id
        public static string GetQuoteReceipt(int quoteId) => $"{ApiRoot}/quote/getquotereceipt/{quoteId}";

        // Get Quote Receipt Details by Quote Number
        public static string GetQuoteByQuoteNumber(string quotenumber) => $"{ApiRoot}/quote/getquotebyquotenumber/{quotenumber}";

        // Get Quote details by QuoteId.
        public static string GetQuoteById(int omsQuoteId) => $"{ApiRoot}/quote/getquotebyid/{omsQuoteId}";

        //Convert to Order Endpoint.
        public static string ConvertQuoteToOrder() => $"{ApiRoot}/quote/ConvertQuoteToOrder";

        //Get Quote LineItems by QuoteId.
        public static string GetQuoteLineItemByQuoteId(int omsQuoteId) => $"{ApiRoot}/quote/getquotelineitembyquoteid/{omsQuoteId}";

        //update existing Quote endpoint.
        public static string UpdateQuote() => $"{ApiRoot}/quote/updatequote";

        // Get Quote Total
        public static string GetQuoteTotal(string quoteNumber) => $"{ApiRoot}/quote/getquotetotal/{quoteNumber}";
    }
}
