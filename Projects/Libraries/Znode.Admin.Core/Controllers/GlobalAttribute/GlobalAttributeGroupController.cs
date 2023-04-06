using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class GlobalAttributeGroupController : BaseController
    {
        #region Private Variables
        private readonly IGlobalAttributeGroupAgent _globalAttributeGroupAgent;
        #endregion

        #region Public Constructor
        public GlobalAttributeGroupController(IGlobalAttributeGroupAgent globalAttributeGroupAgent)
        {
            _globalAttributeGroupAgent = globalAttributeGroupAgent;
        }
        #endregion

        #region Public Methods
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleAttributeGroups", Key = "GlobalAttributeGroup", Area = "", ParentKey = "Admin")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int entityId = 0, string entityType = null)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeGlobalAttributeGroupList.ToString(), model);
            //Get the list of global attribute group list.
            GlobalAttributeGroupListViewModel globalAttributeGroupList = _globalAttributeGroupAgent.GetGlobalAttributeGroups(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, entityId, entityType);

            //Get the grid model.
            globalAttributeGroupList.GridModel = FilterHelpers.GetDynamicGridModel(model, globalAttributeGroupList.AttributeGroupList, GridListType.ZnodeGlobalAttributeGroupList.ToString(), string.Empty, null, true, true, globalAttributeGroupList?.GridModel?.FilterColumn?.ToolMenuList);
            globalAttributeGroupList.GridModel.TotalRecordCount = globalAttributeGroupList.TotalResults;

            //Returns the attribute group list.
            return ActionView(AdminConstants.ListView, globalAttributeGroupList);
        }

        //Create global attribute group.
        [HttpGet]
        public virtual ActionResult Create()
        {
            GlobalAttributeGroupViewModel model = new GlobalAttributeGroupViewModel();
            model.GlobalEntityType = _globalAttributeGroupAgent.GetGlobalEntityTypes();
            return ActionView(AdminConstants.CreateEdit, model);
        }

        //Save global attribute group details.
        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            GlobalAttributeGroupViewModel attributeGroup = _globalAttributeGroupAgent.Create(model);
            if (!attributeGroup.HasError)
            {
                TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.RecordCreationSuccessMessage, NotificationType.success);
                return RedirectToAction<GlobalAttributeGroupController>(x => x.Edit(attributeGroup.GlobalAttributeGroupId));
            }
            TempData[AdminConstants.Notifications] = GenerateNotificationMessages(attributeGroup.ErrorMessage, NotificationType.error);
            return View(AdminConstants.CreateEdit, attributeGroup);
        }

        //Edit global attribute group.
        public virtual ActionResult Edit(int globalAttributeGroupId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(AdminConstants.CreateEdit, _globalAttributeGroupAgent.Get(globalAttributeGroupId));
        }

        //Update global attribute group data.
        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            GlobalAttributeGroupViewModel updatedModel = _globalAttributeGroupAgent.UpdateAttributeGroup(model);
            TempData[AdminConstants.Notifications] = updatedModel.HasError
                ? GenerateNotificationMessages(Admin_Resources.UpdateErrorMessage, NotificationType.error)
                : GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success);
            return RedirectToAction<GlobalAttributeGroupController>(x => x.Edit(updatedModel.GlobalAttributeGroupId));
        }

        //Get tab structure for global attribute group.
        public virtual ActionResult GetTabStructure(int globalAttributeGroupId)
     => PartialView(AdminConstants.TabStructurePath, _globalAttributeGroupAgent.GetTabStructure(globalAttributeGroupId));

        //Save attribute group locale values.
        public virtual ActionResult CreateAttributeGroupLocale(int id)
         => PartialView("~/Views/GlobalAttribute/_Locale.cshtml", _globalAttributeGroupAgent.GetLocales(id));

        //Check Group Code Already exists or not.
        [HttpPost]
        public virtual JsonResult IsGroupCodeExist(string GroupCode, int globalAttributeGroupId = 0)
          => Json(!_globalAttributeGroupAgent.CheckGroupCodeExist(GroupCode, globalAttributeGroupId), JsonRequestBehavior.AllowGet);

        public virtual JsonResult Delete(string globalAttributeGroupId)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(globalAttributeGroupId))
            {
                bool status = _globalAttributeGroupAgent.Delete(globalAttributeGroupId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get assigned attributes list.
        public virtual ActionResult GetAssignedAttribute(int id, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            if (id > 0)
            {
                //Assign default view filter and sorting if exists for the first request.
                FilterHelpers.GetDefaultView(GridListType.ZnodeGlobalAssignedAttributes.ToString(), model);
                if (IsNotNull(model.Filters))
                {
                    //If global attribute group id already present in filters then remove it.
                    model.Filters.RemoveAll(x => x.Item1 == FilterKeys.GlobalAttributeGroupId);

                    //Add new global attribute group id filter.
                    model.Filters.Add(new FilterTuple(FilterKeys.GlobalAttributeGroupId, FilterOperators.Equals, id.ToString()));
                }

                GlobalAttributeGroupMapperListViewModel listViewModel = _globalAttributeGroupAgent.GetAssignedAttributes(model.Filters, model.SortCollection, null, model.Page, model.RecordPerPage);
                listViewModel.GlobalAttributeGroupId = id;
                TempData[AdminConstants.GlobalAttributeGroupId] = id;
                GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, listViewModel.AttributeGroupMappers, GridListType.ZnodeGlobalAssignedAttributes.ToString(), "", null);
                gridModel.TotalRecordCount = listViewModel.TotalResults;
                listViewModel.GridModel = gridModel;

                return ActionView("_AssignedAttribute", listViewModel);

            }
            return null;
        }

        //Get list of unassigned attributes.
        public virtual ActionResult GetUnAssignedAttribute(int id)
            => PartialView("_UnassignedAttributes", _globalAttributeGroupAgent.GetUnAssignedAttributes(id));

        //Associate global attributes.
        [HttpGet]
        public virtual JsonResult AssociateAttributes(string selectedIds)
        {
            bool status = false;
            if (selectedIds?.Length > 0 && IsNotNull(TempData[AdminConstants.GlobalAttributeGroupId]))
            {
                string message = string.Empty;
                status = _globalAttributeGroupAgent.AssociateAttributes(selectedIds, (int)TempData[AdminConstants.GlobalAttributeGroupId], out message);
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.AssignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        //Unassociate global attributes.
        public virtual ActionResult RemoveAssociatedAttribute(int globalAttributeId, int globalAttributeGroupId)
        {
            string message = string.Empty;
            if (globalAttributeId > 0 && globalAttributeGroupId > 0)
            {
                bool status = _globalAttributeGroupAgent.RemoveAssociatedAttribute(globalAttributeGroupId, globalAttributeId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        //Update display order.
        public virtual ActionResult UpdateAttributeDisplayOrder(int globalAttributeId, string data)
        {
            GlobalAttributeViewModel attribute = _globalAttributeGroupAgent.UpdateAttributeDisplayOrder(globalAttributeId, data);
            if (!attribute.HasError)
                return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
