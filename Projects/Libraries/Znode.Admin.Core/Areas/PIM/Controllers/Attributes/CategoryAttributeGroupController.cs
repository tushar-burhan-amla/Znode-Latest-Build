using MvcSiteMapProvider;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Areas.PIM.Controllers.Attributes
{
    public class CategoryAttributeGroupController : BaseController
    {
        #region Private Variables
        private IPIMAttributeGroupAgent _pimAttributeGroupAgent;

        public readonly string createEditPath = "~/Areas/PIM/Views/ProductAttributeGroup/CreateEdit.cshtml";
        public readonly string assignedAttributeView = "~/Areas/PIM/Views/ProductAttributeGroup/_AssignedAttribute.cshtml";
        public readonly string unassignedAttributeView = "~/Areas/PIM/Views/ProductAttributeGroup/_UnassignedAttributes.cshtml";
        public readonly string listView = "~/Areas/PIM/Views/ProductAttributeGroup/List.cshtml";
        #endregion

        #region Public Constructor
        public CategoryAttributeGroupController(IPIMAttributeGroupAgent pIMAttributeGroupAgent)
        {
            _pimAttributeGroupAgent = pIMAttributeGroupAgent;
        }
        #endregion

        [HttpGet]
        public virtual ActionResult Create()
            => ActionView(createEditPath, new PIMAttributeGroupViewModel() { IsCategory = true });

        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            PIMAttributeGroupViewModel attributeGroup = _pimAttributeGroupAgent.Create(model);

            if (!attributeGroup.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                return RedirectToAction<CategoryAttributeGroupController>(x => x.Edit(attributeGroup.PimAttributeGroupId));
            }

            SetNotificationMessage(GetErrorNotificationMessage(attributeGroup.ErrorMessage));
            return View(createEditPath, attributeGroup);
        }

        public virtual ActionResult Edit(int pimAttributeGroupId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(createEditPath, _pimAttributeGroupAgent.Get(pimAttributeGroupId));
        }
        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            PIMAttributeGroupViewModel updatedModel = _pimAttributeGroupAgent.UpdateAttributeGroup(model);
            SetNotificationMessage(updatedModel.HasError
                ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
                : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
            return RedirectToAction<CategoryAttributeGroupController>(x => x.Edit(updatedModel.PimAttributeGroupId));
        }

        public virtual ActionResult CreateAttributeGroupLocale(int id)
            => PartialView(AdminConstants.LocaleTabPath, _pimAttributeGroupAgent.GetLocales(id));

        public virtual ActionResult GetTabStructure(int pimAttributeGroupId)
            => PartialView(AdminConstants.TabStructurePath, _pimAttributeGroupAgent.GetTabStructure(pimAttributeGroupId, true));

        public virtual ActionResult GetAssignedAttribute(int id, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            if (id > 0)
            {
                //Assign default view filter and sorting if exists for the first request.
                FilterHelpers.GetDefaultView(GridListType.ZnodeCategoryAssignedAttributes.ToString(), model);

                _pimAttributeGroupAgent.SetFilters(model.Filters, id);
                PIMAttributeGroupMapperListViewModel listViewModel = _pimAttributeGroupAgent.GetAssignedAttributes(model.Filters, model.SortCollection, null, model.Page, model.RecordPerPage);
                listViewModel.PimAttributeGroupId = id;
                TempData[AdminConstants.PIMAttributeGroupId] = id;
                GridModel gridModel = FilterHelpers.GetDynamicGridModel(model, listViewModel.AttributeGroupMappers, GridListType.ZnodeCategoryAssignedAttributes.ToString(), "", null);
                gridModel.TotalRecordCount = listViewModel.TotalResults;
                listViewModel.GridModel = gridModel;

                return PartialView(assignedAttributeView, listViewModel);
            }
            return null;
        }

        public virtual ActionResult GetUnAssignedAttribute(int id)
            => PartialView(unassignedAttributeView, _pimAttributeGroupAgent.GetUnAssignedAttributes(id, true));

        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,TitleAttributeGroups", Key = "CategoryAttributeGroup", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCategoryAttributeGroup.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCategoryAttributeGroup.ToString(), model);

            //Sets the IsCategory filter
            HelperMethods.SetIsCategoryFilters(model.Filters, "1");

            //Get the list of PIM attribute group list.
            PIMAttributeGroupListViewModel pimAttributeGroupList = _pimAttributeGroupAgent.GetPIMAttributeGroups(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            pimAttributeGroupList?.GridModel?.FilterColumn?.ToolMenuList?.Select(x => { x.ControllerName = "CategoryAttributeGroup"; return x; })?.ToList();

            //Get the grid model.
            pimAttributeGroupList.GridModel = FilterHelpers.GetDynamicGridModel(model, pimAttributeGroupList.AttributeGroupList, GridListType.ZnodeCategoryAttributeGroup.ToString(), string.Empty, null, true, true, pimAttributeGroupList?.GridModel?.FilterColumn?.ToolMenuList);
            pimAttributeGroupList.GridModel.TotalRecordCount = pimAttributeGroupList.TotalResults;
            //Returns the attribute group list.
            return ActionView(listView, pimAttributeGroupList);
        }

        public virtual JsonResult AssociateAttributes(string selectedIds)
        {
            bool status = false;
            if (selectedIds?.Length > 0 && HelperUtility.IsNotNull(TempData[AdminConstants.PIMAttributeGroupId]))
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

                bool status = _pimAttributeGroupAgent.RemoveAssociatedAttribute(pimAttributeGroupId, pimAttributeId, true, out message);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult Delete(string pimAttributeGroupId)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(pimAttributeGroupId))
            {
                bool status = _pimAttributeGroupAgent.Delete(pimAttributeGroupId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        //Update Attribute Display Order for category
        public virtual ActionResult UpdateAttributeDisplayOrder(int pimAttributeId, string data)
        {
            PIMAttributeDataViewModel attribute = _pimAttributeGroupAgent.UpdateAttributeDisplayOrder(pimAttributeId, data);
            if (!attribute.HasError)
                return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }
    }
}