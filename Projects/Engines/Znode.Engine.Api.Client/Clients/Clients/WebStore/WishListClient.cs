using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class WishListClient : BaseClient, IWishListClient
    {
        //Add product to user's wishlist.
        public virtual WishListModel AddToWishList(WishListModel model)
        {
            string endpoint = WishListEndPoint.AddToWishList();

            ApiStatus status = new ApiStatus();
            WishListResponse response = PostResourceToEndpoint<WishListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.WishList;
        }
        
        //Delete wishlist against a user.
        public virtual bool DeleteWishList(int wishListId)
        {
            string endpoint = WishListEndPoint.DeleteWishList(wishListId);
            ApiStatus status = new ApiStatus();
            bool response = DeleteResourceFromEndpoint<TrueFalseResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response;
        }
    }
}
