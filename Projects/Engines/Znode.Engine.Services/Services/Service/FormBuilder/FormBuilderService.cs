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
using System.IO;
using System.Web;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Admin;

namespace Znode.Engine.Services
{
    public class FormBuilderService : BaseService, IFormBuilderService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeFormBuilder> _formBuilderRepository;
        private readonly IZnodeRepository<ZnodeGlobalAttribute> _globalAttributeRepository;
        private readonly IZnodeRepository<ZnodeGlobalAttributeGroup> _globalAttributeGroupRepository;
        private readonly IZnodeRepository<ZnodeGlobalAttributeGroupMapper> _groupMapperRepository;
        private readonly IZnodeRepository<ZnodeFormBuilderAttributeMapper> _formMapperRepository;
        private readonly IZnodeRepository<ZnodeAttributeType> _attributeTypeRepository;
        private readonly IZnodeRepository<ZnodeGlobalAttributeGroupLocale> _attributeGroupLocaleRepository;
        private readonly IZnodeRepository<ZnodeCMSFormWidgetConfiguration> _formWidgetConfigurationRepository;
        private readonly IZnodeRepository<ZnodeGlobalAttributeLocale> _attributeLocaleRepository;
        private readonly IZnodeRepository<ZnodeFormWidgetEmailConfiguration> _formWidgetEmailConfigurationRepository;
        private readonly IZnodeRepository<ZnodeEmailTemplateArea> _emailTemplateAreaRepository;
        private readonly IZnodeRepository<ZnodeEmailTemplateMapper> _emailTemplateMapperRepository;
        private readonly IEmailTemplateSharedService _emailTemplateSharedService;
        #endregion

        #region Public Constructor
        public FormBuilderService()
        {
            _formBuilderRepository = new ZnodeRepository<ZnodeFormBuilder>();
            _globalAttributeRepository = new ZnodeRepository<ZnodeGlobalAttribute>();
            _globalAttributeGroupRepository = new ZnodeRepository<ZnodeGlobalAttributeGroup>();
            _groupMapperRepository = new ZnodeRepository<ZnodeGlobalAttributeGroupMapper>();
            _formMapperRepository = new ZnodeRepository<ZnodeFormBuilderAttributeMapper>();
            _attributeTypeRepository = new ZnodeRepository<ZnodeAttributeType>();
            _attributeGroupLocaleRepository = new ZnodeRepository<ZnodeGlobalAttributeGroupLocale>();
            _formWidgetConfigurationRepository = new ZnodeRepository<ZnodeCMSFormWidgetConfiguration>();
            _attributeLocaleRepository = new ZnodeRepository<ZnodeGlobalAttributeLocale>();
            _formWidgetEmailConfigurationRepository = new ZnodeRepository<ZnodeFormWidgetEmailConfiguration>();
            _emailTemplateAreaRepository = new ZnodeRepository<ZnodeEmailTemplateArea>();
            _emailTemplateMapperRepository = new ZnodeRepository<ZnodeEmailTemplateMapper>();
            _emailTemplateSharedService = GetService<IEmailTemplateSharedService>();
        }
        #endregion

        #region Public Methods
        //Get the list of form builder.
        public FormBuilderListModel GetFormBuilderList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            IZnodeViewRepository<FormBuilderModel> objStoredProc = new ZnodeViewRepository<FormBuilderModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            List<FormBuilderModel> formBuilderList = objStoredProc.ExecuteStoredProcedureList("Znode_GetFormBuilderList @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT", 4, out pageListModel.TotalRowCount).ToList();
            ZnodeLogging.LogMessage("formBuilderList count", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { formBuilderList?.Count });

            FormBuilderListModel listModel = new FormBuilderListModel();

            listModel.FormBuilderList = formBuilderList?.Count > 0 ? formBuilderList : new List<FormBuilderModel>();
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return listModel;
        }

        //Create form.
        public virtual FormBuilderModel CreateForm(FormBuilderModel formBuilderModel)
        {
            ZnodeLogging.LogMessage("CreateForm method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("FormCode and FormBuilderId values: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { formBuilderModel?.FormCode, formBuilderModel?.FormBuilderId });
            if (IsNull(formBuilderModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (IsFormCodeExist(formBuilderModel.FormCode))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorFormCodeAlreadyExists);

            //Insert into form builder table and maps id into model.
            formBuilderModel.FormBuilderId = _formBuilderRepository.Insert(formBuilderModel.ToEntity<ZnodeFormBuilder>()).ToModel<FormBuilderModel>().FormBuilderId;
            ZnodeLogging.LogMessage("Insert form builder table and maps id into model with  FormBuilderId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, formBuilderModel?.FormBuilderId);

            if (formBuilderModel?.FormBuilderId > 0)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessFormIdCreate, formBuilderModel.FormBuilderId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return formBuilderModel;
            }
            ZnodeLogging.LogMessage(Admin_Resources.ErrorFormCreate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("CreateForm method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return formBuilderModel;
        }

        public virtual FormBuilderModel GetFormBuilderById(int formBuilderId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters formBuilderId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, formBuilderId);

            if (formBuilderId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            FormBuilderModel model = _formBuilderRepository.Table.Where(x => x.FormBuilderId == formBuilderId)?.FirstOrDefault()?.ToModel<FormBuilderModel>();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return model ?? new FormBuilderModel();
        }

        public virtual bool AssignGroupsToForm(GlobalAttributeGroupEntityModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(model) || IsNull(model.FormBuilderId) || string.IsNullOrEmpty(model.GroupIds))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            string assignedGroups = GroupAttributesAssignedToForm(model.FormBuilderId, model.LocaleId, model.GroupIds);
            ZnodeLogging.LogMessage("assignedGroups returned from GroupAttributesAssignedToForm ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, assignedGroups);

            if (!string.IsNullOrEmpty(assignedGroups))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Api_Resources.AttributeAlreadyAssociated);

            List<ZnodeFormBuilderAttributeMapper> formMapperList = SetFormBuilderGroupAttributes(model.FormBuilderId, model.GroupIds);
            ZnodeLogging.LogMessage("formMapperList count returned from SetFormBuilderGroupAttributes", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, formMapperList?.Count);

            if (formMapperList?.Count > 0)
            {
                IEnumerable<ZnodeFormBuilderAttributeMapper> list = _formMapperRepository.Insert(formMapperList);
                ZnodeLogging.LogMessage("ZnodeFormBuilderAttributeMapper list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, list?.Count());
                if (list?.Count() > 0)
                {
                    ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessAttributeGroupAssociateToForm, model.GroupIds, model.FormBuilderId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                    return true;
                }
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorAssociateGroupToForm, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return false;
            }
            ZnodeLogging.LogMessage("AssignGroupsToForm method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return false;
        }

        public virtual bool AssignAttributesToForm(GlobalAttributeGroupEntityModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);


            if (IsNull(model) || IsNull(model.FormBuilderId) || string.IsNullOrEmpty(model.AttributeIds))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            string assignedAttributes = AttributesAssignedToForm(model.FormBuilderId, model.LocaleId, model.AttributeIds);
            ZnodeLogging.LogMessage("Assigned Attributes:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, assignedAttributes);

            if (!string.IsNullOrEmpty(assignedAttributes))
                throw new ZnodeException(ErrorCodes.InvalidData, string.Format(Admin_Resources.ErrorAssociateAttributeToForm, assignedAttributes));

            List<ZnodeFormBuilderAttributeMapper> formMapperList = SetFormBuilderAttributes(model.FormBuilderId, model.AttributeIds);
            ZnodeLogging.LogMessage("formMapperList returned from SetFormBuilderAttributes", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, formMapperList?.Count);

            if (formMapperList?.Count > 0)
            {
                IEnumerable<ZnodeFormBuilderAttributeMapper> list = _formMapperRepository.Insert(formMapperList);
                ZnodeLogging.LogMessage("ZnodeFormBuilderAttributeMapper list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, list?.Count());
                if (list?.Count() > 0)
                {
                    ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessGlobalAttributeGroupAssociateToForm, model.GroupIds, model.FormBuilderId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                    return true;
                }
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorAssociateAttributeToForm, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return false;
            }
            return false;
        }

        public virtual bool UnAssignFormBuilderGroup(int formBuilderId, int groupId)
        {
            ZnodeLogging.LogMessage("UnAssignFormBuilderGroup method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (formBuilderId < 0 && groupId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorMapperPresent);

            ZnodeLogging.LogMessage("Input parameters formBuilderId, groupId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { formBuilderId, groupId });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("GlobalAttributeId", string.Empty, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("GlobalAttributeGroupId", groupId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("FormBuilderId", formBuilderId, ParameterDirection.Input, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UnassociateFormBuilderGroupAttribute @GlobalAttributeId,@GlobalAttributeGroupId,@FormBuilderId ");
            ZnodeLogging.LogMessage("deleteResult list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, deleteResult?.Count());
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessUnassignGroupFromFormBuilder, groupId, formBuilderId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorUnAssociateGroupFromFormBuilder);
            }
        }

        public virtual bool UnAssignFormBuilderAttribute(int formBuilderId, int attributeId)
        {
            ZnodeLogging.LogMessage("UnAssignFormBuilderAttribute method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (formBuilderId < 0 && attributeId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorMapperPresent);

            ZnodeLogging.LogMessage("Input parameters formBuilderId, attributeId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { formBuilderId, attributeId });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("GlobalAttributeId", attributeId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("GlobalAttributeGroupId", 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("FormBuilderId", formBuilderId, ParameterDirection.Input, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UnassociateFormBuilderGroupAttribute @GlobalAttributeId,@GlobalAttributeGroupId,@FormBuilderId");
            ZnodeLogging.LogMessage("deleteResult list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, deleteResult?.Count());
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessUnassignAttributeFromFormBuilder, attributeId, formBuilderId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorUnAssociateAttributeFromFormBuilder);
            }
        }

        public virtual bool UpdateGroupDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            bool isUpdated = false;

            if (IsNull(model) && model.FormBuilderId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (IsNull(model.GroupId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            ZnodeLogging.LogMessage("Update group display order GroupId and FormBuilderId ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.GroupId, model?.FormBuilderId });

            ZnodeFormBuilderAttributeMapper mapper = _formMapperRepository.Table.FirstOrDefault(x => x.GlobalAttributeGroupId == model.GroupId && x.FormBuilderId == model.FormBuilderId);
            int prevDisplayOrder = mapper.DisplayOrder.Value;
            bool isNavigateUpward = model.IsNavigateUpward;
            int newDisplayOrder = isNavigateUpward ? prevDisplayOrder - 1 : prevDisplayOrder + 1;
            ZnodeLogging.LogMessage("newDisplayOrder : ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { newDisplayOrder });

            if (ToggleDisplayOrder(model.FormBuilderId, prevDisplayOrder, isNavigateUpward))
            {
                mapper.DisplayOrder = newDisplayOrder;
                _formMapperRepository.Update(mapper);
                isUpdated = true;
            }
            ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.SuccessAttributeDisplayOrderUpdate : Admin_Resources.ErrorAttributeDisplayOrderUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return isUpdated;
        }

        public virtual bool UpdateAttributeDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            bool isUpdated = false;

            if (IsNull(model) && model.FormBuilderId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (IsNull(model.AttributeId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);
            ZnodeLogging.LogMessage("Update group display order AttributeId and FormBuilderId ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.AttributeId, model?.FormBuilderId });

            ZnodeFormBuilderAttributeMapper mapper = _formMapperRepository.Table.FirstOrDefault(x => x.GlobalAttributeId == model.AttributeId && x.FormBuilderId == model.FormBuilderId);
            int prevDisplayOrder = mapper.DisplayOrder.Value;
            bool isNavigateUpward = model.IsNavigateUpward;
            int newDisplayOrder = isNavigateUpward ? prevDisplayOrder - 1 : prevDisplayOrder + 1;
            ZnodeLogging.LogMessage("newDisplayOrder : ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { newDisplayOrder });

            if (ToggleDisplayOrder(model.FormBuilderId, prevDisplayOrder, isNavigateUpward))
            {
                mapper.DisplayOrder = newDisplayOrder;
                _formMapperRepository.Update(mapper);
                isUpdated = true;
            }
            ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.SuccessAttributeDisplayOrderUpdate : Admin_Resources.ErrorAttributeDisplayOrderUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return isUpdated;
        }

        public virtual GlobalAttributeListModel GetUnAssignedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //gets the order by clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("Where clause in EntityWhereClauseModel method get data", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);
            List<int?> assignedAttributes = _formMapperRepository.GetPagedList(whereClauseModel.WhereClause, orderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.Where(w => w.GlobalAttributeId != null)?.Select(x => x.GlobalAttributeId).ToList();

            int formBuilderId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, ZnodeFormBuilderEnum.FormBuilderId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            ZnodeLogging.LogMessage("assignedAttributes count and formBuilderId :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { assignedAttributes?.Count, formBuilderId });

            assignedAttributes = GetFormBuilderAttributes(formBuilderId, assignedAttributes);
            ZnodeLogging.LogMessage("assignedAttributes returned from GetFormBuilderAttributes  ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { assignedAttributes });

            FilterCollection filtersForAttributes = new FilterCollection();

            if (assignedAttributes.Count > 0)
                filtersForAttributes.Add(ZnodeGlobalAttributeEnum.GlobalAttributeId.ToString(), FilterOperators.NotIn, string.Join(",", assignedAttributes));

            int linkAttributeTypeId = _attributeTypeRepository.Table.Where(x => x.AttributeTypeName == "Link").Select(x => x.AttributeTypeId).FirstOrDefault();
            ZnodeLogging.LogMessage("linkAttributeTypeId ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { linkAttributeTypeId });

            filtersForAttributes.Add(ZnodeGlobalAttributeEnum.AttributeTypeId.ToString(), FilterOperators.NotEquals, linkAttributeTypeId.ToString());
            EntityWhereClauseModel whereClauseModelForAttributes = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersForAttributes.ToFilterDataCollection());
            ZnodeLogging.LogMessage("Where clause in EntityWhereClauseModel method get data", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);
            IList<ZnodeGlobalAttribute> attributesList = _globalAttributeRepository.GetEntityList(whereClauseModelForAttributes.WhereClause, null, GetExpands(expands), whereClauseModelForAttributes.FilterValues);

            GlobalAttributeListModel list = GlobalAttributeMap.ToListModel(attributesList);

            list.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return list;
        }

        public virtual GlobalAttributeGroupListModel GetUnAssignedGroups(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            // set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            if (IsNull(expands) || Equals(expands?.Count, 0))
                expands = new NameValueCollection();

            if (IsNotNull(expands))
            {
                expands.Add(ExpandKeys.GroupMappers, ExpandKeys.GroupMappers);
                expands.Add(ExpandKeys.GlobalAttributeGroupLocale, ExpandKeys.GlobalAttributeGroupLocale);
            }

            List<string> navigationProperties = GetExpands(expands);

            //Gets the where clause.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("Where clause in GetUnAssignedGroups method get data", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);
            List<int?> assignedGroups = _formMapperRepository.GetPagedList(whereClauseModel.WhereClause, pageListModel.OrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.Where(w => w.GlobalAttributeGroupId != null)?.Select(x => x.GlobalAttributeGroupId).ToList();
            ZnodeLogging.LogMessage("assignedGroups count ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { assignedGroups?.Count });

            FilterCollection filtersForAttributeGroups = new FilterCollection();
            if (assignedGroups?.Count > 0)
                filtersForAttributeGroups.Add(ZnodeGlobalAttributeGroupEnum.GlobalAttributeGroupId.ToString(), FilterOperators.NotIn, string.Join(",", assignedGroups));

            EntityWhereClauseModel whereClauseForAttributes = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersForAttributeGroups.ToFilterDataCollection());
            IList<ZnodeGlobalAttributeGroup> attributeGroupList = _globalAttributeGroupRepository.GetEntityList(whereClauseForAttributes.WhereClause, navigationProperties, whereClauseForAttributes.FilterValues)?.AsEnumerable().ToList();
            GlobalAttributeGroupListModel list = GlobalAttributeMap.AddGroupNameToListModel(attributeGroupList);
            ZnodeLogging.LogMessage("attributeGroupList count ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { attributeGroupList?.Count });

            list.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return list;
        }

        public virtual FormBuilderAttributeGroupModel GetFormBuilderAttributeGroup(int formBuilderId, int localeId, int mappingId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            FormBuilderAttributeGroupModel model;
            if (formBuilderId < 1 || localeId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            model = GetFormBuilderConfiguration(formBuilderId, localeId, mappingId);
            IZnodeViewRepository<GlobalAttributeValuesModel> globalAttributeValues = new ZnodeViewRepository<GlobalAttributeValuesModel>();
            globalAttributeValues.SetParameter(ZnodeFormBuilderSubmitEnum.FormBuilderId.ToString(), formBuilderId, ParameterDirection.Input, DbType.Int32);
            globalAttributeValues.SetParameter(ZnodeFormBuilderSubmitEnum.UserId.ToString(), 0, ParameterDirection.Input, DbType.Int32);
            globalAttributeValues.SetParameter(ZnodeFormBuilderSubmitEnum.PortalId.ToString(), 0, ParameterDirection.Input, DbType.Int32);
            globalAttributeValues.SetParameter(ZnodeFormBuilderSubmitEnum.FormBuilderSubmitId.ToString(), 0, ParameterDirection.Input, DbType.Int32);
            globalAttributeValues.SetParameter(ZnodeFormBuilderSubmitEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
            model.Attributes = globalAttributeValues.ExecuteStoredProcedureList("Znode_GetFormBuilderGlobalAttributeValue @FormBuilderId,@UserId,@PortalId,@FormBuilderSubmitId,@LocaleId").ToList();
            ZnodeLogging.LogMessage("Attributes:", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, model.Attributes);
            model.Groups = GetFormAssociatedGroup(formBuilderId, localeId);
            ZnodeLogging.LogMessage("Groups returned from GetFormAssociatedGroup ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.Groups.Count });

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return model;
        }


        public virtual string GroupAttributesAssignedToForm(int formBuilderId, int localeId, string groupIds)
        {
            ZnodeLogging.LogMessage("Input parameters formBuilderId, localeId, groupIds", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { formBuilderId, localeId, groupIds });

            List<int> attributegroupIds = new List<int>(Array.ConvertAll(groupIds.Split(','), int.Parse));
            ZnodeLogging.LogMessage("attributegroupIds count", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { attributegroupIds?.Count });

            List<string> attributes = (from grpMap in _groupMapperRepository.Table
                                       join formMap in _formMapperRepository.Table on grpMap.GlobalAttributeId equals formMap.GlobalAttributeId
                                       join attribute in _globalAttributeRepository.Table on grpMap.GlobalAttributeId equals attribute.GlobalAttributeId
                                       join lcl in _attributeLocaleRepository.Table on attribute.GlobalAttributeId equals lcl.GlobalAttributeId
                                       where attributegroupIds.Contains(grpMap.GlobalAttributeGroupId.Value)
                                       && formMap.FormBuilderId == formBuilderId
                                       && lcl.LocaleId == localeId
                                       select (lcl.AttributeName)).ToList();

            ZnodeLogging.LogMessage("attributes count", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { attributes.Count });

            return string.Join(",", attributes);
        }

        public virtual string AttributesAssignedToForm(int formBuilderId, int localeId, string attributeIds)
        {
            ZnodeLogging.LogMessage("Input parameters formBuilderId, localeId, attributeIds ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { formBuilderId, localeId, attributeIds });

            List<int> listattributeId = new List<int>(Array.ConvertAll(attributeIds.Split(','), int.Parse));
            List<string> attributes = (from grpMap in _groupMapperRepository.Table
                                       join attribute in _globalAttributeRepository.Table on grpMap.GlobalAttributeId equals attribute.GlobalAttributeId
                                       join lcl in _attributeLocaleRepository.Table on attribute.GlobalAttributeId equals lcl.GlobalAttributeId
                                       join formMap in _formMapperRepository.Table on grpMap.GlobalAttributeId equals formMap.GlobalAttributeId
                                       where listattributeId.Contains(grpMap.GlobalAttributeId.Value)
                                       && formMap.FormBuilderId == formBuilderId
                                       && lcl.LocaleId == localeId
                                       select (lcl.AttributeName)).ToList();
            ZnodeLogging.LogMessage("Attributes list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, attributes?.Count());
            if (attributes?.Count < 1)
            {
                List<int> groupIds = _groupMapperRepository.Table.Where(x => listattributeId.Contains(x.GlobalAttributeId.Value)).Select(x => x.GlobalAttributeGroupId.Value).Distinct().ToList();

                List<int> groupAttributeIds = _groupMapperRepository.Table.Where(x => groupIds.Contains(x.GlobalAttributeGroupId.Value)).Select(x => x.GlobalAttributeId.Value).ToList();
                ZnodeLogging.LogMessage("groupIds and groupAttributeIds count :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { groupIds?.Count, groupAttributeIds?.Count });

                List<int> ids = new List<int>();
                foreach (var item in listattributeId)
                {
                    if (groupAttributeIds.Contains(item))
                        ids.Add(item);
                }
                if (ids.Count > 0 && _formMapperRepository.Table.Any(x => x.FormBuilderId == formBuilderId && groupIds.Contains(x.GlobalAttributeGroupId.Value)))
                {
                    attributes = (from attr in _globalAttributeRepository.Table
                                  join lcl in _attributeLocaleRepository.Table on attr.GlobalAttributeId equals lcl.GlobalAttributeId
                                  where ids.Contains(attr.GlobalAttributeId)
                                   && lcl.LocaleId == localeId
                                  select (lcl.AttributeName)).ToList();
                }
                ZnodeLogging.LogMessage("Attributes:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, attributes);
            }
            ZnodeLogging.LogMessage("Method executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return string.Join(", ", attributes);
        }

        public virtual bool DeleteFormBuilder(ParameterModel formBuilderId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("formBuilderId to be deleted :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { formBuilderId?.Ids });

            if (IsNull(formBuilderId) || string.IsNullOrEmpty(formBuilderId.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorValidFormBuilderId);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeFormBuilderEnum.FormBuilderId.ToString(), formBuilderId.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_DeleteFormBuilder @FormBuilderId,@Status OUT", 1, out status);
            if (status == 1)
            {
                ZnodeLogging.LogMessage(Admin_Resources.HelpTextForFormBuilderDeleteMessage, string.Empty, TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorFailedToDeleteFormBuilder, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorFailedToDeleteFormBuilder);
            }
        }

        //Check form code exist or not.
        public bool IsFormCodeExist(string formCode)
        {
            ZnodeLogging.LogMessage("IsFormCodeExist method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(formCode))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.FormCodeNotNull);
            return _formBuilderRepository.Table.Any(x => x.FormCode.ToLower() == formCode.ToLower());
        }

        //Update form builder
        public virtual bool Update(FormBuilderModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            bool isFormBuilderUpdated = false;
            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model.FormBuilderId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            ZnodeLogging.LogMessage("FormBuilderModel with FormBuilderId :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.FormBuilderId });

            if (model?.FormBuilderId > 0)
            {
                //Update form builder
                isFormBuilderUpdated = _formBuilderRepository.Update(model.ToEntity<ZnodeFormBuilder>());
                ZnodeLogging.LogMessage(isFormBuilderUpdated ? Admin_Resources.SuccessFromBuilderUpdate : Admin_Resources.ErrorFromBuilderUpdate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return isFormBuilderUpdated;
        }

        //Save Form builder data.
        public virtual FormSubmitModel CreateFormTemplate(FormSubmitModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            var xmlData = HelperUtility.ToXML<List<FormSubmitAttributeModel>>(model.Attributes);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("GlobalEntityValueXml", xmlData, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("FormBuilderId", model.FormBuilderId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("LocaleId", model.LocaleId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("PortalId", model.PortalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);

            View_ReturnBoolean result = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateFormBuilderGlobalAttributeValue @GlobalEntityValueXml,@FormBuilderId,@LocaleId,@PortalId,@UserId")?.FirstOrDefault();
            ZnodeLogging.LogMessage("result:", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, result);
            ZnodeLogging.LogMessage(Admin_Resources.SuccessFormAttributeSave, string.Empty, TraceLevel.Info);

            List<string> attachments = GetAttributeFiles(model.Attributes);
            string attributeIds = string.Join(",", model.Attributes.Select(x => x.GlobalAttributeId));
            List<GlobalAttributeModel> attributes = AttributesAssignedToFormForRMA(model.FormBuilderId, model.LocaleId, attributeIds);
            model = SetAttributeName(model, attributes);
            ZnodeLogging.LogMessage("SendEmailNotification method call with parameters", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.FormBuilderId, model?.CustomerEmail, model.LocaleId, model?.PortalId });
            //Send notification email
            SetEmailData(model, attachments);
            model.IsSuccess = result?.Status ?? false;
            return model;
        }

        //Set the attribute name and remove the attribute if it contains file
        public virtual FormSubmitModel SetAttributeName(FormSubmitModel model, List<GlobalAttributeModel> attributes)
        {      
            model.Attributes = (from globalAttribute in attributes
                                                  from formAttribute in model.Attributes
                                                  where formAttribute.GlobalAttributeId == globalAttribute.GlobalAttributeId
                                                  select new FormSubmitAttributeModel()
                                                  {
                                                      GlobalAttributeId = formAttribute.GlobalAttributeId,
                                                      GlobalAttributeValueId = formAttribute.GlobalAttributeValueId,
                                                      GlobalAttributeDefaultValueId = formAttribute.GlobalAttributeDefaultValueId,
                                                      AttributeCode = formAttribute.AttributeCode,
                                                      AttributeValue = formAttribute.AttributeValue,
                                                      LocaleId = formAttribute.LocaleId,
                                                      AttributeName = globalAttribute.AttributeName
                                                  }).ToList();           
            List<int> attributeIdList = attributes.Select(x => x.GlobalAttributeId).ToList();
            model.Attributes = model.Attributes?.Where(x => attributeIdList.Contains(x.GlobalAttributeId)).ToList();
            return model;
        }

        //Data is set for the email.
        public virtual void SetEmailData(FormSubmitModel formSubmitModel, List<string> attachments = null)
        {
            DataTable formDetailItems = BindDataToTabelRow(formSubmitModel);
            //Get email detail in which email need to send
            FormWidgetEmailConfigurationModel model = FormNotificationSetting(formSubmitModel.FormBuilderId);
            SendEmail(formSubmitModel, formDetailItems, model, attachments);
        }

        // Send email according to email template.
        public virtual void SendEmail(FormSubmitModel formSubmitModel, DataTable formDetailItems, FormWidgetEmailConfigurationModel model, List<string> attachments = null)
        {
            if (!string.IsNullOrEmpty(model.NotificationEmailId))
            {
                SendEmailNotificationEmail(formSubmitModel, formDetailItems, model.NotificationEmailTemplateId, model.NotificationEmailId, attachments);
            }
            if (model.AcknowledgementEmailTemplateId > 0 && !string.IsNullOrEmpty(formSubmitModel.CustomerEmail))
            {
                string storeName = GetPortalName(formSubmitModel.PortalId);
                SendEmailNotificationEmail(formSubmitModel, formDetailItems, model.AcknowledgementEmailTemplateId, formSubmitModel.CustomerEmail, null, storeName);
            }
        }

        //Notification email is sent.
        public virtual void SendEmailNotificationEmail(FormSubmitModel formSubmitModel, DataTable formDetailItems, int notificationEmailTemplateId, string customerEmail, List<string> attachments = null, string storeName = "")
        {
            try
            {
                string templatePath = string.Empty;
                string emailTemplateCode = GetEmailTemplateCode(notificationEmailTemplateId);
                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(emailTemplateCode, formSubmitModel.PortalId, formSubmitModel.LocaleId);
                //ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(emailTemplateMapperModel.Descriptions);
                templatePath = GetDynamicHtmlForTemplate(templatePath, emailTemplateMapperModel, formSubmitModel);

                ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(emailTemplateMapperModel.Descriptions);
                string messageText = GetMessageText(formDetailItems, receiptHelper, storeName,formSubmitModel.PortalId);
                if (attachments?.Count > 0)
                {
                    ZnodeEmail.SendEmail(customerEmail, ZnodeConfigManager.SiteConfig.AdminEmail, emailTemplateMapperModel.Subject, messageText, string.Empty, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, formSubmitModel.PortalId, string.Empty), attachments, true);
                }
                else
                {
                    ZnodeEmail.SendEmail(formSubmitModel.PortalId, customerEmail, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, formSubmitModel.PortalId, string.Empty), emailTemplateMapperModel.Subject, messageText, true, "");
                }
            }
            catch (Exception ex)

            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
        }

        //To find the store name.
        protected virtual string GetPortalName(int portalId)
        {
            IZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();
            string portalName = _portalRepository?.Table.FirstOrDefault(x => x.PortalId == portalId).StoreName;
            return portalName;
        }

        //Get the message for the receipt.
        public virtual string GetMessageText(DataTable productDetailItems, ZnodeReceiptHelper receiptHelper, string storeName = "",int portalId = 0)
        {
            receiptHelper.ParseForFormBuilder(productDetailItems.CreateDataReader());
            string messageText = receiptHelper.Output;
            if (!string.IsNullOrEmpty(storeName))
            {
                messageText = ReplaceTokenWithMessageText("#StoreName#", storeName, messageText);
            }
            PortalModel portalModel = GetCustomPortalDetails(portalId);
            messageText = ReplaceTokenWithMessageText("#StoreLogo#", portalModel?.StoreLogo, messageText);
            messageText = ReplaceTokenWithMessageText("#CustomerServicePhoneNumber#", portalModel?.CustomerServicePhoneNumber, messageText);
            messageText = ReplaceTokenWithMessageText("#CustomerServiceEmail#", portalModel?.CustomerServiceEmail, messageText);
           
            return messageText;
        }

        //Get the Dynamic html template.
        public virtual string GetDynamicHtmlForTemplate(string templatePath, EmailTemplateMapperModel emailTemplateMapperModel, FormSubmitModel formSubmitModel)
        {
            List<string> attributeType = new List<string>(new string[] { ZnodeConstant.ProductType , ZnodeConstant.ProductImage, ZnodeConstant.FrequentlyBought, ZnodeConstant.ProductName });           
            foreach (FormSubmitAttributeModel item in formSubmitModel.Attributes)
            {              
                if (!attributeType.Contains(item.AttributeCode))
                {
                    templatePath = templatePath + $"<div style=color:#af0604;font-weight:bold; padding: 0 1rem;>{item.AttributeName} : {"#"}{ item.AttributeCode}{"#"}</div><br></br>";
                }
            }
            emailTemplateMapperModel.Descriptions = emailTemplateMapperModel?.Descriptions?.Replace("DyanamicHtml", templatePath);
            return templatePath;
        }

        //Bind the data to add in Data Row.
        public virtual DataTable BindDataToTabelRow(FormSubmitModel formSubmitModel)
        {
            DataTable formDetailItems = new DataTable();
            DataRow productImageRow = formDetailItems.NewRow();
            foreach (FormSubmitAttributeModel item in formSubmitModel.Attributes)
            {
                DataColumnCollection columns = formDetailItems.Columns;
                if (!columns.Contains(Convert.ToString(item.AttributeCode)))
                {
                    formDetailItems.Columns.Add(Convert.ToString(item.AttributeCode));
                }
                if (item.AttributeValue != null)
                {
                    productImageRow[Convert.ToString(item.AttributeCode)] = item.AttributeValue;
                }
                else
                {
                    productImageRow[Convert.ToString(item.AttributeCode)] = item.AttributeValue;
                }
               
            }
            formDetailItems.Rows.Add(productImageRow);
            return formDetailItems;
        }

        //Get the assigned attributes to form and delete the file contain attributes.
        public virtual List<GlobalAttributeModel> AttributesAssignedToFormForRMA(int formBuilderId, int localeId, string attributeIds)
        {
            List<int> listattributeId = new List<int>(Array.ConvertAll(attributeIds.Split(','), int.Parse));
            List<GlobalAttributeModel> attributes = new List<GlobalAttributeModel>();
            attributes = (from grpMap in _attributeLocaleRepository.Table
                          join formMap in _formMapperRepository.Table
                          on grpMap.GlobalAttributeId equals formMap.GlobalAttributeId
                          join glbAttribute in _globalAttributeRepository.Table
                          on grpMap.GlobalAttributeId equals glbAttribute.GlobalAttributeId
                          join attributetype in _attributeTypeRepository.Table
                          on glbAttribute.AttributeTypeId equals attributetype.AttributeTypeId
                          where listattributeId.Contains(grpMap.GlobalAttributeId.Value)
                          && formMap.FormBuilderId == formBuilderId
                          && grpMap.LocaleId == localeId
                          select new GlobalAttributeModel
                          {
                              AttributeName = grpMap.AttributeName,
                              GlobalAttributeId = formMap.GlobalAttributeId.HasValue ? formMap.GlobalAttributeId.Value : 0,
                              AttributeTypeId = glbAttribute.AttributeTypeId,
                              AttributeTypeName = attributetype.AttributeTypeName
                          }).OrderBy(x => x.GlobalAttributeId).ToList();
            
            List<GlobalAttributeModel> groupAttributes = GroupAttributesAssignedToFormForRMA(formBuilderId, localeId, listattributeId);
            if (groupAttributes != null && groupAttributes.Count > 0)
            {
                attributes.AddRange(groupAttributes);
                attributes.OrderBy(x => x.GlobalAttributeId);
            }

            attributes?.RemoveAll(x => x.AttributeTypeName.ToLower() == ZnodeConstant.File);

            return attributes;
        }

        public virtual List<GlobalAttributeModel> GroupAttributesAssignedToFormForRMA(int formBuilderId, int localeId, List<int> listattributeId)
        {
            List<GlobalAttributeModel> attributes = new List<GlobalAttributeModel>();
            attributes = (from glbAttrLoc in _attributeLocaleRepository.Table
                          join grpMap in _groupMapperRepository.Table
                          on glbAttrLoc.GlobalAttributeId equals grpMap.GlobalAttributeId
                          join formMap in _formMapperRepository.Table
                          on grpMap.GlobalAttributeGroupId equals formMap.GlobalAttributeGroupId
                          join glbAttribute in _globalAttributeRepository.Table
                          on grpMap.GlobalAttributeId equals glbAttribute.GlobalAttributeId
                          join attributetype in _attributeTypeRepository.Table
                          on glbAttribute.AttributeTypeId equals attributetype.AttributeTypeId
                          where listattributeId.Contains(grpMap.GlobalAttributeId.Value)
                          && formMap.FormBuilderId == formBuilderId
                          && glbAttrLoc.LocaleId == localeId
                          select new GlobalAttributeModel
                          {
                              AttributeName = glbAttrLoc.AttributeName,
                              GlobalAttributeId = glbAttrLoc.GlobalAttributeId.HasValue ? glbAttrLoc.GlobalAttributeId.Value : 0,
                              AttributeTypeId = glbAttribute.AttributeTypeId,
                              AttributeTypeName = attributetype.AttributeTypeName
                          }).ToList();
            return attributes;
        }

        //Method to check value of attribute is already exists or not.
        public virtual string FormAttributeValueUnique(GlobalAttributeValueParameterModel attributeCodeValues)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(attributeCodeValues))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            if (attributeCodeValues.Id <= 0 || string.IsNullOrEmpty(attributeCodeValues.EntityType))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorEntityIdOrType);

            var xmlData = ToXML(attributeCodeValues.GlobalAttributeCodeValueList);

            IZnodeViewRepository<GlobalAttributeParameterModel> objStoredProc = new ZnodeViewRepository<GlobalAttributeParameterModel>();
            objStoredProc.SetParameter("AttributeCodeValues", xmlData, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("PortalId", attributeCodeValues.PortalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("FormBuilderId", attributeCodeValues.Id, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), attributeCodeValues.LocaleId, ParameterDirection.Input, DbType.Int32);
            List<GlobalAttributeParameterModel> attributeNames = objStoredProc.ExecuteStoredProcedureList("Znode_CheckUniqueFormBuilderAttributevalues @AttributeCodeValues,@FormBuilderId,@PortalId")?.ToList();
            ZnodeLogging.LogMessage("attributeNames count :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { attributeNames });

            if (attributeNames?.Count > 0)
                return string.Join(",", attributeNames.Select(x => x.AttributeName));

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return string.Empty;
        }

        //Get list of files from attributes which type is 'File'
        protected virtual List<string> GetAttributeFiles(List<FormSubmitAttributeModel> attributes)
        {
            string mediaPath = Path.Combine(HttpContext.Current.Server.MapPath($"~/{ZnodeConstant.FormBuilderMediaPath}/"));
            List<int> attributeIds = attributes?.Select(m => m.GlobalAttributeId)?.ToList();
            if ((attributeIds?.Any()).GetValueOrDefault())
            {
                List<int> fileAttributeIds = (from globalAttribute in _globalAttributeRepository.Table.Where(m => attributeIds.Contains(m.GlobalAttributeId))
                                              join attributeType in _attributeTypeRepository.Table on globalAttribute.AttributeTypeId equals attributeType.AttributeTypeId
                                              where attributeType.AttributeTypeName.ToLower() == ZnodeConstant.File
                                              select globalAttribute.GlobalAttributeId)?.ToList();

                List<string> files = attributes.Where(attribute => (fileAttributeIds?.Contains(attribute.GlobalAttributeId)).GetValueOrDefault())?.Select(m => $"{mediaPath}{m.AttributeValue}")?.ToList();
                return files;
            }
            return null;
        }

        #endregion

        #region Private Method

        //Get Groups and its Attributes by Group Ids 
        private List<ZnodeFormBuilderAttributeMapper> SetFormBuilderGroupAttributes(int formId, string groupIds)
        {
            ZnodeLogging.LogMessage("Input parameters formId, groupIds ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { formId, groupIds });

            List<int> listIds = new List<int>(Array.ConvertAll(groupIds.Split(','), int.Parse));
            List<ZnodeFormBuilderAttributeMapper> list = new List<ZnodeFormBuilderAttributeMapper>();
            int displayOrder = GetDisplayOrder(formId);
            foreach (int id in listIds)
            {
                list.Add(new ZnodeFormBuilderAttributeMapper
                {
                    FormBuilderId = formId,
                    GlobalAttributeGroupId = id,
                    GlobalAttributeId = null,
                    DisplayOrder = displayOrder
                });
                displayOrder++;
            }
            ZnodeLogging.LogMessage("SetFormBuilderGroupAttributes list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, list?.Count());
            return list ?? new List<ZnodeFormBuilderAttributeMapper>();
        }

        private List<ZnodeFormBuilderAttributeMapper> SetFormBuilderAttributes(int formId, string attributeIds)
        {
            List<int> listIds = new List<int>(Array.ConvertAll(attributeIds.Split(','), int.Parse));
            List<ZnodeFormBuilderAttributeMapper> attributeList = new List<ZnodeFormBuilderAttributeMapper>();
            int displayOrder = GetDisplayOrder(formId);
            ZnodeLogging.LogMessage("displayOrder returned from GetDisplayOrder :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { displayOrder });

            foreach (int id in listIds)
            {
                attributeList.Add(new ZnodeFormBuilderAttributeMapper
                {
                    FormBuilderId = formId,
                    GlobalAttributeGroupId = null,
                    GlobalAttributeId = id,
                    DisplayOrder = displayOrder
                });
                displayOrder++;
            }
            ZnodeLogging.LogMessage("attributeList list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, attributeList?.Count());
            return attributeList ?? new List<ZnodeFormBuilderAttributeMapper>();
        }

        private int GetDisplayOrder(int formId)
        {
            return _formMapperRepository.Table.Where(x => x.FormBuilderId == formId).Max(x => x.DisplayOrder) + 1 ?? 1;
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
                    if (Equals(key, ExpandKeys.GlobalAttributeGroupLocale)) SetExpands(ZnodeGlobalAttributeGroupEnum.ZnodeGlobalAttributeGroupLocales.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Get Form Associated Group
        private List<GlobalAttributeGroupModel> GetFormAssociatedGroup(int formBuilderId, int localeId)
        {
            ZnodeLogging.LogMessage("Input formBuilderId, localeId :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { formBuilderId, localeId });

            List<GlobalAttributeGroupModel> groupList = (from frm in _formBuilderRepository.Table
                                                         join map in _formMapperRepository.Table on frm.FormBuilderId equals map.FormBuilderId
                                                         join grp in _globalAttributeGroupRepository.Table on map.GlobalAttributeGroupId equals grp.GlobalAttributeGroupId
                                                         join loc in _attributeGroupLocaleRepository.Table on grp.GlobalAttributeGroupId equals loc.GlobalAttributeGroupId
                                                         where frm.FormBuilderId == formBuilderId && loc.LocaleId == localeId
                                                         orderby map.DisplayOrder
                                                         select new GlobalAttributeGroupModel
                                                         {
                                                             AttributeGroupName = loc.AttributeGroupName,
                                                             GroupCode = grp.GroupCode,
                                                             GlobalAttributeGroupId = grp.GlobalAttributeGroupId,
                                                             DisplayOrder = map.DisplayOrder

                                                         }).ToList();
            ZnodeLogging.LogMessage("group list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, groupList?.Count());
            if (groupList?.Count < 1)
            {
                localeId = Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale);
                groupList = (from frm in _formBuilderRepository.Table
                             join map in _formMapperRepository.Table on frm.FormBuilderId equals map.FormBuilderId
                             join grp in _globalAttributeGroupRepository.Table on map.GlobalAttributeGroupId equals grp.GlobalAttributeGroupId
                             join loc in _attributeGroupLocaleRepository.Table on grp.GlobalAttributeGroupId equals loc.GlobalAttributeGroupId
                             where frm.FormBuilderId == formBuilderId && loc.LocaleId == localeId
                             orderby map.DisplayOrder
                             select new GlobalAttributeGroupModel
                             {
                                 AttributeGroupName = loc.AttributeGroupName,
                                 GroupCode = grp.GroupCode,
                                 GlobalAttributeGroupId = grp.GlobalAttributeGroupId,
                                 DisplayOrder = map.DisplayOrder

                             }).ToList();
                ZnodeLogging.LogMessage("group list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, groupList?.Count());
            }
            return groupList ?? new List<GlobalAttributeGroupModel>();
        }

        private FormBuilderAttributeGroupModel GetFormBuilderConfiguration(int formBuilderId, int localeId, int mappingid)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters formBuilderId, localeId, mappingid:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { formBuilderId, localeId, mappingid });

            FormBuilderAttributeGroupModel configuration = null;
            if (formBuilderId > 0)
            {
                configuration = (from frm in _formBuilderRepository.Table
                                 join cms in _formWidgetConfigurationRepository.Table on frm.FormBuilderId equals cms.FormBuilderId
                                 where frm.FormBuilderId == formBuilderId && cms.LocaleId == localeId && (mappingid > 0 ? cms.CMSMappingId == mappingid : true)
                                 select new FormBuilderAttributeGroupModel
                                 {
                                     FormBuilderId = frm.FormBuilderId,
                                     LocaleId = cms.LocaleId,
                                     FormCode = frm.FormCode,
                                     ButtonText = cms.ButtonText,
                                     TextMessage = cms.TextMessage,
                                     RedirectURL = cms.RedirectURL,
                                     IsTextMessage = cms.IsTextMessage.Value,
                                     IsShowCaptcha = cms.IsShowCaptcha.Value,
                                     FormTitle = cms.FormTitle
                                 })?.FirstOrDefault();
                ZnodeLogging.LogMessage("Configuration:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, configuration);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return configuration ?? new FormBuilderAttributeGroupModel() { FormBuilderId = formBuilderId };
        }

        //update the display order of respective item
        private bool ToggleDisplayOrder(int formBuilderId, int displayorder, bool isUpward)
        {
            bool isSuccess = false;
            List<ZnodeFormBuilderAttributeMapper> mapper = _formMapperRepository.Table.Where(x => x.FormBuilderId == formBuilderId).OrderBy(x => x.DisplayOrder).ToList();
            ZnodeLogging.LogMessage("mapper list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, mapper?.Count());
            try
            {
                int toggleOrder = 0;
                if (isUpward)
                    toggleOrder = mapper.Where(x => x.DisplayOrder < displayorder).Max(x => x.DisplayOrder).Value;
                else
                    toggleOrder = mapper.Where(x => x.DisplayOrder > displayorder).Min(x => x.DisplayOrder).Value;

                int mapperId = mapper.Where(x => x.DisplayOrder == toggleOrder).Select(x => x.FormBuilderAttributeMapperId)?.FirstOrDefault() ?? 0;

                if (mapperId > 0)
                {
                    ZnodeFormBuilderAttributeMapper map = _formMapperRepository.GetById(mapperId);
                    map.DisplayOrder = displayorder;
                    _formMapperRepository.Update(map);
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return isSuccess;
        }

        private List<int?> GetFormBuilderAttributes(int formbuilderId, List<int?> assignedAttributes)
        {
            List<int?> grpAttributes = (from frm in _formMapperRepository.Table
                                        join grp in _groupMapperRepository.Table on frm.GlobalAttributeGroupId equals grp.GlobalAttributeGroupId
                                        where frm.FormBuilderId == formbuilderId && frm.GlobalAttributeGroupId != null
                                        select (grp.GlobalAttributeId)).ToList() ?? null;
            ZnodeLogging.LogMessage("grpAttributes count ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { grpAttributes?.Count });

            if (grpAttributes?.Count > 0)
            {
                foreach (int? attr in grpAttributes)
                    assignedAttributes.Add(attr);
            }
            return assignedAttributes;
        }



        // Method is used to send email notification to customer
        protected virtual void SendEmailNotificationEmail(int notificationEmailTemplateId, int portalId, int localeId, string customerEmail, string storeLogo, List<string> attachments = null)
        {
            try
            {
                string emailTemplateCode = GetEmailTemplateCode(notificationEmailTemplateId);
                ZnodeLogging.LogMessage("emailTemplateCode returned from GetEmailTemplateCode ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { emailTemplateCode });

                EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(emailTemplateCode, portalId, localeId);
                if (IsNotNull(emailTemplateMapperModel))
                {
                    Dictionary<string, string> setDictionary = new Dictionary<string, string>
                        {
                        {"#CustomerServiceEmail#", ZnodeConfigManager.SiteConfig.CustomerServiceEmail.Replace(",", ", ")},
                        {"#CustomerServicePhoneNumber#", ZnodeConfigManager.SiteConfig.CustomerServicePhoneNumber},
                        {"#StoreLogo#", storeLogo}
                        };
                    string messageText = _emailTemplateSharedService.ReplaceTemplateTokens(emailTemplateMapperModel.Descriptions, setDictionary);
                    if (attachments?.Count > 0)
                    {
                        ZnodeEmail.SendEmail(customerEmail, ZnodeConfigManager.SiteConfig.AdminEmail, emailTemplateMapperModel.Subject, messageText, string.Empty, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, portalId, string.Empty), attachments, true);
                    }
                    else
                    {
                        ZnodeEmail.SendEmail(customerEmail, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, portalId, string.Empty), emailTemplateMapperModel.Subject, messageText, true);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
        }

        //Method is used to get email template code
        private string GetEmailTemplateCode(int emailTemplateId)
        {
            ZnodeLogging.LogMessage("emailTemplateId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { emailTemplateId });

            var emailTemplateCode = (from map in _emailTemplateMapperRepository.Table
                                     join area in _emailTemplateAreaRepository.Table on map.EmailTemplateAreasId equals area.EmailTemplateAreasId
                                     where map.EmailTemplateId == emailTemplateId
                                     select (area.Code)).FirstOrDefault();
            ZnodeLogging.LogMessage("Email Template Code:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, emailTemplateCode);
            return Convert.ToString(emailTemplateCode);
        }

        //Method is used to get email notification setting
        private FormWidgetEmailConfigurationModel FormNotificationSetting(int formBuilderId)
        {
            ZnodeLogging.LogMessage("formBuilderId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { formBuilderId });

            FormWidgetEmailConfigurationModel model = (from frm in _formWidgetEmailConfigurationRepository.Table
                                                       join map in _formWidgetConfigurationRepository.Table on frm.CMSContentPagesId equals map.CMSMappingId
                                                       where map.FormBuilderId == formBuilderId
                                                       select new FormWidgetEmailConfigurationModel
                                                       {
                                                           NotificationEmailId = frm.NotificationEmailId,
                                                           NotificationEmailTemplateId = frm.NotificationEmailTemplateId.Value,
                                                           AcknowledgementEmailTemplateId = frm.AcknowledgementEmailTemplateId.Value
                                                       }).FirstOrDefault() ?? new FormWidgetEmailConfigurationModel();
            ZnodeLogging.LogMessage("FormWidget Email Configuration Id:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, model?.FormWidgetEmailConfigurationId);
            return model;
        }

        #endregion
    }
}
