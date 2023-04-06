using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Resources;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;
using Znode.Libraries.Data.Helpers;
using Znode.Engine.Services.Maps;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace Znode.Engine.Services
{
    public class FormSubmissionService : BaseService, IFormSubmissionService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeFormBuilder> _formBuilderRepository;
        private readonly IZnodeRepository<ZnodeGlobalAttributeGroup> _globalAttributeGroupRepository;
        private readonly IZnodeRepository<ZnodeFormBuilderAttributeMapper> _formMapperRepository;
        private readonly IZnodeRepository<ZnodeGlobalAttributeGroupLocale> _attributeGroupLocaleRepository;
        private readonly IZnodeRepository<ZnodeFormBuilderSubmit> _formSubmitRepository;
        private readonly IZnodeRepository<ZnodeExportProcessLog> _znodeExportProcessLog;
        protected readonly IExportService _exportService;
        #endregion

        #region Public Constructor
        public FormSubmissionService()
        {
            _formBuilderRepository = new ZnodeRepository<ZnodeFormBuilder>();
            _globalAttributeGroupRepository = new ZnodeRepository<ZnodeGlobalAttributeGroup>();
            _formMapperRepository = new ZnodeRepository<ZnodeFormBuilderAttributeMapper>();
            _attributeGroupLocaleRepository = new ZnodeRepository<ZnodeGlobalAttributeGroupLocale>();
            _formSubmitRepository = new ZnodeRepository<ZnodeFormBuilderSubmit>();
            _znodeExportProcessLog = new ZnodeRepository<ZnodeExportProcessLog>();
            _exportService = GetService<IExportService>();
        }
        #endregion

        #region Public Method
        //Get Form Submission list.
        public virtual FormSubmissionListModel GetFormSubmissionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            IZnodeViewRepository<FormSubmissionModel> objStoredProc = new ZnodeViewRepository<FormSubmissionModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            List<FormSubmissionModel> formSubmissionList = objStoredProc.ExecuteStoredProcedureList("Znode_GetFormBuilderSubmitList @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT", 4, out pageListModel.TotalRowCount).ToList();
            ZnodeLogging.LogMessage("formSubmissionList count ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { formSubmissionList?.Count });

            FormSubmissionListModel listModel = new FormSubmissionListModel();
            listModel.FormSubmissionList = formSubmissionList?.Count > 0 ? formSubmissionList : new List<FormSubmissionModel>();
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listModel;
        }

        //Get Form Submission Details
        public virtual FormBuilderAttributeGroupModel GetFormSubmitDetails(int formSubmitId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            FormBuilderAttributeGroupModel model = new FormBuilderAttributeGroupModel();
            if (formSubmitId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            ZnodeLogging.LogMessage("Input parameters formSubmitId ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { formSubmitId });

            int localeId = _formSubmitRepository.GetById(formSubmitId)?.LocaleId ?? 0;
            ZnodeLogging.LogMessage("localeId to set parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { localeId });

            localeId = localeId == 0 ? Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale) : localeId;

            IZnodeViewRepository<GlobalAttributeValuesModel> globalAttributeValues = new ZnodeViewRepository<GlobalAttributeValuesModel>();
            globalAttributeValues.SetParameter(ZnodeFormBuilderSubmitEnum.FormBuilderId.ToString(), 0, ParameterDirection.Input, DbType.Int32);
            globalAttributeValues.SetParameter(ZnodeFormBuilderSubmitEnum.UserId.ToString(), 0, ParameterDirection.Input, DbType.Int32);
            globalAttributeValues.SetParameter(ZnodeFormBuilderSubmitEnum.PortalId.ToString(), 0, ParameterDirection.Input, DbType.Int32);
            globalAttributeValues.SetParameter(ZnodeFormBuilderSubmitEnum.FormBuilderSubmitId.ToString(), formSubmitId, ParameterDirection.Input, DbType.Int32);
            globalAttributeValues.SetParameter(ZnodeFormBuilderSubmitEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
            model.Attributes = globalAttributeValues.ExecuteStoredProcedureList("Znode_GetFormBuilderGlobalAttributeValue @FormBuilderId,@UserId,@PortalId,@FormBuilderSubmitId,@LocaleId").ToList();
            model.Groups = GetFormAssociatedGroup(formSubmitId);
            ZnodeLogging.LogMessage("Groups count returned from GetFormAssociatedGroup :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { model?.Groups?.Count });

            GetFormBuilderDetails(formSubmitId, model);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return model;
        }

        //Get Response For Export Form Submission in form of Export Model
        public virtual ExportModel GetAllFormsListForExport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string exportType)
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

            exportModel = GetFormSubmissionExportDataSetResponse(filters, pageListModel, exportType);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return exportModel;
        }

        //Get Response For Export Form Submission in form of Export Model from sp
        public virtual ExportModel GetFormSubmissionExportDataSetResponse(FilterCollection filters, PageListModel pageListModel, string exportType)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ExportModel exportModel = new ExportModel();
            string whereClause = GenerateWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("pageListModel to set SP parameters ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { whereClause });
            //Create a thread to start export process of formsubmission. #Step 1
            HttpContext httpContext = HttpContext.Current;
            Action threadWorker = delegate ()
            {
                HttpContext.Current = httpContext;
                try
                {
                    String FileType = exportType;
                    ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
                    objStoredProc.GetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, SqlDbType.NVarChar);
                    objStoredProc.GetParameter("@FileType", FileType, ParameterDirection.Input, SqlDbType.NVarChar);
                    objStoredProc.GetParameter("@Status", null, ParameterDirection.Output, SqlDbType.Int);
                    int status = 0;
                    DataSet data = objStoredProc.GetSPResultInDataSet("Znode_ExportFormSubmissions");
                    string tableName = Convert.ToString(data.Tables[0].Rows[0]["TableName"]);
                    int counts = Convert.ToInt32(data.Tables[1].Rows[0]["Count"]);
                    string strFilePath = ZnodeConstant.ExportFolderPath;
                    string filePath = _exportService.CreateFilePath(strFilePath, tableName);
                    int exportChunkSize = Convert.ToInt32(ConfigurationManager.AppSettings["ZnodeExportChunkLimit"].ToString());
                    int loopValue = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(counts) / Convert.ToDouble(exportChunkSize)));
                    int chunkSizeForLastLoop = counts % exportChunkSize;
                    int loopCount = 1;
                    for (int fileInChunk = 1; fileInChunk <= loopValue; fileInChunk++)
                    {
                        DataTable dataTables = GetDataFromSP(pageListModel, filters, tableName, loopCount, exportChunkSize);
                        if (fileInChunk != loopValue)
                        {
                            _exportService.SaveFile(dataTables, exportType, "FormSubmission", tableName, loopCount, null);
                        }
                        else
                        {
                            _exportService.SaveFile(dataTables, exportType, "FormSubmission", tableName, loopCount, chunkSizeForLastLoop);
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


        #endregion

        #region Private Method

        //Get Form Associated Group
        private List<GlobalAttributeGroupModel> GetFormAssociatedGroup(int formSubmitId)
        {

            List<GlobalAttributeGroupModel> groupList = (from formSubmit in _formSubmitRepository.Table
                                                         join frm in _formBuilderRepository.Table on formSubmit.FormBuilderId equals frm.FormBuilderId
                                                         join map in _formMapperRepository.Table on frm.FormBuilderId equals map.FormBuilderId
                                                         join grp in _globalAttributeGroupRepository.Table on map.GlobalAttributeGroupId equals grp.GlobalAttributeGroupId
                                                         join loc in _attributeGroupLocaleRepository.Table on grp.GlobalAttributeGroupId equals loc.GlobalAttributeGroupId
                                                         where formSubmit.FormBuilderSubmitId == formSubmitId
                                                         && loc.LocaleId == formSubmit.LocaleId
                                                         orderby map.DisplayOrder
                                                         select new GlobalAttributeGroupModel
                                                         {
                                                             AttributeGroupName = loc.AttributeGroupName,
                                                             GroupCode = grp.GroupCode,
                                                             GlobalAttributeGroupId = grp.GlobalAttributeGroupId,
                                                             DisplayOrder = map.DisplayOrder

                                                         }).ToList();
            if (groupList?.Count < 1)
            {
                int localeId = Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale);
                groupList = (from formSubmit in _formSubmitRepository.Table
                             join frm in _formBuilderRepository.Table on formSubmit.FormBuilderId equals frm.FormBuilderId
                             join map in _formMapperRepository.Table on frm.FormBuilderId equals map.FormBuilderId
                             join grp in _globalAttributeGroupRepository.Table on map.GlobalAttributeGroupId equals grp.GlobalAttributeGroupId
                             join loc in _attributeGroupLocaleRepository.Table on grp.GlobalAttributeGroupId equals loc.GlobalAttributeGroupId
                             where formSubmit.FormBuilderSubmitId == formSubmitId
                           && loc.LocaleId == localeId
                             orderby map.DisplayOrder
                             select new GlobalAttributeGroupModel
                             {
                                 AttributeGroupName = loc.AttributeGroupName,
                                 GroupCode = grp.GroupCode,
                                 GlobalAttributeGroupId = grp.GlobalAttributeGroupId,
                                 DisplayOrder = map.DisplayOrder

                             }).ToList();
            }
            return groupList ?? new List<GlobalAttributeGroupModel>();
        }

        //Get FormBuilder Details By formSubmissionId
        private void GetFormBuilderDetails(int formSubmissionId, FormBuilderAttributeGroupModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Input parameters formSubmissionId ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { formSubmissionId });

            FormBuilderModel formDetails = (from sub in _formSubmitRepository.Table
                                            join frm in _formBuilderRepository.Table on sub.FormBuilderId equals frm.FormBuilderId
                                            where sub.FormBuilderSubmitId == formSubmissionId
                                            select new FormBuilderModel
                                            {
                                                FormBuilderId = frm.FormBuilderId,
                                                FormCode = frm.FormCode

                                            })?.FirstOrDefault() ?? null;
            if (IsNotNull(formDetails))
            {
                model.FormBuilderId = formDetails.FormBuilderId;
                model.FormCode = formDetails.FormCode;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

        }

        //Get Data Table From SP. 
        private DataTable GetDataFromSP(PageListModel pageListModel, FilterCollection filters, string tableName, int loopCnt, int exportChunkSize)
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
            var data = objStoredProc.GetSPResultInDataSet("Znode_ExportFormSubmissionByTableName");
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
                //To update the status in znode export process table.
                _znodeExportProcessLog.Update(znodeExportProcessLog);
            }
        }

        //Export Success Call Back.
        protected virtual void ExportSuccessCallBack(IAsyncResult ar)
        {
            AsyncResult result = ar as AsyncResult;
        }

        #endregion

        #region Protected Method
        //Check Export file insertion is in progress or not.
        protected virtual bool IsExportPublishInProgress()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            byte publishStateId = Convert.ToByte(ZnodePublishStatesEnum.PROCESSING);

            bool isExportPublishInProgress = _znodeExportProcessLog.Table.Any(x => x.Status == ZnodeConstant.ExportStatusInprogress || x.Status == ZnodeConstant.SearchIndexStartedStatus);


            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isExportPublishInProgress;
        }
        #endregion
    }
}
