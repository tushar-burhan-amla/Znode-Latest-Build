using MvcSiteMapProvider;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Areas.PIM.Attributes.Controllers
{
    public class ProductAttributeGroupController : BaseController
    {
        #region Private Variables
        private IPIMAttributeGroupAgent _pimAttributeGroupAgent;
        #endregion

        #region Public Constructor
        public ProductAttributeGroupController(IPIMAttributeGroupAgent pIMAttributeGroupAgent)
        {
            _pimAttributeGroupAgent = pIMAttributeGroupAgent;
        }
        #endregion

        [HttpGet]
        public virtual ActionResult Create()
            => ActionView(AdminConstants.CreateEdit, new PIMAttributeGroupViewModel());

        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            PIMAttributeGroupViewModel attributeGroup = _pimAttributeGroupAgent.Create(model);
            if (!attributeGroup.HasError)
            {
                TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.RecordCreationSuccessMessage, NotificationType.success);
                return RedirectToAction<ProductAttributeGroupController>(x => x.Edit(attributeGroup.PimAttributeGroupId));
            }
            TempData[AdminConstants.Notifications] = GenerateNotificationMessages(attributeGroup.ErrorMessage, NotificationType.error);
            return View(AdminConstants.CreateEdit, attributeGroup);
        }

        public virtual ActionResult Edit(int pimAttributeGroupId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(AdminConstants.CreateEdit, _pimAttributeGroupAgent.Get(pimAttributeGroupId));
        }
        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            PIMAttributeGroupViewModel updatedModel = _pimAttributeGroupAgent.UpdateAttributeGroup(model);
            TempData[AdminConstants.Notifications] = updatedModel.HasError
                ? GenerateNotificationMessages(Admin_Resources.UpdateErrorMessage, NotificationType.error)
                : GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success);
            return RedirectToAction<ProductAttributeGroupController>(x => x.Edit(updatedModel.PimAttributeGroupId));
        }

        public virtual ActionResult CreateAttributeGroupLocale(int id)
            => PartialView(AdminConstants.LocaleTabPath, _pimAttributeGroupAgent.GetLocales(id));

        public virtual ActionResult GetTabStructure(int pimAttributeGroupId)
            => PartialView(AdminConstants.TabStructurePath, _pimAttributeGroupAgent.GetTabStructure(pimAttributeGroupId, false));

        public virtual ActionResult GetAssignedAttribute(int id, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            if (id > 0)
            {
                //Assign default view filter and sorting if exists for the first request.
                FilterHelpers.GetDefaultView(GridListType.ZnodePimAssignedAttributes.ToString(), model);

                _pimAttributeGroupAgent.SetFilters(model.Filters, id);
                PIMAttributeGroupMapperListViewModel listViewModel = _pimAttributeGroupAgent.GetAssignedAttributes(model.Filters, model.SortCollection, null, model.Page, model.RecordPerPage);
                listViewModel.PimAttributeGroupId = id;
                TempData[AdminConstants.PIMAttributeGroupId] = id;
                listViewModel.IsNonEditable = _pimAttributeGroupAgent.Get(id).IsNonEditable;
                GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, listViewModel.AttributeGroupMappers, GridListType.ZnodePimAssignedAttributes.ToString(), "", null);
                gridModel.TotalRecordCount = listViewModel.TotalResults;
                listViewModel.GridModel = gridModel;

                return ActionView("_AssignedAttribute", listViewModel);
            }
            return null;
        }

        public virtual ActionResult GetUnAssignedAttribute(int id)
            => PartialView("_UnassignedAttributes", _pimAttributeGroupAgent.GetUnAssignedAttributes(id, false));

        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,TitleAttributeGroups", Key = "ProductAttributeGroup", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodePimAttributeGroup.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePimAttributeGroup.ToString(), model);

            //Sets the IsCategory filter
            HelperMethods.SetIsCategoryFilters(model.Filters, "0");

            //Get the list of PIM attribute group list.
            PIMAttributeGroupListViewModel pimAttributeGroupList = _pimAttributeGroupAgent.GetPIMAttributeGroups(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            pimAttributeGroupList?.GridModel?.FilterColumn?.ToolMenuList?.Select(x => { x.ControllerName = "ProductAttributeGroup"; return x; })?.ToList();

            //Get the grid model.
            pimAttributeGroupList.GridModel = FilterHelpers.GetDynamicGridModel(model, pimAttributeGroupList.AttributeGroupList, GridListType.ZnodePimAttributeGroup.ToString(), string.Empty, null, true, true, pimAttributeGroupList?.GridModel?.FilterColumn?.ToolMenuList);
            pimAttributeGroupList.GridModel.TotalRecordCount = pimAttributeGroupList.TotalResults;
            //Returns the attribute group list.
            return ActionView(AdminConstants.ListView, pimAttributeGroupList);
        }
        [HttpGet]
        public virtual JsonResult AssociateAttributes(string selectedIds)
        {
            bool status = false;
            if (selectedIds?.Length > 0 && !Equals(TempData[AdminConstants.PIMAttributeGroupId], null))
            {
                string message = string.Empty;
                status = _pimAttributeGroupAgent.AssociateAttributes(selectedIds, (int)TempData[AdminConstants.PIMAttributeGroupId], out message);
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.AssignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult RemoveAssociatedAttribute(int pimAttributeId, int pimAttributeGroupId, bool isSystemDefined)
        {
            string message = string.Empty;
            if (pimAttributeId > 0 && pimAttributeGroupId > 0)
            {
                if (isSystemDefined)
                    return Json(new { status = false, message = Attributes_Resources.ErrorFailToDeleteSystemDefineAttribute }, JsonRequestBehavior.AllowGet);

                bool status = _pimAttributeGroupAgent.RemoveAssociatedAttribute(pimAttributeGroupId, pimAttributeId, false, out message);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult Delete(string pimAttributeGroupId)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(pimAttributeGroupId))
            {
                bool status = _pimAttributeGroupAgent.Delete(pimAttributeGroupId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult UpdateAttributeDisplayOrder(int pimAttributeId, string data)
        {
            PIMAttributeDataViewModel attribute = _pimAttributeGroupAgent.UpdateAttributeDisplayOrder(pimAttributeId, data);
            if (!attribute.HasError)
                return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Check Group Code Already exists or not.
        [HttpPost]
        public virtual JsonResult IsGroupCodeExist(string GroupCode, bool IsCategory, int PimAttributeGroupId = 0)
          => Json(!_pimAttributeGroupAgent.CheckGroupCodeExist(GroupCode, IsCategory, PimAttributeGroupId), JsonRequestBehavior.AllowGet);
    }
}