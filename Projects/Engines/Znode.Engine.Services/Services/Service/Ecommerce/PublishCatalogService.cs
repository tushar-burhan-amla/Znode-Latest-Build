using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Constants;
using Znode.Libraries.Data;
using Znode.Libraries.ECommerce.Utilities;
using Utilities = Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Data.DataModel;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class PublishCatalogService : BaseService, IPublishCatalogService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePimCatalog> _catalogRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        #endregion

        #region Constructor
        public PublishCatalogService()
        {
            _catalogRepository = new ZnodeRepository<ZnodePimCatalog>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
        }
        #endregion

        #region Public Methods
        public virtual PublishCatalogModel GetPublisCatalog(int publishCatalogId, int? localeId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //get publish category  
            ZnodeLogging.LogMessage("publishCatalogId and localeId to get publish catalog: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { publishCatalogId, localeId });
            PublishCatalogModel publishCatalog = GetService<IPublishedCatalogDataService>().GetPublishCatalogById(publishCatalogId)?.ToModel<PublishCatalogModel>();

            //get products,categories associated to catalog from expands
            GetDataFromExpands(expands, publishCatalog, Convert.ToInt32(localeId));
            ZnodeLogging.LogMessage("PublishCatalogModel details: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { publishCatalog?.PublishCatalogId, publishCatalog?.PromotionId,
            publishCatalog?.CatalogName, publishCatalog?.PublishCategories.Count, publishCatalog?.PublishProducts.Count});
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return publishCatalog;
        }

        //Get list of published catelog from mango
        public virtual PublishCatalogListModel GetPublisCatalogList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int localeId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName == Utilities.FilterKeys.LocaleId)?.FilterValue, out localeId);
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.LocaleId);
            
            filters.Add(WebStoreEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());

            string versionIds = filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType) ? GetCatalogAllVersionIds(localeId) : GetCatalogAllVersionIds();
            if(!string.IsNullOrEmpty(versionIds))
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.In, versionIds);
            //Replace filter keys filter keys
            ReplaceFilterKeys(ref filters);
            ReplaceSortKeys(ref sorts);
            if (filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType))
                filters.RemoveAll(x => x.Item1 == Utilities.FilterKeys.RevisionType);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            //get publish categories   
            ZnodeLogging.LogMessage("PageListModel to get catalog list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<PublishCatalogModel> catalogs = GetService<IPublishedCatalogDataService>().GetPublishedCatalogPagedList(pageListModel)?.ToModel<PublishCatalogModel>()?.ToList();

        
            //map catalog entity to catalog model
            PublishCatalogListModel publishCatalogs = new PublishCatalogListModel() { PublishCatalogs = catalogs };

            //get products,categories associated to catalogs from expands
            GetDataFromExpands(expands, publishCatalogs, localeId);

            //Map pagination parameters
            publishCatalogs.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Publish catalog list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, publishCatalogs?.PublishCatalogs?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return publishCatalogs;
        }

        //Get publish catelog excluding assigned Ids.
        public virtual PublishCatalogListModel GetUnAssignedPublishCatelogList(string assignedIds, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            filters.Add(Utilities.FilterKeys.ZnodeCatalogId, FilterOperators.NotIn, assignedIds);
            return GetPublisCatalogList(expands, filters, sorts, page);
        }


        #endregion

        #region Private Methods
        //get products,categories associated to catalog from expands
        private void GetDataFromExpands(NameValueCollection expands, PublishCatalogModel publishCatalog, int localeId)
        {
            if (publishCatalog?.PublishCatalogId > 0 && (expands?.HasKeys() ?? false))
            {
                foreach (string key in expands.Keys)
                {
                    if (string.Equals(key, ExpandKeys.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        //get publish categories associated with category
                        List<PublishCategoryModel> categories = GetPublishCategories(new List<int> { publishCatalog.PublishCatalogId }, localeId);

                        //map categories to catalog
                        publishCatalog.PublishCategories = categories;
                    }

                    if (string.Equals(key, ExpandKeys.Product, StringComparison.OrdinalIgnoreCase))
                    {
                        //get publish products associated with catalog
                        List<PublishProductModel> products = GetPublishProducts(new List<int> { publishCatalog.PublishCatalogId }, localeId);

                        //map products to catalog
                        publishCatalog.PublishProducts = products;
                    }
                }
            }
        }

        //get products,categories associated to catalogs from expands
        private void GetDataFromExpands(NameValueCollection expands, PublishCatalogListModel publishCatalogs, int localeId)
        {
            if (publishCatalogs?.PublishCatalogs?.Count > 0 && (expands?.HasKeys() ?? false))
            {
                foreach (string key in expands.Keys)
                {
                    if (string.Equals(key, ExpandKeys.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        //get publish categories associated with category
                        List<PublishCategoryModel> categories = GetPublishCategories(publishCatalogs.PublishCatalogs.Select(s => s.PublishCatalogId), localeId);

                        //map categories to catalog
                        publishCatalogs.PublishCatalogs.ForEach(
                            x => x.PublishCategories = categories.Where(s => s.ZnodeCatalogId == x.PublishCatalogId)?.ToList());
                    }

                    if (string.Equals(key, ExpandKeys.Product, StringComparison.OrdinalIgnoreCase))
                    {
                        //get publish products associated with catalog
                        List<PublishProductModel> products = GetPublishProducts(publishCatalogs.PublishCatalogs.Select(s => s.PublishCatalogId), localeId);

                        //map products to catalog
                        publishCatalogs.PublishCatalogs.ForEach(
                           x => x.PublishProducts = products.Where(s => s.ZnodeCatalogId == x.PublishCatalogId)?.ToList());
                    }
                }
            }
        }


        //get publish products associated with catalog
        private List<PublishProductModel> GetPublishProducts(IEnumerable<int> catalogIds, int localeId)
        {
            FilterCollection filters = new FilterCollection();
             filters.Add("ZnodeCatalogId", FilterOperators.Equals, string.Join(",", catalogIds));
            if (localeId > 0) 
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());

            return GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(filters, null, null))?.ToModel<PublishProductModel>()?.ToList();

        }

        //get publish categories associated with catalog
        private List<PublishCategoryModel> GetPublishCategories(IEnumerable<int> catalogIds, int localeId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add("ZnodeCatalogId", FilterOperators.Equals, string.Join(",", catalogIds));
            if (localeId > 0)
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());

            return GetService<IPublishedCategoryDataService>().GetPublishedCategoryList(new PageListModel(filters, null, null))?.ToModel<PublishCategoryModel>()?.ToList();
        }

        //Replace Filter Keys
        private void ReplaceFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.CatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.CatalogId, Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.PublishedCatalogName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.PublishedCatalogName.ToLower(), Utilities.FilterKeys.PublishedCatalogName); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ZnodecatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ZnodecatalogId, Utilities.FilterKeys.ZnodeCatalogId); }           
            }
            ReplaceFilterKeysForOr(ref filters);
        }

        //Replace Filter Keys
        private void ReplaceFilterKeysForOr(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (tuple.Item1.Contains("|"))
                {
                    List<string> newValues = new List<string>();
                    foreach (var item in tuple.Item1.Split('|'))
                    {
                        if (string.Equals(item, Utilities.FilterKeys.PublishedCatalogName, StringComparison.OrdinalIgnoreCase)) {newValues.Add(Utilities.FilterKeys.PublishedCatalogName); }
                        else if (string.Equals(item, Utilities.FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.Name); }
                        else newValues.Add(item);
                    }
                    ReplaceFilterKeyName(ref filters, tuple.Item1, string.Join("|", newValues));
                }
            }
        }

        //Replace sort Keys
        private void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            foreach (string key in sorts.Keys)
            {
                if (string.Equals(key, Utilities.FilterKeys.PublishCatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishCatalogId.ToLower(), Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(key, Utilities.FilterKeys.PublishedCatalogName.ToLower(), StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishedCatalogName.ToLower(), Utilities.FilterKeys.PublishedCatalogName); }
            }
        }



        #endregion
    }
}
