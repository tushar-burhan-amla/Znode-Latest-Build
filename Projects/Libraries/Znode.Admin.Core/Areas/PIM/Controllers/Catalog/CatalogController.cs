using MvcSiteMapProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;    
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Areas.PIM.Controllers
{
    public class CatalogController : BaseController
    {
        #region Private Variables

        private readonly IStoreAgent _storeAgent;
        private readonly ICatalogAgent _catalogAgent;
        private readonly IProductAgent _productAgent;
        private IMediaManagerAgent _mediaManagerAgent;       
        private readonly IERPTaskSchedulerAgent _erpTaskSchedulerAgent;
        private readonly IHelperAgent _helperAgent;

        private const string copyCatalogView = "CopyCatalog";
        private const string createSchedulerView = "~/Views/TouchPointConfiguration/Create.cshtml";

        #endregion Private Variables

        #region Constructor

        public CatalogController(IStoreAgent storeAgent, ICatalogAgent catalogAgent, IProductAgent productAgent, IMediaManagerAgent mediaManagerAgent, IERPTaskSchedulerAgent erpTaskSchedulerAgent, IHelperAgent helperAgent)
        {
            _storeAgent = storeAgent;
            _catalogAgent = catalogAgent;
            _productAgent = productAgent;
            _mediaManagerAgent = mediaManagerAgent;
            _erpTaskSchedulerAgent = erpTaskSchedulerAgent;
            _helperAgent = helperAgent;
        }

        #endregion Constructor

        #region Public Methods

        #region Catalog Settings

        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,TitleCatalog", Key = "Catalog", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult CatalogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodePimCatalog.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePimCatalog.ToString(), model);

            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            CatalogListViewModel catalogList = _catalogAgent.GetCatalogList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            catalogList.GridModel = FilterHelpers.GetDynamicGridModel(model, catalogList.Catalogs, GridListType.ZnodePimCatalog.ToString(), string.Empty, null, true, true, catalogList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            catalogList.GridModel.TotalRecordCount = catalogList.TotalResults;
            return ActionView(catalogList);
        }

        [HttpGet]
        public virtual ActionResult Create()
            => ActionView(AdminConstants.CreateEdit, new CatalogViewModel());

        [HttpPost]
        public virtual ActionResult Create(CatalogViewModel catalogViewModel)
        {
            if (ModelState.IsValid)
            {
                catalogViewModel.IsActive = true;
                catalogViewModel = _catalogAgent.CreateCatalog(catalogViewModel);
                if (catalogViewModel?.PimCatalogId > 0)
                {
                    SetNotificationMessage(catalogViewModel.IsAllowIndexing ? GetSuccessNotificationMessage(Admin_Resources.EnableIndexingMessage) :
                                               GetSuccessNotificationMessage(PIM_Resources.CreateMessage));
                    return RedirectToAction<CatalogController>(x => x.Edit(catalogViewModel.PimCatalogId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(PIM_Resources.ErrorFailedToCreate));
            return ActionView(AdminConstants.CreateEdit, new CatalogViewModel());
        }

        public virtual ActionResult Edit(int pimCatalogId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            if (pimCatalogId > 0)
                return RedirectToAction<CatalogController>(x => x.Manage(null, pimCatalogId, -1, null, -1, true));

            return RedirectToAction<CatalogController>(x => x.CatalogList(new FilterCollectionDataModel()));
        }

        public virtual ActionResult EditCatalog(int pimCatalogId)
        {
            CatalogViewModel catalogViewModel = _catalogAgent.GetCatalog(pimCatalogId);
            return ActionView(catalogViewModel);
        }

        [HttpPost]
        public virtual ActionResult EditCatalog(CatalogViewModel catalogViewModel)
        {            
            if (ModelState.IsValid)
            {
                catalogViewModel = _catalogAgent.UpdateCatalog(catalogViewModel);
                string message = catalogViewModel.IsAllowIndexing ? Admin_Resources.EnableIndexingMessage : Admin_Resources.UpdateMessage;
                if (!catalogViewModel.HasError)
                    return Json(new { status = true, message = message }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }


        //Update catalog
        public virtual JsonResult EditCatalogName(int pimCatalogId, string data)
        {
            CatalogViewModel catalogViewModel = JsonConvert.DeserializeObject<CatalogViewModel[]>(data)[0];

            bool exists = _catalogAgent.CheckCatalogNameExist(catalogViewModel.CatalogName, pimCatalogId);
            if (exists)
                return Json(new { status = false, message = PIM_Resources.AlreadyExistCatalogName }, JsonRequestBehavior.AllowGet);

            if (ModelState.IsValid)
            {
                catalogViewModel = _catalogAgent.UpdateCatalog(catalogViewModel);
                if (!catalogViewModel.HasError)
                    return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult Copy(int pimCatalogId)
        {
            CatalogViewModel catalogs = _catalogAgent.GetCatalog(pimCatalogId);
            if (Equals(catalogs, null))
                return RedirectToAction<CatalogController>(x => x.CatalogList(null));

            catalogs.CatalogName = $"Copy Of {catalogs.CatalogName}";
            catalogs.CatalogCode = string.Empty;
            return ActionView(copyCatalogView, catalogs);
        }

        //Get Suggestions.
        [HttpGet]
        public virtual JsonResult GetSuggestions(string type, string fieldname, string query)
        => Json(ZnodeDependencyResolver.GetService<ITypeaheadAgent>()?.GetAutocompleteList(query, type, fieldname), JsonRequestBehavior.AllowGet);

        [HttpPost]
        public virtual ActionResult Copy(CatalogViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool status = _catalogAgent.CopyCatalog(model);
                SetNotificationMessage(status ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) : GetErrorNotificationMessage(model.ErrorMessage));
                return status ? RedirectToAction<CatalogController>(x => x.CatalogList(null)) : ActionView(copyCatalogView, model);
            }
            return ActionView(copyCatalogView, model);
        }

        public virtual ActionResult Delete(string pimCatalogId, bool isDeletePublishCatalog)
        {
            string message = Admin_Resources.ErrorFailedToDelete;
            bool status = false;
            if (!string.IsNullOrEmpty(pimCatalogId))
            {
                status = _catalogAgent.DeleteCatalog(pimCatalogId, isDeletePublishCatalog);
                message = status ? Admin_Resources.DeleteMessage : Admin_Resources.ErrorCatalogDelete;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get details of categories associated to catalog.
        [HttpGet]
        public virtual ActionResult EditCategorySettings(int catalogId, int categoryHierarchyId)
            => ActionView("_EditCategorySetting", _catalogAgent.GetAssociatedCategoryDetails(new CatalogAssociateCategoryViewModel { PimCatalogId = catalogId, PimCategoryHierarchyId = categoryHierarchyId }));

        //Update details of category associated to catalog.
        [HttpPost]
        public virtual ActionResult EditCategorySettings(CatalogAssociateCategoryViewModel catalogAssociateCategoryViewModel)
        {
            //update category details.
            if (ModelState.IsValid)
                catalogAssociateCategoryViewModel = _catalogAgent.UpdateAssociatedCategoryDetails(catalogAssociateCategoryViewModel);
            else
                catalogAssociateCategoryViewModel.HasError = true;

            SetNotificationMessage(catalogAssociateCategoryViewModel.HasError ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage) : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
            return RedirectToAction<CatalogController>(x => x.Manage(null, catalogAssociateCategoryViewModel.PimCatalogId, catalogAssociateCategoryViewModel.PimCategoryHierarchyId, null, -1, true));
        }

        //Checks weather catalog name exists.
        [HttpPost]
        public virtual JsonResult IsCatalogNameExist(string CatalogName, int PimCatalogId = 0)
            => Json(!_catalogAgent.CheckCatalogNameExist(CatalogName, PimCatalogId), JsonRequestBehavior.AllowGet);

        #endregion Catalog Settings

        //Preview Catalog
        public virtual JsonResult Preview(int pimCatalogId)
        {
            if (pimCatalogId > 0)
            {
                string errorMessage;
                bool status = _catalogAgent.PublishCatalog(pimCatalogId, ZnodePublishStatesEnum.PREVIEW.ToString(), out errorMessage);
                return Json(new { status = status, message = status ? PIM_Resources.TextPublishStarted : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, message = PIM_Resources.ErrorPublished }, JsonRequestBehavior.AllowGet);
        }

        //Publish Catalog
        public virtual JsonResult Publish(int pimCatalogId, string revisionType, string publishContent = null)
        {
            if (pimCatalogId > 0)
            {
                string errorMessage;
                bool status = _catalogAgent.PublishCatalog(pimCatalogId, revisionType, out errorMessage, publishContent);
                return Json(new { status = status, message = status ? PIM_Resources.TextPublishStarted : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, message = PIM_Resources.ErrorPublished }, JsonRequestBehavior.AllowGet);
        }

        //Publish catalog category associated products.
        public virtual JsonResult PublishCatalogCategoryProducts(int pimCatalogId, int pimCategoryHierarchyId, string revisionType)
        {
            if (pimCatalogId > 0 && pimCategoryHierarchyId > 0)
            {
                string errorMessage;
                bool status = _catalogAgent.PublishCategoryProducts(pimCatalogId, pimCategoryHierarchyId, revisionType, out errorMessage);
                return Json(new { status = status, message = status ? PIM_Resources.CategoryPublishSuccessMsg : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = false, message = PIM_Resources.ErrorPublished }, JsonRequestBehavior.AllowGet);
        }
        public virtual ActionResult GetCatalogPublishStatus(int pimCatalogId, string catalogName, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePublishCatalogLog.ToString(), model);
            PublishCatalogLogListViewModel publishCatalogLogListViewModel = _catalogAgent.GetCatalogPublishStatus(pimCatalogId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            publishCatalogLogListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, publishCatalogLogListViewModel.PublishCatalogLog, GridListType.ZnodePublishCatalogLog.ToString(), string.Empty, null, true, true, publishCatalogLogListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            publishCatalogLogListViewModel.GridModel.TotalRecordCount = publishCatalogLogListViewModel.TotalResults;
            publishCatalogLogListViewModel.CatalogName = catalogName;
            return ActionView("_CatalogPublishStatus", publishCatalogLogListViewModel);
        }

        //Returns the View of manage catalog.
        public virtual ActionResult Manage([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int pimCatalogId, int pimCategoryHierarchyId = -1, string displayMode = "", int portalId = -1, bool isCategoryController = false)
        {            
            bool isActiveProducts = _catalogAgent.GetActiveProductFilter(model.Params,model.Filters);

            ActionResult action = GotoBackURL();
            // checks backURL from CatalogController or CategoryController.
            if (action != null && isCategoryController == false)
                return action;
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeGetCatalogAssociatedProduct.ToString(), model);

            _catalogAgent.SetActiveProductFilter(model.Filters, isActiveProducts);

            CatalogAssociationViewModel catalogList = _catalogAgent.GetAssociatedProducts(pimCatalogId, pimCategoryHierarchyId, portalId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            catalogList.Tree = _catalogAgent.GetTree(pimCatalogId);
            catalogList.PortalId = portalId;
            catalogList.PortalList = _storeAgent.GetPortalListByCatalogId(pimCatalogId, catalogList.PortalId);
            
            //To get Checkbox in tileview.
            if (!string.IsNullOrEmpty(displayMode))
                model.ViewMode = displayMode;

            model.IsMultiSelectList = true;            

            //Get the grid model.
            catalogList.GridModel = FilterHelpers.GetDynamicGridModel(model, catalogList.AssociatedProducts, GridListType.ZnodeGetCatalogAssociatedProduct.ToString(), string.Empty, null, true, true, catalogList?.GridModel?.FilterColumn?.ToolMenuList);

            //Gets the list of view modes.
            catalogList.GridModel.ViewModeType = _mediaManagerAgent.GetViewModes(model.ViewMode);

            catalogList.IsActive = isActiveProducts;

            catalogList.GridModel.TotalRecordCount = catalogList.TotalResults;
            return Request.IsAjaxRequest() ? PartialView("AssociatedCategoriesList", catalogList) : ActionView(catalogList);
        }

        //Returns list of unassociated categories.
        public virtual ActionResult UnAssociatedCategories([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int pimCatalogId, int pimCategoryId, int pimCategoryHierarchyId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedCategoriesToCatalog.ToString(), model);

            CatalogAssociateCategoryListViewModel categoryList = _catalogAgent.GetUnAssociatedCategoryList(pimCatalogId, pimCategoryId, pimCategoryHierarchyId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            model.ViewMode = ViewModeTypes.List.ToString();

            categoryList.AttributeColumnName?.Remove(ZnodeConstant.CategoryImage);
            //Get the grid model.
            categoryList.GridModel = FilterHelpers.GetDynamicGridModel(model, IsNull(categoryList?.XmlDataList) ? new List<dynamic>() : categoryList.XmlDataList, GridListType.UnAssociatedCategoriesToCatalog.ToString(), string.Empty, null, true, true, categoryList?.GridModel?.FilterColumn?.ToolMenuList, AttrColumn(categoryList.AttributeColumnName));

            //Set the total record count
            categoryList.GridModel.TotalRecordCount = categoryList.TotalResults;
            return ActionView(categoryList);
        }

        //Returns list of unassociated products.
        public virtual ActionResult UnAssociatedProducts([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int pimCatalogId, int pimCategoryId, int pimCategoryHierarchyId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.UnAssociatedProductsToCatalog.ToString(), model);

            ProductDetailsListViewModel productList = _catalogAgent.GetUnAssociatedProductsList(pimCatalogId, pimCategoryId, pimCategoryHierarchyId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            productList.PimCatalogId = pimCatalogId;
            productList.PimCategoryId = pimCategoryId;
            productList.PimCategoryHierarchyId = pimCategoryHierarchyId;
            model.ViewMode = ViewModeTypes.List.ToString();
            //Get the grid model.
            productList.GridModel = FilterHelpers.GetDynamicGridModel(model, productList?.ProductDetailList, GridListType.UnAssociatedProductsToCatalog.ToString(), string.Empty, null, true, true, productList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            productList.GridModel.TotalRecordCount = productList.TotalResults;
            return ActionView(productList);
        }

        //Delete associated categories to catalog.
        public virtual JsonResult DeleteAssociateCategory(int pimCatalogId, string pimCategoryHierarchyId)
        {
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _catalogAgent.UnAssociateCategoryFromCatalog(new CatalogAssociationViewModel { CatalogId = pimCatalogId, PimCategoryHierarchyIds = pimCategoryHierarchyId });
            responsemessage.Message = responsemessage.HasNoError ? Admin_Resources.DeleteMessage : Admin_Resources.ErrorFailedToDelete;
            responsemessage.FolderJsonTree = _catalogAgent.GetTree(pimCatalogId);
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Move folder to another folder.
        public virtual ActionResult MoveFolder(int addtoFolderId, int folderId, int pimCatalogId)
        {
            string message = string.Empty;
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _catalogAgent.MoveCategory(folderId, addtoFolderId, pimCatalogId, out message);
            responsemessage.Message = responsemessage.HasNoError ? MediaManager_Resources.MoveFolderSuccess : message;
            responsemessage.FolderJsonTree = _catalogAgent.GetTree(pimCatalogId);
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }

        //Delete associated categories to catalog.
        public virtual JsonResult DeleteAssociateProducts(CatalogAssociationViewModel catalogAssociationViewModel)
            => Json(new { status = _catalogAgent.UnAssociateProductsFromCatalogCategory(catalogAssociationViewModel) });

        //Associate category to catalog.
        [Obsolete("This action is not in use now, as new method has been introduced to unassociate category")]
        public virtual JsonResult AssociateCategory(CatalogAssociationViewModel catalogAssociationViewModel)
            => Json(new { status = _catalogAgent.AssociateCategory(catalogAssociationViewModel), tree = _catalogAgent.GetTree(catalogAssociationViewModel.CatalogId) });

        //Associate products to catalog category.
        public virtual JsonResult AssociateProduct(CatalogAssociationViewModel catalogAssociationViewModel)
            => Json(new { status = _catalogAgent.AssociateProductsToCatalogCategory(catalogAssociationViewModel) });

        public virtual JsonResult UpdateCatalogCategoryProduct(int ProductId, int PimCatalogId, int PimCategoryHierarchyId, string data)
        {
            CatalogAssociationViewModel catalogAssociationViewModel = JsonConvert.DeserializeObject<CatalogAssociationViewModel[]>(data)[0];

            bool status = false;
            string message = string.Empty;
            if (ModelState.IsValid)
                status = _catalogAgent.UpdateCatalogCategoryProduct(
                    new CatalogAssociationViewModel
                    {
                        CatalogId = PimCatalogId,
                        PimCategoryHierarchyId = PimCategoryHierarchyId,
                        ProductId = ProductId,
                        DisplayOrder = catalogAssociationViewModel.DisplayOrder
                    });

            message = status ? PIM_Resources.UpdateMessage : PIM_Resources.UpdateErrorMessage;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //To do
        public virtual ActionResult UpdateDisplayOrder(int pimCatalogId, int pimCategoryHierarchyId, int displayOrder, bool isDown = false)
        {
            TreeResponseMessage responsemessage = new TreeResponseMessage();
            responsemessage.HasNoError = _catalogAgent.MoveCategory(pimCatalogId, pimCategoryHierarchyId, displayOrder, isDown);
            responsemessage.Message = responsemessage.HasNoError ? "Success" : "Fail";
            responsemessage.FolderJsonTree = _catalogAgent.GetTree(pimCatalogId);
            return Json(responsemessage, JsonRequestBehavior.AllowGet);
        }      

        //Get type method to Create new ERP Task Scheduler.
        [HttpGet]
        public virtual ActionResult CreateScheduler(string ConnectorTouchPoints, string indexName = "", string schedulerCallFor = "", int portalId = 0, int catalogId = 0, int catalogIndexId = 0)
        {
            if (!string.IsNullOrEmpty(ConnectorTouchPoints))
            {
                ConnectorTouchPoints = HttpUtility.UrlDecode(ConnectorTouchPoints);
                int erpTaskSchedulerId = _erpTaskSchedulerAgent.GetSchedulerIdByTouchPointName(ConnectorTouchPoints, schedulerCallFor);
                if (erpTaskSchedulerId == 0)
                {
                    ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.SetTaskSchedulerData(ConnectorTouchPoints, indexName, schedulerCallFor, portalId, catalogId, catalogIndexId);
                    return ActionView(createSchedulerView, erpTaskSchedulerViewModel);
                }
                else
                    return RedirectToAction<CatalogController>(x => x.EditScheduler(erpTaskSchedulerId, indexName, schedulerCallFor, portalId, catalogId, catalogIndexId));
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorEmptyTouchPoint));
                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }
        }

        //Get:Edit ERP Task Scheduler.
        [HttpGet]
        public virtual ActionResult EditScheduler(int erpTaskSchedulerId = 0, string indexName = "", string schedulerCallFor = "", int portalId = 0, int catalogId = 0, int catalogIndexId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return Equals(schedulerCallFor, ZnodeConstant.PublishCatalog) ? RedirectToAction<CatalogController>(x => x.CatalogList(null)) : action;

            if (erpTaskSchedulerId == 0)
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorEmptyTouchPoint));
                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }

            ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.GetTaskSchedulerDataForUpdate(erpTaskSchedulerId, indexName, schedulerCallFor, portalId, catalogId, catalogIndexId);

            return ActionView(createSchedulerView, erpTaskSchedulerViewModel);
        }

        //Get:Associated Catalog
        [HttpGet]
        public virtual JsonResult GetAssociatedCatalog(int pimProductId) => Json(new { catalogTree = _catalogAgent.GetAssociatedCatalogTree(pimProductId) }, JsonRequestBehavior.AllowGet);

        //Associate category to catalog .
        public virtual JsonResult AssociateCategoryToCatalog(CatalogAssociationViewModel catalogAssociationViewModel)
        {
            bool result = false;

            result = _catalogAgent.AssociateCategoryToCatalog(catalogAssociationViewModel);

            return Json(new { status = result, tree = _catalogAgent.GetTree(catalogAssociationViewModel.CatalogId) });
        }

        //Check Catalog already present in DB.
        [HttpGet]
        public virtual JsonResult IsCatalogCodeExists(string codeField)
       {
            bool isExist = true;
            if (!string.IsNullOrEmpty(codeField))
            {
                isExist = _helperAgent.IsCodeExists(codeField, CodeFieldService.CatalogService.ToString(), CodeFieldService.IsCodeExists.ToString());                
            }
            return Json(new { isExist = !isExist, message = Admin_Resources.ErrorCatalogCodeExist }, JsonRequestBehavior.AllowGet);
        }
        #endregion Public Methods      
    }
}