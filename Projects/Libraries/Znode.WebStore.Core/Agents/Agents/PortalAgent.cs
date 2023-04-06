using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using System.Diagnostics;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore.Agents
{
    public class PortalAgent : BaseAgent, IPortalAgent
    {
        #region Private Variables
        private readonly IWebStorePortalClient _webStorePortalClient;
        private readonly IDomainClient _domainClient;
        private readonly IPortalClient _portalClient;
        #endregion

        #region Constructor
        public PortalAgent(IWebStorePortalClient portalClient, IDomainClient domainClient, IPortalClient storeClient)
        {
            _webStorePortalClient = GetClient<IWebStorePortalClient>(portalClient);
            _domainClient = GetClient<IDomainClient>(domainClient);
            _portalClient = GetClient<IPortalClient>(storeClient);
        }
        #endregion

        #region Public Methods

        public static int LocaleId
        {
            get
            {
                return GetStoreLocale();
            }
        }       
        //Get Current Portal's information.
        public static PortalViewModel CurrentPortal
        {
            get
            {
                PortalViewModel model;
                int localeId = LocaleId;
                //If data is not cached make a call else get cached data for portal.
                if (HelperUtility.IsNull(HttpRuntime.Cache[HttpContext.Current.Request.Url.Authority + "?LocaleId=" + localeId]))
                {
                    IWebstoreHelper helper = GetService<IWebstoreHelper>();
                    model = helper.GetCurrentPortal();

                    if (HelperUtility.IsNull(model) || model.PortalId <= 0 || model.PublishState <= 0)
                    { //Check if portal has been published before.
                        throw new ZnodeException(ErrorCodes.StoreNotPublished, WebStore_Resources.ErrorStoreNotPublished);
                    }
                    else
                    {
                        SetPublishState(model.PublishState);
                        SetStoreLocale(model.LocaleId);
                        localeId = model.LocaleId;
                    }
                    model.IsEnableSinglePageCheckout = true;
                    if (HelperUtility.IsNotNull(model))
                        Helper.AddIntoCache(model, HttpContext.Current.Request.Url.Authority + "?LocaleId=" + localeId, "CurrentPortalCacheDuration");

                    return model;
                }

                model = Helper.GetFromCache<PortalViewModel>(HttpContext.Current.Request.Url.Authority + "?LocaleId=" + localeId);
                SetPublishState(model.PublishState);
                SetStoreLocale(model.LocaleId);
                return model;
            }
        }

        //Set Default Logging Setting in cache
        public virtual void SetGlobalLoggingSetting()
        {
            try
            {
                if (HttpRuntime.Cache["DefaultLoggingConfigCache"] == null)
                {
                    Dictionary<string, string> loggingConfigurationSettings = ZnodeDependencyResolver.GetService<IDefaultGlobalConfigClient>()
                                                            ?.GetLoggingGlobalConfigList();
                    HttpRuntime.Cache.Insert("DefaultLoggingConfigCache", loggingConfigurationSettings);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
        }

        //Set current portal in cache.
        public static PortalViewModel SetCurrentPortalInCache()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            IWebstoreHelper helper = GetService<IWebstoreHelper>();
            PortalViewModel model = helper.GetCurrentPortal(Convert.ToInt32(SessionHelper.GetDataFromSession<object>("PortalId") ?? "0"));

            if (HelperUtility.IsNull(model) || model.PortalId <= 0 || model.PublishState <= 0)
                //Check if portal has been published before.
                throw new ZnodeException(ErrorCodes.StoreNotPublished, WebStore_Resources.ErrorStoreNotPublished);

            model.IsEnableSinglePageCheckout = true;
            if (HelperUtility.IsNotNull(model))
                Helper.AddIntoCache(model, HttpContext.Current.Request.Url.Authority + "?LocaleId=" + LocaleId, "CurrentPortalCacheDuration");

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return model;
        }

        //Change locale for current portal.
        public virtual bool ChangeLocale(string localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            int storeLocalId = 0;
            int.TryParse(localeId, out storeLocalId);
            //Apply new locale to the subsequent requests to API.
            SetStoreLocale(storeLocalId);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return true;
        }

       
        //Get current portal's information.
        public virtual PortalViewModel GetCurrentPortal()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            DomainListModel list = _domainClient.GetDomains(new FilterCollection() { new FilterTuple(FilterKeys.DomainName, FilterOperators.Equals, "\"" + HttpContext.Current.Request.Url.Authority + "\"") }, new SortCollection(), null, null);
            if (list.Domains.Any())
            {
                DomainModel domain = list.Domains.First();

                ApplicationTypesEnum applicationType;
                Enum.TryParse(domain?.ApplicationType, true, out applicationType);

                WebStorePortalModel webStorePortalModel = _webStorePortalClient.GetPortal(domain.PortalId, LocaleId, applicationType, null);
                if (HelperUtility.IsNotNull(webStorePortalModel))
                {
                    PortalViewModel portalViewModel = webStorePortalModel.ToViewModel<PortalViewModel>();
                    portalViewModel.PersistentCartEnabled = GetPortalFeatureValue(HelperUtility.StoreFeature.Persistent_Cart.ToString(), webStorePortalModel);
                    portalViewModel.IsAllowMultipleCoupon = GetPortalFeatureValue(HelperUtility.StoreFeature.Allow_multiple_coupons.ToString(), webStorePortalModel);
                    portalViewModel.ApplicationType = applicationType.ToString();
                    ZnodeLogging.LogMessage("PersistentCartEnabled and IsAllowMultipleCoupon:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { portalViewModel?.PersistentCartEnabled, portalViewModel?.IsAllowMultipleCoupon });
                    return portalViewModel;
                }
            }
            return new PortalViewModel();
        }

        //Get current portal's information.
        public virtual PortalViewModel GetCurrentPortal(int cachePortalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            int portalId = cachePortalId;
            DomainListModel list = _domainClient.GetDomains(new FilterCollection() { new FilterTuple(FilterKeys.DomainName, FilterOperators.Equals, "\"" + HttpContext.Current.Request.Url.Authority + "\"") }, new SortCollection(), null, null);

            if (list.Domains.Any())
            {
                DomainModel domain = list.Domains.First();

                if (portalId == 0)
                    portalId = domain.PortalId;

                ApplicationTypesEnum applicationType;
                Enum.TryParse(domain?.ApplicationType, true, out applicationType);

                WebStorePortalModel webStorePortalModel = _webStorePortalClient.GetPortal(portalId, LocaleId, applicationType, null);

                if (HelperUtility.IsNotNull(webStorePortalModel))
                {
                    PortalViewModel portalViewModel = webStorePortalModel.ToViewModel<PortalViewModel>();
                    portalViewModel.PersistentCartEnabled = GetPortalFeatureValue(HelperUtility.StoreFeature.Persistent_Cart.ToString(), webStorePortalModel);
                    portalViewModel.IsAllowMultipleCoupon = GetPortalFeatureValue(HelperUtility.StoreFeature.Allow_multiple_coupons.ToString(), webStorePortalModel);
                    portalViewModel.ApplicationType = applicationType.ToString();
                    ZnodeLogging.LogMessage("PersistentCartEnabled and IsAllowMultipleCoupon:", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { portalViewModel?.PersistentCartEnabled, portalViewModel?.IsAllowMultipleCoupon });
                    return portalViewModel;
                }
            }
            
            return new PortalViewModel();
        }

        //Get barcode scanner details
        public virtual BarcodeReaderViewModel GetBarcodeScannerDetail()
        {
            try
            {
                BarcodeReaderModel barcodeModel = _portalClient.GetBarcodeScannerDetail();
                return barcodeModel.ToViewModel<BarcodeReaderViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return new BarcodeReaderViewModel();
        }

        #region Robots.Txt
        //Get robots.txt data.
        public virtual RobotsTxtViewModel GetRobotsTxt()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            RobotsTxtModel robotsTxtModel = _portalClient.GetRobotsTxt(CurrentPortal.PortalId, null);
            if (HelperUtility.IsNull(robotsTxtModel))
                throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.NoDataFound);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return robotsTxtModel.ToViewModel<RobotsTxtViewModel>();
        }
        #endregion
        #endregion

        #region Private Methods
        //Get the value of portal features.
        private bool GetPortalFeatureValue(string featureName, WebStorePortalModel webStorePortalModel)
          => (webStorePortalModel.PortalFeatureValues?.Where(x => x.Key == featureName)?.FirstOrDefault().Value).GetValueOrDefault();

#if DEBUG
        //Gets list of portals available as a select list 
        public virtual List<SelectListItem> GetDevPortalSelectList()
        => (_portalClient.GetDevPortalList()?.PortalList)?.Select(item => new SelectListItem
        {
            Text = item.StoreName,
            Value = item.PortalId.ToString(),
        }).ToList() ?? new List<SelectListItem>();
#endif
        #endregion
    }
}