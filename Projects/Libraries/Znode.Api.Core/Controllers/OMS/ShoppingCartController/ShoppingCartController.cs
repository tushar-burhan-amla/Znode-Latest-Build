using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers.OMS.ShoppingCartController
{
    public class ShoppingCartController : BaseController
    {
        #region Private Variables
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartCache _shoppingCartCache;
        #endregion

        #region Constructor
        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
            _shoppingCartCache = new ShoppingCartCache(_shoppingCartService);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a shopping cart for a cookie.
        /// </summary>
        /// <param name="cartParameterModel">Shopping cart model.</param>
        /// <returns>This endpoint relies on data passed into the request header instead of explicit parameters.</returns>
        [ResponseType(typeof(ShoppingCartResponse))]
        [HttpPost]
        public HttpResponseMessage GetShoppingCart(CartParameterModel cartParameterModel)
        {
            HttpResponseMessage response;

            try
            {
                ShoppingCartModel shoppingCartModel = _shoppingCartCache.GetShoppingCart(RouteUri, cartParameterModel);
                response = IsNotNull(shoppingCartModel) ? CreateOKResponse(new ShoppingCartResponse { ShoppingCart = shoppingCartModel }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShoppingCartResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShoppingCartResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }



        /// <summary>
        /// Gets a shopping cart count.
        /// </summary>
        /// <param name="cartParameterModel">Shopping cart model.</param>
        /// <returns>This endpoint relies on data passed into the request header instead of explicit parameters.</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpPost]
        public HttpResponseMessage GetCartCount(CartParameterModel cartParameterModel)
         {
            HttpResponseMessage response;

            try
            {
                string count = _shoppingCartCache.GetCartCount(RouteUri, cartParameterModel);
                response = IsNotNull(count) ? CreateOKResponse(new StringResponse { Response = count }) : CreateNoContentResponse();
            }
            
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }


        


        /// <summary>
        /// Creates a new shopping cart and saves it to the database.
        /// </summary>
        /// <param name="model">The model of the shopping cart.</param>
        /// <returns>Shopping Cart Response</returns>
        [ResponseType(typeof(ShoppingCartResponse))]
        [HttpPost]
        public HttpResponseMessage Create([FromBody] ShoppingCartModel model)
        {
            HttpResponseMessage response;
            try
            {
                ShoppingCartModel shoppingCart = _shoppingCartCache.CreateCart(RouteUri, model);

                response = IsNotNull(shoppingCart) ? CreateCreatedResponse(new ShoppingCartResponse { ShoppingCart = shoppingCart }) : CreateNoContentResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShoppingCartResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ShoppingCartResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Creates a new shopping cart and saves it to the database.
        /// </summary>
        /// <param name="model">The model of the shopping cart.</param>
        /// <returns>Add to Cart Response</returns>
        [ResponseType(typeof(AddToCartResponse))]
        [HttpPost]
        public HttpResponseMessage AddToCartProduct([FromBody] AddToCartModel model)
        {
            HttpResponseMessage response;
            try
            {
                AddToCartModel shoppingCart = _shoppingCartCache.AddToCartProduct(RouteUri, model);

                response = IsNotNull(shoppingCart) ? CreateCreatedResponse(new AddToCartResponse { AddToCart = shoppingCart }) : CreateNoContentResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new AddToCartResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AddToCartResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Performs calculations for a shopping cart.
        /// </summary>
        /// <param name="model">The model of the shopping cart.</param>
        /// <returns>Performs calculations and gives Shopping Cart Response </returns>
        [ResponseType(typeof(ShoppingCartResponse))]
        [HttpPost]
        public HttpResponseMessage Calculate([FromBody] ShoppingCartModel model)
        {
            HttpResponseMessage response;

            try
            {
                ShoppingCartModel shoppingCartModel = _shoppingCartCache.Calculate(RouteUri, model);
                response = IsNotNull(shoppingCartModel) ? CreateOKResponse(new ShoppingCartResponse { ShoppingCart = shoppingCartModel }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShoppingCartResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ShoppingCartResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Remove all cart items.
        /// </summary>
        /// <param name="cartParameterModel">parameter model contains parameters required to remove cart item.</param>
        /// <returns>Returns cart item.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage RemoveAllCartItem(CartParameterModel cartParameterModel)
        {
            HttpResponseMessage response;
            try
            {
                int cookieId = !string.IsNullOrEmpty(cartParameterModel.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(cartParameterModel.CookieMappingId)) : 0;
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _shoppingCartService.RemoveSavedCartItems(Convert.ToInt32(cartParameterModel.UserId), cookieId, cartParameterModel.PortalId) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        ///  Remove Saved Cart Line Item by omsSavedCartLineItemId
        /// </summary>
        /// <param name="omsSavedCartLineItemId"></param>
        /// <returns>return true if saved cart item deleted.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpDelete]
        public HttpResponseMessage RemoveCartLineItem(int omsSavedCartLineItemId)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _shoppingCartService.RemoveSavedCartLineItem(omsSavedCartLineItemId) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get shipping estimates by zip code.
        /// </summary>
        /// <param name="zipCode">zip Code</param>
        /// <param name="model">ShoppingCartModel</param>
        /// <returns>Shipping estimates by zip code.</returns>
        [ResponseType(typeof(ShippingListResponse))]
        [HttpPost]
        public HttpResponseMessage GetShippingEstimates(string zipCode, [FromBody] ShoppingCartModel model)
        {
            HttpResponseMessage response;
            try
            {
                ShippingListModel list = _shoppingCartCache.GetShippingEstimates(zipCode, RouteUri, model);
                response = IsNotNull(list) ? CreateOKResponse(new ShippingListResponse { ShippingList = list?.ShippingList }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShippingListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ShippingListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get OmsLineItem Detail by omsOrderId
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <returns>List of OmsLineItems</returns>
        [ResponseType(typeof(OmsLineItemListResponse))]
        [HttpGet]
        public HttpResponseMessage GetOmsLineItemDetails(int omsOrderId)
        {
            HttpResponseMessage response;
            try
            {
                OrderLineItemDataListModel LineItemDetailList = _shoppingCartCache.GetOrderLineItemDetails(omsOrderId);
                response = IsNotNull(LineItemDetailList) ? CreateOKResponse(new OmsLineItemListResponse { LineItemList = LineItemDetailList}) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OmsLineItemListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OmsLineItemListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Merge Cart after login
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage MergeGuestUsersCart()
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _shoppingCartCache.MergeGuestUsersCart(RouteUri) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion
    }
}