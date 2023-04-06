using MvcSiteMapProvider;
using System;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Areas.MediaManager.Attributes.Controllers
{
    public class AttributeGroupController : BaseController
    {
        #region Private Variables
        private IAttributeGroupAgent _attributeGroupAgent;
        private INavigationAgent _genericNavigationAgent;
        private ILocaleAgent _localeAgent;
        private readonly string controllerName = "AttributeGroup";
        private readonly string _AssociatedAttributeListView = "_AssociatedAttributeList";
        private readonly string _CreateEditAttributeGroupView = "CreateEditAttributeGroup";
        private const string MediaAttributeGroupId = "MediaAttributeGroupId";
        #endregion

        #region Constructor
        public AttributeGroupController(IAttributeGroupAgent attributeGroupAgent, INavigationAgent genericNavigationAgent, ILocaleAgent localeAgent)
        {
            _attributeGroupAgent = attributeGroupAgent;
            _genericNavigationAgent = genericNavigationAgent;
            _localeAgent = localeAgent;
        }
        #endregion

        #region Public Methods

        //Method to get attribute group list
        [MvcSiteMapNodeAttribute(Title = "$Resources:MediaManager_Resources.MediaManager_Resources, TitleAttributeGroups", Key = "AttributeGroup", Area = "MediaManager", ParentKey = "MediaManager")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeMediaAttributeGroup.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeMediaAttributeGroup.ToString(), model);
            AttributeGroupListViewModel attributeGroups = _attributeGroupAgent.GetAttributeGroups(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //get the grid model
            attributeGroups.GridModel = FilterHelpers.GetDynamicGridModel(model, attributeGroups.AttributeGroups, GridListType.ZnodeMediaAttributeGroup.ToString(), "", null, true, true, attributeGroups?.GridModel?.FilterColumn?.ToolMenuList);

            //set the total record count
            attributeGroups.GridModel.TotalRecordCount = attributeGroups.TotalResults;
            //returns the view
            return ActionView(attributeGroups);
        }

        //Method to render create page for attribute group
        [HttpGet]
        public virtual ActionResult Create()
            => ActionView(_CreateEditAttributeGroupView, new AttributeGroupViewModel());

        public virtual ActionResult EditAttributeGroupLocale(int id)
       => PartialView("~/Areas/MediaManager/Views/Attributes/_Locale.cshtml", _attributeGroupAgent.GetLocales(id));

        //Method to create attribute group
        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            AttributeGroupViewModel attribute = _attributeGroupAgent.Create(model);
            if (!attribute.HasError)
            {
                TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.RecordCreationSuccessMessage, NotificationType.success);
                return RedirectToAction<AttributeGroupController>(x => x.Edit(attribute.MediaAttributeGroupId));
            }
            TempData[AdminConstants.Notifications] = GenerateNotificationMessages(attribute.ErrorMessage, NotificationType.error);
            return View(_CreateEditAttributeGroupView, attribute);
        }

        //Method to render edit page for attribute group.
        [HttpGet]
        public virtual ActionResult Edit(int mediaAttributeGroupId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            AttributeGroupViewModel model = _attributeGroupAgent.GetAttributeGroup(mediaAttributeGroupId);
            model.NavigationViewModel = Request.IsAjaxRequest() ? _genericNavigationAgent.GetNavigationDetails(Convert.ToString(mediaAttributeGroupId), controllerName, ZnodeEntities.ZnodeMediaAttributeGroup.ToString(), "mediaAttributeGroupId", "MediaManager", AdminConstants.Edit, AdminConstants.Delete, AdminConstants.Edit) : new NavigationViewModel();
            model.ViewModeType = Request.IsAjaxRequest() ? AdminConstants.DetailsView : string.Empty;
            return ActionView(_CreateEditAttributeGroupView, model);
        }

        //Method to update attribute group
        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            TempData[AdminConstants.Notifications] = _attributeGroupAgent.UpdateAttributeGroup(model)
               ? GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success)
               : GenerateNotificationMessages(Admin_Resources.UpdateErrorMessage, NotificationType.error);
            return RedirectToAction<AttributeGroupController>(x => x.Edit(Convert.ToInt32(TempData[MediaAttributeGroupId])));
        }

        //Method to delete attribute group
        public virtual JsonResult Delete(string mediaAttributeGroupId)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(mediaAttributeGroupId))
            {
                bool status = _attributeGroupAgent.DeleteAttributeGroup(mediaAttributeGroupId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult AssignedAttributesList(int mediaAttributeGroupId, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            if (mediaAttributeGroupId > 0)
            {
                //Assign default view filter and sorting if exists for the first request.
                FilterHelpers.GetDefaultView(GridListType.AssociatedAttributes.ToString(), model);
                _attributeGroupAgent.SetFilters(model.Filters, mediaAttributeGroupId);
                AttributeGroupMapperListViewModel listViewModel = _attributeGroupAgent.GetAssignedAttributes(model.Filters, model.SortCollection, model.Expands, model.Page, model.RecordPerPage);
                TempData[MediaAttributeGroupId] = mediaAttributeGroupId;

                GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, listViewModel.AttributeGroupMappers, GridListType.AssociatedAttributes.ToString(), string.Empty, null);
                gridModel.TotalRecordCount = listViewModel.TotalResults;

                listViewModel.GridModel = gridModel;
                listViewModel.MediaAttributeGroupId = mediaAttributeGroupId;

                return PartialView(_AssociatedAttributeListView, listViewModel);
            }

            return RedirectToAction<AttributeGroupController>(x => x.List(null));
        }

        public virtual JsonResult AssociateAttributes(string selectedIds)
        {
            bool status = false;

            if (selectedIds?.Length > 0 && !Equals(TempData[MediaAttributeGroupId], null))
            {
                string message = string.Empty;
                status = _attributeGroupAgent.AssociateAttributes(selectedIds, (int)TempData[MediaAttributeGroupId], out message);
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.AssignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult RemoveAssociatedAttribute(int mediaAttributeGroupMapperId, int mediaAttributeGroupId, bool isSystemDefined)
        {
            if (mediaAttributeGroupMapperId > 0 && mediaAttributeGroupId > 0)
            {
                //Cannot delete system defined attributes.
                if (isSystemDefined)
                    return Json(new { status = false, message = Attributes_Resources.ErrorFailToDeleteSystemDefineAttribute }, JsonRequestBehavior.AllowGet);

                bool status = _attributeGroupAgent.RemoveAssociatedAttribute(mediaAttributeGroupMapperId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GetUnAssignedAttribute(int id)
            => PartialView("_UnassignedAttributes", _attributeGroupAgent.GetUnAssignedAttributes(id));

        public virtual ActionResult GetTabStructure(int mediaAttributeGroupId)
           => PartialView("~/Views/Shared/Controls/_TabStructure.cshtml", _attributeGroupAgent.CreateGroupTabStructure(mediaAttributeGroupId));

        //Check Group Code Already exists or not.
        [HttpPost]
        public virtual JsonResult IsGroupCodeExist(string groupCode, int mediaAttributeGroupId = 0)
          => Json(!_attributeGroupAgent.CheckGroupCodeExist(groupCode, mediaAttributeGroupId), JsonRequestBehavior.AllowGet);
        #endregion
    }
}