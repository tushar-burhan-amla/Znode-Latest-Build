using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IWebStoreWishListService
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
        /// <param name="wishlistId">WishList Id.</param>
        /// <returns>Returns true if wishlist deleted successfully else return false.</returns>
        bool DeleteWishlist(int wishlistId);

        /// <summary>
        /// Update product to user wish list.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool UpdateWishList(WishListModel model);
    }
}
