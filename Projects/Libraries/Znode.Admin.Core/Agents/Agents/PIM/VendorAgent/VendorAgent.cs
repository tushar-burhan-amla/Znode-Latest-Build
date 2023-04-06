using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public class VendorAgent : BaseAgent, IVendorAgent
    {
        #region Private Variables

        private readonly IVendorClient _vendorClient;
        private readonly ICountryClient _countryClient;
        private readonly IProductsClient _productClient;

        #endregion Private Variables

        public VendorAgent(IVendorClient vendorClient, ICountryClient countryClient, IProductsClient productsClient)
        {
            _vendorClient = GetClient<IVendorClient>(vendorClient);
            _countryClient = GetClient<ICountryClient>(countryClient);
            _productClient = GetClient<IProductsClient>(productsClient);
        }

        #region Public  methods

        #region Vendor

        public virtual VendorListViewModel GetVendorList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetVendorList(expands, filters, sorts, null, null);

        //Get vendor list
        public virtual VendorListViewModel GetVendorList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { expands = expands, filters= filters, sorts = sorts });
            VendorListModel vendorList = _vendorClient.GetVendorList(expands, filters, sorts, pageIndex, pageSize);
            VendorListViewModel listViewModel = new VendorListViewModel { Vendors = vendorList?.Vendors?.ToViewModel<VendorViewModel>().ToList() };
            SetListPagingData(listViewModel, vendorList);

            //Set tool menu for vendor list grid view.
            SetVendorListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            return vendorList?.Vendors?.Count > 0 ? listViewModel : new VendorListViewModel() { Vendors = new List<VendorViewModel>() };
        }

        //Create vendor.
        public virtual VendorViewModel CreateVendor(VendorViewModel vendorViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            try
            {
                VendorModel vendorModel = _vendorClient.CreateVendor(vendorViewModel.ToModel<VendorModel>());
                return IsNotNull(vendorModel) ? vendorModel?.ToViewModel<VendorViewModel>() : new VendorViewModel() { VendorCodeList = new List<SelectListItem>(), Address = new WarehouseAddressViewModel() { Countries = new List<SelectListItem>() } };
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (VendorViewModel)GetViewModelWithErrorMessage(vendorViewModel, Admin_Resources.AlreadyExistCode);

                    default:
                        return (VendorViewModel)GetViewModelWithErrorMessage(vendorViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return (VendorViewModel)GetViewModelWithErrorMessage(vendorViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get vendor by vendorId
        public virtual VendorViewModel GetVendor(int pimVendorId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (pimVendorId > 0)
            {
                VendorViewModel vendorViewModel = new VendorViewModel();
                vendorViewModel = _vendorClient.GetVendor(pimVendorId)?.ToViewModel<VendorViewModel>();

                if (IsNotNull(vendorViewModel))
                {
                    vendorViewModel.VendorCodeList = GetVendorCodeList();
                    vendorViewModel.Address.Countries = GetCountries();
                    return vendorViewModel;
                }
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return new VendorViewModel();
        }

        //Get vendor code list.
        public virtual List<SelectListItem> GetVendorCodeList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            VendorListModel vendorCodeList = _vendorClient.GetVendorCodeList(AdminConstants.Vendor);
            List<SelectListItem> selectedVendorCodeList = new List<SelectListItem>();
            if (vendorCodeList?.VendorCodes?.Count > 0)
                vendorCodeList.VendorCodes.ToList().ForEach(item =>
                {
                    string defaultValueLocale = item.ValueLocales.FirstOrDefault(x => x.LocaleId?.ToString() == DefaultSettingHelper.DefaultLocale)?.DefaultAttributeValue;
                    selectedVendorCodeList.Add(new SelectListItem() { Text = string.IsNullOrEmpty(defaultValueLocale) ? item.AttributeDefaultValueCode : defaultValueLocale, Value = item.AttributeDefaultValueCode });
                });
            ZnodeLogging.LogMessage("selectedVendorCodeList list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, selectedVendorCodeList?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return selectedVendorCodeList;
        }

        //Update vendor information
        public virtual VendorViewModel UpdateVendor(VendorViewModel viewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            try
            {
                VendorModel model = _vendorClient.UpdateVendor(viewModel.ToModel<VendorModel>());
                return IsNotNull(model) ? model.ToViewModel<VendorViewModel>() : new VendorViewModel() { HasError = true, ErrorMessage = PIM_Resources.UpdateErrorMessage, VendorCodeList = new List<SelectListItem>(), Address = new WarehouseAddressViewModel() { Countries = new List<SelectListItem>() } };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return (VendorViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete vendor.
        public virtual bool DeleteVendor(string pimVendorId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            message = Admin_Resources.ErrorFailedToDelete;
            if (!string.IsNullOrEmpty(pimVendorId))
            {
                try
                {
                    return _vendorClient.DeleteVendor(new ParameterModel { Ids = pimVendorId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Active/Inactive Vendor
        public virtual bool ActiveInactiveVendor(string vendorIds, bool isActive)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            try
            {
                return _vendorClient.ActiveInactiveVendor(vendorIds, isActive);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get List of Active Countries.
        public virtual List<SelectListItem> GetCountries()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            List<SelectListItem> countriesSelectList = new List<SelectListItem>();
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCountryEnum.IsActive.ToString(), FilterOperators.Equals, "true"));
            CountryListModel countries = _countryClient.GetCountryList(null, filters, null);
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info,new { filters = filters });

            if (countries?.Countries?.Count > 0)
            {
                //Set default country on top in dropdown as per in Global setting.
                SetDefaultCountry(countries);
                foreach (CountryModel country in countries.Countries)
                    countriesSelectList.Add(new SelectListItem() { Text = country.CountryName, Value = country.CountryCode });
            }
            ZnodeLogging.LogMessage("countriesSelectList list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, countriesSelectList?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return countriesSelectList;
        }

        #endregion Vendor

        #region Associated Products

        //Get product List
        public virtual VendorListViewModel AssociatedProductList(FilterCollectionDataModel model, int pimVendorId, string vendorCode, string vendorName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            //Checking For vendor already Exists in Filters Or Not
            SetFilterforVendor(model, vendorCode);
            VendorListViewModel vendorProducts = new VendorListViewModel();
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedVendorProductList.ToString(), model);

            ProductDetailsListModel ProductList = _productClient.GetProducts(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            ProductList.ProductDetailList.ForEach(item => item.AttributeValue = vendorCode);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.ProductImage);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Vendor);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Assortment);
            vendorProducts.GridModel = FilterHelpers.GetDynamicGridModel(model, ProductList?.XmlDataList ?? new List<dynamic>(), GridListType.AssociatedVendorProductList.ToString(), string.Empty, null, true, true, null, CustomAttributeColumn(ProductList.AttributeColumnName));
            SetAssociatedVendorProductListToolMenu(vendorProducts);
            SetListPagingData(vendorProducts, ProductList);
            vendorProducts.VendorCode = vendorCode;
            vendorProducts.VendorName = vendorName;
            vendorProducts.PimVendorId = pimVendorId;
            vendorProducts.GridModel.TotalRecordCount = vendorProducts.TotalResults;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return vendorProducts;
        }

        //Get product List
        public virtual VendorListViewModel UnAssociatedProductList(FilterCollectionDataModel model, string vendorCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedProductList.ToString(), model);
            SetFilterforNotEqualVendor(model, vendorCode);

            VendorListViewModel vendorProducts = new VendorListViewModel();
            ProductDetailsListModel ProductList = _productClient.GetProducts(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.ProductImage);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Vendor);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Assortment);
            vendorProducts.GridModel = FilterHelpers.GetDynamicGridModel(model, ProductList?.XmlDataList ?? new List<dynamic>(), GridListType.AssociatedProductList.ToString(), string.Empty, null, true, true, null, CustomAttributeColumn(ProductList.AttributeColumnName));
            SetListPagingData(vendorProducts, ProductList);
            vendorProducts.VendorCode = vendorCode;
            vendorProducts.GridModel.TotalRecordCount = vendorProducts.TotalResults;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return vendorProducts;
        }

        //Associate vendor to product.
        public virtual bool AssociateVendorProduct(string vendorCode, string productIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            try
            {
                return !string.IsNullOrEmpty(productIds) ?
                    _vendorClient.AssociateAndUnAssociateProduct(GetVendorProductModel(AdminConstants.Vendor, vendorCode, productIds, false)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Associate vendor to product.
        public virtual bool UnAssociateVendorProduct(string vendorCode, string productIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            try
            {
                return !string.IsNullOrEmpty(productIds) ?
                    _vendorClient.AssociateAndUnAssociateProduct(GetVendorProductModel(AdminConstants.Vendor, vendorCode, productIds, true)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Check whether the vendor name already exists.
        public virtual bool CheckVendorNameExist(string vendorName, int vendorId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimVendorEnum.VendorCode.ToString(), FilterOperators.Is, vendorName));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Get the vendor List based on the vendor name filter.
            VendorListModel vendorList = _vendorClient.GetVendorList(null, filters, null, null, null);
            if (IsNotNull(vendorList) && IsNotNull(vendorList.Vendors))
            {
                if (vendorId > 0)
                {
                    //Set the status in case the vendor is open in edit mode.
                    VendorModel vendor = vendorList.Vendors.Find(x => x.PimVendorId == vendorId);
                    if (IsNotNull(vendor))
                        return !Equals(vendor.VendorCode, vendorName);
                }
                return vendorList.Vendors.Any(x => x.VendorCode == vendorName);
            }
            return false;
        }

        #endregion Associated Products

        #endregion Public  methods

        #region Private Methods

        //Set tool menu for catalog list grid view.
        private void SetVendorListToolMenu(VendorListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('VendorDeletePopup')", ControllerName = "Vendor", ActionName = "Delete" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "Vendor.prototype.ActiveInactiveVendor('True')" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextInactive, JSFunctionName = "Vendor.prototype.ActiveInactiveVendor('False')" });
            }
        }

        //Method to set default country on top in dropdown as per in Global setting.
        private static void SetDefaultCountry(CountryListModel countries)
        {
            int defaultCountryId = countries.Countries.FindIndex(x => x.CountryId == countries.Countries.FirstOrDefault(i => i.IsDefault == true)?.CountryId);
            if (defaultCountryId >= 0)
            {
                CountryModel defaultCountry = countries.Countries[defaultCountryId];
                countries.Countries[defaultCountryId] = countries.Countries[0];
                countries.Countries[0] = defaultCountry;
            }           
        }

        //Get vendor product model.
        private VendorProductModel GetVendorProductModel(string attributeCode, string attributeValue, string productIds, bool isUnAssociated)
        {
            return new VendorProductModel()
            {
                AttributeCode = attributeCode,
                AttributeValue = attributeValue,
                ProductIds = productIds,
                IsUnAssociated = isUnAssociated
            };
        }

        //Set tool menu for associated vendor product list.
        private void SetAssociatedVendorProductListToolMenu(VendorListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('VendorAssociatedPopup')" });
            }
        }

        //Set filter for equal clause.
        private void SetFilterforVendor(FilterCollectionDataModel model, string vendorCode)
        {
            if (!model.Filters.Exists(x => x.Item1 == AdminConstants.Vendor))
                model.Filters.Add(new FilterTuple(AdminConstants.Vendor, FilterOperators.Is, vendorCode));

            if (!model.Filters.Exists(x => x.Item1 == AdminConstants.IsCallForAttribute))
                model.Filters.Add(new FilterTuple(AdminConstants.IsCallForAttribute, FilterOperators.Equals, "True"));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = Filters });
        }

        //Set filter for not equal clause.
        private void SetFilterforNotEqualVendor(FilterCollectionDataModel model, string vendorCode)
        {
            //If null set it to new.
            if (IsNull(model.Filters))
                model.Filters = new FilterCollection();

            if (!model.Filters.Exists(x => x.Item1 == AdminConstants.Vendor))
                model.Filters.Add(new FilterTuple(AdminConstants.Vendor, FilterOperators.Is, vendorCode));
            if (!model.Filters.Exists(x => x.Item1 == FilterKeys.IsProductNotIn))
                model.Filters.Add(new FilterTuple(FilterKeys.IsProductNotIn, FilterOperators.Equals, "True"));
            if (!model.Filters.Exists(x => x.Item1 == AdminConstants.IsCallForAttribute))
                model.Filters.Add(new FilterTuple(AdminConstants.IsCallForAttribute, FilterOperators.Equals, "True"));

            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = Filters });
        }

        #endregion Private Methods
    }
}