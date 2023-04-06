using System.Collections.Generic;
using System.Linq;

using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.Search
{
    public class SearchCategoryService : ISearchCategoryService
    {
        #region Protected Variables

        protected readonly IZnodeRepository<ZnodePublishCategoryEntity> _publishCategoryEntity;

        #endregion Protected Variables

        #region Constructor

        public SearchCategoryService()
        {
            _publishCategoryEntity = new ZnodeRepository<ZnodePublishCategoryEntity>(HelperMethods.Context);
        }

        #endregion Constructor

        #region Public Methods

        //Gets product Category.
        public virtual List<SearchCategory> GetProductCategory(int categoryIds, int localeId, int catalogId, List<SEODetailsModel> categorySeoDetails)
        {
            FilterCollection categoryFilter = new FilterCollection() { new FilterTuple(WebStoreEnum.ZnodeCategoryId.ToString(), FilterOperators.Equals,  categoryIds.ToString()),
                                                                               new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()),
                                                                               new FilterTuple(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString())};

            List<ZnodePublishCategoryEntity> _categoryList = _publishCategoryEntity.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(categoryFilter.ToFilterDataCollections()).WhereClause).ToList();

            List<PublishedCategoryEntityModel> categoryList = _categoryList.ToModel<PublishedCategoryEntityModel>().ToList();

            return categoryList?.Select(x => GetElasticCategory(categorySeoDetails, x)).ToList();
        }


        //Get published category list
        public virtual List<ZnodePublishCategoryEntity> GetPublishCategoryList(int catalogId, int versionId)
        {
            return GetPublishCategoryList(catalogId, new int[] { versionId });
        }

        //Get all categories id associated to the catalog
        public virtual List<ZnodePublishCategoryEntity> GetPublishCategoryList(int catalogId, int[] versionIds)
        {
            FilterCollection filters = new FilterCollection();

            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString());
            filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.In, string.Join(",", versionIds));
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollections());

            List<ZnodePublishCategoryEntity> publishCategoryEntity = _publishCategoryEntity.GetEntityList(whereClauseModel.WhereClause).ToList();

            //Filter list by expiration date and activation date.
            publishCategoryEntity = GetFilterDateResult(publishCategoryEntity);

            return publishCategoryEntity;
        }
        
        #endregion Public Methods

        #region Protected Methods

        //Gets parent category hierarchy.
        protected virtual  List<SearchCategory> GetParentCategories(int[] parentCategoryIds, List<SEODetailsModel> categorySeoDetails)
        {
            List<SearchCategory> parentCategories = new List<SearchCategory>();
            foreach (int categoryId in parentCategoryIds)
            {
                FilterCollection categoryFilter = new FilterCollection() { new FilterTuple(WebStoreEnum.ZnodeCategoryId.ToString(), FilterOperators.In, string.Join(",", categoryId)) };

                ZnodePublishCategoryEntity _parentCategory = _publishCategoryEntity.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(categoryFilter.ToFilterDataCollections()).WhereClause);

                PublishedCategoryEntityModel parentCategory = _parentCategory.ToModel<PublishedCategoryEntityModel>();
                if (HelperUtility.IsNotNull(parentCategory))
                {
                    SearchCategory elasticParentCategory = GetElasticCategory(categorySeoDetails, parentCategory);

                    parentCategories.Add(elasticParentCategory);
                }
            }
            return parentCategories;
        }

        //Maps category entity to Elastic category.
        protected virtual SearchCategory GetElasticCategory(List<SEODetailsModel> categorySeoDetails, PublishedCategoryEntityModel parentCategory)
        {
            SearchCategory category = new SearchCategory();

            category.categoryname = parentCategory.Name;
            category.categoryid = parentCategory.ZnodeCategoryId;
            category.seourl = categorySeoDetails.Where(seoDetail => seoDetail.SEOId == parentCategory.ZnodeCategoryId && seoDetail.LocaleId == parentCategory.LocaleId)?.FirstOrDefault()?.SEOUrl;
            category.parentcategories = (parentCategory.ZnodeParentCategoryIds?.Count() > 0) ? GetParentCategories(parentCategory.ZnodeParentCategoryIds, categorySeoDetails) : null;
            category.isactive = parentCategory.IsActive;
            category.ActivationDate = parentCategory.ActivationDate;
            category.ExpirationDate = parentCategory.ExpirationDate;
            return category;
        }

        //Filter list by expiration date and activation date.
        protected virtual List<ZnodePublishCategoryEntity> GetFilterDateResult(List<ZnodePublishCategoryEntity> publishCategoryEntity)
            => publishCategoryEntity.Where(x =>
             (x.ActivationDate == null || x.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate())
              && (x.ExpirationDate == null || x.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate()))
             .ToList();

        #endregion Protected Methods
    }
}
