using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Controllers;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.WebStore.Core.Agents;
using Znode.Engine.WebStore;
using System.Collections.Generic;
using Znode.Engine.WebStore.Models;
using Znode.Engine.Api.Models;
using System;

namespace Znode.WebStore.Core.Controllers.Savecart
{
    public class SavedCartController : BaseController
    {
        #region Private Readonly Variables
        private readonly ISavedCartAgent _savedCartAgent;
        private readonly IUserAgent _userAgent;
        private readonly ICartAgent _cartAgent;
        private readonly ISaveForLaterAgent _saveForLaterAgent;
        #endregion

        #region Public Constructor
        public SavedCartController(ISavedCartAgent savedCartAgent, IUserAgent userAgent, ICartAgent cartAgent, ISaveForLaterAgent saveForLaterAgent)
        {
            _savedCartAgent = savedCartAgent;
            _userAgent = userAgent;
            _cartAgent = cartAgent;
            _saveForLaterAgent = saveForLaterAgent;
        }
        #endregion

        [HttpPost]
        public virtual ActionResult CreateSavedCart(string templateName)
        {
            if (!string.IsNullOrEmpty(templateName))
            {
                TemplateViewModel templateViewModel = _savedCartAgent.CreateSavedCart(templateName);
                if (HelperUtility.IsNotNull(templateViewModel))
                {
                    _cartAgent.RemoveAllCartItems();
                    TempData["IsRedirected"] = "true";
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false}, JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction<CartController>(x => x.Index());
        }

        public virtual ActionResult SavedCartList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //If session expires redirect to login page.
            if (string.IsNullOrEmpty(_userAgent.GetUserViewModelFromSession()?.RoleName))
                return RedirectToAction<UserController>(x => x.Login(string.Empty));

            ViewBag.IsRedirected = (string)TempData["IsRedirected"];
            TemplateListViewModel templateList = _cartAgent.GetTemplateList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, ZnodeConstant.SavedCart);
            _cartAgent.SetTemplateCartModelSessionToNull();

            templateList.GridModel = FilterHelpers.GetDynamicGridModel(model, templateList?.List, WebStoreConstants.ZnodeSavedCart.ToString(), string.Empty, null, true, true, templateList?.GridModel?.FilterColumn?.ToolMenuList);
            templateList.GridModel.TotalRecordCount = templateList.TotalResults;

            return ActionView("SavedCartList", templateList);
        }

        [HttpGet]
        public virtual ActionResult GetTemplate()
        {
            List<DropdownListModel> templateList = _savedCartAgent.GetSavedCart();

            string template = RenderRazorViewToString("SavedCartView", templateList);
            return Json(new
            {
                templateHtml = template
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult EditSaveCart(int omsTemplateId) 
        {
            if (HelperUtility.IsNotNull(omsTemplateId))
            {
                bool status = _savedCartAgent.EditSaveCart(omsTemplateId);
                if (status)
                {
                    _cartAgent.RemoveAllCartItems();
                    return Json(new { status }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status }, JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction<CartController>(x => x.Index());
        }

        public virtual ActionResult AddProductToCartForSaveCart(int omsTemplateId)
        {
            string status = _savedCartAgent.AddProductToCartForSaveCart(omsTemplateId);
            if (HelperUtility.IsNull(status) && string.Equals(status, ZnodeConstant.FalseValue, StringComparison.CurrentCultureIgnoreCase))
            {
                SetNotificationMessage(GetErrorNotificationMessage(WebStore_Resources.FailedMoveToCart));
                return RedirectToAction<SavedCartController>(x => x.SavedCartList(null));
            }
            else if (string.Equals(status, ZnodeConstant.TrueValue, StringComparison.CurrentCultureIgnoreCase))
            {
                SetNotificationMessage(GetSuccessNotificationMessage(WebStore_Resources.SuccessMoveToCart));
                return RedirectToAction<CartController>(x => x.Index());
            }
            else
            {
                return RedirectToAction<SavedCartController>(x => x.EditSavedCartItem(omsTemplateId,false,"",""));                
            }
        }

        //Edit Template.
        [Authorize]
        public virtual ActionResult EditSavedCartItem(int omsTemplateId, bool isClearAll = false,string productId = "", string errorMessageInTemplateToCart = "")
        {
            TemplateViewModel templateViewModel = _cartAgent.GetTemplate(omsTemplateId, isClearAll);
            if (HelperUtility.IsNull(templateViewModel))
                return Redirect("/404");

            if (templateViewModel.HasError && templateViewModel.ErrorMessage == WebStore_Resources.HttpCode_401_AccessDeniedMsg)
                return Redirect("/404");

            foreach (var item in templateViewModel.TemplateCartItems)
            {
                if (item.GroupProducts?.Count > 0)
                {
                    foreach (var groupProduct in item.GroupProducts)
                    {
                        templateViewModel.IsAddToCartButtonDisable = templateViewModel.IsAddToCartButtonDisable ? templateViewModel.IsAddToCartButtonDisable : (bool)GetCartItems(groupProduct.ProductId, Convert.ToInt32(groupProduct.Quantity)).GetDynamicProperty("Data").GetProperty("status");
                        if (groupProduct.ProductId.ToString() == productId)
                            item.ErrorMessage = errorMessageInTemplateToCart;
                    }
                }
                else
                    templateViewModel.IsAddToCartButtonDisable = templateViewModel.IsAddToCartButtonDisable ? templateViewModel.IsAddToCartButtonDisable : (bool)GetCartItems(Convert.ToInt32(item.ProductId), Convert.ToInt32(item.Quantity)).GetDynamicProperty("Data").GetProperty("status");

            }
            return View("EditSavedCartView", HelperUtility.IsNull(templateViewModel) ? new TemplateViewModel() : templateViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditSavedCartItem(TemplateViewModel cartItem)
        {
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_savedCartAgent.EditSavedCartItem(cartItem) ? GetSuccessNotificationMessage(WebStore_Resources.SuccessEditSavedCart)
                : GetErrorNotificationMessage(WebStore_Resources.ErrorEditSavedCart));

                if (cartItem.HasError && cartItem.ErrorMessage == WebStore_Resources.HttpCode_401_AccessDeniedMsg)
                    return Redirect("/404");

                return RedirectToAction<SavedCartController>(x => x.SavedCartList(null));
            }

            return View("EditSavedCartView", HelperUtility.IsNull(cartItem) ? new TemplateViewModel() : cartItem);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RemoveAllSavedCartItem(int omsTemplateId)
        {
            SetNotificationMessage(_saveForLaterAgent.RemoveAllTemplateCartItems(omsTemplateId, Convert.ToBoolean(ZnodeConstant.True)) ? GetSuccessNotificationMessage(WebStore_Resources.DeleteMessage)
              : GetErrorNotificationMessage(WebStore_Resources.DeleteFailMessage));

            return omsTemplateId > 0 ? RedirectToAction<SavedCartController>(x => x.EditSavedCartItem(omsTemplateId, false,"", "")) : RedirectToAction<CartController>(x => x.Index());
        }

        //Remove single cart item from template.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RemoveSavedCartItem(string guid, int omsTemplateId)
        {
            SetNotificationMessage(_cartAgent.RemoveTemplateCartItem(guid, WebStoreConstants.SavedCart) ? GetSuccessNotificationMessage(WebStore_Resources.DeleteMessage)
            : GetErrorNotificationMessage(WebStore_Resources.DeleteFailMessage));

            return omsTemplateId > 0 ? RedirectToAction<SavedCartController>(x => x.EditSavedCartItem(omsTemplateId, false, "", "")) : RedirectToAction<CartController>(x => x.Index());
        }

        public virtual ActionResult GetCartItems(int productId, int selectedQty)
        {
            ShoppingCartModel cart = _cartAgent.GetCartItems();

            foreach (ShoppingCartItemModel item in cart.ShoppingCartItems)
            {
                if (item.Product.PublishProductId == productId)
                {
                    if ((item.Quantity + selectedQty) > item.MaxQuantity)
                    {
                        return Json(new
                        {
                            status = true
                        }, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            return Json(new
            {
                status = false
            }, JsonRequestBehavior.AllowGet);
        }

        //Update quantity of cart item.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UpdateSavedCartQuantity(string guid, decimal quantity, int productId = 0, int omsTemplateId = 0)
        {
            _savedCartAgent.UpdateSavedItemQuantity(guid, quantity, productId);
            return (omsTemplateId > 0) ? RedirectToAction<SavedCartController>(x => x.EditSavedCartItem(omsTemplateId, false,"", ""))
                : RedirectToAction<CartController>(x => x.Index());
        }

        public virtual ActionResult EditSaveCartName(string templateName, int templateId) 
        {
            bool status=_savedCartAgent.EditSaveCartName(templateName, templateId);
            if (status)
            {
                return Json(new { status }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status }, JsonRequestBehavior.AllowGet);
            }
        }

        // Delete saved cart on the basis of omsTemplateId.
        [Authorize]
        public virtual ActionResult DeleteSavedCartTemplate(string omsTemplateId)
        {
            if (!string.IsNullOrEmpty(omsTemplateId))
            {
                bool isDeleted = _cartAgent.DeleteTemplate(omsTemplateId);

                return Json(new { status = isDeleted, message = isDeleted ? WebStore_Resources.SavedCartDeleteMessage : WebStore_Resources.SavedCartDeleteFailMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = WebStore_Resources.SavedCartDeleteFailMessage }, JsonRequestBehavior.AllowGet);
        }
    }
}
