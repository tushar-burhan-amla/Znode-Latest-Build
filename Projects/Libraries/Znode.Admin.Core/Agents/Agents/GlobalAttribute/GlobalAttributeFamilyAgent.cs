using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class GlobalAttributeFamilyAgent : BaseAgent, IGlobalAttributeFamilyAgent
    {
        #region Private Variables
        private readonly IGlobalAttributeEntityClient _globalAttributesEntityClient;
        private readonly IGlobalAttributeFamilyClient _globalAttributesFamilyClient;
        #endregion

        #region Constructor
        public GlobalAttributeFamilyAgent(IGlobalAttributeEntityClient globalAttributeEntityClient, IGlobalAttributeFamilyClient globalAttributeFamilyClient)
        {
            _globalAttributesEntityClient = GetClient<IGlobalAttributeEntityClient>(globalAttributeEntityClient);
            _globalAttributesFamilyClient = GetClient<IGlobalAttributeFamilyClient>(globalAttributeFamilyClient);
        }
        #endregion

        //Gets the List of Attribute Family
        public virtual GlobalAttributeFamilyListViewModel List(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int entityId = 0, string entityType = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //set locale filters if not present.
            SetLocaleFilterIfNotPresent(ref filters);

            BindEntityIdWithFilters(filters, entityId);

            GlobalAttributeFamilyListModel listModel = _globalAttributesFamilyClient.GetAttributeFamilyList(null, filters, sortCollection, pageIndex, recordPerPage);
            GlobalAttributeFamilyListViewModel attributeFamilyListModel = new GlobalAttributeFamilyListViewModel { AttributeFamilyList = listModel?.AttributeFamilyList?.ToViewModel<GlobalAttributeFamilyViewModel>().ToList() };

            SetListPagingData(attributeFamilyListModel, listModel);

            SetToolMenu(attributeFamilyListModel);

            BindEntityFilterValues(attributeFamilyListModel, entityId, entityType);

            if (attributeFamilyListModel?.AttributeFamilyList?.Count > 0)
                return attributeFamilyListModel;
            else
                return new GlobalAttributeFamilyListViewModel() { AttributeFamilyList = new List<GlobalAttributeFamilyViewModel>() };
    
        }

        //Create a new Attribute Family
        public virtual GlobalAttributeFamilyViewModel Create(BindDataModel model)
        {
            GlobalAttributeFamilyCreateModel familyModel = GlobalAttributeFamilyViewModelMap.ToCreateModel(model);
            familyModel.GlobalEntityName = BindEntityName(familyModel.GlobalEntityId);
            GlobalAttributeFamilyModel attributeFamilyModel = _globalAttributesFamilyClient.CreateAttributeFamily(familyModel);
            return attributeFamilyModel.ToViewModel<GlobalAttributeFamilyViewModel>(); 
        }

        //Edit Attribute Family
        public virtual GlobalAttributeFamilyViewModel Edit(string familyCode)
        {
           ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            GlobalAttributeFamilyModel model = _globalAttributesFamilyClient.GetAttributeFamily(familyCode);
           GlobalAttributeFamilyViewModel viewModel  = HelperUtility.IsNotNull(model) ? model.ToViewModel<GlobalAttributeFamilyViewModel>() : new GlobalAttributeFamilyViewModel();
           viewModel.GlobalEntityType = GetGlobalEntityTypes();
           return viewModel;
        }

        //Update Attribute Family
        public virtual GlobalAttributeFamilyViewModel UpdateAttributeFamily(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            GlobalAttributeFamilyModel attributeFamilyModel = _globalAttributesFamilyClient.UpdateAttributeFamily(GlobalAttributeFamilyViewModelMap.ToModel(model));
            if (HelperUtility.IsNotNull(attributeFamilyModel))
                return attributeFamilyModel.ToViewModel<GlobalAttributeFamilyViewModel>();

            return (GlobalAttributeFamilyViewModel)GetViewModelWithErrorMessage(new GlobalAttributeFamilyViewModel(), Admin_Resources.UpdateErrorMessage);
           
        }

        //Delete Attribute Family
        public virtual bool DeleteAttributeFamily(string globalAttributeFamilyIds, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
             try
                {
                    return _globalAttributesFamilyClient.DeleteAttributeFamily(new ParameterModel { Ids = globalAttributeFamilyIds });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = Admin_Resources.ErrorDeleteGlobalAttributeFamily;
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

        //Returns the List of Groups that are associated to a Family
        public virtual GlobalAttributeGroupListViewModel GetAssignedAttributeGroups(string familyCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            GlobalAttributeGroupListViewModel model = new GlobalAttributeGroupListViewModel();

            GlobalAttributeGroupListModel globalAttributeGroupList = _globalAttributesFamilyClient.GetAssignedAttributeGroups(familyCode);

            model.AttributeGroupList = globalAttributeGroupList?.AttributeGroupList?.ToViewModel<GlobalAttributeGroupViewModel>()?.ToList();

            return model;
        }

        //Returns the List of Unassigned groups of a Family
        public virtual List<BaseDropDownList> GetUnassignedAttributeGroups(string familyCode)
          => GlobalAttributeFamilyViewModelMap.ToBaseDropDownList(_globalAttributesFamilyClient.GetUnassignedAttributeGroups(familyCode));

        //Assign Attribute Groups to a Family
        public virtual bool AssignAttributeGroups(string attributeGroupIds, string familyCode, out string message)
        {
            message = string.Empty;
            if (!string.IsNullOrEmpty(attributeGroupIds) && !string.IsNullOrEmpty(familyCode))
            {
                try
                {
                    return _globalAttributesFamilyClient.AssignAttributeGroups(attributeGroupIds, familyCode);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    message = Admin_Resources.TextInvalidData;
                    
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToAssign;
                    return false;
                }
            }
            return false;
        }

        //Unassign attribute Groups from a family
        public virtual bool UnassignAttributeGroups(string groupCode, string familyCode, out string message)
        {
            message = string.Empty;
            bool isUnAssign = false;
            try
            {
                if (!string.IsNullOrEmpty(familyCode) && !string.IsNullOrEmpty(groupCode))
                    isUnAssign = _globalAttributesFamilyClient.UnassignAttributeGroups(groupCode, familyCode);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
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
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                message = Admin_Resources.UnassignError;
                return isUnAssign;
            }
            return isUnAssign;
        }


        //Update Attribute Group Display Order
        public virtual bool UpdateAttributeGroupDisplayOrder(string groupCode, string familyCode, int DisplayOrder)
            =>_globalAttributesFamilyClient.UpdateAttributeGroupDisplayOrder(groupCode, familyCode, DisplayOrder);
            
        //Get Global Entity List
        public virtual List<SelectListItem> GetGlobalEntityTypes(bool IsCreate = false)
        {
            GlobalEntityListModel globalEntityList = _globalAttributesEntityClient.GetGlobalEntity();

            List<SelectListItem> entityList = new List<SelectListItem>();

            if(!IsCreate)
                globalEntityList?.GlobalEntityList.ToList().ForEach(item =>{ entityList.Add(new SelectListItem() { Text = item.EntityName, Value = Convert.ToString(item.GlobalEntityId) }); });
             else
                globalEntityList?.GlobalEntityList.Where(x => !x.IsFamilyUnique)?.ToList().ForEach(item =>{ entityList.Add(new SelectListItem() { Text = item.EntityName, Value = Convert.ToString(item.GlobalEntityId) }); });

            return entityList;

        }

        //Create tab structure.
        public virtual TabViewListModel GetTabStructure(string familyCode)
        {
            TabViewListModel tabList = new TabViewListModel();
            tabList.MaintainAllTabData = false;

            if (!string.IsNullOrEmpty(familyCode))
            {
                tabList.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.TitleGlobalAttributesList, IsVisible = true, Url = $"/GlobalAttributeFamily/GetAssignedAttributeGroups?familyCode={familyCode}", IsSelected = true });
                tabList.Tabs.Add(new TabViewModel() { Id = 2, Name = Attributes_Resources.TitleLocale, IsVisible = true, Url = $"/GlobalAttributeFamily/CreateAttributeFamilyLocale?familyCode={familyCode}" });
            }
            else
                tabList.Tabs.Add(new TabViewModel() { Id = 1, Name = Attributes_Resources.TitleLocale, IsVisible = true, Url = $"/GlobalAttributeFamily/CreateAttributeFamilyLocale?id={familyCode}", IsSelected = true });
            return tabList;
        }

        //Get family locales by groupId.
        public virtual List<LocaleDataModel> GetLocales(string familyCode)
            => GlobalAttributeFamilyViewModelMap.ToLocaleDataModel(DefaultSettingHelper.GetActiveLocaleList(), !string.IsNullOrEmpty(familyCode) ? _globalAttributesFamilyClient.GetGlobalAttributeFamilyLocales(familyCode) : new GlobalAttributeFamilyLocaleListModel());

        //Check if the family Code Exists
        public virtual bool IsFamilyCodeExist(string familyCode)
         => !string.IsNullOrEmpty(familyCode) ? _globalAttributesFamilyClient.IsFamilyCodeExist(familyCode) : true;

        //Set tools dropdown
        public void SetToolMenu(GlobalAttributeFamilyListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('GlobalAttributeFamilyDeletePopup')", ControllerName = "GlobalAttributeFamily", ActionName = "Delete" });
            }
        }

        //Bind Filter values
        private void BindEntityIdWithFilters(FilterCollection filters, int entityId)
        {
            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeGlobalEntityEnum.GlobalEntityId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            if (entityId > 0)
                filters.Add(ZnodeGlobalEntityEnum.GlobalEntityId.ToString(), FilterOperators.Equals, entityId.ToString());
        }

        //Bind filter values
        private void BindEntityFilterValues(GlobalAttributeFamilyListViewModel attributeFamilyListModel, int entityId, string entityType)
        {
            attributeFamilyListModel.GlobalEntity = string.IsNullOrEmpty(entityType) ? Admin_Resources.LabelAllEntities : entityType;
            attributeFamilyListModel.GlobalEntityId = entityId;
        }

        //Bind Entity Name
        private string BindEntityName(int globalEntityId)
        {
            List<SelectListItem> entityTypes = GetGlobalEntityTypes();
            return entityTypes.FirstOrDefault(x => x.Value == globalEntityId.ToString()).Text;
        }

    }
}
