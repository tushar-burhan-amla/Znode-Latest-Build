namespace Znode.Engine.Api.Client.Endpoints
{
    //Product History Endpoint.
    public class ProductHistoryEndpoint : BaseEndpoint
    {
        //Get Product History list endpoint. 
        public static string GetProductHistoryList() => $"{ApiRoot}/producthistory/list";

        //Get Product History by id endpoint. 
        public static string GetProductHistory(int id) => $"{ApiRoot}/producthistory/{id}";

        //Create Product History endpoint.
        public static string CreateProductHistory() => $"{ApiRoot}/producthistory";

        //Update Product History endpoint.
        public static string UpdateProductHistory() => $"{ApiRoot}/producthistory/update";

        //Delete Product History by id endpoint.
        public static string DeleteProductHistory(int id) => $"{ApiRoot}/producthistory/delete/{id}";
    }
}
