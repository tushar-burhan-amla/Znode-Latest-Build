using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Constants;
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
    public class AttributeFamilyService : BaseService, IAttributeFamilyService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeMediaAttributeFamily> _attributeFamily;
        private readonly IZnodeRepository<ZnodeMediaFamilyGroupMapper> _attributeFamilyGroupMapper;
        private readonly IZnodeRepository<ZnodeMediaAttributeGroupMapper> _attributeGroupMapper;
        private readonly IZnodeRepository<ZnodeMediaFamilyLocale> _attributeFamilyLocale;
        private readonly IZnodeRepository<ZnodeMediaAttribute> _attributeRepository;
        private readonly IZnodeRepository<ZnodeAttributeType> _attributeType;
        private readonly IZnodeRepository<ZnodeMediaCategory> _mediaCategory;
        private readonly IZnodeRepository<ZnodeMediaFamilyLocale> _familyLocaleRepository;
        private readonly IZnodeRepository<ZnodeLocale> _localeRepository;
        private readonly IZnodeRepository<ZnodeMediaAttributeGroup> _mediaAttributeGroup;
        #endregion

        #region Constructor
        public AttributeFamilyService()
        {
            _attributeFamily = new ZnodeRepository<ZnodeMediaAttributeFamily>();
            _attributeFamilyGroupMapper = new ZnodeRepository<ZnodeMediaFamilyGroupMapper>();
            _attributeGroupMapper = new ZnodeRepository<ZnodeMediaAttributeGroupMapper>();
            _attributeFamilyLocale = new ZnodeRepository<ZnodeMediaFamilyLocale>();
            _attributeRepository = new ZnodeRepository<ZnodeMediaAttribute>();
            _attributeType = new ZnodeRepository<ZnodeAttributeType>();
            _mediaCategory = new ZnodeRepository<ZnodeMediaCategory>();
            _familyLocaleRepository = new ZnodeRepository<ZnodeMediaFamilyLocale>();
            _localeRepository = new ZnodeRepository<ZnodeLocale>();
            _mediaAttributeGroup = new ZnodeRepository<ZnodeMediaAttributeGroup>();
        }
        #endregion

        #region Public Methods

        //Get a List of Attribute Family
        public virtual AttributeFamilyListModel GetAttributeFamilyList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate AttributeFamilyModel list ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //get the order by clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);
            ZnodeLogging.LogMessage("orderBy: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, orderBy);
            //set locale Filters if not present
            SetLocaleFilterIfNotPresent(ref filters);

            //get the where clause with filter Values.              
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause to generate AttributeFamilyModel list: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, whereClause);
            IZnodeViewRepository<AttributeFamilyModel> objStoredProc = new ZnodeViewRepository<AttributeFamilyModel>();
            objStoredProc.SetParameter("@WhereClause", whereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", orderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<AttributeFamilyModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetMediaAttributeFamilies @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("AttributeFamilyModel list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, list?.Count());
            AttributeFamilyListModel attributeFamilyListModel = new AttributeFamilyListModel { AttributeFamilies = list?.ToList() };

            attributeFamilyListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return attributeFamilyListModel;
        }

        //Get Attribute Family by Attribute Family Id.
        public virtual AttributeFamilyModel GetAttributeFamily(int attributeFamilyId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter attributeFamilyId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { attributeFamilyId });
            if (attributeFamilyId > 0)
            {
                ZnodeMediaAttributeFamily attributeFamily = _attributeFamily.GetById(attributeFamilyId);
                return !Equals(attributeFamily, null) ? attributeFamily.ToModel<AttributeFamilyModel>() : null;
            }
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return null;
        }

        public virtual AttributeFamilyModel Create(AttributeFamilyModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter AttributeFamilyModel having FamilyCode and ExistingAttributeFamilyId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { model.FamilyCode, model.ExistingAttributeFamilyId.ToString() });
            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (IsAttributeFamilyExist(model.FamilyCode))
                throw new ZnodeException(ErrorCodes.AlreadyExist, PIM_Resources.ErrorFamilyAlreadyExists);

            //Creates new attribute family.
            ZnodeMediaAttributeFamily attributeFamily = _attributeFamily.Insert(model.ToEntity<ZnodeMediaAttributeFamily>());
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessCreateAttributeFamily, model.FamilyCode), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            //Filter to generate where clause.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeMediaAttributeFamilyEnum.MediaAttributeFamilyId.ToString(), ProcedureFilterOperators.Equals, model.ExistingAttributeFamilyId.ToString()));

            //Where clause of existing attribute family.     
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            if (model.ExistingAttributeFamilyId > 0)
                ZnodeLogging.LogMessage(CreateAttributeFamilyGroupMapper(attributeFamily, whereClauseModel) ? Admin_Resources.SuccessInsertMediaAttributeFamilyGroup : Admin_Resources.ErrorInsertMediaAttributeFamilyGroup, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage(CreateMediaFamilyLocale(attributeFamily, whereClauseModel) ? Admin_Resources.SuccessInsertMediaAttributeFamilyLocale : Admin_Resources.ErrorInsertMediaAttributeFamilyLocale, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            if (!Equals(attributeFamily, null))
                return attributeFamily.ToModel<AttributeFamilyModel>();
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }

        //Get A List of Assigned Attribute Group
        public virtual AttributeGroupListModel GetAssignedAttributeGroups(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            AttributeGroupListModel listModel = new AttributeGroupListModel();

            //Generate the order by clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);

            List<string> navigationProperties = GetExpands(expands);

            //Gets the where clause.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel to generate groupFamiliesMapper list ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClauseModel);
            IList<ZnodeMediaFamilyGroupMapper> groupFamiliesMapper = _attributeFamilyGroupMapper.GetEntityList(whereClauseModel.WhereClause, navigationProperties).GroupBy(item => item.MediaAttributeGroupId).Select(g => g.First()).ToList();
            ZnodeLogging.LogMessage("groupFamiliesMapper list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, groupFamiliesMapper?.Count());
            foreach (var groupFamily in groupFamiliesMapper)
            {
                ZnodeMediaAttributeGroup attributeGroup = groupFamily.ZnodeMediaAttributeGroup;
                if (IsNotNull(attributeGroup))
                    listModel.AttributeGroups.Add(attributeGroup.ToModel<AttributeGroupModel>());
            }
            listModel.AttributeGroups = listModel.AttributeGroups?.OrderBy(x => x.DisplayOrder).ToList();
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return listModel;
        }

        public virtual bool AssignAttributeGroups(FamilyGroupAttributeListModel listModel)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (Equals(listModel, null) || Equals(listModel.FamilyGroupAttributes, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            //Save attribute groups against attribute family.
            var familyGroupAttributes = _attributeFamilyGroupMapper.Insert(AttributeFamilyMap.ToFamilyGroupAttributeListEntity(listModel));
            ZnodeLogging.LogMessage("familyGroupAttributes:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, familyGroupAttributes);
            if (familyGroupAttributes?.Count() > 0)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessAssignAttributeGroupsToFamily, string.Join(",", familyGroupAttributes.Select(a => a.MediaAttributeGroupId.ToString()).ToArray()), familyGroupAttributes.Select(a => a.MediaAttributeFamilyId).FirstOrDefault()), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return true;
            }
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return false;
        }

        public virtual bool UnAssignAttributeGroups(int attributeFamilyId, int attributeGroupId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter attributeFamilyId and attributeGroupId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { attributeFamilyId, attributeGroupId });
            if (attributeFamilyId < 0 && attributeGroupId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorMapperAbsent);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeMediaFamilyGroupMapperEnum.MediaAttributeFamilyId.ToString(), ProcedureFilterOperators.Equals, Convert.ToString(attributeFamilyId)));
            filters.Add(new FilterTuple(ZnodeMediaFamilyGroupMapperEnum.MediaAttributeGroupId.ToString(), ProcedureFilterOperators.Equals, Convert.ToString(attributeGroupId)));

            //gets the where clause.              
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            if (Convert.ToBoolean(_attributeFamilyGroupMapper.GetEntity(whereClause.WhereClause, whereClause.FilterValues)?.IsSystemDefined))
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.SystemDefinedAttributeGroup);

            //Delete mapping of attribute group against attribute family.
            bool status = _attributeFamilyGroupMapper.Delete(whereClause.WhereClause, whereClause.FilterValues);
            if (status) 
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessUnassignAttributeGroupFromFamily, attributeGroupId, attributeFamilyId), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return status;
        }

        public virtual bool DeleteAttributeFamily(ParameterModel attributeFamilyId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeMediaAttributeFamilyEnum.MediaAttributeFamilyId.ToString(), attributeFamilyId.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteMediaFamily @MediaAttributeFamilyId,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, deleteResult?.Count());
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessDeleteAttributeFamily, attributeFamilyId.Ids), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return true;
            }
            else
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorDeleteAttributeFamily);
        }

        //Get list from AttributeFamilyGroupMapper table.
        public virtual AttributeGroupListModel GetUnAssignedAttributeGroups(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            AttributeGroupListModel listModel = new AttributeGroupListModel();

            //Generate the order by clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);

            NameValueCollection expands = new NameValueCollection();
            expands.Add(ExpandKeys.GroupLocale, ExpandKeys.GroupLocale);
            expands.Add(ExpandKeys.GroupMappers, ExpandKeys.GroupMappers);
            expands.Add(ZnodeMediaAttributeGroupEnum.ZnodeMediaAttributeGroupLocales.ToString().ToLower(), ZnodeMediaAttributeGroupEnum.ZnodeMediaAttributeGroupLocales.ToString().ToLower());
            expands.Add(ZnodeMediaAttributeGroupEnum.ZnodeMediaAttributeGroupMappers.ToString().ToLower(), ZnodeMediaAttributeGroupEnum.ZnodeMediaAttributeGroupMappers.ToString().ToLower());
            List<string> navigationProperties = GetExpands(expands);

            //Gets the where clause.   
            FilterCollection groupFilterList = new FilterCollection();
            groupFilterList.Add(new FilterTuple(ZnodeMediaAttributeGroupEnum.IsHidden.ToString(), ProcedureFilterOperators.Equals, "false"));
            IList<ZnodeMediaAttributeGroup> mediaAttributeGroupList = _mediaAttributeGroup.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(groupFilterList.ToFilterDataCollection()).WhereClause, navigationProperties);
            ZnodeLogging.LogMessage("mediaAttributeGroupList list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, mediaAttributeGroupList?.Count());
            NameValueCollection assignedExpands = new NameValueCollection();
            assignedExpands.Add(ExpandKeys.AttributeGroup, ExpandKeys.AttributeGroup);

            AttributeGroupListModel assignedAttributeGroups = GetAssignedAttributeGroups(assignedExpands, filters, sorts, page);
            AttributeGroupListModel unAssignedAttributeGroups = new AttributeGroupListModel();
            foreach (var item in mediaAttributeGroupList)
            {
                if (Equals(assignedAttributeGroups.AttributeGroups, null))
                    unAssignedAttributeGroups = assignedAttributeGroups;
                else if ((!assignedAttributeGroups?.AttributeGroups?.Any(g => g.MediaAttributeGroupId == item.MediaAttributeGroupId)).GetValueOrDefault() && !item.IsSystemDefined && item.ZnodeMediaAttributeGroupMappers.Count > 0)
                    unAssignedAttributeGroups.AttributeGroups.Add(new AttributeGroupModel
                    {
                        MediaAttributeGroupId = item.MediaAttributeGroupId,
                        GroupCode = item.GroupCode,
                        AttributeGroupName = item.ZnodeMediaAttributeGroupLocales.Count > 0 ? item.ZnodeMediaAttributeGroupLocales?.Where(x => x.LocaleId == Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale)).Select(x => x.AttributeGroupName).FirstOrDefault().ToString() : string.Empty
                    });
            }
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return unAssignedAttributeGroups;
        }

        public virtual AttributeGroupMapperListModel GetAttributesByGroupIds(ParameterModel attributeGroupId)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeMediaAttributeGroupMapperEnum.MediaAttributeGroupId.ToString(), ProcedureFilterOperators.In, string.Join(",", attributeGroupId.Ids)));

            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause;
            IList<ZnodeMediaAttributeGroupMapper> attributeGroupMapperListEntity = _attributeGroupMapper.GetEntityList(whereClause);
            ZnodeLogging.LogMessage("attributeGroupMapperListEntity list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, attributeGroupMapperListEntity?.Count());
            //maps the entity list to model
            AttributeGroupMapperListModel attributeGroupMapperList = AttributeFamilyMap.ToGroupMapperListModel(attributeGroupMapperListEntity);
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return attributeGroupMapperList;
        }

        #region Family Locale
        //Get family locale By family Id.
        public virtual FamilyLocaleListModel GetFamilyLocale(int attributeFamilyId)
         => AttributeFamilyMap.ToLocaleListModel(_familyLocaleRepository.GetEntityList(GetWhereClauseForMediaAttributeFamilyId(attributeFamilyId).WhereClause));

        //Save Family Locales
        public virtual FamilyLocaleListModel SaveLocales(FamilyLocaleListModel model)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter FamilyLocaleListModel having FamilyLocales,FamilyCode", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { model.FamilyLocales, model.FamilyCode });
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model.FamilyLocales?.Count > 0)
            {
                _attributeFamilyLocale.Delete(GetWhereClauseForMediaAttributeFamilyId(model.FamilyLocales.FirstOrDefault().AttributeFamilyId).WhereClause);
                int defaultLocaleId = GetDefaultLocaleId();
                ZnodeLogging.LogMessage("defaultLocaleId: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, defaultLocaleId);
                //If locale name is present then family code will be save as default locale name.
                if (model.FamilyLocales.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.AttributeFamilyName.Trim())))
                    model.FamilyLocales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.AttributeFamilyName = model.FamilyCode);

                model.FamilyLocales.RemoveAll(x => x.AttributeFamilyName == string.Empty);

                //Insert LocaleList Into DataBase
                _attributeFamilyLocale.Insert(model.FamilyLocales.ToEntity<ZnodeMediaFamilyLocale>().ToList());

            }
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return model;
        }
        #endregion

        #endregion

        #region Private Methods
        //Check if attribute family is already exist or not.
        private bool IsAttributeFamilyExist(string attributeFamilyCode)
        {
            ZnodeLogging.LogMessage("Input Parameter attributeFamilyCode:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { attributeFamilyCode });
            //Get the list of all attribute families.
            IList<ZnodeMediaAttributeFamily> attributeFamilyList = _attributeFamily.GetEntityList(string.Empty);
            ZnodeLogging.LogMessage("attributeFamilyList list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, attributeFamilyList?.Count());
            return attributeFamilyList.Any(x => x.FamilyCode == attributeFamilyCode);
        }

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (expands != null && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ExpandKeys.AttributeGroup)) SetExpands(ZnodeMediaAttributeGroupMapperEnum.ZnodeMediaAttributeGroup.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.AttributeFamily)) { SetExpands(ZnodeMediaFamilyLocaleEnum.ZnodeMediaAttributeFamily.ToString(), navigationProperties); }
                    if (Equals(key, ZnodeMediaAttributeGroupEnum.ZnodeMediaAttributeGroupLocales.ToString().ToLower())) SetExpands(ZnodeMediaAttributeGroupEnum.ZnodeMediaAttributeGroupLocales.ToString(), navigationProperties);
                    if (Equals(key, ZnodeMediaAttributeGroupEnum.ZnodeMediaAttributeGroupMappers.ToString().ToLower())) SetExpands(ZnodeMediaAttributeGroupEnum.ZnodeMediaAttributeGroupMappers.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Insert Media Family Locale on creation of Family.
        private bool CreateMediaFamilyLocale(ZnodeMediaAttributeFamily attributeFamily, EntityWhereClauseModel whereClauseModel)
        {
            //Get the list of locales of existing attribute family.
            ZnodeMediaFamilyLocale model = new ZnodeMediaFamilyLocale();

            model.LocaleId = GetDefaultLocaleId();
            model.MediaAttributeFamilyId = attributeFamily.MediaAttributeFamilyId;
            model.AttributeFamilyName = attributeFamily.FamilyCode;

            return !Equals(_attributeFamilyLocale.Insert(model));
        }

        //Insert Media Attribute Family Group on creation of Family.
        private bool CreateAttributeFamilyGroupMapper(ZnodeMediaAttributeFamily attributeFamily, EntityWhereClauseModel whereClauseModel)
        {
            //Get the list of attribute groups of existing attribute family.
            IList<ZnodeMediaFamilyGroupMapper> attributeFamilyGroups = _attributeFamilyGroupMapper.GetEntityList(whereClauseModel.WhereClause);
            ZnodeLogging.LogMessage("attributeFamilyGroups list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, attributeFamilyGroups?.Count());
            if (attributeFamilyGroups?.Count > 0)
            {
                foreach (var attributeFamilyGroup in attributeFamilyGroups)
                {
                    attributeFamilyGroup.MediaAttributeFamilyId = attributeFamily.MediaAttributeFamilyId;
                }
                //Save the list of attribute groups across newly created attribute family
                return !Equals(_attributeFamilyGroupMapper.Insert(attributeFamilyGroups), null);
            }
            return false;
        }

        //Get Where Clause By AttributeFamilyId
        private EntityWhereClauseModel GetWhereClauseForMediaAttributeFamilyId(int? attributeFamilyId)
        {
            ZnodeLogging.LogMessage("Input Parameter attributeFamilyId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new object[] { attributeFamilyId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeMediaAttributeFamilyEnum.MediaAttributeFamilyId.ToString(), ProcedureFilterOperators.Equals, attributeFamilyId.ToString()));
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
        }
        #endregion
    }
}
