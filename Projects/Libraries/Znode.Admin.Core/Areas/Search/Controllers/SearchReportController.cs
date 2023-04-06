using System.Web.Mvc;

using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Areas.Search.Controllers
{
    public class SearchReportController : BaseController
    {
        #region Private Variable
        private readonly ISearchReportAgent _searchReportAgent;
        #endregion

        #region Public Controller
        public SearchReportController(ISearchReportAgent searchReportAgent)
        {
            _searchReportAgent = searchReportAgent;
        }
        #endregion

        #region Public Methods
        public virtual ActionResult GetTopKeywordsReport([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId = 0, string portalName = "")
        {
            //Remove DateTimeRange Filter From Cookie.
            DateRangePickerHelper.RemoveDateTimeRangeFiltersFromCookies(GridListType.SearchTopKeywordsReport.ToString(), model);

            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.SearchTopKeywordsReport.ToString(), model);

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.SearchTopKeywordsReport.ToString(), model);

            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            //Get the list of top keywords
            SearchReportListViewModel topKeywordReportList = _searchReportAgent.GetTopKeywordsReport(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, portalId, portalName);

            topKeywordReportList.GridModel = FilterHelpers.GetDynamicGridModel(model, topKeywordReportList.SearchReportList, GridListType.SearchTopKeywordsReport.ToString(), string.Empty, null, true, true, topKeywordReportList?.GridModel?.FilterColumn?.ToolMenuList);

            topKeywordReportList.GridModel.TotalRecordCount = topKeywordReportList.TotalResults;

            return ActionView(topKeywordReportList);
        }

        //get no result found report list.
        public virtual ActionResult GetNoResultsFoundReport([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int portalId = 0, string portalName = "")
        {
            //Remove DateTimeRange Filter From Cookie.
            DateRangePickerHelper.RemoveDateTimeRangeFiltersFromCookies(GridListType.SearchTopKeywordsReport.ToString(), model);

            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.SearchNoResultsFoundReport.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.SearchNoResultsFoundReport.ToString(), model);

            //Assign global filters in filter if exists.
            SetGlobalSearchFilter(model);

            SearchReportListViewModel noResultsFoundReportList = _searchReportAgent.GetNoResultsFoundReport(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, portalId, portalName);

            noResultsFoundReportList.GridModel = FilterHelpers.GetDynamicGridModel(model, noResultsFoundReportList.SearchReportList, GridListType.SearchNoResultsFoundReport.ToString(), string.Empty, null, true, true, noResultsFoundReportList?.GridModel?.FilterColumn?.ToolMenuList);

            noResultsFoundReportList.GridModel.TotalRecordCount = noResultsFoundReportList.TotalResults;

            return ActionView(noResultsFoundReportList);
        }

        //Get tab structure for search report
        [HttpGet]
        public virtual ActionResult GetTabStructureSearchReport()
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView("GetTabsForSearchReport", _searchReportAgent.GetTabStructureSearchReport());
        }
        #endregion
    }
}
