using System;
using System.Collections.Generic;

namespace Znode.Libraries.Search
{
    //Add new properties in small case only for proper filtering of search result.
    public class SearchProduct
    {
        public string publishedId { get; set; }
        public string publishproductentityid { get; set; }

        public string externalid { get; set; }

        public int znodeproductid { get; set; }
        public string sku { get; set; }
        public int version { get; set; }
        public int localeid { get; set; }
        public string name { get; set; }
        [Obsolete("This property is unused and will be removed in next release")]
        public string rawname { get; set; }
        [Obsolete("This property is unused and will be removed in next release")]
        public string rawsku { get; set; }
        public int catalogid { get; set; }
        public int categoryid { get; set; }
        public string categoryname { get; set; }

        public decimal productprice { get; set; }
        public decimal rating { get; set; }
        public int totalreviewcount { get; set; }

        public string indexid { get; set; }
        public bool isactive { get; set; }
        public string[] categoryprofileids { get; set; }
        public int productindex { get; set; }

        public List<SearchAttributes> searchableattributes { get; set; }

        public List<SearchAttributes> attributes { get; set; }
        public List<ElasticBrands> brands { get; set; }

        public decimal productboost { get; set; }
        public decimal categoryboost { get; set; }

        public List<string> highlightlist { get; set; }

        public int displayorder { get; set; }
        public long timestamp { get; set; }
        public string revisionType { get; set; }
        public string salesprice { get; set; }
        public string retailprice { get; set; }
        public string culturecode { get; set; }
        public string currencysuffix { get; set; }
        public string currencycode { get; set; }

        public string seodescription { get; set; }
        public string seokeywords { get; set; }
        public string seotitle { get; set; }
        public string seourl { get; set; }
        public string imagesmallpath { get; set; }
        public Nullable<int> ElasticSearchEvent { get; set; }
        public SearchProduct()
        {
            brands = new List<ElasticBrands>();
            attributes = new List<SearchAttributes>();
            searchableattributes = new List<SearchAttributes>();
            highlightlist = new List<string>();
        }
        public List<int> parentcategoryids { get; set; }
    }
}
