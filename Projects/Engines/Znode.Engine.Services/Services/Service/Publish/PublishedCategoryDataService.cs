using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;


namespace Znode.Engine.Services
{
    public class PublishedCategoryDataService : BaseService, IPublishedCategoryDataService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodePublishCategoryEntity> _publishCategoryEntity;

        #endregion

        #region Constructor
        public PublishedCategoryDataService()
        {
            _publishCategoryEntity = new ZnodeRepository<ZnodePublishCategoryEntity>(HelperMethods.Context);
        }
        #endregion

        //Get Publish Category List
        public virtual List<ZnodePublishCategoryEntity> GetPublishedCategoryList(PageListModel pageListModel)
        {
            if(string.IsNullOrEmpty(pageListModel?.OrderBy))
                return _publishCategoryEntity.GetEntityListWithoutOrderBy(pageListModel.EntityWhereClause.WhereClause, null, pageListModel.EntityWhereClause.FilterValues)?.ToList();
            
            else
                return _publishCategoryEntity.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues)?.ToList();
        }


        //Get Publish Category Paged List
        public virtual List<ZnodePublishCategoryEntity> GetPublishCategoryPageList(PageListModel pageListModel, out int rowCount)
            => _publishCategoryEntity.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out rowCount)?.ToList();


        //Get published category
        public virtual ZnodePublishCategoryEntity GetPublishedCategory(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishCategoryEntity.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);

        }

        //Get category List by Catalog Id
        public virtual List<ZnodePublishCategoryEntity> GetCategoryListByCatalogId(int publishCatalogId, int localeId)
            => _publishCategoryEntity.Table.Where(x => x.ZnodeCatalogId == publishCatalogId && x.LocaleId == localeId)?.ToList();

        public virtual List<ZnodePublishCategoryEntity> GetActiveCategoryListByCatalogId(int publishCatalogId, int versionId)
           => _publishCategoryEntity.Table.Where(x => x.ZnodeCatalogId == publishCatalogId && x.IsActive && x.IsActive && x.VersionId == versionId)?.ToList();


        //Get category by category Id
        public virtual ZnodePublishCategoryEntity GetCategoryByCategoryId(int categoryId, int localeId)
            => _publishCategoryEntity.Table.FirstOrDefault(x => x.ZnodeCategoryId == categoryId && x.LocaleId == localeId);

        //Get Product Categories by Ids
        public virtual List<ZnodePublishCategoryEntity> GetCategoryByIds(int localeId, int[] categoryIds)
        {
            string Ids = string.Join<int>(",", categoryIds);
            FilterCollection filters = new FilterCollection();
            filters.Add("ZnodeCategoryId", FilterOperators.In, Ids);
            filters.Add(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString());

            PageListModel pageListModel = new PageListModel(filters, null, null);

            return GetPublishedCategoryList(pageListModel);
        }

        //Get catalog Category List
        public virtual List<ZnodePublishCategoryEntity> GetCatalogCategoryList(int localeId, int catalogId, NameValueCollection orderBy, int? versionId = 0)
        {

            versionId = versionId > 0 ? versionId : GetCatalogVersionId();

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple("ZnodeCatalogId", FilterOperators.Equals, catalogId.ToString()));
            filters.Add(new FilterTuple("LocaleId", FilterOperators.Equals, localeId.ToString()));
            filters.Add(new FilterTuple("VersionId", FilterOperators.Equals, versionId.ToString()));
            PageListModel pageListModel = new PageListModel(filters, orderBy, null);

            return GetPublishedCategoryList(pageListModel);
        }














    }
}
