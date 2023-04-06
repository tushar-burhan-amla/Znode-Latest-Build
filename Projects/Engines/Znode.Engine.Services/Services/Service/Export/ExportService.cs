using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class ExportService : BaseService, IExportService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeExportProcessLog> _znodeExportProcessLog;
        #endregion

        #region Public Constructor
        public ExportService()
        {
            _znodeExportProcessLog = new ZnodeRepository<ZnodeExportProcessLog>();
        }
        #endregion

        //Get the Export Logs  ExportProcessLogId order by ExportProcessLogId desc
        public virtual ExportLogsListModel GetExportLogs(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            if (HelperUtility.IsNotNull(expands))
                expands = new NameValueCollection();

            IZnodeViewRepository<ExportLogsModel> objStoredProc = new ZnodeViewRepository<ExportLogsModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<ExportLogsModel> ExportLogList = objStoredProc.ExecuteStoredProcedureList("Znode_GetExportLogs @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("ExportLogsModel list count: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, ExportLogList?.Count());
            ExportLogsListModel model = new ExportLogsListModel { ExportLogs = ExportLogList?.ToList() };
            model.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            return model;
        }

        // Delete export files.
        public virtual bool DeleteExportFiles(ParameterModel exportProcessLogIds)
        {
            {
                ExportModel exportModel = new ExportModel();
                List<string> list = GetExportDelete(exportProcessLogIds);
                DeleteExportFiles(list, exportModel);
                if (!exportModel.HasError)
                {
                    return true;
                }
                else
                {
                    ZnodeLogging.LogMessage("Method : DeleteExpiredExportFiles  Message: Export Files Delete Failed  Error: ");
                    return false;
                }
            }
        }

        // Delete outdated export files.
        public virtual bool DeleteExpiredExportFiles()
        {
            ExportModel exportModel = new ExportModel();
            List<string> list = GetExportFileNamesList();
            DeleteExportFiles(list, exportModel);
            if (!exportModel.HasError)
            {
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage("Method : DeleteExpiredExportFiles  Message: Export Files Delete Failed  Error: ");
                return false;
            }
        }

        //Get Export files names list which are deleted from database.
        public virtual List<string> GetExportFileNamesList()
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            IZnodeViewRepository<string> objStoredProc = new ZnodeViewRepository<string>();
            int durationInDays = ZnodeApiSettings.ExportFileDeletionDuration;
            int exportProcessLogIds = 0;
            objStoredProc.SetParameter("@DurationInDays", durationInDays, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ExportProcessLogIds", exportProcessLogIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<string> ExportLogList = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteExportLogs @DurationInDays, @exportProcessLogIds, @Status OUT", 1, out status);
            IList<string> namesList = ExportLogList;
            return namesList.ToList();
        }

        public virtual void SaveFile(DataTable dtDataTable, string exportType, string type, string tableName, int loopCount, int? chunkSizeForRemainingData)
        {
            string strFilePath = ZnodeConstant.ExportFolderPath;
            //Normal file name based on type
            string fileType = type;
            //Folder Name and Zip name based on type and date time.
            string folderName = tableName;
            int exportChunkSize = HelperUtility.IsNull(chunkSizeForRemainingData) ? Convert.ToInt32(ConfigurationManager.AppSettings["ZnodeExportChunkLimit"].ToString()) : Convert.ToInt32(chunkSizeForRemainingData);
            try
            {
                int tempRowCount = 0;
                //Save files into ChunkSize
                if (dtDataTable.Rows.Count > 0)
                {
                    for (int fileInChunk = 1; fileInChunk <= (dtDataTable.Rows.Count); fileInChunk++)
                    {
                        string file = CreateFilePath(strFilePath, folderName) + "/" + (Equals(exportType, ZnodeConstant.ExportType) ? $"{fileType}_{loopCount}.xls" : $"{fileType}_{loopCount}.csv");

                        StreamWriter sw = new StreamWriter(file, false);
                        //headers
                        if (dtDataTable.Columns.Count > 0)
                        {
                            for (int i = 0; i < dtDataTable.Columns.Count; i++)
                            {
                                sw.Write(dtDataTable.Columns[i]);
                                if (i < dtDataTable.Columns.Count - 1)
                                {
                                    sw.Write(",");
                                }
                            }
                        }
                        sw.Write(sw.NewLine);
                        for (int j = tempRowCount; j < (tempRowCount + exportChunkSize); j++)
                        {
                            DataRow dr = dtDataTable.Rows[j];
                            for (int i = 0; i < dtDataTable.Columns.Count; i++)
                            {
                                if (!Convert.IsDBNull(dr[i]))
                                {
                                    string value = dr[i].ToString();
                                    if (value.Contains(','))
                                    {
                                        value = String.Format("\"{0}\"", value);
                                        sw.Write(value);
                                    }
                                    else
                                    {
                                        sw.Write(Convert.ToString(dr[i]));
                                    }
                                }
                                if (i < dtDataTable.Columns.Count - 1)
                                {
                                    sw.Write(",");
                                }
                            }
                            sw.Write(sw.NewLine);
                        }
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
        }
        //Creates Path for File
        public virtual string CreateFilePath(string path, string folderName)
        {
            CreateTypeFolderIfNotExists(path, folderName);
            return Path.Combine(HttpContext.Current.Server.MapPath(path), folderName);
        }

        //Creates Type Folder if not exist. 
        public virtual void CreateTypeFolderIfNotExists(string folderpath, string folderName)
        {
            if (!Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(folderpath), folderName)))
                Directory.CreateDirectory(Path.Combine(HttpContext.Current.Server.MapPath(folderpath), folderName));
        }

        public virtual void GetZipFile(string filePath, string folderName)
        {
            string zipFilePath = CreateZipPath(ZnodeConstant.ExportFolderPath);

            ZnodeLogging.LogMessage("Service method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { filePath = zipFilePath, fileName = folderName });

            //Create a zip file to download.
            Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
            zip.AddDirectory(filePath);
            zip.Save($"{ zipFilePath}/{folderName}.zip");

            //Dispose the zip file object.
            zip.Dispose();

            ZnodeLogging.LogMessage("Service method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }


        //Deletes the temporary created folder after creation of zip.
        public virtual void DeleteTemporaryCreatedFolder(string strFilePath, string folderName)
        {
            if (Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(strFilePath), folderName).ToString()))
                Directory.Delete(new FileInfo(Path.Combine(HttpContext.Current.Server.MapPath(strFilePath), folderName)).ToString(), true);
        }
        //Get the value of Billing Account Number Global Attribute
        public string GetExportFilePath(string tableName)
        {
            //Check Export file insertion is in progress or not.
            if (IsExportPublishInProgress(tableName))
            {
                string Message = Api_Resources.ExportDownloadErrorMessage;
                return Message;
            }

            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            if (Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(ZnodeConstant.ExportFolderPath))))
                return Path.Combine(HttpContext.Current.Server.MapPath(ZnodeConstant.ExportFolderPath));

            return null;
        }

        //Check Export file insertion is in progress or not.
        protected virtual bool IsExportPublishInProgress(string tableName)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);

            string isExportInProgress = _znodeExportProcessLog.Table?.FirstOrDefault(x => x.TableName == tableName)?.Status;
            if (string.Equals(isExportInProgress, ZnodeConstant.ExportStatusInprogress, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return false;
        }

        //Creates Zip Path
        protected virtual string CreateZipPath(string path)
        {
            CreateFolderIfNotExistsForZip(path);
            return Path.Combine(HttpContext.Current.Server.MapPath(path));
        }

        //Creates Zip Folder if not exist. 
        protected virtual void CreateFolderIfNotExistsForZip(string folderpath)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath(folderpath)))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folderpath));
        }

        //Deletes the zip file after deletion from the database.
        protected virtual ExportModel DeleteExportFiles(IList<string> fileNames, ExportModel model)
        {
            string strFilePath = ZnodeConstant.ExportFolderPath;
            string path = (Path.Combine(HttpContext.Current.Server.MapPath(strFilePath)).ToString());
            try
            {
                if (HelperUtility.IsNotNull(fileNames))
                {
                    foreach (var name in fileNames)
                    {
                        string filePath = string.Concat(path, "\\", name, ".zip");
                        if (Directory.Exists(Path.Combine(HttpContext.Current.Server.MapPath(strFilePath)).ToString()))
                            File.Delete(filePath);
                    }
                }
                model.HasError = false;
            }
            catch (Exception e)
            {
                model.HasError = true;
            }

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return model;
        }
        //Get Export files names list which are deleted from database.
        protected virtual List<string> GetExportDelete(ParameterModel exportProcessLogIds)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            IZnodeViewRepository<string> objStoredProc = new ZnodeViewRepository<string>();
            int durationInDays = 0;
            objStoredProc.SetParameter("@DurationInDays", durationInDays, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ExportProcessLogIds", exportProcessLogIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<string> ExportLogList = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteExportLogs @DurationInDays, @ExportProcessLogIds,  @Status OUT", 2, out status);
            IList<string> namesList = ExportLogList;
            return namesList.ToList();
        }

    }
}
