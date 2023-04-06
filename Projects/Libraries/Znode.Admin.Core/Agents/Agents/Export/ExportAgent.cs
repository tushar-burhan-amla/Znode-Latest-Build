using System.Collections.Generic;
using System.Web;
using Znode.Engine.Admin.Helpers;
using System.Dynamic;
using System.Linq;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;
using System.Web.Mvc;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client;
using Znode.Admin.Core.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;
using System;
using System.IO;

namespace Znode.Engine.Admin.Agents
{
    public class ExportAgent : BaseAgent, IExportAgent
    {
        #region Private Variables
        private IProductAgent _productAgent;
        private IMediaManagerAgent _mediaAgent;
        private IImportAgent _importAgent;
        private IExportClient _exportclient;
        private IFormSubmissionAgent _formSubmissionAgent;
        private readonly IExportClient _exportClient;
        public ExportAgent(IExportClient exportclient)
        {
            _exportclient = GetClient<IExportClient>(exportclient);
        }
        #endregion

        #region Public Method
        //Get Export Data List
        public virtual List<dynamic> GetExportList(string type, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int folderId = 0, int localId = 0, int pimCatalogId = 0, string catalogName = null, int paramId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", string.Empty, TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            List<dynamic> exportList = new List<dynamic>();
            switch (type)
            {
                case AdminConstants.Product:
                    _productAgent = DependencyResolver.Current.GetService<IProductAgent>();
                    ProductDetailsListViewModel productListModel = new ProductDetailsListViewModel() { TotalResults = 1 };
                    //PageIndex = 1 to get all product details by pageIndex
                    exportList.AddRange(GetAllProductList(productListModel, filters, sorts, 1, localId, pimCatalogId, catalogName));
                    break;
                case AdminConstants.Media:
                    _mediaAgent = DependencyResolver.Current.GetService<IMediaManagerAgent>();
                    MediaManagerListViewModel mediaManagerViewModel = _mediaAgent.GetMedias(filters, sorts, null, null, folderId);
                    exportList.AddRange(mediaManagerViewModel.MediaList);
                    break;
                case AdminConstants.ImportErrorLogDetails:
                    _importAgent = DependencyResolver.Current.GetService<IImportAgent>();
                    ImportLogsDownloadListViewModel importLogsDownloadListViewModel = _importAgent.DownloadImportLogDetails(paramId, null,  filters,  sorts,pageIndex.GetValueOrDefault(), pageSize.GetValueOrDefault());
                    exportList.AddRange(importLogsDownloadListViewModel.LogsList);
                    break;
                default:
                    break;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", string.Empty, TraceLevel.Info);
            return exportList;
        }

        //Create data source.
        public virtual List<dynamic> CreateDataSource(List<dynamic> dataModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);

            List<dynamic> xmlConfigurableList = GetFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey);
            List<dynamic> _sortedXmlList = GetFilteredXmlList(xmlConfigurableList);
            List<dynamic> _resultList = new List<dynamic>();

            dataModel.ForEach(row =>
            {
                IDictionary<string, object> columnObject = new ExpandoObject();
                _sortedXmlList.ForEach(col =>
                {
                    columnObject.Add(col.headertext, GetColumnData(row, col));
                });
                _resultList.Add(columnObject);
            });

            ZnodeLogging.LogMessage("Agent method execution done.", string.Empty, TraceLevel.Info);
            return _resultList;
        }


        //Get Export log lists.
        public virtual ExportProcessLogsListViewModel GetExportLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands, filters and sorts", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            ExportLogsListModel exportLogsList = _exportclient.GetExportLogs(expands, filters, sorts, pageIndex, pageSize);

            ExportProcessLogsListViewModel listViewModel = new ExportProcessLogsListViewModel { ProcessLogs = exportLogsList?.ExportLogs?.ToViewModel<ExportProcessLogsViewModel>().ToList() };
            SetListPagingData(listViewModel, exportLogsList);

            //Set tool option menus for Export process grid.
            SetExportListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            return exportLogsList?.ExportLogs?.Count > 0 ? listViewModel : new ExportProcessLogsListViewModel() { ProcessLogs = new List<ExportProcessLogsViewModel>() };
        }

        //Get Export file details by id.
        public virtual string DownloadExportFile(string tableName)
        { 
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            string filePath = _exportclient.GetExportFilePath(tableName);
            if(string.Equals(filePath, Admin_Resources.ExportDownloadErrorMessage, StringComparison.InvariantCultureIgnoreCase))
            {
                string Message = Api_Resources.ExportDownloadErrorMessage;
                return Message;
            }
            if (HelperUtility.IsNotNull(filePath))
            {
                return string.Concat(filePath,"\\",tableName);
            }
            return null;
        }

        //Get Zip File by filePath. 
        public virtual byte[] GetZipFile(string filePath)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Import.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.Import.ToString(), TraceLevel.Verbose, new { filePath = filePath, });
            // Read File Bytes.
            byte[] data = File.ReadAllBytes($"{ filePath}.zip");
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return data;
        }

        //Get Export Form Submission Response Message.
        public virtual ExportResponseMessageModel GetExportFormSubmissionList(string exportType, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int folderId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", string.Empty, TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            _formSubmissionAgent = DependencyResolver.Current.GetService<IFormSubmissionAgent>();
            FormSubmissionListViewModel formSubmissionListModel = new FormSubmissionListViewModel() { TotalResults = 1 };
            //PageIndex = 1 to get all Form Submission details by pageIndex
            ExportResponseMessageModel exportFormSubmissionMessage = _formSubmissionAgent.GetExportFormSubmissionList(exportType, filters, sorts, pageIndex);
            ZnodeLogging.LogMessage("Agent method execution done.", string.Empty, TraceLevel.Info);
            return exportFormSubmissionMessage;
        }
        // Delete the Export logs
        public virtual bool DeleteLog(string exportProcessLogIds) => _exportclient.DeleteLogs(new ParameterModel { Ids = exportProcessLogIds });


        /// get import details for csv,excel and pdf download.
        public virtual ExportResponseMessageModel GetExportLogList(string fileType, string type, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int folderId = 0, int localId = 0, int pimCatalogId = 0, string catalogName = null, int paramId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", string.Empty, TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            _importAgent = DependencyResolver.Current.GetService<IImportAgent>();
            ImportLogsListViewModel ExportListModel = new ImportLogsListViewModel() { TotalResults = 1 };
            //PageIndex = 1 to get all Form Submission details by pageIndex
            ExportResponseMessageModel importLogMessage = _importAgent.GetImportErrorLogList(fileType, paramId, null, filters, sorts, pageIndex.GetValueOrDefault(), pageSize.GetValueOrDefault());
                ZnodeLogging.LogMessage("Agent method execution done.", string.Empty, TraceLevel.Info);
                return importLogMessage;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get All Product List
        /// </summary>
        /// <param name="productListModel">ProductListModel</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="pageIndex">Page Index</param>
        /// <returns> </returns>
        private IEnumerable<dynamic> GetAllProductList(ProductDetailsListViewModel productListModel, FilterCollection filters, SortCollection sorts, int? pageIndex, int localId, int pimCatalogId = 0, string catalogName = null)
        {
            List<dynamic> exportList = new List<dynamic>();
            // Get the record from Product agent
            exportList = _productAgent.GetExportProductList(filters, sorts, pageIndex, ZnodeAdminSettings.ProductExportChunkSize, localId);
            return exportList;
        }
        /// <summary>
        /// Get Column Data by type
        /// </summary>
        /// <param name="row">row element</param>
        /// <param name="col">col element</param>
        /// <returns>row value</returns>
        private object GetColumnData(dynamic row, dynamic col)
        {
            if (HelperUtility.IsNotNull(row.GetType().GetProperty(col.name)))
                return row.GetType().GetProperty(col.name).GetValue(row, null);

            return row[col.name];
        }
        /// <summary>
        /// Get Filtered XmlList
        /// </summary>
        /// <param name="XMLConfigurableList">XmlConfigurationList</param>
        /// <returns>Filtered XmlConfigurationList</returns>
        private List<dynamic> GetFilteredXmlList(List<dynamic> XMLConfigurableList)
        {
            string[] excludedColumn = { "Action", "Checkbox" };
            return XMLConfigurableList?.FindAll(x => x.isvisible.Equals(DynamicGridConstants.Yes) && !excludedColumn.Contains((string)x.headertext)
                                               && !x.name.Equals("Image") && (!HelperUtility.IsPropertyExist(x, "isattributecolumn") || !x.isattributecolumn.Equals(DynamicGridConstants.Yes))
                                                );
        }
        private static void SetExportListToolMenu(ExportProcessLogsListViewModel listViewModel)
        {
            if (HelperUtility.IsNotNull(listViewModel))
            {
                listViewModel.GridModel = new GridModel();
                listViewModel.GridModel.FilterColumn = new FilterColumnListModel();
                listViewModel.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                listViewModel.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ExportLogsDeletePopup')", ControllerName = "Export", ActionName = "DeleteLogs" });
            }
        }
    }

    #endregion
}


