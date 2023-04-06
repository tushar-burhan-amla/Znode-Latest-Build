using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ICatalogClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of Catalogs.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with catalog list.</param>
        /// <param name="filters">Filters to be applied on catalog list.</param>
        /// <param name="sorts">Sorting to be applied on catalog list.</param>
        /// <returns>Catalog list model.</returns>
        CatalogListModel GetCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of Catalogs.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with catalog list.</param>
        /// <param name="filters">Filters to be applied on catalog list.</param>
        /// <param name="sorts">Sorting to be applied on catalog list.</param>
        /// <param name="pageIndex">Start page index of catalog list.</param>
        /// <param name="pageSize">Page size of catalog list.</param>
        /// <returns>Catalog list model.</returns>
        CatalogListModel GetCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets a Catalog by catalog ID.
        /// </summary>
        /// <param name="pimCatalogId">ID of the catalog to be retrieved.</param>
        /// <returns>Catalog model.</returns>
        CatalogModel GetCatalog(int pimCatalogId);

        /// <summary>
        /// Creates a Catalog.
        /// </summary>
        /// <param name="model">Catalog model to be created.</param>
        /// <returns>Newly created Catalog model.</returns>
        CatalogModel CreateCatalog(CatalogModel model);

        /// <summary>
        /// Updates a Catalog.
        /// </summary>
        /// <param name="model">Catalog model to be updated.</param>
        /// <returns>Updated Catalog model.</returns>
        CatalogModel UpdateCatalog(CatalogModel model);

        /// <summary>
        /// Copy Catalog based on CatalogId
        /// </summary>
        /// <param name="model">Catalog model to be Copied.</param>
        /// <returns>Return true or false</returns>
        bool CopyCatalog(CatalogModel model);

        /// <summary>
        /// Deletes a Catalog by catalog ID.
        /// </summary>
        /// <param name="model">Catalog IDs to delete and flag to delete Publish Catalog.</param>
        /// <returns>True/False value according the status of delete operation.</returns>
        bool DeleteCatalog(CatalogDeleteModel model);

        /// <summary>
        /// Get the tree structure for category.
        /// <param name="catalogAssociationModel">CatalogAssociation Model</param>
        /// </summary>
        /// <returns>CategoryTreeModel</returns>
        ContentPageTreeModel GetCategoryTree(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// Associate the categories to catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel containing category Ids and CatalogIds.</param>
        /// <returns>Returns true if associated else false.</returns>
        bool AssociateCategory(CatalogAssociationModel catalogAssociationModel); 

        /// <summary>
        /// Get list of categories associated to catalog.
        /// </summary>
        /// <param name="filters">Filters to be applied on associatedCategory list.</param>
        /// <param name="sorts">Sorting to be applied on associatedCategory list.</param>
        /// <param name="pageIndex">Start page index of associatedCategory list.</param>
        /// <param name="pageSize">Page size of AssociatedCategory list.</param>
        /// <returns>Categories which are associated to associatedCategory.</returns>
        CatalogAssociateCategoryListModel GetAssociatedCategoryList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// UnAssociate the categories to catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel containing category Ids and CatalogIds.</param>
        /// <returns>Returns true if associated else false.</returns>
        bool UnAssociateCategory(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// Get the list of all products associated to category.
        /// <param name="catalogAssociationModel">catalog Association Model having values for CatalogId CategoryId and LocaleId.</param>
        /// <param name="filters">Filters to be applied on associated Products list.</param>
        /// <param name="sorts">Sorting to be applied on associated Products list.</param>
        /// <param name="pageIndex">Start page index of associated Products list.</param>
        /// <param name="pageSize">Page size of Associated Products list.</param>
        /// </summary>
        /// <returns>ProductDetailsListModel</returns>
        ProductDetailsListModel GetCategoryAssociatedProducts(CatalogAssociationModel catalogAssociationModel, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Published catalog with associated product as per isDraftProductsOnly flag passed.
        /// </summary>
        /// <param name="pimCatalogId">Catalog id to published.</param>
        /// <param name="revisionType">revision Type.</param>
        /// <param name="isDraftProductsOnly">Publish Draft/All as per isDraftProductsOnly flag is passed Products Only.</param>
        /// <returns>Returns published model.</returns>
        PublishedModel PublishCatalog(int pimCatalogId, string revisionType,bool isDraftProductsOnly);

        /// <summary>
        /// Published catalog with associated product which has draft status.
        /// </summary>
        /// <param name="pimCatalogId">Catalog id to published.</param>
        /// <param name="revisionType">revision Type.</param>
        /// <returns>Returns published model.</returns>
        PublishedModel PublishCatalog(int pimCatalogId, string revisionType);

        /// <summary>
        /// Publish catalog category associated products.
        /// </summary>
        /// <param name="pimCatalogId">pimCatalogId to published.</param>
        /// <param name="pimCategoryHierarchyId">pimCategoryHierarchyId id to published.</param>
        /// <param name="revisionType">For preview publish selection.</param>
        /// <returns>Returns published model.</returns>
        PublishedModel PublishCategoryProducts(int pimCatalogId, int pimCategoryHierarchyId, string revisionType);

        /// <summary>
        /// Get details(Display order, active status, etc.)of category associated to catalog.
        /// </summary>
        /// <param name="catalogAssociateCategoryModel">Catalog Associate Category Model</param>
        /// <returns>Catalog Associate Category Model.</returns>
        CatalogAssociateCategoryModel GetAssociateCategoryDetails(CatalogAssociateCategoryModel catalogAssociateCategoryModel);

        /// <summary>
        /// Update details(Display order, active status, etc.)of category associated to catalog.
        /// </summary>
        /// <param name="catalogAssociateCategoryModel">Catalog Associate Category Model.</param>
        /// <returns>Catalog Associate Category Model.</returns>
        CatalogAssociateCategoryModel UpdateAssociateCategoryDetails(CatalogAssociateCategoryModel catalogAssociateCategoryModel);

        /// <summary>
        /// Move one category to other category within catalog.
        /// </summary>
        /// <param name="model">CatalogAssociateCategoryModel</param>
        /// <returns>Returns true if moved successfully else false.</returns>
        bool MoveCategory(CatalogAssociateCategoryModel model);

        /// <summary>
        /// Remove products from catalog, If ProfileCatalogId is > 0 and ProfileCatalogCategoryIds are there, method will remove products from catlog profile.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel</param>
        /// <returns>Returns true if products removed successfully else false.</returns>
        bool UnAssociateProduct(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        ///  Get Catalog Publish Status
        /// </summary>
        /// <param name="filters">Filters to be applied on associated Products list.</param>
        /// <param name="sorts">Sorting to be applied on associated Products list.</param>
        /// <param name="pageIndex">Start page index of associated Products list.</param>
        /// <param name="pageSize">Page size of Associated Products list.</param>
        /// <returns>Publish Catalog Log List Model</returns>
        PublishCatalogLogListModel GetCatalogPublishStatus(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Update Catalog Category Product
        /// </summary>
        /// <param name="catalogAssociationModel">catalogAssociationModel</param>
        /// <returns>Catalog Association Model.</returns>
        bool UpdateCatalogCategoryProduct(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// Get Associated Catalogs and Categories by product id
        /// </summary>
        /// <param name="pimProductId">Product id</param>
        /// <returns>List Catalog Tree Model</returns>
        List<CatalogTreeModel> GetCatalogCategoryHierarchy(int pimProductId);

        #region Product(s) & Category(s) operation to catalog category  

        /// <summary>
        /// Associate the product(s) to catalog categories
        /// </summary>
        /// <param name="catalogAssociationModel">Model will carry product ids and category id</param>
        /// <returns>Returns if successful true else false</returns>
        bool AssociateProductsToCatalogCategory(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// //UnAssociate the product(s) from catalog category
        /// </summary>
        /// <param name="catalogAssociationModel">catalogAssociationModel</param>
        /// <returns>Returns if products removed successfully true else false.</returns>
        bool UnAssociateProductsFromCatalogCategory(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// Associate the category(s) to catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel containing category Ids and CatalogIds.</param>
        /// <returns>Returns if category associated true else false.</returns>
        bool AssociateCategoryToCatalog(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// UnAssociate the category(s) from catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel</param>
       
       /// <returns>Returns if category unassociated successfully true else false</returns>
        bool UnAssociateCategoryFromCatalog(CatalogAssociationModel catalogAssociationModel);

        #endregion Product(s) & Category(s) operation to catalog category
    }
}
