using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICatalogService
    {
        /// <summary>
        /// Gets a list of catalogs.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with catalog list.</param>
        /// <param name="filters">Filters to be applied on catalog list.</param>
        /// <param name="sorts">Sorting to be applied on catalog list.</param>
        /// <returns>Catalog list model.</returns>
        CatalogListModel GetCatalogs(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets catalog using catalogId passed.
        /// </summary>
        /// <param name="pimCatalogId">ID of a catalog to be retrieved.</param>
        /// <returns>Catalog model.</returns>
        CatalogModel GetCatalog(int pimCatalogId);

        /// <summary>
        /// Creates a catalog.
        /// </summary>
        /// <param name="catalogModel">Catalog model to be created.</param>
        /// <returns>Newly created catalog model.</returns>
        CatalogModel CreateCatalog(CatalogModel catalogModel);

        /// <summary>
        /// Updates a catalog.
        /// </summary>
        /// <param name="model">Catalog model to be updated.</param>
        /// <returns>Updated catalog model.</returns>
        bool UpdateCatalog(CatalogModel model);

        /// <summary>
        /// Copy a catalog.
        /// </summary>
        /// <param name="model">Catalog model to be Copied</param>
        /// <returns>Return true or false.</returns>
        bool CopyCatalog(CatalogModel model);

        /// <summary>
        /// Deletes a catalog.
        /// </summary>
        /// <param name="model">Catalog IDs to delete and flag to delete Publish Catalog.</param>
        /// <returns>True/False value according the status of delete operation.</returns>
        bool DeleteCatalog(CatalogDeleteModel model);

        /// <summary>
        /// Delete catalog by catalog Code.
        /// </summary>
        /// <param name="catalogCodes">catalog Codes</param>
        /// <returns>return status</returns>
        bool DeleteCatalogByCode(CatalogDeleteModel catalogCodes);

        /// <summary>
        /// This method is for getting category tree structure.
        /// <param name="catalogAssociationModel">CatalogAssociation Model</param>
        /// </summary>      
        /// <returns>retruns CategoryTreeModel</returns>
        ContentPageTreeModel GetCatgoryTreeNode(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// This method is for getting Associated Catalog-Category Hierarchy.
        /// <param name="pimProductId">ProductId</param>
        /// </summary>      
        /// <returns>retruns CatalogTreeModelList</returns>
        List<CatalogTreeModel> GetAssociatedCatalogHierarchy(int pimProductId);

        /// <summary>
        /// Get list of categories associated to catalog.
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>List of categories which are associated to catalog.</returns>
        CatalogAssociateCategoryListModel GetAssociatedCategories(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Un Associate categories from catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel containing category Ids and CatalogId.</param>
        /// <returns>Returns true if unassociated else false.</returns>
        bool UnAssociateCategoryFromCatalog(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// Get the list of all products associated to category.
        /// <param name="catalogAssociationModel">catalog Association Model having values for CatalogId CategoryId and LocaleId.</param>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// </summary>
        /// <returns>ProductDetailsListModel</returns>
        ProductDetailsListModel GetCategoryAssociatedProducts(CatalogAssociationModel catalogAssociationModel, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// published a catalog with associated product in draft/all status as per isDraftProductsOnly flag is passed.
        /// </summary>
        /// <param name="pimCatalogId">ID of a catalog to be deleted.</param>
        /// <param name="revisionType">Revision Type</param>
        /// <param name="isDraftProductsOnly">Publish draft status/all status Products as per isDraftProductsOnly flag is passed.</param>
        /// <returns>Published Model</returns>
        PublishedModel Publish(int pimCatalogId, string revisionType,bool isDraftProductsOnly);

        /// <summary>
        /// published a catalog with associated product in draft status default.
        /// </summary>
        /// <param name="pimCatalogId">ID of a catalog to be deleted.</param>
        /// <param name="revisionType">Revision Type</param>
        /// <returns>Published Model</returns>
        PublishedModel Publish(int pimCatalogId, string revisionType);

        /// <summary>
        /// Publish catalog category associated products.
        /// </summary>
        /// <param name="pimCatalogId">pimCatalog id to published.</param>
        /// <param name="pimCategoryHierarchyId">pimCategoryHierarchyId id to published.</param>
        /// <param name="revisionType">For publish preview selection.</param>
        /// <returns>True/False value according the status of publish operation.</returns>
        PublishedModel PublishCategoryProducts(int pimCatalogId, int pimCategoryHierarchyId, string revisionType);

        /// <summary>
        /// Delete publish Catalog along with associated category and products
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId to delete</param>
        /// <returns></returns>
        bool DeletePublishCatalog(int publishCatalogId);

        /// <summary>
        /// Get Catalog Publish Status
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>Publish Catalog Log List Model</returns>
        PublishCatalogLogListModel GetCatalogPublishStatus(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

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
        /// <returns>True if updated else false.</returns>
        bool UpdateAssociateCategoryDetails(CatalogAssociateCategoryModel catalogAssociateCategoryModel);

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
        bool UnAssociateProductFromCatalog(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// Update Display order of product associated to catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">Catalog Association Model.</param>
        /// <returns>True if updated else false.</returns>
        bool UpdateCatalogCategoryProduct(CatalogAssociationModel catalogAssociationModel);


        #region Product(s) & Category(s) operation to catalog category 

        /// <summary>
        /// Associate the product(s) to catalog categories
        /// </summary>
        /// <param name="catalogAssociationModel">Model will carry product ids and category id</param>
        /// <returns>Returns if successful true else false.</returns>
        bool AssociateProductsToCatalogCategory(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// UnAssociate the product(s) from catalog category
        /// </summary>
        /// <param name="catalogAssociationModel">catalogAssociationModel</param>
        /// <returns>Returns if products removed successfully true else false.</returns>
        bool UnAssociateProductsFromCatalogCategory(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// Associate the category(s) to catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel</param>
        /// <returns>Returns if associated true else false.</returns>
        bool AssociateCategoryToCatalog(CatalogAssociationModel catalogAssociationModel);

        /// <summary>
        /// UnAssociate the category(s) from catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel</param>
        /// <param name="message">Message if error occurred.</param>
        /// <returns>Returns if category unassociated successfully true else false</returns>
        bool UnAssociateCategoryFromCatalogCategory(CatalogAssociationModel catalogAssociationModel);

        #endregion Product(s) & Category(s) operation to catalog category

        /// <summary>
        /// Get Catalog Details
        /// </summary>
        /// <param name="catalogcode">catalogcode</param>
        /// <returns>CatalogModel</returns>
        CatalogModel GetCatalogByCatalogCode(string catalogcode);

        /// <summary>
        ///If catalog publish is true then create index for its all associated stores 
        /// </summary>
        /// <param name="publishCatalogId">publishCatalog Id</param>
        /// <param name="portalIndex">portalIndex</param>
        /// <param name="isPreviewProductionEnabled">isPreviewProductionEnabled</param>
        /// <param name="revisionType">revisionType</param>
        /// <returns>All associated stores</returns>
        PortalIndexModel AssociatedPortalCreateIndex(int publishCatalogId, PortalIndexModel portalIndex, bool isPreviewProductionEnabled, string revisionType = null);
    }
}
