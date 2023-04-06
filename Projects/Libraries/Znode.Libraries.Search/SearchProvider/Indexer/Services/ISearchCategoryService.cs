using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Libraries.Search
{
    public interface ISearchCategoryService
    {
        /// <summary>
        /// Gets list of categories and its hierarchy.
        /// </summary>
        /// <param name="categoryIds">Ids for which category will be fetched.</param>
        /// <param name="localeId">Locale id of the category.</param>
        /// <param name="catalogId">Catalog Id of the category.</param>
        /// <param name="categorySeoDetails">SEO details list from where category's SEO will be assigned.</param>
        /// <returns>List of Categories.</returns>
        List<SearchCategory> GetProductCategory(int categoryIds, int localeId, int catalogId, List<SEODetailsModel> categorySeoDetails);

        /// <summary>
        /// Get Active Category list
        /// </summary>
        /// <param name="catalogId">catalog id</param>
        /// <param name="versionId">version id of catalog</param>
        /// <returns>list of active category.</returns>
        List<ZnodePublishCategoryEntity> GetPublishCategoryList(int catalogId, int versionId);

        /// <summary>
        /// Get Active Category list
        /// </summary>
        /// <param name="catalogId">catalog id</param>
        /// <param name="versionIds">version ids of catalog</param>
        /// <returns>list of active category.</returns>
        List<ZnodePublishCategoryEntity> GetPublishCategoryList(int catalogId, int[] versionIds);

    }
}
