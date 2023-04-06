using System;
using System.Collections.Generic;
using System.Linq;

using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class SearchReportAgent : BaseAgent, ISearchReportAgent
    {
        #region Private Variables
        private readonly ISearchReportClient _searchReportClient;
        #endregion

        #region Public Constructor
        public SearchReportAgent(ISearchReportClient searchReportClient)
        {
            _searchReportClient = GetClient<ISearchReportClient>(searchReportClient);
        }
        #endregion

        #region Public Method

        //Get tab structure for search activity report
        public SearchReportViewModel GetTabStructureSearchReport()
        {
            SearchReportViewModel viewModel = new SearchReportViewModel();
            TabViewListModel tabList = new TabViewListModel();
            SetTabData("Top Keywords", $"/Search/SearchReport/GetTopKeywordsReport", tabList);
            SetTabData("No Results Found", $"/Search/SearchReport/GetNoResultsFoundReport", tabList);

            tabList.MaintainAllTabData = false;

            viewModel.Tabs = tabList;
            return viewModel;
        }

        //Get no result found search report
        public SearchReportListViewModel GetNoResultsFoundReport(FilterCollection filters, SortCollection sorts, int page, int recordPerPage, int portalId, string portalName)
        {
            SelectDefaultPortalIfNotPresent(ref portalId, ref portalName);

            FilterCollection newFilter = new FilterCollection();
            newFilter.AddRange(filters);

            //Add portal id in filter collection.
            AddPortalIdInFilters(newFilter, portalId);

            DateRangePickerHelper.FormatFilterForDateTimeRange(newFilter, DateTimeRange.Last_30_Days.ToString(), string.Empty);

            SearchReportListModel searchReportList = _searchReportClient.GetNoResultsFoundReport(null, newFilter, sorts, page, recordPerPage);

            SearchReportListViewModel listViewModel = new SearchReportListViewModel { SearchReportList = searchReportList?.SearchReportList?.ToViewModel<SearchReportViewModel>().ToList() };

            BindStoreFilterValues(listViewModel, portalId, portalName);

            SetListPagingData(listViewModel, searchReportList);

            return searchReportList?.SearchReportList?.Count > 0 ? listViewModel : new SearchReportListViewModel() { SearchReportList = new List<SearchReportViewModel>(), PortalId = portalId, PortalName = portalName };
        }

        //Get top search keyword report.
        public SearchReportListViewModel GetTopKeywordsReport(FilterCollection filters, SortCollection sorts, int page, int recordPerPage, int portalId, string portalName)
        {

            SelectDefaultPortalIfNotPresent(ref portalId, ref portalName);

            FilterCollection newFilter = new FilterCollection();
            newFilter.AddRange(filters);

            //Add portal id in filter collection.
            AddPortalIdInFilters(newFilter, portalId);

            DateRangePickerHelper.FormatFilterForDateTimeRange(newFilter, DateTimeRange.Last_30_Days.ToString(), string.Empty);

            SearchReportListModel searchReportList = _searchReportClient.GetTopKeywordsReport(null, newFilter, sorts, page, recordPerPage);

            SearchReportListViewModel listViewModel = new SearchReportListViewModel { SearchReportList = searchReportList?.SearchReportList?.ToViewModel<SearchReportViewModel>().ToList() };

            BindStoreFilterValues(listViewModel, portalId, portalName);

            SetListPagingData(listViewModel, searchReportList);

            return searchReportList?.SearchReportList?.Count > 0 ? listViewModel : new SearchReportListViewModel() { SearchReportList = new List<SearchReportViewModel>(), PortalId = portalId, PortalName = portalName };
        }

        private void SelectDefaultPortalIfNotPresent(ref int portalId, ref string portalName)
        {
            if (portalId <= 0)
            {
                List<AutoComplete> portalList = ZnodeDependencyResolver.GetService<ITypeaheadAgent>()?.GetAutocompleteList("", ZnodeConstant.StoreList, "", "");

                if (HelperUtility.IsNotNull(portalList) && portalList.Count > 0)
                {
                    //Select Default portal from portal list.
                    AutoComplete defaultPortal = portalList.FirstOrDefault();

                    portalId = defaultPortal.Id;
                    portalName = defaultPortal.Name;
                }
            }
        }

        #endregion

        #region Private Method
        //Add Portal id in filter collection 
        private void AddPortalIdInFilters(FilterCollection filters, int portalId)
        {
            if (portalId > 0)
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase));
                filters.Add(new FilterTuple(FilterKeys.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            }
        }

        //Map store filter values in view model
        private void BindStoreFilterValues(SearchReportListViewModel orderListModel, int portalId, string portalName)
        {
            orderListModel.PortalName = string.IsNullOrEmpty(portalName) ? Admin_Resources.DefaultAllStores : portalName;
            orderListModel.PortalId = portalId;
        }
        #endregion
    }
}
