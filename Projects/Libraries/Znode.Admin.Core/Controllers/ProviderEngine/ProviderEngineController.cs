using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class ProviderEngineController : BaseController
    {
        #region Private Variables
        private readonly IProviderEngineAgent _providerEngineTypeAgent;
        private readonly IERPConfiguratorAgent _eRPConfiguratorAgent;
        #endregion

        #region Constructor
        public ProviderEngineController(IProviderEngineAgent providerEngineTypeAgent, IERPConfiguratorAgent eRPConfiguratorAgent)
        {
            _providerEngineTypeAgent = providerEngineTypeAgent;
            _eRPConfiguratorAgent = eRPConfiguratorAgent;
        }
        #endregion

        #region Public Action Methods

        #region TaxRule Type
        //Get the list of all Tax rule types having parameter fillter collection model.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,LabelAdmin", Key = "ProviderEngine", Area = "", ParentKey = "Admin")]
        public virtual ActionResult TaxRuleTypeList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeProviderEngine.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeProviderEngine.ToString(), model);
            //Get the list of tax Rule Types.
            ProviderEngineListViewModel taxRuleTypeList = _providerEngineTypeAgent.GetTaxRuleTypeList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            taxRuleTypeList.GridModel = FilterHelpers.GetDynamicGridModel(model, taxRuleTypeList.ProvideEngineTypes, GridListType.ZnodeProviderEngine.ToString(), string.Empty, null, true, true, taxRuleTypeList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            taxRuleTypeList.GridModel.TotalRecordCount = taxRuleTypeList.TotalResults;
            //Returns the attribute list view
            return ActionView(AdminConstants.ListView, taxRuleTypeList.GridModel);
        }

        //Get type method to Create new tax rule type.
        [HttpGet]
        public virtual ActionResult CreateTaxRuleType()
            => View(AdminConstants.CreateEdit, new ProviderEngineViewModel { IsActive = true, ProviderEngineClasses = _providerEngineTypeAgent.GetAllTaxRuleTypesNotInDatabase() });

        //Post type method to Create new tax rule type.
        [HttpPost]
        public virtual ActionResult CreateTaxRuleType(ProviderEngineViewModel taxRuleTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                taxRuleTypeViewModel = _providerEngineTypeAgent.CreateTaxRuleType(taxRuleTypeViewModel);
                SetNotificationMessage(taxRuleTypeViewModel?.Id > 0 ?
                    GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage) : GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
                return RedirectToAction<ProviderEngineController>(x => x.EditTaxRuleType((taxRuleTypeViewModel.Id)));
            }
            _providerEngineTypeAgent.GetAllTaxRuleTypesNotInDatabase();
            return View(AdminConstants.CreateEdit, taxRuleTypeViewModel);
        }

        //Get type method to Edit new tax rule type.
        public virtual ActionResult EditTaxRuleType(int id)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return View(AdminConstants.CreateEdit, _providerEngineTypeAgent.GetTaxRuleType(id));
        }

        //Post type method to Edit new tax rule type.
        [HttpPost]
        public virtual ActionResult EditTaxRuleType(ProviderEngineViewModel taxRuleTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                ProviderEngineViewModel providerEngineViewModel = _providerEngineTypeAgent.UpdateTaxRuleType(taxRuleTypeViewModel);
                if (IsNotNull(providerEngineViewModel))
                    SetNotificationMessage(providerEngineViewModel.HasError ? GetErrorNotificationMessage(providerEngineViewModel.ErrorMessage) : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateError));

                return RedirectToAction<ProviderEngineController>(x => x.EditTaxRuleType(taxRuleTypeViewModel.Id));
            }
            return View(AdminConstants.CreateEdit, taxRuleTypeViewModel);
        }

        //Delete tax rule type.
        public virtual ActionResult DeleteTaxRuleType(string id)
        {
            bool status = false;
            string message = Admin_Resources.ErrorFailedToDelete;
            if (!string.IsNullOrEmpty(id))
            {
                status = _providerEngineTypeAgent.DeleteTaxRuleType(id, out message);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get tax rule type by class name.
        [HttpGet]
        public virtual JsonResult GetTaxRuleTypeByClassName(string className)
            => Json(_providerEngineTypeAgent.GetTaxRuleTypeByClassName(className), JsonRequestBehavior.AllowGet);

        //enable/disable bulky tax rule types. 
        public virtual ActionResult BulkEnableDisableTaxRuleTypes(string id, bool isEnable)
        {
            bool status = false;
            string message = string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                status = _providerEngineTypeAgent.BulkEnableDisableTaxRuleTypes(id, isEnable, out message);
                if (status && isEnable)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.EnableMessage));
                else if (status && !isEnable)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.DisableMessage));
                else if (!status && !string.IsNullOrEmpty(message))
                    SetNotificationMessage(GetErrorNotificationMessage(message));
            }

            return RedirectToAction<ProviderEngineController>(x => x.TaxRuleTypeList(null));
        }
        #endregion      

        #region Shipping Type
        //Get the list of all shipping types having parameter fillter collection model.
        public virtual ActionResult ShippingTypeList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeShippingType.ToString(), model);
            //Get the list of Shipping Types.
            ProviderEngineListViewModel shippingTypeList = _providerEngineTypeAgent.GetShippingTypeList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            shippingTypeList.GridModel = FilterHelpers.GetDynamicGridModel(model, shippingTypeList.ProvideEngineTypes, GridListType.ZnodeShippingType.ToString(), string.Empty, null, true, true, shippingTypeList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            shippingTypeList.GridModel.TotalRecordCount = shippingTypeList.TotalResults;
            //Returns the attribute list view
            return ActionView(AdminConstants.ListView, shippingTypeList.GridModel);
        }

        //Get type method to Create new shipping type.
        [HttpGet]
        public virtual ActionResult CreateShippingType()
            => View(AdminConstants.CreateEdit, new ProviderEngineViewModel { IsActive = true, ProviderEngineClasses = _providerEngineTypeAgent.GetAllShippingTypesNotInDatabase() });

        //post type method to Create new shipping type.
        [HttpPost]
        public virtual ActionResult CreateShippingType(ProviderEngineViewModel shippingTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                shippingTypeViewModel = _providerEngineTypeAgent.CreateShippingType(shippingTypeViewModel);
                if (IsNotNull(shippingTypeViewModel))
                {
                    SetNotificationMessage(shippingTypeViewModel.Id > 0
                    ? GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage)
                    : GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
                    return RedirectToAction<ProviderEngineController>(x => x.EditShippingType(shippingTypeViewModel.Id));
                }               
            }
            _providerEngineTypeAgent.GetAllShippingTypesNotInDatabase();
            return View(AdminConstants.CreateEdit, shippingTypeViewModel);
        }

        //Get type method to Edit new shipping type.
        public virtual ActionResult EditShippingType(int id)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return View(AdminConstants.CreateEdit, _providerEngineTypeAgent.GetShippingType(id));
        }

        //Post type method to Edit new shipping type.
        [HttpPost]
        public virtual ActionResult EditShippingType(ProviderEngineViewModel shippingTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                ProviderEngineViewModel providerEngineViewModel = _providerEngineTypeAgent.UpdateShippingType(shippingTypeViewModel);
                if (IsNotNull(providerEngineViewModel))
                    SetNotificationMessage(providerEngineViewModel.HasError ? GetErrorNotificationMessage(providerEngineViewModel.ErrorMessage) : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateError));

                return RedirectToAction<ProviderEngineController>(x => x.EditShippingType(shippingTypeViewModel.Id));
            }
            return View(AdminConstants.CreateEdit, shippingTypeViewModel);
        }

        //Delete shipping type.
        public virtual JsonResult DeleteShippingType(string id)
        {
            bool status = false;
            string message = Admin_Resources.ErrorFailedToDelete;
            if (!string.IsNullOrEmpty(id))
            {
                status = _providerEngineTypeAgent.DeleteShippingType(id, out message);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get shipping type by class name.
        [HttpGet]
        public virtual JsonResult GetShippingTypeByClassName(string className)
          => Json(_providerEngineTypeAgent.GetShippingTypeByClassName(className), JsonRequestBehavior.AllowGet);

        //Enable/Disable bulky shipping types.
        public virtual ActionResult BulkEnableDisableShippingTypes(string id, bool isEnable)
        {
            bool status = false;
            string message = string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                status = _providerEngineTypeAgent.BulkEnableDisableShippingTypes(id, isEnable, out message);
                if (status && isEnable)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.EnableMessage));
                else if (status && !isEnable)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.DisableMessage));
                else if (!status && !string.IsNullOrEmpty(message))
                    SetNotificationMessage(GetErrorNotificationMessage(message));
            }
            return RedirectToAction<ProviderEngineController>(x => x.ShippingTypeList(null));
        }
        #endregion

        #region Promotion Type
        //Get the list of all promotion types having parameter fillter collection model.
        public virtual ActionResult PromotionTypeList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePromotionType.ToString(), model);
            //Get the list of tax Rule Types.
            ProviderEngineListViewModel promotionTypeList = _providerEngineTypeAgent.GetPromotionTypeList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            promotionTypeList.GridModel = FilterHelpers.GetDynamicGridModel(model, promotionTypeList.ProvideEngineTypes, GridListType.ZnodePromotionType.ToString(), string.Empty, null, true, true, promotionTypeList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            promotionTypeList.GridModel.TotalRecordCount = promotionTypeList.TotalResults;
            //Returns the attribute list view
            return ActionView(AdminConstants.ListView, promotionTypeList.GridModel);
        }

        //Get type method to Create new promotion type.
        [HttpGet]
        public virtual ActionResult CreatePromotionType()
            => View(AdminConstants.CreateEdit, new ProviderEngineViewModel { IsActive = true, ProviderEngineClasses = _providerEngineTypeAgent.GetAllPromotionTypesNotInDatabase() });

        //Post type method to Create new promotion type.
        [HttpPost]
        public virtual ActionResult CreatePromotionType(ProviderEngineViewModel promotionTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                promotionTypeViewModel = _providerEngineTypeAgent.CreatePromotionType(promotionTypeViewModel);
                SetNotificationMessage(promotionTypeViewModel?.Id > 0
                    ? GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage)
                    : GetErrorNotificationMessage(Admin_Resources.ErrorFailedToCreate));
                return RedirectToAction<ProviderEngineController>(x => x.EditPromotionType(promotionTypeViewModel.Id));
            }
            _providerEngineTypeAgent.GetAllPromotionTypesNotInDatabase();
            return View(AdminConstants.CreateEdit, promotionTypeViewModel);
        }

        //Get type method to Edit new promotion type.
        public virtual ActionResult EditPromotionType(int id)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return View(AdminConstants.CreateEdit, _providerEngineTypeAgent.GetPromotionType(id));
        }

        //Post type method to Edit new promotion type.
        [HttpPost]
        public virtual ActionResult EditPromotionType(ProviderEngineViewModel promotionTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                promotionTypeViewModel = _providerEngineTypeAgent.UpdatePromotionType(promotionTypeViewModel);
                SetNotificationMessage(IsNotNull(promotionTypeViewModel)
                    ? !promotionTypeViewModel.HasError ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                    : GetErrorNotificationMessage(promotionTypeViewModel.ErrorMessage) : GetErrorNotificationMessage(Admin_Resources.UpdateError));
                return RedirectToAction<ProviderEngineController>(x => x.EditPromotionType(promotionTypeViewModel.Id));
            }
            return View(AdminConstants.CreateEdit, promotionTypeViewModel);
        }

        //Delete Promotion Type.
        public virtual JsonResult DeletePromotionType(string id)
        {
            bool status = false;
            string message = Admin_Resources.ErrorFailedToDelete;
            if (!string.IsNullOrEmpty(id))
            {
                status = _providerEngineTypeAgent.DeletePromotionType(id, out message);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get promotion type by class name.
        [HttpGet]
        public virtual JsonResult GetPromotionTypeByClassName(string className)
          => Json(_providerEngineTypeAgent.GetPromotionTypeByClassName(className), JsonRequestBehavior.AllowGet);

        //Enable/Disable bulky promotion types.
        public virtual ActionResult BulkEnableDisablePromotionTypes(string id, bool isEnable)
        {
            bool status = false;
            string message = Admin_Resources.EnableErrorMessage;
            if (!string.IsNullOrEmpty(id))
            {
                status = _providerEngineTypeAgent.BulkEnableDisablePromotionTypes(id, isEnable, out message);
                if (status && isEnable)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.EnableMessage));
                else if (status && !isEnable)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.DisableMessage));
                else if (!status)
                    SetNotificationMessage(GetErrorNotificationMessage(message));
            }
            return RedirectToAction<ProviderEngineController>(x => x.PromotionTypeList(null));
        }
        #endregion

        #region ERP Configurator
        public virtual ActionResult ERPConfiguratorList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeERPConfigurator.ToString(), model);
            //Get the list of ERPConfigurator.
            ERPConfiguratorListViewModel eRPConfiguratorList = _eRPConfiguratorAgent.GetERPConfiguratorList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, eRPConfiguratorList.ERPConfiguratorList, GridListType.ZnodeERPConfigurator.ToString(), string.Empty, null, true, true, eRPConfiguratorList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            gridModel.TotalRecordCount = eRPConfiguratorList.TotalResults;
            //Returns the attribute list view
            return ActionView(AdminConstants.ListView, gridModel);
        }

        //Get type method to Create new ERP Configurator.
        [HttpGet]
        public virtual ActionResult CreateERPConfigurator()
            => View("/Views/ERPConfigurator/Create.cshtml", new ERPConfiguratorViewModel { IsActive = false, ERPConfiguratorClasses = _eRPConfiguratorAgent.GetAllERPConfiguratorClassesNotInDatabase() });

        //Post type method to Create new ERP Configurator.
        [HttpPost]
        public virtual ActionResult CreateERPConfigurator(ERPConfiguratorViewModel eRPConfiguratorViewModel)
        {
            if (ModelState.IsValid)
            {
                eRPConfiguratorViewModel = _eRPConfiguratorAgent.Create(eRPConfiguratorViewModel);

                if (!eRPConfiguratorViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<ProviderEngineController>(x => x.EditERPConfigurator(eRPConfiguratorViewModel.ERPConfiguratorId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(eRPConfiguratorViewModel.ErrorMessage));
            eRPConfiguratorViewModel.ERPConfiguratorClasses = _eRPConfiguratorAgent.GetAllERPConfiguratorClassesNotInDatabase();
            return View("/Views/ERPConfigurator/Create.cshtml", eRPConfiguratorViewModel);
        }

        //Get:Edit ERPConfigurator.
        [HttpGet]
        public virtual ActionResult EditERPConfigurator(int eRPConfiguratorId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView("/Views/ERPConfigurator/Edit.cshtml", _eRPConfiguratorAgent.GetERPConfigurator(eRPConfiguratorId));
        }

        //Post:Edit ERPConfigurator.
        [HttpPost]
        public virtual ActionResult EditERPConfigurator(ERPConfiguratorViewModel eRPConfiguratorViewModel)
        {
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_eRPConfiguratorAgent.Update(eRPConfiguratorViewModel).HasError
                ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
                : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<ProviderEngineController>(x => x.EditERPConfigurator(eRPConfiguratorViewModel.ERPConfiguratorId));
            }
            return View("/Views/ERPConfigurator/Edit.cshtml", eRPConfiguratorViewModel);
        }

        //Delete ERPConfigurator.
        public virtual JsonResult DeleteERPConfigurator(string eRPConfiguratorId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(eRPConfiguratorId))
            {
                status = _eRPConfiguratorAgent.Delete(eRPConfiguratorId, out message);

                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Enable/Disable ERP Configurator 
        public virtual ActionResult EnableDisableERPConfigurator(string eRPConfiguratorId, bool isActive)
        {
            if (!string.IsNullOrEmpty(eRPConfiguratorId))
            {
                bool status = _eRPConfiguratorAgent.EnableDisableERPConfigurator(eRPConfiguratorId, isActive);
                if (status && isActive)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.EnableMessage));
                else if (status && !isActive)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.DisableMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorMessageEnableDisable));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorMessageEnableDisable));
            return RedirectToAction<ProviderEngineController>(x => x.ERPConfiguratorList(null));
        }
        #endregion
        #endregion
    }
}