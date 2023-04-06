using Autofac;
using Autofac.Integration.WebApi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Znode.Engine.Promotions;
using Znode.Engine.Services;
using Znode.Engine.Services.Maps;
using Znode.Engine.Shipping;
using Znode.Engine.Shipping.Helper;
using Znode.Engine.Taxes;
using Znode.Engine.Taxes.Helper;
using Znode.Engine.Taxes.Interfaces;
using Znode.Libraries.Admin;
using Znode.Libraries.Admin.Import;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Recommendations;
using Znode.Engine.Payment.Client;
using Znode.Engine.Hangfire;
using Znode.Engine.SMS;

namespace Znode.Engine.Api
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public virtual void Register(ContainerBuilder builder)
        {
            //Register Assemblies Includes the API Controller.
            builder.RegisterApiControllers(AppDomain.CurrentDomain.GetAssemblies());

            RegisterServices(builder);
            RegisterServicesV2(builder);
        }

        public int Order
        {
            get { return 0; }
        }

        void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<LocaleService>().As<ILocaleService>().InstancePerRequest();
            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerRequest();
            builder.RegisterType<AccessPermissionService>().As<IAccessPermissionService>().InstancePerRequest();
            builder.RegisterType<AccountQuoteService>().As<IAccountQuoteService>().InstancePerRequest();
            builder.RegisterType<EmailTemplateService>().As<IEmailTemplateService>().InstancePerRequest();
            builder.RegisterType<CustomerReviewService>().As<ICustomerReviewService>().InstancePerRequest();
            builder.RegisterType<DynamicContentService>().As<IDynamicContentService>().InstancePerRequest();
            builder.RegisterType<CSSService>().As<ICSSService>().InstancePerRequest();
            builder.RegisterType<ContentPageService>().As<IContentPageService>().InstancePerRequest();
            builder.RegisterType<RecommendationService>().As<IRecommendationService>().InstancePerRequest();
            builder.RegisterType<CMSWidgetsService>().As<ICMSWidgetsService>().InstancePerRequest();
            builder.RegisterType<CMSWidgetConfigurationService>().As<ICMSWidgetConfigurationService>().InstancePerRequest();
            builder.RegisterType<WebSiteService>().As<IWebSiteService>().InstancePerDependency();
            builder.RegisterType<UrlRedirectService>().As<IUrlRedirectService>().InstancePerRequest();
            builder.RegisterType<ThemeService>().As<IThemeService>().InstancePerRequest();
            builder.RegisterType<ManageMessageService>().As<IManageMessageService>().InstancePerRequest();
            builder.RegisterType<SEOService>().As<ISEOService>().InstancePerRequest();
            builder.RegisterType<SliderService>().As<ISliderService>().InstancePerRequest();
            builder.RegisterType<TemplateService>().As<ITemplateService>().InstancePerRequest();
            builder.RegisterType<PublishProductService>().As<IPublishProductService>().InstancePerRequest();
            builder.RegisterType<PublishCategoryService>().As<IPublishCategoryService>().InstancePerRequest();
            builder.RegisterType<PublishCatalogService>().As<IPublishCatalogService>().InstancePerRequest();
            builder.RegisterType<EcommerceCatalogService>().As<IEcommerceCatalogService>().InstancePerRequest();
            builder.RegisterType<CityService>().As<ICityService>().InstancePerRequest();
            builder.RegisterType<WeightUnitService>().As<IWeightUnitService>().InstancePerRequest();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerRequest();
            builder.RegisterType<StateService>().As<IStateService>().InstancePerRequest();
            builder.RegisterType<LocaleService>().As<ILocaleService>().InstancePerRequest();
            builder.RegisterType<GeneralSettingService>().As<IGeneralSettingService>().InstancePerRequest();
            builder.RegisterType<DefaultGlobalConfigService>().As<IDefaultGlobalConfigService>().InstancePerRequest();
            builder.RegisterType<CurrencyService>().As<ICurrencyService>().InstancePerRequest();
            builder.RegisterType<HighlightService>().As<IHighlightService>().InstancePerRequest();
            builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerRequest();
            builder.RegisterType<RMARequestItemService>().As<IRMARequestItemService>().InstancePerRequest();
            builder.RegisterType<RMARequestService>().As<IRMARequestService>().InstancePerRequest();
            builder.RegisterType<RMAConfigurationService>().As<IRMAConfigurationService>().InstancePerRequest();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerRequest();
            builder.RegisterType<ImportService>().As<IImportService>().InstancePerRequest();
            builder.RegisterType<ExportService>().As<IExportService>().InstancePerRequest();
            builder.RegisterType<ProductFeedService>().As<IProductFeedService>().InstancePerRequest();
            builder.RegisterType<PaymentSettingService>().As<IPaymentSettingService>().InstancePerRequest();
            builder.RegisterType<TaxClassService>().As<ITaxClassService>().InstancePerRequest();
            builder.RegisterType<ShippingService>().As<IShippingService>().InstancePerRequest();
            builder.RegisterType<TaxRuleTypeService>().As<ITaxRuleTypeService>().InstancePerRequest();
            builder.RegisterType<ShippingTypeService>().As<IShippingTypeService>().InstancePerRequest();
            builder.RegisterType<PromotionTypeService>().As<IPromotionTypeService>().InstancePerRequest();
            builder.RegisterType<PromotionService>().As<IPromotionService>().InstancePerRequest();
            builder.RegisterType<VendorService>().As<IVendorService>().InstancePerRequest();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerRequest();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerRequest();
            builder.RegisterType<CatalogService>().As<ICatalogService>().InstancePerRequest();
            builder.RegisterType<BrandService>().As<IBrandService>().InstancePerRequest();
            builder.RegisterType<PIMAttributeService>().As<IPIMAttributeService>().InstancePerRequest();
            builder.RegisterType<PIMAttributeFamilyService>().As<IPIMAttributeFamilyService>().InstancePerRequest();
            builder.RegisterType<PIMAttributeGroupService>().As<IPIMAttributeGroupService>().InstancePerRequest();
            builder.RegisterType<PIMAttributeGroupService>().As<IPIMAttributeGroupService>().InstancePerRequest();
            builder.RegisterType<AttributeFamilyService>().As<IAttributeFamilyService>().InstancePerRequest();
            builder.RegisterType<AttributeGroupService>().As<IAttributeGroupService>().InstancePerRequest();
            builder.RegisterType<AttributesService>().As<IAttributesService>().InstancePerRequest();
            builder.RegisterType<MediaConfigurationService>().As<IMediaConfigurationService>().InstancePerRequest();
            builder.RegisterType<MediaManagerServices>().As<IMediaManagerServices>().InstancePerRequest();
            builder.RegisterType<ReportService>().As<IReportService>().InstancePerRequest();
            builder.RegisterType<AddonGroupService>().As<IAddonGroupService>().InstancePerRequest();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerRequest();
            builder.RegisterType<ERPConfiguratorService>().As<IERPConfiguratorService>().InstancePerRequest();
            builder.RegisterType<ERPTaskSchedulerService>().As<IERPTaskSchedulerService>().InstancePerRequest();
            builder.RegisterType<TouchPointConfigurationService>().As<ITouchPointConfigurationService>().InstancePerRequest();
            builder.RegisterType<GiftCardService>().As<IGiftCardService>().InstancePerRequest();
            builder.RegisterType<InventoryService>().As<IInventoryService>().InstancePerRequest();
            builder.RegisterType<ApplicationSettingsService>().As<IApplicationSettingsService>().InstancePerRequest();
            builder.RegisterType<WarehouseService>().As<IWarehouseService>().InstancePerRequest();
            builder.RegisterType<ServerValidationService>().As<IServerValidationService>().InstancePerRequest();
            builder.RegisterType<MenuService>().As<IMenuService>().InstancePerRequest();
            builder.RegisterType<RoleService>().As<IRoleService>().InstancePerRequest();
            builder.RegisterType<ProfileService>().As<IProfileService>().InstancePerRequest();
            builder.RegisterType<PriceService>().As<IPriceService>().InstancePerRequest();
            builder.RegisterType<PortalService>().As<IPortalService>().InstancePerRequest();
            builder.RegisterType<PortalCountryService>().As<IPortalCountryService>().InstancePerRequest();
            builder.RegisterType<PortalProfileService>().As<IPortalProfileService>().InstancePerRequest();
            builder.RegisterType<PortalUnitService>().As<IPortalUnitService>().InstancePerRequest();
            builder.RegisterType<SMTPService>().As<ISMTPService>().InstancePerRequest();
            builder.RegisterType<StoreLocatorService>().As<IStoreLocatorService>().InstancePerRequest();
            builder.RegisterType<WebStoreWishListService>().As<IWebStoreWishListService>().InstancePerRequest();
            builder.RegisterType<WebStoreWidgetService>().As<IWebStoreWidgetService>().InstancePerRequest();
            builder.RegisterType<WebStoreCaseRequestService>().As<IWebStoreCaseRequestService>().InstancePerRequest();
            builder.RegisterType<SearchService>().As<ISearchService>().InstancePerRequest();
            builder.RegisterType<ProductReviewStateService>().As<IProductReviewStateService>().InstancePerRequest();
            builder.RegisterType<OrderStateService>().As<IOrderStateService>().InstancePerRequest();
            builder.RegisterType<DomainService>().As<IDomainService>().InstancePerRequest();
            builder.RegisterType<ERPConnectorService>().As<IERPConnectorService>().InstancePerRequest();
            builder.RegisterType<DashboardService>().As<IDashboardService>().InstancePerRequest();
            builder.RegisterType<TagManagerService>().As<ITagManagerService>().InstancePerRequest();
            builder.RegisterType<BlogNewsService>().As<IBlogNewsService>().InstancePerRequest();
            builder.RegisterType<GlobalAttributeService>().As<IGlobalAttributeService>().InstancePerRequest();
            builder.RegisterType<GlobalAttributeGroupService>().As<IGlobalAttributeGroupService>().InstancePerRequest();
            builder.RegisterType<GlobalAttributeGroupEntityService>().As<IGlobalAttributeGroupEntityService>().InstancePerRequest();
            builder.RegisterType<GlobalAttributeFamilyService>().As<IGlobalAttributeFamilyService>().InstancePerRequest();
            builder.RegisterType<SearchProfileService>().As<ISearchProfileService>().InstancePerRequest();
            builder.RegisterType<FormBuilderService>().As<IFormBuilderService>().InstancePerRequest();
            builder.RegisterType<FormSubmissionService>().As<IFormSubmissionService>().InstancePerRequest();
            builder.RegisterType<LogMessageService>().As<ILogMessageService>().InstancePerRequest();
            builder.RegisterType<SearchBoostAndBuryRuleService>().As<ISearchBoostAndBuryRuleService>().InstancePerRequest();
            builder.RegisterType<ZnodeOrderHelper>().As<IZnodeOrderHelper>().InstancePerDependency();
            builder.RegisterType<PublishProductHelper>().As<IPublishProductHelper>().InstancePerRequest();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerRequest();
            builder.RegisterType<ContentContainerService>().As<IContentContainerService>().InstancePerRequest();
            builder.RegisterType<ContainerTemplateService>().As<IContainerTemplateService>().InstancePerRequest();
           

            builder.RegisterType<PublishStateService>().As<IPublishStateService>().InstancePerRequest();
            builder.RegisterType<PublishHistoryService>().As<IPublishHistoryService>().InstancePerRequest();
            builder.RegisterType<ProgressNotificationService>().As<IProgressNotificationService>().InstancePerRequest();
            builder.RegisterType<ECertService>().As<IECertService>().InstancePerRequest();
            builder.RegisterType<DevExpressReportService>().As<IDevExpressReportService>().InstancePerRequest();
            builder.RegisterType<TypeaheadService>().As<ITypeaheadService>().InstancePerRequest();
            builder.RegisterType<ShoppingCartMap>().As<IShoppingCartMap>().InstancePerDependency();
            builder.RegisterType<ShoppingCartItemMap>().As<IShoppingCartItemMap>().InstancePerDependency();
            builder.RegisterType<ZnodeShoppingCart>().As<IZnodeShoppingCart>().InstancePerDependency();
            builder.RegisterType<ZnodeCheckout>().As<IZnodeCheckout>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<ImportHelper>().As<IImportHelper>().InstancePerRequest();
            builder.RegisterType<AttributeSwatchHelper>().As<IAttributeSwatchHelper>().InstancePerRequest();
            builder.RegisterType<ImageHelper>().As<IImageHelper>().InstancePerDependency();
            builder.RegisterType<ImageMediaHelper>().As<IImageMediaHelper>().InstancePerDependency();
            builder.RegisterType<ZnodeProductFeedHelper>().As<IZnodeProductFeedHelper>().InstancePerDependency();

            builder.RegisterType<DiagnosticsService>().As<IDiagnosticsService>().InstancePerRequest();
            builder.RegisterType<CatalogHistoryService>().As<ICatalogHistoryService>().InstancePerDependency();
            builder.RegisterType<CategoryHistoryService>().As<ICategoryHistoryService>().InstancePerDependency();
            builder.RegisterType<RMAReturnService>().As<IRMAReturnService>().InstancePerDependency();

            builder.RegisterType<ZnodeOrderReceipt>().As<IZnodeOrderReceipt>().InstancePerRequest();
            builder.RegisterType<ZnodeMultipleAddressCart>().As<IZnodeMultipleAddressCart>().InstancePerDependency();
            builder.RegisterType<OrderInventoryManageHelper>().As<IOrderInventoryManageHelper>().InstancePerDependency();
            

            builder.RegisterType<PublishBrandService>().As<IPublishBrandService>().InstancePerRequest();
            builder.RegisterType<SearchReportService>().As<ISearchReportService>().InstancePerRequest();
            builder.RegisterType<UserLoginHelper>().As<IUserLoginHelper>().InstancePerDependency();
            builder.RegisterType<CMSPageSearchService>().As<ICMSPageSearchService>().InstancePerRequest();
            builder.RegisterType<QuoteService>().As<IQuoteService>().InstancePerRequest();
            builder.RegisterType<BarcodeHelper>().As<IBarcodeHelper>().InstancePerDependency();
            builder.RegisterType<ZnodeQuoteHelper>().As<IZnodeQuoteHelper>().InstancePerDependency();
            builder.RegisterType<BarcodeHelper>().As<IBarcodeHelper>().InstancePerDependency();
            builder.RegisterType<ERPJobs>().As<IERPJobs>().InstancePerDependency();

            builder.RegisterType<ZnodeEmail>().As<IZnodeEmail>().InstancePerRequest();
            builder.RegisterType<EmailTemplateSharedService>().As<IEmailTemplateSharedService>().InstancePerRequest();

            builder.RegisterType<DiscountHelper>().As<IDiscountHelper>().InstancePerDependency();
            builder.RegisterType<ZnodeOrderDiscountHelper>().As<IZnodeOrderDiscountHelper>().InstancePerDependency();
            builder.RegisterType<ZnodeOrderShippingHelper>().As<IZnodeOrderShippingHelper>().InstancePerDependency();
            builder.RegisterType<ZnodeCartPromotionManager>().As<IZnodeCartPromotionManager>().InstancePerDependency();
            builder.RegisterType<ZnodePortalCart>().As<IZnodePortalCart>().InstancePerDependency();
            builder.RegisterType<ZnodeShippingCartMap>().As<IZnodeShippingCartMap>().InstancePerDependency();
            builder.RegisterType<ZnodeShippingManager>().As<IZnodeShippingManager>().InstancePerDependency();
            builder.RegisterType<ZnodeShippingHelper>().As<IZnodeShippingHelper>().InstancePerDependency();
            builder.RegisterType<ZnodeTaxManager>().As<IZnodeTaxManager>().InstancePerDependency();
            builder.RegisterType<ZnodeTaxHelper>().As<IZnodeTaxHelper>().InstancePerDependency();
            builder.RegisterType<ZnodeRssWriter>().As<IZnodeRssWriter>().InstancePerRequest();
            builder.RegisterType<ZnodePromotionHelper>().As<IZnodePromotionHelper>().InstancePerDependency();
            builder.RegisterType<PaymentTokenService>().As<IPaymentTokenService>().InstancePerRequest();
            builder.RegisterType<SMSService>().As<ISMSService>().InstancePerRequest();
            builder.RegisterType<ZnodeSMSManager>().As<IZnodeSMSManager>().InstancePerRequest();
            

            #region Publish Entities
            builder.RegisterType<PublishCatalogDataService>().As<IPublishCatalogDataService>().InstancePerRequest();
            builder.RegisterType<PublishCategoryDataService>().As<IPublishCategoryDataService>().InstancePerRequest();
            builder.RegisterType<PublishedCatalogDataService>().As<IPublishedCatalogDataService>().InstancePerRequest();
            builder.RegisterType<PublishedPortalDataService>().As<IPublishedPortalDataService>().InstancePerRequest();
            builder.RegisterType<PublishedCategoryDataService>().As<IPublishedCategoryDataService>().InstancePerRequest();
            builder.RegisterType<PublishedProductDataService>().As<IPublishedProductDataService>().InstancePerRequest();
            builder.RegisterType<PublishPortalDataService>().As<IPublishPortalDataService>().InstancePerRequest();
            builder.RegisterType<PublishProductDataService>().As<IPublishProductDataService>().InstancePerRequest();
            builder.RegisterType<PublishContainerDataService>().As<IPublishContainerDataService>().InstancePerRequest();
            #endregion

            #region TradeCentric
            builder.RegisterType<TradeCentricService>().As<ITradeCentricService>().InstancePerRequest();
            #endregion

            #region Avatax
            builder.RegisterType<AvataxClient>().As<IAvataxClient>().InstancePerRequest();
            builder.RegisterType<AvataxMapper>().As<IAvataxMapper>().InstancePerRequest();
            builder.RegisterType<AvataxHelper>().As<IAvataxHelper>().InstancePerRequest();
            #endregion

            #region Promotion Assemblies Registration
            RegisterPromotionTypes(builder);
            #endregion

            #region Taxes Assemblies Registration
            RegisterTaxTypes(builder);
            #endregion

            #region Shipping Assemblies Registration
            RegisterShippingTypes(builder);
            #endregion

            #region Sms Provider Types Assemblies Registration
            RegisterSmsProviderTypes(builder);
            #endregion

            #region Recommendation Engine  
            builder.RegisterType<RecommendationService>().As<IRecommendationService>().InstancePerRequest();
            builder.RegisterType<RecommendationEngine>().As<IRecommendationEngine>().InstancePerRequest();
            #endregion

            #region Register Payment Client
            builder.RegisterType<PaymentClient>().As<IPaymentClient>().InstancePerDependency();
            #endregion

            #region Register Helper
            builder.RegisterType<PaymentHelper>().As<IPaymentHelper>().InstancePerDependency();
            #endregion

            #region Maintenance
            builder.RegisterType<MaintenanceService>().As<IMaintenanceService>().InstancePerRequest();
            #endregion Maintenance 

            #region Quick Order
            builder.RegisterType<QuickOrderService>().As<IQuickOrderService>().InstancePerRequest();
            #endregion

            #region SiteMap
            builder.RegisterType<SiteMapService>().As<ISiteMapService>().InstancePerRequest();
            #endregion
        }

        //Register Promotion Types
        void RegisterPromotionTypes(ContainerBuilder builder)
        {
            RegisterKeyedTypes<IZnodePromotionsType>(typeof(IZnodePromotionsType), builder);
        }

        //Register Tax Types
        void RegisterTaxTypes(ContainerBuilder builder)
        {
            RegisterKeyedTypes<IZnodeTaxesType>(typeof(IZnodeTaxesType), builder);
        }

        //Register Shipping Types
        void RegisterShippingTypes(ContainerBuilder builder)
        {
            RegisterKeyedTypes<IZnodeShippingsType>(typeof(IZnodeShippingsType), builder);
        }

        //Register SMS Provider Types
        void RegisterSmsProviderTypes(ContainerBuilder builder)
        {
            RegisterKeyedTypes<IZnodeSMSProvider>(typeof(IZnodeSMSProvider), builder);
        }

        //Register assemblies using key
        private void RegisterKeyedTypes<T>(Type interfaceType, ContainerBuilder builder) where T : class
        {
            List<Type> classList = GetClassesOfType(interfaceType);

            //Register each types
            foreach (Type type in classList)
            {
                builder.RegisterType(type).Keyed<T>(type.Name).InstancePerDependency();
            }
        }

        //Get Classes Of Interface Type
        private static List<Type> GetClassesOfType(Type type)
        {
            Assembly[] projectAssemblies = GetProjectAssemblies();

            List<Type> classList = new List<Type>();

            foreach (Assembly assembly in projectAssemblies)
            {
                List<Type> classes = assembly?.GetTypes()?.Where(t => t.GetInterfaces().Contains(type))?.ToList();
                if (classes?.Count > 0)
                {
                    classList.AddRange(classes);
                }
            }
            return classList;
        }

        //Get Project Assemblies
        private static Assembly[] GetProjectAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName.Contains("Znode.") || assembly.FullName.Contains(ZnodeApiSettings.CustomAssemblyLookupPrefix)).ToArray<Assembly>();
        }

        void RegisterServicesV2(ContainerBuilder builder)
        {
            builder.RegisterType<CategoryServiceV2>().As<ICategoryServiceV2>().InstancePerRequest();
            builder.RegisterType<UserServiceV2>().As<IUserServiceV2>().InstancePerRequest();
            builder.RegisterType<ShoppingCartServiceV2>().As<IShoppingCartServiceV2>().InstancePerRequest();
            builder.RegisterType<OrderServiceV2>().As<IOrderServiceV2>().InstancePerRequest();
            builder.RegisterType<PublishProductServiceV2>().As<IPublishProductServiceV2>().InstancePerRequest();
        }
    }
}
