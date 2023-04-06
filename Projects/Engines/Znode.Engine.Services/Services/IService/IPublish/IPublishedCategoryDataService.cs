using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPublishedCategoryDataService
    {
        /// <summary>
        /// Get Publish Category List
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns>List of ZnodePublishCategoryEntity<ZnodePublishCategoryEntity></returns>
        List<ZnodePublishCategoryEntity> GetPublishedCategoryList(PageListModel pageListModel);

        /// <summary>
        /// Get Publish Category Paged List
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <param name="rowCount">rowCount</param>
        /// <returns>List of ZnodePublishCategoryEntity</returns>
        List<ZnodePublishCategoryEntity> GetPublishCategoryPageList(PageListModel pageListModel, out int rowCount);


        /// <summary>
        /// Get Publish Category
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns>ZnodePublishCategoryEntity</returns>
        ZnodePublishCategoryEntity GetPublishedCategory(FilterCollection filters);

        /// <summary>
        /// Get Active Category List By catalog Id
        /// </summary>
        /// <param name="publishCatalogId">publish Catalog Id</param>
        /// <param name="localeId">locale Id</param>
        /// <returns>List of ZnodePublishCategoryEntity</returns>
        List<ZnodePublishCategoryEntity> GetActiveCategoryListByCatalogId(int publishCatalogId, int versionId);

        /// <summary>
        /// Get category List by Catalog Id
        /// </summary>
        /// <param name="publishCatalogId">publishCatalog Id</param>
        /// <param name="localeId">locale Id</param>
        /// <returns>List of ZnodePublishCategoryEntity<ZnodePublishCategoryEntity</returns>
        List<ZnodePublishCategoryEntity> GetCategoryListByCatalogId(int publishCatalogId, int localeId);

        /// <summary>
        /// Get category by category Id
        /// </summary>
        /// <param name="categoryId">category Id</param>
        /// <param name="localeId">locale Id</param>
        /// <returns>ZnodePublishCategoryEntity</returns>
        ZnodePublishCategoryEntity GetCategoryByCategoryId(int categoryId, int localeId);

        /// <summary>
        ///  Get catalog Category List
        /// </summary>
        /// <param name="localeId">locale Id</param>
        /// <param name="catalogId">catalog Id</param>
        /// <param name="versionId">version Id</param>
        /// <param name="profileId">profile Id</param>
        /// <param name="orderBy">orderBy</param>
        /// <returns>List of ZnodePublishCategoryEntity<ZnodePublishCategoryEntity> </returns>
        List<ZnodePublishCategoryEntity> GetCatalogCategoryList(int localeId, int catalogId, NameValueCollection orderBy, int? versionId = 0 );

        /// <summary>
        /// Get category by ids
        /// </summary>
        /// <param name="localeId">locale Id</param>
        /// <param name="categoryIds">category Ids</param>
        /// <returns>List of ZnodePublishCategoryEntity</returns>
        List<ZnodePublishCategoryEntity> GetCategoryByIds(int localeId, int[] categoryIds);




    }
}
