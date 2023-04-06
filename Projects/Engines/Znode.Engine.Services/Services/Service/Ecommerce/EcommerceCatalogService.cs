using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class EcommerceCatalogService : BaseService, IEcommerceCatalogService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IPublishedCatalogDataService _publishedCatalogDataService;
        #endregion

        #region Constructor
        public EcommerceCatalogService()
        {
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _publishedCatalogDataService = GetService<IPublishedCatalogDataService>();
        }
        #endregion

        #region Public Methods
        //Method to get the list of all published catalogs.
        public virtual PublishCatalogListModel GetPublishCatalogList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("PageListModel to get publish catalog list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodePublishCatalogEntity> publishCatalogs = PublishCatalogList(pageListModel, expands);

            ZnodeLogging.LogMessage("Publish catalog list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, publishCatalogs?.Count);
            PublishCatalogListModel publishCatalogList = new PublishCatalogListModel();
            publishCatalogList.PublishCatalogs = publishCatalogs?.ToModel<PublishCatalogModel>()?.ToList();

            publishCatalogList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return publishCatalogList;
        }

        //Method to get catalogs associated with portal as per portalId.
        public virtual PortalCatalogListModel GetAssociatedPortalCatalogByPortalId(int portalId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNull(filters))
                filters = new FilterCollection();

            if (IsNull(expands))
                expands = new NameValueCollection();

            //Add portal Id in filter
            filters.Add(Convert.ToString(ZnodePortalCatalogEnum.PortalId), ProcedureFilterOperators.Equals, Convert.ToString(portalId));
            expands.Add(ZnodePortalCatalogEnum.ZnodePublishCatalog.ToString(), ZnodePortalCatalogEnum.ZnodePublishCatalog.ToString());

            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("PageListModel to get portal catalog list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<ZnodePortalCatalog> portalCatalog = _portalCatalogRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Portal catalog list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, portalCatalog?.Count);

            PortalCatalogListModel listModel = new PortalCatalogListModel();
            listModel.PortalCatalogs = portalCatalog?.Count > 0 ? portalCatalog.ToModel<PortalCatalogModel>().ToList() : new List<PortalCatalogModel>();

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Method to get catalogs associated with portal.
        public virtual PortalCatalogListModel GetAssociatedPortalCatalog(ParameterModel filterIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Checks promotion id.
            if (string.IsNullOrEmpty(filterIds?.Ids))
                return null;

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalCatalogEnum.PortalId.ToString(), FilterOperators.In, filterIds.Ids));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());


            ZnodeLogging.LogMessage("Where clause and filter values to get portal catalog list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { whereClauseModel?.WhereClause, whereClauseModel?.FilterValues });
            IList<ZnodePortalCatalog> portalCatalog = _portalCatalogRepository.GetEntityList(whereClauseModel?.WhereClause, whereClauseModel?.FilterValues);

            PortalCatalogListModel listModel = new PortalCatalogListModel();
            listModel.PortalCatalogs = portalCatalog?.Count > 0 ? portalCatalog.ToModel<PortalCatalogModel>().GroupBy(test => test.CatalogName).Select(grp => grp.First()).ToList() : new List<PortalCatalogModel>();
            ZnodeLogging.LogMessage("Portal catalog list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, listModel?.PortalCatalogs?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Method Update catalog associated with portal.
        public virtual bool UpdatePortalCatalog(PortalCatalogModel portalCatalogModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNull(portalCatalogModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            if (portalCatalogModel.PortalCatalogId > 0)
            {
                //Get existing Portal Catalog Details.
                ZnodePortalCatalog portalCatalog = _portalCatalogRepository.Table.Where(x => x.PortalCatalogId == portalCatalogModel.PortalCatalogId)?.FirstOrDefault();

                //Check whether the Existing Catalog Id changed in the request. To Update the Catalog Details.
                if (portalCatalog?.PublishCatalogId != portalCatalogModel.PublishCatalogId)
                {
                    //Delete all the associated mapping for the Category & Product Widget.
                    DeletePortalWidgetConfiguration(portalCatalogModel.PortalId);
                }

                status = _portalCatalogRepository.Update(portalCatalogModel.ToEntity<ZnodePortalCatalog>());
            }
            ZnodeLogging.LogMessage(status ? string.Format(Admin_Resources.SuccessPortalCatalogUpdate, portalCatalogModel.PortalCatalogId) : Admin_Resources.ErrorPortalCatalogUpdate, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return status;
        }

        //Method Gets Portal catalog.
        public virtual PortalCatalogModel GetPortalCatalog(int portalCatalogId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (portalCatalogId > 0)
            {
                ZnodeRepository<ZnodePublishCatalogEntity> _publishCatalogEntity = new ZnodeRepository<ZnodePublishCatalogEntity>(HelperMethods.Context);
                ZnodeLogging.LogMessage("portalCatalogId to get PortalCatalogModel: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, portalCatalogId);
                PortalCatalogModel portalCatalogModel = _publishedCatalogDataService.GetPublishCatalogById(portalCatalogId)?.ToModel<PortalCatalogModel>();
                if(IsNotNull(portalCatalogModel))
                portalCatalogModel.PublishCatalogs = _publishCatalogEntity.Table?.ToList()?.ToModel<PublishCatalogModel>().ToList();
                ZnodeLogging.LogMessage("PortalCatalogModel list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, portalCatalogModel?.PublishCatalogs?.Count);
                return portalCatalogModel;
            }
            return null;
        }

        //Method Gets the tree structure for Catalog.
        public virtual List<CategoryTreeModel> GetCatalogTree(int catalogId, int categoryId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (catalogId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCatalogIdShouldBeGreaterThanOne);

            ZnodeLogging.LogMessage("catalogId and categoryId to get GetCatalogTree: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { catalogId, categoryId });
            int defaultLocaleId = Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale);
            List<CategoryTreeModel> list = new List<CategoryTreeModel>();

            // to identify catalog (i.e root note categoryId is set to -1)
            if (categoryId == -1)
                BindTreeWithCatalog(catalogId, ref list);

            // to identify parent categories associated to catalog categoryId is set to 0)
            else if (categoryId == 0)
                BindTreeWithParentCategories(catalogId, defaultLocaleId, ref list);

            //bind tree with Publish Child category  and products Details
            else
                BindTreeWithChildCategoriesAndProducts(catalogId, categoryId, defaultLocaleId, ref list);

            ZnodeLogging.LogMessage("CategoryTreeModel list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, list?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return list;
        }

        //Method Gets Publish Catalog Details
        public virtual PublishCatalogModel GetPublishCatalogDetails(int publishCatalogId)
         => GetService<IPublishedCatalogDataService>().GetPublishCatalogById(publishCatalogId)?.ToModel<PublishCatalogModel>();

        //Method Gets Publish Category Details
        public virtual PublishCategoryModel GetPublishCategoryDetails(int publishCategoryId)
            => GetService<IPublishedCategoryDataService>().GetCategoryByCategoryId(publishCategoryId, Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale))?.ToModel<PublishCategoryModel>();

        //Method Gets Publish Product Details by Product Id and Locale Id
        public virtual PublishProductModel GetPublishProductDetails(int publishProductId, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodePublishProductEntity publishProductEntity = GetService<IPublishedProductDataService>().GetPublishProduct(publishProductId, portalId, Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), GetCatalogVersionId());
            ZnodeLogging.LogMessage("publishProductId to get ProductEntity: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, publishProductId);

            PublishProductModel productDetails = publishProductEntity?.ToModel<PublishProductModel>();

            if (productDetails?.Attributes?.Count > 0)
            {
                //Get Inventory and Price Details by SKU and PortalId 
                IZnodeViewRepository<ProductAdditionalInfoModel> productAdditionalInfoModel = new ZnodeViewRepository<ProductAdditionalInfoModel>();
                productAdditionalInfoModel.SetParameter("@sku", productDetails.SKU, ParameterDirection.Input, DbType.String);
                productAdditionalInfoModel.SetParameter("@portalId", portalId, ParameterDirection.Input, DbType.Int32);
                productAdditionalInfoModel.SetParameter("@currentUtcDate", GetDateTime().ToShortDateString(), ParameterDirection.Input, DbType.String);
                var productAdditionalInfo = productAdditionalInfoModel.ExecuteStoredProcedureList("Znode_GetPublishProductAdditionalInfo @sku,@portalId,@currentUtcDate")?.FirstOrDefault();

                if (IsNotNull(productAdditionalInfo))
                {
                    //Add inventory and Price Details to attributes Set
                    productDetails.Attributes.Add(new PublishAttributeModel { AttributeName = ZnodeConstant.LabelQuantityOnHand, AttributeValues = Convert.ToString(productAdditionalInfo.QuantityOnHand) });
                    productDetails.Attributes.Add(new PublishAttributeModel { AttributeName = ZnodeConstant.LabelReOrderLevel, AttributeValues = Convert.ToString(productAdditionalInfo.ReOrderLevel) });
                    productDetails.Attributes.Add(new PublishAttributeModel { AttributeName = ZnodeConstant.LabelSalesPrice, AttributeValues = Convert.ToString(productAdditionalInfo.SalesPrice) });
                    productDetails.Attributes.Add(new PublishAttributeModel { AttributeName = ZnodeConstant.LabelRetailPrice, AttributeValues = Convert.ToString(productAdditionalInfo.RetailPrice) });
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return productDetails;
        }
        #endregion

        #region Protected Methods
        // To get the list of published catalogs for the available pim catalogs.
        protected virtual List<ZnodePublishCatalogEntity> PublishCatalogList(PageListModel pageListModel, NameValueCollection expands)
        {
            IZnodeRepository<ZnodePimCatalog> _pimCatalogRepository = new ZnodeRepository<ZnodePimCatalog>();

            // If pim catalog is not available then respective published catalog will not be fetched from database.
            return (from _publishCatalog in _publishedCatalogDataService.GetPublishCatalogs(pageListModel)
                       join _pimCatalog in _pimCatalogRepository.Table on _publishCatalog.ZnodeCatalogId equals _pimCatalog.PimCatalogId
                       select _publishCatalog).ToList();
        }
        #endregion

        #region Private Methods

        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ZnodePortalCatalogEnum.ZnodePublishCatalog.ToString())) SetExpands(ZnodePortalCatalogEnum.ZnodePublishCatalog.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //bind catalog with tree
        private void BindTreeWithCatalog(int catalogId, ref List<CategoryTreeModel> list)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            
            //Get Publish catalog Details
            ZnodePublishCatalogEntity catalog = GetService<IPublishedCatalogDataService>().GetPublishCatalogById(catalogId);

            list.Add(new CategoryTreeModel { text = catalog?.CatalogName, id = "0", icon = ZnodeConstant.TreeCatalogIcon, children = true });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
        }

        //bind Parent Categories with tree
        private void BindTreeWithParentCategories(int catalogId, int defaultLocaleId, ref List<CategoryTreeModel> list)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Get Publish Parent category Details and bind details to model
            list = GetService<IPublishedCategoryDataService>().GetCategoryListByCatalogId(catalogId, defaultLocaleId)?.Where(x => x.ZnodeParentCategoryIds == null)?.Select(x => new CategoryTreeModel
            {
                text = $"{x.Name}({(IsNull(x.ProductIds) ? "0" : $"{x.ProductIds?.Count()}")})",
                id = Convert.ToString(x.ZnodeCategoryId),
                icon = ZnodeConstant.TreeCategoryIcon
            }).ToList();
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
        }

        //Bind ChildCategories And Products with tree
        private void BindTreeWithChildCategoriesAndProducts(int catalogId, int categoryId, int defaultLocaleId, ref List<CategoryTreeModel> list)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            list = GetService<IPublishedCategoryDataService>().GetPublishedCategoryList(new PageListModel(GetFiltersForCategory(catalogId, categoryId, defaultLocaleId), null, null))?.Select(x => new CategoryTreeModel
            {
                text = $"{x.Name}({(IsNull(x.ProductIds) ? "0" : $"{x.ProductIds?.Count()}")})",
                id = Convert.ToString(x.ZnodeCategoryId),
                icon = ZnodeConstant.TreeCategoryIcon
            }).ToList();


            //Get Publish Products Details and bind details to model
            List<CategoryTreeModel> productslist = GetService<IPublishedProductDataService>().GetProductsByCategoryId(categoryId, defaultLocaleId)?.Select(x => new CategoryTreeModel
            {
                text = x.Name,
                id = $"{x.ZnodeProductId}_{categoryId}_product",
                icon = ZnodeConstant.TreeProductIcon,
                children = false
            }).ToList();

            if (productslist?.Count > 0)
            {
                list = IsNotNull(list) ? list : new List<CategoryTreeModel>();
                list?.AddRange(productslist);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
        }

        //Get filters for Category
        private FilterCollection GetFiltersForCategory(int catalogId, int categoryId, int defaultLocaleId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add("ZnodeCatalogId", FilterOperators.Equals, catalogId.ToString());
            filters.Add("ZnodeParentCategoryIds", FilterOperators.Contains, categoryId.ToString());
            filters.Add("LocaleId", FilterOperators.Contains, defaultLocaleId.ToString());
            return filters;

        }

        //Delete the Portal Widget Configuration of Product & Category.
        private void DeletePortalWidgetConfiguration(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            IZnodeRepository<ZnodeCMSWidgetProduct> _cmsWidgetProductRepository = new ZnodeRepository<ZnodeCMSWidgetProduct>();
            IZnodeRepository<ZnodeCMSWidgetCategory> _cmsWidgetCategory = new ZnodeRepository<ZnodeCMSWidgetCategory>();

            ZnodeLogging.LogMessage("Delete product & category widget configuration of portal with Id: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, portalId);

            //Set Filter Values for Deletion of Portal Widget Configuration.
            filters.Add(new FilterTuple(ZnodeCMSWidgetProductEnum.CMSMappingId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString()));
            filters.Add(new FilterTuple(ZnodeCMSWidgetProductEnum.TypeOFMapping.ToString(), ProcedureFilterOperators.Contains, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString()));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            //Delete the Associated Product Widget Configuration for the portal.
            _cmsWidgetProductRepository.Delete(whereClauseModel.WhereClause, whereClauseModel.FilterValues);

            //Delete the Associated Category Widget Configuration for the portal.
            _cmsWidgetCategory.Delete(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
        }
        #endregion
    }
}
