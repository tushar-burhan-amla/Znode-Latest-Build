using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Constants;
using Znode.Engine.Services.Maps;
using ZNode.Libraries.Data;
using ZNode.Libraries.Data.DataModel;
using ZNode.Libraries.Data.Helpers;
using ZNode.Libraries.ECommerce.Utilities;
using ZNode.Libraries.Framework.Business;
using static ZNode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public class ImportLogsService : BaseService, IImportLogsService
    {
        #region Private Variables
        private IZnodeRepository<ZnodeImportLog> _importLogDetails;
        private IZnodeRepository<ZnodeImportProcessLog> _importLogs;
        #endregion

        #region Constructor
        public ImportLogsService()
        {
            _importLogDetails = new ZnodeRepository<ZnodeImportLog>();
            _importLogs = new ZnodeRepository<ZnodeImportProcessLog>();
        }
        #endregion

        #region Public Methods
        //Get the import log details on the basis of Import Log Id
        public ImportLogDetailsListModel GetImportLogDetails(int importProcessLogId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            //check if filter is blank.
            if (IsNull(filters))
                filters = new FilterCollection();

            //add the filter
            filters.Add(new FilterTuple(ZnodeImportProcessLogEnum.ImportProcessLogId.ToString(), FilterOperators.Equals, Convert.ToString(importProcessLogId)));

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<ImportLogDetailsModel> objStoredProc = new ZnodeViewRepository<ImportLogDetailsModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<ImportLogDetailsModel> importLogList = objStoredProc.ExecuteStoredProcedureList("Znode_GetImportProcessLog @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);

            ImportLogDetailsListModel model = new ImportLogDetailsListModel { ImportLogDetails = importLogList?.ToList() };

            model.BindPageListModel(pageListModel);
            return model;
        }

        //Get the Import Logs - Get the template name and ImportProcessLogId order by ImportProcessLogId desc
        public ImportLogsListModel GetImportLogs(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            if (IsNull(sorts))
                sorts = new NameValueCollection();

            sorts.Add(ZnodeImportProcessLogEnum.ImportProcessLogId.ToString(), SortKeys.Descending);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

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
            ImportLogsListModel model = new ImportLogsListModel { ImportLogs = importLogList?.ToList() };

            model.BindPageListModel(pageListModel);
            return model;
        }

        //Get the import log status
        public ImportLogsListModel GetLogStatus(int importProcessLogId)
        {
            ImportLogsModel model = new ImportLogsModel();
            model = _importLogs.GetById(importProcessLogId).ToModel<ImportLogsModel>();

            ImportLogsListModel listModel = new ImportLogsListModel();
            listModel.ImportLogs = new List<ImportLogsModel>();
            listModel.ImportLogs.Add(model);

            return listModel;
        }

        // Delete the import logs
        public bool DeleteLogDetails(int importProcessLogId)
        {
            //Generates filter clause for multiple HighlightIds.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeImportProcessLogEnum.ImportProcessLogId.ToString(), ProcedureFilterOperators.Equals, Convert.ToString(importProcessLogId)));

            //Returns true if ZnodeImportLog mapper table entries deleted sucessfully else return false.
            bool IsDeleted = _importLogDetails.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZNodeLogging.LogMessage(IsDeleted ? "Import log mapper deleted successfully." : "Failed to delete import log mapper.", ZNodeLogging.Components.Import.ToString());

            //Returns true if highlight deleted sucessfully else return false.
            IsDeleted = _importLogs.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZNodeLogging.LogMessage(IsDeleted ? "Import logs deleted successfully." : "Failed to delete import log.", ZNodeLogging.Components.Import.ToString());

            return IsDeleted;
        }
        #endregion

    }
}
