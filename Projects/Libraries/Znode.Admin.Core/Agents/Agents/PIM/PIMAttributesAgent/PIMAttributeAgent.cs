using System;
using System.Collections.Generic;
using Znode.Engine.Api.Client;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Exceptions;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using Znode.Engine.Api.Models;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using System.Linq;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class PIMAttributeAgent : BaseAgent, IPIMAttributeAgent
    {
        #region Private Members
        private readonly IPIMAttributeClient _attributesClient;
        private readonly ILocaleClient _localeClient;
        private readonly IPIMAttributeGroupClient _attributesGroupClient;
        #endregion

        #region Public Constructor
        public PIMAttributeAgent(IPIMAttributeClient pIMAttributeClient, ILocaleClient localeClient, IPIMAttributeGroupClient pIMAttributeGroupClient)
        {
            _attributesClient = GetClient<IPIMAttributeClient>(pIMAttributeClient);
            _localeClient = GetClient<ILocaleClient>(localeClient);
            _attributesGroupClient = GetClient<IPIMAttributeGroupClient>(pIMAttributeGroupClient);
        }
        #endregion

        #region Public Methods

        //Get PIM attribute list.
        public virtual PIMAttributeListViewModel AttributeList(FilterCollectionDataModel model, string gridListName, string IsCategory)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Sets the IsCategory filter
            HelperMethods.SetIsCategoryFilters(model.Filters, IsCategory);
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { sorts = model.SortCollection, filter= model.Filters });
            PIMAttributeListViewModel attributesViewModel = AttributeList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Set tool menu for product attribute list grid view.
            SetPIMAttributeListToolMenu(attributesViewModel);

            //GEt Grid Values for Pim attribute.
            attributesViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, attributesViewModel.List, gridListName, "", null, true, true, attributesViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            attributesViewModel.GridModel.TotalRecordCount = attributesViewModel.TotalResults;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return attributesViewModel;
        }

        //Get PIM attribute list.
        public virtual PIMAttributeListViewModel AttributeList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sortCollection });
            //Set locale filters if not present
            SetLocaleFilterIfNotPresent(ref filters);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return PIMAttributeViewModelMap.ToListViewModel(_attributesClient.GetAttributeList(null, filters, sortCollection, pageIndex, recordPerPage));
        }

        //Create PIM attribute.
        public virtual PIMAttributeViewModel Create(bool isCategory) => new PIMAttributeViewModel() { Tabs = GetTabsData(isCategory), IsCategory = isCategory };

        //Get PIM attribute details.
        public virtual PIMAttributeViewModel Attribute(int attributeId, bool isCategory) => GetAttributeData(attributeId, isCategory);

        //Get attribute validation list with rules.
        public virtual List<AttributeInputValidationModel> AttributeInputValidations(int typeId, int attributeId = 0) => GetAttributeInputValidations(typeId, attributeId);

        //Save complete attribute date.
        public virtual PIMAttributeDataViewModel Save(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                string AttributeCode = Convert.ToString(model.GetValue("AttributeCode"));
                if (string.IsNullOrEmpty(AttributeCode))
                    throw new ZnodeException(ErrorCodes.InvalidData, "Attribute Can not be create without attribute code ");

                PIMAttributeDataViewModel _model = PIMAttributeViewModelMap.ToDataViewModel(_attributesClient.CreateAttributeModel(GetAttributeDataModel(model)));
                model.ControlsData["AttributeId"] = _model.AttributeViewModel.PimAttributeId;
                SaveLocale(model);
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return _model;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return new PIMAttributeDataViewModel { HasError = true, ErrorMessage = Attributes_Resources.ErrorAttributeAlreadyExists };
                    case ErrorCodes.InvalidData:
                        return new PIMAttributeDataViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorAttributeCreate };
                    default:
                        return new PIMAttributeDataViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return new PIMAttributeDataViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };

            }
        }

        //Update the existing attribute values.
        public virtual PIMAttributeDataViewModel Update(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                PIMAttributeDataViewModel _model = PIMAttributeViewModelMap.ToDataViewModel(_attributesClient.UpdateAttributeModel(GetAttributeDataModel(model)));
                SaveLocale(model);
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return _model;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                return UpdateErrorMessage(model, Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return UpdateErrorMessage(model, Admin_Resources.UpdateErrorMessage);

            }
        }

        //Delete existing Attribute by attribute Id.
        public virtual bool DeleteAttribute(string pimAttributeIds, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(pimAttributeIds))
            {
                try
                {
                    return _attributesClient.DeleteAttributeModel(new ParameterModel { Ids = pimAttributeIds });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = PIM_Resources.ErrorDeleteAttribute;
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
            return false;
        }

        //PIM attribute group list.
        public virtual List<SelectListItem> AttributeGroupList(int attributeGroupId, string isCategory)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodePimAttributeGroupEnum.IsCategory.ToString(), FilterOperators.Equals, isCategory);
            filter.Add(ZnodePimAttributeGroupEnum.IsNonEditable.ToString(), FilterOperators.Equals, "0");
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filter = filter });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return PIMAttributeViewModelMap.GetAttributeGroups(attributeGroupId, _attributesGroupClient.GetAttributeGroupList(null, filter, null)?.AttributeGroupList);
        }

        //Get Locale by attribute Id.
        public virtual List<LocaleDataModel> GetLocales(int attributeId) =>
        PIMAttributeViewModelMap.ToLocaleDataModel(LocaleList(), GetAttributeLocale(attributeId));

        //Get PIM attribute default value by attribute Id.
        public virtual List<DefaultValueListModel> DefaultValues(int attributeId)
          => PIMAttributeViewModelMap.ToDefaultValues(LocaleList(), attributeId == 0 ? null : GetDefaultValues(attributeId));

        //Get frontend properties by attribute Id.
        public virtual PIMFrontPropertiesViewModel FrontEndProperties(int pimAttributeId) => PIMAttributeViewModelMap.ToFrontEndViewModel(_attributesClient.FrontEndProperties(pimAttributeId));

        //Get attribute by attribute Id.
        public virtual PIMAttributeViewModel GetAttribute(int attributeId, bool isCategory) => GetAttributeData(attributeId, isCategory);

        //Save PIM attribute default values.
        public virtual int SaveDefaultValues(string model, int attributeId, string defaultValueCode, bool isDefault, bool isSwatch, string swatchText, int displayOrder = 0, int defaultValueId = 0)
         => _attributesClient.SaveDefaultValues(PIMAttributeViewModelMap.ToAttributeDefaultValuesModel(model, attributeId, defaultValueCode, isDefault, isSwatch, swatchText, displayOrder, defaultValueId)).PimDefaultAttributeValueId;

        //Delete PIM attribute default value.
        public virtual bool DeleteDefaultValues(int defaultvalueId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            errorMessage = string.Empty;

            try
            {
                return _attributesClient.DeleteDefaultValues(defaultvalueId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.RestrictSystemDefineDeletion:
                        errorMessage = PIM_Resources.ErrorDeleteDefaultProductAttributeValues;
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

        // Check attribute Code already exist or not
        public virtual bool IsAttributeCodeExist(string attributeCode, bool isCategory)
           => !string.IsNullOrEmpty(attributeCode) ? _attributesClient.IsAttributeCodeExist(attributeCode, isCategory) : true;

        //Check value of attribute is already exists or not.
        public virtual string IsAttributeValueUnique(string attributeCodeValues, int id, bool isCategory)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(attributeCodeValues))
            {
                string[] arrayAttributeCodeValues = attributeCodeValues.Split('~');
                ParameterModel parameterModel = _attributesClient.IsAttributeValueUnique(GetPimAttributeValueParameterDetails(arrayAttributeCodeValues, id, isCategory));
                if (!string.IsNullOrEmpty(parameterModel?.Ids))
                    errorMessage = string.Format(PIM_Resources.ErrorAlreadyExistsAttributeCode, parameterModel.Ids);
            }
            ZnodeLogging.LogMessage("Output Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { errorMessage = errorMessage });
            return errorMessage;
        }

        #endregion

        #region Private Methods
        //Get Deafult values By Attribute ID
        private PIMAttributeDefaultValueListModel GetDefaultValues(int attributeId) =>
           _attributesClient.GetDefaultValues(attributeId);

        //Get attribute Locale By Attribute Id
        private PIMAttributeLocaleListModel GetAttributeLocale(int attributeId) => attributeId == 0 ? null : _attributesClient.GetAttributeLocale(attributeId);

        //Create tab structure.
        private TabViewListModel GetTabsData(bool isCategory)
        {
            Dictionary<string, string> tabs = new Dictionary<string, string>();
            string controllerName = isCategory ? "CategoryAttribute" : "ProductAttribute";

            tabs.Add(Attributes_Resources.TitleAttributeDetails, $"/" + controllerName + "/Attribute");
            tabs.Add(Attributes_Resources.TitleLocaleValues, $"/" + controllerName + "/Locale");

            return SetTabData(tabs);
        }

        //Get Attribute Types
        private List<AttributeTypeModel> GetAttributeTypes(bool isCategory) => PIMAttributeViewModelMap.ToAttributeTypeListModel(_attributesClient.GetAttributeTypes(isCategory).Types);

        //Get Attribute Validation List
        private List<AttributeInputValidationModel> GetAttributeInputValidations(int typeId, int attributeId = 0) => PIMAttributeViewModelMap.ToInputValidationListModel(_attributesClient.GetInputValidations(typeId, attributeId).InputValidations);

        //Get attribute data.
        private PIMAttributeDataModel GetAttributeDataModel(BindDataModel bindDataModel)
        {
            PIMAttributeDataModel pimAttributeDataModel = PIMAttributeViewModelMap.ToDataModel(bindDataModel);
            pimAttributeDataModel.ValidationRule = GetAttributeValidationList(bindDataModel, Convert.ToInt32(pimAttributeDataModel.AttributeModel.AttributeTypeId));
            return pimAttributeDataModel;
        }

        //Save attribute locale
        private void SaveLocale(BindDataModel bindDataModel)
         => _attributesClient.SaveLocales(PIMAttributeViewModelMap.ToLocaleListModel(bindDataModel));

        //Get PIM attribute details by Id.
        private PIMAttributeViewModel GetAttributeData(int attributeId, bool isCategory)
        {
            if (attributeId > 0)
            {
                PIMAttributeViewModel viewModel = PIMAttributeViewModelMap.ToPIMAttributeViewModel(_attributesClient.GetAttribute(attributeId, null));
                if (IsNotNull(viewModel))
                {
                    viewModel.AttributeTypes = GetAttributeTypes(isCategory);
                    viewModel.IsCategory = isCategory;
                    TabViewListModel tabList = new TabViewListModel();
                    string controllerName = isCategory ? "CategoryAttribute" : "ProductAttribute";

                    SetTabData(Attributes_Resources.TitleAttributeDetails, $"/" + controllerName + "/Attribute?attributeId=" + attributeId, tabList);
                    SetTabData(Attributes_Resources.TitleLocaleValues, $"/" + controllerName + "/Locale?attributeId=" + attributeId, tabList);
                    tabList.MaintainAllTabData = true;

                    viewModel.Tabs = tabList;
                }
                return viewModel;
            }
            return new PIMAttributeViewModel() { AttributeTypes = GetAttributeTypes(isCategory), IsCategory = isCategory };
        }

        //Method to get locale list 
        private LocaleListModel LocaleList()
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "true"));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters });
            return _localeClient.GetLocaleList(null, filters, null, null, null);
        }

        //Get attributevalidation list from BindDataModel.
        private List<PIMAttributeValidationModel> GetAttributeValidationList(BindDataModel model, int attributeTypeId) => PIMAttributeViewModelMap.ToAttributeValidationModel(model, AttributeInputValidations(attributeTypeId));

        //Get PIM attributevalue parameter details.
        private PimAttributeValueParameterModel GetPimAttributeValueParameterDetails(string[] attributeCodeValues, int id, bool isCategory)
        {
            List<PIMAttributeCodeValueModel> pimAttributeCodeValueList = new List<PIMAttributeCodeValueModel>();
            foreach (var attribute in attributeCodeValues)
                pimAttributeCodeValueList.Add(new PIMAttributeCodeValueModel { AttributeCode = attribute.Split('#')[0], AttributeValues = attribute.Split('#')[1].Trim() });
            ZnodeLogging.LogMessage("pimAttributeCodeValueList list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeCodeValueList?.Count());
            return new PimAttributeValueParameterModel()
            {
                IsCategory = isCategory,
                Id = id,
                LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale),
                PIMAttributeCodeValueList = pimAttributeCodeValueList
            };
        }

        //Set tool menu for product attribute list grid view.
        private void SetPIMAttributeListToolMenu(PIMAttributeListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PIMDeleteAttributePopup')", ControllerName = "ProductAttribute", ActionName = "Delete" });
            }
        }

        //For setting error messages
        private PIMAttributeDataViewModel UpdateErrorMessage(BindDataModel model, string message)
        {
            PIMAttributeDataViewModel errorModel = new PIMAttributeDataViewModel { AttributeViewModel = new PIMAttributeViewModel() };
            errorModel.AttributeViewModel.PimAttributeId = Convert.ToInt32(model.GetValue("AttributeId"));
            errorModel.HasError = true;
            errorModel.ErrorMessage = message;
            return errorModel;
        }

        //Check whether the attribute default value code already exists.
        public virtual bool CheckAttributeDefaultValueCodeExist(int attributeId, string defaultValueCode, int defaultValueId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Get the attribute default value list.
            PIMAttributeDefaultValueListModel attributedefaultValueList = GetDefaultValues(attributeId);
            if (IsNotNull(defaultValueCode) && IsNotNull(attributedefaultValueList.DefaultValues))
            {
                if (defaultValueId > 0)
                {
                    //Set the status in case the default value code is open in edit mode.
                    PIMAttributeDefaultValueModel pimAttributeDefaultValueModel = attributedefaultValueList.DefaultValues.Find(x => x.PimDefaultAttributeValueId == defaultValueId);
                    if (IsNotNull(pimAttributeDefaultValueModel))
                        return !Equals(pimAttributeDefaultValueModel.AttributeDefaultValueCode, defaultValueCode);
                }
                return attributedefaultValueList.DefaultValues.Any(x => x.AttributeDefaultValueCode.Equals(defaultValueCode));
            }
            return false;
        }
        #endregion
    }
}