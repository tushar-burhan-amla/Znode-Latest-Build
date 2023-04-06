using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using System.Collections.Generic;

namespace Znode.Engine.Api.Client
{
    public interface ICategoryClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of Categories.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Category list.</param>
        /// <param name="filters">Filters to be applied on Category list.</param>
        /// <param name="sorts">Sorting to be applied on Category list.</param>
        /// <param name="pageIndex">Start page index of Category list.</param>
        /// <param name="pageSize">page size of Category list.</param>
        /// <returns>Category list model.</returns>
        CategoryListModel GetCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets a Category by category ID.
        /// </summary>
        /// <param name="categoryId">ID of the category to be retrieved.</param>
        /// <returns>PIMFamily Details Model.</returns>
        PIMFamilyDetailsModel GetCategory(int categoryId, int familyId, int localeId);

        /// <summary>
        /// Creates a Category.
        /// </summary>
        /// <param name="model">Category Values ListModel to be created.</param>
        /// <returns>Newly created Category Values ListModel.</returns>
        CategoryValuesListModel CreateCategory(CategoryValuesListModel model);

        /// <summary>
        /// Updates a Category.
        /// </summary>
        /// <param name="model">Category model to be updated.</param>
        /// <returns>Updated Category model.</returns>
        CategoryValuesListModel UpdateCategory(CategoryValuesListModel model);

        /// <summary>
        /// Deletes a Category by category ID.
        /// </summary>
        /// <param name="categoryIds">IDs of a category to be deleted.</param>
        /// <returns>True/False value according the status of delete operation.</returns>
        bool DeleteCategory(string categoryIds);


        /// <summary>
        /// Delete Category Product
        /// </summary>
        /// <param name="pimCategoryProductId">PimCategoryProductId's of a CategoryProduct to be deleted.</param>
        /// <returns>True/False value according the status of delete operation.</returns>
        bool DeleteCategoryProduct(string pimCategoryProductId);

        /// <summary>
        /// Delete all associated  categories from product. 
        /// </summary>
        /// <param name="pimCategoryProductId"> list of unique Id for Product Category</param>
        /// <returns> True/False value according the status of delete operation.</returns>
        bool DeleteAssociatedCategoriesToProduct(string pimCategoryProductId);

        /// <summary>
        /// Associate Products to Category
        /// </summary>
        /// <param name="categoryProductsList">List of CategoryProductModel</param>
        /// <returns>True/False value according the status of AssociateCategoryProduct operation.</returns>
        bool AssociateCategoryProduct(List<CategoryProductModel> categoryProductsList);

        /// <summary>
        /// Associate Categories to Products.
        /// </summary>
        /// <param name="categoryProductsList"></param>
        /// <returns> Return True or False.</returns>
        bool AssociateCategoriesToProduct(List<CategoryProductModel> categoryProductsList);

        /// <summary>
        /// Get Category Product
        /// </summary>
        /// <param name="categoryId">categoryId to get CategoryProduct</param>
        ///  <param name="associatedProducts">associatedProducts to get Category</param>
        /// <returns>Category Product List Model</returns>
        CategoryProductListModel GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProducts, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Associated Category Product
        /// </summary>
        /// <param name="categoryId">categoryId to get CategoryProduct</param>
        /// <param name="associatedProducts">associatedProducts to get CategoryProduct</param>
        /// <returns>Category Product List Model</returns>
        CategoryProductListModel GetAssociatedCategoryProducts(int categoryId, bool associatedProducts, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get the List of Categories associated to Product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="associatedProducts"></param>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        CategoryProductListModel GetAssociatedCategoriesToProduct(int productId, bool associatedProducts, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);


        #region Publish Category
        /// <summary>
        /// Publish Category
        /// </summary>
        /// <param name="parameterModel">Parameter model containing category id for publishing.</param>
        /// <returns>Returns published model.</returns>
        PublishedModel PublishCategory(ParameterModel parameterModel);

        #endregion

        /// <summary>
        /// Update product details associated to category.
        /// </summary>
        /// <param name="categoryProductModel"> CategoryProductModel with product details</param>
        /// <returns>Return True or False.</returns>
        bool UpdateCategoryProductDetail(CategoryProductModel categoryProductModel);
    }
}
