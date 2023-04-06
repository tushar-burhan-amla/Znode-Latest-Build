using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System.IO;
using Znode.Engine.WebStore.Helpers;

namespace Znode.Engine.WebStore
{
    public static class Helper
    {
        #region Private Variables
        private static readonly IMessageAgent _messageAgent;
        #endregion
        #region Constructor
        static Helper()
        {
            IWebstoreHelper helper = GetService<IWebstoreHelper>();
            _messageAgent = helper.MessageAgent();
        }
        #endregion
        //Get message.
        public static string GetMessage(string key, string controller)
        => _messageAgent.GetMessage(key, controller);

        //For Price according to currency
        public static string FormatPriceWithCurrency(decimal? price, string cultureName, string UOM = "")
        {
            if (HelperUtility.IsNotNull(price))
            {
                string cultureValue;
                cultureName = string.IsNullOrEmpty(cultureName) ? PortalAgent.CurrentPortal?.CultureCode : cultureName;
                if (!string.IsNullOrEmpty(cultureName))
                {
                    CultureInfo info = new CultureInfo(cultureName);
                    info.NumberFormat.CurrencyDecimalDigits = Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff);
                    cultureValue = $"{price.GetValueOrDefault().ToString("c", info.NumberFormat)}";
                }
                else
                    cultureValue = Convert.ToString(price);

                return !string.IsNullOrEmpty(UOM) ? $"{cultureValue} / {UOM}" : cultureValue;
            }
            return null;
        }

        public static bool InfinityLoading
        {
            get
            {
                return (HelperUtility.IsNotNull(SessionHelper.GetDataFromSession<bool>("ViewAllMode"))) ? SessionHelper.GetDataFromSession<bool>("ViewAllMode") : Convert.ToBoolean(ConfigurationManager.AppSettings["InfinityLoading"]);
            }
        }

        public static bool IsAsyncPrice
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["IsAsyncPrice"]);

            }
        }

        //Set Cart parameters for Add To cart.
        public static void SetProductCartParameter(ProductViewModel product, string AddOnProductSKUs = null)
        {
            if (HelperUtility.IsNotNull(product))
            {
                string sku = !string.IsNullOrEmpty(product.ConfigurableProductSKU) ? product.SKU : product.Attributes.Value(ZnodeConstant.ProductSKU);
                product.CartParameter = new Dictionary<string, string>();
                product.ProductType = product.Attributes.SelectAttributeList(ZnodeConstant.ProductType)?.FirstOrDefault()?.Code;
                product.CartParameter.Add("ProductId", product.PublishProductId.ToString());
                product.CartParameter.Add("SKU", sku);
                product.CartParameter.Add("ProductType", product.ProductType);
                product.CartParameter.Add("Quantity", product.Attributes.Value(ZnodeConstant.MinimumQuantity));
                product.CartParameter.Add("ParentProductId", product.ParentProductId.ToString());
                product.CartParameter.Add("ConfigurableProductSKUs", product.ConfigurableProductSKU);
                product.CartParameter.Add("AddOnProductSKUs", AddOnProductSKUs ?? string.Empty);
                product.CartParameter.Add("PersonalisedCodes", string.Empty);
                product.CartParameter.Add("PersonalisedValues", string.Empty);
                if (product.AddOns.Count > 0)
                    product.CartParameter.Add("AutoAddonSKUs", string.Join(",", product.AddOns.Where(x => x.IsAutoAddon)?.Select(y => y.AutoAddonSKUs)));
                switch (product.ProductType)
                {
                    case ZnodeConstant.GroupedProduct:
                        product.CartParameter.Add("GroupProductSKUs", string.Empty);
                        product.CartParameter.Add("GroupProductsQuantity", string.Empty);
                        break;
                    case ZnodeConstant.BundleProduct:
                        product.CartParameter.Add("BundleProductSKUs", string.Empty);
                        break;
                    case ZnodeConstant.ConfigurableProduct:
                        product.CartParameter.Add("ConfigurableProductQuantity", string.Empty);
                        break;
                    default:
                        break;
                }
            }
        }

        //Set Cart parameters for Add To cart.
        public static void SetProductCartParameter(ShortProductViewModel product)
        {
            if (HelperUtility.IsNotNull(product))
            {
                string sku = !string.IsNullOrEmpty(product.ConfigurableProductSKU) ? product.SKU : product.Attributes.Value(ZnodeConstant.ProductSKU);
                product.CartParameter = new Dictionary<string, string>();
                product.MiscellaneousDetails.ProductType = product.Attributes.SelectAttributeList(ZnodeConstant.ProductType)?.FirstOrDefault()?.Code;
                product.CartParameter.Add("ProductId", product.PublishProductId.ToString());
                product.CartParameter.Add("SKU", sku);
                product.CartParameter.Add("ProductType", product.MiscellaneousDetails.ProductType);
                product.CartParameter.Add("Quantity", product.Attributes.Value(ZnodeConstant.MinimumQuantity));
                product.CartParameter.Add("ParentProductId", product.ParentProductId.ToString());
                product.CartParameter.Add("ConfigurableProductSKUs", product.ConfigurableProductSKU);
                product.CartParameter.Add("AddOnProductSKUs", string.Empty);
                product.CartParameter.Add("PersonalisedCodes", string.Empty);
                product.CartParameter.Add("PersonalisedValues", string.Empty);
                if (product.AddOns.Count > 0)
                    product.CartParameter.Add("AutoAddonSKUs", string.Join(",", product.AddOns.Where(x => x.IsAutoAddon)?.Select(y => y.AutoAddonSKUs)));
                switch (product.MiscellaneousDetails.ProductType)
                {
                    case ZnodeConstant.GroupedProduct:
                        product.CartParameter.Add("GroupProductSKUs", string.Empty);
                        product.CartParameter.Add("GroupProductsQuantity", string.Empty);
                        break;
                    case ZnodeConstant.BundleProduct:
                        product.CartParameter.Add("BundleProductSKUs", string.Empty);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void AddIntoCache<T>(T model, string cacheKey, string CacheDurationSettingKey)
        {
            //Cache settings.
            CacheDurationSettings cacheDurationSetting = CacheDurationSettings.GetCacheDurationSettingsByMappingKey(CacheDurationSettingKey);

            HttpRuntime.Cache.Insert(cacheKey, model, null, DateTime.UtcNow.AddMinutes(Equals(cacheDurationSetting, null)
                ? 0 : Convert.ToDouble(cacheDurationSetting.Value)), Cache.NoSlidingExpiration);
        }

        public static void AddIntoCache<T>(T model, string cacheKey)
        {
            HttpRuntime.Cache.Insert(cacheKey, model);
        }
        public static T GetFromCache<T>(string cacheKey)
        {
            var model = HttpRuntime.Cache[cacheKey];
            return (T)Convert.ChangeType(model, typeof(T));
        }

        public static void ClearCache(string cacheKey)
        => HttpContext.Current.Cache.Remove(cacheKey);


        /// <summary>
        /// Sets the filter for userId property.
        /// </summary>
        /// <param name="filters">Filters to set for userId.</param>
        /// <param name="userId">Value to set for userId.</param>
        public static void SetUserIdFilters(FilterCollection filters, int userId)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                //Checking for Id already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == ZnodeNoteEnum.UserId.ToString()))
                {
                    //If Id Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == ZnodeNoteEnum.UserId.ToString());

                    //Add New Id Into filter.
                    filters.Add(new FilterTuple(ZnodeNoteEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodeNoteEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            }
        }

        //Get User Details.
        public static UserViewModel GetUserDetails()
        {
            IWebstoreHelper helper = GetService<IWebstoreHelper>();
            UserViewModel currentUser = helper.GetUserViewModelFromSession();
            return currentUser;
        }

        public static int? GetLocaleId()
        {
            int localeId = (PortalAgent.LocaleId == 0) ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : PortalAgent.LocaleId;
            return localeId;
        }

        public static int? GetProfileId()
        {
            IWebstoreHelper helper = GetService<IWebstoreHelper>();
            UserViewModel currentUser = helper.GetUserViewModelFromSession();
            return currentUser?.Profiles?.Count > 0 ? currentUser?.ProfileId > 0 ? currentUser.ProfileId : currentUser.Profiles?.FirstOrDefault(x => x.IsDefault.GetValueOrDefault())?.ProfileId : (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated) ? 0 : PortalAgent.CurrentPortal.ProfileId;
        }

        //To get the account Id of user.
        public static int GetUserAccountId()
        {
            IWebstoreHelper helper = GetService<IWebstoreHelper>();
            UserViewModel currentUser = helper.GetUserViewModelFromSession();            
            return HelperUtility.IsNotNull(currentUser) ? currentUser.AccountId.GetValueOrDefault() : 0;
        }

        public static int? GetCurrentPortalId()=>
             PortalAgent.CurrentPortal.PortalId;


        //Round off the quantity of cart item/cartcount.
        public static string GetRoundOffQuantity(decimal quantity)
        {
            string productQuantity = quantity.ToString();
            if (!string.IsNullOrEmpty(productQuantity) && productQuantity != "0")
            {
                productQuantity = Convert.ToDecimal(productQuantity).ToInventoryRoundOff();
                if (productQuantity.Contains("."))
                {
                    decimal count = Convert.ToDecimal(productQuantity.Split('.')[1]);
                    if (!(count > 0))
                        productQuantity = productQuantity.Split('.')[0];
                }
            }
            return productQuantity;
        }

        //Round off the quantity of cart item/cartcount.
        public static string GetRoundOffQuantity(string quantity)
        {
            string productQuantity = quantity;
            if (!string.IsNullOrEmpty(productQuantity) && productQuantity != "0")
            {
                productQuantity = Convert.ToDecimal(productQuantity, CultureInfo.InvariantCulture).ToInventoryRoundOff();
                if (productQuantity.Contains("."))
                {
                    decimal count = Convert.ToDecimal(productQuantity.Split('.')[1]);
                    if (!(count > 0))
                        productQuantity = productQuantity.Split('.')[0];
                }
            }
            return productQuantity;
        }

        /// <summary>
        /// Gets the folder name for current theme.
        /// </summary>
        /// <returns></returns>
        public static string GetThemeFolderPath()
        {
            return "~/Views/Themes/" + PortalAgent.CurrentPortal.Theme;
        }

        //Method is use to get theme path.
        public static string GetPortalThemePath()
           => $"/Views/Themes/{PortalAgent.CurrentPortal.Theme}";

        /// <summary>
        /// Gets the complete file path for theme file based on it's existence in child or parent theme.
        /// </summary>
        /// <param name="themeFolderRootPath"></param>
        /// <param name="relativeFilePath"></param>
        /// <returns></returns>
        public static string GetThemeFilePath(string themeFolderRootPath, string relativeFilePath)
        {
            string result = string.Empty;

            if (RelationalThemeHelper.ThemeFileExists(themeFolderRootPath, PortalAgent.CurrentPortal.Theme, relativeFilePath))
                result = themeFolderRootPath + PortalAgent.CurrentPortal.Theme + relativeFilePath;
            else if (!string.IsNullOrEmpty(PortalAgent.CurrentPortal.ParentTheme) && RelationalThemeHelper.ThemeFileExists(themeFolderRootPath, PortalAgent.CurrentPortal.ParentTheme, relativeFilePath))
                result = themeFolderRootPath + PortalAgent.CurrentPortal.ParentTheme + relativeFilePath;

            return result;
        }


        //Replace Home text with Home icon
        public static string ReplaceHomeIconInBreadcrumb(string breadcrumbHtml)
        => breadcrumbHtml?.Replace("<a href='/'>" + WebStore_Resources.LinkTextHome + "</a> /", "<a href='/' class='home-icon pr-5'></a>");

        public static bool GetCMSMode() => HttpContext.Current.Request.QueryString["cmsmode"] == "CMS_MODE" ? true : false;

        //Check whether the Logged in user has valid Session as well as Valid Auth Cookie.
        public static bool IsUserSessionsValid()
        {
            bool isValidSessionData = true;

            UserViewModel currentUser = GetUserDetails();

            //Validate whether the logged in user has the valid Session data.
            if (IsNotNull(currentUser) && string.IsNullOrEmpty(SessionProxyHelper.GetAuthenticatedUserData()))
            {
                isValidSessionData = false;
            }
            else if (!string.IsNullOrEmpty(SessionProxyHelper.GetAuthenticatedUserData()))
            {
                //Validate whether the logged in user has the valid cookie. i.e. Check whether the Auth cookie expired or not.
                HttpCookie authCookie = CookieHelper.GetCookie(FormsAuthentication.FormsCookieName);
                if (authCookie == null)
                {
                    isValidSessionData = false;
                }
            }
            return isValidSessionData;
        }

        //Check whether cms page search is enable or not
        public static bool IsEnableCMSPageSearch()
         =>  (PortalAgent.CurrentPortal.PortalFeatureValues?.FirstOrDefault(x => x.Key == HelperUtility.StoreFeature.Enable_CMS_Page_Results.ToString()).Value).GetValueOrDefault();

        //gets the currency assigned to the current portal
        public static string GetPortalCurrency() =>
             PortalAgent.CurrentPortal.CurrencyCode;

        //Checks whether enhanced ecommerce tracking is enabled for the current portal
        public static bool IsEnhancedEcommerceTrackingEnabled() =>
             PortalAgent.CurrentPortal.EnableEnhancedEcommerce;

        //Get PublishCatalogId from cache.
        public static int? GetUserProfileCatalogId()
        {
            if (HttpContext.Current.User != null)
            {
                string cachekey = string.Concat("UserProfileCatalogId", HttpContext.Current.User.Identity.Name, Convert.ToString(PortalAgent.CurrentPortal?.PortalId));
                if (HelperUtility.IsNotNull(HttpRuntime.Cache[cachekey]))
                {
                    return CacheHelper.GetFromCache<int>(cachekey);
                }
            }
            return 0;
        }

        //Get current portal publish state
        public static string GetPublishState()
        {
            string publishState = Convert.ToString(PortalAgent.CurrentPortal.PublishState);
            return !string.IsNullOrEmpty(publishState) ? publishState : "PRODUCTION";
        }

        //Is Add/Edit Address Allowed.
        public static bool IsAddEditAddressAllow(string roleName)
        {
            bool isAddEditAddressAllow = true;

            if (!string.IsNullOrEmpty(roleName))
                isAddEditAddressAllow = !(string.Equals(roleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(roleName, ZnodeRoleEnum.User.ToString(), StringComparison.OrdinalIgnoreCase));
            return isAddEditAddressAllow;
        }

        //Get address list for dropdown.
        public static List<SelectListItem> GetAddressListDropDown(string optionType, AddressListViewModel Model)
        {
            List<SelectListItem> selectListItem = new List<SelectListItem>();

            // If address type is shipping generate shipping address list.
            if (string.Equals(optionType, WebStoreConstants.ShippingAddressType, StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (AddressViewModel address in Model.AddressList.ToList())
                {
                    if (Model.AccountId > 0 && !string.IsNullOrEmpty(Model.RoleName) && !address.IsBilling)
                    {
                        selectListItem.Add(new SelectListItem() { Text = address.DisplayName, Value = address.AddressId.ToString() });
                    }
                    else if (address.IsShipping || address.IsDefaultShipping)
                    {
                        selectListItem.Add(new SelectListItem() { Text = address.DisplayName, Value = address.AddressId.ToString() });
                    }
                }
            }
            // If address type is billing generate billing address list.
            else
            {
                foreach (AddressViewModel address in Model.AddressList.ToList())
                {
                    if (Model.AccountId > 0 && !string.IsNullOrEmpty(Model.RoleName) && !address.IsShipping)
                    {
                        selectListItem.Add(new SelectListItem() { Text = address.DisplayName, Value = address.AddressId.ToString() });
                    }
                    else if (address.IsBilling || address.IsDefaultBilling)
                    {
                        selectListItem.Add(new SelectListItem() { Text = address.DisplayName, Value = address.AddressId.ToString() });
                    }
                }
            }
            return selectListItem;
        }

        //Perform encryption and return encrypted value
        public static string GetEncryptedValue(string value)
        {
            string encryptedValue = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                encryptedValue = Helpers.Encryption.EncryptPaymentToken(value.Replace(",", "."));
            }

            return encryptedValue;
        }

        //Get the content container based on the containerKey
        public static ContentContainerDataViewModel GetContentContainer(string containerKey)
        => _messageAgent.GetContainer(containerKey);

        //Set category true/false as per attribute HideCategoryOnMenu value set for subcategory 
        public static bool IsPresent(CategoryHeaderViewModel category)
        {
            int subCategoryHideCategoryOnMenuTrue = 0;
            int? subCategoryHideCategoryOnMenuCount = category?.SubCategoryItems.Count;
            category?.SubCategoryItems?.ForEach(x =>
            {

                if (string.Equals(x?.Attributes?.FirstOrDefault(y => String.Equals(y.AttributeCode, WebStoreConstants.HideCategoryOnMenu, StringComparison.InvariantCultureIgnoreCase))?.AttributeValues, ZnodeConstant.True, StringComparison.InvariantCultureIgnoreCase))
                    subCategoryHideCategoryOnMenuTrue++;
            });
            return subCategoryHideCategoryOnMenuCount == subCategoryHideCategoryOnMenuTrue ? false :
                (subCategoryHideCategoryOnMenuCount > 1 && (subCategoryHideCategoryOnMenuTrue < subCategoryHideCategoryOnMenuCount)) ||
                (string.Equals(subCategoryHideCategoryOnMenuTrue.ToString(), WebStoreConstants.ZeroValue,StringComparison.InvariantCultureIgnoreCase) && string.Equals(subCategoryHideCategoryOnMenuCount.ToString(),WebStoreConstants.OneValue,StringComparison.InvariantCultureIgnoreCase)) ? true : false;
        }

        public static bool IsBoolen(string booleanKey)
        {
            bool isBoolen = false;
            bool.TryParse(booleanKey, out isBoolen);
            return isBoolen;
        }

        //Get Guid By Media Url
        public static string GetGuidByMediaUrl(string url)
        {
            url = HttpUtility.UrlDecode(url);
            string guid = null;
            if (url != null)
            {
               string imagePath = url.Substring(url.LastIndexOf('/') + 1);
                if (imagePath != null)
                    guid = imagePath.Split('?')[0];
            }
            return EncodeBase64(guid);

        }
    }
}