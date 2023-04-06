using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;

namespace Znode.Engine.Services
{
    public class PublishedCatalogDataService : BaseService, IPublishedCatalogDataService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodePublishCatalogEntity> _publishCatalogEntity;
        private readonly IZnodeRepository<ZnodePublishCatalogAttributeEntity> _publishCatalogAttributeEntity;

        #endregion

        #region Constructor
        public PublishedCatalogDataService()
        {
            _publishCatalogEntity = new ZnodeRepository<ZnodePublishCatalogEntity>(HelperMethods.Context);
            _publishCatalogAttributeEntity = new ZnodeRepository<ZnodePublishCatalogAttributeEntity>(HelperMethods.Context);
        }
        #endregion

        // Gets the list of Published catalogs
        public virtual List<ZnodePublishCatalogEntity> GetPublishCatalogs(PageListModel pageListModel)
             => _publishCatalogEntity.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues)?.ToList();

        //Gets Paged List
        public virtual List<ZnodePublishCatalogEntity> GetPublishedCatalogPagedList(PageListModel pageListModel)
         => _publishCatalogEntity.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount).ToList();


        //Get Publish Catalog by Id
        public virtual ZnodePublishCatalogEntity GetPublishCatalogById(int publishCatalogId)
            => _publishCatalogEntity.Table.FirstOrDefault(x => x.ZnodeCatalogId == publishCatalogId);

        //Gets the published catalog List by catalog id
        public virtual List<ZnodePublishCatalogEntity> GetPublishCatalogListById(int publishCatalogId, int? versionId = 0)
            => _publishCatalogEntity.Table.Where(x => x.ZnodeCatalogId == publishCatalogId && x.VersionId == versionId)?.ToList();

        //Gets the catalog attribute based on parameters
        public virtual List<ZnodePublishCatalogAttributeEntity> GetPublishCatalogAttribute(int publishCatalogId, int localeId, int? versionId = 0)
            => _publishCatalogAttributeEntity.Table.Where(x => x.ZnodeCatalogId == publishCatalogId && x.LocaleId == localeId && x.VersionId == versionId)?.ToList();

        // Gets the Catalog attribute List
        public virtual List<ZnodePublishCatalogAttributeEntity> GetPublishCatalogAttributeList(PageListModel pageListModel)
        {
            if(string.IsNullOrEmpty(pageListModel?.OrderBy))
                return _publishCatalogAttributeEntity.GetEntityListWithoutOrderBy(pageListModel.EntityWhereClause.WhereClause, null, pageListModel.EntityWhereClause.FilterValues)?.ToList();
            else
              return _publishCatalogAttributeEntity.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues)?.ToList();

        }



        //Gets Paged List
        public virtual List<ZnodePublishCatalogAttributeEntity> GetPublishCatalogAttributePagedList(PageListModel pageListModel)
         => _publishCatalogAttributeEntity.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();


    }
}
