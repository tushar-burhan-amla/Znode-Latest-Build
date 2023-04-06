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

namespace Znode.Engine.Services
{
    public class GlobalAttributeFamilyService : BaseService, IGlobalAttributeFamilyService
    {
        #region Private variables
        protected readonly IZnodeRepository<ZnodeGlobalAttributeFamily> _globalAttributeFamilyRepository;
        protected readonly IZnodeRepository<ZnodeGlobalAttributeFamilyLocale> _globalAttributeFamilyLocaleRepository;
        protected readonly IZnodeRepository<ZnodeGlobalFamilyGroupMapper> _globalFamilyGroupMapperRepository;
        protected readonly IZnodeRepository<ZnodeGlobalAttributeGroup> _globalAttributeGroupRepository;
        protected readonly IZnodeRepository<ZnodeGlobalEntity> _globalEntityRepository;


        #endregion

        #region Public Constructor
        public GlobalAttributeFamilyService()
        {
            _globalAttributeFamilyRepository = new ZnodeRepository<ZnodeGlobalAttributeFamily>();
            _globalAttributeFamilyLocaleRepository = new ZnodeRepository<ZnodeGlobalAttributeFamilyLocale>();
            _globalFamilyGroupMapperRepository = new ZnodeRepository<ZnodeGlobalFamilyGroupMapper>();
            _globalAttributeGroupRepository = new ZnodeRepository<ZnodeGlobalAttributeGroup>();
            _globalEntityRepository = new ZnodeRepository<ZnodeGlobalEntity>();


        }
        #endregion

        #region Public Methods
        //Gets the List of Attribute Family
        public virtual GlobalAttributeFamilyListModel List(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //set locale Filters if not present
            SetLocaleFilterIfNotPresent(ref filters);

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<GlobalAttributeFamilyModel> objStoredProc = new ZnodeViewRepository<GlobalAttributeFamilyModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<GlobalAttributeFamilyModel> attributeFamilies = objStoredProc.ExecuteStoredProcedureList("Znode_GetGlobalAttributeFamilies @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);

            GlobalAttributeFamilyListModel attributeFamilyListModel = new GlobalAttributeFamilyListModel();
            attributeFamilyListModel.AttributeFamilyList = attributeFamilies?.Count > 0 ? attributeFamilies?.ToList() : null;

            attributeFamilyListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return attributeFamilyListModel;
        }

        //Create a new Attribute Family
        public virtual GlobalAttributeFamilyModel Create(GlobalAttributeFamilyCreateModel model)
        {

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (IsAttributeFamilyExist(model.FamilyCode))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.GlobalAttributeGroupAlreadyExist);

            if (!ValidateGlobalEntity(model, true))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidGlobalEntity);

            GlobalAttributeFamilyModel attributeFamily = _globalAttributeFamilyRepository.Insert(model.ToEntity<ZnodeGlobalAttributeFamily>()).ToModel<GlobalAttributeFamilyModel>();
            if (attributeFamily?.GlobalAttributeFamilyId > 0)
            {
                attributeFamily.AttributeFamilyLocales = model.AttributeFamilyLocales;
                attributeFamily.EntityName = model.GlobalEntityName;
                SaveAttributeFamilyDetails(attributeFamily);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessAttributeFamilyCreate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            }
            else
                ZnodeLogging.LogMessage(Admin_Resources.FailureAttributeFamilyCreate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return attributeFamily;

        }

        //Update Global Attribute Family
        public virtual GlobalAttributeFamilyModel Update(GlobalAttributeFamilyUpdateModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            GlobalAttributeFamilyModel globalAttributeFamily = _globalAttributeFamilyRepository.Table.FirstOrDefault(x => x.FamilyCode.ToLower() == model.FamilyCode.ToLower())?.ToModel<GlobalAttributeFamilyModel>();

            if (Equals(globalAttributeFamily, null))
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.InvalidFamilyCode);

            if (!Equals(model.AttributeFamilyLocales, null))
            {
                globalAttributeFamily.AttributeFamilyLocales = model.AttributeFamilyLocales;
                SaveAttributeFamilyDetails(globalAttributeFamily);
            }

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return globalAttributeFamily;
        }

        //Get the Attribute Family
        public virtual GlobalAttributeFamilyModel GetAttributeFamily(string familyCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            ZnodeGlobalAttributeFamily model = _globalAttributeFamilyRepository.Table.FirstOrDefault(x => x.FamilyCode.ToLower() == familyCode.ToLower());

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.CodeNotExists);

            return model.ToModel<GlobalAttributeFamilyModel>();
        }


        //Delete Global Attribute Family
        public virtual bool Delete(ParameterModel globalAttributeFamilyIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (Equals(globalAttributeFamilyIds, null) || string.IsNullOrEmpty(globalAttributeFamilyIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdsCanNotEmpty);

            return DeleteAttributeFamily(globalAttributeFamilyIds.Ids);

        }

        //Delete Family By code
        public virtual bool DeleteFamilyByCode(string familyCode)
        {
            if (string.IsNullOrEmpty(familyCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidFamilyCode);

            int attributeFamilyId = GetAttributeFamilyId(familyCode);

            return DeleteAttributeFamily(Convert.ToString(attributeFamilyId));
        }

        //Returns the List of Groups that are associated to a Family
        public virtual GlobalAttributeGroupListModel GetAssignedAttributeGroups(string familyCode)
        {
            GlobalAttributeGroupListModel listModel = new GlobalAttributeGroupListModel();

            if (string.IsNullOrEmpty(familyCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidFamilyCode);

            IList<ZnodeGlobalFamilyGroupMapper> groupEntityMapper = AssociatedGroupsList(familyCode);

            foreach (var groupEntity in groupEntityMapper)
            {
                ZnodeGlobalAttributeGroup attributeGroup = groupEntity.ZnodeGlobalAttributeGroup;
                if (HelperUtility.IsNotNull(attributeGroup))
                {
                    GlobalAttributeGroupModel attributeGroupModel = attributeGroup?.ToModel<GlobalAttributeGroupModel>();
                    if (HelperUtility.IsNotNull(attributeGroupModel))
                        attributeGroupModel.DisplayOrder = groupEntity.AttributeGroupDisplayOrder;
                    listModel.AttributeGroupList.Add(attributeGroupModel);
                }
            }

            listModel.AttributeGroupList = listModel.AttributeGroupList?.OrderBy(x => x.DisplayOrder).ToList();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listModel;

        }

        // Returns the List of Unassigned groups of a Family
        public virtual GlobalAttributeGroupListModel GetUnassignedAttributeGroups(string familyCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(familyCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidFamilyCode);

            int attributeFamilyId = GetAttributeFamilyId(familyCode);

            if (attributeFamilyId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            List<int> assignedGroups = (from mapperRepository in _globalFamilyGroupMapperRepository.Table.Where(x => x.GlobalAttributeFamilyId == attributeFamilyId)
                                        select mapperRepository.GlobalAttributeGroupId).ToList();

            IList<ZnodeGlobalAttributeGroup> attributeGroupList = UnassociatedGroupsList(attributeFamilyId, assignedGroups);
            GlobalAttributeGroupListModel listModel = GlobalAttributeMap.AddGroupNameToListModel(attributeGroupList);

            return listModel;
        }


        // Assign Attribute Groups to a Family
        public virtual bool AssignAttributeGroups(string attributeGroupIds, string familyCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(familyCode) || string.IsNullOrEmpty(attributeGroupIds))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            return AssignGroupsToFamily(attributeGroupIds, familyCode);
        }



        // Assign Attribute Groups to a Family
        public virtual bool AssignAttributeGroupsByGroupCode(string groupCode, string familyCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(familyCode) || string.IsNullOrEmpty(groupCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            int attributeGroupId = GetAttributeGroupId(groupCode);

            return AssignGroupsToFamily(Convert.ToString(attributeGroupId), familyCode);
        }


        //Unassign attribute Groups from a family
        public virtual bool UnassignAttributeGroups(string groupCode, string familyCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(familyCode) && string.IsNullOrEmpty(groupCode))
                throw new ZnodeException( ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            int attributeFamilyId = GetAttributeFamilyId(familyCode);
            int attributeGroupId = GetAttributeGroupId(groupCode);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("GlobalAttributeGroupId", attributeGroupId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("GlobalAttributeFamilyId", attributeFamilyId, ParameterDirection.Input, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UnassociateGroupFromFamily @GlobalAttributeGroupId,@GlobalAttributeFamilyId");
            ZnodeLogging.LogMessage("deleteResult count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { deleteResult.Count });

            if (deleteResult.FirstOrDefault().Status.Value)
                return true;
            else
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorUnassociateGroup);
        }

        //Update attribute group display order.
        public virtual bool UpdateAttributeGroupDisplayOrder(string groupCode, string familyCode, int displayOrder)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            bool status = false;

            if (displayOrder < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidDisplayOrder);

            if (string.IsNullOrEmpty(familyCode) && string.IsNullOrEmpty(groupCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            int attributeFamilyId = GetAttributeFamilyId(familyCode);
            int attributeGroupId = GetAttributeGroupId(groupCode);
            List<ZnodeGlobalAttributeGroup> groupList = new List<ZnodeGlobalAttributeGroup>
            {
                new ZnodeGlobalAttributeGroup
                {
                    GlobalAttributeGroupId = attributeGroupId,
                    DisplayOrder =displayOrder
                }
            };
            status = SaveOrUpdateGroupMapper(groupList, attributeFamilyId, ZnodeConstant.UpdateData);

            return status;
        }


        //Get attribute Family locale By group Id.
        public virtual GlobalAttributeFamilyLocaleListModel GetAttributeFamilyLocale(string familyCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(familyCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidFamilyCode);

            int attributeFamilyId = GetAttributeFamilyId(familyCode);

            IList<ZnodeGlobalAttributeFamilyLocale> groupLocales = _globalAttributeFamilyLocaleRepository.Table.Where(x => x.GlobalAttributeFamilyId == attributeFamilyId)?.ToList();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return groupLocales?.Count > 0 ? new GlobalAttributeFamilyLocaleListModel() { AttributeFamilyLocales = groupLocales.ToModel<GlobalAttributeFamilyLocaleModel>().ToList() } : null;
        }

        //Get Expands
        public virtual List<string> GetExpands(NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            List<string> navigationProperties = new List<string>();
            if (!Equals(expands, null) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ExpandKeys.GlobalAttributeGroup)) SetExpands(ZnodeGlobalAttributeGroupMapperEnum.ZnodeGlobalAttributeGroup.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.GroupMappers)) SetExpands(ZnodeGlobalAttributeGroupEnum.ZnodeGlobalAttributeGroupMappers.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.GlobalAttributeGroupLocale)) SetExpands(ZnodeGlobalAttributeGroupEnum.ZnodeGlobalAttributeGroupLocales.ToString(), navigationProperties);
                }
            }
   
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return navigationProperties;
        }

        //Associate Group List
        public virtual IList<ZnodeGlobalFamilyGroupMapper> AssociatedGroupsList(string familyCode)
        {
            NameValueCollection expands = new NameValueCollection();
            FilterCollection filters = new FilterCollection();

            int attributeFamilyId = GetAttributeFamilyId(familyCode);

            expands.Add(ExpandKeys.GlobalAttributeGroup, ExpandKeys.GlobalAttributeGroup);
            filters.Add(ZnodeGlobalFamilyGroupMapperEnum.GlobalAttributeFamilyId.ToString(), FilterOperators.Equals, attributeFamilyId.ToString());

            List<string> navigationProperties = GetExpands(expands);

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _globalFamilyGroupMapperRepository.GetEntityList(whereClauseModel.WhereClause, navigationProperties);
        }

        public bool IsAttributeFamilyExist(string familyCode)
        {
            if (string.IsNullOrEmpty(familyCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidFamilyCode);

            return _globalAttributeFamilyRepository.Table.Any(x => x.FamilyCode.ToLower() == familyCode.ToLower());
        }
        

        public virtual IList<ZnodeGlobalAttributeGroup> UnassociatedGroupsList(int attributeFamilyId, List<int> assignedGroups)
        {
            NameValueCollection expands = new NameValueCollection();
            FilterCollection filters = new FilterCollection();

            expands.Add(ExpandKeys.GroupMappers, ExpandKeys.GroupMappers);
            expands.Add(ExpandKeys.GlobalAttributeGroupLocale, ExpandKeys.GlobalAttributeGroupLocale);
            expands.Add(ZnodeGlobalAttributeGroupEnum.ZnodeGlobalAttributeGroupLocales.ToString().ToLower(), ZnodeGlobalAttributeGroupEnum.ZnodeGlobalAttributeGroupLocales.ToString().ToLower());

            int associatedEntityId = (from attributeFamily in _globalAttributeFamilyRepository.Table.Where(x => x.GlobalAttributeFamilyId == attributeFamilyId)
                                       select attributeFamily.GlobalEntityId).FirstOrDefault();

            if (assignedGroups?.Count > 0)
                filters.Add(ZnodeGlobalAttributeGroupEnum.GlobalAttributeGroupId.ToString(), FilterOperators.NotIn, string.Join(",", assignedGroups));

            filters.Add(ZnodeGlobalAttributeGroupEnum.GlobalEntityId.ToString(), FilterOperators.Equals, associatedEntityId.ToString());

            List<string> navigationProperties = GetExpands(expands);

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _globalAttributeGroupRepository.GetEntityList(whereClauseModel.WhereClause, navigationProperties);
        }

        protected virtual bool DeleteAttributeFamily(string attributeIds)
        {
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("GlobalAttributeFamilyIds", attributeIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteGlobalAttributeFamily @GlobalAttributeFamilyIds,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { deleteResult });

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.DeleteMessage, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorFailedToDelete);
            }

        }

        protected virtual bool AssignGroupsToFamily(string attributeGroupIds, string familyCode)
        {
            bool status = false;
            int attributeFamilyId = GetAttributeFamilyId(familyCode);
            List<int> attributegroupIds = new List<int>(Array.ConvertAll(attributeGroupIds.Split(','), int.Parse));

            List<ZnodeGlobalAttributeGroup> groupList = _globalAttributeGroupRepository.Table.Where(x => attributegroupIds.Contains(x.GlobalAttributeGroupId)).ToList();

            if (HelperUtility.IsNotNull(groupList))
            {
                status=SaveOrUpdateGroupMapper(groupList, attributeFamilyId, ZnodeConstant.InsertData);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return status;
        }

        #endregion

        #region Private Methods

        private bool ValidateGlobalEntity(GlobalAttributeFamilyCreateModel globalAttributeFamilymodel, bool isCreate = false)
        {

            if (!string.IsNullOrEmpty(globalAttributeFamilymodel.GlobalEntityName))
            {
                ZnodeGlobalEntity model = _globalEntityRepository.Table.FirstOrDefault(x => x.EntityName.ToLower() == globalAttributeFamilymodel.GlobalEntityName.ToLower());

                if (HelperUtility.IsNotNull(model))
                {
                    globalAttributeFamilymodel.GlobalEntityId = model.GlobalEntityId;
                    if (!isCreate)
                        return true;
                    else
                        return !model.IsFamilyUnique;
                }
            }
            return false;
        }


        private EntityWhereClauseModel GetWhereClause(int attributeFamilyId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeGlobalFamilyGroupMapperEnum.GlobalAttributeFamilyId.ToString(), ProcedureFilterOperators.Equals, attributeFamilyId.ToString()));
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
        }

        private void SaveAttributeFamilyDetails(GlobalAttributeFamilyModel model)
        {
            bool isDataInserted = false;
            GlobalAttributeFamilyModel attributeFamily = GetLocaleByDefaultLocaleId(model);
            if (attributeFamily?.AttributeFamilyLocales?.Count > 0)
            {
                _globalAttributeFamilyLocaleRepository.Delete(GetWhereClause(attributeFamily.GlobalAttributeFamilyId).WhereClause);

                attributeFamily.AttributeFamilyLocales.ForEach(x => x.GlobalAttributeFamilyId = attributeFamily.GlobalAttributeFamilyId);
                attributeFamily.AttributeFamilyLocales = _globalAttributeFamilyLocaleRepository.Insert(attributeFamily.AttributeFamilyLocales.ToEntity<ZnodeGlobalAttributeFamilyLocale>().ToList())?.ToModel<GlobalAttributeFamilyLocaleModel>()?.ToList();
                isDataInserted = !Equals(attributeFamily.AttributeFamilyLocales, null);
            }
            ZnodeLogging.LogMessage(isDataInserted ? Admin_Resources.SuccessAttributegroupLocaleCreate : Admin_Resources.ErrorAttributegroupLocaleCreate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Insert or update Group entity mapper
        protected virtual bool SaveOrUpdateGroupMapper(List<ZnodeGlobalAttributeGroup> groupList,int attributeFamilyId, string action) 
        {
            bool IsSuccess = false;
            List<GlobalAttributeGroupMapperModel> entityList = GetEntityMapperList(groupList, attributeFamilyId);
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("EntiryMapper",HelperUtility.ToXML(entityList),ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("UserId", HelperMethods.GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Action", action, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(View_ReturnBooleanEnum.Status.ToString(), null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateGlobalEntityMapper @EntiryMapper, @UserId,@Action, @Status OUT", 3, out status);
            if (status == 1)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessAssignAttributeGroupsToFamily, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                IsSuccess = true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorAssociateAttributeGroup, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return IsSuccess;
        }

        protected virtual List<GlobalAttributeGroupMapperModel> GetEntityMapperList(List<ZnodeGlobalAttributeGroup> groupList, int attributeFamilyId)
        {
            List<GlobalAttributeGroupMapperModel> entityList = new List<GlobalAttributeGroupMapperModel>();
            foreach (ZnodeGlobalAttributeGroup group in groupList ?? new List<ZnodeGlobalAttributeGroup>())
            {
                GlobalAttributeGroupMapperModel entityMapper = new GlobalAttributeGroupMapperModel
                {
                    GlobalAttributeId = attributeFamilyId,
                    GlobalAttributeGroupId = group.GlobalAttributeGroupId,
                    AttributeDisplayOrder = !Equals(group.DisplayOrder, null) ? group.DisplayOrder : 999
                };
                entityList.Add(entityMapper);
            }
            return entityList;
        }

        private GlobalAttributeFamilyModel GetLocaleByDefaultLocaleId(GlobalAttributeFamilyModel model)
        {
            int defaultLocaleId = GetDefaultLocaleId();

            if (model.AttributeFamilyLocales.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.AttributeFamilyName.Trim())))
                model.AttributeFamilyLocales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.AttributeFamilyName = model.FamilyCode);

            model.AttributeFamilyLocales.RemoveAll(x => x.AttributeFamilyName == string.Empty);
            return model;
        }

        private int GetAttributeFamilyId(string attributeFamilyCode)
        {
            int familyId = (from attributeFamily in _globalAttributeFamilyRepository.Table.Where(x => x.FamilyCode.ToLower() == attributeFamilyCode.ToLower())
                            select attributeFamily.GlobalAttributeFamilyId).FirstOrDefault();

            if (familyId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidFamilyCode);

            return familyId;
        }

        private int GetAttributeGroupId(string attributeGroupCode)
        {
            int groupId = (from attributeGroup in _globalAttributeGroupRepository.Table.Where(x => x.GroupCode.ToLower() == attributeGroupCode.ToLower())
                           select attributeGroup.GlobalAttributeGroupId).FirstOrDefault();

            if (groupId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidGroupCode);

            return groupId;
        }
        #endregion
    }
}
