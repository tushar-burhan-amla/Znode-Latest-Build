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
    public class AttributeFamilyController : BaseController
    {
        #region Private Variables
        private IAttributeFamilyAgent _attributeFamilyAgent;
        private INavigationAgent _genericNavigationAgent;
        private IAttributeGroupAgent _attributeGroupAgent;

        private readonly string controllerName = "AttributeFamily";
        private readonly string detailAction = "AttributeFamilyDetails";
        private const string MediaAttributeFamilyId = "MediaAttributeFamilyId";
        private readonly string _AttributeFamilyGroupListView = "~/Areas/MediaManager/Views/AttributeFamily/_AssignedAttributeGroupList.cshtml";
        private readonly string AttributeFamilyDetailsView = "~/Areas/MediaManager/Views/AttributeFamily/AttributeFamilyDetails.cshtml";
        #endregion

        #region Constructor
        public AttributeFamilyController(IAttributeFamilyAgent attributeFamilyAgent, INavigationAgent navigationAgent, IAttributeGroupAgent attributeGroupAgent)
        {
            _genericNavigationAgent = navigationAgent;
            _attributeGroupAgent = attributeGroupAgent;
            _attributeFamilyAgent = attributeFamilyAgent;
        }
        #endregion

        #region Public Action Methods
        [MvcSiteMapNode(Title = "$Resources:MediaManager_Resources.MediaManager_Resources, TitleAttributeFamily", Key = "AttributeFamily", Area = "MediaManager", ParentKey = "MediaManager")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeMediaAttributeFamily.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeMediaAttributeFamily.ToString(), model);
            AttributeFamilyListViewModel attributeFamilies = _attributeFamilyAgent.GetAttributeFamilies(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //get the grid model
            attributeFamilies.GridModel = FilterHelpers.GetDynamicGridModel(model, attributeFamilies.AttributeFamilies, GridListType.ZnodeMediaAttributeFamily.ToString(), "", null, true, true, attributeFamilies?.GridModel?.FilterColumn?.ToolMenuList);
            
            //set the total record count
            attributeFamilies.GridModel.TotalRecordCount = attributeFamilies.TotalResults;
            //returns the view
            return ActionView(attributeFamilies);
        }

        [HttpGet]
        public virtual ActionResult Create() => View(new AttributeFamilyViewModel());

        [HttpPost]
        public virtual ActionResult Create(AttributeFamilyViewModel attributeFamilyViewModel)
        {
            if (ModelState.IsValid)
            {
                attributeFamilyViewModel = _attributeFamilyAgent.CreateAttributeFamily(attributeFamilyViewModel);
                if (!attributeFamilyViewModel.HasError)
                {
                    TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.RecordCreationSuccessMessage, NotificationType.success);
                    return RedirectToAction<AttributeFamilyController>(x => x.Edit(attributeFamilyViewModel.MediaAttributeFamilyId));
                }
            }
            TempData[AdminConstants.Notifications] = GenerateNotificationMessages(attributeFamilyViewModel.ErrorMessage, NotificationType.error);
            return View(attributeFamilyViewModel);
        }

        public virtual ActionResult GetExistingFamilies()
         => PartialView("_ExistingAttributeFamilies", _attributeFamilyAgent.GetAttributeFamilyList());

        //Get method to assign attributeGroups to attribute family.
        //Returns a view to display already assigned attribute groups to attribute family.
        [HttpGet]
        public virtual ActionResult AssignedAttributeGroupList(int mediaAttributeFamilyId)
        {
            if (mediaAttributeFamilyId > 0)
            {
                //Get the list of already assigned attribute groups.
                AttributeGroupListViewModel listViewModel = _attributeFamilyAgent.GetAssignedAttributeGroups(mediaAttributeFamilyId);

                TempData[MediaAttributeFamilyId] = mediaAttributeFamilyId;
                return PartialView(_AttributeFamilyGroupListView, new AttributeGroupListViewModel()
                {
                    AttributeGroups = listViewModel.AttributeGroups,
                    AttributeFamilyId = mediaAttributeFamilyId
                });
            }
            return View(_AttributeFamilyGroupListView, new FamilyGroupAttributeViewModel());
        }

        public virtual ActionResult GetUnAssignedMediaAttributeGroups(int id)
           => PartialView("_UnAssignedMediaAttributeGroups", _attributeFamilyAgent.GetUnAssignedAttributeGroups(id));

        //Gets Assigned attributes for a group which is associated with Family.
        [HttpGet]
        public virtual ActionResult AssignedAttributes(int attributeGroupId, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            AttributeGroupMapperListViewModel attributeGroupMappers;
            if (attributeGroupId > 0)
            {
                _attributeGroupAgent.SetFilters(model.Filters, attributeGroupId);
                attributeGroupMappers = _attributeGroupAgent.GetAssignedAttributes(model.Filters, model.SortCollection, null, 1, int.MaxValue);
                return PartialView("_AttributeData", attributeGroupMappers);
            }
            return null;
        }

        [HttpGet]
        public virtual JsonResult AssignAttributeGroups(string selectedIds)
        {
            bool status = false;

            if (selectedIds?.Length > 0)
            {
                string message = string.Empty;
                status = _attributeFamilyAgent.AssignAttributeGroups(selectedIds, (int)TempData[MediaAttributeFamilyId], out message);
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.AssignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult UnAssignAttributeGroups(int attributeGroupId)
        {
            if (!Equals(TempData[MediaAttributeFamilyId], null) && attributeGroupId > 0)
            {
                string message = string.Empty;
                TempData[AdminConstants.Notifications] = _attributeFamilyAgent.UnAssignAttributeGroups((int)TempData[MediaAttributeFamilyId], attributeGroupId, out message)
                    ? GenerateNotificationMessages(Admin_Resources.UnassignSuccessful, NotificationType.success)
                    : GenerateNotificationMessages(message, NotificationType.error);
            }
            return RedirectToAction("Edit", "AttributeFamily", new { mediaAttributeFamilyId = Convert.ToInt32(TempData[MediaAttributeFamilyId]), area = "MediaManager" });
        }

        [HttpGet]
        public virtual ActionResult Edit(int mediaAttributeFamilyId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            ParentAttributeFamilyViewModel parent = new ParentAttributeFamilyViewModel();
            parent.AttributeFamily = _attributeFamilyAgent.GetAttributeFamily(mediaAttributeFamilyId);
            parent.navigationModel = Request.IsAjaxRequest() ? _genericNavigationAgent.GetNavigationDetails(mediaAttributeFamilyId.ToString(), controllerName, ZnodeEntities.ZnodeMediaAttributeFamily.ToString(), "mediaAttributeFamilyId", null, AdminConstants.Edit, AdminConstants.Delete, AdminConstants.Edit) : new NavigationViewModel();
            parent.ViewModeType = Request.IsAjaxRequest() ? AdminConstants.DetailsView : string.Empty;

            parent.TabViewModel = _attributeFamilyAgent.CreateTabStructure(mediaAttributeFamilyId);
            return ActionView(parent);
        }

        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            TempData[AdminConstants.Notifications] = _attributeFamilyAgent.SaveFamilyLocales(model)
                ? GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success)
                : GenerateNotificationMessages(Admin_Resources.UpdateErrorMessage, NotificationType.error);
            return RedirectToAction<AttributeFamilyController>(x => x.Edit(Convert.ToInt32(TempData[MediaAttributeFamilyId])));
        }

        public virtual JsonResult Delete(string mediaAttributeFamilyId)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(mediaAttributeFamilyId))
            {
                bool status = _attributeFamilyAgent.DeleteAttributeFamily(mediaAttributeFamilyId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult AttributeFamilyDetails(string Id)
        {
            AttributeFamilyViewModel model = new AttributeFamilyViewModel();
            if (!string.IsNullOrEmpty(Id))
            {
                model.NavigationViewModel = _genericNavigationAgent.GetNavigationDetails(Id, controllerName, ZnodeEntities.ZnodeMediaAttributeFamily.ToString(), AdminConstants.Edit, AdminConstants.Delete, detailAction);
                return View(AttributeFamilyDetailsView, model);
            }
            return View(AttributeFamilyDetailsView, model);
        }

        //Get: Family Locale values
        public virtual ActionResult Locale(int id = 0)
            => PartialView("_MediaFamilyLocale", _attributeFamilyAgent.GetLocales(id));
        #endregion

    }
}