using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
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
    public class GlobalAttributeGroupAgent : BaseAgent, IGlobalAttributeGroupAgent
    {
        #region Private Variables
        private readonly IGlobalAttributeGroupClient _globalAttributeGroupClient;
        private readonly IGlobalAttributeEntityClient _globalAttributesEntityClient;
        #endregion

        #region Constructor
        public GlobalAttributeGroupAgent(IGlobalAttributeGroupClient globalAttributeGroupClient, IGlobalAttributeEntityClient globalAttributeEntityClient)
        {
            _globalAttributeGroupClient = GetClient<IGlobalAttributeGroupClient>(globalAttributeGroupClient);
            _globalAttributesEntityClient = GetClient<IGlobalAttributeEntityClient>(globalAttributeEntityClient);
        }
        #endregion

        #region Public Methods
        //Get global attribute list.
        public virtual GlobalAttributeGroupListViewModel GetGlobalAttributeGroups(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int entityId = 0, string entityType = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //set locale filters if not present.
            SetLocaleFilterIfNotPresent(ref filters);

            BindEntityIdWithFilters(filters, entityId);           

            GlobalAttributeGroupListModel listModel = _globalAttributeGroupClient.GetAttributeGroupList(null, filters, sortCollection, pageIndex, recordPerPage);
            GlobalAttributeGroupListViewModel attributeGroups = new GlobalAttributeGroupListViewModel { AttributeGroupList = listModel?.AttributeGroupList?.ToViewModel<GlobalAttributeGroupViewModel>().ToList() };

            SetListPagingData(attributeGroups, listModel);

            //Set tool menu for global attribute group list grid view.
            SetGlobalAttributeGroupsToolMenu(attributeGroups);

            BindEntityFilterValues(attributeGroups, entityId, entityType);

            if (attributeGroups?.AttributeGroupList?.Count > 0)
                return attributeGroups;
            else
                return new GlobalAttributeGroupListViewModel() { AttributeGroupList = new List<GlobalAttributeGroupViewModel>() };
        }

        //Create global attribute group.
        public virtual GlobalAttributeGroupViewModel Create(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                GlobalAttributeGroupModel groupModel = GlobalAttributeGroupViewModelMap.ToModel(model);
                groupModel.EntityName = BindEntityName(groupModel.GlobalEntityId);
                groupModel = _globalAttributeGroupClient.CreateAttributeGroupModel(groupModel);
               
                return IsNotNull(groupModel) ? groupModel.ToViewModel<GlobalAttributeGroupViewModel>() : new GlobalAttributeGroupViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (GlobalAttributeGroupViewModel)GetViewModelWithErrorMessage(new GlobalAttributeGroupViewModel(), Admin_Resources.GroupAlreadyExists);
                    default:
                        return (GlobalAttributeGroupViewModel)GetViewModelWithErrorMessage(new GlobalAttributeGroupViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (GlobalAttributeGroupViewModel)GetViewModelWithErrorMessage(new GlobalAttributeGroupViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get global attribute group details.
        public virtual GlobalAttributeGroupViewModel Get(int globalAttributeGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            GlobalAttributeGroupModel globalAttributeGroupModel = _globalAttributeGroupClient.GetAttributeGroup(globalAttributeGroupId, null);
            GlobalAttributeGroupViewModel viewModel = IsNotNull(globalAttributeGroupModel) ? globalAttributeGroupModel.ToViewModel<GlobalAttributeGroupViewModel>() : new GlobalAttributeGroupViewModel();
            viewModel.GlobalEntityType = GetGlobalEntityTypes();
            return viewModel;
        }

        //Get Available Entity Types
        public virtual List<SelectListItem> GetGlobalEntityTypes()
        {
            GlobalEntityListModel globalEntityList = _globalAttributesEntityClient.GetGlobalEntity();

            List<SelectListItem> entityList = new List<SelectListItem>();
            globalEntityList?.GlobalEntityList?.ToList().ForEach(item => { entityList.Add(new SelectListItem() { Text = item.EntityName, Value = Convert.ToString(item.GlobalEntityId) }); });
            return entityList;

        }

        //Update global attribute group.
        public virtual GlobalAttributeGroupViewModel UpdateAttributeGroup(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                GlobalAttributeGroupModel groupModel = GlobalAttributeGroupViewModelMap.ToModel(model);
                groupModel.EntityName = BindEntityName(groupModel.GlobalEntityId);
                groupModel = _globalAttributeGroupClient.UpdateAttributeGroupModel(groupModel);

                return IsNotNull(groupModel) ? groupModel.ToViewModel<GlobalAttributeGroupViewModel>() : (GlobalAttributeGroupViewModel)GetViewModelWithErrorMessage(new GlobalAttributeGroupViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (GlobalAttributeGroupViewModel)GetViewModelWithErrorMessage(new GlobalAttributeGroupViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Create tab structure.
        public virtual TabViewListModel GetTabStructure(int globalAttributeGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            TabViewListModel tabList = new TabViewListModel();
            tabList.MaintainAllTabData = false;

            if (globalAttributeGroupId > 0)
            {
                tabList.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.TitleGlobalAttributesList, IsVisible = true, Url = $"/GlobalAttributeGroup/GetAssignedAttribute?id={globalAttributeGroupId}", IsSelected = true });
                tabList.Tabs.Add(new TabViewModel() { Id = 2, Name = Attributes_Resources.TitleLocale, IsVisible = true, Url = $"/GlobalAttributeGroup/CreateAttributeGroupLocale?id={globalAttributeGroupId}" });
            }
            else
                tabList.Tabs.Add(new TabViewModel() { Id = 1, Name = Attributes_Resources.TitleLocale, IsVisible = true, Url = $"/GlobalAttributeGroup/CreateAttributeGroupLocale?id={globalAttributeGroupId}", IsSelected = true });
            return tabList;
        }

        //Get global group locales by groupId.
        public virtual List<LocaleDataModel> GetLocales(int globalAttributeGroupId)
            => GlobalAttributeGroupViewModelMap.ToLocaleDataModel(DefaultSettingHelper.GetActiveLocaleList(), globalAttributeGroupId > 0 ? _globalAttributeGroupClient.GetGlobalAttributeGroupLocales(globalAttributeGroupId) : new GlobalAttributeGroupLocaleListModel());

        //Check whether the group code already exists.
        public virtual bool CheckGroupCodeExist(string groupCode, int globalAttributeGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (globalAttributeGroupId > 0)
                return false;

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeGlobalAttributeGroupEnum.GroupCode.ToString(), FilterOperators.Is, groupCode));
            ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { filters = filters });

            //Get the global Attribute GroupCode List based on the groupCode code filter.
            GlobalAttributeGroupListModel groupList = _globalAttributeGroupClient.GetAttributeGroupList(null, filters, null, null, null);
            if (IsNotNull(groupList) && IsNotNull(groupList.AttributeGroupList))
                return groupList.AttributeGroupList.Any(x => string.Equals(x.GroupCode, groupCode, StringComparison.OrdinalIgnoreCase));

            return false;
        }

        //Set tool menu for global attribute group list grid view.
        public void SetGlobalAttributeGroupsToolMenu(GlobalAttributeGroupListViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('GlobalAttributeGroupDeletePopup')", ActionName = "Delete" });
            }
        }

        //Delete Global attribute group.
        public virtual bool Delete(string globalAttributeGroupId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(globalAttributeGroupId))
            {
                try
                {
                    return _globalAttributeGroupClient.DeleteAttributeGroup(new ParameterModel { Ids = globalAttributeGroupId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = Admin_Resources.ErrorFailToDeleteGlobalAttributeGroup;
                            return false;
                        default:
                            errorMessage = Admin_Resources.ErrorFailedToDelete;
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            errorMessage = Admin_Resources.ErrorFailedToDelete;
            return false;
        }

        //Get assign attributes.
        public virtual GlobalAttributeGroupMapperListViewModel GetAssignedAttributes(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(expands))
                expands = new ExpandCollection();
            expands.Add(ExpandKeys.Attributes);

            sortCollection = new SortCollection();
            sortCollection.Add("ZnodeGlobalAttribute.DisplayOrder", "asc");
            sortCollection.Add("ZnodeGlobalAttribute.GlobalAttributeId", "asc");
            ZnodeLogging.LogMessage("expands and sorts:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { expands = expands, sorts = sortCollection });
            return GlobalAttributeGroupViewModelMap.ToListViewModel(_globalAttributeGroupClient.GetAssignedAttributes(expands, filters, sortCollection, pageIndex, recordPerPage));
        }

        //Get unassign global attributes.
        public virtual List<BaseDropDownList> GetUnAssignedAttributes(int globalAttributeGroupId)
        {

            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeGlobalAttributeGroupEnum.GlobalAttributeGroupId.ToString(), FilterOperators.Equals, globalAttributeGroupId.ToString());
            return  GlobalAttributeGroupViewModelMap.ToBaseDropDownList(_globalAttributeGroupClient.GetUnAssignedAttributes(new ExpandCollection { ZnodeGlobalAttributeEnum.ZnodeGlobalAttributeLocales.ToString() }, filters, null, null, null));

        }

        //Associate attributes to group.
        public virtual bool AssociateAttributes(string attributeIds, int attributeGroupId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            message = string.Empty;
            if (!string.IsNullOrEmpty(attributeIds) && attributeGroupId > 0)
            {
                try
                {
                    //Assign attributes to group.
                    return IsNotNull(_globalAttributeGroupClient.AssociateAttributes(GlobalAttributeGroupViewModelMap.ToAttributeGroupMapperListModel(attributeIds.Split(',').ToList(), attributeGroupId)));
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
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
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    message = Admin_Resources.AssociatedErrorMessage;
                    return false;
                }
            }
            return false;
        }

        //Unassign global attribute from group. 
        public virtual bool RemoveAssociatedAttribute(int globalAttributeGroupId, int globalAttributeId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            message = Admin_Resources.ErrorFailedToDelete;
            try
            {
                return _globalAttributeGroupClient.RemoveAssociatedAttributes(new RemoveGroupAttributesModel() { GlobalAttributeGroupId = globalAttributeGroupId, GlobalAttributeIds = globalAttributeId.ToString() });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        message = Admin_Resources.ErrorFailToDeleteGlobalAttribute;
                        return false;
                    default:
                        message = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                message = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Update attribute display order.
        public virtual GlobalAttributeViewModel UpdateAttributeDisplayOrder(int globalAttributeId, string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNotNull(data))
            {
                try
                {
                    GlobalAttributeViewModel attributeDataViewModel = new GlobalAttributeViewModel();
                    GlobalAttributeDataModel attributeDataModel = new GlobalAttributeDataModel();
                    attributeDataViewModel = JsonConvert.DeserializeObject<GlobalAttributeViewModel[]>(data)[0];

                    attributeDataModel.AttributeModel = new GlobalAttributeModel() { DisplayOrder = attributeDataViewModel.DisplayOrder, GlobalAttributeId = attributeDataViewModel.GlobalAttributeId };

                    attributeDataViewModel = (_globalAttributeGroupClient.UpdateAttributeDisplayOrder(attributeDataModel))?.AttributeModel?.ToViewModel<GlobalAttributeViewModel>();
                    ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    return attributeDataViewModel;
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    return (GlobalAttributeViewModel)GetViewModelWithErrorMessage(new GlobalAttributeViewModel(), Admin_Resources.UpdateErrorMessage);
                }
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    return (GlobalAttributeViewModel)GetViewModelWithErrorMessage(new GlobalAttributeViewModel(), Admin_Resources.UpdateErrorMessage);
                }
            }
            else
                return (GlobalAttributeViewModel)GetViewModelWithErrorMessage(new GlobalAttributeViewModel(), Admin_Resources.UpdateErrorMessage);
        }
        #endregion

        //Bind Filter Values
        private void BindEntityFilterValues(GlobalAttributeGroupListViewModel attributeViewModel, int entityId, string entityType)
        {
            attributeViewModel.GlobalEntity = string.IsNullOrEmpty(entityType) ? Admin_Resources.LabelAllEntities : entityType;
            attributeViewModel.GlobalEntityId = entityId;
        }

        //Bind EntityId with filters
        private void BindEntityIdWithFilters(FilterCollection filters, int entityId)
        {
            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeGlobalEntityEnum.GlobalEntityId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            if (entityId > 0)
                filters.Add(ZnodeGlobalEntityEnum.GlobalEntityId.ToString(), FilterOperators.Equals, entityId.ToString());
        }

        //Bind Entity Name
        private string BindEntityName(int globalEntityId)
        {
            List<SelectListItem> entityTypes = GetGlobalEntityTypes();
            return entityTypes.FirstOrDefault(x => x.Value == globalEntityId.ToString()).Text;
        }


    }
}
