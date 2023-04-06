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
    public class PIMAttributeGroupService : BaseService, IPIMAttributeGroupService
    {
        #region Private variables
        private readonly IZnodeRepository<ZnodePimAttributeGroup> _pimAttributeGroupRepository;
        private readonly IZnodeRepository<ZnodePimAttributeGroupLocale> _pimAttributeGroupLocaleRepository;
        private readonly IZnodeRepository<ZnodePimAttributeGroupMapper> _pimAttributeGroupMapperRepository;
        private readonly IZnodeRepository<ZnodePimFamilyGroupMapper> _pimFamilyGroupMapperRepository;
        private readonly IZnodeRepository<ZnodePimAttribute> _pimAttributeRepository;
        private readonly IZnodeRepository<ZnodeAttributeType> _attributeTypeRepository;

        private const string _Link = "Link";
        #endregion

        public PIMAttributeGroupService()
        {
            _pimAttributeGroupRepository = new ZnodeRepository<ZnodePimAttributeGroup>();
            _pimAttributeGroupLocaleRepository = new ZnodeRepository<ZnodePimAttributeGroupLocale>();
            _pimAttributeGroupMapperRepository = new ZnodeRepository<ZnodePimAttributeGroupMapper>();
            _pimAttributeRepository = new ZnodeRepository<ZnodePimAttribute>();
            _attributeTypeRepository = new ZnodeRepository<ZnodeAttributeType>();
            _pimFamilyGroupMapperRepository = new ZnodeRepository<ZnodePimFamilyGroupMapper>();
        }

        //Gets the list of PIMAttributeGroups.
        public virtual PIMAttributeGroupListModel GetAttributeGroupList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //set locale Filters if not present
            SetLocaleFilterIfNotPresent(ref filters);
            ResetFilters(filters);

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<PIMAttributeGroupModel> objStoredProc = new ZnodeViewRepository<PIMAttributeGroupModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            ZnodeLogging.LogMessage("pageListModel to get domainListEntity: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<PIMAttributeGroupModel> pimAttributeGroupList = objStoredProc.ExecuteStoredProcedureList("Znode_GetPimAttributeGroups @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("pimAttributeGroupList count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeGroupList?.Count);
            PIMAttributeGroupListModel attributeGroupListModel = new PIMAttributeGroupListModel();
            attributeGroupListModel.AttributeGroupList = pimAttributeGroupList?.Count > 0 ? pimAttributeGroupList?.ToList() : null;

            attributeGroupListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return attributeGroupListModel;
        }

        //Gets the list of PIMAttributeGroups.
        public virtual PIMAttributeGroupMapperListModel GetAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            
            List<ZnodeAttributeType> attributeTypeList = _attributeTypeRepository.GetEntityList(string.Empty)?.ToList();
            _pimAttributeGroupMapperRepository.EnableDisableLazyLoading = true;
            ZnodeLogging.LogMessage("pageListModel to get assignedAttributes list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<ZnodePimAttributeGroupMapper> assignedAttributes = _pimAttributeGroupMapperRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), null, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("attributeTypeList and assignedAttributes list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { AttributeTypeListCount = attributeTypeList?.Count, AssignedAttributesCount = assignedAttributes?.Count });
            PIMAttributeGroupMapperListModel pimAttributeGroupMapperList = PIMAttributeGroupMap.ToListModel(assignedAttributes);
            pimAttributeGroupMapperList.AttributeGroupMappers.ForEach(item => item.Attribute.AttributeType = attributeTypeList?.Where(x => x.AttributeTypeId == item.Attribute.AttributeTypeId)?.FirstOrDefault()?.AttributeTypeName ?? string.Empty);
            pimAttributeGroupMapperList.AttributeGroupMappers = pimAttributeGroupMapperList.AttributeGroupMappers.OrderBy(x => x.AttributeDisplayOrder).ToList();

            pimAttributeGroupMapperList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return pimAttributeGroupMapperList;
        }

        //Gets the list of PIMAttributeGroups.
        public virtual PIMAttributeListModel GetUnAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            Boolean isCategory = Convert.ToBoolean(RemoveIsCategoryFilter(filters));
            int? linkAttributeTypeId = _attributeTypeRepository.Table.FirstOrDefault(x => x.AttributeTypeName == _Link)?.AttributeTypeId;
            ZnodeLogging.LogMessage("linkAttributeTypeId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, linkAttributeTypeId);

            IList<ZnodePimAttribute> attributesList = _pimAttributeRepository.Table.Where(x => !_pimAttributeGroupMapperRepository.Table.
                                                            Select(a => a.PimAttributeId).ToList()
                                                            .Contains(x.PimAttributeId) && !x.IsSystemDefined
                                                             && x.IsCategory == isCategory && !x.IsPersonalizable &&
                                                             x.AttributeTypeId != linkAttributeTypeId).OrderBy(x => x.PimAttributeId).ToList();

            ZnodeLogging.LogMessage("attributesList count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributesList?.Count);
            PIMAttributeListModel listModel = PIMAttributesMap.ToListModel(attributesList);

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Gets PIMAttributeGroup by ID.
        public virtual PIMAttributeGroupModel GetAttributeGroupById(int pimAttributeGroupId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (pimAttributeGroupId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorIDLessThanOne);

            EntityWhereClauseModel whereClauseModel = GetWhereClauseForPIMAttributeGroupId(pimAttributeGroupId);
            ZnodeLogging.LogMessage("WhereClause and pimAttributeGroupId to get PIM attribute group: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { whereClauseModel?.WhereClause, pimAttributeGroupId });
            ZnodePimAttributeGroup entity = _pimAttributeGroupRepository.GetEntity(whereClauseModel.WhereClause, GetExpands(expands), whereClauseModel.FilterValues);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return !Equals(entity, null) ? entity.ToModel<PIMAttributeGroupModel>() : null;
        }

        //Update PIMAttributeGroup.
        public virtual bool UpdateAttributeGroup(PIMAttributeGroupModel pimAttributeGroupModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (Equals(pimAttributeGroupModel, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            if (pimAttributeGroupModel.PimAttributeGroupId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorIDLessThanOne);

            //Update PIMAttributeGroup
            ZnodeLogging.LogMessage("Pim attribute group with Id to be updated: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeGroupModel?.PimAttributeGroupId);
            bool pimAttributeGroup = _pimAttributeGroupRepository.Update(pimAttributeGroupModel.ToEntity<ZnodePimAttributeGroup>());
            
            //Update PIMAttributeGroup Locale
            if (!Equals(pimAttributeGroupModel.AttributeGroupLocales, null))
                SaveAttributeGroupLocales(new PIMAttributeGroupLocaleListModel()
                {
                    GroupCode = pimAttributeGroupModel.GroupCode,
                    AttributeGroupLocales = pimAttributeGroupModel.AttributeGroupLocales,
                });

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return pimAttributeGroup;
        }

        //Update Attribute Display Order
        public virtual bool UpdateAttributeDisplayOrder(PIMAttributeDataModel attributeDataModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool statusForAttribute = false;
            if (Equals(attributeDataModel, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            if (attributeDataModel?.AttributeModel?.PimAttributeId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorIDLessThanOne);
            ZnodeLogging.LogMessage("PimAttributeId to get PIMAttributeModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeDataModel?.AttributeModel?.PimAttributeId);
            PIMAttributeModel attributeModel = _pimAttributeRepository.GetById(attributeDataModel.AttributeModel.PimAttributeId).ToModel<PIMAttributeModel>();
            
            if (Equals(attributeDataModel.AttributeModel.DisplayOrder, attributeModel.DisplayOrder))
                return true;

            if (IsNotNull(attributeModel))
            {
                attributeModel.DisplayOrder = attributeDataModel?.AttributeModel?.DisplayOrder;
                ZnodeLogging.LogMessage("PIMAttributeModel with DisplayOrder to be updated: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeModel?.DisplayOrder);
                statusForAttribute = _pimAttributeRepository.Update(attributeModel?.ToEntity<ZnodePimAttribute>());
                ZnodeLogging.LogMessage(statusForAttribute ? PIM_Resources.SuccessUpdateAttribute : PIM_Resources.ErrorUpdateAttribute, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return statusForAttribute;
        }

        //Create PIMAttributeGroup.
        public virtual PIMAttributeGroupModel Create(PIMAttributeGroupModel pimAttributeGroupModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (Equals(pimAttributeGroupModel, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (IsAttributeGroupExist(pimAttributeGroupModel.GroupCode, pimAttributeGroupModel.IsCategory))
                throw new ZnodeException(ErrorCodes.AlreadyExist, PIM_Resources.ErrorAttributeGroupExists);

            ZnodePimAttributeGroup createdGroup = _pimAttributeGroupRepository.Insert(pimAttributeGroupModel.ToEntity<ZnodePimAttributeGroup>());
            if (createdGroup?.PimAttributeGroupId > 0)
            {
                pimAttributeGroupModel.PimAttributeGroupId = createdGroup.PimAttributeGroupId;
                CreatePIMAttributeGroupLocales(pimAttributeGroupModel);
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessCreateAttributeGroup, createdGroup?.PimAttributeGroupId), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }
            else
                ZnodeLogging.LogMessage(PIM_Resources.ErrorCreateAttributeGroup, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return pimAttributeGroupModel;
        }

        //Create PIMAttributeGroup.
        public virtual PIMAttributeGroupLocaleListModel SaveAttributeGroupLocales(PIMAttributeGroupLocaleListModel pimAttributeGroupLocaleListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNull(pimAttributeGroupLocaleListModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (pimAttributeGroupLocaleListModel.AttributeGroupLocales?.Count > 0)
            {
                int defaultLocaleId = GetDefaultLocaleId();
                //If locale name is present then group code will be save as default locale name.
                if (pimAttributeGroupLocaleListModel.AttributeGroupLocales.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.AttributeGroupName.Trim())))
                    pimAttributeGroupLocaleListModel.AttributeGroupLocales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.AttributeGroupName = pimAttributeGroupLocaleListModel.GroupCode);

                pimAttributeGroupLocaleListModel.AttributeGroupLocales.RemoveAll(x => x.AttributeGroupName == string.Empty);

                ZnodeLogging.LogMessage("PIM attribute group with Id: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeGroupLocaleListModel?.AttributeGroupLocales?.FirstOrDefault()?.PIMAttributeGroupId);
                _pimAttributeGroupLocaleRepository.Delete(GetWhereClauseForPIMAttributeGroupId(pimAttributeGroupLocaleListModel.AttributeGroupLocales.FirstOrDefault().PIMAttributeGroupId).WhereClause);
                
                IEnumerable<ZnodePimAttributeGroupLocale> entity = _pimAttributeGroupLocaleRepository.Insert(pimAttributeGroupLocaleListModel.AttributeGroupLocales.ToEntity<ZnodePimAttributeGroupLocale>().ToList());
                
                pimAttributeGroupLocaleListModel.AttributeGroupLocales = entity.ToModel<PIMAttributeGroupLocaleModel>().ToList();
                ZnodeLogging.LogMessage("AttributeGroupLocales list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeGroupLocaleListModel?.AttributeGroupLocales?.Count);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return pimAttributeGroupLocaleListModel;
        }

        //Create PIMAttributeGroup.
        public virtual PIMAttributeGroupMapperListModel AssociateAttributes(PIMAttributeGroupMapperListModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (model?.AttributeGroupMappers?.Count > 0)
            {
                //Saves Attributes against AttributeGroup.
                PIMAttributeGroupMapperListModel insertedMapperList = PIMAttributeGroupMap.ToListModel(_pimAttributeGroupMapperRepository.Insert(model.AttributeGroupMappers.ToEntity<ZnodePimAttributeGroupMapper>().ToList()));
                ZnodeLogging.LogMessage("insertedMapperList count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, insertedMapperList?.AttributeGroupMappers?.Count);
                InsertIntoPimFamilyGroupMapper(model);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return insertedMapperList;
            }
            else
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ErrorAssociateAttributeGroup);
        }

        //Remove associated attribute from group.
        public virtual bool RemoveAssociatedAttributes(RemoveAssociatedAttributesModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (Equals(model, null) || string.IsNullOrEmpty(model.PimAttributeIds) || model.PimAttributeGroupId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorInvalidModel);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodePimFamilyGroupMapperEnum.PimAttributeId.ToString(), model.PimAttributeIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePimFamilyGroupMapperEnum.PimAttributeGroupId.ToString(), model.PimAttributeGroupId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("IsCategory", model.isCategory, ParameterDirection.Input, DbType.Boolean);
            ZnodeLogging.LogMessage("PimAttributeId, PimAttributeGroupId and isCategory: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, 
                new object[] { ZnodePimFamilyGroupMapperEnum.PimAttributeId.ToString(), ZnodePimFamilyGroupMapperEnum.PimAttributeGroupId.ToString(), model.isCategory });
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UnassignPimAttributeFromGroup @PimAttributeId,@PimAttributeGroupId,@IsCategory");
            
            if (deleteResult.FirstOrDefault().Status.Value)
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteAttributeGroupMapper, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorDeleteAttributeGroupMapper, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, PIM_Resources.ErrorDeleteAttributeGroup);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return deleteResult.FirstOrDefault().Status.Value;
        }

        //Delete pim attribute group.
        public virtual bool Delete(ParameterModel attributeGroupIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (Equals(attributeGroupIds, null) || string.IsNullOrEmpty(attributeGroupIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAttributeGroupIDNull);

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("PimAttributegroupedIds", attributeGroupIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("attributeGroupIds to be deleted: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeGroupIds?.Ids);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("ZNode_DeleteAttributeGroup @PimAttributegroupedIds,@Status OUT", 1, out status);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteAttributeGroup, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorDeleteAttributeGroup, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, PIM_Resources.ErrorDeleteAttributeGroup);
            }
        }

        //Get group locale By group Id.
        public virtual PIMAttributeGroupLocaleListModel GetAttributeGroupLocale(int attributeGroupId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("attributeGroupId to get attribute group locales list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeGroupId);
            IList<ZnodePimAttributeGroupLocale> groupLocales = _pimAttributeGroupLocaleRepository.GetEntityList(GetWhereClauseForPIMAttributeGroupId(attributeGroupId).WhereClause, GetExpands(expands));
            ZnodeLogging.LogMessage("Attribute group locales count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, groupLocales.Count);
            return groupLocales?.Count > 0 ? new PIMAttributeGroupLocaleListModel() { AttributeGroupLocales = groupLocales.ToModel<PIMAttributeGroupLocaleModel>().ToList() } : null;
        }

        #region Private Methods

        //Insert details in pimFamilyGroupMapper.
        private void InsertIntoPimFamilyGroupMapper(PIMAttributeGroupMapperListModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int selectedGroupId = model.AttributeGroupMappers.First().PimAttributeGroupId.GetValueOrDefault();

            //Generate where clause.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeGroupEnum.PimAttributeGroupId.ToString(), ProcedureFilterOperators.Equals, selectedGroupId.ToString()));
            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("WhereClause and selectedGroupId to get family list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { entityWhereClauseModel?.WhereClause, selectedGroupId });
            IList<ZnodePimFamilyGroupMapper> families = _pimFamilyGroupMapperRepository.GetEntityList(entityWhereClauseModel?.WhereClause, entityWhereClauseModel?.FilterValues)?.GroupBy(x => x.PimAttributeFamilyId).Select(g => g.First()).ToList();
            ZnodeLogging.LogMessage("family list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, families?.Count);

            if (families?.Count > 0)
            {
                var selectedAttributeIds = model.AttributeGroupMappers.Select(x => x.PimAttributeId);
                IList<ZnodePimFamilyGroupMapper> familyGroupMapper = new List<ZnodePimFamilyGroupMapper>();
                foreach (ZnodePimFamilyGroupMapper family in families)
                {
                    foreach (var attributesToAdd in selectedAttributeIds)
                    {
                        familyGroupMapper.Add(new ZnodePimFamilyGroupMapper() { PimAttributeId = attributesToAdd, PimAttributeFamilyId = family.PimAttributeFamilyId, PimAttributeGroupId = selectedGroupId });
                    }
                }
                ZnodeLogging.LogMessage(!Equals(_pimFamilyGroupMapperRepository.Insert(familyGroupMapper)) ? PIM_Resources.SuccessCreateFamilyGroupMapperLocale : PIM_Resources.ErrorCreateFamilyGroupMapperLocale, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
        }

        //Insert group locales.
        private void CreatePIMAttributeGroupLocales(PIMAttributeGroupModel pimAttributeGroupModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool isCreated = false;
            PIMAttributeGroupModel attributeGroup = GetLocaleByDefaultLocaleId(pimAttributeGroupModel);
            int pimAttributeGroupId = attributeGroup.PimAttributeGroupId;
            if (attributeGroup?.AttributeGroupLocales?.Count > 0)
            {
                attributeGroup.AttributeGroupLocales.ForEach(x => x.PIMAttributeGroupId = pimAttributeGroupId);
                ZnodeLogging.LogMessage("AttributeGroupLocales list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeGroup?.AttributeGroupLocales?.Count);
                isCreated = !Equals(_pimAttributeGroupLocaleRepository.Insert(attributeGroup.AttributeGroupLocales.ToEntity<ZnodePimAttributeGroupLocale>().ToList()), null);
            }
            ZnodeLogging.LogMessage(isCreated ? PIM_Resources.SuccessCreateAttributeGroupLocale : PIM_Resources.ErrorCreateAttributeGroupLocale, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
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
                    if (Equals(key, ExpandKeys.GroupLocale)) SetExpands(ZnodePimAttributeGroupEnum.ZnodePimAttributeGroupLocales.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.GroupMappers)) SetExpands(ZnodePimAttributeGroupEnum.ZnodePimAttributeGroupMappers.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.PIMAttribute)) SetExpands(ZnodePimAttributeGroupMapperEnum.ZnodePimAttribute.ToString(), navigationProperties);
                    if (Equals(key, ZnodePimAttributeEnum.ZnodePimAttributeLocales.ToString().ToLower())) SetExpands(ZnodePimAttributeEnum.ZnodePimAttributeLocales.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Checks if attribute group already exists or not.
        private bool IsAttributeGroupExist(string groupCode, bool isCategory)
            => _pimAttributeGroupRepository.Table.Any(x => x.GroupCode == groupCode && x.IsCategory == isCategory);

        //Gets the where clause for pim attribute group id.
        private EntityWhereClauseModel GetWhereClauseForPIMAttributeGroupId(int pimAttributeGroupId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeGroupEnum.PimAttributeGroupId.ToString(), ProcedureFilterOperators.Equals, pimAttributeGroupId.ToString()));
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
        }

        //Get group locales by default localeid.
        private PIMAttributeGroupModel GetLocaleByDefaultLocaleId(PIMAttributeGroupModel pimAttributeGroupModel)
        {
            int defaultLocaleId = GetDefaultLocaleId();
            ZnodeLogging.LogMessage("defaultLocaleId to get locale: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, defaultLocaleId);

            if (pimAttributeGroupModel.AttributeGroupLocales.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.AttributeGroupName.Trim())))
                pimAttributeGroupModel.AttributeGroupLocales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.AttributeGroupName = pimAttributeGroupModel.GroupCode);

            pimAttributeGroupModel.AttributeGroupLocales.RemoveAll(x => x.AttributeGroupName == string.Empty);
            return pimAttributeGroupModel;
        }

        //Remove IsCategory filter.
        private string RemoveIsCategoryFilter(FilterCollection filters)
        {
            string returnValue = "false";
            int data = filters.FindIndex(x => Equals(x.Item1, ZnodePimAttributeGroupEnum.IsCategory.ToString().ToLower()));
            if (data > -1)
            {
                returnValue = filters.First(x => Equals(x.Item1, ZnodePimAttributeGroupEnum.IsCategory.ToString().ToLower())).FilterValue;
                filters.RemoveAt(data);
            }
            return returnValue;
        }

        //Reset group filters.
        private void ResetFilters(FilterCollection filters)
        {
            filters = IsNull(filters) ? new FilterCollection() : filters;
            if (!Equals(filters.Where(x => x.FilterName.Equals(ZnodePimAttributeFamilyEnum.IsSystemDefined.ToString().ToLower()))?.FirstOrDefault(), null))
                SetFilterValue(filters, ZnodePimAttributeFamilyEnum.IsSystemDefined.ToString().ToLower());
        }

        //Set filter value.
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
        #endregion
    }
}
