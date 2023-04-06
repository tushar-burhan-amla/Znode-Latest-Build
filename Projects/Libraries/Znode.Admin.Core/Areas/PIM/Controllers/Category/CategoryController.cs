using MvcSiteMapProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Areas.PIM.Controllers
{
    public class CategoryController : BaseController
    {
        #region Private Variables

        private readonly ICategoryAgent _categoryAgent;
        private INavigationAgent _navigationAgent;

        #endregion Private Variables

        public CategoryController(ICategoryAgent categoryAgent, INavigationAgent navigationAgent)
        {
            _categoryAgent = categoryAgent;
            _navigationAgent = navigationAgent;
        }

        // GET: Category list
        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,HeaderCategoryList", Key = "Category", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int pimCatalogId = 0, string catalogName = null)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.View_PimCategoryDetail.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.View_PimCategoryDetail.ToString(), model);
            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            RemoveCatalogFilterValue(model, pimCatalogId);

            CategoryListViewModel categoryList = _categoryAgent.GetCategoryList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, pimCatalogId, catalogName);

            categoryList.AttrubuteColumnName?.Remove(ZnodeConstant.CategoryImage);
            //Get the grid model.
            categoryList.GridModel = FilterHelpers.GetDynamicGridModel(model, IsNull(categoryList?.XmlDataList) ? new List<dynamic>() : categoryList.XmlDataList, GridListType.View_PimCategoryDetail.ToString(), string.Empty, null, true, true, categoryList?.GridModel?.FilterColumn?.ToolMenuList, AttrColumn(categoryList.AttrubuteColumnName));

            //Set the total record count
            categoryList.GridModel.TotalRecordCount = categoryList.TotalResults;
            return ActionView(categoryList);
        }

        //Get: Create new category.
        [HttpGet]
        public virtual ActionResult Create(int catalogId = 0, int parentCategoryId = 0)
        {
            //Create category.
            PIMFamilyDetailsViewModel attributeFamilyDetails = _categoryAgent.GetAttributeFamilyDetails();

            if (IsNotNull(attributeFamilyDetails))
            {
                //Assign the values.
                attributeFamilyDetails.PimCatalogId = catalogId;
                attributeFamilyDetails.PimParentCategoryId = parentCategoryId;
                return ActionView(AdminConstants.CreateEdit, attributeFamilyDetails);
            }

            SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.ErrorLoadingdata));
            return RedirectToAction<CategoryController>(x => x.List(null, 0, null));
        }

        //Post: Create new category.
        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel bindDataModel)
        {
            string selectedtab = Request.QueryString["selectedtab"];
            CategoryViewModel categoryViewModel = _categoryAgent.CreateCategory(bindDataModel);
            if (categoryViewModel?.PimCategoryId > 0)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.CreateMessage));
                // If request is from catalog.
                if (categoryViewModel.PimCatalogId > 0)
                    return RedirectToAction<CatalogController>(x => x.Manage(null, categoryViewModel.PimCatalogId.GetValueOrDefault(), categoryViewModel.PimCategoryId, null, -1, true));

                return RedirectToAction<CategoryController>(x => x.Edit(categoryViewModel.PimCategoryId, selectedtab));
            }
            SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.ErrorFailedToCreate));
            return RedirectToAction<CategoryController>(x => x.List(null, 0, null));
        }

        //Get: get category details for edit.
        [HttpGet]
        public virtual ActionResult Edit(int pimCategoryId, string selectedtab = null)
        {
            ActionResult action = GotoBackURL();
            if (IsNotNull(action))
                return action;

            PIMFamilyDetailsViewModel categoryAttributes = _categoryAgent.GetCategory(pimCategoryId, 0);
            if (IsNotNull(categoryAttributes))
            {
                return ActionView(AdminConstants.CreateEdit, categoryAttributes);
            }

            SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.ErrorLoadingdata));
            return RedirectToAction<CategoryController>(x => x.List(null, 0, null));
        }

        //Post: Edit category on basis of categoryId.
        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel bindDataModel)
        {
            string selectedtab = Request.QueryString["selectedtab"];
            int categoryId = Convert.ToInt32(bindDataModel.GetValue("CategoryId"));
            SetNotificationMessage(_categoryAgent.UpdateCategory(bindDataModel)
                ? GetSuccessNotificationMessage(PIM_Resources.UpdateMessage) : GetErrorNotificationMessage(PIM_Resources.UpdateErrorMessage));
            return RedirectToAction<CategoryController>(x => x.Edit(categoryId, selectedtab));
        }

        //Delete category on basis of categoryId.
        public virtual ActionResult Delete(string pimCategoryId)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(pimCategoryId))
            {
                status = _categoryAgent.DeleteCategory(pimCategoryId);
                errorMessage = status ? Admin_Resources.DeleteMessage : PIM_Resources.ErrorDeleteCategory;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get category attributes on family change dropdown.
        [HttpGet]
        public virtual ActionResult GetAttributes(int familyId, int categoryId, int localeId = 0)
        {
            PIMFamilyDetailsViewModel attributeFamilyDetails = null;

            attributeFamilyDetails = (categoryId > 0) ? _categoryAgent.GetCategoryAttributes(categoryId, familyId, localeId) : _categoryAgent.GetAttributeFamilyDetails(familyId, localeId);

            if (IsNotNull(attributeFamilyDetails))
                return ActionView(AdminConstants.CreateEdit, attributeFamilyDetails);

            SetNotificationMessage(GetSuccessNotificationMessage(PIM_Resources.ErrorLoadingdata));
            return RedirectToAction<CategoryController>(x => x.List(null, 0, null));
        }

        //Get list of associated category products.
        public virtual ActionResult GetAssociatedCategoryProducts([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int pimCategoryId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedCategoryProducts.ToString(), model);
            CategoryProductsListViewModel categoryProducts = _categoryAgent.GetAssociatedCategoryProducts(pimCategoryId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            categoryProducts.PimCategoryId = pimCategoryId;

            categoryProducts.GridModel = FilterHelpers.GetDynamicGridModel(model, categoryProducts?.CategoryProducts, GridListType.AssociatedCategoryProducts.ToString(), string.Empty, null, true, true, categoryProducts?.GridModel?.FilterColumn?.ToolMenuList);
            categoryProducts.GridModel.TotalRecordCount = categoryProducts.TotalResults;
            return PartialView("_GetAssociatedCategoryProducts", categoryProducts);
        }

        //Get the list of Associated categories to Product.
        public virtual ActionResult GetAssociatedCategoriesToProduct([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, bool isAssociateCategories, int productId)
        {
            //Assign default view filter and sorting if exists for the first request.
            CategoryProductsListViewModel categoryProducts = GetCategoriesToProduct(model, isAssociateCategories, productId);

            return PartialView(AdminConstants.AssociatedCategoriesView, categoryProducts);
        }



        //Get list of un-associated categories to product.
        public virtual ActionResult GetUnAssociatedCategoriesToProduct([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, bool isAssociateCategories, int productId)
        {
            //Assign default view filter and sorting if exists for the first request.
            CategoryProductsListViewModel categoryProducts = GetCategoriesToProduct(model, isAssociateCategories, productId);

            return PartialView(AdminConstants.UnAssociatedCategoriesView, categoryProducts);
        }

        public CategoryProductsListViewModel GetCategoriesToProduct(FilterCollectionDataModel model, bool isAssociateCategories, int productId)
        {
            FilterHelpers.GetDefaultView((isAssociateCategories ? GridListType.UnAssociatedCategoriesToProduct.ToString() : GridListType.AssociatedCategoriesToProduct.ToString()), model);
            CategoryProductsListViewModel categoryProducts = _categoryAgent.GetAssociatedCategoriesToProduct(productId, isAssociateCategories, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            categoryProducts.GridModel = FilterHelpers.GetDynamicGridModel(model, categoryProducts?.CategoryProducts,
              (isAssociateCategories ? GridListType.UnAssociatedCategoriesToProduct.ToString() : GridListType.AssociatedCategoriesToProduct.ToString()),
                string.Empty, null, true, true, categoryProducts?.GridModel?.FilterColumn?.ToolMenuList);

            categoryProducts.GridModel.TotalRecordCount = categoryProducts.TotalResults;
            return categoryProducts;
        }
        //Get list of Un-associated category products.
        public virtual ActionResult GetUnAssociatedCategoryProducts([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int categoryId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedCategoryProducts.ToString(), model);

            CategoryProductsListViewModel categoryProducts = _categoryAgent.GetUnAssociatedCategoryProducts(categoryId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            categoryProducts.GridModel = FilterHelpers.GetDynamicGridModel(model, categoryProducts?.CategoryProducts, GridListType.UnAssociatedCategoryProducts.ToString(), string.Empty, null, true);
            categoryProducts.PimCategoryId = categoryId;
            categoryProducts.GridModel.TotalRecordCount = categoryProducts.TotalResults;
            return PartialView("_GetUnAssociatedCategoryProducts", categoryProducts);
        }

        //Associate products to category.
        public virtual JsonResult AssociatedCategoryProducts(int categoryId, string productIds)
        {
            string errorMessage = PIM_Resources.ErrorAssociateProducts;
            bool status = false;
            if (!string.IsNullOrEmpty(productIds) && categoryId > 0)
            {
                status = _categoryAgent.AssociateCategoryProduct(categoryId, productIds);
                errorMessage = status ? PIM_Resources.SuccessAssociateProducts : PIM_Resources.ErrorAssociateProducts;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Associate categories to Products
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="productIds"></param>
        /// <returns></returns>
        public virtual JsonResult AssociateCategoriesToProduct(int productId, string categoryIds)
        {
            string errorMessage = PIM_Resources.ErrorAssociateProducts;
            bool status = false;
            if (!string.IsNullOrEmpty(categoryIds) && productId > 0)
            {
                status = _categoryAgent.AssociateCategoriesToProduct(productId, categoryIds);
                errorMessage = status ? PIM_Resources.SuccessProductCategoriesAssociation : PIM_Resources.ErrorProductCategoriesAssociation;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete associated products.
        public virtual ActionResult DeleteAssociatedProducts(string pimCategoryProductId)
        {
            string errorMessage = Admin_Resources.ErrorFailedToDelete;
            bool status = false;
            if (!string.IsNullOrEmpty(pimCategoryProductId))
            {
                status = _categoryAgent.DeleteCategoryProduct(pimCategoryProductId);
                errorMessage = status ? Admin_Resources.DeleteMessage : Admin_Resources.ErrorFailedToDelete;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        // delete all the selected Category Ids.
        public virtual ActionResult DeleteAssociatedCategories(string pimCategoryProductId)
        {
            string errorMessage = Admin_Resources.ErrorFailedToDelete;
            bool status = false;
            if (!string.IsNullOrEmpty(pimCategoryProductId))
            {
                status = _categoryAgent.DeleteCategoriesAssociatedToProduct(pimCategoryProductId);
                errorMessage = status ? Admin_Resources.DeleteMessage : Admin_Resources.ErrorFailedToDelete;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #region Publish Category
        //Publish Category 
        public virtual ActionResult PublishCategory(int pimCategoryId, string revisionType)
        {
            if (pimCategoryId > 0)
            {
                string errorMessage;
                bool status = _categoryAgent.PublishCategory(Convert.ToString(pimCategoryId), revisionType, out errorMessage);
                return Json(new { status = status, message = status ? PIM_Resources.TextPublishedSuccessfully : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, message = PIM_Resources.ErrorPublished }, JsonRequestBehavior.AllowGet);
        }

        //Draft category and then publish category 
        public virtual ActionResult UpdateAndPublishCategory([ModelBinder(typeof(ControlsModelBinder))] BindDataModel bindDataModel)
        {
            string errorMessage = string.Empty;
            int pimCategoryId = Convert.ToInt32(bindDataModel.GetValue("CategoryId"));
            string revisionType = Convert.ToString(bindDataModel.GetValue("revisionType"));
            bool updateStatus = _categoryAgent.UpdateCategory(bindDataModel);
            if (updateStatus)
                SetNotificationMessage(_categoryAgent.PublishCategory(Convert.ToString(pimCategoryId), revisionType, out errorMessage) ?
                    GetSuccessNotificationMessage(PIM_Resources.TextPublishedSuccessfully) : GetErrorNotificationMessage(errorMessage));
            else
                SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.UpdateErrorMessage));

            return RedirectToAction<CategoryController>(x => x.Edit(pimCategoryId, null));
        }
        #endregion Publish Category


        //Update product details associated to category.
        public virtual JsonResult UpdateCategoryProductDetail(int pimCategoryProductId, string data)
        {
            bool status = false;
            string message = string.Empty;
            if (!string.IsNullOrEmpty(data))
            {
                CategoryProductViewModel categoryProductViewModel = JsonConvert.DeserializeObject<CategoryProductViewModel[]>(data)[0];

                if (ModelState.IsValid)
                    status = _categoryAgent.UpdateCategoryProductDetail(
                        new CategoryProductViewModel
                        {
                            PimCategoryProductId = pimCategoryProductId,
                            DisplayOrder = categoryProductViewModel.DisplayOrder
                        });
            }
            message = status ? PIM_Resources.UpdateMessage : PIM_Resources.UpdateErrorMessage;
            return Json(new { status = status, message = message, pimCategoryProductId = pimCategoryProductId }, JsonRequestBehavior.AllowGet);
        }

        #region Private Methods

        //Remove catalog filter flag in FilterCollectionDataModel
        private void RemoveCatalogFilterValue(FilterCollectionDataModel model, int pimCatalogId)
        {
            model.Filters?.RemoveAll(x => string.Equals(x.FilterName, AdminConstants.IsCatalogFilter, StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion
    }
}