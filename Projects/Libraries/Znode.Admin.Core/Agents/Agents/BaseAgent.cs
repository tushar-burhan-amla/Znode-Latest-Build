using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Admin.Core.Agents;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public abstract class BaseAgent
    {
        public ExpandCollection Expands { get; set; }
        public FilterCollection Filters { get; set; }
        public SortCollection Sorts { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        private int count = 1;

        /// <summary>
        /// Get API client object with current domain name and key.
        /// </summary>
        /// <typeparam name="T">The type of API client object.</typeparam>
        /// <returns>An API client object of type T.</returns>
        protected T GetClient<T>() where T : class
        {
            var obj = Activator.CreateInstance<T>();

            if (!(obj is BaseClient)) return obj;
            if (!(obj as BaseClient).IsGlobalAPIAuthorization)
            {
                // Need to use Url.Authority to set domain name and its API key for multi store
                var urlAuthority = HttpContext.Current.Request.Url.Authority;
                (obj as BaseClient).DomainName = urlAuthority;
                (obj as BaseClient).DomainKey = ConfigurationManager.AppSettings[urlAuthority];
            }
            else
            {
                (obj as BaseClient).DomainName = ConfigurationManager.AppSettings["ZnodeApiDomainName"];
                (obj as BaseClient).DomainKey = ConfigurationManager.AppSettings["ZnodeApiDomainKey"];
            }

            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var model = SessionProxyHelper.GetUserDetails();
                if (HelperUtility.IsNotNull(model))
                {
                    (obj as BaseClient).UserId = model.UserId;
                    (obj as BaseClient).RefreshCache = true;
                }
            }

            return obj;
        }

        /// <summary>
        /// Get API client object with current domain name and key.
        /// </summary>
        /// <typeparam name="T">The type of API client object.</typeparam>
        /// <returns>An API client object of type T.</returns>
        protected T GetClient<T>(T obj) where T : IBaseClient
        {
            if (!(obj is BaseClient)) return obj;
            if (!(obj as BaseClient).IsGlobalAPIAuthorization)
            {
                // Need to use Url.Authority to set domain name and its API key for multi store
                var urlAuthority = HttpContext.Current.Request.Url.Authority;
                (obj as BaseClient).DomainName = urlAuthority;
                (obj as BaseClient).DomainKey = ConfigurationManager.AppSettings[urlAuthority];
            }
            else
            {
                (obj as BaseClient).DomainName = ConfigurationManager.AppSettings["ZnodeApiDomainName"];
                (obj as BaseClient).DomainKey = ConfigurationManager.AppSettings["ZnodeApiDomainKey"];
            }

            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var model = SessionProxyHelper.GetUserDetails();
                if (HelperUtility.IsNotNull(model))
                {
                    (obj as BaseClient).UserId = model.UserId;
                    (obj as BaseClient).RefreshCache = true;
                }
            }

            ICustomHeaders _headerAgent = DependencyResolver.Current.GetService<ICustomHeaders>();

            if (HelperUtility.IsNotNull(_headerAgent))
            {
                Dictionary<string, string> headers = _headerAgent.SetCustomHeaderOfClient();

                int? count = headers?.Count;

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        switch (i.ToString())
                        {
                            case "0":
                                (obj as BaseClient).Custom1 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                                break;
                            case "1":
                                (obj as BaseClient).Custom2 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                                break;
                            case "2":
                                (obj as BaseClient).Custom3 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                                break;
                            case "3":
                                (obj as BaseClient).Custom4 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                                break;
                            case "4":
                                (obj as BaseClient).Custom5 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                                break;
                        }
                    }
                }
            }
            return obj;
        }

        protected T GetComponentClient<T>(T obj) where T : Znode.Libraries.Abstract.Client.IBaseClient
        {
            if (!(obj is Znode.Libraries.Abstract.Client.BaseClient)) return obj;
            if (!(obj as Znode.Libraries.Abstract.Client.BaseClient).IsGlobalAPIAuthorization)
            {
                // Need to use Url.Authority to set domain name and its API key for multi store
                var urlAuthority = HttpContext.Current.Request.Url.Authority;
                (obj as Znode.Libraries.Abstract.Client.BaseClient).DomainName = urlAuthority;
                (obj as Znode.Libraries.Abstract.Client.BaseClient).DomainKey = ConfigurationManager.AppSettings[urlAuthority];
            }
            else
            {
                (obj as Znode.Libraries.Abstract.Client.BaseClient).DomainName = ConfigurationManager.AppSettings["ZnodeApiDomainName"];
                (obj as Znode.Libraries.Abstract.Client.BaseClient).DomainKey = ConfigurationManager.AppSettings["ZnodeApiDomainKey"];
            }

            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var model = SessionProxyHelper.GetUserDetails();
                if (HelperUtility.IsNotNull(model))
                {
                    (obj as Znode.Libraries.Abstract.Client.BaseClient).UserId = model.UserId;
                    (obj as Znode.Libraries.Abstract.Client.BaseClient).RefreshCache = true;
                }
            }

            ICustomHeaders _headerAgent = DependencyResolver.Current.GetService<ICustomHeaders>();

            if (HelperUtility.IsNotNull(_headerAgent))
            {
                Dictionary<string, string> headers = _headerAgent.SetCustomHeaderOfClient();

                int? count = headers?.Count;

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        switch (i.ToString())
                        {
                            case "0":
                                (obj as Znode.Libraries.Abstract.Client.BaseClient).Custom1 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                                break;
                            case "1":
                                (obj as Znode.Libraries.Abstract.Client.BaseClient).Custom2 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                                break;
                            case "2":
                                (obj as Znode.Libraries.Abstract.Client.BaseClient).Custom3 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                                break;
                            case "3":
                                (obj as Znode.Libraries.Abstract.Client.BaseClient).Custom4 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                                break;
                            case "4":
                                (obj as Znode.Libraries.Abstract.Client.BaseClient).Custom5 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                                break;
                        }
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// Gets a cookie value.
        /// </summary>
        /// <param name="key">The key for the value being retrieved.</param>
        /// <returns>The value for the key.</returns>
        protected string GetFromCookie(string key)
        {
            string value = CookieHelper.GetCookieValue<string>(key);
            return HelperUtility.IsNull(value) ? string.Empty : value;
        }

        /// <summary>
        /// Gets an object from session.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="key">The key for the session object being retrieved.</param>
        /// <returns>The object of type T from session.</returns>
        protected T GetFromSession<T>(string key)
        {
            return SessionHelper.GetDataFromSession<T>(key);
        }

        /// <summary>
        /// Saves a cookie value.
        /// </summary>
        /// <param name="key">The key for the cookie value.</param>
        /// <param name="value">The cookie value.</param>
        protected void SaveInCookie(string key, string value) =>
            CookieHelper.SetCookie(key, value, ZnodeConstant.MinutesInAYear);

        /// <summary>
        /// Removes a cookie value.
        /// </summary>
        /// <param name="key">The key for the cookie value being removed.</param>
        protected void RemoveCookie(string key) =>
             CookieHelper.RemoveCookie(key);


        /// <summary>
        /// Saves an object in session.
        /// </summary>
        /// <typeparam name="T">The type of object being saved.</typeparam>
        /// <param name="key">The key for the session object.</param>
        /// <param name="value">The value of the session object.</param>
        protected void SaveInSession<T>(string key, T value)
        {
            SessionHelper.SaveDataInSession<T>(key, value);
        }

        /// <summary>
        /// Removes an object from session.
        /// </summary>
        /// <param name="key">The key of the session object.</param>
        protected void RemoveInSession(string key)
        {
            SessionHelper.RemoveDataFromSession(key);
        }

        protected void SetStatusMessage(BaseViewModel model)
        {
            string errorMsg = GetFromSession<string>("ErrorMessage");
            string successMsg = GetFromSession<string>("SuccessMessage");
            if (errorMsg != null)
            {
                model.ErrorMessage = errorMsg;
                model.HasError = true;
                RemoveInSession("ErrorMessage");
            }
            else if (successMsg != null)
            {
                model.SuccessMessage = successMsg;
                RemoveInSession("SuccessMessage");
            }
        }

        /// <summary>
        /// Replace filter key name
        /// </summary>
        /// <param name="filters">Reference of filter collection</param>
        /// <param name="keyName">Old Key Name</param>
        /// <param name="newKeyName">New key name</param>
        protected void ReplaceFilterKeyName(ref FilterCollection filters, string keyName, string newKeyName)
        {
            FilterCollection tempCollection = new FilterCollection();
            tempCollection = filters;
            FilterCollection newCollection = new FilterCollection();

            foreach (var tuple in filters)
            {
                if (Equals(tuple.Item1.ToLower(), keyName.ToLower()))
                    newCollection.Add(new FilterTuple(newKeyName, tuple.Item2, tuple.Item3));
            }
            foreach (var temp in tempCollection)
            {
                if (!Equals(temp.Item1.ToLower(), keyName.ToLower()))
                    newCollection.Add(temp);
            }
            filters = newCollection;
        }

        /// <summary>
        /// Replace sort key name
        /// </summary>
        /// <param name="sort">Sort collection</param>
        /// <param name="keyName">Old key name</param>
        /// <param name="newKeyName">New key name</param>
        protected void ReplaceSortKeyName(SortCollection sort, string keyName, string newKeyName)
        {
            if (!Equals(sort, null) && !Equals(sort.Count, 0))
            {
                if (sort["sort"].ToString().ToLower().Equals(keyName.ToLower()))
                {
                    sort["sort"] = newKeyName;
                }
            }
        }

        /// <summary>
        /// This method will log the general message in the admin log files
        /// </summary>
        /// <param name="message">string message to be logged</param>
        protected void LogMessage(string message)
        {
            //ZnodeLoggingBase.LogAdminMessage(message);
        }

        ///// <summary>
        ///// This Method Is Use To Set Data For Tab
        ///// </summary>
        ///// <param name="tabName"> String For Tab Name</param>
        ///// <param name="tabUrl">String For Tab URL</param>
        ///// <param name="tabList">Tablist view model</param>
        /////

        /// <summary>
        /// This Method Is Use To Set Data For Tab
        /// </summary>
        /// <param name="tabName">String For Tab Name</param>
        /// <param name="tabUrl">String For Tab URL</param>
        /// <param name="tabList">Tablist view model</param>
        /// <param name="isSelected">parameter to select the tab </param>
        protected void SetTabData(string tabName, string tabUrl, TabViewListModel tabList, bool isSelected = false)
        {
            tabList.Tabs.Add(new TabViewModel() { Id = count, Name = tabName, IsVisible = true, Url = tabUrl, IsSelected = isSelected });
            count++;
        }

        /// <summary>
        /// This Method Is Use To Set Data For Tab
        /// </summary>
        /// <param name="tabs">Tab Data In Key Value Pair</param>
        /// <returns></returns>
        protected TabViewListModel SetTabData(Dictionary<string, string> tabs = null)
        {
            TabViewListModel tabList = new TabViewListModel();
            tabList.MaintainAllTabData = true;
            int count = 1;
            if (!Equals(tabs, null))
            {
                foreach (var item in tabs)
                {
                    tabList.Tabs.Add(new TabViewModel() { Id = count, Name = item.Key.ToString(), IsVisible = true, Url = item.Value.ToString(), IsSelected = true });
                    count++;
                }
            }
            return tabList;
        }

        /// <summary>
        /// Set List Paging Details.
        /// </summary>
        /// <param name="listViewModel">List of View Model</param>
        /// <param name="listModel">Api List Model</param>
        protected void SetListPagingData(BaseViewModel listViewModel, BaseListModel listModel)
        {
            listViewModel.Page = Convert.ToInt32(listModel.PageIndex);
            listViewModel.RecordPerPage = Convert.ToInt32(listModel.PageSize);
            listViewModel.TotalPages = Convert.ToInt32(listModel.TotalPages);
            listViewModel.TotalResults = Convert.ToInt32(listModel.TotalResults);
        }

        /// <summary>
        /// Removes keys from bind data model which are not Attributes of the Group.
        /// To be used if all the controlls are created by CreateControl method.
        /// </summary>
        /// <param name="bindDataModel">Bind data model.</param>
        protected void RemoveNonAttributeKeys(BindDataModel bindDataModel)
        {
            foreach (var item in bindDataModel.ControlsData.Where(kvp => !kvp.Key.ToString().EndsWith("_attr")).ToList())
                bindDataModel.ControlsData.Remove(item.Key);
        }

        /// <summary>
        /// Removes attributes from bind data model which are empty or null value.
        /// To be used if all the controlls are created by CreateControl method.
        /// </summary>
        /// <param name="bindDataModel">Bind data model.</param>
        protected void RemoveAttributeWithEmptyValue(BindDataModel bindDataModel)
        {
            foreach (var item in bindDataModel.ControlsData.Where(kvp => string.IsNullOrEmpty(kvp.Value.ToString())).ToList())
                bindDataModel.ControlsData.Remove(item.Key);
        }

        /// <summary>
        /// Removes attributes from bind data model which are empty or null value.
        /// </summary>
        /// <param name="bindDataModel">Bind data model.</param>
        /// <param name="attributes">attribute list </param>
        protected void RemoveAttributeWithEmptyValue(BindDataModel bindDataModel, List<GlobalAttributeValuesViewModel> attributes)
        {
            foreach (var item in bindDataModel.ControlsData.Where(kvp => string.IsNullOrEmpty(kvp.Value.ToString())).ToList())
            {
                string key = item.Key.Split('_')?.First();
                string attributeValue= (attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, key, StringComparison.InvariantCultureIgnoreCase))?.AttributeValue);
                if (string.IsNullOrEmpty(attributeValue))
                    bindDataModel.ControlsData.Remove(item.Key);
            }
        }

        /// <summary>
        /// Removes _attr and mceEditor keyword from attributes.
        /// </summary>
        /// <param name="bindDataModel"></param>
        protected void RemoveAttrAndMceEditorKeyWord(BindDataModel bindDataModel)
        {
            Dictionary<string, object> keysToUpdate = new Dictionary<string, object>();
            foreach (var item in bindDataModel.ControlsData.Where(kvp => kvp.Key.ToString().EndsWith("_attr")).ToList())
            {
                string key = item.Key.Replace("_attr", string.Empty);
                key = item.Key.Contains("mceEditor") ? item.Key.Replace("mceEditor", string.Empty) : key;
                keysToUpdate.Add(key, item.Value);
            }
            bindDataModel.ControlsData.Clear();
            bindDataModel.ControlsData = new Dictionary<string, object>(keysToUpdate);
        }

        /// <summary>
        /// Get BaseViewModel with HasError and ErrorMessage set.
        /// </summary>
        /// <param name="viewModel">View model to set.</param>
        /// <param name="errorMessage">Error message to set.</param>
        /// <returns>Returns BaseViewModel with HasError and ErrorMessage set.</returns>
        protected BaseViewModel GetViewModelWithErrorMessage(BaseViewModel viewModel, string errorMessage)
        {
            viewModel.HasError = true;
            viewModel.ErrorMessage = errorMessage;
            return viewModel;
        }

        /// <summary>
        /// Set locale filter if not present
        /// </summary>
        /// <param name="filters">filters</param>
        public void SetLocaleFilterIfNotPresent(ref FilterCollection filters)
        {
            filters = HelperUtility.IsNull(filters) ? new FilterCollection() : filters;

            if (filters.Where(x => x.Item1.ToLower() == ZnodeLocaleEnum.LocaleId.ToString().ToLower()).Count() < 1)
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, DefaultSettingHelper.DefaultLocale));
        }

        /// <summary>
        /// Set display order sort if not present
        /// </summary>
        /// <param name="sorts">sorts</param>
        public void SetDisplayOrderSortIfNotPresent(ref SortCollection sorts)
        {
            if (HelperUtility.IsNull(sorts))
            {
                sorts = new SortCollection();
                if (sorts.Where(x => x.Key.ToLower() == ZnodePimCategoryHierarchyEnum.DisplayOrder.ToString().ToLower()).Count() < 1)
                    sorts.Add(ZnodePimCategoryHierarchyEnum.DisplayOrder.ToString(), DynamicGridConstants.ASCKey);
            }
        }

        //Get Image path.
        public void SetImagePath(PublishProductListModel productListModel)
        {
            IMediaConfigurationClient _mediaConfigurationClient = GetClient<MediaConfigurationClient>();
            MediaConfigurationViewModel media = _mediaConfigurationClient.GetDefaultMediaConfiguration().ToViewModel<MediaConfigurationViewModel>();

            //Create media image path.
            string mediapath = $"{ media.URL}{media.ThumbnailFolderName}/";

            productListModel?.PublishProducts?.ForEach(publishedProduct => SetProductImagePath(publishedProduct, mediapath));
        }

        //Sets image path for the product.
        private void SetProductImagePath(PublishProductModel publishedProduct, string mediapath)
        {
            string imageName = publishedProduct?.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues;

            if (publishedProduct.Attributes.Where(attribute => attribute.AttributeCode == ZnodeConstant.ProductImage)?.Count() > 0)
            {
                publishedProduct.Attributes.FirstOrDefault(attribute => attribute.AttributeCode == ZnodeConstant.ProductImage).AttributeValues =
                       string.IsNullOrEmpty(imageName) ? ZnodeAdminSettings.DefaultImagePath : $"{ mediapath}{imageName}";
            }
        }

        /// <summary>
        /// //Set value for locale dropdown as per selected in locale dropdown.
        /// </summary>
        /// <param name="localeList">List of Locale</param>
        /// <param name="localeId">Current locale is selected</param>
        /// <returns></returns>
        public virtual List<SelectListItem> SetLocaleValueForDropdown(List<SelectListItem> localeList, string localeId)
        {
            foreach (SelectListItem item in localeList)
            {
                item.Selected = false;
                if (Equals(item.Value, localeId))
                    item.Selected = true;
            }
            return localeList;
        }

        /// <summary>
        /// Set value for True/False drop down
        /// </summary>
        /// <returns>returns list of True and false</returns>
        public virtual List<SelectListItem> GetBooleanList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "True", Value = "true" });
            selectList.Add(new SelectListItem() { Text = "False", Value = "false" });
            return selectList;
        }

        //Get the grid model for binding the tools.
        public virtual GridModel GetGridModel()
            => new GridModel { FilterColumn = new FilterColumnListModel { ToolMenuList = new List<ToolMenuModel>() } };

        /// <summary>
        /// This method used for converting dictionary to list.
        /// </summary>
        /// <param name="columns">columns dictionary</param>
        /// <returns></returns>
        public List<CustomColumnViewModel> CustomAttributeColumn(Dictionary<string, object> columns)
        {
            var _list = new List<CustomColumnViewModel>();
            if (!Equals(columns, null))
            {
                foreach (var item in columns)
                {
                    _list.Add(new CustomColumnViewModel { FieldName = item.Key, DisplayName = Convert.ToString(item.Value) });
                }
            }

            return _list;
        }

        /// <summary>
        /// This method used to save shoppingCartModel in session
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="shoppingCartModel">shoppingCartModel</param>
        /// <returns></returns>
        protected virtual void SaveCartModelInSession(int? userId, ShoppingCartModel shoppingCartModel, bool isQuote = false) =>
            SaveInSession(SetCartModelSessionKey(userId, isQuote), shoppingCartModel);

        /// <summary>
        /// This method used to get shoppingCartModel in session
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>cart(return the value of shopping cart model from the session)</returns>
        protected virtual ShoppingCartModel GetCartModelFromSession(int? userId, bool isQuote = false)
        {
            return GetFromSession<ShoppingCartModel>(SetCartModelSessionKey(userId, isQuote));
        }

        /// <summary>
        /// This method used to remove shoppingCartModel in session
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        protected virtual void RemoveCartModelInSession(int? userId) =>
            RemoveInSession(SetCartModelSessionKey(userId));

        /// <summary>
        /// This method used to set CartModel session key
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>Session Key</returns>
        protected virtual string SetCartModelSessionKey(int? userId, bool isQuote = false)
        {
            if (isQuote)
            {
                return userId > 0 ? AdminConstants.CartModelSessionKey + "_" + AdminConstants.QuoteCartSessionKey + userId : AdminConstants.CartModelSessionKey;
            }
            else
            {
                return userId > 0 ? AdminConstants.CartModelSessionKey + "_" + userId : AdminConstants.CartModelSessionKey;
            }
        }

    }
}
