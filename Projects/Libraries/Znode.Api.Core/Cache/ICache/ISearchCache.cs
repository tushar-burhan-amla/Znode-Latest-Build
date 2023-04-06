using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface ISearchCache
    {
        /// <summary>
        /// Get SEO Url details on the basis of SEO Url.
        /// </summary>
        /// <param name="seoUrl">SEOUrl.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns SEO Url details.</returns>
        string GetSEOUrlDetails(string seoUrl, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets portal index data.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>String data.</returns>
        string GetCatalogIndexData(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets list of Global level product boost.
        /// </summary>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>List of Global level product boost.</returns>
        string GetGlobalProductBoostList(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets list of Global level product category boost.
        /// </summary>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>List of Global level product category boost.</returns>
        string GetGlobalProductCategoryBoostList(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets list of field level boost.
        /// </summary>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>List of field level boost.</returns>
        string GetFieldBoostList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Keyword Search Suggestion.  
        /// </summary>
        /// <param name="model">Model containing search keys and parameters.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns KeywordSearchModel containing suggestions.</returns>
        string GetKeywordSearchSuggestion(SearchRequestModel model, string routeUri, string routeTemplate);

        #region Elastic search
        /// <summary>
        /// Performs full text keyword search.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string FullTextSearch(SearchRequestModel model, string routeUri, string routeTemplate);

        /// <summary>
        /// Get product details by SKU
        /// </summary>
        /// <param name="model">Search request model</param>
        /// <param name="routeUri">route Uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns></returns>
        string GetProductDetailsBySKU(SearchRequestModel model, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets search index monitor list.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>String data containing search index monitor list.</returns>
        string GetSearchIndexMonitorList(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets search index server status list.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>String data containing search index server status list. </returns>
        string GetSearchIndexServerStatusList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get facet search result.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string FacetSearch(SearchRequestModel model, string routeUri, string routeTemplate);
        #endregion

        #region Synonyms
        /// <summary>
        /// Get synonyms for search.
        /// </summary>
        /// <param name="searchSynonymsId">Uses id to retrieve data.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns model with data.</returns>
        string GetSearchSynonyms(int searchSynonymsId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of synonyms.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Returns list of synonyms.</returns>
        string GetSearchSynonymsList(string routeUri, string routeTemplate);
        #endregion

        #region Keywords Redirect
        /// <summary>
        /// Get list of keyword redirect.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Returns list of keywords and their redirected url.</returns>
        string GetCatalogKeywordsRedirectList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get keywords for search.
        /// </summary>
        /// <param name="searchKeywordsRedirectId">Uses id to retrieve data.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns model with data.</returns>
        string GetSearchKeywordsRedirect(int searchKeywordsRedirectId, string routeUri, string routeTemplate);

        string GetSearchProfileProducts(SearchProfileModel model, string routeUri, string routeTemplate);
        #endregion
    }
}