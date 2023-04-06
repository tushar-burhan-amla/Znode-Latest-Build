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
    public class GlobalAttributeAgent : BaseAgent, IGlobalAttributeAgent
    {
        #region Private Members
        private readonly IPIMAttributeClient _attributesClient;
        private readonly IGlobalAttributeClient _globalAttributeClient;
        private readonly IGlobalAttributeEntityClient _globalAttributesEntityClient;
        #endregion

        #region Public Constructor
        public GlobalAttributeAgent(IPIMAttributeClient pimAttributeClient, IGlobalAttributeClient globalAttributeClient, IGlobalAttributeEntityClient globalAttributeEntityClient)
        {
            _attributesClient = GetClient<IPIMAttributeClient>(pimAttributeClient);
            _globalAttributesEntityClient = GetClient<IGlobalAttributeEntityClient>(globalAttributeEntityClient);
            _globalAttributeClient = GetClient<IGlobalAttributeClient>(globalAttributeClient);
        }
        #endregion

        #region Public Methods
        //Get global attribute list.
        public virtual GlobalAttributeListViewModel AttributeList(FilterCollectionDataModel model, string gridListName, int entityId = 0, string entityType = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(gridListName, model);

            BindEntityIdWithFilters(model, entityId);
            GlobalAttributeListViewModel attributeViewModel = AttributeList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Set tool menu for global attribute list grid view.
            SetGlobalAttributeListToolMenu(attributeViewModel);

            //Get Grid Values for global attribute.
            attributeViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, attributeViewModel.List, gridListName, "", null, true, true, attributeViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            BindEntityFilterValues(attributeViewModel, entityId, entityType);
            //Set the total record count
            attributeViewModel.GridModel.TotalRecordCount = attributeViewModel.TotalResults;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return attributeViewModel;
        }

        //Create global attribute.
        public virtual GlobalAttributeViewModel Create() => new GlobalAttributeViewModel() { Tabs = GetTabsData() };

        //Save complete attribute date.
        public virtual GlobalAttributeViewModel Save(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                string AttributeCode = Convert.ToString(model.GetValue("AttributeCode"));
                ZnodeLogging.LogMessage("AttributeCode:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, AttributeCode);
                if (string.IsNullOrEmpty(AttributeCode))
                    throw new ZnodeException(ErrorCodes.InvalidData, "Attribute can not be create without attribute code. ");

                GlobalAttributeDataModel attributeDataModel = _globalAttributeClient.CreateAttributeModel(GetAttributeDataModel(model));
                GlobalAttributeViewModel viewModel = GlobalAttributeViewModelMap.ToGlobalAttributeViewModel(attributeDataModel.AttributeModel);

                model.ControlsData["AttributeId"] = viewModel.GlobalAttributeId;
                SaveLocale(model);
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return viewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return new GlobalAttributeViewModel { HasError = true, ErrorMessage = Attributes_Resources.ErrorAttributeAlreadyExists };
                    case ErrorCodes.InvalidData:
                        return new GlobalAttributeViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorAttributeCreate };
                    default:
                        return new GlobalAttributeViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return new GlobalAttributeViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
            }
        }

        //Get global attribute details by Id.
        public virtual GlobalAttributeViewModel GetAttributeData(int attributeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (attributeId > 0)
            {
                GlobalAttributeViewModel viewModel = (_globalAttributeClient.GetAttribute(attributeId, null))?.ToViewModel<GlobalAttributeViewModel>();
                if (IsNotNull(viewModel))
                {
                    viewModel.AttributeTypes = GetAttributeTypes();
                    viewModel.GlobalEntityType = GetGlobalEntityTypes();

                    TabViewListModel tabList = new TabViewListModel();
                    SetTabData(Attributes_Resources.TitleAttributeDetails, $"/GlobalAttribute/Attribute?attributeId=" + attributeId, tabList);
                    SetTabData(Attributes_Resources.TitleLocaleValues, $"/GlobalAttribute/Locale?attributeId=" + attributeId, tabList);
                    tabList.MaintainAllTabData = true;
                    viewModel.Tabs = tabList;
                }                
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return viewModel;
            }

            return new GlobalAttributeViewModel() { AttributeTypes = GetAttributeTypes(), GlobalEntityType = GetGlobalEntityTypes() };
        }

        //Get Global Entity Types
        public virtual List<SelectListItem> GetGlobalEntityTypes()
        {
            GlobalEntityListModel globalEntityList = _globalAttributesEntityClient.GetGlobalEntity();

            List<SelectListItem> entityList = new List<SelectListItem>();
            globalEntityList?.GlobalEntityList?.ToList().ForEach(item => { entityList.Add(new SelectListItem() { Text = item.EntityName, Value = Convert.ToString(item.GlobalEntityId) }); });
            return entityList;

        }
        //Update the existing attribute values.
        public virtual GlobalAttributeViewModel Update(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                GlobalAttributeDataModel _model = _globalAttributeClient.UpdateAttributeModel(GetAttributeDataModel(model));
                GlobalAttributeViewModel attributeViewModel = _model.AttributeModel.ToViewModel<GlobalAttributeViewModel>();
                SaveLocale(model);
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return attributeViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return UpdateErrorMessage(model, Admin_Resources.UpdateErrorMessage);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return UpdateErrorMessage(model, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete existing Attribute by attribute Id.
        public virtual bool DeleteAttribute(string globalAttributeIds, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(globalAttributeIds))
            {
                try
                {
                    return _globalAttributeClient.DeleteAttributeModel(new ParameterModel { Ids = globalAttributeIds });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.AssociationDeleteError:
                            errorMessage = Admin_Resources.ErrorFailToDeleteGlobalAttribute;
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
            return false;
        }

        //Get attribute validation list with rules.
        public virtual List<AttributeInputValidationModel> AttributeInputValidations(int typeId, int attributeId = 0) => GetAttributeInputValidations(typeId, attributeId);

        //Get global attribute list.
        public virtual GlobalAttributeListViewModel AttributeList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Set locale filters if not present
            SetLocaleFilterIfNotPresent(ref filters);
            GlobalAttributeListModel globalAttributeListModel = _globalAttributeClient.GetAttributeList(null, filters, sortCollection, pageIndex, recordPerPage);

            //Maps global attribute list model to global attribute list view model.
            GlobalAttributeListViewModel listViewModel = new GlobalAttributeListViewModel { List = globalAttributeListModel?.Attributes?.ToViewModel<GlobalAttributeViewModel>().ToList() };

            SetListPagingData(listViewModel, globalAttributeListModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return listViewModel;
        }

        //Get Locale by attribute Id.
        public virtual List<LocaleDataModel> GetLocales(int attributeId)
            => GlobalAttributeViewModelMap.ToLocaleDataModel(DefaultSettingHelper.GetActiveLocaleList(), attributeId > 0 ? _globalAttributeClient.GetAttributeLocale(attributeId) : new GlobalAttributeLocaleListModel());

        //Get global attribute default value by attribute Id.
        public virtual List<DefaultValueListModel> GetDefaultValues(int attributeId)
          => GlobalAttributeViewModelMap.ToDefaultValues(DefaultSettingHelper.GetActiveLocaleList(), attributeId > 0 ? _globalAttributeClient.GetDefaultValues(attributeId) : new GlobalAttributeDefaultValueListModel());

        // Check attribute Code already exist or not.
        public virtual bool IsAttributeCodeExist(string attributeCode)
           => !string.IsNullOrEmpty(attributeCode) ? _globalAttributeClient.IsAttributeCodeExist(attributeCode) : true;

        //Save global attribute default values.
        public virtual int SaveDefaultValues(string model, int attributeId, string defaultValueCode, bool isDefault, bool isSwatch, string swatchText, int displayOrder = 0, int defaultValueId = 0)
         => _globalAttributeClient.SaveDefaultValues(GlobalAttributeViewModelMap.ToAttributeDefaultValuesModel(model, attributeId, defaultValueCode, isDefault, isSwatch, swatchText, displayOrder, defaultValueId)).GlobalAttributeDefaultValueId;

        //Delete global attribute default value.
        public virtual bool DeleteDefaultValues(int defaultvalueId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;

            try
            {
                return _globalAttributeClient.DeleteDefaultValues(defaultvalueId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.RestrictSystemDefineDeletion:
                        errorMessage = Admin_Resources.ErrorDeleteAttributeDefaultValues;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Check whether the attribute default value code already exists.
        public virtual bool CheckAttributeDefaultValueCodeExist(int attributeId, string defaultValueCode, int defaultValueId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Get the attribute default value list.
            GlobalAttributeDefaultValueListModel attributedefaultValueList = attributeId > 0 ? _globalAttributeClient.GetDefaultValues(attributeId) : null;
            if (IsNotNull(defaultValueCode) && IsNotNull(attributedefaultValueList.DefaultValues))
            {
                if (defaultValueId > 0)
                {
                    //Set the status in case the default value code is open in edit mode.
                    GlobalAttributeDefaultValueModel globalAttributeDefaultValueModel = attributedefaultValueList.DefaultValues.Find(x => x.GlobalAttributeDefaultValueId == defaultValueId);
                    if (IsNotNull(globalAttributeDefaultValueModel))
                        return !Equals(globalAttributeDefaultValueModel.AttributeDefaultValueCode, defaultValueCode);
                }
                return attributedefaultValueList.DefaultValues.Any(x => x.AttributeDefaultValueCode.Equals(defaultValueCode));
            }
            return false;
        }

        //Get Attribute Types
        public virtual List<AttributeTypeModel> GetAttributeTypes() => PIMAttributeViewModelMap.ToAttributeTypeListModel(_attributesClient.GetAttributeTypes(true).Types);

        //Create tab structure.
        public virtual TabViewListModel GetTabsData()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            Dictionary<string, string> tabs = new Dictionary<string, string>();

            tabs.Add(Attributes_Resources.TitleAttributeDetails, $"/GlobalAttribute/Attribute");
            tabs.Add(Attributes_Resources.TitleLocaleValues, $"/GlobalAttribute/Locale");
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return SetTabData(tabs);
        }

        //Save attribute locale.
        public virtual void SaveLocale(BindDataModel bindDataModel)
         => _globalAttributeClient.SaveLocales(GlobalAttributeViewModelMap.ToLocaleListModel(bindDataModel));

        //Get attribute data.
        public virtual GlobalAttributeDataModel GetAttributeDataModel(BindDataModel bindDataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            GlobalAttributeDataModel globalAttributeDataModel = new GlobalAttributeDataModel();
            globalAttributeDataModel.AttributeModel = GlobalAttributeViewModelMap.ToGlobalAttributeModel(bindDataModel);
            globalAttributeDataModel.AttributeModel.EntityName = BindEntityName(globalAttributeDataModel.AttributeModel.GlobalEntityId);
            globalAttributeDataModel.ValidationRule = GetAttributeValidationList(bindDataModel, Convert.ToInt32(globalAttributeDataModel.AttributeModel.AttributeTypeId));
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return globalAttributeDataModel;
        }

        //Get attribute validation list from BindDataModel.
        public virtual List<GlobalAttributeValidationModel> GetAttributeValidationList(BindDataModel model, int attributeTypeId) => GlobalAttributeViewModelMap.ToAttributeValidationModel(model, AttributeInputValidations(attributeTypeId));

        //Get Attribute Validation List
        public virtual List<AttributeInputValidationModel> GetAttributeInputValidations(int typeId, int attributeId = 0) => GlobalAttributeViewModelMap.ToInputValidationListModel(_globalAttributeClient.GetInputValidations(typeId, attributeId).InputValidations);

        //Set tool menu for global attribute list grid view.
        public virtual void SetGlobalAttributeListToolMenu(GlobalAttributeListViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeleteGlobalAttributePopup')", ControllerName = "GlobalAttribute", ActionName = "Delete" });
            }
        }

        //For setting error messages
        public virtual GlobalAttributeViewModel UpdateErrorMessage(BindDataModel model, string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            GlobalAttributeViewModel errorModel = new GlobalAttributeViewModel();
            errorModel.GlobalAttributeId = Convert.ToInt32(model.GetValue("AttributeId"));
            errorModel.HasError = true;
            errorModel.ErrorMessage = message;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return errorModel;
        }

        //Check value of attribute is already exists or not.
        public virtual string IsAttributeValueUnique(GlobalAttributeValueParameterModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            string errorMessage = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(model.AttributeCodeValues))
                {
                    ZnodeLogging.LogMessage("GlobalAttributeValueParameterModel having AttributeCodeValues", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, model.AttributeCodeValues);
                    string[] arrayAttributeCodeValues = model.AttributeCodeValues.Split('~');
                    ParameterModel parameterModel = _globalAttributeClient.IsAttributeValueUnique(GetGlobalAttributeValueParameterDetails(arrayAttributeCodeValues, model.Id, model.EntityType));
                    if (!string.IsNullOrEmpty(parameterModel?.Ids))
                        errorMessage = string.Format(Admin_Resources.ErrorAlreadyExistsAttributeCode, parameterModel.Ids);
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return errorMessage;
            }
            catch(ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.IdLessThanOne:
                        errorMessage = Admin_Resources.InvalidEntityInformation;
                        break;
                    case ErrorCodes.NullModel:
                        errorMessage = ZnodeConstant.NullModelError;
                        break;
                    default:
                        errorMessage = Admin_Resources.TextInvalidData;
                        break;
                }
                return errorMessage;
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.TextInvalidData;
                return errorMessage;
            }
        }

        //Get global attributevalue parameter details.
        public GlobalAttributeValueParameterModel GetGlobalAttributeValueParameterDetails(string[] attributeCodeValues, int id, string entityType)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<GlobalAttributeCodeValueModel> globalAttributeCodeValueList = new List<GlobalAttributeCodeValueModel>();
            foreach (var attribute in attributeCodeValues)
                globalAttributeCodeValueList.Add(new GlobalAttributeCodeValueModel { AttributeCode = attribute.Split('#')[0], AttributeValues = attribute.Split('#')[1].Trim() });

            ZnodeLogging.LogMessage("globalAttributeCodeValueList list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, globalAttributeCodeValueList?.Count());
            return new GlobalAttributeValueParameterModel()
            {
                Id = id,
                LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale),
                EntityType = entityType,
                GlobalAttributeCodeValueList = globalAttributeCodeValueList
            };
        }

        //Bind Filter Values for Lis
        private void BindEntityFilterValues(GlobalAttributeListViewModel attributeViewModel, int entityId, string entityType)
        {
            attributeViewModel.GlobalEntity = string.IsNullOrEmpty(entityType) ? Admin_Resources.LabelAllEntities : entityType;
            attributeViewModel.GlobalEntityId = entityId;
        }

        //Bind EntityId
        private void BindEntityIdWithFilters(FilterCollectionDataModel model, int entityId)
        {
            model.Filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeGlobalEntityEnum.GlobalEntityId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            if (entityId > 0)
                model.Filters.Add(ZnodeGlobalEntityEnum.GlobalEntityId.ToString(), FilterOperators.Equals, entityId.ToString());
        }

        //Bind Entity name
        private string BindEntityName(int globalEntityId)
        {
            List<SelectListItem> entityTypes = GetGlobalEntityTypes();
            return entityTypes.FirstOrDefault(x => x.Value == globalEntityId.ToString()).Text;
        }
        #endregion
    }
}