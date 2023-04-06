using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class ShoppingCartClient : BaseClient, IShoppingCartClient
    {
        //Get cart details by cookie.
        public ShoppingCartModel GetShoppingCart(CartParameterModel cartParameterModel)
        {
            //Get Endpoint
            string endpoint = ShoppingCartEndpoint.GetShoppingCart();

            //Get response
            ApiStatus status = new ApiStatus();
            ShoppingCartResponse response = PostResourceToEndpoint<ShoppingCartResponse>(endpoint, JsonConvert.SerializeObject(cartParameterModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ShoppingCart;
        }

        //Get Cart Count 
        public string GetCartCount(CartParameterModel cartParameterModel)
        {
            //Get Endpoint
            string endpoint = ShoppingCartEndpoint.GetCartCount();

            //Get response
            ApiStatus status = new ApiStatus();
            StringResponse response = PostResourceToEndpoint<StringResponse>(endpoint, JsonConvert.SerializeObject(cartParameterModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Response;
        }

        //Create new shooping cart.
        public virtual ShoppingCartModel CreateCart(ShoppingCartModel shoppingCartModel)
        {
            //Get Endpoint
            string endpoint = ShoppingCartEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ShoppingCartResponse response = PostResourceToEndpoint<ShoppingCartResponse>(endpoint, JsonConvert.SerializeObject(shoppingCartModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.Created, HttpStatusCode.InternalServerError };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ShoppingCart;
        }

        //Add product in cart.
        public virtual AddToCartModel AddToCartProduct(AddToCartModel addToCartModel)
        {
            //Get Endpoint
            string endpoint = ShoppingCartEndpoint.AddToCartProduct();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            AddToCartResponse response = PostResourceToEndpoint<AddToCartResponse>(endpoint, JsonConvert.SerializeObject(addToCartModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.Created, HttpStatusCode.InternalServerError };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AddToCart;
        }

        //Performs calculations for a shopping cart.
        public virtual ShoppingCartModel Calculate(ShoppingCartModel shoppingCartModel)
        {
            //Get Endpoint
            string endpoint = ShoppingCartEndpoint.Calculate();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ShoppingCartResponse response = PostResourceToEndpoint<ShoppingCartResponse>(endpoint, JsonConvert.SerializeObject(shoppingCartModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.ShoppingCart;
        }

        //Remove all saved cart items on the basis of user Id and cookie Mapping Id.
        public virtual bool RemoveAllCartItem(CartParameterModel cartParameterModel)
        {
            //Get Endpoint.
            string endpoint = ShoppingCartEndpoint.RemoveSavedCartItems();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse deleted = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cartParameterModel), status);

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return deleted.IsSuccess;
        }

        //Remove Saved Cart Line Item by omsSavedCartLineItemId
        public virtual bool RemoveCartLineItem(int omsSavedCartLineItemId)
        {
            string endpoint = ShoppingCartEndpoint.RemoveCartLineItem(omsSavedCartLineItemId);

            ApiStatus status = new ApiStatus();
            bool response = DeleteResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response;
        }

        public virtual ShippingListModel GetShippingEstimates(string zipCode, ShoppingCartModel shoppingCartModel)
        {
            //Get Endpoint
            string endpoint = ShoppingCartEndpoint.GetShippingEstimates(zipCode);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ShippingListResponse response = PostResourceToEndpoint<ShippingListResponse>(endpoint, JsonConvert.SerializeObject(shoppingCartModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            //Shipping list.
            ShippingListModel shippingList = new ShippingListModel { ShippingList = response?.ShippingList };

            return shippingList;
        }

        public virtual OrderLineItemDataListModel GetOmsLineItemDetails(int omsOrderId)
        {
            string endpoint = ShoppingCartEndpoint.GetOmsLineItemDetails(omsOrderId);

            ApiStatus status = new ApiStatus();
            OmsLineItemListResponse response = GetResourceFromEndpoint<OmsLineItemListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.LineItemList;
        }

        public virtual bool MergeGuestUsersCart(FilterCollection filters)
        {
            string endpoint = ShoppingCartEndpoint.MergeGuestUsersCart();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
    }
}
