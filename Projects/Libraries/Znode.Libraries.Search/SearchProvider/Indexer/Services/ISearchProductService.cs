using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Libraries.Search
{
    public interface ISearchProductService
    {
        /// <summary>
        /// Get all products for creating index.
        /// </summary>
        /// <param name="catalogId">Catalog Id</param>
        /// <param name="versionId">catalog version Id</param>
        /// <param name="start">Start index of the product list.</param>
        /// <param name="pageLength">Page length of the product list.</param>
        /// <param name="indexstartTime">current create index start time</param>
        /// <param name="totalCount">Total count of the product.</param>
        /// <returns>All the products according to catalog Id.</returns>
        List<SearchProduct> GetAllProducts(int catalogId, int versionId, IEnumerable<int> publishCategoryIds, int start, int pageLength,long indexStartTime, out decimal totalPages);

        /// <summary>
        ///  Get all products for creating index.
        /// </summary>
        /// <param name="catalogId">Catalog Id</param>
        /// <param name="versionIds">VersionIds</param>
        /// <param name="publishCategoryIds">PublishCategoryIds</param>
        /// <param name="indexStartTime">IndexStartTime</param>
        /// <param name="pageIndex">Paging start from</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="isPublishDraftProductsOnly">To specify whether to publish only draft products or not.</param>
        /// <returns>All the products according to catalogId, versionIds, publishCategoryIds.</returns>
        List<SearchProduct> GetAllProducts(int catalogId, int[] versionIds, IEnumerable<int> publishCategoryIds, long indexStartTime, int pageIndex, int pageSize, bool isPublishDraftProductsOnly);

        /// <summary>
        /// Get CatalogVersion id
        /// </summary>
        /// <param name="catalogId">catalog id.</param>
        /// <returns>catalog version id.</returns>
        int? GetCatalogVersionId(int catalogId);

        /// <summary>
        /// Get the synonyms data.
        /// </summary>
        /// <param name="catalogId">Catalog id.</param>
        /// <returns>Returns synonyms data.</returns>
        List<ZnodeSearchSynonym> GetSynonymsData(int catalogId);

        /// <summary>
        /// Get keywords data.
        /// </summary>
        /// <param name="catalogId">Catalog id.</param>
        /// <returns>Returns keywords data.</returns>
        List<ZnodeSearchKeywordsRedirect> GetKeywordsData(int catalogId);

        /// <summary>
        /// Converts the published products to elastic products.
        /// </summary>
        /// <param name="publishedProducts">Published Products.</param>
        /// <returns>Returns elastic products.</returns>
        List<SearchProduct> GetElasticProducts(List<ZnodePublishProductEntity> publishedProducts, long indexStartTime);

        /// <summary>
        /// Get Catalog Version Id.
        /// </summary>
        /// <param name="publishCatalogId">publish catalog id.</param>
        /// <param name="revisionType"></param>
        /// <param name="localeId"></param>
        /// <returns></returns>
        int GetVersionId(int publishCatalogId, string revisionType, int localeId = 0);

        /// <summary>
        /// Get Catalog Latest Version Id.
        /// </summary>
        /// <param name="publishCatalogId">publish catalog id.</param>
        /// <param name="revisionType"></param>
        /// <param name="localeId"></param>
        /// <returns>Return Version id</returns>
        int GetLatestVersionId(int publishCatalogId, string revisionType, int localeId = 0);

        /// <summary>
        /// Get catalog latest version Ids.
        /// </summary>
        /// <param name="publishCatalogId">Publish Catalog Id</param>
        /// <param name="revisionType">RevisionType</param>
        /// <param name="localeIds">LocaleIds</param>
        /// <returns>Return version ids based on active locales and revision type</returns>
        int[] GetLatestVersionId(int publishCatalogId, string revisionType, int[] localeIds);

        /// <summary>
        /// Get all publish product count
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="versionIds">versionIds</param>
        /// <returns>Return publish product count</returns>
        int GetAllProductCount(int publishCatalogId, int[] versionIds);

        /// <summary>
        /// Set elastic product.
        /// </summary>
        /// <param name="publishCategoyIds"></param>
        /// <param name="indexStartTime"></param>
        /// <param name="elasticProductList"></param>
        /// <param name="productsWithReviewCount"></param>
        void SetElasticProduct(IEnumerable<int> publishCategoyIds, long indexStartTime, List<SearchProduct> elasticProductList, List<PublishProductModel> productsWithReviewCount);

        /// <summary>
        /// Get Search Profile based on publish catalog Id
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <returns>PublishSearchProfileModel </returns>
        PublishSearchProfileModel GetPublishSearchProfile(int publishCatalogId);

        /// <summary>
        /// To the catalog index details from database.
        /// </summary>
        /// <param name="publishCatalogId">PublishCatalogId</param>
        /// <returns>ZnodeCatalogIndex</returns>
        ZnodeCatalogIndex GetCatalogIndexByPublishCatalogId(int publishCatalogId);

        /// <summary>
        /// To update catalog index details in ZnodeCatalogIndex table
        /// </summary>
        /// <param name="catalogIndex">ZnodeCatalogIndex</param>
        /// <returns>True if data updated successfully</returns>
        bool UpdateCatalogIndex(ZnodeCatalogIndex catalogIndex);
    }
}