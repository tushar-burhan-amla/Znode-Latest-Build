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
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class AttributesAgent : BaseAgent, IAttributesAgent
    {
        #region Private Members
        private readonly IAttributesClient _attributesClient;
        private readonly ILocaleClient _localeClient;
        private readonly IAttributeGroupClient _attributesGroupClient;
        #endregion

        #region public Constructor
        public AttributesAgent(IAttributesClient attributesClient, ILocaleClient localeClient, IAttributeGroupClient attributeGroupClient)
        {
            _attributesClient = GetClient<IAttributesClient>(attributesClient);
            _localeClient = GetClient<ILocaleClient>(localeClient);
            _attributesGroupClient = GetClient<IAttributeGroupClient>(attributeGroupClient);
        }
        #endregion

        #region public Methods
        //Get attribute List
        public virtual AttributesListViewModel AttributeList(FilterCollectionDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            AttributesListViewModel attributesViewModel = AttributeList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Set tool menu for attribute list grid view.
            SetAttributeListToolMenu(attributesViewModel);

            //Get Grid Values for media attribute
            attributesViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, attributesViewModel.List, GridListType.ZnodeMediaAttribute.ToString(), "", null, true, true, attributesViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            attributesViewModel.GridModel.TotalRecordCount = attributesViewModel.TotalResults;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            return attributesViewModel;
        }

        //Get AttributeList
        public virtual AttributesListViewModel AttributeList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection });

            AttributesListModel list = _attributesClient.GetAttributeList(null, filters, sortCollection, pageIndex, recordPerPage);
            AttributesListViewModel listViewModel = new AttributesListViewModel { List = list?.Attributes?.ToViewModel<AttributesViewModel>().ToList() };

            SetListPagingData(listViewModel, list);

            return listViewModel?.List?.Count > 0 ? listViewModel
                : new AttributesListViewModel { List = new List<AttributesViewModel>() };
        }

        //Create Attribute
        public virtual AttributesViewModel Create()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            AttributesViewModel viewModel = new AttributesViewModel();
            Dictionary<string, string> tabs = new Dictionary<string, string>();
            tabs.Add(Attributes_Resources.TitleAttributeDetails, $"/Attributes/Attribute");
            tabs.Add(Attributes_Resources.TitleLocaleValues, $"/Attributes/Locale");
            viewModel.Tabs = SetTabData(tabs);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
    
            return viewModel;
        }

        //Get Attribute bt attribute id
        public virtual AttributesViewModel GetAttribute(int attributeId)
        {
          ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
          ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { attributeId = attributeId });

           if (attributeId > 0)
            {
                AttributesViewModel viewModel = _attributesClient.GetAttribute(attributeId, null).ToViewModel<AttributesViewModel>();
                viewModel.AttributeTypes = GetAttributeTypes();
                ReplaceMediaAttributeType(viewModel);

                TabViewListModel tabList = new TabViewListModel();
                SetTabData(Attributes_Resources.TitleAttributeDetails, $"/Attributes/Attribute?attributeId={attributeId}", tabList);
                SetTabData(Attributes_Resources.TitleLocaleValues, $"/Attributes/Locale?attributeId={attributeId}", tabList);
                tabList.MaintainAllTabData = true;

                viewModel.Tabs = tabList;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return viewModel;
            }
               ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return new AttributesViewModel() { AttributeTypes = GetAttributeTypes() };
        }

        //Get Attribute Validation List with Rules
        public virtual List<AttributeInputValidationModel> AttributeInputValidations(int typeId, int attributeId = 0) => GetAttributeInputValidations(typeId, attributeId);

        //Save Attribute Data
        public virtual AttributesViewModel Save(BindDataModel model)
        {
           ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
              try
              {
                string AttributeCode = Convert.ToString(model.GetValue("AttributeCode"));

                if (string.IsNullOrEmpty(AttributeCode))
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorAttributeCreate);

                AttributesDataModel datamodel = _attributesClient.CreateAttributeModel(GetAttributeDataModel(model));

                AttributesViewModel _model = datamodel.ToViewModel<AttributesViewModel>();

                model.ControlsData["AttributeId"] = _model.MediaAttributeId;
                SaveLocale(model);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return _model;
               }
              catch (ZnodeException ex)
               {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return new AttributesViewModel { HasError = true, ErrorMessage = Attributes_Resources.ErrorAttributeAlreadyExists };
                    case ErrorCodes.InvalidData:
                        return new AttributesViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorAttributeCreate };
                    default:
                        return new AttributesViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                return new AttributesViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
            }
        }

        //Update The Existing Attribute Values
        public virtual AttributesViewModel Update(BindDataModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                AttributesViewModel _model = _attributesClient.UpdateAttributeModel(GetAttributeDataModel(model)).ToViewModel<AttributesViewModel>();
                SaveLocale(model);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return _model;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributesViewModel errorModel = new AttributesViewModel();
                errorModel.MediaAttributeId = Convert.ToInt32(model.GetValue("AttributeId"));
                errorModel.HasError = true;
                return errorModel;
            }
        }

        //Delete existing Attribute By Attribute Id
        public virtual bool DeleteAttribute(string AttributeIds, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(AttributeIds))
            {
                try
                {
                    return _attributesClient.DeleteAttributeModel(new ParameterModel { Ids = AttributeIds });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = MediaManager_Resources.ErrorDeleteAttribute;
                            return false;
                        default:
                            errorMessage = Admin_Resources.ErrorFailedToDelete;
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Get attribute Group list
        public virtual List<SelectListItem> AttributeGroupList(int attributeGroupId)
            => AttributesModelMap.GetAttributeGroups(attributeGroupId, _attributesGroupClient.GetAttributeGroupList(null, null, null).AttributeGroups);

        //Get Locale By attribute Id
        public virtual List<LocaleDataModel> GetLocales(int attributeId) =>
        AttributesModelMap.ToLocaleDataModel(LocaleList(), GetAttributeLocale(attributeId));

        //Get Default Values by attribute Id
        public virtual List<DefaultValueListModel> DefaultValues(int attributeId) =>
            AttributesModelMap.ToDefaultValues(LocaleList(), attributeId == 0 ? null : GetDefaultValues(attributeId));

        //Save Default Values
        public virtual int SaveDefaultValues(string model, int attributeId, string defaultvaluecode, int defaultvalueId = 0)
        {
            AttributesDefaultValueModel _model = _attributesClient.SaveDefaultValues(AttributesModelMap.ToAttributeDefaultValuesModel(model, attributeId, defaultvaluecode, defaultvalueId));
            return _model.DefaultAttributeValueId;
        }

        //Save Default Values
        public virtual bool DeleteDefaultValues(int defaultvalueId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            errorMessage = string.Empty;
            try
            {
                return _attributesClient.DeleteDefaultValues(defaultvalueId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.RestrictSystemDefineDeletion:
                        errorMessage = PIM_Resources.ErrorDeleteDefaultAttributeValues;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }
        //Get regular expression value.
        public virtual string GetValidationRuleRegularExpression(int AttributeTypeId, string ruleName = null)
            => AttributeInputValidations(AttributeTypeId, 0)?.Where(x => x.Name.Contains(ZnodeAttributeInputValidationRuleEnum.ValidationRule.ToString())).ToList()?.FirstOrDefault().Rules.Where(x => x.ValidationName.Contains(ruleName)).FirstOrDefault().RegExp;

        public virtual bool IsAttributeCodeExist(string attributeCode)
            => !string.IsNullOrEmpty(attributeCode) ? _attributesClient.IsAttributeCodeExist(attributeCode) : true;

        //Check whether the attribute default value code already exists.
        public virtual bool CheckAttributeDefaultValueCodeExist(int attributeId, string defaultValueCode, int defaultValueId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            //Get the attribute default value list.
            AttributesDefaultValueListModel attributedefaultValueList = GetDefaultValues(attributeId);
            if (IsNotNull(defaultValueCode) && IsNotNull(attributedefaultValueList.DefaultValues))
            {
                if (defaultValueId > 0)
                {
                    //Set the status in case the default value code is open in edit mode.
                    AttributesDefaultValueModel attributeDefaultValueModel = attributedefaultValueList.DefaultValues.Find(x => x.DefaultAttributeValueId == defaultValueId);
                    if (IsNotNull(attributeDefaultValueModel))
                        return !Equals(attributeDefaultValueModel.AttributeDefaultValueCode, defaultValueCode);
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return attributedefaultValueList.DefaultValues.Any(x => x.AttributeDefaultValueCode.Equals(defaultValueCode));
            }
            return false;
        }
        #endregion

        #region Private Methods
        //Get Deafult values By Attribute ID
        private AttributesDefaultValueListModel GetDefaultValues(int attributeId) =>
           _attributesClient.GetDefaultValues(attributeId);

        //Get attribute Locale By Attribute Id
        private AttributesLocaleListModel GetAttributeLocale(int attributeId) => attributeId == 0 ? null : _attributesClient.GetAttributeLocale(attributeId);

        //Get Attribute Types
        private List<AttributeTypeModel> GetAttributeTypes()

        {
            List<AttributeTypeDataModel> attributeTypes = _attributesClient.GetAttributeTypes().Types;
            ZnodeLogging.LogMessage("attributeTypes count :", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { attributeTypesCount = attributeTypes?.Count });

            if (attributeTypes?.Count > 0)
                return attributeTypes.ToViewModel<AttributeTypeModel>().ToList();
            else
                return new List<AttributeTypeModel>();
        }

        //Get Attribute Validation List
        private List<AttributeInputValidationModel> GetAttributeInputValidations(int typeId, int attributeId = 0) => AttributesModelMap.ToInputValidationListModel(_attributesClient.GetInputValidations(typeId, attributeId).InputValidations);

        //Get attribute Data
        private AttributesDataModel GetAttributeDataModel(BindDataModel model)
        {
            AttributesDataModel dataModel = AttributesModelMap.ToDataModel(model);
            dataModel.ValidationRule = GetAttributeValidationList(model, Convert.ToInt32(dataModel.AttributeTypeId));
            return dataModel;
        }

        //Save attribute Locale
        private void SaveLocale(BindDataModel model) =>
            _attributesClient.SaveLocales(AttributesModelMap.ToLocaleListModel(model));

        //Method To Get Locale List 
        private LocaleListModel LocaleList()
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.IsActive, FilterOperators.Equals, "true"));
            return _localeClient.GetLocaleList(null, filters, null, null, null);
        }

        //Get attributevalidation list from BindDataModel.
        private List<AttributesValidationModel> GetAttributeValidationList(BindDataModel model, int attributeTypeId) => AttributesModelMap.ToAttributeValidationModel(model, AttributeInputValidations(attributeTypeId));

        //In media Attributes their is no Image, Audio,Video and File Attribute types so change the Attribute Type Dropdown value by checking Attribute Code
        private void ReplaceMediaAttributeType(AttributesViewModel attribute)
        {
            if (ControlTypes.Image.ToString().Equals(attribute.AttributeCode) && attribute.AttributeTypes?.Count > 0)
                attribute.AttributeTypes.First().AttributeTypeName = ControlTypes.Image.ToString();
            else if (ControlTypes.File.ToString().Equals(attribute.AttributeCode) && attribute.AttributeTypes?.Count > 0)
                attribute.AttributeTypes.First().AttributeTypeName = ControlTypes.File.ToString();
            else if (ControlTypes.Video.ToString().Equals(attribute.AttributeCode) && attribute.AttributeTypes?.Count > 0)
                attribute.AttributeTypes.First().AttributeTypeName = ControlTypes.Video.ToString();
            else if (ControlTypes.Audio.ToString().Equals(attribute.AttributeCode) && attribute.AttributeTypes?.Count > 0)
                attribute.AttributeTypes.First().AttributeTypeName = ControlTypes.Audio.ToString();
        }

        //Set tool menu for attribute list grid view.
        private void SetAttributeListToolMenu(AttributesListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('MediaDeleteAttributePopup')", ControllerName = "Attributes", ActionName = "Delete" });
            }
        }
        #endregion
    }
}