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
    public class AttributeGroupService : BaseService, IAttributeGroupService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeMediaAttributeGroup> _attributeGroupRepository;
        private readonly IZnodeRepository<ZnodeMediaAttributeGroupLocale> _attributeGroupLocaleRepository;
        private readonly IZnodeRepository<ZnodeMediaAttributeGroupMapper> _attributeGroupMapperRepository;
        private readonly IZnodeRepository<ZnodeAttributeType> _attributeTypeRepository;
        private readonly IZnodeRepository<ZnodeMediaAttribute> _attributeRepository;
        #endregion

        #region Constructor
        public AttributeGroupService()
        {
            _attributeGroupRepository = new ZnodeRepository<ZnodeMediaAttributeGroup>();
            _attributeGroupLocaleRepository = new ZnodeRepository<ZnodeMediaAttributeGroupLocale>();
            _attributeGroupMapperRepository = new ZnodeRepository<ZnodeMediaAttributeGroupMapper>();
            _attributeTypeRepository = new ZnodeRepository<ZnodeAttributeType>();
            _attributeRepository = new ZnodeRepository<ZnodeMediaAttribute>();
        }
        #endregion

        #region Public Methods

        //Get a list of AttributeGroup
        public virtual AttributeGroupListModel GetAttributeGroupList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            //set locale Filters if not present
            SetLocaleFilterIfNotPresent(ref filters);

            // set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate AttributeGroupModel list ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<AttributeGroupModel> objStoredProc = new ZnodeViewRepository<AttributeGroupModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            IList<AttributeGroupModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetMediaAttributeGroups @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("AttributeGroupModel list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, list?.Count());
            AttributeGroupListModel attributeGroupListModel = new AttributeGroupListModel();
            attributeGroupListModel.AttributeGroups = list?.Count > 0 ? list?.ToList() : null;

            attributeGroupListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return attributeGroupListModel;
        }

        //Get AttributeGroup
        public virtual AttributeGroupModel GetAttributeGroup(int attributeGroupId)
        {
            ZnodeLogging.LogMessage("Execution started. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("attributeGroupId:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, attributeGroupId);

            if (attributeGroupId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorAttributeGroupIdGreaterThanOne);

            AttributeGroupModel attributeGroupModel = _attributeGroupRepository.GetById(attributeGroupId)?.ToModel<AttributeGroupModel>();

            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(new FilterTuple(ZnodeMediaAttributeGroupEnum.MediaAttributeGroupId.ToString(), ProcedureFilterOperators.Equals, attributeGroupId.ToString()));


            //gets the where clause.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());

            ZnodeLogging.LogMessage("whereClauseModel to generate ZnodeMediaAttributeGroupLocale list ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, whereClauseModel);
            IList<ZnodeMediaAttributeGroupLocale> list = _attributeGroupLocaleRepository.GetEntityList(whereClauseModel.WhereClause);

            ZnodeLogging.LogMessage("ZnodeMediaAttributeGroupLocale list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, list?.Count());

            if (IsNotNull(attributeGroupModel))
                attributeGroupModel.GroupLocaleListModel = list?.Count > 0 ? list.ToModel<AttributeGroupLocaleModel>().ToList() : null;
            ZnodeLogging.LogMessage("Executed. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return attributeGroupModel;
        }

        //Create New AttributeGroup
        public virtual AttributeGroupModel CreateAttributeGroup(AttributeGroupModel model)
        {
            ZnodeLogging.LogMessage("Execution started. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorAttributeGroupModelNull);

            if (IsAttributeGroupExist(model.GroupCode))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.GroupAlreadyExists);

            AttributeGroupModel attributeGroup = _attributeGroupRepository.Insert(model.ToEntity<ZnodeMediaAttributeGroup>())?.ToModel<AttributeGroupModel>();
            ZnodeLogging.LogMessage("Inserted AttributeGroup with code: ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, attributeGroup?.GroupCode);
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessCreateMediaAttributeGroup, model.GroupCode), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);

            GetLocaleListByDeafaultId(model);
            if (model?.GroupLocaleListModel?.Count() > 0)
            {
                foreach (AttributeGroupLocaleModel groupLocaleModel in model.GroupLocaleListModel)
                {
                    if(IsNotNull(attributeGroup))
                    {
                        groupLocaleModel.MediaAttributeGroupId = attributeGroup.MediaAttributeGroupId;
                        attributeGroup.GroupLocaleListModel.Add(groupLocaleModel);
                    }
                }
                
                _attributeGroupLocaleRepository.Insert(model.GroupLocaleListModel.ToEntity<ZnodeMediaAttributeGroupLocale>().ToList());
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessInsertMediaAttributeLocales, model.MediaAttributeGroupId), ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Executed. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return attributeGroup;
        }

        //Update AttributeGroup 
        public virtual bool UpdateAttributeGroup(AttributeGroupModel model)
        {
            ZnodeLogging.LogMessage("Execution started. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorAttributeGroupModelNull);

            status = !Equals(_attributeGroupRepository.Update(model.ToEntity<ZnodeMediaAttributeGroup>()), null);

            if (model.GroupLocaleListModel?.Count > 0)
            {
                GetLocaleListByDeafaultId(model);
                _attributeGroupLocaleRepository.Delete(GetWhereClauseForAttributeGroupId(model.MediaAttributeGroupId).WhereClause);

                return !Equals(_attributeGroupLocaleRepository.Insert(model.GroupLocaleListModel.ToEntity<ZnodeMediaAttributeGroupLocale>().ToList()), null);
            }
            else
                return status;
        }

        //Delete AttributeGroup
        public virtual bool DeleteAttributeGroup(ParameterModel attributeGroupIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (Equals(attributeGroupIds, null) || string.IsNullOrEmpty(attributeGroupIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAttributeGroupIDNull);

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("MediaAttributeGroupIds", attributeGroupIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteMediaGroups @MediaAttributeGroupIds, @Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, deleteResult?.Count());
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteAttributeGroup, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, PIM_Resources.ErrorDeleteAttributeGroup);
            }
        }

        //Saves list of attributes against an AttributeGroup.
        public virtual AttributeGroupMapperListModel AssignAttributes(AttributeGroupMapperListModel attributeGroupMapperList)
        {
            ZnodeLogging.LogMessage("Execution started. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            if (attributeGroupMapperList?.AttributeGroupMappers?.Count > 0)
            {
                //Saves Attributes against AttributeGroup.
                IEnumerable<ZnodeMediaAttributeGroupMapper> attributeGroupMappers = _attributeGroupMapperRepository.Insert(attributeGroupMapperList.AttributeGroupMappers.ToEntity<ZnodeMediaAttributeGroupMapper>().ToList());
                return attributeGroupMappers?.Count() > 0 ? new AttributeGroupMapperListModel() { AttributeGroupMappers = attributeGroupMappers.ToModel<AttributeGroupMapperModel>().ToList() } : null;
            }
            throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);
        }

        //Gets the list of assigned attributes.
        public virtual AttributeGroupMapperListModel GetAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            // set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate attributeTypeList list ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Gets paged list if paging is present.
            _attributeGroupMapperRepository.EnableDisableLazyLoading = true;

            IList<ZnodeMediaAttributeGroupMapper> assignedAttributes = _attributeGroupMapperRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), null, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            AttributeGroupMapperListModel listModel = AttributeGroupMap.ToListModel(assignedAttributes);
            List<ZnodeAttributeType> attributeTypeList = _attributeTypeRepository.GetEntityList(string.Empty)?.ToList();
            ZnodeLogging.LogMessage("attributeTypeList list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, new { assignedAttributesCount= assignedAttributes?.Count(), attributeTypeListCount= attributeTypeList?.Count() });
            listModel.AttributeGroupMappers.ForEach(item => item.Attribute.AttributeTypeName = attributeTypeList?.Where(x => x.AttributeTypeId == item.Attribute.AttributeTypeId)?.FirstOrDefault()?.AttributeTypeName ?? string.Empty);
            listModel.AttributeGroupMappers = listModel.AttributeGroupMappers.OrderBy(x => x.AttributeDisplayOrder).ToList();

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Update attribute group mapper.
        public virtual bool UpdateAttributeGroupMapper(AttributeGroupMapperModel model)
        {
            ZnodeLogging.LogMessage("Execution started. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter AttributeGroupMapperModel having MediaAttributeGroupMapperId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { model?.MediaAttributeGroupMapperId });
            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ErrorAttributeGroupMapperModelNull);
            if (model.MediaAttributeGroupMapperId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne,Admin_Resources.ErrorAttributeGroupMapperIdLessThanOne);

            bool status = false;
            if (model?.MediaAttributeGroupMapperId > 0)
            {
                status = _attributeGroupMapperRepository.Update(model.ToEntity<ZnodeMediaAttributeGroupMapper>());
                if (status)
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessUpdateAttributeGroupMapper, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
                else
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorUpdateAttributeGroupMapper, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                    throw new ZnodeException(ErrorCodes.AssociationUpdateError, Admin_Resources.ErrorUpdateAttributeGroupMapper);
                }
            }
            ZnodeLogging.LogMessage("Executed. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return status;
        }

        public virtual bool DeleteAssociatedAttribute(int attributeGroupMapperId)
        {
            ZnodeLogging.LogMessage("Execution started. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter attributeGroupMapperId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { attributeGroupMapperId });
            if (Equals(attributeGroupMapperId, 0))
                throw new ZnodeException(ErrorCodes.IdLessThanOne,Admin_Resources.ErrorAttributeGroupMapperIdLessThanOne);

            FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodeMediaAttributeGroupMapperEnum.MediaAttributeGroupMapperId.ToString(), ProcedureFilterOperators.Equals, attributeGroupMapperId.ToString()) };

            //gets the where clause.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel generated", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, whereClauseModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return _attributeGroupMapperRepository.Delete(whereClauseModel.WhereClause);
        }

        public virtual AttributeGroupLocaleListModel GetAttributeGroupLocale(int attributeGroupId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter attributeGroupId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { attributeGroupId });
            IList<ZnodeMediaAttributeGroupLocale> groupLocales = _attributeGroupLocaleRepository.GetEntityList(GetWhereClauseForAttributeGroupId(attributeGroupId).WhereClause, GetExpands(expands));
            ZnodeLogging.LogMessage("groupLocales list count:", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, groupLocales?.Count());
            return groupLocales?.Count > 0 ? new AttributeGroupLocaleListModel() { AttributeGroupLocales = groupLocales.ToModel<AttributeGroupLocaleModel>().ToList() } : null;
        }

        //Gets the list of AttributeGroups.
        public virtual AttributesListDataModel GetUnAssignedAttributes(int attributeGroupId, NameValueCollection expands, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            // set paging parameters.
            PageListModel pageListModel = new PageListModel(null, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate assignedAttributes list ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<ZnodeMediaAttributeGroupMapper> assignedAttributes = _attributeGroupMapperRepository.GetPagedList(null, pageListModel.OrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            IList<ZnodeMediaAttribute> attributesList = _attributeRepository.GetEntityList(string.Empty, GetMediaAttributeLocaleExpand());
            IList<ZnodeMediaAttribute> unAssignedAttributeGroups = (from attribute in attributesList
                                                                    where ((!assignedAttributes?.Any(x => x.MediaAttributeId == attribute.MediaAttributeId)).GetValueOrDefault() && !attribute.IsSystemDefined)
                                                                    select attribute)?.ToList();
            ZnodeLogging.LogMessage("unAssignedAttributeGroups list count:", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Verbose,new { assignedAttributesCount=assignedAttributes?.Count(), attributesListCount=attributesList?.Count(), unAssignedAttributeGroupsCount= unAssignedAttributeGroups?.Count() });

            AttributesListDataModel listModel = AttributeGroupMap.AddGroupNameToListModel(unAssignedAttributeGroups);

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed. ", ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Info);
            return listModel;
        }


        #endregion

        #region Private Methods

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (expands != null && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ExpandKeys.GroupLocale)) SetExpands(ZnodeMediaAttributeGroupEnum.ZnodeMediaAttributeGroupLocales.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.GroupMappers)) SetExpands(ZnodeMediaAttributeGroupEnum.ZnodeMediaAttributeGroupMappers.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.MediaAttribute)) SetExpands(ZnodeMediaAttributeGroupMapperEnum.ZnodeMediaAttribute.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.AttributeLocale)) SetExpands(ZnodeMediaAttributeEnum.ZnodeMediaAttributeLocales.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Checks if attribute group already exists or not.
        private bool IsAttributeGroupExist(string groupCode) => _attributeGroupRepository.Table.Any(g => Equals(g.GroupCode, groupCode));

        //Gets the where clause for attribute group id.
        private EntityWhereClauseModel GetWhereClauseForAttributeGroupId(int attributeGroupId)
        {

            ZnodeLogging.LogMessage("Input Parameter attributeGroupId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { attributeGroupId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeMediaAttributeGroupEnum.MediaAttributeGroupId.ToString(), ProcedureFilterOperators.Equals, attributeGroupId.ToString()));
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
        }

        //Get locale list by default id. 
        private AttributeGroupModel GetLocaleListByDeafaultId(AttributeGroupModel localeList)
        {
            int defaultLocaleId = GetDefaultLocaleId();

            ZnodeLogging.LogMessage("defaultLocaleId:", ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Verbose, new object[] { defaultLocaleId });
            //If locale name is present then group code will be save as default locale name.
            if (localeList.GroupLocaleListModel.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.AttributeGroupName.Trim())))
                localeList.GroupLocaleListModel.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.AttributeGroupName = localeList.GroupCode);
            localeList.GroupLocaleListModel.RemoveAll(x => x.AttributeGroupName == string.Empty);

            return localeList;
        }

        //Get media attribute locale expands.
        private List<string> GetMediaAttributeLocaleExpand()
        {
            NameValueCollection attributeExpands = new NameValueCollection();
            attributeExpands.Add(ExpandKeys.AttributeLocale, ExpandKeys.AttributeLocale);
            List<string> navigationProperties = GetExpands(attributeExpands);
            return navigationProperties;
        }
        #endregion
    }
}
