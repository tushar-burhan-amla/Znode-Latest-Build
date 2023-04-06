using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public class BrandAgent : BaseAgent, IBrandAgent
    {
        #region Private Variables

        private readonly IBrandClient _brandClient;
        private readonly IProductsClient _productClient;
        private readonly ILocaleAgent _localeAgent;
        private readonly ISEOSettingClient _seoSettingClient;

        #endregion Private Variables

        #region Constructor

        public BrandAgent(IBrandClient brandClient, IProductsClient productsClient, ISEOSettingClient sEOSettingClient)
        {
            _brandClient = GetClient<IBrandClient>(brandClient);
            _productClient = GetClient<IProductsClient>(productsClient);
            _localeAgent = new LocaleAgent(GetClient<LocaleClient>());
            _seoSettingClient = GetClient<ISEOSettingClient>(sEOSettingClient); ;
        }

        #endregion Constructor

        #region Public Methods

        //Get Brand List
        public virtual BrandListViewModel GetBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int localeId = GetLocaleValue();
            //Checking For LocaleId already Exists in Filters Or Not

            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, expands = expands, sorts = sorts });
            BrandListModel brandList = _brandClient.GetBrandList(expands, filters, sorts, pageIndex, pageSize);
            BrandListViewModel listViewModel = new BrandListViewModel { Brands = brandList?.Brands?.ToViewModel<BrandViewModel>()?.ToList() };
            SetListPagingData(listViewModel, brandList);
            listViewModel.Locale = _localeAgent.GetLocalesList();

            //Set locale value as per selected from locale dropdown
            SetLocaleValueForDropdown(listViewModel.Locale, localeId.ToString());

            //Set tool menu for brand list grid view.
            SetBrandListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return listViewModel?.Brands?.Count > 0 ? listViewModel : new BrandListViewModel() { Brands = new List<BrandViewModel>(), Locale = _localeAgent.GetLocalesList() };
        }

        //Create brand.
        public virtual BrandViewModel CreateBrand(BrandViewModel brandModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (!CheckUniqueBrandCode(brandModel?.BrandCode))
                return _brandClient.CreateBrand(brandModel.ToModel<BrandModel>())?.ToViewModel<BrandViewModel>();
            else
            {
                brandModel.Locale = _localeAgent.GetLocalesList();
                brandModel.HasError = true;
                brandModel.ErrorMessage = string.Format(Admin_Resources.ErrorAlreadyExistsAttributeCode, brandModel.BrandCode);
                return brandModel;
            }
        }

        //Get brand by brandId.
        public virtual BrandViewModel GetBrand(int brandId, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (brandId > 0)
            {
                localeId = localeId > 0 ? localeId : GetLocaleValue();

                BrandViewModel brandViewModel = _brandClient.GetBrand(brandId, localeId)?.ToViewModel<BrandViewModel>();

                BrandListModel brandCodeList = _brandClient.GetBrandCodeList(AdminConstants.Brand);

                if (IsNull(brandViewModel))
                    brandViewModel = new BrandViewModel();

                brandViewModel.BrandCodeList = GetBrandCodeList(brandCodeList);

                brandViewModel.Locale = _localeAgent.GetLocalesList();

                brandViewModel.LocaleId = localeId;
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                return brandViewModel;
            }
            return new BrandViewModel();
        }

        //Get brand view model.
        public virtual BrandViewModel GetBrandViewModel()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            BrandViewModel brandViewModel = new BrandViewModel();
            BrandListModel brandCodeList = _brandClient.GetBrandCodeList(AdminConstants.Brand);
            brandViewModel.BrandCodeList = GetBrandCodeList(brandCodeList);
            brandViewModel.Locale = _localeAgent.GetLocalesList();
            brandViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);

            brandViewModel.BrandName = brandCodeList.BrandCodes?.FirstOrDefault()?.ValueLocales.FirstOrDefault(x => x.LocaleId == brandViewModel.LocaleId)?.DefaultAttributeValue;

            SetBrandDefaultLocaleValue();
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return brandViewModel;
        }

        //Update brand.
        public virtual BrandViewModel UpdateBrand(BrandViewModel viewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                BrandModel model = _brandClient.UpdateBrand(viewModel.ToModel<BrandModel>());
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                return !Equals(model, null) ? model.ToViewModel<BrandViewModel>() : new BrandViewModel() { HasError = true, ErrorMessage = PIM_Resources.UpdateErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return (BrandViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete brand.
        public virtual bool DeleteBrand(string brandId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            message = Admin_Resources.ErrorFailedToDelete;
            if (!string.IsNullOrEmpty(brandId))
            {
                try
                {
                    ZnodeLogging.LogMessage("Deleting Brand with ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info, new { Ids = brandId });

                    return _brandClient.DeleteBrand(new ParameterModel { Ids = brandId });
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

        //Get brand code list.
        public virtual List<SelectListItem> GetBrandCodeList(BrandListModel brandCodeList)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            List<SelectListItem> selectedBrandCodeList = new List<SelectListItem>();
            if (brandCodeList?.BrandCodes?.Count > 0)
            {
                brandCodeList.BrandCodes.ForEach(item =>
                {
                    selectedBrandCodeList.Add(new SelectListItem() { Text = item.AttributeDefaultValueCode, Value = item.AttributeDefaultValueCode });
                });
            }
            ZnodeLogging.LogMessage("selectedBrandCodeList list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, selectedBrandCodeList?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return selectedBrandCodeList;
        }

        //Check whether the brand name already exists.
        public virtual bool CheckBrandNameExist(string brandName, int brandId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeBrandDetailEnum.BrandCode.ToString(), FilterOperators.Is, brandName));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters });

            //Get the brand List based on the brand name filter.
            BrandListModel brandList = _brandClient.GetBrandList(null, filters, null, null, null);
            if (IsNotNull(brandList) && IsNotNull(brandList.Brands))
            {
                if (brandId > 0)
                {
                    //Set the status in case the brand is open in edit mode.
                    BrandModel brand = brandList.Brands.Find(x => x.BrandId == brandId);
                    if (IsNotNull(brand))
                        return !Equals(brand.BrandCode, brandName);
                }
                return brandList.Brands.Any(x => x.BrandCode == brandName);
            }
            return false;
        }

        //Check whether the brand friendly page seo url name already exists.
        public virtual bool CheckBrandSEOFriendlyPageNameExist(string seoFriendlyPageName, int seoDetailId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCMSSEODetailEnum.SEOUrl.ToString(), FilterOperators.Is, seoFriendlyPageName));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { Filters = filters });

            SEODetailsListModel seoDetailsListModel = _seoSettingClient.GetSeoDetails(null, filters, null, null, null);
            if (IsNotNull(seoDetailsListModel) && IsNotNull(seoDetailsListModel.SEODetailsList))
            {
                if (seoDetailId > 0)
                {
                    //Set the status in case the brand seo url is open in edit mode.
                    SEODetailsModel seoDetailsModel = seoDetailsListModel.SEODetailsList.Find(x => x.CMSSEODetailId == seoDetailId);
                    if (IsNotNull(seoDetailsModel?.SEOUrl))
                        return !Equals(seoDetailsModel.SEOUrl, seoFriendlyPageName);
                }
                return seoDetailsListModel.SEODetailsList.Any(x => Equals(x.SEOUrl, seoFriendlyPageName));
            }
            return false;
        }

        //Get product List
        public virtual BrandListViewModel AssociatedProductList(FilterCollectionDataModel model, int brandId, string brandCode, int localeId, string brandName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Checking For brand already Exists in Filters Or Not
            SetFilterforBrand(model, brandCode, localeId);
            BrandListViewModel brandProducts = new BrandListViewModel();
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedBrandProductList.ToString(), model);
            ZnodeLogging.LogMessage("Input parameters FilterCollectionDataModel having:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { Filters = model.Filters, sorts = model.SortCollection });

            ProductDetailsListModel ProductList = _productClient.GetProducts(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Brand);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.ProductImage);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Assortment);
            brandProducts.GridModel = FilterHelpers.GetDynamicGridModel(model, ProductList?.XmlDataList ?? new List<dynamic>(), GridListType.AssociatedBrandProductList.ToString(), string.Empty, null, true, true, null, CustomAttributeColumn(ProductList.AttributeColumnName));
            ProductList.ProductDetailList.ForEach(item => item.AttributeValue = brandCode);
            SetAssociatedBrandProductListToolMenu(brandProducts);
            SetListPagingData(brandProducts, ProductList);
            brandProducts.BrandCode = brandCode;
            brandProducts.BrandName = brandName;
            brandProducts.BrandId = brandId;
            brandProducts.LocaleId = localeId;
            brandProducts.GridModel.TotalRecordCount = brandProducts.TotalResults;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return brandProducts;
        }

        //Get product List
        public virtual BrandListViewModel UnAssociatedProductList(FilterCollectionDataModel model, string brandCode, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            SetFilterforNotEqualBrand(model, brandCode);

            BrandListViewModel brandProducts = new BrandListViewModel();
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedProductList.ToString(), model);
            if (IsNull(model.Filters))
                model.Filters = new FilterCollection();
            else
                model.Filters.RemoveAll(x => x.FilterName == FilterKeys.LocaleId);

            model.Filters.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString()));
            model.Filters.Add(new FilterTuple(FilterKeys.BrandCode, FilterOperators.Equals, brandCode.ToString()));
            ZnodeLogging.LogMessage("Input parameters FilterCollectionDataModel having:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { Filters = model.Filters, sorts = model.SortCollection });

            ProductDetailsListModel ProductList = _productClient.GetProducts(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.ProductImage);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Brand);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Assortment);
            brandProducts.GridModel = FilterHelpers.GetDynamicGridModel(model, ProductList?.XmlDataList ?? new List<dynamic>(), GridListType.AssociatedProductList.ToString(), string.Empty, null, true, true, null, CustomAttributeColumn(ProductList.AttributeColumnName));
            SetListPagingData(brandProducts, ProductList);
            brandProducts.BrandCode = brandCode;
            brandProducts.LocaleId = localeId;
            brandProducts.GridModel.TotalRecordCount = brandProducts.TotalResults;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return brandProducts;
        }

        //Associate brand to product.
        public virtual bool AssociateBrandProduct(string brandCode, string productIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                return !string.IsNullOrEmpty(productIds) ?
                    _brandClient.AssociateAndUnAssociateProduct(GetBrandProductModel(AdminConstants.Brand, brandCode, productIds, false)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Associate brand to product.
        public virtual bool UnAssociateBrandProduct(string brandCode, string productIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                return !string.IsNullOrEmpty(productIds) ?
                    _brandClient.AssociateAndUnAssociateProduct(GetBrandProductModel(AdminConstants.Brand, brandCode, productIds, true)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get LocaleId
        public virtual int GetLocaleValue()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            int localeId = 0;
            if (CookieHelper.IsCookieExists("_brandCulture"))
            {
                string cookieValue = CookieHelper.GetCookieValue<string>("_brandCulture");
                localeId = string.IsNullOrEmpty(cookieValue) ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : Convert.ToInt32(cookieValue);
            }
            else
            {
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                CookieHelper.SetCookie("_brandCulture", Convert.ToString(localeId));
            }
            ZnodeLogging.LogMessage("Output parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { localeId = localeId });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return localeId;
        }

        //Active/Inactive Brands
        public virtual bool ActiveInactiveBrand(string brandIds, bool isActive)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                return _brandClient.ActiveInactiveBrand(brandIds, isActive);
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

        //Get product List
        public virtual BrandListViewModel AssociatedStoreList(FilterCollectionDataModel model, int brandId, string brandCode, int localeId, string brandName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Checking For brand already Exists in Filters Or Not
            SetFilterforBrand(model, brandCode, localeId);
            BrandListViewModel brandStores = new BrandListViewModel();
            ZnodeLogging.LogMessage("Input parameters FilterCollectionDataModel having:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { Filters = model.Filters, sorts = model.SortCollection });

            PortalBrandListModel portalList = _brandClient.GetBrandPortalList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            brandStores.GridModel = FilterHelpers.GetDynamicGridModel(model, portalList?.PortalBrandModel, GridListType.ZnodeStorePortal.ToString(), string.Empty, null, true, true, null);
            SetAssociatedBrandStoreListToolMenu(brandStores);
            SetListPagingData(brandStores, portalList);
            brandStores.LocaleId = localeId;
            brandStores.BrandCode = brandCode;
            brandStores.BrandName = brandName;
            brandStores.BrandId = brandId;
            brandStores.GridModel.TotalRecordCount = brandStores.TotalResults;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return brandStores;
        }

        //Get product List
        public virtual BrandListViewModel UnAssociatedStoreList(FilterCollectionDataModel model, string brandCode, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            SetFilterForBrand(model, brandCode);

            BrandListViewModel brandPortals = new BrandListViewModel() { LocaleId = localeId, BrandCode = brandCode };

            if (IsNull(model.Filters))
                model.Filters = new FilterCollection();
            else
                model.Filters.RemoveAll(x => x.FilterName == FilterKeys.LocaleId);

            model.Filters.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString()));
            ZnodeLogging.LogMessage("Input parameters FilterCollectionDataModel having:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { Filters = model.Filters, sorts = model.SortCollection });
            PortalBrandListModel portalList = _brandClient.GetBrandPortalList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            brandPortals.GridModel = FilterHelpers.GetDynamicGridModel(model, portalList?.PortalBrandModel, GridListType.AssociateBrandPortal.ToString(), string.Empty, null, true, true, null, null);
            brandPortals.GridModel.TotalRecordCount = IsNull(portalList.PortalBrandModel) ? 0 : portalList.PortalBrandModel.Count;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return brandPortals;
        }

        //Associate brand to product.
        public virtual bool UnAssociateBrandPortal(int brandId, string portalIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                return !string.IsNullOrEmpty(portalIds) ?
                    _brandClient.AssociateAndUnAssociatePortal(GetBrandPortalModel(brandId, portalIds, true)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Associate brand to product.
        public virtual bool AssociateBrandPortal(int brandId, string portalIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                return !string.IsNullOrEmpty(portalIds) ?
                    _brandClient.AssociateAndUnAssociatePortal(GetBrandPortalModel(brandId, portalIds, false)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //To check unique brand code.
        public virtual bool CheckUniqueBrandCode(string code)
            => !string.IsNullOrEmpty(code) ? _brandClient.CheckUniqueBrandCode(code) : false;

        //Get Brand Name BY Brand Code
        public virtual string GetBrandName(string code, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            BrandListModel brandCodeList = _brandClient.GetBrandCodeList(AdminConstants.Brand);

            string defaultValueLocale = brandCodeList.BrandCodes?.FirstOrDefault(x => x.AttributeDefaultValueCode == code)?.ValueLocales.FirstOrDefault(x => x.LocaleId == localeId)?.DefaultAttributeValue;

            ZnodeLogging.LogMessage("Output Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { defaultValueLocale = defaultValueLocale });

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return defaultValueLocale;
        }
     
        //Associate/Unassociate Portal brands.
        public virtual bool AssociateAndUnAssociatePortalBrand(string brandIds, int portalId, bool isAssociated, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);         
            message = string.Empty;
            try
            {
                return !string.IsNullOrEmpty(brandIds) && _brandClient.AssociateAndUnAssociatePortalBrand(GetPortalBrandAssociationModel(portalId, brandIds, isAssociated));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);             
                message = isAssociated ? Admin_Resources.ErrorAssociateStoreBrand : Admin_Resources.ErrorUnAssociateStoreBrand;
                return false;               
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                message = isAssociated ? Admin_Resources.ErrorAssociateStoreBrand : Admin_Resources.ErrorUnAssociateStoreBrand;
                return false;
            }
        }

        //Get portal brand list.
        public virtual BrandListViewModel GetPortalBrandList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId, bool isAssociated)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.DisplayOrder, DynamicGridConstants.ASCKey);
            }
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, isAssociated = isAssociated });
            SetFilterforStoreBrandList(filters, portalId, isAssociated);

            BrandListModel brandList = _brandClient.GetPortalBrandList(null, filters, sorts, pageIndex, pageSize);
            BrandListViewModel listViewModel = new BrandListViewModel { Brands = brandList?.Brands?.ToViewModel<BrandViewModel>()?.ToList() };
            SetListPagingData(listViewModel, brandList);
            //Set tool menu for brand list grid view.
            if (isAssociated)
                SetPortalBrandListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return listViewModel?.Brands?.Count > 0 ? listViewModel : new BrandListViewModel() { Brands = new List<BrandViewModel>(), Locale = _localeAgent.GetLocalesList() };
        }

        //Update associated brand  data which is associated with Store.
        public virtual bool UpdateAssociatedPortalBrandDetail(string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            PortalBrandDetailViewModel portalBrandDetailViewModel = null;

            if (!string.IsNullOrEmpty(data))
                portalBrandDetailViewModel = JsonConvert.DeserializeObject<PortalBrandDetailViewModel[]>(data)[0];
            else
                return false;
            try
            {
                portalBrandDetailViewModel = _brandClient.UpdateAssociatedPortalBrandDetail(portalBrandDetailViewModel?.ToModel<PortalBrandDetailModel>())?.ToViewModel<PortalBrandDetailViewModel>();
                return (IsNotNull(portalBrandDetailViewModel) && !portalBrandDetailViewModel.HasError);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                portalBrandDetailViewModel.ErrorMessage = Admin_Resources.UpdateErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Price.ToString(), TraceLevel.Error);
                portalBrandDetailViewModel.ErrorMessage = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }

        #endregion Public Methods

        #region Private Methods

        //Set tool menu for brand list grid view.
        private void SetBrandListToolMenu(BrandListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('BrandDeletePopup')", ControllerName = "Brand", ActionName = "Delete" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "Brand.prototype.ActiveInactiveBrand('True')" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.TextInactive, JSFunctionName = "Brand.prototype.ActiveInactiveBrand('False')" });
            }
        }

        //Set tool menu for brand list grid view.
        private void SetPortalBrandListToolMenu(BrandListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('StoreBrandDeletePopup',this)", ControllerName = "Store", ActionName = "UnAssociatePortalBrand" });
            }
        }

        //Get brand product model.
        private BrandProductModel GetBrandProductModel(string attributeCode, string attributeValue, string productIds, bool isUnAssociated)
        {
            return new BrandProductModel()
            {
                AttributeCode = attributeCode,
                AttributeValue = attributeValue,
                ProductIds = productIds,
                IsUnAssociated = isUnAssociated
            };
        }

        //Get brand product model.
        private BrandPortalModel GetBrandPortalModel(int brandId, string productIds, bool isUnAssociated)
        {
            return new BrandPortalModel()
            {
                PortalIds = productIds,
                IsUnAssociated = isUnAssociated,
                BrandId = brandId
            };
        }
        //Set tool menu for associated brand product list.
        private void SetAssociatedBrandProductListToolMenu(BrandListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('BrandAssociatedPopup')" });
            }
        }

        //Set tool menu for associated brand store list.
        private void SetAssociatedBrandStoreListToolMenu(BrandListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('BrandPortalAssociatedPopup')" });
            }
        }

        //Set filter for equal clause.
        private void SetFilterforBrand(FilterCollectionDataModel model, string brandCode, int localeId)
        {
            //If null set it to new.
            if (IsNull(model.Filters))
                model.Filters = new FilterCollection();

            if (model.Filters.Exists(x => x.Item1 == AdminConstants.Brand))
            {
                model.Filters.RemoveAll(x => x.Item1 == AdminConstants.Brand);
                model.Filters.Insert(0, new FilterTuple(AdminConstants.Brand, FilterOperators.Is, brandCode));
            }
            else
                model.Filters.Insert(0, new FilterTuple(AdminConstants.Brand, FilterOperators.Is, brandCode));

            if (model.Filters.Exists(x => x.Item1 == FilterKeys.LocaleId))
                model.Filters.RemoveAll(x => x.FilterName == FilterKeys.LocaleId);
            model.Filters.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString()));

            if (!model.Filters.Exists(x => x.Item1 == AdminConstants.IsCallForAttribute))
                model.Filters.Add(new FilterTuple(AdminConstants.IsCallForAttribute, FilterOperators.Equals, "True"));

            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { Filters = model.Filters });

        }

        //Set filter for not equal clause.
        private void SetFilterforNotEqualBrand(FilterCollectionDataModel model, string brandCode)
        {
            //If null set it to new.
            if (IsNull(model.Filters))
                model.Filters = new FilterCollection();

            if (!model.Filters.Exists(x => x.Item1 == AdminConstants.Brand))
                model.Filters.Add(new FilterTuple(AdminConstants.Brand, FilterOperators.Is, brandCode));
            if (!model.Filters.Exists(x => x.Item1 == FilterKeys.IsProductNotIn))
                model.Filters.Add(new FilterTuple(FilterKeys.IsProductNotIn, FilterOperators.Equals, "True"));
            if (!model.Filters.Exists(x => x.Item1 == AdminConstants.IsCallForAttribute))
                model.Filters.Add(new FilterTuple(AdminConstants.IsCallForAttribute, FilterOperators.Equals, "true"));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { Filters = model.Filters });

        }

        //Set filter for not equal clause.
        private void SetFilterForBrand(FilterCollectionDataModel model, string brandCode)
        {
            //If null set it to new.
            if (IsNull(model.Filters))
                model.Filters = new FilterCollection();

            if (!model.Filters.Exists(x => x.Item1 == FilterKeys.BrandCode))
                model.Filters.Add(new FilterTuple(FilterKeys.BrandCode, FilterOperators.Is, brandCode));
            if (!model.Filters.Exists(x => x.Item1 == FilterKeys.IsAssociatedStore))
                model.Filters.Add(new FilterTuple(FilterKeys.IsAssociatedStore, FilterOperators.Equals, "0"));

            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { Filters = model.Filters });
        }

        //Set default locale value.
        private void SetBrandDefaultLocaleValue()
        {
            CookieHelper.SetCookie("_brandCulture", DefaultSettingHelper.DefaultLocale);
        }

        //Get AssociatedPortalBrandModel model.
        private PortalBrandAssociationModel GetPortalBrandAssociationModel(int portalId, string brandIds, bool isAssociated)
        {
            return new PortalBrandAssociationModel()
            {
                PortalId = portalId,
                IsAssociated = isAssociated,
                BrandIds = brandIds
            };
        }

        //Set the portal id field.
        private void SetFilterByValueAndName(FilterCollection filters, string filterName, string filterValue, string filterOperators)
        {
            if (IsNotNull(filters))
            {
                if (filters.Exists(x => string.Equals(x.Item1, filterName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    filters.RemoveAll(x => string.Equals(x.Item1, filterName, StringComparison.InvariantCultureIgnoreCase));
                    filters.Add(new FilterTuple(filterName, filterOperators, filterValue));
                }
                else
                {
                    filters.Add(new FilterTuple(filterName, filterOperators, filterValue));
                }
            }
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, filterName = filterName, filterValue = filterValue, filterOperators = filterOperators });
        }

        //Set Filter for store Brand List list.
        private void SetFilterforStoreBrandList(FilterCollection filters, int portalId, bool isAssociated)
        {
            SetFilterByValueAndName(filters, FilterKeys.PortalId, portalId.ToString(), FilterOperators.Equals);           
            SetFilterByValueAndName(filters, FilterKeys.IsAssociated, isAssociated.ToString(), FilterOperators.Equals);
            if (!isAssociated)
                SetFilterByValueAndName(filters, FilterKeys.IsActive, ZnodeConstant.TrueValue, FilterOperators.Equals);
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { filters = filters, portalId = portalId, isAssociated = isAssociated });
        }

        #endregion Private Methods
    }
}