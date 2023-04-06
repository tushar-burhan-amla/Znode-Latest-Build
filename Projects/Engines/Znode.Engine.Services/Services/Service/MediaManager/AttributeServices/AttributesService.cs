using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Services
{
    public class AttributesService : BaseService, IAttributesService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeMediaAttribute> _attributesRepository;
        private readonly IZnodeRepository<ZnodeMediaAttributeValidation> _attributeValidationRepository;
        private readonly IZnodeRepository<ZnodeMediaAttributeDefaultValue> _defaultAttributeValueRepository;
        private readonly IZnodeRepository<ZnodeMediaAttributeDefaultValueLocale> _defaultAttributeLocaleValueRepository;
        private readonly IZnodeRepository<ZnodeMediaAttributeValue> _mediaAttributeValueRepository;
        private readonly IZnodeRepository<ZnodeAttributeType> _attributeTypeRepository;
        private readonly IZnodeRepository<ZnodeMediaAttributeGroup> _attributeGroupRepository;
        private readonly IZnodeRepository<ZnodeMediaAttributeGroupMapper> _attributeGroupMapperRepository;
        #endregion

        //Constructor
        public AttributesService()
        {
            _attributesRepository = new ZnodeRepository<ZnodeMediaAttribute>();
            _attributeValidationRepository = new ZnodeRepository<ZnodeMediaAttributeValidation>();
            _defaultAttributeLocaleValueRepository = new ZnodeRepository<ZnodeMediaAttributeDefaultValueLocale>();
            _defaultAttributeValueRepository = new ZnodeRepository<ZnodeMediaAttributeDefaultValue>();
            _mediaAttributeValueRepository = new ZnodeRepository<ZnodeMediaAttributeValue>();
            _attributeTypeRepository = new ZnodeRepository<ZnodeAttributeType>();
            _attributeGroupMapperRepository = new ZnodeRepository<ZnodeMediaAttributeGroupMapper>();
            _attributeGroupRepository = new ZnodeRepository<ZnodeMediaAttributeGroup>();
        }

        #region Attribute
        //Get paged attributes list
        public virtual AttributesListDataModel GetAttributeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            SetLocaleFilterIfNotPresent(ref filters);

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate AttributesDataModel list ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<AttributesDataModel> objStoredProc = new ZnodeViewRepository<AttributesDataModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<AttributesDataModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetMediaAttributes  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("AttributesDataModel list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());
            AttributesListDataModel attributesListDataModel = new AttributesListDataModel();
            attributesListDataModel.Attributes = list?.Count > 0 ? list?.ToList() : null;

            attributesListDataModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return attributesListDataModel;
        }

        //Create new attribute
        public virtual AttributesDataModel CreateAttribute(AttributesDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Input Parameter AttributesDataModel having AttributeCode,ValidationRule and AttributeTypeId", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { model?.AttributeCode, model?.ValidationRule, model?.AttributeTypeId });
            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.NullModel, PIM_Resources.ErrorNullAttributeModel);

            if (IsAttributeCodeExist(model.AttributeCode))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorAttributeCodeAlreadyExists);

            if (!Equals(model, null))
            {
                ZnodeMediaAttribute attributeModel = _attributesRepository.Insert(model.ToEntity<ZnodeMediaAttribute>());
                ZnodeLogging.LogMessage("Inserted Attribute with code:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, attributeModel?.AttributeCode);
                if (attributeModel.MediaAttributeId > 0)
                {
                    if (model.AttributeGroupId > 0)
                    {
                        model.MediaAttributeId = attributeModel.MediaAttributeId;

                        //Insert into MediaAttributeGroupMapper for attribute and group mapping.
                        ZnodeLogging.LogMessage(!Equals(_attributeGroupMapperRepository.Insert(new ZnodeMediaAttributeGroupMapper() { MediaAttributeId = model.MediaAttributeId, MediaAttributeGroupId = model.AttributeGroupId }), null) ?
                            string.Format(Admin_Resources.SuccessSaveMediaAttributeInGroupMapper,model.AttributeCode) : string.Format(Admin_Resources.ErrorSaveMediaAttributeInGroupMapper, model.AttributeCode), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                    }

                    string _attributeType = _attributeTypeRepository.GetById(model.AttributeTypeId.GetValueOrDefault())?.AttributeTypeName;
                    ZnodeLogging.LogMessage("AttributeType:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, _attributeType);
                    if (!Equals(model.ValidationRule))
                    {
                        model.ValidationRule.ForEach(item =>
                        {
                            //Convert date value to sql format for date type attribute.
                            item.Name = _attributeType.Equals(ZnodeConstant.DateType) ? ConvertStringToSqlDateFormat(item.Name) : item.Name;
                            item.MediaAttributeId = attributeModel.MediaAttributeId;
                            ZnodeMediaAttributeValidation validation = _attributeValidationRepository.Insert(item.ToEntity<ZnodeMediaAttributeValidation>());
                        });
                    }
                    return attributeModel.ToModel<AttributesDataModel>();
                }
                return model;
            }
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return null;
        }

        //Update  attribute
        public virtual bool UpdateAttribute(AttributesDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter MediaAttributeId,AttributeGroupId and AttributeCode", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { model?.MediaAttributeId.ToString(), model?.AttributeGroupId, model?.AttributeCode });
            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.NullModel, PIM_Resources.ErrorNullAttributeModel);

            bool status = false;

            if (model?.MediaAttributeId > 0)
            {
                status = _attributesRepository.Update(model.ToEntity<ZnodeMediaAttribute>());

                ZnodeLogging.LogMessage(status ? string.Format(Admin_Resources.SuccessUpdateMediaAttribute,model.AttributeCode) : string.Format(Admin_Resources.ErrorUpdateMediaAttribute, model.AttributeCode), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

                if (status && !model.IsSystemDefined.GetValueOrDefault())
                    UpdateIntoGroupMapper(model);

                if (model?.ValidationRule.Count > 0)
                {
                    string _attributeType = _attributeTypeRepository.GetById(model.AttributeTypeId.GetValueOrDefault())?.AttributeTypeName;
                    status = UpdateValidations(model.ValidationRule, model.MediaAttributeId, _attributeType);
                    ZnodeLogging.LogMessage(status ? PIM_Resources.SuccessUpdateValidations : PIM_Resources.ErrorUpdateValidations, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                }
            }
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return status;
        }

        //Get attribute by attribute id.
        public virtual AttributesDataModel GetAttribute(int attributeId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Input Parameter attributeId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { attributeId });
            ZnodeMediaAttributeGroupMapper mediaAttributeGroupMapper = _attributeGroupMapperRepository.Table.Where(x => x.MediaAttributeId == attributeId).ToList()?.FirstOrDefault();
            AttributesDataModel attribute = _attributesRepository.GetById(attributeId).ToModel<AttributesDataModel>();
            attribute.AttributeGroupId = Convert.ToInt32(mediaAttributeGroupMapper?.MediaAttributeGroupId);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return attribute;
        }

        //Get attributetype list
        public virtual AttributesListDataModel GetAttributeTypeList()
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            //Filter to get attribute types for Media attribute.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeAttributeTypeEnum.IsMediaAttributeType.ToString(), ProcedureFilterOperators.Equals, "true"));
            IList<ZnodeAttributeType> attributeType = _attributeTypeRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage("attributeType list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, attributeType?.Count());
            if (attributeType?.Count > 0)
                return new AttributesListDataModel() { AttributeTypes = attributeType.ToModel<AttributeTypeDataModel>().ToList() };
            else
                return null;
        }

        //Get attribute input validation list
        public virtual AttributesInputValidationListModel GetAttributesInputValidations(int attributeTypeId, int attributeId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter attributeTypeId and attributeId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { attributeTypeId, attributeId });
            IZnodeRepository<ZnodeAttributeInputValidation> _attributeInputValidationRepository = new ZnodeRepository<ZnodeAttributeInputValidation>();
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeAttributeInputValidationEnum.AttributeTypeId.ToString(), ProcedureFilterOperators.Equals, attributeTypeId.ToString()));

            _attributeInputValidationRepository.EnableDisableLazyLoading = true;

            AttributesInputValidationListModel _model = AttributesMap.ToInputValidationListModel(_attributeInputValidationRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause));
            if (attributeId != 0)
                GetValidationValue(_model, attributeId);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return _model;
        }

        //Create Attribute Locales
        public virtual AttributesLocaleListModel SaveLocales(AttributesLocaleListModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model.Locales?.Count > 0)
            {
                IZnodeRepository<ZnodeMediaAttributeLocale> _attributeLocaleRepository = new ZnodeRepository<ZnodeMediaAttributeLocale>();

                _attributeLocaleRepository.Delete(GetWhereClauseForAttributeId(model.Locales.FirstOrDefault().MediaAttributeId).WhereClause);
                ZnodeLogging.LogMessage("Locales with AttributeCode: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, model?.AttributeCode);
                int defaultLocaleId = GetDefaultLocaleId();
                ZnodeLogging.LogMessage("defaultLocaleId: ", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { defaultLocaleId });
                //If locale name is present then attribute code will be save as default locale name.
                if (model.Locales.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.AttributeName.Trim())))
                    model.Locales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.AttributeName = model.AttributeCode);

                model.Locales.RemoveAll(x => x.AttributeName == string.Empty);

                //Insert LocaleList Into DataBase
                _attributeLocaleRepository.Insert(model.Locales.ToEntity<ZnodeMediaAttributeLocale>().ToList());
            }
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }

        //Create Attribute default Value
        public virtual AttributesDefaultValueModel SaveDefaultValues(AttributesDefaultValueModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameters DefaultAttributeValueId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, model.DefaultAttributeValueId);
            if (model.DefaultAttributeValueId == 0)
                InsertDefaultValues(model);
            else
                InsertUpdateDefaultValueLocale(model, model.DefaultAttributeValueId);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }

        //Delete Attribute Default Values By AttributeDefaultValueId
        public virtual bool DeleteDefaultValues(int defaultvalueId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameters defaultvalueId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info,defaultvalueId);
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeMediaAttributeDefaultValueEnum.MediaAttributeDefaultValueId.ToString(), defaultvalueId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteMediaDefaultAttributeValues @MediaAttributeDefaultValueId,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, deleteResult?.Count());
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessDeleteAttributeDefaultValues, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {              
                throw new ZnodeException(ErrorCodes.RestrictSystemDefineDeletion, PIM_Resources.ErrorDeleteDefaultAttributeValues);
            }
        }

        //Delete  existing Attribute By Attribute Id
        public virtual bool DeleteAttribute(ParameterModel mediaAttributeIds)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

            objStoredProc.SetParameter(ZnodeMediaAttributeEnum.MediaAttributeId.ToString(), mediaAttributeIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteMediaAttribute @MediaAttributeId,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, deleteResult?.Count());
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessDeleteMediaAttribute, mediaAttributeIds.Ids), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorDeleteAttribute);
            }
        }

        //Get Attribute Default Values
        public virtual AttributesDefaultValueListModel GetDefaultValues(int attributeId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameters attributeId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, attributeId);
            _defaultAttributeValueRepository.EnableDisableLazyLoading = true;
            return AttributesMap.ToDefaultValueListModel(_defaultAttributeValueRepository.GetEntityList(GetWhereClauseForAttributeId(attributeId).WhereClause));
        }

        //Get attribute local by attribute id.
        public virtual AttributeLocalDataListModel GetAttributeLocalByAttributeId(int atributeId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameters atributeId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info, atributeId);
            IZnodeRepository<ZnodeMediaAttributeLocale> _attributeLocaleRepository = new ZnodeRepository<ZnodeMediaAttributeLocale>();

            FilterTuple filter = new FilterTuple(ZnodeMediaAttributeEnum.MediaAttributeId.ToString(), ProcedureFilterOperators.Equals, atributeId.ToString());
            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(filter);
            IList<ZnodeMediaAttributeLocale> entity = _attributeLocaleRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage("ZnodeMediaAttributeLocale list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, entity?.Count());
            if (entity?.Count > 0)
                return new AttributeLocalDataListModel() { AttributeLocalList = entity.ToModel<AttributeLocalDataModel>().ToList() };
            return
                null;
        }

        //Check attribute Code already exist or not
        public virtual bool IsAttributeCodeExist(string attributeCode)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("attributeCode:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, attributeCode);
            if (string.IsNullOrEmpty(attributeCode))
                throw new ZnodeException(ErrorCodes.NullModel, PIM_Resources.ErrorNullAttributeCodeModel);

            return _attributesRepository.Table.Any(x => x.AttributeCode.ToLower() == attributeCode.ToLower());
        }

        #endregion

        #region Private Methods

        //Update media attribute group mapper
        private void UpdateIntoGroupMapper(AttributesDataModel model)
        {
            ZnodeLogging.LogMessage("Input Parameter MediaAttributeId,AttributeGroupId and AttributeCode", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { model?.MediaAttributeId.ToString(), model?.AttributeGroupId, model?.AttributeCode });
            FilterTuple filter = new FilterTuple(ZnodeMediaAttributeGroupMapperEnum.MediaAttributeId.ToString(), ProcedureFilterOperators.Equals, model.MediaAttributeId.ToString());
            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(filter);
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection()).WhereClause;

            if (IsNull(model?.AttributeGroupId))
                _attributeGroupMapperRepository.Delete(whereClause);
            else
            {
                ZnodeMediaAttributeGroupMapper mediaAttributeGroupMapper = _attributeGroupMapperRepository.GetEntity(whereClause);
                if (Equals(mediaAttributeGroupMapper, null))
                    ZnodeLogging.LogMessage(!Equals(_attributeGroupMapperRepository.Insert(new ZnodeMediaAttributeGroupMapper() { MediaAttributeId = model.MediaAttributeId, MediaAttributeGroupId = model.AttributeGroupId, IsSystemDefined = model.IsSystemDefined.GetValueOrDefault() }), null) ? " Attribute is inserted successfully in GroupMapper." : "MediaAttribute not save in GroupMapper", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                else
                    ZnodeLogging.LogMessage(!Equals(_attributeGroupMapperRepository.Update(new ZnodeMediaAttributeGroupMapper() { MediaAttributeId = model.MediaAttributeId, MediaAttributeGroupId = model.AttributeGroupId, MediaAttributeGroupMapperId = mediaAttributeGroupMapper.MediaAttributeGroupMapperId, IsSystemDefined = mediaAttributeGroupMapper.IsSystemDefined }), null)
                        ? string.Format(Admin_Resources.SuccessUpdateMediaAttributeInGroupMapper,model.AttributeCode) : string.Format(Admin_Resources.ErrorUpdateMediaAttributeInGroupMapper, model.AttributeCode), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            }
        }

        //Update existing input validation values
        private bool UpdateValidations(List<AttributesValidationModel> model, int attributeId, string attributeType)
        {
            ZnodeLogging.LogMessage("Input Parameter attributeId and attributeType", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { attributeId, attributeType });
            if (model?.Count > 0)
            {
                ZnodeLogging.LogMessage("AttributesValidationModel list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, model?.Count());
                FilterCollection filter = new FilterCollection();
                filter.Add(new FilterTuple(ZnodeMediaAttributeEnum.MediaAttributeId.ToString(), ProcedureFilterOperators.Equals, attributeId.ToString()));

                //Delete Exiting Validation Values for attribute
                ZnodeLogging.LogMessage(_attributeValidationRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause)
                    ? Admin_Resources.SuccessDeleteAttributeValidation : Admin_Resources.ErrorDeleteAttributeValidation, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

                //Convert date value to sql format for date type attribute.
                model.ForEach(item =>
                {
                    item.Name = attributeType.Equals(ZnodeConstant.DateType) ? ConvertStringToSqlDateFormat(item.Name) : item.Name;
                    item.MediaAttributeId = attributeId;
                });

                _attributeValidationRepository.Insert(model.ToEntity<ZnodeMediaAttributeValidation>().ToList());
                return true;
            }
            else
                return false;
        }

        //Get list of validation rule
        private void GetValidationValue(AttributesInputValidationListModel model, int attributeId)
        {
            ZnodeLogging.LogMessage("Input Parameter attributeId", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { attributeId });
            _attributeValidationRepository.EnableDisableLazyLoading = false;
            foreach (AttributeInputValidationDataModel item in model.InputValidations)
            {
                FilterCollection filter = new FilterCollection();
                filter.Add(new FilterTuple(ZnodeAttributeInputValidationEnum.InputValidationId.ToString(), ProcedureFilterOperators.Equals, item.AttributeValidationId.ToString()));
                filter.Add(new FilterTuple(ZnodeMediaAttributeEnum.MediaAttributeId.ToString(), ProcedureFilterOperators.Equals, attributeId.ToString()));

                //Get Multiple Default values if validation rules is more than 2
                if (item.Rules.Count > 2)
                {
                    List<ZnodeMediaAttributeValidation> entity = _attributeValidationRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause).ToList();
                    item.DefaultValue = GetValueRuleIds(entity);
                }
                else
                {
                    ZnodeMediaAttributeValidation values = _attributeValidationRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
                    item.DefaultValue = Equals(values?.InputValidationRuleId, null) ? Convert.ToString(values?.Name) : Convert.ToString(values.InputValidationRuleId);
                }
            }
        }

        //Save or update attribute default values
        private void InsertUpdateDefaultValueLocale(AttributesDefaultValueModel model, int defaultAttributeValueId)
        {
            ZnodeLogging.LogMessage("Input Parameter defaultAttributeValueId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { defaultAttributeValueId });
            ZnodeMediaAttributeDefaultValue defaultValueEntity = new ZnodeMediaAttributeDefaultValue();

            defaultValueEntity.MediaAttributeId = model.AttributeId;
            defaultValueEntity.MediaAttributeDefaultValueId = defaultAttributeValueId;
            defaultValueEntity.AttributeDefaultValueCode = model.AttributeDefaultValueCode;
            _defaultAttributeValueRepository.Update(defaultValueEntity);

            foreach (DefaultAttributeValueLocaleModel item in model.ValueLocales)
            {
                item.MediaAttributeDefaultValueId = defaultAttributeValueId;
                FilterCollection filterCol = new FilterCollection();
                filterCol.Add(new FilterTuple(ZnodeMediaAttributeLocaleEnum.LocaleId.ToString(), ProcedureFilterOperators.Equals, item.LocaleId.ToString()));
                filterCol.Add(new FilterTuple(ZnodeMediaAttributeDefaultValueLocaleEnum.MediaAttributeDefaultValueId.ToString(), ProcedureFilterOperators.Equals, item.MediaAttributeDefaultValueId.ToString()));

                ZnodeMediaAttributeDefaultValueLocale _localeEntity = _defaultAttributeLocaleValueRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterCol.ToFilterDataCollection()).WhereClause);
                item.MediaAttributeDefaultValueId = defaultAttributeValueId;

                ZnodeMediaAttributeDefaultValueLocale defaultValueLocaleEntity = item.ToEntity<ZnodeMediaAttributeDefaultValueLocale>();

                if (!Equals(_localeEntity, null))
                {
                    defaultValueLocaleEntity.MediaAttributeDefaultValueLocaleId = _localeEntity.MediaAttributeDefaultValueLocaleId;
                    _defaultAttributeLocaleValueRepository.Update(defaultValueLocaleEntity);
                }
                else
                    _defaultAttributeLocaleValueRepository.Insert(defaultValueLocaleEntity);
            }
        }

        //Get multiple validation ruleId
        private string GetValueRuleIds(List<ZnodeMediaAttributeValidation> list)
        {
            string ruleIds = string.Empty;
            ZnodeLogging.LogMessage("ZnodeMediaAttributeValidation list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());
            list.ForEach(item =>
            {
                ruleIds += item.InputValidationRuleId.ToString() + ",";
            });
            return !string.IsNullOrEmpty(ruleIds) ? ruleIds.Remove(ruleIds.Length - 1) : string.Empty;
        }

        //save attribute default values
        private void InsertDefaultValues(AttributesDefaultValueModel model)
        {
            ZnodeMediaAttributeDefaultValue defaultValueEntity = new ZnodeMediaAttributeDefaultValue();
            defaultValueEntity.MediaAttributeId = model.AttributeId;
            defaultValueEntity.AttributeDefaultValueCode = model.AttributeDefaultValueCode;
            ZnodeMediaAttributeDefaultValue _defaultValue = _defaultAttributeValueRepository.Insert(defaultValueEntity);

            if (!Equals(_defaultValue, null))
            {
                model.ValueLocales.ForEach(x =>
                {
                    x.MediaAttributeDefaultValueId = _defaultValue.MediaAttributeDefaultValueId;
                    _defaultAttributeLocaleValueRepository.Insert(x.ToEntity<ZnodeMediaAttributeDefaultValueLocale>());
                });
                model.DefaultAttributeValueId = _defaultValue.MediaAttributeDefaultValueId;
            }
        }

        //Get where clause by attribute id
        private EntityWhereClauseModel GetWhereClauseForAttributeId(int? AttributeId)
        {
            ZnodeLogging.LogMessage("Input Parameter AttributeId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { AttributeId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeMediaAttributeEnum.MediaAttributeId.ToString(), ProcedureFilterOperators.Equals, AttributeId.ToString()));
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
        }

        #endregion
    }

}



