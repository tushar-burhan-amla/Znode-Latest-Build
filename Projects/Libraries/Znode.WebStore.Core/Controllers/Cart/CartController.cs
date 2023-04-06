using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.WebStore.Core.Agents;
using Znode.WebStore.Core.Extensions;

namespace Znode.Engine.WebStore.Controllers
{
    [NoCacheAttribute]
    public class CartController : BaseController
    {
        #region Private Variables
        private readonly ICartAgent _cartAgent;
        private readonly IWSPromotionAgent _promotionAgent;
        private readonly string shoppingCartView = "Cart";
        private readonly string shoppingCart = "_shoppingCart";
        #endregion

        #region Constructor
        public CartController(ICartAgent cartAgent, IPortalAgent portalAgent, IWSPromotionAgent promotionAgent)
        {
            _cartAgent = cartAgent;
            _promotionAgent = promotionAgent;

        }
        #endregion

        #region Action Methods
        // GET: Cart
        public virtual ActionResult Index()
        {
            CartViewModel cartdata = _cartAgent.GetCart(false, false);
            return View(shoppingCartView, cartdata);
        }

        // GET: Get Shopping Cart
        public virtual ActionResult GetShoppingCart()
        {
            CartViewModel cartdata = _cartAgent.GetCart(false, false);
            return ActionView(shoppingCartView, cartdata);
        }

        public virtual ActionResult CartPromotionBar()
        {
            CartViewModel cartdata = new CartViewModel();

            cartdata.CultureCode = PortalAgent.CurrentPortal.CultureCode;

            cartdata.ShippingModel = _promotionAgent.GetPromotionListByPortalId(PortalAgent.CurrentPortal.PortalId);
            return PartialView("_CartPromotionBar", cartdata);
        }


        public virtual ActionResult CalculateCart()
        {
            CartViewModel cartdata = _cartAgent.CalculateCart();

            return PartialView("../_TotalTable", cartdata);
        }

        //Remove cart item after successfully added the item to the saved cart 
        public virtual ActionResult RemoveLineItemSaveForLater(string guid)
        {
             return RemoveCartItem(guid);
        }

        //Update quantity of cart item.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UpdateCartQuantity(string guid, string quantity, int productId = 0)
        {
            CartViewModel cartView = _cartAgent.UpdateCartItemQuantity(guid, quantity, productId);

            return (cartView?.OmsQuoteId > 0)
            ? RedirectToAction<CheckoutController>(x => x.CartReview(0, 0, cartView.ShippingName ?? string.Empty, string.Empty, false, true, cartView.IsPendingOrderRequest, string.Empty))
            : RedirectToAction<CartController>(x => x.GetShoppingCart());
        }

        //Update quantity of cart item.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UpdateQuantityOfCartItem(string guid, string quantity, int productId = 0)
        {
            _cartAgent.UpdateQuantityOfCartItem(guid, quantity, productId);
            CartViewModel cartdata = _cartAgent.GetCart(false,false);
            cartdata.ShippingModel = _promotionAgent.GetPromotionListByPortalId(PortalAgent.CurrentPortal.PortalId);
            return PartialView(shoppingCart, cartdata);
        }

        //Remove single cart item from shopping cart.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RemoveCartItem(string guid)
        {
            _cartAgent.RemoveCartItem(guid);
            return RedirectToAction<CartController>(x => x.Index());
        }

        //Remove single cart item from shopping cart.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RemoveCartItems(string[] guids, string editQuoteId)
        {
            _cartAgent.RemoveCartItems(guids);
            return (string.IsNullOrEmpty(editQuoteId))
                ? RedirectToAction<CartController>(x => x.Index())
                : RedirectToAction<CheckoutController>(x => x.CartReview(0, 0, string.Empty, string.Empty, false, true, false, string.Empty));
        }


        //Remove all cart item from shopping cart.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RemoveAllCartItem()
        {
            _cartAgent.RemoveAllCartItems();
            return RedirectToAction<CartController>(x => x.Index());
        }

        //Get the available shipping options with estimates for the provided Zip Code
        public virtual ActionResult GetShippingEstimates(string zipCode)
        {
            ShippingOptionListViewModel shippingOptions = _cartAgent.GetShippingEstimates(zipCode);
            return Json(new
            {
                zipCode = zipCode,
                shippingOptions = shippingOptions.ShippingOptions
            }, JsonRequestBehavior.AllowGet);
        }


        public virtual ActionResult GetCalculatedShipping(int shippingId, string zipCode)
        {
            _cartAgent.AddEstimatedShippingDetailsToCartViewModel(shippingId, zipCode);

            CartViewModel cartModel = _cartAgent.CalculateShipping(shippingId, 0, null);
            return PartialView("_TotalTable", cartModel);
        }

        //Get the cart count of the product.
        [HttpGet]
        public virtual JsonResult GetCartCount(int productId)
        {
            return Json(_cartAgent.GetCartCount(productId), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}