using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class WishListController : BaseController
    {
        #region Private Variables
        private readonly IWebStoreWishListService _service;
        #endregion

        #region Constructor
        public WishListController(IWebStoreWishListService service)
        {
            _service = service;
        }
        #endregion

        /// <summary>
        /// Add product to user wishlist.
        /// </summary>
        /// <param name="model">Model with wish list data.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(WishListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AddToWishList([FromBody] WishListModel model)
        {
            HttpResponseMessage response;

            try
            {
                WishListModel wishlist = _service.AddToWishList(model);
                response = HelperUtility.IsNotNull(wishlist) ? CreateCreatedResponse(new WishListResponse { WishList = wishlist }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WishListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WishListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete a product from wishlist.
        /// </summary>
        /// <param name="wishListId">Wishlist Id of product.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpDelete]
        public virtual HttpResponseMessage DeleteWishList(int wishListId)
        {
            HttpResponseMessage response;

            try
            {  
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteWishlist(wishListId) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}
