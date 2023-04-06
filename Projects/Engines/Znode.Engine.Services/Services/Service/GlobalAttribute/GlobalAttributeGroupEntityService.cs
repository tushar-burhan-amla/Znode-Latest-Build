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
using EntityFramework.Extensions;
using Znode.Libraries.Resources;
using Znode.Libraries.Observer;

namespace Znode.Engine.Services
{
    public class GlobalAttributeGroupEntityService : BaseService, IGlobalAttributeGroupEntityService
    {
        #region Private Variable
        protected readonly IZnodeRepository<ZnodeGlobalEntity> _globalEntityRepository;
        protected readonly IZnodeRepository<ZnodeGlobalAttribute> _globalAttributeRepository;
        protected readonly IZnodeRepository<ZnodeGlobalGroupEntityMapper> _groupEntityMapperRepository;
        protected readonly IZnodeRepository<ZnodeGlobalAttributeGroup> _globalAttributeGroupRepository;
        protected readonly IZnodeRepository<ZnodeGlobalAttributeGroupMapper> _groupMapperRepository;
        protected readonly IZnodeRepository<ZnodeLocale> _localeRepository;
        protected readonly IZnodeRepository<ZnodeOmsOrderState> _omsOrderStateRepository;
        protected readonly IZnodeRepository<ZnodeUser> _userRepository;
        protected readonly IZnodeRepository<ZnodeUserPortal> _userPortalRepository;
        protected readonly IZnodeRepository<ZnodeGlobalEntityFamilyMapper> _familyEntityMapperRepository;
        protected readonly IZnodeRepository<ZnodeGlobalAttributeFamily> _attributeFamilyRepository;


        #endregion

        #region Public Constructor
        public GlobalAttributeGroupEntityService()
        {
            _globalEntityRepository = new ZnodeRepository<ZnodeGlobalEntity>();
            _globalAttributeRepository = new ZnodeRepository<ZnodeGlobalAttribute>();
            _groupEntityMapperRepository = new ZnodeRepository<ZnodeGlobalGroupEntityMapper>();
            _globalAttributeGroupRepository = new ZnodeRepository<ZnodeGlobalAttributeGroup>();
            _groupMapperRepository = new ZnodeRepository<ZnodeGlobalAttributeGroupMapper>();
            _localeRepository = new ZnodeRepository<ZnodeLocale>();
            _omsOrderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _userPortalRepository = new ZnodeRepository<ZnodeUserPortal>();
            _familyEntityMapperRepository = new ZnodeRepository<ZnodeGlobalEntityFamilyMapper>();
            _attributeFamilyRepository = new ZnodeRepository<ZnodeGlobalAttributeFamily>();

        }
        #endregion

        #region Public Methods

        //Get Global Entity List
        public virtual GlobalEntityListModel GetGlobalEntityList()
        {
            GlobalEntityListModel list = new GlobalEntityListModel { GlobalEntityList = _globalEntityRepository.Table?.ToList()?.ToModel<GlobalEntityModel>().ToList() ?? null };
            return list;
        }

        //Get list from Global Attribute Group Mapper.
        public virtual GlobalAttributeGroupListModel GetAssignedAttributeGroups(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            GlobalAttributeGroupListModel listModel = new GlobalAttributeGroupListModel();
            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetEntityList", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });
            List<string> navigationProperties = GetExpands(expands);
            IList<ZnodeGlobalGroupEntityMapper> groupEntityMapper = _groupEntityMapperRepository.GetEntityList(pageListModel.EntityWhereClause.WhereClause, navigationProperties).GroupBy(item => item.GlobalAttributeGroupId).Select(g => g.First()).ToList();

            foreach (var groupEntity in groupEntityMapper)
            {
                ZnodeGlobalAttributeGroup attributeGroup = groupEntity.ZnodeGlobalAttributeGroup;
                if (IsNotNull(attributeGroup))
                {
                    GlobalAttributeGroupModel attributeGroupModel = attributeGroup?.ToModel<GlobalAttributeGroupModel>();
                    if(IsNotNull(attributeGroupModel))
                        attributeGroupModel.DisplayOrder = groupEntity.AttributeGroupDisplayOrder;
                    listModel.AttributeGroupList.Add(attributeGroupModel);
                }
            }

            listModel.AttributeGroupList = listModel.AttributeGroupList?.OrderBy(x => x.DisplayOrder).ToList();
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listModel;
        }

        //Get list from Global Attribute Group Mapper.
        public virtual GlobalAttributeGroupListModel GetUnAssignedAttributeGroups(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            // set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            if (IsNull(expands) || Equals(expands?.Count, 0))
                expands = new NameValueCollection();

            expands.Add(ExpandKeys.GroupMappers, ExpandKeys.GroupMappers);
            expands.Add(ExpandKeys.GlobalAttributeGroupLocale, ExpandKeys.GlobalAttributeGroupLocale);
            expands.Add(ZnodeGlobalAttributeGroupEnum.ZnodeGlobalAttributeGroupLocales.ToString().ToLower(), ZnodeGlobalAttributeGroupEnum.ZnodeGlobalAttributeGroupLocales.ToString().ToLower());

            List<string> navigationProperties = GetExpands(expands);

            //Gets the where clause.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            List<int?> assignedGroups = _groupEntityMapperRepository.GetPagedList(whereClauseModel.WhereClause, pageListModel.OrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.Select(x => x.GlobalAttributeGroupId).ToList();
            ZnodeLogging.LogMessage("assignedGroups count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { assignedGroups?.Count });

            FilterCollection filtersForAttributeGroups = new FilterCollection();
            if (assignedGroups?.Count > 0)
                filtersForAttributeGroups.Add(ZnodeGlobalAttributeGroupEnum.GlobalAttributeGroupId.ToString(), FilterOperators.NotIn, string.Join(",", assignedGroups));

            EntityWhereClauseModel whereClauseForAttributes = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersForAttributeGroups.ToFilterDataCollection());
            IList<ZnodeGlobalAttributeGroup> attributeGroupList = _globalAttributeGroupRepository.GetEntityList(whereClauseForAttributes.WhereClause, navigationProperties, whereClauseForAttributes.FilterValues)?.AsEnumerable().ToList();
            GlobalAttributeGroupListModel listModel = GlobalAttributeMap.AddGroupNameToListModel(attributeGroupList);
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listModel;
        }

        //Get list of attributes associated to group.
        public virtual GlobalAttributeListModel GetAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            GlobalAttributeListModel listModel = new GlobalAttributeListModel();

            //Set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //Maps the entity list to model.
            List<ZnodeGlobalGroupEntityMapper> globalGroupEntityMapper = _groupEntityMapperRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount).ToList();

            if (IsNotNull(globalGroupEntityMapper) && String.IsNullOrEmpty(expands.Get(ExpandKeys.GlobalAttribute)))
            {
                foreach (ZnodeGlobalGroupEntityMapper group in globalGroupEntityMapper)
                {
                    if (group.GlobalAttributeGroupId > 0)
                    {
                        List<GlobalAttributeModel> groupAttributes = GetAttributesByGroupId(group.GlobalAttributeGroupId.Value);
                        foreach (GlobalAttributeModel attribute in groupAttributes)
                        {
                            listModel.Attributes.Add(attribute);
                        }
                    }
                }
            }
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listModel;
        }

        //Update attribute group display order.
        public virtual bool UpdateAttributeGroupDisplayOrder(GlobalAttributeGroupModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            bool statusForAttribute = false;

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model.GlobalAttributeGroupId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            ZnodeLogging.LogMessage("GlobalAttributeGroupModel with GlobalAttributeGroupId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { model?.GlobalAttributeGroupId });

            //Add filters
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeGlobalAttributeGroupEnum.GlobalAttributeGroupId.ToString(), ProcedureFilterOperators.Equals, model.GlobalAttributeGroupId.ToString()));
            filters.Add(new FilterTuple(ZnodeGlobalEntityEnum.GlobalEntityId.ToString(), ProcedureFilterOperators.Equals, model.GlobalEntityId.ToString()));
            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            IList<ZnodeGlobalGroupEntityMapper> groupEntityList = _groupEntityMapperRepository.GetEntityList(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
            groupEntityList?.ToList().ForEach(groupEntityMapper =>
            {
                groupEntityMapper.AttributeGroupDisplayOrder = model.DisplayOrder;
                groupEntityMapper.ModifiedDate = DateTime.Now;
                statusForAttribute = _groupEntityMapperRepository.Update(groupEntityMapper);

                ZnodeLogging.LogMessage(statusForAttribute ? Admin_Resources.SuccessEntityGroupDispalyOrderUpdate : Admin_Resources.ErrorEntityGroupDispalyOrderUpdate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return statusForAttribute;
        }

        //Assign Groups group to Entity.
        public virtual bool AssignEntityGroups(GlobalAttributeGroupEntityModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (Equals(model, null) || string.IsNullOrEmpty(model.GroupIds))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            List<int> attributegroupIds = new List<int>(Array.ConvertAll(model.GroupIds.Split(','), int.Parse));
            ZnodeLogging.LogMessage("attributegroupIds count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { attributegroupIds });

            List<ZnodeGlobalAttributeGroup> groupList = _globalAttributeGroupRepository.Table.Where(x => attributegroupIds.Contains(x.GlobalAttributeGroupId)).ToList();
            ZnodeLogging.LogMessage("groupList count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { groupList });

            if (IsNotNull(groupList))
            {
                List<ZnodeGlobalGroupEntityMapper> entityList = new List<ZnodeGlobalGroupEntityMapper>();
                foreach (ZnodeGlobalAttributeGroup group in groupList ?? new List<ZnodeGlobalAttributeGroup>())
                {
                    ZnodeGlobalGroupEntityMapper groupEntityMapper = new ZnodeGlobalGroupEntityMapper
                    {
                        GlobalEntityId = model.EntityId,
                        GlobalAttributeGroupId = group.GlobalAttributeGroupId,
                        AttributeGroupDisplayOrder = !Equals(group.DisplayOrder, null) ? group.DisplayOrder : 999
                    };
                    entityList.Add(groupEntityMapper);
                }
                IEnumerable<ZnodeGlobalGroupEntityMapper> list = _groupEntityMapperRepository.Insert(entityList);

                if (list?.Count() > 0)
                {
                    ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessGlobalAttributegroup, model.GroupIds, model.EntityId), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    return true;
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return false;
        }

        //Un-assign global group from Entity.
        public virtual bool UnAssignEntityGroup(int entityId, int groupId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (entityId < 0 && groupId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorMapperPresent);

            ZnodeLogging.LogMessage("Input parameters entityId, groupId", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { entityId, groupId });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("GlobalAttributeGroupId", groupId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("GlobalEntityId", entityId, ParameterDirection.Input, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UnassociateGroupEntity @GlobalAttributeGroupId,@GlobalEntityId");
            ZnodeLogging.LogMessage("deleteResult count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { deleteResult.Count });

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessGlobalAttributegroup, groupId, entityId), ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorUnassociateGroup);
            }
        }

        public virtual EntityAttributeModel SaveEntityAttributeDetails(EntityAttributeModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            var xmlData = HelperUtility.ToXML<List<EntityAttributeDetailsModel>>(model.Attributes);

            int familyId = string.IsNullOrEmpty(model.FamilyCode) ? 0 : GetAttributeFamilyId(model.FamilyCode);


            model.EntityType = string.Equals(ZnodeConstant.Widget, model.EntityType, StringComparison.CurrentCultureIgnoreCase) ? ZnodeConstant.ContentContainers : model.EntityType;

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("GlobalEntityValueXml", xmlData, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("GlobalEntityValueId", model.EntityValueId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("EntityName", model.EntityType, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("FamilyId", familyId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;

            int userId = model.EntityType.Equals(ZnodeConstant.AccountUser) ? model.EntityValueId : 0;
            ZnodeLogging.LogMessage("userId :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { userId });

            if (userId > 0)
            {
                IAccountQuoteService _accountQuoteService = ZnodeDependencyResolver.GetService<IAccountQuoteService>();
                _accountQuoteService.GetUsersAdditionalAttributes(userId);
            }
            IList<View_ReturnBoolean> createResult = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateGlobalEntityAttributeValue @GlobalEntityValueXml,@GlobalEntityValueId,@EntityName,@FamilyId,@UserId, @status OUT", 5, out status);
            if (createResult.FirstOrDefault().Status.Value)
            {
                model.IsSuccess = true;
                if (model.Attributes.Exists(x => x.AttributeCode == ZnodeConstant.BillingAccountNumber && !string.IsNullOrEmpty(x.AttributeValue)))
                    model.IsSuccess = UpdateQuoteStatus(model.EntityValueId);
                if (!model.IsSuccess)
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorQuoteStatusUpdate,ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    model.IsSuccess = false;
                }
                else
                {
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessEntityAttributeSave,ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    model.IsSuccess = true;
                }
                return model;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorEntityAttributeSave,ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return model;
            }
        }

        //Get the Attribute Values based on the Entity ID & its Type.
        public virtual GlobalAttributeEntityDetailsModel GetEntityAttributeDetails(int entityId, string entityType)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters entityId, entityType", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { entityId, entityType });
            entityType = string.Equals(ZnodeConstant.Widget, entityType, StringComparison.CurrentCultureIgnoreCase) ? ZnodeConstant.ContentContainers : entityType;

            GlobalAttributeEntityDetailsModel model = new GlobalAttributeEntityDetailsModel();
            if (entityId > 0 && !string.IsNullOrEmpty(entityType))
            {
                IZnodeViewRepository<GlobalAttributeValuesModel> globalAttributeValues = new ZnodeViewRepository<GlobalAttributeValuesModel>();
                globalAttributeValues.SetParameter("EntityName", entityType, ParameterDirection.Input, DbType.String);
                globalAttributeValues.SetParameter("GlobalEntityValueId", entityId, ParameterDirection.Input, DbType.Int32);
                model.Attributes = globalAttributeValues.ExecuteStoredProcedureList("Znode_GetGlobalEntityAttributeValue @EntityName, @GlobalEntityValueId").ToList();
                model.EntityId = entityId;
                model.EntityType = entityType;
                model.FamilyCode = GetAssociatedFamily(entityId,  entityType);
                model.HasChildAccount = IsAccountHasChildAccount(entityId, entityType);

                model.Groups = GetAssociatedGroups(entityType, model.FamilyCode, Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale));

            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return model;
        }

        //Get expands and add them to navigation properties
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

        //Get publish global attributes.
        public virtual GlobalSelectedAttributeEntityDetailsModel GetGlobalEntityAttributes(int entityId, string entityType, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            int localeId = 0;
            string localeCode = string.Empty;
            string groupCode = string.Empty;
            if (IsNotNull(filters) && filters.Count > 0)
            {
                localeCode = Convert.ToString(filters?.Find(x => string.Equals(x.FilterName, ZnodeLocaleEnum.Code.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
                groupCode = Convert.ToString(filters?.Find(x => string.Equals(x.FilterName, ZnodeGlobalAttributeGroupEnum.GroupCode.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            }
            localeId = localeId == 0 ? Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale) : localeId;

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return GetSelectedEntityAttributeDetails(entityId, entityType, groupCode, localeCode);
        }

        //Get the Attribute Values based on the Entity ID & its Type.
        public virtual GlobalSelectedAttributeEntityDetailsModel GetSelectedEntityAttributeDetails(int entityId, string entityType, string groupCode = "", string localeCode = "")
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            GlobalSelectedAttributeEntityDetailsModel model = new GlobalSelectedAttributeEntityDetailsModel();
            if (entityId > 0 && !string.IsNullOrEmpty(entityType))
            {
                IZnodeViewRepository<GlobalSelectedAttributeModel> globalAttributeValues = new ZnodeViewRepository<GlobalSelectedAttributeModel>();
                globalAttributeValues.SetParameter("EntityName", entityType, ParameterDirection.Input, DbType.String);
                globalAttributeValues.SetParameter("GlobalEntityValueId", entityId, ParameterDirection.Input, DbType.Int32);
                globalAttributeValues.SetParameter("GroupCode", groupCode ?? string.Empty, ParameterDirection.Input, DbType.String);
                globalAttributeValues.SetParameter("LocaleCode", localeCode ?? string.Empty, ParameterDirection.Input, DbType.String);
                globalAttributeValues.SetParameter("SelectedValue", 1, ParameterDirection.Input, DbType.Int16);
                List<GlobalSelectedAttributeModel> attr = globalAttributeValues.ExecuteStoredProcedureList("Znode_GetGlobalEntityAttributeValue @EntityName, @GlobalEntityValueId, @LocaleCode, @GroupCode,@SelectedValue").ToList();
                ZnodeLogging.LogMessage("globalAttributeValues attr count", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { attr.Count });

                model.Groups = GetSelectedEntityAssociatedGroup(entityType, localeCode, groupCode);
                ZnodeLogging.LogMessage("Groups count returned from GetSelectedEntityAssociatedGroup ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { model?.Groups?.Count });

                MapAttributeAndGroup(attr, model);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return model;
        }

        //gets the global attributes based on the passed familyCode for setting the values for default container variant.
        public virtual GlobalAttributeEntityDetailsModel GetGlobalAttributesForDefaultVariantData(string familyCode, string entityType)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters familyCode, entityType", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { familyCode, entityType });
            entityType = string.Equals(ZnodeConstant.Widget, entityType, StringComparison.CurrentCultureIgnoreCase) ? ZnodeConstant.ContentContainers : entityType;

            GlobalAttributeEntityDetailsModel model = new GlobalAttributeEntityDetailsModel();
            if (!string.IsNullOrEmpty(familyCode) && !string.IsNullOrEmpty(entityType))
            {
                IZnodeViewRepository<GlobalAttributeValuesModel> globalAttributeValues = new ZnodeViewRepository<GlobalAttributeValuesModel>();
                globalAttributeValues.SetParameter("EntityName", entityType, ParameterDirection.Input, DbType.String);
                globalAttributeValues.SetParameter("FamilyCode", familyCode, ParameterDirection.Input, DbType.String);
                model.Attributes = globalAttributeValues.ExecuteStoredProcedureList("Znode_GetContentWidgetGlobalAttributeValue @EntityName, @FamilyCode").ToList();
                model.EntityType = entityType;
                model.Groups = GetAssociatedGroups(entityType, familyCode, Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale));
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return model;
        }

        //gets the global attributes based on the passed container variantId and LocaleId.
        public virtual GlobalAttributeEntityDetailsModel GetGlobalAttributesForAssociatedVariant(int variantId, string entityType, int localeId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters familyId, entityType", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { variantId, localeId, entityType });
            entityType = string.Equals(ZnodeConstant.Widget, entityType, StringComparison.CurrentCultureIgnoreCase) ? ZnodeConstant.Widget : entityType;
            string localeCode = string.Empty;
            IZnodeRepository<ZnodeCMSContentContainer> _contentContainerRepository = new ZnodeRepository<ZnodeCMSContentContainer>();
            IZnodeRepository<ZnodeCMSContainerProfileVariant> _containerProfileVariantRepository = new ZnodeRepository<ZnodeCMSContainerProfileVariant>();
            int familyId = (from _contentContainer in _contentContainerRepository.Table
                            join _containerProfileVariant in _containerProfileVariantRepository.Table on _contentContainer.CMSContentContainerId equals _containerProfileVariant.CMSContentContainerId
                            where _containerProfileVariant.CMSContainerProfileVariantId == variantId
                            select _contentContainer.FamilyId).FirstOrDefault();

            GlobalAttributeEntityDetailsModel model = new GlobalAttributeEntityDetailsModel();
            if (variantId > 0 && !string.IsNullOrEmpty(entityType))
            {
                if(localeId > 0)
                    localeCode = _localeRepository.Table.FirstOrDefault(x => x.LocaleId == localeId)?.Code;

                try
                {
                    IZnodeViewRepository<GlobalAttributeValuesModel> globalAttributeValues = new ZnodeViewRepository<GlobalAttributeValuesModel>();
                    globalAttributeValues.SetParameter("EntityName", entityType, ParameterDirection.Input, DbType.String);
                    globalAttributeValues.SetParameter("GlobalEntityValueId", variantId, ParameterDirection.Input, DbType.Int32);
                    globalAttributeValues.SetParameter("LocaleCode", localeCode, ParameterDirection.Input, DbType.String);
                    model.Attributes = globalAttributeValues.ExecuteStoredProcedureList("Znode_GetWidgetGlobalAttributeValue @EntityName,@GlobalEntityValueId,@LocaleCode").ToList();
                    model.EntityType = entityType;

                    model.FamilyCode = _attributeFamilyRepository.Table.FirstOrDefault(x => x.GlobalAttributeFamilyId == familyId)?.FamilyCode;
                    model.Groups = GetAssociatedGroups(entityType, model.FamilyCode, Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale));
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                }

            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return model;
        }
        #endregion

        #region Private Method

        //Mapping Attribute and Groups.
        private void MapAttributeAndGroup(List<GlobalSelectedAttributeModel> attr, GlobalSelectedAttributeEntityDetailsModel model)
        => model.Groups.ForEach(x => x.Attributes = attr.Where(g => g.GlobalAttributeGroupId == x.GlobalAttributeGroupId).ToList());

        //Get Attributes By GroupId
        private List<GlobalAttributeModel> GetAttributesByGroupId(int groupId)
        {
            List<GlobalAttributeModel> attributesList = (from attribute in _globalAttributeRepository.Table
                                                         join mapper in _groupMapperRepository.Table on attribute.GlobalAttributeId equals mapper.GlobalAttributeId
                                                         where mapper.GlobalAttributeGroupId == groupId
                                                         orderby attribute.DisplayOrder
                                                         select new GlobalAttributeModel
                                                         {

                                                             GlobalAttributeId = attribute.GlobalAttributeId,
                                                             AttributeTypeId = attribute.AttributeTypeId,
                                                             AttributeCode = attribute.AttributeCode,
                                                             IsRequired = attribute.IsRequired,
                                                             IsLocalizable = attribute.IsLocalizable,
                                                             AttributeGroupId = groupId,
                                                             HelpDescription = attribute.HelpDescription,
                                                             DisplayOrder = attribute.DisplayOrder,
                                                         }
                                              ).ToList();

            return attributesList ?? new List<GlobalAttributeModel>();

        }

        //Get Entity Associated group.
        protected virtual List<GlobalAttributeGroupModel> GetEntityAssociatedGroup(string entityType, int localeId)
        {
            IZnodeRepository<ZnodeGlobalAttributeGroup> _attributeGroupRepository = new ZnodeRepository<ZnodeGlobalAttributeGroup>();
            IZnodeRepository<ZnodeGlobalAttributeGroupLocale> _attributeGroupLocaleRepository = new ZnodeRepository<ZnodeGlobalAttributeGroupLocale>();
            IZnodeRepository<ZnodeGlobalGroupEntityMapper> _attributeGroupEntityRepository = new ZnodeRepository<ZnodeGlobalGroupEntityMapper>();
            return (from globalEntity in _globalEntityRepository.Table
                    join entityMapper in _attributeGroupEntityRepository.Table on globalEntity.GlobalEntityId equals entityMapper.GlobalEntityId
                    join groupEntity in _attributeGroupRepository.Table on entityMapper.GlobalAttributeGroupId equals groupEntity.GlobalAttributeGroupId
                    join groupLocale in _attributeGroupLocaleRepository.Table on groupEntity.GlobalAttributeGroupId equals groupLocale.GlobalAttributeGroupId
                    where globalEntity.EntityName == entityType && groupLocale.LocaleId == localeId
                    orderby entityMapper.AttributeGroupDisplayOrder
                    select new GlobalAttributeGroupModel
                    {
                        AttributeGroupName = groupLocale.AttributeGroupName,
                        GroupCode = groupEntity.GroupCode,
                        GlobalAttributeGroupId = groupEntity.GlobalAttributeGroupId,

                    }).ToList();
        }

        protected virtual List<GlobalAttributeGroupModel> GetAssociatedGroups(string entityType, string familyCode, int localeId)
        {
            IZnodeRepository<ZnodeGlobalAttributeGroup> _attributeGroupRepository = new ZnodeRepository<ZnodeGlobalAttributeGroup>();
            IZnodeRepository<ZnodeGlobalAttributeGroupLocale> _attributeGroupLocaleRepository = new ZnodeRepository<ZnodeGlobalAttributeGroupLocale>();
            IZnodeRepository<ZnodeGlobalFamilyGroupMapper> _attributeFamilyGroupRepository = new ZnodeRepository<ZnodeGlobalFamilyGroupMapper>();

            return (from globalEntity in _globalEntityRepository.Table
                    join attributeFamily in _attributeFamilyRepository.Table on globalEntity.GlobalEntityId equals attributeFamily.GlobalEntityId
                    join familyGroupMapper in _attributeFamilyGroupRepository.Table on attributeFamily.GlobalAttributeFamilyId equals familyGroupMapper.GlobalAttributeFamilyId
                    join attributeGroup in _attributeGroupRepository.Table on familyGroupMapper.GlobalAttributeGroupId equals attributeGroup.GlobalAttributeGroupId
                    join attributeGrouplocale in _attributeGroupLocaleRepository.Table on attributeGroup.GlobalAttributeGroupId equals attributeGrouplocale.GlobalAttributeGroupId
                    where globalEntity.EntityName == entityType && attributeGrouplocale.LocaleId == localeId && attributeFamily.FamilyCode == familyCode
                    orderby familyGroupMapper.AttributeGroupDisplayOrder
                    select new GlobalAttributeGroupModel
                    {
                        AttributeGroupName = attributeGrouplocale.AttributeGroupName,
                        GroupCode = attributeGroup.GroupCode,
                        GlobalAttributeGroupId = attributeGroup.GlobalAttributeGroupId,

                    }).ToList();
        }


        protected virtual string GetAssociatedFamily(int entityValueId, string entityType)
        {
            if (_globalEntityRepository.Table.FirstOrDefault(x => x.EntityName == entityType).IsFamilyUnique)
            {
                return  (from globalentity in _globalEntityRepository.Table
                        join mapper in _familyEntityMapperRepository.Table
                         on globalentity.GlobalEntityId equals mapper.GlobalEntityId
                        join attributeFamily in _attributeFamilyRepository.Table
                        on mapper.GlobalAttributeFamilyId equals attributeFamily.GlobalAttributeFamilyId
                        where globalentity.EntityName == entityType
                        select attributeFamily.FamilyCode).FirstOrDefault();
            }
            else if (string.Equals(ZnodeConstant.ContentContainers, entityType,StringComparison.CurrentCultureIgnoreCase))
            {
                IZnodeRepository<ZnodeCMSContainerProfileVariant> _containerProfileVariantRepository = new ZnodeRepository<ZnodeCMSContainerProfileVariant>();
                IZnodeRepository<ZnodeCMSContentContainer> _contentContainerRepository = new ZnodeRepository<ZnodeCMSContentContainer>();
                return (from profileVariant in _containerProfileVariantRepository.Table
                        join contentContainer in _contentContainerRepository.Table
                        on profileVariant.CMSContentContainerId equals contentContainer.CMSContentContainerId
                        join attributeFamily in _attributeFamilyRepository.Table
                        on contentContainer.FamilyId equals attributeFamily.GlobalAttributeFamilyId
                        where profileVariant.CMSContainerProfileVariantId == entityValueId
                        select attributeFamily.FamilyCode).FirstOrDefault();
            }
            return (from mapper in _familyEntityMapperRepository.Table join attributeFamily in _attributeFamilyRepository.Table
                    on mapper.GlobalAttributeFamilyId equals attributeFamily.GlobalAttributeFamilyId
                    where mapper.GlobalEntityValueId == entityValueId select attributeFamily.FamilyCode).FirstOrDefault();
        }


        //Get Entity Associated group.
        private List<GlobalSelectedAttributeGroupModel> GetSelectedEntityAssociatedGroup(string entityType, string locale, string groupCode = "")
        {
            int localeId = string.IsNullOrEmpty(locale) ? Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale) : (_localeRepository.Table.Any(x => x.Code.ToLower() == locale.ToLower()) ? _localeRepository.Table.FirstOrDefault(x => x.Code.ToLower() == locale.ToLower()).LocaleId : 0);

            IZnodeRepository<ZnodeGlobalAttributeGroup> _attributeGroupRepository = new ZnodeRepository<ZnodeGlobalAttributeGroup>();
            IZnodeRepository<ZnodeGlobalAttributeGroupLocale> _attributeGroupLocaleRepository = new ZnodeRepository<ZnodeGlobalAttributeGroupLocale>();
            IZnodeRepository<ZnodeGlobalGroupEntityMapper> _attributeGroupEntityRepository = new ZnodeRepository<ZnodeGlobalGroupEntityMapper>();
            return string.IsNullOrEmpty(groupCode) ? (from globalEntity in _globalEntityRepository.Table
                                                      join entityMapper in _attributeGroupEntityRepository.Table on globalEntity.GlobalEntityId equals entityMapper.GlobalEntityId
                                                      join groupEntity in _attributeGroupRepository.Table on entityMapper.GlobalAttributeGroupId equals groupEntity.GlobalAttributeGroupId
                                                      join groupLocale in _attributeGroupLocaleRepository.Table on groupEntity.GlobalAttributeGroupId equals groupLocale.GlobalAttributeGroupId
                                                      where globalEntity.EntityName == entityType && groupLocale.LocaleId == localeId
                                                      orderby entityMapper.AttributeGroupDisplayOrder
                                                      select new GlobalSelectedAttributeGroupModel
                                                      {
                                                          AttributeGroupName = groupLocale.AttributeGroupName,
                                                          GroupCode = groupEntity.GroupCode,
                                                          GlobalAttributeGroupId = groupEntity.GlobalAttributeGroupId,
                                                      }).ToList() :
             (from globalEntity in _globalEntityRepository.Table
              join entityMapper in _attributeGroupEntityRepository.Table on globalEntity.GlobalEntityId equals entityMapper.GlobalEntityId
              join groupEntity in _attributeGroupRepository.Table on entityMapper.GlobalAttributeGroupId equals groupEntity.GlobalAttributeGroupId
              join groupLocale in _attributeGroupLocaleRepository.Table on groupEntity.GlobalAttributeGroupId equals groupLocale.GlobalAttributeGroupId
              where globalEntity.EntityName == entityType && groupLocale.LocaleId == localeId && groupEntity.GroupCode == groupCode
              orderby entityMapper.AttributeGroupDisplayOrder
              select new GlobalSelectedAttributeGroupModel
              {
                  AttributeGroupName = groupLocale.AttributeGroupName,
                  GroupCode = groupEntity.GroupCode,
                  GlobalAttributeGroupId = groupEntity.GlobalAttributeGroupId,
              }).ToList();
        }

        //Check for Account, whether the Account has any Child Account.
        protected virtual bool IsAccountHasChildAccount(int entityId, string entityType)
        {
            bool hasChildAccount = false;
            if (string.Equals(entityType, Convert.ToString(EntityTypeEnum.Account), StringComparison.CurrentCultureIgnoreCase))
            {
                IZnodeRepository<ZnodeAccount> _accountRepository = new ZnodeRepository<ZnodeAccount>();
                int? childAccountId = _accountRepository.Table.Where(x => x.ParentAccountId == entityId)?.Select(x => x.AccountId).FirstOrDefault();
                ZnodeLogging.LogMessage("childAccountId :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { childAccountId });

                hasChildAccount = childAccountId > 0;
            }
            return hasChildAccount;
        }

        #region Update Quote Status
        private bool UpdateQuoteStatus(int userId)
        {
            try
            {
                List<OrderStateModel> omsOrderStates = _omsOrderStateRepository.Table.Select(x => new OrderStateModel { OrderStateId = x.OmsOrderStateId, OrderStateName = x.OrderStateName }).ToList();
                int? omsorderstateIdPendingPayment = omsOrderStates?.FirstOrDefault(x => x.OrderStateName.ToLower() == ZnodeConstant.PendingPaymentStateName.ToString()).OrderStateId;
                int? omsOrderStateIdPendingApproval = omsOrderStates?.FirstOrDefault(x => x.OrderStateName.ToLower() == ZnodeConstant.PendingApprovalStateName.ToString()).OrderStateId;
                HelperMethods.CurrentContext.ZnodeOmsQuotes.Where(x => x.UserId == userId && x.IsPendingPayment && x.OmsOrderStateId == omsorderstateIdPendingPayment).Update(y => new ZnodeOmsQuote() { IsPendingPayment = false, OmsOrderStateId = Convert.ToInt32(omsOrderStateIdPendingApproval) });
                ZnodeUser model = _userRepository.Table.FirstOrDefault(x => x.UserId == userId);
                int portalId = _userPortalRepository.Table.FirstOrDefault(x => x.UserId == userId)?.PortalId ?? 0;
                ZnodeLogging.LogMessage("portalId :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { portalId });

                if (portalId > 0)
                {
                    SendEmailToCustomer($"{model.FirstName} {model.LastName}", model.Email, portalId, GetDefaultLocaleId());
                    var onBillingAccountNumberAddedInit = new ZnodeEventNotifier<UserModel>(model.ToModel<UserModel>(), EventConstant.OnBillingAccountNumberAdded);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //This method will send email to relevant approver about quote creation.        
        private void SendEmailToCustomer(string approverName, string approverEmail, int portalId, int localeId)
        {
            PortalModel portalModel = GetCustomPortalDetails(portalId);
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.BillingAccountNumberAdded, portalId, localeId);

            if (HelperUtility.IsNotNull(emailTemplateMapperModel))
            {
                string subject = $"{emailTemplateMapperModel?.Subject} - {portalModel.StoreName}";

                string messageText = emailTemplateMapperModel.Descriptions;
                messageText = HelperUtility.ReplaceTokenWithMessageText(ZnodeConstant.CustomerName, approverName, messageText);

                //Send  mail to approver.
                try
                {
                    ZnodeEmail.SendEmail(portalId, approverEmail, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(true, portalId, string.Empty), subject, messageText, true);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorMailSendingToCustomer, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error, ex);
                }
            }
        }

        private int GetAttributeFamilyId(string attributeFamilyCode)
        {
            int familyId =  (from attributeFamily in _attributeFamilyRepository.Table.Where(x => x.FamilyCode.ToLower() == attributeFamilyCode.ToLower())
                            select attributeFamily.GlobalAttributeFamilyId).FirstOrDefault();

            if(familyId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidFamilyCode);

            return familyId;
        }

        #endregion

        #endregion
    }
}
