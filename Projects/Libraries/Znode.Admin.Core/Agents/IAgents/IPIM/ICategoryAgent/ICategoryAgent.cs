using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ICategoryAgent
    {
        /// <summary>
        /// Gets the list of Categories.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Category list.</param>
        /// <param name="filters">Filters to be applied on Category list.</param>
        /// <param name="sorts">Sorting to be applied on Category list.</param>
        /// <param name="pageIndex">Start page index of Category list.</param>
        /// <param name="pageSize">Page size of Category list.</param>
        /// <param name="pimCatalogId">pimCatalogId for catalog filter</param>
        /// <param name="catalogName">catalogName for catalog filter</param>
        /// <returns>Category list model.</returns>
        CategoryListViewModel GetCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int pimCatalogId = 0, string catalogName = null);

        /// <summary>
        /// Get Category by Category Id
        /// </summary>
        /// <param name="categoryId">categoryId to get category</param>
        /// <returns>PIMFamilyDetails View Model</returns>
        PIMFamilyDetailsViewModel GetCategory(int categoryId, int familyId);

        /// <summary>
        /// Create new category.
        /// </summary>
        /// <param name="bindDataModel">BindDataModel</param>
        /// <returns>CategoryViewModel.</returns>
        CategoryViewModel CreateCategory(BindDataModel bindDataModel);

        /// <summary>
        /// Update the category.
        /// </summary>
        /// <param name="bindDataModel">BindDataModel</param>
        /// <returns>Return true if updated else false.</returns>
        bool UpdateCategory(BindDataModel bindDataModel);

        /// <summary>
        /// Deletes an existing Category.
        /// </summary>
        /// <param name="pimCategoryId">Ids for the Selected Category.</param>
        /// <returns>true / false</returns>
        bool DeleteCategory(string pimCategoryId);

        /// <summary>
        ///  Get Product Attributes and Family as per selected Family.
        /// </summary>
        /// <param name="familyId">Product Family ID</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>Returns PIMFamilyDetailsViewModel.</returns>
        PIMFamilyDetailsViewModel GetAttributeFamilyDetails(int familyId = 0, int localeId = 0);


        /// <summary>
        ///  Get all attributes associated with categoryId with default category family and provided familyID
        /// </summary>
        /// <param name="pimCategoryId">pimCategoryId</param>
        /// <param name="familyId">familyId</param>
        /// <param name="localeId">localeId</param>
        /// <returns>PIMFamilyDetails View Model</returns>
        PIMFamilyDetailsViewModel GetCategoryAttributes(int pimCategoryId, int familyId, int localeId = 0);


        /// <summary>
        /// Get Associated Products with Category.
        /// </summary>
        /// <param name="categoryId">categoryId to get category Associated Products</param>
        /// <param name="expands">Expands to be retrieved along with Category list.</param>
        /// <param name="filters">Filters to be applied on Category list.</param>
        /// <param name="sorts">Sorting to be applied on Category list.</param>
        /// <param name="pageIndex">Start page index of Category list.</param>
        /// <param name="pageSize">Page size of Category list.</param>
        /// <returns>CategoryProductsListViewModel</returns>
        CategoryProductsListViewModel GetAssociatedCategoryProducts(int categoryId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get the list of Categories associated to Product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        CategoryProductsListViewModel GetAssociatedCategoriesToProduct(int productId, bool isAssociateCategories, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
        

        /// <summary>
        /// Get Associated Products with Category.
        /// </summary>
        /// <param name="categoryId">categoryId to get category UnAssociated Products</param>
        /// <param name="expands">Expands to be retrieved along with Category list.</param>
        /// <param name="filters">Filters to be applied on Category list.</param>
        /// <param name="sorts">Sorting to be applied on Category list.</param>
        /// <param name="pageIndex">Start page index of Category list.</param>
        /// <param name="pageSize">Page size of Category list.</param>
        /// <returns>CategoryProductsListViewModel</returns>
        CategoryProductsListViewModel GetUnAssociatedCategoryProducts(int categoryId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Delete Category Product
        /// </summary>
        /// <param name="categoryProductId">categoryProductId</param>
        /// <returns>Returns true if deleted else false</returns>
        bool DeleteCategoryProduct(string categoryProductId);

        /// <summary>
        /// Delete associated categories from Product.
        /// </summary>
        /// <param name="pimCategoryId"></param>
        /// <returns>Returns true if deleted else false</returns>
        bool DeleteCategoriesAssociatedToProduct(string pimCategoryId);
        
        /// <summary>
        /// Associate Product to Category
        /// </summary>
        /// <param name="categoryId">categoryId</param>
        /// <param name="productIds">productIds</param>
        /// <returns>Returns true if Product Associated else false</returns>
        bool AssociateCategoryProduct(int categoryId, string productIds);

        /// <summary>
        /// Associate categories to Products
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="categoryIds"></param>
        /// <returns></returns>
        bool AssociateCategoriesToProduct(int productId, string categoryIds);



        #region Publish Category
        /// <summary>
        /// Publish Category
        /// </summary>
        /// <param name="categoryIds">Category Id to publish.</param>
        /// <param name="revisionType">revisionType for publish preview selection</param>
        /// <param name="errorMessage">error Message</param>
        /// <returns>Returns true if published successfully else return false.</returns>
        bool PublishCategory(string categoryIds,string revisionType, out string errorMessage);
        #endregion

        /// <summary>
        /// Update product details associated to category.
        /// </summary>
        /// <param name="categoryProductViewModel">CategoryProductViewModel with product details</param>
        /// <returns>Return True/False</returns>
        bool UpdateCategoryProductDetail(CategoryProductViewModel categoryProductViewModel);

    }
}
