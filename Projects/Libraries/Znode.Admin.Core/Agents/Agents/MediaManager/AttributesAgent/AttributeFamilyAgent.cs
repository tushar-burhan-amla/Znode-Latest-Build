using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Admin.Agents
{
    public class AttributeFamilyAgent : BaseAgent, IAttributeFamilyAgent
    {
        #region Private Variables
        private readonly IAttributeFamilyClient _attributeFamilyClient;
        private const string AttributeListUrl = "/AttributeFamily/AssignedAttributeGroupList?mediaAttributeFamilyId={0}";
        private readonly ILocaleClient _localeClient;
        #endregion

        #region Constructor
        public AttributeFamilyAgent(IAttributeFamilyClient attributeFamilyClient, ILocaleClient localeClient)
        {
            _attributeFamilyClient = GetClient<IAttributeFamilyClient>(attributeFamilyClient);
            _localeClient = GetClient<ILocaleClient>(localeClient);
        }
        #endregion

        #region public virtual Methods

        public virtual AttributeFamilyListViewModel GetAttributeFamilies(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sortCollection.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { filters, sortCollection });

            //set locale filters if not present
            SetLocaleFilterIfNotPresent(ref filters);
            AttributeFamilyListViewModel attributeFamilyList = AttributeFamilyViewModelMap.ToListViewModel(_attributeFamilyClient.GetAttributeFamilyList(filters, sortCollection, pageIndex, recordPerPage));

            //Set tool menu for attribute family list grid view.
            SetAttributeFamilyListToolMenu(attributeFamilyList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return (attributeFamilyList?.AttributeFamilies?.Count() > 0) ? attributeFamilyList : new AttributeFamilyListViewModel() { AttributeFamilies = new List<AttributeFamilyViewModel>() };
        }

        public virtual List<SelectListItem> GetAttributeFamilyList()
            => AttributeFamilyViewModelMap.ToListItems(_attributeFamilyClient.GetAttributeFamilyList(null, null, null, null)?.AttributeFamilies);

        public virtual AttributeFamilyViewModel CreateAttributeFamily(AttributeFamilyViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                AttributeFamilyModel familyModel = _attributeFamilyClient.CreateAttributeFamily(viewModel.ToModel<AttributeFamilyModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return !Equals(familyModel, null) ? familyModel.ToViewModel<AttributeFamilyViewModel>() : new AttributeFamilyViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return new AttributeFamilyViewModel { HasError = true, ErrorMessage = Admin_Resources.FamilyAlreadyExists };
                    default:
                        return new AttributeFamilyViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                return new AttributeFamilyViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
            }
        }

        public virtual AttributeGroupListViewModel GetAssignedAttributeGroups(int attributeFamilyId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        => AttributeGroupViewModelMap.ToListViewModel(_attributeFamilyClient.GetAssignedAttributeGroups(GetMediaAttributeGroupExpand(), GetMediaAttributeFamilyFilter(attributeFamilyId), sortCollection, pageIndex, recordPerPage));

        public virtual bool AssignAttributeGroups(string attributeGroupIds, int attributeFamilyId, out string message)
        {
            message = string.Empty;
            if (!string.IsNullOrEmpty(attributeGroupIds) && attributeFamilyId > 0)
            {
                try
                {
                    ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

                    //Get attributes by attribute group ids.
                    AttributeGroupMapperListModel attributes = _attributeFamilyClient.GetAttributesByGroupIds(new ParameterModel { Ids = attributeGroupIds });

                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

                    //Assign attribute groups to attribute family.
                    return !Equals(_attributeFamilyClient.AssociateAttributeGroups(AttributeFamilyViewModelMap.ToFamilyGroupAttributeListModel(attributeGroupIds.Split(',').ToList(), attributeFamilyId, attributes)), null);
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
                    message = Admin_Resources.TextInvalidData;
                    return false;
                }
            }
            return false;
        }

        public virtual bool UnAssignAttributeGroups(int attributeFamilyId, int attributeGroupId, out string message)
        {
            message = string.Empty;
            bool isUnAssign = false;
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

                if (attributeFamilyId > 0 && attributeGroupId > 0)
                    //UnAssign attribute group which associate with attribute family.
                    isUnAssign = _attributeFamilyClient.UnAssociateAttributeGroups(attributeFamilyId, attributeGroupId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        message = Admin_Resources.TextInvalidData;
                        return isUnAssign;
                    case ErrorCodes.AssociationDeleteError:
                        message = Attributes_Resources.ErrorFailToDeleteSystemDefineAttributeGroup;
                        return isUnAssign;
                    default:
                        message = Admin_Resources.TextInvalidData;
                        return isUnAssign;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                message = Admin_Resources.UnassignError;
                return isUnAssign;
            }
            return isUnAssign;
        }

        public virtual bool DeleteAttributeFamily(string attributeFamilyId, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(attributeFamilyId))
            {
                try
                {
                    ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                    return _attributeFamilyClient.DeleteAttributeFamily(new ParameterModel { Ids = attributeFamilyId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = Attributes_Resources.ErrorDeleteMediaFamily;
                            break;
                        default:
                            errorMessage = Admin_Resources.ErrorFailedToDelete;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        public virtual List<BaseDropDownList> GetUnAssignedAttributeGroups(int attributeFamilyId)
            => AttributeFamilyViewModelMap.ToBaseDropDownList(_attributeFamilyClient.GetUnAssignedAttributeGroups(null, GetMediaAttributeFamilyFilter(attributeFamilyId), null, null, null));


        public virtual AttributeFamilyViewModel GetAttributeFamily(int attributeFamilyId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            AttributeFamilyModel familyModel = _attributeFamilyClient.GetAttributeFamily(attributeFamilyId);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return !Equals(familyModel, null) && (familyModel.MediaAttributeFamilyId > 0) ? familyModel.ToViewModel<AttributeFamilyViewModel>() : new AttributeFamilyViewModel();
        }

        public virtual TabViewListModel CreateTabStructure(int mediaAttributeFamilyId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters mediaAttributeFamilyId.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { mediaAttributeFamilyId });

            TabViewListModel TabStructModel = new TabViewListModel();
            TabStructModel.MaintainAllTabData = true;
            TabStructModel.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.LabelAttributeList, IsVisible = true, Url = string.Format(AttributeListUrl, mediaAttributeFamilyId) });

            TabStructModel.Tabs.Add(new TabViewModel() { Id = 2, Name = Admin_Resources.LabelLocale, IsVisible = true, Url = $"/AttributeFamily/Locale?id={mediaAttributeFamilyId}" });
            return TabStructModel;
        }

        //Get Locale By attribute family Id.
        public virtual List<LocaleDataModel> GetLocales(int attributeFamilyId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "true"));
            return AttributeFamilyViewModelMap.ToLocaleDataModel(_localeClient.GetLocaleList(null, filters, null, null, null), GetFamilyLocale(attributeFamilyId));
        }

        //Update The Existing family locale Values.
        public virtual bool SaveFamilyLocales(BindDataModel model)
          => !Equals(_attributeFamilyClient.SaveLocales(AttributeFamilyViewModelMap.ToFamilyLocaleListModel(model)), null);

        public virtual List<SelectListItem> GetViewModes(FilterCollectionDataModel model)
        {
            List<SelectListItem> viewModes = new List<SelectListItem>();
            viewModes.Add(new SelectListItem() { Text = (ViewModeTypes.Detail).ToString(), Value = (ViewModeTypes.Detail).ToString(), Selected = model.ViewMode.Equals((ViewModeTypes.Detail).ToString()) ? true : false });
            viewModes.Add(new SelectListItem() { Text = (ViewModeTypes.List).ToString(), Value = (ViewModeTypes.List).ToString(), Selected = model.ViewMode.Equals((ViewModeTypes.List).ToString()) ? true : false });
            return viewModes;
        }
        #endregion

        #region Private Methods
        //Get family Locale By Family Id.
        private FamilyLocaleListModel GetFamilyLocale(int attributeFamilyId)
            => attributeFamilyId == 0 ? null : _attributeFamilyClient.GetFamilyLocale(attributeFamilyId);

        private FilterCollection GetMediaAttributeFamilyFilter(int attributeFamilyId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.MediaAttributeFamilyId, FilterOperators.Equals, attributeFamilyId.ToString()));
            return filters;
        }

        private ExpandCollection GetMediaAttributeGroupExpand()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.AttributeGroup);
            return expands;
        }

        //Set tool menu for attribute family list grid view.
        private void SetAttributeFamilyListToolMenu(AttributeFamilyListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('MediaAttributeFamilyDeletePopup')", ControllerName = "AttributeFamily", ActionName = "Delete" });
            }
        }
        #endregion
    }
}