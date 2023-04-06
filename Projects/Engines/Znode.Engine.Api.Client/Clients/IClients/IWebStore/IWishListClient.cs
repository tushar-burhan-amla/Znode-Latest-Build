using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IWishListClient : IBaseClient
    {
        /// <summary>
        /// Add product to user wish list.
        /// </summary>
        /// <param name="model">Model with user product wishlist.</param>
        /// <returns>Returns wish list model.</returns>
        WishListModel AddToWishList(WishListModel model);

        /// <summary>
        /// Delete wishlist against a user.
        /// </summary>
        /// <param name="wishListId">WishList Id.</param>
        /// <returns>Returns true if wishlist deleted successfully else return false.</returns>
        bool DeleteWishList(int wishListId);
    }
}
