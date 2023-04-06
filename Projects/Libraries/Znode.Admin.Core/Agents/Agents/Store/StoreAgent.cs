using Microsoft.Ajax.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

using Znode.Admin.Core.Helpers;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Admin.Agents
{
    public class StoreAgent : BaseAgent, IStoreAgent
    {
        #region Private Variables
        private readonly IPortalClient _portalClient;
        private readonly IEcommerceCatalogClient _ecommerceCatalogClient;
        private readonly IThemeClient _themeClient;
        private readonly IDomainClient _domainClient;
        private readonly IPriceClient _priceClient;
        private readonly IOrderStateClient _orderStateClient;
        private readonly IProductReviewStateClient _productReviewClient;
        private readonly IPortalProfileClient _portalProfileClient;
        private readonly IWarehouseClient _warehouseClient;
        private readonly ICSSClient _cssClient;
        private readonly IPortalCountryClient _portalCountryClient;
        private readonly IPortalCountryAgent _portalCountryAgent;
        private readonly IManageMessageClient _manageMessageClient;
        private readonly IContentPageClient _contentPageClient;
        private readonly ITaxClassClient _taxClassClient;
        private readonly IPaymentClient _paymentClient;
        private readonly IShippingClient _shippingClient;
        private readonly ITagManagerClient _tagManagerClient;
        private readonly IGeneralSettingClient _generalSettingClient;
        private readonly IGeneralSettingAgent _generalSettingAgent;
        #endregion

        #region Constructor
        public StoreAgent(IPortalClient portalClient, IEcommerceCatalogClient ecommerceCatalogClient, IThemeClient themeClient, IDomainClient domainClient, IPriceClient priceClient, IOrderStateClient orderStateClient,
            IProductReviewStateClient productReviewClient, IPortalProfileClient portalProfileClient, IWarehouseClient warehouseClient, ICSSClient cssClient, IManageMessageClient manageMessageClient,
            IContentPageClient contentPageClient, ITaxClassClient taxClassClient, IPaymentClient paymentClient, IShippingClient shippingClient, IPortalCountryClient portalCountryClient, ITagManagerClient tagManagerClient, IGeneralSettingClient generalSettingClient)
        {
            _portalClient = GetClient<IPortalClient>(portalClient);
            _themeClient = GetClient<IThemeClient>(themeClient);
            _domainClient = GetClient<IDomainClient>(domainClient);
            _orderStateClient = GetClient<IOrderStateClient>(orderStateClient);
            _productReviewClient = GetClient<IProductReviewStateClient>(productReviewClient);
            _portalProfileClient = GetClient<IPortalProfileClient>(portalProfileClient);
            _warehouseClient = GetClient<IWarehouseClient>(warehouseClient);
            _ecommerceCatalogClient = GetClient<IEcommerceCatalogClient>(ecommerceCatalogClient);
            _priceClient = GetClient<IPriceClient>(priceClient);
            _cssClient = GetClient<ICSSClient>(cssClient);
            _portalCountryClient = GetClient<IPortalCountryClient>(portalCountryClient);
            _manageMessageClient = GetClient<IManageMessageClient>(manageMessageClient);
            _contentPageClient = GetClient<IContentPageClient>(contentPageClient);
            _taxClassClient = GetClient<ITaxClassClient>(taxClassClient);
            _paymentClient = GetClient<IPaymentClient>(paymentClient);
            _shippingClient = GetClient<IShippingClient>(shippingClient);
            _portalCountryAgent = new PortalCountryAgent(_portalCountryClient);
            _tagManagerClient = GetClient<ITagManagerClient>(tagManagerClient);
            _generalSettingClient = GetClient<IGeneralSettingClient>(generalSettingClient);
            _generalSettingAgent = new GeneralSettingAgent(_generalSettingClient);

        }
        #endregion

        #region Public Methods
        //Get the list of all stores.
        public virtual StoreListViewModel GetStoreList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = default(int?), int? pageSize = default(int?))
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            sorts = HelperMethods.SortAsc(ZnodePortalEnum.StoreName.ToString(), sorts);
            PortalListModel storeList = _portalClient.GetPortalList(new ExpandCollection { ZnodePortalEnum.ZnodeDomains.ToString().ToLower() }, filters, sorts, pageIndex, pageSize);
            StoreListViewModel storeListViewModel = new StoreListViewModel { StoreList = storeList?.PortalList?.ToViewModel<StoreViewModel>().ToList() };
            storeListViewModel?.StoreList?.ForEach(item => { item.UrlEncodedStoreName = HttpUtility.UrlEncode(item.StoreName); });

            //Set the Tool Menus for Store List Grid View.
            SetStoreListToolMenus(storeListViewModel);

            SetListPagingData(storeListViewModel, storeList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return storeListViewModel;
        }

        //Get store details by store Id.
        public virtual StoreViewModel GetStore(int portalId, ExpandCollection expands = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            StoreViewModel storeViewModel = portalId > 0 ? StoreViewModelMap.ToViewModel(_portalClient.GetPortal(portalId, expands)) : new StoreViewModel();
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { expands = expands });

            GetStoreFeatureValue(GetStoreInformation(storeViewModel));

            switch (storeViewModel.ReviewStatusId)
            {
                case AdminConstants.PublishImmediately:
                    storeViewModel.ReviewStatus = AdminConstants.PublishImmediatelyText;
                    break;
                case AdminConstants.DoNotPublish:
                    storeViewModel.ReviewStatus = AdminConstants.DoNotPublishText;
                    break;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return storeViewModel;
        }

        //Get store details by store code.
        public virtual StoreViewModel GetStore(string storeCode, ExpandCollection expands = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            StoreViewModel storeViewModel = !string.IsNullOrEmpty(storeCode) ? StoreViewModelMap.ToViewModel(_portalClient.GetPortal(storeCode, expands)) : new StoreViewModel();
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { expands = expands });

            GetStoreFeatureValue(GetStoreInformation(storeViewModel));

            switch (storeViewModel.ReviewStatusId)
            {
                case AdminConstants.PublishImmediately:
                    storeViewModel.ReviewStatus = AdminConstants.PublishImmediatelyText;
                    break;
                case AdminConstants.DoNotPublish:
                    storeViewModel.ReviewStatus = AdminConstants.DoNotPublishText;
                    break;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return storeViewModel;
        }

        //Create new store.
        public virtual StoreViewModel CreateStore(StoreViewModel storeViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                //If DefaultGlobalSetting session key is set to null then session will be updated for the same key in further execution.
                HelperMethods.ClearCache(AdminConstants.DefaultGlobalSettingCacheKey);

                storeViewModel.DefaultCurrency = DefaultSettingHelper.DefaultCurrency;
                storeViewModel.DefaultCulture = DefaultSettingHelper.DefaultCulture;
                storeViewModel.DefaultWeightUnit = DefaultSettingHelper.DefaultWeightUnit;
                storeViewModel.DefaultDimensionUnit = DimensionUnit.CM.ToString();

                StoreViewModel storeModel = StoreViewModelMap.ToViewModel(_portalClient.CreatePortal(storeViewModel?.ToModel<PortalModel>()));

                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return storeModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NullModel:
                        return (StoreViewModel)GetViewModelWithErrorMessage(storeViewModel, Admin_Resources.ErrorFailedToCreate);
                    case ErrorCodes.ExceptionalError:
                        return (StoreViewModel)GetViewModelWithErrorMessage(storeViewModel, Admin_Resources.CopyContentError);
                    default:
                        return (StoreViewModel)GetViewModelWithErrorMessage(storeViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (StoreViewModel)GetViewModelWithErrorMessage(storeViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Update the store details by store Id.
        public virtual bool UpdateStore(StoreViewModel storeViewModel, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                errorMessage = string.Empty;
                return IsNotNull(StoreViewModelMap.ToViewModel(_portalClient.UpdatePortal(storeViewModel?.ToModel<PortalModel>())));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.DuplicateSearchIndexName:
                        errorMessage = Admin_Resources.ErrorUpdateStoreSearchIndex;
                        break;
                    default:
                        errorMessage = Admin_Resources.UpdateErrorMessage;
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }

        //Delete a store by store Id.
        public virtual bool DeleteStore(string portalId, out string errorMessage, bool isDeleteByStoreCode = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorStoreDelete;
            try
            {
                ZnodeLogging.LogMessage("Deleting Store with:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { Ids = portalId });

                return _portalClient.DeletePortal(new ParameterModel { Ids = portalId }, isDeleteByStoreCode);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                if (Equals(ex.ErrorCode, ErrorCodes.NullModel))
                    errorMessage = Admin_Resources.ErrorSelectAtLeastone;
                else if (Equals(ex.ErrorCode, ErrorCodes.ExceptionalError))
                    errorMessage = Admin_Resources.ErrorStoreDelete;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorStoreDelete;
            }
            return false;
        }

        // Method to copy store.
        public virtual bool CopyStore(StoreViewModel storeViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("Store copying with:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, new { StoreId = storeViewModel.PortalId });

                return _portalClient.CopyStore(storeViewModel.ToModel<PortalModel>());
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get others details required to create/update a store.
        public virtual StoreViewModel GetStoreInformation(StoreViewModel storeViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNotNull(storeViewModel))
            {
                //Bind the catalog list.
                storeViewModel.CatalogList = StoreViewModelMap.ToPublishCatalogList(_ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null)?.PublishCatalogs);

                //Bind the order status list.
                FilterCollection fcOrderStatus = new FilterCollection();
                fcOrderStatus.Add(new FilterTuple(ZnodeOmsOrderStateEnum.IsAccountStatus.ToString(), FilterOperators.Equals, AdminConstants.False));
                fcOrderStatus.Add(new FilterTuple(ZnodeOmsOrderStateEnum.IsOrderState.ToString(), FilterOperators.Equals, AdminConstants.True));
                storeViewModel.OrderStatusList = StoreViewModelMap.ToOrderStateList(_orderStateClient.GetOrderStates(null, fcOrderStatus, null, null, null).OrderStates);

                //Bind theme list.
                storeViewModel.ThemeList = StoreViewModelMap.ToThemeList(_themeClient.GetThemes(null, null, null, null).Themes);

                //Bind CSS list.
                storeViewModel.CSSList = storeViewModel.CMSThemeId > 0 && IsNotNull(storeViewModel?.CMSThemeId) ?
                       StoreViewModelMap.ToCSSList(_cssClient.GetCssListByThemeId(storeViewModel.CMSThemeId.GetValueOrDefault(), null, null, null, null)?.CSSs) : new List<SelectListItem>();

                //Bind features.
                storeViewModel.AvailableStoreFeatureList = storeViewModel.AvailableStoreFeatureList?.Count > 0 ? storeViewModel.AvailableStoreFeatureList : StoreViewModelMap.ToStoreFeatureListViewModel(_portalClient.GetPortalFeatureList());

                //Bind the customer review status list.
                storeViewModel.CustomerReviewStatusList = GetCustomerReviewState();

                //Bind the portal list.
                if (storeViewModel.PortalId <= 0)
                    storeViewModel.PortalList = GetPortalSelectItemList();
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return storeViewModel;
        }

        //Gets css list based on selected theme in store.
        public virtual List<SelectListItem> GetCSSList(int cmsThemeId)
          => StoreViewModelMap.ToCSSList(_cssClient.GetCssListByThemeId(cmsThemeId, null, null, null, null)?.CSSs);

        // Check store code already exist or not.
        public virtual bool IsStoreCodeExist(string storeCode)
           => !string.IsNullOrEmpty(storeCode) ? _portalClient.IsPortalCodeExist(storeCode) : true;
        
        //Gets the current store
        public static StoreViewModel CurrentStore
        {
            get
            {
                if (Equals(HttpContext.Current.Cache[HttpContext.Current.Request.Url.Authority], null))
                {
                    StoreViewModel model = GetService<IDependencyHelper>().GetCurrentPortal();
                    if (IsNotNull(model))
                        HttpContext.Current.Cache.Insert(HttpContext.Current.Request.Url.Authority, model, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);
                }
                return HttpContext.Current.Cache[HttpContext.Current.Request.Url.Authority] as StoreViewModel;
            }
        }

        public virtual TabViewListModel CreateTabStructure(int portalId, int? profileId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            TabViewListModel TabStructModel = new TabViewListModel();
            TabStructModel.MaintainAllTabData = false;
            string controllerName = ZnodeConstant.Store;
            TabStructModel.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.TextBasePriceManagement, IsVisible = true, Url = $"/{controllerName}/GetAssociatedPriceListForStore?portalId={portalId}", IsSelected = true });
            TabStructModel.Tabs.Add(new TabViewModel() { Id = 2, Name = Admin_Resources.TextProfileBasePriceManagement, IsVisible = true, Url = $"/{controllerName}/GetAssociatedPriceListForProfile?portalId={portalId}&profileId={profileId}" });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return TabStructModel;
        }

        public virtual TabViewListModel CreateTabStructureForSearch(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            TabViewListModel TabStructModel = new TabViewListModel();
            TabStructModel.MaintainAllTabData = false;
            string controllerName = ZnodeConstant.Store;
            TabStructModel.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.TabSortList, IsVisible = true, Url = $"/{controllerName}/GetAssociatedSortForStore?portalId={portalId}", IsSelected = true });
            TabStructModel.Tabs.Add(new TabViewModel() { Id = 2, Name = Admin_Resources.TabPageList, IsVisible = true, Url = $"/{controllerName}/GetAssociatedPageForStore?portalId={portalId}" });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return TabStructModel;
        }

        public virtual TabViewListModel CreateTabStructureForShippingOrigin(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            TabViewListModel TabStructModel = new TabViewListModel();
            TabStructModel.MaintainAllTabData = false;
            string controllerName = "Store";
            IEnumerable<PublishStateMappingViewModel> publishState = _generalSettingAgent.GetAvailablePublishStateMappings();

            foreach (var ps in publishState)
            {
                if (ps.PublishStateCode == ZnodePublishStatesEnum.PRODUCTION.ToString())
                    TabStructModel.Tabs.Add(new TabViewModel() { Id = 1, Name = ps.PublishStateCode, IsVisible = true, Url = $"/{controllerName}/GetShippingOriginForProduction?portalId={portalId}", IsSelected = true });
                else
                    TabStructModel.Tabs.Add(new TabViewModel() { Id = 2, Name = ps.PublishStateCode, IsVisible = true, Url = $"/{controllerName}/GetShippingOriginForPreview?portalId={portalId}" });
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return TabStructModel;
        }


        public virtual void SetFiltersForPortalId(FilterCollection filters, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            if (IsNotNull(filters))
            {
                ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });
                SetPortalIdFilter(filters, portalId);
                SetFilterForMode(filters, "Portal");
            }

        }

        public virtual void SetFiltersForProfileId(FilterCollection filters, int profileId, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNotNull(filters))
            {
                SetPortalIdFilter(filters, portalId);

                //If ProfileId is already present in filters, remove it.
                filters.RemoveAll(x => string.Equals(x.Item1, ZnodePriceListProfileEnum.PortalProfileId.ToString(), StringComparison.CurrentCultureIgnoreCase));
                if (profileId > 0)
                    filters.Add(ZnodePriceListProfileEnum.PortalProfileId.ToString(), FilterOperators.Equals, profileId.ToString());
                ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });

                SetFilterForMode(filters, "profile");
            }
        }

        public virtual List<SelectListItem> GetPortalProfiles(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            if (portalId > 0)
                filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));

            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.Profiles);
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return PriceViewModelMap.PriceProfileModelToListItems(_portalProfileClient.GetPortalProfiles(expands, filters, null, null, null)?.PortalProfiles);
        }

        // Get available portal list.
        public virtual List<SelectListItem> GetPortalSelectList(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            List<SelectListItem> listItems = new List<SelectListItem>();

            //Get all available portal list.
            PortalListModel portalList = _portalClient.GetPortalList(null, null, null, null, null);

            //Maps the portal id and value from the list.
            if (portalList?.PortalList?.Count > 0)
            {
                listItems = (from item in portalList.PortalList
                             select new SelectListItem
                             {
                                 Text = item.StoreName,
                                 Value = item.PortalId.ToString(),
                                 Selected = item.PortalId == portalId
                             }).ToList();
            }
            ZnodeLogging.LogMessage("listItems list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, listItems?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return listItems;
        }

        // Get available portal list.
        public virtual List<SelectListItem> GetPortalListByCatalogId(int catalogId, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            List<SelectListItem> listItems = new List<SelectListItem>();

            //Get all available portal list.
            PortalListModel portalList = _portalClient.GetPortalListByCatalogId(catalogId);

            //Maps the portal id and value from the list.
            if (portalList?.PortalList?.Count > 0)
            {
                listItems = (from item in portalList.PortalList
                             select new SelectListItem
                             {
                                 Text = item.StoreName,
                                 Value = item.PortalId.ToString(),
                                 Selected = item.PortalId == portalId
                             }).ToList();
            }
            ZnodeLogging.LogMessage("listItems list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, listItems?.Count());

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return listItems;
        }

        //Get publish catalog list
        public virtual PortalCatalogListViewModel GetPublishCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            PublishCatalogListModel catalogList = _ecommerceCatalogClient.GetPublishCatalogList(expands, filters, sorts, pageIndex, recordPerPage);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            PortalCatalogListViewModel listViewModel = new PortalCatalogListViewModel { PortalCatalogs = catalogList?.PublishCatalogs?.ToViewModel<PortalCatalogViewModel>().ToList() };
            SetListPagingData(listViewModel, catalogList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return listViewModel;
        }

        //Get publish catalog list with json result for autocomplete catalog
        public virtual PortalCatalogListViewModel GetCatalogList(string catalogName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            if (!string.IsNullOrEmpty(catalogName))
            {

                filters.RemoveAll(x => x.FilterName == ZnodePimCatalogEnum.CatalogName.ToString());
                filters.Add(ZnodePimCatalogEnum.CatalogName.ToString(), FilterOperators.Contains, catalogName);
            }
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });
            PublishCatalogListModel catalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, filters, null, null, null);

            PortalCatalogListViewModel listViewModel = new PortalCatalogListViewModel { PortalCatalogs = catalogList?.PublishCatalogs?.ToViewModel<PortalCatalogViewModel>().ToList() };
            SetListPagingData(listViewModel, catalogList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return listViewModel;
        }

        #region Inventory Management
        // Get list of associated warehouse.
        public virtual PortalWarehouseViewModel GetAssociatedWarehouseList(int portalId, int warehouseId, ExpandCollection expands = null, FilterCollection filters = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ExpandCollection expand = new ExpandCollection();
            expand.Add(ZnodePortalAlternateWarehouseEnum.ZnodeWarehouse.ToString());
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters });
            SetFiltersForPortalWarehouse(filters, portalId, warehouseId);

            PortalWarehouseViewModel associatedWarehouses = _portalClient.GetAssociatedWarehouseList(portalId, filters, expand)?.ToViewModel<PortalWarehouseViewModel>();

            if (warehouseId > 0)
            {
                associatedWarehouses.WarehouseId = warehouseId;
                associatedWarehouses.PortalId = portalId;
            }

            //Get a list of all warehouses as SelectListItem.
            GetWarehouses(associatedWarehouses);

            //Get a list of associated warehouse as SelectListItem.
            AssociatedWarehouse(associatedWarehouses);

            //Get a list of Unassociated warehouse as SelectListItem.
            UnassociatedWarehouse(associatedWarehouses);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return associatedWarehouses;
        }

        //Associate warehouse to store.
        public virtual bool AssociateWarehouseToStore(int portalId, int warehouseId, string alternateWarehouseIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            PortalWarehouseViewModel viewModel = new PortalWarehouseViewModel();
            viewModel.PortalId = portalId;
            viewModel.WarehouseId = warehouseId;

            if (!string.IsNullOrEmpty(alternateWarehouseIds))
            {
                int[] selectedWarehouseIds = alternateWarehouseIds.Split(',').Select(int.Parse).ToArray();
                viewModel.AlternateWarehouses = new List<PortalAlternateWarehouseViewModel>();

                foreach (int alternateWarehouseId in selectedWarehouseIds)
                {
                    PortalAlternateWarehouseViewModel alternateViewModel = new PortalAlternateWarehouseViewModel { WarehouseId = alternateWarehouseId };
                    viewModel.AlternateWarehouses.Add(alternateViewModel);
                }
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return _portalClient.AssociateWarehouseToStore(viewModel.ToModel<PortalWarehouseModel>());
        }

        public virtual DomainListViewModel GetDomains(int portalId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            //checks if the filter collection null
            filters = IsNotNull(filters) ? filters : new FilterCollection();
            //Adding filter to show only webstore related list in the webstore url list

            FilterTuple webstoreFilterAdminTuple = new FilterTuple(FilterKeys.ApplicationType, FilterOperators.NotContains, ApplicationTypesEnum.Admin.ToString());
            FilterTuple webstoreFilterAPITuple = new FilterTuple(FilterKeys.ApplicationType, FilterOperators.NotContains, ApplicationTypesEnum.API.ToString());
            filters.Remove(webstoreFilterAdminTuple);
            filters.Add(webstoreFilterAdminTuple);
            filters.Remove(webstoreFilterAPITuple);
            filters.Add(webstoreFilterAPITuple);
            if (portalId > 0)
            {
                //If PortalId is already present in filters, remove it.
                filters.RemoveAll(x => string.Equals(x.Item1, FilterKeys.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase));
                filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));
            }
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { sortCollection = sortCollection, filters = filters });

            DomainListModel domainList = _domainClient.GetDomains(filters, sortCollection, pageIndex, recordPerPage);
            if (IsNotNull(domainList?.Domains))
            {
                foreach (DomainModel item in domainList?.Domains)
                {
                    item.Status = item.IsActive;
                    item.IsActive = !item.IsActive;
                }
            }
            DomainListViewModel listViewModel = new DomainListViewModel { Domains = domainList?.Domains?.ToViewModel<DomainViewModel>().ToList() };
            SetListPagingData(listViewModel, domainList);

            //Set tool menu for URL list grid view.
            SetUrlListToolMenu(listViewModel);
            listViewModel.Domains?.ForEach(item => { item.IsDefaultList = GetBooleanList(); });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return domainList?.Domains?.Count > 0 ? listViewModel : new DomainListViewModel() { Domains = new List<DomainViewModel>() };
        }
        #endregion

        #region Portal Locale
        //Method to get Locale list
        public virtual LocaleListViewModel GetLocales(int portalId, ExpandCollection expands, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sortCollection });
            SetFiltersForLocales(filters, portalId);
            LocaleListModel localeList = _portalClient.GetLocaleList(expands, filters, sortCollection, pageIndex, recordPerPage);
            LocaleListViewModel listViewModel = new LocaleListViewModel { Locales = localeList?.Locales?.ToViewModel<LocaleViewModel>().ToList() };
            SetListPagingData(listViewModel, localeList);

            //Set tool menu for portal locale list grid view.
            SetActiveLocaleListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return localeList?.Locales?.Count > 0 ? listViewModel : new LocaleListViewModel() { Locales = new List<LocaleViewModel>() };
        }

        //Update Locale
        public virtual bool UpdateLocale(DefaultGlobalConfigViewModel model, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = Admin_Resources.UpdateErrorMessage;
            try
            {
                return _portalClient.UpdateLocale(StoreViewModelMap.ToGlobalConfigurationListModel(model));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorMessage)
                {
                    case "ErrorDefault":
                        message = GlobalSetting_Resources.ErrorDefault;
                        return false;
                    case "ErrorDeactivate":
                        message = GlobalSetting_Resources.ErrorDeactivate;
                        return false;
                    default:
                        message = Admin_Resources.UpdateErrorMessage;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                message = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }
        private void SetFiltersForPortalWarehouse(FilterCollection filters, int portalId, int warehouseId)
        {
            if (IsNotNull(filters))
            {
                filters.Clear();
                filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));

                if (warehouseId > 0)
                    filters.Add(new FilterTuple(ZnodeWarehouseEnum.WarehouseId.ToString(), FilterOperators.Equals, warehouseId.ToString()));
            }
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });
        }

        #endregion

        #region Portal Shipping
        //Get Portal shipping by portalId.
        public virtual PortalShippingViewModel GetPortalShippingInformation(int portalId, int publishStateId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            PortalShippingViewModel portalShippingViewModel = new PortalShippingViewModel();
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodePublishStateEnum.PublishStateId.ToString(), FilterOperators.Equals, publishStateId.ToString());
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });
            portalShippingViewModel = _portalClient.GetPortalShippingInformation(portalId, filters)?.ToViewModel<PortalShippingViewModel>();

            if (IsNull(portalShippingViewModel))
                portalShippingViewModel = new PortalShippingViewModel();

            //Bind portal shipping related data like PackagingTypes, FedexDropOffTypes,CountryList.
            BindPageDropdown(portalShippingViewModel, portalId);
            portalShippingViewModel.PortalId = portalId;
            portalShippingViewModel.PortalName = _portalClient.GetPortal(portalId, null)?.StoreName;
            portalShippingViewModel.PublishStateId = publishStateId;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return portalShippingViewModel;
        }

        //Update portal shipping.
        public virtual PortalShippingViewModel UpdatePortalShipping(PortalShippingViewModel portalShippingViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return _portalClient.UpdatePortalShipping(portalShippingViewModel?.ToModel<PortalShippingModel>())?.ToViewModel<PortalShippingViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (PortalShippingViewModel)GetViewModelWithErrorMessage(portalShippingViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }
        #endregion

        #region Portal Tax
        //Get portal tax by portalId.
        public virtual TaxPortalViewModel GetTaxPortalInformation(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalId > 0)
            {
                TaxPortalViewModel taxPortalViewModel = new TaxPortalViewModel();
                taxPortalViewModel = _portalClient.GetTaxPortalInformation(portalId, SetExpandForPortal())?.ToViewModel<TaxPortalViewModel>();
                if (IsNull(taxPortalViewModel))
                    taxPortalViewModel = new TaxPortalViewModel();

                taxPortalViewModel.PortalId = portalId;
                return taxPortalViewModel;
            }
            else
                return new TaxPortalViewModel();
        }

        //Update portal tax.
        public virtual TaxPortalViewModel UpdateTaxPortal(TaxPortalViewModel taxPortalViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return _portalClient.UpdateTaxPortal(taxPortalViewModel?.ToModel<TaxPortalModel>())?.ToViewModel<TaxPortalViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (TaxPortalViewModel)GetViewModelWithErrorMessage(taxPortalViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        public virtual string TestAvalaraConnection(TaxPortalViewModel taxPortalViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return $"[{JsonConvert.SerializeObject(_taxClassClient.TestAvalaraConnection(taxPortalViewModel?.ToModel<TaxPortalModel>()))}]";
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return Admin_Resources.TestConnectionFailure;
            }
        }
        #endregion               

        //Get Price list.
        public virtual PriceListViewModel GetPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            PriceListModel priceList = _priceClient.GetPriceList(expands, filters, sorts, pageIndex, pageSize);
            PriceListViewModel listViewModel = new PriceListViewModel { PriceList = priceList?.PriceList?.ToViewModel<PriceViewModel>().ToList() };
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            SetListPagingData(listViewModel, priceList);

            if (filters.FirstOrDefault(x => x.FilterName == AdminConstants.Mode)?.FilterValue == AdminConstants.Profile)
                //Set the Tool Menus for profile List associated to store Grid View.
                SetPriceListToolMenuForProfile(listViewModel);
            else
                //Set tool menu for price list grid view.
                SetPriceListToolMenuForStore(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return priceList?.PriceList?.Count > 0 ? listViewModel : new PriceListViewModel() { PriceList = new List<PriceViewModel>() };
        }
        #region Tax
        //Get the list of all tax classes.
        public virtual TaxClassListViewModel GetTaxClassList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId, bool isAssociated)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.DisplayOrder, DynamicGridConstants.ASCKey);
            }
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            //Checking For LocaleId already Exists in Filters Or Not
            SetFilterforStoreTaxClassList(filters, portalId, isAssociated);
            TaxClassListModel taxClassListModel = _taxClassClient.GetTaxClassList(filters, sorts, pageIndex, pageSize);
            TaxClassListViewModel listViewModel = new TaxClassListViewModel { TaxClassList = taxClassListModel?.TaxClassList?.ToViewModel<TaxClassViewModel>().ToList() };
            SetListPagingData(listViewModel, taxClassListModel);
            if (isAssociated)
                //Set tool menu for tax class list grid view.
                SetTaxClassListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return taxClassListModel?.TaxClassList?.Count > 0 ? listViewModel : new TaxClassListViewModel() { TaxClassList = new List<TaxClassViewModel>() };
        }
        //Associate/Unassociate tax class.
        public virtual bool AssociateUnAssociateTaxClass(int portalId, string taxClassIds, bool isUnAssociated, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = isUnAssociated ? Admin_Resources.ErrorUnAssociateStoreTax : Admin_Resources.ErrorAssociateStoreTax;
            try
            {
                return !string.IsNullOrEmpty(taxClassIds) ? _portalClient.AssociateAndUnAssociateTaxClass(GetTaxClassPortalModel(portalId, taxClassIds, isUnAssociated)) : false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.DefaultDataDeletionError:
                        message = Admin_Resources.ErrorStoreTaxUnassigned;
                        return false;
                    default:
                        message = isUnAssociated ? Admin_Resources.ErrorUnAssociateStoreTax : Admin_Resources.ErrorAssociateStoreTax;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                message = isUnAssociated ? Admin_Resources.ErrorUnAssociateStoreTax : Admin_Resources.ErrorAssociateStoreTax;
                return false;
            }
        }
        //Set portal default tax class.
        public virtual bool SetPortalDefaultTax(int portalId, string taxClassIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = Admin_Resources.ErrorUnableToSetDefaultTax;
            try
            {
                return !string.IsNullOrEmpty(taxClassIds) ? _portalClient.SetPortalDefaultTax(GetTaxClassPortalModel(portalId, taxClassIds, false)) : false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                //return false;
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.SetDefaultDataError:
                        message = Admin_Resources.StoreTaxClassDefaultError;
                        return false;
                    default:
                        message = Admin_Resources.ErrorUnableToSetDefaultTax;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorUnableToSetDefaultTax;
                return false;
            }
        }
        #endregion

        #region Portal Search Filter Setting
        //Get Search settings list.
        public virtual PortalSortSettingListViewModel GetPortalSortSettingsList(int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isAssociated)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            sorts = SetSort(sorts);
            SetPortalIdFilter(filters, portalId);
            SetIsAssociatedFilter(filters, isAssociated);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            PortalSortSettingListModel sortSettingList = _portalClient.GetPortalSortSettings(null, filters, sorts, pageIndex, pageSize);
            PortalSortSettingListViewModel listViewModel = new PortalSortSettingListViewModel { SortSettings = sortSettingList?.SortSettings?.ToViewModel<PortalSortSettingViewModel>().ToList() };
            SetListPagingData(listViewModel, sortSettingList);
            if (isAssociated)
                SetSortToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return sortSettingList?.SortSettings?.Count > 0 ? listViewModel : new PortalSortSettingListViewModel() { SortSettings = new List<PortalSortSettingViewModel>() };
        }
        //Remove associated page settings to portal.
        public virtual bool RemoveAssociatedSortSettings(int portalId, string portalSortSettingIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return !string.IsNullOrEmpty(portalSortSettingIds) ? _portalClient.RemoveAssociatedSortSettings(new SortSettingAssociationModel { PortalId = portalId, PortalSortSettingIds = portalSortSettingIds }) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Remove associated page settings to portal.
        public virtual bool RemoveAssociatedPageSettings(int portalId, string portalPageSettingIds, out string errorMessage)
        {
            errorMessage = Admin_Resources.UnassignError;
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                bool result = true;
                if (!string.IsNullOrEmpty(portalPageSettingIds))
                {
                    result = _portalClient.RemoveAssociatedPageSettings(new PageSettingAssociationModel { PortalId = portalId, PortalPageSettingIds = portalPageSettingIds });
                    errorMessage = result ? string.Empty : Admin_Resources.PortalPageDefaultUnassignError;
                }
                return result;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Associate page settings to portal.
        public virtual bool AssociateSortSettings(int portalId, string sortSettingIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return !string.IsNullOrEmpty(sortSettingIds) ? _portalClient.AssociateSortSettings(new SortSettingAssociationModel { PortalId = portalId, SortSettingIds = sortSettingIds }) : false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotPermitted:
                        message = Admin_Resources.ErrorSortAssociation;
                        return false;
                    default:
                        message = Admin_Resources.AssociatedErrorMessage;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                message = Admin_Resources.AssociatedErrorMessage;
                return false;
            }
        }

        //Associate page settings to portal.
        public virtual bool AssociatePageSettings(int portalId, string pageSettingIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return !string.IsNullOrEmpty(pageSettingIds) ? _portalClient.AssociatePageSettings(new PageSettingAssociationModel { PortalId = portalId, PageSettingIds = pageSettingIds }) : false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotPermitted:
                        message = Admin_Resources.ErrorPageAssociation;
                        return false;
                    default:
                        message = Admin_Resources.AssociatedErrorMessage;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                message = Admin_Resources.AssociatedErrorMessage;
                return false;
            }
        }

        //Get Page settings list.
        public virtual PortalPageSettingListViewModel GetPortalPageSettingsList(int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isAssociated)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            sorts = SetSort(sorts);
            SetPortalIdFilter(filters, portalId);
            SetIsAssociatedFilter(filters, isAssociated);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            PortalPageSettingListModel pageSettingList = _portalClient.GetPortalPageSettings(null, filters, sorts, pageIndex, pageSize);
            PortalPageSettingListViewModel listViewModel = new PortalPageSettingListViewModel { PageSettings = pageSettingList?.PageSettings?.ToViewModel<PortalPageSettingViewModel>().ToList() };
            SetListPagingData(listViewModel, pageSettingList);
            if (isAssociated)
                SetPageToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            listViewModel?.PageSettings?.ForEach(item => { item.IsDefaultList = GetBooleanList(); });

            return pageSettingList?.PageSettings?.Count > 0 ? listViewModel : new PortalPageSettingListViewModel() { PageSettings = new List<PortalPageSettingViewModel>() };
        }

        //Update Portal Page settings list.
        public virtual bool UpdatePortalPageSetting(PortalPageSettingViewModel domainViewModel, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method UpdatePortalPageSetting execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                errorMessage = string.Empty;
                return _portalClient.UpdatePortalPageSetting(domainViewModel?.ToModel<PortalPageSettingModel>());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AtLeastSelectOne:
                        errorMessage = Admin_Resources.PortalPageAtLeastOneDefault;
                        break;
                    default:
                        errorMessage = Admin_Resources.UpdateErrorMessage;
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }
        #endregion

        #region Payment
        //Get payment settings list.
        public virtual PaymentSettingListViewModel GetPaymentSettingsList(int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isAssociated)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            sorts = SetSort(sorts);
            SetPortalIdFilter(filters, portalId);
            SetIsAssociatedFilter(filters, isAssociated);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            PaymentSettingListModel paymentSettingList = _paymentClient.GetPaymentSettings(null, filters, sorts, pageIndex, pageSize);
            PaymentSettingListViewModel listViewModel = new PaymentSettingListViewModel { PaymentSettings = paymentSettingList?.PaymentSettings?.ToViewModel<PaymentSettingViewModel>().ToList() };
            listViewModel.PaymentSettings?.ForEach(item =>
            {
                item.IsApprovalRequiredList = GetBooleanList();
                item.IsOABRequiredList = GetBooleanList();
            });
            listViewModel.PaymentSettings?.ForEach(item => { item.IsApprovalRequiredList = GetBooleanList(); });
            SetListPagingData(listViewModel, paymentSettingList);
            if (isAssociated)
                SetPaymentToolMenu(listViewModel);
            SetPortalId(listViewModel, portalId);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return paymentSettingList?.PaymentSettings?.Count > 0 ? listViewModel : new PaymentSettingListViewModel() { PaymentSettings = new List<PaymentSettingViewModel>() };
        }

        //Associate payment settings to portal.
        public virtual bool AssociatePaymentSettings(int portalId, string paymentSettingIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return !string.IsNullOrEmpty(paymentSettingIds) ? _paymentClient.AssociatePaymentSettings(new PaymentSettingAssociationModel { PortalId = portalId, PaymentSettingId = paymentSettingIds, PublishState = ZnodePublishStatesEnum.PRODUCTION.ToString() }) : false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotPermitted:
                        message = Admin_Resources.ErrorPaymentMethodAssociation;
                        return false;
                    default:
                        message = Admin_Resources.AssociatedErrorMessage;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                message = Admin_Resources.AssociatedErrorMessage;
                return false;
            }
        }

        //Associate payment settings to portal.
        public virtual bool AssociatePaymentSettingsForInvoice(int portalId, string paymentSettingIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return !string.IsNullOrEmpty(paymentSettingIds) ? _paymentClient.AssociatePaymentSettingsForInvoice(new PaymentSettingAssociationModel { PortalId = portalId, PaymentSettingId = paymentSettingIds, PublishState = ZnodePublishStatesEnum.PRODUCTION.ToString() }) : false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotPermitted:
                        message = Admin_Resources.ErrorPaymentMethodAssociation;
                        return false;
                    default:
                        message = Admin_Resources.AssociatedErrorMessage;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                message = Admin_Resources.AssociatedErrorMessage;
                return false;
            }
        }

        //Remove associated payment settings to portal.
        public virtual bool RemoveAssociatedPaymentSettings(int portalId, string paymentSettingIds, bool isUsedForOfflinePayment = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return !string.IsNullOrEmpty(paymentSettingIds) ? _paymentClient.RemoveAssociatedPaymentSettings(new PaymentSettingAssociationModel { PortalId = portalId, PaymentSettingId = paymentSettingIds, IsUserForOfflinePayment = isUsedForOfflinePayment }) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Update portal payment settings.
        public virtual bool UpdatePortalPaymentSettings(int paymentSettingId, int portalId, string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                PaymentSettingViewModel paymentSettingViewModel = GetPortalPaymentSettingViewModel(portalId, paymentSettingId, data);
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

                return paymentSettingId > 0 ? _paymentClient.UpdatePortalPaymentSettings(new PaymentSettingPortalModel { PortalId = portalId, PaymentSettingId = paymentSettingId, PaymentDisplayName = paymentSettingViewModel.PaymentDisplayName, PaymentExternalId = paymentSettingViewModel.PaymentExternalId, IsApprovalRequired = paymentSettingViewModel.IsApprovalRequired, IsOABRequired = paymentSettingViewModel.IsOABRequired, PublishState = paymentSettingViewModel.PublishState }) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion

        #region Associate Portal Shipping
        //Get associated shipping list for portal.
        public virtual ShippingListViewModel GetAssociatedShippingList(int portalId, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalId > 0)
                SetPortalIdFilter(filters, portalId);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            ShippingListModel shippingListModel = _shippingClient.GetAssociatedShippingList(new ExpandCollection() { ZnodeProfileShippingEnum.ZnodeShipping.ToString() }, filters, sorts, pageIndex, recordPerPage);
            if (shippingListModel?.ShippingList?.Count > 0)
            {
                ShippingListViewModel shippingListViewModel = new ShippingListViewModel { ShippingList = shippingListModel?.ShippingList?.ToViewModel<ShippingViewModel>()?.ToList(), PortalName = shippingListModel.PortalName };

                shippingListViewModel.ShippingList?.ForEach(item =>
                {
                    if (IsNotNull(item.DestinationCountryCode))
                        item.DestinationCountryCode = shippingListViewModel?.ShippingList?.Where(x => x.ShippingId == item.ShippingId).Select(x => x.DestinationCountryCode).FirstOrDefault();
                    else
                        item.DestinationCountryCode = Admin_Resources.LabelAll;
                });

                SetListPagingData(shippingListViewModel, shippingListModel);

                //Set tool menu for associated shipping portal list grid view.
                SetAssociatedPortalShippingListToolMenu(shippingListViewModel);
                return shippingListViewModel;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>(), PortalName = shippingListModel?.PortalName };
        }

        //Get list of unassociated shipping for portal.
        public virtual ShippingListViewModel GetUnAssociatedShippingList(int portalId, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalId > 0)
                SetPortalIdFilter(filters, portalId);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            ShippingListModel shippingListModel = _shippingClient.GetUnAssociatedShippingList(null, filters, sorts, pageIndex, recordPerPage);
            if (shippingListModel?.ShippingList?.Count > 0)
            {
                ShippingListViewModel shippingListViewModel = new ShippingListViewModel { ShippingList = shippingListModel?.ShippingList?.ToViewModel<ShippingViewModel>()?.ToList() };
                shippingListViewModel.ShippingList?.ForEach(item =>
                {
                    if (IsNotNull(item.DestinationCountryCode))
                        item.DestinationCountryCode = shippingListViewModel?.ShippingList?.Where(x => x.ShippingId == item.ShippingId).Select(x => x.DestinationCountryCode).FirstOrDefault();
                    else
                        item.DestinationCountryCode = Admin_Resources.LabelAll;
                });
                SetListPagingData(shippingListViewModel, shippingListModel);
                return shippingListViewModel;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };
        }

        //Associate shipping to portal.
        public virtual bool AssociateShipping(int portalId, string shippingIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("Associate Shipping with:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { ShippingIds = shippingIds });

                return _shippingClient.AssociateShipping(new PortalProfileShippingModel { ShippingIds = shippingIds, PortalId = portalId, PublishState = ZnodePublishStatesEnum.PRODUCTION.ToString() });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Remove associated shipping from portal.
        public virtual bool UnAssociateAssociatedShipping(string shippingId, int portalId)
             => _shippingClient.UnAssociateAssociatedShipping(new PortalProfileShippingModel() { ShippingIds = shippingId, PortalId = portalId });
        #endregion


        //Get analytics data for store.
        public virtual AnalyticsViewModel GetAnalytics(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalId > 0)
            {
                AnalyticsViewModel viewModel = new AnalyticsViewModel();
                viewModel.TagManager = _tagManagerClient.GetTagManager(portalId, SetExpandForPortal()).ToViewModel<TagManagerViewModel>();
                viewModel.TrackingPixel = _portalClient.GetPortalTrackingPixel(portalId, new ExpandCollection() { ZnodePortalProfileEnum.ZnodePortal.ToString() })?.ToViewModel<PortalTrackingPixelViewModel>();
                viewModel.PortalId = viewModel.TagManager.PortalId;
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

                return viewModel;
            }
            else
                return new AnalyticsViewModel { PortalId = portalId, TagManager = new TagManagerViewModel { PortalId = portalId }, TrackingPixel = new PortalTrackingPixelViewModel { PortalId = portalId } };
        }

        //Save analytics data for store.
        public bool SaveAnalytics(AnalyticsViewModel viewModel, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                errorMessage = string.Empty;
                bool isTagManagerSave = true;
                bool isTrackingPixelSave = true;

                if ((IsNull(viewModel.TagManager.AnalyticsUId) && (viewModel.TagManager.AnalyticsIsActive)) && (IsNull(viewModel.TagManager.ContainerId) && viewModel.TagManager.IsActive))
                {
                    isTagManagerSave = false;
                    viewModel.HasError = true;
                    throw new ZnodeException(ErrorCodes.InvalidData, errorMessage=Admin_Resources.ErrorAnalyticsAndContainerIDs);
                }
                if ((IsNull(viewModel.TagManager.AnalyticsUId) && viewModel.TagManager.AnalyticsIsActive))
                {
                    isTagManagerSave = false;
                    viewModel.TagManager.AnalyticsIsActive = false;
                    viewModel.HasError = true;
                    throw new ZnodeException(ErrorCodes.InvalidData, errorMessage=Admin_Resources.ErrrorAnalyticsID);
                }

                if ((IsNull(viewModel.TagManager.ContainerId) && viewModel.TagManager.IsActive))
                {
                    viewModel.TagManager.IsActive = false;
                    viewModel.TagManager.EnableEnhancedEcommerce = false;
                    viewModel.HasError = true;
                    throw new ZnodeException(ErrorCodes.InvalidData, errorMessage=Admin_Resources.ErrorContainerID);
                }
                if (!viewModel.HasError)
                {
                    return _tagManagerClient.SaveTagManager(viewModel?.TagManager?.ToModel<TagManagerModel>());
                }

                if (IsNotNull(viewModel?.TrackingPixel))
                     return  _portalClient.SavePortalTrackingPixel(viewModel?.TrackingPixel?.ToModel<PortalTrackingPixelModel>());

                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                errorMessage = ex.ErrorMessage;
                return false;
            }
        }
        public virtual PublishPortalLogListViewModel GetPortalPublishStatus(int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            filters = IsNull(filters) ? new FilterCollection() : filters;

            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodePortalEnum.PortalId.ToString(), StringComparison.OrdinalIgnoreCase));
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });

            PublishPortalLogListModel publishPortalLogList = _portalClient.GetPortalPublishStatus(filters, sorts, pageIndex, pageSize);
            PublishPortalLogListViewModel publishPortalLogListViewModel = new PublishPortalLogListViewModel { PublishPortalLog = publishPortalLogList?.PublishPortalLogList?.ToViewModel<PublishPortalLogViewModel>().ToList(), StoreName = publishPortalLogList?.StoreName };
            SetListPagingData(publishPortalLogListViewModel, publishPortalLogList);

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return publishPortalLogList?.PublishPortalLogList?.Count > 0 ? publishPortalLogListViewModel : new PublishPortalLogListViewModel { PublishPortalLog = new List<PublishPortalLogViewModel>(), StoreName = publishPortalLogList?.StoreName };
        }

        #region Robots.Txt
        //Get robots.txt data.
        public virtual RobotsTxtViewModel GetRobotsTxt(int portalId)
             => (portalId > 0) ? (_portalClient.GetRobotsTxt(portalId, SetExpandForPortal())).ToViewModel<RobotsTxtViewModel>() : new RobotsTxtViewModel { PortalId = portalId };

        //Save robots.txt data.
        public virtual bool SaveRobotsTxt(RobotsTxtViewModel viewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(viewModel) && viewModel.PortalId > 0)
                    return _portalClient.SaveRobotsTxt(viewModel?.ToModel<RobotsTxtModel>());

                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }

        }
        #endregion

        public virtual PortalApprovalViewModel GetPortalApproverDetailsById(int portalId, int selectedApprovalTypeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (portalId > 0)
            {
                PortalApprovalViewModel portalApprovalViewModel = _portalClient.GetPortalApproverDetailsById(portalId)?.ToViewModel<PortalApprovalViewModel>();

                portalApprovalViewModel.PortalName = _portalClient.GetPortal(portalId, null)?.StoreName;
                FilterCollection filters = new FilterCollection
                {
                    new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()),
                    new FilterTuple(View_GetAssociatedCMSThemeToPortalEnum.IsAssociated.ToString(), FilterOperators.Equals, AdminConstants.True)
                };
                ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });

                List<PaymentSettingViewModel> paymentSettings = GetPaymentSettingsList(portalId, filters, null, null, null, true)?.PaymentSettings?.Where(m => m.IsOABSupported())?.ToList();

                ZnodeLogging.LogMessage("paymentSettings list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, paymentSettings?.Count());
                if (portalApprovalViewModel.PortalPaymentUserApproverList?.Count > 0)
                {
                    foreach (var item in portalApprovalViewModel.PortalPaymentUserApproverList)
                    {
                        string[] paymentSettingIds = portalApprovalViewModel.PortalPaymentUserApproverList.Where(x => x.PortalPaymentGroupId != item.PortalPaymentGroupId)?.SelectMany(y => y.PaymentSettingIds)?.ToArray();
                        item.PaymentTypes = GetSelectedPaymentSettings(paymentSettings, paymentSettingIds);
                    }
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

                return portalApprovalViewModel;
            }
            return new PortalApprovalViewModel();
        }

        //Delete portal approvers by id.
        public bool DeletePortalApproverUserById(string userApproverId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return _portalClient.DeletePortalApproverUserById(new ParameterModel { Ids = userApproverId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Create approver level.
        public virtual bool SaveUpdatePortalApprovalDetails(PortalApprovalViewModel model, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                if (model.PortalPaymentUserApproverList?.Count > 0)
                {
                    foreach (var portalPaymentApproval in model.PortalPaymentUserApproverList)
                    {
                        if (IsNotNull(portalPaymentApproval.ApprovalUserIds) && IsNotNull(portalPaymentApproval.PaymentSettingIds))
                            portalPaymentApproval.UserApprover = GetUserApproverListModel(portalPaymentApproval.ApprovalUserIds);
                        else
                            throw new ZnodeException(ErrorCodes.InvalidData, errorMessage = Admin_Resources.TextAddValidData);
                    }
                }
                else
                    model.UserApprover = GetUserApproverListModel(model.ApprovalUserIds);
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

                return _portalClient.SaveUpdatePortalApprovalDetails(model.ToModel<PortalApprovalModel>());

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        public List<UserApproverViewModel> GetUserApproverListModel(string[] approvalUserIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            List<UserApproverViewModel> userApprovers = new List<UserApproverViewModel>();
            for (int i = 0; i < approvalUserIds?.Length; i++)
            {
                int[] approvalId = Array.ConvertAll(approvalUserIds[i].Split('_'), int.Parse);
                int approverUserid = approvalId[0];
                int userApprovalId = approvalId[1];
                int isActive = approvalId[2];
                userApprovers.Add(new UserApproverViewModel
                {
                    ApproverLevelId = 1,
                    ApproverOrder = 1,
                    UserId = null,
                    ApproverUserId = approverUserid,
                    UserApproverId = userApprovalId,
                    IsActive = Convert.ToBoolean(isActive)
                });
            }
            ZnodeLogging.LogMessage("userApprovers list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, userApprovers?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return userApprovers;
        }

        public List<SelectListItem> GetPaymentTypeList(int portalId, string[] paymentIds = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection
            {
                new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()),
                new FilterTuple(View_GetAssociatedCMSThemeToPortalEnum.IsAssociated.ToString(), FilterOperators.Equals, AdminConstants.True)
            };
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });

            List<PaymentSettingViewModel> paymentSettings = GetPaymentSettingsList(portalId, filters, null, null, null, true)?.PaymentSettings?.Where(m => m.IsOABSupported())?.ToList();

            ZnodeLogging.LogMessage("paymentSettings list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, paymentSettings?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return GetSelectedPaymentSettings(paymentSettings, paymentIds);
        }

        public List<SelectListItem> GetSelectedPaymentSettings(List<PaymentSettingViewModel> paymentSettings, string[] paymentIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            List<SelectListItem> selectListItem = new List<SelectListItem>();
            if (paymentIds?.Count() > 0)
                selectListItem.AddRange(paymentSettings?.Where(x => !paymentIds.Contains(x.PaymentSettingId.ToString())).Select(x => new SelectListItem { Text = x.PaymentDisplayName, Value = x.PaymentSettingId.ToString() })?.ToList());
            else
                selectListItem.AddRange(paymentSettings?.Select(x => new SelectListItem { Text = x.PaymentDisplayName, Value = x.PaymentSettingId.ToString() })?.ToList());

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return selectListItem;
        }
        #endregion

        #region Private Methods
        //Get List of select list items for Portal having content page and manage messages.
        private List<SelectListItem> GetPortalSelectItemList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            List<SelectListItem> selectListItem = new List<SelectListItem>();

            List<ManageMessageModel> manageMessageList = _manageMessageClient.GetManageMessages(null, null, null, null, null)?.ManageMessages;
            List<ContentPageModel> contentPageList = _contentPageClient.GetContentPageList(null, null, null, null, null)?.ContentPageList;
            ZnodeLogging.LogMessage("List count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { manageMessageListCount = manageMessageList?.Count(), contentPageListCount = contentPageList?.Count() });

            //If manage message list count is greater bind details to select list item.
            if (manageMessageList?.Count > 0)
            {
                manageMessageList = manageMessageList?.DistinctBy(x => x.PortalId)?.ToList();
                selectListItem = manageMessageList?.Where(x=>x.PortalId != null).Select(x => new SelectListItem { Text = x.StoreName, Value = x.PortalId.ToString() })?.ToList();
            }

            //If content page list count is greater bind details to select list item.
            if (contentPageList?.Count > 0)
            {
                contentPageList = contentPageList?.DistinctBy(x => x.PortalId)?.ToList();
                selectListItem.AddRange(contentPageList?.Select(x => new SelectListItem { Text = x.PortalName, Value = x.PortalId.ToString() })?.ToList());
            }

            if (selectListItem?.Count > 0)
                selectListItem = selectListItem?.DistinctBy(x => x.Value)?.OrderBy(x => x.Text)?.ToList();
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return selectListItem;
        }

        //Get the list of customer review states.
        private List<SelectListItem> GetCustomerReviewState()
            => new List<SelectListItem>(){
                 new SelectListItem(){Text=AdminConstants.DoNotPublishText,Value=AdminConstants.DoNotPublish,Selected=true},
                 new SelectListItem(){Text=AdminConstants.PublishImmediatelyText, Value=AdminConstants.PublishImmediately} };

        //Set the value of store feature.
        private StoreViewModel GetStoreFeatureValue(StoreViewModel storeViewModel)
        {
            if (storeViewModel?.AvailableStoreFeatureList?.Count > 0 && storeViewModel?.SelectedStoreFeatureList?.Count > 0)
            {
                foreach (StoreFeatureViewModel storeFeature in storeViewModel.AvailableStoreFeatureList)
                {
                    foreach (StoreFeatureViewModel item in storeViewModel.SelectedStoreFeatureList)
                    {
                        if (item.StoreFeatureId == storeFeature.StoreFeatureId)
                            storeFeature.StoreFeatureValue = true;
                    }
                }
            }
            return storeViewModel;
        }

        //Gets Current Portal 
        public StoreViewModel GetCurrentPortal()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            DomainListModel list = _domainClient.GetDomains(
             new FilterCollection()
             {
                            new FilterTuple(ZnodeDomainEnum.DomainName.ToString() ,FilterOperators.Like ,HttpContext.Current.Request.Url.Authority)
            }, new SortCollection(), null, null);

            if (list.Domains.Any())
            {
                PortalModel portal = _portalClient.GetPortal(list.Domains.First().PortalId, null);
                return StoreViewModelMap.ToViewModel(portal);
            }
            return null;
        }

        //Get a list of all warehouses .
        private void GetWarehouses(PortalWarehouseViewModel associatedWarehouses)
        {
            if (IsNotNull(associatedWarehouses))
            {
                associatedWarehouses.MainWarehouseList = new List<SelectListItem>();

                if (associatedWarehouses.WarehouseList?.Count > 0)
                    associatedWarehouses.WarehouseList.OrderBy(x => x.WarehouseCode).ToList().ForEach(item => { associatedWarehouses.MainWarehouseList.Add(new SelectListItem() { Text = item.WarehouseCode, Value = item.WarehouseId.ToString() }); });
            }
        }

        //Get a list of associated warehouse.
        private void AssociatedWarehouse(PortalWarehouseViewModel associatedWarehouses)
        {
            if (associatedWarehouses?.AlternateWarehouses?.Count > 0)
            {
                associatedWarehouses.AssociatedWarehouseList = new List<SelectListItem>();
                associatedWarehouses.AlternateWarehouses.ForEach(item => { associatedWarehouses.AssociatedWarehouseList.Add(new SelectListItem { Text = item.WarehouseCode, Value = item.WarehouseId.ToString() }); });
            }
        }

        //Get a list of Unassociated warehouse
        private void UnassociatedWarehouse(PortalWarehouseViewModel associatedWarehouses)
        {
            string filteredWarehouseIds = string.Join(",", associatedWarehouses.AlternateWarehouses?.Select(a => a.WarehouseId).ToArray());
            filteredWarehouseIds = string.IsNullOrEmpty(filteredWarehouseIds) ? associatedWarehouses.WarehouseId.ToString() : string.Concat(filteredWarehouseIds, "," + associatedWarehouses.WarehouseId.ToString());

            var unassociatedWarehouseList = associatedWarehouses.WarehouseList.Where(r => !filteredWarehouseIds.Split(',').Select(int.Parse).Contains(r.WarehouseId)).ToList();
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { unassociatedWarehouseList = unassociatedWarehouseList });
            if (associatedWarehouses?.WarehouseList?.Count > 0)
            {
                associatedWarehouses.UnassociatedWarehouseList = new List<SelectListItem>();
                unassociatedWarehouseList?.ForEach(item => { associatedWarehouses.UnassociatedWarehouseList.Add(new SelectListItem() { Text = item.WarehouseCode, Value = item.WarehouseId.ToString() }); });
            }
        }

        private void SetFiltersForLocales(FilterCollection filters, int portalId)
        {
            if (IsNotNull(filters))
                SetPortalIdFilter(filters, portalId);
        }

        //Set the Tool Menus for Store List Grid View.
        private void SetStoreListToolMenus(StoreListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('StoreDeletePopup');", ControllerName = "Store", ActionName = "DeleteStore" });
            }
        }


        //Set the Tool Menus for URL List Grid View.
        private void SetURLListToolMenus(DomainListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, DataTarget = "#URLDeletePopup", DataToggleModel = "modal", ControllerName = "Store", ActionName = "DeleteUrl" });
            }
        }

        //Set the Tool Menus for price List associated to store Grid View.
        private void SetPriceListToolMenuForStore(PriceListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PriceListStoreDeletePopup',this)", ControllerName = "Store", ActionName = "RemoveAssociatedPriceListToStore" });
            }
        }

        //Set the Tool Menus for profile List associated to store Grid View.
        private void SetPriceListToolMenuForProfile(PriceListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PriceListProfileDeletePopup',this)", ControllerName = "Store", ActionName = "RemoveAssociatedPriceListToProfile" });
            }
        }

        //Set tool menu for Locale list grid view.
        private void SetActiveLocaleListToolMenu(LocaleListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "Store.prototype.ActiveSubmit('','Store','UpdateLocale','LocaleList')", ControllerName = "Store", ActionName = "UpdateLocale" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextInactive, JSFunctionName = "Store.prototype.DeActivateSubmit('','Store','UpdateLocale','LocaleList')", ControllerName = "Store", ActionName = "UpdateLocale" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = GlobalSetting_Resources.IsDefault, JSFunctionName = "Store.prototype.DefaultSubmit('','Store','UpdateLocale','LocaleList')", ControllerName = "Store", ActionName = "UpdateLocale" });
            }
        }

        //Set tool menu for URL list grid view.
        private void SetUrlListToolMenu(DomainListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('URLDeletePopup')", ControllerName = "Store", ActionName = "DeleteUrl" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "EditableText.prototype.DialogDelete('domainEnable')", ControllerName = "Store", ActionName = "EnableDisableDomain" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextInactive, JSFunctionName = "EditableText.prototype.DialogDelete('domainDisable')", ControllerName = "Store", ActionName = "EnableDisableDomain" });
            }
        }

        //Get List of select list items for Fedex drop off types.
        private List<SelectListItem> GetFedexDropOffType()
        {
            List<SelectListItem> fedexDropOffTypes = new List<SelectListItem>(){
                new SelectListItem(){Text=Admin_Resources.BUSINESS_SERVICE_CENTERText, Value=Admin_Resources.BUSINESS_SERVICE_CENTER},
                new SelectListItem(){Text=Admin_Resources.DROP_BOXText,Value=Admin_Resources.DROP_BOX},
                new SelectListItem(){Text=Admin_Resources.REGULAR_PICKUPText, Value=Admin_Resources.REGULAR_PICKUP},
                new SelectListItem(){Text=Admin_Resources.REQUEST_COURIERText, Value=Admin_Resources.REQUEST_COURIER},
                new SelectListItem(){Text=Admin_Resources.STATIONText, Value=Admin_Resources.STATION}
            };
            ZnodeLogging.LogMessage("fedexDropOffTypes list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, fedexDropOffTypes?.Count());

            return fedexDropOffTypes;
        }

        // Get List of select list items for Packaging types.
        private List<SelectListItem> GetFedexPackagingType()
        {
            List<SelectListItem> fedexPackagingType = new List<SelectListItem>(){
                new SelectListItem(){Text=Admin_Resources.YOUR_PACKAGINGText, Value=Admin_Resources.YOUR_PACKAGING},
                new SelectListItem(){Text=Admin_Resources.FEDEX_BOXText,Value=Admin_Resources.FEDEX_BOX},
                new SelectListItem(){Text=Admin_Resources.FEDEX_ENVELOPEText, Value=Admin_Resources.FEDEX_ENVELOPE},
                new SelectListItem(){Text=Admin_Resources.FEDEX_TUBEText, Value=Admin_Resources.FEDEX_TUBE},
                new SelectListItem(){Text=Admin_Resources.FEDEX_PAKText, Value=Admin_Resources.FEDEX_PAK}
            };
            ZnodeLogging.LogMessage("fedexPackagingType list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, fedexPackagingType?.Count());

            return fedexPackagingType;
        }


        //Get List of select list items for UPS drop off types.
        public virtual List<SelectListItem> GetUPSDropOffType()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            List<SelectListItem> upsDropOffTypes = new List<SelectListItem>(){
                new SelectListItem(){Text=Admin_Resources.Regular_Daily_Pickup, Value=Admin_Resources.Regular_Daily_Pickup},
                new SelectListItem(){Text=Admin_Resources.On_Call_Air,Value=Admin_Resources.On_Call_Air},
                new SelectListItem(){Text=Admin_Resources.One_Time_Pickup, Value=Admin_Resources.One_Time_Pickup},
                new SelectListItem(){Text=Admin_Resources.Letter_Center, Value=Admin_Resources.Letter_Center},
                new SelectListItem(){Text=Admin_Resources.Customer_Counter, Value=Admin_Resources.Customer_Counter}
            };
            ZnodeLogging.LogMessage("upsDropOffTypes list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, upsDropOffTypes?.Count());

            return upsDropOffTypes;
        }

        // Get List of select list items for Packaging types.
        public virtual List<SelectListItem> GetUPSPackagingType()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            List<SelectListItem> upsPackagingType = new List<SelectListItem>(){
                new SelectListItem(){Text=Admin_Resources.UPS_Letter_Envelope, Value=Admin_Resources.UPS_Letter_Envelope_Value},
                new SelectListItem(){Text=Admin_Resources.UPS_Express_Box,Value=Admin_Resources.UPS_Express_Box_Value},
                new SelectListItem(){Text=Admin_Resources.UPS_Tube, Value=Admin_Resources.UPS_Tube_Value},
                new SelectListItem(){Text=Admin_Resources.UPS_Worldwide_10KG_Box, Value=Admin_Resources.UPS_Worldwide_10KG_Box_Value},
                new SelectListItem(){Text=Admin_Resources.UPS_Worldwide_25KG_Box, Value=Admin_Resources.UPS_Worldwide_25KG_Box_Value},
                new SelectListItem(){Text=Admin_Resources.UPS_My_Packaging, Value=Admin_Resources.UPS_My_Packaging_Value},
                new SelectListItem(){Text=Admin_Resources.UPS_Express_Box_Small, Value=Admin_Resources.UPS_Express_Box_Small_Value},
                new SelectListItem(){Text=Admin_Resources.UPS_Express_Box_Medium, Value=Admin_Resources.UPS_Express_Box_Medium_Value},
                new SelectListItem(){Text=Admin_Resources.UPS_Express_Box_Large, Value=Admin_Resources.UPS_Express_Box_Large_Value},
                new SelectListItem(){Text=Admin_Resources.UPS_Pak, Value=Admin_Resources.UPS_Pak_Value},
                new SelectListItem(){Text=Admin_Resources.UPS_Pallet, Value=Admin_Resources.UPS_Pallet_Value}
            };
            ZnodeLogging.LogMessage("upsPackagingType list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, upsPackagingType?.Count());
            return upsPackagingType;
        }

        //Get country List
        public virtual List<SelectListItem> GetCountryListByPortalId(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            if (portalId > 0)
                filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });


            CountryListModel countryList = _portalCountryClient.GetAssociatedCountryList(null, filters, null, null, null);
            List<SelectListItem> selectedCountryList = new List<SelectListItem>();
            if (countryList?.Countries?.Count > 0)
                countryList.Countries.ToList().ForEach(item => { selectedCountryList.Add(new SelectListItem() { Text = item.CountryCode, Value = item.CountryCode.ToString() }); });

            ZnodeLogging.LogMessage("selectedCountryList list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, selectedCountryList?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return selectedCountryList;
        }

        //Bind portal shipping related data like PackagingTypes, FedexDropOffTypes,CountryList.
        public virtual PortalShippingViewModel BindPageDropdown(PortalShippingViewModel model, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            //Country list         
            model.countryList = GetCountryListByPortalId(portalId);
            //PackagingTypes list
            model.PackagingTypes = GetFedexPackagingType();
            //Fedex Drop Off Types
            model.FedexDropOffTypes = GetFedexDropOffType();
            // UPS PackagingTypes list
            model.UPSPackagingTypes = GetUPSPackagingType();
            // UPS Drop Off Types
            model.UPSDropOffTypes = GetUPSDropOffType();

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return model;
        }

        //Set filter for mode i.e profile or portal.
        private void SetFilterForMode(FilterCollection filters, string mode)
        {
            if (filters.Exists(x => x.Item1 == FilterKeys.Mode))
            {
                //If Mode is already present in filters, remove it.
                filters.RemoveAll(x => x.Item1 == FilterKeys.Mode);

                //Add New Mode into filters.
                filters.Add(new FilterTuple(FilterKeys.Mode, FilterOperators.Equals, mode));
            }
            else
                filters.Add(new FilterTuple(FilterKeys.Mode, FilterOperators.Equals, mode));

            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });

        }

        //Set the portal id field.
        private void SetPortalIdFilter(FilterCollection filters, int portalId)
        {
            if (IsNotNull(filters))
            {
                if (filters.Exists(x => string.Equals(x.Item1, FilterKeys.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    //If PortalId is already present in filters, remove it.
                    filters.RemoveAll(x => string.Equals(x.Item1, FilterKeys.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase));

                    //Add New PortalId into filters.
                    filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));
            }
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });

        }
        //Get brand product model.
        private TaxClassPortalModel GetTaxClassPortalModel(int portalId, string taxClassIds, bool isUnAssociated)
        {
            return new TaxClassPortalModel()
            {
                TaxClassIds = taxClassIds,
                PortalId = portalId,
                IsUnAssociated = isUnAssociated
            };
        }

        //Set the tool menus for taxclass associated to store.
        private void SetTaxClassListToolMenu(TaxClassListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('StoreTaxDeletePopup',this)", ControllerName = "Store", ActionName = "UnAssociateTaxClass" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = GlobalSetting_Resources.IsDefault, JSFunctionName = "Store.prototype.SetPortalDefaultTax()", ControllerName = "Store", ActionName = "SetPortalDefaultTax" });
            }
            //Set tool menu for brand list grid view.
        }

        //Set Filter for store taxclass list.
        private void SetFilterforStoreTaxClassList(FilterCollection filters, int portalId, bool isAssociated)
        {
            SetPortalIdFilter(filters, portalId);
            SetIsAssociatedFilter(filters, isAssociated);
            if (!isAssociated)
                SetIsActiveFilter(filters, true);

            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });

        }

        //Set payment tools.
        private void SetPaymentToolMenu(PaymentSettingListViewModel listViewModel)
        {
            if (IsNotNull(listViewModel))
            {
                listViewModel.GridModel = GetGridModel();
                listViewModel.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('StoreTaxDeletePopup',this)", ControllerName = "Store", ActionName = "RemoveAssociatedPaymentSetting" });
            }
        }

        //Set the isAssociated field.
        private void SetIsAssociatedFilter(FilterCollection filters, bool isAssociated)
        {
            if (IsNotNull(filters))
            {
                if (filters.Exists(x => x.Item1 == FilterKeys.IsAssociated.ToString()))
                {
                    filters.RemoveAll(x => x.Item1 == FilterKeys.IsAssociated.ToString());
                    filters.Add(new FilterTuple(FilterKeys.IsAssociated.ToString(), FilterOperators.Equals, isAssociated.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.IsAssociated.ToString(), FilterOperators.Equals, isAssociated.ToString()));


            }
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });

        }

        //Set isactive filter.
        private void SetIsActiveFilter(FilterCollection filters, bool isActive)
        {
            if (IsNotNull(filters))
            {
                if (filters.Exists(x => x.Item1 == FilterKeys.IsActive.ToString()))
                {
                    filters.RemoveAll(x => x.Item1 == FilterKeys.IsAssociated.ToString());
                    filters.Add(new FilterTuple(FilterKeys.IsActive.ToString(), FilterOperators.Equals, isActive.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.IsActive.ToString(), FilterOperators.Equals, isActive.ToString()));
            }
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters });

        }
        private static SortCollection SetSort(SortCollection sorts)
        {
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(ZnodePaymentSettingEnum.DisplayOrder.ToString(), DynamicGridConstants.ASCKey);
            }
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { sorts = sorts });

            return sorts;
        }

        //Set tool menu for associated shipping portal list grid view.
        private void SetAssociatedPortalShippingListToolMenu(ShippingListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PortalShippingDeletePopup',this)", ControllerName = "Store", ActionName = "UnAssociateAssociatedShipping" });
            }
        }

        //Set portal id in model.
        private void SetPortalId(PaymentSettingListViewModel listViewModel, int portalId)
        {
            if (listViewModel?.PaymentSettings?.Count > 0)
                listViewModel.PaymentSettings.ForEach(x => x.PortalId = portalId);
        }

        //Set expand for portal. 
        private static ExpandCollection SetExpandForPortal()
        {
            //Expands to get data from another table.
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeGoogleTagManagerEnum.ZnodePortal.ToString());
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { expands = expands });

            return expands;
        }

        //Get the portal payment settings.
        private PaymentSettingViewModel GetPortalPaymentSettingViewModel(int portalId, int paymentSettingId, string data)
        {
            PaymentSettingViewModel portalPaymentDetail = JsonConvert.DeserializeObject<PaymentSettingViewModel[]>(data)[0];
            return new PaymentSettingViewModel()
            {
                PortalId = portalId,
                ProfileId = portalPaymentDetail.ProfileId,
                DisplayOrder = portalPaymentDetail.DisplayOrder,
                PaymentDisplayName = portalPaymentDetail.PaymentDisplayName,
                PaymentGateway = portalPaymentDetail.PaymentGateway,
                PaymentGatewayId = portalPaymentDetail.PaymentGatewayId,
                IsActive = portalPaymentDetail.IsActive,
                PaymentExternalId = portalPaymentDetail.PaymentExternalId,
                IsApprovalRequired = portalPaymentDetail.IsApprovalRequired,
                IsOABRequired = portalPaymentDetail.IsOABRequired,
                PublishState = DefaultSettingHelper.GetCurrentOrDefaultAppType(portalPaymentDetail.PublishState),
                PaymentSettingId = paymentSettingId
            };
        }
        // Get Application Type 
        public List<SelectListItem> GetApplicationType()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            IGeneralSettingAgent _generalSettingAgent = new GeneralSettingAgent(new GeneralSettingClient());

            IEnumerable<PublishStateMappingViewModel> mappingList = _generalSettingAgent.GetAvailablePublishStateMappings();
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return mappingList.Select(x => new SelectListItem() { Text = x.PublishStateCode, Value = x.PublishState }).ToList();
        }

        //Associate shipping to portal.
        public virtual bool UpdateShippingToPortal(int portalId, string shippingIds, string applicationType = "")
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            try
            {
                ZnodeLogging.LogMessage("Updated shipping with", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { ShippingIds = shippingIds });


                applicationType = DefaultSettingHelper.GetCurrentOrDefaultAppType(applicationType);

                return _shippingClient.UpdateShippingToPortal(new PortalProfileShippingModel { ShippingIds = shippingIds, PortalId = portalId, PublishState = applicationType });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return false;
            }
        }

        private void SetPageToolMenu(PortalPageSettingListViewModel listViewModel)
        {
            if (IsNotNull(listViewModel))
            {
                listViewModel.GridModel = GetGridModel();
                listViewModel.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('StorePageSettingPopup',this)", ControllerName = "Store", ActionName = "RemoveAssociatedPageSetting" });
            }
        }

        //Set sort tools.
        private void SetSortToolMenu(PortalSortSettingListViewModel listViewModel)
        {
            if (IsNotNull(listViewModel))
            {
                listViewModel.GridModel = GetGridModel();
                listViewModel.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('StoreSortSettingPopup',this)", ControllerName = "Store", ActionName = "RemoveAssociatedSortSetting" });
            }
        }
        #endregion
    }
}