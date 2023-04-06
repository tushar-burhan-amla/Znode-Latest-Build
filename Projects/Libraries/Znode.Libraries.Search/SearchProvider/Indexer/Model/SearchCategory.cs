using System;
using System.Collections.Generic;

namespace Znode.Libraries.Search
{
    //Add new properties in small case only for proper filtering of search result.
    public class SearchCategory
    {
        public int categoryid { get; set; }
        public string categoryname { get; set; }
        public string title { get; set; }
        public string seourl { get; set; }
        public int[] profileids { get; set; }
        public List<SearchCategory> parentcategories{ get; set; }
        public bool isactive { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
