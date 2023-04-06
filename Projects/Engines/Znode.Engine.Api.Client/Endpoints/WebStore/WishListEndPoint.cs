namespace Znode.Engine.Api.Client.Endpoints
{
    public class WishListEndPoint : BaseEndpoint
    {
        //Endpoint for add product wishlist.
        public static string AddToWishList() => $"{ApiRoot}/wishlist/addtowishlist";

        //Endpoint to delete wishlist.
        public static string DeleteWishList(int wishListId) => $"{ApiRoot}/wishlist/deletewishlist/{wishListId}";
    }
}
