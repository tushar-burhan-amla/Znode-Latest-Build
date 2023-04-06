using System.Collections.Generic;

using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Search;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Libraries.ElasticSearch
{
    public class ElasticSearchProviderService : IZnodeSearchProvider
    {
        #region Protected Variables

        protected readonly IElasticSearchBaseService elasticSearchBaseService;

        #endregion

        #region Constructor

        public ElasticSearchProviderService()
        {
            elasticSearchBaseService = GetService<IElasticSearchBaseService>();
        }

        #endregion

        #region Public Methods       

        //Find terms matching the given partial word that appear in the highest number of documents
        public virtual IZnodeSearchResponse SuggestTermsFor(IZnodeSearchRequest request)
        {
            IZnodeSearchResponse elasticresponse = new ElasticSearchResponse();           
            IElasticSuggestionsService suggestions = GetService<IElasticSuggestionsService>();

            elasticresponse = suggestions.SuggestTermsFromExistingIndex(request);
            return elasticresponse;
        }

        #region Elastic search

        //Full text search for a search keyword
        public virtual IZnodeSearchResponse FullTextSearch(IZnodeSearchRequest request)
        {
            if (string.IsNullOrEmpty(request.SearchText)) request.SearchText = string.Empty;

            // Search the category / Sub-Category for the given text.
            IZnodeSearchResponse searchResponse = elasticSearchBaseService.FullTextSearch(request);

            GetsFacetsAndCategories(request, searchResponse);

            return searchResponse;
        }

        public virtual List<string> FieldValueList(string indexName)
        {
            List<string> searchResponse = elasticSearchBaseService.FieldValueList(indexName);
            return searchResponse;
        }


        // Method to check if elastic search is working.
        public virtual string CheckElasticSearch()
        {
            return elasticSearchBaseService.CheckElasticSearch();
        }

        #endregion Elastic search

        #region CMS Page search

        //Get cms page for a search keyword.
        public virtual IZnodeCMSPageSearchResponse FullTextContentPageSearch(IZnodeCMSPageSearchRequest request)
        {
            IElasticCMSPageSearchService _elasticCMSPageSearch = GetService<IElasticCMSPageSearchService>();

            // Search the CMS text pages for the given text.
            IZnodeCMSPageSearchResponse searchResponse = _elasticCMSPageSearch.FullTextContentPageSearch(request);

            return searchResponse;
        }

        //Get cms page count for a search keyword. 
        public virtual IZnodeCMSPageSearchResponse GetSearchContentPageCount(IZnodeCMSPageSearchRequest request)
        {
            IElasticCMSPageSearchService _elasticCMSPageSearch = GetService<IElasticCMSPageSearchService>();

            // Search the CMS text pages for the given text.
            IZnodeCMSPageSearchResponse searchResponse = _elasticCMSPageSearch.GetSearchContentPageCount(request);

            return searchResponse;
        }

        #endregion CMS Page search

        #endregion Public Methods  

        #region Protected Methods
        //Populate the facets for the given input       
        protected virtual List<IZNodeSearchCategoryItem> PopulateCategoryFacets(IZnodeSearchResponse searchResponse)
        {
            return searchResponse.CategoryItems;
        }

        //Gets facets and categories for the search result.
        protected virtual void GetsFacetsAndCategories(IZnodeSearchRequest request, IZnodeSearchResponse searchResponse)
        {
            if (HelperUtility.IsNotNull(searchResponse) && request.GetCategoriesHierarchy)
            {
                searchResponse.CategoryItems = PopulateCategoryFacets(searchResponse);
            }
        }

        #endregion Protected Methods

    }
}
