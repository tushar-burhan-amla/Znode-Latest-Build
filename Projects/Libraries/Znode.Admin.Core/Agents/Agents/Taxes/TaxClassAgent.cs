using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
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

namespace Znode.Engine.Admin.Agents
{
    public class TaxClassAgent : BaseAgent, ITaxClassAgent
    {
        #region Private Variables

        private readonly ITaxClassClient _taxClassClient;
        private readonly ITaxRuleTypeClient _taxRuleTypeClient;
        private readonly ICountryAgent _countryAgent;
        private readonly IStateAgent _stateAgent;
        private readonly ICityAgent _cityAgent;
        private readonly IProductsClient _productClient;
        private readonly ICityClient _cityClient;
        private readonly IStateClient _stateClient;

        #endregion Private Variables

        #region Constructor

        public TaxClassAgent(ITaxClassClient taxClassClient, ITaxRuleTypeClient taxRuleTypeClient, IProductsClient productsClient, ICityClient cityClient, IStateClient stateClient)
        {
            _taxClassClient = GetClient<ITaxClassClient>(taxClassClient);
            _taxRuleTypeClient = GetClient<ITaxRuleTypeClient>(taxRuleTypeClient);
            _countryAgent = new CountryAgent(GetClient<CountryClient>());
            _stateAgent = new StateAgent(GetClient<StateClient>());
            _cityAgent = new CityAgent(GetClient<CityClient>());
            _productClient = GetClient<IProductsClient>(productsClient);
            _cityClient = GetClient<ICityClient>(cityClient);
            _stateClient = GetClient<IStateClient>(stateClient);
        }

        #endregion Constructor

        #region Public Methods

        //Get the list of all tax classes.
        public virtual TaxClassListViewModel GetTaxClassList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.DisplayOrder, DynamicGridConstants.ASCKey);
            }

            TaxClassListModel taxClassListModel = _taxClassClient.GetTaxClassList(filters, sorts, pageIndex, pageSize);
            TaxClassListViewModel listViewModel = new TaxClassListViewModel { TaxClassList = taxClassListModel?.TaxClassList?.ToViewModel<TaxClassViewModel>().ToList() };

            SetListPagingData(listViewModel, taxClassListModel);

            //Set tool menu for tax class list grid view.
            SetTaxClassListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return taxClassListModel?.TaxClassList?.Count > 0 ? listViewModel : new TaxClassListViewModel() { TaxClassList = new List<TaxClassViewModel>() };
        }

        //Get a tax class on the basis of id.
        public virtual TaxClassViewModel GetTaxClass(int taxClassId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            TaxClassModel taxClassModel = _taxClassClient.GetTaxClass(taxClassId);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return IsNotNull(taxClassModel) ? taxClassModel.ToViewModel<TaxClassViewModel>() : null;
        }

        //Create a new tax class.
        public virtual TaxClassViewModel CreateTaxClass(TaxClassViewModel taxClassViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                //Get the imported file and convert SKUs in it into comma seperated string.
                if (IsNotNull(taxClassViewModel.ImportFile))
                    taxClassViewModel.ImportedSKUs = GetSKUsFromImportedFile(taxClassViewModel.ImportFile);

                ZnodeLogging.LogMessage("ImportedSKUs:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { ImportedSKUs = taxClassViewModel?.ImportedSKUs });
                return _taxClassClient.CreateTaxClass(taxClassViewModel.ToModel<TaxClassModel>()).ToViewModel<TaxClassViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (TaxClassViewModel)GetViewModelWithErrorMessage(new TaxClassViewModel(), Admin_Resources.TaxClassAlreadyExists);

                    default:
                        return (TaxClassViewModel)GetViewModelWithErrorMessage(new TaxClassViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (TaxClassViewModel)GetViewModelWithErrorMessage(new TaxClassViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Update a tax class details.
        public virtual TaxClassViewModel UpdateTaxClass(TaxClassViewModel taxClassViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                //Get the imported file and convert SKUs in it into comma seperated string.
                if (IsNotNull(taxClassViewModel.ImportFile))
                    taxClassViewModel.ImportedSKUs = GetSKUsFromImportedFile(taxClassViewModel.ImportFile);

                ZnodeLogging.LogMessage("ImportedSKUs:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { ImportedSKUs = taxClassViewModel?.ImportedSKUs });
                TaxClassModel taxClassModel = _taxClassClient.UpdateTaxClass(taxClassViewModel.ToModel<TaxClassModel>());
                return IsNotNull(taxClassModel) ? taxClassModel.ToViewModel<TaxClassViewModel>() : (TaxClassViewModel)GetViewModelWithErrorMessage(new TaxClassViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (TaxClassViewModel)GetViewModelWithErrorMessage(new TaxClassViewModel(), Admin_Resources.TaxClassAlreadyExists);

                    case ErrorCodes.AssociationUpdateError:
                        return (TaxClassViewModel)GetViewModelWithErrorMessage(new TaxClassViewModel(), Admin_Resources.UnableToUpdateTaxClassError);

                    default:
                        return (TaxClassViewModel)GetViewModelWithErrorMessage(new TaxClassViewModel(), Admin_Resources.UpdateError);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (TaxClassViewModel)GetViewModelWithErrorMessage(new TaxClassViewModel(), Admin_Resources.UpdateError);
            }
        }

        //Delete a tax class on the basis of tax class id.
        public virtual bool DeleteTaxClass(string taxClassIds, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(taxClassIds))
            {
                try
                {
                    return _taxClassClient.DeleteTaxClass(new ParameterModel { Ids = taxClassIds });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = Admin_Resources.ErrorDeleteTaxClassAsAssociatedWithOrder;
                            return false;

                        default:
                            errorMessage = Admin_Resources.ErrorFailedToDelete;
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Get the list of all active tax rule types in key value pair
        public virtual List<SelectListItem> GetTaxRuleTypes(int taxClassId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeTaxRuleTypeEnum.IsActive.ToString(), FilterOperators.Equals, "true"));

            filters.Add(new FilterTuple(ZnodeTaxClassEnum.TaxClassId.ToString(), FilterOperators.Equals, taxClassId.ToString()));

            ZnodeLogging.LogMessage("Filters to get taxRuleTypeList:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { Filters = filters });
            TaxRuleTypeListModel taxRuleTypeList = _taxRuleTypeClient.GetTaxRuleTypeList(filters, null, null, null);
            List<SelectListItem> selectedTaxRuleTypeList = new List<SelectListItem>();
            if (taxRuleTypeList?.TaxRuleTypes?.Count > 0)
                taxRuleTypeList.TaxRuleTypes.ToList().ForEach(item => { selectedTaxRuleTypeList.Add(new SelectListItem() { Text = item.Name, Value = item.TaxRuleTypeId.ToString() }); });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return selectedTaxRuleTypeList;
        }

        //Set filter for taxClassId
        public virtual void SetFilters(FilterCollection filters, int taxClassId)
        {
            if (!Equals(filters, null))
            {
                //Checking For TaxClassId already Exists in Filters Or Not
                if (filters.Exists(x => x.Item1 == FilterKeys.TaxClassId))
                {
                    //If TaxClassId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == FilterKeys.TaxClassId);

                    //Add New TaxClassId Into filters
                    filters.Add(new FilterTuple(FilterKeys.TaxClassId, FilterOperators.Equals, taxClassId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.TaxClassId, FilterOperators.Equals, taxClassId.ToString()));
            }
            ZnodeLogging.LogMessage("Filters: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { Filters = filters });
        }

        //Get taxrule details.
        public virtual TaxRuleViewModel GetTaxRuleDetails(int taxClassId)
        {
            return new TaxRuleViewModel
            {
                TaxClassId = taxClassId,
                TaxRuleTypeList = GetTaxRuleTypes(taxClassId),
                CountryList = BindCountryList(),
                StateList = BindStateList(null),
                CountyList = BindCityList(null),
            };
        }

        #region Tax Class SKU

        //Get taxClassSKU list viewmodel
        public virtual TaxClassSKUListViewModel GetTaxClassSKUListViewModel(FilterCollectionDataModel model, int taxClassId, string name)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeTaxClassSKU.ToString(), model);
            SetFilters(model.Filters, taxClassId);
            ZnodeLogging.LogMessage("Filters, SortCollection, taxClassId and name:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { Filters = model.Filters, Sorts = model.SortCollection, TaxClassId = taxClassId, name = name });
            TaxClassSKUListViewModel taxClassSKUList = GetTaxClassSKUList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            taxClassSKUList.TaxClassId = taxClassId;
            taxClassSKUList.Name = name;

            taxClassSKUList.GridModel = FilterHelpers.GetDynamicGridModel(model, taxClassSKUList?.TaxClassSKUList, GridListType.ZnodeTaxClassSKU.ToString(), string.Empty, null, true, true, taxClassSKUList?.GridModel?.FilterColumn?.ToolMenuList);
            taxClassSKUList.GridModel.TotalRecordCount = taxClassSKUList.TotalResults;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return taxClassSKUList;
        }

        //Get tax class SKU list.
        public virtual TaxClassSKUListViewModel GetTaxClassSKUList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            TaxClassSKUListModel taxClassSKUList = _taxClassClient.GetTaxClassSKUList(filters, sorts, pageIndex, recordPerPage);
            TaxClassSKUListViewModel taxClassSKUListViewModel = new TaxClassSKUListViewModel { TaxClassSKUList = taxClassSKUList?.TaxClassSKUList?.ToViewModel<TaxClassSKUViewModel>().ToList() };
            SetListPagingData(taxClassSKUListViewModel, taxClassSKUList);

            //Set tool menu for tax class sku list grid view.
            SetTaxClassSKUListToolMenu(taxClassSKUListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return taxClassSKUList?.TaxClassSKUList?.Count > 0 ? taxClassSKUListViewModel
                : new TaxClassSKUListViewModel() { TaxClassSKUList = new List<TaxClassSKUViewModel>() };
        }

        //Add Tax class SKU.
        public virtual TaxClassSKUViewModel AddTaxClassSKU(TaxClassSKUViewModel taxClassSKUViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                return _taxClassClient.AddTaxClassSKU(taxClassSKUViewModel?.ToModel<TaxClassSKUModel>())?.ToViewModel<TaxClassSKUViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (TaxClassSKUViewModel)GetViewModelWithErrorMessage(new TaxClassSKUViewModel(), Admin_Resources.ErrorSKUAlreadyAssociated);

                    default:
                        return (TaxClassSKUViewModel)GetViewModelWithErrorMessage(new TaxClassSKUViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (TaxClassSKUViewModel)GetViewModelWithErrorMessage(new TaxClassSKUViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Delete Tax class SKU.
        public virtual bool DeleteTaxClassSKU(string taxClassSKUId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            message = string.Empty;
            if (!string.IsNullOrEmpty(taxClassSKUId))
            {
                try
                {
                    return _taxClassClient.DeleteTaxClassSKU(taxClassSKUId);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        #endregion Tax Class SKU

        #region Tax Rule

        //Get Tax rule list.
        public virtual TaxRuleListViewModel GetTaxRuleList(ExpandCollection expands, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            }

            if (IsNull(expands))
            {
                expands = new ExpandCollection();
                expands.Add(ZnodeTaxRuleEnum.ZnodeTaxRuleType.ToString());
            }

            ZnodeLogging.LogMessage("Input parameters expands, filters, sort collection:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { Expands = expands, Filters = filters, Sorts = sorts });
            TaxRuleListModel taxRuleList = _taxClassClient.GetTaxRuleList(expands, filters, sorts, pageIndex, recordPerPage);
            TaxRuleListViewModel taxRuleListViewModel = new TaxRuleListViewModel { TaxRuleList = taxRuleList?.TaxRuleList?.ToViewModel<TaxRuleViewModel>().ToList() ?? new List<TaxRuleViewModel>() };
            foreach (var item in taxRuleListViewModel.TaxRuleList)
            {
                if (string.IsNullOrEmpty(item.DestinationCountryCode))
                    item.DestinationCountryCode = Admin_Resources.LabelAll;

                if (string.IsNullOrEmpty(item.DestinationStateCode))
                    item.DestinationStateCode = Admin_Resources.LabelAll;

                if (string.IsNullOrEmpty(item.CountyFIPS))
                    item.CountyFIPS = Admin_Resources.LabelAll;
            }
            SetListPagingData(taxRuleListViewModel, taxRuleList);

            //Set tool menu for tax rule list grid view.
            SetTaxRuleListToolMenu(taxRuleListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return taxRuleList?.TaxRuleList?.Count > 0 ? taxRuleListViewModel
                : new TaxRuleListViewModel() { TaxRuleList = new List<TaxRuleViewModel>() };
        }

        //Get tax rule details.
        public virtual TaxRuleViewModel GetTaxRule(int taxRuleId, int taxClassId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            TaxRuleModel taxRuleModel = _taxClassClient.GetTaxRule(taxRuleId);
            TaxRuleViewModel taxRuleViewModel = IsNotNull(taxRuleModel) ? taxRuleModel.ToViewModel<TaxRuleViewModel>() : new TaxRuleViewModel();
            taxRuleViewModel.TaxRuleTypeList = GetTaxRuleTypes(taxClassId);
            taxRuleViewModel.CountryList = BindCountryList();
            taxRuleViewModel.StateList = BindStateList(taxRuleViewModel.DestinationCountryCode);
            taxRuleViewModel.CountyList = BindCityList(taxRuleViewModel.DestinationStateCode);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return taxRuleViewModel;
        }

        //Add tax rule details.
        public virtual TaxRuleViewModel AddTaxRule(TaxRuleViewModel taxRuleViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                return _taxClassClient.AddTaxRule(taxRuleViewModel?.ToModel<TaxRuleModel>())?.ToViewModel<TaxRuleViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (TaxRuleViewModel)GetViewModelWithErrorMessage(new TaxRuleViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Update tax rule details
        public virtual TaxRuleViewModel UpdateTaxRule(TaxRuleViewModel taxRuleViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                return _taxClassClient.UpdateTaxRule(taxRuleViewModel?.ToModel<TaxRuleModel>())?.ToViewModel<TaxRuleViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (TaxRuleViewModel)GetViewModelWithErrorMessage(new TaxRuleViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete tax rule.
        public virtual bool DeleteTaxRule(string taxRuleId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            message = string.Empty;
            if (!string.IsNullOrEmpty(taxRuleId))
            {
                try
                {
                    return _taxClassClient.DeleteTaxRule(taxRuleId);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Get state list by country code.
        public virtual List<SelectListItem> BindStateList(string countryCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<SelectListItem> selectedStateList = new List<SelectListItem>();

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.CountryCode, FilterOperators.Is, countryCode));

            SortCollection sorts = new SortCollection();
            sorts.Add(ZnodeStateEnum.StateName.ToString(), DynamicGridConstants.ASCKey);

            ZnodeLogging.LogMessage("Filters and sorts to get state list:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new {Filters = filters, Sorts = sorts });
            StateListModel stateList = _stateClient.GetStateList(filters, sorts);

            if (stateList?.States?.Count > 0)
                stateList.States.ToList().ForEach(item => { selectedStateList.Add(new SelectListItem() { Text = item.StateName, Value = item.StateCode }); });

            selectedStateList.Insert(0, new SelectListItem() { Value = string.Empty, Text = Admin_Resources.LabelAllStates });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return selectedStateList;
        }

        //Get city(county) list by state code.
        public virtual List<SelectListItem> BindCityList(string stateCode = "")
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<SelectListItem> selectedCityList = new List<SelectListItem>();

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.StateCode, FilterOperators.Is, stateCode));

            ZnodeLogging.LogMessage("Filters to get city list:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { Filters = filters });
            CityListModel cityListModel = _cityClient.GetCityList(filters, null);

            if (cityListModel?.Cities?.Count > 0)
                cityListModel.Cities.ToList().GroupBy(x => x.CountyFIPS).Select(y => y.First()).ToList().ForEach(item => { selectedCityList.Add(new SelectListItem() { Text = item.CountyFIPS, Value = item.CountyFIPS }); });
            selectedCityList.Insert(0, new SelectListItem() { Value = string.Empty, Text = Admin_Resources.LabelAllCounties });

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return selectedCityList;
        }

        //Get country list.
        public virtual List<SelectListItem> BindCountryList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<SelectListItem> countryList = _countryAgent.GetActiveCountryList();
            countryList.Insert(0, new SelectListItem() { Value = string.Empty, Text = Admin_Resources.LabelAllCountries });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return countryList;
        }

        //Get the list of all unassociated product.
        public virtual ProductDetailsListViewModel GetUnassociatedProductList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Set display order sort if not present.
            SetDisplayOrderSortIfNotPresent(ref sorts);
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.DisplayOrder, DynamicGridConstants.ASCKey);
            }
            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts});
            ProductDetailsListModel ProductList = _taxClassClient.GetUnassociatedProductList(null, filters, sorts, pageIndex, pageSize);// _taxClassClient.GetUnassociatedProductList(null, filters, sorts, pageIndex, pageSize);

            ProductDetailsListViewModel products = new ProductDetailsListViewModel()
            {
                ProductDetailList = ProductList?.ProductDetailList?.ToViewModel<ProductDetailsViewModel>().ToList(),
                Locale = PIMAttributeFamilyViewModelMap.ToLocaleListItem(ProductList.Locale),
                AttrubuteColumnName = ProductList?.AttributeColumnName,
                XmlDataList = ProductList?.XmlDataList ?? new List<dynamic>()
            };
            products.AttrubuteColumnName?.Remove(AdminConstants.PublishStatus);
            products.AttrubuteColumnName?.Remove(AdminConstants.ProductImage);
            products.AttrubuteColumnName?.Remove(AdminConstants.Assortment);
            SetListPagingData(products, ProductList);
            ZnodeLogging.LogMessage("ProductDetailList, Locale and XmlDataList count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { ProductDetailListCount = products?.ProductDetailList?.Count, LocaleListCount = products?.Locale?.Count, XmlDataListCount = products?.XmlDataList?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return products;
        }

        //Get LocaleId
        public virtual int GetLocaleValue()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            int localeId = 0;
            if (CookieHelper.IsCookieExists("_productCulture"))
            {
                string cookieValue = CookieHelper.GetCookieValue<string>("_productCulture");
                localeId = string.IsNullOrEmpty(cookieValue) ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : Convert.ToInt32(cookieValue);
            }
            else
            {
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                CookieHelper.SetCookie("_productCulture", Convert.ToString(localeId));
            }
            ZnodeLogging.LogMessage("localeId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { LocaleId = localeId });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return localeId;
        }

        #endregion Tax Rule

        #endregion Public Methods

        #region Private Methods

        //Set tool menu for tax class list grid view.
        private void SetTaxClassListToolMenu(TaxClassListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('TaxClassDeletePopup')", ControllerName = "TaxClass", ActionName = "Delete" });
            }
        }

        //Set tool menu for tax class sku list grid view.
        private void SetTaxClassSKUListToolMenu(TaxClassSKUListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('TaxClassSKUDeletePopup',this)", ControllerName = "TaxClass", ActionName = "DeleteTaxClassSKU" });
            }
        }

        //Set tool menu for tax rule list grid view.
        private void SetTaxRuleListToolMenu(TaxRuleListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('TaxRuleDeletePopup')", ControllerName = "TaxClass", ActionName = "DeleteTaxRule" });
            }
        }

        //Set tool menu for associate store list grid view.
        private void SetAssociateStoreListToolMenu(TaxClassPortalListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('TaxClassPortalDeletePopup')", ControllerName = "TaxClass", ActionName = "UnassociateStore" });
            }
        }

        //get SKUs from Imported file.
        private string GetSKUsFromImportedFile(HttpPostedFileBase importedSKUFile)
        {
            DataTable skuTable = HelperMethods.GetImportDetails(importedSKUFile);
            return string.Join(",", skuTable.AsEnumerable().Select(row => row.Field<string>("SKU")));
        }

        #endregion Private Methods
    }
}