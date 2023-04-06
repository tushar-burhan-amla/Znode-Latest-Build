using MvcSiteMapProvider;
using System;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Areas.PIM.Attributes.Controllers
{
    public class CategoryAttributeFamilyController : BaseController
    {
        #region Private Variables
        private IPIMAttributeFamilyAgent _pimAttributeFamilyAgent;
        private IPIMAttributeGroupAgent _pimAttributeGroupAgent;
        private readonly string _PIMAttributeFamilyGroupListView = "~/Areas/PIM/Views/ProductAttributeFamily/_AssignedPIMAttributeGroupList.cshtml";
        private readonly string createPath = "~/Areas/PIM/Views/ProductAttributeFamily/Create.cshtml";
        #endregion

        #region Public Constructor
        public CategoryAttributeFamilyController(IPIMAttributeFamilyAgent pIMAttributeFamilyAgent, IPIMAttributeGroupAgent pIMAttributeGroupAgent)
        {
            _pimAttributeFamilyAgent = pIMAttributeFamilyAgent;
            _pimAttributeGroupAgent = pIMAttributeGroupAgent;
        }
        #endregion

        #region Public Properties
        [HttpGet]
        public virtual ActionResult Create() => View(createPath, new PIMAttributeFamilyViewModel { IsCategory = true });

        public virtual ActionResult GetExistingFamilies(string isCategory = "1")
          => PartialView("~/Areas/PIM/Views/ProductAttributeFamily/_ExistingFamilies.cshtml", _pimAttributeFamilyAgent.GetPIMAttributeFamilyList(isCategory));

        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            PIMAttributeFamilyViewModel family = _pimAttributeFamilyAgent.Save(model);
            if (!family.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                return RedirectToAction<CategoryAttributeFamilyController>(x => x.Edit(family.PimAttributeFamilyId));
            }
            SetNotificationMessage(GetErrorNotificationMessage(family.ErrorMessage));
            return RedirectToAction<CategoryAttributeFamilyController>(x => x.Create());
        }

        [HttpGet]
        public virtual ActionResult Edit(int pimAttributeFamilyId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView("~/Areas/PIM/Views/ProductAttributeFamily/Edit.cshtml", _pimAttributeFamilyAgent.GetPIMAttributeFamily(pimAttributeFamilyId));
        }
        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            TempData[AdminConstants.Notifications] = _pimAttributeFamilyAgent.SaveFamilyLocales(model)
                ? GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success)
                : GenerateNotificationMessages(Admin_Resources.UpdateErrorMessage, NotificationType.error);
            return RedirectToAction<CategoryAttributeFamilyController>(x => x.Edit(Convert.ToInt32(TempData[AdminConstants.AttributeFamilyId])));
        }

        //Get assigned PIM Attribute Group List along with un assigned PIM Attribute Group list.
        [HttpGet]
        public virtual ActionResult AssignedPIMAttributeGroupList(int pimAttributeFamilyId)
        {
            if (pimAttributeFamilyId > 0)
            {
                //Get the list of already assigned pim attribute group list.
                PIMAttributeGroupListViewModel listViewModel = _pimAttributeFamilyAgent.GetAssignedPIMAttributeGroups(pimAttributeFamilyId);

                TempData[AdminConstants.AttributeFamilyId] = pimAttributeFamilyId;

                return PartialView(_PIMAttributeFamilyGroupListView, new PIMAttributeGroupListViewModel()
                {
                    AttributeGroupList = listViewModel.AttributeGroupList,
                    PimAttributeFamilyId = pimAttributeFamilyId
                });
            }
            return View(_PIMAttributeFamilyGroupListView, new PIMFamilyGroupAttributeViewModel());
        }

        public virtual ActionResult GetUnAssignedAttributeGroups(int id)
            => PartialView("~/Areas/PIM/Views/ProductAttributeFamily/_UnAssignedAttributeGroups.cshtml", _pimAttributeFamilyAgent.GetUnAssignedPIMAttributeGroups(id, true));

        #region Attributes
        //Gets Assigned pim attributes for a group which is associated with Family.
        [HttpGet]
        public virtual ActionResult AssignedPIMAttributes(int attributeGroupId, int attributeFamilyId, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            if (attributeGroupId > 0)
            {
                if (attributeFamilyId > 0)
                {
                    model.Filters.RemoveAll(x => x.Item1 == FilterKeys.PIMAttributeFamilyId);
                    model.Filters.Add(new FilterTuple(FilterKeys.PIMAttributeFamilyId, FilterOperators.Equals, attributeFamilyId.ToString()));
                }

                _pimAttributeGroupAgent.SetFilters(model.Filters, attributeGroupId);
                PIMAttributeListViewModel attributeGroupMappers = _pimAttributeFamilyAgent.GetAssignedAttributes(model.Filters, model.SortCollection, null, 1, int.MaxValue);
                attributeGroupMappers.PimAttributeFamilyId = attributeFamilyId;
                attributeGroupMappers.AttributeGroupId = attributeGroupId;
                return PartialView("~/Areas/PIM/Views/ProductAttributeFamily/_PIMAttributes.cshtml", attributeGroupMappers);
            }
            return null;
        }

        //Get unassigned attributes.
        public virtual ActionResult GetUnAssignedAttributes(int attributeFamilyId, int attributeGroupId)
        {
            if (attributeFamilyId > 0 && attributeGroupId > 0)
            {
                TempData[AdminConstants.AttributeFamilyId] = attributeFamilyId;
                TempData[AdminConstants.PIMAttributeGroupId] = attributeGroupId;
            }
            return PartialView("~/Areas/PIM/Views/ProductAttributeFamily/_UnAssignedAttributes.cshtml", _pimAttributeFamilyAgent.GetUnAssignedPIMAttributes(attributeFamilyId, attributeGroupId));
        }

        //Assign attributes to group.
        [HttpGet]
        public virtual JsonResult AssignAttributes(string selectedIds)
        {
            bool status = false;

            if (selectedIds?.Length > 0)
            {
                string message = string.Empty;
                status = _pimAttributeFamilyAgent.AssignAttributes(selectedIds, (int)TempData[AdminConstants.AttributeFamilyId], (int)TempData[AdminConstants.PIMAttributeGroupId], out message);
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.AssignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        //Unassociate attributes from group.
        public virtual ActionResult UnAssignAttributes(int pimAttributeId, int pimAttributeGroupId)
        {
            if (!Equals(TempData[AdminConstants.AttributeFamilyId], null) && pimAttributeGroupId > 0 && pimAttributeId > 0)
            {
                string message = string.Empty;
                SetNotificationMessage(_pimAttributeFamilyAgent.UnAssignAttributes((int)TempData[AdminConstants.AttributeFamilyId], pimAttributeGroupId, pimAttributeId, out message)
               ? GetSuccessNotificationMessage(Admin_Resources.UnassignSuccessful) : GetErrorNotificationMessage(message));
            }
            return RedirectToAction("Edit", "CategoryAttributeFamily", new { pimAttributeFamilyId = Convert.ToInt32(TempData[AdminConstants.AttributeFamilyId]), area = "PIM" });
        }
        #endregion

        [HttpGet]
        public virtual JsonResult AssignAttributeGroups(string selectedIds)
        {
            bool status = false;

            if (selectedIds?.Length > 0)
            {
                string message = string.Empty;
                status = _pimAttributeFamilyAgent.AssignAttributeGroups(selectedIds, (int)TempData[AdminConstants.AttributeFamilyId], out message);
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.AssignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult UnAssignAttributeGroups(int pimAttributeGroupId)
        {
            if (!Equals(TempData[AdminConstants.AttributeFamilyId], null) && pimAttributeGroupId > 0)
            {
                string message = string.Empty;
                SetNotificationMessage(_pimAttributeFamilyAgent.UnAssignAttributeGroups((int)TempData[AdminConstants.AttributeFamilyId], pimAttributeGroupId, true, out message)
               ? GetSuccessNotificationMessage(Admin_Resources.UnassignSuccessful) : GetErrorNotificationMessage(message));
            }
            return RedirectToAction<CategoryAttributeFamilyController>(x => x.Edit(Convert.ToInt32(TempData[AdminConstants.AttributeFamilyId])));
        }

        // Get PIM attributefamily list.
        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,TitleAttributeFamily", Key = "CategoryAttributeFamily", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCategoryAttributeFamily.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCategoryAttributeFamily.ToString(), model);

            //Sets the IsCategory filter
            HelperMethods.SetIsCategoryFilters(model.Filters, "1");

            //Get the list of PIM attribute family list.
            PIMAttributeFamilyListViewModel pimAttributeFamilyList = _pimAttributeFamilyAgent.GetPIMAttributeFamilies(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            pimAttributeFamilyList?.GridModel?.FilterColumn?.ToolMenuList?.Select(x => { x.ControllerName = "CategoryAttributeFamily"; return x; })?.ToList();

            //Get the grid model.
            pimAttributeFamilyList.GridModel = FilterHelpers.GetDynamicGridModel(model, pimAttributeFamilyList.List, GridListType.ZnodeCategoryAttributeFamily.ToString(), string.Empty, null, true, true, pimAttributeFamilyList?.GridModel?.FilterColumn?.ToolMenuList);
            pimAttributeFamilyList.GridModel.TotalRecordCount = pimAttributeFamilyList.TotalResults;

            //Returns the attribute family list.
            return ActionView("~/Areas/PIM/Views/ProductAttributeFamily/List.cshtml", pimAttributeFamilyList);
        }

        //Get: Family Locale values
        public virtual ActionResult Locale(int id = 0)
            => PartialView("~/Areas/PIM/Views/ProductAttributeFamily/_FamilyLocale.cshtml", _pimAttributeFamilyAgent.GetLocales(id));

        public virtual ActionResult Delete(string pimAttributeFamilyId)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(pimAttributeFamilyId))
            {
                bool status = _pimAttributeFamilyAgent.DeletePIMAttributeFamily(pimAttributeFamilyId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GetTabStructure(int pimAttributeFamilyId = 0)
           => PartialView(AdminConstants.TabStructurePath, _pimAttributeFamilyAgent.CreateTabStructure(pimAttributeFamilyId, true));
        #endregion
    }
}