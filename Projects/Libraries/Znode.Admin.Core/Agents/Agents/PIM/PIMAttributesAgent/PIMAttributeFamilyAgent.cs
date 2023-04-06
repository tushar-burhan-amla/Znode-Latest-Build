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
    public class PIMAttributeFamilyAgent : BaseAgent, IPIMAttributeFamilyAgent
    {
        #region Private Variables
        private readonly IPIMAttributeFamilyClient _pimAttributeFamilyClient;

        private readonly ILocaleClient _localeClient;
        #endregion

        #region Constructor
        public PIMAttributeFamilyAgent(IPIMAttributeFamilyClient pIMAttributeFamilyClient, ILocaleClient localeClient)
        {
            _pimAttributeFamilyClient = GetClient<IPIMAttributeFamilyClient>(pIMAttributeFamilyClient);
            _localeClient = GetClient<ILocaleClient>(localeClient);
        }
        #endregion

        #region Public Methods

        //Get PIM attribute family list.
        public virtual List<SelectListItem> GetPIMAttributeFamilyList(string isCategory)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            List<PIMAttributeFamilyModel> familyList = new List<PIMAttributeFamilyModel>();
            filters.Add(new FilterTuple(ZnodePimAttributeFamilyEnum.IsCategory.ToString(), FilterOperators.Equals, isCategory));
            familyList = _pimAttributeFamilyClient.GetAttributeFamilyList(filters, null, null, null)?.PIMAttributeFamilies.ToList();
            ZnodeLogging.LogMessage("familyList list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, familyList?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return PIMAttributeFamilyViewModelMap.ToListItems(familyList);
        }

        //Create PIM attribute family.
        public virtual PIMAttributeFamilyViewModel Save(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                PIMAttributeFamilyViewModel pimAttributeFamilyViewModel = PIMAttributeFamilyViewModelMap.ToViewModel(model);
                PIMAttributeFamilyModel familyModel = _pimAttributeFamilyClient.CreateAttributeFamily(pimAttributeFamilyViewModel.ToModel<PIMAttributeFamilyModel>());
                if (familyModel?.PimAttributeFamilyId > 0)
                    model.ControlsData["PimAttributeFamilyId"] = familyModel.PimAttributeFamilyId;
                SaveFamilyLocales(model);
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return !Equals(familyModel, null) ? familyModel.ToViewModel<PIMAttributeFamilyViewModel>() : new PIMAttributeFamilyViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return new PIMAttributeFamilyViewModel { HasError = true, ErrorMessage = PIM_Resources.ErrorFamilyAlreadyExists };
                    default:
                        return new PIMAttributeFamilyViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return new PIMAttributeFamilyViewModel { HasError = true, ErrorMessage = ex.Message };
            }
        }

        //Get PIM family details.
        public virtual PIMAttributeFamilyViewModel GetPIMAttributeFamily(int pimAttributeFamilyId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            PIMAttributeFamilyModel pimAttributeFamilyModel = _pimAttributeFamilyClient.GetAttributeFamily(pimAttributeFamilyId);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return IsNotNull(pimAttributeFamilyModel) ? pimAttributeFamilyModel.ToViewModel<PIMAttributeFamilyViewModel>() : null;
        }

        //Get assigned PIM attribute groups.
        public virtual PIMAttributeGroupListViewModel GetAssignedPIMAttributeGroups(int pimAttributeFamilyId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
            => PIMAttributeGroupViewModelMap.ToListViewModel(_pimAttributeFamilyClient.GetAssignedAttributeGroups(GetPIMAttributeGroupExpand(), GetPIMAttributeFamilyFilter(pimAttributeFamilyId), sortCollection, pageIndex, recordPerPage));

        //Get unAssigned PIM attribute groups.
        public virtual List<BaseDropDownList> GetUnAssignedPIMAttributeGroups(int pimAttributeFamilyId, bool isCategory)
            => PIMAttributeFamilyViewModelMap.ToBaseDropDownList(_pimAttributeFamilyClient.GetUnAssignedAttributeGroups(null, GetPIMAttributeFamilyFilter(pimAttributeFamilyId, isCategory), null, null, null));

        //Assign PIM attribute group to family.
        public virtual bool AssignAttributeGroups(string attributeGroupIds, int attributeFamilyId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            message = string.Empty;
            if (!string.IsNullOrEmpty(attributeGroupIds) && attributeFamilyId > 0)
            {
                try
                {
                    //Get attributes by attribute group ids.
                    PIMAttributeGroupMapperListModel attributes = _pimAttributeFamilyClient.GetAttributesByGroupIds(new ParameterModel { Ids = attributeGroupIds });

                    //Assign attribute groups to attribute family.
                    return !Equals(_pimAttributeFamilyClient.AssociateAttributeGroups(PIMAttributeFamilyViewModelMap.ToFamilyGroupAttributeListModel(attributeGroupIds.Split(',').ToList(), attributeFamilyId, attributes)), null);
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

        //Unassign PIM attribute group from family.
        public virtual bool UnAssignAttributeGroups(int attributeFamilyId, int attributeGroupId, bool isCategory, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            message = string.Empty;
            bool isUnAssign = false;
            try
            {
                if (attributeFamilyId > 0 && attributeGroupId > 0)
                    //UnAssign attribute group which is associated with attribute family.
                    isUnAssign = _pimAttributeFamilyClient.UnAssociateAttributeGroups(attributeFamilyId, attributeGroupId, isCategory);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        message = Attributes_Resources.ErrorFailToDeleteSystemDefineAttributeGroup;
                        return isUnAssign;
                    default:
                        message = Admin_Resources.TextInvalidData;
                        return isUnAssign;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                message = Admin_Resources.TextInvalidData;
                return isUnAssign;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return isUnAssign;
        }

        //Create tab structure.
        public virtual TabViewListModel CreateTabStructure(int pimAttributeFamilyId, bool isCategory)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            TabViewListModel TabStructModel = new TabViewListModel();
            TabStructModel.MaintainAllTabData = true;
            string controllerName = isCategory ? "CategoryAttributeFamily" : "ProductAttributeFamily";
            if (pimAttributeFamilyId > 0)
            {
                TabStructModel.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.LabelAttributeList, IsVisible = true, Url = $"/{controllerName}/AssignedPIMAttributeGroupList?pimAttributeFamilyId={pimAttributeFamilyId}", IsSelected = true });
                TabStructModel.Tabs.Add(new TabViewModel() { Id = 2, Name = Admin_Resources.LabelLocale, IsVisible = true, Url = $"/{controllerName}/Locale?id={pimAttributeFamilyId}" });
            }
            else
                TabStructModel.Tabs.Add(new TabViewModel() { Id = 2, Name = Admin_Resources.LabelLocale, IsVisible = true, Url = $"/{controllerName}/Locale?id={pimAttributeFamilyId}" });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return TabStructModel;
        }

        //Get PIM attribute family list.
        public virtual PIMAttributeFamilyListViewModel GetPIMAttributeFamilies(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filter = filters,sorts = sortCollection });

            //set locale filters if not present
            SetLocaleFilterIfNotPresent(ref filters);
            PIMAttributeFamilyListViewModel attributeFamilyList = PIMAttributeFamilyViewModelMap.ToListViewModel(_pimAttributeFamilyClient.GetAttributeFamilyList(filters, sortCollection, pageIndex, recordPerPage));

            //Set tool menu for category attribute family list grid view.
            SetPIMAttributeFamiliesToolMenu(attributeFamilyList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return (attributeFamilyList?.List?.Count() > 0) ? attributeFamilyList : new PIMAttributeFamilyListViewModel() { List = new List<PIMAttributeFamilyViewModel>() };
        }

        //Get locale by attribute family Id.
        public virtual List<LocaleDataModel> GetLocales(int pimAttributeFamilyId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "true"));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filter = filters });

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return PIMAttributeFamilyViewModelMap.ToLocaleDataModel(_localeClient.GetLocaleList(null, filters, null, null, null), GetFamilyLocale(pimAttributeFamilyId));
        }

        //Update the existing family locale values.
        public virtual bool SaveFamilyLocales(BindDataModel model)
          => !Equals(_pimAttributeFamilyClient.SaveLocales(PIMAttributeFamilyViewModelMap.ToFamilyLocaleListModel(model)), null);

        //Delete PIM attribute family. 
        public virtual bool DeletePIMAttributeFamily(string pimAttributeFamilyId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(pimAttributeFamilyId))
            {
                try
                {
                    ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    return _pimAttributeFamilyClient.DeleteAttributeFamily(new ParameterModel { Ids = pimAttributeFamilyId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = PIM_Resources.ErrorDeleteFamily;
                            break;
                        default:
                            errorMessage = Admin_Resources.ErrorFailedToDelete;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        //Check whether the family code already exists.
        public virtual bool CheckFamilyCodeExist(string familyCode, bool isCategory, int pimAttributeFamilyId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (pimAttributeFamilyId > 0)
                return false;

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeFamilyEnum.FamilyCode.ToString(), FilterOperators.Is, familyCode));
            filters.Add(new FilterTuple(ZnodeConstant.IsFamilyExists, FilterOperators.Equals, AdminConstants.True));
            filters.Add(new FilterTuple(ZnodePimAttributeFamilyEnum.IsCategory.ToString(), FilterOperators.Equals, isCategory ? "1" : "0"));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filter = filters});

            //Get the Pim Attribute Family List based on the family code filter.
            PIMAttributeFamilyListModel familyList = _pimAttributeFamilyClient.GetAttributeFamilyList(filters, null, null, null);
            if (IsNotNull(familyList) && IsNotNull(familyList.PIMAttributeFamilies))
                return familyList.PIMAttributeFamilies.Any(x => string.Equals(x.FamilyCode, familyCode, StringComparison.OrdinalIgnoreCase));
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return false;
        }

        //Update Attribute Group Display Order
        public virtual PIMAttributeGroupViewModel UpdateAttributeGroupDisplayOrder(int pimAttributeGroupId, int pimAttributeFamilyId, int DisplayOrder)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                PIMAttributeGroupModel createdModel = new PIMAttributeGroupModel() { DisplayOrder = DisplayOrder, PimAttributeGroupId = pimAttributeGroupId, PimAttributeFamilyId = pimAttributeFamilyId };
                PIMAttributeGroupViewModel attributeDataViewModel = _pimAttributeFamilyClient.UpdateAttributeGroupDisplayOrder(createdModel).ToViewModel<PIMAttributeGroupViewModel>();
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return attributeDataViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                return (PIMAttributeGroupViewModel)GetViewModelWithErrorMessage(new PIMAttributeGroupViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return (PIMAttributeGroupViewModel)GetViewModelWithErrorMessage(new PIMAttributeGroupViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        #region Attributes
        //Get assigned attributes.
        public virtual PIMAttributeListViewModel GetAssignedAttributes(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(expands))
                expands = new ExpandCollection();
            expands.Add(ExpandKeys.Attributes);

            if (IsNull(sortCollection))
                sortCollection = new SortCollection();
            sortCollection.Add("ZnodePimAttribute.DisplayOrder", "asc");
            sortCollection.Add("ZnodePimAttribute.PimAttributeId", "asc");
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filter= filters, expand = expands, sorts = sortCollection });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return PIMAttributeViewModelMap.ToListViewModel(_pimAttributeFamilyClient.GetAssignedAttributes(expands, filters, sortCollection, pageIndex, recordPerPage));
        }

        //Get unassigned PIM attributes.
        public virtual List<BaseDropDownList> GetUnAssignedPIMAttributes(int pimAttributeFamilyId, int attributeGroupId)
            => (pimAttributeFamilyId > 0 && attributeGroupId > 0) ? PIMAttributeFamilyViewModelMap.ToBaseDropDownAttributeList(_pimAttributeFamilyClient.GetUnAssignedAttributes(null, GetPIMAttributeFamilyFilter(pimAttributeFamilyId, attributeGroupId), null, null, null)) : new List<BaseDropDownList>();

        //Assign PIM attributes to group.
        public virtual bool AssignAttributes(string attributeIds, int attributeFamilyId, int attributeGroupId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            message = Admin_Resources.TextInvalidData;
            if (!string.IsNullOrEmpty(attributeIds) && attributeFamilyId > 0 && attributeGroupId > 0)
            {
                try
                {
                    AttributeDataModel attributeModel = new AttributeDataModel();
                    attributeModel.PimAttributeIds = attributeIds;
                    attributeModel.AttributeGroupId = attributeGroupId;
                    attributeModel.AttributeFamilyId = attributeFamilyId;
                    ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                    return IsNotNull(_pimAttributeFamilyClient.AssignAttributes(attributeModel));
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
        public virtual bool UnAssignAttributes(int attributeFamilyId, int attributeGroupId, int attributeId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            message = Admin_Resources.TextInvalidData;
            try
            {
                if (attributeFamilyId > 0 && attributeGroupId > 0 && attributeId > 0)
                    //UnAssign attribute which is associated with attribute group.
                    return _pimAttributeFamilyClient.UnAssignAttributes(new AttributeDataModel { AttributeFamilyId = attributeFamilyId, AttributeGroupId = attributeGroupId, PimAttributeId = attributeId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        message = Attributes_Resources.ErrorFailToDeleteSystemDefineAttribute;
                        return false;
                    default:
                        message = Admin_Resources.TextInvalidData;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
            return false;
        }
        #endregion
        #endregion

        #region Private Methods

        //Get family locale by family Id.
        private PIMFamilyLocaleListModel GetFamilyLocale(int pimAttributeFamilyId) => pimAttributeFamilyId == 0 ? null : _pimAttributeFamilyClient.GetFamilyLocale(pimAttributeFamilyId);

        //Create filter for PIM attributefamilyId.
        private FilterCollection GetPIMAttributeFamilyFilter(int pimAttributeFamilyId, int attributeGroupId = 0)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.PIMAttributeFamilyId, FilterOperators.Equals, pimAttributeFamilyId.ToString()));

            if (attributeGroupId > 0)
                filters.Add(new FilterTuple(ZnodePimAttributeGroupEnum.PimAttributeGroupId.ToString(), FilterOperators.Equals, attributeGroupId.ToString()));
            ZnodeLogging.LogMessage("Output Parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info,new { filters = filters });

            return filters;
        }
        //Add PIM family expands.
        private ExpandCollection GetPIMAttributeGroupExpand()
        {
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ExpandKeys.PIMAttributeGroup);
            return expands;
        }

        //Add filters for PimAttributeFamilyId and IsCategory.
        private FilterCollection GetPIMAttributeFamilyFilter(int pimAttributeFamilyId, bool isCategory)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeFamilyEnum.PimAttributeFamilyId.ToString(), FilterOperators.Equals, pimAttributeFamilyId.ToString()));
            filters.Add(new FilterTuple(ZnodePimAttributeFamilyEnum.IsCategory.ToString(), FilterOperators.Equals, isCategory.ToString()));
            return filters;
        }

        //Set tool menu for category/pim attribute family list grid view.
        private void SetPIMAttributeFamiliesToolMenu(PIMAttributeFamilyListViewModel model)
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