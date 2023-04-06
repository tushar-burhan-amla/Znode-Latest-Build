using System.Collections.Generic;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services
{
    public interface IPublishedCatalogDataService
    {
        /// <summary>
        /// Gets the list of Published catalogs
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns>List of ZnodePublishCatalogEntity</returns>
        List<ZnodePublishCatalogEntity> GetPublishCatalogs(PageListModel pageListModel);

        /// <summary>
        /// Gets the list of Published catalogs
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns>List of ZnodePublishCatalogEntity</returns>
        List<ZnodePublishCatalogEntity> GetPublishedCatalogPagedList(PageListModel pageListModel);


        /// <summary>
        /// Gets the Published catalog by Id
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <returns>ZnodePublishCatalogEntity</returns>
        ZnodePublishCatalogEntity GetPublishCatalogById(int publishCatalogId);

        /// <summary>
        /// Gets the published catalog List by catalog id
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="versionId">versionId</param>
        /// <returns>List of ZnodePublishCatalogEntity</returns>
        List<ZnodePublishCatalogEntity> GetPublishCatalogListById(int publishCatalogId, int? versionId = 0);

        /// <summary>
        /// Gets the catalog attribute
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="localeId">localeId</param>
        /// <param name="versionId">versionId</param>
        /// <returns>List of ZnodePublishCatalogAttributeEntity</returns>
        List<ZnodePublishCatalogAttributeEntity> GetPublishCatalogAttribute(int publishCatalogId, int localeId, int? versionId = 0);

        /// <summary>
        /// Gets the Catalog attribute List
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns>List of ZnodePublishCatalogAttributeEntity</returns>
        List<ZnodePublishCatalogAttributeEntity> GetPublishCatalogAttributeList(PageListModel pageListModel);

        /// <summary>
        /// Gets the paged Catalog attribute List
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns>List of ZnodePublishCatalogAttributeEntity</returns>
        List<ZnodePublishCatalogAttributeEntity> GetPublishCatalogAttributePagedList(PageListModel pageListModel);



    }
}
