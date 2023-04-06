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
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Resources;
namespace Znode.Engine.Services
{
    public class PIMAttributeService : BaseService, IPIMAttributeService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeAttributeType> _attributeTypeRepository;
        private readonly IZnodeRepository<ZnodeAttributeInputValidation> _inputValidationRepository;
        private readonly IZnodeRepository<ZnodePimFrontendProperty> _frontEndProperties;
        private readonly IZnodeRepository<ZnodePimAttribute> _pimAttributeRepository;
        private readonly IZnodeRepository<ZnodePimAttributeValidation> _validationRepository;
        private readonly IZnodeRepository<ZnodePimAttributeLocale> _LocaleRepository;
        private readonly IZnodeRepository<ZnodePimAttributeDefaultValue> _defaultValueRepository;
        private readonly IZnodeRepository<ZnodePimAttributeDefaultValueLocale> _defaultValueLocaleRepository;
        private readonly IZnodeRepository<ZnodePimAttributeGroupMapper> _pimAttributeGroupMapperRepository;
        private readonly IZnodeRepository<ZnodePimFamilyGroupMapper> _pimFamilyGroupMapperRepository;
        private readonly IZnodeRepository<ZnodePimAttributeValue> _pimAttributeValueRepository;
        #endregion

        #region Public Constructor
        public PIMAttributeService()
        {
            //Initialization of PIM Repositories.
            _attributeTypeRepository = new ZnodeRepository<ZnodeAttributeType>();
            _inputValidationRepository = new ZnodeRepository<ZnodeAttributeInputValidation>();
            _frontEndProperties = new ZnodeRepository<ZnodePimFrontendProperty>();
            _pimAttributeRepository = new ZnodeRepository<ZnodePimAttribute>();
            _validationRepository = new ZnodeRepository<ZnodePimAttributeValidation>();
            _LocaleRepository = new ZnodeRepository<ZnodePimAttributeLocale>();
            _defaultValueRepository = new ZnodeRepository<ZnodePimAttributeDefaultValue>();
            _defaultValueLocaleRepository = new ZnodeRepository<ZnodePimAttributeDefaultValueLocale>();
            _pimAttributeGroupMapperRepository = new ZnodeRepository<ZnodePimAttributeGroupMapper>();
            _pimFamilyGroupMapperRepository = new ZnodeRepository<ZnodePimFamilyGroupMapper>();
            _pimAttributeValueRepository = new ZnodeRepository<ZnodePimAttributeValue>();
        }
        #endregion

        #region Public Methods
        //Gets the list of Attribute.
        public virtual PIMAttributeListModel GetAttributeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //set locale Filters if not present
            SetLocaleFilterIfNotPresent(ref filters);
            ResetFilters(filters);

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get attribute list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<PIMAttributeModel> objStoredProc = new ZnodeViewRepository<PIMAttributeModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<PIMAttributeModel> pimAttributeList = objStoredProc.ExecuteStoredProcedureList("Znode_GetPimAttributes @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("pimAttributeList count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeList?.Count);
            PIMAttributeListModel pimAttributeListModel = new PIMAttributeListModel();
            pimAttributeListModel.Attributes = pimAttributeList?.Count > 0 ? pimAttributeList?.ToList() : null;

            pimAttributeListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return pimAttributeListModel;
        }

        //Gets Attribute by ID.
        public virtual PIMAttributeModel GetAttributeById(int attributeId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter attributeId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeId);

            if (attributeId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorIDLessThanOne);

            PIMAttributeModel pimAttribute = PIMAttributesMap.ToAttributeModel(_pimAttributeRepository.GetById(attributeId));
            if (IsNotNull(pimAttribute))
            {
                pimAttribute.AttributeGroupId = Convert.ToInt32(_pimAttributeGroupMapperRepository.Table.Where(x => x.PimAttributeId == attributeId).ToList()?.FirstOrDefault()?.PimAttributeGroupId);
  
                pimAttribute.AttributeTypeName = IsNotNull(pimAttribute) ? _attributeTypeRepository.GetById(pimAttribute.AttributeTypeId.GetValueOrDefault())?.AttributeTypeName : null;

                FilterCollection filtersForAttributes = new FilterCollection();
                filtersForAttributes.Add(ZnodePimAttributeDefaultValueEnum.PimAttributeId.ToString(), FilterOperators.Equals, attributeId.ToString());

                int? DefaultValueId = _defaultValueRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersForAttributes.ToFilterDataCollection()).WhereClause)?.PimAttributeDefaultValueId;
                ZnodeLogging.LogMessage("DefaultValueId generated: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, DefaultValueId);

                if (IsNotNull(DefaultValueId))
                {
                    filtersForAttributes = new FilterCollection();
                    filtersForAttributes.Add(ZnodePimAttributeDefaultValueLocaleEnum.PimAttributeDefaultValueId.ToString(), FilterOperators.Equals, DefaultValueId.ToString());
                    filtersForAttributes.Add(ZnodePimAttributeDefaultValueLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetDefaultLocaleId().ToString());

                    ZnodePimAttributeDefaultValueLocale pimAttributeDefaultValueLocale = _defaultValueLocaleRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersForAttributes.ToFilterDataCollection()).WhereClause);
                    pimAttribute.AttributeDefaultValueId = DefaultValueId;
                    if (IsNotNull(pimAttributeDefaultValueLocale))
                        pimAttribute.AttributeDefaultValue = pimAttributeDefaultValueLocale.AttributeDefaultValue;
                }
                pimAttribute.UsedInProductsCount = _pimAttributeValueRepository.Table.Where(x => x.PimAttributeId == attributeId).Count();
            }
            ZnodeLogging.LogMessage("pimAttribute to be returned: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttribute);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return pimAttribute;
        }

        //Update attribute details with validation rule, front end properties, group and family mapping          
        public virtual bool UpdateAttribute(PIMAttributeDataModel pimAttributemodel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(pimAttributemodel))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorNullAttributeModel);
            ZnodeLogging.LogMessage("PimAttributeId and IsCategory properties of pimAttributemodel.AttributeModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pimAttributemodel?.AttributeModel?.PimAttributeId, pimAttributemodel?.AttributeModel?.IsCategory });

            bool statusForAttribute = false;
            if (pimAttributemodel?.AttributeModel?.PimAttributeId > 0)
            {
                statusForAttribute = _pimAttributeRepository.Update(pimAttributemodel.AttributeModel.ToEntity<ZnodePimAttribute>());
                ZnodeLogging.LogMessage(statusForAttribute ? PIM_Resources.SuccessUpdateAttribute : PIM_Resources.ErrorUpdateAttribute, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                //Update product frontproperties if IsCategory is false.
                if (!pimAttributemodel.AttributeModel.IsCategory)
                {
                    statusForAttribute = UpdateFrontProperties(pimAttributemodel.FrontProperties, pimAttributemodel.AttributeModel.PimAttributeId);
                    ZnodeLogging.LogMessage(statusForAttribute ? PIM_Resources.SuccessUpdateFrontProperties : PIM_Resources.ErrorUpdateFrontProperties, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                }

                string _attributeType = _attributeTypeRepository.GetById(pimAttributemodel.AttributeModel.AttributeTypeId.GetValueOrDefault())?.AttributeTypeName;
                ZnodeLogging.LogMessage("_attributeType: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, _attributeType);

                //Update validation rules.
                if (pimAttributemodel?.ValidationRule?.Count > 0)
                {
                    statusForAttribute = UpdateValidations(pimAttributemodel.ValidationRule, pimAttributemodel.AttributeModel.PimAttributeId, _attributeType);
                    ZnodeLogging.LogMessage(statusForAttribute ? PIM_Resources.SuccessUpdateValidations : PIM_Resources.ErrorUpdateValidations, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                }
                
                //Save defaultvalue. 
                if (IsNotNull(_attributeType) && (_attributeType.Equals(ZnodeConstant.NumberType) || _attributeType.Equals(ZnodeConstant.YesNoType) || _attributeType.Equals(ZnodeConstant.DateType)))
                    SaveDefaultValues(PIMAttributesMap.ToPIMAttributeDefaultValueModel(pimAttributemodel, GetDefaultLocaleId(), _attributeType));

                //Logic to assign attribute to family with its group
                if (statusForAttribute && !pimAttributemodel.AttributeModel.IsSystemDefined)
                    InsertAndUpdateGroupMapperandFamilyGroupMapper(pimAttributemodel);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return statusForAttribute;
        }

        //Create attribute details with validation rule, front end properties, group and family mapping.
        public virtual PIMAttributeDataModel CreateAttribute(PIMAttributeDataModel pimAttributemodel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(pimAttributemodel))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorNullAttributeModel);

            if (IsAttributeCodeExist(pimAttributemodel.AttributeModel.AttributeCode, pimAttributemodel.AttributeModel.IsCategory))
                throw new ZnodeException(ErrorCodes.AlreadyExist, PIM_Resources.ErrorAttributeExists);

            ZnodeLogging.LogMessage("AttributeCode and IsCategory properties of pimAttributemodel.AttributeModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pimAttributemodel?.AttributeModel?.AttributeCode, pimAttributemodel?.AttributeModel?.IsCategory });

            //insert attribute details.
            ZnodePimAttribute attributeModel = _pimAttributeRepository.Insert(pimAttributemodel.AttributeModel.ToEntity<ZnodePimAttribute>());
            ZnodeLogging.LogMessage(IsNotNull(attributeModel) ? PIM_Resources.SuccessInsertAttribute : PIM_Resources.ErrorInsertAttribute, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNotNull(attributeModel))
            {
                if (attributeModel.PimAttributeId > 0)
                {
                    pimAttributemodel.AttributeModel.PimAttributeId = attributeModel.PimAttributeId;

                    //Insert into MediaAttributeGroupMapper for attribute and group mapping.
                    if (pimAttributemodel.AttributeModel.AttributeGroupId > 0)
                        ZnodeLogging.LogMessage(IsNotNull(_pimAttributeGroupMapperRepository.Insert(new ZnodePimAttributeGroupMapper() { PimAttributeId = pimAttributemodel.AttributeModel.PimAttributeId, PimAttributeGroupId = pimAttributemodel.AttributeModel.AttributeGroupId })) ?
                            PIM_Resources.SuccessAttributeSaveInGroupMapper : PIM_Resources.ErrorAttributeSaveInGroupMapper, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                    //Insert front properties. 
                    pimAttributemodel.FrontProperties.PimAttributeId = attributeModel.PimAttributeId;
                    ZnodePimFrontendProperty frontPropertiesInsert = _frontEndProperties.Insert(PIMAttributesMap.ToFrontPropertiesEntity(pimAttributemodel.FrontProperties, 0));
                    ZnodeLogging.LogMessage(IsNotNull(frontPropertiesInsert) ? PIM_Resources.SuccessInsertFrontProperties : PIM_Resources.ErrorInsertFrontProperties, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                    //Logic to assign attribute to family with its group
                    if (pimAttributemodel.AttributeModel.AttributeGroupId > 0)
                        AssignAttributeToFamilyWithGroup(pimAttributemodel.AttributeModel.AttributeGroupId, attributeModel);

                    ZnodeLogging.LogMessage("AttributeGroupId and PimAttributeId properties of pimAttributemodel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pimAttributemodel?.AttributeModel?.AttributeGroupId, pimAttributemodel?.FrontProperties?.PimAttributeId });

                    string _attributeType = _attributeTypeRepository.GetById(pimAttributemodel.AttributeModel.AttributeTypeId.GetValueOrDefault())?.AttributeTypeName;
                    ZnodeLogging.LogMessage("_attributeType: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, _attributeType);

                    //Insert validation rule.
                    if (IsNotNull(pimAttributemodel.ValidationRule))
                    {
                        pimAttributemodel.ValidationRule.ForEach(x =>
                        {
                            x.PimAttributeId = attributeModel.PimAttributeId;
                            ZnodeLogging.LogMessage(IsNotNull(_validationRepository.Insert(PIMAttributesMap.ToValidationEntity(x, _attributeType))) ? PIM_Resources.SuccessInsertValidation : PIM_Resources.ErrorInsertValidation, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                        });
                    }

                    //Save defaultvalue 

                    if (_attributeType.Equals(ZnodeConstant.NumberType) || _attributeType.Equals(ZnodeConstant.YesNoType) || _attributeType.Equals(ZnodeConstant.DateType))
                        SaveDefaultValues(PIMAttributesMap.ToPIMAttributeDefaultValueModel(pimAttributemodel, GetDefaultLocaleId(), _attributeType));
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    return PIMAttributesMap.ToPIMAttributeDataModel(attributeModel, frontPropertiesInsert);
                }
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return pimAttributemodel;
            }
            return null;
        }

        //Delete  existing Attribute By Attribute Id
        public virtual bool DeleteAttribute(ParameterModel pimAttributeIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter pimAttributeIds to delete existing attribute: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeIds);

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("PimAttributeId", pimAttributeIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimAttributeWithReference @PimAttributeId,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("Deleted result count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, deleteResult?.Count);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteAttribute, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorAttributeDeletion, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, PIM_Resources.ErrorAttributeDeletion);
            }
        }

        //Create Attribute Locales
        public virtual PIMAttributeLocaleListModel SaveLocales(PIMAttributeLocaleListModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            ZnodeLogging.LogMessage("AttributeCode property and locale list count of PIMAttributeLocaleListModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { AttributeCode = model?.AttributeCode , LocaleListCount = model?.Locales?.Count });

            if (model.Locales?.Count > 0)
            {
                //Delete Existing Locale Values Before Update or save
                _LocaleRepository.Delete(GetWhereClauseForPIMAttributeId((model.Locales.FirstOrDefault().PimAttributeId)).WhereClause);

                int defaultLocaleId = GetDefaultLocaleId();
                ZnodeLogging.LogMessage("defaultLocaleId generated by GetDefaultLocaleId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, defaultLocaleId);

                //If locale name is present then attribute code will be save as default locale name.
                if (model.Locales.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.AttributeName.Trim())))
                    model.Locales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.AttributeName = model.AttributeCode);

                model.Locales.RemoveAll(x => x.AttributeName == string.Empty);

                _LocaleRepository.Insert(model.Locales.ToEntity<ZnodePimAttributeLocale>().ToList());
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return model;
        }

        //Create Attribute default Value
        public virtual PIMAttributeDefaultValueModel SaveDefaultValues(PIMAttributeDefaultValueModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PimDefaultAttributeValueId property of PIMAttributeDefaultValueModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.PimDefaultAttributeValueId);

            if (model.PimDefaultAttributeValueId == 0)
            {
                //If attribute code is already present.
                if(IsAttributeCodeExist(model.AttributeDefaultValueCode, Convert.ToInt32(model.PimAttributeId)))
                    throw new ZnodeException(ErrorCodes.AlreadyExist, Api_Resources.AttributeCodeValidation);
                InsertDefaultValues(model);
            }
            else
                InsertUpdateDefaultValueLocale(model, model.PimDefaultAttributeValueId);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return model;
        }

        //Delete Attribute Default Values By PimAttributeDefaultValueId
        public virtual bool DeleteDefaultValues(int defaultvalueId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter defaultvalueId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, defaultvalueId);

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodePimAttributeDefaultValueEnum.PimAttributeDefaultValueId.ToString(), defaultvalueId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimDefaultAttributeValues @PimAttributeDefaultValueId,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("Deleted result count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, deleteResult?.Count);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteAttribute, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorDeleteDefaultAttributeValue, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.RestrictSystemDefineDeletion, PIM_Resources.ErrorDeleteDefaultAttributeValue);
            }
        }

        //Get attributetype list
        public virtual PIMAttributeTypeListModel GetAttributeTypes(bool isCategory)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter isCategory: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, isCategory);
            //Filter to get attribute types for PIM attribute.

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeAttributeTypeEnum.IsPimAttributeType.ToString(), ProcedureFilterOperators.Equals, "true"));

            List<ZnodeAttributeType> attributeType = _attributeTypeRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause).ToList();
            ZnodeLogging.LogMessage("attributeType list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeType?.Count);

            if (isCategory)
                attributeType.RemoveAll(x => x.AttributeTypeName == "Link");

            if (attributeType?.Count > 0)
                return new PIMAttributeTypeListModel() { Types = attributeType.ToModel<PIMAttributeTypeModel>().ToList() };
            else
                return null;
        }

        //Get List Of Input Validation
        public virtual PIMAttributeInputValidationListModel GetInputValidations(int typeId, int attributeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeAttributeInputValidationEnum.AttributeTypeId.ToString(), ProcedureFilterOperators.Equals, typeId.ToString()));

            _inputValidationRepository.EnableDisableLazyLoading = true;
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to get input validation list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClause?.WhereClause);
            IList<ZnodeAttributeInputValidation> list = _inputValidationRepository.GetEntityList(whereClause.WhereClause);
            ZnodeLogging.LogMessage("Input validation list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, list?.Count);

            if (Equals(_attributeTypeRepository.GetEntity(whereClause.WhereClause)?.AttributeTypeName, "File"))
            {
                List<ZnodeAttributeInputValidationRule> validationrule = new List<ZnodeAttributeInputValidationRule>();
                _inputValidationRepository.Table.Where(x => (x.ZnodeAttributeType.AttributeTypeName == "Video" || x.ZnodeAttributeType.AttributeTypeName == "Audio") && (x.Name == ZnodeConstant.Extensions))?.ToList().ForEach(item => { validationrule.AddRange(item.ZnodeAttributeInputValidationRules); });

                list?.ToList()?.ForEach(item =>
                {
                    if (item.Name.Equals(ZnodeConstant.Extensions))
                    {
                        validationrule.AddRange(item.ZnodeAttributeInputValidationRules);
                        item.ZnodeAttributeInputValidationRules = validationrule;
                    }
                });
            }

            PIMAttributeInputValidationListModel _model = PIMAttributesMap.ToInputValidationListModel(list);
            if (attributeId != 0)
                GetValidationValue(_model, attributeId);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return _model;
        }

        //Get FrontEndProperties
        public virtual PIMFrontPropertiesModel FrontEndProperties(int pimAttributeId) =>
         PIMAttributesMap.ToFrontPropertiesModel(_frontEndProperties.GetEntity(GetWhereClauseForPIMAttributeId(pimAttributeId).WhereClause));

        //Get Attribute locale By attribute Id
        public virtual PIMAttributeLocaleListModel GetAttributeLocale(int attributeId)
         => PIMAttributesMap.ToLocaleListModel(_LocaleRepository.GetEntityList(GetWhereClauseForPIMAttributeId(attributeId).WhereClause));

        //Gets the locale of the attribute according to attributeCode and Locale Id.
        public virtual string GetAttributeLocale(string attributeCode, int localeId)
        {
            PIMAttributeLocaleListModel attributeLocaleList = 
                GetAttributeLocale((_pimAttributeRepository.Table.Where(x => x.AttributeCode.ToLower() == attributeCode.ToLower())
                ?.FirstOrDefault()?.PimAttributeId) ?? 0);
            return attributeLocaleList.Locales.Where(x => x.LocaleId == localeId)?.FirstOrDefault()?.AttributeName;
        }

        //Get Attribute Default Values
        public virtual PIMAttributeDefaultValueListModel GetDefaultValues(int attributeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter attributeId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeId);

            _validationRepository.EnableDisableLazyLoading = true;
            PIMAttributeDefaultValueListModel model;
            model = PIMAttributesMap.ToDefaultValueListModel(_defaultValueRepository.GetEntityList(GetWhereClauseForPIMAttributeId(attributeId).WhereClause));
            foreach (var item in model.DefaultValues)
            {
                item.MediaPath = item.MediaId > 0 ? GetMediaPath(item) : string.Empty;
            }
            ZnodeLogging.LogMessage("DefaultValues list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.DefaultValues?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return model;
        }

        //Method To find if Attribute Code is already Present in table or not.
        public virtual bool IsAttributeCodeExist(string attributeCode, bool isCategory)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters attributeCode and isCategory: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { attributeCode , isCategory});

            if (string.IsNullOrEmpty(attributeCode))
                throw new ZnodeException(ErrorCodes.NullModel, PIM_Resources.ErrorNullAttributeCodeModel);

            return _pimAttributeRepository.Table.Any(x => x.AttributeCode.ToLower() == attributeCode.ToLower() && x.IsCategory == isCategory);
        }

        //Method to check value of attribute is already exists or not.
        public virtual string IsAttributeValueUnique(PimAttributeValueParameterModel attributeCodeValues)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(attributeCodeValues))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            ZnodeLogging.LogMessage("IsCategory, LocaleId and id properties of PimAttributeValueParameterModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose,new object[] { attributeCodeValues?.IsCategory, attributeCodeValues?.LocaleId, attributeCodeValues?.Id });

            var xmlData = HelperUtility.ToXML<List<PIMAttributeCodeValueModel>>(attributeCodeValues.PIMAttributeCodeValueList);

            IZnodeViewRepository<PimAttributeParameterModel> objStoredProc = new ZnodeViewRepository<PimAttributeParameterModel>();
            objStoredProc.SetParameter("AttributeCodeValues", xmlData, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePimAttributeEnum.IsCategory.ToString(), attributeCodeValues.IsCategory, ParameterDirection.Input, DbType.Boolean);
            if (!attributeCodeValues.IsCategory)
            {
                objStoredProc.SetParameter("ProductId", attributeCodeValues.Id, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("CategoryId", 0, ParameterDirection.Input, DbType.Int32);
            }
            else
            {
                objStoredProc.SetParameter("ProductId", 0, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("CategoryId", attributeCodeValues.Id, ParameterDirection.Input, DbType.Int32);
            }
            objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), attributeCodeValues.LocaleId, ParameterDirection.Input, DbType.Int32);
            List<PimAttributeParameterModel> attributeNames = objStoredProc.ExecuteStoredProcedureList("Znode_CheckUniqueAttributeValues @AttributeCodeValues,@IsCategory,@ProductId,@CategoryId,@LocaleId")?.ToList();
            ZnodeLogging.LogMessage("attributeNames list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeNames?.Count);

            if (attributeNames?.Count > 0)
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return string.Join(",", attributeNames.Select(x => x.AttributeName));
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return string.Empty;
        }

        // Get attribute validation by attribute code.
        public virtual PIMFamilyDetailsModel GetAttributeValidationByCodes(ParameterProductModel attributeCodes)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter attributeCodes: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeCodes);

            IZnodeViewRepository<PIMProductAttributeValuesModel> objStoredProc = new ZnodeViewRepository<PIMProductAttributeValuesModel>();
            objStoredProc.SetParameter("AttributeCode", attributeCodes.HighLightsCodes, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("LocaleId", attributeCodes.LocaleId, ParameterDirection.Input, DbType.Int32);

            IList<PIMProductAttributeValuesModel> attributeValidation = objStoredProc.ExecuteStoredProcedureList("Znode_GetPublishAttributeDetail @AttributeCode,@LocaleId");
            ZnodeLogging.LogMessage("attributeValidation list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeValidation?.Count);

            PIMFamilyDetailsModel model = new PIMFamilyDetailsModel();

            model.Attributes = IsNotNull(attributeValidation) ? attributeValidation.ToList() : new List<PIMProductAttributeValuesModel>();
            ZnodeLogging.LogMessage("Attributes list count of PIMFamilyDetailsModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.Attributes?.Count);
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return model;
        }
        #endregion

        #region Private Methods

        //Insert And UpdateGroupMapperandFamilyGroupMapper ,if group and attribute not in use.
        private void InsertAndUpdateGroupMapperandFamilyGroupMapper(PIMAttributeDataModel model)
        {
            FilterTuple atttributeIdFilter = new FilterTuple(ZnodePimFamilyGroupMapperEnum.PimAttributeId.ToString(), ProcedureFilterOperators.Equals, model.AttributeModel.PimAttributeId.ToString());
            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(atttributeIdFilter);

            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage("WhereClause generated to get pimAttributeGroupMapper: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClause);

            ZnodePimFamilyGroupMapper pimAttributeGroupMapper = _pimFamilyGroupMapperRepository.GetEntity(whereClause);

            if (UnAssignAttributeToFamilyMapper(model) && model.AttributeModel.AttributeGroupId.GetValueOrDefault() > 0)
            {
                UpdateIntoGroupMapper(model);
                InsertIntoPIMFamilyGroupMapper(model, pimAttributeGroupMapper);
            }
        }

        //This method is used to get media path from media Id.
        private string GetMediaPath(PIMAttributeDefaultValueModel model)
        {
            IMediaManagerServices mediaService = GetService<IMediaManagerServices>();
            MediaManagerModel mediaData = mediaService.GetMediaByID(Convert.ToInt32(model?.MediaId), null);
            ZnodeLogging.LogMessage("mediaData generated to get by method GetMediaByID: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, mediaData);
            return model.MediaPath = IsNotNull(mediaData) ? mediaData.MediaServerThumbnailPath : ZnodeAdminSettings.DefaultImagePath;
        }

        //Update pim attribute group mapper
        private void UpdateIntoGroupMapper(PIMAttributeDataModel model) =>
            ZnodeLogging.LogMessage(IsNotNull(_pimAttributeGroupMapperRepository.Insert(new ZnodePimAttributeGroupMapper() { PimAttributeId = model.AttributeModel.PimAttributeId, PimAttributeGroupId = model.AttributeModel.AttributeGroupId, IsSystemDefined = model.AttributeModel.IsSystemDefined })) ? PIM_Resources.SuccessPIMAttributeInsert : PIM_Resources.ErrorPIMAttributeInsert, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

        //Insert into family group mapper and group mapper.
        private void InsertIntoPIMFamilyGroupMapper(PIMAttributeDataModel model, ZnodePimFamilyGroupMapper familyGroupMapper)
        {
            if (familyGroupMapper?.PimAttributeGroupId == model.AttributeModel?.AttributeGroupId)
                _pimFamilyGroupMapperRepository.Insert(new ZnodePimFamilyGroupMapper() { PimFamilyGroupMapperId = 0, PimAttributeFamilyId = familyGroupMapper?.PimAttributeFamilyId, PimAttributeGroupId = familyGroupMapper?.PimAttributeGroupId, PimAttributeId = familyGroupMapper.PimAttributeId, IsSystemDefined = familyGroupMapper.IsSystemDefined, GroupDisplayOrder = familyGroupMapper.GroupDisplayOrder });
            else
            {
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter("PimAttributeId", model.AttributeModel.PimAttributeId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("PimAttributeGroupId", model.AttributeModel.AttributeGroupId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("UserId", HelperMethods.GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                int status = 0;
                IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_InsertPimAttributeToFamily @PimAttributeId, @PimAttributeGroupId, @UserId", 1, out status);
                ZnodeLogging.LogMessage("deleteResult list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, deleteResult?.Count);
                if (deleteResult.FirstOrDefault().Status.Value)
                    ZnodeLogging.LogMessage(PIM_Resources.SuccessAssignAttributeToFamily, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                else
                    ZnodeLogging.LogMessage(PIM_Resources.ErrorAssignAttributeToFamily, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }
        }

        //Update Front Properties Values
        private bool UpdateFrontProperties(PIMFrontPropertiesModel model, int attributeId)
        {
            ZnodePimFrontendProperty frontEndProperties = _frontEndProperties.GetEntity(GetWhereClauseForPIMAttributeId(attributeId).WhereClause);
            if (IsNotNull(frontEndProperties))
            {
                model.PimAttributeId = attributeId;
                return _frontEndProperties.Update(PIMAttributesMap.ToFrontPropertiesEntity(model, frontEndProperties.ZnodePimFrontendPropertiesId));
            }
            else
                return false;
        }

        //Update Existing iNput Validation values
        private bool UpdateValidations(List<PIMAttributeValidationModel> model, int attributeId, string attributeType)
        {
            ZnodeLogging.LogMessage("Input parameters attributeId and attributeType and PIMAttributeValidationModel list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { attributeId = attributeId, attributeType = attributeType, PIMAttributeValidationModelListCount = model?.Count });

            if (model.Count > 0)
            {
                FilterCollection filter = new FilterCollection();
                filter.Add(new FilterTuple(ZnodePimAttributeEnum.PimAttributeId.ToString(), ProcedureFilterOperators.Equals, attributeId.ToString()));

                //Delete Exiting Validation Values for attribute
                ZnodeLogging.LogMessage(_validationRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause)
                    ? PIM_Resources.SuccessDeleteAttributeValidation : PIM_Resources.ErrorDeleteAttributeValidation, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                List<ZnodePimAttributeValidation> pimAttributeValidation = new List<ZnodePimAttributeValidation>();
                model.ForEach(validationModel =>
                {
                    validationModel.PimAttributeId = attributeId;
                    pimAttributeValidation.Add(PIMAttributesMap.ToValidationEntity(validationModel, attributeType));
                });
                _validationRepository.Insert(pimAttributeValidation);
                return true;
            }
            else
                return false;
        }

        //Get List Of Validation Rule
        private void GetValidationValue(PIMAttributeInputValidationListModel model, int attributeId)
        {
            ZnodeLogging.LogMessage("Input parameter InputValidations list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.InputValidations?.Count);

            _validationRepository.EnableDisableLazyLoading = false;
            foreach (PIMAttributeInputValidationModel item in model.InputValidations)
            {
                FilterCollection filter = new FilterCollection();
                filter.Add(new FilterTuple(ZnodeAttributeInputValidationEnum.InputValidationId.ToString(), ProcedureFilterOperators.Equals, item.AttributeValidationId.ToString()));
                filter.Add(new FilterTuple(ZnodePimAttributeEnum.PimAttributeId.ToString(), ProcedureFilterOperators.Equals, attributeId.ToString()));
                //Get Multiple Default values if validation rules is more than 2
                if (item.Rules.Count > 2)
                {
                    List<ZnodePimAttributeValidation> entity = _validationRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause).ToList();
                    item.DefaultValue = GetValueRuleIds(entity);
                }
                else
                {
                    ZnodePimAttributeValidation values = _validationRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
                    if(IsNotNull(values))
                        item.DefaultValue = IsNull(values.InputValidationRuleId) ? Convert.ToString(values.Name) : Convert.ToString(values.InputValidationRuleId);
                }
            }
        }

        //Get Multiple Validation Rule Id
        private string GetValueRuleIds(List<ZnodePimAttributeValidation> list)
        {
            string ruleIds = string.Empty;
            list.ForEach(item =>
            {
                ruleIds += item.InputValidationRuleId.ToString() + ",";
            });
            return !string.IsNullOrEmpty(ruleIds) ? ruleIds.Remove(ruleIds.Length - 1) : string.Empty;
        }

        //Method To create and edit The attribute locale default values
        private void InsertUpdateDefaultValueLocale(PIMAttributeDefaultValueModel model, int pimDefaultAttributeValueId)
        {
            //Set AttributeDefaultValueCode as if attribute type are Number, Datetime and YesNo type..
            if (model.AttributeDefaultValueCode == null)
                model.AttributeDefaultValueCode = model.ValueLocales?.Select(x => x.DefaultAttributeValue)?.FirstOrDefault();

            ZnodeLogging.LogMessage("AttributeDefaultValueCode property of PIMAttributeDefaultValueModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.AttributeDefaultValueCode);

            ZnodePimAttributeDefaultValue defaultValueEntity = PIMAttributesMap.ToDefaultValueEntity(model);
            defaultValueEntity.PimAttributeDefaultValueId = pimDefaultAttributeValueId;

            //If new added model has IsDefault true ,it disables previous ones. 
            if (Equals(defaultValueEntity.IsDefault, true))
                DisableDefaultAttribute(defaultValueEntity);

            ZnodeLogging.LogMessage(_defaultValueRepository.Update(defaultValueEntity) ? PIM_Resources.SuccessUpdateAttributeDefaultValue : PIM_Resources.ErrorUpdateAttributeDefaultValue, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            foreach (PIMAttributeDefaultValueLocaleModel item in model.ValueLocales)
            {
                item.PimDefaultAttributeValueId = pimDefaultAttributeValueId;
                FilterCollection filterCol = new FilterCollection();
                filterCol.Add(new FilterTuple(ZnodePimAttributeLocaleEnum.LocaleId.ToString(), ProcedureFilterOperators.Equals, item.LocaleId.ToString()));
                filterCol.Add(new FilterTuple(ZnodePimAttributeDefaultValueEnum.PimAttributeDefaultValueId.ToString(), ProcedureFilterOperators.Equals, item.PimDefaultAttributeValueId.ToString()));

                ZnodePimAttributeDefaultValueLocale _localeEntity = _defaultValueLocaleRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterCol.ToFilterDataCollection()).WhereClause);
                item.PimDefaultAttributeValueId = pimDefaultAttributeValueId;

                ZnodePimAttributeDefaultValueLocale defaultValueLocaleEntity = PIMAttributesMap.ToDefaultValueLocaleEntity(item);
                if (IsNotNull(_localeEntity))
                {
                    defaultValueLocaleEntity.PimAttributeDefaultValueLocaleId = _localeEntity.PimAttributeDefaultValueLocaleId;

                    //If locale code if empty than delete record other wise update existing record.
                    if (string.IsNullOrEmpty(defaultValueLocaleEntity.AttributeDefaultValue))
                        ZnodeLogging.LogMessage(_defaultValueLocaleRepository.Delete(defaultValueLocaleEntity) ? PIM_Resources.SuccessUpdateAttributeLocale : PIM_Resources.ErrorUpdateAttributeLocale, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    else
                        ZnodeLogging.LogMessage(_defaultValueLocaleRepository.Update(defaultValueLocaleEntity) ? PIM_Resources.SuccessUpdateAttributeLocale : PIM_Resources.ErrorUpdateAttributeLocale, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                }
                else
                {
                    if (!string.IsNullOrEmpty(defaultValueLocaleEntity.AttributeDefaultValue))
                        ZnodeLogging.LogMessage(IsNotNull(_defaultValueLocaleRepository.Insert(defaultValueLocaleEntity)) ? PIM_Resources.SuccessInsertAttributeLocale : PIM_Resources.ErrorInsertAttributeLocale, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                }
            }
        }

        //Insert Attribute Default values
        private void InsertDefaultValues(PIMAttributeDefaultValueModel model)
        {
            //Set AttributeDefaultValueCode as if attribute type are Number, Datetime and YesNo type..
            if (model.AttributeDefaultValueCode == null)
                model.AttributeDefaultValueCode = model.ValueLocales?.Select(x => x.DefaultAttributeValue)?.FirstOrDefault();

            ZnodeLogging.LogMessage("AttributeDefaultValueCode property of PIMAttributeDefaultValueModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.AttributeDefaultValueCode);

            //If new added model has IsDefault true ,it disables previous ones. 
            if (Equals(model.IsDefault, true))
                DisableDefaultAttribute(PIMAttributesMap.ToDefaultValueEntity(model));

            ZnodePimAttributeDefaultValue _defaultValue = _defaultValueRepository.Insert(PIMAttributesMap.ToDefaultValueEntity(model));
            ZnodeLogging.LogMessage(IsNotNull(_defaultValue) ? PIM_Resources.SuccessInsertAttributeDefaultValue : PIM_Resources.ErrorInsertAttributeDefaultValue, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNotNull(_defaultValue))
            {
                model.ValueLocales.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.DefaultAttributeValue))
                    {
                        x.PimDefaultAttributeValueId = _defaultValue.PimAttributeDefaultValueId;
                        ZnodeLogging.LogMessage(IsNotNull(_defaultValueLocaleRepository.Insert(PIMAttributesMap.ToDefaultValueLocaleEntity(x))) ? PIM_Resources.SuccessInsertAttributeLocale : PIM_Resources.ErrorInsertAttributeLocale, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    }
                });
                model.PimDefaultAttributeValueId = _defaultValue.PimAttributeDefaultValueId;
            }
        }

        //Get where clause by PimAttributeId
        private EntityWhereClauseModel GetWhereClauseForPIMAttributeId(int? pimAttributeId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeEnum.PimAttributeId.ToString(), ProcedureFilterOperators.Equals, pimAttributeId.ToString()));
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
        }

        //This function use for assign attribute to family group mapper and Unassign based on attributeId, if attribute not in use.
        private bool UnAssignAttributeToFamilyMapper(PIMAttributeDataModel model)
        {
            FilterTuple atttributeIdFilter = new FilterTuple(ZnodePimFamilyGroupMapperEnum.PimAttributeId.ToString(), ProcedureFilterOperators.Equals, model.AttributeModel.PimAttributeId.ToString());
            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(atttributeIdFilter);

            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage("WhereClause generated to get attributeGroupMapper: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClause);
            ZnodePimAttributeGroupMapper attributeGroupMapper = _pimAttributeGroupMapperRepository.GetEntity(whereClause);
            if (attributeGroupMapper?.PimAttributeGroupId == model.AttributeModel.AttributeGroupId)
                return false;

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodePimFamilyGroupMapperEnum.PimAttributeId.ToString(), model.AttributeModel.PimAttributeId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePimFamilyGroupMapperEnum.PimAttributeGroupId.ToString(), 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("IsCategory", model.AttributeModel.IsCategory, ParameterDirection.Input, DbType.Boolean);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UnassignPimAttributeFromGroup @PimAttributeId,@PimAttributeGroupId,@IsCategory");
            ZnodeLogging.LogMessage("deleteResult list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, deleteResult?.Count);

            if (deleteResult.FirstOrDefault().Status.Value)
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteAttributeGroupMapper, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            else
                ZnodeLogging.LogMessage(PIM_Resources.ErrorDeleteAttributeGroupMapper, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return deleteResult.FirstOrDefault().Status.Value;
        }

        //This funcation use for reset filter value of IsSystemDefined,IsRequiredand, IsPersonalizable and IsLocalizable.
        private void ResetFilters(FilterCollection filters)
        {
            filters = IsNull(filters) ? new FilterCollection() : filters;
            if (IsNotNull(filters.Where(x => x.FilterName.Equals(ZnodePimAttributeEnum.IsSystemDefined.ToString().ToLower()))?.FirstOrDefault()))
                SetFilterValue(filters, ZnodePimAttributeEnum.IsSystemDefined.ToString().ToLower());
            if (IsNotNull(filters.Where(x => x.FilterName.Equals(ZnodePimAttributeEnum.IsRequired.ToString().ToLower()))?.FirstOrDefault()))
                SetFilterValue(filters, ZnodePimAttributeEnum.IsRequired.ToString().ToLower());
            if (IsNotNull(filters.Where(x => x.FilterName.Equals(ZnodePimAttributeEnum.IsLocalizable.ToString().ToLower()))?.FirstOrDefault()))
                SetFilterValue(filters, ZnodePimAttributeEnum.IsLocalizable.ToString().ToLower());
            if (IsNotNull(filters.Where(x => x.FilterName.Equals(ZnodePimAttributeEnum.IsPersonalizable.ToString().ToLower()))?.FirstOrDefault()))
                SetFilterValue(filters, ZnodePimAttributeEnum.IsPersonalizable.ToString().ToLower());
        }

        //This funcation use for set filter value.
        private void SetFilterValue(FilterCollection filters, string filterName)
        {
            FilterTuple _filterAttributeType = filters.Where(x => x.FilterName.Equals(filterName))?.FirstOrDefault();
            filters.RemoveAll(x => x.FilterName.Equals(filterName));
            if(IsNotNull(_filterAttributeType))
            {
                if (_filterAttributeType.FilterValue.Equals("true", StringComparison.OrdinalIgnoreCase))
                    filters.Add(filterName, ProcedureFilterOperators.Equals, "1");
                else if (_filterAttributeType.FilterValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                    filters.Add(filterName, ProcedureFilterOperators.Equals, "0");
            }
        }

        //Assign attribute to family and group.
        private void AssignAttributeToFamilyWithGroup(int? AttributeGroupId, ZnodePimAttribute attributeModel)
        {
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("PimAttributeId", attributeModel.PimAttributeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("PimAttributeGroupId", AttributeGroupId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("UserId", HelperMethods.GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_InsertPimAttributeToFamily @PimAttributeId, @PimAttributeGroupId, @UserId", 1, out status);
            ZnodeLogging.LogMessage("deleteResult list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, deleteResult?.Count);
            if (deleteResult.FirstOrDefault().Status.Value)
                ZnodeLogging.LogMessage(PIM_Resources.SuccessAssignAttributeToFamily, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            else
                ZnodeLogging.LogMessage(PIM_Resources.ErrorAssignAttributeToFamily, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
        }

        //If new added model has IsDefault true ,it disables previous ones. 
        private void DisableDefaultAttribute(ZnodePimAttributeDefaultValue znodePimAttributeDefaultValueEntity)
        {
            //checks if the filter collection null
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeEnum.PimAttributeId.ToString(), ProcedureFilterOperators.Equals, znodePimAttributeDefaultValueEntity.PimAttributeId.ToString()));

            IList<ZnodePimAttributeDefaultValue> ZnodePimAttributeDefaultValue = _defaultValueRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage("ZnodePimAttributeDefaultValue list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, ZnodePimAttributeDefaultValue?.Count);

            if (znodePimAttributeDefaultValueEntity.IsDefault == true)
            {
                List<ZnodePimAttributeDefaultValue> defaultPimAttributeDefaultValueList = ZnodePimAttributeDefaultValue.Where(x => x.IsDefault == true).ToList();
                defaultPimAttributeDefaultValueList.ForEach(x => x.IsDefault = false);
                defaultPimAttributeDefaultValueList.ForEach(x => _defaultValueRepository.Update(x));
            }
        }
        #endregion

        #region Protected Method
        //Check uniqueness of attribute code
        protected virtual bool IsAttributeCodeExist(string attributeCode, int attributeId)
        {
            if (IsNotNull(attributeCode) && IsNotNull(attributeId))
            {
                return _defaultValueRepository.Table.Any(x => x.AttributeDefaultValueCode.Equals(attributeCode, StringComparison.InvariantCultureIgnoreCase) && x.PimAttributeId == attributeId);
            }
            return false;
        }
        #endregion
    }
}
