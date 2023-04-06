using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class WebStoreWidgetService : BaseService, IWebStoreWidgetService
    {
        #region Protected Variables

        protected readonly IPublishProductService _publishProductService;
        protected readonly IZnodeRepository<ZnodeFormBuilder> _formBuilderRepository;
        protected readonly IZnodeRepository<ZnodeCMSFormWidgetConfiguration> _formWidgetConfigurationRepository;
        protected readonly IZnodeRepository<ZnodeMedia> _mediaDetails;
        protected readonly IPublishedPortalDataService publishedPortalDataService;
        #endregion

        #region Constructor

        public WebStoreWidgetService()
        {

            _publishProductService = GetService<IPublishProductService>();
            _formBuilderRepository = new ZnodeRepository<ZnodeFormBuilder>();
            _mediaDetails = new ZnodeRepository<ZnodeMedia>();
             publishedPortalDataService = GetService<IPublishedPortalDataService>();
            _formWidgetConfigurationRepository = new ZnodeRepository<ZnodeCMSFormWidgetConfiguration>();

    }

    #endregion

        #region Public Methods
        //Get slider details.
        public virtual CMSWidgetConfigurationModel GetSlider(WebStoreWidgetParameterModel parameter)

        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter WebStoreWidgetParameterModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, parameter);

            CMSWidgetConfigurationModel model = publishedPortalDataService.GetSliderBanner(CreateGenericFilter(parameter))?.ToModel<CMSWidgetConfigurationModel>();
            ZnodeLogging.LogMessage("CMSWidgetSliderBannerId value of CMSWidgetConfigurationModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, model?.CMSWidgetSliderBannerId);

            if (model?.SliderBanners?.Count > 0)
                model.SliderBanners = model.SliderBanners.Where(x => (x.ActivationDate == null || x.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate()) && (x.ExpirationDate == null || x.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate()))?.OrderBy(y => y.BannerSequence).ToList();
            ZnodeLogging.LogMessage("SliderBanners count of CMSWidgetConfigurationModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, model?.SliderBanners?.Count);

            return HelperUtility.IsNotNull(model) ? model : new CMSWidgetConfigurationModel();
        }

        //this would not be modified for the  phase 1 implementation
        //Get Product list widget with product details.
        public virtual WebStoreWidgetProductListModel GetProducts(WebStoreWidgetParameterModel parameter, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            PublishProductListModel model = new PublishProductListModel();

            List<ZnodePublishWidgetProductEntity> productListWidgetEntity = publishedPortalDataService.GetProductWidget(GetFilters(parameter));

            int? versionId = GetCatalogVersionId();

            List<ZnodePublishCategoryEntity> categoryListWidgetEntity = GetService<IPublishedCategoryDataService>().GetCategoryListByCatalogId(parameter.PublishCatalogId, parameter.LocaleId).Where(x => x.VersionId == versionId && x.IsActive).ToList();

            List<string> SKUs = productListWidgetEntity?.OrderBy(a => a.DisplayOrder).Select(x => x.SKU)?.ToList();
            foreach (ZnodePublishCategoryEntity item in categoryListWidgetEntity)
            {
                if ((item.ActivationDate == null || item.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate()) && (item.ExpirationDate == null || item.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate()))
                    parameter.CategoryIds += item.ZnodeCategoryId.ToString() + ",";
            }

            if(SKUs?.Count > 0 )
                 model = GetPublishProducts(expands, ProductListFilters(parameter, SKUs), null, null);

            ZnodeLogging.LogMessage("Count of PublishProducts list in PublishProductListModel returned from method GetPublishProducts: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, model?.PublishProducts?.Count);

            WebStoreWidgetProductListModel listModel = ToWebstoreWidgetProductListModel(model?.PublishProducts, productListWidgetEntity);

            listModel.DisplayName = parameter.DisplayName;
            ZnodeLogging.LogMessage("DisplayName property of WebStoreWidgetProductListModel", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, listModel?.DisplayName);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get publish products.
        public virtual PublishProductListModel GetPublishProducts(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
            => _publishProductService.GetPublishProductList(expands, filters, sorts, page);

        //Get link widget data.
        public virtual LinkWidgetConfigurationListModel GetLinkWidget(WebStoreWidgetParameterModel parameter)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter WebStoreWidgetParameterModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, parameter);

            List<ZnodePublishWidgetTitleEntity> model = publishedPortalDataService.GetLinkWidget(CreateGenericFilter(parameter));

            ZnodeLogging.LogMessage("WidgetTitleEntity list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, model?.Count);

            //Check validity of content pages. 
            CheckContentPages(model, parameter.ProfileId, parameter.PortalId);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            List<LinkWidgetConfigurationModel> LinkWidgetConfigurationItemsList = model?.Count > 0 ?
                                                                                  model.ToModel<LinkWidgetConfigurationModel>().OrderBy(user => user.DisplayOrder).ThenBy(user => user.Title).ToList()
                                                                                  : new List<LinkWidgetConfigurationModel>();

            return new LinkWidgetConfigurationListModel() { LinkWidgetConfigurationList = LinkWidgetConfigurationItemsList };
        }

        //Get Media Details
        public virtual CMSMediaWidgetConfigurationModel GetMediaWidgetDetails(WebStoreWidgetParameterModel parameter)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodePublishMediaWidgetEntity entity = publishedPortalDataService.GetMediaWidget(GetFilters(parameter));

            CMSMediaWidgetConfigurationModel model = new CMSMediaWidgetConfigurationModel();
            GetWidgetDetails(model, entity);
            ZnodeLogging.LogMessage("Executed ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return model;
        }


        //this would not be modified for the  phase 1 implementation
        //Get Category list widget with category details.
        public virtual WebStoreWidgetCategoryListModel GetCategories(WebStoreWidgetParameterModel parameter)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter WebStoreWidgetParameterModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, parameter);

            List<ZnodePublishWidgetCategoryEntity> categoryListWidgetEntity = publishedPortalDataService.GetCategoryWidget(GetFilters(parameter));

            ZnodeLogging.LogMessage("categoryListWidgetEntity count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, categoryListWidgetEntity?.Count);

            WebStoreWidgetCategoryListModel listModel = new WebStoreWidgetCategoryListModel();

            if (categoryListWidgetEntity?.Count > 0)
            {
                listModel.Categories = categoryListWidgetEntity?.OrderBy(x => x.DisplayOrder)?.ToModel<WebStoreWidgetCategoryModel>().ToList();

                IPublishCategoryService _publishCategoryService = GetService<IPublishCategoryService>();

                NameValueCollection expands = new NameValueCollection();
                expands.Add(ZnodeConstant.SEO, ZnodeConstant.SEO);

                //Get category data .
                PublishCategoryListModel categoryListEntity = _publishCategoryService.GetPublishCategoryList(expands, CategoryListFilters(parameter, listModel), new NameValueCollection(), null);

                //Bind PublishCategoryModel. 
                listModel.Categories.ForEach(item => item.PublishCategoryModel = categoryListEntity?.PublishCategories.FirstOrDefault(x => x.CategoryCode == item.CategoryCode));

                listModel.Categories = listModel.Categories.OrderBy(x => x.DisplayOrder).ToList();
            }
            ZnodeLogging.LogMessage("Categories list of WebStoreWidgetCategoryListModel count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, listModel?.Categories?.Count);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return listModel;
        }

        //this would not be modified for the  phase 1 implementation
        //Get list of link products.
        public virtual WebStoreLinkProductListModel GetLinkProductList(WebStoreWidgetParameterModel parameter, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter WebStoreWidgetParameterModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, parameter);

            //For getting price add pricing in expands
            expands.Add(ZnodeConstant.Pricing, ZnodeConstant.Pricing);

            List<PublishProductModel> model = GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(LinkProductListFilters(parameter),null, null))?.ToModel<PublishProductModel>()?.ToList();
            IEnumerable<PublishAttributeModel> attributeLinkProduct = GetAttributeLinkedProductsList(model);

            WebStoreLinkProductListModel webStoreLinkProductListModel = new WebStoreLinkProductListModel();
            List<WebStoreLinkProductModel> linkProductList = new List<WebStoreLinkProductModel>();

            //Map link product data.
            MapLinkProducts(parameter, attributeLinkProduct, linkProductList, expands);
            webStoreLinkProductListModel.LinkProductList = linkProductList;
            ZnodeLogging.LogMessage("LinkProductList of WebStoreLinkProductListModel count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, webStoreLinkProductListModel?.LinkProductList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return webStoreLinkProductListModel;
        }

        //get tag manager data
        public virtual CMSTextWidgetConfigurationModel GetTagManager(WebStoreWidgetParameterModel parameter)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            ZnodePublishTextWidgetEntity entity = publishedPortalDataService.GetTextWidget(CreateGenericFilter(parameter));

            ZnodeLogging.LogMessage("Executed ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return ToTextWidget(entity);
        }

        //Get brand list widget with brand details.
        public virtual WebStoreWidgetBrandListModel GetBrands(WebStoreWidgetParameterModel parameter)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter WebStoreWidgetParameterModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, parameter);

            List<ZnodePublishWidgetBrandEntity> brandwidgetentity = publishedPortalDataService.GetBrandWidget(GetFilters(parameter));
            WebStoreWidgetBrandListModel listModel = new WebStoreWidgetBrandListModel();

            if (brandwidgetentity?.Count > 0)
            {
                listModel.Brands = brandwidgetentity?.OrderBy(x => x.DisplayOrder)?.ToModel<WebStoreWidgetBrandModel>().ToList();

                IBrandService _brandService = GetService<IBrandService>();

                //Get brand data service.
                BrandListModel brandListModel = _brandService.GetBrandList(null, BrandListFilters(parameter, listModel), new NameValueCollection(), null);

                //Bind BrandModel. 
                listModel.Brands?.ForEach(item => item.BrandModel = brandListModel?.Brands.FirstOrDefault(x => x.BrandId == item.BrandId));

            }
            ZnodeLogging.LogMessage("BrandsList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, listModel?.Brands?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return listModel;
        }

        // Get Form Configuration By CMSMappingId.
        public virtual WebStoreWidgetFormParameters GetFormConfigurationByCMSMappingId(int mappingId, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters mappingId and localeId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { mappingId, localeId });

            WebStoreWidgetFormParameters formParameters = null;
            if (mappingId > 0)
            {
                formParameters = (from frm in _formBuilderRepository.Table
                                  join cms in _formWidgetConfigurationRepository.Table on frm.FormBuilderId equals cms.FormBuilderId
                                  where cms.CMSMappingId == mappingId && cms.LocaleId == localeId
                                  select new WebStoreWidgetFormParameters
                                  {
                                      LocaleId = cms.LocaleId,
                                      FormCode = frm.FormCode,
                                      CMSMappingId = mappingId,
                                      FormBuilderId = frm.FormBuilderId
                                  })?.FirstOrDefault();

            }
            ZnodeLogging.LogMessage("formParameters to be returned: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, formParameters);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return formParameters ?? new WebStoreWidgetFormParameters();
        }

        public virtual WebStoreWidgetSearchModel GetSearchWidgetData(WebStoreSearchWidgetParameterModel parameter, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter WebStoreSearchWidgetParameterModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, parameter);


            ZnodePublishSearchWidgetEntity searchWidget = publishedPortalDataService.GetSearchWidget(CreateSearchFilter(parameter));
            SearchRequestModel requestModel = parameter as SearchRequestModel;
            requestModel.Keyword = searchWidget?.SearchKeyword;
            requestModel.IsFacetList = true;
            requestModel.IsSearchWidget = true;
            KeywordSearchModel SearchResult = GetService<ISearchService>().FullTextSearch(requestModel, expands, filters, sorts, page);
            WebStoreWidgetSearchModel model = new WebStoreWidgetSearchModel();
            model.Products = SearchResult.Products;
            model.Facets = SearchResult.Facets;
            model.TotalProductCount = SearchResult.TotalProductCount;
            ZnodeLogging.LogMessage("Products list, Facets list and TotalProductCount of WebStoreWidgetSearchModel count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { ProductsListCount = model?.Products?.Count, FacetsListCount = model?.Facets?.Count, TotalProductCount = model?.TotalProductCount });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return model;
        }

        //Get Content Container details.
        public virtual string GetContainer(WebStoreWidgetParameterModel parameter)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter WebStoreWidgetParameterModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, parameter);

            string ContainerKey = publishedPortalDataService.GetContentContainer(CreateGenericFilter(parameter));
            ZnodeLogging.LogMessage("ContainerKey value of CmsContainer Widget: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, ContainerKey);

            return ContainerKey;
        }

        //tag manager mapping
        protected virtual CMSTextWidgetConfigurationModel ToTextWidget(ZnodePublishTextWidgetEntity entity)
        {
            if (!Equals(entity, null))
            {
                return new CMSTextWidgetConfigurationModel
                {
                    Text = entity.Text
                };
            }
            return new CMSTextWidgetConfigurationModel();
        }
        #endregion

        #region Protected Methods

        protected virtual FilterCollection CreateGenericFilter(WebStoreWidgetParameterModel parameter)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add("MappingId", FilterOperators.Equals, parameter.CMSMappingId.ToString());
            filters.Add("WidgetsKey", FilterOperators.Is, parameter.WidgetKey);
            filters.Add("TypeOFMapping", FilterOperators.Like, parameter.TypeOfMapping);
            filters.Add("VersionId", FilterOperators.Equals, WebstoreVersionId.ToString());
            if (parameter.LocaleId > 0)
                filters.Add("LocaleId", FilterOperators.Equals, parameter.LocaleId.ToString());

            return filters;

        }

        protected virtual FilterCollection GetFilters(WebStoreWidgetParameterModel parameter)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add("MappingId", FilterOperators.Equals, parameter.CMSMappingId.ToString());
            filters.Add("WidgetsKey", FilterOperators.Is, parameter.WidgetKey);
            filters.Add("TypeOFMapping", FilterOperators.Like, parameter.TypeOfMapping);
            filters.Add("VersionId", FilterOperators.Equals, WebstoreVersionId.ToString());
            return filters;

        }


        protected virtual FilterCollection CreateSearchFilter(WebStoreSearchWidgetParameterModel parameter)
        => new FilterCollection() {
                new FilterTuple("MappingId", FilterOperators.Equals, parameter.CMSMappingId.ToString()),
                new FilterTuple("WidgetsKey", FilterOperators.Is, parameter.WidgetKey),
                new FilterTuple("TypeOFMapping", FilterOperators.Like, parameter.TypeOfMapping),
                new FilterTuple("VersionId", FilterOperators.Equals, WebstoreVersionId.ToString()),
          };


        //Generate filters to render product list.
        protected virtual FilterCollection ProductListFilters(WebStoreWidgetParameterModel parameter, List<string> SKUs)
         {
            FilterCollection filter = new FilterCollection();
            if(SKUs?.Count > 0)
                filter.Add(new FilterTuple(WebStoreEnum.SKU.ToString(), FilterOperators.In, string.Join(",", SKUs.Select(x => $"\"{x}\""))));
            filter.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, parameter.LocaleId.ToString()));
            filter.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, parameter.PortalId.ToString()));
            filter.Add(new FilterTuple(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));
            filter.Add(FilterKeys.ProductIndex, FilterOperators.Equals, ZnodeConstant.DefaultPublishProductIndex.ToString());
            filter.Add(new FilterTuple(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, parameter.PublishCatalogId.ToString()));
            if (!string.IsNullOrEmpty(parameter.CategoryIds))
                filter.Add(new FilterTuple(FilterKeys.CategoryIds, FilterOperators.In, parameter.CategoryIds?.TrimEnd(',')));
            return filter;
        }

        //Generate filters to render category list.
        protected virtual FilterCollection CategoryListFilters(WebStoreWidgetParameterModel parameter, WebStoreWidgetCategoryListModel listModel)
        {
            FilterCollection filter = new FilterCollection();
            List<string> categoryCode = listModel.Categories.Select(x => x.CategoryCode).ToList();
            if(categoryCode?.Count > 0)
                filter.Add(new FilterTuple(WebStoreEnum.CategoryCode.ToString(), FilterOperators.In, string.Join(",", categoryCode.Select(x => $"\"{x}\""))));
            filter.Add(new FilterTuple(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, parameter.PublishCatalogId.ToString()));
            filter.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, parameter.LocaleId.ToString()));
            filter.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, parameter.PortalId.ToString()));
            filter.Add(new FilterTuple(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue));
            return filter;
        }

        //Generate filters to render brand list.
        protected virtual FilterCollection BrandListFilters(WebStoreWidgetParameterModel parameter, WebStoreWidgetBrandListModel listModel)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSWidgetBrandEnum.BrandId.ToString(), FilterOperators.In, string.Join(",", listModel.Brands.Select(x => x.BrandId))));
            filter.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, parameter.LocaleId.ToString()));
            return filter;
        }

        //Generate filters to render product list.
        protected virtual FilterCollection LinkProductListFilters(WebStoreWidgetParameterModel parameter)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.Equals, parameter.CMSMappingId.ToString()));
            filter.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, parameter.LocaleId.ToString()));
            int? versionIds = GetCatalogVersionId(Convert.ToInt32(parameter.PublishCatalogId), Convert.ToInt32(parameter.LocaleId));
            ZnodeLogging.LogMessage("Catalog versionIds returned from method GetCatalogVersionId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, versionIds);

            filter.Add(FilterKeys.VersionId, FilterOperators.Equals, versionIds.HasValue ? versionIds.Value.ToString() : "0");

            SetProductIndexFilter(filter);
            return filter;
        }

        //Filters for link products.
        protected virtual FilterCollection LinkProductFilters(WebStoreWidgetParameterModel parameter, PublishAttributeModel item)
        {
            FilterCollection filters = new FilterCollection();

            string[] sku = item?.AttributeValues?.Split(',')?.ToArray();

            if (sku?.Count() > 0)
                filters.Add(WebStoreEnum.SKU.ToString(), FilterOperators.In, string.Join(",", sku.Select(x => $"\"{x}\"")));
            else
                filters.Add(WebStoreEnum.SKU.ToString(), FilterOperators.Is, string.Empty);

            filters.Add(FilterKeys.LocaleId, FilterOperators.Equals, parameter.LocaleId.ToString());
            filters.Add(FilterKeys.PortalId, FilterOperators.Equals, parameter.PortalId.ToString());
            filters.Add(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, Convert.ToString(parameter.PublishCatalogId));
            return filters;
        }

        //Map link product data.
        public virtual void MapLinkProducts(WebStoreWidgetParameterModel parameter, IEnumerable<PublishAttributeModel> attributeLinkProduct, List<WebStoreLinkProductModel> linkProductList, NameValueCollection expands)
        {
            IZnodeRepository<ZnodePimAttributeLocale> _znodePimAttributeLocaleRepository = new ZnodeRepository<ZnodePimAttributeLocale>();
            IZnodeRepository<ZnodePimLinkProductDetail> _linkDetailRepository = new ZnodeRepository<ZnodePimLinkProductDetail>();
            foreach (var item in attributeLinkProduct)
            {
                if (!string.IsNullOrEmpty(item.AttributeValues))
                {
                    WebStoreLinkProductModel data = new WebStoreLinkProductModel();

                    PublishProductListModel publishProductListModel = GetPublishProducts(expands, LinkProductFilters(parameter, item), null, null);
                    data.AttributeName = item.AttributeName;
                    //Code  is done for showing display order

                    List<WebstorePublishAssociatedProductModel> pimLinkProductDetail = (from publishAssociatedProduct in _linkDetailRepository.Table
                                                                                        where publishAssociatedProduct.PimParentProductId == parameter.CMSMappingId 
                                                                                        select new WebstorePublishAssociatedProductModel
                                                                                        {
                                                                                            DisplayOrder = publishAssociatedProduct.DisplayOrder,
                                                                                            PimProductId = publishAssociatedProduct.PimProductId,
                                                                                            PimParentProductId = publishAssociatedProduct.PimParentProductId,
                                                                                        })?.ToList();


                    publishProductListModel.PublishProducts.ForEach(product =>
                    {
                        int? ParentProductId = pimLinkProductDetail.Where(a => a.PimProductId == product.PublishProductId).FirstOrDefault()?.PimParentProductId;
                        if ( !string.IsNullOrEmpty(item.AttributeName) && HelperUtility.IsNotNull(ParentProductId))
                        {

                            product.DisplayOrder = (from Locale in _znodePimAttributeLocaleRepository.Table
                                                    join LinkDetail in _linkDetailRepository.Table on Locale.PimAttributeId equals LinkDetail.PimAttributeId
                                                    where Locale.AttributeName.Equals(item.AttributeName, StringComparison.OrdinalIgnoreCase) &&
                                                    LinkDetail.PimProductId == product.PublishProductId && LinkDetail.PimParentProductId == ParentProductId
                                                    orderby LinkDetail.PimLinkProductDetailId
                                                    select (LinkDetail.DisplayOrder))?.FirstOrDefault();
                        }
                    });

                    data.PublishProduct.AddRange(publishProductListModel.PublishProducts.OrderBy(x => x.DisplayOrder));
                    linkProductList.Add(data);
                }
            }
        }

        //Mapping data from List<PublishProductModel> model to widget product model.
        protected virtual WebStoreWidgetProductListModel ToWebstoreWidgetProductListModel(List<PublishProductModel> model, List<ZnodePublishWidgetProductEntity> productListWidgetEntity)
        {
            WebStoreWidgetProductListModel webStoreWidgetProductListModel = new WebStoreWidgetProductListModel();
            webStoreWidgetProductListModel.Products = new List<WebStoreWidgetProductModel>();

            if (HelperUtility.IsNotNull(model) && model?.Count > 0)
            {
                foreach (PublishProductModel data in model)
                {
                    WebStoreWidgetProductModel webStoreWidgetProductModel = new WebStoreWidgetProductModel();
                    webStoreWidgetProductModel.WebStoreProductModel = MapData(data, productListWidgetEntity);
                    webStoreWidgetProductListModel.Products.Add(webStoreWidgetProductModel);

                }
            }
            ZnodeLogging.LogMessage("webStoreWidgetProductListModel list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, webStoreWidgetProductListModel?.Products?.Count);
            webStoreWidgetProductListModel.Products = webStoreWidgetProductListModel.Products?.OrderBy(x => x.WebStoreProductModel.DisplyOrder).ToList();
            return webStoreWidgetProductListModel;
        }

        //Map PublishProductModel to WebStoreProductModel.
        protected virtual WebStoreProductModel MapData(PublishProductModel model, List<ZnodePublishWidgetProductEntity> productListWidgetEntity)
        {
            return new WebStoreProductModel
            {
                Attributes = model.Attributes,
                LocaleId = model.LocaleId,
                CatalogId = model.PublishedCatalogId,
                PublishProductId = model.PublishProductId,
                Name = model.Name,
                RetailPrice = model.RetailPrice,
                SalesPrice = model.SalesPrice,
                PromotionalPrice = model.PromotionalPrice,
                SKU = model.SKU,
                ProductReviews = model.ProductReviews,
                CurrencyCode = model.CurrencyCode,
                CultureCode = model.CultureCode,
                ImageMediumPath = model.ImageMediumPath,
                ImageSmallPath = model.ImageSmallPath,
                ImageSmallThumbnailPath = model.ImageSmallThumbnailPath,
                ImageThumbNailPath = model.ImageThumbNailPath,
                ImageLargePath = model.ImageLargePath,
                SEODescription = model.SEODescription,
                SEOTitle = model.SEOTitle,
                Rating = model.Rating,
                TotalReviews = model.TotalReviews,
                SEOUrl = model.SEOUrl,
                SEOKeywords = model.SEOKeywords,
                GroupProductPriceMessage = model.GroupProductPriceMessage,
                Promotions = model.Promotions,
                DisplyOrder = productListWidgetEntity?.FirstOrDefault(x => x?.SKU == model?.SKU)?.DisplayOrder.GetValueOrDefault(),
                ProductType = model.ProductType
            };
        }

        //Get details of Widget
        protected virtual void GetWidgetDetails(CMSMediaWidgetConfigurationModel model, ZnodePublishMediaWidgetEntity entity)
        {
            if (!Equals(entity, null))
            {
                model.MediaPath = GetWidgetImagePath(entity.PortalId, entity.MediaPath);
                if (!string.IsNullOrEmpty(model.MediaPath))
                {
                    model.DisplayName = _mediaDetails.Table.FirstOrDefault(x => x.Path == entity.MediaPath).FileName;
                }
            }

        }

        //Get Image Path
        protected virtual string GetWidgetImagePath(int portalId, string imageName)
        {
            //Get Product Image Path
            if (portalId > 0)
            {
                IImageHelper image = GetService<IImageHelper>();
                return image.GetOriginalImagepath(imageName);
            }

            return string.Empty;
        }



        //Get Associated configurable Product.
        protected virtual PublishProductModel GetAssociatedConfigurableProduct(int productId, int localeId, int? catalogVersionId)
        {
            IPublishProductHelper publishProductHelper = GetService<IPublishProductHelper>();
            //Get configurable product entity.
            List<PublishedConfigurableProductEntityModel> configEntity = publishProductHelper.GetConfigurableProductEntity(productId, catalogVersionId);

            //Get associated product list.
            List<PublishProductModel> associatedProducts = publishProductHelper.GetAssociatedProducts(productId, localeId, catalogVersionId, configEntity);
            if (associatedProducts?.Count > 0)
            {
                //Get first product from list of associated products 
                PublishProductModel publishProduct = associatedProducts.FirstOrDefault();
                IPublishProductHelper helper = GetService<IPublishProductHelper>();
                //Get expands associated to Product.
                helper.GetProductPriceData(publishProduct, PortalId, GetLoginUserId(), GetProfileId());
                return publishProduct;
            }
            return new PublishProductModel();
        }

        //Map Price and Inventory of group products.
        protected virtual void MapPriceAndInventory(List<WebStoreGroupProductModel> groupProducts, List<PriceSKUModel> priceList)
        {
            if (priceList?.Count > 0)
            {
                groupProducts.ForEach(product =>
                {
                    PriceSKUModel productSKU = priceList
                                .Where(productdata => productdata.SKU == product.SKU)
                                ?.FirstOrDefault();
                    if (HelperUtility.IsNotNull(productSKU))
                    {
                        product.SalesPrice = productSKU.SalesPrice;
                        product.RetailPrice = productSKU.RetailPrice;
                        product.CurrencyCode = productSKU.CurrencyCode;
                        product.CultureCode = productSKU.CultureCode;
                        product.CurrencySuffix = productSKU.CurrencySuffix;
                    }
                });
            }
        }

        //Check validity of content pages.
        protected virtual void CheckContentPages(List<ZnodePublishWidgetTitleEntity> pages, int? profileId, int portalId)
        {
            ZnodeLogging.LogObject(pages.GetType(), pages, "Cache Issue");

            IZnodeRepository<ZnodeCMSSEOType> _seoType = new ZnodeRepository<ZnodeCMSSEOType>();
            IZnodeRepository<ZnodeCMSSEODetail> _seoDetail = new ZnodeRepository<ZnodeCMSSEODetail>();
            IZnodeRepository<ZnodePublishContentPageConfigEntity> _publishcontentpageconfigentity = new ZnodeRepository<ZnodePublishContentPageConfigEntity>(HelperMethods.Context);

            pages.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.Url) && x.Url.StartsWith("/") && (x.Url.Count(y => y == '/') == 1))
                {
                    //Get the seo name from url removing the '/'.
                    string _seoName = x.Url.Substring(1);
                    if (!_seoName.Contains('/'))
                    {

                        string seoCode = (from seoDetail in _seoDetail.Table
                                          join seoType in _seoType.Table on seoDetail.CMSSEOTypeId equals seoType.CMSSEOTypeId
                                          where seoType.Name.ToLower() == ZnodeConstant.ContentPage.ToLower() && seoDetail.PortalId == portalId && seoDetail.SEOUrl == _seoName
                                          select seoDetail.SEOCode)?.FirstOrDefault();
                        ZnodeLogging.LogMessage("seoCode generated: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, seoCode);


                        ZnodePublishContentPageConfigEntity contentPageEntity = _publishcontentpageconfigentity.Table.FirstOrDefault(a => a.PageName == seoCode && a.IsActive);

                        if (HelperUtility.IsNotNull(contentPageEntity))
                        {
                            if ((HelperUtility.IsNull(contentPageEntity.ActivationDate) || contentPageEntity.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate())
                                && (HelperUtility.IsNull(contentPageEntity.ExpirationDate) || (contentPageEntity.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate()))
                                && (HelperUtility.IsNull(contentPageEntity.ProfileId) || contentPageEntity.ProfileId.Contains(profileId.ToString())) && contentPageEntity.IsActive)
                                x.IsActive = true;
                        }
                    }
                }
                else
                    x.IsActive = true;
            });
        }

        //Get associated attribute linked products list for products 
        protected virtual IEnumerable<PublishAttributeModel> GetAttributeLinkedProductsList(List<PublishProductModel> model)
        {
            IEnumerable<PublishAttributeModel> attributeLinkProduct = new List<PublishAttributeModel>();

            if(model.Any(x => Convert.ToBoolean(x.Attributes?.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.IsObsolete)?.AttributeValues)))
            {
                if(model.Any(x=>x.Attributes.FirstOrDefault(y=>y.AttributeCode == ZnodeConstant.ProductType).SelectValues.Any(z=>z.Code == ZnodeConstant.BundleProduct)))
                {
                    attributeLinkProduct = model.SelectMany(x => x.Attributes.Where(y => y.AttributeTypeName == ZnodeConstant.Link))?.DistinctBy(e => e.AttributeName);
                }
                else
                    attributeLinkProduct = model.SelectMany(x => x.Attributes.Where(y => y.AttributeTypeName == ZnodeConstant.Link && y.AttributeCode == ZnodeConstant.ProductSuggestions))?.DistinctBy(e => e.AttributeName);
            }
            else
            {
                attributeLinkProduct = model.SelectMany(x => x.Attributes.Where(y => y.AttributeTypeName == ZnodeConstant.Link && y.AttributeCode != ZnodeConstant.ProductSuggestions))?.DistinctBy(e => e.AttributeName);
            }
            return attributeLinkProduct;
        }
        #endregion
    }
}