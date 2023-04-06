using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.Maps;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.WebStore.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.Agents
{
    public class ImportAgent : BaseAgent, IImportAgent
    {
        #region Private Variables
        private readonly IImportClient _importClient;
        #endregion

        #region Constructor
        public ImportAgent(IImportClient importClient)
        {
            _importClient = GetClient<IImportClient>(importClient);
        }
        #endregion

        #region Public Methods
        public virtual ImportViewModel DownloadShippingAddressTemplate(HttpResponseBase response)
        {
            if (IsAdminUser())
            {
                DownloadHelper helper = new DownloadHelper();
                ImportModel importModel = _importClient.GetDefaultTemplate(TemplateName.ImportShippingAddress.ToString());
                if (HelperUtility.IsNotNull(importModel))
                    helper.ExportDownload(_importClient.DownloadTemplate(importModel.ImportTypeId, 0)?.data, "2", response, ",", $"{importModel.ImportType }{WebStoreConstants.CSV}");
            }
            return new ImportViewModel();
        }

        //Import shipping address.
        public virtual ImportViewModel ImportShippingAddress(HttpPostedFileBase postedFile)
        {
            if (IsAdminUser())
            {
                string fileName = UploadImportFile(postedFile);
                bool isSuccess = false;
                if (HelperUtility.IsNotNull(postedFile))
                {
                    try
                    {
                        ImportModel importModel = _importClient.GetDefaultTemplate(TemplateName.ImportShippingAddress.ToString());
                        if (HelperUtility.IsNotNull(importModel))
                        {
                            isSuccess = _importClient.ImportData(ImportViewModelMap.ToModel(fileName, importModel));
                            RemoveTemporaryFiles(fileName);
                        }
                    }
                    catch (ZnodeException ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Warning);
                        //Remove the file if any error comes in.
                        RemoveTemporaryFiles(fileName);
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                    }
                }
                ImportViewModel importViewModel = new ImportViewModel();
                if (!isSuccess)
                {
                    importViewModel.HasError = true;
                    importViewModel.ErrorMessage = WebStore_Resources.ImportProcessFailed;
                }
                else
                {
                    importViewModel.SuccessMessage = WebStore_Resources.ImportProcessInitiated;
                }
                return importViewModel;
            }
            else
                return new ImportViewModel();

        }

        public virtual ImportViewModel DownloadUserTemplate(HttpResponseBase response)
        {
            if (IsAdminUser())
            {
                DownloadHelper helper = new DownloadHelper();

                ImportModel importModel = _importClient.GetDefaultTemplate(TemplateName.ImportB2BCustomer.ToString());
                if (HelperUtility.IsNotNull(importModel))
                    helper.ExportDownload(_importClient.DownloadTemplate(importModel.ImportTypeId, 0)?.data, "2", response, ",", $"{importModel.ImportType }{WebStoreConstants.CSV}");
            }

            return new ImportViewModel();
        }

        public virtual ImportViewModel ImportUsers(HttpPostedFileBase postedFile)
        {
            if (IsAdminUser())
            {
                string fileName = UploadImportFile(postedFile);
                bool isSuccess = false;
                if (HelperUtility.IsNotNull(postedFile))
                {
                    try
                    {
                        ImportModel importModel = _importClient.GetDefaultTemplate(TemplateName.ImportB2BCustomer.ToString());
                        if (HelperUtility.IsNotNull(importModel))
                        {
                            isSuccess = _importClient.ImportData(ImportViewModelMap.ToModel(fileName, importModel));
                            RemoveTemporaryFiles(fileName);
                        }
                    }
                    catch (ZnodeException ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Warning);
                        //Remove the file if any error comes in.
                        RemoveTemporaryFiles(fileName);
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                    }
                }
                ImportViewModel importViewModel = new ImportViewModel();
                if (!isSuccess)
                {
                    importViewModel.HasError = true;
                    importViewModel.ErrorMessage = WebStore_Resources.ImportProcessFailed;
                }
                else
                {
                    importViewModel.SuccessMessage = WebStore_Resources.ImportProcessInitiated;
                }

                return importViewModel;
            }
            return new ImportViewModel();
        }

        //This methodd will upload the file on the server.
        public virtual string UploadImportFile(HttpPostedFileBase file)
        {
            string temporaryFilePath = string.Empty;
            if (HelperUtility.IsNotNull(file))
            {
                try
                {
                    if (!string.IsNullOrEmpty(file.FileName) && Equals(Path.GetExtension(file.FileName), $".{WebStoreConstants.CSVFileType}"))
                    {
                        temporaryFilePath = GetFileNamewithPath(file.FileName);
                        file.SaveAs(temporaryFilePath);
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                    if (File.Exists(temporaryFilePath))
                        File.Delete(temporaryFilePath);
                }
            }
            return temporaryFilePath;
        }

        public virtual string GetFileNamewithPath(string fileName)
        {
            //Remove space and special characters from file name.
            fileName = Regex.Replace(fileName, WebStoreConstants.FileValidation, string.Empty);

            string pathwithFileName = $"{Path.GetFileNameWithoutExtension(fileName)}{"_"}{DateTime.Now.ToString("MMddyyyyHHmmsstt")}{"_"}{/*SessionProxyHelper.GetUserDetails()?.UserId*/ SessionHelper.GetDataFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId}{"."}{WebStoreConstants.CSVFileType}";
            return AppendFolderToFileName(pathwithFileName);
        }

        //This method will append the Import folder name against the file
        public virtual string AppendFolderToFileName(string fileName)
        {
            CreateFolderIfNotExists();
            return Path.Combine(HttpContext.Current.Server.MapPath(WebStoreConstants.ImportFolderPath), fileName);
        }

        //This method will create the directory to save the import files
        public virtual void CreateFolderIfNotExists()
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath(WebStoreConstants.ImportFolderPath)))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(WebStoreConstants.ImportFolderPath));
        }

        public virtual void RemoveTemporaryFiles(string fileName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(WebStoreConstants.ImportFolderPath));

            //Delete file from the directory.
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.FullName.Equals(fileName))
                {
                    file.Delete();
                    break;
                }
            }
        }

        public virtual UserViewModel GetUserViewModel() => GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey) ?? new UserViewModel();

        public virtual bool IsAdminUser() => string.Equals(GetUserViewModel().RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.InvariantCultureIgnoreCase);

        //Get shipping import logs.
        public virtual ImportProcessLogsListViewModel ImportShippingLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            ImportProcessLogsListViewModel listViewModel = GetImportLogs(expands, ref filters, sorts, pageIndex, pageSize, _importClient.GetDefaultTemplate(TemplateName.ImportShippingAddress.ToString()));

            // Set tool option menus for import process grid.
            SetImportListToolMenu(listViewModel);
            return listViewModel;
        }

        //Get import user logs.
        public virtual ImportProcessLogsListViewModel ImportUserLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            ImportProcessLogsListViewModel viewModel = GetImportLogs(expands, ref filters, sorts, pageIndex, pageSize, _importClient.GetDefaultTemplate(TemplateName.ImportB2BCustomer.ToString()));

            // Set tool option menus for user import process grid.
            SetImportListToolMenu(viewModel);
            return viewModel;
        }

        //Get import logs.
        public virtual ImportProcessLogsListViewModel GetImportLogs(ExpandCollection expands, ref FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize, ImportModel importModel)
        {
            if (HelperUtility.IsNull(importModel))
                return new ImportProcessLogsListViewModel() { ProcessLogs = new List<ImportProcessLogsViewModel>() };

            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            filters.Add(new FilterTuple(ZnodeUserEnum.CreatedBy.ToString(), FilterOperators.Equals, GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId.ToString()));
            filters.Add(new FilterTuple(ZnodeImportProcessLogEnum.ImportTemplateId.ToString(), FilterOperators.Equals, importModel.TemplateId.ToString()));

            ImportLogsListModel importLogsList = _importClient.GetImportLogs(expands, filters, sorts, pageIndex, pageSize);

            ImportProcessLogsListViewModel listViewModel = new ImportProcessLogsListViewModel { ProcessLogs = importLogsList?.ImportLogs?.ToViewModel<ImportProcessLogsViewModel>().ToList() };
            SetListPagingData(listViewModel, importLogsList);

            // Set tool option menus for import process grid.
            SetImportListToolMenu(listViewModel);

            return importLogsList?.ImportLogs?.Count > 0 ? listViewModel : new ImportProcessLogsListViewModel() { ProcessLogs = new List<ImportProcessLogsViewModel>() };
        }
        //Get the import log details.
        public virtual ImportLogsListViewModel GetImportLogDetails(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            if (HelperUtility.IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            }

            ImportLogDetailsListModel importLogsList = _importClient.GetImportLogDetails(importProcessLogId, expands, filters, sorts, pageIndex, pageSize);

            ImportLogsListViewModel listViewModel = new ImportLogsListViewModel { LogsList = importLogsList?.ImportLogDetails?.ToViewModel<ImportLogsViewModel>().ToList() };
            SetListPagingData(listViewModel, importLogsList);

            return importLogsList?.ImportLogDetails?.Count > 0 ? listViewModel : new ImportLogsListViewModel() { LogsList = new List<ImportLogsViewModel>() };
        }

        // Delete the Import logs
        public virtual bool DeleteLog(string importProcessLogIds) => _importClient.DeleteLogs(new ParameterModel { Ids = importProcessLogIds });

        //This method is to get the csv file headers.
        public virtual string GetCsvHeaders(HttpPostedFileBase file, out string fileName)
        {
            string headers = string.Empty;
            fileName = UploadImportFile(file);
            if (HelperUtility.IsNotNull(file))
            {
                using (var reader = new StreamReader(fileName))
                {
                    IEnumerable<string> firstLine = File.ReadLines(fileName).Take(1);
                    //get the headers from CSV file
                    if (HelperUtility.IsNotNull(firstLine))
                        headers = firstLine.First().ToString();
                }
            }
            return headers;
        }
        #endregion

        #region Private Methods
        //Set tool option menus for import grid.
        private void SetImportListToolMenu(ImportProcessLogsListViewModel listViewModel)
        {
            if (HelperUtility.IsNotNull(listViewModel))
            {
                listViewModel.GridModel = new GridModel();
                listViewModel.GridModel.FilterColumn = new FilterColumnListModel();
                listViewModel.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                listViewModel.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = WebStore_Resources.LinkTextDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ImportLogsDeletePopup')", ControllerName = "Import", ActionName = "DeleteLogs" });
            }
        }

        //Set tool option menus for user import grid.
        private static void SetImportUserListToolMenu(ImportProcessLogsListViewModel listViewModel)
        {
            if (HelperUtility.IsNotNull(listViewModel))
            {
                listViewModel.GridModel = new GridModel();
                listViewModel.GridModel.FilterColumn = new FilterColumnListModel();
                listViewModel.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                listViewModel.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = WebStore_Resources.LinkTextDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ImportUserLogsDeletePopup')", ControllerName = "Import", ActionName = "DeleteLogs" });
            }
        }
        #endregion

        #region Protected Methods
        //Import data according to template.
        protected virtual ImportViewModel ImportData(HttpPostedFileBase postedFile, ImportModel importModel)
        {
            string fileName = string.Empty;
            string header = GetCsvHeaders(postedFile, out fileName);
            fileName = string.IsNullOrEmpty(header) ? string.Empty : fileName;

            bool isSuccess = false;
            if (HelperUtility.IsNotNull(postedFile) && !string.IsNullOrEmpty(fileName))
            {
                try
                {
                    if (HelperUtility.IsNotNull(importModel))
                    {
                        isSuccess = _importClient.ImportData(ImportViewModelMap.ToModel(fileName, importModel));
                        RemoveTemporaryFiles(fileName);
                    }
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Warning);
                    //Remove the file if any error comes in.
                    RemoveTemporaryFiles(fileName);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                    RemoveTemporaryFiles(fileName);
                }
            }

            return !isSuccess ? new ImportViewModel() { HasError = true, ErrorMessage = WebStore_Resources.ImportProcessFailed } :
                new ImportViewModel() { SuccessMessage = WebStore_Resources.ImportProcessInitiated };
        }
        #endregion
    }
}
