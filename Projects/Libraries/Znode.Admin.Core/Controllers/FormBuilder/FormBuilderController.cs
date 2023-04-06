using System;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class FormBuilderController : BaseController
    {
        #region Private Variables
        private readonly IFormBuilderAgent _formBuilderAgent;
        private const string AssignedAttributeGroupListView = "_AssignedAttributeGroupList";
        private const string UnAssignedAttributeGroups = "_UnAssignedAttributeGroups";
        #endregion

        #region Public Constructor
        public FormBuilderController(IFormBuilderAgent formBuilderAgent)
        {
            _formBuilderAgent = formBuilderAgent;
        }
        #endregion

        #region Public Methods

        //Create form.
        public virtual ActionResult Create()
            => ActionView(AdminConstants.CreateEdit, new FormBuilderViewModel());

        //Save the data of the form.
        [HttpPost]
        public virtual ActionResult Create(FormBuilderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //Create from.
                viewModel = _formBuilderAgent.CreateForm(viewModel);

                //Based on the hasError property sets success or failure message.
                if (!viewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SaveMessage));
                    return RedirectToAction<FormBuilderController>(x => x.Edit(viewModel.FormBuilderId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(viewModel.ErrorMessage));
            return ActionView(AdminConstants.CreateEdit, viewModel);
        }

        //Method to edit form.
        public virtual ActionResult Edit(int formBuilderId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(AdminConstants.CreateEdit, _formBuilderAgent.GetForm(formBuilderId));
        }

        // Get the list of form builder.
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeFormBuilderList.ToString(), model);
            FormBuilderListViewModel formBuilderListViewModel = _formBuilderAgent.GetFormBuilderList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            formBuilderListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, formBuilderListViewModel.FormBuilderList, GridListType.ZnodeFormBuilderList.ToString(), string.Empty, null, true, true, formBuilderListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            formBuilderListViewModel.GridModel.TotalRecordCount = formBuilderListViewModel.TotalResults;

            return ActionView(formBuilderListViewModel);
        }

        //Delete form builder.
        public virtual JsonResult Delete(string formBuilderId)
        {
            bool status = false;
            if (!string.IsNullOrEmpty(formBuilderId))
                status = _formBuilderAgent.DeleteFormBuilder(formBuilderId);

            return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.ErrorDeleteForm }, JsonRequestBehavior.AllowGet);
        }

        //Check form code already exist or not.
        [HttpGet]
        public virtual ActionResult IsFormCodeExist(string formCode)
            => Json(new { data = _formBuilderAgent.IsFormCodeExist(formCode) }, JsonRequestBehavior.AllowGet);

        //Create tab structure for form builder.
        public virtual ActionResult GetTabStructure(int formBuilderId = 0)
        => PartialView(AdminConstants.TabStructurePath, _formBuilderAgent.CreateTabStructure(formBuilderId));

        [HttpGet]
        public virtual ActionResult AssignedAttributeGroupList(int id)
        => PartialView(AssignedAttributeGroupListView, _formBuilderAgent.GetFormBuilderAttributeDetails(id));

        public virtual ActionResult GetUnAssignedAttributeGroups(int id)
           => PartialView(UnAssignedAttributeGroups, _formBuilderAgent.GetUnAssignedGroups(id));

        //Get list of unassigned attributes.
        public virtual ActionResult GetUnAssignedAttribute(int id)
           => PartialView("_UnassignedAttributes", _formBuilderAgent.GetUnAssignedAttributes(id));

        //Assign groups to form.
        public virtual JsonResult AssignGroups(string selectedIds)
        {
            bool status = false;
            int formBuilderId = Convert.ToInt32(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[ZnodeFormBuilderEnum.FormBuilderId.ToString()]);

            if (!string.IsNullOrEmpty(selectedIds))
            {
                string message = string.Empty;
                status = _formBuilderAgent.AssignGroups(selectedIds, formBuilderId, out message);
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.AssignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        //Assign attributes to form.
        public virtual JsonResult AssignAttributes(string selectedIds)
        {
            bool status = false;
            int formBuilderId = Convert.ToInt32(HttpUtility.ParseQueryString(Request.UrlReferrer.Query)[ZnodeFormBuilderEnum.FormBuilderId.ToString()]);

            if (!string.IsNullOrEmpty(selectedIds))
            {
                string message = string.Empty;
                status = _formBuilderAgent.AssignAttributes(selectedIds, formBuilderId, out message);
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.AssignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        //Update attribute attribute display order.
        public virtual JsonResult UpdateAttributeDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model)
        {
            bool status = false;

            if (IsNotNull(model) && model?.FormBuilderId > 0 && model.AttributeId > 0)
            {
                string message = string.Empty;
                status = _formBuilderAgent.UpdateAttributeDisplayOrder(model, out message);
                return Json(new { HasNoError = status, Message = message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        //Update attribute group display order.
        public virtual JsonResult UpdateGroupDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model)
        {
            bool status = false;

            if (IsNotNull(model) && model?.FormBuilderId > 0 && model.GroupId > 0)
            {
                string message = string.Empty;
                status = _formBuilderAgent.UpdateGroupDisplayOrder(model, message);
                return Json(new { HasNoError = status, Message = message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        //Un assign attributes from form.
        public virtual JsonResult UnAssignAttribute(int formBuilderId, int attributeId)
        {
            bool status = false;
            string message = string.Empty;
            if (formBuilderId > 0 && attributeId > 0)
            {
                status = (_formBuilderAgent.UnAssignFormBuilderAttributes(formBuilderId, attributeId, out message));
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        //Un assign group from form.
        public virtual JsonResult UnAssignGroup(int formBuilderId, int groupId)
        {
            bool status = false;

            if (formBuilderId > 0 && groupId > 0)
            {
                string message = string.Empty;
                status = (_formBuilderAgent.UnAssignFormBuilderGroups(formBuilderId, groupId, out message));
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }
        //Post: Attribute Update
        [HttpPost]
        public virtual ActionResult Edit(FormBuilderViewModel formBuilderViewModel)
        {
            FormBuilderViewModel builder = _formBuilderAgent.Update(formBuilderViewModel);
            if (!builder.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<FormBuilderController>(x => x.Edit(builder.FormBuilderId));
            }

            SetNotificationMessage(GetErrorNotificationMessage(builder.ErrorMessage));
            return RedirectToAction<FormBuilderController>(x => x.Edit(builder.FormBuilderId));
        }
        #endregion
    }
}
