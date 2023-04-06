using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.Helpers.Extensions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System.Web.SessionState;

namespace Znode.Engine.Admin.Controllers
{
    public class OrderController : BaseController
    {
        #region Private ReadOnly members

        private readonly IOrderAgent _orderAgent;
        private readonly IUserAgent _userAgent;
        private readonly IShippingAgent _shippingAgent;
        private readonly ICartAgent _cartAgent;
        private readonly IAccountQuoteAgent _quoteAgent;
        private readonly IStoreAgent _storeAgent;
        private readonly IRMARequestAgent _rmaRequestAgent;
        private readonly IWebSiteAgent _websiteAgent;
        private readonly IPaymentAgent _paymentAgent;

        private const string storeListView = "_StoreListAsidePanel";
        private const string orderLineItemView = "_OrderLineItemList";
        private const string manageCustomerAddressView = "_ManageCustomerAddress";
        private const string manageCustomerView = "_ManageCustomerInformation";
        private const string manageTotalTableView = "_ManageTotalTable";
        private const string manageShoppingCartView = "ManageShoppingCart";
        private const string shippingOptionsView = "_ShippingOptions";
        private const string quoteShoppingCartView = "~/views/Quote/QuoteShoppingCart.cshtml";

        #endregion

        #region Constructor

        public OrderController(IOrderAgent orderAgent, IUserAgent userAgent, IShippingAgent shippingAgent, ICartAgent cartAgent, IAccountQuoteAgent quoteAgent, IStoreAgent storeAgent, IRMARequestAgent rmaRequestAgent, IWebSiteAgent websiteAgent, IPaymentAgent paymentAgent)
        {
            _orderAgent = orderAgent;
            _userAgent = userAgent;
            _shippingAgent = shippingAgent;
            _cartAgent = cartAgent;
            _quoteAgent = quoteAgent;
            _storeAgent = storeAgent;
            _rmaRequestAgent = rmaRequestAgent;
            _websiteAgent = websiteAgent;
            _paymentAgent = paymentAgent;
        }

        #endregion

        #region Public methods

        public virtual ActionResult Index() => View();

        #region Order Details

        /// <summary>
        /// This method will fetch the list of all the orders details.
        /// </summary>
        /// <param name="model">Filtercollection model</param>
        /// <returns>Returns the list of admin account details.</returns>
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelAdmin", Key = "Orders", Area = "", ParentKey = "OMS")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int userId = 0, int accountId = 0, int portalId = 0, string portalName = null)
        {
            //Remove DateTimeRange Filter From Cookie.
            DateRangePickerHelper.RemoveDateTimeRangeFiltersFromCookies(GridListType.ZnodeOrder.ToString(), model);
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeOrder.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeOrder.ToString(), model);

            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            //Get the list of Orders
            OrdersListViewModel orders = _orderAgent.GetOrderList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, userId, accountId, portalId, portalName);

            //get distinct order list
            List<OrderViewModel> ordersModel = orders.List.GroupBy(x => x.OmsOrderId).Select(y => y.FirstOrDefault()).ToList();
            //Get the grid model
            orders.GridModel = FilterHelpers.GetDynamicGridModel(model, ordersModel, GridListType.ZnodeOrder.ToString(), string.Empty, null, true, true, orders?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            orders.GridModel.TotalRecordCount = orders.TotalResults;

            //Returns the attribute list view
            return ActionView(orders);
        }



        /// <summary>
        /// The index page for create order.
        /// </summary>
        /// <param name="portalId">current portal id</param>
        /// <param name="userId">current user id</param>
        /// <returns>Return view</returns>
        [HttpGet]
        public virtual ActionResult CreateOrder(int portalId = 0, int userId = 0)
        {
            CreateOrderViewModel viewModel = _orderAgent.GetCreateOrderDetails(portalId);
            // This method is call to get payment Api header as store on the view
            ViewBag.PaymentApiRequestHeader = _paymentAgent.GetPaymentAuthToken(HttpContext.User.Identity.Name.ToString(), true);
            StoreListViewModel storeList = _storeAgent.GetStoreList(null, null, null, null);
            if (storeList?.StoreList?.Count() == 1)
            {
                StoreViewModel storeViewModel = storeList.StoreList.FirstOrDefault();
                viewModel.StoreName = storeViewModel?.StoreName;
                viewModel.PortalId = storeViewModel.PortalId;
            }
            if (userId > 0)
            {
                if (HelperUtility.IsNull(viewModel.CartViewModel))
                    viewModel.CartViewModel = new CartViewModel();
                viewModel.CartViewModel.UserId = userId;
            }
            return ActionView(viewModel);
        }

        [HttpPost]
        public virtual ActionResult CreateCustomerOrder(CartParameterModel cartParameter)
        {
            //Remove all cart items.
            _cartAgent.RemoveCartSession(cartParameter?.UserId);
            CreateOrderViewModel viewModel = _orderAgent.GetCreateOrderDetails(cartParameter.PortalId);
            StoreListViewModel storeList = _storeAgent.GetStoreList(null, null, null, null);
            if (storeList?.StoreList?.Count() == 1)
            {
                StoreViewModel storeViewModel = storeList.StoreList.FirstOrDefault();
                viewModel.StoreName = storeViewModel?.StoreName;
                viewModel.PortalId = storeViewModel.PortalId;
            }
            if (cartParameter.UserId > 0)
            {
                if (HelperUtility.IsNull(viewModel.CartViewModel))
                    viewModel.CartViewModel = new CartViewModel();
                viewModel.CartViewModel.UserId = cartParameter.UserId.GetValueOrDefault();

                viewModel = _orderAgent.GetUserFullDetails(cartParameter);

                viewModel.StoreName = storeList.StoreList.FirstOrDefault(x => x.PortalId == cartParameter.PortalId)?.StoreName;
                viewModel.PortalId = cartParameter.PortalId;
                viewModel.UserId = cartParameter.UserId.GetValueOrDefault();
                viewModel.CartViewModel.PublishedCatalogId = cartParameter.PublishedCatalogId;
                viewModel.CustomerName = viewModel.UserAddressDataViewModel.FullName;
                viewModel.IsFromUserCart = true;
            }
            return ActionView(AdminConstants.CreateCustomerOrder, viewModel);
        }

        /// <summary>
        /// Submit order.
        /// </summary>
        /// <param name="model">CreateOrderViewModel</param>
        /// <returns>Returns View.</returns>
        [HttpPost]
        public virtual ActionResult SubmitOrder(CreateOrderViewModel model)
        {
            //If order is present then order will go for update.
            CreateOrderViewModel order = _orderAgent.SubmitOrder(model);

            if (IsNull(order))
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorUnablePlaceOrder));
                return ActionView(AdminConstants.CreateEditOrderView);
            }
            if (!order.HasError)
            {
                order.ReceiptHtml = WebUtility.HtmlEncode(order.ReceiptHtml);
                return new JsonResult { Data = order, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            if (!Equals(order, null) && order.HasError)
            {
                if (string.IsNullOrEmpty(order.ErrorMessage))
                    order.ErrorMessage = Admin_Resources.OrderSubmitError;
                SetNotificationMessage(GetErrorNotificationMessage(order.ErrorMessage));

                return new JsonResult { Data = order, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return ActionView();
        }

        [ActionSessionState(SessionStateBehavior.ReadOnly)]
        public virtual ActionResult GetPublishProduct(int publishProductId, int localeId, int portalId, int? userId = 0, int? catalogId = 0, bool isQuote = false)
           => ActionView("_ProductDetails", _orderAgent.GetPublishProduct(publishProductId, localeId, portalId, Convert.ToInt32(userId), Convert.ToInt32(catalogId), isQuote));

        [HttpGet]
        public virtual ActionResult GetBundleProduct(int productId)
            => View("_BundleProducts", _orderAgent.GetBundleProduct(productId));

        //Get Associated Configurable products.
        [HttpPost]
        public virtual ActionResult GetConfigurableProduct(ParameterProductModel parameters)
            => ActionView("_ProductDetails", _orderAgent.GetConfigurableProduct(parameters));

        //Get Associated Group products.
        public virtual ActionResult GetGroupProductList(int productId, int localeId, int portalId, int userId, int? catalogId = 0)
           => ActionView("_GroupProductList", _orderAgent.GetGroupProductList(productId, localeId, portalId, userId, catalogId));

        [HttpGet]
        public virtual ActionResult GetPaymentDetails(int paymentSettingId, int? userId = 0)
         => Json(_orderAgent.GetOrderPaymentDetails(paymentSettingId, userId), JsonRequestBehavior.AllowGet);

        //to get encrypted amount by supplied amount
        [HttpGet]
        public virtual ActionResult GetEncryptedAmount(string amount)
        => Json(new
        {
            data = Encryption.EncryptPaymentToken(amount)
        }, JsonRequestBehavior.AllowGet);

        public virtual ActionResult GenerateOrderNumber(int portalId)
        {
            return Json(new { orderNumber = _orderAgent.GenerateOrderNumber(portalId) }, JsonRequestBehavior.AllowGet);
        }

        //This method will use to call the payment and process the order
        [HttpPost]
        public virtual JsonResult SubmitPayment(SubmitPaymentViewModel submitPaymentViewModel)
        => Json(new
        {
            Data = _orderAgent.ProcessCreditCardPayment(submitPaymentViewModel)
        }, JsonRequestBehavior.AllowGet);

        [HttpPost]
        public virtual JsonResult ProcessPayPalPayment(SubmitPaymentViewModel paymentmodel)
        {
            List<string> response = _orderAgent.ProcessPayPalCheckout(paymentmodel);
            return Json(new { message = response.FirstOrDefault(), token = response.LastOrDefault() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult SubmitPaypalOrder(string token, int paymentOptionId, int shippingId, string additionalNotes)
        {
            if (string.IsNullOrEmpty(token))
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorUnablePlaceOrder));
                return RedirectToAction<OrderController>(x => x.CreateOrder(0, 0));
            }

            SubmitPaymentViewModel model = new SubmitPaymentViewModel();
            model.Token = token;
            model.PaymentSettingId = paymentOptionId;
            model.ShippingOptionId = shippingId;
            model.AdditionalInfo = additionalNotes;

            SubmitOrderViewModel order = _orderAgent.ProcessCreditCardPayment(model, true);
            if (!order.HasError)
            {
                CreateOrderViewModel receipt = new CreateOrderViewModel();
                receipt.OrderId = order.OrderId;
                receipt.ReceiptHtml = WebUtility.HtmlDecode(order.ReceiptHtml);
                return ActionView(AdminConstants.CheckoutReceipt, receipt);
            }
            if (!string.IsNullOrEmpty(order.ErrorMessage))
                SetNotificationMessage(GetErrorNotificationMessage(order.ErrorMessage));

            return RedirectToAction<OrderController>(x => x.CreateOrder(0, 0));
        }

        //Check Group Product Inventory.
        public virtual ActionResult CheckGroupProductInventory(ProductParameterModel parameters, string productSKU = "", string quantity = "")
        => Json(_orderAgent.CheckGroupProductInventory(parameters, productSKU, quantity), JsonRequestBehavior.AllowGet);

        [HttpPost]
        public virtual ActionResult AddCustomShippingAmount(decimal? customShippingCost, decimal? estimateShippingCost, int? userId = 0, bool isQuote = false)
        => Json(_orderAgent.AddCustomShippingAmount(customShippingCost, estimateShippingCost, userId, isQuote), JsonRequestBehavior.AllowGet);

        #endregion

        #region Customer Details

        public virtual ActionResult GetCustomerList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId, int accountId = 0, bool isAccountCustomer = false)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeOrderCustomer.ToString(), model);
            _orderAgent.AddCustomerNameToFilterCollection(model, isAccountCustomer);
            //Get the list of Customers.
            CustomerListViewModel customerList = _orderAgent.GetCustomerList(portalId, accountId, isAccountCustomer, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            customerList.GridModel = FilterHelpers.GetDynamicGridModel(model, customerList.List, GridListType.ZnodeOrderCustomer.ToString(), string.Empty, null, true, true, customerList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set portalId and IsAccountCustomer.
            customerList.PortalId = portalId;
            customerList.AccountId = accountId;
            customerList.IsAccountCustomer = isAccountCustomer;
            //Set the total record count.
            customerList.GridModel.TotalRecordCount = customerList.TotalResults;

            return ActionView(AdminConstants.CustomerListView, customerList);
        }

        //Get Shipping Billing Address and other details of customer.
        public virtual ActionResult GetCustomerDetails(int portalId, int userId)
            => ActionView(AdminConstants.OrderCustomerDetailsView, _orderAgent.GetCustomerDetails(portalId, userId));

        //Get Shipping Billing Address and other details of customer.
        public virtual ActionResult SetCustomerAddressDetails()
        {
            UserAddressDataViewModel userAddressDataViewModel = _userAgent.GetUserAccountViewModel();

            if (IsNull(userAddressDataViewModel))
            {
                userAddressDataViewModel = new UserAddressDataViewModel()
                {
                    BillingAddress = new AddressViewModel(),
                    ShippingAddress = new AddressViewModel()
                };
            }
            List<SelectListItem> countries = HelperMethods.GetCountries();
            //Set countries for billing and shipping.
            userAddressDataViewModel.BillingAddress.Countries = countries;
            userAddressDataViewModel.ShippingAddress.Countries = countries;
            return ActionView(AdminConstants.CreateEditCustomerAddressView, userAddressDataViewModel);
        }

        //Set customer details by user id.
        public virtual ActionResult SetCustomerDetailsById(int userId)
        {
            //Get User details by user id.
            UserViewModel userViewModel = _orderAgent.GetUserDetailsByUserId(userId);

            return Json(new
            {
                status = false,
                message = userViewModel.HasError ? userViewModel.ErrorMessage : Admin_Resources.SaveMessage,
                userId = userViewModel.UserId,
                fullName = userViewModel.FullName
            }, JsonRequestBehavior.AllowGet);
        }

        //Create new customer.
        [HttpPost]
        public virtual ActionResult SetCustomerAddressDetails(UserAddressDataViewModel userAddressDataViewModel)
        {
            //Remove Model state validation if Billing and Shipping address are same.
            if (userAddressDataViewModel.UseSameAsBillingAddress)
                ModelState.RemoveFor<UserAddressDataViewModel>(x => x.ShippingAddress);

            //Remove Model state validation for address Name Field.
            ModelState.Remove(AdminConstants.BillingAddress1Key);
            ModelState.Remove(AdminConstants.ShippingAddress1Key);

            //Check if model state is valid and create new customer.
            if (ModelState.IsValid)
            {
                userAddressDataViewModel = _orderAgent.CreateUpdateCustomerAddress(userAddressDataViewModel);

                if (!userAddressDataViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                    return RedirectToAction<OrderController>(x => x.GetShippingMethods());
                }
            }
            //Get all countries for shipping / billing address
            List<SelectListItem> countries = HelperMethods.GetCountries();
            userAddressDataViewModel.BillingAddress = new AddressViewModel { Countries = countries };
            userAddressDataViewModel.ShippingAddress = new AddressViewModel { Countries = countries };
            return ActionView(AdminConstants.CreateEditOrderCustomerView, userAddressDataViewModel);
        }

        [HttpGet]
        public virtual ActionResult ChangeAddress(int portalId)
        {
            return ActionView("_CreateEditCustomer");
        }

        //
        [HttpGet]
        public virtual ActionResult AddNewCustomer(int portalId)
        {
            CustomerViewModel customerViewModel = new CustomerViewModel
            {
                StoreName = _userAgent.GetStoreName(portalId),
                PortalId = portalId
            };
            return ActionView("CreateCustomer", customerViewModel);
        }

        //Create new customer.
        [HttpPost]
        public virtual ActionResult AddNewCustomer(CustomerViewModel userAddressDataViewModel)
        {
            if (IsNotNull(userAddressDataViewModel.AccountId) && !string.IsNullOrEmpty(userAddressDataViewModel.RoleName) && string.Equals(userAddressDataViewModel.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                userAddressDataViewModel.AccountPermissionList = _userAgent.GetPermissionList(userAddressDataViewModel.AccountId.GetValueOrDefault(), userAddressDataViewModel.AccountPermissionAccessId.GetValueOrDefault());
            }

            if (ModelState.IsValid)
            {
                CustomerViewModel model = _userAgent.CreateCustomerAccount(userAddressDataViewModel);

                if (IsNotNull(model) && !model.HasError)
                {
                    _userAgent.SetOMSCustomerViewModel(userAddressDataViewModel);
                    userAddressDataViewModel.UserId = model.UserId;

                    return ActionView(AdminConstants.CreateCustomerView, userAddressDataViewModel);
                }
            }
            _userAgent.SetOMSCustomerViewModel(userAddressDataViewModel);
            userAddressDataViewModel.Portals = _userAgent.GetPortals();
            if (userAddressDataViewModel?.RoleName?.ToLower() == AdminConstants.UserRoleName)
                _userAgent.GetPermissionList(userAddressDataViewModel.AccountId.GetValueOrDefault(), userAddressDataViewModel.AccountPermissionAccessId.GetValueOrDefault());

            return ActionView(AdminConstants.CreateCustomerView, userAddressDataViewModel);
        }

        //Get customer list by customer fullname.
        [HttpGet]
        public virtual JsonResult GetCustomerListByName(string customerName, int portalId, bool isAccountCustomer = false, int accountId = 0)
            => Json(_orderAgent.GetCustomerListByName(portalId, customerName, isAccountCustomer, accountId), JsonRequestBehavior.AllowGet);

        //Get view to update existing customer address.
        [HttpGet]
        public virtual JsonResult GetAddressById(int userAddressId, string fromBillingShipping, bool isB2BCustomer, int userId, int portalId, int accountId = 0)
        {
            AddressViewModel address = _orderAgent.GetUserAccountAddressByAddressId(userAddressId, isB2BCustomer, portalId);
            address.UserId = userId;
            address.AccountId = accountId;
            address.FromBillingShipping = fromBillingShipping;

            //Get html of _Address view with data.
            string addressView = RenderRazorViewToString("~/Views/Account/_Address.cshtml", address);

            return Json(new { address = address, html = addressView }, JsonRequestBehavior.AllowGet);
        }

        //Set customer details on customer selection.
        public virtual ActionResult BindCustomerDetails(CartParameterModel cartParameter)
       {
            if (cartParameter.IsQuote)
            {
                return ActionView(AdminConstants.CreateQuoteView, _orderAgent.GetUserFullDetailsForQuote(cartParameter));
            }
            else
            {
                return ActionView( AdminConstants.CreateEditOrderView, _orderAgent.GetUserFullDetails(cartParameter));
            }
        }

        #endregion

            #region Shopping Cart Details

            //Get Published Product List
        [ActionSessionState(SessionStateBehavior.ReadOnly)]
        public virtual ActionResult ProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalCatalogId, int portalId, int userId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeOrderProductList.ToString(), model);
            //Set portalCatalog id and portal Id as filter.
            _orderAgent.SetProductListFilter(model, portalCatalogId, portalId, userId);

            //Get the list of all published products.
            PublishProductsListViewModel productList = _orderAgent.GetPublishProducts(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            productList.GridModel = FilterHelpers.GetDynamicGridModel(model, IsNull(productList?.PublishProductsList) ? new List<PublishProductsViewModel>() : productList.PublishProductsList, GridListType.ZnodeOrderProductList.ToString(), string.Empty, null, true, true, productList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            productList.GridModel.TotalRecordCount = productList.TotalResults;

            productList.publishCatalogId = portalCatalogId;
            productList.portalId = portalId;
            productList.UserId = userId;

            return ActionView("_ProductList", productList);
        } 

        //Get shopping cart of selected user.
        public virtual ActionResult GetShoppingCart(int portalId = 0, int userId = 0)
        {
            if (userId > 0)
                //Set User data in session.
                _orderAgent.SaveUserDetailsInSession(portalId, userId);

            return ActionView("ShoppingCart", _cartAgent.GetCart(0, userId));
        }

        [Obsolete("To be discontinued in one of the upcoming versions. Instead of this use AddToCartProduct")]
        //Create new Cart.
        public virtual ActionResult AddToCart(CartItemViewModel cartItem)
            => ActionView(AdminConstants.ShoppingCartView, _cartAgent.CreateCart(cartItem));

        //Update cart item quantity.
        [HttpPost]
        public virtual ActionResult UpdateCartQuantity(string guid, decimal quantity, int productId = 0, int shippingId = 0, bool isQuote = false, int? userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.UpdateCartItemQuantity(guid, quantity, productId, shippingId, userId, isQuote);
            cartViewModel.IsQuote = isQuote;

            if (cartViewModel?.UserId > 0 && cartViewModel.PortalId > 0)
            {
                return GetShoppingCartItems(cartViewModel.UserId, cartViewModel.PortalId, 0, isQuote);

            }
            GetErrorNotificationMessage(Admin_Resources.ErrorFailedToUpdateCart);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        [Obsolete("To be discontinued in one of the upcoming versions. Instead of this use RemoveCartItem")]
        //Removes item from shopping cart.
        [HttpPost]
        public virtual ActionResult RemoveShoppingCartItem(string guid, int orderId = 0, bool isQuote = false, int? userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.RemoveShoppingCartItem(guid, orderId, userId);
            _orderAgent.SetTrackingUrlByOrderId(orderId, cartViewModel?.ShoppingCartItems);

            cartViewModel.IsQuote = isQuote;
            string shoppingCartView = RenderRazorViewToString(AdminConstants.ShoppingCartView, cartViewModel);
            if (cartViewModel.HasError && orderId > 0)
                return Json(new
                {
                    hasError = cartViewModel.HasError,
                    errorMessage = cartViewModel.ErrorMessage
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    html = orderId > 0 ? RenderRazorViewToString(orderLineItemView, cartViewModel) : shoppingCartView,
                    coupons = cartViewModel.Coupons
                }, JsonRequestBehavior.AllowGet);
        }


        //Removes item from shopping cart.
        [HttpPost]
        public virtual ActionResult RemoveCartItem(string guid, int orderId = 0, bool isQuote = false, int userId = 0, int portalId = 0)
        {
            if (_cartAgent.RemoveCartItem(guid, orderId, userId, portalId, isQuote))
            {
                return GetShoppingCartItems(userId, portalId, orderId, isQuote);
            }
            GetErrorNotificationMessage(Admin_Resources.ErrorFailedToUpdateCart);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        //Get Product Price.
        [HttpGet]
        public virtual ActionResult GetProductPrice(int portalId, string productSKU = "", string parentProductSKU = "", string quantity = "", string addOnIds = "", int parentProductId = 0, int omsOrderId = 0, int? userId = 0)
        {
            PublishProductsViewModel viewModel = _orderAgent.GetProductPriceAndInventory(productSKU, parentProductSKU, quantity, addOnIds, portalId, parentProductId, omsOrderId, userId);
            return Json(new
            {
                success = viewModel.ShowAddToCart,
                message = viewModel.InventoryMessage,
                data = new
                {
                    style = viewModel.ShowAddToCart ? "success" : "error",
                    price = HelperMethods.FormatPriceWithCurrency(viewModel.ProductPrice, viewModel.CultureCode),
                    sku = viewModel?.SKU,
                    addOnMessage = !(viewModel?.AddOns?.Count > 0) ? viewModel.AddOns.Select(x => x.InventoryMessage) : null,
                    isOutOfStock = !(viewModel?.AddOns?.Count > 0) ? viewModel.AddOns.Select(x => x.IsOutOfStock) : null,
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [Obsolete("To be discontinued in one of the upcoming versions. Instead of this use AddToCartProduct")]
        // Add multiple product to cart.
        public virtual ActionResult AddProductToCart(bool cartItems, int orderId, int? userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.AddProductToCart(cartItems, orderId, userId);
            _orderAgent.SetTrackingUrlByOrderId(orderId, cartViewModel?.ShoppingCartItems);

            string shoppingCartView = RenderRazorViewToString(orderId > 0 ? orderLineItemView : AdminConstants.ShoppingCartView, cartViewModel);
            return Json(new
            {
                html = shoppingCartView,
                coupons = cartViewModel.Coupons
            }, JsonRequestBehavior.AllowGet);
        }

        [Obsolete("To be discontinued in one of the upcoming versions. Instead of this use AddToCartProduct")]
        // Add to cart user product
        public virtual ActionResult AddToCartUserProduct(CartItemViewModel cartItem)
        {
            CartViewModel cartViewModel = _cartAgent.CreateCart(cartItem);
            return Json(new { success = true });
        }

        // Add to cart user product
        public virtual ActionResult AddToCartProduct(AddToCartViewModel cartItem)
        {
            if (IsNotNull(cartItem) && IsNotNull(_cartAgent.AddToCartProduct(cartItem, cartItem.IsQuote)))
            {
                return GetShoppingCartItems(cartItem.UserId, cartItem.PortalId, 0, cartItem.IsQuote);
            }
            GetErrorNotificationMessage(Admin_Resources.ErrorFailedToAddToCart);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);

        }

        // GET: Get Shopping Cart
        public virtual ActionResult GetShoppingCartItems(int userId, int portalId, int omsOrderId = 0, bool isQuote = false)
        {
            CartViewModel cartViewModel = _cartAgent.GetShoppingCart(userId, portalId, omsOrderId, isQuote);
            cartViewModel.IsQuote = isQuote;
            if (IsNotNull(cartViewModel))
            {
                if (isQuote)
                {
                    return Json(new
                    {
                        html = RenderRazorViewToString(quoteShoppingCartView, cartViewModel),
                        success = true
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    html = RenderRazorViewToString(omsOrderId > 0 ? orderLineItemView : AdminConstants.ShoppingCartView, cartViewModel),
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            GetErrorNotificationMessage(Admin_Resources.ErrorFailedToLoadCart);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        //Calculate Shopping Cart
        public virtual ActionResult CalculateShoppingCart(int userId, int portalId, int orderId = 0, bool isQuote = false)
        {
            CartViewModel cartdata = _cartAgent.CalculateShoppingCart(userId, portalId, orderId, isQuote);
            if (IsNotNull(cartdata))
            {
                cartdata.IsQuote = isQuote;
                return Json(new
                {
                    html = RenderRazorViewToString(orderId > 0 ? "_ManageTotalTable" : "_TotalTable", cartdata),
                    coupons = cartdata.Coupons,
                    subtotal = HelperMethods.FormatPriceWithCurrency(cartdata.SubTotal, cartdata.CultureCode),
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            GetErrorNotificationMessage(Admin_Resources.ErrorFailedToLoadCart);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        // Remove cart items from cart.
        [HttpGet]
        public virtual JsonResult RemoveItemFromCart(string productIds = "", int omsOrderId = 0, bool isRemoveAllCartItems = false, int? userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.RemoveItemFromCart(productIds, omsOrderId, userId);
            string shoppingCartView = RenderRazorViewToString(omsOrderId > 0 ? orderLineItemView : AdminConstants.ShoppingCartView, cartViewModel);
            return Json(new
            {
                html = shoppingCartView,
                isRemoveAllCartItems = isRemoveAllCartItems,
                coupons = cartViewModel.Coupons
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Shipping Details

        [HttpGet]
        public virtual ActionResult GetShippingMethods()
        => ActionView("ShippingOptions", _shippingAgent.BindShippingList(new CreateOrderViewModel(), new ProfileListModel()));

        [HttpGet]
        public virtual JsonResult GetShippingOptionsListWithRates(int? userId = 0, bool excludeCustomShippingFromCreateOrder = false, bool isQuote = false)
        {
            ShippingListViewModel model = _orderAgent.GetShippingListWithRates(userId, excludeCustomShippingFromCreateOrder, isQuote);
            return Json(new { result = RenderRazorViewToString(shippingOptionsView, model) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public virtual JsonResult GetShippingOptionsListForManage(int orderId)
        {
            ShippingListViewModel model = _orderAgent.GetShippingListForManage(orderId);
            return Json(new { ShippingOptionList = model?.ShippingList }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Refund/Void/capture Payment

        /// <summary>
        /// Capture Payment
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <param name="paymentTransactionToken">paymentTransactionToken</param>
        /// <param name="paymentStatus">paymentStatus</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult CapturePayment(int omsOrderId, string paymentTransactionToken, string paymentStatus)
        {
            if (string.Equals(paymentStatus, ZnodeConstant.AUTHORIZED.ToString(), StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(paymentTransactionToken) && omsOrderId > 0)
            {
                string message = string.Empty;
                BooleanModel booleanModel = _orderAgent.CapturePayment(paymentTransactionToken);
                bool status = _orderAgent.UpdateCapturedPaymentDetails(omsOrderId, paymentTransactionToken, true, booleanModel, out message);
                SetNotificationMessage(status ?
                        GetSuccessNotificationMessage(Admin_Resources.PaymentCaptureSuccessMessage) :
                        GetErrorNotificationMessage(!string.IsNullOrEmpty(message) ? message : Admin_Resources.PaymentCaptureErrorMessage));

                return RedirectToAction<OrderController>(x => x.Manage(omsOrderId, 0, null));
            }
            return RedirectToAction<OrderController>(x => x.Manage(omsOrderId, 0, null));
        }

        /// <summary>
        /// void Payment
        /// </summary>
        /// <param name="OmsOrderId">OmsOrderId</param>
        /// <param name="paymentTransactionToken">paymentTransactionToken</param>
        /// <param name="paymentStatus">paymentStatus</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult VoidPayment(int omsOrderId, string paymentTransactionToken, string paymentStatus)
        {
            if ((string.Equals(paymentStatus, ZnodeConstant.AUTHORIZED.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                (string.Equals(paymentStatus, ZnodeConstant.CAPTURED.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                && !string.IsNullOrEmpty(paymentTransactionToken) && omsOrderId > 0)
            {
                string message = string.Empty;
                SetNotificationMessage(_orderAgent.VoidPayment(omsOrderId, paymentTransactionToken, out message) ?
                        GetSuccessNotificationMessage(Admin_Resources.PaymentVoidSuccessMessage) :
                        GetErrorNotificationMessage(!string.IsNullOrEmpty(message) ? message : Admin_Resources.PaymentVoidErrorMessage));

                return RedirectToAction<OrderController>(x => x.Manage(omsOrderId, 0, null));
            }
            return RedirectToAction<OrderController>(x => x.Manage(omsOrderId, 0, null));
        }

        //Get Order Line Items With Refund details
        [HttpGet]
        public virtual ActionResult RefundOrder(int orderDetailsId, int orderId)
        {
            OrderItemsRefundViewModel orderItemsRefundViewModel = _orderAgent.GetOrderLineItemsWithRefund(orderDetailsId);
            if (orderItemsRefundViewModel?.RefundOrderLineitems?.Count > 0)
                return View(orderItemsRefundViewModel);

            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.Error));
            return RedirectToAction<OrderController>(x => x.Manage(orderId, 0, null));
        }

        //Submit Refund
        [HttpPost]
        public virtual ActionResult RefundOrder(OrderItemsRefundViewModel model)
        {
            if (IsNotNull(model))
            {
                string errorMessage;
                SetNotificationMessage(_orderAgent.AddRefundPaymentDetails(model, out errorMessage) ?
                        GetSuccessNotificationMessage(Admin_Resources.PaymentRefundSuccessMessage) :
                        GetErrorNotificationMessage(errorMessage));

                return RedirectToAction<OrderController>(x => x.Manage(model.OmsOrderId, 0, null));
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.PaymentRefundErrorMessage));
            return RedirectToAction<OrderController>(x => x.List(null, 0, 0, 0, null));
        }

        #endregion



        //Get ordered details of user for edit order.
        public virtual ActionResult EditOrder(int omsOrderId = 0, int userId = 0, int accountId = 0, string updatePageType = null)
        => ActionView(AdminConstants.CreateEditOrderView, _orderAgent.EditOrder(omsOrderId, userId, accountId, updatePageType));

        //Get ordered details of user for edit order.
        public virtual ActionResult ReOrder(int omsOrderId = 0, int userId = 0, int accountId = 0, string updatePageType = null)
        {
            //Remove all cart items.
            _cartAgent.RemoveCartSession(userId);

            var model = _orderAgent.ReOrder(omsOrderId, userId, accountId, updatePageType);

            return ActionView(AdminConstants.CreateEditOrderView, model);
        }

        [HttpGet]
        //Reorder complete order.
        public virtual ActionResult ReorderCompleteOrder(int userId, int portalId, int omsOrderId = 0, string actionName = "", int accountId = 0)
        {
            //if not valid input then redirect to list page.
            if (userId > 0 && portalId > 0)
            {
                UserModel userModel = _orderAgent.GetUserDetailById(userId, portalId);

                //check if user is not disabled and is not a guest user
                if (HelperUtility.IsNotNull(userModel) && !Convert.ToBoolean(userModel.IsLock) && !userModel.IsGuestUser)
                {
                    CreateOrderViewModel createOrderViewModel = _orderAgent.ReorderCompleteOrder(userId, portalId, omsOrderId);
                    createOrderViewModel.AccountId = accountId;

                    if(HelperUtility.IsNotNull(createOrderViewModel))
                    {
                        TempData.Keep("ActionName");
                        TempData.Keep("AccountId");
                        //This method is call to get payment Api header as store on the view
                        ViewBag.PaymentApiRequestHeader = _paymentAgent.GetPaymentAuthToken(HttpContext.User.Identity.Name.ToString(), true);
                        StoreListViewModel storeList = _storeAgent.GetStoreList(null, null, null, null);

                        if (storeList?.StoreList?.Count() > 0)
                        {
                            StoreViewModel storeViewModel = storeList.StoreList.Find(x => x.PortalId == portalId);
                            storeViewModel = HelperUtility.IsNotNull(storeViewModel) ? storeViewModel : storeList.StoreList.FirstOrDefault();
                            createOrderViewModel.StoreName = storeViewModel?.StoreName;
                            createOrderViewModel.PortalId = storeViewModel.PortalId;
                        }

                        if (HelperUtility.IsNull(createOrderViewModel.CartViewModel))
                            createOrderViewModel.CartViewModel = new CartViewModel();
                        createOrderViewModel.CartViewModel.UserId = userId;

                        return ActionView(AdminConstants.CreateEditOrderView, createOrderViewModel);
                    }
                }
                else
                {
                    string errorMessage = Convert.ToBoolean(userModel.IsLock) ? Admin_Resources.ErrorFailedToReorderForDeactivateUser : Admin_Resources.ErrorFailedToReorderForGuest;
                    SetNotificationMessage(GetErrorNotificationMessage(errorMessage));
                }
            }
            if (string.Equals(actionName, AdminConstants.CustomerOrderView, StringComparison.CurrentCultureIgnoreCase))
            {
                return RedirectToAction<CustomerController>(x => x.GetOrderList(null, userId));
            }
            if (string.Equals(actionName, AdminConstants.AccountOrderView, StringComparison.CurrentCultureIgnoreCase))
            {
                return RedirectToAction<AccountController>(x => x.AccountUserOrderList(null, 0, accountId));
            }
            return RedirectToAction<OrderController>(x => x.List(null, 0, 0, 0, null));
        }


        //Get order review details.
        [HttpPost]
        public virtual ActionResult GetReviewOrder(CreateOrderViewModel createOrderModel)
        => ActionView(AdminConstants.CheckoutReviewView, _orderAgent.GetReviewOrder(createOrderModel));

        // Checkout receipt after successfull order submit.
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult CheckoutReceipt(int orderId, string ReceiptHtml, bool? IsEmailSend)
        {
            //if order is not present then redirect to list page.
            if (orderId > 0)
            {
                CreateOrderViewModel model = new CreateOrderViewModel();
                //Decode the checkout receipt html.
                model.ReceiptHtml = WebUtility.HtmlDecode(ReceiptHtml);

                //Set order receipt Message if unable to send email receipt.
                if (!Convert.ToBoolean(IsEmailSend))
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToSendOrderReceipt));

                return View(model);
            }
            return RedirectToAction<OrderController>(x => x.List(null, 0, 0, 0, null));
        }

        [HttpGet]
        public virtual ActionResult CheckoutReceipt()
        {
            return RedirectToAction<OrderController>(x => x.CreateOrder(0, 0));
        }

        /// <summary>
        /// To Calculate Shipping Charges
        /// </summary>
        /// <param name="model">Create Order View Model</param>
        /// <returns>Returns View.</returns>
        [HttpPost]
        public virtual ActionResult CalculateShippingCharges(CreateOrderViewModel model)
        => ActionView(AdminConstants.TotalTableView, _orderAgent.GetShippingChargesById(model));

        /// <summary>
        /// To Calculate Shipping Charges.
        /// </summary>
        /// <param name="model">Create Order View Model</param>
        /// <returns>Returns json</returns>
        [HttpPost]
        public virtual JsonResult CalculateShippingInManage(int userId, int shippingId, int omsOrderId)
        {
            CartViewModel cartViewModel = _orderAgent.GetShippingChargesForManage(userId, shippingId, omsOrderId);

            if (!string.IsNullOrEmpty(cartViewModel.ShippingErrorMessage))
            {
                return Json(new
                {
                    shippingErrorMessage = cartViewModel.ShippingErrorMessage
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    CartView = RenderRazorViewToString(orderLineItemView, cartViewModel),
                    Coupons = cartViewModel.Coupons,
                    ShippingType = cartViewModel.ShippingName
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Download Invoice

        //Get order invoice detail in PDF format.
        [HttpPost]
        public virtual ActionResult DownloadPDF(string orderIds)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(orderIds.Trim()))
            {
                var orderInvoiceModel = _orderAgent.GetOrderInvoiceDetails(orderIds);
                //foreach (var item in orderInvoiceModel.Orders)
                //{
                //    StoreViewModel storeViewModel = _storeAgent.GetStore(item.PortalId, null);
                //    item.CustomerServiceEmail = storeViewModel?.CustomerServiceEmail;
                //    item.CustomerServicePhoneNumber = storeViewModel?.CustomerServicePhoneNumber;
                //}
                var htmlContent = RenderRazorViewToString("_OrderInvoice", orderInvoiceModel);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                try
                {
                    var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
                    using (MemoryStream Stream = new MemoryStream(pdfBytes))
                    {
                        // set HTTP response headers
                        HttpContext.Response.Clear();
                        HttpContext.Response.AddHeader("Content-Type", "application/pdf");
                        HttpContext.Response.AddHeader("Cache-Control", "max-age=0");
                        HttpContext.Response.AddHeader("Accept-Ranges", "none");

                        HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=Invoice_" + DateTime.Now.ToString("MMddyyyyhhmmss") + ".pdf");

                        // send the generated PDF
                        Stream.WriteTo(Response.OutputStream);
                        Stream.Close();
                        HttpContext.Response.Flush();
                        HttpContext.Response.End();
                        status = true;
                    }

                    //split order ids to send invoice email accordingly to users.
                    foreach (var item in orderIds.Split(','))
                        SendInvoiceEmail(Convert.ToInt32(item));
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    errorMessage = ex.Message;
                    throw;
                }
            }
            return Json(new { success = status });
        }

        #endregion

        /// <summary>
        /// To apply & validate Coupon
        /// </summary>
        /// <param name="coupon">string coupon</param>
        /// <returns>Returns Calculated Total view</returns>
        [HttpGet]
        public virtual ActionResult ApplyCoupon(string coupon, int? userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.ApplyCoupon(coupon, 0, userId);
            string shoppingCartView = RenderRazorViewToString(AdminConstants.TotalTableView, cartViewModel);
            return Json(new
            {
                html = shoppingCartView,
                coupons = cartViewModel.Coupons
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Removes the applied coupon code from the cart.
        /// </summary>
        /// <param name="coupon">Coupon applied to the cart.</param>
        /// <returns>Returns Json result.</returns>
        [HttpGet]
        public virtual ActionResult RemoveCoupon(string coupon, int? userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.RemoveCoupon(coupon, 0, userId);
            cartViewModel.ErrorMessage = string.IsNullOrEmpty(coupon) ? Admin_Resources.RequiredCouponCode : Admin_Resources.ErrorCouponCode;
            string totalView = RenderRazorViewToString(AdminConstants.TotalTableView, cartViewModel);
            return Json(new
            {
                html = totalView,
                coupons = cartViewModel.Coupons
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To apply & validate GiftCard
        /// </summary>
        /// <param name="number">string number</param>
        /// <returns>Returns Calculated Total view</returns>
        [HttpGet]
        [Obsolete("This method is marked as obsolete from Znode version 9.6.1, instead of this ApplyVoucher method should be used.")]
        public virtual ActionResult ApplyGiftCard(string giftCardNumber, int? userId = 0)
            => ActionView(AdminConstants.TotalTableView, _cartAgent.ApplyGiftCard(giftCardNumber, 0, userId));

        #region Voucher
        /// <summary>
        /// To apply & validate vouchers From Manage Screen.
        /// </summary>
        /// <param name="orderId">order Id</param>
        /// <param name="number">string number</param>
        /// <returns>Returns Calculated cart </returns>       
        [HttpGet]
        public virtual ActionResult ManageApplyVoucher(int orderId, string voucherNumber)
        {
            CartViewModel cartViewModel = _cartAgent.ApplyVoucher(voucherNumber, orderId);
            string totalView = RenderRazorViewToString(manageShoppingCartView, cartViewModel);
            return Json(new
            {
                html = totalView,
                coupons = cartViewModel.Coupons,
                vouchers = cartViewModel.Vouchers,
                giftCardCode = voucherNumber,
                message = cartViewModel.SuccessMessage
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To remove the appied voucher from manage order.
        /// </summary>
        /// <param name="orderId">order Id</param>
        /// <param name="voucher">voucher Number</param>
        /// <param name="userId">User Id</param>
        /// <returns>Returns Calculated cart</returns>
        [HttpGet]
        public virtual ActionResult ManageRemoveVoucher(int orderId, string voucher, int userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.RemoveVoucher(voucher, orderId, userId);
            _orderAgent.SetTrackingUrlByOrderId(orderId, cartViewModel?.ShoppingCartItems);

            return Json(new
            {
                html = RenderRazorViewToString(manageShoppingCartView, cartViewModel),
                vouchers = cartViewModel.Vouchers
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To apply the voucher from create order.
        /// </summary>
        /// <param name="voucherNumber"> Voucher Number</param>
        /// <param name="userId">User Id</param>
        /// <returns>Returns Calculated cart</returns>
        [HttpGet]
        public virtual ActionResult ApplyVoucher(string voucherNumber, int? userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.ApplyVoucher(voucherNumber, 0, userId);
            string totalView = RenderRazorViewToString(AdminConstants.TotalTableView, cartViewModel);
            return Json(new
            {
                html = totalView,
                coupons = cartViewModel.Coupons,
                vouchers = cartViewModel.Vouchers,
                giftCardCode = voucherNumber,
                message = cartViewModel.SuccessMessage
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To remove the voucher from create order.
        /// </summary>
        /// <param name="voucher">voucherNumber</param>
        /// <param name="userId">User Id</param>
        /// <returns>Returns Calculated cart</returns>
        [HttpGet]
        public virtual ActionResult RemoveVoucher(string voucher, int userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.RemoveVoucher(voucher, 0, userId);          
            string totalView = RenderRazorViewToString(AdminConstants.TotalTableView, cartViewModel);
            return Json(new
            {
                html = totalView,
                vouchers = cartViewModel?.Vouchers
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //Get User Address By AddressId
        [Obsolete("Method not in use. Create another method GetUserAddress() instead.")]
        public virtual ActionResult GetUserAddressById(int selectedAddressId, int shippingAddressId, int billingAddressId, int userId, int portalId)
        {
            AddressViewModel addresViewModel = _orderAgent.GetUserAddressById(selectedAddressId, userId, portalId);
            addresViewModel.SelectedBillingId = billingAddressId;
            addresViewModel.SelectedShippingId = shippingAddressId;
            return ActionView("_CustomerAddress", addresViewModel);
        }

        //Get User Address By AddressId
        public virtual ActionResult GetUserAddress(int shippingAddressId, int billingAddressId, int userId, int portalId, string addressType, bool isQuote = false)
        {
            int selectedAddressId = addressType == "billing" ? billingAddressId : shippingAddressId;
            AddressViewModel addresViewModel = _orderAgent.GetUserAddressById(selectedAddressId, userId, portalId);
            addresViewModel.SelectedBillingId = billingAddressId;
            addresViewModel.SelectedShippingId = shippingAddressId;
            addresViewModel.IsQuote = isQuote;
            return ActionView("_CustomerAddress", addresViewModel);
        }

        //Get User Address By AddressId
        public virtual ActionResult GetUserAddressForManageById(int selectedAddressId, int orderId, int userId, int shippingAddressId, int billingAddressId, int portalId, string control = "")
        {
            AddressViewModel addresViewModel = new AddressViewModel();     
            addresViewModel = _orderAgent.GetAddressDefaultDataWithShipment(selectedAddressId, userId, portalId, orderId);
            addresViewModel.IsShippingBillingDifferent = true;
            addresViewModel.SelectedBillingId = billingAddressId;
            addresViewModel.SelectedShippingId = shippingAddressId;
            addresViewModel.FromBillingShipping = control;
            addresViewModel.omsOrderId = orderId;
            if ((addresViewModel.IsShipping && addresViewModel.IsBilling) || (addresViewModel.IsDefaultBilling && addresViewModel.IsDefaultShipping))
            {
                addresViewModel.IsShippingBillingDifferent = false;
            }
            return ActionView(manageCustomerAddressView, addresViewModel);
        }

        //Create New customer Address.
        public virtual ActionResult CreateNewAddress(int userId, int portalId, string addressType = "", int shippingAddressId = 0, int billingAddressId = 0, bool isQuote = false)
        {
            AddressViewModel addressViewModel = null;
            if (string.IsNullOrEmpty(addressType) || addressType == "undefined")
            {
                addressViewModel = new AddressViewModel { UserId = userId, IsDefaultBilling = true, IsDefaultShipping = true, IsShippingAddressChange = true, Countries = HelperMethods.GetPortalAssociatedCountries(portalId), IsQuote = isQuote };

            }
            else
            {
                addressViewModel = new AddressViewModel { UserId = userId, FromBillingShipping = addressType, Countries = HelperMethods.GetPortalAssociatedCountries(portalId), SelectedBillingId = billingAddressId, SelectedShippingId = shippingAddressId, IsQuote = isQuote };
            }
            return ActionView("_CustomerAddress", addressViewModel);

        }

        public virtual ActionResult GetShippingPanel(int omsOrderId)
        {
            ShippingListViewModel shippingList = _orderAgent.GetShippingListForManage(omsOrderId);
            return Json(new { status = shippingList }, JsonRequestBehavior.AllowGet);
        }


        public virtual ActionResult GetAdditionalNotes()
                  => ActionView("_ManageNotes", new OrderViewModel());

        //Update or create customer address.
        [HttpPost]
        public virtual ActionResult ChangeCustomerAddress(AddressViewModel addressViewModel)
        {
            if (ModelState.IsValid)
            {
                CreateOrderViewModel createOrderViewModel = _orderAgent.UpdateCustomerAddressAndCalculate(addressViewModel);

                if (createOrderViewModel.HasError)
                {
                    return Json(new
                    {
                        ErrorMassage = createOrderViewModel.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
                createOrderViewModel.CartViewModel.IsQuote = addressViewModel.IsQuote;
                string totalView = RenderRazorViewToString(AdminConstants.TotalTableView, createOrderViewModel.CartViewModel);
                string addressView = RenderRazorViewToString("UserShippingBillingAddress", createOrderViewModel.UserAddressDataViewModel);

                return Json(new
                {
                    totalView = totalView,
                    addressView = addressView,
                    isShippingAddressChange = addressViewModel.IsShippingAddressChange
                }, JsonRequestBehavior.AllowGet);
            }

            AddressViewModel defaultAddressData = _orderAgent.GetAddressDefaultData(addressViewModel.AddressId, addressViewModel.UserId, addressViewModel.AccountId, addressViewModel.PortalId);
            addressViewModel.UsersAddressNameList = defaultAddressData.UsersAddressNameList;
            addressViewModel.Countries = defaultAddressData.Countries;

            return Json(new
            {
                addressView = RenderRazorViewToString("_CustomerAddress", addressViewModel)
            }, JsonRequestBehavior.AllowGet);
        }

        #region Manage Order

        //Manage order details

        public virtual ActionResult Manage(int OmsOrderId, int accountId = 0, string updatePageType = null)
        {
            OrderViewModel orderViewModel = _orderAgent.Manage(OmsOrderId);
            // This method is call to get payment Api header as store on the view
            ViewBag.PaymentApiRequestHeader = _paymentAgent.GetPaymentAuthToken(HttpContext.User.Identity.Name.ToString(), true);
            if (IsNotNull(TempData["SuccessMessage"]))
                SetNotificationMessage(GetSuccessNotificationMessage(Convert.ToString(TempData["SuccessMessage"])));
            else if (IsNotNull(TempData["ErrorMessage"]))
                SetNotificationMessage(GetErrorNotificationMessage(Convert.ToString(TempData["ErrorMessage"])));
            orderViewModel.UpdatePageType = updatePageType;
            return ActionView("Manage", orderViewModel);
        }

        public virtual ActionResult GetOrderInformation(int orderId)
            => ActionView("_ManageOrderInformation", _orderAgent.GetOrderInformation(orderId));

        public virtual ActionResult GetCustomerInformation(int orderId)
           => ActionView(manageCustomerView, _orderAgent.GetCustomerInformation(orderId));

        public virtual ActionResult GetOrderLineItems(int orderId)
        {
            CartViewModel cartItemList = _orderAgent.GetOrderLineItems(orderId);
            return Json(new
            {
                CartView = RenderRazorViewToString(orderLineItemView, cartItemList),
                Coupons = cartItemList.Coupons
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GetReturnLineItems(int orderId)
            => ActionView("ReturnItemList", _orderAgent.GetReturnLineItemList(orderId));

        public virtual ActionResult GetOrderHistory([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, int orderId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeOmsHistory.ToString(), model);

            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            //Get the list of Orders
            OrderHistoryListViewModel history = _orderAgent.GetOrderHistory(orderId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            history.GridModel = FilterHelpers.GetDynamicGridModel(model, history.List, GridListType.ZnodeOmsHistory.ToString(), string.Empty, null, true, true, history?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            history.GridModel.TotalRecordCount = history.TotalResults;

            //Returns the attribute list view
            return ActionView("_ManageOrderHistory", history);
        }

        public virtual ActionResult GetPaymentHistory(int orderId)
        {
            //Get the list of Orders
            PaymentHistoryListViewModel history = _orderAgent.GetPaymentHistory(orderId);
            
            //Returns the attribute list view
            return ActionView("_ManagePaymentHistory", history);
        }

        public virtual ActionResult GetOrderStateValueById(int omsOrderStateId)
            => Json(new { isEdit = _orderAgent.GetOrderStateValueById(omsOrderStateId) }, JsonRequestBehavior.AllowGet);

         //Update Order In Hands Date.
        [HttpPost]
        public virtual JsonResult UpdateInHandDate(OrderInfoViewModel orderStatus)
        {
            if (IsNotNull(orderStatus) && orderStatus.OmsOrderId > 0)
                return Json(new { InHandDate = orderStatus.InHandDate, response = _orderAgent.UpdateInHandDate(orderStatus.OmsOrderId, orderStatus.InHandDate.ToString()), OmsOrderId = orderStatus.OmsOrderId }, JsonRequestBehavior.AllowGet);
            else
                return Json(new{ErrorMassage = Admin_Resources.UnableToUpdateDate}, JsonRequestBehavior.AllowGet);
        }

        //Update Order Job Name
        [HttpPost]
        public virtual JsonResult UpdateJobName(OrderInfoViewModel orderStatus)
        {
            if (IsNotNull(orderStatus) && orderStatus.OmsOrderId > 0)
                return Json(new
                {
                    orderStatus.JobName,
                    Response = _orderAgent.UpdateJobName(orderStatus.OmsOrderId, orderStatus.JobName),
                    orderStatus.OmsOrderId
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { JobName = string.Empty, Response = Admin_Resources.UnableToUpdateJobName }, JsonRequestBehavior.AllowGet);
        }

        //Update Order Shipping Constraint
        [HttpPost]
        public virtual JsonResult UpdateShippingConstraintCode(OrderInfoViewModel orderStatus)
        {
            if (IsNotNull(orderStatus) && orderStatus.OmsOrderId > 0)
                return Json(new
                {
                    orderStatus.ShippingConstraintCode,
                    Response = _orderAgent.UpdateShippingConstraintCode(orderStatus.OmsOrderId, orderStatus.ShippingConstraintCode),
                    orderStatus.OmsOrderId
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { ShippingConstraintCode = string.Empty, Response = Admin_Resources.UnableToUpdateShippingConstraintCode }, JsonRequestBehavior.AllowGet);
        }

        //Update Order Tracking Number.
        [HttpGet]
        public virtual JsonResult UpdateTrackingNumber(OrderInfoViewModel orderStatus)
         => Json(new { TrackingNumber = orderStatus.TrackingNumber, TrackingUrl = _orderAgent.UpdateTrackingNumber(orderStatus.OmsOrderId, orderStatus.TrackingNumber), OmsOrderId = orderStatus.OmsOrderId }, JsonRequestBehavior.AllowGet);

        //Update Discount
        public virtual ActionResult UpdateDiscount(int omsOrderId, string CSRDiscountAmount, string pageName)
            => ActionView(manageShoppingCartView, _orderAgent.UpdateAmounts(omsOrderId, CSRDiscountAmount, AdminConstants.CSRDiscountAmountView));

        //Update Tax
        public virtual ActionResult UpdateOrderText(int omsOrderId, string orderTextValue, string pageName)
        => ActionView(manageShoppingCartView, _orderAgent.UpdateAmounts(omsOrderId, orderTextValue, pageName));

        //Update Tax on create Order.
        public virtual ActionResult UpdateTaxExemptOnCreateOrder(int userId, string orderTextValue, string pageName, bool isTaxExempt, bool isQuote = false)
        => ActionView(AdminConstants.TotalTableView, _orderAgent.UpdateCartTaxAmounts(userId, orderTextValue, pageName , isTaxExempt, isQuote));

        //Update shipping and Handling
        public virtual ActionResult UpdateShippingHandling(int omsOrderId, string ShippingCost, string pageName)
          => ActionView(manageShoppingCartView, _orderAgent.UpdateAmounts(omsOrderId, ShippingCost, AdminConstants.ShippingView));

        public virtual ActionResult GetTaxCost(int orderId, decimal taxCost)
        => ActionView("_ManageOrderText", new OrderStatusList { OrderTextValue = taxCost.ToPriceRoundOff().ToString(), OmsOrderId = orderId, pageName = AdminConstants.TaxView });

        public virtual ActionResult UpdateShippingType(int omsOrderId, string shippingType)
        {
            if (omsOrderId > 0)
            {
                OrderViewModel viewModel = new OrderViewModel();
                if (IsNotNull(viewModel))
                {
                    return Json(new
                    {
                        success = true,
                        trackingNumber = viewModel.TrackingNumber,
                        isStatusButtonShow = false,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult ManageCustomerAddress(AddressViewModel addressViewModel, int omsOrderId)
        {
            if (addressViewModel.IsGuest)
                ModelState.Remove("DisplayName");

            if (ModelState.IsValid)
            {
                CreateOrderViewModel createOrderViewModel = _orderAgent.UpdateCustomerAddressAndCalculate(addressViewModel, true, omsOrderId);

                if (!string.IsNullOrEmpty(createOrderViewModel.CartViewModel.ShippingErrorMessage))
                {
                    return Json(new
                    {
                        shippingErrorMessage = createOrderViewModel.CartViewModel.ShippingErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (createOrderViewModel.HasError)
                {
                    return Json(new
                    {
                        shippingErrorMessage = createOrderViewModel.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        totalView = RenderRazorViewToString(manageShoppingCartView, createOrderViewModel.CartViewModel),
                        addressView = RenderRazorViewToString(manageCustomerView, _orderAgent.MapAndUpdateCustomerAddress(createOrderViewModel)),
                        orderTotal = RenderRazorViewToString(manageTotalTableView, createOrderViewModel.CartViewModel)
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            AddressViewModel defaultAddressData = _orderAgent.GetAddressDefaultData(addressViewModel.AddressId, addressViewModel.UserId, addressViewModel.AccountId, addressViewModel.PortalId);
            addressViewModel.UsersAddressNameList = defaultAddressData.UsersAddressNameList;
            addressViewModel.Countries = defaultAddressData.Countries;
            return Json(new
            {
                addressView = RenderRazorViewToString(manageCustomerAddressView, addressViewModel)
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult ManageOrderNotes(OrderViewModel model)
            => Json(new { success = _orderAgent.ManageOrderNotes(model) }, JsonRequestBehavior.AllowGet);

        /// <summary>
        /// For Updateing Order Select fields.
        /// </summary>
        /// <param name="model">OrderStatusList</param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult UpdateOrderStatus(OrderStatusList model)
        {
            model.pageName = AdminConstants.OrderStatus;
            return Json(_orderAgent.UpdateOrderAndPaymentStatus(model), JsonRequestBehavior.AllowGet);

        }


        public virtual JsonResult UpdatePaymentStatus(OrderStatusList model)
        {
            model.pageName = AdminConstants.PaymentStatus;
            return Json(_orderAgent.UpdateOrderAndPaymentStatus(model), JsonRequestBehavior.AllowGet);
        }


        //Fetch the List of Order
        public virtual ActionResult ManageOrderStatus(int omsOrderId, string orderStatus, string pageName = null)
           => ActionView("_ManageOrderStatus", _orderAgent.GetOrderStatus(omsOrderId, orderStatus, AdminConstants.OrderStatus));

        //Fetch the list of Payment Status
        public virtual ActionResult ManagePaymentStatus(int omsOrderId, string orderStatus, string pageName = null)
           => ActionView("_ManagePaymentStatus", _orderAgent.GetOrderStatus(omsOrderId, orderStatus, AdminConstants.PaymentStatus));



        //Update Order Payment Status.
        [HttpGet]
        public virtual JsonResult UpdateOrderPaymentStatus(int omsOrderId, string paymentStatus)
        {
            if (omsOrderId > 0)
            {
                OrderViewModel viewModel = _orderAgent.UpdateOrderPaymentStatus(omsOrderId, paymentStatus);
                if (IsNotNull(viewModel))
                {
                    return Json(new
                    {
                        success = true,
                        paymentStatus = viewModel.PaymentStatus,
                        isStatusButtonShow = viewModel.PaymentStatus == ZnodeConstant.PENDING.ToString()
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult ManageTrackingNumber(string trackingNumber, int omsOrderId)
            => ActionView("_ManageTrackingNumber", new OrderStatusList() { OrderTextValue = trackingNumber, OmsOrderId = omsOrderId });


        [HttpGet]
        public virtual JsonResult UpdateShippingAccountNumber(string AccountNumber, int omsOrderId)
         => Json(new { ShippingAccountNumber = AccountNumber, shippingAccountNumber = _orderAgent.UpdateShippingAccountNumber(omsOrderId, AccountNumber), OmsOrderId = omsOrderId }, JsonRequestBehavior.AllowGet);
        [HttpGet]
        public virtual JsonResult UpdateShippingMethod(string ShippingMethod, int omsOrderId)
         => Json(new { ShippingMethod = ShippingMethod, shippingMethod = _orderAgent.UpdateShippingMethod(omsOrderId, ShippingMethod), OmsOrderId = omsOrderId }, JsonRequestBehavior.AllowGet);

        public virtual ActionResult GetPaymentById(int userId, int portalId = 0, string paymentType = "")
        {
            return ActionView("_ManagePaymentOption", new PaymentSettingViewModel() { PaymentTypeList = _orderAgent.BindPaymentList(userId, portalId, paymentType), IsGuest = _orderAgent.CheckForGuestUser(userId) });
        }

        /// <summary>
        /// To apply & validate Coupon from manage screen.
        /// </summary>
        /// <param name="orderId">order Id</param>
        /// <param name="coupon">string coupon</param>
        /// <returns>Returns Calculated Total view</returns>
        [HttpGet]
        public virtual ActionResult ManageApplyCoupon(int orderId, string coupon, int userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.ApplyCoupon(coupon, orderId, userId);
            _orderAgent.SetTrackingUrlByOrderId(orderId, cartViewModel?.ShoppingCartItems);

            return Json(new
            {
                html = RenderRazorViewToString(manageShoppingCartView, cartViewModel),
                coupons = cartViewModel.Coupons
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To apply & validate GiftCard From Manage Screen.
        /// </summary>
        /// <param name="orderId">order Id</param>
        /// <param name="number">string number</param>
        /// <returns>Returns Calculated Total view</returns>
        [HttpGet]
        [Obsolete("This method is marked as obsolete from Znode version 9.6.1, instead of this ManageApplyVoucher method should be used.")]
        public virtual ActionResult ManageApplyGiftCard(int orderId, string giftCardNumber)
            => ActionView(manageShoppingCartView, _cartAgent.ApplyGiftCard(giftCardNumber, orderId));

        [HttpGet]
        public virtual ActionResult ManageRemoveCoupon(int orderId, string coupon, int userId = 0)
        {
            CartViewModel cartViewModel = _cartAgent.RemoveCoupon(coupon, orderId, userId);
            _orderAgent.SetTrackingUrlByOrderId(orderId, cartViewModel?.ShoppingCartItems);

            cartViewModel.ErrorMessage = string.IsNullOrEmpty(coupon) ? Admin_Resources.RequiredCouponCode : Admin_Resources.ErrorCouponCode;
            return Json(new
            {
                html = RenderRazorViewToString(manageShoppingCartView, cartViewModel),
                coupons = cartViewModel.Coupons
            }, JsonRequestBehavior.AllowGet);
        }

        //TO DO Need to change list of parameters into model
        /// <summary>
        /// Updates the Quantity updated in the shopping cart page..
        /// </summary>
        /// <returns>Returns Calculated Total view</returns>
        public virtual ActionResult UpdateCartItem(ManageOrderDataModel orderDataModel)
        {
            CartViewModel cartViewModel = _orderAgent.UpdateOrderLineItemDetails(orderDataModel);
            ReturnOrderLineItemListViewModel returnOrderLineItemListViewModel = _orderAgent.GetReturnLineItemList(orderDataModel.OrderId);
            if ((orderDataModel.CustomQuantity > 0 || Equals(orderDataModel.OrderLineItemStatus, ZnodeOrderStatusEnum.RETURNED.ToString()) && IsNotNull(cartViewModel)))
            {
                return Json(new
                {
                    cartView = RenderRazorViewToString(manageShoppingCartView, cartViewModel),
                    returnLineItemView = returnOrderLineItemListViewModel?.ReturnItemList?.Count > 0 ? RenderRazorViewToString("ReturnItemList", returnOrderLineItemListViewModel) : null,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                CartItemViewModel cartItem = cartViewModel?.ShoppingCartItems?.FirstOrDefault(x => x.ExternalId == orderDataModel.Guid);
                if (IsNotNull(cartItem))
                {
                    return Json(new
                    {
                        totalView = RenderRazorViewToString(manageTotalTableView, cartViewModel),
                        unitPrice = HelperMethods.FormatPriceWithCurrency(cartItem.UnitPrice, cartItem.CultureCode),
                        extendedPrice = HelperMethods.FormatPriceWithCurrency(cartItem.ExtendedPrice, cartItem.CultureCode),
                        quantity = cartItem.GroupProducts?.Count > 0 ? (cartItem.GroupProducts.FirstOrDefault()?.Quantity).GetValueOrDefault().ToInventoryRoundOff() : cartItem.Quantity.ToInventoryRoundOff(),
                        trackingNumber = cartItem.TrackingNumber,
                        hasError = cartViewModel.HasError,
                        errorMessage = cartViewModel.ErrorMessage,
                        orderLineItemStatus = cartItem.OrderLineItemStatus,
                        orderLineItemStatusId = cartItem.OrderLineItemStatusId,
                        isEditStatus = cartItem.IsEditStatus,
                        partialRefund = HelperMethods.FormatPriceWithCurrency(cartItem.PartialRefundAmount, cartItem.CultureCode),
                        shippingCost = HelperMethods.FormatPriceWithCurrency(cartItem.ShippingCost, cartItem.CultureCode),
                        coupons = cartViewModel.Coupons
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    totalView = RenderRazorViewToString(manageTotalTableView, cartViewModel),
                    coupons = cartViewModel.Coupons
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Update manage order.
        [HttpPost]
        public virtual ActionResult UpdateOrder(int orderId, string additionalNote)
        {
            OrderViewModel orderViewModel = _orderAgent.UpdateOrder(orderId, additionalNote);
            if (orderViewModel.HasError)
                return Json(new { hasError = orderViewModel.HasError, errorMessage = orderViewModel.ErrorMessage });
            else
            {
                TempData["SuccessMessage"] = orderViewModel.SuccessMessage;
                return new JsonResult { Data = orderViewModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //To process payment on Payment API using customerProfileId & customerPaymentId
        [HttpPost]
        public virtual JsonResult SubmitEditOrderpayment(SubmitPaymentViewModel submitPaymentViewModel)
        {
            SubmitOrderViewModel submitOrderViewModel = _orderAgent.SubmitEditOrderpayment(submitPaymentViewModel);
            if (!submitOrderViewModel.HasError)
                TempData["SuccessMessage"] = Admin_Resources.UpdateMessage;

            return Json(new
            {
                Data = submitOrderViewModel
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GetReasonsForReturn()
            => Json(new
            {
                data = _rmaRequestAgent.BindReasonForReturn()
            }, JsonRequestBehavior.AllowGet);

        public virtual ActionResult UpdateReturnShippingHistory(int lineItemId, int omsOrderId, bool isInsert)
            => Json(new
            {
                data = _orderAgent.UpdateReturnShippingHistory(lineItemId, omsOrderId, isInsert)
            }, JsonRequestBehavior.AllowGet);

        #endregion

        //Resend order confirmation email
        public virtual ActionResult ResendOrderConfirmationEmail(int omsOrderId)
        {
            SetNotificationMessage(GetSuccessNotificationMessage(_orderAgent.ResendOrderConfirmationEmail(omsOrderId)));
            return RedirectToAction<OrderController>(x => x.Manage(omsOrderId, 0, string.Empty));
        }

        public virtual void ResendOrderLineItemConfirmMail(int omsOrderId, int orderLineItemId)
            => SetNotificationMessage(GetSuccessNotificationMessage(_orderAgent.ResendOrderConfirmationEmailForCart(omsOrderId, orderLineItemId)));

        // Send returned order email.
        public virtual ActionResult SendReturnedOrderEmail(int omsOrderId)
        {
            bool status = _orderAgent.SendReturnedOrderEmail(omsOrderId);
            return Json(new { hasError = status , message = status ? Admin_Resources.EmailSuccess : Admin_Resources.EmailError }, JsonRequestBehavior.AllowGet);
        }

        #region Order Quote

        /// <summary>
        /// The index page for create quote.
        /// </summary>
        /// <param name="portalId">current portal id</param>
        /// <returns>Return view</returns>
        [HttpGet]
        public virtual ActionResult CreateQuote(int portalId = 0)
        {
            //Remove all cart items.

            CreateOrderViewModel createQuote = _orderAgent.GetCreateOrderDetails(portalId);
            createQuote.IsQuote = true;

            return ActionView(createQuote);
        }

        /// <summary>
        /// Submit order.
        /// </summary>
        /// <param name="model">CreateOrderViewModel</param>
        /// <returns>Returns View.</returns>
        [HttpPost]
        public virtual ActionResult SubmitQuote(CreateOrderViewModel model)
        {
            //Submit quote for approval.
            CreateOrderViewModel order = _orderAgent.SubmitQuote(model);
            if (IsNull(order) || (order.HasError && string.IsNullOrEmpty(order.ErrorMessage)))
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorSubmitQuote));
            else
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SuccessQuoteCreated));

            return new JsonResult { Data = order, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Create quote for customer.
        public virtual ActionResult CreateQuoteForCustomer(int userId, int accountId)
        {
            //Remove all cart items.
            return View("~/Views/Quote/CreateQuote.cshtml", _orderAgent.CreateQuoteForCustomer(userId, accountId));
        }

        //Get ordered details of user for edit order.
        public virtual ActionResult ConvertToOrder(int omsQuoteId, string orderStatus)
        {
            CreateOrderViewModel orderViewMode = _quoteAgent.ConvertToOrder(omsQuoteId);
            if (orderViewMode.HasError)
            {
                SetNotificationMessage(GetErrorNotificationMessage(orderViewMode.ErrorMessage));
                return RedirectToAction<QuoteController>(x => x.AccountQuoteList(null));
            }
            return ActionView(AdminConstants.CreateEditOrderView, orderViewMode);
        }

        #endregion

        #region Order status

        /// <summary>
        /// Gets Order status details.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <returns>Returns View</returns>
        public virtual ActionResult OrderStatus(int omsOrderId, int accountId = 0, string updatePageType = null)
        {
            OrderStateParameterViewModel orderModel = _orderAgent.GetOrderStatusDetails(omsOrderId);
            orderModel.OrderStatusList = _orderAgent.BindOrderStatus();
            orderModel.AccountId = accountId;
            orderModel.UpdatePageType = updatePageType;
            return View(orderModel);
        }

        /// <summary>
        /// Posts the order status details for updating the order status.
        /// </summary>
        /// <param name="model">Model of type OrderView model</param>
        /// <returns>Returns View</returns>
        [HttpPost]
        public virtual ActionResult OrderStatus(OrderStateParameterViewModel model)
        {
            if (IsNotNull(model))
            {
                string errorMessage = string.Empty;
                SetNotificationMessage(_orderAgent.UpdateOrderStatus(model) ?
                        GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) :
                        GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            }
            switch (model.UpdatePageType)
            {
                case (AdminConstants.Quotes): return RedirectToAction<QuoteController>(x => x.AccountQuoteList(null));
                case (AdminConstants.AccountQuotes): return RedirectToAction<AccountController>(x => x.AccountQuoteList(null, 0, model.AccountId));
                case (AdminConstants.QuoteHistory): return RedirectToAction<AccountController>(x => x.AccountQuoteList(null, model.UserId, model.AccountId));
                case (AdminConstants.OrderHistory): return RedirectToAction<AccountController>(x => x.AccountUserOrderList(null, model.UserId, model.AccountId));
                default: return RedirectToAction<OrderController>(x => x.Manage(model.OmsOrderId, 0, null));
            }
        }

        #endregion

        //To apply & validate CSR Discount.
        [HttpGet]
        public virtual ActionResult ApplyCSRDiscount(decimal csrDiscount, string csrDesc, int userId = 0)
          => ActionView(AdminConstants.TotalTableView, _cartAgent.ApplyCsrDiscount(csrDiscount, csrDesc, userId));

        //Add new order notes.
        public virtual ActionResult AddOrderNotes(string additionalNotes, int? omsOrderDetailsId, int? omsQuoteId, int omsOrderId = 0)
            => PartialView("_OrderNotes", _orderAgent.AddOrderNote(additionalNotes, omsOrderDetailsId, omsQuoteId, omsOrderId));

        public virtual ActionResult PrintOnManage(int omsOrderId)
            => ActionView("_ManageOrderReceipt", _orderAgent.GetDataForReceipt(omsOrderId));

        public virtual ActionResult PrintPackagingSlip(int omsOrderId, string OmsOrderLineItemsId)
        {
            OrderViewModel model = _orderAgent.GetDataForReceipt(omsOrderId);
            if (!string.IsNullOrEmpty(OmsOrderLineItemsId))
                model.OrderLineItems = model.OrderLineItems?.Where(x => x.OmsOrderLineItemsId == Convert.ToInt32(OmsOrderLineItemsId)).ToList();

            return ActionView("_ManagePackageOrderReceipt", model);
        }

        // Get payment options on payment tab
        public virtual ActionResult GetPaymentMethods(int portalId, int userId)
        => ActionView("PaymentOptionList", _orderAgent.GetPaymentMethods(portalId, userId));

        //Get the cart count of the shopping cart.
        [HttpGet]
        public virtual JsonResult GetCartCount(int portalId, int userId)
        {
            return Json(_cartAgent.GetCartCount(portalId, userId), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Quick Order

        [HttpGet]
        public virtual ActionResult GetQuickOrder()
         => PartialView("_QuickOrder", new AutoComplete());

        //Get Product List By SKU.
        public virtual ActionResult GetProductListBySKU(string query)
        => Json(_orderAgent.GetProductListBySKU(query), JsonRequestBehavior.AllowGet);

        #endregion

        #region Personalize Attribute

        //Get attribute validations list.
        public virtual ActionResult GetPersonalisedAttributes(int productId = 0, Dictionary<string, string> PersonliseValues = null)
        => ActionView("_PersonalisedAttribute", _orderAgent.GetAttributeValidationByCodes(PersonliseValues, productId));

        #endregion

        //Get Portal List
        public virtual ActionResult GetPortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserPortalList.ToString(), model);
            StoreListViewModel storeList = _storeAgent.GetStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeUserPortalList.ToString(), string.Empty, null, true, true);

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView(storeListView, storeList);
        }

        [Obsolete("To be discontinued in one of the upcoming versions. Instead of this use RemoveAllShoppingCartItems")]
        //Remove all cart item from shopping cart.
        public virtual ActionResult RemoveAllCartItem(int? userId = 0)
        => ActionView(AdminConstants.ShoppingCartView, _cartAgent.RemoveAllCart(userId));

        //Remove all cart item from shopping cart.
        public virtual ActionResult RemoveAllShoppingCartItems(int userId, int portalId, bool isFromUserCart = false)
        {
            CartViewModel cartViewModel = _cartAgent.RemoveAllShoppingCartItems(userId, portalId);

            if (IsNotNull(cartViewModel))
            {
                if (isFromUserCart)
                {
                    return ActionView(AdminConstants.UserCartView, _cartAgent.RemoveAllCart(userId));
                }

                return Json(new
                {
                    html = RenderRazorViewToString(AdminConstants.ShoppingCartView, cartViewModel),
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            GetErrorNotificationMessage(Admin_Resources.ErrorFailedToLoadCart);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        public virtual ActionResult SendPOEmail(string receiverEmail, int omsOrderId)
        {
            OrderInvoiceViewModel orderInvoiceModel = GetInvoiceDetails(omsOrderId);
            string htmlContent = RenderRazorViewToString("_OrderInvoice", orderInvoiceModel);

            SendInvoiceViewModel invoiceModel = new SendInvoiceViewModel
            {
                ReceiptHtml = htmlContent,
                PortalId = orderInvoiceModel.Orders[0].PortalId,
                ReceiverEmail = receiverEmail,
                OrderNumber = orderInvoiceModel?.Orders[0]?.OrderNumber
            };
            _orderAgent.SendPOEmail(invoiceModel);
            return RedirectToAction<OrderController>(x => x.Manage(omsOrderId, 0, string.Empty));
        }

        //Get Payment App Header
        public virtual JsonResult GetPaymentAppHeader()
        {
            AjaxHeadersModel response = _orderAgent.GetPaymentAPIHeader();
            return Json(new
            {
                response.Authorization
            }, JsonRequestBehavior.AllowGet);
        }

        //Get Address Details
        [HttpGet]
        public virtual JsonResult GetAddressDetails(int userAddressId, int otherAddressId, string addressType, bool isB2BCustomer, int userId, int portalId, int accountId = 0, bool isQuote = false)
        {
            AddressViewModel address = _orderAgent.GetUserAccountAddressByAddressId(userAddressId, isB2BCustomer, portalId);
            address.UserId = userId;
            address.AccountId = accountId;
            address.FromBillingShipping = addressType;
            address.PortalId = portalId;
            address.IsQuote = isQuote;
            if (addressType == "shipping")
            {
                address.SelectedShippingId = userAddressId;
                address.SelectedBillingId = otherAddressId;
                address.IsShippingAddressChange = true;
            }
            else if (addressType == "billing")
            {
                address.SelectedShippingId = otherAddressId;
                address.SelectedBillingId = userAddressId;
            }

            return Json(ChangeCustomerAddress(address), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get User list based on search term.
        /// </summary>
        /// <param name="searchTerm">searchTerm</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Return list of complete customer details.</returns>
        public virtual JsonResult GetCustomerListBySearchTerm(string searchTerm, int portalId)
            => Json(_orderAgent.GetCustomerList(searchTerm, portalId), JsonRequestBehavior.AllowGet);

        private OrderInvoiceViewModel GetInvoiceDetails(int omsOrderId)
        {
            OrderInvoiceViewModel orderInvoiceModel = _orderAgent.GetOrderInvoiceDetails(Convert.ToString(omsOrderId));
            StoreViewModel storeViewModel = _storeAgent.GetStore(orderInvoiceModel.Orders[0].PortalId, null);
            orderInvoiceModel.Orders[0].CustomerServiceEmail = storeViewModel?.CustomerServiceEmail;
            orderInvoiceModel.Orders[0].CustomerServicePhoneNumber = storeViewModel?.CustomerServicePhoneNumber;
            //Get the available WebSite Logo details.
            WebSiteLogoViewModel model = _websiteAgent.GetWebSiteLogoDetails(orderInvoiceModel.Orders[0].PortalId);
            orderInvoiceModel.Orders[0].StoreLogo = model?.LogoUrl;
            return orderInvoiceModel;
        }

        private void SendInvoiceEmail(int OmsOrderId)
        {
            OrderViewModel orderViewModel = _orderAgent.Manage(OmsOrderId);
            var orderInvoiceModel = GetInvoiceDetails(orderViewModel.OmsOrderId);

            var htmlContent = RenderRazorViewToString("_OrderInvoice", orderInvoiceModel);

            if (orderViewModel?.PaymentType?.ToLower() == "purchase_order" && orderViewModel?.OrderState?.ToUpper() == ZnodeOrderStatusEnum.SHIPPED.ToString())
            {
                SendInvoiceViewModel invoiceModel = new SendInvoiceViewModel();
                invoiceModel.ReceiptHtml = htmlContent;
                invoiceModel.PortalId = orderViewModel.PortalId;
                invoiceModel.ReceiverEmail = orderViewModel?.UserName;
                invoiceModel.OrderNumber = orderViewModel?.OrderNumber;
                _orderAgent.SendPOEmail(invoiceModel);
            }
        }

        /// <summary>
        /// This method fetches the list of all the failed orders transaction list.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel model</param>
        /// <returns>Returns the list of failed transaction detail</returns>
        public virtual ActionResult FailedOrderTransactionList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Remove DateTimeRange Filter From Cookie.
            DateRangePickerHelper.RemoveDateTimeRangeFiltersFromCookies(GridListType.ZnodeOmsFailedOrderPayments.ToString(), model);

            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeOmsFailedOrderPayments.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeOmsFailedOrderPayments.ToString(), model);

            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            //Get the list of Orders
            FailedOrderTransactionListViewModel failedOrderTransactionListVM = _orderAgent.FailedOrderTransactionList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            failedOrderTransactionListVM.GridModel = FilterHelpers.GetDynamicGridModel(model, failedOrderTransactionListVM.FailedOrderTransactionListVM, GridListType.ZnodeOmsFailedOrderPayments.ToString(), string.Empty, null, true, true);

            //Set the total record count
            failedOrderTransactionListVM.GridModel.TotalRecordCount = failedOrderTransactionListVM.TotalResults;

            //Returns the attribute list view
            return ActionView(failedOrderTransactionListVM);
        }


        [HttpPost]
        public virtual ActionResult GetAuthorizeNetToken(PaymentTokenViewModel paymentTokenModel)
        {
            var data = _orderAgent.GetCustomerDetails(paymentTokenModel.PortalId, paymentTokenModel.UserId);
            paymentTokenModel.AddressListViewModel = new AddressListViewModel();
            paymentTokenModel.AddressListViewModel.BillingAddress = data.BillingAddress;
            paymentTokenModel.AddressListViewModel.ShippingAddress = data.ShippingAddress;
            paymentTokenModel.AddressListViewModel.EmailAddress = data.Email;

            PaymentTokenViewModel paymentTokenViewModel = _paymentAgent.GetPaymentGatewayToken(paymentTokenModel);
            string authorizeIFrameHtml = RenderRazorViewToString("_AuthorizeNet", paymentTokenViewModel);
            return Json(new
            {
                html = authorizeIFrameHtml,
                customerProfileId = paymentTokenViewModel.CustomerProfileId
            }, JsonRequestBehavior.AllowGet);
        }

       

        [AllowAnonymous]
        public ActionResult AuthorizeIframeCommunicator()
        {     // This method calls when payment get cancel or not success. Write the code of cancelation or warning
            return View();
        }

        /// <summary>
        /// Get data token for cyber source. 
        /// </summary>
        /// <returns>Return iframe for cyner source payment.</returns>
        [HttpPost]
        public virtual ActionResult GetPaymentGatewayToken(PaymentTokenViewModel paymentTokenModel)
        {
            PaymentTokenViewModel paymentTokenViewModel = _paymentAgent.GetPaymentGatewayToken(paymentTokenModel);

            string auth = RenderRazorViewToString("_CyberSourceGateway", paymentTokenViewModel);
            return Json(new
            {
                html = auth,
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get data token for Braintree. 
        /// </summary>
        /// <returns>Return hosted fields for Braintree payment.</returns>
        [HttpPost]
        public virtual ActionResult GetIframeViewWithToken(PaymentTokenViewModel paymentTokenModel, string partialView)
        {
            PaymentTokenViewModel paymentTokenViewModel = _paymentAgent.GetPaymentGatewayToken(paymentTokenModel);
            if (!string.IsNullOrEmpty(paymentTokenViewModel.PaymentGatewayToken))
            {
                string auth = RenderRazorViewToString(partialView, paymentTokenViewModel);
                return Json(new
                {
                    isSuccess = true,
                    html = auth,
                    customerProfileId = paymentTokenViewModel.CustomerProfileId
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    isSuccess = false,
                    error = WebStore_Resources.ErrorConnectingPaymentMethod
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}