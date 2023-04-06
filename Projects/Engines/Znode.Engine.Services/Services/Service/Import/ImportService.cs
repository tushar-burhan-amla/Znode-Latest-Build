using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using GenericParsing;
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
using Znode.Libraries.Admin.Import;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;

namespace Znode.Engine.Services
{
    public class ImportService : BaseService, IImportService
    {
        #region Private Variables
        private ApplicationUserManager _userManager;
        private readonly IZnodeRepository<ZnodeImportHead> _znodeImportHead;
        private readonly IZnodeRepository<ZnodeImportTemplate> _znodeImportTemplate;
        private readonly IZnodeRepository<ZnodeImportTemplateMapping> _znodeImportTemplateMapping;
        private readonly IZnodeRepository<ZnodeImportAccountDefaultTemplate> _importAccountDefaultTemplate;
        private readonly IZnodeRepository<ZnodeImportLog> _importLogDetails;
        private readonly IZnodeRepository<ZnodeImportProcessLog> _importLogs;
        private readonly IZnodeRepository<ZnodePimAttributeFamily> _znodeFamily;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IImportHelper importHelper;
        private readonly IUserService _userService;
        private readonly IZnodeRepository<ZnodeExportProcessLog> _znodeExportProcessLog;
        protected readonly IExportService _exportService;

        #endregion

        #region Constructor
        public ImportService()
        {
            _znodeImportHead = new ZnodeRepository<ZnodeImportHead>();
            _znodeImportTemplate = new ZnodeRepository<ZnodeImportTemplate>();
            _znodeImportTemplateMapping = new ZnodeRepository<ZnodeImportTemplateMapping>();
            _importLogDetails = new ZnodeRepository<ZnodeImportLog>();
            _importLogs = new ZnodeRepository<ZnodeImportProcessLog>();
            _znodeFamily = new ZnodeRepository<ZnodePimAttributeFamily>();
            _importAccountDefaultTemplate = new ZnodeRepository<ZnodeImportAccountDefaultTemplate>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            importHelper = ZnodeDependencyResolver.GetService<IImportHelper>();
            _userService = ZnodeDependencyResolver.GetService<IUserService>();
            _znodeExportProcessLog = new ZnodeRepository<ZnodeExportProcessLog>();
            _exportService = ZnodeDependencyResolver.GetService<IExportService>();
        }
        #endregion

        #region Public Methods
        //get all templates with respect to import head id
        public virtual ImportModel GetAllTemplates(int importHeadId, int familyId, int promotionTypeId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters importHeadId familyId and promotionTypeId values: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new object[] { importHeadId, familyId, promotionTypeId });

            if (importHeadId > 0)
            {
                //generate the filter for Import head id
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeImportTemplateEnum.ImportHeadId.ToString(), FilterOperators.Equals, Convert.ToString(importHeadId)));
                filters.Add(new FilterTuple(ZnodeImportTemplateEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));
                if (familyId.Equals(0))
                    filters.Add(new FilterTuple(ZnodeImportTemplateEnum.PimAttributeFamilyId.ToString(), FilterOperators.Equals, "Null"));
                else
                    filters.Add(new FilterTuple(ZnodeImportTemplateEnum.PimAttributeFamilyId.ToString(), FilterOperators.Equals, Convert.ToString(familyId)));
                if (promotionTypeId.Equals(0))
                    filters.Add(new FilterTuple(ZnodeImportTemplateEnum.PromotionTypeId.ToString(), FilterOperators.Equals, "Null"));
                else
                    filters.Add(new FilterTuple(ZnodeImportTemplateEnum.PromotionTypeId.ToString(), FilterOperators.Equals, Convert.ToString(promotionTypeId)));

                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("WhereClause generated to get importTemplates list: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);

                //pass the filter to get list of templates
                List<ZnodeImportTemplate> importTemplates = _znodeImportTemplate.GetEntityList(whereClauseModel.WhereClause, string.Empty).ToList();
                ZnodeLogging.LogMessage("importTemplates list count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, importTemplates?.Count());

                ImportModel model = new ImportModel();

                //bind template list in model
                model.TemplateList = new ImportTemplateListModel { TemplateList = importTemplates.ToModel<ImportTemplateModel>().ToList() };
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
                return model;
            }
            else
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
                return new ImportModel();
            }
        }

        //get complete list of import types available
        public virtual ImportModel GetImportTypeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to get list of import types available: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            List<ZnodeImportHead> importHead = _znodeImportHead.GetEntityList(pageListModel?.EntityWhereClause?.WhereClause)?.ToList();
            ZnodeLogging.LogMessage("importHead list count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, importHead?.Count());

            ImportModel model = new ImportModel();
            model.ImportTypeList = new ImportTypeListModel { ImportTypeList = importHead.ToModel<ImportTypeModel>().ToList() };
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return model;
        }

        //get all data of templates
        public virtual ImportModel GetTemplateData(int templateId, int importHeadId, int familyId, int promotionTypeId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters templateId, importHeadId , familyId and promotionTypeId : ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new object[] { templateId, importHeadId, familyId, promotionTypeId });

            //If template id greater than 0 then we will show template mappings.
            if (templateId > 0)
            {
                //generate the filter with Import template id
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeImportTemplateEnum.ImportTemplateId.ToString(), FilterOperators.Equals, Convert.ToString(templateId)));

                if (promotionTypeId > 0)
                {
                    return GetPromotionsTemplateData(null, templateId, importHeadId, familyId, promotionTypeId);
                }
                else
                {
                    if (familyId.Equals(0))
                        filters.Add(new FilterTuple(ZnodeImportTemplateEnum.PimAttributeFamilyId.ToString(), FilterOperators.Equals, "Null"));
                    else
                        filters.Add(new FilterTuple(ZnodeImportTemplateEnum.PimAttributeFamilyId.ToString(), FilterOperators.Equals, Convert.ToString(familyId)));
                    EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                    ZnodeLogging.LogMessage("WhereClause generated to get templates data: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);

                    //get the templates data from import template id
                    ZnodeImportTemplate importTemplate = _znodeImportTemplate.GetEntity(whereClauseModel.WhereClause);

                    filters = new FilterCollection();
                    filters.Add(new FilterTuple(ZnodeImportTemplateEnum.ImportTemplateId.ToString(), FilterOperators.Equals, Convert.ToString(templateId)));
                    whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                    ZnodeLogging.LogMessage("WhereClause generated to get all mappings with respect to templateId: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);

                    //use the above created filter to get all mappings with respect to template id
                    List<ZnodeImportTemplateMapping> mappings = _znodeImportTemplateMapping.GetEntityList(whereClauseModel.WhereClause).ToList();

                    //bind Template mappings and template data in model
                    ImportModel model = new ImportModel();
                    model.SelectedTemplate = importTemplate.ToModel<ImportTemplateModel>();
                    model.TemplateMappingList = new ImportTemplateMappingListModel { Mappings = mappings.ToModel<ImportTemplateMappingModel>().ToList() };
                    ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
                    return model;
                }
            }
            else
            {
                //If template id is 0 then we will show the attributes 

                //generate where clause
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeImportTemplateEnum.ImportHeadId.ToString(), FilterOperators.Equals, Convert.ToString(importHeadId)));

                //create object to fetch the records
                IZnodeViewRepository<ImportTemplateMappingModel> objStoredProc = new ZnodeViewRepository<ImportTemplateMappingModel>();
                objStoredProc.SetParameter("@ImportHeadId", importHeadId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PimAttributeFamilyId", familyId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PromotionTypeId", promotionTypeId, ParameterDirection.Input, DbType.Int32);

                //create list model
                ImportTemplateMappingListModel listModel = new ImportTemplateMappingListModel();

                //assign the SP result back to model
                List<ImportTemplateMappingModel> importTemplateMapping = objStoredProc.ExecuteStoredProcedureList("Znode_ImportGetDefaultFamilyAttribute @ImportHeadId, @PimAttributeFamilyId, @PromotionTypeId")?.ToList();
                ZnodeLogging.LogMessage("importTemplateMapping list count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, importTemplateMapping?.Count());

                listModel.Mappings = importTemplateMapping?.Count > 0 ? importTemplateMapping : new List<ImportTemplateMappingModel>();
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
                //return the model
                return new ImportModel { TemplateMappingList = listModel };
            }
        }

        // This method will fetch the data from file and insert it into DB and then inserted data will be processed.
        public virtual int ProcessData(ImportModel importModel)
        {
            try
            {
                int userId = GetLoginUserId();
                if (Equals(importModel.ImportType, ZnodeConstant.AdminUser))
                    return ImportAdminUsers(importModel.FileName);
                else
                    return importHelper.ProcessData(importModel, userId);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Failed to Process Import Data", ZnodeLogging.Components.Import.ToString(), TraceLevel.Error, ex);
                if (Equals(importModel.ImportType, ZnodeConstant.AdminUser))
                {
                    IZnodeRepository<ZnodeImportProcessLog> _importProcessLogRepository = new ZnodeRepository<ZnodeImportProcessLog>();
                    IZnodeRepository<ZnodeImportTemplate> _importTemplateRepository = new ZnodeRepository<ZnodeImportTemplate>();
                    IZnodeRepository<ZnodeImportLog> _importLogRepository = new ZnodeRepository<ZnodeImportLog>();

                    var importProcessLog = _importProcessLogRepository.Insert(new ZnodeImportProcessLog
                    {
                        ImportTemplateId = _importTemplateRepository.Table.FirstOrDefault(x => x.TemplateName == "AdminUserTemplate")?.ImportTemplateId,
                        Status = ZnodeConstant.SearchIndexFailedStatus,
                        ProcessStartedDate = DateTime.UtcNow,
                        ProcessCompletedDate = DateTime.UtcNow.AddMinutes(1.0)
                    });

                    _importLogRepository.Insert(new ZnodeImportLog
                    {
                        ImportProcessLogId = importProcessLog.ImportProcessLogId,
                        ErrorDescription = ex.Message,
                        Guid = Convert.ToString(new Guid()),

                    });
                }
                throw new ZnodeException(ErrorCodes.ImportError, ex.Message);
            }
        }

        // Downloads the model
        public virtual DownloadModel DownLoadTemplate(int importHeadId, int familyId, int promotionTypeId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters importHeadId, familyId and promotionTypeId values: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new object[] { importHeadId, familyId, promotionTypeId });
            DownloadModel model = new DownloadModel();
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            executeSpHelper.GetParameter("@importHeadId", importHeadId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@PimAttributeFamilyId", familyId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@PromotionTypeId", promotionTypeId, ParameterDirection.Input, SqlDbType.Int);
            DataSet ds = executeSpHelper.GetSPResultInDataSet("Znode_ImportGetDefaultFamilyAttribute");
            model.data = ConvertImportDataSetToList(ds);
            ZnodeLogging.LogMessage("ConvertImportDataSetToList list count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, model?.data?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return model;
        }

        //Get the import log details on the basis of Import Log Id
        public virtual ImportLogDetailsListModel GetImportLogDetails(int importProcessLogId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter importProcessLogId: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, importProcessLogId);

            //check if filter is blank.
            if (IsNull(filters))
                filters = new FilterCollection();

            //add the filter
            filters.Add(new FilterTuple(ZnodeImportProcessLogEnum.ImportProcessLogId.ToString(), FilterOperators.Equals, Convert.ToString(importProcessLogId)));

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<ImportLogDetailsModel> objStoredProc = new ZnodeViewRepository<ImportLogDetailsModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<ImportLogDetailsModel> importLogList = objStoredProc.ExecuteStoredProcedureList("Znode_GetImportProcessLog @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("ImportLogDetailsModel list count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, importLogList?.Count());

            ImportLogDetailsListModel model = new ImportLogDetailsListModel { ImportLogDetails = importLogList?.ToList() };
            model.ImportLogs = GetCurrentLogStatus(importProcessLogId);
            model.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return model;
        }

        //Get the Import Logs - Get the template name and ImportProcessLogId order by ImportProcessLogId desc
        public virtual ImportLogsListModel GetImportLogs(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            if (IsNull(expands))
                expands = new NameValueCollection();

            List<string> navigationProperties = new List<string>();

            SetExpands(ExpandKeys.ZnodeImportTemplate, navigationProperties);

            IZnodeViewRepository<ImportLogsModel> objStoredProc = new ZnodeViewRepository<ImportLogsModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<ImportLogsModel> importLogList = objStoredProc.ExecuteStoredProcedureList("Znode_GetImportTemplateLogs @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("ImportLogsModel list count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, importLogList?.Count());
            ImportLogsListModel model = new ImportLogsListModel { ImportLogs = importLogList?.ToList() };
            model.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            return model;
        }

        //Get the import log status
        public virtual ImportLogsListModel GetLogStatus(int importProcessLogId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter importProcessLogId: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, importProcessLogId);

            ImportLogsModel model = new ImportLogsModel();
            model = _importLogs.GetById(importProcessLogId).ToModel<ImportLogsModel>();
            ZnodeLogging.LogMessage("ImportLogsModel: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, model);
            ImportLogsListModel listModel = new ImportLogsListModel();
            listModel.ImportLogs = new List<ImportLogsModel>();
            listModel.ImportLogs.Add(model);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return listModel;
        }

        public virtual ImportLogsModel GetCurrentLogStatus(int importProcessLogId)
        {
            IZnodeViewRepository<ImportLogsModel> objStoredProc = new ZnodeViewRepository<ImportLogsModel>();
            objStoredProc.SetParameter("@ImportProcessLogId", importProcessLogId, ParameterDirection.Input, DbType.Int32);
            IList<ImportLogsModel> importLogList = objStoredProc.ExecuteStoredProcedureList("Znode_GetImportLogs @ImportProcessLogId");
            return importLogList?.FirstOrDefault();
        }

        // Delete the import logs
        public virtual bool DeleteLogDetails(ParameterModel importProcessLogIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(importProcessLogIds?.Ids))
                return false;
            ZnodeLogging.LogMessage("Input parameter importProcessLogIds to be deleted: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, importProcessLogIds?.Ids);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@ImportProcessLogId", importProcessLogIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> result = objStoredProc.ExecuteStoredProcedureList("Znode_ImportRemoveLogs @ImportProcessLogId,  @Status OUT", 1, out status);
            ZnodeLogging.LogMessage("Deleted result count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, result?.Count());

            return result.FirstOrDefault().Status.Value;
        }

        //Get all families for product import
        public virtual ImportModel GetFamilies(bool isCategory)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimAttributeFamilyEnum.IsCategory.ToString(), FilterOperators.Equals, Convert.ToString(isCategory)));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to get familyEntityList: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);
            List<ZnodePimAttributeFamily> familyEntityList = _znodeFamily.GetEntityList(whereClauseModel.WhereClause)?.ToList();
            ZnodeLogging.LogMessage("familyEntityList list count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, familyEntityList?.Count());

            ImportModel model = new ImportModel();
            model.FamilyList = new ImportProductFamilyListModel { FamilyList = familyEntityList.ToModel<ImportProductFamilyModel>().ToList() };
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return model;
        }

        public virtual int UpdateTemplateMappings(ImportModel model) => importHelper.UpdateTemplateData(model);

        //check the import status
        public virtual bool CheckImportStatus() => importHelper.CheckImportStatus();

        public virtual ImportModel GetDefaultTemplate(string templateName)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(templateName))
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ImportTypeNotEmpty);
            ZnodeLogging.LogMessage("Input parameter templateName: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, templateName);

            ImportModel importModel = (from defaultTemplate in _importAccountDefaultTemplate.Table
                                       join importHead in _znodeImportHead.Table on defaultTemplate.ImportHeadId equals importHead.ImportHeadId
                                       join importTemplate in _znodeImportTemplate.Table on importHead.ImportHeadId equals importTemplate.ImportHeadId
                                       where defaultTemplate.TemplateName == templateName.Trim()
                                       select new ImportModel
                                       {
                                           ImportTypeId = defaultTemplate.ImportHeadId,
                                           TemplateId = importTemplate.ImportTemplateId,
                                           TemplateName = importTemplate.TemplateName,
                                           ImportType = importHead.Name

                                       })?.FirstOrDefault() ?? new ImportModel();
            ZnodeLogging.LogMessage("TemplateId, ImportType and ImportTypeId properties of importModel: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new object[] { importModel?.TemplateId, importModel?.ImportType, importModel?.ImportTypeId });

            importModel.TemplateMappingList = new ImportTemplateMappingListModel { Mappings = _znodeImportTemplateMapping.Table.Where(w => w.ImportTemplateId == importModel.TemplateId).ToModel<ImportTemplateMappingModel>().ToList() };
            return importModel ?? new ImportModel();
        }

        //Get custom import template list. It will not return the system defined import template.
        public virtual ImportManageTemplateListModel GetCustomImportTemplateList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            if (IsNull(expands))
                expands = new NameValueCollection();

            List<string> navigationProperties = new List<string>();

            SetExpands(ExpandKeys.ZnodeImportTemplate, navigationProperties);

            IZnodeViewRepository<ImportManageTemplateModel> objStoredProc = new ZnodeViewRepository<ImportManageTemplateModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<ImportManageTemplateModel> importManageTemplatesList = objStoredProc.ExecuteStoredProcedureList("Znode_GetImportTemplates @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("ImportManageTemplateModel list count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, importManageTemplatesList?.Count());
            ImportManageTemplateListModel model = new ImportManageTemplateListModel { ImportManageTemplates = importManageTemplatesList?.ToList() };
            model.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return model;
        }

        // Deletes the custom import templates. It will not delete the system defined import templates.
        public virtual bool DeleteImportTemplate(ParameterModel importTemplateIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(importTemplateIds?.Ids))
                return false;
            ZnodeLogging.LogMessage("Input parameter custom import template to be deleted: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, importTemplateIds?.Ids);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@importTemplateId", importTemplateIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> result = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteImportTemplates @ImportTemplateId,  @Status OUT", 1, out status);
            ZnodeLogging.LogMessage("Deleted result count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, result?.Count());

            return result.FirstOrDefault().Status.Value;
        }

        // Export the import data in excel,csv and pdf format.
        public virtual ExportModel GetAllFormsListForExport(string fileType, int importProcessLogId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetFormSubmissionExportDataSetResponse:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            ExportModel exportModel = new ExportModel();
            //Check Export file insertion is in progress or not.
            if (IsExportPublishInProgress())
            {
                exportModel.Message = PIM_Resources.ErrorPublishCatalog;
                return exportModel;
            }

            exportModel = GetFormSubmissionExportDataSetResponse(fileType, importProcessLogId, filters, pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return exportModel;
        }

        public virtual ExportModel GetFormSubmissionExportDataSetResponse(string fileType, int importProcessLogId, FilterCollection filters, PageListModel pageListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ExportModel exportModel = new ExportModel();
            string whereClause = GenerateWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("pageListModel to set SP parameters ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { whereClause });
            //Create a thread to start export process of import. #Step 1
            HttpContext httpContext = HttpContext.Current;
            Action threadWorker = delegate ()
            {
                HttpContext.Current = httpContext;
                try
            {
                String FileType = fileType;
                ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
                objStoredProc.GetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, SqlDbType.NVarChar);
                objStoredProc.GetParameter("@FileType", FileType, ParameterDirection.Input, SqlDbType.NVarChar);
                objStoredProc.GetParameter("@ImportProcessLogId", importProcessLogId, ParameterDirection.Input, SqlDbType.Int);
                objStoredProc.GetParameter("@Status", null, ParameterDirection.Output, SqlDbType.Int);
                int status = 0;
                DataSet data = objStoredProc.GetSPResultInDataSet("Znode_ExportImportErrorLog");
                string tableName = Convert.ToString(data.Tables[0].Rows[0]["TableName"]);
                int counts = Convert.ToInt32(data.Tables[1].Rows[0]["Count"]);
                string strFilePath = ZnodeConstant.ExportFolderPath;
                string filePath = _exportService.CreateFilePath(strFilePath, tableName);
                int exportChunkSize = Convert.ToInt32(ConfigurationManager.AppSettings["ZnodeImportExportChunkLimit"].ToString());
                int loopValue = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(counts) / Convert.ToDouble(exportChunkSize)));
                int chunkSizeForLastLoop = counts % exportChunkSize;
                int loopCount = 1;
                string file = "";
                int tempRowCount = 0;
                int exportChunkSizePDF = HelperUtility.IsNull(chunkSizeForLastLoop) ? Convert.ToInt32(ConfigurationManager.AppSettings["ZnodeImportExportChunkLimit"].ToString()) : Convert.ToInt32(chunkSizeForLastLoop);
                string values = tableName;
                string output = values.Replace("ImportErrorLog", "");
                output = values.Replace("_", "");
                output = Regex.Replace(output, "[0-9]{2,}", "");
                output = Regex.Replace(output, @"[\d-]", string.Empty);

                    for (int fileInChunk = 1; fileInChunk <= loopValue; fileInChunk++)
                  {
                    DataTable dataTables = GetExportData(pageListModel, filters, tableName, loopCount, exportChunkSize);

                            dataTables.Columns.Remove("ImportProcessLogId");
                            dataTables.Columns["Column Name"].ColumnName = "CSV Column Name";
                            dataTables.Columns["Column Data"].ColumnName = "CSV Column Data";

                        if (fileType != "PDF")
                            {
                                if (fileInChunk != loopValue)
                                {
                                     if(fileType=="Excel")
                                    {
                                       file = _exportService.CreateFilePath(strFilePath, tableName) + "/" + (Equals("ImportErrorLog", ZnodeConstant.ExportType) ? $"{ output}_{loopCount}.xls" : $"{output}_{loopCount}.xls");
                                    }
                                    else
                                    {
                                      file = _exportService.CreateFilePath(strFilePath, tableName) + "/" + (Equals("ImportErrorLog", ZnodeConstant.ExportType) ? $"{ output}_{loopCount}.csv" : $"{output}_{loopCount}.csv");
                                    }
                                    WriteCsvOrExcel(dataTables,file);
                                }
                                else
                                {
                                    if (fileType == "Excel")
                                   {
                                     file = _exportService.CreateFilePath(strFilePath, tableName) + "/" + (Equals("ImportErrorLog", ZnodeConstant.ExportType) ? $"{ output}_{loopCount}.xls" : $"{output}_{loopCount}.xls");
                                   }
                                   else
                                   {
                                      file = _exportService.CreateFilePath(strFilePath, tableName) + "/" + (Equals("ImportErrorLog", ZnodeConstant.ExportType) ? $"{ output}_{loopCount}.csv" : $"{output}_{loopCount}.csv");
                                   }
                                   WriteCsvOrExcel(dataTables, file);
                                }
                            }
                            else
                            {
                                if (fileInChunk != loopValue)
                                {

                                    file = _exportService.CreateFilePath(strFilePath, tableName) + "/" + (Equals("ImportErrorLog", ZnodeConstant.ExportType) ? $"{ output}_{loopCount}.pdf" : $"{output}_{loopCount}.pdf");
                                    string htmlContent = ConvertDataTableToHTML(dataTables);
                                    PDFExport(file, htmlContent);
                                }
                                else
                                {
                                    file = _exportService.CreateFilePath(strFilePath, tableName) + "/" + (Equals("ImportErrorLog", ZnodeConstant.ExportType) ? $"{ output}_{loopCount}.pdf" : $"{output}_{loopCount}.pdf");
                                    string htmlContent = ConvertDataTableToHTML(dataTables);
                                    PDFExport(file, htmlContent);
                                }
                            }
                    loopCount++;
                }
                _exportService.GetZipFile(filePath, tableName);
                _exportService.DeleteTemporaryCreatedFolder(strFilePath, tableName);
                //insert status into the znode export process log table.
                InsertStatusIntoExportTable(tableName);
                exportModel.Message = Api_Resources.ExportSuccessMessage;
                exportModel.HasError = false;
            }
            catch (Exception e)
            {
              //insert status into the znode export process log table.
              InsertStatusIntoExportTableFailed();
              exportModel.Message = Api_Resources.ExportErrorMessage;
              exportModel.HasError = true;
            }
            };
            AsyncCallback callBack = new AsyncCallback(ExportSuccessCallBack);
            threadWorker.BeginInvoke(callBack, null);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return exportModel;
        }

        //Generate XML where clause for SP filters.
        public static string GenerateWhereClauseForSP(FilterDataCollection filters)
        {
            return DynamicClauseHelper.GenerateWhereClauseForSP(filters);
        }

        public void PDFExport(string file, string htmlContent)
        {
            byte[] pdfBytes;
            NReco.PdfGenerator.HtmlToPdfConverter htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
            using (MemoryStream Stream = new MemoryStream(pdfBytes))
            {
                byte[] bytes = pdfBytes;
                FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
        }

        public static void WriteCsvOrExcel(DataTable dt, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(",", dt.Columns.Cast<DataColumn>().Select(dc => dc.ColumnName)));
                foreach (DataRow row in dt.Rows)
                {
                    writer.WriteLine(string.Join(",", row.ItemArray));
                }
            }
        }

        public static string ConvertDataTableToHTML(DataTable dt)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<html>");
            builder.Append("<head>");
            builder.Append("<title>");
            builder.Append("Page-");
            builder.Append(Guid.NewGuid());
            builder.Append("</title>");
            builder.Append("</head>");
            builder.Append("<body>");
            builder.Append("<table border='1px' cellpadding='7' cellspacing='0' width='100%' height='auto'");
            builder.Append("style='border: 1px; font-size: 15px;'>");
            builder.Append("<tr align='left' valign='top' display='block'>");
            foreach (DataColumn c in dt.Columns)
            {
                builder.Append("<td align='left' valign='top'><b>");
                builder.Append(c.ColumnName);
                builder.Append("</b></td>");
            }
            builder.Append("</tr>");
            foreach (DataRow r in dt.Rows)
            {
                builder.Append("<tr align='left' valign='top' display='block'>");
                foreach (DataColumn c in dt.Columns)
                {
                    builder.Append("<td align='left' valign='top'>");
                    builder.Append(r[c.ColumnName]);
                    builder.Append("</td>");
                }
                builder.Append("</tr>");
            }
            builder.Append("</table>");
            builder.Append("</body>");
            builder.Append("</html>");

            return builder.ToString();
        }

        //Check Export file insertion is in progress or not.
        public virtual bool IsExportPublishInProgress()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            byte publishStateId = Convert.ToByte(ZnodePublishStatesEnum.PROCESSING);

            bool isExportPublishInProgress = _znodeExportProcessLog.Table.Any(x => x.Status == ZnodeConstant.ExportStatusInprogress || x.Status == ZnodeConstant.SearchIndexStartedStatus);


            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isExportPublishInProgress;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method convert the Dataset to List
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns>List<dynamic></returns>
        private List<dynamic> ConvertImportDataSetToList(DataSet ds)
        {
            var tblPivot = new DataTable();
            for (int iCount = 0; iCount < ds.Tables[0].Rows.Count; iCount++)
                tblPivot.Columns.Add(Convert.ToString(ds.Tables[0].Rows[iCount][0]));

            return ConvertToList(tblPivot);
        }

        /// <summary>
        /// This method will convert Datatable to list
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>List<dynamic></returns>
        private List<dynamic> ConvertToList(DataTable dt)
        {
            var dynamicDt = new List<dynamic>();

            for (int i = 0; i < 1; i++)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = "";
                }
            }
            return dynamicDt;
        }

        private int ImportAdminUsers(string fileName)
        {
            DataTable dt = new DataTable();
            try
            {
                //Read data from file and create Data Table.
                using (GenericParserAdapter parser = new GenericParserAdapter(fileName))
                {
                    parser.ColumnDelimiter = ZnodeConstant.ColumnDelimiter;
                    parser.FirstRowHasHeader = true;
                    dt = parser.GetDataTable();
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                throw ex;
            }
            //throw ZnodeException if more than 100 Admin Users are getting imported at a time.
            if (dt?.Rows.Count > 100)
                throw new ZnodeException(ErrorCodes.ImportError, "It is recommended to import 100 Admin Users at a time.");

            //Import Admin Users.
            foreach (var item in dt.ToList<UserModel>())
            {
                item.User = new LoginUserModel { Username = item.UserName, Email = item.UserName };
                item.Email = item.UserName;

                if (!string.IsNullOrEmpty(item.StoreCode))
                {
                    FilterCollection filters = new FilterCollection();
                    filters.Add(ZnodePortalEnum.StoreCode.ToString(), FilterOperators.In, !string.IsNullOrEmpty(item.StoreCode) ? string.Join(",", item.StoreCode.Split(',')?.ToArray()?.Select(x => $"\"{x}\"")) : null);
                    item.PortalIds = _portalRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection())?.WhereClause)?.ToList()?.Select(x => x.PortalId.ToString())?.ToArray();
                }
                else
                    item.PortalIds = new string['0'];

                _userService.CreateAdminUser(item);
            }

            IZnodeRepository<ZnodeImportProcessLog> _importProcessLogRepository = new ZnodeRepository<ZnodeImportProcessLog>();
            IZnodeRepository<ZnodeImportTemplate> _importTemplateRepository = new ZnodeRepository<ZnodeImportTemplate>();

            _importProcessLogRepository.Insert(new ZnodeImportProcessLog
            {
                ImportTemplateId = _importTemplateRepository.Table.FirstOrDefault(x => x.TemplateName == "AdminUserTemplate")?.ImportTemplateId,
                Status = ZnodeConstant.CompletedStatus,
                ProcessStartedDate = DateTime.UtcNow,
                ProcessCompletedDate = DateTime.UtcNow.AddMinutes(1.0)
            });
            return 1;
        }

        private DataTable GetExportData(PageListModel pageListModel, FilterCollection filters, string tableName, int loopCnt, int exportChunkSize)
        {
            pageListModel.PagingLength = exportChunkSize;
            pageListModel.PagingStart = loopCnt;
            string whereClause = GenerateWhereClauseForSP(filters.ToFilterDataCollection());
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@TableName", tableName, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, SqlDbType.Int);
            var data = objStoredProc.GetSPResultInDataSet("Znode_ExportImportErrorLogByTableName");
            DataTable datatable = data.Tables[0];
            return datatable;
        }

        //update status in znode export process log table after file insertion completion.
        private void InsertStatusIntoExportTable(string tableName)
        {
            ZnodeExportProcessLog znodeExportProcessLog = _znodeExportProcessLog.Table.FirstOrDefault(x => x.TableName == tableName);
            if (IsNotNull(znodeExportProcessLog))
            {
                znodeExportProcessLog.Status = ZnodeConstant.CompletedExportStatus;
                znodeExportProcessLog.ProcessCompletedDate = GetDateWithTime();
                //To update the status in znode export process table.
                _znodeExportProcessLog.Update(znodeExportProcessLog);
            }
        }

        private void InsertStatusIntoExportTableFailed()
        {
            ZnodeExportProcessLog znodeExportProcessLog = _znodeExportProcessLog.Table.FirstOrDefault(x => x.Status == "In Progress");
            if (IsNotNull(znodeExportProcessLog))
            {
                znodeExportProcessLog.Status = ZnodeConstant.FailedExportStatus;
                znodeExportProcessLog.ProcessCompletedDate = GetDateWithTime();
                //To update the status in znode export process table.
                _znodeExportProcessLog.Update(znodeExportProcessLog);
            }
        }
        #endregion

        #region Protected Methods
        // Get all promotions template mappings.
        protected virtual ZnodeImportTemplate GetPromotionsTemplateMapping( int promotionTypeId = 0)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeImportTemplateEnum.PromotionTypeId.ToString(), FilterOperators.Equals, Convert.ToString(promotionTypeId)));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to get templates data: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);

            //Get the templates data from import template id.
             return _znodeImportTemplate.GetEntity(whereClauseModel.WhereClause);   
        }

        // Get all data of promotions templates.
        protected virtual ImportModel GetPromotionsTemplateData(ZnodeImportTemplate importTemplate, int templateId, int importHeadId, int familyId, int promotionTypeId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters templateId, importHeadId , familyId and promotionTypeId : ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new object[] { templateId, importHeadId, familyId, promotionTypeId });

            //Create object to fetch the records.
            IZnodeViewRepository<ZnodeImportTemplateMapping> objStoredProc = new ZnodeViewRepository<ZnodeImportTemplateMapping>();
            objStoredProc.SetParameter("@ImportTemplateId", templateId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PimAttributeFamilyId", familyId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PromotionTypeId", promotionTypeId, ParameterDirection.Input, DbType.Int32);

            //Assign the SP result back to model.
            List<ZnodeImportTemplateMapping> znodeImportTemplateMapping = objStoredProc.ExecuteStoredProcedureList("Znode_ImportGetTemplateMapping @ImportTemplateId, @PimAttributeFamilyId, @PromotionTypeId")?.ToModel<ZnodeImportTemplateMapping>().ToList();

            //Get the promotion templates data by promotionTypeId.
            ZnodeImportTemplate znodeImportTemplate = GetPromotionsTemplateMapping(promotionTypeId);

            //Bind Template mappings and template data in model.
            ImportModel importModel = new ImportModel();
            importModel.SelectedTemplate = znodeImportTemplate.ToModel<ImportTemplateModel>();
            importModel.TemplateMappingList = new ImportTemplateMappingListModel { Mappings = znodeImportTemplateMapping.ToModel<ImportTemplateMappingModel>().ToList() };
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return importModel;
        }

        //Export Success Call Back.
        protected virtual void ExportSuccessCallBack(IAsyncResult ar)
        {
            AsyncResult result = ar as AsyncResult;
        }
        #endregion
    }
}
