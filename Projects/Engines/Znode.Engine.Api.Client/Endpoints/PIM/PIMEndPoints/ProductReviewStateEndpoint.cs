namespace Znode.Engine.Api.Client.Endpoints
{
    public class ProductReviewStateEndpoint : BaseEndpoint
    {
        //ProductReviewState List Endpoint
        public static string ProductReviewStateList() => $"{ApiRoot}/productreviewstates/list";
    }
}
