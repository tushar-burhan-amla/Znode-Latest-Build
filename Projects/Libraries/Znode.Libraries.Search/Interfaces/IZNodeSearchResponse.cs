using System.Collections.Generic;

namespace Znode.Libraries.Search
{
    public interface IZnodeSearchResponse
    {
        List<IZNodeSearchCategoryItem> CategoryItems { get; set; }
        List<IZNodeSearchFacet> Facets { get; set; }
        List<IZNodeSearchItem> Products { get; set; }
        List<string> Highlight { get; set; }
        long TotalProductCount { get; set; }
        string SuggestionTerm { get; set; }
        bool IsSearchFromSuggestion { get; set; }
        List<int> ProductIds { get; set; }

        List<dynamic> ProductDetails { get; set; }

        string RequestBody { get; set; }
    }

    public interface IZNodeSearchCategoryItem
    {
        int CategoryID { get; set; }
        string Name { get; set; }
        string Title { get; set; }
        long? Count { get; set; }
        string SEOUrl { get; set; }
        List<IZNodeSearchCategoryItem> ParentCategory { get; set; }
    }

    public interface IZNodeSearchItem
    {
        string Id { get; set; }
        string ExternalId { get; set; }
        int ZnodeProductId { get; set; }
        string SKU { get; set; }
        int Version { get; set; }
        int LocaleId { get; set; }
        string Name { get; set; }
        int CatalogId { get; set; }
        int CategoryId { get; set; }
        string CategoryName { get; set; }
        string SEOUrl { get; set; }
        string SEOTitle { get; set; }
        string SEOKeyword { get; set; }
        string SEODescription { get; set; }
        List<IZNodeSearchCategoryItem> Categories { get; set; }
        List<SearchAttributes> Attributes { get; set; }
        decimal Boost { get; set; }
        List<string> HighlightList { get; set; }
        decimal RetailPrice { get; set; }
    }

    public interface IZNodeSearchFacet
    {
        string AttributeName { set; get; }
        string AttributeCode { get; set; }
        List<ElasticSearchFacetValue> AttributeValues { set; get; }
        int ControlTypeID { get; set; }
        int DisplayOrder { get; set; }
    }

    public interface IZNodeSearchFacetValue
    {
        string AttributeValue { get; set; }
        long FacetCount { set; get; }
        bool Selected { get; set; }
        int? displayorder { get; set; }
    }
}
