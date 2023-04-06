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
    public class PIMAttributeFamilyService : BaseService, IPIMAttributeFamilyService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePimAttributeFamily> _pimAttributeFamilyRepository;
        private readonly IZnodeRepository<ZnodePimFamilyGroupMapper> _pimAttributeFamilyGroupMapperRepository;
        private readonly IZnodeRepository<ZnodePimFamilyLocale> _pimAttributeFamilyLocaleRepository;
        private readonly IZnodeRepository<ZnodePimAttributeGroup> _pimAttributeGroupRepository;
        private readonly IZnodeRepository<ZnodePimFamilyLocale> _localeRepository;
        private readonly IZnodeRepository<ZnodePimAttributeGroupMapper> _pimAttributeGroupMapperRepository;
        private readonly IZnodeRepository<ZnodePimAttribute> _pimAttributeRepository;
        #endregion

        public PIMAttributeFamilyService()
        {
            //Initialization of PIM Repositories.
            _pimAttributeFamilyRepository = new ZnodeRepository<ZnodePimAttributeFamily>();
            _pimAttributeFamilyGroupMapperRepository = new ZnodeRepository<ZnodePimFamilyGroupMapper>();
            _pimAttributeFamilyLocaleRepository = new ZnodeRepository<ZnodePimFamilyLocale>();
            _pimAttributeGroupRepository = new ZnodeRepository<ZnodePimAttributeGroup>();
            _localeRepository = new ZnodeRepository<ZnodePimFamilyLocale>();
            _pimAttributeGroupMapperRepository = new ZnodeRepository<ZnodePimAttributeGroupMapper>();
            _pimAttributeRepository = new ZnodeRepository<ZnodePimAttribute>();
        }

        #region Public Methods

        //Get attribute family list.
        public virtual PIMAttributeFamilyListModel GetAttributeFamilyList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (filters.Any(filter => string.Equals(filter.FilterName, ZnodeConstant.IsFamilyExists, StringComparison.OrdinalIgnoreCase)))
                filters.Remove(filters.SingleOrDefault(x => string.Equals(x.FilterName, ZnodeConstant.IsFamilyExists, StringComparison.OrdinalIgnoreCase)));

            //set locale filters if not present
            SetLocaleFilterIfNotPresent(ref filters);

            //Set Paging Parameters
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<PIMAttributeFamilyModel> objStoredProc = new ZnodeViewRepository<PIMAttributeFamilyModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            ZnodeLogging.LogMessage("pageListModel to get attribute family list: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<PIMAttributeFamilyModel> attributeFamilyListEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetPimAttributeFamilies @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Attribute family list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, attributeFamilyListEntity?.Count);

            PIMAttributeFamilyListModel attributeFamilyListModel = new PIMAttributeFamilyListModel { PIMAttributeFamilies = attributeFamilyListEntity?.ToList() };
            attributeFamilyListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return attributeFamilyListModel;
        }

        //Create PIM attribute family.
        public virtual PIMAttributeFamilyModel Create(PIMAttributeFamilyModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (Equals(model, null))
            {
                ZnodeLogging.LogMessage(Admin_Resources.ModelNotNull, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            }
            if (IsPIMAttributeFamilyExist(model.FamilyCode, model.IsCategory))
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorAttributeFamilyExists, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AlreadyExist, PIM_Resources.ErrorAttributeFamilyExists);
            }

            //Creates new pim attribute family.
            ZnodePimAttributeFamily pimAttributeFamily = _pimAttributeFamilyRepository.Insert(model.ToEntity<ZnodePimAttributeFamily>());
            ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessCreateAttributeFamily, pimAttributeFamily?.PimAttributeFamilyId), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Filter to generate where clause.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeFamilyEnum.PimAttributeFamilyId.ToString(), ProcedureFilterOperators.Equals, model.ExistingAttributeFamilyId.ToString()));

            //Where clause of existing pim attribute family.     
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            if (model.ExistingAttributeFamilyId > 0)
                ZnodeLogging.LogMessage(CreatePIMAttributeFamilyGroup(pimAttributeFamily, whereClauseModel) ? PIM_Resources.SuccessInsertAttributeFamilyGroup : PIM_Resources.ErrorInsertAttributeFamilyGroup, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage(CreatePIMAttributeFamilyLocales(pimAttributeFamily) ? PIM_Resources.SuccessInsertAttributeFamilyLocale : PIM_Resources.ErrorInsertAttributeFamilyLocale, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (!Equals(pimAttributeFamily, null))
                return pimAttributeFamily.ToModel<PIMAttributeFamilyModel>();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return model;
        }

        //Get PIM attribute family by family Id.
        public virtual PIMAttributeFamilyModel GetAttributeFamily(int pimAttributeFamilyId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (pimAttributeFamilyId > 0)
            {
                ZnodeLogging.LogMessage("pimAttributeFamilyId to get attribute family: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeFamilyId);
                ZnodePimAttributeFamily pimAttributeFamily = _pimAttributeFamilyRepository.GetById(pimAttributeFamilyId);
                return !Equals(pimAttributeFamily, null) ? pimAttributeFamily.ToModel<PIMAttributeFamilyModel>() : null;
            }
            return null;
        }

        //Get list from PIMAttributeFamilyGroupMapper table.
        public virtual PIMAttributeGroupListModel GetAssignedAttributeGroups(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            PIMAttributeGroupListModel listModel = new PIMAttributeGroupListModel();

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            List<string> navigationProperties = GetExpands(expands);
            ZnodeLogging.LogMessage("WhereClause to get pimGroupFamiliesMapper list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.EntityWhereClause?.WhereClause);
            IList<ZnodePimFamilyGroupMapper> pimGroupFamiliesMapper = _pimAttributeFamilyGroupMapperRepository.GetEntityList(pageListModel.EntityWhereClause.WhereClause, navigationProperties).GroupBy(item => item.PimAttributeGroupId).Select(g => g.First()).ToList();
            foreach (var groupFamily in pimGroupFamiliesMapper)
            {
                ZnodePimAttributeGroup pimAttributeGroup = groupFamily.ZnodePimAttributeGroup;
                if (IsNotNull(pimAttributeGroup) && !pimAttributeGroup.IsNonEditable)
                {
                    PIMAttributeGroupModel pimAttributeGroupModel = pimAttributeGroup?.ToModel<PIMAttributeGroupModel>();
                    pimAttributeGroupModel.DisplayOrder = Convert.ToInt32(groupFamily?.ZnodePimAttributeGroup.ZnodePimFamilyGroupMappers?.FirstOrDefault()?.GroupDisplayOrder);
                    listModel.AttributeGroupList.Add(pimAttributeGroupModel);
                }
            }
            listModel.AttributeGroupList = listModel.AttributeGroupList?.OrderBy(x => x.DisplayOrder).ToList();
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("AttributeGroupList count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { listModel?.AttributeGroupList?.Count });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get list from PIMAttributeFamilyGroupMapper table.
        public virtual PIMAttributeGroupListModel GetUnAssignedAttributeGroups(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            // set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string isCategory = RemoveIsCategoryFilter(filters);

            NameValueCollection expands = new NameValueCollection();
            expands.Add(ExpandKeys.GroupLocale, ExpandKeys.PIMGroupLocale);
            expands.Add(ExpandKeys.GroupMappers, ExpandKeys.GroupMappers);
            expands.Add(ZnodePimAttributeGroupEnum.ZnodePimAttributeGroupLocales.ToString().ToLower(), ZnodePimAttributeGroupEnum.ZnodePimAttributeGroupLocales.ToString().ToLower());
            List<string> navigationProperties = GetExpands(expands);

            //Gets the where clause.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause and pageListModel to get assignedAttributeGroups list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { whereClauseModel.WhereClause, pageListModel?.ToDebugString() });
            List<int?> assignedAttributeGroups = _pimAttributeFamilyGroupMapperRepository.GetPagedList(whereClauseModel.WhereClause, pageListModel.OrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.Select(x => x.PimAttributeGroupId).Distinct().ToList();

            FilterCollection filtersForAttributeGroups = new FilterCollection();
            if (assignedAttributeGroups?.Count > 0)
                filtersForAttributeGroups.Add(ZnodePimAttributeGroupEnum.PimAttributeGroupId.ToString(), FilterOperators.NotIn, string.Join(",", assignedAttributeGroups));
            filtersForAttributeGroups.Add(ZnodePimAttributeGroupEnum.IsCategory.ToString(), FilterOperators.Equals, isCategory);
            filtersForAttributeGroups.Add(ZnodePimAttributeGroupEnum.IsNonEditable.ToString(), FilterOperators.Equals, "false");
            filtersForAttributeGroups.Add(new FilterTuple(ZnodePimAttributeFamilyEnum.IsSystemDefined.ToString(), FilterOperators.Equals, "false"));
            EntityWhereClauseModel whereClauseModelForAttributes = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersForAttributeGroups.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause to get attributeGroupList: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClauseModelForAttributes?.WhereClause);
            IList<ZnodePimAttributeGroup> attributeGroupList = _pimAttributeGroupRepository.GetEntityList(whereClauseModelForAttributes.WhereClause, navigationProperties, whereClauseModelForAttributes.FilterValues)?.AsEnumerable().Where(x => x.ZnodePimAttributeGroupMappers.Count > 0).ToList();
            ZnodeLogging.LogMessage("assignedAttributeGroups and attributeGroupList count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { assignedAttributeGroupsCount = assignedAttributeGroups?.Count,  attributeGroupListCount = attributeGroupList?.Count });

            PIMAttributeGroupListModel listModel = PIMAttributeGroupMap.AddGroupNameToListModel(attributeGroupList);
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Assign PIM group to family.
        public virtual bool AssignAttributeGroups(PIMFamilyGroupAttributeListModel listModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (Equals(listModel, null) || Equals(listModel.FamilyAttributeGroups, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            //Save pim attribute groups against pim attribute family.
            IEnumerable<ZnodePimFamilyGroupMapper> familyGroupAttributes = _pimAttributeFamilyGroupMapperRepository.Insert(PIMAttributeFamilyMap.ToFamilyGroupAttributeListEntity(listModel));
            ZnodeLogging.LogMessage("familyGroupAttributes list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, familyGroupAttributes?.Count());

            if (familyGroupAttributes?.Count() > 0)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessAssignAttributeGroupsToFamily, string.Join(",", familyGroupAttributes.Select(a => a.PimAttributeGroupId.ToString()).ToArray()), familyGroupAttributes.Select(a => a.PimAttributeFamilyId).FirstOrDefault()), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return false;
        }

        //Assign PIM group from family.
        public virtual bool UnAssignAttributeGroups(int attributeFamilyId, int attributeGroupId, bool isCategory)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (attributeFamilyId < 0 && attributeGroupId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorMapperAbsent);

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("PimAttributeGroupId", attributeGroupId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("PimAttributeFamilyId", attributeFamilyId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("IsCategory", isCategory, ParameterDirection.Input, DbType.Boolean);
            ZnodeLogging.LogMessage("SP parameter values:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { attributeGroupId, attributeFamilyId, isCategory });
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_PimUnassociatedGroup @PimAttributeGroupId,@PimAttributeFamilyId,@Status OUT,@IsCategory", 2, out status);
            ZnodeLogging.LogMessage("Deleted records count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, deleteResult?.Count);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorUnassignAttributeGroupsToFamily, attributeFamilyId, attributeFamilyId), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorDeleteAttributeGroupFromFamily, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, PIM_Resources.ErrorDeleteAttributeGroupFromFamily);
            }
        }

        //Delete PIM attribute family.
        public virtual bool DeleteAttributeFamily(ParameterModel attributeFamilyId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("PIMAttributeFamilyId", attributeFamilyId.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("Attribute families with Ids to be deleted: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeFamilyId?.Ids);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimFamily @PIMAttributeFamilyId,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("Deleted records count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, deleteResult?.Count);
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteFamily, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorDeleteAttributeFamily, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, PIM_Resources.ErrorDeleteAttributeFamily);
            }
        }

        //Get family locale By family Id.
        public virtual PIMFamilyLocaleListModel GetFamilyLocale(int attributeFamilyId)
        {
            ZnodeLogging.LogMessage("attributeFamilyId to get familyLocales list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeFamilyId);
            IList<ZnodePimFamilyLocale> familyLocales = _localeRepository.GetEntityList(GetWhereClauseForPIMAttributeFamilyId(attributeFamilyId).WhereClause);
            ZnodeLogging.LogMessage("familyLocales list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, familyLocales?.Count);
            return familyLocales?.Count > 0 ? new PIMFamilyLocaleListModel() { FamilyLocales = familyLocales.ToModel<PIMFamilyLocaleModel>().ToList() } : null;
        }

        //Create PIM family locales.
        public virtual PIMFamilyLocaleListModel SaveLocales(PIMFamilyLocaleListModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model.FamilyLocales?.Count > 0)
            {
                int defaultLocaleId = GetDefaultLocaleId();

                //If locale name is present then family code will be save as default locale name.
                if (model.FamilyLocales.Exists(x => x.LocaleId == defaultLocaleId && string.IsNullOrEmpty(x.AttributeFamilyName.Trim())))
                    model.FamilyLocales.Where(x => x.LocaleId == defaultLocaleId).ToList().ForEach(x => x.AttributeFamilyName = model.FamilyCode);

                model.FamilyLocales.RemoveAll(x => x.AttributeFamilyName == string.Empty);

                ZnodeLogging.LogMessage("Attribute family with Id to be deleted: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.FamilyLocales?.FirstOrDefault()?.PimAttributeFamilyId);
                _pimAttributeFamilyLocaleRepository.Delete(GetWhereClauseForPIMAttributeFamilyId(model.FamilyLocales.FirstOrDefault().PimAttributeFamilyId).WhereClause);

                ZnodeLogging.LogMessage("Count of family locale to be inserted: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model.FamilyLocales.ToEntity<ZnodePimFamilyLocale>().ToList().Count);
                _pimAttributeFamilyLocaleRepository.Insert(model.FamilyLocales.ToEntity<ZnodePimFamilyLocale>().ToList());
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return model;
        }

        //Get PIM attributes by group Ids.
        public virtual PIMAttributeGroupMapperListModel GetAttributesByGroupIds(ParameterModel attributeGroupId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePimAttributeGroupMapperEnum.PimAttributeGroupId.ToString(), ProcedureFilterOperators.In, string.Join(",", attributeGroupId.Ids)));

            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause;
            ZnodeLogging.LogMessage("whereClause to get attributeGroupMapperListEntity: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClause);
            IList<ZnodePimAttributeGroupMapper> attributeGroupMapperListEntity = _pimAttributeGroupMapperRepository.GetEntityList(whereClause);
            ZnodeLogging.LogMessage("attributeGroupMapperListEntity count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, attributeGroupMapperListEntity?.Count);

            //maps the entity list to model
            PIMAttributeGroupMapperListModel pimAttributeGroupMapperList = new PIMAttributeGroupMapperListModel() { AttributeGroupMappers = attributeGroupMapperListEntity?.ToModel<PIMAttributeGroupMapperModel>().ToList() };

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return pimAttributeGroupMapperList;
        }

        //Update attribute group display order.
        public virtual bool UpdateAttributeGroupDisplayOrder(PIMAttributeGroupModel pimAttributeGroupModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool statusForAttribute = false;
            if (Equals(pimAttributeGroupModel, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            if (pimAttributeGroupModel.PimAttributeGroupId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorIDLessThanOne);

            //Add filters
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeGroupEnum.PimAttributeGroupId.ToString(), ProcedureFilterOperators.Equals, pimAttributeGroupModel.PimAttributeGroupId.ToString()));
            filters.Add(new FilterTuple(ZnodePimFamilyGroupMapperEnum.PimAttributeFamilyId.ToString(), ProcedureFilterOperators.Equals, pimAttributeGroupModel.PimAttributeFamilyId.ToString()));
            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("WhereClause to get pimFamilyGroupMapperList: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, entityWhereClauseModel?.WhereClause);
            IList<ZnodePimFamilyGroupMapper> pimFamilyGroupMapperList = _pimAttributeFamilyGroupMapperRepository.GetEntityList(entityWhereClauseModel?.WhereClause, entityWhereClauseModel?.FilterValues);
            ZnodeLogging.LogMessage("pimFamilyGroupMapperList count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimFamilyGroupMapperList?.Count);
            pimFamilyGroupMapperList?.ToList().ForEach(familyGroupMapper =>
            {
                familyGroupMapper.GroupDisplayOrder = pimAttributeGroupModel.DisplayOrder;
                familyGroupMapper.ModifiedDate = DateTime.Now;
                statusForAttribute = _pimAttributeFamilyGroupMapperRepository.Update(familyGroupMapper);
                ZnodeLogging.LogMessage(statusForAttribute ? PIM_Resources.SuccessUpdateGroupDisplayOrder : PIM_Resources.ErrorUpdateGroupDisplayOrder, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return statusForAttribute;
        }

        #region Attributes
        //Get list of attributes associated to group.
        public virtual PIMAttributeListModel GetAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            PIMAttributeListModel listModel = new PIMAttributeListModel();
            IZnodeRepository<ZnodeAttributeType> _attributeTypeRepository = new ZnodeRepository<ZnodeAttributeType>();

            //Set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            
            List<ZnodeAttributeType> attributeTypeList = _attributeTypeRepository.GetEntityList(string.Empty)?.ToList();

            //Maps the entity list to model.
            List<ZnodePimFamilyGroupMapper> pimAttributeGroupFamiliesMapper = _pimAttributeFamilyGroupMapperRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount).ToList();
            ZnodeLogging.LogMessage("pimAttributeGroupFamiliesMapper list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeGroupFamiliesMapper?.Count);

            if (IsNotNull(pimAttributeGroupFamiliesMapper))
                foreach (ZnodePimFamilyGroupMapper attribute in pimAttributeGroupFamiliesMapper)
                {
                    if (attribute.PimAttributeId > 0)
                    {
                        PIMAttributeModel pimAttributeModel = attribute.ZnodePimAttribute?.ToModel<PIMAttributeModel>();
                        if(IsNotNull(pimAttributeModel))
                            pimAttributeModel.AttributeTypeName = attributeTypeList?.Where(x => x.AttributeTypeId == attribute.ZnodePimAttribute.AttributeTypeId)?.FirstOrDefault()?.AttributeTypeName ?? string.Empty;
                        listModel.Attributes.Add(pimAttributeModel);
                    }
                }

            listModel.Attributes = listModel.Attributes?.OrderBy(x => x.DisplayOrder).ToList();
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get list of unassigned attributes.
        public virtual PIMAttributeListModel GetUnAssignedAttributes(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            // set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            string attributeGroupId = filters.First(x => Equals(x.Item1, ZnodePimAttributeGroupEnum.PimAttributeGroupId.ToString().ToLower())).FilterValue;
            NameValueCollection expands = new NameValueCollection();
            expands.Add(ExpandKeys.PIMAttribute, ExpandKeys.PIMAttribute);
            List<string> navigationProperties = GetExpands(expands);

            //Get the where clause.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("WhereClause and pageListModel to get assignedAttributes: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { whereClauseModel?.WhereClause, pageListModel?.ToDebugString() });
            List<int?> assignedAttributes = _pimAttributeFamilyGroupMapperRepository.GetPagedList(whereClauseModel.WhereClause, pageListModel.OrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.Where(x => x.PimAttributeId != null).Select(x => x.PimAttributeId).ToList();
            
            FilterCollection filtersForAttributes = new FilterCollection();
            if (assignedAttributes?.Count > 0)
                filtersForAttributes.Add(ZnodePimAttributeEnum.PimAttributeId.ToString(), FilterOperators.NotIn, string.Join(",", assignedAttributes));

            filtersForAttributes.Add(ZnodePimAttributeGroupEnum.PimAttributeGroupId.ToString(), FilterOperators.Equals, attributeGroupId.ToString());

            EntityWhereClauseModel whereClauseModelForAttributes = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersForAttributes.ToFilterDataCollection());

            ZnodeLogging.LogMessage("WhereClause to get attributeList: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClauseModelForAttributes?.WhereClause);
            IList<ZnodePimAttribute> attributeList = _pimAttributeGroupMapperRepository.GetEntityList(whereClauseModelForAttributes.WhereClause, navigationProperties, whereClauseModelForAttributes.FilterValues)?.AsEnumerable().Select(x => x.ZnodePimAttribute).ToList();
            ZnodeLogging.LogMessage("assignedAttributes and attribute list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { AssignedAttributesCount = assignedAttributes?.Count, AttributeListCount = attributeList?.Count });

            PIMAttributeListModel listModel = PIMAttributesMap.ToListModel(attributeList);
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Assign attribute to group.
        public virtual bool AssignAttributes(AttributeDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            List<ZnodePimFamilyGroupMapper> attributes = new List<ZnodePimFamilyGroupMapper>();

            List<int> attributeIds = model.PimAttributeIds.Split(',').Select(int.Parse).ToList();

            if (attributeIds?.Count() > 0)
                foreach (var id in attributeIds)
                    attributes.Add(new ZnodePimFamilyGroupMapper { PimAttributeFamilyId = model.AttributeFamilyId, PimAttributeGroupId = model.AttributeGroupId, PimAttributeId = id, GroupDisplayOrder = 999 });
            
            IEnumerable<ZnodePimFamilyGroupMapper> GroupAttributes = _pimAttributeFamilyGroupMapperRepository.Insert(attributes)?.ToList();
            ZnodeLogging.LogMessage("GroupAttributes list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, GroupAttributes?.Count());

            if (GroupAttributes?.Count() > 0)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessAssignAttributesToGroup, string.Join(",", GroupAttributes.Select(a => a.PimAttributeId.ToString()).ToArray()), GroupAttributes.Select(a => a.PimAttributeGroupId).FirstOrDefault()), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return false;
        }

        //Unassociate PIM attribute from group.
        public virtual bool UnAssignAttributes(AttributeDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (model.AttributeGroupId <= 0 && model.PimAttributeId <= 0 && model.AttributeFamilyId <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorMapperAbsent);

            if (_pimAttributeRepository.Table.Any(x => x.PimAttributeId == model.PimAttributeId && x.IsSystemDefined))
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, PIM_Resources.ErrorDeleteSystemDefinedAttribute);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimFamilyGroupMapperEnum.PimAttributeId.ToString(), FilterOperators.Equals, model.PimAttributeId.ToString()));
            filters.Add(new FilterTuple(ZnodePimFamilyGroupMapperEnum.PimAttributeGroupId.ToString(), FilterOperators.Equals, model.AttributeGroupId.ToString()));
            filters.Add(new FilterTuple(ZnodePimFamilyGroupMapperEnum.PimAttributeFamilyId.ToString(), FilterOperators.Equals, model.AttributeFamilyId.ToString()));
            ZnodeLogging.LogMessage("PimAttributeId, AttributeGroupId and AttributeFamilyId to generate whereClause: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { model?.PimAttributeId, model?.AttributeGroupId, model?.AttributeFamilyId });

            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;

            ZnodeLogging.LogMessage("whereClause to unassociate attribute: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClause);
            bool isDeleted = _pimAttributeFamilyGroupMapperRepository.Delete(whereClause);
            ZnodeLogging.LogMessage(isDeleted ? PIM_Resources.SuccessUnassociateAttribute : PIM_Resources.ErrorUnassociateAttribute, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return isDeleted;
        }
        #endregion
        #endregion

        #region Private Methods

        //Check if pim attribute family is already exist or not.
        private bool IsPIMAttributeFamilyExist(string pimAttributeFamilyCode, bool isCategory)
         => _pimAttributeFamilyRepository.Table.Any(x => x.FamilyCode == pimAttributeFamilyCode && x.IsCategory == isCategory);

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (!Equals(expands, null) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ExpandKeys.PIMAttributeGroup)) SetExpands(ZnodePimAttributeGroupMapperEnum.ZnodePimAttributeGroup.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.AttributeFamily)) { SetExpands(ZnodePimFamilyLocaleEnum.ZnodePimAttributeFamily.ToString(), navigationProperties); }
                    if (Equals(key, ExpandKeys.PIMGroupLocale)) SetExpands(ZnodePimAttributeGroupEnum.ZnodePimAttributeGroupLocales.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.GroupMappers)) SetExpands(ZnodePimAttributeGroupEnum.ZnodePimAttributeGroupMappers.ToString(), navigationProperties);
                    if (Equals(key, ZnodePimAttributeGroupEnum.ZnodePimAttributeGroupLocales.ToString().ToLower())) SetExpands(ZnodePimAttributeGroupEnum.ZnodePimAttributeGroupLocales.ToString(), navigationProperties);
                    if (Equals(key, ExpandKeys.PIMAttribute)) SetExpands(ZnodePimAttributeGroupMapperEnum.ZnodePimAttribute.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Insert PIM Attribute Family Locales on creation of PIM Family.
        private bool CreatePIMAttributeFamilyLocales(ZnodePimAttributeFamily pimAttributeFamily)
        {
            ZnodePimFamilyLocale model = new ZnodePimFamilyLocale();
            model.AttributeFamilyName = pimAttributeFamily.FamilyCode;
            model.LocaleId = GetDefaultLocaleId();
            model.PimAttributeFamilyId = pimAttributeFamily.PimAttributeFamilyId;
            ZnodeLogging.LogMessage("AttributeFamilyName, LocaleId and PimAttributeFamilyId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { model?.AttributeFamilyName, model?.LocaleId, model?.PimAttributeFamilyId });
            return !Equals(_pimAttributeFamilyLocaleRepository.Insert(model));
        }

        //Insert PIM Attribute Family Group on creation of PIM Family.
        private bool CreatePIMAttributeFamilyGroup(ZnodePimAttributeFamily pimAttributeFamily, EntityWhereClauseModel whereClauseModel)
        {
            //Get the list of pim attribute groups of existing attribute family.
            ZnodeLogging.LogMessage("WhereClause to get pimAttributeFamilyGroups list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);
            IList<ZnodePimFamilyGroupMapper> pimAttributeFamilyGroups = _pimAttributeFamilyGroupMapperRepository.GetEntityList(whereClauseModel.WhereClause);
            if (pimAttributeFamilyGroups?.Count > 0)
            {
                ZnodeLogging.LogMessage("pimAttributeFamilyGroups list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeFamilyGroups?.Count);
                foreach (var pimAttributeFamilyGroup in pimAttributeFamilyGroups)
                {
                    pimAttributeFamilyGroup.PimAttributeFamilyId = pimAttributeFamily.PimAttributeFamilyId;
                }
                //Save the list of pim attribute groups across newly created pim attribute family.
                return !Equals(_pimAttributeFamilyGroupMapperRepository.Insert(pimAttributeFamilyGroups), null);
            }
            return false;
        }

        //Get Where Clause By PimAttributeFamilyId
        private EntityWhereClauseModel GetWhereClauseForPIMAttributeFamilyId(int? attributeFamilyId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeFamilyEnum.PimAttributeFamilyId.ToString(), ProcedureFilterOperators.Equals, attributeFamilyId.ToString()));
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
        }

        //Remove IsCategory filter.
        private string RemoveIsCategoryFilter(FilterCollection filters)
        {
            string returnValue = "false";
            int data = filters.FindIndex(x => Equals(x.Item1, ZnodePimAttributeFamilyEnum.IsCategory.ToString().ToLower()));
            if (data > -1)
            {
                returnValue = filters.First(x => Equals(x.Item1, ZnodePimAttributeFamilyEnum.IsCategory.ToString().ToLower())).FilterValue;
                filters.RemoveAt(data);
            }
            return returnValue;
        }

        #endregion
    }
}

