using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Newtonsoft.Json;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class PIMAttributeGroupAgent : BaseAgent, IPIMAttributeGroupAgent
    {
        #region Private Variables
        private readonly IPIMAttributeGroupClient _pimAttributeGroupClient;
        private readonly ILocaleClient _localeClient;
        #endregion

        #region Constructor
        public PIMAttributeGroupAgent(IPIMAttributeGroupClient pIMAttributeGroupClient, ILocaleClient ILocaleClient)
        {
            _pimAttributeGroupClient = GetClient<IPIMAttributeGroupClient>(pIMAttributeGroupClient);
            _localeClient = GetClient<ILocaleClient>(ILocaleClient);
        }
        #endregion

        #region Public Methods

        //Get PIM group locales by groupId.
        public virtual List<LocaleDataModel> GetLocales(int pimAttributeGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "true"));
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose,new{ filters= filters });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return PIMAttributeGroupViewModelMap.ToLocaleDataModel(_localeClient.GetLocaleList(null, filters, null, null, null), _pimAttributeGroupClient.GetPIMAttributeGroupLocales(pimAttributeGroupId));
        }

        //Create PIM attribute group.
        public virtual PIMAttributeGroupViewModel Create(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                PIMAttributeGroupModel createdModel = _pimAttributeGroupClient.CreateAttributeGroupModel(PIMAttributeGroupViewModelMap.ToModel(model));
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return !Equals(createdModel, null) ? createdModel.ToViewModel<PIMAttributeGroupViewModel>() : new PIMAttributeGroupViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (PIMAttributeGroupViewModel)GetViewModelWithErrorMessage(new PIMAttributeGroupViewModel(), PIM_Resources.ErrorAttributeGroupAlreadyExists);
                    default:
                        return (PIMAttributeGroupViewModel)GetViewModelWithErrorMessage(new PIMAttributeGroupViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return (PIMAttributeGroupViewModel)GetViewModelWithErrorMessage(new PIMAttributeGroupViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }
        //Get PIM attribute group details.
        public virtual PIMAttributeGroupViewModel Get(int pimAttributeGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            PIMAttributeGroupModel pimAttributeGroupModel = _pimAttributeGroupClient.GetAttributeGroup(pimAttributeGroupId, null);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return HelperUtility.IsNotNull(pimAttributeGroupModel) ? pimAttributeGroupModel.ToViewModel<PIMAttributeGroupViewModel>() : null;
        }

        //Get assign attributes.
        public virtual PIMAttributeGroupMapperListViewModel GetAssignedAttributes(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (Equals(expands, null))
                expands = new ExpandCollection();
            expands.Add(ExpandKeys.Attributes);
            sortCollection = new SortCollection();
            sortCollection.Add("ZnodePimAttribute.DisplayOrder", "asc");
            sortCollection.Add("ZnodePimAttribute.PimAttributeId", "asc");
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters= filters,expands = expands, sorts = sortCollection });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return PIMAttributeGroupViewModelMap.ToListViewModel(_pimAttributeGroupClient.GetAssignedAttributes(expands, filters, sortCollection, pageIndex, recordPerPage));
        }

        //Create tab structure.
        public virtual TabViewListModel GetTabStructure(int pimAttributeGroupId, bool isCategory)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            TabViewListModel tabList = new TabViewListModel();
            tabList.MaintainAllTabData = false;
            string controllerName = isCategory ? "CategoryAttributeGroup" : "ProductAttributeGroup";
            if (pimAttributeGroupId > 0)
            {
                tabList.Tabs.Add(new TabViewModel() { Id = 1, Name = PIM_Resources.TitlePIMAttributeList, IsVisible = true, Url = $"/PIM/{controllerName}/GetAssignedAttribute?id={pimAttributeGroupId}", IsSelected = true });
                tabList.Tabs.Add(new TabViewModel() { Id = 2, Name = Attributes_Resources.TitleLocale, IsVisible = true, Url = $"/PIM/{controllerName}/CreateAttributeGroupLocale?id={pimAttributeGroupId}" });
            }
            else
                tabList.Tabs.Add(new TabViewModel() { Id = 1, Name = Attributes_Resources.TitleLocale, IsVisible = true, Url = $"/PIM/{controllerName}/CreateAttributeGroupLocale?id={pimAttributeGroupId}", IsSelected = true });

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return tabList;
        }

        //Set filters for PIM attribute group Id.
        public virtual void SetFilters(FilterCollection filters, int pimAttributeGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (!Equals(filters, null))
            {
                //Checking For PIMAttributeGroupId already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == FilterKeys.PIMAttributeGroupId))
                {
                    //If PIMAttributeGroupId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == FilterKeys.PIMAttributeGroupId);

                    //Add New PIMAttributeGroupId Into filters
                    filters.Add(new FilterTuple(FilterKeys.PIMAttributeGroupId, FilterOperators.Equals, pimAttributeGroupId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.PIMAttributeGroupId, FilterOperators.Equals, pimAttributeGroupId.ToString()));
            }
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
        }

        //Get PIM unassign group attribute.
        public virtual List<BaseDropDownList> GetUnAssignedAttributes(int pimAttributeGroupId, bool isCategory)
            => PIMAttributeGroupViewModelMap.ToBaseDropDownList(_pimAttributeGroupClient.GetUnAssignedAttributes(new ExpandCollection { ZnodePimAttributeEnum.ZnodePimAttributeLocales.ToString() }, GetPIMAttributeGroupFilter(pimAttributeGroupId, isCategory), null, null, null));

        //Get PIM attribute list.
        public virtual PIMAttributeGroupListViewModel GetPIMAttributeGroups(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sortCollection });
            //set locale filters if not present.
            SetLocaleFilterIfNotPresent(ref filters);

            PIMAttributeGroupListViewModel attributeGroups = PIMAttributeGroupViewModelMap.ToListViewModel(_pimAttributeGroupClient.GetAttributeGroupList(null, filters, sortCollection, pageIndex, recordPerPage));

            //Set tool menu for pim attribute group list grid view.
            SetPIMAttributeGroupsToolMenu(attributeGroups);

            if (attributeGroups?.AttributeGroupList?.Count > 0)
                return attributeGroups;
            else
                return new PIMAttributeGroupListViewModel() { AttributeGroupList = new List<PIMAttributeGroupViewModel>() };
        }

        //Update PIM attribute group.
        public virtual PIMAttributeGroupViewModel UpdateAttributeGroup(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                PIMAttributeGroupModel createdModel = _pimAttributeGroupClient.UpdateAttributeGroupModel(PIMAttributeGroupViewModelMap.ToModel(model));
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return !Equals(createdModel, null) ? createdModel.ToViewModel<PIMAttributeGroupViewModel>() : (PIMAttributeGroupViewModel)GetViewModelWithErrorMessage(new PIMAttributeGroupViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return (PIMAttributeGroupViewModel)GetViewModelWithErrorMessage(new PIMAttributeGroupViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Associate attributes to group.
        public virtual bool AssociateAttributes(string attributeIds, int attributeGroupId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            message = string.Empty;
            if (!string.IsNullOrEmpty(attributeIds) && attributeGroupId > 0)
            {
                try
                {
                    //Assign attributes to group.
                    return !Equals(_pimAttributeGroupClient.AssociateAttributes(PIMAttributeGroupViewModelMap.ToAttributeGroupMapperListModel(attributeIds.Split(',').ToList(), attributeGroupId)), null);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
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
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    message = Admin_Resources.TextInvalidData;
                    return false;
                }
            }
            return false;
        }

        //Unassign PIM attribute from group. 
        public virtual bool RemoveAssociatedAttribute(int pimAttributeGroupId, int pimAttributeId, bool isCategory, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            message = Admin_Resources.ErrorFailedToDelete;
            try
            {
                return _pimAttributeGroupClient.RemoveAssociatedAttributes(new RemoveAssociatedAttributesModel() { PimAttributeGroupId = pimAttributeGroupId, PimAttributeIds = pimAttributeId.ToString(), isCategory = isCategory });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        message = PIM_Resources.ErrorDeleteAttribute;
                        return false;
                    default:
                        message = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Delete PIM attribute group.
        public virtual bool Delete(string pimAttributeGroupIds, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(pimAttributeGroupIds))
            {
                try
                {
                    return _pimAttributeGroupClient.DeleteAttributeGroupModel(new ParameterModel { Ids = pimAttributeGroupIds });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = PIM_Resources.ErrorDeleteAttributeGroup;
                            return false;
                        default:
                            errorMessage = Admin_Resources.ErrorFailedToDelete;
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            errorMessage = Admin_Resources.ErrorFailedToDelete;
            return false;
        }

        //Check whether the group code already exists.
        public virtual bool CheckGroupCodeExist(string groupCode, bool isCategory, int pimAttributeGroupId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (pimAttributeGroupId > 0)
                return false;

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeGroupEnum.GroupCode.ToString(), FilterOperators.Is, groupCode));
            filters.Add(new FilterTuple(ZnodePimAttributeGroupEnum.IsCategory.ToString(), FilterOperators.Equals, isCategory ? "1" : "0"));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Get the Pim Attribute GroupCode List based on the groupCode code filter.
            PIMAttributeGroupListModel groupList = _pimAttributeGroupClient.GetAttributeGroupList(null, filters, null);
            if (IsNotNull(groupList) && IsNotNull(groupList.AttributeGroupList))
                return groupList.AttributeGroupList.Any(x => string.Equals(x.GroupCode, groupCode, StringComparison.OrdinalIgnoreCase));
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return false;
        }

        //Update Attribute Display Order
        public virtual PIMAttributeDataViewModel UpdateAttributeDisplayOrder(int pimAttributeId, string data)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNotNull(data))
            {
                try
                {
                    PIMAttributeDataViewModel attributeDataViewModel = new PIMAttributeDataViewModel();
                    PIMAttributeDataModel attributeDataModel = new PIMAttributeDataModel();
                    attributeDataViewModel.AttributeViewModel = JsonConvert.DeserializeObject<PIMAttributeViewModel[]>(data)[0];

                    attributeDataModel.AttributeModel = new PIMAttributeModel() { DisplayOrder = attributeDataViewModel.AttributeViewModel.DisplayOrder, PimAttributeId = attributeDataViewModel.AttributeViewModel.PimAttributeId };
                    attributeDataViewModel = PIMAttributeViewModelMap.ToDataViewModel(_pimAttributeGroupClient.UpdateAttributeDisplayOrder(attributeDataModel));
                    ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    return attributeDataViewModel;
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                    return (PIMAttributeDataViewModel)GetViewModelWithErrorMessage(new PIMAttributeDataViewModel(), Admin_Resources.UpdateErrorMessage);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    return (PIMAttributeDataViewModel)GetViewModelWithErrorMessage(new PIMAttributeDataViewModel(), Admin_Resources.UpdateErrorMessage);

                }
            }
            else
                return (PIMAttributeDataViewModel)GetViewModelWithErrorMessage(new PIMAttributeDataViewModel(), Admin_Resources.UpdateErrorMessage);
        }
        #endregion

        #region Private Methods

        //Gets the filter for pimAttributeGroupId
        private FilterCollection GetPIMAttributeGroupFilter(int pimAttributeGroupId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.PIMAttributeGroupId, FilterOperators.Equals, pimAttributeGroupId.ToString()));
            return filters;
        }
        //Create filter for IsCategory.

        private FilterCollection GetPIMAttributeGroupFilter(int pimAttributeGroupId, bool isCategory)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeGroupEnum.IsCategory.ToString(), FilterOperators.Equals, isCategory.ToString()));
            return filters;
        }

        //Set tool menu for category/pim attribute group list grid view.
        private void SetPIMAttributeGroupsToolMenu(PIMAttributeGroupListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PIMDeletePopup')", ActionName = "Delete" });
            }
        }
        #endregion
    }
}