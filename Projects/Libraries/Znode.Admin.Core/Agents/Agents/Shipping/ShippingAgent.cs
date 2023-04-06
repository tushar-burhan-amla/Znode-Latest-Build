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
    public class ShippingAgent : BaseAgent, IShippingAgent
    {
        #region Private Variables

        private readonly IShippingClient _shippingsClient;
        private readonly IShippingTypeClient _shippingTypeClient;
        private readonly ICountryAgent _countryAgent;
        private readonly IProfileAgent _profileAgent;
        private readonly IStateClient _stateClient;
        private readonly ICityClient _cityClient;
        private readonly IProviderEngineAgent _providerEngineAgent;
        private readonly IUserAgent _userAgent;
        private readonly IProductsClient _productClient;
        private const string zeroIndex = "0";
        private const int zeroValue = 0;
        private readonly IBrandClient _brandClient;
        private readonly ICurrencyClient _currencyClient;

        #endregion Private Variables

        #region Constructor

        public ShippingAgent(IShippingClient shippingClient, IShippingTypeClient shippingTypeClient, IStateClient stateClient,
            ICityClient cityClient, IProductsClient productClient, IBrandClient brandClient,
            IUserClient userClient, IPortalClient portalClient, IAccountClient accountClient, IRoleClient roleClient, IDomainClient domainClient, ICurrencyClient currencyClient, ICountryClient countryClient)
        {
            _shippingsClient = GetClient<IShippingClient>(shippingClient);
            _shippingTypeClient = GetClient<IShippingTypeClient>(shippingTypeClient);
            _stateClient = GetClient<IStateClient>(stateClient);
            _countryAgent = new CountryAgent(GetClient<CountryClient>());
            _profileAgent = new ProfileAgent(GetClient<ProfileClient>(), GetClient<CatalogClient>(), GetClient<PaymentClient>(), _shippingsClient);
            _cityClient = GetClient<ICityClient>(cityClient);
            _providerEngineAgent = new ProviderEngineAgent(GetClient<TaxRuleTypeClient>(), _shippingTypeClient, GetClient<PromotionTypeClient>());
            _userAgent = new UserAgent(userClient, portalClient, accountClient, roleClient, domainClient, stateClient, GetClient<GlobalAttributeEntityClient>(), GetClient<ShoppingCartClient>());
            _productClient = GetClient<IProductsClient>(productClient);
            _brandClient = GetClient<IBrandClient>(brandClient);
            _currencyClient = GetClient<ICurrencyClient>(currencyClient);
        }

        #endregion Constructor

        #region Public Methods

        #region Shipping

        //Get shipping by shipping Id.
        public virtual ShippingViewModel GetShippingById(int shippingId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            ShippingModel shippingModel = _shippingsClient.GetShipping(shippingId);
            ShippingViewModel shippingViewModel = new ShippingViewModel();
            if (IsNotNull(shippingModel))
            {
                shippingModel.ProfileId = Equals(shippingModel.ProfileId, null) ? zeroValue : shippingModel.ProfileId;
                shippingViewModel = SetHandlingChargesBasedOn(shippingModel?.ToViewModel<ShippingViewModel>());
                shippingViewModel.ClassName = _providerEngineAgent.GetShippingType(shippingViewModel.ShippingTypeId)?.ClassName;
                shippingViewModel.ShippingCode = shippingModel.ShippingCode;
                BindDropdownValues(shippingViewModel, true);
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            return shippingViewModel;
        }

        //Get list of shippings.
        public virtual ShippingListViewModel GetShippingList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, filters and sorts: ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { expands = expands, filters= filters, sorts = sorts });
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.DisplayOrder, DynamicGridConstants.ASCKey);
            }
            if (IsNull(expands))
            {
                expands = new ExpandCollection();
                //expands.Add(ZnodeShippingEnum.ZnodeProfile.ToString());
                expands.Add(ZnodeShippingEnum.ZnodeShippingType.ToString());
            }
            ShippingListModel shippingListModel = _shippingsClient.GetShippingList(expands, filters, sorts, pageIndex, pageSize);
            ShippingListViewModel listViewModel = new ShippingListViewModel { ShippingList = shippingListModel?.ShippingList?.ToViewModel<ShippingViewModel>().ToList() };
            listViewModel.ShippingList?.ForEach(item =>
            {
                if (IsNotNull(item.DestinationCountryCode))
                    item.DestinationCountryCode = listViewModel?.ShippingList?.Where(x => x.ShippingId == item.ShippingId).Select(x => x.DestinationCountryCode).FirstOrDefault();
                else
                    item.DestinationCountryCode = Admin_Resources.LabelAll;
            });

            SetListPagingData(listViewModel, shippingListModel);

            //Set tool menu for Shipping list grid view.
            SetShippingListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return shippingListModel?.ShippingList?.Count > 0 ? listViewModel : new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };
        }

        //Create shipping.
        public virtual ShippingViewModel CreateShipping(ShippingViewModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                FilterCollection shippingTypeFilters = new FilterCollection();
                shippingTypeFilters.Add(new FilterTuple(FilterKeys.ClassName, FilterOperators.Contains, model.ClassName.ToString()));

                //Get the imported file and convert SKUs in it into comma seperated string.
                if (IsNotNull(model.ImportFile))
                    model.ImportedSkus = GetSKUsFromImportedFile(model.ImportFile);

                ShippingTypeListModel shippingTypeListModel = _shippingTypeClient.GetShippingTypeList(shippingTypeFilters, null, null, null);
                ZnodeLogging.LogMessage("ShippingTypeList count ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { ShippingTypeList = shippingTypeListModel?.ShippingTypeList?.Count });

                if (shippingTypeListModel?.ShippingTypeList?.Count > 0)
                    model.ShippingTypeId = shippingTypeListModel.ShippingTypeList.FirstOrDefault().ShippingTypeId;

                if (CheckShippingMethod(model.ClassName))
                    model.ShippingCode = _shippingsClient.GetShippingServiceCode(model.ShippingServiceCodeId)?.Code;
                SetDropDownSelectAllValue(model);
                return _shippingsClient.CreateShipping(model.ToModel<ShippingModel>()).ToViewModel<ShippingViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (ShippingViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.AlreadyExistShipping);

                    default:
                        return (ShippingViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Error);
                return (ShippingViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorFailedToCreate);
            }
        }

        public virtual bool CheckShippingMethod(string className)
        {
            if (className.Equals(AdminConstants.ZnodeShippingFedEx.ToString()) || className.Equals(AdminConstants.ZnodeShippingUps.ToString()) || className.Equals(AdminConstants.ZnodeShippingUSPS.ToString()))
                return true;
            else
                return false;
        }

        //Update shipping
        public virtual ShippingViewModel UpdateShipping(ShippingViewModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

                FilterCollection shippingTypeFilters = new FilterCollection();
                shippingTypeFilters.Add(new FilterTuple(FilterKeys.ClassName, FilterOperators.Contains, model.ClassName.ToString()));

                ShippingTypeListModel shippingTypeListModel = _shippingTypeClient.GetShippingTypeList(shippingTypeFilters, null, null, null);

                if (shippingTypeListModel?.ShippingTypeList?.Count > 0)
                    model.ShippingTypeId = shippingTypeListModel.ShippingTypeList.FirstOrDefault().ShippingTypeId;

                if (CheckShippingMethod(model.ClassName))
                    model.ShippingCode = _shippingsClient.GetShippingServiceCode(model.ShippingServiceCodeId)?.Code;

                //Get the imported file and convert SKUs in it into comma seperated string.
                if (IsNotNull(model.ImportedSkus))
                    model.ImportedSkus = GetSKUsFromImportedFile(model.ImportFile);
                SetDropDownSelectAllValue(model);
                ShippingModel shippingModel = _shippingsClient.UpdateShipping(model.ToModel<ShippingModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

                return IsNotNull(shippingModel) ? shippingModel.ToViewModel<ShippingViewModel>() : (ShippingViewModel)GetViewModelWithErrorMessage(new ShippingViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (ShippingViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.AlreadyExistShipping);

                    default:
                        return (ShippingViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Error);
                return (ShippingViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete shipping by shipping Id.
        public virtual bool DeleteShipping(string shippingId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(shippingId))
            {
                try
                {
                    return _shippingsClient.DeleteShipping(new ParameterModel { Ids = shippingId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = Admin_Resources.ErrorDeleteShippingAsAssociatedWithOrder;
                            return false;
                        case ErrorCodes.DefaultDataDeletionError:
                            errorMessage = Admin_Resources.ErrorDeleteFreeShipping;
                            return false;
                        default:
                            errorMessage = Admin_Resources.ErrorFailedToDelete;
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Get shipping service code list.
        public virtual List<SelectListItem> GetShippingServiceCodeList(string className)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            List<SelectListItem> selectedShippingServiceList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(className))
            {
                ShippingServiceCodeListModel shippingServiceList = new ShippingServiceCodeListModel();
                ShippingViewModel model = new ShippingViewModel();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(FilterKeys.ServiceShippingTypeId, FilterOperators.Equals, Convert.ToString(GetShippingTypeId(className))));
                if ((className.Equals(AdminConstants.ZnodeShippingFedEx)) || className.Equals(AdminConstants.ZnodeShippingUps.ToString()) || className.Equals(AdminConstants.ZnodeShippingUSPS.ToString()))
                    shippingServiceList = _shippingsClient.GetShippingServiceCodeList(filters, null, null, null);
                else
                    shippingServiceList = _shippingsClient.GetShippingServiceCodeList(null, null, null, null);

                ZnodeLogging.LogMessage("ShippingServiceCodes count: ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { shippingServiceListCount = shippingServiceList?.ShippingServiceCodes?.Count });

                if (shippingServiceList?.ShippingServiceCodes?.Count > 0)
                    shippingServiceList.ShippingServiceCodes.ToList().ForEach(item => { selectedShippingServiceList.Add(new SelectListItem() { Text = item.Description, Value = item.ShippingServiceCodeId.ToString() }); });
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return selectedShippingServiceList;
        }

        //Set filter for shippingId
        public virtual void SetFilters(FilterCollection filters, int shippingId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (!Equals(filters, null))
            {
                //Checking For ShippingId already Exists in Filters Or Not
                if (filters.Exists(x => x.Item1 == FilterKeys.ShippingId))
                {
                    //If ShippingId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == FilterKeys.ShippingId);

                    //Add New ShippingId Into filters
                    filters.Add(new FilterTuple(FilterKeys.ShippingId, FilterOperators.Equals, shippingId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.ShippingId, FilterOperators.Equals, shippingId.ToString()));
            }
        }

        //Set filter for ShippingRuleId.
        public virtual void SetShippingRuleFilters(FilterCollection filters, int shippingRuleId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (!Equals(filters, null))
            {
                //Checking For ShippingId already Exists in Filters Or Not
                if (filters.Exists(x => x.Item1 == FilterKeys.ShippingRuleId))
                {
                    //If ShippingId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == FilterKeys.ShippingRuleId);

                    //Add New ShippingId Into filters
                    filters.Add(new FilterTuple(FilterKeys.ShippingRuleId, FilterOperators.Equals, shippingRuleId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.ShippingRuleId, FilterOperators.Equals, shippingRuleId.ToString()));
            }
        }

        //Bind dropdowns.
        public virtual void BindDropdownValues(ShippingViewModel viewModel, bool isEditMode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            viewModel.ShippingTypeList = GetShippingTypes(isEditMode);
            viewModel.CountryList = GetCountryList();
            viewModel.CurrencyList = GetActiveCurrency();
            viewModel.CityList = GetCityListByStateCode(viewModel.StateCode);
            viewModel.StateList = GetStateListByCountryCode(viewModel.DestinationCountryCode);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

        }

        //Get statelist on the basis of country code.
        public virtual List<SelectListItem> GetStateListByCountryCode(string countryCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            List<SelectListItem> selectedStateList = new List<SelectListItem>();

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.CountryCode, FilterOperators.Is, countryCode));

            SortCollection sorts = new SortCollection();
            sorts.Add(ZnodeStateEnum.StateName.ToString(), DynamicGridConstants.ASCKey);
            ZnodeLogging.LogMessage("Input parameters filters and sorts of method GetStateList:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            StateListModel stateListModel = _stateClient.GetStateList(filters, sorts);

            ZnodeLogging.LogMessage("States count:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { statesCount = stateListModel?.States?.Count });

            if (stateListModel?.States?.Count > 0)
                stateListModel.States.ToList().ForEach(item => { selectedStateList.Add(new SelectListItem() { Text = item.StateName, Value = item.StateCode }); });

            selectedStateList.Insert(0, new SelectListItem() { Value = Admin_Resources.LabelAllStates, Text = Admin_Resources.LabelAllStates });

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return selectedStateList;
        }

        //Get citylist(countyList) on the basis of statecode.
        public virtual List<SelectListItem> GetCityListByStateCode(string stateCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            List<SelectListItem> selectedCityList = new List<SelectListItem>();

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.StateCode, FilterOperators.Is, stateCode));

            CityListModel cityListModel = _cityClient.GetCityList(filters, null);

            ZnodeLogging.LogMessage("Cities count:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { citiesCount = cityListModel?.Cities?.Count });

            if (cityListModel?.Cities?.Count > 0)
                cityListModel.Cities.ToList().GroupBy(x => x.CountyFIPS).Select(y => y.First()).ToList().ForEach(item => { selectedCityList.Add(new SelectListItem() { Text = item.CountyFIPS, Value = item.CountyFIPS }); });
            selectedCityList.Insert(0, new SelectListItem() { Value = null, Text = Admin_Resources.LabelAllCounties });

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return selectedCityList;
        }

        //Bind shipping option list.
        public virtual ShippingListViewModel BindShippingList(CreateOrderViewModel createOrderViewModel, ProfileListModel profileList, int userId = 0, int portalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            if (!IsValidShippingAddress(createOrderViewModel))
            {
                return new ShippingListViewModel();
            }

            ExpandCollection expands = null;
            FilterCollection filters = null;
            SortCollection sorts = null;

            //Set filter, expand and sorts for getting shipping list.
            filters = SetFilterDataForShippingList(createOrderViewModel?.UserAddressDataViewModel, ref expands, ref sorts);
            filters.Add(new FilterTuple(FilterKeys.ShippingDestinationCountryCode, FilterOperators.Equals, createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress?.CountryName));
            filters.Add(new FilterTuple(FilterKeys.ShippingDestinationStateCode, FilterOperators.Equals, createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress?.StateName));
            if (userId > 0)
                filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, Convert.ToString(userId)));

            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, Convert.ToString(portalId)));

            ZnodeLogging.LogMessage("Input parameters expands, filters and sorts of method GetShippingList:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { expands = expands, filters= filters, sorts = sorts });
            // Get shipping option list.
            ShippingListModel shippingListModel = _shippingsClient.GetShippingList(expands, filters, sorts, null, null);

            if (!IsNotNull(shippingListModel))
                shippingListModel = new ShippingListModel() { ShippingList = new List<ShippingModel>() };

            // Filter shipping option by customer country code and all country code & state code.
            shippingListModel.ShippingList = shippingListModel?.ShippingList?.ToList();

            string shippingZipCode = createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress?.PostalCode ?? string.Empty;

            // Filter shipping option by customer zipcode.
            if (!string.IsNullOrEmpty(shippingZipCode))
                shippingListModel.ShippingList = GetShippingByZipCode(shippingZipCode, shippingListModel.ShippingList);

            // Get user details.
            UserAddressDataViewModel userModel = _userAgent.GetUserAccountViewModel();

            ZnodeLogging.LogMessage("ShippingList count:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { shippingListCount = shippingListModel?.ShippingList?.Count });

            if (shippingListModel?.ShippingList?.Count > 0)
                // Get Account ProfileID, or Get Default Anonymous ProfileId from Portal.
                GetShippingOptionsByProfile(shippingListModel, profileList);

            ShippingListViewModel listViewModel = new ShippingListViewModel { ShippingList = shippingListModel?.ShippingList?.ToViewModel<ShippingViewModel>().ToList() };

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            return listViewModel?.ShippingList?.Count > 0 ? listViewModel : new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };
        }

        public virtual ShippingListViewModel BindManageShippingList(int omsOrderId, ProfileListModel profileList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            OrderModel orderModel = GetFromSession<OrderModel>(AdminConstants.OMSOrderSessionKey + omsOrderId);

            //Get shipping option list.
            ShippingListModel model = _shippingsClient.GetShippingList(null, GetFilterForShipping(orderModel), null, null, null);

            // Filter shipping option by customer zipcode.
            if (!string.IsNullOrEmpty(orderModel?.ShippingAddress?.PostalCode) && model?.ShippingList?.Count > 0)
                model.ShippingList = GetShippingByZipCode(orderModel?.ShippingAddress?.PostalCode, model.ShippingList);

            ZnodeLogging.LogMessage("ShippingList count:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { shippingListCount = model?.ShippingList?.Count });

            if (model?.ShippingList?.Count > 0)
                // Get Account ProfileID, or Get Default Anonymous ProfileId from Portal.
                GetShippingOptionsByProfile(model, profileList);

            ShippingListViewModel listViewModel = new ShippingListViewModel { ShippingList = model?.ShippingList?.ToViewModel<ShippingViewModel>().ToList() };
            listViewModel.SelectedShippingCode = orderModel.ShippingId;
            listViewModel.ShippingId = orderModel.ShippingId;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return listViewModel?.ShippingList?.Count > 0 ? listViewModel : new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };
        }

        // Get filter.
        public virtual FilterCollection GetFilterForShipping(OrderModel orderModel)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "1"));
            filters.Add(new FilterTuple(FilterKeys.ShippingDestinationCountryCode, FilterOperators.Equals, orderModel?.ShippingAddress?.CountryName));
            filters.Add(new FilterTuple(FilterKeys.ShippingDestinationStateCode, FilterOperators.Equals, orderModel?.ShippingAddress?.StateName));
            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, Convert.ToString(orderModel.PortalId)));
            filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.Equals, Convert.ToString(orderModel.UserId)));
            return filters;
        }

        //Get currency list.
        public virtual List<SelectListItem> GetActiveCurrency()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, FilterKeys.ActiveTrueValue));
            CurrencyListModel currencyList = _currencyClient.GetCurrencyList(null, filters, null, null, null);
            List<SelectListItem> selectedCountryList = new List<SelectListItem>();
            if (currencyList?.Currencies?.Count > 0)
                currencyList.Currencies.OrderByDescending(x => x.IsDefault).ToList().ForEach(item => { selectedCountryList.Add(new SelectListItem() { Text = item.CurrencyName + " " + item.Symbol, Value = item.CurrencyId.ToString() }); });
         
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return selectedCountryList;
        }

        #endregion Shipping

        #region Shipping SKU

        //Get shipping SKUs list.
        public virtual ShippingSKUListViewModel GetShippingSKUList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Input parameters expands, filters and sorts: ", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { expands = expands, filters= filters, sorts = sorts });

            ShippingSKUListModel shippingSKUList = _shippingsClient.GetShippingSKUList(expands, filters, sorts, pageIndex, recordPerPage);
            ShippingSKUListViewModel shippingSKUListViewModel = new ShippingSKUListViewModel { ShippingSKUList = shippingSKUList?.ShippingSKUList?.ToViewModel<ShippingSKUViewModel>().ToList() };
            SetListPagingData(shippingSKUListViewModel, shippingSKUList);

            //Set tool menu for Shipping SKU list grid view.
            SetShippingSKUListToolMenu(shippingSKUListViewModel);
            return shippingSKUList?.ShippingSKUList?.Count > 0 ? shippingSKUListViewModel
                : new ShippingSKUListViewModel() { ShippingSKUList = new List<ShippingSKUViewModel>() };
        }

        //Add shipping SKU(comma separated).
        public virtual ShippingSKUViewModel AddShippingSKU(ShippingSKUViewModel shippingSKUViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return _shippingsClient.AddShippingSKU(shippingSKUViewModel?.ToModel<ShippingSKUModel>())?.ToViewModel<ShippingSKUViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (ShippingSKUViewModel)GetViewModelWithErrorMessage(new ShippingSKUViewModel(), Admin_Resources.ErrorShippingSKUAlreadyAssociated);

                    default:
                        return (ShippingSKUViewModel)GetViewModelWithErrorMessage(new ShippingSKUViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Error);
                return (ShippingSKUViewModel)GetViewModelWithErrorMessage(new ShippingSKUViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Delete shipping SKUs
        public virtual bool DeleteShippingSKU(string shippingSKUId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            message = string.Empty;
            if (!string.IsNullOrEmpty(shippingSKUId))
            {
                try
                {
                    return _shippingsClient.DeleteShippingSKU(shippingSKUId);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Warning);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return false;
        }

        //Get shipping SKU list.
        public virtual ShippingSKUListViewModel GetShippingSKUListViewModel(FilterCollectionDataModel model, int shippingId, int shippingRuleId, string shippingRuleType)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedShippingProductList.ToString(), model);
            //Set filter.
            SetFilterforShippingCost(model, shippingRuleType);

            ShippingSKUListViewModel shippingProduct = new ShippingSKUListViewModel();
            ProductDetailsListModel ProductList = shippingId > 0 ? _productClient.GetProducts(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage) : new ProductDetailsListModel() { XmlDataList = new List<dynamic>() };
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Assortment);
            shippingProduct.GridModel = FilterHelpers.GetDynamicGridModel(model, ProductList?.XmlDataList ?? new List<dynamic>(), GridListType.AssociatedShippingProductList.ToString(), string.Empty, null, true, true, null, CustomAttributeColumn(ProductList.AttributeColumnName));

            //Set paging
            SetListPagingData(shippingProduct, ProductList);
            shippingProduct.ShippingId = shippingId;
            shippingProduct.ShippingRuleId = shippingRuleId;
            shippingProduct.ShippingRuleTypeCode = shippingRuleType;
            shippingProduct.GridModel.TotalRecordCount = shippingProduct.TotalResults;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return shippingProduct;
        }

        #endregion Shipping SKU

        #region Shipping Rule

        //Get shipping rule list.
        public virtual ShippingRuleListViewModel ShippingRuleList(FilterCollectionDataModel model, int shippingId, int shippingTypeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeShippingRule.ToString(), model);
            SetFilters(model.Filters, shippingId);
            ZnodeLogging.LogMessage("Input parameters expands, filters and sorts of method GetShippingRuleList.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { expands = model?.Expands,filters = model?.Filters ,sorts = model?.SortCollection });

            ShippingRuleListViewModel shippingRuleList = GetShippingRuleList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            shippingRuleList.ShippingId = shippingId;
            shippingRuleList.ShippingTypeId = shippingTypeId;

            //Get the Shipping Type Details
            ProviderEngineViewModel shippingModel = _providerEngineAgent.GetShippingType(shippingTypeId);
            shippingRuleList.ClassName = shippingModel?.ClassName;

            shippingRuleList.GridModel = FilterHelpers.GetDynamicGridModel(model, shippingRuleList?.ShippingRuleList, GridListType.ZnodeShippingRule.ToString(), string.Empty, null, true, true, shippingRuleList?.GridModel?.FilterColumn?.ToolMenuList);
            shippingRuleList.GridModel.TotalRecordCount = shippingRuleList.TotalResults;

            shippingRuleList.Name = _shippingsClient.GetShipping(shippingId)?.ShippingName;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return shippingRuleList;
        }

        //Get shipping rule list.
        public virtual ShippingRuleListViewModel GetShippingRuleList(ExpandCollection expands, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            }

            if (IsNull(expands))
            {
                expands = new ExpandCollection();
                expands.Add(ZnodeShippingRuleEnum.ShippingRuleTypeCode.ToString());
            }

            ShippingRuleListModel shippingRuleList = _shippingsClient.GetShippingRuleList(expands, filters, sorts, pageIndex, recordPerPage);
            ShippingRuleListViewModel shippingRuleListViewModel = new ShippingRuleListViewModel { ShippingRuleList = shippingRuleList?.ShippingRuleList?.ToViewModel<ShippingRuleViewModel>().ToList() ?? new List<ShippingRuleViewModel>() };
            SetListPagingData(shippingRuleListViewModel, shippingRuleList);

            //Set tool menu for Shipping Rule list grid view.
            SetShippingRuleListToolMenu(shippingRuleListViewModel);
            return shippingRuleList?.ShippingRuleList?.Count > 0 ? shippingRuleListViewModel
                : new ShippingRuleListViewModel() { ShippingRuleList = new List<ShippingRuleViewModel>() };
        }

        //Get shipping rule by shipruleId.
        public virtual ShippingRuleViewModel GetShippingRule(int shippingRuleId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            ShippingRuleModel shippingRuleModel = _shippingsClient.GetShippingRule(shippingRuleId);
            ShippingViewModel shippingViewModel = GetShippingById(shippingRuleModel.ShippingId);
            shippingRuleModel.ClassName = _providerEngineAgent.GetShippingType(shippingViewModel.ShippingTypeId)?.ClassName;

            ShippingRuleViewModel shippingRuleViewModel = shippingRuleModel.ToViewModel<ShippingRuleViewModel>();
            shippingRuleViewModel.ShippingTypeId = shippingViewModel.ShippingTypeId;
            shippingRuleViewModel.ShippingRuleTypeList = GetShippingRuleTypes(shippingRuleModel.ShippingId, true);
            shippingRuleViewModel.CurrencyList = GetActiveCurrency();
            shippingRuleViewModel.CurrencyId = shippingViewModel.CurrencyId;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return shippingRuleViewModel;
        }

        //Add shipping rule.
        public virtual ShippingRuleViewModel AddShippingRule(ShippingRuleViewModel shippingRuleViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            try
            {
                return _shippingsClient.AddShippingRule(shippingRuleViewModel?.ToModel<ShippingRuleModel>())?.ToViewModel<ShippingRuleViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (ShippingRuleViewModel)GetViewModelWithErrorMessage(new ShippingRuleViewModel(), Admin_Resources.ErrorShippingRuleAlreadyExists);

                    default:
                        return (ShippingRuleViewModel)GetViewModelWithErrorMessage(new ShippingRuleViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Error);
                return (ShippingRuleViewModel)GetViewModelWithErrorMessage(new ShippingRuleViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Update shipping rule.
        public virtual ShippingRuleViewModel UpdateShippingRule(ShippingRuleViewModel shippingRuleViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            try
            {
                return _shippingsClient.UpdateShippingRule(shippingRuleViewModel?.ToModel<ShippingRuleModel>())?.ToViewModel<ShippingRuleViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Error);
                return (ShippingRuleViewModel)GetViewModelWithErrorMessage(new ShippingRuleViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete shipping rule.
        public virtual bool DeleteShippingRule(string shippingRuleId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            message = string.Empty;
            if (!string.IsNullOrEmpty(shippingRuleId))
            {
                try
                {
                    return _shippingsClient.DeleteShippingRule(shippingRuleId);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Warning);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Get Shipping rule type list.
        public virtual List<SelectListItem> GetShippingRuleTypes(int shippingId, bool isEditMode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeShippingTypeEnum.IsActive.ToString(), FilterOperators.Equals, "true"));

            if (!isEditMode)
                filters.Add(new FilterTuple(ZnodeShippingEnum.ShippingId.ToString(), FilterOperators.Equals, shippingId.ToString()));

            ZnodeLogging.LogMessage("Filters to get shippingRuleTypeList", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { Filters = filters });
            ShippingRuleTypeListModel shippingRuleTypeList = _shippingsClient.GetShippingRuleTypeList(filters, null);

            List<SelectListItem> selectedShippingRuleTypeList = new List<SelectListItem>();
            if (shippingRuleTypeList?.ShippingRuleTypeList?.Count > 0)
                shippingRuleTypeList.ShippingRuleTypeList.OrderBy(x => x.AttributeDefaultValueCode).ToList().ForEach(item => { selectedShippingRuleTypeList.Add(new SelectListItem() { Text = item?.ValueLocales?.Where(x => x.LocaleId?.ToString() == DefaultSettingHelper.DefaultLocale)?.FirstOrDefault()?.DefaultAttributeValue, Value = item.AttributeDefaultValueCode.ToString() }); });

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return selectedShippingRuleTypeList;
        }

        #endregion Shipping Rule

        #endregion Public Methods

        #region Private Methods

        //get ShipingtypeId list by class name.
        private int GetShippingTypeId(string className)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(AdminConstants.ClassName, FilterOperators.Contains, className));
            ShippingTypeListModel shippingTypeListModel = _shippingTypeClient.GetShippingTypeList(filters, null, null, null);
            return shippingTypeListModel.ShippingTypeList.FirstOrDefault().ShippingTypeId;
        }

        //Set tool menu for Shipping list grid view.
        private void SetShippingListToolMenu(ShippingListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ShippingDeletePopup')", ControllerName = "Shippings", ActionName = "Delete" });
            }
        }

        //Set tool menu for Shipping SKU list grid view.
        private void SetShippingSKUListToolMenu(ShippingSKUListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ShippingSKUDeletePopup')", ControllerName = "Shippings", ActionName = "DeleteShippingSKU" });
            }
        }

        //Set tool menu for Shipping Rule list grid view.
        private void SetShippingRuleListToolMenu(ShippingRuleListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ShippingRuleDeletePopup')", ControllerName = "Shippings", ActionName = "DeleteShippingRule" });
            }
        }

        //Set tool menu for Shipping portal list grid view.
        private void SetShippingPortalListToolMenu(ShippingPortalListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ShippingPortalDeletePopup')", ControllerName = "Shippings", ActionName = "UnassociateStore" });
            }
        }

        //Get SKUsFromImportedFil.
        private string GetSKUsFromImportedFile(HttpPostedFileBase importedSKUFile)
        {
            DataTable skuTable = HelperMethods.GetImportDetails(importedSKUFile);
            return string.Join(",", skuTable.AsEnumerable().Select(row => row.Field<string>("SKU")));
        }

        //Set handling charges.
        private ShippingViewModel SetHandlingChargesBasedOn(ShippingViewModel model)
        {
            if (!IsNull(model))
            {
                switch (model.HandlingChargeBasedOn)
                {
                    case "Shipping":
                        model.IsSubtotal = false;
                        model.IsShipping = true;
                        model.IsAmount = false;
                        break;

                    case "SubTotal":
                        model.IsSubtotal = true;
                        model.IsShipping = false;
                        model.IsAmount = false;
                        break;

                    case "Amount":
                        model.IsSubtotal = false;
                        model.IsShipping = false;
                        model.IsAmount = true;
                        break;

                    default:
                        model.IsAmount = true;
                        break;
                }
            }
            else
                model = new ShippingViewModel();

            return model;
        }

        //Get Shipping types list.
        private List<SelectListItem> GetShippingTypes(bool isEditMode)
        {
            FilterCollection filters = new FilterCollection();
            if (!isEditMode)
                filters.Add(new FilterTuple(ZnodeShippingTypeEnum.IsActive.ToString(), FilterOperators.Equals, "true"));

            ShippingTypeListModel shippingTypeList = _shippingTypeClient.GetShippingTypeList(filters, null, null, null);
            List<SelectListItem> selectedShippingTypeList = new List<SelectListItem>();
            if (shippingTypeList?.ShippingTypeList?.Count > 0)
            {
                shippingTypeList.ShippingTypeList.OrderBy(x => x.Name).ToList().ForEach(item => { selectedShippingTypeList.Add(new SelectListItem() { Text = item.Name, Value = item.ClassName.ToString() }); });
            }
            return selectedShippingTypeList;
        }

        //Get profile list.
        private List<SelectListItem> GetProfileTypes()
        {
            List<SelectListItem> profileList = _profileAgent.GetProfileList();
            profileList?.Insert(0, new SelectListItem() { Value = zeroIndex, Text = Admin_Resources.LabelAllProfiles });
            return profileList;
        }

        //Set country ,state and county value as null when selected for all countries,all states and all counties.
        private void SetDropDownSelectAllValue(ShippingViewModel shippingModel)
        {
            if (!Equals(shippingModel, null))
            {
                shippingModel.DestinationCountryCode = Equals(shippingModel.DestinationCountryCode, Admin_Resources.LabelAllCountries) ? null : shippingModel.DestinationCountryCode;
                shippingModel.StateCode = Equals(shippingModel.StateCode, Admin_Resources.LabelAllStates) ? null : shippingModel.StateCode;
                shippingModel.CountyFIPS = Equals(shippingModel.CountyFIPS, Admin_Resources.LabelAllCounties) ? null : shippingModel.CountyFIPS;
            }
        }

        //Get country list.
        private List<SelectListItem> GetCountryList()
        {
            List<SelectListItem> countryList = _countryAgent.GetActiveCountryList();
            countryList.Insert(0, new SelectListItem() { Value = null, Text = Admin_Resources.LabelAllCountries });
            return countryList;
        }

        // Get shipping address details from session.
        public virtual AddressViewModel GetShippingAddress()
        {
            AddressViewModel addressModel = GetFromSession<AddressViewModel>(AdminConstants.ShippingAddressKey);
            if (IsNull(addressModel))
            {
                //Get user data if AddressViewModel is null.
                UserAddressDataViewModel userModel = _userAgent.GetUserAccountViewModel();
                if (IsNotNull(userModel))
                    addressModel = userModel.ShippingAddress;
            }
            return addressModel;
        }

        // Get Account ProfileID, or Get Default Anonymous ProfileId from Portal.
        private void GetShippingOptionsByProfile(ShippingListModel shippingListModel, ProfileListModel profileList)
        {
            if (profileList?.Profiles?.Count > 0)
                shippingListModel.ShippingList = shippingListModel?.ShippingList.Where(p => profileList.Profiles.Any(p2 => p2.ProfileId == p.ProfileId || p.ProfileId == null)).ToList();
        }

        //Set filter, expand and sorts for getting shipping list.
        public virtual FilterCollection SetFilterDataForShippingList(UserAddressDataViewModel addressModel, ref ExpandCollection expands, ref SortCollection sorts)
        {
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.DisplayOrder, DynamicGridConstants.ASCKey);
            }
            if (IsNull(expands))
            {
                expands = new ExpandCollection();
                //expands.Add(ZnodeShippingEnum.ZnodeProfile.ToString());
                expands.Add(ZnodeShippingEnum.ZnodeShippingType.ToString());
                expands.Add(ZnodePortalEnum.ZnodePortalProfiles.ToString());
            }

            //add filter for active shipping.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, FilterKeys.ActiveTrue));
            return filters;
        }

        //Set filter for equal clause.
        private void SetFilterforShippingCost(FilterCollectionDataModel model, string shippingRuleTypeCode)
        {
            if (model.Filters.Exists(x => x.Item1 == AdminConstants.ShippingCost))
            {
                model.Filters.RemoveAll(x => x.Item1 == AdminConstants.ShippingCost);
                model.Filters.Insert(0, new FilterTuple(AdminConstants.ShippingCost, FilterOperators.Is, shippingRuleTypeCode));
            }
            else
                model.Filters.Insert(0, new FilterTuple(AdminConstants.ShippingCost, FilterOperators.Is, shippingRuleTypeCode));

            if (!model.Filters.Exists(x => x.Item1 == AdminConstants.IsCallForAttribute))
                model.Filters.Add(new FilterTuple(AdminConstants.IsCallForAttribute, FilterOperators.Equals, "True"));
        }

        //to get filtered shipping option by zipcode
        private List<ShippingModel> GetShippingByZipCode(string zipcode, List<ShippingModel> shippinglist)
        {
            if (shippinglist?.Count > 0)
            {
                List<ShippingModel> filteredShippingList = new List<ShippingModel>();
                //to check each shipping option have zipcode entered by user
                foreach (ShippingModel shipping in shippinglist)
                {
                    //if shipping option zipcode is null or "*" then allow for all zipcode entered by user
                    if (string.IsNullOrEmpty(shipping.ZipCode) || shipping.ZipCode.Trim() == "*")
                    {
                        filteredShippingList.Add(shipping);
                    }
                    else
                    {
                        //if shipping option zipcode contains "," then it will have more than one zipcode allows
                        if (shipping.ZipCode.Contains(","))
                        {
                            string[] allZipCodesAssignToShipping = shipping.ZipCode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            //to check each zipcode that entered against shipping  option comma separate
                            foreach (string shippingZipCode in allZipCodesAssignToShipping)
                            {
                                //to check zipcode for each shipping
                                if (IsValidShippingZipCode(zipcode, shippingZipCode, shipping, filteredShippingList))
                                    break;
                            }
                        }
                        else
                        {
                            IsValidShippingZipCode(zipcode, shipping.ZipCode, shipping, filteredShippingList);
                        }
                    }
                }
                return filteredShippingList;
            }
            return shippinglist;
        }

        //to check zipcode is valid for shipping option
        private bool IsValidShippingZipCode(string userZipcode, string shippingOptionZipcode, ShippingModel shipping, List<ShippingModel> filteredShippingList)
        {
            ZnodeLogging.LogMessage("Input parameters userZipcode, shippingOptionZipcode: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { userZipcode= userZipcode, shippingOptionZipcode= shippingOptionZipcode });

            bool result = false;
            //add shipping Option zipcode having "*"
            if (shippingOptionZipcode.Contains("*"))
            {
                string shippingZipCode = shippingOptionZipcode.Replace("*", string.Empty).Trim();
                //shipping Option Zipcode start with the user zipcode then allow to add
                if (userZipcode.Trim().StartsWith(shippingZipCode))
                {
                    filteredShippingList.Add(shipping);
                    result = true;
                }
            }
            //add if shipping Option Zipcode is same as user zipcode then allow to add
            else if (string.Equals(shippingOptionZipcode.Trim(), userZipcode.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                filteredShippingList.Add(shipping);
                result = true;
            }
            return result;
        }

        //to check user shippingaddress is valid
        private bool IsValidShippingAddress(CreateOrderViewModel createOrderViewModel)
        {
            bool isValid = true;
            string shippingCountryCode = createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress?.CountryName ?? string.Empty;
            string shippingstateCode = createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress?.StateName ?? string.Empty;
            string shippingZipCode = createOrderViewModel?.UserAddressDataViewModel?.ShippingAddress?.PostalCode ?? string.Empty;

            //if user shipping CountryCode, state and zipcode is null then no shipping option will available for that user address
            if (string.IsNullOrEmpty(shippingCountryCode) || string.IsNullOrEmpty(shippingstateCode) || string.IsNullOrEmpty(shippingZipCode))
            {
                isValid = false;
            }
            return isValid;
        }

        //Check whether the shipping name already exists.
        public virtual bool CheckShippingNameExist(string shippingName, int shippingId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            if (!string.IsNullOrEmpty(shippingName))
            {
                shippingName = shippingName.Trim();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeShippingEnum.ShippingName.ToString(), FilterOperators.Contains, shippingName));

                //Get the payment list based on the shipping name filter.
                ZnodeLogging.LogMessage("Filters to get shippingList: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters });
                ShippingListModel shippingList = _shippingsClient.GetShippingList(null, filters, null, null, null);
                
                if (shippingList?.ShippingList?.Count > 0)
                {
                    if (shippingId > 0)
                        //Set the status in case the shipping is open in edit mode.
                        shippingList.ShippingList.RemoveAll(x => x.ShippingId == shippingId);

                    return shippingList.ShippingList.FindIndex(x => string.Equals(x.ShippingName, shippingName, StringComparison.CurrentCultureIgnoreCase)) != -1;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

            return false;
        }

        #endregion Private Methods
    }
}