using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Services
{
    public class SearchReportService : BaseService, ISearchReportService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeSearchActivity> _znodeSearchActivityRepository;
        #endregion

        #region Public Constructor
        public SearchReportService()
        {
            _znodeSearchActivityRepository = new ZnodeRepository<ZnodeSearchActivity>();
        }
        #endregion

        #region Public methods

        //Get no result found keyword list.
        public SearchReportListModel GetNoResultsFoundReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Service method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            int portalId;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);

            //Remove portal id from filter.
            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);

            ServiceHelper.AddDateTimeValueInFilterByName(filters, "CreatedDate");

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<SearchReportModel> objStoredProc = new ZnodeViewRepository<SearchReportModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);

            IList<SearchReportModel> searchReportList = objStoredProc.ExecuteStoredProcedureList("Znode_GetKeywordSearchNoResultsFoundReport @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@PortalId", 4, out pageListModel.TotalRowCount);

            SearchReportListModel reportListModel = new SearchReportListModel();

            reportListModel.SearchReportList = searchReportList?.Count > 0 ? searchReportList?.ToList() : null;

            reportListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Service method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return reportListModel;
        }

        //Get top keyword search report.
        public SearchReportListModel GetTopKeywordsReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            int portalId;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);

            //Remove portal id filter.
            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);

            ServiceHelper.AddDateTimeValueInFilterByName(filters, "CreatedDate");

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<SearchReportModel> objStoredProc = new ZnodeViewRepository<SearchReportModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);

            IList<SearchReportModel> searchReportList = objStoredProc.ExecuteStoredProcedureList("Znode_GetTopKeywordSearchResult @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@PortalId", 4, out pageListModel.TotalRowCount);

            SearchReportListModel reportListModel = new SearchReportListModel();

            reportListModel.SearchReportList = searchReportList?.Count > 0 ? searchReportList?.ToList() : null;

            reportListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Service method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return reportListModel;
        }

        //Save search report data
        public SearchReportModel SaveSearchReportData(SearchReportModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeSearchActivity report = _znodeSearchActivityRepository.Insert(model.ToEntity<ZnodeSearchActivity>());

            if (report.SearchActivityId > 0)
            {
                model.SearchActivityId = report.SearchActivityId;
                ZnodeLogging.LogMessage($"Search report data save successfully for report id {model.SearchActivityId}", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return model;
            }
            else
            {
                ZnodeLogging.LogMessage($"Failed to save search report data.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return model;
            }

        }
        #endregion
    }
}
