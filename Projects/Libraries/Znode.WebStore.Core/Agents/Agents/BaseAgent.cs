using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.WebStore.Core.Agents.IAgent;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore
{
    public abstract class BaseAgent
    {
        public ExpandCollection Expands { get; set; }
        public FilterCollection Filters { get; set; }
        public SortCollection Sorts { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        int count = 1;

        #region Protected Methods
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

            // Set account ID to get the profile based API response
            if (HelperUtility.IsNotNull(HttpContext.Current.User) && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                    (obj as BaseClient).UserId = GetUserId();
            }

            (obj as BaseClient).PublishStateHeader = GetPublishState().ToString();
            (obj as BaseClient).LocaleHeader = GetStoreLocale().ToString();
            (obj as BaseClient).ImpersonationHeader = GetImpersonationCSRId().ToString();
            (obj as BaseClient).ImpersonationPortalHeader = GetImpersonationPortalId().ToString();
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
                    (obj as BaseClient).UserId = GetUserId();
            }

            (obj as BaseClient).PublishStateHeader = GetPublishState().ToString();
            (obj as BaseClient).LocaleHeader = GetStoreLocale().ToString();
            (obj as BaseClient).ImpersonationHeader = GetImpersonationCSRId().ToString();
            (obj as BaseClient).ImpersonationPortalHeader = GetImpersonationPortalId().ToString();
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
              (obj as Znode.Libraries.Abstract.Client.BaseClient).UserId = GetUserId();
            }

        (obj as Znode.Libraries.Abstract.Client.BaseClient).PublishStateHeader = GetPublishState().ToString();
            (obj as Znode.Libraries.Abstract.Client.BaseClient).LocaleHeader = GetStoreLocale().ToString();
            (obj as Znode.Libraries.Abstract.Client.BaseClient).ImpersonationHeader = GetImpersonationCSRId().ToString();
            (obj as Znode.Libraries.Abstract.Client.BaseClient).ImpersonationPortalHeader = GetImpersonationPortalId().ToString();
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

        //Get currently logged in user details.
        public virtual UserViewModel GetUserDetails()
        {
            UserViewModel model = null;
            try
            {
                if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    IUserClient client = new UserClient();

                    //get user details from session if session dat is not null.
                    if (Equals(GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey), null))
                    {
                        //Get the User Details.
                        //Don't Use the Agent here, it will cause the infinite looping. As this method gets called from the BaseAgent.
                        UserModel userModel = client.GetAccountByUser(HttpContext.Current.User.Identity.Name);
                        if (HelperUtility.IsNotNull(userModel))
                        {
                            SaveInSession<UserViewModel>(WebStoreConstants.UserAccountKey, userModel.ToViewModel<UserViewModel>());
                            model = userModel.ToViewModel<UserViewModel>();
                        }
                        client = null;
                    }
                    else
                        //Set value from Session Variable
                        model = GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return model;
        }

        //Get logged in user UserId from cookie.
        //This method also update the cookie value based on the logged in user details.
        public virtual int GetUserId()
        {
            int userId = 0;
            try
            {
                string userCookie = CookieHelper.GetCookie(WebStoreConstants.UserIdCookie)?.Value;

                if (!string.IsNullOrEmpty(userCookie))
                int.TryParse(DecodeBase64(userCookie), out userId);
                //Check userId greater than 0.
                if (userId <= 0)
                {
                    userId = (GetUserDetails()?.UserId).GetValueOrDefault() ;
                    SetLoginUserCookie(userId);
                } 
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return userId;
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
        protected virtual T GetFromSession<T>(string key) => SessionHelper.GetDataFromSession<T>(key);

        // Get a Deep Copy of the session object to remove the reference type dependency of session object.
        protected virtual T GetCloneFromSession<T>(string key)
        {
            T sessionObject = SessionHelper.GetDataFromSession<T>(key);
            return HelperUtility.CreateDeepCloneObject(sessionObject);
        }

        /// <summary>
        /// Saves a cookie value.
        /// </summary>
        /// <param name="key">The key for the cookie value.</param>
        /// <param name="value">The cookie value.</param>
        protected void SaveInCookie(string key, string value,double ExpireTime) =>
            CookieHelper.SetCookie(key, value, ExpireTime);

        /// <summary>
        /// Removes a cookie value.
        /// </summary>
        /// <param name="key">The key for the cookie value being removed.</param>
        protected void RemoveCookie(string key)
        {
            CookieHelper.RemoveCookie(key);
        }

        /// <summary>
        /// Saves an object in session.
        /// </summary>
        /// <typeparam name="T">The type of object being saved.</typeparam>
        /// <param name="key">The key for the session object.</param>
        /// <param name="value">The value of the session object.</param>
        protected void SaveInSession<T>(string key, T value) => SessionHelper.SaveDataInSession<T>(key, value);

        /// <summary>
        /// Removes an object from session.
        /// </summary>
        /// <param name="key">The key of the session object.</param>
        protected void RemoveInSession(string key) => SessionHelper.RemoveDataFromSession(key);

        /// <summary>
        /// Set status message.
        /// </summary>
        /// <param name="model"></param>
        protected void SetStatusMessage(BaseViewModel model)
        {
            if (GetFromSession<string>("ErrorMessage") != null)
            {
                model.ErrorMessage = GetFromSession<string>("ErrorMessage");
                model.HasError = true;
                RemoveInSession("ErrorMessage");
            }
            else if (GetFromSession<string>("SuccessMessage") != null)
            {
                model.SuccessMessage = GetFromSession<string>("SuccessMessage");
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

        //Get Mandatory Filters.
        //If there is any filter is added/removed in this method
        //Then also need to made the same changes in 'GenerateCacheKeyForBreadCrumb' method.
        protected FilterCollection GetRequiredFilters()
        {
            FilterCollection filters = new FilterCollection();

            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, Convert.ToString(GetCatalogId()));
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString());
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString());
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);


            return filters;
        }

        //Get Filter For Brand.
        protected FilterCollection GetBrandFilter()
        {
            FilterCollection filters = new FilterCollection
            {
                { ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString() },
                { ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString()},
                { ZnodeBrandDetailEnum.IsActive.ToString(), FilterOperators.Is, ZnodeConstant.TrueValue },
            };
            return filters;
        }
        //Set Profile Id Filter.
        [Obsolete]
        protected void SetProfileIdFilter(FilterCollection filters)
        {
            Dictionary<string, bool> PortalFeatureValues = PortalAgent.CurrentPortal.PortalFeatureValues;
            if (PortalFeatureValues?.Count > 0 && PortalFeatureValues.ContainsKey(HelperUtility.StoreFeature.Enable_Profile_Based_Search.ToString()) && PortalFeatureValues[HelperUtility.StoreFeature.Enable_Profile_Based_Search.ToString()])
            {
                IWebstoreHelper helper = GetService<IWebstoreHelper>();
                UserViewModel currentUser = helper.GetUserViewModelFromSession();
                //If catalog is not associated to account then profile base catalog work.
                if (HelperUtility.IsNull(currentUser?.PublishCatalogId))
                {
                    int? profileId = currentUser?.Profiles?.Count > 0 ? currentUser?.ProfileId > 0 ? currentUser.ProfileId : currentUser.Profiles?.Where(x => x.IsDefault.GetValueOrDefault())?.FirstOrDefault()?.ProfileId : PortalAgent.CurrentPortal.ProfileId;
                    filters.Add(WebStoreEnum.ProfileIds.ToString(), FilterOperators.In, Convert.ToString(profileId));
                }
            }
        }

        //Get Profile Id.
        protected static int? GetProfileId()
        {
            Dictionary<string, bool> PortalFeatureValues = PortalAgent.CurrentPortal.PortalFeatureValues;
            if (PortalFeatureValues?.Count > 0 && PortalFeatureValues.ContainsKey(HelperUtility.StoreFeature.Enable_Profile_Based_Search.ToString()) && PortalFeatureValues[HelperUtility.StoreFeature.Enable_Profile_Based_Search.ToString()])
            {
                IWebstoreHelper helper = GetService<IWebstoreHelper>();
                UserViewModel currentUser = helper.GetUserViewModelFromSession();
                //If catalog is not associated to account then profile base catalog work.
                if (HelperUtility.IsNull(currentUser?.PublishCatalogId))
                    return currentUser?.Profiles?.Count > 0 ? currentUser.ProfileId > 0 ? currentUser?.ProfileId : currentUser.Profiles?.FirstOrDefault(x => x.IsDefault.GetValueOrDefault())?.ProfileId : PortalAgent.CurrentPortal.ProfileId;
            }
            return null;
        }

        protected int? GetCatalogId(bool isFromUserLogin = false)
        {
            UserViewModel currentUser = GetUserViewModelFromSession(isFromUserLogin);

            if (GetUserProfileId(currentUser) > 0)
                return GetProfilePublishCatalogId(currentUser);
            else if (HelperUtility.IsNotNull(currentUser) && currentUser.AccountId > 0 && currentUser.PublishCatalogId > 0)
            {
                SetUserProfileCatalogId(currentUser.PublishCatalogId);
                return currentUser.PublishCatalogId;
            }
            else if (HelperUtility.IsNull(currentUser))
                return GetStoreCatalogId();
            else
                return PortalAgent.CurrentPortal.PublishCatalogId;
        }

        //Get user view model from session.
        protected virtual UserViewModel GetUserViewModelFromSession(bool isFromUserLogin = false)
        {
            if (isFromUserLogin)
            {
                return (GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey) ?? GetFromSession<UserViewModel>(WebStoreConstants.GuestUserKey));
            }
            IWebstoreHelper helper = GetService<IWebstoreHelper>();
            return helper.GetUserViewModelFromSession();
        }

        //Get publish Catalog id associated with store Profile.
        protected virtual int? GetStoreCatalogId()
        {
            if(PortalAgent.CurrentPortal.ProfileId > 0 && PortalAgent.CurrentPortal.PortalProfileCatalogId > 0)
            {
                SetUserProfileCatalogId(PortalAgent.CurrentPortal.PortalProfileCatalogId);
                return PortalAgent.CurrentPortal.PortalProfileCatalogId;
            }
            return PortalAgent.CurrentPortal.PublishCatalogId;
        }

        //Get Catalog id associated with Profile.
        protected int? GetProfilePublishCatalogId(UserViewModel currentUser)
        {
            int? publishCatalogId = 0;
            publishCatalogId = currentUser?.Profiles?.FirstOrDefault(x => x.IsDefault.GetValueOrDefault())?.PublishCatalogId;
            if (publishCatalogId > 0)
            {
                SetUserProfileCatalogId(publishCatalogId);
                return publishCatalogId;
            }
            else
                return PortalAgent.CurrentPortal.PublishCatalogId;
        }

        //Get Profile Id.
        protected int? GetUserProfileId(UserViewModel currentUser)
        {
            Dictionary<string, bool> PortalFeatureValues = PortalAgent.CurrentPortal.PortalFeatureValues;
            if (PortalFeatureValues?.Count > 0 && PortalFeatureValues.ContainsKey(HelperUtility.StoreFeature.Enable_Profile_Based_Search.ToString()) && PortalFeatureValues[HelperUtility.StoreFeature.Enable_Profile_Based_Search.ToString()])
            {
                if(HelperUtility.IsNotNull(currentUser))
                    return currentUser.Profiles?.Count > 0 ? currentUser.ProfileId > 0 ? currentUser?.ProfileId : currentUser.Profiles?.FirstOrDefault(x => x.IsDefault.GetValueOrDefault())?.ProfileId : PortalAgent.CurrentPortal.ProfileId;
            }
            return null;
        }

        // Removes keys from bind data model which are not Attributes of the Group.
        protected void RemoveNonAttributeKeys(BindDataModel bindDataModel)
        {
            foreach (var item in bindDataModel.ControlsData.Where(kvp => !kvp.Key.ToString().EndsWith("_attr")).ToList())
                bindDataModel.ControlsData.Remove(item.Key);
        }

        //Removes attributes from bind data model which are empty or null value.
        protected void RemoveAttributeWithEmptyValue(BindDataModel bindDataModel)
        {
            foreach (var item in bindDataModel.ControlsData.Where(kvp => string.IsNullOrEmpty(kvp.Value.ToString())).ToList())
                bindDataModel.ControlsData.Remove(item.Key);
        }

        /// <summary>
        /// Get publish state from cookie, if cookie is null we set default 0 in cookie.
        /// </summary>
        /// <returns></returns>
        protected static ZnodePublishStatesEnum GetPublishState()
        {
            if (!CookieHelper.IsCookieExists("_WebStorePublishState") || HelperUtility.IsNull(CookieHelper.GetCookieValue<string>("_WebStorePublishState")))
            {
                ZnodePublishStatesEnum Default = 0;
                SetPublishState(Default);
            }
            return (ZnodePublishStatesEnum)Enum.Parse(typeof(ZnodePublishStatesEnum), CookieHelper.GetCookieValue<string>("_WebStorePublishState"), true);
        }

        /// <summary>
        /// Set publish state in cookie
        /// </summary>
        /// <param name="znodePublishStates"></param>
        protected static void SetPublishState(ZnodePublishStatesEnum znodePublishStates)
        {
            CookieHelper.SetCookie("_WebStorePublishState", znodePublishStates.ToString());
        }

        /// <summary>
        /// Set Locale in cookie
        /// </summary>
        /// <param name="localeId"></param>
        protected static void SetStoreLocale(int localeId)
        {
            CookieHelper.SetCookie("_WebStoreculture", Convert.ToString(localeId));
        }

        /// <summary>
        /// Get locale from cookie, if cookie is null we set store default locale in cookie.
        /// </summary>
        /// <returns></returns>
        protected static int GetStoreLocale()
        {
            int localeId = 0;
            Int32.TryParse(CookieHelper.GetCookieValue<String>("_WebStoreculture"), out localeId);
            return localeId;
        }

        protected static string GetCurrentWebstoreDomain() => HttpContext.Current.Request.Url.Authority;

        /// <summary>
        /// Get  impersonation CRS Id from session
        /// </summary>
        /// <returns name="impersonationCSRId"> CSR Id</returns>
        protected int GetImpersonationCSRId()
        {
            int impersonationCSRId = 0;
            if (HelperUtility.IsNotNull(SessionHelper.GetDataFromSession<ImpersonationModel>(WebStoreConstants.ImpersonationSessionKey)))
            {
                Int32.TryParse(Convert.ToString(SessionHelper.GetDataFromSession<ImpersonationModel>(WebStoreConstants.ImpersonationSessionKey).CRSUserId), out impersonationCSRId);
            }
            return impersonationCSRId;
        }
        /// <summary>
        /// Get  impersonation Portal Id from session
        /// </summary>
        /// <returns name="ImpersonationPortalId"> Portal Id</returns>
        protected int GetImpersonationPortalId()
        {
            int portalId = 0;
            if (HelperUtility.IsNotNull(SessionHelper.GetDataFromSession<ImpersonationModel>(WebStoreConstants.ImpersonationSessionKey)))
            {
                Int32.TryParse(Convert.ToString(SessionHelper.GetDataFromSession<ImpersonationModel>(WebStoreConstants.ImpersonationSessionKey).PortalId), out portalId);
            }
            return portalId;
        }

        //Add PublishCatalogId in cache.
        protected void SetUserProfileCatalogId(int? catalogId)
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                string cachekey = string.Concat("UserProfileCatalogId", HttpContext.Current.User.Identity.Name, Convert.ToString(PortalAgent.CurrentPortal?.PortalId));
                if (catalogId > 0)
                {
                    CacheHelper.AddIntoCache(catalogId, cachekey);
                }
            }
        }

        //Create the cookie to store userId for Logged in user. 
        protected virtual void SetLoginUserCookie(int userId)
        {
            //Check if the browser support cookies 
            if ((HttpContext.Current.Request.Browser.Cookies) && userId > 0)
            {
                SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
                CookieHelper.SetHttpOnlyCookie(WebStoreConstants.UserIdCookie, EncodeBase64(userId.ToString()), Convert.ToDouble(sessionStateSection.Timeout.TotalMinutes), true);
            }
        }
        protected virtual string EncodeBase64(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

        protected virtual string DecodeBase64(string value) => Encoding.UTF8.GetString(Convert.FromBase64String(value));
        #endregion
    }
}