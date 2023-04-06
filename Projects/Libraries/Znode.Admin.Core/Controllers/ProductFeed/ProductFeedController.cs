using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class ProductFeedController : BaseController
    {
        #region Private Variables
        private readonly IProductFeedAgent _productFeedAgent;
        private readonly IERPTaskSchedulerAgent _erpTaskSchedulerAgent;
        private readonly IERPConfiguratorAgent _erpConfiguratorAgent;
        private const string createSchedulerView = "~/Views/TouchPointConfiguration/Create.cshtml";
        #endregion

        #region Public Constructor
        public ProductFeedController(IProductFeedAgent productFeedAgent, IERPTaskSchedulerAgent erpTaskSchedulerAgent, IERPConfiguratorAgent erpConfiguratorAgent)
        {
            _productFeedAgent = productFeedAgent;
            _erpTaskSchedulerAgent = erpTaskSchedulerAgent;
            _erpConfiguratorAgent = erpConfiguratorAgent;
        }
        #endregion

        #region Public Methods
        public virtual ActionResult CreateProductFeed() => View(AdminConstants.Create, _productFeedAgent.GetProductFeedMasterDetails());

        //Create product feed.
        [HttpPost]
        public virtual ActionResult CreateProductFeed(ProductFeedViewModel model)
        {
            if (HelperUtility.IsNull(model.PortalIds))
                SetNotificationMessage(GenerateNotificationMessages(Admin_Resources.ValidationStoreSelect, NotificationType.error));
            ModelState.Remove("PortalId");
            if (ModelState.IsValid)
            {
                string domainName = ZnodeAdminSettings.ZnodeApiRootUri;
                
                ProductFeedViewModel productFeedstatus = _productFeedAgent.CreateProductFeed(model, domainName);

                SetNotificationMessage((!string.IsNullOrEmpty(productFeedstatus.ErrorMessage) || productFeedstatus.SuccessXMLGenerationMessage.ToLower().IndexOf(".xml").Equals(-1)) ?
                    GetErrorNotificationMessage(string.IsNullOrEmpty(productFeedstatus.ErrorMessage) ? Admin_Resources.FailureXMLGenerationMessage : productFeedstatus.ErrorMessage) :
                    GetSuccessNotificationMessage(string.Format(Admin_Resources.SuccessXMLGenerationMessage, productFeedstatus.SuccessXMLGenerationMessage)));

                return RedirectToAction<ProductFeedController>(x => x.List(null));
            }
            return View(AdminConstants.Create, _productFeedAgent.SetProductFeedDetails(model));
        }

        //Check File name already exists or not.
        public virtual JsonResult IsFileNameExist(string name)
        {
            return Json(_productFeedAgent.CheckFileNameExist(name), JsonRequestBehavior.AllowGet);
        }

        //Get product feed list.
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeProductFeed.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeProductFeed.ToString(), model);
            ProductFeedListViewModel productFeedList = _productFeedAgent.GetProductFeedList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            productFeedList.GridModel = FilterHelpers.GetDynamicGridModel(model, productFeedList?.ProductFeeds, GridListType.ZnodeProductFeed.ToString(), string.Empty, null, true, true, productFeedList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            productFeedList.GridModel.TotalRecordCount = productFeedList.TotalResults;
            return ActionView(productFeedList);
        }

        //Action for show edit view.
        public virtual ActionResult Edit(int productFeedId)
        {
            ActionResult action = GotoBackURL();
            if (HelperUtility.IsNotNull(action))
                return action;

            return View(AdminConstants.Create, _productFeedAgent.GetProductFeedById(productFeedId));
        }

        //Action for edit product feed.
        [HttpPost]
        public virtual ActionResult Edit(ProductFeedViewModel model)
        {
            if (HelperUtility.IsNull(model.PortalIds))
                SetNotificationMessage(GenerateNotificationMessages(Admin_Resources.ValidationStoreSelect, NotificationType.error));
            ModelState.Remove("PortalId");
            if (ModelState.IsValid)
            {
                string domainName = ZnodeAdminSettings.ZnodeApiRootUri;
                ProductFeedViewModel updatedModel = _productFeedAgent.UpdateProductFeed(model, domainName);
                SetNotificationMessage((!string.IsNullOrEmpty(updatedModel.ErrorMessage) || updatedModel.SuccessXMLGenerationMessage.ToLower().IndexOf(".xml").Equals(-1)) ?
                    GetErrorNotificationMessage(Admin_Resources.FailureXMLGenerationMessage) :
                    GetSuccessNotificationMessage(string.Format(Admin_Resources.SuccessXMLGenerationMessage, updatedModel.SuccessXMLGenerationMessage)));

                return RedirectToAction<ProductFeedController>(x => x.List(null));
            }
            return View(AdminConstants.Create, _productFeedAgent.SetProductFeedDetails(model));
        }

        [HttpGet]
        //Delete product feed and Sitemap url from Robots.txt.
        public virtual JsonResult Delete(string productFeedId)
        {
            string message = string.Empty;
            
            if (!string.IsNullOrEmpty(productFeedId))
            {
                bool status = false;
                status = _productFeedAgent.DeleteProductFeed(productFeedId);

                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        //Get type method to Create new ERP Task Scheduler.
        [HttpGet]
        public virtual ActionResult CreateScheduler(string ConnectorTouchPoints, string indexName = "", string schedulerCallFor = "", int portalId = 0, int catalogId = 0, int catalogIndexId = 0)
        {
            if (!string.IsNullOrEmpty(ConnectorTouchPoints))
            {
                int erpTaskSchedulerId = _erpTaskSchedulerAgent.GetSchedulerIdByTouchPointName(ConnectorTouchPoints, schedulerCallFor);
                if (erpTaskSchedulerId == 0)
                {
                    ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.SetTaskSchedulerData(ConnectorTouchPoints, indexName, schedulerCallFor, portalId, catalogId, catalogIndexId);
                    return View(createSchedulerView, erpTaskSchedulerViewModel);
                }
                else
                    return RedirectToAction<ProductFeedController>(x => x.EditScheduler(erpTaskSchedulerId, indexName, schedulerCallFor, portalId,catalogId, catalogIndexId));
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorEmptyTouchPoint));
                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }
        }

        //Post type method to Create new ERP Task Scheduler.
        [HttpPost]
        public virtual ActionResult CreateScheduler(ERPTaskSchedulerViewModel erpTaskSchedulerViewModel)
        {
            bool status = false;
            _erpTaskSchedulerAgent.CheckValidation(erpTaskSchedulerViewModel, out status);

            if (erpTaskSchedulerViewModel.SchedulerFrequency == ZnodeConstant.OneTime)
            {
                ModelState.Remove("CronExpression");
            }
            if (ModelState.IsValid && status)
            {
                erpTaskSchedulerViewModel.DomainName = ZnodeAdminSettings.ZnodeApiRootUri;
                erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.Create(erpTaskSchedulerViewModel);
                if (!erpTaskSchedulerViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SchedulerCreatedSuccessfully));
                    return RedirectToAction<ProductFeedController>(x => x.EditScheduler(erpTaskSchedulerViewModel.ERPTaskSchedulerId, erpTaskSchedulerViewModel.IndexName, erpTaskSchedulerViewModel.SchedulerCallFor, erpTaskSchedulerViewModel.PortalId, erpTaskSchedulerViewModel.CatalogId, erpTaskSchedulerViewModel.CatalogIndexId));
                }
            }
            
            erpTaskSchedulerViewModel.ERPClassName = _erpConfiguratorAgent.GetERPClassName();            
            
            if (Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.SearchIndex))
                return Json(new { status = false, message = erpTaskSchedulerViewModel.ErrorMessage }, JsonRequestBehavior.AllowGet);
            else
                SetNotificationMessage(GetErrorNotificationMessage(erpTaskSchedulerViewModel.ErrorMessage));
            return View(createSchedulerView, erpTaskSchedulerViewModel);
        }

        //Get:Edit ERP Task Scheduler.
        [HttpGet]
        public virtual ActionResult EditScheduler(int erpTaskSchedulerId = 0, string indexName = "", string schedulerCallFor = "", int portalId = 0, int catalogId = 0, int portalIndexId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return Equals(schedulerCallFor, ZnodeConstant.ProductFeed) ? RedirectToAction<ProductFeedController>(x => x.List(null)) : action;

            if (erpTaskSchedulerId == 0)
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ErrorEmptyTouchPoint));
                return RedirectToAction<TouchPointConfigurationController>(x => x.List(null));
            }

            ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.GetTaskSchedulerDataForUpdate(erpTaskSchedulerId, indexName, schedulerCallFor, portalId, catalogId, portalIndexId);

            return View(createSchedulerView, erpTaskSchedulerViewModel);
        }

        //Post:Edit ERPTaskScheduler.
        [HttpPost]
        public virtual ActionResult EditScheduler(ERPTaskSchedulerViewModel erpTaskSchedulerViewModel)
        {
            bool status = false;
            _erpTaskSchedulerAgent.CheckValidation(erpTaskSchedulerViewModel, out status);
            if (erpTaskSchedulerViewModel.SchedulerFrequency == ZnodeConstant.OneTime)
            {
                ModelState.Remove("CronExpression");
            }
            if (ModelState.IsValid && status)
            {
                erpTaskSchedulerViewModel.DomainName = ZnodeAdminSettings.ZnodeApiRootUri;
                erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.Create(erpTaskSchedulerViewModel);
                if (!erpTaskSchedulerViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SchedulerUpdatedSuccessfully));
                    return RedirectToAction<ProductFeedController>(x => x.EditScheduler(erpTaskSchedulerViewModel.ERPTaskSchedulerId, erpTaskSchedulerViewModel.IndexName, erpTaskSchedulerViewModel.SchedulerCallFor, erpTaskSchedulerViewModel.PortalId, erpTaskSchedulerViewModel.CatalogId, erpTaskSchedulerViewModel.CatalogIndexId));
                }
            }
            erpTaskSchedulerViewModel.ERPClassName = _erpConfiguratorAgent.GetERPClassName();
            
            if (Equals(erpTaskSchedulerViewModel.SchedulerCallFor, ZnodeConstant.SearchIndex))
                return Json(new { status = !erpTaskSchedulerViewModel.HasError, message = erpTaskSchedulerViewModel.ErrorMessage }, JsonRequestBehavior.AllowGet);
            else
                SetNotificationMessage(GetErrorNotificationMessage(erpTaskSchedulerViewModel.ErrorMessage));
            return View(createSchedulerView, erpTaskSchedulerViewModel);
        }

        //Generate product feed link.
        [HttpGet]
        public virtual JsonResult GenerateProductFeedLink(int productFeedId)
        {
            string domainName = ZnodeAdminSettings.ZnodeApiRootUri;
            ProductFeedViewModel updatedModel = _productFeedAgent.GenerateProductFeedLink(productFeedId, domainName);
            bool status = HelperUtility.IsNotNull(updatedModel) ? !string.IsNullOrEmpty(updatedModel.ErrorMessage) || updatedModel.SuccessXMLGenerationMessage.ToLower().IndexOf(".xml").Equals(-1) : true;
            return Json(new
            {
                success = !status,
                message = (status ?
                Admin_Resources.FailureXMLGenerationMessage :
                string.Format(Admin_Resources.SuccessXMLGenerationMessage, updatedModel.SuccessXMLGenerationMessage))
            }, JsonRequestBehavior.AllowGet);
        }

        //Check if file name combination already exists.
        [HttpGet]
        public virtual JsonResult IsFileNameCombinationAlreadyExist(int localeId, string fileName)
            => Json(new { data =_productFeedAgent.FileNameCombinationAlreadyExist(localeId, fileName) }, JsonRequestBehavior.AllowGet);
        
        #endregion
    }
}