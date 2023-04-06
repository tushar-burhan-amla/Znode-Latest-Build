using System;
using System.Collections.Generic;

namespace Znode.Libraries.Search
{
    //Add new properties in small case only for proper filtering of search result.

    //Attribute Class
    public class SearchAttributes
    {
        public string attributecode { get; set; }
        public string attributename { get; set; }
        public string attributevalues { get; set; }
        [Obsolete("This property is unused and will be removed in next release")]
        public string[] rawlowercaseattributevalues { get; set; }
        [Obsolete("This property is unused and will be removed in next release")]
        public string[] rawattributevalues { get; set; }
        public string attributetypename { get; set; }
        public bool ispromorulecondition { get; set; }
        public bool iscomparable { get; set; }
        public bool ishtmltags { get; set; }
        public bool isuseinsearch { get; set; }
        public bool isfacets { get; set; }
        public bool ispersonalizable { get; set; }
        public int displayorder { get; set; }
        public List<ElasticSelectValues> selectvalues { get; set; }
    }

    //Select value class.
    public class ElasticSelectValues
    {
        public string value { get; set; }
        public string code { get; set; }
        public int displayorder { get; set; }
        public string swatchText { get; set; }
        public string path { get; set; }
        public int? mediaconfigurationid { get; set; }
        public int variantdisplayorder { get; set; }
        public string variantimagepath { get; set; }
        public string variantsku { get; set; }
    }
}
