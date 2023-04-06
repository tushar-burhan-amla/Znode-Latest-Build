using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// Gets a list of categories.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with category list.</param>
        /// <param name="filters">Filters to be applied on category list.</param>
        /// <param name="sorts">Sorting to be applied on category list.</param>
        /// <returns>Category list model.</returns>
        CategoryListModel GetCategories(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets category using categoryId passed.
        /// </summary>
        /// <param name="categoryId">ID of a category to be retrieved.</param>
        /// <returns>PIMFamily Details Model model.</returns>
        PIMFamilyDetailsModel GetCategory(int categoryId, int familyId, int localeId);

        /// <summary>
        /// Creates a category.
        /// </summary>
        /// <param name="model">Category Values ListModel to be created.</param>
        /// <returns>Newly created Category Values ListModel.</returns>
        CategoryValuesListModel CreateCategory(CategoryValuesListModel model);

        /// <summary>
        /// Updates a category.
        /// </summary>
        /// <param name="model">Category Values ListModel to be updated.</param>
        /// <returns>Return True/False</returns>
        bool UpdateCategory(CategoryValuesListModel model);

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="categoryIds">IDs of a category to be deleted.</param>
        /// <returns>True/False value according the status of delete operation.</returns>
        bool DeleteCategory(ParameterModel categoryIds);

        /// <summary>
        /// Delete Category Associated Products
        /// </summary>
        /// <param name="PimCategoryProductId">PimCategory ProductId to be deleted.</param>
        /// <returns>Return True/False</returns>
        bool DeleteCategoryAssociatedProducts(ParameterModel PimCategoryProductId);

        /// <summary>
        /// Delete Associated categories to Product
        /// </summary>
        /// <param name="PimCategoryProductId"></param>
        /// <returns></returns>
        bool DeleteAssociatedCategoriesToProduct(ParameterModel PimCategoryProductId);

        /// <summary>
        /// Associate Products to Category
        /// </summary>
        /// <param name="categoryProductModel">List of CategoryProductModel</param>
        /// <returns>Return True/False</returns>
        bool AssociateCategoryProduct(List<CategoryProductModel> categoryProductModel);

        /// <summary>
        /// Associate categories to Product. 
        /// </summary>
        /// <param name="categoryProductModel"></param>
        /// <returns>Return True or False as per delete operation.</returns>
        bool AssociateCategoriesToProduct(List<CategoryProductModel> categoryProductModel);

        /// <summary>
        /// Get associated or unassociated Products with Category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="associatedProduts">Set true for associatedProduts and false for unAssociatedProduts</param>
        /// <param name="expands">Expands to be retrieved along with category list.</param>
        /// <param name="filters">Filters to be applied on category list.</param>
        /// <param name="sorts">Sorting to be applied on category list.</param>
        /// <param name="page">Page Number</param>
        /// <returns>Category Product ListModel</returns>
        CategoryProductListModel GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProduts, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get associated Products with Category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="expands">Expands to be retrieved along with category list.</param>
        /// <param name="filters">Filters to be applied on category list.</param>
        /// <param name="sorts">Sorting to be applied on category list.</param>
        /// <param name="page">Page Number</param>
        /// <returns>Category Product List Model</returns>
        CategoryProductListModel GetAssociatedCategoryProducts(int categoryId, bool associatedProducts, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get the list of associated/unassociated categories for product.
        /// </summary>
        /// <param name="productId"> passing product Id.</param>
        /// <param name="associatedProducts"></param>
        /// <param name="expands">Expands to be retrieved along with category list.</param>
        /// <param name="filters">Filters to be applied on category list.</param>
        /// <param name="sorts">Sorting to be applied on category list.</param>
        /// <param name="page">Page Number</param>
        /// <returns>CategoryProductListModel</returns>
        CategoryProductListModel GetAssociatedCategoriesToProducts(int productId, bool associatedProducts, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);





        #region WebStoreCategory
        /// <summary>
        /// Gets a list of publish Categories, SubCategories and Products.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with category list.</param>
        /// <param name="filters">Filters to be applied on category list.</param>
        /// <param name="sorts">Sorting to be applied on category list.</param>
        /// <returns>Category list model.</returns>
        WebStoreCategoryListModel GetCategoryDetails(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        #endregion

        #region Publish Category
        /// <summary>
        /// Publish Category.
        /// </summary>
        /// <param name="parameterModel">Category Id to be published.</param>
        /// <returns>Returns published model.</returns>
        PublishedModel Publish(ParameterModel parameterModel);
        #endregion

        /// <summary>
        /// Update product details associated to category.
        /// </summary>
        /// <param name="categoryProductModel">CategoryProductModel with product details</param>
        /// <returns>Return True/False</returns>
        bool UpdateCategoryProductDetail(CategoryProductModel categoryProductModel);
    }
}
