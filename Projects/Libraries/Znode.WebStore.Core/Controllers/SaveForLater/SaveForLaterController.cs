using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Controllers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.WebStore.Core.Agents;

namespace Znode.WebStore.Core.Controllers.SaveForLater
{
    public class SaveForLaterController : BaseController
    {

        #region Private Readonly Variables
        private readonly ISaveForLaterAgent _saveForLaterAgent;
        private readonly ICartAgent _cartAgent;
        #endregion

        #region Public Constructor
        public SaveForLaterController(ISaveForLaterAgent saveForLaterAgent, ICartAgent cartAgent)
        {
            _saveForLaterAgent = saveForLaterAgent;
            _cartAgent = cartAgent;
        }
        #endregion

        #region Public Methods

        //Create Save for Later
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SaveForLater(string guiId)
        {
            if(!string.IsNullOrEmpty(guiId))
            {
                TemplateViewModel templateViewModel = _saveForLaterAgent.CreateCartForLater(guiId);
                if (HelperUtility.IsNotNull(templateViewModel))
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.SuccessSaveForLater));
                    return RedirectToAction<CartController>(x => x.RemoveLineItemSaveForLater(guiId));
                }
                else
                {
                    SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.FailedSaveForLater));
                }
            }

            return RedirectToAction<CartController>(x => x.Index());
        }

        //Get saved cart for later
        [Authorize]
        public virtual ActionResult GetSavedCartForLater()
        {
            TemplateViewModel templateViewModel = _saveForLaterAgent.GetSavedCartForLater();

            return View("_SaveForLater", HelperUtility.IsNull(templateViewModel) ? new TemplateViewModel() : templateViewModel);

        }

        //Remove single cart line item from cart.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RemoveTemplateCartItem(int omsTemplateId, int omsTemplateLineItemId)
        {
            bool status = _saveForLaterAgent.RemoveSingleCartLineItem(omsTemplateId, omsTemplateLineItemId);
            SetNotificationMessageByStatus(status, GetSuccessNotificationMessage(WebStore_Resources.DeleteMessage), GetErrorNotificationMessage(WebStore_Resources.DeleteFailMessage));

            return RedirectToAction<CartController>(x => x.Index());
        }

        //Remove all cart item from template.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RemoveAllTemplateCartItem(int omsTemplateId)
        {
            SetNotificationMessage(_saveForLaterAgent.RemoveAllTemplateCartItems(omsTemplateId) ? GetSuccessNotificationMessage(WebStore_Resources.DeleteMessage)
            : GetErrorNotificationMessage(WebStore_Resources.DeleteFailMessage));

            return RedirectToAction<CartController>(x => x.Index());
        }

        //Add Product to cart.
        [Authorize]
        public virtual ActionResult AddProductToCart(int omsTemplateId, int omsTemplateLineItemId)
        {
            string errorMessage = _cartAgent.AddProductToCartForLater(omsTemplateId, omsTemplateLineItemId);

            if (string.IsNullOrEmpty(errorMessage))
            {
                return RedirectToAction<SaveForLaterController>(x => x.RemoveProductFromSavedCart(omsTemplateId, omsTemplateLineItemId));
            }
            else{
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.FailedMoveToCart));
                return RedirectToAction<CartController>(x => x.Index());
            }
        }

        //To remove the line item after succussfully moved to cart.
        public ActionResult RemoveProductFromSavedCart(int omsTemplateId, int omsTemplateLineItemId)
        {
            bool status = _saveForLaterAgent.RemoveSingleCartLineItem(omsTemplateId, omsTemplateLineItemId);
            SetNotificationMessageByStatus(status, GetSuccessNotificationMessage(WebStore_Resources.SuccessMoveToCartForLater), GetErrorNotificationMessage(WebStore_Resources.FailedMoveToCart));

            return RedirectToAction<CartController>(x => x.Index());
        }


        #endregion
    }
}
