using System.Collections.Generic;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ICatalogAgent
    {
        /// <summary>
        /// Gets the list of Catalogs.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with catalog list.</param>
        /// <param name="filters">Filters to be applied on catalog list.</param>
        /// <param name="sorts">Sorting to be applied on catalog list.</param>
        /// <returns>Catalog list model.</returns>
        CatalogListViewModel GetCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of Catalogs.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with catalog list.</param>
        /// <param name="filters">Filters to be applied on catalog list.</param>
        /// <param name="sorts">Sorting to be applied on catalog list.</param>
        /// <param name="pageIndex">Start page index of catalog list.</param>
        /// <param name="pageSize">Page size of catalog list.</param>
        /// <returns>Catalog list model.</returns>
        CatalogListViewModel GetCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create new catalog.
        /// </summary>
        /// <param name="catalogModel">Catalog model.</param>
        /// <returns>Returns newly created catalog model</returns>
        CatalogViewModel CreateCatalog(CatalogViewModel catalogModel);

        /// <summary>
        /// Get catalog by catalog Id.
        /// </summary>
        /// <param name="pimCatalogId">Id to get catalog.</param>
        /// <returns>Catalog View Model.</returns>
        CatalogViewModel GetCatalog(int pimCatalogId);

        /// <summary>
        /// Update the Catalog Information.
        /// </summary>
        /// <param name="model">Catalog model.</param>
        /// <returns>Return the Updated Catalog Model.</returns>
        CatalogViewModel UpdateCatalog(CatalogViewModel model);

        // <summary>
        /// Creates a copy of an existing catalog.
        /// </summary>
        /// <param name="catalogModel">Model of the Catalog</param>
        /// <returns>Returns true or false.</returns>
        bool CopyCatalog(CatalogViewModel catalogModel);

        /// <summary>
        /// Deletes an existing catalog.
        /// </summary>
        /// <param name="pimCatalogId">Ids for the Selected Catalog.</param>
        /// <param name="isDeletePublishCatalog">True to Delete Publish Catalog.</param>
        /// <returns>true / false</returns>
        bool DeleteCatalog(string pimCatalogId, bool isDeletePublishCatalog);

        /// <summary>
        /// Get the Category tree.
        /// </summary>
        /// <param name="catalogId">Catalog Id.</param>
        /// <returns>Category tree.</returns>
        string GetTree(int catalogId);

        /// <summary>
        /// Associate the categories to catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel containing category Ids and CatalogId.</param>
        /// <returns>Returns true if associated else false.</returns>
        bool AssociateCategory(CatalogAssociationViewModel catalogAssociationViewModel);

        /// <summary>
        /// UnAssociate the categories to catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel containing category Ids and CatalogId.</param>
        /// <param name="message">Message if error occurred.</param>
        /// <returns>Returns true if associated else false.</returns>
        bool UnAssociateCategory(CatalogAssociationViewModel catalogAssociationViewModel);

        /// <summary>
        /// Published catalog.
        /// </summary>
        /// <param name="pimCatalogId">pimCatalog id to published.</param> 
        /// <param name="revisionType">versionType "preview" then Preview</param>
        /// <param name="errorMessage">Error Message occurred during publish catalog</param>
        /// <param name="publishContent">publishContent</param>
        /// <returns>Returns true if catalog published.</returns>
        bool PublishCatalog(int pimCatalogId, string revisionType, out string errorMessage,string publishContent = null);

        /// <summary>
        ///Publish catalog category associated products.
        /// </summary>
        /// <param name="pimCatalogId">pimCatalog id to published.</param>
        /// <param name="pimCategoryHierarchyId">pimCategoryHierarchyId id to published.</param>
        /// <param name="revisionType">For publish preview selection.</param>
        /// <param name="errorMessage">Error Message occurred during publish catalog category associated products</param>
        /// <returns>Returns true if publish successfully.</returns>
        bool PublishCategoryProducts(int pimCatalogId, int pimCategoryHierarchyId,string revisionType, out string errorMessage);

        /// <summary>
        /// Get details(Display order, active status, etc.)of category associated to catalog.
        /// </summary>
        /// <param name="catalogAssociateCategoryModel">Catalog Associate Category Model</param>
        /// <returns>Catalog Associate Category Model.</returns>
        CatalogAssociateCategoryViewModel GetAssociatedCategoryDetails(CatalogAssociateCategoryViewModel catalogAssociateCategoryViewModel);

        /// <summary>
        /// Update details(Display order, active status, etc.)of category associated to catalog.
        /// </summary>
        /// <param name="catalogAssociateCategoryModel">Catalog Associate Category Model.</param>
        /// <returns>Catalog Associate Category Model.</returns>
        CatalogAssociateCategoryViewModel UpdateAssociatedCategoryDetails(CatalogAssociateCategoryViewModel catalogAssociateCategoryViewModel);

        /// <summary>
        /// Get the list of associated products to catalog.
        /// </summary>
        /// <param name="catalogId">Catalog id.</param>
        /// <param name="categoryHierarchyId">Category id.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns>Returns the list of associated products to catalog.</returns>
        CatalogAssociationViewModel GetAssociatedProducts(int catalogId, int categoryHierarchyId, int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get the list of unassociated categories to catalog.
        /// </summary>
        /// <param name="catalogId">Catalog id.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns>Returns the list of unassociated categories to catalog.</returns>
        CatalogAssociateCategoryListViewModel GetUnAssociatedCategoryList(int catalogId, int categoryId, int categoryHierarchyId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get the list of unassociated products to catalog.
        /// </summary>
        /// <param name="catalogId">Catalog id.</param>
        /// <param name="categoryId">Category id.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns>Returns the list of unassociated products to catalog.</returns>
        ProductDetailsListViewModel GetUnAssociatedProductsList(int catalogId, int categoryId, int categoryHierarchyId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Checks whether catalog name already exists.
        /// </summary>
        /// <param name="catalogName">catalog name to check.</param>
        /// <param name="pimCatalogId">pim catalog id.</param>
        /// <returns>Returns true if catalog name already exists else false.</returns>
        bool CheckCatalogNameExist(string catalogName, int pimCatalogId);

        /// <summary>
        /// Move category.
        /// </summary>
        /// <param name="folderId">category Id to move</param>
        /// <param name="addtoFolderId">Category Id to which move.</param>
        /// <param name="catalogId">catalog Id.</param>
        /// <returns>Returns true if MoveCategory else false.</returns>
        bool MoveCategory(int folderId, int addtoFolderId, int catalogId, out string errorMessage);

        /// <summary>
        /// Remove products from catalog, If ProfileCatalogId is > 0 and ProfileCatalogCategoryIds are there, method will remove products from catlog profile.
        /// </summary>
        /// <param name="catalogAssociationViewModel">CatalogAssociationViewModel</param>
        /// <returns>Returns true if products removed successfully else false.</returns>
        bool UnAssociateProduct(CatalogAssociationViewModel catalogAssociationViewModel);

        /// <summary>
        /// Get Catalog Publish Status
        /// </summary>
        /// <param name="pimCatalogId">pimCatalogId</param>
        /// <param name="expands">Expands to be retrieved along with catalog list.</param>
        /// <param name="filters">Filters to be applied on catalog list.</param>
        /// <param name="sorts">Sorting to be applied on catalog list.</param>
        /// <param name="pageIndex">Start page index of catalog list.</param>
        /// <param name="pageSize">Page size of catalog list.</param>
        /// <returns>Publish CatalogLog List View Model</returns>
        PublishCatalogLogListViewModel GetCatalogPublishStatus(int pimCatalogId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Update Catalog Category Product.
        /// </summary>
        /// <param name="catalogAssociationViewModel">catalogAssociationViewModel</param>
        /// <returns>Returns true if updated else false</returns>
        bool UpdateCatalogCategoryProduct(CatalogAssociationViewModel catalogAssociationViewModel);
           
        /// <summary>
        /// Get Associated Catalog Tree
        /// </summary>
        /// <param name="pimProductId">Product Id</param>
        /// <returns>Catalog Tree JSON</returns>
        string GetAssociatedCatalogTree(int pimProductId);

        /// <summary>
        /// Get Active product Filter
        /// </summary>
        /// <param name="pimProductId">Product Id</param>
        /// <returns>bool</returns>
        bool GetActiveProductFilter(Dictionary<string, string> queryStringValues, FilterCollection filters);

        /// <summary>
        /// Set Active product Filter
        /// </summary>
        /// <param name="pimProductId">Product Id</param>       
        void SetActiveProductFilter(FilterCollection filters, bool isActiveProducts);

        #region Product(s) & Category(s) operation to catalog category  

        /// <summary>
        /// Associate the product(s) to catalog categories
        /// </summary>
        /// <param name="catalogAssociationModel">Model will carry product ids and category id</param>
        /// <returns>Returns if successful true else false</returns>
        bool AssociateProductsToCatalogCategory(CatalogAssociationViewModel catalogAssociationViewModel);

        /// <summary>
        /// //UnAssociate the product(s) from catalog category
        /// </summary>
        /// <param name="catalogAssociationViewModel">CatalogAssociationViewModel</param>
        /// <returns>Returns if products removed successfully true else false.</returns>
        bool UnAssociateProductsFromCatalogCategory(CatalogAssociationViewModel catalogAssociationViewModel);


        /// <summary>
        /// Associate the category(s) to catalog tree
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel</param>
        /// <returns>Returns if associated true else false</returns>
        bool AssociateCategoryToCatalog(CatalogAssociationViewModel catalogAssociationViewModel);

        /// <summary>
        /// UnAssociate the category(s) from catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel</param>
        /// <param name="message">Message if error occurred.</param>
        /// <returns>Returns if category unassociated successfully true else false</returns>
        bool UnAssociateCategoryFromCatalog(CatalogAssociationViewModel catalogAssociationViewModel);

        #endregion Product(s) & Category(s) operation to catalog category     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pimCatalogId"></param>
        /// <param name="pimCategoryId"></param>
        /// <param name="displayOrder"></param>
        /// <param name="isDown"></param>
        /// <returns></returns>
        bool MoveCategory(int pimCatalogId, int pimCategoryHierarchyId, int displayOrder, bool isDown);

    }
}