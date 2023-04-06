using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Extensions;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System.Linq;
using System.Collections.Generic;
using Znode.Engine.Api.Client;
using System.Web.Mvc;
using Znode.Libraries.Resources;
using Znode.Engine.Exceptions;
using System;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.Maps;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class PromotionAgent : BaseAgent, IPromotionAgent
    {
        #region Private Variables
        private readonly IPromotionClient _promotionClient;
        private readonly IPromotionTypeClient _promotionTypeClient;
        private readonly IPortalClient _portalClient;
        private readonly IProfileClient _profileClient;
        private readonly IEcommerceCatalogClient _ecommerceCatalogClient;
        private readonly IPublishProductClient _publishProductClient;
        private readonly IPortalProfileClient _portalProfileClient;
        private readonly IPublishCategoryClient _categoryClient;
        private readonly IPublishCatalogClient _catelogClient;
        private const string CallForPrice = "Call For Pricing";
        #endregion

        #region Constructor
        public PromotionAgent(IPromotionClient promotionClient, IPromotionTypeClient promotionTypeClient, IPortalClient portalClient, IProfileClient profileClient, IEcommerceCatalogClient ecommerceCatalogClient,
            IPortalProfileClient portalProfileClient, IPublishProductClient publishProductClient, IPublishCategoryClient categoryClient, IPublishCatalogClient catelogClient)
        {
            _promotionClient = GetClient<IPromotionClient>(promotionClient);
            _promotionTypeClient = GetClient<IPromotionTypeClient>(promotionTypeClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _profileClient = GetClient<IProfileClient>(profileClient);
            _ecommerceCatalogClient = GetClient<IEcommerceCatalogClient>(ecommerceCatalogClient);
            _portalProfileClient = GetClient<IPortalProfileClient>(portalProfileClient);
            _publishProductClient = GetClient<IPublishProductClient>(publishProductClient);
            _categoryClient = GetClient<IPublishCategoryClient>(categoryClient);
            _catelogClient = GetClient<IPublishCatalogClient>(catelogClient);
        }
        #endregion

        #region Promotion

        //Get promotion by promotion Id.
        public virtual PromotionViewModel GetPromotionById(int promotionId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ExpandCollection expands = new ExpandCollection();
            expands = new ExpandCollection();
            expands.Add(ZnodePromotionEnum.ZnodePromotionType.ToString());
            ZnodeLogging.LogMessage("Input parameters of method GetPromotion: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { promotionId = promotionId, expands = expands });
            PromotionModel promotionModel = _promotionClient.GetPromotion(promotionId, expands);

            PromotionViewModel promotionViewModel = promotionModel?.ToViewModel<PromotionViewModel>();

            //In case of Call for price get promotion message in CallForPriceMessage field.
            if (Equals(promotionModel.PromotionType.Name, CallForPrice))
            {
                promotionViewModel.CallForPriceMessage = promotionModel.PromotionMessage;
                promotionViewModel.PromotionMessage = string.Empty;
            }
            promotionViewModel.ProfileIds = promotionViewModel?.ProfileIds?.Select(x => { x= IsNull(x) ? 0: x ; return x; })?.ToList();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return !Equals(promotionViewModel, null) ? promotionViewModel : new PromotionViewModel();
        }

        //Get list of promotions.
        public virtual PromotionListViewModel GetPromotionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetPromotionList(expands, filters, sorts, null, null);

        //Get list of promotions.
        public virtual PromotionListViewModel GetPromotionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isForExport = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            //set default sort of display order if not present.
            SetDisplayOrderSortIfNotPresent(ref sorts);

            if (IsNull(expands))
            {
                expands = new ExpandCollection();
                expands.Add(ZnodePromotionEnum.ZnodePromotionType.ToString());
            }
            ZnodeLogging.LogMessage("Input parameters of method GetPromotionList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            PromotionListModel promotionListModel = _promotionClient.GetPromotionList(expands, filters, sorts, pageIndex, pageSize);
            PromotionListViewModel listViewModel = null;
            if (!isForExport)
                listViewModel = new PromotionListViewModel { PromotionList = promotionListModel?.PromotionList?.ToViewModel<PromotionViewModel>().ToList() };
            else
                listViewModel = new PromotionListViewModel { PromotionExportList = promotionListModel?.PromotionList?.ToViewModel<PromotionExportViewModel>().ToList() };


            SetListPagingData(listViewModel, promotionListModel);

            //Set tool menu for Promotion list grid view.
            SetPromotionListToolMenu(listViewModel);
            if (!isForExport)
                return promotionListModel?.PromotionList?.Count > 0 ? listViewModel : new PromotionListViewModel() { PromotionList = new List<PromotionViewModel>() };
            else
                return promotionListModel?.PromotionList?.Count > 0 ? listViewModel : new PromotionListViewModel() { PromotionExportList = new List<PromotionExportViewModel>() };

        }


        //Create promotion.
        public virtual PromotionViewModel CreatePromotion(PromotionViewModel viewModel, BindDataModel bindDataModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                viewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                ZnodeLogging.LogMessage("LocaleId of PromotionViewModel: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { LocaleId = viewModel?.LocaleId });
                PromotionViewModel promotionViewModel = GetPromotionViewModel(viewModel, bindDataModel);
                if (!string.IsNullOrEmpty(promotionViewModel.CallForPriceMessage))
                    promotionViewModel.PromotionMessage = promotionViewModel.CallForPriceMessage;
                return _promotionClient.CreatePromotion(promotionViewModel.ToModel<PromotionModel>()).ToViewModel<PromotionViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning); 
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return new PromotionViewModel { HasError = true, ErrorMessage = Admin_Resources.AlreadyExistsCouponCode };
                    case ErrorCodes.CreationFailed:
                        return new PromotionViewModel { HasError = true, ErrorMessage = Admin_Resources.GenerateCouponCode };
                    default:
                        return new PromotionViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                return new PromotionViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
            }
        }

        //Get attribute Data
        public virtual PromotionViewModel GetPromotionViewModel(PromotionViewModel viewModel, BindDataModel bindDataModel)
        {
            PromotionViewModel promotionViewModel = new PromotionViewModel();
            promotionViewModel = viewModel;
            promotionViewModel.IsUnique = viewModel.PromotionId > 0 ? (Equals(Convert.ToBoolean(bindDataModel.GetValue("hdnIsUnique")), "") ? false : Convert.ToBoolean(bindDataModel.GetValue("hdnIsUnique"))) : viewModel.IsUnique;
            promotionViewModel.CouponList = GetCouponList(bindDataModel, viewModel);
            promotionViewModel.PromotionTypeName = Convert.ToString(bindDataModel.GetValue("PromotionTypeName"));
            promotionViewModel.PortalId = Convert.ToInt32(bindDataModel.GetValue("hdnPortalIds"));
            RemoveNonAttributeKeys(bindDataModel);
            promotionViewModel.QuantityMinimum = Convert.ToInt32(bindDataModel.GetValue("MinimumQuantity_0_0_0_0_attr"));
            promotionViewModel.OrderMinimum = Convert.ToDecimal(bindDataModel.GetValue("MinimumOrderAmount_0_0_0_0_attr"));
            promotionViewModel.Discount = Convert.ToDecimal(bindDataModel.GetValue("DiscountAmount_0_0_0_0_attr"));
            promotionViewModel.AssociatedCatelogIds = Convert.ToString(bindDataModel.GetValue("hdnPromotionCatalogs_attr"));
            promotionViewModel.AssociatedCategoryIds = Convert.ToString(bindDataModel.GetValue("hdnPromotionCategories_attr"));
            promotionViewModel.AssociatedProductIds = Convert.ToString(bindDataModel.GetValue("hdnPromotionProducts_attr"));
            promotionViewModel.AssociatedBrandIds = Convert.ToString(bindDataModel.GetValue("hdnPromotionBrands_attr"));
            promotionViewModel.AssociatedShippingIds = Convert.ToString(bindDataModel.GetValue("hdnPromotionShippings_attr"));
            promotionViewModel.ReferralPublishProductId = Convert.ToInt32((bindDataModel.GetValue("hdnReferalProductId_attr") == "") ? null : bindDataModel.GetValue("hdnReferalProductId_attr"));
            promotionViewModel.PromotionProductQuantity = Convert.ToDecimal(bindDataModel.GetValue("ProductQuantity_0_0_0_0_attr"));
            promotionViewModel.CallForPriceMessage = Convert.ToString(bindDataModel.GetValue("CallForPriceMessage_0_0_0_0_attr"));
            if (viewModel.PromotionId > 0 && IsNull(promotionViewModel.PromotionTypeId))
                promotionViewModel.PromotionTypeId = Convert.ToInt32(bindDataModel.GetValue("ddlPromotionType_attr").ToString().Split(',').FirstOrDefault());
            return promotionViewModel;
        }


        //Update promotion
        public virtual PromotionViewModel UpdatePromotion(PromotionViewModel viewModel, BindDataModel bindDataModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                viewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                ZnodeLogging.LogMessage("LocaleId of PromotionViewModel: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { LocaleId = viewModel?.LocaleId });
                PromotionViewModel promotionViewModel = GetPromotionViewModel(viewModel, bindDataModel);
                if (!string.IsNullOrEmpty(promotionViewModel.CallForPriceMessage))
                    promotionViewModel.PromotionMessage = promotionViewModel.CallForPriceMessage;
                PromotionModel promotionModel = _promotionClient.UpdatePromotion(promotionViewModel.ToModel<PromotionModel>());
                return IsNotNull(promotionModel) ? promotionModel.ToViewModel<PromotionViewModel>() : (PromotionViewModel)GetViewModelWithErrorMessage(new PromotionViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (PromotionViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.AlreadyExistsCouponCode);
                    case ErrorCodes.InternalItemNotUpdated:
                        return (PromotionViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.AlreadyExistsCouponCode);

                    default:
                        return (PromotionViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                return (PromotionViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete promotion by promotionId.
        public virtual bool DeletePromotion(string promotionId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorFailedToDelete;

            if (!string.IsNullOrEmpty(promotionId))
            {
                try
                {
                    return _promotionClient.DeletePromotion(new ParameterModel { Ids = promotionId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = Admin_Resources.ErrorDeletePromotion;
                            return false;
                        default:
                            errorMessage = Admin_Resources.ErrorFailedToDelete;
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Check whether the Promotion Name already exists.
        public virtual bool CheckPromotionCodeExist(string promoCode, int promotionId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ExpandCollection expands = new ExpandCollection();
            expands = new ExpandCollection();
            expands.Add(ZnodePromotionEnum.ZnodePromotionType.ToString());

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePromotionEnum.PromoCode.ToString(), FilterOperators.Is, promoCode));
            ZnodeLogging.LogMessage("Input parameters of method GetPromotionList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters });
            //Get the Promotion List based on the promotion name filter.
            PromotionListModel promotionModelList = _promotionClient.GetPromotionList(expands, filters, null, null, null);
            if (IsNotNull(promotionModelList) && IsNotNull(promotionModelList.PromotionList))
            {
                if (promotionId > 0)
                {
                    //Set the status in case the promotion is open in edit mode.
                    PromotionModel promotion = promotionModelList.PromotionList.Find(x => x.PromotionId == promotionId);
                    if (IsNotNull(promotion))
                        return !Equals(promotion.PromoCode.ToLower(), promoCode.ToLower());
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                return promotionModelList.PromotionList.Any(x => x.PromoCode.ToLower() == promoCode.ToLower());
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return false;
        }

        //Check whether the coupon code already exists.
        public virtual bool CheckCouponCodeExist(string code, int promotionCouponId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { code = code, promotionCouponId = promotionCouponId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePromotionCouponEnum.Code.ToString(), FilterOperators.Contains, code));
            if (promotionCouponId > 0)
                filters.Add(new FilterTuple(ZnodePromotionCouponEnum.PromotionId.ToString(), FilterOperators.NotIn, promotionCouponId.ToString()));
            ZnodeLogging.LogMessage("Input parameter of method GetCouponList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Get the Promotion List based on the promotion name filter.
            CouponListModel couponModelList = _promotionClient.GetCouponList(null, filters, null, null, null);
            if (IsNotNull(couponModelList) && IsNotNull(couponModelList.CouponList))
            {
                if (promotionCouponId > 0)
                {
                    //Set the status in case the promotion is open in edit mode.
                    CouponModel coupon = couponModelList.CouponList.Find(x => x.PromotionCouponId == promotionCouponId);
                    if (IsNotNull(coupon))
                        return !Equals(coupon.Code, code);
                }
                return couponModelList.CouponList.Any(x => x.Code == code);
            }
            return false;
        }

        public virtual void BindDropdownValues(PromotionViewModel promotionViewModel)
        {
            promotionViewModel.PromotionTypeList = GetPromotionTypes();
            promotionViewModel.ProfileList = promotionViewModel.PortalId == 0 ? GetProfileList() : ProfileListByStorId(promotionViewModel.PortalId);
            promotionViewModel.CatalogList = GetPublishedCatalogList(promotionViewModel.PortalId);
            promotionViewModel.CategoryList = GetPublishedCategoryList(promotionViewModel.PortalId);
            if(IsNull(promotionViewModel.ProfileIds))
                promotionViewModel.ProfileIds = new List<int?>();
        }

        //Get Promotion types list.
        private List<SelectListItem> GetPromotionTypes()
        {
            SortCollection sorts = new SortCollection();
            sorts.Add(SortKeys.Name, DynamicGridConstants.ASCKey);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePromotionTypeEnum.IsActive.ToString(), FilterOperators.Equals, "true"));
            ZnodeLogging.LogMessage("Input parameter of method GetPromotionTypeList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            PromotionTypeListModel promotionTypeList = _promotionTypeClient.GetPromotionTypeList(filters, sorts, null, null);
            List<SelectListItem> selectedPromotionTypeList = new List<SelectListItem>();

            if (promotionTypeList?.PromotionTypes?.Count > 0)
                promotionTypeList.PromotionTypes.ToList().ForEach(item => { selectedPromotionTypeList.Add(new SelectListItem() { Text = item.Name, Value = item.PromotionTypeId.ToString() }); });

            return selectedPromotionTypeList;
        }

        //Get Profile List
        private List<SelectListItem> GetProfileList()
        {
            List<SelectListItem> selectedProfileList = new List<SelectListItem>();
            getPortalProfileListForAllPortal(selectedProfileList);
            return selectedProfileList;
        }

        //Get Published Catalog List
        public virtual List<SelectListItem> GetPublishedCatalogList(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { portalId = portalId });
            PortalCatalogListModel catalogList = _ecommerceCatalogClient.GetAssociatedPortalCatalog(new ParameterModel { Ids = portalId.ToString() });

            List<SelectListItem> selectedCatalogList = new List<SelectListItem>();
            if (portalId > 0 && catalogList?.PortalCatalogs?.Count > 0)
                catalogList?.PortalCatalogs?.ForEach(item => { selectedCatalogList.Add(new SelectListItem() { Text = item.CatalogName, Value = item.PublishCatalogId.ToString() }); });

            selectedCatalogList.Insert(0, new SelectListItem() { Value = AdminConstants.Zero.ToString(), Text = Admin_Resources.LabelAllCatalog });
            ZnodeLogging.LogMessage("selectedCatalogList count: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { selectedCatalogListCount = selectedCatalogList?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return selectedCatalogList;
        }

        //Get Published Category List
        public virtual List<SelectListItem> GetPublishedCategoryList(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { portalId = portalId });
            List<SelectListItem> selectedCategoryList = new List<SelectListItem>();

            List<int> publishedCatalogIds = _ecommerceCatalogClient.GetAssociatedPortalCatalog(new ParameterModel { Ids = portalId.ToString() })?.PortalCatalogs?.Select(a => a.PublishCatalogId)?.Distinct()?.ToList();
            if (publishedCatalogIds?.Count > 0)
            {
                string filterCatalogIds = string.Join(",", publishedCatalogIds);
                CategoryListModel categoryList = _promotionClient.GetPublishedCategories(new ParameterModel { Ids = filterCatalogIds });

                if (categoryList?.Categories?.Count > 0)
                    categoryList.Categories.ToList().ForEach(item => { selectedCategoryList.Add(new SelectListItem() { Text = item.CategoryName, Value = item.PublishCategoryId.ToString() }); });
            }

            selectedCategoryList.Insert(0, new SelectListItem() { Value = AdminConstants.Zero.ToString(), Text = Admin_Resources.LabelAllCategory });
            ZnodeLogging.LogMessage("selectedCategoryList count: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { selectedCategoryListCount = selectedCategoryList?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return selectedCategoryList;
        }

        //Get Published Product List
        public virtual PublishProductsListViewModel GetPublishedProductList(int portalId, int promotionId, string assignedIds, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            PublishProductsListViewModel publishedProductList = new PublishProductsListViewModel();
            ManageFilters(portalId, promotionId, filters);
            filters.Add(new FilterTuple(FilterKeys.RevisionType, FilterOperators.Is, ZnodePublishStatesEnum.PRODUCTION.ToString()));
            //Checking For PromotionId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString()))
                filters.RemoveAll(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString());

            //Remove old filters for Attribute Code and value.
            filters.RemoveAll(x => x.FilterName == FilterKeys.AttributeCodeForPromotion);

            PublishProductListModel productList = new PublishProductListModel();
            assignedIds = !string.IsNullOrEmpty(assignedIds) ? assignedIds : "0";
            
            ZnodeLogging.LogMessage("Input parameters of method GetUnAssignedPublishProductList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            if (promotionId <= 0 && assignedIds != "0")
                //Get un-assigned product list when no promotion created.
                productList = _publishProductClient.GetUnAssignedPublishProductList(new ParameterModel { Ids = assignedIds }, null, filters, sorts, pageIndex, recordPerPage);
            else if (promotionId > 0)
            {
                PromotionModel promotionModel = new PromotionModel();
                promotionModel.AssociatedProductIds = !string.IsNullOrEmpty(assignedIds) ? assignedIds : "0";
                promotionModel.PortalId = portalId;
                promotionModel.PromotionId = promotionId;
                //Get un-assigned product list when promotion created.
                productList = _promotionClient.GetAssociatedUnAssociatedProductList(promotionModel, false, null, filters, sorts, pageIndex, recordPerPage);
            }
            else
                productList = _publishProductClient.GetPublishProductList(null, filters, sorts, pageIndex, recordPerPage);
            if (filters.Exists(x => x.Item1 == FilterKeys.RevisionType))
                filters.RemoveAll(x => x.Item1 == FilterKeys.RevisionType);
            if (IsNotNull(productList?.PublishProducts))
            {
                publishedProductList = new PublishProductsListViewModel { PublishProductsList = productList?.PublishProducts?.ToViewModel<PublishProductsViewModel>().ToList() };
                publishedProductList.PublishProductsList.ForEach(i => { i.Image = i.ImageThumbNailPath; });
            }
            SetListPagingData(publishedProductList, productList);

            publishedProductList.portalId = portalId;
            publishedProductList.ProductIds = assignedIds;
            publishedProductList.PromotionId = promotionId;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return publishedProductList;
        }

        //Get Published Category List
        public virtual CategoryListViewModel GetPublishedCategoryList(int portalId, int promotionId, string assignedIds, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            CategoryListViewModel publishedCategoryList = new CategoryListViewModel();
            
            ManageFilters(portalId, promotionId, filters);
            filters.Add(new FilterTuple(FilterKeys.RevisionType, FilterOperators.Is, ZnodePublishStatesEnum.PRODUCTION.ToString()));
            //Checking For PromotionId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString()))
                filters.RemoveAll(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString());

            PublishCategoryListModel categoryList = new PublishCategoryListModel();
            assignedIds = !string.IsNullOrEmpty(assignedIds) ? assignedIds : "0";
            ZnodeLogging.LogMessage("Input parameters of method GetUnAssignedPublishCategoryList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            if (promotionId <= 0 && assignedIds != "0")
                //Get un-assigned category list when no promotion created.
                categoryList = _categoryClient.GetUnAssignedPublishCategoryList(assignedIds, null, filters, sorts, pageIndex, recordPerPage);
            else if (promotionId > 0)
            {
                PromotionModel promotionModel = new PromotionModel();
                promotionModel.AssociatedCategoryIds = !string.IsNullOrEmpty(assignedIds) ? assignedIds : "0";
                promotionModel.PortalId = portalId;
                promotionModel.PromotionId = promotionId;
                //Get un-assigned category list when promotion created.
                categoryList = _promotionClient.GetAssociatedUnAssociatedCategoryList(promotionModel, false, null, filters, sorts, pageIndex, recordPerPage);
            }
            else
                categoryList = _categoryClient.GetPublishCategoryList(null, filters, sorts, pageIndex, recordPerPage);
            if (filters.Exists(x => x.Item1 == FilterKeys.RevisionType))
                filters.RemoveAll(x => x.Item1 ==FilterKeys.RevisionType);
            if (IsNotNull(categoryList?.PublishCategories))
                publishedCategoryList = new CategoryListViewModel { Categories = categoryList?.PublishCategories?.ToViewModel<CategoryViewModel>().ToList() };
            
            SetListPagingData(publishedCategoryList, categoryList);

            publishedCategoryList.PortalId = portalId;
            publishedCategoryList.CategoryIds = assignedIds;
            publishedCategoryList.PromotionId = promotionId;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return publishedCategoryList;
        }

        //Get Published Catelog List
        public virtual PortalCatalogListViewModel GetPublishedCatelogList(int portalId, int promotionId, string assignedIds, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            PortalCatalogListViewModel publishedCatelogList = new PortalCatalogListViewModel();

            ManageFilters(portalId, promotionId, filters);

            filters.Add(new FilterTuple(FilterKeys.RevisionType, FilterOperators.Is, ZnodePublishStatesEnum.PRODUCTION.ToString()));
            //Checking For PromotionId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString()))
                filters.RemoveAll(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString());

           // recordPerPage=2000;

             PublishCatalogListModel catelogList = new PublishCatalogListModel();

            assignedIds = !string.IsNullOrEmpty(assignedIds) ? assignedIds : "0";
            ZnodeLogging.LogMessage("Input parameters of method GetUnAssignedPublishCatelogList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { assignedIds = assignedIds, filters = filters, sorts = sorts });
            if (promotionId <= 0 && assignedIds != "0")
                //Get un-assigned category list when no promotion created.
                catelogList = _catelogClient.GetUnAssignedPublishCatelogList(assignedIds, null, filters, sorts, pageIndex, recordPerPage);
            else if (promotionId > 0)
            {
                PromotionModel promotionModel = new PromotionModel();
                promotionModel.AssociatedCatelogIds = !string.IsNullOrEmpty(assignedIds) ? assignedIds : "0";
                promotionModel.PortalId = portalId;
                promotionModel.PromotionId = promotionId;
                //Get un-assigned category list when promotion created.
                catelogList = _promotionClient.GetAssociatedUnAssociatedCatalogList(promotionModel, false, null, filters, sorts, pageIndex, recordPerPage);
            }
            else
                catelogList = _catelogClient.GetPublishCatalogList(null, filters, sorts, pageIndex, recordPerPage);

            if (IsNotNull(catelogList?.PublishCatalogs))
                publishedCatelogList = new PortalCatalogListViewModel { PortalCatalogs = catelogList?.PublishCatalogs?.ToViewModel<PortalCatalogViewModel>().ToList() };

            SetListPagingData(publishedCatelogList, catelogList);

            publishedCatelogList.PortalId = portalId;
            publishedCatelogList.PromotionId = promotionId;
            publishedCatelogList.CatelogIds = assignedIds;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return publishedCatelogList;
        }

        public virtual List<SelectListItem> ProfileListByStorId(int storeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            List<SelectListItem> selectedPortalProfileList = new List<SelectListItem>();
            if (Equals(storeId, 0))
            {
                //Get profile list for all portals selected value.
                getPortalProfileListForAllPortal(selectedPortalProfileList);
            }
            else
            {
                //Get profile list for by store ID.
                getPortalProfileListByPortalId(storeId, selectedPortalProfileList);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return selectedPortalProfileList;
        }

        public virtual List<SelectListItem> CatalogListByStorId(int storeId)
        {
            List<SelectListItem> selectedCategoryList = new List<SelectListItem>();
            PortalCatalogListModel portalCatalogListModel = _ecommerceCatalogClient.GetAssociatedPortalCatalog(new ParameterModel { Ids = storeId.ToString() });

            if (portalCatalogListModel?.PortalCatalogs?.Count > 0)
                portalCatalogListModel?.PortalCatalogs.ToList().ForEach(item => { selectedCategoryList.Add(new SelectListItem() { Text = item.CatalogName, Value = item.PublishCatalogId.ToString() }); });

            return selectedCategoryList;
        }

        //Get associated UnAssociated Catelog List
        public virtual PortalCatalogListViewModel GetAssociatedUnAssociatedCatelogList(int portalId, string catalogIds, int promotionId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            PortalCatalogListViewModel publishedCatelogList = new PortalCatalogListViewModel();

            ManageFilters(portalId, promotionId, filters);
            //Checking For PromotionId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString()))
                filters.RemoveAll(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString());
            PromotionModel promotionModel = new PromotionModel();
            promotionModel.AssociatedCatelogIds = !string.IsNullOrEmpty(catalogIds) ? catalogIds : "0";
            promotionModel.PortalId = portalId;
            promotionModel.PromotionId = promotionId;

            PublishCatalogListModel catelogList = new PublishCatalogListModel();
            bool isAssociatedCatalog = (promotionId > 0||portalId<=0) ? true : false;
            ZnodeLogging.LogMessage("Input parameters of method GetAssociatedUnAssociatedCatalogList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            catelogList = _promotionClient.GetAssociatedUnAssociatedCatalogList(promotionModel, isAssociatedCatalog, null, filters, sorts, pageIndex, recordPerPage);

            if (IsNotNull(catelogList.PublishCatalogs))
                publishedCatelogList = new PortalCatalogListViewModel { PortalCatalogs = catelogList?.PublishCatalogs?.ToViewModel<PortalCatalogViewModel>().ToList() };

            SetListPagingData(publishedCatelogList, catelogList);

            publishedCatelogList.PortalId = portalId;
            publishedCatelogList.CatelogIds = catalogIds;
            publishedCatelogList.PromotionId = promotionId;
            //Set tool menu for Catalog list grid view.
            SetAssociatedCatalogListToolMenu(publishedCatelogList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return publishedCatelogList;
        }

        public virtual bool AssociateCatalogToPromotion(int storeId, string associatedCatelogIds, int promotionId)
           => _promotionClient.AssociateCatalogToPromotion(storeId, associatedCatelogIds, promotionId);

        //Get associated UnAssociated category List
        public virtual CategoryListViewModel GetAssociatedUnAssociatedCategoryList(int portalId, string categoryIds, int promotionId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            CategoryListViewModel publishedCategoryList = new CategoryListViewModel();

            ManageFilters(portalId, promotionId, filters);
            filters.Add(new FilterTuple(FilterKeys.RevisionType, FilterOperators.Is, ZnodePublishStatesEnum.PRODUCTION.ToString()));
            PromotionModel promotionModel = new PromotionModel();
            promotionModel.AssociatedCategoryIds = !string.IsNullOrEmpty(categoryIds) ? categoryIds : "0";
            promotionModel.PortalId = portalId;
            promotionModel.PromotionId = promotionId;
            PublishCategoryListModel categoryList = new PublishCategoryListModel();
            bool isAssociatedCategory = promotionId > 0 ? true : false;
            ZnodeLogging.LogMessage("Input parameters of method GetAssociatedUnAssociatedCategoryList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            categoryList = _promotionClient.GetAssociatedUnAssociatedCategoryList(promotionModel, isAssociatedCategory, null, filters, sorts, pageIndex, recordPerPage);
            if (filters.Exists(x => x.Item1 == FilterKeys.RevisionType))
                filters.RemoveAll(x => x.Item1 == FilterKeys.RevisionType);
            if (IsNotNull(categoryList?.PublishCategories))
                publishedCategoryList = new CategoryListViewModel { Categories = categoryList?.PublishCategories?.ToViewModel<CategoryViewModel>().ToList() };

            SetListPagingData(publishedCategoryList, categoryList);

            publishedCategoryList.PortalId = portalId;
            publishedCategoryList.CategoryIds = categoryIds;
            publishedCategoryList.PromotionId = promotionId;
            //Set tool menu for Category list grid view.
            SetAssociatedCategoryListToolMenu(publishedCategoryList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return publishedCategoryList;
        }

        //Associate category to already created promotion.
        public virtual bool AssociateCategoryToPromotion(int storeId, string associateCategoryIds, int promotionId)
         => _promotionClient.AssociateCategoryToPromotion(storeId, associateCategoryIds, promotionId);

        //Get associated UnAssociated product List
        public virtual PublishProductsListViewModel GetAssociatedUnAssociatedProductList(int portalId, string productIds, int promotionId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ManageFilters(portalId, promotionId, filters);
            filters.Add(new FilterTuple(FilterKeys.RevisionType, FilterOperators.Is, ZnodePublishStatesEnum.PRODUCTION.ToString()));
            //Checking For PromotionId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString()))
                filters.RemoveAll(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString());
            PublishProductsListViewModel publishedProductList = new PublishProductsListViewModel();

            PromotionModel promotionModel = new PromotionModel();
            promotionModel.AssociatedProductIds = !string.IsNullOrEmpty(productIds) ? productIds : "0";
            promotionModel.PortalId = portalId;
            promotionModel.PromotionId = promotionId;

            PublishProductListModel productList = new PublishProductListModel();

            bool isAssociatedProduct = (promotionId > 0 || portalId<=0) ? true : false;
            ZnodeLogging.LogMessage("Input parameters of method GetAssociatedUnAssociatedProductLists: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            productList = _promotionClient.GetAssociatedUnAssociatedProductList(promotionModel, isAssociatedProduct, null, filters, sorts, pageIndex, recordPerPage);
            if (filters.Exists(x => x.Item1 == FilterKeys.RevisionType))
                filters.RemoveAll(x => x.Item1 == FilterKeys.RevisionType);
            if (IsNotNull(productList?.PublishProducts))
                publishedProductList = new PublishProductsListViewModel { PublishProductsList = productList?.PublishProducts?.ToViewModel<PublishProductsViewModel>().ToList() };

            SetListPagingData(publishedProductList, productList);

            publishedProductList.portalId = portalId;
            publishedProductList.ProductIds = productIds;
            publishedProductList.PromotionId = promotionId;
            //Set tool menu for product list grid view.
            SetAssociatedProductListToolMenu(publishedProductList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return publishedProductList;
        }

        public virtual void ManageFilters(int portalId, int promotionId, FilterCollection filters)
        {
            //Remove old filters for Category, PromotionId and locale.
            filters.RemoveAll(x => x.FilterName == FilterKeys.LocaleId || x.FilterName == FilterKeys.ZnodeCategoryIds || x.FilterName == FilterKeys.PromotionId || x.FilterName == FilterKeys.PublishCategoryId || x.FilterName == FilterKeys.ZnodeCatalogId);

            //Add promotion filter
            filters.Add(new FilterTuple(FilterKeys.PromotionId, FilterOperators.Equals, promotionId.ToString()));

            //Get portal
            LocaleListModel portalLocaleList = _portalClient.GetLocaleList(null, new FilterCollection() { new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()) }, null, null, null);
            //Set locale filter            
            int? localeId = portalLocaleList.Locales?.Find(x => x.IsDefault)?.LocaleId;
            ZnodeLogging.LogMessage("localeId: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { localeId = localeId });
            filters.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId?.ToString() ?? (portalLocaleList.Locales?.Count > 0 ? portalLocaleList.Locales.FirstOrDefault().LocaleId.ToString() : DefaultSettingHelper.DefaultLocale)));

            //Get catalog Id by portal Id.
            if (portalId > 0)
            {
                int? catalogId = _portalClient.GetPortal(portalId, null)?.PublishCatalogId;
                //Set catalog filter.
                if (catalogId.GetValueOrDefault() > 0)
                    filters.Add(new FilterTuple(FilterKeys.ZnodeCatalogId, FilterOperators.Equals, catalogId.ToString()));
            }
            ZnodeLogging.LogMessage("filters generated: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters});
        }

        //Associate Products to already created promotion.
        public virtual bool AssociateProductToPromotion(int storeId, string associatedProductIds, int promotionId, string discountTypeName)
          => _promotionClient.AssociateProductToPromotion(storeId, associatedProductIds, promotionId, discountTypeName);

        //Removes a product type association entry from promotion.
        public virtual bool UnAssociateProduct(string publishProductIds, int promotionId) => _promotionClient.UnAssociateProduct(new ParameterModel { Ids = publishProductIds }, promotionId);

        //Removes a Category type association entry from promotion.
        public virtual bool UnAssociateCategory(string publishCategoryIds, int promotionId) => _promotionClient.UnAssociateCategory(new ParameterModel { Ids = publishCategoryIds }, promotionId);

        //Removes a Catalog type association entry from promotion.
        public virtual bool UnAssociateCatalog(string publishCatalogIds, int promotionId) => _promotionClient.UnAssociateCatalog(new ParameterModel { Ids = publishCatalogIds }, promotionId);

        //Get the list of all stores.
        public virtual StoreListViewModel GetStoreList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = default(int?), int? pageSize = default(int?))
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            PortalListModel storeList = _portalClient.GetPortalList(null, filters, sorts, pageIndex, pageSize);
            StoreListViewModel storeListViewModel = new StoreListViewModel { StoreList = storeList?.PortalList?.ToViewModel<StoreViewModel>().ToList() };

            //Set paging data.
            SetListPagingData(storeListViewModel, storeList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return storeListViewModel;
        }

        //Show all stores checkbox or not
        public bool ShowAllStoreCheckbox() => SessionProxyHelper.IsAdminUser();

        #region Brand 

        //Get associated UnAssociated Brand List
        public virtual BrandListViewModel GetAssociatedUnAssociatedBrandList(string brandIds, int promotionId, bool isAssociatedBrand, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            //Filter to get Active brand list.
            if (HelperUtility.IsNotNull(filters))
                filters.Add(ZnodeBrandDetailEnum.IsActive.ToString(), FilterOperators.Is, ZnodeConstant.TrueValue);
            else
            {
                filters = new FilterCollection();
                filters.Add(ZnodeBrandDetailEnum.IsActive.ToString(), FilterOperators.Is, ZnodeConstant.TrueValue);
            }

            PromotionModel promotionModel = new PromotionModel();
            promotionModel.AssociatedBrandIds = !string.IsNullOrEmpty(brandIds) ? brandIds : "0";
            promotionModel.PromotionId = promotionId;

            BrandListModel brandList = new BrandListModel();
            ZnodeLogging.LogMessage("Input parameters of method GetAssociatedUnAssociatedBrandList ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            brandList = _promotionClient.GetAssociatedUnAssociatedBrandList(promotionModel, isAssociatedBrand, null, filters, sorts, pageIndex, recordPerPage);

            BrandListViewModel brandListViewModel = brandList?.Brands?.Count > 0 ? new BrandListViewModel { Brands = brandList?.Brands?.ToViewModel<BrandViewModel>().ToList() } : new BrandListViewModel() { Brands = new List<BrandViewModel>() };

            SetListPagingData(brandListViewModel, brandList);

            brandListViewModel.BrandIds = brandIds;
            brandListViewModel.PromotionId = promotionId;
            //Set tool menu for Brand list grid view.
            SetAssociatedBrandListToolMenu(brandListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return brandListViewModel;
        }

        //Associate Brand to already created promotion.
        public virtual bool AssociateBrandToPromotion(string associatedBrandIds, int promotionId)
          => _promotionClient.AssociateBrandToPromotion(associatedBrandIds, promotionId);

        //Removes a Brand type association entry from promotion.
        public virtual bool UnAssociateBrand(string associatedBrandIds, int promotionId) => _promotionClient.UnAssociateBrand(new ParameterModel { Ids = associatedBrandIds }, promotionId);

        #endregion

        #region Shipping 
        //Get associated UnAssociated Shipping List
        public virtual ShippingListViewModel GetAssociatedUnAssociatedShippingList(int portalId, string shippingIds, int promotionId, bool isAssociatedShipping, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            PromotionModel promotionModel = new PromotionModel();
            promotionModel.AssociatedShippingIds = !string.IsNullOrEmpty(shippingIds) ? shippingIds : "0";
            promotionModel.PromotionId = promotionId;
            promotionModel.PortalId = portalId;

            ShippingListModel ShippingList = new ShippingListModel();
            ZnodeLogging.LogMessage("Input parameters filters and sorts of method GetAssociatedUnAssociatedShippingList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            ShippingList = _promotionClient.GetAssociatedUnAssociatedShippingList(promotionModel, isAssociatedShipping, null, filters, sorts, pageIndex, recordPerPage);

            ShippingListViewModel ShippingListViewModel = ShippingList?.ShippingList?.Count > 0 ? new ShippingListViewModel { ShippingList = ShippingList?.ShippingList?.ToViewModel<ShippingViewModel>().ToList() } : new ShippingListViewModel() { ShippingList = new List<ShippingViewModel>() };

            SetListPagingData(ShippingListViewModel, ShippingList);

            ShippingListViewModel.ShippingIds = shippingIds;
            ShippingListViewModel.PromotionId = promotionId;
            ShippingListViewModel.PortalId = portalId;
            //Set tool menu for Shipping list grid view.
            if (isAssociatedShipping)
                SetAssociatedShippingListToolMenu(ShippingListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return ShippingListViewModel;
        }

        //Associate Shipping to already created promotion.
        public virtual bool AssociateShippingToPromotion(string associatedShippingIds, int promotionId)
          => _promotionClient.AssociateShippingToPromotion(associatedShippingIds, promotionId);

        //Removes a Shipping type association entry from promotion.
        public virtual bool UnAssociateShipping(string associatedShippingIds, int promotionId) => _promotionClient.UnAssociateShipping(new ParameterModel { Ids = associatedShippingIds }, promotionId);
        #endregion
        #endregion

        #region Coupon

        //Get coupon by promotion Id.
        public virtual CouponViewModel GetCoupon(int promotionId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(FilterKeys.PromotionId, FilterOperators.Equals, promotionId.ToString());
            CouponModel couponModel = _promotionClient.GetCoupon(filters);
            return !Equals(couponModel, null) ? couponModel.ToViewModel<CouponViewModel>() : new CouponViewModel();
        }

        //Get list of coupon
        public virtual CouponListViewModel GetCouponList(int promotionId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isForExport = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            if (IsNull(expands))
            {
                expands = new ExpandCollection();
                expands.Add(ZnodePromotionCouponEnum.ZnodePromotion.ToString());
            }
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.CreatedDate, DynamicGridConstants.DESCKey);
            }
            //Checking For PromotionId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString()))
                filters.RemoveAll(x => x.Item1 == ZnodePromotionProductEnum.PromotionId.ToString());

            filters.Add(FilterKeys.PromotionId, FilterOperators.Equals, promotionId.ToString());
            ZnodeLogging.LogMessage("Input parameters of method GetCouponList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            CouponListModel couponListModel = _promotionClient.GetCouponList(expands, filters, sorts, pageIndex, pageSize);
            CouponListViewModel listViewModel = null;

            if (!isForExport)
                listViewModel = new CouponListViewModel { CouponList = couponListModel?.CouponList?.ToViewModel<CouponViewModel>().ToList() };
            else
                listViewModel = new CouponListViewModel { CouponExportList = couponListModel?.CouponList?.ToViewModel<CouponExportViewModel>().ToList() };

            SetListPagingData(listViewModel, couponListModel);

            //Set tool menu for coupon list grid view.
            SetCouponListToolMenu(listViewModel);

            if (!isForExport)
                return couponListModel?.CouponList?.Count > 0 ? listViewModel : new CouponListViewModel() { CouponList = new List<CouponViewModel>() };
            else
                return couponListModel?.CouponList?.Count > 0 ? listViewModel : new CouponListViewModel() { CouponExportList = new List<CouponExportViewModel>() };
        }

        //Get promotion attribute on changing discount type
        public virtual PIMFamilyDetailsViewModel GetPromotionAttribute(string discountName)
        => ProviderEngineViewModelMap.ToPIMFamilyDetailsViewModel(_promotionClient.GetPromotionAttribute(discountName));
        #endregion

        #region Private Method
        //Set tool menu for Promotion list grid view.
        private void SetPromotionListToolMenu(PromotionListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PromotionDeletePopup')", ControllerName = "Promotion", ActionName = "Delete" });
            }
        }

        //Set tool menu for coupon list grid view.
        private void SetCouponListToolMenu(CouponListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PromotionDeletePopup')", ControllerName = "Promotion", ActionName = "Delete" });
            }
        }

        //Set tool menu for associated product list grid view.
        private void SetAssociatedProductListToolMenu(PublishProductsListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AssociatedProductDeletePopup',this)", ControllerName = "Promotion", ActionName = "UnAssociateProducts" });
            }
        }

        //Set tool menu for associated Category list grid view.
        private void SetAssociatedCategoryListToolMenu(CategoryListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AssociatedCategoryDeletePopup',this)", ControllerName = "Promotion", ActionName = "UnAssociateCategories" });
            }
        }

        //Set tool menu for associated Catalog list grid view.
        private void SetAssociatedCatalogListToolMenu(PortalCatalogListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AssociatedCatalogDeletePopup',this)", ControllerName = "Promotion", ActionName = "UnAssociateCatalogs" });
            }
        }

        //Set tool menu for associated brand list grid view.
        private void SetAssociatedBrandListToolMenu(BrandListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AssociatedBrandDeletePopup',this)", ControllerName = "Promotion", ActionName = "UnAssociateBrands" });
            }
        }

        //Get profile list for by store ID.
        private void getPortalProfileListByPortalId(int storeId, List<SelectListItem> selectedPortalProfileList)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, storeId.ToString()));

            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.Profiles);
            ZnodeLogging.LogMessage("Input parameters of method GetPortalProfiles: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { filters = filters, expands = expands });
            PortalProfileListModel portalProfileList = _portalProfileClient.GetPortalProfiles(expands, filters, null, null, null);
            if (portalProfileList?.PortalProfiles?.Count > 0)
            {
                portalProfileList?.PortalProfiles?.ToList().ForEach(item => { selectedPortalProfileList.Add(new SelectListItem() { Text = item.ProfileName, Value = item.ProfileId.ToString() }); });
            }
        }

        //Get profile list for all portals selected value.
        private void getPortalProfileListForAllPortal(List<SelectListItem> selectedPortalProfileList)
        {
            ProfileListModel profileList = _profileClient.GetProfileList(null, null, null, null);
            if (profileList?.Profiles?.Count > 0)
                profileList.Profiles.ToList().ForEach(item => { selectedPortalProfileList.Add(new SelectListItem() { Text = item.ProfileName, Value = item.ProfileId.ToString() }); });
        }

        // Get coupon list from binddata model.
        private CouponListViewModel GetCouponList(BindDataModel bindDataModel, PromotionViewModel viewModel)
        {
            CouponListViewModel couponListViewModel = new CouponListViewModel() { CouponList = new List<CouponViewModel>() };
            string couponCode = viewModel.IsUnique ? Convert.ToString(bindDataModel.GetValue("MultipleCouponCode")) : Convert.ToString(bindDataModel.GetValue("CouponCode"));
            string customCouponCode = Convert.ToString(bindDataModel.GetValue("CustomCouponCode"));
            ZnodeLogging.LogMessage("couponCode and customCouponCode: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new { couponCode = couponCode, customCouponCode = customCouponCode });
            if (!string.IsNullOrEmpty(couponCode))
            {
                List<string> couponCodeList = couponCode.Split(',')?.ToList();
                foreach (string item in couponCodeList)
                {
                    if (!string.IsNullOrEmpty(item))
                        couponListViewModel.CouponList.Add(new CouponViewModel
                        {
                            AvailableQuantity = Equals(Convert.ToString(bindDataModel.GetValue("AvailableQuantity")), "") ? 0 : Convert.ToInt32(bindDataModel.GetValue("AvailableQuantity")),
                            InitialQuantity = Equals(Convert.ToString(bindDataModel.GetValue("InitialQuantity")), "") ? 0 : Convert.ToInt32(bindDataModel.GetValue("InitialQuantity")),
                            Code = item,
                            IsActive = (viewModel.IsUnique) ? (!string.IsNullOrEmpty(Convert.ToString(bindDataModel.GetValue("isActiveCoupon_" + item))) ? true : false) : true,
                            IsCustomCoupon = string.IsNullOrEmpty(customCouponCode) ? false : true,
                            CustomCouponCode = customCouponCode
                        });
                }
            }
            return couponListViewModel;
        }

        //Set tool menu for associated shipping portal list grid view.
        private void SetAssociatedShippingListToolMenu(ShippingListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AssociatedShippingDeletePopup',this)", ControllerName = "Promotion", ActionName = "UnAssociateShippings" });
            }
        }
        #endregion
    }
}