using System.Collections.Generic;

namespace Znode.Libraries.Search
{
    public class ElasticSearchResponse : IZnodeSearchResponse
    {
        public ElasticSearchResponse()
        {
            Highlight = new List<string>();
        }

        public List<IZNodeSearchCategoryItem> CategoryItems { get; set; }
        public List<IZNodeSearchFacet> Facets { get; set; }
        public List<IZNodeSearchItem> Products { get; set; }
        public List<string> Highlight { get; set; }
        public long TotalProductCount { get; set; }
        public string SuggestionTerm { get; set; }
        public bool IsSearchFromSuggestion { get; set; }
      
        public List<int> ProductIds { get; set; }
        public List<dynamic> ProductDetails { get; set; }

        public string RequestBody { get; set; }
   
    }

    public class ElasticSearchCategoryItem : IZNodeSearchCategoryItem
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public long? Count { get; set; }
        public string SEOUrl { get; set; }
        public List<IZNodeSearchCategoryItem> ParentCategory { get; set; }
        public string Tags { get; set; }
    }

    public class ElasticSearchItem : IZNodeSearchItem
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public int ZnodeProductId { get; set; }
        public string SKU { get; set; }
        public int Version { get; set; }
        public int LocaleId { get; set; }
        public string Name { get; set; }
        public int CatalogId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SEOUrl { get; set; }
        public string SEOTitle { get; set; }
        public string SEOKeyword { get; set; }
        public string SEODescription { get; set; }
        public List<IZNodeSearchCategoryItem> Categories { get; set; }
        public List<SearchAttributes> Attributes { get; set; }
        public decimal Boost { get; set; }
        public List<string> HighlightList { get; set; }
        public decimal RetailPrice { get; set; }
    }

    public class ElasticSearchFacet : IZNodeSearchFacet
    {
        public string AttributeName { set; get; }
        public string AttributeCode { get; set; }
        public List<ElasticSearchFacetValue> AttributeValues { set; get; }
        public int ControlTypeID { get; set; }
        public int DisplayOrder { get; set; }        
    }

    public class ElasticSearchFacetValue : IZNodeSearchFacetValue
    {
        public string AttributeValue { get; set; }
        public long FacetCount { set; get; }
        public bool Selected { get; set; }
        public int? displayorder { get; set; }                
    }
}