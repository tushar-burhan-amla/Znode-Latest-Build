using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.WebStore.Models
{
    public class GridPagerModel
    {
        public int PageCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string Sort { get; set; }
        public string SortFieldName { get; set; }
        public string SortDir { get; set; }
        public string SortDirFieldName { get; set; }

        public string Url { get; set; }
        public string RecordPerPageFieldName { get; set; }
        public string PageFieldName { get; set; }
        public IEnumerable<SelectListItem> PageSizeOptions { get; set; }
    }
}