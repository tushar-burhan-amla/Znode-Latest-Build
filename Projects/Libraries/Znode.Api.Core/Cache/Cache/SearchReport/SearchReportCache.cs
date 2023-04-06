using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class SearchReportCache : BaseCache, ISearchReportCache
    {
        #region Private Variable
        private ISearchReportService _searchReportService;
        #endregion

        #region Public Constructor
        public SearchReportCache(ISearchReportService searchReportService)
        {
            _searchReportService = searchReportService;
        }
        #endregion

        #region Public Methods
        //Get no result found search keyword list.
        public string GetNoResultsFoundReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (HelperUtility.IsNull(data))
            {
                SearchReportListModel list = _searchReportService.GetNoResultsFoundReport(Expands, Filters, Sorts, Page);

                if (list?.SearchReportList?.Count > 0)
                {
                    //Create response.
                    SearchReportListResponse response = new SearchReportListResponse { SearchReportList = list.SearchReportList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get top search keyword list.
        public string GetTopKeywordsReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (HelperUtility.IsNull(data))
            {
                SearchReportListModel list = _searchReportService.GetTopKeywordsReport(Expands, Filters, Sorts, Page);

                if (list?.SearchReportList?.Count > 0)
                {
                    //Create response.
                    SearchReportListResponse response = new SearchReportListResponse { SearchReportList = list.SearchReportList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
