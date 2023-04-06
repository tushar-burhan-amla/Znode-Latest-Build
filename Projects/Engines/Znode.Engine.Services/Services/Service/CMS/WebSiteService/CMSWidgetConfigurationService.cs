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
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class CMSWidgetConfigurationService : BaseService, ICMSWidgetConfigurationService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeCMSWidgetTitleConfiguration> _cmsWidgetTitleConfiguration;
        private readonly IZnodeRepository<ZnodeCMSWidgetTitleConfigurationLocale> _cmsWidgetTitleConfigurationLocale;
        private readonly IZnodeRepository<ZnodeCMSTextWidgetConfiguration> _textWidgetRepository;
        private readonly IZnodeRepository<ZnodeCMSMediaConfiguration> _mediaWidgetRepository;
        private readonly IZnodeRepository<ZnodeCMSWidgetSliderBanner> _cmsWidgetSliderBanner;
        private readonly IZnodeRepository<ZnodeCMSWidgetProduct> _cmsWidgetProductRepository;
        private readonly IZnodeRepository<ZnodeCMSWidgetCategory> _cmsWidgetCategory;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IZnodeRepository<ZnodeCMSWidgetBrand> _cmsWidgetbrand;
        private readonly IZnodeRepository<ZnodeCMSContentPagesLocale> _contentPageLocale;
        private readonly IZnodeRepository<ZnodeCMSFormWidgetConfiguration> _formWidgetRepository;
        private readonly IZnodeRepository<ZnodeFormWidgetEmailConfiguration> _formWidgetEmailRepository;
        private readonly IZnodeRepository<ZnodeCMSSearchWidget> _searchWidgetRepository;
        private readonly IZnodeRepository<ZnodeCMSContainerConfiguration> _containerWidgetRepository;
        #endregion

        #region Constructor
        public CMSWidgetConfigurationService()
        {
            _cmsWidgetTitleConfiguration = new ZnodeRepository<ZnodeCMSWidgetTitleConfiguration>();
            _cmsWidgetTitleConfigurationLocale = new ZnodeRepository<ZnodeCMSWidgetTitleConfigurationLocale>();
            _textWidgetRepository = new ZnodeRepository<ZnodeCMSTextWidgetConfiguration>();
            _mediaWidgetRepository = new ZnodeRepository<ZnodeCMSMediaConfiguration>();
            _cmsWidgetSliderBanner = new ZnodeRepository<ZnodeCMSWidgetSliderBanner>();
            _cmsWidgetProductRepository = new ZnodeRepository<ZnodeCMSWidgetProduct>();
            _cmsWidgetCategory = new ZnodeRepository<ZnodeCMSWidgetCategory>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _cmsWidgetbrand = new ZnodeRepository<ZnodeCMSWidgetBrand>();
            _contentPageLocale = new ZnodeRepository<ZnodeCMSContentPagesLocale>();
            _formWidgetRepository = new ZnodeRepository<ZnodeCMSFormWidgetConfiguration>();
            _formWidgetEmailRepository = new ZnodeRepository<ZnodeFormWidgetEmailConfiguration>();
            _searchWidgetRepository = new ZnodeRepository<ZnodeCMSSearchWidget>();
            _containerWidgetRepository = new ZnodeRepository<ZnodeCMSContainerConfiguration>();
        }
        #endregion

        #region Public Methods

        #region CMSWidgetProduct
        //Get associated product list.
        public virtual CMSWidgetProductListModel GetAssociatedProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Rename sort key to retrieve sorted data
            ReplaceSortKeys(ref sorts);

            FilterCollection oldFilter = new FilterCollection();
            oldFilter.AddRange(filters);

            FilterCollection productFilter = new FilterCollection();
            productFilter.AddRange(filters);

            //Method to remove filter to get widget product list.
            PageListModel pageListModel = RemoveFilterToGetWidgetList(sorts, page, oldFilter);

            //Get associated product list.
            List<CMSWidgetProductModel> cmsWidgetPublishProducts = _cmsWidgetProductRepository.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues)?.ToModel<CMSWidgetProductModel>()?.ToList();
            ZnodeLogging.LogMessage("cmsWidgetPublishProducts list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsWidgetPublishProducts?.Count());

            string localeId, portalId;
            //Get value of portal id and locale id form filter.
            GetPortalIdAndLocaleId(productFilter, out portalId, out localeId);

            //Get catalog id from catalog associated to store.
            int? catalogId = GetCatalogID(portalId);
            ZnodeLogging.LogMessage("catalogId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, catalogId);
            //Get product ids.
            List<string> SKUs = cmsWidgetPublishProducts?.OrderBy(a=>a.DisplayOrder).Select(x => x.SKU)?.ToList();
            if (SKUs?.Count > 0)
                productFilter.Add(FilterKeys.SKU, FilterOperators.In, string.Join(",", SKUs.Select(x => $"\"{x}\"")));

            
            productFilter.Add(FilterKeys.VersionId, FilterOperators.Equals, Convert.ToString(GetCatalogVersionId(catalogId.HasValue ? catalogId.Value : 0)));

            //Check if products are taken for some specific category.
            SetProductIndexFilter(productFilter);

            //Get Product list

            List<ZnodePublishProductEntity> data = SKUs?.Count > 0 ? GetService<IPublishedProductDataService>().GetPublishProductsPageList(RemoveFilterToGetList(sorts, page, ref productFilter), out pageListModel.TotalRowCount) : new List<ZnodePublishProductEntity>();
         
            CMSWidgetProductListModel listModel = new CMSWidgetProductListModel();

            //Map properties to CMSWidgetProductCategoryModel
            List<CMSWidgetProductCategoryModel> cmsWidgetProductCategoryListModel = MapParametersForProduct(cmsWidgetPublishProducts, data?.ToModel<PublishProductModel>()?.ToList());
            ZnodeLogging.LogMessage("List counts:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose,new { productIdsCount = SKUs?.Count, ProductEntityCount =data?.Count, cmsWidgetProductCategoryListModelCount = cmsWidgetProductCategoryListModel?.Count });
            listModel.CMSWidgetProductCategories = cmsWidgetProductCategoryListModel;

            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get unassociated product list.
        public virtual ProductDetailsListModel GetUnAssociatedProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
           
            //Rename sort key to retrieve sorted data
            ReplaceSortKeys(ref sorts);

            FilterCollection productFilter = new FilterCollection();
            productFilter.AddRange(filters);
            
            if(HelperUtility.IsNotNull(filters))
                filters.RemoveAll(x => x.FilterName == FilterKeys.LocaleId.ToLower());

            //Method to remove filter to get widget list
            PageListModel pageListModel = RemoveFilterToGetWidgetList(sorts, page, filters);
            ZnodeLogging.LogMessage("pageListModel to get cmsWidgetPublishProductIds list ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Method to get publish product ids.
            List<string> SKUs = _cmsWidgetProductRepository.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.EntityWhereClause.FilterValues)?.Select(x => x.SKU)?.ToList();
            ZnodeLogging.LogMessage("cmsWidgetPublishProductIds list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, SKUs?.Count);
            if (SKUs?.Count > 0)
                productFilter.Add(FilterKeys.SKU, FilterOperators.NotIn, string.Join(",", SKUs.Select(x => $"\"{x}\"")));

            productFilter.Add(FilterKeys.ZnodeCategoryIds, FilterOperators.NotEquals, "0");
            string localeId, portalId;

            //Get value of portal id and locale id form filter.
            GetPortalIdAndLocaleId(productFilter, out portalId, out localeId);

            //Get portal id.
            //Get catalog id from catalog associated to store.
            ZnodeLogging.LogMessage("portalId to get catalogId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, portalId);
            int? catalogId = GetCatalogID(portalId);
            ZnodeLogging.LogMessage("catalogId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, catalogId);
            //Add Catalog id to filter.
            productFilter.Add(FilterKeys.ZnodeCatalogId, FilterOperators.Equals, catalogId.ToString());

            productFilter.Add(FilterKeys.VersionId, FilterOperators.Equals, Convert.ToString(GetCatalogVersionId(catalogId.HasValue ? catalogId.Value : 0)));
        
            //Check if products are taken for some specific category.
            SetProductIndexFilter(productFilter);

            //Method to get unassociated product list.
            List<ZnodePublishProductEntity> data =  GetService<IPublishedProductDataService>().GetPublishProductsPageList(RemoveFilterToGetList(sorts, page, ref productFilter), out pageListModel.TotalRowCount);

            ZnodeLogging.LogMessage("ProductEntity list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, data?.Count);
            ProductDetailsListModel listModel = new ProductDetailsListModel();
            listModel.ProductDetailList = data?.Count > 0 ? data.ToModel<ProductDetailsModel>()?.ToList() : new List<ProductDetailsModel>();

            //Maps image path for unassociated product.
            MapParameterForImagePath(listModel.ProductDetailList, data?.ToModel<PublishProductModel>()?.ToList());

            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Associate product .
        public virtual bool AssociateProduct(CMSWidgetProductListModel cmsWidgetProductListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(cmsWidgetProductListModel) || IsNull(cmsWidgetProductListModel.CMSWidgetProducts))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            if (cmsWidgetProductListModel.CMSWidgetProducts.Count < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCMSWidgetProductsCountLessThanZero);
            
            IEnumerable<ZnodeCMSWidgetProduct> associateProduct = _cmsWidgetProductRepository.Insert(cmsWidgetProductListModel.CMSWidgetProducts.ToEntity<ZnodeCMSWidgetProduct>()?.ToList());

            if (cmsWidgetProductListModel.CMSWidgetProducts.FirstOrDefault().TypeOfMapping.ToLower() == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString().ToLower() && associateProduct?.Count() > 0)
            {
                int cmsMappingId = cmsWidgetProductListModel?.CMSWidgetProducts?.FirstOrDefault().CMSMappingId ?? 0;
                if (cmsMappingId > 0)
                {
                    PublishContentPage(GetCMSTypeMappingModel(cmsMappingId, cmsWidgetProductListModel.LocaleId, cmsWidgetProductListModel.EnableCMSPreview));
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return associateProduct?.Count() > 0;
        }

        //Unassociate associated products.
        public virtual bool UnassociateProduct(ParameterModel cmsWidgetProductId)
        {
            bool isDeleted = false;
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(cmsWidgetProductId?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCMSWidgetProductIdLessThanOne);

            ZnodeCMSWidgetProduct cmsProductWidget = GetProductWidget(cmsWidgetProductId.Ids);

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSWidgetProductEnum.CMSWidgetProductId.ToString(), ProcedureFilterOperators.In, cmsWidgetProductId.Ids));

            ZnodeLogging.LogMessage("CMS widget product with Ids to be deleted: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsWidgetProductId?.Ids);
            isDeleted = _cmsWidgetProductRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);

            if (IsNotNull(cmsProductWidget) && isDeleted && (String.Equals(cmsProductWidget.TypeOFMapping, ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString(), StringComparison.InvariantCultureIgnoreCase)))
            {
                PublishContentPage(GetCMSTypeMappingModel(cmsProductWidget.CMSMappingId, cmsWidgetProductId.LocaleId, cmsWidgetProductId.EnableCMSPreview));
            }
            return isDeleted;

        }

        //Update CMS Widget Product.
        public virtual bool UpdateCMSAssociateProduct(ProductDetailsModel productDetailsModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            bool isUpdated = false;
            if (HelperUtility.IsNull(productDetailsModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            ZnodeCMSWidgetProduct znodeCMSWidgetProduct = _cmsWidgetProductRepository.Table.Where(x => x.CMSWidgetProductId == productDetailsModel.CMSWidgetProductId)?.FirstOrDefault();

            //Assign value to DisplayOrder.
            if (IsNotNull(znodeCMSWidgetProduct))
            {
                znodeCMSWidgetProduct.ModifiedDate = DateTime.Now;
                znodeCMSWidgetProduct.DisplayOrder = productDetailsModel.DisplayOrder;
                ZnodeLogging.LogMessage("CMSWidgetCategory with Id to be updated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, znodeCMSWidgetProduct?.CMSWidgetProductId);
                isUpdated = _cmsWidgetProductRepository.Update(znodeCMSWidgetProduct);
                ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.UpdateMessage : Admin_Resources.ErrorFailedToUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            return isUpdated;
        }

        //get product widget details
        protected virtual ZnodeCMSWidgetProduct GetProductWidget(string widgetProductIds)
        {
            int widgetProductId = 0;
            if (widgetProductIds?.Split(',')?.Length > 0)
            {
                int.TryParse(widgetProductIds?.Split(',')[0], out widgetProductId);
            }
            ZnodeCMSWidgetProduct cmsProductWidget = _cmsWidgetProductRepository.Table.FirstOrDefault(m => m.CMSWidgetProductId == widgetProductId);
            return cmsProductWidget;
        }

        #endregion

        #region CMS Widget Slider Banner
        //Get the CMS Widget Slider Banner Details.
        public virtual CMSWidgetConfigurationModel GetCMSWidgetSliderBanner(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause to get cmsWidgetSliderBanner:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClause?.WhereClause);
            CMSWidgetConfigurationModel cmsWidgetSliderBanner = _cmsWidgetSliderBanner.GetEntity(whereClause.WhereClause, whereClause.FilterValues).ToModel<CMSWidgetConfigurationModel>();
            return IsNull(cmsWidgetSliderBanner) ? new CMSWidgetConfigurationModel() : cmsWidgetSliderBanner;
        }

        //Save New CMS Widget Slider Banner Details.
        public virtual bool SaveCMSWidgetSliderBanner(CMSWidgetConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            bool isSaved;

            if (model.CMSWidgetSliderBannerId > 0)
            {
                //Update CMS Widget Slider Banner Details.
                isSaved = _cmsWidgetSliderBanner.Update(model.ToEntity<ZnodeCMSWidgetSliderBanner>());
                ZnodeLogging.LogMessage(isSaved ? string.Format(Admin_Resources.SuccessCMSWidgetSliderBannerConfigurationUpdate, model.CMSWidgetSliderBannerId) : Admin_Resources.ErrorCMSWidgetSliderBannerConfigurationUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            else
            {
                ZnodeCMSWidgetSliderBanner entity = _cmsWidgetSliderBanner.Insert(model.ToEntity<ZnodeCMSWidgetSliderBanner>());
                ZnodeLogging.LogMessage((entity?.CMSWidgetSliderBannerId > 0) ? Admin_Resources.ErrorCMSWidgetSliderBannerConfigurationInsert : Admin_Resources.SuccessCMSWidgetSliderBannerConfigurationInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                isSaved = IsNotNull(entity);
            }
            if (model.TypeOFMapping.ToLower() == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString().ToLower() && isSaved)
            {
                PublishContentPage(GetCMSTypeMappingModel(model.CMSMappingId, model.LocaleId, model.EnableCMSPreview));
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isSaved;
        }
        #endregion

        #region Link Widget Configuration
        //Create Link Widget Configuration.
        public virtual LinkWidgetConfigurationModel CreateUpdateLinkWidgetConfiguration(LinkWidgetConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            int widgetTitleConfigurationId = 0;
            bool isSaved = false;

            if (_cmsWidgetTitleConfigurationLocale.Table.Any(x => x.CMSWidgetTitleConfigurationId == model.CMSWidgetTitleConfigurationId && x.LocaleId == model.LocaleId))
            {
                isSaved = _cmsWidgetTitleConfigurationLocale.Update(model.ToEntity<ZnodeCMSWidgetTitleConfigurationLocale>());
                widgetTitleConfigurationId = model.CMSWidgetTitleConfigurationId;
            }
            else
            {
                ZnodeCMSWidgetTitleConfiguration linkWidgetConfiguration = new ZnodeCMSWidgetTitleConfiguration();
                if (IsNull(model.TitleCode))
                {
                    model.TitleCode = model.Title;
                    linkWidgetConfiguration = _cmsWidgetTitleConfiguration.Insert(model.ToEntity<ZnodeCMSWidgetTitleConfiguration>());
                    model.CMSWidgetTitleConfigurationId = linkWidgetConfiguration.CMSWidgetTitleConfigurationId;
                    ZnodeLogging.LogMessage((linkWidgetConfiguration?.CMSWidgetTitleConfigurationId > 0) ? Admin_Resources.SuccessLinkWidgetInsert : Admin_Resources.ErrorLinkWidgetInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                }
                if (IsNotNull(linkWidgetConfiguration))
                {
                    linkWidgetConfiguration = model?.ToEntity<ZnodeCMSWidgetTitleConfiguration>();
                    widgetTitleConfigurationId = linkWidgetConfiguration.CMSWidgetTitleConfigurationId;
                    model.CMSWidgetTitleConfigurationId = linkWidgetConfiguration.CMSWidgetTitleConfigurationId;
                    //Insert record in ZnodeCMSWidgetTitleConfigurationLocale table
                    ZnodeCMSWidgetTitleConfigurationLocale linkWidgetConfigurationLocale = _cmsWidgetTitleConfigurationLocale.Insert(model.ToEntity<ZnodeCMSWidgetTitleConfigurationLocale>());
                    ZnodeLogging.LogMessage((linkWidgetConfigurationLocale?.CMSWidgetTitleConfigurationLocaleId > 0) ? Admin_Resources.SuccessLinkWidgetLocaleInsert : Admin_Resources.ErrorLinkWidgetLocaleInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

                    isSaved = true;
                }
            }
            
            if (isSaved && widgetTitleConfigurationId > 0)
            {
                if (model.TypeOFMapping.ToLower() == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString().ToLower())
                {
                    PublishContentPage(GetCMSTypeMappingModel(model.CMSMappingId, model.LocaleId, model.EnableCMSPreview));
                }
                ZnodeLogging.LogMessage("widgetTitleConfigurationId to get LinkWidgetConfigurationModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, widgetTitleConfigurationId);
                return _cmsWidgetTitleConfiguration.Table.FirstOrDefault(x => x.CMSWidgetTitleConfigurationId == widgetTitleConfigurationId)?.ToModel<LinkWidgetConfigurationModel>();
            }
            return null;
        }

        //Get Link Widget Configuration List
        public virtual LinkWidgetConfigurationListModel LinkWidgetConfigurationList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Method to get locale id from filters.
            int localeId = 0;
            GetLocaleId(filters, ref localeId);

            filters.RemoveAll(x => x.FilterName == FilterKeys.LocaleId);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("pageListModel and localeId to get LinkWidgetConfigurationListModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), localeId });
            LinkWidgetConfigurationListModel list = GetLinkWidgetConfigurationList(pageListModel, localeId);

            if (list?.LinkWidgetConfigurationList?.Count < 1 && localeId != GetDefaultLocaleId())
            {
                filters.Add(new FilterTuple(ZnodeCMSWidgetTitleConfigurationLocaleEnum.LocaleId.ToString(), ProcedureFilterOperators.Equals, GetDefaultLocaleId().ToString()));
                pageListModel = new PageListModel(filters, sorts, page);
                list = GetLinkWidgetConfigurationList(pageListModel, localeId);
            }

            LinkWidgetConfigurationListModel linkWidgetConfigurationList = new LinkWidgetConfigurationListModel { LinkWidgetConfigurationList = list?.LinkWidgetConfigurationList?.ToList() };
            linkWidgetConfigurationList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return linkWidgetConfigurationList;
        }

        private static LinkWidgetConfigurationListModel GetLinkWidgetConfigurationList(PageListModel pageListModel, int localeId)
        {
            IZnodeViewRepository<LinkWidgetConfigurationModel> objStoredProc = new ZnodeViewRepository<LinkWidgetConfigurationModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);

            List<LinkWidgetConfigurationModel> list;
            ZnodeLogging.LogMessage("pageListModel and localeId to get LinkWidgetConfiguration list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), localeId });
            list = objStoredProc.ExecuteStoredProcedureList("Znode_GetCMSWidgetsConfigurationList  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount).ToList();

            LinkWidgetConfigurationListModel linkWidgetList = new LinkWidgetConfigurationListModel();
            if (list?.Count > 0)
                linkWidgetList.LinkWidgetConfigurationList = list;
            ZnodeLogging.LogMessage("LinkWidgetConfigurationList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, linkWidgetList?.LinkWidgetConfigurationList?.Count);
            return linkWidgetList;
        }

        //Method to get locale id from filters.
        private void GetLocaleId(FilterCollection filters, ref int localeId)
        {
            if (filters?.Count > 0)
            {
                localeId = Convert.ToInt32(filters.FirstOrDefault(x => x.FilterName == FilterKeys.LocaleId.ToLower()).FilterValue);
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower());
            }
        }

        //Delete Link Widget Configuration.
        public virtual bool DeleteLinkWidgetConfiguration(ParameterModel parameterModel, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (parameterModel?.Ids?.Count() <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIdLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCMSWidgetTitleConfigurationLocaleEnum.CMSWidgetTitleConfigurationId.ToString(), ProcedureFilterOperators.In, parameterModel.Ids.ToString()));
            filters.Add(new FilterTuple(ZnodeCMSWidgetTitleConfigurationLocaleEnum.LocaleId.ToString(), ProcedureFilterOperators.Equals, localeId.ToString()));

            ZnodeLogging.LogMessage("parameterModel with Ids and localeId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { parameterModel.Ids.ToString(), localeId });
            return _cmsWidgetTitleConfigurationLocale.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }
        #endregion

        #region Text Widget Configuration

        //Get List of Text Widget Configuration
        public virtual CMSTextWidgetConfigurationListModel GetTextWidgetConfigurationList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            //maps the entity list to model
            IList<ZnodeCMSTextWidgetConfiguration> widgetList = _textWidgetRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            
            if (widgetList.Count < 1)
            {
                //If LocaleId is already present in filters, remove it.
                filters.RemoveAll(x => x.Item1 == FilterKeys.LocaleId);
                //Add New LocaleId Into filters.
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetDefaultLocaleId().ToString()));
                //Bind the Filter, sorts & Paging details.
                pageListModel = new PageListModel(filters, sorts, page);
                widgetList = _textWidgetRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            }

            ZnodeLogging.LogMessage("pageListModel to get CMSTextWidgetConfiguration list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            ZnodeLogging.LogMessage("CMSTextWidgetConfiguration list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, widgetList?.Count);

            CMSTextWidgetConfigurationListModel listModel = new CMSTextWidgetConfigurationListModel();
            listModel.TextWidgetConfigurationList = Equals(widgetList, null) ? new List<CMSTextWidgetConfigurationModel>() : widgetList?.ToModel<CMSTextWidgetConfigurationModel>().ToList();

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get Text Widget Configuration by Widget Configuration id.
        public virtual CMSTextWidgetConfigurationModel GetTextWidgetConfiguration(int textWidgetConfigurationId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (textWidgetConfigurationId > 0)
            {
                //Get Widget Configuration based on Configuration Id.
                ZnodeLogging.LogMessage("textWidgetConfigurationId to get text widget configuration: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, textWidgetConfigurationId);
                ZnodeCMSTextWidgetConfiguration entity = _textWidgetRepository.Table.FirstOrDefault(x => x.CMSTextWidgetConfigurationId == textWidgetConfigurationId);
                return entity.ToModel<CMSTextWidgetConfigurationModel>();
            }
            return null;
        }

        //Create Text Widget Configuration.
        public virtual CMSTextWidgetConfigurationModel CreateTextWidgetConfiguration(CMSTextWidgetConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelNull);

            //Call Repository to Save the Entity.
            ZnodeCMSTextWidgetConfiguration entity = _textWidgetRepository.Insert(model.ToEntity<ZnodeCMSTextWidgetConfiguration>());
            ZnodeLogging.LogMessage((entity?.CMSTextWidgetConfigurationId > 0) ? Admin_Resources.ErrorTextWidgetConfigurationInsert : Admin_Resources.SuccessTextWidgetConfigurationInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNotNull(entity))
            {
                CMSTypeMappingModel typeModel = GetCMSTypeMappingModel(entity.CMSMappingId, entity.LocaleId, model.EnableCMSPreview);
                var clearCacheInitializer = new ZnodeEventNotifier<CMSTypeMappingModel>(typeModel);
                return entity.ToModel<CMSTextWidgetConfigurationModel>();
            }

            return model;
        }

        //Save and Update Media Details
        public virtual CMSMediaWidgetConfigurationModel SaveAndUpdateMediawidgetConfiguration(CMSMediaWidgetConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelNull);

            int? CMSMediaConfigurationId;
            bool status = false;

            CMSMediaConfigurationId = _mediaWidgetRepository.Table.FirstOrDefault(x => x.CMSMappingId == model.CMSMappingId && x.WidgetsKey == model.WidgetsKey)?.CMSMediaConfigurationId;

            model.CMSMediaConfigurationId = CMSMediaConfigurationId.GetValueOrDefault();

            if (model.CMSMediaConfigurationId > 0)
            {
                status = _mediaWidgetRepository.Update(model.ToEntity<ZnodeCMSMediaConfiguration>());

                if (status && IsNotNull(model))
                {
                    CMSTypeMappingModel typeModel = GetCMSTypeMappingModel(model.CMSMappingId, 0, model.EnableCMSPreview);
                    var clearCacheInitializer = new ZnodeEventNotifier<CMSTypeMappingModel>(typeModel);
                    return model;
                }
                return null; 
            }
            else
            {
                ZnodeCMSMediaConfiguration insert = _mediaWidgetRepository.Insert(model.ToEntity<ZnodeCMSMediaConfiguration>());
                if (IsNotNull(insert))
                {
                    CMSTypeMappingModel typeModel = GetCMSTypeMappingModel(model.CMSMappingId, 0, model.EnableCMSPreview);
                    var clearCacheInitializer = new ZnodeEventNotifier<CMSTypeMappingModel>(typeModel);
                    return insert.ToModel<CMSMediaWidgetConfigurationModel>();

                }
            }


            return model;
        }
        //Remove Widget configuration Details
        public virtual bool RemoveWidgetDataFromContentPageConfiguration(CmsContainerWidgetConfigurationModel removeWidgetConfigurationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            bool status = false;

            if (IsNull(removeWidgetConfigurationModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorModelNull);

            if (removeWidgetConfigurationModel?.CMSMappingId <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorMessageForCMSMappingId);

            if (string.IsNullOrEmpty(removeWidgetConfigurationModel?.WidgetKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorMessageForWidgetsKey);

            if (string.IsNullOrEmpty(removeWidgetConfigurationModel?.WidgetCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorMessageForWidgetsKey);
      
            //Remove Media Widget configuration
            if (removeWidgetConfigurationModel.WidgetCode.Equals(ZnodeConstant.ImageWidget, StringComparison.InvariantCultureIgnoreCase) || removeWidgetConfigurationModel.WidgetCode.Equals(ZnodeConstant.VideoWidget, StringComparison.InvariantCultureIgnoreCase))
            {
                CMSMediaWidgetConfigurationModel mediaWidgetConfigurationModel = _mediaWidgetRepository.Table.FirstOrDefault(x => x.CMSMappingId == removeWidgetConfigurationModel.CMSMappingId && x.WidgetsKey == removeWidgetConfigurationModel.WidgetKey).ToModel<CMSMediaWidgetConfigurationModel>();
                if (mediaWidgetConfigurationModel?.CMSMediaConfigurationId > 0)
                    status = _mediaWidgetRepository.Delete(mediaWidgetConfigurationModel.ToEntity<ZnodeCMSMediaConfiguration>());
                else status = true;
            }
            //Remove Container Widget configuration
            if(removeWidgetConfigurationModel.WidgetCode.Equals(ZnodeConstant.ContentContainer, StringComparison.InvariantCultureIgnoreCase))
            {
                CmsContainerWidgetConfigurationModel containerWidgetConfigurationModel = _containerWidgetRepository.Table.FirstOrDefault(x => x.CMSMappingId == removeWidgetConfigurationModel.CMSMappingId && x.WidgetKey.Equals(removeWidgetConfigurationModel.WidgetKey, StringComparison.InvariantCultureIgnoreCase)).ToModel<CmsContainerWidgetConfigurationModel>();
                if(containerWidgetConfigurationModel?.CMSContainerConfigurationId > 0)
                    status = _containerWidgetRepository.Delete(containerWidgetConfigurationModel.ToEntity<ZnodeCMSContainerConfiguration>());
                else status = true;
            }
            //If Widget configuration data deleted successfully, then publish the content page in preview.
            if (status && IsNotNull(removeWidgetConfigurationModel))
            {
                CMSTypeMappingModel typeModel = GetCMSTypeMappingModel(removeWidgetConfigurationModel.CMSMappingId, 0, removeWidgetConfigurationModel.EnableCMSPreview);
                var clearCacheInitializer = new ZnodeEventNotifier<CMSTypeMappingModel>(typeModel);
            }
            return status;
        }

        //Update Text Widget Configuration.
        public virtual bool UpdateTextWidgetConfiguration(CMSTextWidgetConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelNull);

            if (model.CMSTextWidgetConfigurationId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            bool isUpdated = false;
            //Get the cms text widget configuration to update.
            ZnodeCMSTextWidgetConfiguration entityToUpdate = GetWidgetForLocale(model);
            if (entityToUpdate?.CMSTextWidgetConfigurationId > 0)
            {
                //Map the id to update in database.
                model.CMSTextWidgetConfigurationId = entityToUpdate.CMSTextWidgetConfigurationId;

                //Update Text Widget Configuration
                isUpdated = _textWidgetRepository.Update(model.ToEntity<ZnodeCMSTextWidgetConfiguration>());
                if (model.TypeOFMapping.ToLower() == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString().ToLower() && isUpdated)
                {
                    UpdateContentPageAfterPublish(model.CMSMappingId, false);

                    CMSTypeMappingModel typeModel = GetCMSTypeMappingModel(model.CMSMappingId, model.LocaleId, model.EnableCMSPreview);
                    var clearCacheInitializer = new ZnodeEventNotifier<CMSTypeMappingModel>(typeModel);
                }
                ZnodeLogging.LogMessage(isUpdated ? string.Format(Admin_Resources.SuccessTextWidgetConfigurationUpdate, model.CMSTextWidgetConfigurationId) : Admin_Resources.ErrorTextWidgetConfigurationUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            else
            {
                isUpdated = _textWidgetRepository.Insert(model.ToEntity<ZnodeCMSTextWidgetConfiguration>())?.CMSTextWidgetConfigurationId > 0;

                //On updating data in text widget configuration table for content page, insert the entry for it in content page locale table. 
                if (model.TypeOFMapping.ToLower() == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString().ToLower() && isUpdated)
                {
                    int localeId = GetDefaultLocaleId();
                    string pageTitle = _contentPageLocale.Table.Where(x => x.CMSContentPagesId == model.CMSMappingId && x.LocaleId == localeId)?.FirstOrDefault()?.PageTitle;
                    ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { localeId = localeId, pageTitle = pageTitle });
                    _contentPageLocale.Insert(new ZnodeCMSContentPagesLocale { CMSContentPagesId = model.CMSMappingId, LocaleId = model.LocaleId, PageTitle = pageTitle });
                    UpdateContentPageAfterPublish(model.CMSMappingId, false);
                    CMSTypeMappingModel typeModel = GetCMSTypeMappingModel(model.CMSMappingId, localeId, model.EnableCMSPreview);
                    var clearCacheInitializer = new ZnodeEventNotifier<CMSTypeMappingModel>(typeModel);
                }

                ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.SuccessTextWidgetConfigurationInsert : Admin_Resources.ErrorTextWidgetConfigurationInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isUpdated;
        }

        //Method sets the value of CMSTypeMappingModel
        private CMSTypeMappingModel GetCMSTypeMappingModel(int CMSMappingId, int localeId, bool EnableCMSPreview)
        {
            CMSTypeMappingModel typeModel = new CMSTypeMappingModel();
            typeModel.CMSMappingId = CMSMappingId;
            typeModel.TypeOfMapping = ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString().ToLower();
            typeModel.LocaleId = localeId;
            typeModel.EnableCMSPreview = EnableCMSPreview;
            return typeModel;
        }
        private void UpdateContentPageAfterPublish(int contentPageId, bool value)
        {
            ZnodeRepository<ZnodeCMSContentPage> _contentPageRepository = new ZnodeRepository<ZnodeCMSContentPage>();
            //Updating the IsPublished flag for Content Page
            ZnodeLogging.LogMessage("contentPageId to get content page entity: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, contentPageId);
            var entity = _contentPageRepository.GetById(contentPageId);
            entity.IsPublished = value;
            _contentPageRepository.Update(entity);
        }
        #endregion

        #region Form Widget Configration

        //Get List of Text Widget Configuration
        public virtual CMSFormWidgetConfigurationListModel GetFormWidgetConfigurationList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            //maps the entity list to model
            IList<ZnodeCMSFormWidgetConfiguration> widgetList = _formWidgetRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);

            if (widgetList?.Count < 1)
            {
                //If LocaleId is already present in filters, remove it.
                filters.RemoveAll(x => x.Item1 == FilterKeys.LocaleId);
                //Add New LocaleId Into filters.
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetDefaultLocaleId().ToString()));
                //Bind the Filter, sorts & Paging details.
                pageListModel = new PageListModel(filters, sorts, page);
                widgetList = _formWidgetRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            }
            ZnodeLogging.LogMessage("pageListModel to get CMSFormWidgetConfiguration: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            ZnodeLogging.LogMessage("CMSFormWidgetConfiguration list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, widgetList?.Count);

            CMSFormWidgetConfigurationListModel listModel = new CMSFormWidgetConfigurationListModel();
            listModel.FormWidgetConfigurationList = IsNull(widgetList) ? new List<CMSFormWidgetConfigrationModel>() : widgetList?.ToModel<CMSFormWidgetConfigrationModel>().ToList();

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }


        //Create Form Widget Configuration.
        public virtual CMSFormWidgetConfigrationModel CreateFormWidgetConfiguration(CMSFormWidgetConfigrationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            
            //Call Repository to Save the Entity.
            ZnodeCMSFormWidgetConfiguration entity = _formWidgetRepository.Insert(model.ToEntity<ZnodeCMSFormWidgetConfiguration>());
            ZnodeLogging.LogMessage((entity?.CMSFormWidgetConfigurationId > 0) ? Admin_Resources.FormWidgetInsertFailed : Admin_Resources.FormWidgetInsertSuccess, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNotNull(entity))
            {
                PublishContentPage(GetCMSTypeMappingModel(model.CMSMappingId, model.LocaleId, model.EnableCMSPreview));
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return entity.ToModel<CMSFormWidgetConfigrationModel>();
            }

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Update Form Widget Configuration
        public virtual bool UpdateFormWidgetConfiguration(CMSFormWidgetConfigrationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model.CMSFormWidgetConfigurationId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IDLengthCanNotLessOne);

            ZnodeLogging.LogMessage("CMSFormWidgetConfigrationModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, model);
            bool isUpdated = false;

            if (model.IsTextMessage)
                model.RedirectURL = string.Empty;
            else model.TextMessage = string.Empty;

            //Get the cms Form widget configuration to update.
            ZnodeCMSFormWidgetConfiguration entityToUpdate = GetFormWidgetForLocale(model);
            if (entityToUpdate?.CMSFormWidgetConfigurationId > 0)
            {
                //Map the id to update in database.
                model.CMSFormWidgetConfigurationId = entityToUpdate.CMSFormWidgetConfigurationId;

                //Update form Widget Configuration
                ZnodeLogging.LogMessage("CMSFormWidgetConfiguration with Id to be updated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, model?.CMSFormWidgetConfigurationId);
                isUpdated = _formWidgetRepository.Update(model.ToEntity<ZnodeCMSFormWidgetConfiguration>());
                ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.FormWidgetInsertSuccess : Admin_Resources.FormWidgetInsertFailed, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            else
            {
                isUpdated = _formWidgetRepository.Insert(model.ToEntity<ZnodeCMSFormWidgetConfiguration>())?.CMSFormWidgetConfigurationId > 0;

                //On updating data in form widget configuration table for content page, insert the entry for it in content page locale table. 
                if (model.TypeOFMapping == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString() && isUpdated)
                {
                    int localeId = GetDefaultLocaleId();
                    string pageTitle = _contentPageLocale.Table.Where(x => x.CMSContentPagesId == model.CMSMappingId && x.LocaleId == localeId)?.FirstOrDefault()?.PageTitle;
                    _contentPageLocale.Insert(new ZnodeCMSContentPagesLocale { CMSContentPagesId = model.CMSMappingId, LocaleId = model.LocaleId, PageTitle = pageTitle });
                }

                ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.FormWidgetInsertSuccess : Admin_Resources.FormWidgetInsertFailed, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isUpdated;
        }

        #endregion

        #region Category Association

        //Get associated category list
        public virtual CategoryListModel GetAssociatedCategories(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Rename sort key to retrieve sorted data.
            ReplaceSortKeys(ref sorts);

            FilterCollection oldFilter = new FilterCollection();
            oldFilter.AddRange(filters);

            FilterCollection categoryFilter = new FilterCollection();
            categoryFilter.AddRange(filters);

            string localeId, portalId;

            //Get value of portal id and locale id form filter.
            GetPortalIdAndLocaleId(filters, out portalId, out localeId);

            //Method to remove filter to get widget category list.
            PageListModel pageListModel = RemoveFilterToGetWidgetList(sorts, page, oldFilter);

            //Method to get associated widgets categories.
            ZnodeLogging.LogMessage("pageListModel to get associatedWidgetsCategories list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());           
            List<CategoryModel> associatedWidgetsCategories = _cmsWidgetCategory.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues)?.ToModel<CategoryModel>()?.ToList();
            ZnodeLogging.LogMessage("associatedWidgetsCategories list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, associatedWidgetsCategories?.Count);
        
            //Method to get publishCategoryIds.
            List<string> categoryCodes = associatedWidgetsCategories.Select(x => x.CategoryCode)?.ToList();

            ZnodeLogging.LogMessage("publishCategoryCodes to generate whereclause: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, categoryCodes);
          
            ZnodeLogging.LogMessage("publishCategoriesCodes to generate categoryFilter: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, categoryCodes);

            SetCategoryIndexFilter(categoryFilter); 

            //Get catalog id from catalog associated to store.
            int? catalogId = GetCatalogID(portalId);

            FilterCollection categoryFilters = GetCategoryFilters(categoryCodes, catalogId.GetValueOrDefault(), categoryFilter);
            //Get categories data.
            ZnodeLogging.LogMessage("pageListModel to get CategoryEntity list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //Get categories associated to catalog by catalog id.
            List<ZnodePublishCategoryEntity> data = categoryCodes?.Count > 0 ? GetService<IPublishedCategoryDataService>().GetPublishCategoryPageList(new PageListModel(categoryFilters, sorts, page), out pageListModel.TotalRowCount)?.GroupBy(x => x.CategoryCode).Select(y => y.First())?.ToList() : new List<ZnodePublishCategoryEntity>();

            ZnodeLogging.LogMessage("CategoryEntity list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, data?.Count);

            //get category name from category data.
            CategoryListModel listModel = new CategoryListModel();

            //Map properties to CMSWidgetProductCategoryModel
            List<CMSWidgetProductCategoryModel> cmsWidgetProductCategoryListModel = MapParametersForCategory(associatedWidgetsCategories, data);
            ZnodeLogging.LogMessage("CMSWidgetProductCategoryModel list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsWidgetProductCategoryListModel?.Count);

            listModel.CMSWidgetProductCategories = cmsWidgetProductCategoryListModel;

            //Set for pagination
          //  pageListModel.TotalRowCount = data.Count;

            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }
        // Get list of unAssociate categories.
        public virtual CategoryListModel GetUnAssociatedCategories(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Rename sort key to retrieve sorted data 
            ReplaceSortKeys(ref sorts);
            FilterCollection categoryFilter = new FilterCollection();
            categoryFilter.AddRange(filters);

            string portalId, localeId;

            //Get value of portal id and locale id form filter.
            GetPortalIdAndLocaleId(filters, out portalId, out localeId);

            PageListModel pageListModel = RemoveFilterToGetWidgetList(sorts, page, filters);

            //Get portal id.
            //portalId = GetPortalId(filters, portalId); // This method is removed because PortalId is already called.

            //Get catalog id from catalog associated to store.
            int? catalogId = GetCatalogID(portalId);

            //Method to get categories.
            List<ZnodeCMSWidgetCategory> list = _cmsWidgetCategory.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.EntityWhereClause.FilterValues)?.ToList();

            //Get publish category codes .
            List<string> categoryCodes = list.Select(x => x.CategoryCode)?.ToList();
            
            ZnodeLogging.LogMessage("categoryCode list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, categoryCodes);

            //Add Catalog id to filter.
            ZnodeLogging.LogMessage("catalogId to generate categoryFilter: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, catalogId);
            categoryFilter.Add(FilterKeys.ZnodeCatalogId, FilterOperators.Equals, catalogId.ToString());

            SetCategoryIndexFilter(categoryFilter);

            FilterCollection categoryEntityFilter = GetFilters(localeId, ref categoryFilter, categoryCodes, catalogId.GetValueOrDefault());

            //Get categories associated to catalog by catalog id.

            List<ZnodePublishCategoryEntity> data = GetService<IPublishedCategoryDataService>().GetPublishCategoryPageList(new PageListModel(categoryEntityFilter, sorts, page), out pageListModel.TotalRowCount) ;
            
            ZnodeLogging.LogMessage("Categories list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, data?.Count);
            CategoryListModel categoryList = new CategoryListModel { Categories = data.ToModel<CategoryModel>()?.ToList() };

            categoryList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return categoryList;
        }

        protected virtual FilterCollection GetFilters(string localeId, ref FilterCollection categoryFilter, List<string> categoryCode, int catalogId)
        {

            FilterCollection filters = new FilterCollection();
            filters.AddRange(Removefilters(ref categoryFilter));

            if(categoryCode?.Count > 0)
                filters.Add("CategoryCode", FilterOperators.NotIn, string.Join(",", categoryCode?.Distinct().Select(x => $"\"{x}\"")));
            if(!string.IsNullOrEmpty(localeId))
                filters.Add("LocaleId", FilterOperators.Equals, localeId);

            filters.Add("ZnodeCatalogId", FilterOperators.Equals, catalogId.ToString());
            if(categoryFilter.Any(filter => filter.FilterName == FilterKeys.CategoryIndex))
                filters.Add("CategoryIndex", FilterOperators.Equals, ZnodeConstant.DefaultPublishCategoryIndex.ToString());
            filters.Add("VersionId", FilterOperators.Equals, GetCatalogVersionId(catalogId).ToString());

            return filters;
        }              

        // Get category filter
        public virtual FilterCollection GetCategoryFilters(List<string> categoryCode, int catalogId, FilterCollection categoryFilter)
        {
            FilterCollection filters = new FilterCollection();
            filters.AddRange(Removefilters(ref categoryFilter));

            if(categoryCode?.Count > 0)
                filters.Add("CategoryCode", FilterOperators.In, string.Join(",", categoryCode?.Distinct().Select(x => $"\"{x}\"")));
            filters.Add("VersionId", FilterOperators.Equals, GetCatalogVersionId(catalogId).ToString());

            return filters;


   }

        //Remove the parameters from filters 
        private FilterCollection Removefilters( ref FilterCollection categoryFilters)
        {
            ReplaceFilterKeys(ref categoryFilters);

            categoryFilters.RemoveAll(x => x.FilterName == ZnodeCMSWidgetProductEnum.CMSWidgetsId.ToString());
            categoryFilters.RemoveAll(x => x.FilterName == ZnodeCMSWidgetProductEnum.WidgetsKey.ToString());
            categoryFilters.RemoveAll(x => x.FilterName == ZnodeCMSWidgetProductEnum.TypeOFMapping.ToString());
            categoryFilters.RemoveAll(x => x.FilterName == ZnodeCMSWidgetProductEnum.CMSMappingId.ToString());
            categoryFilters.RemoveAll(x => x.FilterName == FilterKeys.MappingId);
            return categoryFilters;
        }

        //Remove associated categories.
        public virtual bool DeleteCategories(ParameterModel cmsWidgetCategoryId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(cmsWidgetCategoryId.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCmsWidgetCategoryIdNullOrEmpty);

            //Generates filter clause for multiple customer review ids.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSWidgetCategoryEnum.CMSWidgetCategoryId.ToString(), ProcedureFilterOperators.In, cmsWidgetCategoryId.Ids));

            //Returns true if deleted successfully else return false.
            bool IsDeleted = _cmsWidgetCategory.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);

            ZnodeLogging.LogMessage(IsDeleted ? string.Format(Admin_Resources.SuccessCategoriesUnassociate, cmsWidgetCategoryId.Ids) : Admin_Resources.ErrorCategoriesUnassociate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return IsDeleted;
        }

        public virtual bool AssociateCategories(ParameterModelForWidgetCategory model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (model?.CategoryCodes.Length < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelCountLessThanZero);

            string[] categoryCodes = model?.CategoryCodes.Split(',');

            ZnodeLogging.LogMessage("Categories with Ids to be associated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, categoryCodes);
            List<ZnodeCMSWidgetCategory> entriesToInsert = new List<ZnodeCMSWidgetCategory>();
            
            if(!Equals(categoryCodes, null))
                foreach (string item in categoryCodes)
                    entriesToInsert.Add(new ZnodeCMSWidgetCategory() { CMSWidgetsId = model.CMSWidgetsId, CMSMappingId = model.CMSMappingId, WidgetsKey = model.WidgetsKey, TypeOFMapping = model.TypeOFMapping, CategoryCode = item
                        , DisplayOrder = 1
                    });

            var associateCategories = _cmsWidgetCategory.Insert(entriesToInsert);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return associateCategories?.Count() > 0;
        }

        //Update CMS Widget Category.
        public virtual bool UpdateCMSWidgetCategory(CategoryModel categoryModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            bool isUpdated = false;
            if (HelperUtility.IsNull(categoryModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            ZnodeCMSWidgetCategory znodeCMSWidgetCategory = _cmsWidgetCategory.Table.Where(x => x.CMSWidgetCategoryId == categoryModel.CMSWidgetCategoryId)?.FirstOrDefault();

            //Assign value to DisplayOrder.
            if (IsNotNull(categoryModel) && IsNotNull(znodeCMSWidgetCategory))
            {
                znodeCMSWidgetCategory.ModifiedDate = DateTime.Now;
                znodeCMSWidgetCategory.DisplayOrder = categoryModel.DisplayOrder;
                ZnodeLogging.LogMessage("CMSWidgetCategory with Id to be updated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, znodeCMSWidgetCategory?.CMSWidgetCategoryId);
                isUpdated = _cmsWidgetCategory.Update(znodeCMSWidgetCategory);
                ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.UpdateMessage : Admin_Resources.ErrorFailedToUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            return isUpdated;
        }
        #endregion

        #region Brand Association

        //Get associated brand list
        public virtual BrandListModel GetAssociatedBrands(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            FilterCollection oldFilter = new FilterCollection();
            oldFilter.AddRange(filters);

            FilterCollection brandFilter = new FilterCollection();
            brandFilter.AddRange(filters);

            PageListModel pageListModel = RemoveFilterToGetWidgetList(sorts, page, oldFilter);

            //Method to get associated widgets brands.
            ZnodeLogging.LogMessage("pageListModel to get associatedWidgetsBrands list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<BrandModel> associatedWidgetsBrands = _cmsWidgetbrand.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues)?.ToModel<BrandModel>()?.ToList();

            brandFilter.Add(FilterKeys.BrandId, FilterOperators.In, associatedWidgetsBrands?.Count > 0 ? string.Join(",", associatedWidgetsBrands.Select(x => x.BrandId)?.ToList()) : string.Empty);
            brandFilter = ManageBrandFilters(brandFilter);

            //Get brand data.        
            IBrandService _brandService = GetService<IBrandService>();
            BrandListModel brandData = _brandService.GetBrandList(null, brandFilter, sorts, page);

            //Map properties to CMSWidgetBrandModel.

            brandData.Brands = MapParametersForBrand(associatedWidgetsBrands, brandData); 
            ZnodeLogging.LogMessage("Brands and BrandCodes list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { brandData?.Brands?.Count, brandData?.BrandCodes?.Count });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return brandData;
        }

        // Get list of unAssociate brands.
        public virtual BrandListModel GetUnAssociatedBrands(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            FilterCollection brandFilter = new FilterCollection();
            brandFilter.AddRange(filters);

            PageListModel pageListModel = RemoveFilterToGetWidgetList(sorts, page, filters);

            //Method to get categories.
            ZnodeLogging.LogMessage("pageListModel to get CMSWidgetBrand list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodeCMSWidgetBrand> list = _cmsWidgetbrand.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.EntityWhereClause.FilterValues)?.ToList();

            //Get brand ids .
            string brandIds = list?.Count > 0 ? string.Join(",", list.Select(x => x.BrandId)?.ToList()) : string.Empty;
            brandIds = !string.IsNullOrEmpty(brandIds) ? brandIds : "0";
            //Method to get ZnodeBrandIds
            ZnodeLogging.LogMessage("brandIds to generate brandFilter: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, brandIds);

            string localeId, portalId;
            //Get value of portal id and locale id form filter.
            GetPortalIdAndLocaleId(brandFilter, out portalId, out localeId);
            brandFilter.Add(FilterKeys.IsAssociated, FilterOperators.Equals, ZnodeConstant.TrueValue);
            brandFilter.Add(FilterKeys.PortalId, FilterOperators.Equals, portalId);
            brandFilter.Add(FilterKeys.BrandId, FilterOperators.NotIn, brandIds);
            brandFilter = ManageBrandFilters(brandFilter);
            IBrandService _brandService = GetService<IBrandService>();
            BrandListModel data = _brandService.GetPortalBrandList(null, brandFilter, sorts, page);
            ZnodeLogging.LogMessage("Brands and BrandCodes list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { data?.Brands?.Count, data?.BrandCodes?.Count });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return data;
        }

        //Remove associated brands.
        public virtual bool DeleteBrands(ParameterModel cmsWidgetBrandId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(cmsWidgetBrandId.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorCMSWidgetBrandIdNullOrEmpty);

            //Generates filter clause for multiple brand ids.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSWidgetBrandEnum.CMSWidgetBrandId.ToString(), ProcedureFilterOperators.In, cmsWidgetBrandId.Ids));

            //Returns true if deleted successfully else return false.
            ZnodeLogging.LogMessage("Brands with Ids to be deleted: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsWidgetBrandId?.Ids);
            bool IsDeleted = _cmsWidgetbrand.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);

            ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessBrandsUnassociate : Admin_Resources.ErrorBrandsUnassociate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return IsDeleted;
        }

        //Associate brands.
        public virtual bool AssociateBrands(ParameterModelForWidgetBrand model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (model?.BrandId.Length < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelCountLessThanZero);

            string[] brandIds = model?.BrandId.Split(',');

            ZnodeLogging.LogMessage("Brands with Ids to be associated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, brandIds);
            List<ZnodeCMSWidgetBrand> entitiesToInsert = new List<ZnodeCMSWidgetBrand>();

            if (IsNotNull(brandIds))
                foreach (string item in brandIds)
                    entitiesToInsert.Add(new ZnodeCMSWidgetBrand() { CMSWidgetsId = model.CMSWidgetsId, CMSMappingId = model.CMSMappingId, WidgetsKey = model.WidgetsKey, TypeOFMapping = model.TypeOFMapping, BrandId = Convert.ToInt32(item),DisplayOrder=1});

            var associateBrands = _cmsWidgetbrand.Insert(entitiesToInsert);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return associateBrands?.Count() > 0;
        }

        //Update CMS Widget Brand.
        public virtual bool UpdateCMSWidgetBrand(BrandModel brandModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            bool isUpdated = false;
            if (IsNull(brandModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            ZnodeCMSWidgetBrand znodeCMSWidgetBrand = _cmsWidgetbrand.Table.Where(x => x.CMSWidgetBrandId == brandModel.CMSWidgetBrandId)?.FirstOrDefault();

            //Assign value to DisplayOrder.
            if (IsNotNull(znodeCMSWidgetBrand))
            {
                znodeCMSWidgetBrand.ModifiedDate = DateTime.Now;
                znodeCMSWidgetBrand.DisplayOrder = brandModel.DisplayOrder;
                ZnodeLogging.LogMessage("CMSWidgetCategory with Id to be updated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, znodeCMSWidgetBrand?.CMSWidgetBrandId);
                isUpdated = _cmsWidgetbrand.Update(znodeCMSWidgetBrand);
                ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.UpdateMessage : Admin_Resources.ErrorFailedToUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            return isUpdated;
        }
        


        #endregion

        #region Search Widget Configuration

        //Get Search Widget Configuration by Widget Configuration id.
        public virtual CMSSearchWidgetConfigurationModel GetSearchWidgetConfiguration(FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, null, null);

            ZnodeCMSSearchWidget widgetData = _searchWidgetRepository.GetEntity(pageListModel.EntityWhereClause.WhereClause, pageListModel.EntityWhereClause.FilterValues);
            if (IsNull(widgetData))
            {
                //If LocaleId is already present in filters, remove it.
                filters.RemoveAll(x => x.Item1 == FilterKeys.LocaleId);
                //Add New LocaleId Into filters.
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetDefaultLocaleId().ToString()));
                //Bind the Filter, sorts & Paging details.
                pageListModel = new PageListModel(filters, null, null);
                widgetData = _searchWidgetRepository.GetEntity(pageListModel.EntityWhereClause.WhereClause, pageListModel.EntityWhereClause.FilterValues);
            }

            ZnodeLogging.LogMessage("ZnodeCMSSearchWidget: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, widgetData);
            CMSSearchWidgetConfigurationModel model = IsNotNull(widgetData) ? widgetData.ToModel<CMSSearchWidgetConfigurationModel>() : new CMSSearchWidgetConfigurationModel();
            model.SearchableAttributes = GetSearchableAttributesForSearchWidget(filters);
            ZnodeLogging.LogMessage("SearchableAttributes list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, model?.SearchableAttributes);
            return model;
        }

        private List<PublishAttributeModel> GetSearchableAttributesForSearchWidget(FilterCollection filters)
        {
            IZnodeRepository<ZnodeCMSContentPage> _ContentPageRepository = new ZnodeRepository<ZnodeCMSContentPage>();

            int contentPageId = Convert.ToInt32(filters.Find(x => x.FilterName == ZnodeCMSWidgetProductEnum.CMSMappingId.ToString().ToLower()).FilterValue);

            int portalId = 0;
            string typeOfMapping = filters.Find(x => x.FilterName.Equals(ZnodeCMSSearchWidgetEnum.TypeOFMapping.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue;

            if (String.Equals(typeOfMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString(), StringComparison.InvariantCultureIgnoreCase))
                portalId = Convert.ToInt32(filters.Find(x => x.FilterName == ZnodeCMSWidgetProductEnum.CMSMappingId.ToString().ToLower()).FilterValue);
            else
                portalId = _ContentPageRepository.Table.FirstOrDefault(x => x.CMSContentPagesId == contentPageId)?.PortalId ?? 0;

            int publishCatalogId = (_portalCatalogRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.PublishCatalogId).GetValueOrDefault();

            if (publishCatalogId > 0)
            {
                int localeId = Convert.ToInt32(filters.Find(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase)).FilterValue);
                int catalogVersionId = GetCatalogVersionId(publishCatalogId) ?? 0;

                FilterCollection filter = new FilterCollection();
                filter.Add("ZnodeCatalogId", FilterOperators.Equals, publishCatalogId.ToString());
                filter.Add("LocaleId", FilterOperators.Equals, localeId.ToString());
                filter.Add("VersionId", FilterOperators.Equals, catalogVersionId.ToString());

                List<ZnodePublishCatalogAttributeEntity> attributeList = GetService<IPublishedCatalogDataService>().GetPublishCatalogAttributeList(new PageListModel(filter, null, null));

                if (attributeList?.Count > 0)
                    return attributeList.ToModel<PublishAttributeModel>().ToList();
            }
            return new List<PublishAttributeModel>();
        }

        //Create the search Widget Configuration.
        public virtual CMSSearchWidgetConfigurationModel CreateSearchWidgetConfiguration(CMSSearchWidgetConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelNull);

            ZnodeCMSSearchWidget entity = _searchWidgetRepository.Insert(model.ToEntity<ZnodeCMSSearchWidget>());
            ZnodeLogging.LogMessage((entity?.CMSSearchWidgetId > 0) ? Admin_Resources.ErrorSearchWidgetConfigurationInsert : Admin_Resources.SuccessSearchWidgetConfigurationInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNotNull(entity))
            {
                PublishContentPage(GetCMSTypeMappingModel(entity.CMSMappingId, entity.LocaleId, model.EnableCMSPreview));
                return entity.ToModel<CMSSearchWidgetConfigurationModel>();
            }

            return model;
        }

        //Update Text Widget Configuration.
        public virtual bool UpdateSearchWidgetConfiguration(CMSSearchWidgetConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelNull);

            if (model.CMSSearchWidgetId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);
            
            bool isUpdated = false;

            //Get the cms text widget configuration to update.
            ZnodeCMSSearchWidget entityToUpdate = GetSearchWidgetForLocale(model);

            if (entityToUpdate?.CMSSearchWidgetId > 0)
            {
                //Map the id to update in database.
                model.CMSSearchWidgetId = entityToUpdate.CMSSearchWidgetId;

                //Update Search Widget Configuration
                isUpdated = _searchWidgetRepository.Update(model.ToEntity<ZnodeCMSSearchWidget>());
                ZnodeLogging.LogMessage(isUpdated ? string.Format(Admin_Resources.SuccessSearchWidgetConfigurationUpdate, model.CMSSearchWidgetId) : Admin_Resources.ErrorSearchWidgetConfigurationUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            else
            {
                isUpdated = _searchWidgetRepository.Insert(model.ToEntity<ZnodeCMSSearchWidget>())?.CMSSearchWidgetId > 0;

                //On updating data in text widget configuration table for content page, insert the entry for it in content page locale table. 
                if (model.TypeOFMapping == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString() && isUpdated)
                {
                    int localeId = GetDefaultLocaleId();
                    string pageTitle = _contentPageLocale.Table.Where(x => x.CMSContentPagesId == model.CMSMappingId && x.LocaleId == localeId)?.FirstOrDefault()?.PageTitle;
                    _contentPageLocale.Insert(new ZnodeCMSContentPagesLocale { CMSContentPagesId = model.CMSMappingId, LocaleId = model.LocaleId, PageTitle = pageTitle });
                }

                ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.SuccessSearchWidgetConfigurationInsert : Admin_Resources.ErrorSearchWidgetConfigurationInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }

            if (model.TypeOFMapping.ToLower() == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString().ToLower() && isUpdated)
            {                
                PublishContentPage(GetCMSTypeMappingModel(model.CMSMappingId, model.LocaleId, model.EnableCMSPreview));
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isUpdated;
        }
        #endregion

        //Save the CMS Widget Content Container Details.
        public virtual bool SaveCmsContainerDetails(CmsContainerWidgetConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Api_Resources.ModelNotNull);

            bool isSaved;
            int? containerWidgetConfigurationId;

            containerWidgetConfigurationId = _containerWidgetRepository.Table.FirstOrDefault(x => x.CMSMappingId == model.CMSMappingId && x.WidgetKey == model.WidgetKey)?.CMSContainerConfigurationId;
            model.CMSContainerConfigurationId = containerWidgetConfigurationId.GetValueOrDefault();
            
            if (model.CMSContainerConfigurationId > 0)
            {
                //Update CMS Widget container Details.
                isSaved = _containerWidgetRepository.Update(model.ToEntity<ZnodeCMSContainerConfiguration>());
                ZnodeLogging.LogMessage(isSaved ? string.Format(Api_Resources.SuccessCMSWidgetContainerConfigurationUpdate, model.CMSContainerConfigurationId)
                    : Api_Resources.ErrorCMSWidgetContainerConfigurationUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            else
            {
                ZnodeCMSContainerConfiguration entity = _containerWidgetRepository.Insert(model.ToEntity<ZnodeCMSContainerConfiguration>());
                ZnodeLogging.LogMessage((entity?.CMSContainerConfigurationId > 0) ? Api_Resources.ErrorCMSWidgetContainerConfigurationInsert
                    : Api_Resources.SuccessCMSWidgetContainerConfigurationInsert, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                isSaved = IsNotNull(entity);
            }
            if (model.TypeOFMapping.ToLower() == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString().ToLower() && isSaved)
            {
                PublishContentPage(GetCMSTypeMappingModel(model.CMSMappingId, 0, model.EnableCMSPreview));
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isSaved;
        }

        //trigger notifier to publish content page
        protected virtual void PublishContentPage(CMSTypeMappingModel model)
        {
            UpdateContentPageAfterPublish(model.CMSMappingId, false);
            var clearCacheInitializer = new ZnodeEventNotifier<CMSTypeMappingModel>(model);
        }
        #endregion

        #region Form widget email Configuration
        public virtual FormWidgetEmailConfigurationModel GetFormWidgetEmailConfiguration(int cMSContentPagesId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("cMSContentPagesId to generate whereClause:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cMSContentPagesId);
            //gets the where clause with filter Values.      
            FilterCollection filters = new FilterCollection();
            if (cMSContentPagesId > 0)
            {
                filters.Add(new FilterTuple(ZnodeFormWidgetEmailConfigurationEnum.CMSContentPagesId.ToString(), ProcedureFilterOperators.Equals, cMSContentPagesId.ToString()));
            }
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause to get FormWidgetEmailConfigurationModel ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClause?.WhereClause);
            FormWidgetEmailConfigurationModel formWidgetEmailConfigurationModel = _formWidgetEmailRepository.GetEntity(whereClause.WhereClause, whereClause.FilterValues).ToModel<FormWidgetEmailConfigurationModel>();
            IZnodeRepository<ZnodeEmailTemplate> _cmsEmailTemplate = new ZnodeRepository<ZnodeEmailTemplate>();
            if (!IsNull(formWidgetEmailConfigurationModel))
            {
                formWidgetEmailConfigurationModel.AcknowledgementEmailTemplate = string.Join(",", _cmsEmailTemplate?.Table?.Where(x => x.EmailTemplateId == formWidgetEmailConfigurationModel.AcknowledgementEmailTemplateId)?.Select(x => x.TemplateName));
                formWidgetEmailConfigurationModel.NotificationEmailTemplate = string.Join(",", _cmsEmailTemplate?.Table?.Where(x => x.EmailTemplateId == formWidgetEmailConfigurationModel.NotificationEmailTemplateId)?.Select(x => x.TemplateName));
            }
            else
            {
                formWidgetEmailConfigurationModel = new FormWidgetEmailConfigurationModel();
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return formWidgetEmailConfigurationModel;
        }

        //Create Form Email Widget Configuration.
        public virtual FormWidgetEmailConfigurationModel CreateFormWidgetEmailConfiguration(FormWidgetEmailConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            //Call Repository to Save the Entity.
            ZnodeFormWidgetEmailConfiguration entity = _formWidgetEmailRepository.Insert(model.ToEntity<ZnodeFormWidgetEmailConfiguration>());
            ZnodeLogging.LogMessage((entity?.FormWidgetEmailConfigurationId > 0) ? Admin_Resources.FormWidgetEmailInsertFailed : Admin_Resources.FormWidgetEmailInsertSuccess, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNotNull(entity))
                return entity.ToModel<FormWidgetEmailConfigurationModel>();
            return model;
        }

        //Update Form Email Widget Configuration
        public virtual bool UpdateFormWidgetEmailConfiguration(FormWidgetEmailConfigurationModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (model.FormWidgetEmailConfigurationId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IDLengthCanNotLessOne);
            bool isUpdated = false;

            //Get the cms Form widget email configuration to update.
            ZnodeFormWidgetEmailConfiguration entityToUpdate = GetFormWidgetEmailForLocale(model);
            if (entityToUpdate?.FormWidgetEmailConfigurationId > 0)
            {
                //Map the id to update in database.
                model.FormWidgetEmailConfigurationId = entityToUpdate.FormWidgetEmailConfigurationId;
                //Update form Widget Configuration
                ZnodeLogging.LogMessage("FormWidgetEmailConfigurationModel with Id to be updated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, model?.FormWidgetEmailConfigurationId);
                isUpdated = _formWidgetEmailRepository.Update(model.ToEntity<ZnodeFormWidgetEmailConfiguration>());
                ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.FormWidgetEmailInsertSuccess : Admin_Resources.FormWidgetEmailInsertFailed, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isUpdated;
        }

        #endregion

        #region Private Method
        //Method to get widget id and locale id from filters.
        protected virtual void GetPortalIdAndLocaleId(FilterCollection filters, out string portalId, out string localeId)
        {
            portalId = string.Empty;
            localeId = string.Empty;
            IZnodeRepository<ZnodeCMSContentPage> _contentPageRepository = new ZnodeRepository<ZnodeCMSContentPage>();
            if (filters?.Count < 0)
                return;

            string typeOfMapping = filters.Find(x => x.FilterName.Equals(ZnodeCMSSearchWidgetEnum.TypeOFMapping.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue;

            if (String.Equals(typeOfMapping, ZnodeCMSTypeofMappingEnum.PortalMapping.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                portalId = filters.FirstOrDefault(x => x.FilterName == ZnodeCMSWidgetCategoryEnum.CMSMappingId.ToString().ToLower()).FilterValue;
            }
            else
            {
                int cmsMappingId = Convert.ToInt32(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeCMSWidgetCategoryEnum.CMSMappingId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
                portalId = Convert.ToString(_contentPageRepository.Table.FirstOrDefault(x => x.CMSContentPagesId == cmsMappingId)?.PortalId);
            }
            localeId = filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.LocaleId , StringComparison.InvariantCultureIgnoreCase)).FilterValue;
            ZnodeLogging.LogMessage("portalId and localeId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalId, localeId });

        }

        //Method to get catalog id from content page id.
        private int? GetCatalogID(string portalId)
        {
            int portalID; 
            int.TryParse(portalId, out portalID);

            return (from portalcatalog in _portalCatalogRepository.Table
                    where portalcatalog.PortalId == portalID
                    select portalcatalog)?.FirstOrDefault()?.PublishCatalogId;
        }


        //Replace the key name of sort to get sorted data
        private void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            for (int index = 0; index < sorts.Keys.Count; index++)
            {
                if (sorts.Keys.Get(index) == FilterKeys.ProductName.ToLower()) { ReplaceSortKeyName(ref sorts, FilterKeys.ProductName.ToLower(), FilterKeys.Name); }
                if (sorts.Keys.Get(index) == FilterKeys.PublishProductId.ToLower()) { ReplaceSortKeyName(ref sorts, FilterKeys.PublishProductId.ToLower(), FilterKeys.PublishedProductId); }               
                if (sorts.Keys.Get(index) == FilterKeys.SKU.ToLower()) { ReplaceSortKeyName(ref sorts, FilterKeys.Sku.ToLower(), FilterKeys.SKU); }
                if (sorts.Keys.Get(index) == FilterKeys.CategoryName.ToLower()) { ReplaceSortKeyName(ref sorts, FilterKeys.CategoryName.ToLower(), FilterKeys.Name); }
                if (sorts.Keys.Get(index) == FilterKeys.PublishCategoryId.ToLower()) { ReplaceSortKeyName(ref sorts, FilterKeys.PublishCategoryId.ToLower(), FilterKeys.ZnodeCategoryId); }
                if (sorts.Keys.Get(index) == FilterKeys.DisplayOrder.ToLower()) { ReplaceSortKeyName(ref sorts, FilterKeys.DisplayOrder.ToLower(), FilterKeys.DisplayOrder); }
            }
        }

        //Replace the key name of filters to get filtered data 
        private void ReplaceFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (tuple.Item1 == ZnodeCMSWidgetProductEnum.CMSMappingId.ToString().ToLower()) ReplaceFilterKeyName(ref filters, ZnodeCMSWidgetProductEnum.CMSMappingId.ToString().ToLower(), FilterKeys.MappingId.ToString());
                if (tuple.Item1 == ZnodeCMSWidgetProductEnum.TypeOFMapping.ToString().ToLower()) ReplaceFilterKeyName(ref filters, ZnodeCMSWidgetProductEnum.TypeOFMapping.ToString().ToLower(), ZnodeCMSWidgetProductEnum.TypeOFMapping.ToString());
                if (tuple.Item1 == ZnodeCMSWidgetProductEnum.WidgetsKey.ToString().ToLower()) ReplaceFilterKeyName(ref filters, ZnodeCMSWidgetProductEnum.WidgetsKey.ToString().ToLower(), ZnodeCMSWidgetProductEnum.WidgetsKey.ToString());
                if (tuple.Item1 == ZnodeCMSWidgetProductEnum.CMSWidgetsId.ToString().ToLower()) ReplaceFilterKeyName(ref filters, ZnodeCMSWidgetProductEnum.CMSWidgetsId.ToString().ToLower(), ZnodeCMSWidgetProductEnum.CMSWidgetsId.ToString());
                if (tuple.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()) ReplaceFilterKeyName(ref filters, ZnodeLocaleEnum.LocaleId.ToString().ToLower(), ZnodeLocaleEnum.LocaleId.ToString());
                if (tuple.Item1 == FilterKeys.ProductName.ToString().ToLower()) ReplaceFilterKeyName(ref filters, View_ManageProductListEnum.ProductName.ToString().ToLower(), FilterKeys.Name.ToString());
                if (tuple.Item1 == FilterKeys.Sku.ToString().ToLower()) ReplaceFilterKeyName(ref filters, FilterKeys.Sku.ToString().ToLower(), FilterKeys.SKU.ToString());
                if (tuple.Item1 == FilterKeys.CategoryName.ToString().ToLower()) ReplaceFilterKeyName(ref filters, FilterKeys.CategoryName.ToString().ToLower(), FilterKeys.Name.ToString());
                if (tuple.Item1 == FilterKeys.CategoryCode.ToString().ToLower()) ReplaceFilterKeyName(ref filters, FilterKeys.CategoryCode.ToString().ToLower(), FilterKeys.Name.ToString());
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
                        if (string.Equals(item, FilterKeys.ProductName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(FilterKeys.Name); }
                        else if (string.Equals(item, FilterKeys.Sku, StringComparison.OrdinalIgnoreCase)) { newValues.Add(FilterKeys.SKU); }
                        else if (string.Equals(item, FilterKeys.CategoryName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(FilterKeys.Name); }
                        else newValues.Add(item);
                    }
                    ZnodeLogging.LogMessage("newValues list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, newValues?.Count());
                    ReplaceFilterKeyName(ref filters, tuple.Item1, string.Join("|", newValues));
                }
            }
        }

        //Method to remove filter to get product/category list 
        private PageListModel RemoveFilterToGetList(NameValueCollection sorts, NameValueCollection page, ref FilterCollection productFilter)
        {
            ReplaceFilterKeys(ref productFilter);
            PageListModel pageListModel = new PageListModel(productFilter, sorts, page);
            productFilter.RemoveAll(x => x.FilterName == ZnodeCMSWidgetProductEnum.CMSWidgetsId.ToString());
            productFilter.RemoveAll(x => x.FilterName == ZnodeCMSWidgetProductEnum.WidgetsKey.ToString());
            productFilter.RemoveAll(x => x.FilterName == ZnodeCMSWidgetProductEnum.TypeOFMapping.ToString());
            productFilter.RemoveAll(x => x.FilterName == ZnodeCMSWidgetProductEnum.CMSMappingId.ToString());
            productFilter.RemoveAll(x => x.FilterName == FilterKeys.MappingId);
            return pageListModel;
        }

        //Method to remove filter to get widget product/Category list.
        private static PageListModel RemoveFilterToGetWidgetList(NameValueCollection sorts, NameValueCollection page, FilterCollection oldFilter)
        {
            oldFilter.RemoveAll(x => x.FilterName == FilterKeys.LocaleId.ToLower());
            oldFilter.RemoveAll(x => x.FilterName == View_ManageProductListEnum.ProductName.ToString().ToLower());
            oldFilter.RemoveAll(x => x.FilterName == FilterKeys.Sku);
            oldFilter.RemoveAll(x => x.FilterName == FilterKeys.CategoryName.ToLower());
            oldFilter.RemoveAll(x => x.FilterName == FilterKeys.BrandName.ToLower());
            oldFilter.RemoveAll(x => x.FilterName.Contains("|"));
            PageListModel pageListModel = new PageListModel(oldFilter, sorts, page);
            return pageListModel;
        }

        //Method to map properties to CMSWidgetProductCategoryModel for associate product
        protected virtual List<CMSWidgetProductCategoryModel> MapParametersForProduct(List<CMSWidgetProductModel> cmsWidgetPublishProducts, List<PublishProductModel> productEntity)
        {
            ZnodeLogging.LogMessage("cmsWidgetPublishProducts count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsWidgetPublishProducts?.Count());
            MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
            string serverPath = GetMediaServerUrl(configurationModel);
            ZnodeLogging.LogMessage("serverPath:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, serverPath);
           
            List<CMSWidgetProductCategoryModel> cmsWidgetProductCategoryListModel = new List<CMSWidgetProductCategoryModel>();
            cmsWidgetPublishProducts?.ForEach(products =>
            {
                var cmsWidgetProduct = productEntity?.FirstOrDefault(x => x.SKU == products.SKU);
                if (IsNotNull(cmsWidgetProduct))
                {
                    CMSWidgetProductCategoryModel cmsWidgetProductCategoryModel = new CMSWidgetProductCategoryModel();
                    cmsWidgetProductCategoryModel.CMSWidgetsId = products.CMSWidgetsId;
                    cmsWidgetProductCategoryModel.CMSMappingId = products.CMSMappingId;
                    cmsWidgetProductCategoryModel.TypeOfMapping = products.TypeOfMapping;
                    cmsWidgetProductCategoryModel.WidgetsKey = products.WidgetsKey;
                    cmsWidgetProductCategoryModel.CMSWidgetProductId = products.CMSWidgetProductId;
                    cmsWidgetProductCategoryModel.PublishProductId = products.PublishProductId;
                    cmsWidgetProductCategoryModel.ProductName = cmsWidgetProduct?.Name;
                    cmsWidgetProductCategoryModel.SKU = cmsWidgetProduct?.SKU;
                    cmsWidgetProductCategoryModel.DisplayOrder = products.DisplayOrder ?? cmsWidgetProduct.DisplayOrder;
                    cmsWidgetProductCategoryModel.ProductType = cmsWidgetProduct.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductType)?.SelectValues.FirstOrDefault().Value;
                    string imageName = cmsWidgetProduct.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues;
                    cmsWidgetProductCategoryModel.ImagePath = $"{serverPath}{imageName}";
                    cmsWidgetProductCategoryListModel.Add(cmsWidgetProductCategoryModel);
                }
            });
            ZnodeLogging.LogMessage("cmsWidgetProductCategoryListModel list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsWidgetProductCategoryListModel?.Count());
            return cmsWidgetProductCategoryListModel;
        }

        //Method to map properties to CMSWidgetProductCategoryModel for associate category.
        public virtual List<CMSWidgetProductCategoryModel> MapParametersForCategory(List<CategoryModel> cmsWidgetPublishCategories, List<ZnodePublishCategoryEntity> categoryEntity)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("cmsWidgetPublishCategories count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsWidgetPublishCategories?.Count());
            List<CMSWidgetProductCategoryModel> cmsWidgetProductCategoryListModel = new List<CMSWidgetProductCategoryModel>();

            cmsWidgetPublishCategories.ForEach(category =>
            {
                var cmsWidgetCategory = categoryEntity.FirstOrDefault(x => x?.CategoryCode == category?.CategoryCode);
                CMSWidgetProductCategoryModel cmsWidgetProductCategoryModel = new CMSWidgetProductCategoryModel();
                if(IsNotNull(cmsWidgetCategory))
                {
                    cmsWidgetProductCategoryModel.CMSWidgetsId = category.CMSWidgetsId;
                    cmsWidgetProductCategoryModel.CMSMappingId = category.CMSMappingId;
                    cmsWidgetProductCategoryModel.TypeOfMapping = category.TypeOFMapping;
                    cmsWidgetProductCategoryModel.WidgetsKey = category.WidgetsKey;
                    cmsWidgetProductCategoryModel.DisplayOrder = category?.DisplayOrder;
                    cmsWidgetProductCategoryModel.CategoryName = cmsWidgetCategory?.Name;
                    cmsWidgetProductCategoryModel.CategoryCode = cmsWidgetCategory.CategoryCode;
                    cmsWidgetProductCategoryModel.CMSWidgetCategoryId = Convert.ToInt32(category.CMSWidgetCategoryId);
                    cmsWidgetProductCategoryListModel.Add(cmsWidgetProductCategoryModel);
                }
            });
            ZnodeLogging.LogMessage("cmsWidgetProductCategoryListModel list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsWidgetProductCategoryListModel?.Count());
            return cmsWidgetProductCategoryListModel;
        }

        //Method to map properties to CMSWidgetProductCategoryModel for associate category.
        private static List<BrandModel> MapParametersForBrand(List<BrandModel> cmsWidgetBrands, BrandListModel brandEntity)
        {
            List<BrandModel> brandModel = new List<BrandModel>();
            ZnodeLogging.LogMessage("cmsWidgetBrands count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsWidgetBrands?.Count());
            cmsWidgetBrands.ForEach(brands =>
            {
                var cmsWidgetProduct = brandEntity.Brands.FirstOrDefault(x => x.BrandId == brands.BrandId);
                ZnodeLogging.LogMessage("brands:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, brands);
                BrandModel cmsWidgetProductBrandModel = new BrandModel();
                if(IsNotNull(cmsWidgetProduct))
                {
                    cmsWidgetProductBrandModel.CMSWidgetsId = brands.CMSWidgetsId;
                    cmsWidgetProductBrandModel.CMSMappingId = brands.CMSMappingId;
                    cmsWidgetProductBrandModel.TypeOFMapping = brands.TypeOFMapping;
                    cmsWidgetProductBrandModel.WidgetsKey = brands.WidgetsKey;
                    cmsWidgetProductBrandModel.BrandId = brands.BrandId;
                    cmsWidgetProductBrandModel.DisplayOrder = brands?.DisplayOrder ?? cmsWidgetProduct.DisplayOrder;
                    cmsWidgetProductBrandModel.BrandCode = cmsWidgetProduct?.BrandCode;
                    cmsWidgetProductBrandModel.BrandName = cmsWidgetProduct?.BrandName;
                    cmsWidgetProductBrandModel.CMSWidgetBrandId = Convert.ToInt32(brands.CMSWidgetBrandId);
                    brandModel.Add(cmsWidgetProductBrandModel);
                }
            });
            ZnodeLogging.LogMessage("brandModel count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, brandModel?.Count());
            return brandModel;
        }

        private string GetPortalId(FilterCollection filters, string portalId)
        {
            if (filters?.Count > 0 && Equals(ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString().ToLower(), filters.Find(x => x.FilterName == ZnodeCMSWidgetCategoryEnum.TypeOFMapping.ToString().ToLower())?.FilterValue.ToLower()))
            {
                IZnodeRepository<ZnodeCMSContentPage> _contentPageRepository = new ZnodeRepository<ZnodeCMSContentPage>();
                int tempPortalId = Convert.ToInt32(portalId);
                ZnodeLogging.LogMessage("tempPortalId to get portalId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, tempPortalId);
                portalId = Convert.ToString(_contentPageRepository.Table.Where(x => x.CMSContentPagesId == tempPortalId)?.Select(x => x.PortalId)?.FirstOrDefault());
            }
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, portalId);
            return portalId;
        }

        //Get the text widget.
        private ZnodeCMSTextWidgetConfiguration GetWidgetForLocale(CMSTextWidgetConfigurationModel model)
           => _textWidgetRepository.Table.FirstOrDefault(x => x.WidgetsKey == model.WidgetsKey && x.TypeOFMapping == model.TypeOFMapping
              && x.CMSWidgetsId == model.CMSWidgetsId && x.CMSMappingId == model.CMSMappingId && x.LocaleId == model.LocaleId);

        //Get the form widget.
        private ZnodeCMSFormWidgetConfiguration GetFormWidgetForLocale(CMSFormWidgetConfigrationModel model)
           => _formWidgetRepository.Table.FirstOrDefault(x => x.WidgetsKey == model.WidgetsKey && x.TypeOFMapping == model.TypeOFMapping
              && x.CMSWidgetsId == model.CMSWidgetsId && x.CMSMappingId == model.CMSMappingId && x.LocaleId == model.LocaleId);

        private ZnodeCMSSearchWidget GetSearchWidgetForLocale(CMSSearchWidgetConfigurationModel model)
         => _searchWidgetRepository.Table.FirstOrDefault(x => x.WidgetsKey == model.WidgetsKey && x.TypeOFMapping == model.TypeOFMapping
            && x.CMSWidgetsId == model.CMSWidgetsId && x.CMSMappingId == model.CMSMappingId && x.LocaleId == model.LocaleId);

        //Get the form widget email.
        private ZnodeFormWidgetEmailConfiguration GetFormWidgetEmailForLocale(FormWidgetEmailConfigurationModel model)
           => _formWidgetEmailRepository.Table.Where(x => x.CMSContentPagesId == model.CMSContentPagesId
               )?.FirstOrDefault();


        //Manage brand filters.
        private FilterCollection ManageBrandFilters(FilterCollection brandFilter)
        {
            ReplaceFilterKeys(ref brandFilter);
            brandFilter.RemoveAll(x => x.FilterName == ZnodeCMSWidgetBrandEnum.CMSWidgetsId.ToString());
            brandFilter.RemoveAll(x => x.FilterName == ZnodeCMSWidgetBrandEnum.WidgetsKey.ToString());
            brandFilter.RemoveAll(x => x.FilterName == ZnodeCMSWidgetBrandEnum.TypeOFMapping.ToString());
            brandFilter.RemoveAll(x => x.FilterName == ZnodeCMSWidgetBrandEnum.CMSMappingId.ToString());
            brandFilter.RemoveAll(x => x.FilterName == FilterKeys.MappingId);
            return brandFilter;
        }

        //Method to map image path for unassociated product.
        private void MapParameterForImagePath(List<ProductDetailsModel> productDetailsListModel, List<PublishProductModel> productEntity)
        {            
            ZnodeLogging.LogMessage("ProductDetailsModel list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, productDetailsListModel?.Count());
            MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
            string serverPath = GetMediaServerUrl(configurationModel);

            string thumbnailPath = $"{serverPath}{configurationModel.ThumbnailFolderName}";
            ZnodeLogging.LogMessage("Parameter: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { serverPath= serverPath, thumbnailPath =thumbnailPath });

            productDetailsListModel?.ForEach(d =>
            {
                var product = productEntity.FirstOrDefault(s => s.SKU == d.SKU);
                string imageName = product.Attributes.Where(y => y.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues;
                d.ImagePath = $"{thumbnailPath}/{imageName}";
            });
        }

        #endregion
    }
}
