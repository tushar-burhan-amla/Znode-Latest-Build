using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Znode.Engine.Admin.Agents
{
    public class CategoryAgent : BaseAgent, ICategoryAgent
    {
        #region Private Variables
        private readonly ICategoryClient _categoryClient;
        private readonly IProductsClient _productClient;
        private readonly ProductAgent _productAgent;
        private readonly ICatalogClient _catalogClient;
        #endregion

        #region Constructor
        public CategoryAgent(ICategoryClient categoryClient, IProductsClient productsClient, ICatalogClient catalogClient)
        {
            _categoryClient = GetClient<ICategoryClient>(categoryClient);
            _productClient = GetClient<IProductsClient>(productsClient);
            _productAgent = new ProductAgent(GetClient<PIMAttributeClient>(), GetClient<PIMAttributeFamilyClient>(), GetClient<ProductsClient>(), GetClient<LocaleClient>());
            _catalogClient = GetClient<ICatalogClient>(catalogClient);
        }
        #endregion

        #region Public Methods
        //Get Category List
        public virtual CategoryListViewModel GetCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int pimCatalogId = 0, string catalogName = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int localeId = _productAgent.GetLocaleValue();
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info,new { localeId = localeId });
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            if (filters.Exists(x => x.Item1 == ZnodePimCustomFieldLocaleEnum.LocaleId.ToString()))
                filters.RemoveAll(x => x.Item1 == ZnodePimCustomFieldLocaleEnum.LocaleId.ToString());
            filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));

            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);

            AddPIMCatalogIdInFilters(_filters, pimCatalogId);

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters= _filters, expands = expands, sorts = sorts });
            CategoryListModel categoryList = _categoryClient.GetCategoryList(expands, _filters, sorts, pageIndex, pageSize);
            CategoryListViewModel listViewModel = new CategoryListViewModel
            {
                XmlDataList = categoryList?.XmlDataList,
                Locale = PIMAttributeFamilyViewModelMap.ToLocaleListItem(categoryList.Locale),
                AttrubuteColumnName = categoryList?.AttrubuteColumnName
            };

            BindCatalogFilterValues(listViewModel, pimCatalogId, catalogName);

            //Set locale value as per selected from locale dropdown
            SetLocaleValueForDropdown(listViewModel.Locale, localeId.ToString());

            SetListPagingData(listViewModel, categoryList);

            //Set tool menu for category list grid view.
            SetCategoryListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return categoryList?.XmlDataList?.Count > 0 ? listViewModel : new CategoryListViewModel() { XmlDataList = new List<dynamic>(), Locale = listViewModel.Locale, PimCatalogId = listViewModel.PimCatalogId, PimCatalogName = listViewModel.PimCatalogName };
        }

        public virtual CategoryListViewModel GetAssociatedCategoriesToProduct(int productId)
        {
            /// get the list here and assign the grid model.
            return null;
        }

        //Get category data on basis of category id for edit category .
        public virtual PIMFamilyDetailsViewModel GetCategory(int categoryId, int familyId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            PIMFamilyDetailsViewModel categoryDetails = PIMAttributeFamilyViewModelMap.ToPIMFamilyDetailsViewModel(_categoryClient.GetCategory(categoryId, familyId, _productAgent.GetLocaleValue()));
            //Set locale value as per selected from locale dropdown
            SetLocaleValueForDropdown(categoryDetails.Locale, _productAgent.GetLocaleValue().ToString());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return categoryDetails;
        }

        //Create category
        public virtual CategoryViewModel CreateCategory(BindDataModel bindDataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                CategoryValuesListModel categoryValuesListModel = new CategoryValuesListModel();

                categoryValuesListModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                categoryValuesListModel.PimCatalogId = Equals(Convert.ToString(bindDataModel.GetValue("PimCatalogId")), "")
                                        ? (int?)null : Convert.ToInt32(bindDataModel.GetValue("PimCatalogId"));
                categoryValuesListModel.PimParentCategoryId = Equals(Convert.ToString(bindDataModel.GetValue("PimParentCategoryId")), "")
                                        ? (int?)null : Convert.ToInt32(bindDataModel.GetValue("PimParentCategoryId"));
                RemoveNonAttributeKeys(bindDataModel);
                RemoveAttributeWithEmptyValue(bindDataModel);
                RemoveAttrAndMceEditorKeyWord(bindDataModel);
                categoryValuesListModel = _categoryClient.CreateCategory(PIMCategoryViewModelMap.ToListModel(bindDataModel, categoryValuesListModel));
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return categoryValuesListModel?.ToViewModel<CategoryViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return null;
            }
        }

        //Update existing category
        public virtual bool UpdateCategory(BindDataModel bindDataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                CategoryValuesListModel categoryValuesListModel = new CategoryValuesListModel();
                categoryValuesListModel.LocaleId = _productAgent.GetLocaleValue();
                categoryValuesListModel.PimCategoryId = Convert.ToInt32(bindDataModel.GetValue("CategoryId"));
                RemoveNonAttributeKeys(bindDataModel);
                RemoveAttributeWithEmptyValue(bindDataModel);
                RemoveAttrAndMceEditorKeyWord(bindDataModel);
                _categoryClient.UpdateCategory(PIMCategoryViewModelMap.ToListModel(bindDataModel, categoryValuesListModel));
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Delete category
        public virtual bool DeleteCategory(string pimCategoryId) => _categoryClient.DeleteCategory(pimCategoryId);

        //Get category attributes with Group and family.
        public virtual PIMFamilyDetailsViewModel GetAttributeFamilyDetails(int familyId = 0, int localeId = 0)
        => PIMAttributeFamilyViewModelMap.ToPIMFamilyDetailsViewModel(_productClient.GetProductFamilyDetails(new PIMFamilyModel { Id = 0, PIMAttributeFamilyId = familyId, LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale), IsCategory = true }));

        //Get category attribute after family dropdown changes.
        public virtual PIMFamilyDetailsViewModel GetCategoryAttributes(int pimCategoryId, int familyId, int localeId = 0)
        => PIMAttributeFamilyViewModelMap.ToPIMFamilyDetailsViewModel(_categoryClient.GetCategory(pimCategoryId, familyId, _productAgent.GetLocaleValue()));

        //Get products associated to category.
        public virtual CategoryProductsListViewModel GetAssociatedCategoryProducts(int categoryId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            SetDisplayOrderSortIfNotPresent(ref sorts);
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, _productAgent.GetLocaleValue().ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, _productAgent.GetLocaleValue().ToString()));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters,  sorts = sorts });

            CategoryProductListModel categoryProductListModel = _categoryClient.GetAssociatedCategoryProducts(categoryId, false, expands, filters, sorts, pageIndex, pageSize);
            CategoryProductsListViewModel categoryProductsListViewModel = new CategoryProductsListViewModel { CategoryProducts = categoryProductListModel?.CategoryProducts?.ToViewModel<CategoryProductViewModel>().ToList() };
            SetListPagingData(categoryProductsListViewModel, categoryProductListModel);

            //Set tool menu for associated product list grid view.
            SetAssociatedProductListToolMenu(categoryProductsListViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return categoryProductsListViewModel?.CategoryProducts?.Count > 0 ? categoryProductsListViewModel : new CategoryProductsListViewModel();
        }

        //Get Categories associated to products.
        public virtual CategoryProductsListViewModel GetAssociatedCategoriesToProduct(int productId, bool isAssociateCategories, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, _productAgent.GetLocaleValue().ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, _productAgent.GetLocaleValue().ToString()));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });

            CategoryProductListModel categoryProductListModel = _categoryClient.GetAssociatedCategoriesToProduct(productId, isAssociateCategories, expands, filters, sorts, pageIndex, pageSize);
            CategoryProductsListViewModel categoryProductsListViewModel = new CategoryProductsListViewModel { CategoryProducts = categoryProductListModel?.CategoryProducts?.ToViewModel<CategoryProductViewModel>().ToList() };

            if (productId > 0)
                categoryProductsListViewModel.PimProductId = productId;

            SetListPagingData(categoryProductsListViewModel, categoryProductListModel);

            //Set tool menu for associated product list grid view.
            if (!isAssociateCategories)
                SetAssociatedCategoriesListToolMenu(categoryProductsListViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return categoryProductsListViewModel?.CategoryProducts?.Count > 0 ? categoryProductsListViewModel : new CategoryProductsListViewModel() { PimProductId = productId};
        }

        //Get products which are not associated to category.
        public virtual CategoryProductsListViewModel GetUnAssociatedCategoryProducts(int categoryId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, _productAgent.GetLocaleValue().ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, _productAgent.GetLocaleValue().ToString()));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });

            CategoryProductListModel categoryProductListModel = _categoryClient.GetAssociatedUnAssociatedCategoryProducts(categoryId, true, expands, filters, sorts, pageIndex, pageSize);
            CategoryProductsListViewModel categoryProductsListViewModel = new CategoryProductsListViewModel { CategoryProducts = categoryProductListModel?.CategoryProducts?.ToViewModel<CategoryProductViewModel>().ToList() };
            SetListPagingData(categoryProductsListViewModel, categoryProductListModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return categoryProductsListViewModel?.CategoryProducts?.Count > 0 ? categoryProductsListViewModel : new CategoryProductsListViewModel();
        }

        //Delete category associated products.
        public virtual bool DeleteCategoryProduct(string categoryProductId) => _categoryClient.DeleteCategoryProduct(categoryProductId);

        //Delete associated categories to product.
        public virtual bool DeleteCategoriesAssociatedToProduct(string categoryIds) => _categoryClient.DeleteAssociatedCategoriesToProduct(categoryIds);

        public virtual bool AssociateCategoryProduct(int categoryId, string productIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                return categoryId > 0 && !string.IsNullOrEmpty(productIds) ?
                    _categoryClient.AssociateCategoryProduct(CategoryViewModelMap.ToAssociateStoreListModel(categoryId, productIds)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        // Associate categories to Products.
        public virtual bool AssociateCategoriesToProduct(int productId, string categoryIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                return productId > 0 && !string.IsNullOrEmpty(categoryIds) ?
                    _categoryClient.AssociateCategoriesToProduct(CategoryViewModelMap.ToAssociateCategoriesToProductListModel(productId, categoryIds)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Update product details associated to category.
        public virtual bool UpdateCategoryProductDetail(CategoryProductViewModel categoryProductViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool status = false;
            try
            {
                status = _categoryClient.UpdateCategoryProductDetail(categoryProductViewModel.ToModel<CategoryProductModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #endregion

        #region Private Method
        //Set tool menu for category list grid view.
        private void SetCategoryListToolMenu(CategoryListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeletePopup')", ControllerName = "Category", ActionName = "Delete" });
            }
        }

        //Set tool menu for associated product list grid view.
        private void SetAssociatedProductListToolMenu(CategoryProductsListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeletePopup',this)", ControllerName = "Category", ActionName = "DeleteAssociatedProducts" });
            }
        }

        // Set Tool Menu for Associated categories to product for list grid view.
        private void SetAssociatedCategoriesListToolMenu(CategoryProductsListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('CategoryDeletePopup',this)", ControllerName = "Category", ActionName = "DeleteAssociatedCategories" });
            }
        }

        //Add PIM Catalog id in filter collection
        private void AddPIMCatalogIdInFilters(FilterCollection filters, int pimCatalogId)
        {
            if (pimCatalogId > 0 || pimCatalogId == -1)
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.CatalogId, StringComparison.InvariantCultureIgnoreCase));
                filters.Add(new FilterTuple(FilterKeys.CatalogId.ToString(), FilterOperators.Equals, pimCatalogId.ToString()));
            }
        }

        //Map catalog filter values in view model
        private void BindCatalogFilterValues(CategoryListViewModel categoryList, int pimCatalogId, string catalogName)
        {
            categoryList.PimCatalogName = string.IsNullOrEmpty(catalogName) ? Admin_Resources.LabelAllCategory : catalogName;
            categoryList.PimCatalogId = pimCatalogId;
        }
        #endregion

        #region Publish Category

        //Publish Category 
        public virtual bool PublishCategory(string categoryIds, string revisionType, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            errorMessage = PIM_Resources.ErrorPublished;
            try
            {
                return Convert.ToBoolean(_categoryClient.PublishCategory(new ParameterModel { Ids = categoryIds, RevisionType = revisionType })?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = PIM_Resources.CategoryPublishError;
                        return false;
                    case ErrorCodes.NotPermitted:
                        errorMessage = PIM_Resources.ErrorPublishCatalog;
                        return false;
                    case ErrorCodes.CategoryPublishError:
                        errorMessage = PIM_Resources.ErrorPublishCategoryInProcess;
                        break;
                 }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion Publish Category
    }
}