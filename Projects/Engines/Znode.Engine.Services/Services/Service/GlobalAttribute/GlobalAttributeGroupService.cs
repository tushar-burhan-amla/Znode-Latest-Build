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
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class GlobalAttributeGroupService : BaseService, IGlobalAttributeGroupService
    {
        #region Private variables
        protected readonly IZnodeRepository<ZnodeGlobalAttributeGroup> _globalAttributeGroupRepository;
        protected readonly IZnodeRepository<ZnodeGlobalAttributeGroupLocale> _globalAttributeGroupLocaleRepository;
        protected readonly IZnodeRepository<ZnodeGlobalAttributeGroupMapper> _globalAttributeGroupMapperRepository;
        protected readonly IZnodeRepository<ZnodeGlobalAttribute> _globalAttributeRepository;
        protected readonly IZnodeRepository<ZnodeAttributeType> _attributeTypeRepository;
        protected readonly IZnodeRepository<ZnodeGlobalEntity> _globalEntityRepository;
        #endregion

        #region Public Constructor
        public GlobalAttributeGroupService()
        {
            _globalAttributeGroupRepository = new ZnodeRepository<ZnodeGlobalAttributeGroup>();
            _globalAttributeGroupLocaleRepository = new ZnodeRepository<ZnodeGlobalAttributeGroupLocale>();
            _globalAttributeGroupMapperRepository = new ZnodeRepository<ZnodeGlobalAttributeGroupMapper>();
            _globalAttributeRepository = new ZnodeRepository<ZnodeGlobalAttribute>();
            _attributeTypeRepository = new ZnodeRepository<ZnodeAttributeType>();
            _globalEntityRepository = new ZnodeRepository<ZnodeGlobalEntity>();
        }
        #endregion

        #region Public Methods

        //Gets the list of Global Attribute Groups.
        public virtual GlobalAttributeGroupListModel GetAttributeGroupList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //set locale Filters if not present
            SetLocaleFilterIfNotPresent(ref filters);

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            IZnodeViewRepository<GlobalAttributeGroupModel> objStoredProc = new ZnodeViewRepository<GlobalAttributeGroupModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<GlobalAttributeGroupModel> attributeGroups = objStoredProc.ExecuteStoredProcedureList("Znode_GetGlobalAttributeGroups @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("attributeGroups count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { attributeGroups?.Count });

            GlobalAttributeGroupListModel attributeGroupListModel = new GlobalAttributeGroupListModel();
            attributeGroupListModel.AttributeGroupList = attributeGroups?.Count > 0 ? attributeGroups?.ToList() : null;

            attributeGroupListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return attributeGroupListModel;
        }

        //Create Global Attribute Group.
        public virtual GlobalAttributeGroupModel Create(GlobalAttributeGroupModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            
            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (IsAttributeGroupExist(model.GroupCode))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.GlobalAttributeGroupAlreadyExist);

            if (!ValidateGlobalEntity(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidGlobalEntity);

            ZnodeLogging.LogMessage("GlobalAttributeGroupModel with GlobalAttributeGroupId and  AttributeGroupName", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose,new object[] { model?.GlobalAttributeGroupId, model?.AttributeGroupName });

            ZnodeGlobalAttributeGroup createdGroup = _globalAttributeGroupRepository.Insert(model.ToEntity<ZnodeGlobalAttributeGroup>());
            if (createdGroup?.GlobalAttributeGroupId > 0)
            {
                model.GlobalAttributeGroupId = createdGroup.GlobalAttributeGroupId;
                CreateAttributeGroupLocales(model);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessAttributeGroupCreate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            }
            else
                ZnodeLogging.LogMessage(Admin_Resources.ErrorAttributeGroupCreate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return model;
        }

        //Update Global Attribute Group.
        public virtual bool Update(GlobalAttributeGroupModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model.GlobalAttributeGroupId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            if (!ValidateGlobalEntity(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidGlobalEntity);

            ZnodeLogging.LogMessage("Global Attribute Group with GlobalAttributeGroupId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { model?.GlobalAttributeGroupId });

            //Update Global Attribute Group
            bool attributeGroupUpdated = _globalAttributeGroupRepository.Update(model.ToEntity<ZnodeGlobalAttributeGroup>());

            //Update Global Attribute Group Locale
            if (!Equals(model.AttributeGroupLocales, null))
                SaveAttributeGroupLocales(new GlobalAttributeGroupLocaleListModel()
                {
                    GroupCode = model.GroupCode,
                    AttributeGroupLocales = model.AttributeGroupLocales,
                });

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return attributeGroupUpdated;
        }

        //Associate Attributes to Group.
        public virtual GlobalAttributeGroupMapperListModel AssociateAttributes(GlobalAttributeGroupMapperListModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (model?.AttributeGroupMappers?.Count > 0)
            {
                //Saves Attributes against AttributeGroup.
                IEnumerable<ZnodeGlobalAttributeGroupMapper> entity = _globalAttributeGroupMapperRepository.Insert(model.AttributeGroupMappers.ToEntity<ZnodeGlobalAttributeGroupMapper>().ToList());
                GlobalAttributeGroupMapperListModel listModel = new GlobalAttributeGroupMapperListModel { AttributeGroupMappers = entity?.ToModel<GlobalAttributeGroupMapperModel>().ToList() ?? null };
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return listModel;
            }
            else
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.TextInvalidData);
        }

        //Update Attribute Display Order
        public virtual bool UpdateAttributeDisplayOrder(GlobalAttributeDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            bool isUpdated = false;

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model?.AttributeModel?.GlobalAttributeId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            ZnodeLogging.LogMessage("Global Attribute Group with GlobalAttributeId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { model?.AttributeModel?.GlobalAttributeId });

            GlobalAttributeModel attributeModel = _globalAttributeRepository.GetById(model.AttributeModel.GlobalAttributeId)?.ToModel<GlobalAttributeModel>();

            if (Equals(model.AttributeModel.DisplayOrder, attributeModel?.DisplayOrder))
                return true;

            if (IsNotNull(attributeModel))
            {
                attributeModel.DisplayOrder = model?.AttributeModel?.DisplayOrder;
                isUpdated = _globalAttributeRepository.Update(attributeModel?.ToEntity<ZnodeGlobalAttribute>());
                ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.SuccessAttributeUpdate : Admin_Resources.ErrorAttributeUpdate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            }
            return isUpdated;
        }

        //Remove associated attribute from group.
        public virtual bool RemoveAssociatedAttributes(RemoveGroupAttributesModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (Equals(model, null) || string.IsNullOrEmpty(model.GlobalAttributeIds) || model.GlobalAttributeGroupId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelValid);

            ZnodeLogging.LogMessage("RemoveGroupAttributesModel with GlobalAttributeIds and GlobalAttributeGroupId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { model?.GlobalAttributeIds,model?.GlobalAttributeGroupId });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeGlobalAttributeGroupMapperEnum.GlobalAttributeId.ToString(), model.GlobalAttributeIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeGlobalAttributeGroupMapperEnum.GlobalAttributeGroupId.ToString(), model.GlobalAttributeGroupId, ParameterDirection.Input, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UnassignGlobalAttributeFromGroup @GlobalAttributeId,@GlobalAttributeGroupId");
            ZnodeLogging.LogMessage("deleteResult count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { deleteResult });
      
            if (deleteResult.FirstOrDefault().Status.Value)
                ZnodeLogging.LogMessage(Admin_Resources.SuccessAttributegroupMapperDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorAttributegroupMapperDelete);
            }
            return deleteResult.FirstOrDefault().Status.Value;
        }

        //Delete Global attribute group.
        public virtual bool Delete(ParameterModel attributeGroupIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (Equals(attributeGroupIds, null) || string.IsNullOrEmpty(attributeGroupIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorAttributeGroupIdNull);
            ZnodeLogging.LogMessage("attributeGroupIds to be deleted", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { attributeGroupIds });

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("GlobalAttributeGroupIds", attributeGroupIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("ZNode_DeleteGlobalAttributeGroup @GlobalAttributeGroupIds,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { deleteResult });

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessAttributeGroupDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorDeleteAttributeGroup);
            }

        }

        //Gets Global AttributeGroup by ID.
        public virtual GlobalAttributeGroupModel GetAttributeGroupById(int Id, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (Id < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);
            ZnodeLogging.LogMessage("Get global attribute group by ID", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { Id });

            EntityWhereClauseModel whereClauseModel = GetWhereClauseForAttributeGroupId(Id);
            ZnodeGlobalAttributeGroup entity = _globalAttributeGroupRepository.GetEntity(whereClauseModel.WhereClause, GetExpands(expands), whereClauseModel.FilterValues);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return !Equals(entity, null) ? entity.ToModel<GlobalAttributeGroupModel>() : null;
        }

        //Get attribute group locale By group Id.
        public virtual GlobalAttributeGroupLocaleListModel GetAttributeGroupLocale(int attributeGroupId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            IList<ZnodeGlobalAttributeGroupLocale> groupLocales = _globalAttributeGroupLocaleRepository.GetEntityList(GetWhereClauseForAttributeGroupId(attributeGroupId).WhereClause, GetExpands(expands));

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return groupLocales?.Count > 0 ? new GlobalAttributeGroupLocaleListModel() { AttributeGroupLocales = groupLocales.ToModel<GlobalAttributeGroupLocaleModel>().ToList() } : null;
        }

        //Gets the list of Global AttributeGroups.
        public virtual GlobalAttributeGroupMapperListModel GetAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            List<ZnodeAttributeType> attributeTypeList = _attributeTypeRepository.GetEntityList(string.Empty)?.ToList();
            _globalAttributeGroupMapperRepository.EnableDisableLazyLoading = true;

            IList<ZnodeGlobalAttributeGroupMapper> assignedAttributes = _globalAttributeGroupMapperRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), null, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("assignedAttributes count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { assignedAttributes });

            GlobalAttributeGroupMapperListModel groupMapperList = GlobalAttributeGroupMap.ToListModel(assignedAttributes);

            if (groupMapperList?.AttributeGroupMappers?.Count > 0)
            {
                groupMapperList.AttributeGroupMappers.ForEach(item => item.Attribute.AttributeType = attributeTypeList?.Where(x => x.AttributeTypeId == item.Attribute.AttributeTypeId)?.FirstOrDefault()?.AttributeTypeName ?? string.Empty);
                groupMapperList.AttributeGroupMappers = groupMapperList.AttributeGroupMappers.OrderBy(x => x.AttributeDisplayOrder).ToList();
                groupMapperList.BindPageListModel(pageListModel);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return groupMapperList;
        }

        //Gets the list of Global Attribute Groups.
        public virtual GlobalAttributeListModel GetUnAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {

            int groupId = 0;
            GetValuesFromFilter(out groupId, filters);

            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //gets the order by clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            List<int?> assignedAttributes = _globalAttributeGroupMapperRepository.GetPagedList(whereClauseModel.WhereClause, orderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.Select(x => x.GlobalAttributeId).ToList();
            ZnodeLogging.LogMessage("assignedAttributes :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { assignedAttributes });

            FilterCollection filtersForAttributes = new FilterCollection();

            int entityId = (from globalAttribute in  _globalAttributeGroupRepository.Table.Where(x => x.GlobalAttributeGroupId == groupId)
                           select globalAttribute.GlobalEntityId).FirstOrDefault();

            if (assignedAttributes?.Count > 0)
                filtersForAttributes.Add(ZnodeGlobalAttributeEnum.GlobalAttributeId.ToString(), FilterOperators.NotIn, string.Join(",", assignedAttributes?.Where(o => o.HasValue)));

            int linkAttributeTypeId = _attributeTypeRepository.Table.Where(x => x.AttributeTypeName == "Link").Select(x => x.AttributeTypeId).FirstOrDefault();
            ZnodeLogging.LogMessage("linkAttributeTypeId :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { linkAttributeTypeId });

            filtersForAttributes.Add(ZnodeGlobalAttributeEnum.AttributeTypeId.ToString(), FilterOperators.NotEquals, linkAttributeTypeId.ToString());
            filtersForAttributes.Add(ZnodeGlobalAttributeEnum.GlobalEntityId.ToString(), FilterOperators.Equals, entityId.ToString());

            EntityWhereClauseModel whereClauseModelForAttributes = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersForAttributes.ToFilterDataCollection());
            IList<ZnodeGlobalAttribute> attributesList = _globalAttributeRepository.GetEntityList(whereClauseModelForAttributes.WhereClause, null, GetExpands(expands), whereClauseModelForAttributes.FilterValues);
            ZnodeLogging.LogMessage("attributesList count :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { attributesList?.Count });

            GlobalAttributeListModel listModel = GlobalAttributeMap.ToListModel(attributesList);

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listModel;
        }

        //Create Global Attribute Group Locales .
        public virtual GlobalAttributeGroupLocaleListModel SaveAttributeGroupLocales(GlobalAttributeGroupLocaleListModel listModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (IsNull(listModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (listModel.AttributeGroupLocales?.Count > 0)
            {
                int defaultLocaleId = GetDefaultLocaleId();

                //If locale name is present then group code will be save as default locale name.
                if (listModel.AttributeGroupLocales.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.AttributeGroupName.Trim())))
                    listModel.AttributeGroupLocales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.AttributeGroupName = listModel.GroupCode);

                listModel.AttributeGroupLocales.RemoveAll(x => x.AttributeGroupName == string.Empty);

                _globalAttributeGroupLocaleRepository.Delete(GetWhereClauseForAttributeGroupId(listModel.AttributeGroupLocales.FirstOrDefault().GlobalAttributeGroupId).WhereClause);

                IEnumerable<ZnodeGlobalAttributeGroupLocale> entity = _globalAttributeGroupLocaleRepository.Insert(listModel.AttributeGroupLocales.ToEntity<ZnodeGlobalAttributeGroupLocale>().ToList());

                listModel.AttributeGroupLocales = entity.ToModel<GlobalAttributeGroupLocaleModel>().ToList();
                ZnodeLogging.LogMessage("AttributeGroupLocales count :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { listModel?.AttributeGroupLocales?.Count });

            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listModel;
        }

        #endregion

        #region Private Method

        //Checks if attribute group already exists or not.
        private bool IsAttributeGroupExist(string groupCode)
            => _globalAttributeGroupRepository.Table.Any(x => x.GroupCode == groupCode);

        //Insert group locales.
        private void CreateAttributeGroupLocales(GlobalAttributeGroupModel model)
        {
            bool isCreated = false;
            GlobalAttributeGroupModel attributeGroup = GetLocaleByDefaultLocaleId(model);
            ZnodeLogging.LogMessage("GlobalAttributeGroupModel with AttributeGroupName:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] {model.AttributeGroupName });

            int attributeGroupId = attributeGroup.GlobalAttributeGroupId;
            ZnodeLogging.LogMessage("attributeGroupId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { attributeGroupId });

            if (attributeGroup?.AttributeGroupLocales?.Count > 0)
            {
                attributeGroup.AttributeGroupLocales.ForEach(x => x.GlobalAttributeGroupId = attributeGroupId);
                isCreated = !Equals(_globalAttributeGroupLocaleRepository.Insert(attributeGroup.AttributeGroupLocales.ToEntity<ZnodeGlobalAttributeGroupLocale>().ToList()), null);
            }
            ZnodeLogging.LogMessage(isCreated ? Admin_Resources.SuccessAttributegroupLocaleCreate :Admin_Resources.ErrorAttributegroupLocaleCreate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Get group locales by default localeid.
        private GlobalAttributeGroupModel GetLocaleByDefaultLocaleId(GlobalAttributeGroupModel model)
        {
            int defaultLocaleId = GetDefaultLocaleId();

            if (model.AttributeGroupLocales.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.AttributeGroupName.Trim())))
                model.AttributeGroupLocales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.AttributeGroupName = model.GroupCode);

            model.AttributeGroupLocales.RemoveAll(x => x.AttributeGroupName == string.Empty);
            return model;
        }

        //Gets the where clause for global attribute group id.
        private EntityWhereClauseModel GetWhereClauseForAttributeGroupId(int attributeGroupId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeGlobalAttributeGroupEnum.GlobalAttributeGroupId.ToString(), ProcedureFilterOperators.Equals, attributeGroupId.ToString()));
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
        }

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (!Equals(expands, null) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    //Add expand keys
                    if (Equals(key, ExpandKeys.GroupLocale)) SetExpands(ZnodeGlobalAttributeGroupEnum.ZnodeGlobalAttributeGroupLocales.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.GroupMappers)) SetExpands(ZnodeGlobalAttributeGroupEnum.ZnodeGlobalAttributeGroupMappers.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.GlobalAttribute)) SetExpands(ZnodeGlobalAttributeGroupMapperEnum.ZnodeGlobalAttribute.ToString(), navigationProperties);
                    if (Equals(key, ZnodeGlobalAttributeEnum.ZnodeGlobalAttributeLocales.ToString().ToLower())) SetExpands(ZnodeGlobalAttributeEnum.ZnodeGlobalAttributeLocales.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        private void GetValuesFromFilter(out int groupId, FilterCollection filters)
        {
            groupId = 0;
            if (filters.Any(x => string.Equals(x.FilterName, Constants.FilterKeys.GlobalAttributeGroupId, StringComparison.CurrentCultureIgnoreCase)));
            {
                int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(Constants.FilterKeys.GlobalAttributeGroupId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out groupId);
                filters.RemoveAll(x => x.FilterName.Equals(Constants.FilterKeys.GlobalAttributeGroupId, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        private bool ValidateGlobalEntity(GlobalAttributeGroupModel globalAttributeGroupModel)
        {
            ZnodeGlobalEntity model = _globalEntityRepository.Table.FirstOrDefault(x => x.EntityName.ToLower() == globalAttributeGroupModel.EntityName.ToLower());

            if (HelperUtility.IsNotNull(model))
            {
                globalAttributeGroupModel.GlobalEntityId = model.GlobalEntityId;
                return true;
            }

            return false;
        }
        #endregion
    }
}
