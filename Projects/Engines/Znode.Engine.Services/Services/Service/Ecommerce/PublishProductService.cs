using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Engine.Promotions;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Utilities = Znode.Libraries.ECommerce.Utilities;
using Newtonsoft.Json;
using Znode.Libraries.ECommerce.ShoppingCart;

namespace Znode.Engine.Services
{
    public class PublishProductService : BaseService, IPublishProductService
    {
        #region Protected Variables
        protected readonly IZnodeRepository<ZnodeUserProfile> _userProfile;
        protected readonly IPublishProductHelper publishProductHelper;
        protected readonly IZnodeRepository<ZnodePortal> _portalRepository;
        protected readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        protected readonly IZnodeRepository<ZnodeAccount> _userAccount;
        protected readonly IZnodeRepository<ZnodeUser> _user;
        protected readonly IZnodeRepository<ZnodePublishConfigurableProductEntity> _publishConfigurableProductEntity;
        protected readonly IZnodeRepository<ZnodePublishBundleProductEntity> _publishBundleProductEntity;
        protected readonly IZnodeRepository<ZnodeStockNotice> _stockNoticeRepository;

        public static string SKU { get; } = "sku";
        public static string Width { get; } = "width";
        public static string Height { get; } = "height";

        #endregion

        #region Constructor
        public PublishProductService()
        {
            publishProductHelper = GetService<IPublishProductHelper>();
            _userProfile = new ZnodeRepository<ZnodeUserProfile>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _userAccount = new ZnodeRepository<ZnodeAccount>();
            _user = new ZnodeRepository<ZnodeUser>();
            _publishConfigurableProductEntity = new ZnodeRepository<ZnodePublishConfigurableProductEntity>(HelperMethods.Context);
            _publishBundleProductEntity = new ZnodeRepository<ZnodePublishBundleProductEntity>(HelperMethods.Context);
            _stockNoticeRepository = new ZnodeRepository<ZnodeStockNotice>();
        }
    #endregion

        #region Public Methods      

        public virtual PublishProductModel GetPublishProduct(int publishProductId, FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Input Parameters publishProductId:", string.Empty, TraceLevel.Info, publishProductId);

            bool isChildPersonalizableAttribute = false;
            List<PublishAttributeModel> parentPersonalizableAttributes = null;
            int portalId, localeId;
            int? catalogVersionId;
            PublishProductModel publishProduct = null;
            List<PublishProductModel> products;
            string parentProductImageName = null;

            //Get publish product 
            products = GetPublishProductFromPublishedData(publishProductId, filters, out portalId, out localeId, out catalogVersionId, publishProduct, out products);

            List<int> associatedCategoryIds = new List<int>();

            if (HelperUtility.IsNotNull(products) && products.Count > 0)
            {
                List<int> categoryIds = products.Select(x => x.ZnodeCategoryIds)?.ToList();

                FilterCollection filter = new FilterCollection();
                if(categoryIds?.Count > 0)
                    filter.Add("ZnodeCategoryId", FilterOperators.In, string.Join(",", categoryIds));
                filter.Add("IsActive", FilterOperators.Equals, ZnodeConstant.TrueValue);
                filter.Add("VersionId", FilterOperators.Equals, catalogVersionId.ToString());

                List<PublishedCategoryEntityModel> categoryEntities = GetService<IPublishedCategoryDataService>().GetPublishedCategoryList(new PageListModel(filter, null, null))?.ToModel<PublishedCategoryEntityModel>()?.ToList();

                associatedCategoryIds.AddRange(categoryEntities.Select(x => x.ZnodeCategoryId));

                //If no category associated to product then perform else part
                if (associatedCategoryIds?.Count > 0)
                    publishProduct = products.FirstOrDefault(x => associatedCategoryIds.Contains(x.ZnodeCategoryIds));
                else
                    publishProduct = products?.FirstOrDefault();

                parentProductImageName = publishProduct?.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues;
            }

            if (HelperUtility.IsNotNull(publishProduct))
            {
                List<PublishProductModel> associatedProducts = null;
                bool isConfigurable = IsConfigurableProductCheck(publishProduct);
                bool displayVariantsOnGrid = Convert.ToBoolean(publishProduct?.Attributes?.FirstOrDefault(x => string.Equals(x.AttributeCode, ZnodeConstant.DisplayVariantsOnGrid, StringComparison.InvariantCultureIgnoreCase))?.AttributeValues) && isConfigurable;

                if (isConfigurable)
                    associatedProducts = GetService<IPublishedProductDataService>().GetAssociatedConfigurableProducts(publishProductId, localeId, catalogVersionId).ToModel<PublishProductModel>()?.ToList();

                if (associatedProducts?.Count > 0 && !displayVariantsOnGrid)
                {
                    parentPersonalizableAttributes = publishProduct.Attributes?.Where(x => x.IsPersonalizable).ToList();
                    publishProduct = GetDefaultConfigurableProduct(expands, portalId, localeId, publishProduct, associatedProducts, null, catalogVersionId);
                }
                else
                {
                    if (IsBundleProductCheck(publishProduct))
                        BindAssociatedBundleProductList(publishProductId, expands, portalId, localeId, catalogVersionId, publishProduct);
                    publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProduct, localeId, WhereClauseForPortalId(portalId), GetLoginUserId(), catalogVersionId.GetValueOrDefault(), WebstoreVersionId, GetProfileId());
                }


                isChildPersonalizableAttribute = publishProduct.Attributes.Where(x => x.IsPersonalizable).Count() > 0;

                GetProductImagePath(portalId, publishProduct, true, parentProductImageName);

                //set stored based In Stock, Out Of Stock, Back Order Message.
                SetPortalBasedDetails(portalId, publishProduct);

                publishProduct.ZnodeProductCategoryIds = associatedCategoryIds;

                if(displayVariantsOnGrid && isConfigurable)
                    publishProduct.IsConfigurableProduct = isConfigurable;
            }
            return AddPersonalizeAttributeInChildProduct(publishProduct, parentPersonalizableAttributes, isChildPersonalizableAttribute);
        }

        protected virtual void BindAssociatedBundleProductList(int publishProductId, NameValueCollection expands, int portalId, int localeId, int? catalogVersionId, PublishProductModel publishProduct)
        {
            IZnodeShoppingCart znodeShoppingCarts = GetService<IZnodeShoppingCart>();
            publishProduct.PublishBundleProducts = new List<AssociatedPublishedBundleProductModel>();
            List<AssociatedPublishedBundleProductModel> publishBundleChildModel = znodeShoppingCarts.BindBundleProductChildByParentSku(publishProduct.SKU, publishProduct.ZnodeCatalogId, publishProduct.LocaleId);
            if (publishBundleChildModel.Count > 0)
                publishProduct.PublishBundleProducts.AddRange(publishBundleChildModel);
        }

        /// <summary>
        /// This method only returns the details of a parent published product.
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="filters"></param>
        /// <param name="expands"></param>
        /// <returns></returns>
        public virtual PublishProductModel GetpublishParentProduct(int publishProductId, FilterCollection filters, NameValueCollection expands)
        {
            PublishProductModel publishProduct = GetPublishedProductFromPublishedData(publishProductId, filters);
            return publishProduct;

        }

        /// <summary>
        /// This method only returns the brief details of a published product .
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="filters"></param>
        /// <param name="expands"></param>
        /// <returns></returns>
        public virtual PublishProductModel GetPublishProductBrief(int publishProductId, FilterCollection filters, NameValueCollection expands)
        {
            //Get parameter values from filters.
            int catalogId, portalId, localeId;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

            //get catalog current version id by catalog id.
            int? catalogVersionId = GetCatalogVersionId(catalogId);

            PublishProductModel publishProduct = GetPublishedProductFromPublishedData(publishProductId, filters);

            if (HelperUtility.IsNotNull(publishProduct))
            {
                List<PublishedConfigurableProductEntityModel> configEntity = publishProductHelper.GetConfigurableProductEntity(publishProductId, catalogVersionId);

                //Get associated configurable product list.
                List<PublishProductModel> associatedProducts = publishProductHelper.GetAssociatedProducts(publishProductId, localeId, catalogVersionId, configEntity);

                if (associatedProducts?.Count > 0)
                    publishProduct = GetDefaultConfigurableProduct(expands, portalId, localeId, publishProduct, associatedProducts, configEntity?.SelectMany(x => x.ConfigurableAttributeCodes).Distinct()?.ToList(), catalogVersionId.GetValueOrDefault());
                else
                    //Get expands associated to Product
                    publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProduct, localeId, WhereClauseForPortalId(portalId), GetLoginUserId(), catalogVersionId.GetValueOrDefault());

                GetProductImagePath(portalId, publishProduct, false);

                SetPortalBasedDetails(portalId, publishProduct);
            }
            return publishProduct;
        }

        /// <summary>
        /// This method only returns the extended details of a published product based on the supplied expands.
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="filters"></param>
        /// <param name="expands">Pass appropriate expands to get the corresponding detail in response.</param>
        /// <returns></returns>
        public virtual PublishProductModel GetExtendedPublishProductDetails(int publishProductId, FilterCollection filters, NameValueCollection expands)
        {
            //Get parameter values from filters.
            int catalogId, portalId, localeId;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

            //get catalog current version id by catalog id.
            int? catalogVersionId = GetCatalogVersionId(catalogId);

            PublishProductModel publishProduct = GetPublishedProductFromPublishedData(publishProductId, filters);

            if (HelperUtility.IsNotNull(publishProduct))
            {
                if (ContainsExpandables(expands))
                {
                    List<PublishedConfigurableProductEntityModel> configEntity = publishProductHelper.GetConfigurableProductEntity(publishProduct.PublishProductId, catalogVersionId);

                    //Get associated configurable product list.
                    List<PublishProductModel> associatedProducts = publishProductHelper.GetAssociatedProducts(publishProduct.PublishProductId, localeId, catalogVersionId, configEntity);

                    if (associatedProducts?.Count > 0)
                        publishProduct = GetDefaultConfigurableProduct(expands, portalId, localeId, publishProduct, associatedProducts, configEntity?.SelectMany(x => x.ConfigurableAttributeCodes).Distinct()?.ToList(), catalogVersionId.GetValueOrDefault());
                    else
                        //Get expands associated to Product
                        publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProduct, localeId, WhereClauseForPortalId(portalId), GetLoginUserId(), catalogVersionId.GetValueOrDefault());
                }

                if (expands.AllKeys.Contains(ZnodeConstant.ProductImage.ToLowerInvariant()))
                    GetProductImagePath(portalId, publishProduct, true);
            }

            return publishProduct;
        }

        //set stored based In Stock, Out Of Stock, Back Order Message.
        protected virtual void SetPortalBasedDetails(int portalId, PublishProductModel publishProduct)
        {
            ZnodePortal portalDetails = GetPortalDetailsById(portalId);
            if (HelperUtility.IsNotNull(portalDetails) && HelperUtility.IsNotNull(publishProduct))
            {
                publishProduct.InStockMessage = portalDetails.InStockMsg;
                publishProduct.OutOfStockMessage = portalDetails.OutOfStockMsg;
                publishProduct.BackOrderMessage = portalDetails.BackOrderMsg;
                publishProduct?.PublishBundleProducts?.ForEach(x => {
                    x.InStockMessage = portalDetails.InStockMsg;
                    x.OutOfStockMessage = portalDetails.OutOfStockMsg;
                    x.BackOrderMessage = portalDetails.BackOrderMsg;
                });
            }
        }

        //Get product by product sku.
        public virtual PublishProductModel GetPublishProductBySKU(ParameterProductModel model, NameValueCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            int catalogId, portalId, localeId;

            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

            string configurableProductSKU = model.ParentProductSKU;
            //get catalog current version id by catalog id.
            int? catalogVersionId = GetCatalogVersionId(catalogId, localeId);
            ZnodeLogging.LogMessage("Parameter:", string.Empty, TraceLevel.Verbose, new { configurableProductSKU = configurableProductSKU, catalogVersionId = catalogVersionId });
            //Get publish product 
            PublishProductModel publishProduct = GetService<IPublishedProductDataService>().GetPublishProductBySKU(model.SKU, catalogId, localeId, catalogVersionId)?.ToModel<PublishProductModel>();

            if (model.ParentProductId > 0 && !string.IsNullOrEmpty(configurableProductSKU) && HelperUtility.IsNotNull(publishProduct))
            {
                publishProduct.ConfigurableProductSKU = model.SKU;
                publishProduct.ConfigurableProductId = model.ParentProductId;
                publishProduct.SKU = configurableProductSKU;
            }

            if (HelperUtility.IsNotNull(publishProduct))
            {
                List<PublishedConfigurableProductEntityModel> configEntity = publishProductHelper.GetConfigurableProductEntity(publishProduct.PublishProductId, catalogVersionId);

                //Get associated configurable product list.
                List<PublishProductModel> associatedProducts = publishProductHelper.GetAssociatedProducts(publishProduct.PublishProductId, localeId, catalogVersionId, configEntity);
                ZnodeLogging.LogMessage("List counts:", string.Empty, TraceLevel.Verbose, new { configEntityCount = configEntity?.Count(), associatedProductsCount = associatedProducts?.Count() });

                //If associatedProducts count is greater, get default configurable product.
                if (associatedProducts?.Count > 0)
                    publishProduct = GetDefaultConfigurableProduct(expands, portalId, localeId, publishProduct, associatedProducts, configEntity.FirstOrDefault()?.ConfigurableAttributeCodes);
                else
                {
                    if(IsBundleProductCheck(publishProduct))
                    {
                        IZnodeShoppingCart znodeShoppingCarts = GetService<IZnodeShoppingCart>();
                        // Get all the child product details by bundle product sku and bind to PublishBundleProducts
                        publishProduct.PublishBundleProducts = znodeShoppingCarts.BindBundleProductChildByParentSku(publishProduct.SKU, catalogId, publishProduct.LocaleId);
                    }
                }
            }
            ZnodeLogging.LogMessage("Parameter for GetProductImagePath:", string.Empty, TraceLevel.Verbose, new object[] { portalId, publishProduct });
            GetProductImagePath(portalId, publishProduct);
            //set stored based In Stock, Out Of Stock, Back Order Message.
            SetPortalBasedDetails(portalId, publishProduct);

            //Get expands associated to Product
            publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProduct, localeId, "", GetLoginUserId(), null, null, GetProfileId());
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return publishProduct;
        }

        protected virtual bool IsBundleProductCheck(PublishProductModel publishProduct)
        {
            if(publishProduct.ProductType == ZnodeConstant.BundleProduct || !string.IsNullOrEmpty(publishProduct.BundleProductSKUs)
                || publishProduct.Attributes.Where(x=>x.AttributeCode == ZnodeConstant.ProductType).FirstOrDefault().SelectValues.Where(x=>x.Code == ZnodeConstant.BundleProduct).Any())
            {
                return true;
            }
            return false;
        }

        protected virtual bool IsConfigurableProductCheck(PublishProductModel publishProduct)
        {
            if (publishProduct.ProductType == ZnodeConstant.ConfigurableProduct || !string.IsNullOrEmpty(publishProduct.ConfigurableProductSKUs)
                || publishProduct.Attributes.Where(x => x.AttributeCode == ZnodeConstant.ProductType).FirstOrDefault().SelectValues.Where(x => x.Code == ZnodeConstant.ConfigurableProduct).Any())
            {
                return true;
            }
            return false;
        }

        public virtual PublishProductListModel GetPublishProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            int catalogId, portalId, localeId, userId;
            string productType;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.UserId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out userId);
            productType = filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.ProductType, StringComparison.InvariantCultureIgnoreCase))?.FilterValue;
            ZnodeLogging.LogMessage("productType:", string.Empty, TraceLevel.Verbose, new object[] { productType });
          
            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);
            filters.RemoveAll(x => x.FilterName == FilterKeys.UserId);

            int versionId = GetCatalogVersionId(catalogId, localeId).GetValueOrDefault();
            ZnodeLogging.LogMessage("Parameter:", string.Empty, TraceLevel.Verbose, new object[] { versionId = versionId, catalogId = catalogId });
            //get catalog current version id by catalog id.
            if (catalogId > 0)
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(versionId));
            else
            {
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.In, filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType) ? GetCatalogAllVersionIds(localeId) : GetCatalogAllVersionIds());
                //Get comma separated ids of the catalogs which are associated to the store(s).
                List<string> activeCatalogIds = null;
                IZnodeViewRepository<string> objStoredProc = new ZnodeViewRepository<string>();
                activeCatalogIds = objStoredProc.ExecuteStoredProcedureList("Znode_GetActiveCatalogIds")?.ToList();

                ZnodeLogging.LogMessage("activeCatalogIds:", string.Empty, TraceLevel.Verbose, new object[] { activeCatalogIds });
                if(activeCatalogIds?.Count> 0)
                    filters.Add(FilterKeys.CatalogId, FilterOperators.In, activeCatalogIds[0]);
            }
            if (filters.Exists(x => x.Item1 == FilterKeys.RevisionType))
                filters.RemoveAll(x => x.Item1 == FilterKeys.RevisionType);

            //Get filter value
            string filterValue = filters.FirstOrDefault(x => x.FilterName.ToLower() == FilterKeys.AttributeValuesForPromotion.ToString().ToLower() && x.FilterOperator == FilterOperators.In)?.FilterValue;
            ZnodeLogging.LogMessage("filterValue:", string.Empty, TraceLevel.Verbose, new object[] { filterValue });
            if (!string.IsNullOrEmpty(filterValue))
            {
                //Remove Payment Setting Filters with IN operator from filters list
                filters.RemoveAll(x => x.FilterName.ToLower() == FilterKeys.AttributeValuesForPromotion.ToString().ToLower() && x.FilterOperator == FilterOperators.In);

                //Add Payment Setting Filters
                filters.Add(FilterKeys.AttributeValuesForPromotion, FilterOperators.In, filterValue.Replace('_', ','));
            }

            if (filters.Any(w => w.FilterName.ToLower() == FilterKeys.fromOrder.ToString().ToLower()))
            {
                //Remove ZnodeCategoryId Filters from filters list
                filters.RemoveAll(x => x.FilterName.ToLower() == FilterKeys.fromOrder.ToString().ToLower());

                string activeCategoryList = GetActiveCategoryIds(catalogId);
                if(!string.IsNullOrEmpty(activeCategoryList))
                    filters.Add(FilterKeys.ZnodeCategoryId, FilterOperators.In, activeCategoryList);
            }

            //Replace filter keys with published filter keys
            ReplaceFilterKeys(ref filters);

            if (HelperUtility.IsNotNull(sorts))
                ReplaceSortKeys(ref sorts);

            //Check if products are taken for some specific category.
            SetProductIndexFilter(filters);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to publishProducts list :", string.Empty, TraceLevel.Verbose, pageListModel?.ToDebugString());
            //get publish products 
            List<ZnodePublishProductEntity> publishProducts;

            if (!string.IsNullOrEmpty(productType))
            {
                publishProducts = GetProductPageList(pageListModel);
            }
            else
            {
                publishProducts = GetService<IPublishedProductDataService>().GetPublishProductsPageList(pageListModel, out pageListModel.TotalRowCount);
            }
            ZnodeLogging.LogMessage("publishProducts list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, publishProducts?.Count());
            PublishProductListModel publishProductListModel = new PublishProductListModel(){ PublishProducts = publishProducts.ToModel<PublishProductModel>().ToList()};

            GetPublishBundleProducts(catalogId, localeId, publishProductListModel);

            GetExpands(portalId, localeId, expands, publishProductListModel, versionId);

            //Map pagination parameters
            publishProductListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return publishProductListModel;
        }

        //Used to filter the record using product type
        protected virtual List<ZnodePublishProductEntity> GetProductPageList(PageListModel pageListModel)
        {
            //get publish products 
            ZnodeLogging.LogMessage("pageListModel to publishProducts list :", string.Empty, TraceLevel.Verbose, pageListModel?.ToDebugString());
            //get publish products 
            ZnodeLogging.LogMessage("pageListModel to set parameters of SP Znode_GetProductTypeAttributesValue:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<ZnodePublishProductEntity> objStoredProc = new ZnodeViewRepository<ZnodePublishProductEntity>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            List<ZnodePublishProductEntity> publishProducts = objStoredProc.ExecuteStoredProcedureList("Znode_GetProductTypeAttributesValue @WhereClause,@Rows,@PageNo,@Order_By,@RowsCount OUT", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("page list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { publishProductList = publishProducts?.Count });

            return publishProducts;
        }

        //Associate bundle products with parent product
        protected virtual void GetPublishBundleProducts(int catalogId, int localeId, PublishProductListModel publishProductListModel)
        {
            string allbundleProductSkus = string.Empty;
            publishProductListModel.PublishProducts.ForEach(
                product =>
                {
                    if (product.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductType)?.SelectValues?.FirstOrDefault()?.Code == ZnodeConstant.BundleProduct)
                        allbundleProductSkus += (allbundleProductSkus == string.Empty ? "" : ",") + product.SKU;
                }
            );

            if (!string.IsNullOrEmpty(allbundleProductSkus))
            {
                IZnodeShoppingCart znodeShoppingCarts = GetService<IZnodeShoppingCart>();
                List<AssociatedPublishedBundleProductModel> bundleProductList = znodeShoppingCarts.BindBundleProductChildByParentSku(allbundleProductSkus, catalogId, localeId);

                publishProductListModel.PublishProducts?.ForEach(
                    product =>
                    {
                        if (product.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductType)?.SelectValues?.FirstOrDefault()?.Code == ZnodeConstant.BundleProduct)
                            product.PublishBundleProducts = bundleProductList?.Where(d => d.ParentBundleSKU == product.SKU).ToList();
                    }
                );
            }
        }

        //
        public virtual PublishProductListModel GetPublishProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, ParameterKeyModel parameters)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            PublishProductListModel publishProductListModel = new PublishProductListModel();

            switch (parameters.ParameterKey)
            {
                case ZnodeConstant.PublishedId:
                    int catalogId, portalId, localeId;
                    GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

                    List<int> productIds = parameters.Ids.Split(',').Select(int.Parse).ToList();
                    publishProductListModel.PublishProducts = new List<PublishProductModel>();
                    publishProductListModel.PublishProducts = GetService<IPublishedProductDataService>().GetPublishProductListByIds(productIds)?.ToModel<PublishProductModel>()?.ToList();

                    GetExpands(portalId, localeId, expands, publishProductListModel);
                    //get expands associated to Product
                    publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProductListModel, localeId, GetLoginUserId(), GetProfileId());
                    if (portalId > 0)
                    {
                        ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, portalId);
                        SetProductDetailsForList(portalId, publishProductListModel);
                    }
                    break;
                case ZnodeConstant.ZnodeCategoryIds:
                    filters.Add(new FilterTuple(FilterKeys.ZnodeCategoryIds, FilterOperators.In, parameters.Ids));
                    publishProductListModel = GetPublishProductList(expands, filters, sorts, page);
                    break;
                case ZnodeConstant.ProductSKU:
                    filters.Add(new FilterTuple(FilterKeys.SKU, FilterOperators.In, string.Join(",", parameters.Ids.Split(',').Select(x => $"\"{x}\""))));
                    publishProductListModel = GetPublishProductList(expands, filters, sorts, page);
                    break;
                default:
                    filters.Add(new FilterTuple(FilterKeys.ZnodeProductId, FilterOperators.In, parameters.Ids));
                    publishProductListModel = GetPublishProductList(expands, filters, sorts, page);
                    break;
            }
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return publishProductListModel;
        }


        /// <summary>
        /// To get active products for recent viewed products
        /// </summary>
        /// <param name="parentIds"></param>
        /// <param name="catalogId"></param>
        /// <param name="localeId"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public virtual List<RecentViewProductModel> GetActiveProducts(List<int> parentIds, int catalogId, int localeId, int versionId)
        {
            try
            {

                FilterCollection filters = new FilterCollection();
                filters.Add("ZnodeCatalogId", FilterOperators.Equals, catalogId.ToString());
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());
                filters.Add("VersionId", FilterOperators.Equals, GetCatalogVersionId(catalogId, localeId).ToString());
                filters.Add("IsActive", FilterOperators.Equals, ZnodeConstant.TrueValue);
                if(parentIds.Count > 0)
                    filters.Add("ZnodeProductId", FilterOperators.In, string.Join(",", parentIds));

                List<PublishedProductEntityModel> productList = GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(filters, null, null))?.ToModel<PublishedProductEntityModel>()?.ToList();

                if (HelperUtility.IsNotNull(productList) && productList.Count > 0)
                {
                    return (from p in productList
                            where p.ZnodeCatalogId != 0
                            select new RecentViewProductModel
                            {
                                IsActive = p.IsActive,
                                ZnodeProductId = p.ZnodeProductId,
                                ZnodeCatalogId = p.ZnodeCatalogId
                            }).ToList();
                }

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"An exception occurred while fetching recent viewed products for catalogId: {catalogId}", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error, ex);
            }
            return null;
        }


        //Get product price and inventory.
        public virtual ProductInventoryPriceListModel GetProductPriceAndInventory(ParameterInventoryPriceModel parameters)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            List<InventorySKUModel> inventory = publishProductHelper.GetInventoryBySKUs(parameters.Parameter?.Split(',')?.ToList(), parameters.PortalId);
            List<PriceSKUModel> priceSKU = publishProductHelper.GetPricingBySKUs(parameters.Parameter?.Split(',')?.ToList(), parameters.PortalId, GetLoginUserId(), GetProfileId(),0, parameters.IsBundleProduct, parameters.BundleProductParentSKU);
            ZnodeLogging.LogMessage("list count:", string.Empty, TraceLevel.Verbose, new { priceSKUCount = priceSKU?.Count(), inventoryCount = inventory?.Count() });
            ProductInventoryPriceListModel list = new ProductInventoryPriceListModel { ProductList = new List<ProductInventoryPriceModel>() };

            if (inventory?.Count > 0 || priceSKU?.Count > 0)
            {
                foreach (var sku in parameters.Parameter.Split(',')?.ToList())
                    list.ProductList.Add(GetProductInventoryModel(inventory, priceSKU, sku, parameters.IsBundleProduct));
            }
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return list;
        }

        //Get product price or inventory.
        public virtual ProductInventoryPriceListModel GetPriceWithInventory(ParameterInventoryPriceListModel parameters)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            List<InventorySKUModel> inventoryList = publishProductHelper.GetInventoryBySKUs(parameters.parameterInventoryPriceModels.Select(x => x.SKU)?.ToList(), parameters.parameterInventoryPriceModels.FirstOrDefault().PortalId);
            List<PriceSKUModel> priceSKU = publishProductHelper.GetPricingBySKUs(parameters.parameterInventoryPriceModels.Select(x => x.SKU)?.ToList(), parameters.parameterInventoryPriceModels.FirstOrDefault().PortalId, GetLoginUserId(), GetProfileId());

            ZnodeLogging.LogMessage("list count:", string.Empty, TraceLevel.Verbose, new { priceSKUCount = priceSKU?.Count(), inventoryCount = inventoryList?.Count() });
            ProductInventoryPriceListModel list = new ProductInventoryPriceListModel { ProductList = new List<ProductInventoryPriceModel>() };

            if (inventoryList?.Count > 0 || priceSKU?.Count > 0)
            {
                parameters.parameterInventoryPriceModels.ForEach(x => list.ProductList.Add(GetPriceAndInventoryModel(x, priceSKU, inventoryList)));
            }

            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return list;
        }

        //get associated products.
        public virtual WebStoreBundleProductListModel GetBundleProducts(FilterCollection filters)
        {
            IPublishedProductDataService publishedDataService = ZnodeDependencyResolver.GetService<IPublishedProductDataService>();

            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            WebStoreBundleProductListModel model = new WebStoreBundleProductListModel();
            //Replace filter keys.
            ReplaceFilterKeys(ref filters);

            string localeId = filters.Find(x => x.FilterName == ZnodeLocaleEnum.LocaleId.ToString())?.Item3;
            int catalogId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, WebStoreEnum.ZnodeCatalogId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            ZnodeLogging.LogMessage("localeId:", string.Empty, TraceLevel.Info, localeId);
            filters.RemoveAll(x => x.FilterName == ZnodeLocaleEnum.LocaleId.ToString());

            //get bundle product entity.
            List<ZnodePublishBundleProductEntity> associatedZnodeProductId = publishedDataService.GetPublishedBundleProduct(new PageListModel(filters, null, null));

            ZnodeLogging.LogMessage("associatedZnodeProductId list count:", string.Empty, TraceLevel.Verbose, associatedZnodeProductId?.Count());
            if (HelperUtility.IsNotNull(associatedZnodeProductId) && associatedZnodeProductId.Count > 0)
            {
                filters.Clear();

                //Associated product ids.
                int[] associatedProducts = associatedZnodeProductId.Select(x => x.AssociatedZnodeProductId).ToArray();
                filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.In, string.Join(",", associatedProducts));
                filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId);
                filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
                filters.Add("VersionId", FilterOperators.Equals, GetCatalogVersionId(catalogId).ToString());
                //get associated product list.
                List<ZnodePublishProductEntity> products = publishedDataService.GetPublishProducts(new PageListModel(filters, null, null));

                ZnodeLogging.LogMessage("products list count:", string.Empty, TraceLevel.Verbose, products?.Count());
                products = products.GroupBy(x => x.ZnodeProductId).Select(grp => grp.First()).ToList();
                products?.ForEach(x => x.AssociatedProductDisplayOrder = associatedZnodeProductId.FirstOrDefault(y => y.AssociatedZnodeProductId == x.ZnodeProductId).AssociatedProductDisplayOrder);
                model.BundleProducts = products?.ToModel<WebStoreBundleProductModel>().ToList();
                if (PortalId > 0)
                {
                    string ImageName = string.Empty;
                    IImageHelper image = GetService<IImageHelper>();
                    //Get image path for associated products.
                    model.BundleProducts.ForEach(
                        x =>
                        {
                            ImageName = x.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues;
                            x.ImageSmallPath = image.GetImageHttpPathSmall(ImageName);
                            x.ImageMediumPath = image.GetImageHttpPathMedium(ImageName);
                            x.ImageThumbNailPath = image.GetImageHttpPathThumbnail(ImageName);
                            x.ImageSmallThumbnailPath = image.GetImageHttpPathSmallThumbnail(ImageName);
                            x.AssociatedProductBundleQuantity = associatedZnodeProductId.FirstOrDefault(y => y.AssociatedZnodeProductId == products.FirstOrDefault(m => m.SKU == x.SKU)?.ZnodeProductId)?.AssociatedProductBundleQuantity;
                        });
                }
            }
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return model;
        }

        //get associated products.
        public virtual WebStoreGroupProductListModel GetGroupProducts(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            WebStoreGroupProductListModel model = new WebStoreGroupProductListModel();
            //Replace filter keys.
            ReplaceFilterKeys(ref filters);

            int portalId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.CurrentCultureIgnoreCase))?.Item3);

            string localeId = filters.Find(x => string.Equals(x.FilterName, ZnodeLocaleEnum.LocaleId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3;

            string publishCatalogId = filters.Find(x => string.Equals(x.FilterName, FilterKeys.PublishCatalogId, StringComparison.CurrentCultureIgnoreCase))?.Item3;
            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeLocaleEnum.LocaleId.ToString(), StringComparison.CurrentCultureIgnoreCase));
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.CurrentCultureIgnoreCase));
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.VersionId, StringComparison.CurrentCultureIgnoreCase));
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.PublishCatalogId, StringComparison.CurrentCultureIgnoreCase));
            int? versionIds = GetCatalogVersionId(Convert.ToInt32(publishCatalogId), Convert.ToInt32(localeId));
            filters.Add(FilterKeys.VersionId, FilterOperators.Equals, versionIds.HasValue ? versionIds.Value.ToString() : "0");

            //get group product entity.
            model.GroupProducts = GetGroupProductList(filters, portalId, localeId, true);
            ZnodeLogging.LogMessage("Parameter:", string.Empty, TraceLevel.Info, new { publishCatalogId = publishCatalogId, localeId = localeId, portalId = portalId, versionIds = versionIds, GroupProducts = model.GroupProducts });

            if (portalId > 0)
            {
                string ImageName = string.Empty;
                IImageHelper image = GetService<IImageHelper>();
                ZnodePortal portalDetails = GetPortalDetailsById(portalId);

                //Get image path for associated products.
                model.GroupProducts.ForEach(
                    x =>
                    {
                        ImageName = x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues;
                        x.ImageSmallPath = image.GetImageHttpPathSmall(ImageName);
                        x.ImageMediumPath = image.GetImageHttpPathMedium(ImageName);
                        x.ImageThumbNailPath = image.GetImageHttpPathThumbnail(ImageName);
                        x.ImageSmallThumbnailPath = image.GetImageHttpPathSmallThumbnail(ImageName);
                        x.InStockMessage = portalDetails?.InStockMsg;
                        x.OutOfStockMessage = portalDetails?.OutOfStockMsg;
                        x.BackOrderMessage = portalDetails?.BackOrderMsg;
                    });
            }
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return model;
        }

        //Get Product Attribute
        public virtual ConfigurableAttributeListModel GetProductAttribute(int productId, ParameterProductModel model)
        {
            if (productId > 0)
            {
                int? versionId = GetCatalogVersionId(model.PublishCatalogId);

                FilterCollection filters = new FilterCollection();
                filters.Add("ZnodeProductId", FilterOperators.Equals, model.ParentProductId.ToString());
                filters.Add("IsActive", FilterOperators.Equals, ZnodeConstant.TrueValue);
                filters.Add("VersionId", FilterOperators.Equals, versionId.ToString());

                PublishedProductEntityModel parentProduct = GetService<IPublishedProductDataService>().GetPublishProductByFilters(filters)?.ToModel<PublishedProductEntityModel>();

                //Selecting child SKU
                IEnumerable<string> childSKU = parentProduct.Attributes.Where(x => x.IsConfigurable).SelectMany(x => x.SelectValues.OrderBy(z => z.VariantDisplayOrder).Select(y => y.VariantSKU)).Distinct();

                //Creating new query
                List<PublishProductModel> products = GetAssociatedConfigurableProducts(model.LocaleId, versionId, childSKU);

                PublishedConfigurableProductEntityModel configEntity = GetConfigurableProductEntity(model.ParentProductId, versionId);

                return new ConfigurableAttributeListModel()
                {
                    //Get associated configurable product Attribute list.
                    Attributes = MapWebStoreConfigurableAttributeData(publishProductHelper.GetConfigurableAttributes(products, configEntity?.ConfigurableAttributeCodes), model.SelectedCode, model.SelectedValue, model.SelectedAttributes, products, configEntity?.ConfigurableAttributeCodes, model.PortalId),
                };
            }
            return new ConfigurableAttributeListModel();
        }

        //Get Configurable Product
        public virtual PublishProductModel GetConfigurableProduct(ParameterProductModel productAttributes, NameValueCollection expands)
        {
            PublishProductModel product = null;
            int? versionId = GetCatalogVersionId(productAttributes.PublishCatalogId);

            FilterCollection filters = new FilterCollection();
            filters.Add("ZnodeProductId", FilterOperators.Equals, productAttributes.ParentProductId.ToString());
            filters.Add("IsActive", FilterOperators.Equals, ZnodeConstant.TrueValue);
            filters.Add("VersionId", FilterOperators.Equals, versionId.ToString());

            PublishProductModel parentProduct = GetService<IPublishedProductDataService>().GetPublishProductByFilters(filters)?.ToModel<PublishProductModel>();

            //Selecting child SKU
            IEnumerable<string> childSKU = parentProduct.Attributes.Where(x => x.IsConfigurable).SelectMany(x => x.SelectValues.OrderBy(z => z.VariantDisplayOrder).Select(y => y.VariantSKU)).Distinct();

            //Creating new query
            List<PublishProductModel> productList = GetService<IPublishedProductDataService>().GetAssociatedConfigurableProducts(parentProduct.PublishProductId, productAttributes.LocaleId, versionId).ToModel<PublishProductModel>()?.ToList();


            foreach (var item in productAttributes.SelectedAttributes)
            {

                productList = (from productentity in productList
                               from attribute in productentity.Attributes
                               where attribute.AttributeCode == item.Key && attribute.SelectValues.FirstOrDefault()?.Value == item.Value
                               select productentity).ToList();
            }

            PublishProductModel productEntity = productList.FirstOrDefault();

            //If Combination does not exist.
            if (HelperUtility.IsNull(productEntity))
            {
                //Creating new query
                List<PublishProductModel> newproductList = GetAssociatedConfigurableProducts(productAttributes.LocaleId, versionId, childSKU);
                product = newproductList?.FirstOrDefault();
                if (HelperUtility.IsNotNull(product)) { product.IsDefaultConfigurableProduct = true; }
            }
            else
                product = productEntity;


            if (HelperUtility.IsNotNull(product))
            {
                bool isChildPersonalizableAttribute = product.Attributes.Where(x => x.IsPersonalizable).Count() > 0;

                var parentPersonalizableAttributes = parentProduct.Attributes?.Where(x => x.IsPersonalizable);
                product.AssociatedGroupProducts = MapParametersForProduct(productList);

                product.ConfigurableProductId = productAttributes.ParentProductId;
                product.IsConfigurableProduct = true;

                product.ProductType = ZnodeConstant.ConfigurableProduct;
                product.ConfigurableProductSKU = product.SKU;
                product.SKU = productAttributes.ParentProductSKU;

                publishProductHelper.GetDataFromExpands(productAttributes.PortalId, GetExpands(expands), product, productAttributes.LocaleId, string.Empty, GetLoginUserId(), null, null, GetProfileId());

                GetProductImagePath(productAttributes.PortalId, product);

                //set stored based In Stock, Out Of Stock, Back Order Message.
                SetPortalBasedDetails(productAttributes.PortalId, product);

                if (!isChildPersonalizableAttribute && parentPersonalizableAttributes?.Count() > 0)
                    product.Attributes.AddRange(parentPersonalizableAttributes);
            }
            return product;
        }

        protected virtual List<WebStoreGroupProductModel> MapParametersForProduct( List<PublishProductModel> productEntity)
        {

            List<WebStoreGroupProductModel> associatedGroupProducts = new List<WebStoreGroupProductModel>();
            if (HelperUtility.IsNotNull(productEntity))
            {
                productEntity?.ForEach(products =>
                {
                    WebStoreGroupProductModel groupProduct = new WebStoreGroupProductModel();

                    groupProduct.PublishProductId = products.ProductId;
                    groupProduct.Name = products.Name;
                    groupProduct.ProductName = products.ProductName;
                    groupProduct.LocaleId = products.LocaleId;
                    groupProduct.CurrencySuffix = products.CurrencySuffix;
                    groupProduct.ImageLargePath = products.ImageLargePath;
                    groupProduct.ImageMediumPath = products.ImageMediumPath;
                    groupProduct.ImageThumbNailPath = products.ImageThumbNailPath;
                    groupProduct.ImageSmallPath = products.ImageSmallPath;
                    groupProduct.ImageSmallThumbnailPath = products.ImageSmallThumbnailPath;
                    groupProduct.Attributes = products.Attributes;
                    groupProduct.InStockMessage = products.InStockMessage;
                    groupProduct.BackOrderMessage = products.BackOrderMessage;
                    groupProduct.OutOfStockMessage = products.OutOfStockMessage;
                    groupProduct.Addons = products.AddOns;
                    associatedGroupProducts.Add(groupProduct);

                });
            }

            return associatedGroupProducts;

        }

        //Get publish Product excluding assigned Ids.
        public virtual PublishProductListModel GetUnAssignedPublishProductList(ParameterModel assignedIds, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            filters.Add(Utilities.FilterKeys.ZnodeProductId, FilterOperators.NotIn, assignedIds.Ids);
            return GetPublishProductList(expands, filters, sorts, page);
        }

        //Bind Associated products.
        public virtual void BindAssociatedProductDetails(ProductInventoryPriceModel productModel, int productId, int publishedCatalogId, int portalId, int localeId, string productType)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter productId, portalId,publishedCatalogId,localeId:", string.Empty, TraceLevel.Info, new object[] { productId, portalId, publishedCatalogId, localeId });
            //If main product type is GroupedProduct then get its associated products.            
            if (Equals(productType, ZnodeConstant.GroupedProduct))
                GetAssociatedGroupProductPrice(productModel, productId, portalId, localeId);
            //If main product type is ConfigurableProduct then get first product form associated products and assign price.
            else if (Equals(productType, ZnodeConstant.ConfigurableProduct) && (HelperUtility.IsNull(productModel.SalesPrice) && HelperUtility.IsNull(productModel.RetailPrice)))
            {
                PublishProductModel product = GetAssociatedConfigurableProduct(productId, localeId, GetCatalogVersionId(publishedCatalogId), portalId);
                if (HelperUtility.IsNotNull(product))
                {
                    productModel.SalesPrice = product.SalesPrice;
                    productModel.RetailPrice = product.RetailPrice;
                    productModel.CurrencyCode = product.CurrencyCode;
                    productModel.CultureCode = product.CultureCode;
                }
            }
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
        }

        //Get price for products through ajax async call. 
        public virtual ProductInventoryPriceListModel GetProductPrice(ParameterInventoryPriceModel parameter)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            if (HelperUtility.IsNotNull(parameter))
            {
                switch (parameter.ProductType)
                {
                    //Get price for group products.
                    case ZnodeConstant.GroupedProduct:
                        PageListModel pageListModel = GetPageModel(parameter.Parameter, parameter.CatalogId);
                        ZnodeLogging.LogMessage("pageListModel to product list: ", string.Empty, TraceLevel.Verbose, pageListModel?.ToDebugString());
                        int total = 0;
                        List<PublishedProductEntityModel> productList = GetService<IPublishedProductDataService>().GetPublishProducts(pageListModel)?.ToModel<PublishedProductEntityModel>()?.ToList();

                        ZnodeLogging.LogMessage("productList list count:", string.Empty, TraceLevel.Verbose, productList?.Count());
                        if (productList?.Count > 0)
                            return GetAsyncGroupProductPrice(productList, parameter.LocaleId, parameter.PortalId);
                        break;
                    //Get price for configurable products.
                    case ZnodeConstant.ConfigurableProduct:
                        PageListModel pageModel = GetPageModel(parameter.Parameter, parameter.CatalogId);
                        ZnodeLogging.LogMessage("pageListModel to product list :", string.Empty, TraceLevel.Verbose, pageModel.ToDebugString());
                        int totalRow = 0;
                        List<ZnodePublishProductEntity> products = GetService<IPublishedProductDataService>().GetPublishProducts(pageModel);

                        ZnodeLogging.LogMessage("products list count:", string.Empty, TraceLevel.Verbose, products?.Count());
                        if (products?.Count > 0)
                            return GetAsyncConfigurableProductPrice(products, parameter.CatalogId, parameter.LocaleId, parameter.PortalId);
                        break;
                    //Get price for Simple and Bundle products.
                    default:
                        return new ProductInventoryPriceListModel { ProductList = MapProductPrice(publishProductHelper.GetPricingBySKUs(parameter.Parameter.Split(','), parameter.PortalId > 0 ? parameter.PortalId : PortalId, GetLoginUserId(), GetProfileId())) };
                }
            }
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return new ProductInventoryPriceListModel();
        }

        //Get message for group product.
        public virtual string GetGroupProductMessage(List<WebStoreGroupProductModel> list, ProductInventoryPriceModel product)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            List<decimal?> priceList = new List<decimal?>();
            //Add sales price and retail price in list.
            priceList.AddRange(list.Where(x => x.SalesPrice != null)?.Select(y => y.SalesPrice));
            priceList.AddRange(list.Where(x => x.RetailPrice != null)?.Select(y => y.RetailPrice));
            //Order list in ascending order.
            decimal? price = priceList.OrderBy(x => x.Value).FirstOrDefault();
            //Currency code for price format.
            string cultureCode = list.FirstOrDefault(x => x.SalesPrice == price || x.RetailPrice == price)?.CultureCode;
            ZnodeLogging.LogMessage("Parameter:", string.Empty, TraceLevel.Verbose, new { price = price, cultureCode = cultureCode });
            product.ProductPrice = price;
            if (price > 0)
                return string.Format(WebStore_Resources.GroupProductMessage, ServiceHelper.FormatPriceWithCurrency(price, cultureCode));
            else
                return string.Empty;
        }

        //Get publish products 
        public virtual PublishProductListModel GetPublishProductForSiteMap(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            int catalogId, portalId, localeId, userId;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);
            filters.RemoveAll(x => x.FilterName == FilterKeys.UserId);
            filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(GetCatalogVersionId(catalogId)));

            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.UserId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out userId);

            sorts.Add(ZnodeConstant.ZnodeCategoryIds, "asc");

            //added expand for SEO URL
            expands.Add(ZnodeConstant.SEO.ToString().ToLower(), ZnodeConstant.SEO.ToString().ToLower());

            //Replace filter keys with published filter keys
            ReplaceFilterKeys(ref filters);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to publishProducts list :", string.Empty, TraceLevel.Verbose, pageListModel?.ToDebugString());
            string ZnodeCategoryIds = GetActiveCategoryIdsForSiteMap(catalogId, pageListModel.PagingStart);
            ZnodeLogging.LogMessage("ZnodeCategoryIds:", string.Empty, TraceLevel.Verbose, ZnodeCategoryIds);
            if(!string.IsNullOrEmpty(ZnodeCategoryIds))
              filters.Add(ZnodeConstant.ZnodeCategoryIds, FilterOperators.In, ZnodeCategoryIds);

            //get publish products 
            List<PublishProductModel> publishProducts = GetService<IPublishedProductDataService>().GetPublishProducts(pageListModel)?.ToModel<PublishProductModel>()?.ToList();

            ZnodeLogging.LogMessage("publishProducts list count:", string.Empty, TraceLevel.Verbose, publishProducts?.Count());
            PublishProductListModel publishProductListModel = new PublishProductListModel() { PublishProducts = new List<PublishProductModel>() };

            if (publishProducts?.Count > 0)
            {
                foreach (PublishProductModel publishProductModel in publishProducts)
                    //Gets the produsts which having attributecode as hidden false.
                    if (publishProductModel?.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.HiddenAttribute)?.AttributeValues != ZnodeConstant.TrueValue)
                        publishProductListModel.PublishProducts.Add(publishProductModel);

                if (publishProductListModel?.PublishProducts?.Count > 0)
                    //get expands associated to Product
                    publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProductListModel, localeId, GetLoginUserId(), GetProfileId());
            }
            publishProductListModel.PublishProducts = publishProductListModel.PublishProducts.Where(x => (x.SEOTitle != null) || (x.SEOUrl != null) || (x.SEOKeywords != null) || (x.SEODescription != null)).ToList();
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return publishProductListModel;
        }

        //Get Publish Product Count.
        public virtual int GetPublishProductCount(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            int catalogId, portalId, localeId;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

            //get publish product count
            int rowCount = GetService<IPublishedProductDataService>().GetPublishProductCount(localeId, GetCatalogVersionId(catalogId) ?? 0, catalogId);

            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return rowCount;
        }

        public virtual void GetExpands(int portalId, int localeId, NameValueCollection expands, PublishProductListModel publishProductListModel, int catalogVersionId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter:", string.Empty, TraceLevel.Info, new { portalId = portalId, localeId = localeId });
            //Get required input parameters to get the data of products.
            DataTable productDetails = GetProductFiltersForSP(publishProductListModel.PublishProducts);
            //get expands associated to Product
            publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProductListModel, localeId, GetLoginUserId(), catalogVersionId, GetProfileId());
            GetRequiredProductDetails(publishProductListModel, productDetails, localeId, GetLoginUserId(), portalId);
        }


        public virtual string ValueFromSelectValue(List<PublishAttributeModel> attributes, string attributeCode)
        => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.SelectValues?.FirstOrDefault()?.Code;

        public DataTable GetProductFiltersForSP(List<PublishProductModel> products)
        {
            ZnodeLogging.LogMessage("Execution Started:", string.Empty, TraceLevel.Info);
            DataTable table = new DataTable("ProductTable");
            DataColumn productId = new DataColumn("Id");
            productId.DataType = typeof(int);
            productId.AllowDBNull = false;
            table.Columns.Add(productId);
            table.Columns.Add("ProductType", typeof(string));
            table.Columns.Add("OutOfStockOptions", typeof(string));
            table.Columns.Add("SKU", typeof(string));

            foreach (PublishProductModel item in products)
                table.Rows.Add(item.PublishProductId, ValueFromSelectValue(item.Attributes, ZnodeConstant.ProductType), ValueFromSelectValue(item.Attributes, ZnodeConstant.OutOfStockOptions), item.SKU);

            return table;
        }

        //Get published product list.
        public virtual PublishProductListModel GetPublishedProductsListData(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            int catalogId, portalId, localeId;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

            int versionId = GetCatalogVersionId(catalogId).GetValueOrDefault();
            ZnodeLogging.LogMessage("Parameter:", string.Empty, TraceLevel.Info, new { versionId = versionId });
            filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(versionId));

            //Replace filter keys with published filter keys
            ReplaceFilterKeys(ref filters);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to publishProducts list :", string.Empty, TraceLevel.Verbose, pageListModel?.ToDebugString());

            //get publish products  
            List<ZnodePublishProductEntity> publishProducts = GetService<IPublishedProductDataService>().GetPublishProductsPageList(pageListModel, out pageListModel.TotalRowCount);

            ZnodeLogging.LogMessage("publishProducts list count:", string.Empty, TraceLevel.Verbose, publishProducts?.Count());
            PublishProductListModel publishProductListModel = new PublishProductListModel() { PublishProducts = publishProducts.ToModel<PublishProductModel>().ToList() };
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return publishProductListModel;
        }

        //Get details of category products.
        protected virtual void GetRequiredProductDetails(PublishProductListModel publishProductListModel, DataTable tableDetails, int localeId, int userId = 0, int portalId = 0)
        {

            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter localeId:", string.Empty, TraceLevel.Info, new object[] { localeId });
            IZnodeViewRepository<PublishCategoryProductDetailModel> objStoredProc = new ZnodeViewRepository<PublishCategoryProductDetailModel>();
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@currentUtcDate", HelperUtility.GetDateTime().Date, ParameterDirection.Input, DbType.String);
            IList<PublishCategoryProductDetailModel> productDetails;

            if (DefaultGlobalConfigSettingHelper.IsColumnEncryptionSettingEnabled)
            {
                objStoredProc.SetParameter("@ProductDetailsFromWebStore", tableDetails?.ToJson(), ParameterDirection.Input, DbType.String);
                //Gets the entity list according to where clause, order by clause and pagination
                productDetails = objStoredProc.ExecuteStoredProcedureList("Znode_GetProductInfoForWebStoreWithJSON @PortalId,@LocaleId,@UserId,@ProductDetailsFromWebStore,@currentUtcDate");
            }
            else
            {
                objStoredProc.SetTableValueParameter("@ProductDetailsFromWebStore", tableDetails, ParameterDirection.Input, SqlDbType.Structured, "dbo.ProductDetailsFromWebStore");
                //Gets the entity list according to where clause, order by clause and pagination
                productDetails = objStoredProc.ExecuteStoredProcedureList("Znode_GetProductInfoForWebStore @PortalId,@LocaleId,@UserId,@currentUtcDate,@ProductDetailsFromWebStore");
            }
            //Bind product details.
            BindProductDetails(publishProductListModel, portalId, productDetails);
        }

        //Bind product details.
        protected virtual void BindProductDetails(PublishProductListModel publishProductListModel, int portalId, IList<PublishCategoryProductDetailModel> productDetails)
        {
            ZnodeLogging.LogMessage("Input Parameter portalId:", string.Empty, TraceLevel.Info, new { portalId = portalId });
            IImageHelper imageHelper = GetService<IImageHelper>();

            ZnodePortal portalDetails = GetPortalDetailsById(portalId);

            publishProductListModel?.PublishProducts?.ForEach(product =>
            {
                PublishCategoryProductDetailModel productSKU = productDetails?
                            .FirstOrDefault(productdata => productdata.SKU == product.SKU);

                if (HelperUtility.IsNotNull(productSKU))
                {
                    product.CurrencyCode = productSKU.CurrencyCode;
                    product.CultureCode = productSKU.CultureCode;
                    product.CurrencySuffix = productSKU.CurrencySuffix;
                    product.Quantity = productSKU.Quantity;
                    product.ReOrderLevel = productSKU.ReOrderLevel;
                    product.Rating = productSKU.Rating;
                    product.TotalReviews = productSKU.TotalReviews;
                    string ImageName = product.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues;
                    product.ImageSmallPath = imageHelper.GetImageHttpPathSmall(ImageName);
                    product.ImageMediumPath = imageHelper.GetImageHttpPathMedium(ImageName);
                    product.ImageThumbNailPath = imageHelper.GetImageHttpPathSmall(ImageName);
                    product.ImageSmallThumbnailPath = imageHelper.GetImageHttpPathSmall(ImageName);
                    product.InStockMessage = portalDetails?.InStockMsg;
                    product.OutOfStockMessage = portalDetails?.OutOfStockMsg;
                    product.BackOrderMessage = portalDetails?.BackOrderMsg;
                    product.ProductType = product.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductType)?.SelectValues.FirstOrDefault()?.Value;
                    product.IsActive = Convert.ToBoolean(product.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.IsActive)?.AttributeValues);
                }
            });
        }

        /// <summary>
        /// This method only returns the details of a parent published product .
        /// </summary>
        /// <param name="parentProductId"></param>
        /// <param name="filters"></param>
        /// <param name="expands"></param>
        /// <returns></returns>
        public virtual PublishProductModel GetParentProduct(int parentProductId, FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameters publishProductId:", string.Empty, TraceLevel.Info, parentProductId);
            PublishProductModel publishProduct = null;
            int portalId, localeId;
            int? catalogVersionId;

            //Get publish product 
            List<PublishProductModel> products = GetPublishProductFromPublishedData(parentProductId, filters, out portalId, out localeId, out catalogVersionId, publishProduct, out products);

            ZnodeLogging.LogMessage("Get publish product :", string.Empty, TraceLevel.Verbose, products?.Count());

            if (HelperUtility.IsNotNull(products) && products.Count > 0)
                publishProduct = products?.LastOrDefault();

            ZnodeLogging.LogMessage("GetProductImagePath execution done.", string.Empty, TraceLevel.Info);
            return publishProduct;
        }

        // Submit stock notice.
        public virtual bool SubmitStockRequest(StockNotificationModel stockNotificationModel)
        {
            if (IsNull(stockNotificationModel))
                return false;
            ZnodeStockNotice znodeStockNotice = _stockNoticeRepository.Insert(stockNotificationModel.ToEntity<ZnodeStockNotice>());
            return znodeStockNotice.StockNoticeId > 0;
        }

        // Send stock notification.
        public virtual bool SendStockNotification()
        {
            IZnodeViewRepository<StockNotificationResponseModel> objStoredProc = new ZnodeViewRepository<StockNotificationResponseModel>();
            List<StockNotificationResponseModel> stockNoticeList = objStoredProc.ExecuteStoredProcedureList("Znode_GetRequestedInventory")?.ToList();
            if (IsNotNull(stockNoticeList))
            {
                stockNoticeList = stockNoticeList?.GroupBy(x => x.StockNoticeId)?.Select(y => y.First())?.ToList();
                stockNoticeList?.ForEach(x => x.Attributes = JsonConvert.DeserializeObject<List<PublishAttributeModel>>(x.ProductAttributes));
                if (SendStockNotificationEmail(stockNoticeList))
                    DeleteAwaitingRequest();
                return true;
            }
            else
                return false;
        }
        #endregion

        #region protected Methods

        // Send emails for the products which are in stock.
        protected virtual bool SendStockNotificationEmail(List<StockNotificationResponseModel> stockNoticeList)
        {
            int portalId = (IsNotNull(stockNoticeList?.FirstOrDefault().PortalId)) ? (stockNoticeList.FirstOrDefault().PortalId) : PortalId;
            PortalModel portalModel = GetCustomPortalDetails(portalId);
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.SendWaitingListNoticeInInventory,portalId,GetDefaultLocaleId());
            string webstoreUrl = $"{HttpContext.Current.Request.Url.Scheme}://{portalModel?.DomainUrl}";

            if (IsNotNull(emailTemplateMapperModel))
            {
                foreach (StockNotificationResponseModel stockNotificationModel in stockNoticeList)
                {
                    bool isDisablePurchasing = Convert.ToBoolean(stockNotificationModel.Attributes?.FirstOrDefault(y => y.AttributeCode.Equals(ZnodeConstant.OutOfStockOptions, StringComparison.InvariantCultureIgnoreCase))?.SelectValues?.Where(x => x.Code.Equals(ZnodeConstant.DisablePurchasing, StringComparison.InvariantCultureIgnoreCase))?.Any());
                    if (isDisablePurchasing)
                    {
                        string productLink = webstoreUrl + (string.IsNullOrEmpty(stockNotificationModel?.SEOUrl) ?  "/product/" + stockNotificationModel.PublishProductId :  "/" + stockNotificationModel.SEOUrl);
                        string messageText = emailTemplateMapperModel.Descriptions;
                        string subject = ReplaceTokenWithMessageText("#ProductName#", stockNotificationModel.ProductName, emailTemplateMapperModel?.Subject);
                        messageText = ReplaceTokenWithMessageText("#Emailaddress#", stockNotificationModel?.EmailId, messageText);
                        messageText = ReplaceTokenWithMessageText("#Url#", webstoreUrl, messageText);
                        messageText = ReplaceTokenWithMessageText("#Image#", stockNotificationModel.ImageSmallPath, messageText);
                        messageText = ReplaceTokenWithMessageText("#ProductLink#", productLink, messageText);
                        messageText = ReplaceTokenWithMessageText("#ProductName#", stockNotificationModel.ProductName, messageText);
                        messageText = ReplaceTokenWithMessageText("#SKU#", stockNotificationModel.SKU, messageText);
                        messageText = ReplaceTokenWithMessageText("#StoreName#", portalModel?.StoreName, messageText);
                        ZnodeEmail.SendEmail(PortalId, stockNotificationModel.EmailId, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, PortalId, string.Empty), subject, messageText, true);
                    }
                    stockNotificationModel.IsEmailSent = true;
                }
                _stockNoticeRepository.BatchUpdate(stockNoticeList?.ToEntity<ZnodeStockNotice>().ToList());
                return true;
            }
            return false;
        }

        // Delete pending requests.
        protected virtual void DeleteAwaitingRequest()
        {
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteStockNotifications");
            if ((!deleteResult?.FirstOrDefault()?.Status.Value).GetValueOrDefault())
               ZnodeLogging.LogMessage(Admin_Resources.ErrorDeleteSomeRecord, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
        }

        //Replace Filter Keys
        protected virtual void ReplaceFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (string.Equals(tuple.Item1, FilterKeys.LocaleId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.LocaleId, FilterKeys.PublishedLocaleId); }
                if (string.Equals(tuple.Item1, FilterKeys.ZnodeCatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.ZnodeCatalogId.ToLower(), FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, FilterKeys.Name, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.Name.ToLower(), FilterKeys.Name); }
                if (string.Equals(tuple.Item1, FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.ItemName.ToLower(), FilterKeys.Name); }
                if (string.Equals(tuple.Item1, FilterKeys.Sku, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.Sku, FilterKeys.SKU); }
                if (string.Equals(tuple.Item1, FilterKeys.CatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.CatalogId.ToLower(), FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, FilterKeys.ZnodeCategoryId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.ZnodeCategoryId.ToLower(), FilterKeys.ZnodeCategoryIds); }
                if (string.Equals(tuple.Item1, FilterKeys.ZnodeCategoryIds, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.ZnodeCategoryIds.ToLower(), FilterKeys.ZnodeCategoryIds); }
                if (string.Equals(tuple.Item1, FilterKeys.ZnodeProductId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.ZnodeProductId.ToLower(), FilterKeys.ZnodeProductId); }
                if (string.Equals(tuple.Item1, WebStoreEnum.ProfileIds.ToString(), StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, WebStoreEnum.ProfileIds.ToString().ToLower(), WebStoreEnum.ProfileIds.ToString()); }
                if (string.Equals(tuple.Item1, WebStoreEnum.IsActive.ToString(), StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, WebStoreEnum.IsActive.ToString().ToLower(), WebStoreEnum.IsActive.ToString()); }
                if (string.Equals(tuple.Item1, WebStoreEnum.VersionId.ToString(), StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, WebStoreEnum.VersionId.ToString().ToLower(), WebStoreEnum.VersionId.ToString()); }
                if (string.Equals(tuple.Item1, FilterKeys.AttributeCodeForPromotion, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.AttributeCodeForPromotion.ToLower(), FilterKeys.AttributeCodeForPromotion); }
                if (string.Equals(tuple.Item1, FilterKeys.AttributeValuesForPromotion, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.AttributeValuesForPromotion.ToLower(), FilterKeys.AttributeValuesForPromotion); }
                if (string.Equals(tuple.Item1, FilterKeys.ProductIndex, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.ProductIndex.ToLower(), FilterKeys.ProductIndex); }
                if (string.Equals(tuple.Item1, FilterKeys.PublishProductAttributeValueForSelect, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.PublishProductAttributeValueForSelect.ToLower(), FilterKeys.PublishProductAttributeValueForSelect); }
                if (string.Equals(tuple.Item1, FilterKeys.PublishedCatalogName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.PublishedCatalogName.ToLower(), FilterKeys.PublishedCatalogName); }
                if (string.Equals(tuple.Item1, FilterKeys.AttributeValueForProductType, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.AttributeValueForProductType.ToLower(), FilterKeys.AttributeValueForProductType); }

            }
            ReplaceFilterKeysForOr(ref filters);
        }

        //Replace Filter Keys
        protected virtual void ReplaceFilterKeysForOr(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (tuple.Item1.Contains("|"))
                {
                    List<string> newValues = new List<string>();
                    foreach (var item in tuple.Item1.Split('|'))
                    {
                        if (string.Equals(item, FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(FilterKeys.Name); }
                        else if (string.Equals(item, Utilities.FilterKeys.PublishedCatalogName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.PublishedCatalogName); }
                        else if (string.Equals(item, FilterKeys.Sku, StringComparison.OrdinalIgnoreCase)) { newValues.Add(FilterKeys.SKU); }
                        else if (string.Equals(item, FilterKeys.Name, StringComparison.OrdinalIgnoreCase)) { newValues.Add(FilterKeys.Name); }
                        else if (string.Equals(item, FilterKeys.ProductType, StringComparison.OrdinalIgnoreCase)) { newValues.Add(FilterKeys.Attributes); }
                        else newValues.Add(item);
                    }
                    ReplaceFilterKeyName(ref filters, tuple.Item1, string.Join("|", newValues));
                }
            }
        }

        //Replace sort Keys
        protected virtual void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            foreach (string key in sorts.Keys)
            {
                if (string.Equals(key, FilterKeys.PublishProductId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, FilterKeys.PublishProductId.ToLower(), FilterKeys.ZnodeProductId); }
                if (string.Equals(key, FilterKeys.Name.ToLower(), StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, FilterKeys.Name.ToLower(), FilterKeys.Name); }
                if (string.Equals(key, FilterKeys.Sku, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, FilterKeys.Sku, FilterKeys.SKU); }
                if (string.Equals(key, FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, FilterKeys.ItemName, FilterKeys.Name); }
                if (string.Equals(key, FilterKeys.ItemId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, FilterKeys.ItemId, FilterKeys.ZnodeProductId); }
                if (string.Equals(key, FilterKeys.PublishedCatalogName.ToLower(), StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, FilterKeys.PublishedCatalogName.ToLower(), FilterKeys.PublishedCatalogName); }
            }
        }

        //Get expands and add them to navigation properties
        public virtual List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (HelperUtility.IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    //check if expand key is present or not and add it to navigation properties.
                    switch (key)
                    {
                        case ZnodeConstant.Promotions:
                            SetExpands(ZnodeConstant.Promotions, navigationProperties);
                            break;
                        case ZnodeConstant.Inventory:
                            SetExpands(ZnodeConstant.Inventory, navigationProperties);
                            break;
                        case ZnodeConstant.ProductTemplate:
                            SetExpands(ZnodeConstant.ProductTemplate, navigationProperties);
                            break;
                        case ZnodeConstant.ProductReviews:
                            SetExpands(ZnodeConstant.ProductReviews, navigationProperties);
                            break;
                        case ZnodeConstant.Pricing:
                            SetExpands(ZnodeConstant.Pricing, navigationProperties);
                            break;
                        case ZnodeConstant.SEO:
                            SetExpands(ZnodeConstant.SEO, navigationProperties);
                            break;
                        case ZnodeConstant.AdminSEO:
                            SetExpands(ZnodeConstant.AdminSEO, navigationProperties);
                            break;
                        case ZnodeConstant.AddOns:
                            SetExpands(ZnodeConstant.AddOns, navigationProperties);
                            break;
                        case ZnodeConstant.ConfigurableAttribute:
                            SetExpands(ZnodeConstant.ConfigurableAttribute, navigationProperties);
                            break;
                        case ZnodeConstant.AssociatedProducts:
                            SetExpands(ZnodeConstant.AssociatedProducts, navigationProperties);
                            break;
                        case ZnodeConstant.WishlistAddOns:
                            SetExpands(ZnodeConstant.WishlistAddOns, navigationProperties);
                            break;
                        default:
                            if (Equals(key, ZnodeConstant.Brand.ToLower())) SetExpands(ZnodeConstant.Brand, navigationProperties);
                            break;
                    }
                }
            }
            return navigationProperties;
        }

        //get product inventory model when price list is deactivated.
        protected virtual ProductInventoryPriceModel GetProductInventoryModelWithoutPrice(List<InventorySKUModel> inventoryList, string sku, bool isBundleProduct = false)
        {
            return (from inventory in inventoryList
                    select new ProductInventoryPriceModel
                    {
                        ReOrderLevel = inventory?.ReOrderLevel,
                        Quantity = inventory?.Quantity,
                        SKU = sku
                    })?.FirstOrDefault();
        }

        //get product inventory model.
        protected virtual ProductInventoryPriceModel GetProductInventoryModel(List<InventorySKUModel> inventoryList, List<PriceSKUModel> priceSKU, string sku, bool isBundleProduct = false)
        {
            if (priceSKU?.Count <= 0)
                return GetProductInventoryModelWithoutPrice(inventoryList, sku, isBundleProduct);
            else
            {
                if (!isBundleProduct)
                {
                    return (from inventory in inventoryList
                            join price in priceSKU on inventory.SKU equals price.SKU
                            select new ProductInventoryPriceModel
                            {
                                SalesPrice = price?.SalesPrice,
                                RetailPrice = price?.RetailPrice,
                                ReOrderLevel = inventory?.ReOrderLevel,
                                Quantity = inventory?.Quantity,
                                CurrencyCode = price.CurrencyCode,
                                CultureCode = price.CultureCode,
                                SKU = sku
                            })?.FirstOrDefault();
                }
                else
                {
                    return (from inventory in inventoryList
                            join price in priceSKU on inventory.SKU equals price.SKU
                            where inventory.SKU == sku && price.SKU == sku
                            select new ProductInventoryPriceModel
                            {
                                SalesPrice = price?.SalesPrice,
                                RetailPrice = price?.RetailPrice,
                                ReOrderLevel = inventory?.ReOrderLevel,
                                Quantity = inventory?.Quantity,
                                CurrencyCode = price.CurrencyCode,
                                CultureCode = price.CultureCode,
                                SKU = sku
                            })?.FirstOrDefault();
                }
            }
        }

        //Map Attributes values.        
        protected virtual List<PublishAttributeModel> MapWebStoreConfigurableAttributeData(List<List<PublishAttributeModel>> attributeList, string selectedCode, string selectedValue, Dictionary<string, string> SelectedAttributes, List<PublishProductModel> products, List<string> ConfigurableAttributeCodes, int portalId, bool isDefaultConfigurableProductExist = false)
        {
            List<PublishAttributeModel> configurableAttributeList = new List<PublishAttributeModel>();
            List<ConfigurableAttributeModel> attributesList = new List<ConfigurableAttributeModel>();
            IImageHelper image = GetService<IImageHelper>();

            if (SelectedAttributes.Count <= 0 && !string.IsNullOrEmpty(attributeList?.FirstOrDefault()?.FirstOrDefault()?.AttributeCode) && !string.IsNullOrEmpty(attributeList?.FirstOrDefault()?.FirstOrDefault()?.AttributeValues))
                SelectedAttributes.Add(attributeList.FirstOrDefault().FirstOrDefault().AttributeCode, attributeList.FirstOrDefault().FirstOrDefault().AttributeValues);

            IEnumerable<PublishAttributeModel> attributes = ExitingAttributeList(selectedCode, selectedValue, products, SelectedAttributes);

            var matchAttributeList = attributes.GroupBy(w => w.AttributeCode).Select(g => new
            {
                AttributeCode = g.Key,
                AttributeValues = g.Select(c => c.AttributeValues)
            });

            if (HelperUtility.IsNotNull(attributeList))
            {
                foreach (List<PublishAttributeModel> attributeEntityList in attributeList)
                {
                    attributesList.Clear();
                    PublishAttributeModel attributesModel = new PublishAttributeModel();
                    foreach (PublishAttributeModel attributeValue in attributeEntityList)
                    {
                        //Check if attribute already exist in list.
                        if (!AlreadyExist(attributesList, attributeValue.AttributeValues).GetValueOrDefault())
                        {
                            IEnumerable<string> matchAttribute = matchAttributeList?.FirstOrDefault(x => x.AttributeCode == attributeValue.AttributeCode)?.AttributeValues;

                            ConfigurableAttributeModel attribute = new ConfigurableAttributeModel();

                            if (HelperUtility.IsNotNull(matchAttribute) && !matchAttribute.Contains(attributeValue.AttributeValues) && ConfigurableAttributeCodes?.Count != 1)
                                attribute.IsDisabled = true;

                            attribute.AttributeValue = attributeValue.AttributeValues;

                            //Assign the display order value of configurable type attributes.
                            attribute.SelectValues = attributeValue.SelectValues.ToList();

                            if (attribute.SelectValues?.Count > 0)
                            {
                                AttributesSelectValuesModel selectValues = attribute.SelectValues.FirstOrDefault();

                                if (string.Equals(attributeValue.IsSwatch, ZnodeConstant.TrueValue, StringComparison.InvariantCultureIgnoreCase))
                                    attribute.ImagePath = image.GetImageHttpPathSmallThumbnail(selectValues.Path);

                                attribute.SwatchText = selectValues.SwatchText;
                                attribute.DisplayOrder = selectValues.DisplayOrder;
                            }
                            attributesList.Add(attribute);
                        }
                    }

                    PublishAttributeModel attributeEntity = new PublishAttributeModel();
                    //If default configurable product select product on basis of attribute code and value
                    if (isDefaultConfigurableProductExist)
                    {
                        string attributeCode = attributeEntityList?.FirstOrDefault()?.AttributeCode;
                        string attributeValue = SelectedAttributes?.FirstOrDefault(x => x.Key == attributeCode).Value;
                        attributeEntity = attributeEntityList.FirstOrDefault(x => x.AttributeValues == attributeValue);
                    }
                    else
                        attributeEntity = attributeEntityList.FirstOrDefault();

                    attributesModel.AttributeName = attributeEntity.AttributeName;
                    attributesModel.AttributeCode = attributeEntity.AttributeCode;
                    attributesModel.AttributeValues = attributeEntity.AttributeValues;
                    attributesModel.IsConfigurable = attributeEntity.IsConfigurable;
                    attributesModel.IsSwatch = attributeEntity.IsSwatch;
                    attributesModel.SelectValues = attributeEntity.SelectValues.ToList();
                    attributesModel.DisplayOrder = attributeEntity.DisplayOrder;
                    attributesModel.ConfigurableAttribute.AddRange(attributesList);
                    configurableAttributeList.Add(attributesModel);
                }
            }
            return configurableAttributeList;
        }

        //
        protected virtual IEnumerable<PublishAttributeModel> ExitingAttributeList(string selectedCode, string selectedValue, List<PublishProductModel> products, Dictionary<string, string> SelectedAttributes)
        {
            List<PublishProductModel> matchProductList = new List<PublishProductModel>();

            foreach (PublishProductModel product in products)
            {
                foreach (PublishAttributeModel attribute in product.Attributes)
                {
                    if (SelectedAttributes.Keys.Contains(attribute.AttributeCode) && SelectedAttributes.Values.Contains(attribute.AttributeValues))
                        matchProductList.Add(product);
                }
            }
            IEnumerable<PublishAttributeModel> Attributes = matchProductList.SelectMany(x => x.Attributes?.Where(y => y.IsConfigurable));
            return Attributes;
        }

        //Get attribute values and code.
        protected virtual Dictionary<string, string> GetAttributeValues(string codes, string values)
        {
            //Attribute Code And Value 
            string[] Codes = codes.Split(',');
            string[] Values = values.Split(',');
            Dictionary<string, string> SelectedAttributes = new Dictionary<string, string>();

            //Add code and value pair
            for (int i = 0; i < Codes.Length; i++)
                SelectedAttributes.Add(Codes[i], Values[i]);
            return SelectedAttributes;
        }

        //Get Attribute value already exist
        public virtual bool? AlreadyExist(List<ConfigurableAttributeModel> ConfigurableAttributeList, string value) =>
         ConfigurableAttributeList?.Any(x => x.AttributeValue == value);


        //Get configurable product filter.
        protected virtual FilterCollection GetConfigurableProductFilter(int localeId, List<PublishedConfigurableProductEntityModel> configEntity)
        {
            ZnodeLogging.LogMessage("Input Parameter localeId:", string.Empty, TraceLevel.Info, new object[] { localeId });
            FilterCollection filters = new FilterCollection();
            //Associated product ids.
            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.In, string.Join(",", (configEntity?.Select(x => x.AssociatedZnodeProductId)?.ToArray())));
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            return filters;
        }

        //Get Product Image Path
        protected virtual void GetProductImagePath(int portalId, PublishProductModel publishProduct, bool includeAlternateImages = true, string parentProductImageName = "")
        {
            //Get Product Image Path
            if (portalId > 0 && HelperUtility.IsNotNull(publishProduct))
            {
                IImageHelper image = GetService<IImageHelper>();
                string ProductImageName = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues;

                publishProduct.ImageLargePath = image.GetImageHttpPathLarge(ProductImageName);
                publishProduct.ImageMediumPath = image.GetImageHttpPathMedium(ProductImageName);
                publishProduct.ImageThumbNailPath = image.GetImageHttpPathThumbnail(ProductImageName);
                publishProduct.ImageSmallPath = image.GetImageHttpPathSmall(ProductImageName);
                publishProduct.OriginalImagepath = image.GetOriginalImagepath(ProductImageName);
                if (string.IsNullOrEmpty(publishProduct.ParentProductImageSmallPath) && !string.IsNullOrEmpty(parentProductImageName) && publishProduct.IsConfigurableProduct)
                    publishProduct.ParentProductImageSmallPath = image.GetImageHttpPathSmall(parentProductImageName);
                if (publishProduct?.PublishBundleProducts?.Count() > 0)
                    publishProduct?.PublishBundleProducts.ForEach(x => x.ImageThumbNailPath = image.GetImageHttpPathThumbnail(x.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues));
                if (includeAlternateImages)
                    GetProductAlterNateImages(publishProduct, portalId, image);
            }
        }

        //Get Product AlterNateImages.
        protected virtual void GetProductAlterNateImages(PublishProductModel product, int portalId, IImageHelper imageHelper)
        {
            string alternateImages = product.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.GalleryImages)?.AttributeValues;
            if (!string.IsNullOrEmpty(alternateImages))
            {
                product.AlternateImages = new List<ProductAlterNateImageModel>();
                var images = alternateImages.Split(',');
                foreach (string image in images)
                    product.AlternateImages.Add(new ProductAlterNateImageModel { FileName = image, ImageSmallPath = imageHelper.GetImageHttpPathSmall(image), ImageThumbNailPath = imageHelper.GetImageHttpPathThumbnail(image), OriginalImagePath = imageHelper.GetOriginalImagepath(image), ImageLargePath = imageHelper.GetImageHttpPathLarge(image) });
            }
        }

        //Get Portal and locale id from filter.
        protected virtual void GetLocaleAndPortalId(FilterCollection filters, out int localeId, out int portalId)
        {
            int.TryParse(filters.FirstOrDefault(x => x.FilterName == FilterKeys.LocaleId)?.FilterValue, out localeId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName == FilterKeys.PortalId)?.FilterValue, out portalId);
        }

        //Get Image Path For Category and set stored based In Stock, Out Of Stock, Back Order Message for List.
        protected virtual void SetProductDetailsForList(int portalId, PublishProductListModel publishProductListModel)
        {
            ZnodeLogging.LogMessage("Input Parameter portalId:", string.Empty, TraceLevel.Info, new object[] { portalId });
            string ImageName = string.Empty;
            IImageHelper image = GetService<IImageHelper>();

            if (portalId > 0)
            {
                ZnodePortal portalDetails = GetPortalDetailsById(portalId);

                //Get image path for products.
                publishProductListModel?.PublishProducts.ForEach(
                    x =>
                    {
                        ImageName = x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues;
                        x.ImageSmallPath = image.GetImageHttpPathSmall(ImageName);
                        x.ImageMediumPath = image.GetImageHttpPathMedium(ImageName);
                        x.ImageThumbNailPath = image.GetImageHttpPathThumbnail(ImageName);
                        x.ImageSmallThumbnailPath = image.GetImageHttpPathSmallThumbnail(ImageName);
                        x.InStockMessage = portalDetails?.InStockMsg;
                        x.OutOfStockMessage = portalDetails?.OutOfStockMsg;
                        x.BackOrderMessage = portalDetails?.BackOrderMsg;
                        x.PortalId = portalId;
                        x.ProductType = x.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductType)?.SelectValues.FirstOrDefault()?.Value;
                        x.IsActive = Convert.ToBoolean(x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.IsActive)?.FirstOrDefault()?.AttributeValues);
                    });
            }
            else
            {
                //Get image path for products.
                publishProductListModel?.PublishProducts.ForEach(
                    x =>
                    {
                        ImageName = x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues;
                        x.ImageSmallPath = image.GetImageHttpPathSmall(ImageName);
                        x.ImageMediumPath = image.GetImageHttpPathMedium(ImageName);
                        x.ImageThumbNailPath = image.GetImageHttpPathThumbnail(ImageName);
                        x.ImageSmallThumbnailPath = image.GetImageHttpPathSmallThumbnail(ImageName);
                        x.ProductType = x.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductType)?.SelectValues.FirstOrDefault()?.Value;
                        x.IsActive = Convert.ToBoolean(x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.IsActive)?.FirstOrDefault()?.AttributeValues);
                    });
            }
        }

        //Where Clause For PortalId.
        public virtual string WhereClauseForPortalId(int portalId)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());
            return DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause;
        }

        //Get parameter values from filters.
        protected virtual void GetParametersValueForFilters(FilterCollection filters, out int catalogId, out int portalId, out int localeId)
        {
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(WebStoreEnum.ZnodeCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out catalogId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);
        }

        //Get Default configurable product.
        protected virtual PublishProductModel GetDefaultConfigurableProduct(NameValueCollection expands, int portalId, int localeId, PublishProductModel publishProduct, List<PublishProductModel> associatedProducts, List<string> ConfigurableAttributeCodes = null, int? catalogVersionId = 0)
        {
            //Parent Product Properties
            string sku = publishProduct.SKU;
            string parentSEOCode = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == "SKU")?.AttributeValues;
            int categoryIds = publishProduct.ZnodeCategoryIds;
            List<PublishCategoryModel> categoryHierarchyIds = publishProduct.CategoryHierarchy;
            string parentProductImageSmallPath = publishProduct.ParentProductImageSmallPath;
            List<PublishAttributeModel> parentPersonalizableAttributes = publishProduct.Attributes?.Where(x => x.IsPersonalizable).ToList();
            string parentConfigurableProductName = publishProduct.Name;
            int configurableProductId = publishProduct.PublishProductId;

            //assign values from select values into attribute values
            associatedProducts.ForEach(x => x.Attributes.Where(y => y.IsConfigurable).ToList()
            .ForEach(z =>
            {
                z.AttributeValues = z.SelectValues.FirstOrDefault()?.Value;
            }));

            ZnodePublishConfigurableProductEntity publishConfigurableProductEntity =
                GetPublishConfigurableProductEntity(publishProduct.PublishProductId, publishProduct.ZnodeCatalogId, catalogVersionId.GetValueOrDefault());

            if (HelperUtility.IsNotNull(publishConfigurableProductEntity))
                ConfigurableAttributeCodes = JsonConvert.DeserializeObject<List<string>>(publishConfigurableProductEntity.ConfigurableAttributeCodes);

            //Get Default Product on basis of default/active or at last first or default.
            publishProduct = GetDefaultConfigurableProduct(publishProduct, associatedProducts, catalogVersionId, publishConfigurableProductEntity);

            publishProduct.ConfigurableProductId = configurableProductId;
            publishProduct.ParentSEOCode = parentSEOCode;
            publishProduct.ConfigurableProductSKU = publishProduct.SKU;
            publishProduct.CategoryHierarchy = categoryHierarchyIds;
            publishProduct.ParentProductImageSmallPath = parentProductImageSmallPath;
            publishProduct.SKU = sku;
            publishProduct.ZnodeCategoryIds = categoryIds;
            publishProduct.IsConfigurableProduct = true;
            publishProduct.ParentConfiguarableProductName = parentConfigurableProductName;

            catalogVersionId = catalogVersionId > 0 ? catalogVersionId : GetCatalogVersionId();

            //Get expands associated to Product.
            publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProduct, localeId, WhereClauseForPortalId(portalId), GetLoginUserId(), catalogVersionId, WebstoreVersionId, GetProfileId());

            PublishAttributeModel defaultAttribute = publishProduct.Attributes.FirstOrDefault(x => x.IsConfigurable);

            List<PublishAttributeModel> variants = publishProduct.Attributes?.Where(x => x.IsConfigurable).ToList();

            Dictionary<string, string> selectedAttribute = GetSelectedAttributes(variants);

            ConfigurableAttributeCodes = ConfigurableAttributeCodes ?? variants.Select(x => x.AttributeCode).ToList();

            List<PublishAttributeModel> attributeList = MapWebStoreConfigurableAttributeData(publishProductHelper.GetConfigurableAttributes(associatedProducts, ConfigurableAttributeCodes), defaultAttribute?.AttributeCode, defaultAttribute?.AttributeValues, selectedAttribute, associatedProducts, ConfigurableAttributeCodes, portalId, publishProduct.IsDefaultConfigurableProduct);


            if (HelperUtility.IsNotNull(attributeList) && attributeList.Count > 0)
            {
                publishProduct.Attributes.RemoveAll(x => attributeList.Select(y => y.AttributeCode).Contains(x.AttributeCode));
                publishProduct.Attributes.AddRange(attributeList);
            }

            return publishProduct;
        }

        // Method to get published configurable product entity details.
        protected virtual ZnodePublishConfigurableProductEntity GetPublishConfigurableProductEntity(int publishProductId, int catalogId, int catalogVersionId)
        {
            // As no matching model is available for mapping used var keyword.
            var publishConfigurableEntity = _publishConfigurableProductEntity.Table.Where(x => x.ZnodeProductId == publishProductId
                                 && x.ZnodeCatalogId == catalogId
                                 && x.VersionId == catalogVersionId)
                                 ?.OrderByDescending(x => x.IsDefault)
                                 ?.Select(x => new
                                 {
                                     x.PublishConfigurableProductEntityId,
                                     x.AssociatedZnodeProductId,
                                     x.ConfigurableAttributeCodes,
                                     x.IsDefault
                                 })?.FirstOrDefault();

            if (HelperUtility.IsNull(publishConfigurableEntity))
                return null;

            return new ZnodePublishConfigurableProductEntity()
            {
                PublishConfigurableProductEntityId = publishConfigurableEntity.PublishConfigurableProductEntityId,
                AssociatedZnodeProductId = publishConfigurableEntity.AssociatedZnodeProductId,
                ConfigurableAttributeCodes = publishConfigurableEntity.ConfigurableAttributeCodes,
                IsDefault = publishConfigurableEntity.IsDefault
            };
        }

        protected virtual Dictionary<string, string> GetSelectedAttributes(List<PublishAttributeModel> variants)
        {
            Dictionary<string, string> selectedAttribute = new Dictionary<string, string>();

            variants.ForEach(m =>
            {
                if (HelperUtility.IsNotNull(m) && !selectedAttribute.ContainsKey(m.AttributeCode))
                    selectedAttribute.Add(m.AttributeCode, m.AttributeValues);
            });
            return selectedAttribute;
        }

        //Gets Category hierarchy
        protected virtual List<PublishCategoryModel> GetProductCategory(string categoryIds, int localeId, int catalogId, List<ZnodePublishSeoEntity> categorySeoDetails)
        {
            ZnodeLogging.LogMessage("Input Parameter categoryIds,localeId and catalogId:", string.Empty, TraceLevel.Info, new object[] { categoryIds, localeId, catalogId });
            FilterCollection categoryFilter = new FilterCollection() { new FilterTuple(WebStoreEnum.ZnodeCategoryId.ToString(), FilterOperators.In, !string.IsNullOrEmpty(categoryIds)? categoryIds: "0"),
                                                                               new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()),
                                                                               new FilterTuple(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString())};

            List<PublishedCategoryEntityModel> categoryList = GetService<IPublishedCategoryDataService>().GetPublishedCategoryList(new PageListModel(categoryFilter, null, null ))?.ToModel<PublishedCategoryEntityModel>()?.ToList();

            ZnodeLogging.LogMessage("categoryList list count:", string.Empty, TraceLevel.Verbose, categoryList?.Count());
            return categoryList?.Select(x => GetElasticCategory(categorySeoDetails, x)).ToList();
        }

        //Maps category entity to Elastic category.
        protected virtual PublishCategoryModel GetElasticCategory(List<ZnodePublishSeoEntity> categorySeoDetails, PublishedCategoryEntityModel parentCategory)
        {
            PublishCategoryModel category = new PublishCategoryModel();
            category.SEODetails = new SEODetailsModel();
            category.Name = parentCategory.Name;
            category.PublishCategoryId = parentCategory.ZnodeCategoryId;
            category.SEODetails.SEOUrl = categorySeoDetails.Where(seoDetail => seoDetail.SEOId == parentCategory.ZnodeCategoryId && seoDetail.LocaleId == parentCategory.LocaleId)?.FirstOrDefault()?.SEOUrl;
            category.ParentCategory = (parentCategory.ZnodeParentCategoryIds?.Count() > 0) ? GetParentCategories(parentCategory.ZnodeParentCategoryIds, categorySeoDetails) : null;

            return category;
        }

        //Gets parent category hierarchy.
        protected virtual List<PublishCategoryModel> GetParentCategories(int[] parentCategoryIds, List<ZnodePublishSeoEntity> categorySeoDetails)
        {
            List<PublishCategoryModel> parentCategories = new List<PublishCategoryModel>();
            foreach (int categoryId in parentCategoryIds)
            {
                FilterCollection categoryFilter = new FilterCollection() { new FilterTuple(WebStoreEnum.ZnodeCategoryId.ToString(), FilterOperators.In, string.Join(",", categoryId)) };


                PublishedCategoryEntityModel parentCategory = GetService<IPublishedCategoryDataService>().GetPublishedCategory(categoryFilter)?.ToModel<PublishedCategoryEntityModel>();
                if (HelperUtility.IsNotNull(parentCategory))
                {
                    PublishCategoryModel elasticParentCategory = GetElasticCategory(categorySeoDetails, parentCategory);

                    parentCategories.Add(elasticParentCategory);
                }
            }
            ZnodeLogging.LogMessage("parentCategories list count:", string.Empty, TraceLevel.Verbose, parentCategories?.Count());
            return parentCategories;
        }

        //to check EnableProfileBasedSearch is true or false
        protected virtual bool EnableProfileBasedSearch(int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter portalId:", string.Empty, TraceLevel.Info, new object[] { portalId });
            Dictionary<string, bool> portalFeatureValues = ZnodeConfigManager.GetSiteConfigFeatureValueList(portalId);
            return portalFeatureValues.ContainsKey(HelperUtility.StoreFeature.Enable_Profile_Based_Search.ToString()) && portalFeatureValues[HelperUtility.StoreFeature.Enable_Profile_Based_Search.ToString()];
        }

        //to get user profileid by userId
        protected virtual int? GetUserProfileId(int userId)
        {
            ZnodeLogging.LogMessage("Input Parameter userId:", string.Empty, TraceLevel.Info, new object[] { userId });
            int? profileId = null;
            int? userCatalogId = (from acc in _userAccount.Table
                                  join user in _user.Table on acc.AccountId equals user.AccountId
                                  where user.UserId == userId
                                  select acc)?.FirstOrDefault()?.PublishCatalogId;
            if (Equals(userCatalogId, null))
                profileId = (_userProfile.Table.Where(x => x.UserId == userId && x.IsDefault == true)?.FirstOrDefault()?.ProfileId) ?? null;
            ZnodeLogging.LogMessage("Parameter:", string.Empty, TraceLevel.Info, new { profileId = profileId, userCatalogId = userCatalogId });
            return profileId;
        }

        //Get associated simple product price of group product.
        public virtual void GetAssociatedGroupProductPrice(ProductInventoryPriceModel productModel, int productId, int portalId, int localeId)
        {
            //Get associated simple products.
            List<WebStoreGroupProductModel> groupProductList = GetAssociatedGroupProducts(productId, portalId, localeId);
            ZnodeLogging.LogMessage("groupProductList list count:", string.Empty, TraceLevel.Verbose, groupProductList?.Count());
            productModel.GroupProductPriceMessage = GetGroupProductMessage(groupProductList, productModel);
        }

        //Get associated group products.
        protected virtual List<WebStoreGroupProductModel> GetAssociatedGroupProducts(int productId, int portalId, int localeId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.Equals, productId.ToString());

            //get group product entity.
            return GetGroupProductList(filters, portalId, Convert.ToString(localeId));
        }

        //Get associated product list.
        public virtual PublishProductModel GetAssociatedConfigurableProduct(int productId, int localeId, int? catalogVersionId, int portalId)
        {

            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter localeId,productId and portalId:", string.Empty, TraceLevel.Info, new object[] { localeId, productId, portalId });
            //Get configurable product entity.
            List<PublishedConfigurableProductEntityModel> configEntity = publishProductHelper.GetConfigurableProductEntity(productId, catalogVersionId);
            //Get associated product list.
            List<PublishProductModel> associatedProducts = publishProductHelper.GetAssociatedProducts(productId, localeId, catalogVersionId, configEntity);
            ZnodeLogging.LogMessage("List count:", string.Empty, TraceLevel.Verbose, new { associatedProductsCount = associatedProducts?.Count(), configEntityCount = configEntity?.Count() });
            if (associatedProducts?.Count > 0)
            {
                //Get first product from list of associated products 
                PublishProductModel publishProduct = associatedProducts.FirstOrDefault();
                //Get expands associated to Product.
                publishProductHelper.GetProductPriceData(publishProduct, portalId, GetLoginUserId(), GetProfileId());
                return publishProduct;
            }
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return new PublishProductModel();
        }

        public virtual List<WebStoreGroupProductModel> GetGroupProductList(FilterCollection filters, int portalId, string localeId, bool isInventory = false)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter localeId and portalId:", string.Empty, TraceLevel.Info, new object[] { localeId, portalId });
            List<WebStoreGroupProductModel> productsList = new List<WebStoreGroupProductModel>();


            List<ZnodePublishGroupProductEntity> groupProducts = GetService<IPublishedProductDataService>().GetPublishedGroupProducts(new PageListModel(filters, null, null));
            ZnodeLogging.LogMessage("groupProducts list count:", string.Empty, TraceLevel.Verbose, groupProducts?.Count());
            if (HelperUtility.IsNotNull(groupProducts) && (groupProducts.Count > 0))
            {
                filters.Clear();

                //Associated product ids.
                filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.In, string.Join(",", groupProducts?.Select(x => x.AssociatedZnodeProductId)?.ToArray()));
                filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId);
                filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(groupProducts?.FirstOrDefault()?.VersionId));

                SetProductIndexFilter(filters);

                //get associated product list.
                List<ZnodePublishProductEntity> products = GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(filters, null, null));

                ZnodeLogging.LogMessage("products list count:", string.Empty, TraceLevel.Verbose, products?.Count());
                products?.ForEach(x => x.AssociatedProductDisplayOrder = groupProducts.FirstOrDefault(y => y.AssociatedZnodeProductId == x.ZnodeProductId).AssociatedProductDisplayOrder);
                products = products?.GroupBy(x => x.ZnodeProductId).Select(y => y.First()).ToList();
                productsList = products?.ToModel<WebStoreGroupProductModel>().ToList();

                //Map price of simple products associated to group product.
                if (isInventory)
                    MapGroupProductPriceAndInventory(productsList, publishProductHelper.GetPricingBySKUs(productsList.Select(x => x.SKU), portalId, GetLoginUserId(), GetProfileId()), publishProductHelper.GetInventoryBySKUs(productsList.Select(x => x.SKU), portalId));
                else
                    MapGroupProductPrice(productsList, publishProductHelper.GetPricingBySKUs(productsList.Select(x => x.SKU), portalId, GetLoginUserId(), GetProfileId()));
            }
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return productsList;
        }

        //Get Clipart Value from Xml.
        public virtual string GetClipartValue(string decorationOption, string descendants, string element)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter decorationOption,descendants and element:", string.Empty, TraceLevel.Info, new object[] { decorationOption, descendants, element });
            if (!string.IsNullOrEmpty(decorationOption))
            {
                XDocument xml = XDocument.Parse($"<Root>{decorationOption}</Root>");
                var values = (from el in xml.Descendants(descendants) select new { el.Element(element)?.Value })?.FirstOrDefault();
                return (values != null) ? values.Value : string.Empty;
            }
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return decorationOption;
        }

        //Get product inventory
        public virtual ProductInventoryDetailModel GetProductInventory(int publishProductId, FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameters publishProductId:", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info, publishProductId);
            int portalId, localeId;
            int? catalogVersionId;
            PublishProductModel publishProduct = null;
            ProductInventoryDetailModel productInventory = null;
            //Bind the IsGetAllLocationsInventory from filters to expands
            BindAllLocationsFlagInExpands(filters, expands);

            //Remove Profile id from filter
            filters.RemoveAll(x => x.FilterName.ToLower() == WebStoreEnum.ProfileIds.ToString().ToLower());

            //Get publish product 
            List<PublishProductModel> products = GetPublishProductFromPublishedData(publishProductId, filters, out portalId, out localeId, out catalogVersionId, publishProduct, out products);

            if (products?.Count > 0)
            {
                publishProduct = products.FirstOrDefault();
                productInventory = new ProductInventoryDetailModel() { ProductName = publishProduct.Name, SKU = publishProduct.SKU };
                GetRequiredProductInventoryDetails(productInventory, GetProductFiltersForSP(new List<PublishProductModel> { publishProduct }), GetLoginUserId(), portalId, GetSendLocationsFlag(expands));
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            return productInventory;
        }

        // Get associated configurable variants.
        public virtual List<WebStoreConfigurableProductModel> GetAssociatedConfigurableVariants(int publishProductId)
        {
            int? catalogVersionId = GetCatalogVersionId();

            IZnodeViewRepository<WebStoreConfigurableProductModel> objStoredProc = new ZnodeViewRepository<WebStoreConfigurableProductModel>();
            objStoredProc.SetParameter("@PortalId", PortalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@catalogVersionId", catalogVersionId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@currentUtcDate", HelperUtility.GetDateTime().Date, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@publishProductId", publishProductId, ParameterDirection.Input, DbType.Int32);

            List<WebStoreConfigurableProductModel> associatedProducts = objStoredProc.ExecuteStoredProcedureList("Znode_GetConfigurableVariantData @PortalId,@UserId,@catalogVersionId,@currentUtcDate,@publishProductId")?.ToList();

            if (HelperUtility.IsNotNull(associatedProducts) && (associatedProducts.Count > 0))
            {
                associatedProducts.ForEach(x => x.ProductAttributes = JsonConvert.DeserializeObject<List<PublishAttributeModel>>(x.Attributes));
                associatedProducts.ForEach(x => x.ConfigurableAttributeCodeList = JsonConvert.DeserializeObject<List<string>>(x.ConfigurableAttributeCodes));

                //Get image names for associated products.
                associatedProducts.ForEach(
                    x =>
                    {
                        x.ImageName = x.ProductAttributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues;
                        x.AlternateImageName = x.ProductAttributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.GalleryImages)?.AttributeValues;
                    });
            }
            return associatedProducts;
        }

        //Returns the value for send all locations flag
        protected virtual bool GetSendLocationsFlag(NameValueCollection expands)
       => Convert.ToBoolean(expands?.Get(ZnodeConstant.IsGetAllLocationsInventory));

        //Get product inventory details
        protected virtual void GetRequiredProductInventoryDetails(ProductInventoryDetailModel productInventory, DataTable tableDetails, int userId = 0, int portalId = 0, bool isSendAllLocations = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            DataSet inventoryDetails = null;
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            executeSpHelper.GetParameter("@PortalId", portalId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@UserId", userId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@currentUtcDate", HelperUtility.GetDateTime().Date, ParameterDirection.Input, SqlDbType.Text);

            ZnodeLogging.LogMessage("userId and portalId: ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { userId, portalId });

            if (DefaultGlobalConfigSettingHelper.IsColumnEncryptionSettingEnabled)
            {
                executeSpHelper.GetParameter("@ProductDetailsFromWebStore", tableDetails?.ToJson(), ParameterDirection.Input, SqlDbType.Text);
                //Gets the entity list according to where clause, order by clause and pagination
                inventoryDetails = executeSpHelper.GetSPResultInDataSet("Znode_GetProductInfoForWebStoreWithJSON");
            }
            else
            {
                executeSpHelper.SetTableValueParameter("@ProductDetailsFromWebStore", tableDetails, ParameterDirection.Input, SqlDbType.Structured, "dbo.ProductDetailsFromWebStore");
                executeSpHelper.GetParameter("@IsAllLocation", isSendAllLocations, ParameterDirection.Input, SqlDbType.Bit);
                //Gets the entity list according to where clause, order by clause and pagination
                inventoryDetails = executeSpHelper.GetSPResultInDataSet("Znode_GetProductWarehouseInfoForWebStore");
            }
            ConvertDataTableToList dt = new ConvertDataTableToList();
            List<InventorySKUModel> productInventoryDetails = inventoryDetails?.Tables.Count > 0 ? dt.ConvertDataTable<InventorySKUModel>(inventoryDetails?.Tables[0]) : new List<InventorySKUModel>();
            //Bind product details.
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            BindProductInventoryDetails(productInventory, portalId, productInventoryDetails, isSendAllLocations);
        }

        //Bind product details.
        protected virtual void BindProductInventoryDetails(ProductInventoryDetailModel productInventory, int portalId, IList<InventorySKUModel> productInventoryDetails, bool isSendAllLocations = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("portalId: ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, portalId);

            InventorySKUModel productInventorySKU = productInventoryDetails?.FirstOrDefault(productdata => productdata.SKU == productInventory.SKU);

            if (HelperUtility.IsNotNull(productInventorySKU))
            {
                productInventory.AllLocationQuantity = productInventorySKU.AllLocationQuantity;
                if (isSendAllLocations) productInventory.Inventory = GetAllLocationsInventoryForProduct(productInventory.SKU, productInventoryDetails);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
        }

        //returns the list of all inventories associated to the product for that portal.
        protected virtual List<InventorySKUModel> GetAllLocationsInventoryForProduct(string ProductSKU, IList<InventorySKUModel> productDetails)
        {
            List<InventorySKUModel> productInventoryList = new List<InventorySKUModel>();
            List<InventorySKUModel> products = productDetails?.Where(x => x.SKU == ProductSKU)?.ToList();
            if(products?.Count > 0)
            {
                productInventoryList.AddRange(products);
            }
            return productInventoryList;
        }

        //Bind the IsGetAllLocationsInventory from filters to expands
        protected virtual void BindAllLocationsFlagInExpands(FilterCollection filters, NameValueCollection expands)
        {
            bool isSendAllStockLevels = Convert.ToBoolean(filters.FirstOrDefault(m => string.Compare(m.FilterName, ZnodeConstant.IsGetAllLocationsInventory, true) == 0)?.Item3);
            if (isSendAllStockLevels)
                expands.Add(ZnodeConstant.IsGetAllLocationsInventory, isSendAllStockLevels.ToString());
            filters.RemoveAll(x => x.FilterName.Equals(ZnodeConstant.IsGetAllLocationsInventory, StringComparison.InvariantCultureIgnoreCase));
        }

        //Map Price and inventory of group products.
        protected virtual void MapGroupProductPriceAndInventory(List<WebStoreGroupProductModel> productsList, List<PriceSKUModel> priceList, List<InventorySKUModel> inventoryList)
        {
            if (priceList?.Count > 0)
            {
                ZnodeLogging.LogMessage("productsList list count:", string.Empty, TraceLevel.Verbose, productsList?.Count());
                MapGroupProductPrice(productsList, priceList);
                productsList.ForEach(product =>
                {
                    InventorySKUModel inventorySKU = inventoryList
                                 .Where(productdata => productdata.SKU == product.SKU)
                                 ?.FirstOrDefault();

                    if (HelperUtility.IsNotNull(inventorySKU))
                    {
                        product.Quantity = inventorySKU?.Quantity;
                        product.ReOrderLevel = inventorySKU?.ReOrderLevel;
                        product.DefaultWarehouseName = inventorySKU?.WarehouseName;
                        product.DefaultWarehouseCount = inventorySKU?.DefaultInventoryCount;                        
                    }
                });
            }
        }

        //Map Price of group products.
        protected virtual void MapGroupProductPrice(List<WebStoreGroupProductModel> productsList, List<PriceSKUModel> priceList)
        {
            if (priceList?.Count > 0)
            {
                ZnodeLogging.LogMessage("productsList list count:", string.Empty, TraceLevel.Verbose, productsList?.Count());
                productsList.ForEach(product =>
                {
                    PriceSKUModel priceSKU = priceList
                                 .Where(productdata => productdata.SKU == product.SKU)
                                 ?.FirstOrDefault();


                    if (HelperUtility.IsNotNull(priceSKU))
                    {
                        product.SalesPrice = priceSKU.SalesPrice;
                        product.RetailPrice = priceSKU.RetailPrice;
                        product.CurrencyCode = priceSKU.CurrencyCode;
                        product.CultureCode = priceSKU.CultureCode;
                        product.CurrencySuffix = priceSKU.CurrencySuffix;
                    }

                });
            }
        }



        //Map Price of products.
        protected virtual List<ProductInventoryPriceModel> MapProductPrice(List<PriceSKUModel> productPriceList)
        {
            if (productPriceList?.Count > 0)
            {
                ZnodeLogging.LogMessage("productsList list count:", string.Empty, TraceLevel.Verbose, productPriceList?.Count());
                List<ProductInventoryPriceModel> productList = new List<ProductInventoryPriceModel>();

                foreach (PriceSKUModel productPrice in productPriceList)
                {
                    ProductInventoryPriceModel product = new ProductInventoryPriceModel();
                    product.SalesPrice = productPrice.SalesPrice;
                    product.RetailPrice = productPrice.RetailPrice;
                    product.SKU = productPrice.SKU;
                    product.CurrencyCode = productPrice.CurrencyCode;
                    product.CultureCode = productPrice.CultureCode;
                    productList.Add(product);
                }
                return productList;
            }
            return new List<ProductInventoryPriceModel>();
        }

        //Get paging model with filters.
        protected virtual PageListModel GetPageModel(string skus, int catalogId)
        {
            ZnodeLogging.LogMessage("Input Parameter skus and catalogId:", string.Empty, TraceLevel.Info, new object[] { skus, catalogId });
            FilterCollection filters = new FilterCollection();
            if(!string.IsNullOrEmpty(skus))
                filters.Add(new FilterTuple(ZnodeTaxClassSKUEnum.SKU.ToString(), FilterOperators.In, skus));
            filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(GetCatalogVersionId(catalogId)));
            return new PageListModel(filters, null, null);
        }

        //Get group product price.
        protected virtual ProductInventoryPriceListModel GetAsyncGroupProductPrice(List<PublishedProductEntityModel> productList, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter portalId and localeId:", string.Empty, TraceLevel.Info, new object[] { portalId, localeId });
            ProductInventoryPriceListModel groupProductList = new ProductInventoryPriceListModel { ProductList = new List<ProductInventoryPriceModel>() };
            foreach (PublishedProductEntityModel product in productList)
            {
                ProductInventoryPriceModel productPrice = new ProductInventoryPriceModel();
                GetAssociatedGroupProductPrice(productPrice, product.ZnodeProductId, portalId, localeId);
                productPrice.SKU = product.SKU;
                groupProductList.ProductList.Add(productPrice);
            }

            return groupProductList;
        }

        //Get configurable product price.
        protected virtual ProductInventoryPriceListModel GetAsyncConfigurableProductPrice(List<ZnodePublishProductEntity> productList, int catalogId, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter portalId,catalogId and localeId:", string.Empty, TraceLevel.Info, new object[] { portalId, catalogId, localeId });
            ProductInventoryPriceListModel configurableProductList = new ProductInventoryPriceListModel { ProductList = new List<ProductInventoryPriceModel>() };
            foreach (ZnodePublishProductEntity product in productList)
            {
                PublishProductModel productModel = product?.ToModel<PublishProductModel>();

                //Get price of configurable product.
                publishProductHelper.GetDataFromExpands(portalId, new List<string> { ZnodeConstant.Promotions, ZnodeConstant.Pricing }, productModel, localeId, "", GetLoginUserId(), GetCatalogVersionId(catalogId).GetValueOrDefault(), null, GetProfileId());

                //get associated products price if main configurable product don't have price  associated.
                if (HelperUtility.IsNull(productModel.SalesPrice) && HelperUtility.IsNull(productModel.RetailPrice))
                {
                    productModel = GetAssociatedConfigurableProduct(product.ZnodeProductId, localeId, GetCatalogVersionId(catalogId).GetValueOrDefault(), portalId);
                    productModel.SKU = product.SKU;
                    configurableProductList.ProductList.Add(productModel);
                }
                configurableProductList.ProductList.Add(productModel);
            }
            return configurableProductList;
        }

        //Get Portal Details By Id.
        public virtual ZnodePortal GetPortalDetailsById(int portalId)
        {
            string cacheKey = $"PortalDeatails_{portalId}";
            ZnodePortal portalDeatails = Equals(HttpRuntime.Cache[cacheKey], null)
               ? GetPortalDetailsByIdFromDB(portalId, cacheKey)
               : ((ZnodePortal)HttpRuntime.Cache.Get(cacheKey));
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);
            return portalDeatails;
        }

        protected virtual ZnodePortal GetPortalDetailsByIdFromDB(int portalId, string cacheKey)
        {
            ZnodePortal znodePortal = _portalRepository.GetById(portalId);
            if (HelperUtility.IsNotNull(znodePortal))
                HttpRuntime.Cache.Insert(cacheKey, znodePortal);
            return znodePortal;
        }

        //Get active category ids.
        protected virtual string GetActiveCategoryIds(int catalogId)
        {
            ZnodeLogging.LogMessage("Input Parameter catalogId:", string.Empty, TraceLevel.Info, new object[] { catalogId });
            int versionId = GetCatalogVersionId(catalogId).GetValueOrDefault();
            List<PublishedCategoryEntityModel> publishedCategory = GetService<IPublishedCategoryDataService>().GetActiveCategoryListByCatalogId(catalogId, versionId)?.ToModel<PublishedCategoryEntityModel>()?.ToList();
            return string.Join(",", publishedCategory?.Select(s => s.ZnodeCategoryId)?.Distinct()?.ToArray());
        }

        //Get active category ids.
        protected virtual string GetActiveCategoryIdsForSiteMap(int catalogId, int pageNo)
        {
            ZnodeLogging.LogMessage("Input Parameter catalogId and pageNo:", string.Empty, TraceLevel.Info, new object[] { catalogId, pageNo });
            List<PublishedCategoryEntityModel> publishedCategory = GetService<IPublishedCategoryDataService>().GetActiveCategoryListByCatalogId(catalogId, GetCatalogVersionId(catalogId).GetValueOrDefault())?.ToModel<PublishedCategoryEntityModel>()?.ToList();

            string catgoryId = string.Join(",", publishedCategory.Where(x => x.ZnodeParentCategoryIds == null)?.OrderBy(s => s.DisplayOrder).Select(s => s.ZnodeCategoryId)?.Distinct()?.Skip(pageNo - 1).Take(1));
            ZnodeLogging.LogMessage("catgoryId:", string.Empty, TraceLevel.Info, new object[] { catgoryId });
            return !string.IsNullOrEmpty(catgoryId) ? string.Join(",", catgoryId, GetActiveSubCategoryIdsForSiteMap(catalogId, Convert.ToInt32(catgoryId))) : string.Empty;
        }

        //Get active subcategory id.
        protected virtual string GetActiveSubCategoryIdsForSiteMap(int catalogId, int categoryId)
        {
            List<PublishedCategoryEntityModel> publishedCategory = GetService<IPublishedCategoryDataService>().GetActiveCategoryListByCatalogId(catalogId, GetCatalogVersionId(catalogId).GetValueOrDefault())?.ToModel<PublishedCategoryEntityModel>()?.ToList();

            return string.Join(",", publishedCategory.Where(x => x.ZnodeParentCategoryIds.Contains(categoryId))?.OrderBy(s => s.DisplayOrder).Select(s => s.ZnodeCategoryId)?.Distinct()?.ToArray());


        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// This method returns product entity for supplied publishProductId.
        /// Additionally, it also returns the category hierarchy for the returned product.
        /// Category hierarchy retrieval is merged with product retrieval since they both require traversing through the associated categories at least once. 
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        protected virtual PublishProductModel GetPublishedProductFromPublishedData(int publishProductId, FilterCollection filters)
        {
            ISEOService _seoService = ZnodeDependencyResolver.GetService<ISEOService>();
            //Get parameter values from filters.
            int catalogId, portalId, localeId;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

            //Remove portal id filter.
            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);

            //Replace filter keys.
            ReplaceFilterKeys(ref filters);

            //get catalog current version id by catalog id.
            int? catalogVersionId = GetCatalogVersionId(catalogId);

            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.Equals, Convert.ToString(publishProductId));

            if (catalogVersionId > 0)
                filters.Add(FilterKeys.VersionId, FilterOperators.Equals, catalogVersionId.HasValue ? catalogVersionId.Value.ToString() : "0");

            PublishProductModel publishProduct = null;
            PublishedCategoryEntityModel associatedCategory = null;
            //Get publish product 
            List<PublishProductModel> products = GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(filters, null, null))?.ToModel<PublishProductModel>()?.ToList();

            if (HelperUtility.IsNotNull(products))
            {
                associatedCategory = GetActiveAssociatedCategory(products);
                if (HelperUtility.IsNotNull(associatedCategory))
                {
                    publishProduct = products?.FirstOrDefault(x => x.ZnodeCategoryIds == associatedCategory.ZnodeCategoryId);
                    publishProduct.CategoryHierarchy = GetProductCategory(associatedCategory.ZnodeCategoryId.ToString(), localeId, catalogId,  GetService<IPublishedPortalDataService>().GetSEOSettings(GetFiltersForSEO(portalId, localeId, ZnodeConstant.Category)));
                }
            }

            return publishProduct;
        }

        protected virtual FilterCollection GetFiltersForSEO(int portalId, int localeId, string seoTypeName)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple("SEOTypeName", FilterOperators.Is, seoTypeName));
            filters.Add(new FilterTuple("LocaleId", FilterOperators.Equals, localeId.ToString()));
            filters.Add(new FilterTuple("VersionId", FilterOperators.Equals, GetCatalogVersionId().ToString()));
            if (HelperUtility.IsNotNull(portalId))
                filters.Add(new FilterTuple("PortalId", FilterOperators.Equals, portalId.ToString()));
            return filters;
        }

        /// <summary>
        /// This method returns an associated category on the basis of following criteria:
        /// 1. A category has been associated to any of the products found in supplied product list.
        /// 2. A category has its flag "IsActive" set to "true".
        /// Single category selection is done on "First to appear" basis.
        /// </summary>
        /// <param name="products">Collection of products which have one category associated with each.</param>
        /// <returns></returns>
        protected virtual PublishedCategoryEntityModel GetActiveAssociatedCategory(List<PublishProductModel> products)
        {
            List<int> categoryIds = products.Select(x => x.ZnodeCategoryIds).ToList();
            FilterCollection filters = new FilterCollection();
            if(categoryIds?.Count > 0)
                filters.Add("ZnodeCategoryId", FilterOperators.In, string.Join(",", categoryIds));
            filters.Add("IsActive", FilterOperators.Equals, ZnodeConstant.TrueValue);

            PublishedCategoryEntityModel publishCategory = GetService<IPublishedCategoryDataService>().GetPublishedCategory(filters)?.ToModel<PublishedCategoryEntityModel>();
            return publishCategory;
        }

        /// <summary>
        /// This method checks if the supplied collection of expands has one of the keys which can be expanded as a navigational property within the entity framework context.
        /// </summary>
        /// <param name="expands"></param>
        /// <returns>Returns true if the supplied collection has an expandable key.</returns>
        protected virtual bool ContainsExpandables(NameValueCollection expands)
        {
            string[] expandKeys = expands.AllKeys;
            string[] expandableKeys = new string[]
            {
                ZnodeConstant.Promotions.ToLowerInvariant(),
                ZnodeConstant.SEO.ToLowerInvariant(),
                ZnodeConstant.Inventory.ToLowerInvariant(),
                ZnodeConstant.Pricing.ToLowerInvariant(),
                ZnodeConstant.AddOns.ToLowerInvariant(),
                ZnodeConstant.ProductBrand.ToLowerInvariant(),
                ZnodeConstant.ProductReviews.ToLowerInvariant(),
                ZnodeConstant.AssociatedProducts.ToLowerInvariant(),
                ZnodeConstant.ProductTemplate.ToLowerInvariant()
            };

            return expandKeys.Intersect(expandableKeys).Count() > 0;
        }

        /// <summary>
        /// This method checks if child product has personalized attribute or not
        /// if not it will check parent product's personalized attribute and will add in child products attribute model.
        /// </summary>
        /// <param name="publishProduct"></param>
        /// <param name="childPersonalizableAttribute"></param>
        /// <param name="parentPersonalizableAttribute"></param>
        /// <returns>Returns true if the supplied collection has an expandable key.</returns>
        protected virtual PublishProductModel AddPersonalizeAttributeInChildProduct(PublishProductModel publishProduct, List<PublishAttributeModel> parentPersonalizableAttributes, bool isChildPersonalizableAttribute)
        {
            if (!isChildPersonalizableAttribute)
            {
                if (parentPersonalizableAttributes?.Count > 0)
                    publishProduct.Attributes.AddRange(parentPersonalizableAttributes);
            }
            return publishProduct;
        }

        /// <summary>
        /// This method only returns the details of a parent published product .
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="filters"></param>
        /// <param name="expands"></param>
        /// <returns></returns>
        protected virtual List<PublishProductModel> GetPublishProductFromPublishedData(int publishProductId, FilterCollection filters, out int portalId, out int localeId, out int? catalogVersionId, PublishProductModel publishProduct, out List<PublishProductModel> products)
        {
            //Get parameter values from filters.
            int catalogId;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

            //Remove portal id filter.
            filters.RemoveAll(x => string.Equals(x.FilterName,FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase));

            //Replace filter keys.
            ReplaceFilterKeys(ref filters);

            //get catalog current version id by catalog id.
            catalogVersionId = GetCatalogVersionId(catalogId);

            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.Equals, Convert.ToString(publishProductId));

            if (catalogVersionId > 0)
                filters.Add(FilterKeys.VersionId, FilterOperators.Equals, catalogVersionId.HasValue ? catalogVersionId.Value.ToString() : "0");

            publishProduct = null;

            //Get publish product
            products = GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(filters, null, null)).ToModel<PublishProductModel>().ToList();

            return products;
        }

        public virtual List<PublishProductModel> GetAssociatedConfigurableProducts(int localeId, int? catalogVersionId, IEnumerable<string> childSKU)
        {
            FilterCollection filters = QueryForAssociatedProduct(localeId, catalogVersionId, childSKU);

            List<PublishProductModel> associatedProducts = GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(filters, null, null))?.ToModel<PublishProductModel>()?.ToList();

            associatedProducts.ForEach(x => x.Attributes.Where(y => y.IsConfigurable).ToList()
           .ForEach(z =>
           {
               z.AttributeValues = z.SelectValues.FirstOrDefault()?.Value;
           }));

            associatedProducts = associatedProducts.OrderBy(d => childSKU.ToList().IndexOf(d.SKU)).ToList();

            return associatedProducts;
        }

        protected virtual FilterCollection QueryForAssociatedProduct(int localeId, int? catalogVersionId, IEnumerable<string> childSKU)
        {
            FilterCollection filters = new FilterCollection();
            if (HelperUtility.IsNotNull(childSKU))
                filters.Add("SKU", FilterOperators.In, string.Join(",", childSKU.Select(x => $"\"{x}\"")));
            filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());
            filters.Add("VersionId", FilterOperators.Equals, catalogVersionId.ToString());
            return filters;

        }

        public virtual PublishedConfigurableProductEntityModel GetConfigurableProductEntity(int productId, int? catalogVersionId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add("ZnodeProductId", FilterOperators.Equals, productId.ToString());
            if (catalogVersionId > 0)
                filters.Add("VersionId", FilterOperators.Equals, catalogVersionId.ToString());

            PublishedConfigurableProductEntityModel configEntity = GetService<IPublishedProductDataService>().GetPublishedConfigurableProducts(new PageListModel(filters, null, null)).FirstOrDefault().ToModel<PublishedConfigurableProductEntityModel>();

            return configEntity;
        }

        //get promotion price.
        protected virtual decimal GetPromotionPriceById(int publishProductId, decimal productPrice)
        {
            ZnodePricePromotionManager pricePromoManager = new ZnodePricePromotionManager();
            return pricePromoManager.PromotionalPrice(publishProductId, productPrice);
        }

        protected virtual List<PriceTierModel> GetTierPrice(List<PriceSKUModel> priceSKUModels)
        {
            if (HelperUtility.IsNull(priceSKUModels)) return new List<PriceTierModel>();

            List<PriceTierModel> priceTierModels = new List<PriceTierModel>();
            foreach (PriceSKUModel priceSKUModel in priceSKUModels)
            {
                if (priceSKUModel.TierPrice != null && priceSKUModel.TierQuantity != null)
                    priceTierModels.Add(new PriceTierModel() { Price = priceSKUModel.TierPrice, Quantity = priceSKUModel.TierQuantity });
            }
            return priceTierModels;
        }

        /// <summary>
        /// Method is use to get price and inventory model to display price and inventory on PDP page 
        /// </summary>
        /// <param name="productSkus"></param>
        /// <param name="priceSKU"></param>
        /// <param name="inventoryList"></param>
        /// <returns></returns>
        protected virtual ProductInventoryPriceModel GetPriceAndInventoryModel(ParameterInventoryPriceModel productSkus, List<PriceSKUModel> priceSKU, List<InventorySKUModel> inventoryList)
        {
            if(HelperUtility.IsNull(productSkus))
            {
                return new ProductInventoryPriceModel
                {
                    SKU = productSkus.SKU,
                    PriceView = productSkus.PriceView,
                    ObsoleteClass = productSkus.ObsoleteClass,
                    ProductType = productSkus.ProductType,
                    MinQuantity = productSkus.MinQuantity
                };
            }

            ProductInventoryPriceModel productInventoryPrice = (from price in priceSKU
                                                                join inventory in inventoryList on price.SKU equals inventory.SKU into ps
                                                                where price.SKU == productSkus.SKU
                                                                from inventory in ps.DefaultIfEmpty()
                                                                select new ProductInventoryPriceModel
                                                                {
                                                                    SalesPrice = price?.SalesPrice,
                                                                    RetailPrice = price?.RetailPrice,
                                                                    ReOrderLevel = inventory?.ReOrderLevel,
                                                                    Quantity = inventory?.Quantity,
                                                                    CurrencyCode = price.CurrencyCode,
                                                                    CultureCode = price.CultureCode,
                                                                    SKU = productSkus.SKU,
                                                                    PromotionalPrice = GetPromotionPriceById(productSkus.ProductId, HelperUtility.IsNotNull(price.SalesPrice) ? price.SalesPrice.GetValueOrDefault() : price.RetailPrice.GetValueOrDefault()),
                                                                    TierPriceList = GetTierPrice(priceSKU.Where(x => x.SKU == productSkus.SKU).ToList()),
                                                                    PriceView = productSkus.PriceView,
                                                                    ObsoleteClass = productSkus.ObsoleteClass,
                                                                    ProductType = productSkus.ProductType,
                                                                    MinQuantity = productSkus.MinQuantity
                                                                }).FirstOrDefault();


            return HelperUtility.IsNotNull(productInventoryPrice) ? productInventoryPrice : new ProductInventoryPriceModel
            {
                SKU = productSkus.SKU,
                PriceView = productSkus.PriceView,
                ObsoleteClass = productSkus.ObsoleteClass,
                ProductType = productSkus.ProductType,
                MinQuantity = productSkus.MinQuantity
            };
        }

        //Get Default Product on basis of default/active or at last first or default.
        protected virtual PublishProductModel GetDefaultConfigurableProduct(PublishProductModel publishProduct, List<PublishProductModel> associatedProducts , int? catalogVersionId = 0, ZnodePublishConfigurableProductEntity publishConfigurableProductEntity = null)
        {
            int? defaultProduct = 0;

            if (publishProduct.PublishProductId > 0 && publishProduct.ZnodeCatalogId > 0 && catalogVersionId > 0)
            {
                if (HelperUtility.IsNotNull(publishConfigurableProductEntity))
                    defaultProduct = publishConfigurableProductEntity.IsDefault == true ? publishConfigurableProductEntity?.AssociatedZnodeProductId : null;
                else
                    defaultProduct = _publishConfigurableProductEntity.Table?.FirstOrDefault(x => x.ZnodeProductId == publishProduct.PublishProductId && x.ZnodeCatalogId == publishProduct.ZnodeCatalogId
                                 && x.IsDefault == true && x.VersionId == catalogVersionId)?.AssociatedZnodeProductId;
            }

            if (defaultProduct > 0)
            {
                publishProduct = associatedProducts.FirstOrDefault(x => x.PublishProductId == defaultProduct && x.IsActive == true);

                if (HelperUtility.IsNull(publishProduct))
                {
                    publishProduct = associatedProducts.FirstOrDefault(x => x.IsActive == true);

                    publishProduct = HelperUtility.IsNull(publishProduct) ? associatedProducts.FirstOrDefault() : publishProduct;
                }
                publishProduct.IsDefaultConfigurableProduct = true;
            }
            else
            {
                //Get first product from list of associated products 
                publishProduct = associatedProducts.FirstOrDefault(x => x.IsActive == true);
                publishProduct = HelperUtility.IsNull(publishProduct) ? associatedProducts.FirstOrDefault() : publishProduct;
            }

            return publishProduct;
        }
        #endregion
    }
}
