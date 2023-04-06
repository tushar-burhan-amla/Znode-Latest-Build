using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Libraries.Resources;
using Znode.Engine.Admin.Extensions;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Admin.Agents
{
    public class AttributeGroupAgent : BaseAgent, IAttributeGroupAgent
    {
        #region Private Variables
        private readonly IAttributeGroupClient _attributeGroupClient;
        private readonly ILocaleClient _localeClient;
        #endregion

        #region Constructor
        public AttributeGroupAgent(IAttributeGroupClient attributeGroupClient, ILocaleClient localeClient)
        {
            _attributeGroupClient = GetClient<IAttributeGroupClient>(attributeGroupClient);
            _localeClient = GetClient<ILocaleClient>(localeClient);
        }
        #endregion

        public  List<LocaleDataModel> GetLocales(int pimAttributeGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "true"));
            return AttributeGroupViewModelMap.ToLocaleDataModel(_localeClient.GetLocaleList(null, filters, null, null, null), _attributeGroupClient.GetAttributeGroupLocales(pimAttributeGroupId));
        }

        public virtual AttributeGroupListViewModel GetAttributeGroups(ExpandCollection expands, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, filters and sortCollection.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sortCollection = sortCollection });

            SetLocaleFilterIfNotPresent(ref filters);
            AttributeGroupListViewModel attributeGroupListViewModel = AttributeGroupViewModelMap.ToListViewModel(_attributeGroupClient.GetAttributeGroupList(expands, filters, sortCollection, pageIndex, recordPerPage));

            //Set tool menu for attribute group list grid view.
            SetAttributeGroupListToolMenu(attributeGroupListViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return attributeGroupListViewModel;
        }
        public virtual List<SelectListItem> GetAttributeGroupList() => AttributeGroupViewModelMap.ToListItems(_attributeGroupClient.GetAttributeGroupList(null, null, null).AttributeGroups);

        public virtual AttributeGroupViewModel Create(BindDataModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

                //Create new attribute group.
                AttributeGroupModel model = _attributeGroupClient.CreateAttributeGroup(AttributeGroupViewModelMap.ToModel(viewModel));

                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return !Equals(model, null) ? model.ToViewModel<AttributeGroupViewModel>() : new AttributeGroupViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return new AttributeGroupViewModel { HasError = true, ErrorMessage = Admin_Resources.GroupAlreadyExists };
                    default:
                        return new AttributeGroupViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                return new AttributeGroupViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
            }
        }

        public virtual AttributeGroupViewModel GetAttributeGroup(int attributeGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            if (attributeGroupId > 0)
            {
                var attributeGroupModel = _attributeGroupClient.GetAttributeGroup(attributeGroupId);
                return !Equals(attributeGroupModel, null) ? attributeGroupModel.ToViewModel<AttributeGroupViewModel>() : new AttributeGroupViewModel();
            }

            ZnodeLogging.LogMessage("Agent method execution done", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            return new AttributeGroupViewModel();
        }

        public virtual bool UpdateAttributeGroup(BindDataModel viewModel)
             => !Equals(_attributeGroupClient.UpdateAttributeGroup(AttributeGroupViewModelMap.ToModel(viewModel)), null);

        public virtual bool DeleteAttributeGroup(string attributeGroupId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            message = string.Empty;
            try
            {
                return _attributeGroupClient.DeleteAttributeGroup(new ParameterModel() { Ids = attributeGroupId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                message = Admin_Resources.GroupInUse;
                return false;
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                message = Admin_Resources.DeleteErrorMessage;
                return false;
            }
        }

        public virtual AttributeGroupMapperListViewModel GetAssignedAttributes(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null)
        {
           ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

             if (Equals(expands, null))
              expands = new ExpandCollection();
              expands.Add(ExpandKeys.Attributes);

          ZnodeLogging.LogMessage("Agent method execution done", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

          return AttributeGroupMapperViewModelMap.ToListViewModel(_attributeGroupClient.GetAssociatedAttributes(expands, filters, sortCollection, pageIndex, recordPerPage));
        }

        public virtual List<BaseDropDownList> GetUnAssignedAttributes(int attributeGroupId)
            => attributeGroupId > 0 ? AttributeGroupViewModelMap.ToBaseDropDownList(_attributeGroupClient.GetUnAssignedAttributes(attributeGroupId, null, null, null, null)) : new List<BaseDropDownList>();

        public virtual bool RemoveAssociatedAttribute(int attributeGroupMapperId) => _attributeGroupClient.DeleteAssociatedAttribute(attributeGroupMapperId);

        public virtual List<SelectListItem> GetViewModes(FilterCollectionDataModel model)
        {
            List<SelectListItem> viewModes = new List<SelectListItem>();
            viewModes.Add(new SelectListItem() { Text = (ViewModeTypes.Detail).ToString(), Value = (ViewModeTypes.Detail).ToString(), Selected = model.ViewMode.Equals((ViewModeTypes.Detail).ToString()) ? true : false });
            viewModes.Add(new SelectListItem() { Text = (ViewModeTypes.Tile).ToString(), Value = (ViewModeTypes.Tile).ToString(), Selected = model.ViewMode.Equals((ViewModeTypes.Tile).ToString()) ? true : false });
            viewModes.Add(new SelectListItem() { Text = (ViewModeTypes.List).ToString(), Value = (ViewModeTypes.List).ToString(), Selected = model.ViewMode.Equals((ViewModeTypes.List).ToString()) ? true : false });
            return viewModes;
        }

        //Method to create tab structure for Attribute family
        public virtual TabViewListModel CreateGroupTabStructure(int mediaAttributeGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters mediaAttributeGroupId.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { mediaAttributeGroupId = mediaAttributeGroupId });

            TabViewListModel TabStructModel = new TabViewListModel();
            TabStructModel.MaintainAllTabData = false;
            if (mediaAttributeGroupId > 0)
            {
                TabStructModel.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.LabelAttributeList, IsVisible = true, Url = $"/MediaManager/AttributeGroup/AssignedAttributesList?mediaAttributeGroupId={mediaAttributeGroupId}", IsSelected = true });
                TabStructModel.Tabs.Add(new TabViewModel() { Id = 2, Name = Admin_Resources.LabelLocale, IsVisible = true, Url = $"/MediaManager/AttributeGroup/EditAttributeGroupLocale?id={mediaAttributeGroupId}" });
            }
            else
                TabStructModel.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.LabelLocale, IsVisible = true, Url = $"/MediaManager/AttributeGroup/EditAttributeGroupLocale?id={mediaAttributeGroupId}", IsSelected = true });


          ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

          return TabStructModel;
        }

        public virtual bool AssociateAttributes(string attributeIds, int attributeGroupId, out string message)
        {
          ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
          ZnodeLogging.LogMessage("Input parameters attributeIds and attributeGroupId", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { attributeIds = attributeIds, attributeGroupId = attributeGroupId });

           message = string.Empty;
            if (!string.IsNullOrEmpty(attributeIds) && attributeGroupId > 0)
            {
                try
                {
                    //Assign attribute groups to attribute family.
                    return !Equals(_attributeGroupClient.AssignAttributes(AttributeGroupMapperViewModelMap.ToAttributeGroupMapperListModel(attributeIds.Split(',').ToList(), attributeGroupId)), null);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.InvalidData:
                            message = Admin_Resources.TextInvalidData;
                            return false;
                        default:
                            message = Admin_Resources.TextInvalidData;
                            return false;
                    }
                }
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                    message = Admin_Resources.AssociatedErrorMessage;
                    return false;
                }
            }
            return false;
        }

        public virtual void SetFilters(FilterCollection filters, int attributeGroupId)
        {
          ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
          ZnodeLogging.LogMessage("Input parameters filters", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { filters = filters });

      if (!Equals(filters, null))
            {
                //Checking For MediaAttributeGroupId already Exists in Filters Or Not 
                if (filters.Exists(x => x.FilterName == FilterKeys.MediaAttributeGroupId))
                {
                    //If MediaAttributeGroupId Already present in filters Remove It
                    filters.RemoveAll(x => x.FilterName == FilterKeys.MediaAttributeGroupId);

                    //Add New MediaAttributeGroupId Into filters
                    filters.Add(new FilterTuple(FilterKeys.MediaAttributeGroupId, FilterOperators.Equals, attributeGroupId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.MediaAttributeGroupId, FilterOperators.Equals, attributeGroupId.ToString()));
            }

         ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

    }

    //Check whether the group code already exists.
    public virtual bool CheckGroupCodeExist(string groupCode, int mediaAttributeGroupId)
        {
         ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
         ZnodeLogging.LogMessage("Input parameters groupCode and mediaAttributeGroupId.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { groupCode = groupCode, mediaAttributeGroupId = mediaAttributeGroupId });

      if (mediaAttributeGroupId > 0)
                return false;

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeGroupEnum.GroupCode.ToString(), FilterOperators.Is, groupCode));

            //Get the Pim Attribute GroupCode List based on the groupCode code filter.
            AttributeGroupListModel groupList = _attributeGroupClient.GetAttributeGroupList(null, filters, null, null, null);

         ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

      return groupList?.AttributeGroups?.Any(x => string.Equals(x.GroupCode, groupCode, StringComparison.OrdinalIgnoreCase)) ?? false;

        }

        #region Private Method
        //Set tool menu for attribute group list grid view.
        private void SetAttributeGroupListToolMenu(AttributeGroupListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('MediaAttributeGroupDeletePopup')", ControllerName = "AttributeGroup", ActionName = "Delete" });
            }
        }
        #endregion
    }
}