using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public interface ISearchAgent
    {

        /// <summary>
        /// Get Seo Url details by SeoUrl.
        /// </summary>
        /// <param name="seoUrl">SeoUrl.</param>
        /// <returns>Returns SEOUrlViewModel.</returns>
        SEOUrlViewModel GetSeoUrlDetail(string seoUrl);

        /// <summary>
        /// Get List Of Suggestion for product.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="category"></param>
        /// <param name="UrlHelper">url</param>
        /// <returns></returns>
        List<AutoComplete> GetSuggestions(string searchTerm, string category, UrlHelper url);

        /// <summary>
        /// Set Filter Data from facets.
        /// </summary>
        /// <param name="searchRequestModel">SearchRequestModel model.</param>
        void SetFilterData(SearchRequestViewModel searchRequestModel);
        string CheckURLExistForSearchTerm(string searchTerm);

        /// <summary>
        /// Get Facet Search Result.
        /// </summary>
        /// <param name="searchRequestModel">request model for search.</param>
        /// <param name="expands">Expands for search result.</param>
        /// <param name="filters">Filters for search</param>
        /// <param name="sorts">Sorts for search</param>
        /// <returns>search model.</returns>
        KeywordSearchModel FacetSearch(SearchRequestViewModel searchRequestModel, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get Search result.
        /// </summary>
        /// <param name="searchRequestModel">request model for search.</param>
        /// <param name="expands">Expands for search result.</param>
        /// <param name="filters">Filters for search</param>
        /// <param name="sorts">Sorts for search</param>
        /// <returns>search model.</returns>
        KeywordSearchModel FullTextSearch(SearchRequestViewModel searchRequestModel, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get Sort value based on search
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        SortCollection GetSortForSearch(int sort);

        /// <summary>
        /// Get keyword search model.
        /// </summary>
        /// <param name="searchRequest">SearchRequestViewModel</param>
        /// <returns>return SearchRequestModel</returns>
        SearchRequestModel GetKeywordSearchModel(SearchRequestViewModel searchRequest);
    }
}