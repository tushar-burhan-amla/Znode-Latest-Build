using System;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface ICategoryAgent
    {
        /// <summary>
        /// Get Category list for top  category header.
        /// </summary>
        /// <param name="pageSize">pageSize</param>
        /// <param name="pageLength">pageLength</param>
        /// <returns>Returns category list with subcategories.</returns>
        CategoryHeaderListViewModel GetCategories(int? pageSize = 0, int? pageLength = 0);

        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because sitemap logic is move to sitemap agent." +
        " Please use sitemap controller for sitemap releated functions.")]
        /// <summary>
        /// Get Category list for top  category header.
        /// </summary>
        /// <param name="pageSize">pageSize</param>
        /// <param name="pageLength">pageLength</param>
        /// <returns>Returns category list with subcategories.</returns>
        CategoryHeaderListViewModel GetSiteMapCategories(int? pageSize = 0, int? pageLength = 0);

        /// <summary>
        /// Get Category by category id.
        /// </summary>
        /// <param name="CategoryId">Category Id.</param>
        /// <returns>Returns category view model with category details.</returns>
        CategoryViewModel GetCategoryById(int? CategoryId);

        /// <summary>
        /// Get Category SeoDetails.
        /// </summary>
        /// <param name="categoryId">category id.</param>
        /// <param name="isGetParentCategory"><see langword="true"/>/false</param>
        /// <returns>Category View Model.</returns>
        CategoryViewModel GetCategorySeoDetails(int categoryId, bool isGetParentCategory = true);

        /// <summary>
        /// Removes a key from session.
        /// </summary>
        /// <param name="sessionKey">Name of the session key.</param>
        void RemoveFromSession(string sessionKey);

        /// <summary>
        /// Gets category breadcrumb.
        /// </summary>
        /// <param name="categoryId">Category ID of the current category.</param>
        /// <returns>Breadcrumb HTML.</returns>
        void GetBreadCrumb(CategoryViewModel categoryId);

        /// <summary>
        /// Gets category breadcrumb by category id.
        /// </summary>
        /// <param name="categoryId">Category ID of the current category.</param>
        /// <returns>Breadcrumb HTML.</returns>
        string GetBreadCrumb(int categoryId);
    }
}
