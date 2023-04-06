using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class FormBuilderAgent : BaseAgent, IFormBuilderAgent
    {
        #region Private Variables
        private readonly IFormBuilderClient _formBuilderClient;
        #endregion

        #region Constructor
        public FormBuilderAgent(IFormBuilderClient formBuilderClient)
        {
            _formBuilderClient = GetClient<IFormBuilderClient>(formBuilderClient);
        }
        #endregion

        #region Public Methods
        //Create form template.
        public virtual FormBuilderViewModel CreateForm(FormBuilderViewModel formBuilderViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                formBuilderViewModel = _formBuilderClient.CreateForm(formBuilderViewModel?.ToModel<FormBuilderModel>())?.ToViewModel<FormBuilderViewModel>();
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (FormBuilderViewModel)GetViewModelWithErrorMessage(formBuilderViewModel, Admin_Resources.ErrorFormCodeAlreadyExists);
                    default:
                        return (FormBuilderViewModel)GetViewModelWithErrorMessage(formBuilderViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (FormBuilderViewModel)GetViewModelWithErrorMessage(formBuilderViewModel, Admin_Resources.ErrorFailedToCreate);
            }
            return formBuilderViewModel;
        }

        //Get list of form builder 
        public virtual FormBuilderListViewModel GetFormBuilderList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, sorts and filters.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { expands = expands, sorts = sorts, filters= filters });
            FormBuilderListModel formBuilderListModel = _formBuilderClient.GetFormBuilderList(expands, filters, sorts, pageIndex, pageSize);
            FormBuilderListViewModel formBuilderListViewModel = new FormBuilderListViewModel { FormBuilderList = formBuilderListModel?.FormBuilderList?.ToViewModel<FormBuilderViewModel>()?.ToList() };

            SetListPagingData(formBuilderListViewModel, formBuilderListModel);

            //Set the Tool Menus for Form Builder List Grid View.
            SetFormBuilderToolMenus(formBuilderListViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return formBuilderListModel?.FormBuilderList?.Count > 0 ? formBuilderListViewModel : new FormBuilderListViewModel() { FormBuilderList = new List<FormBuilderViewModel>() };

        }

        //Get form builder details by id.
        public virtual FormBuilderViewModel GetForm(int formBuilderId)
         => (formBuilderId > 0) ? _formBuilderClient.GetForm(formBuilderId, null)?.ToViewModel<FormBuilderViewModel>() : new FormBuilderViewModel();

        //Delete Form Builder
        public virtual bool DeleteFormBuilder(string formBuilderId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _formBuilderClient.DeleteFormBuilder(new ParameterModel { Ids = formBuilderId });
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get unassign form builder attributes.
        public virtual List<BaseDropDownList> GetUnAssignedAttributes(int formBuilderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            Filters = new FilterCollection();
            if (formBuilderId > 0)
                Filters.Add(new FilterTuple(ZnodeFormBuilderEnum.FormBuilderId.ToString(), FilterOperators.Equals, formBuilderId.ToString()));

            return GlobalAttributeGroupViewModelMap.ToBaseDropDownList(_formBuilderClient.GetUnAssignedAttributes(new ExpandCollection { ZnodeGlobalAttributeEnum.ZnodeGlobalAttributeLocales.ToString() }, Filters, null, null, null));
        }


        // Check form code already exist or not.
        public virtual bool IsFormCodeExist(string formCode)
           => !string.IsNullOrEmpty(formCode) ? _formBuilderClient.IsFormCodeExist(formCode) : true;

        //Create tab structure.
        public virtual TabViewListModel CreateTabStructure(int formBuilderId)
        {
            TabViewListModel TabStructModel = new TabViewListModel();
            TabStructModel.MaintainAllTabData = true;
            TabStructModel.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.TextCanvas, IsVisible = true, Url = $"/FormBuilder/AssignedAttributeGroupList?id={formBuilderId}", IsSelected = true });
            return TabStructModel;
        }

        //Get unassigned attribute groups.
        public virtual List<BaseDropDownList> GetUnAssignedGroups(int formBuilderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            Filters = new FilterCollection();
            if (formBuilderId > 0)
                Filters.Add(new FilterTuple(ZnodeFormBuilderEnum.FormBuilderId.ToString(), FilterOperators.Equals, formBuilderId.ToString()));

            ZnodeLogging.LogMessage("Filters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose,new { Filters= Filters });
            return GlobalAttributeEntityViewModelMap.ToBaseDropDownList(_formBuilderClient.GetUnAssignedGroups(new ExpandCollection(), Filters, null, null, null));
        }

        public virtual FormBuilderAttributeGroupViewModel GetFormBuilderAttributeDetails(int formBuilderId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            FormBuilderAttributeGroupViewModel model = new FormBuilderAttributeGroupViewModel();
            if (formBuilderId > 0)
            {
                int localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                model = GlobalAttributeModelMap.ToFormBuilderAttributeGroupViewModel(_formBuilderClient.GetFormAttributeGroup(formBuilderId, localeId));
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Assign groups to form.
        public virtual bool AssignGroups(string attributeGroupIds, int formBuilderId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            message = string.Empty;
            if (!string.IsNullOrEmpty(attributeGroupIds) && formBuilderId > 0)
            {
                try
                {
                    return _formBuilderClient.AssociateGroups(new GlobalAttributeGroupEntityModel { FormBuilderId = formBuilderId, GroupIds = attributeGroupIds, LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale) });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.NotFound:
                            message = Admin_Resources.ErrorFailToAddGroup;
                            return false;
                        case ErrorCodes.AlreadyExist:
                            message = Admin_Resources.AttributeAlreadyAssociated;
                            return false;
                        default:
                            message = Admin_Resources.TextInvalidData;
                            return false;
                    }
                }
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    message = Admin_Resources.TextInvalidData;
                    return false;
                }

            }

            return false;
        }

        //Assign attributes to form.
        public virtual bool AssignAttributes(string attributeIds, int formBuilderId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            message = string.Empty;
            if (!string.IsNullOrEmpty(attributeIds) && formBuilderId > 0)
            {
                try
                {
                    return _formBuilderClient.AssociateAttributes(new GlobalAttributeGroupEntityModel { FormBuilderId = formBuilderId, AttributeIds = attributeIds, LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale) });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.NotFound:
                             message = Admin_Resources.ErrorFailToAddAttribute;
                            return false;
                        default:
                            message = Admin_Resources.TextInvalidData;
                            return false;
                    }
                }
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    message = Admin_Resources.TextInvalidData;
                    return false;
                }
            }
            return false;
        }

        //Update Attribute DisplayOrder
        public virtual bool UpdateAttributeDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            message = string.Empty;
            if (IsNotNull(model) && model?.FormBuilderId > 0 && model.AttributeId > 0)
            {
                try
                {
                    return _formBuilderClient.UpdateAttributeDisplayOrder(model);
                }
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    message = Admin_Resources.TextInvalidData;
                    return false;
                }
            }
            return false;
        }

        //Update formbuilder.
        public virtual FormBuilderViewModel Update(FormBuilderViewModel formBuilderViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                FormBuilderModel formbuilderModel = _formBuilderClient.UpdateFormBuilder(formBuilderViewModel.ToModel<FormBuilderModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return IsNotNull(formbuilderModel) ? formbuilderModel.ToViewModel<FormBuilderViewModel>() : (FormBuilderViewModel)GetViewModelWithErrorMessage(new FormBuilderViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (FormBuilderViewModel)GetViewModelWithErrorMessage(formBuilderViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }
        // Update formbuilder group display order
        public virtual bool UpdateGroupDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model, string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            message = string.Empty;
            if (IsNotNull(model) && model?.FormBuilderId > 0 && model.GroupId > 0)
            {
                try
                {
                    return _formBuilderClient.UpdateGroupDisplayOrder(model);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            message = Admin_Resources.ErrorFailToDeleteGlobalAttributeGroup;
                            return false;
                        default:
                            message = Admin_Resources.TextInvalidData;
                            return false;
                    }
                }
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    message = Admin_Resources.UpdateErrorMessage;
                    return false;
                }
            }
            return false;
        }

        // unassign form builder groups
        public virtual bool UnAssignFormBuilderGroups(int formBuilderId, int groupId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            message = string.Empty;
            bool isUnAssign = false;
            try
            {
                if (formBuilderId > 0 && groupId > 0)
                    isUnAssign = _formBuilderClient.UnAssociateFormBuilderGroups(formBuilderId, groupId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        message = Admin_Resources.ErrorFailToDeleteGlobalAttributeGroup;
                        return isUnAssign;
                    default:
                        message = Admin_Resources.TextInvalidData;
                        return isUnAssign;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                message = Admin_Resources.TextInvalidData;
                return isUnAssign;
            }
            return isUnAssign;
        }

        // unassign form builder attribute
        public virtual bool UnAssignFormBuilderAttributes(int formBuilderId, int attributeId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            message = string.Empty;
            bool isUnAssign = false;
            try
            {
                if (formBuilderId > 0 && attributeId > 0)
                    isUnAssign = _formBuilderClient.UnAssociateFormBuilderAttributes(formBuilderId, attributeId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        message = Admin_Resources.ErrorFailToDeleteGlobalAttribute;
                        return isUnAssign;
                    default:
                        message = Admin_Resources.TextInvalidData;
                        return isUnAssign;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                message = Admin_Resources.TextInvalidData;
                return isUnAssign;
            }
            return isUnAssign;
        }
        #endregion

        #region Private Methods.
        //Set the Tool Menus for Form Builder List Grid View.
        private void SetFormBuilderToolMenus(FormBuilderListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('FormBuilderDeletePopup')", ControllerName = "FormBuilder", ActionName = "Delete" });
            }
        }



        #endregion
    }
}
