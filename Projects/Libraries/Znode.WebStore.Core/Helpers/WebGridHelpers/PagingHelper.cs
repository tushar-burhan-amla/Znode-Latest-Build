using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.Helpers
{
    public static class PagingHelper
    {
        /// <summary>
        /// Gets the pager html
        /// </summary>
        /// <param name="grid">web grid model to invoke this extension method</param>
        /// <param name="html">html</param>
        /// <param name="recordPerPageFieldName">record page page name</param>
        /// <param name="pageFieldName">page </param>
        /// <param name="SortFieldName">sort name</param>
        /// <param name="SortDirFieldName">sort direction name</param>
        /// <returns>Gets the partial view</returns>
        public static MvcHtmlString CustomPager(this WebGrid grid, HtmlHelper html, string recordPerPageFieldName = "recordPerPage", string pageFieldName = "page", string SortFieldName = "sort", string SortDirFieldName = "sortdir")
        {
            //generate the partial view and returns
            return html.Partial("_GridPager", new GridPagerModel()
            {
                //Sets the properties of grid model
                PageCount = grid.PageCount,
                PageIndex = grid.PageIndex,
                PageSize = grid.RowsPerPage,
                Sort = grid.SortColumn,
                SortDir = Equals(grid.SortDirection, SortDirection.Ascending) ? DynamicGridConstants.ASCKey : DynamicGridConstants.DESCKey,
                SortFieldName = SortFieldName,
                SortDirFieldName = SortDirFieldName,
                RecordPerPageFieldName = recordPerPageFieldName,
                PageFieldName = pageFieldName,
                PageSizeOptions = GetPageSizeOptions()
            });
        }

        /// <summary>
        /// Gets the list of page size
        /// </summary>
        /// <returns>Select list of page size</returns>
        private static IEnumerable<SelectListItem> GetPageSizeOptions()
        {
            IEnumerable<SelectListItem> PageSize = Enumerable.Empty<SelectListItem>();
            string[] pageSizeArray = new string[] { "10", "20", "50", "100" };//Array of page size
            //Create a select list of page size and returns                                                           
            return PageSize = pageSizeArray.Select(item => new SelectListItem
            {
                Value = item,
                Text = item,
            });
        }
    }
}