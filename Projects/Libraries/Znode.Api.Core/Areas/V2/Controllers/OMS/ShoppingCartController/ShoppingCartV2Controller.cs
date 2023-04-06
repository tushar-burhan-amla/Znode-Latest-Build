using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses.V2;
using Znode.Engine.Api.Models.V2;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers.OMS.ShoppingCartController
{
    public class ShoppingCartV2Controller : BaseController
    {
        #region Private Variables
        private readonly IShoppingCartServiceV2 _shoppingCartService;
        private readonly IShoppingCartCacheV2 _shoppingCartCache;
        #endregion

        #region Constructor
        public ShoppingCartV2Controller(IShoppingCartServiceV2 shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
            _shoppingCartCache = new ShoppingCartCacheV2(_shoppingCartService);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new shopping cart and saves it to the database.
        /// </summary>
        /// <param name="model">The model of the shopping cart.</param>
        /// <returns></returns>
        [ResponseType(typeof(ShoppingCartResponseV2))]
        [HttpPost]
        public HttpResponseMessage Create([FromBody] ShoppingCartModelV2 model)
        {
            HttpResponseMessage response;
            try
            {
                ShoppingCartModelV2 shoppingCart = _shoppingCartService.CreateCart(model);

                response = IsNotNull(shoppingCart) ? CreateCreatedResponse(new ShoppingCartResponseV2 { ShoppingCart = shoppingCart }) : CreateNoContentResponse();

            }
            catch (ZnodeException ex)
            {
                response = CreateExceptionResponse(ex, true, ZnodeLogging.Components.OMS);
            }
            catch (Exception ex)
            {
                response = CreateExceptionResponse(ex, true, ZnodeLogging.Components.OMS);
            }
            return response;
        }

        /// <summary>
        /// Remove shopping cart items.
        /// </summary>
        /// <param name="model">Remove cart item model.</param>
        /// <returns></returns>
        [ResponseType(typeof(ShoppingCartResponseV2))]
        [HttpDelete, ValidateModel]
        public HttpResponseMessage RemoveCartItems([FromBody] RemoveCartItemModelV2 model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new ShoppingCartResponseV2 { ShoppingCart = _shoppingCartService.RemoveSavedCartItems(model) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShoppingCartResponseV2 { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShoppingCartResponseV2 { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Performs calculations for a shopping cart.
        /// </summary>
        /// <param name="model">The model of the shopping cart.</param>
        /// <returns></returns>
        [ResponseType(typeof(ShoppingCartCalculateResponseV2))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage CalculateV2([FromBody] ShoppingCartCalculateRequestModelV2 model)
        {
            HttpResponseMessage response;

            try
            {
                ShoppingCartModelV2 shoppingCartModel = _shoppingCartCache.CalculateV2(model);
                response = IsNotNull(shoppingCartModel) ? CreateOKResponse(new ShoppingCartCalculateResponseV2 { ShoppingCart = shoppingCartModel }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShoppingCartCalculateResponseV2 { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShoppingCartCalculateResponseV2 { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        [ResponseType(typeof(ShoppingCartResponseV2))]
        [HttpGet, ValidateModel]
        public HttpResponseMessage GetShoppingCart([FromUri] CartParameterModel model)
        {
            HttpResponseMessage response;
            try
            {
                //This is to avoid the error 'Invalid Base64-encoded string' as the '+' in CookieMappingId gets replaced by ' '
                model.CookieMappingId = model.CookieMappingId.Replace(' ', '+');
                response = CreateOKResponse(new ShoppingCartResponseV2 { ShoppingCart = _shoppingCartService.GetShoppingCartV2(model) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShoppingCartResponseV2 { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ShoppingCartResponseV2 { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
    }
}
