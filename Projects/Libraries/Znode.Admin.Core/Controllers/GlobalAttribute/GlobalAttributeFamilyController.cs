using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class GlobalAttributeFamilyController : BaseController
    {
        #region Private Variables
        private readonly IGlobalAttributeFamilyAgent _globalAttributeFamilyAgent;
        private const string UnassignedAttributeGroups = "UnassignedAttributeGroups";
        private const string AssignedAttributeGroups = "AssignedAttributeGroups";
        #endregion

        #region Public Constructor
        public GlobalAttributeFamilyController(IGlobalAttributeFamilyAgent globalAttributeFamilyAgent)
        {
            _globalAttributeFamilyAgent = globalAttributeFamilyAgent;
        }
        #endregion


        // Get Attribute Family
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int entityId = 0, string entityType = null)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeGlobalAttributeFamilyList.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeGlobalAttributeFamilyList.ToString(), model);
            
            //Get the list of global attribute family list.
            GlobalAttributeFamilyListViewModel globalAttributeFamilyList = _globalAttributeFamilyAgent.List(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, entityId, entityType);

            //Get the grid model.
            globalAttributeFamilyList.GridModel = FilterHelpers.GetDynamicGridModel(model, globalAttributeFamilyList.AttributeFamilyList, GridListType.ZnodeGlobalAttributeFamilyList.ToString(), string.Empty, null, true, true, globalAttributeFamilyList?.GridModel?.FilterColumn?.ToolMenuList);
            globalAttributeFamilyList.GridModel.TotalRecordCount = globalAttributeFamilyList.TotalResults;

            //Returns the attribute family list.
            return ActionView(AdminConstants.ListView, globalAttributeFamilyList);
            
        }


        //Create attribute family.
        [HttpGet]
        public virtual ActionResult Create()
        {
            GlobalAttributeFamilyViewModel model = new GlobalAttributeFamilyViewModel();
            model.GlobalEntityType = _globalAttributeFamilyAgent.GetGlobalEntityTypes(true);
            return ActionView(AdminConstants.CreateEdit, model);
        }


        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            GlobalAttributeFamilyViewModel attributeFamily = _globalAttributeFamilyAgent.Create(model);
            attributeFamily.GlobalEntityType = _globalAttributeFamilyAgent.GetGlobalEntityTypes(true);
            if (!attributeFamily.HasError)
            {
                TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.RecordCreationSuccessMessage, NotificationType.success);
                return RedirectToAction<GlobalAttributeFamilyController>(x => x.Edit(attributeFamily.FamilyCode));
            }
            TempData[AdminConstants.Notifications] = GenerateNotificationMessages(attributeFamily.ErrorMessage, NotificationType.error);
            return View(AdminConstants.CreateEdit, attributeFamily);
        }

        //Edit Global Attribute Family
        public virtual ActionResult Edit(string familyCode)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(AdminConstants.CreateEdit, _globalAttributeFamilyAgent.Edit(familyCode));
        }

        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            GlobalAttributeFamilyViewModel viewModel = _globalAttributeFamilyAgent.UpdateAttributeFamily(model);
            TempData[AdminConstants.Notifications] = viewModel.HasError
                ? GenerateNotificationMessages(Admin_Resources.UpdateErrorMessage, NotificationType.error)
                : GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success);
            return RedirectToAction<GlobalAttributeFamilyController>(x => x.Edit(viewModel.FamilyCode));
        }

        //Delete Attribute Family
        public virtual JsonResult Delete(string globalAttributeFamilyId)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(globalAttributeFamilyId))
            {
                bool status = _globalAttributeFamilyAgent.DeleteAttributeFamily(globalAttributeFamilyId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get Assigned Groups
        public virtual ActionResult GetAssignedAttributeGroups(string familyCode)
        {
            if (!string.IsNullOrEmpty(familyCode))
            {
                GlobalAttributeGroupListViewModel listViewModel = _globalAttributeFamilyAgent.GetAssignedAttributeGroups(familyCode);
                listViewModel.FamilyCode = familyCode;
                return PartialView(AssignedAttributeGroups, listViewModel);
            }
            return PartialView(AssignedAttributeGroups, new GlobalAttributeGroupListViewModel());
        }

        //Get Unassigned Groups
        public virtual ActionResult GetUnassignedAttributeGroups(string familyCode)
        {
            TempData[AdminConstants.AttributeCode] = familyCode;
            return PartialView(UnassignedAttributeGroups, _globalAttributeFamilyAgent.GetUnassignedAttributeGroups(familyCode));
        }

        //Get Assigned Groups
        public virtual JsonResult AssignAttributeGroups(string selectedIds)
        {
            bool status = false;

            if (selectedIds?.Length > 0)
            {
                string message = string.Empty;
                status = _globalAttributeFamilyAgent.AssignAttributeGroups(selectedIds, (string)TempData[AdminConstants.AttributeCode], out message);
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.AssignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        //Unassign Attribute Groups
        public virtual JsonResult UnassignAttributeGroups(string groupCode, string familyCode)
        {
            bool status = false;
            string message = string.Empty;

            if (!string.IsNullOrEmpty(familyCode)  && !string.IsNullOrEmpty(groupCode) )
            {
                status = (_globalAttributeFamilyAgent.UnassignAttributeGroups(groupCode, familyCode, out message));
                return Json(new { status = status, Message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = status, Message = Admin_Resources.TextInvalidData }, JsonRequestBehavior.AllowGet);
        }

        //Update attribute group display order.
        public virtual ActionResult UpdateAttributeGroupDisplayOrder(string groupCode, string familyCode, int displayOrder = 0)
        {
            bool isDisplayOrderUpdated = _globalAttributeFamilyAgent.UpdateAttributeGroupDisplayOrder(groupCode, familyCode, displayOrder);

            if (isDisplayOrderUpdated)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
                return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        //Get Tab Structure
        public virtual ActionResult GetTabStructure(string familyCode)
            => PartialView(AdminConstants.TabStructurePath, _globalAttributeFamilyAgent.GetTabStructure(familyCode));

        //Create Attribute Family Locale
        public virtual ActionResult CreateAttributeFamilyLocale(string familyCode)
         => PartialView("~/Views/GlobalAttribute/_Locale.cshtml", _globalAttributeFamilyAgent.GetLocales(familyCode));

        //Verify the the family code already Exist
        [HttpGet]
        public virtual ActionResult IsFamilyCodeExist(string familyCode)
           => Json(new { data = _globalAttributeFamilyAgent.IsFamilyCodeExist(familyCode) }, JsonRequestBehavior.AllowGet);

    }
}
