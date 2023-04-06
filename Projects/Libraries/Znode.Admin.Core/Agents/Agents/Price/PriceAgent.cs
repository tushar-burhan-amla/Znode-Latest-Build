using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
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
using System.Xml.Linq;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class PriceAgent : BaseAgent, IPriceAgent
    {
        #region Private Variables
        private readonly ICurrencyClient _currencyClient;
        private readonly IPriceClient _priceClient;
        private readonly IPortalClient _portalClient;
        private readonly IAccountClient _accountClient;
        private readonly IImportClient _importClient;
        private readonly IImportAgent _importAgent;
        #endregion

        #region Constructor
        public PriceAgent(ICurrencyClient currencyClient, IPriceClient priceClient, IPortalClient portalClient, IAccountClient accountClient, IImportClient importClient)
        {
            _currencyClient = GetClient<ICurrencyClient>(currencyClient);
            _priceClient = GetClient<IPriceClient>(priceClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _accountClient = GetClient<IAccountClient>(accountClient);
            _importClient = GetClient<IImportClient>(importClient);
            _importAgent = new ImportAgent(GetClient<ImportClient>(), GetClient<PriceClient>(), GetClient<CountryClient>(), GetClient<PortalClient>(), GetClient<CatalogClient>());
        }
        #endregion

        #region Public Methods

        #region Price

        //Get currency list.
        public virtual List<SelectListItem> GetCurrencyList(int currencyId)
           => PriceViewModelMap.ToListItems(_currencyClient.GetCurrencyList(null, GetActiveFilter(), GetSortByCurrencyName())?.Currencies, currencyId);

        //Get culture list.
        public virtual List<SelectListItem> GetCultureList(int cultureId,int currencyId)
           => PriceViewModelMap.ToListItems(_currencyClient.GetCultureList(null, GetActiveFilter(currencyId), null)?.Culture, cultureId);

        //Method to create price list.
        public virtual PriceViewModel Create(PriceViewModel priceViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                List<ImportPriceViewModel> importPriceList = null;
                HttpPostedFileBase httpPostedFile = priceViewModel.FilePath;
                //Set import model data.
                ImportViewModel importModel = SetImportModelData(priceViewModel, httpPostedFile);
                //Check for how many pricing records created successfully.
                priceViewModel = _priceClient.CreatePrice(priceViewModel.ToModel<PriceModel>()).ToViewModel<PriceViewModel>();

                if (IsNotNull(httpPostedFile) && priceViewModel.PriceListId > 0)
                {
                    importModel.PriceListId = priceViewModel.PriceListId;
                    ZnodeLogging.LogMessage("PriceViewModel with PriceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListId = priceViewModel?.PriceListId });
                    priceViewModel.FilePath = httpPostedFile;
                    _importAgent.ImportData(importModel);
                }
                if (priceViewModel?.ImportPriceList.Count > 0 && importPriceList?.Count > priceViewModel?.ImportPriceList.GroupBy(x => x.SequenceNumber).Count())
                    priceViewModel = (PriceViewModel)GetViewModelWithErrorMessage(priceViewModel, Admin_Resources.ErrorSomeRecordUpdation);

                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return IsNotNull(priceViewModel) ? priceViewModel : new PriceViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (PriceViewModel)GetViewModelWithErrorMessage(new PriceViewModel(), Admin_Resources.AlreadyExistCode);
                    default:
                        return (PriceViewModel)GetViewModelWithErrorMessage(new PriceViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return (PriceViewModel)GetViewModelWithErrorMessage(new PriceViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Method to get price by priceListId.
        public virtual PriceViewModel GetPrice(int priceListId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter priceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListId = priceListId });
            PriceModel priceModel = _priceClient.GetPrice(priceListId);
            if (IsNotNull(priceModel))
            {
                priceModel.OldCurrencyId = priceModel.CurrencyId;
                return priceModel.ToViewModel<PriceViewModel>();
            }
            return null;
        }

        //Method to update price list.
        public virtual PriceViewModel Update(PriceViewModel priceViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                List<ImportPriceViewModel> importPriceList = null;
                HttpPostedFileBase httpPostedFile = priceViewModel.FilePath;

                //Set import model data.
                ImportViewModel importModel = SetImportModelData(priceViewModel, httpPostedFile);

                //Update new pricing list.
                priceViewModel = _priceClient.UpdatePrice(priceViewModel.ToModel<PriceModel>())?.ToViewModel<PriceViewModel>();
                if (IsNotNull(httpPostedFile))
                {
                    priceViewModel.FilePath = httpPostedFile;
                    ZnodeLogging.LogMessage("PriceViewModel with PriceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListId = priceViewModel?.PriceListId });
                    importModel.PriceListId = priceViewModel.PriceListId;
                    _importAgent.ImportData(importModel);
                }
                //Check for how many pricing records updated successfully.
                if (priceViewModel?.ImportPriceList?.Count > 0 && importPriceList?.Count > priceViewModel?.ImportPriceList?.GroupBy(x => x.SequenceNumber).Count())
                    priceViewModel = (PriceViewModel)GetViewModelWithErrorMessage(priceViewModel, Admin_Resources.ErrorSomeRecordUpdation);

                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return priceViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return (PriceViewModel)GetViewModelWithErrorMessage(new PriceViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Method to get price list.
        public virtual PriceListViewModel GetPriceList(FilterCollection filters, SortCollection sorts) => GetPriceList(filters, sorts, null, null);

        //Get Price list.
        public virtual PriceListViewModel GetPriceList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            //Get the sort collection for price list id desc.
            sorts = HelperMethods.SortDesc(ZnodePriceListEnum.PriceListId.ToString(), sorts);

            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodePriceListEnum.ZnodePriceListProfiles.ToString());

            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });
            PriceListModel priceList = _priceClient.GetPriceList(expands, filters, sorts, pageIndex, pageSize);
            PriceListViewModel listViewModel = new PriceListViewModel { PriceList = priceList?.PriceList?.ToViewModel<PriceViewModel>().ToList() };

            SetListPagingData(listViewModel, priceList);

            //Set tool menu for price list grid view.
            SetPriceListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceList?.PriceList?.Count > 0 ? listViewModel : new PriceListViewModel() { PriceList = new List<PriceViewModel>() };
        }

        //Delete Price.
        public virtual bool DeletePrice(string priceListId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorFailedToDelete;
            try
            {
                return _priceClient.DeletePrice(new ParameterModel { Ids = priceListId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = Admin_Resources.PriceListAssociationDeleteErrorMessage;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Copy price
        public virtual bool CopyPrice(PriceViewModel priceViewModel, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorFailedToCopy;
            try
            {
                ZnodeLogging.LogMessage("PriceViewModel with PriceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListId = priceViewModel?.PriceListId });
                return _priceClient.CopyPrice(priceViewModel.ToModel<PriceModel>());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        errorMessage = Admin_Resources.AlreadyExistCode;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToCopy;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToCopy;
                return false;
            }
        }

        //Method to get price by priceListId.
        public virtual List<SelectListItem> GetAllActivePriceList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            List<SelectListItem> priceList = PriceViewModelMap.PriceListToListItems(_priceClient.GetPriceList(null, null, null, null, null)?.PriceList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return IsNull(priceList) ? new List<SelectListItem>() : priceList;
        }
        #endregion

        #region SKU Price

        //Method to create sku price.
        public virtual PriceSKUDataViewModel AddSKUPrice(PriceSKUDataViewModel priceSKUDataViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                //Bind SKU price data.
                GetPriceTierList(priceSKUDataViewModel);
                priceSKUDataViewModel.PriceSKU = _priceClient.AddSKUPrice(priceSKUDataViewModel?.PriceSKU?.ToModel<PriceSKUModel>())?.ToViewModel<PriceSKUViewModel>();
                
                string quantity = priceSKUDataViewModel?.PriceTierList?.Select(x => x.Quantity)?.FirstOrDefault().ToString();

                ZnodeLogging.LogMessage("Quantity:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Quantity = quantity });
                if (!string.IsNullOrWhiteSpace(quantity))
                    _priceClient.AddTierPrice(PriceViewModelMap.ToPriceTierListModel(priceSKUDataViewModel));

                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return priceSKUDataViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        {
                            priceSKUDataViewModel.PriceTierList.Clear();
                            return (PriceSKUDataViewModel)GetViewModelWithErrorMessage(priceSKUDataViewModel, Admin_Resources.ErrorSKUAlreadyExists);
                        }
                    case ErrorCodes.DuplicateQuantityError:
                        return (PriceSKUDataViewModel)GetViewModelWithErrorMessage(priceSKUDataViewModel, Admin_Resources.DuplicateTierPriceErrorMessage);
                    default:
                        return (PriceSKUDataViewModel)GetViewModelWithErrorMessage(priceSKUDataViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return (PriceSKUDataViewModel)GetViewModelWithErrorMessage(priceSKUDataViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Method to get sku price list.
        public virtual PriceSKUListViewModel GetSKUPriceList(int priceListId, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            //Get the sort collection for price id desc.
            sorts = HelperMethods.SortDesc(ZnodePriceEnum.PriceId.ToString(), sorts);
            ZnodeLogging.LogMessage("Input parameters filters, sort collection and priceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts, PriceListId = priceListId });
            PriceSKUListModel priceSKUList = _priceClient.GetSKUPriceList(null, filters, sorts, pageIndex, recordPerPage);
            PriceSKUListViewModel priceSKUListViewModel = new PriceSKUListViewModel { PriceSKUList = priceSKUList?.PriceSKUList?.ToViewModel<PriceSKUViewModel>().ToList() };
            priceSKUListViewModel.ListName = _priceClient.GetPrice(priceListId)?.ListName;
            SetListPagingData(priceSKUListViewModel, priceSKUList);

            //Set tool menu for sku price list grid view.
            SetPriceSKUListToolMenu(priceSKUListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceSKUList?.PriceSKUList?.Count > 0 ? priceSKUListViewModel
                : new PriceSKUListViewModel() { PriceSKUList = new List<PriceSKUViewModel>(), ListName = priceSKUListViewModel.ListName };
        }

        //Get sku price by priceId.
        public virtual PriceSKUViewModel GetSKUPrice(int priceId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
             PriceSKUModel priceSKUModel = _priceClient.GetSKUPrice(priceId);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return !Equals(priceSKUModel, null) ? priceSKUModel.ToViewModel<PriceSKUViewModel>() : new PriceSKUViewModel();
        }

        //Method to update sku price.
        public virtual PriceSKUDataViewModel UpdateSKUPrice(PriceSKUDataViewModel priceSKUDataViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                //Bind SKU price data.
                GetPriceTierList(priceSKUDataViewModel);
                priceSKUDataViewModel.PriceSKU = _priceClient.UpdateSKUPrice(priceSKUDataViewModel?.PriceSKU?.ToModel<PriceSKUModel>())?.ToViewModel<PriceSKUViewModel>();
                ZnodeLogging.LogMessage("PriceSKUViewModel with PriceId and PriceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceId = priceSKUDataViewModel?.PriceSKU?.PriceId, PriceListId = priceSKUDataViewModel?.PriceSKU?.PriceListId });

                if (!string.IsNullOrWhiteSpace(priceSKUDataViewModel?.PriceTierList?.Select(x => x.Quantity)?.FirstOrDefault().ToString()))
                    _priceClient.AddTierPrice(PriceViewModelMap.ToPriceTierListModel(priceSKUDataViewModel));
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return priceSKUDataViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.DuplicateQuantityError:
                        return (PriceSKUDataViewModel)GetViewModelWithErrorMessage(priceSKUDataViewModel, Admin_Resources.DuplicateTierPriceErrorMessage);
                    default:
                        return (PriceSKUDataViewModel)GetViewModelWithErrorMessage(priceSKUDataViewModel, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return (PriceSKUDataViewModel)GetViewModelWithErrorMessage(new PriceSKUDataViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Method to delete sku price.
        public virtual bool DeleteSKUPrice(string priceId, int priceListId, out string message, int pimProductId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            message = string.Empty;
            if (!string.IsNullOrEmpty(priceId))
            {
                try
                {
                    ZnodeLogging.LogMessage("Input parameters priceId and priceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceId = priceId, PriceListId = priceListId });
                    return _priceClient.DeleteSKUPrice(new SKUPriceDeleteModel { PriceId = priceId.ToString(), PriceListId = priceListId, PimProductId = pimProductId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Method to get list of UOM.
        public virtual List<SelectListItem> UOM()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            List<SelectListItem> uomList = PriceViewModelMap.UOMToListItems(_priceClient.GetUomList(null, null, null, null, null)?.UomList);
            ZnodeLogging.LogMessage("UOM list count:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { UOMListCount = uomList?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return IsNull(uomList) ? new List<SelectListItem>() : uomList;
        }

        //Get price by Sku.
        public virtual PriceSKUViewModel GetPriceBySku(string pimProductId, string sku, string productType)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            PriceSKUModel priceSKUModel = new PriceSKUModel();
            if (!string.IsNullOrEmpty(sku))
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(FilterKeys.PimProductId, FilterOperators.Equals, pimProductId.ToString()));
                filters.Add(new FilterTuple(FilterKeys.Sku, FilterOperators.Equals, sku.ToString()));
                filters.Add(new FilterTuple(FilterKeys.ProductType, FilterOperators.Equals, productType.ToString()));

                ZnodeLogging.LogMessage("Filters to get SKU:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters });
                priceSKUModel = _priceClient.GetPriceBySku(filters);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return IsNotNull(priceSKUModel) ? priceSKUModel.ToViewModel<PriceSKUViewModel>() : new PriceSKUViewModel();
        }

        //Get pricing details by product price model.
        public virtual PriceSKUViewModel GetProductPricingDetailsBySku(ProductPriceListSKUDataViewModel productPriceListSKUDataViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            PriceSKUModel priceSKUModel = new PriceSKUModel();
            if (!string.IsNullOrEmpty(productPriceListSKUDataViewModel.Sku))
            {
                ZnodeLogging.LogMessage("SKU and ProductpriceListId to get product pricing details:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { SKU = productPriceListSKUDataViewModel?.Sku, ProductpriceListId = productPriceListSKUDataViewModel?.ProductpriceListId });
                priceSKUModel = _priceClient.GetProductPricingDetailsBySku(new ProductPriceListSKUDataModel() { Sku = productPriceListSKUDataViewModel.Sku, ProductpriceListId = productPriceListSKUDataViewModel.ProductpriceListId });
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return IsNotNull(priceSKUModel) ? priceSKUModel.ToViewModel<PriceSKUViewModel>() : new PriceSKUViewModel();
        }
        #endregion

        #region Associate Store
        //Method to get associated store list.
        public virtual PricePortalListViewModel GetAssociatedStoreList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });
            PricePortalListModel pricePortalList = _priceClient.GetAssociatedStoreList(null, filters, sorts, pageIndex, recordPerPage);
            PricePortalListViewModel pricePortalListViewModel = new PricePortalListViewModel { PricePortals = pricePortalList?.PricePortalList?.ToViewModel<PricePortalViewModel>().ToList() };
            SetListPagingData(pricePortalListViewModel, pricePortalList);

            //Set tool menu for associate store to price list grid view.
            SetAssociateStoreListToolMenu(pricePortalListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return pricePortalList?.PricePortalList?.Count > 0 ? pricePortalListViewModel
                : new PricePortalListViewModel() { PricePortals = new List<PricePortalViewModel>() };
        }

        public virtual void SetFilters(FilterCollection filters, int priceListId)
        {
            if (!Equals(filters, null))
            {
                //Checking For PriceListId already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == FilterKeys.PriceListId))
                {
                    //If PriceListId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == FilterKeys.PriceListId);

                    //Add New PriceListId Into filters
                    filters.Add(new FilterTuple(FilterKeys.PriceListId, FilterOperators.Equals, priceListId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.PriceListId, FilterOperators.Equals, priceListId.ToString()));
            }
            ZnodeLogging.LogMessage("Filters:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters });
        }

        //Method to get Un-Associated store list.
        public virtual StoreListViewModel GetUnAssociatedStoreList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            PortalListModel storeList = _priceClient.GetUnAssociatedStoreList(null, filters, sorts, pageIndex, recordPerPage);
            StoreListViewModel storeListViewModel = new StoreListViewModel { StoreList = storeList?.PortalList?.ToViewModel<StoreViewModel>().ToList() };
            SetListPagingData(storeListViewModel, storeList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return storeList?.PortalList?.Count > 0 ? storeListViewModel
                : new StoreListViewModel() { StoreList = new List<StoreViewModel>() };
        }

        //Method to associate to store.
        public virtual bool AssociateStore(int priceListId, string storeIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            message = Admin_Resources.ErrorAssociateStore;
            try
            {
                return _priceClient.AssociateStore(PriceViewModelMap.ToAssociateStoreListModel(priceListId, storeIds));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorAssociateStore;
            }
            return false;
        }

        //Method to remove associated store.
        public virtual bool RemoveAssociatedStores(string priceListPortalIds, int priceListId)
            => _priceClient.RemoveAssociatedStores(new RemoveAssociatedStoresModel() { PriceListId = priceListId, PriceListPortalIds = priceListPortalIds.ToString() });

        //Get associated stores precedence value.
        public virtual PricePortalViewModel GetAssociatedStorePrecedence(int priceListPortalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            PricePortalModel pricePortalModel = _priceClient.GetAssociatedStorePrecedence(priceListPortalId, new ExpandCollection() { ZnodePriceListPortalEnum.ZnodePortal.ToString() });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return IsNotNull(pricePortalModel) ? pricePortalModel.ToViewModel<PricePortalViewModel>() : new PricePortalViewModel();
        }

        //Update associated stores precedence value.
        public virtual PricePortalViewModel UpdateAssociatedStorePrecedence(PricePortalViewModel pricePortalViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                pricePortalViewModel = _priceClient.UpdateAssociatedStorePrecedence(pricePortalViewModel?.ToModel<PricePortalModel>())?.ToViewModel<PricePortalViewModel>();
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return pricePortalViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                return (PricePortalViewModel)GetViewModelWithErrorMessage(new PricePortalViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return (PricePortalViewModel)GetViewModelWithErrorMessage(new PricePortalViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }
        #endregion

        #region Associate Customer
        //Method to get associated customer list.
        public virtual PriceUserListViewModel GetAssociatedCustomerList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            //Set the sort collection for user id desc.
            HelperMethods.SortCustomerListDesc(ref sorts);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sorts });
            PriceUserListModel priceAccountListModel = _priceClient.GetAssociatedCustomerList(null, filters, sorts, pageIndex, recordPerPage);
            PriceUserListViewModel priceAccountListViewModel = new PriceUserListViewModel { PriceUserList = priceAccountListModel?.PriceUserList?.ToViewModel<PriceUserViewModel>().ToList() };
            SetListPagingData(priceAccountListViewModel, priceAccountListModel);

            //Set tool menu for associate customer to price list grid view.
            SetAssociateCustomerListToolMenu(priceAccountListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceAccountListModel?.PriceUserList?.Count > 0 ? priceAccountListViewModel
                : new PriceUserListViewModel() { PriceUserList = new List<PriceUserViewModel>() };
        }

        //Method to get Un-Associated customer list.
        public virtual PriceUserListViewModel GetUnAssociatedCustomerList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });
            PriceUserListModel priceAccountListModel = _priceClient.GetUnAssociatedCustomerList(null, filters, sorts, pageIndex, recordPerPage);
            PriceUserListViewModel PriceAccountListViewModel = new PriceUserListViewModel { PriceUserList = priceAccountListModel?.PriceUserList?.ToViewModel<PriceUserViewModel>().ToList() };
            SetListPagingData(PriceAccountListViewModel, priceAccountListModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceAccountListModel?.PriceUserList?.Count > 0 ? PriceAccountListViewModel
                : new PriceUserListViewModel() { PriceUserList = new List<PriceUserViewModel>() };
        }

        //Method to associated customer.
        public virtual bool AssociateCustomer(int priceListId, string customerIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters priceListId and customerIds:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListId = priceListId, CustomerIds = customerIds });
            message = Admin_Resources.ErrorAssociateCustomer;
            try
            {
                return _priceClient.AssociateCustomer(PriceViewModelMap.ToAssociateCustomerListModel(priceListId, customerIds));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorAssociateStore;
            }
            return false;
        }

        //Method to delete associated customer.
        public virtual bool DeleteAssociatedCustomer(string customerIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters customerIds:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { CustomerIds = customerIds });
            message = string.Empty;
            if (!string.IsNullOrEmpty(customerIds))
            {
                try
                {
                    return _priceClient.DeleteAssociatedCustomer(new ParameterModel { Ids = customerIds });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Get associated customer precedence value.
        public virtual PriceUserViewModel GetAssociatedCustomerPrecedence(int priceListUserId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PriceListUserId to get associated customer precedence:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListUserId = priceListUserId });
            PriceUserModel priceAccountModel = _priceClient.GetAssociatedCustomerPrecedence(priceListUserId, new ExpandCollection() { ZnodePriceListUserEnum.ZnodeUser.ToString() });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return IsNotNull(priceAccountModel) ? priceAccountModel.ToViewModel<PriceUserViewModel>() : new PriceUserViewModel();
        }

        //Update associated customer precedence value.
        public virtual PriceUserViewModel UpdateAssociatedCustomerPrecedence(PriceUserViewModel priceAccountViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("PriceUserViewModel with PriceListUserId and PriceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListUserId = priceAccountViewModel?.PriceListUserId, PriceListId = priceAccountViewModel?.PriceListId });
                priceAccountViewModel = _priceClient.UpdateAssociatedCustomerPrecedence(priceAccountViewModel?.ToModel<PriceUserModel>())?.ToViewModel<PriceUserViewModel>();
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return priceAccountViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                return (PriceUserViewModel)GetViewModelWithErrorMessage(new PriceUserViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return (PriceUserViewModel)GetViewModelWithErrorMessage(new PriceUserViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }
        #endregion

        #region Associate Profile
        //Method to get associated profile list.
        public virtual PriceProfileListViewModel GetAssociatedProfileList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts});
            PriceProfileListModel priceProfileList = _priceClient.GetAssociatedProfileList(null, filters, sorts, pageIndex, recordPerPage);
            PriceProfileListViewModel priceProfileListViewModel = new PriceProfileListViewModel { PriceProfiles = priceProfileList?.PriceProfileList?.ToViewModel<PriceProfileViewModel>().ToList() };
            SetListPagingData(priceProfileListViewModel, priceProfileList);

            //Set tool menu for associate profile to price list grid view.
            SetAssociateProfileListToolMenu(priceProfileListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceProfileList?.PriceProfileList?.Count > 0 ? priceProfileListViewModel
                : new PriceProfileListViewModel() { PriceProfiles = new List<PriceProfileViewModel>() };
        }

        //Method to get Un-Associated profile list.
        public virtual ProfileListViewModel GetUnAssociatedProfileList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });
            ProfileListModel profileList = _priceClient.GetUnAssociatedProfileList(null, filters, sorts, pageIndex, recordPerPage);
            ProfileListViewModel profileListViewModel = new ProfileListViewModel { List = profileList?.Profiles?.ToViewModel<ProfileViewModel>().ToList() };
            SetListPagingData(profileListViewModel, profileList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return profileList?.Profiles?.Count > 0 ? profileListViewModel
                : new ProfileListViewModel() { List = new List<ProfileViewModel>() };
        }

        //Method to associate profile.
        public virtual bool AssociateProfile(int priceListId, string profileIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters priceListId and profileIds:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListId = priceListId, ProfileIds = profileIds });
            message = Admin_Resources.ErrorAssociateProfile;
            try
            {
                return _priceClient.AssociateProfile(PriceViewModelMap.ToAssociateProfileListModel(priceListId, profileIds));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorAssociateStore;
            }
            return false;
        }

        //Method to remove associated profiles.
        public virtual bool RemoveAssociatedProfiles(string priceListProfileIds, int priceListId)
         => _priceClient.RemoveAssociatedProfiles(new RemoveAssociatedProfilesModel() { PriceListId = priceListId, PriceListProfileIds = priceListProfileIds.ToString() });

        //Get associated profile precedence value.
        public virtual PriceProfileViewModel GetAssociatedProfilePrecedence(int priceListProfileId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PriceListProfileId to get associated profile precedence:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListProfileId = priceListProfileId });
            PriceProfileModel priceProfileModel = _priceClient.GetAssociatedProfilePrecedence(priceListProfileId, null);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return IsNotNull(priceProfileModel) ? priceProfileModel.ToViewModel<PriceProfileViewModel>() : new PriceProfileViewModel();
        }

        //Update associated profile precedence value.
        public virtual PriceProfileViewModel UpdateAssociatedProfilePrecedence(PriceProfileViewModel priceProfileViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("PriceProfileViewModel with PriceListProfileId and PriceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListProfileId = priceProfileViewModel?.PriceListProfileId, FieldValueDictionaryListCount = priceProfileViewModel?.PriceListId });
                priceProfileViewModel = _priceClient.UpdateAssociatedProfilePrecedence(priceProfileViewModel?.ToModel<PriceProfileModel>())?.ToViewModel<PriceProfileViewModel>();
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return priceProfileViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                return (PriceProfileViewModel)GetViewModelWithErrorMessage(new PriceProfileViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return (PriceProfileViewModel)GetViewModelWithErrorMessage(new PriceProfileViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        #endregion

        #region Associate Account

        //Get Associated AccountList associated to price.
        public virtual PriceAccountListViewModel GetAssociatedAccountList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });
            PriceAccountListModel priceAccountListModel = _priceClient.GetAssociatedAccountList(null, filters, sorts, pageIndex, recordPerPage);
            PriceAccountListViewModel priceAccountListViewModel = new PriceAccountListViewModel { PriceAccountList = priceAccountListModel?.PriceAccountList?.ToViewModel<PriceAccountViewModel>().ToList() };

            SetListPagingData(priceAccountListViewModel, priceAccountListModel);

            //Set tool menu for associate account to price list grid view.
            SetAssociateAccountListToolMenu(priceAccountListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceAccountListModel?.PriceAccountList?.Count > 0 ? priceAccountListViewModel
                : new PriceAccountListViewModel() { PriceAccountList = new List<PriceAccountViewModel>() };
        }

        //Get UnAssociated Account List to price.
        public virtual PriceAccountListViewModel GetUnAssociatedAccountList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });
            PriceAccountListModel priceAccountListModel = _priceClient.GetUnAssociatedAccountList(null, filters, sorts, pageIndex, recordPerPage);
            PriceAccountListViewModel PriceAccountListViewModel = new PriceAccountListViewModel { PriceAccountList = priceAccountListModel?.PriceAccountList?.ToViewModel<PriceAccountViewModel>().ToList() };

            SetListPagingData(PriceAccountListViewModel, priceAccountListModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceAccountListModel?.PriceAccountList?.Count > 0 ? PriceAccountListViewModel
                : new PriceAccountListViewModel() { PriceAccountList = new List<PriceAccountViewModel>() };
        }

        //Associate account to price list.
        public virtual bool AssociateAccount(int priceListId, string customerIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters priceListId and customerIds:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListId = priceListId, CustomerIds = customerIds });
            message = Admin_Resources.ErrorAssociateAccount;
            try
            {
                return _priceClient.AssociateAccount(PriceViewModelMap.ToAssociateAccountListModel(priceListId, customerIds));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorAssociateAccount;
            }
            return false;
        }

        //Remove associated price lists to accounts.
        public virtual bool RemoveAssociatedAccounts(string priceListAccountIds, int priceListId)
       => _priceClient.RemoveAssociatedAccounts(new RemoveAssociatedAccountsModel() { PriceListId = priceListId, PriceListAccountIds = priceListAccountIds.ToString() });

        //Get associated account precedence value.
        public virtual PriceAccountViewModel GetAssociatedAccountPrecedence(int priceListUserId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PriceListUserId to get associated account precedence:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListUserId = priceListUserId });
            PriceAccountModel priceAccountModel = _priceClient.GetAssociatedAccountPrecedence(priceListUserId, new ExpandCollection() { ZnodePriceListAccountEnum.ZnodeAccount.ToString() });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return IsNotNull(priceAccountModel) ? priceAccountModel.ToViewModel<PriceAccountViewModel>() : new PriceAccountViewModel();
        }

        //Update associated account precedence value.
        public virtual PriceAccountViewModel UpdateAssociatedAccountPrecedence(PriceAccountViewModel priceAccountViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("PriceAccountViewModel with PriceListAccountId and PriceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListAccountId = priceAccountViewModel?.PriceListAccountId, PriceListId = priceAccountViewModel?.PriceListId });
                priceAccountViewModel = _priceClient.UpdateAssociatedAccountPrecedence(priceAccountViewModel?.ToModel<PriceAccountModel>())?.ToViewModel<PriceAccountViewModel>();
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return priceAccountViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                return (PriceAccountViewModel)GetViewModelWithErrorMessage(new PriceAccountViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return (PriceAccountViewModel)GetViewModelWithErrorMessage(new PriceAccountViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }
        #endregion

        #region Tier Price
        //Save tier price values.
        public virtual bool AddTierPrice(PriceSKUDataViewModel priceSKUDataViewModel, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return _priceClient.AddTierPrice(PriceViewModelMap.ToPriceTierListModel(priceSKUDataViewModel));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        message = Admin_Resources.DuplicateTierPriceErrorMessage;
                        return false;
                    default:
                        message = Admin_Resources.ErrorAddTierPrice;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorAddTierPrice;
                return false;
            }
        }

        //Method to get tier price list.
        public virtual PriceTierListViewModel GetTierPriceList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            //Sort tier price list in ascending order of quantity applied. 
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(ZnodePriceTierEnum.Quantity.ToString(), DynamicGridConstants.ASCKey);
            }

            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });
            PriceTierListModel tierPriceList = _priceClient.GetTierPriceList(null, filters, sorts, pageIndex, recordPerPage);
            PriceTierListViewModel tierPriceViewModelList = new PriceTierListViewModel { TierPriceList = tierPriceList?.TierPriceList?.ToViewModel<PriceTierViewModel>().ToList() };            
            SetListPagingData(tierPriceViewModelList, tierPriceList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return tierPriceList?.TierPriceList?.Count > 0 ? tierPriceViewModelList
                : new PriceTierListViewModel() { TierPriceList = new List<PriceTierViewModel>() };
        }

        public List<BaseDropDownList> GetCustomColumnList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            List<BaseDropDownList> list = new List<BaseDropDownList>();

            list.Add(new BaseDropDownList()
            {
                id = ZnodePriceTierEnum.Custom1.ToString(),
                name= Admin_Resources.LabelPriceCustom1
            });
            list.Add(new BaseDropDownList()
            {
                id = ZnodePriceTierEnum.Custom2.ToString(),
                name = Admin_Resources.LabelPriceCustom2
            });
            list.Add(new BaseDropDownList()
            {
                id = ZnodePriceTierEnum.Custom3.ToString(),
                name = Admin_Resources.LabelPriceCustom3
            });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return list;
        }

        //Method to get tier price by priceTierId.
        public virtual PriceTierViewModel GetTierPrice(int priceTierId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            PriceTierModel tierPriceModel = _priceClient.GetTierPrice(priceTierId);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return !Equals(tierPriceModel, null) ? tierPriceModel.ToViewModel<PriceTierViewModel>() : new PriceTierViewModel();
        }

        //Method to update tier price.
        public virtual PriceSKUDataViewModel UpdateTierPrice(PriceSKUDataViewModel priceSKUDataViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                priceSKUDataViewModel.PriceTier = _priceClient.UpdateTierPrice(priceSKUDataViewModel?.PriceTier?.ToModel<PriceTierModel>())?.ToViewModel<PriceTierViewModel>();
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return priceSKUDataViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                return (PriceSKUDataViewModel)GetViewModelWithErrorMessage(new PriceSKUDataViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return (PriceSKUDataViewModel)GetViewModelWithErrorMessage(new PriceSKUDataViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Method to delete tier price.
        public virtual bool DeleteTierPrice(int priceTierId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                return _priceClient.DeleteTierPrice(priceTierId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual void SetFiltersForSKUAndPriceList(FilterCollection filters, int priceListId, string sku)
        {
            if (IsNotNull(filters))
            {
                filters.Clear();
                if (priceListId > 0)
                {
                    filters.Add(new FilterTuple(ZnodePriceTierEnum.PriceListId.ToString(), FilterOperators.Equals, priceListId.ToString()));
                    filters.Add(new FilterTuple(ZnodePriceTierEnum.SKU.ToString(), FilterOperators.Is, sku.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodePriceTierEnum.SKU.ToString(), FilterOperators.Is, sku.ToString()));
            }
            ZnodeLogging.LogMessage("Filters for SKU and price list:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters });
        }
        #endregion

        #region Price Management
        //Method to get Un-Associated price list.
        public virtual PriceListViewModel GetUnAssociatedPriceList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("Input parameters expands, filters and sort collection:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Expands = expands, Filters = filters, Sorts = sorts });
                PriceListModel priceList = _priceClient.GetUnAssociatedPriceList(expands, filters, sorts, pageIndex, recordPerPage);
                PriceListViewModel priceListViewModel = new PriceListViewModel { PriceList = priceList?.PriceList?.ToViewModel<PriceViewModel>().ToList() };
                SetListPagingData(priceListViewModel, priceList);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return priceList?.PriceList?.Count > 0 ? priceListViewModel : new PriceListViewModel() { PriceList = new List<PriceViewModel>() };
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.IdLessThanOne:
                        return (PriceListViewModel)GetViewModelWithErrorMessage(new PriceListViewModel() { PriceList = new List<PriceViewModel>() }, Admin_Resources.PriceListAssociationError);
                    default:
                        return (PriceListViewModel)GetViewModelWithErrorMessage(new PriceListViewModel() { PriceList = new List<PriceViewModel>() }, string.Empty);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return (PriceListViewModel)GetViewModelWithErrorMessage(new PriceListViewModel() { PriceList = new List<PriceViewModel>() }, string.Empty);
            }
        }

        //Method to associate price 
        public virtual bool AssociatePriceListToStore(int storeId, string priceListIds)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Input parameters storeId and priceListIds:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { StoreId = storeId, PriceListIds = priceListIds });
                return storeId > 0 && !string.IsNullOrEmpty(priceListIds) ?
                    _priceClient.AssociateStore(PriceViewModelMap.ToAssociatePriceListModelToStore(storeId, priceListIds)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Method to associate price list to profile.
        public virtual bool AssociatePriceListToProfile(int profileId, string priceListIds, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("Input parameters profileId, priceListIds and portalId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { ProfileId = profileId, PriceListIds = priceListIds, PortalId = portalId });
                return portalId > 0 && !string.IsNullOrEmpty(priceListIds) ?
                    _priceClient.AssociateProfile(PriceViewModelMap.ToAssociatePriceListModelToProfile(profileId, priceListIds, portalId)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Method to remove associated price list to store.
        public virtual bool RemoveAssociatedPriceListToStore(string priceListPortalId)
        => _priceClient.RemoveAssociatedPriceListToStore(new ParameterModel() { Ids = priceListPortalId });

        //Method to remove associated price list to profile.
        public virtual bool RemoveAssociatedPriceListToProfile(string priceListProfileIds)
         => _priceClient.RemoveAssociatedPriceListToProfile(new ParameterModel() { Ids = priceListProfileIds });
        #endregion

        #region Import/Export 
        //Method to preview import data.
        public virtual List<ImportPriceViewModel> PreviewImportData(HttpPostedFileBase file)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                return IsNotNull(file) ? BindDataToImportModel(new PriceViewModel(), HelperMethods.GetImportDetails(file)).ToViewModel<ImportPriceViewModel, ImportPriceModel>().ToList() : new List<ImportPriceViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return null;
            }
        }

        //Method to export price data.
        public virtual List<ExportPriceViewModel> ExportPriceData(string priceListIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            try
            {
                ZnodeLogging.LogMessage("Input parameters priceListIds:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListIds = priceListIds });
                return _priceClient.GetExportPriceData(priceListIds).ToViewModel<ExportPriceViewModel, ExportPriceModel>().ToList();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                return null;
            }
        }
        #endregion

        #region Associated price list precedence value for Store/Profile
        //Get associated price list precedence value for Store/Profile.
        public virtual PricePortalViewModel GetAssociatedPriceListPrecedence(int priceListProfileId, int priceListId, int portalId, string listName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters priceListProfileId, priceListId, portalId and listName:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListProfileId = priceListProfileId, PriceListId = priceListId, PortalId = portalId, ListName = listName });
            PricePortalModel pricePortalModel = _priceClient.GetAssociatedPriceListPrecedence(new PricePortalModel { PriceListProfileId = priceListProfileId, PriceListId = priceListId, PortalId = portalId });
            if (IsNotNull(pricePortalModel))
            {
                pricePortalModel.Name = listName;
                return pricePortalModel.ToViewModel<PricePortalViewModel>();
            }
            return new PricePortalViewModel() { Name = listName, PriceListProfileId = priceListProfileId, PriceListId = priceListId };
        }

        //Update associated price list precedence value for Store/Profile.
        public virtual bool UpdateAssociatedPriceListPrecedence(string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            PricePortalViewModel pricePortalViewModel = new PricePortalViewModel();

            if (!string.IsNullOrEmpty(data))
                pricePortalViewModel = JsonConvert.DeserializeObject<PricePortalViewModel[]>(data)[0];
            else
                return false;
            try
            {
                pricePortalViewModel = _priceClient.UpdateAssociatedPriceListPrecedence(pricePortalViewModel?.ToModel<PricePortalModel>())?.ToViewModel<PricePortalViewModel>();
                return (IsNotNull(pricePortalViewModel) && !pricePortalViewModel.HasError) ? true : false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Warning);
                pricePortalViewModel.ErrorMessage = Admin_Resources.UpdateErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                pricePortalViewModel.ErrorMessage = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }
        #endregion

        #region Associate Product

        #endregion

        //Set Import Inventory Details.
        public virtual void SetImportPricingDetails(PriceViewModel priceSKUDataViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PriceViewModel with PriceListId and ImportHeadId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListId = priceSKUDataViewModel?.PriceListId, ImportHeadId = priceSKUDataViewModel?.ImportHeadId });
            //Set import head type id and name.
            SetImportHeadData(priceSKUDataViewModel);
            //Get Import Template List.
            priceSKUDataViewModel.TemplateTypeList = _importAgent.GetImportTemplateList(priceSKUDataViewModel.ImportHeadId, 0);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
        }
        #endregion

        #region Private Methods
        private FilterCollection GetActiveFilter(int currencyId = 0)
        {
            FilterCollection filters = new FilterCollection();
            if(currencyId>0)
                filters.Add(new FilterTuple(ZnodeCurrencyEnum.CurrencyId.ToString(), FilterOperators.Equals, currencyId.ToString()));
            filters.Add(new FilterTuple(ZnodeCurrencyEnum.IsActive.ToString(), FilterOperators.Equals, true.ToString()));
            ZnodeLogging.LogMessage("Active filter:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { ActiveFilter = filters });
            return filters;
        }


        //Get Currency Name sorted ascending
        private SortCollection GetSortByCurrencyName()
        {
            SortCollection sorts = new SortCollection();
                sorts = HelperMethods.SortAsc(ZnodeCurrencyEnum.CurrencyName.ToString(), sorts);
            return sorts;
        }

        //Method to bind data to import model.
        private List<ImportPriceModel> BindDataToImportModel(PriceViewModel priceViewModel, DataTable importPriceList)
        {
            if (HelperMethods.IsValidColumns(importPriceList, typeof(ImportPriceModel).GetProperties().ToList()))
            {
                List<ImportPriceModel> priceList = new List<ImportPriceModel>();
                if (importPriceList?.Rows?.Count > 0)
                {
                    int rowNumber = 1;
                    foreach (DataRow row in importPriceList.Rows)
                    {
                        priceList.Add(new ImportPriceModel
                        {
                            PriceListCode = priceViewModel.ListCode,
                            PriceListName = priceViewModel.ListName,
                            CurrencyId = priceViewModel.CurrencyId.ToString(),
                            ActivationDate = priceViewModel.ActivationDate.ToString(),
                            ExpirationDate = priceViewModel.ExpirationDate.ToString(),
                            RowNumber = rowNumber,
                            SequenceNumber = rowNumber,
                            SKU = row["SKU"].ToString(),
                            SKUActivationDate = !string.IsNullOrEmpty(row["SKUActivationDate"].ToString()) ? Convert.ToDateTime(row["SKUActivationDate"].ToString()).ToDateTimeFormat() : row["SKUActivationDate"].ToString(),
                            SKUExpirationDate = !string.IsNullOrEmpty(row["SKUExpirationDate"].ToString()) ? Convert.ToDateTime(row["SKUExpirationDate"].ToString()).ToDateTimeFormat() : row["SKUExpirationDate"].ToString(),
                            RetailPrice = row["RetailPrice"].ToString(),
                            SalesPrice = row["SalesPrice"].ToString(),
                            TierStartQuantity = row["TierStartQuantity"].ToString(),
                            TierPrice = row["TierPrice"].ToString(),
                            UOM = row["UOM"].ToString(),
                            UnitSize = row["UnitSize"].ToString(),
                        });
                        rowNumber++;
                    }
                }
                ZnodeLogging.LogMessage("PriceList count:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListCount = priceList?.Count });
                return priceList;
            }
            return null;
        }

        //Set tool menu for price list grid view.
        private void SetPriceListToolMenu(PriceListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PriceDeletePopup')", ControllerName = "Price", ActionName = "Delete" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonExport, JSFunctionName = "Price.prototype.ExportPriceData();", ControllerName = "Price", ActionName = "ExportPriceData" });
            }
        }

        //Set tool menu for sku price list grid view.
        private void SetPriceSKUListToolMenu(PriceSKUListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PriceDeletePopup')", ControllerName = "Price", ActionName = "DeleteSKUPrice" });
            }
        }

        //Set tool menu for associate store to price list grid view.
        private void SetAssociateStoreListToolMenu(PricePortalListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PricePortalDeletePopup',this)", ControllerName = "Price", ActionName = "RemoveAssociatedStores" });
            }
        }

        //Set tool menu for associate customer to price list grid view.
        private void SetAssociateCustomerListToolMenu(PriceUserListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PriceDeletePopup',this)", ControllerName = "Price", ActionName = "DeleteAssociatedCustomer" });
            }
        }

        //Set tool menu for associate account to price list grid view.
        private void SetAssociateAccountListToolMenu(PriceAccountListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PriceDeletePopup',this)", ControllerName = "Price", ActionName = "DeleteAssociatedAccount" });
            }
        }

        //Set tool menu for associate profile to price list grid view.
        private void SetAssociateProfileListToolMenu(PriceProfileListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PriceProfileDeletePopup',this)", ControllerName = "Price", ActionName = "RemoveAssociatedProfiles" });
            }
        }

        //Bind SKU price data.
        private void GetPriceTierList(PriceSKUDataViewModel priceSKUDataViewModel)
        {
            XDocument doc = XDocument.Parse(priceSKUDataViewModel.PriceTierListData);

            priceSKUDataViewModel.PriceTierList = doc.Descendants("row").Select(d =>
                                                  new PriceTierViewModel
                                                  {
                                                      Quantity = Convert.ToDecimal(d.Element(ZnodePriceTierEnum.Quantity.ToString()).Value),
                                                      Price = Convert.ToDecimal(d.Element(ZnodePriceTierEnum.Price.ToString()).Value),
                                                      Custom1=d.Element("Custom1").Value,
                                                      Custom2=d.Element("Custom2").Value,
                                                      Custom3=d.Element("Custom3").Value,
                                                      PriceTierId = Convert.ToInt32(d.Element(ZnodePriceTierEnum.PriceTierId.ToString()).Value),
                                                      SKU = priceSKUDataViewModel.PriceSKU.SKU
                                                  }).ToList();
            ZnodeLogging.LogMessage("PriceTierList count:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceTierListCount = priceSKUDataViewModel?.PriceTierList?.Count });
        }

        //Set import head type id and name.
        private void SetImportHeadData(PriceViewModel priceSKUDataViewModel)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeImportHeadEnum.IsUsedInImport.ToString(), FilterOperators.Equals, "true"));

            ZnodeLogging.LogMessage("Filters to get import type list:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Filters = filters });
            ImportTypeListModel importList = _importClient.GetImportTypeList(null, filters, null, null, null)?.ImportTypeList;

            priceSKUDataViewModel.ImportHeadId = importList?.ImportTypeList?.Count > 0 ? importList.ImportTypeList.FirstOrDefault(w => w.Name == ImportHeadEnum.Pricing.ToString()).ImportHeadId : 0;
            priceSKUDataViewModel.ImportType = ImportHeadEnum.Pricing.ToString();
            ZnodeLogging.LogMessage("ImportHeadId and ImportType:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { ImportHeadId = priceSKUDataViewModel?.ImportHeadId, ImportType = priceSKUDataViewModel?.ImportType });
        }

        //Set Import Model Data.
        private static ImportViewModel SetImportModelData(PriceViewModel priceViewModel, HttpPostedFileBase fileType)
        {
            ImportViewModel importModel = new ImportViewModel();
            if (IsNotNull(fileType))
            {
                importModel.FilePath = fileType;
                importModel.FileName = priceViewModel.ChangedFileName;
                importModel.TemplateId = priceViewModel.TemplateId;
                importModel.ImportHeadId = priceViewModel.ImportHeadId;
                importModel.ImportType = priceViewModel.ImportType;
                importModel.IsPartialPage = true;
            }

            return importModel;
        }
        #endregion

    }
}