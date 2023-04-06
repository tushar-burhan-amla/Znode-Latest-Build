using Autofac;
using Autofac.Integration.Mvc;
    
using System;

using Znode.Admin.Core.Helpers;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Api.Client;
using Znode.Engine.klaviyo.Client;
using Znode.Engine.Klaviyo.IClient;
using Znode.Libraries.DevExpress.Report;
using Znode.Libraries.Framework.Business;

namespace Znode.Admin.Core
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public virtual void Register(ContainerBuilder builder)
        {
            //Register Assemblies Includes the Controller & Agent.
            builder.RegisterControllers(AppDomain.CurrentDomain.GetAssemblies());

            #region Register Agent
            builder.RegisterType<UserAgent>().As<IUserAgent>().InstancePerLifetimeScope();
            builder.RegisterType<RoleAgent>().As<IRoleAgent>().InstancePerLifetimeScope();
            builder.RegisterType<AccountAgent>().As<IAccountAgent>().InstancePerLifetimeScope();
            builder.RegisterType<AccountQuoteAgent>().As<IAccountQuoteAgent>().InstancePerLifetimeScope();
            builder.RegisterType<AccessPermissionAgent>().As<IAccessPermissionAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CartAgent>().As<ICartAgent>().InstancePerLifetimeScope();
            builder.RegisterType<OrderAgent>().As<IOrderAgent>().InstancePerLifetimeScope();
            builder.RegisterType<GiftCardAgent>().As<IGiftCardAgent>().InstancePerLifetimeScope();
            builder.RegisterType<DynamicContentAgent>().As<IDynamicContentAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ShippingAgent>().As<IShippingAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ProviderEngineAgent>().As<IProviderEngineAgent>().InstancePerLifetimeScope();
            builder.RegisterType<NavigationAgent>().As<INavigationAgent>().InstancePerLifetimeScope();
            builder.RegisterType<MediaManagerAgent>().As<IMediaManagerAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerAgent>().As<ICustomerAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentAgent>().As<IPaymentAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ContentAgent>().As<IContentAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ManageMessageAgent>().As<IManageMessageAgent>().InstancePerLifetimeScope();
            builder.RegisterType<EmailTemplateAgent>().As<IEmailTemplateAgent>().InstancePerLifetimeScope();
            builder.RegisterType<RecommendationAgent>().As<IRecommendationAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerReviewAgent>().As<ICustomerReviewAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SEOSettingAgent>().As<ISEOSettingAgent>().InstancePerLifetimeScope();
            builder.RegisterType<UrlRedirectAgent>().As<IUrlRedirectAgent>().InstancePerLifetimeScope();
            builder.RegisterType<TemplateAgent>().As<ITemplateAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ThemeAgent>().As<IThemeAgent>().InstancePerLifetimeScope();
            builder.RegisterType<WebSiteAgent>().As<IWebSiteAgent>().InstancePerLifetimeScope();
            builder.RegisterType<StoreAgent>().As<IStoreAgent>().InstancePerLifetimeScope();
            builder.RegisterType<StoreExperienceAgent>().As<IStoreExperienceAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SliderAgent>().As<ISliderAgent>().InstancePerLifetimeScope();
            builder.RegisterType<DomainAgent>().As<IDomainAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SMTPAgent>().As<ISMTPAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PortalProfileAgent>().As<IPortalProfileAgent>().InstancePerLifetimeScope();
            builder.RegisterType<StoreUnitAgent>().As<IStoreUnitAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PriceAgent>().As<IPriceAgent>().InstancePerLifetimeScope();
            builder.RegisterType<EcommerceCatalogAgent>().As<IEcommerceCatalogAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PortalCountryAgent>().As<IPortalCountryAgent>().InstancePerLifetimeScope();
            builder.RegisterType<TaxClassAgent>().As<ITaxClassAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ImportAgent>().As<IImportAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PromotionAgent>().As<IPromotionAgent>().InstancePerLifetimeScope();
            builder.RegisterType<InventoryAgent>().As<IInventoryAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CountryAgent>().As<ICountryAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CurrencyAgent>().As<ICurrencyAgent>().InstancePerLifetimeScope();
            builder.RegisterType<GeneralSettingAgent>().As<IGeneralSettingAgent>().InstancePerLifetimeScope();
            builder.RegisterType<DashboardAgent>().As<IDashboardAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ReportAgent>().As<IReportAgent>().InstancePerLifetimeScope();
            builder.RegisterType<TouchPointConfigurationAgent>().As<ITouchPointConfigurationAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ERPConfiguratorAgent>().As<IERPConfiguratorAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ERPTaskSchedulerAgent>().As<IERPTaskSchedulerAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SearchConfigurationAgent>().As<ISearchConfigurationAgent>().InstancePerLifetimeScope();
            builder.RegisterType<WarehouseAgent>().As<IWarehouseAgent>().InstancePerLifetimeScope();//
            builder.RegisterType<MediaConfigurationAgent>().As<IMediaConfigurationAgent>().InstancePerLifetimeScope();
            builder.RegisterType<LicenseAgent>().As<ILicenseAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CaseRequestAgent>().As<ICaseRequestAgent>().InstancePerLifetimeScope();
            builder.RegisterType<MenuAgent>().As<IMenuAgent>().InstancePerLifetimeScope();
            builder.RegisterType<RoleAgent>().As<IRoleAgent>().InstancePerLifetimeScope();
            builder.RegisterType<RMARequestAgent>().As<IRMARequestAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileAgent>().As<IProfileAgent>().InstancePerLifetimeScope();
            builder.RegisterType<HighlightAgent>().As<IHighlightAgent>().InstancePerLifetimeScope();
            builder.RegisterType<AttributesAgent>().As<IAttributesAgent>().InstancePerLifetimeScope();
            builder.RegisterType<AttributeGroupAgent>().As<IAttributeGroupAgent>().InstancePerLifetimeScope();
            builder.RegisterType<AttributeFamilyAgent>().As<IAttributeFamilyAgent>().InstancePerLifetimeScope();
            builder.RegisterType<RMAConfigurationAgent>().As<IRMAConfigurationAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ProductFeedAgent>().As<IProductFeedAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ServerValidationAgent>().As<IServerValidationAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationSettingsAgent>().As<IApplicationSettingsAgent>().InstancePerLifetimeScope();
            builder.RegisterType<StoreLocatorAgent>().As<IStoreLocatorAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ERPConnectorAgent>().As<IERPConnectorAgent>().InstancePerLifetimeScope();
            builder.RegisterType<BlogNewsAgent>().As<IBlogNewsAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ContentContainerAgent>().As<IContentContainerAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ContainerTemplateAgent>().As<IContainerTemplateAgent>().InstancePerLifetimeScope();

            //PIM
            builder.RegisterType<ProductAgent>().As<IProductAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryAgent>().As<ICategoryAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CatalogAgent>().As<ICatalogAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PIMAttributeAgent>().As<IPIMAttributeAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PIMAttributeGroupAgent>().As<IPIMAttributeGroupAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PIMAttributeFamilyAgent>().As<IPIMAttributeFamilyAgent>().InstancePerLifetimeScope();
            builder.RegisterType<BrandAgent>().As<IBrandAgent>().InstancePerLifetimeScope();
            builder.RegisterType<VendorAgent>().As<IVendorAgent>().InstancePerLifetimeScope();
            builder.RegisterType<LocaleAgent>().As<ILocaleAgent>().InstancePerLifetimeScope();
            builder.RegisterType<AddonGroupAgent>().As<IAddonGroupAgent>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalAttributeAgent>().As<IGlobalAttributeAgent>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalAttributeGroupAgent>().As<IGlobalAttributeGroupAgent>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalAttributeFamilyAgent>().As<IGlobalAttributeFamilyAgent>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalAttributeEntityAgent>().As<IGlobalAttributeEntityAgent>().InstancePerLifetimeScope();
            builder.RegisterType<FormBuilderAgent>().As<IFormBuilderAgent>().InstancePerLifetimeScope();
            builder.RegisterType<FormSubmissionAgent>().As<IFormSubmissionAgent>().InstancePerLifetimeScope();
            builder.RegisterType<UrlManagementAgent>().As<IUrlManagementAgent>().InstancePerLifetimeScope();
            builder.RegisterType<QuoteAgent>().As<IQuoteAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SMSAgent>().As<ISMSAgent>().InstancePerLifetimeScope();

            // Log Message
            builder.RegisterType<LogMessageAgent>().As<ILogMessageAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PublishHistoryAgent>().As<IPublishHistoryAgent>().InstancePerLifetimeScope();
            //Search
            builder.RegisterType<SearchProfileAgent>().As<ISearchProfileAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SearchBoostAndBuryAgent>().As<ISearchBoostAndBuryAgent>().InstancePerLifetimeScope();
            //Export
            builder.RegisterType<ExportAgent>().As<IExportAgent>().InstancePerLifetimeScope();
            //Helper
            builder.RegisterType<HelperAgent>().As<IHelperAgent>().InstancePerLifetimeScope();

            builder.RegisterType<DevExpressReportAgent>().As<IDevExpressReportAgent>().InstancePerLifetimeScope();
            builder.RegisterType<TypeaheadAgent>().As<ITypeaheadAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SalesRepAgent>().As<ISalesRepAgent>().InstancePerLifetimeScope();

            builder.RegisterType<SearchReportAgent>().As<ISearchReportAgent>().InstancePerLifetimeScope();
             //PowerBI
            builder.RegisterType<PowerBIAgent>().As<IPowerBIAgent>().InstancePerLifetimeScope();

            builder.RegisterType<RMAReturnAgent>().As<IRMAReturnAgent>().InstancePerLifetimeScope();

            //Maintenance
            builder.RegisterType<MaintenanceAgent>().As<IMaintenanceAgent>().InstancePerLifetimeScope();

            #endregion

            #region Register Client
            builder.RegisterType<UserClient>().As<IUserClient>().InstancePerLifetimeScope();
            builder.RegisterType<PortalClient>().As<IPortalClient>().InstancePerLifetimeScope();
            builder.RegisterType<AccountClient>().As<IAccountClient>().InstancePerLifetimeScope();
            builder.RegisterType<DynamicContentClient>().As<IDynamicContentClient>().InstancePerLifetimeScope();
            builder.RegisterType<RecommendationClient>().As<IRecommendationClient>().InstancePerLifetimeScope();
            builder.RegisterType<RoleClient>().As<IRoleClient>().InstancePerLifetimeScope();
            builder.RegisterType<DashboardClient>().As<IDashboardClient>().InstancePerLifetimeScope();
            builder.RegisterType<DomainClient>().As<IDomainClient>().InstancePerLifetimeScope();
            builder.RegisterType<ShippingClient>().As<IShippingClient>().InstancePerLifetimeScope();
            builder.RegisterType<ShippingTypeClient>().As<IShippingTypeClient>().InstancePerLifetimeScope();
            builder.RegisterType<StateClient>().As<IStateClient>().InstancePerLifetimeScope();
            builder.RegisterType<CityClient>().As<ICityClient>().InstancePerLifetimeScope();
            builder.RegisterType<OrderClient>().As<IOrderClient>().InstancePerLifetimeScope();
            builder.RegisterType<EcommerceCatalogClient>().As<IEcommerceCatalogClient>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerClient>().As<ICustomerClient>().InstancePerLifetimeScope();
            builder.RegisterType<PublishProductClient>().As<IPublishProductClient>().InstancePerLifetimeScope();
            builder.RegisterType<MediaConfigurationClient>().As<IMediaConfigurationClient>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentClient>().As<IPaymentClient>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartClient>().As<IShoppingCartClient>().InstancePerLifetimeScope();
            builder.RegisterType<AccountQuoteClient>().As<IAccountQuoteClient>().InstancePerLifetimeScope();
            builder.RegisterType<OrderStateClient>().As<IOrderStateClient>().InstancePerLifetimeScope();
            builder.RegisterType<CurrencyClient>().As<ICurrencyClient>().InstancePerLifetimeScope();
            builder.RegisterType<ContentPageClient>().As<IContentPageClient>().InstancePerLifetimeScope();
            builder.RegisterType<CMSWidgetConfigurationClient>().As<ICMSWidgetConfigurationClient>().InstancePerLifetimeScope();
            builder.RegisterType<PortalProfileClient>().As<IPortalProfileClient>().InstancePerLifetimeScope();
            builder.RegisterType<TemplateClient>().As<ITemplateClient>().InstancePerLifetimeScope();
            builder.RegisterType<SEOSettingClient>().As<ISEOSettingClient>().InstancePerLifetimeScope();
            builder.RegisterType<ManageMessageClient>().As<IManageMessageClient>().InstancePerLifetimeScope();
            builder.RegisterType<ThemeClient>().As<IThemeClient>().InstancePerLifetimeScope();
            builder.RegisterType<CSSClient>().As<ICSSClient>().InstancePerLifetimeScope();
            builder.RegisterType<WebSiteClient>().As<IWebSiteClient>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerReviewClient>().As<ICustomerReviewClient>().InstancePerLifetimeScope();
            builder.RegisterType<EmailTemplateClient>().As<IEmailTemplateClient>().InstancePerLifetimeScope();
            builder.RegisterType<PromotionClient>().As<IPromotionClient>().InstancePerLifetimeScope();
            builder.RegisterType<PromotionTypeClient>().As<IPromotionTypeClient>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileClient>().As<IProfileClient>().InstancePerLifetimeScope();
            builder.RegisterType<PublishCategoryClient>().As<IPublishCategoryClient>().InstancePerLifetimeScope();
            builder.RegisterType<PublishCatalogClient>().As<IPublishCatalogClient>().InstancePerLifetimeScope();
            builder.RegisterType<CountryClient>().As<ICountryClient>().InstancePerLifetimeScope();
            builder.RegisterType<GeneralSettingClient>().As<IGeneralSettingClient>().InstancePerLifetimeScope();
            builder.RegisterType<TaxRuleTypeClient>().As<ITaxRuleTypeClient>().InstancePerLifetimeScope();
            builder.RegisterType<TaxClassClient>().As<ITaxClassClient>().InstancePerLifetimeScope();
            builder.RegisterType<ReportClient>().As<IReportClient>().InstancePerLifetimeScope();
            builder.RegisterType<PriceClient>().As<IPriceClient>().InstancePerLifetimeScope();
            builder.RegisterType<WarehouseClient>().As<IWarehouseClient>().InstancePerLifetimeScope();
            builder.RegisterType<TouchPointConfigurationClient>().As<ITouchPointConfigurationClient>().InstancePerLifetimeScope();
            builder.RegisterType<ERPTaskSchedulerClient>().As<IERPTaskSchedulerClient>().InstancePerLifetimeScope();
            builder.RegisterType<ERPConfiguratorClient>().As<IERPConfiguratorClient>().InstancePerLifetimeScope();
            builder.RegisterType<SearchClient>().As<ISearchClient>().InstancePerLifetimeScope();
            builder.RegisterType<LicenseClient>().As<ILicenseClient>().InstancePerLifetimeScope();
            builder.RegisterType<WebStoreCaseRequestClient>().As<IWebStoreCaseRequestClient>().InstancePerLifetimeScope();
            builder.RegisterType<MenuClient>().As<IMenuClient>().InstancePerLifetimeScope();
            builder.RegisterType<RoleClient>().As<IRoleClient>().InstancePerLifetimeScope();
            builder.RegisterType<AccessPermissionClient>().As<IAccessPermissionClient>().InstancePerLifetimeScope();
            builder.RegisterType<GiftCardClient>().As<IGiftCardClient>().InstancePerLifetimeScope();
            builder.RegisterType<RMARequestItemClient>().As<IRMARequestItemClient>().InstancePerLifetimeScope();
            builder.RegisterType<RMARequestClient>().As<IRMARequestClient>().InstancePerLifetimeScope();
            builder.RegisterType<PortalUnitClient>().As<IPortalUnitClient>().InstancePerLifetimeScope();
            builder.RegisterType<RMAConfigurationClient>().As<IRMAConfigurationClient>().InstancePerLifetimeScope();
            builder.RegisterType<ProductReviewStateClient>().As<IProductReviewStateClient>().InstancePerLifetimeScope();
            builder.RegisterType<PortalCountryClient>().As<IPortalCountryClient>().InstancePerLifetimeScope();
            builder.RegisterType<HighlightClient>().As<IHighlightClient>().InstancePerLifetimeScope();
            builder.RegisterType<InventoryClient>().As<IInventoryClient>().InstancePerLifetimeScope();
            builder.RegisterType<WarehouseClient>().As<IWarehouseClient>().InstancePerLifetimeScope();
            builder.RegisterType<AttributesClient>().As<IAttributesClient>().InstancePerLifetimeScope();
            builder.RegisterType<AttributeGroupClient>().As<IAttributeGroupClient>().InstancePerLifetimeScope();
            builder.RegisterType<AttributeFamilyClient>().As<IAttributeFamilyClient>().InstancePerLifetimeScope();
            builder.RegisterType<ServerValidationClient>().As<IServerValidationClient>().InstancePerLifetimeScope();
            builder.RegisterType<MediaManagerClient>().As<IMediaManagerClient>().InstancePerLifetimeScope();
            builder.RegisterType<SliderClient>().As<ISliderClient>().InstancePerLifetimeScope();
            builder.RegisterType<UrlRedirectClient>().As<IUrlRedirectClient>().InstancePerLifetimeScope();
            builder.RegisterType<ImportClient>().As<IImportClient>().InstancePerLifetimeScope();
            builder.RegisterType<ExportClient>().As<IExportClient>().InstancePerLifetimeScope();
            builder.RegisterType<PriceClient>().As<IPriceClient>().InstancePerLifetimeScope();
            builder.RegisterType<NavigationClient>().As<INavigationClient>().InstancePerLifetimeScope();
            builder.RegisterType<SMTPClient>().As<ISMTPClient>().InstancePerLifetimeScope();
            builder.RegisterType<CurrencyClient>().As<ICurrencyClient>().InstancePerLifetimeScope();
            builder.RegisterType<WeightUnitClient>().As<IWeightUnitClient>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultGlobalConfigClient>().As<IDefaultGlobalConfigClient>().InstancePerLifetimeScope();
            builder.RegisterType<StoreLocatorClient>().As<IStoreLocatorClient>().InstancePerLifetimeScope();
            builder.RegisterType<ProductFeedClient>().As<IProductFeedClient>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationSettingsClient>().As<IApplicationSettingsClient>().InstancePerLifetimeScope();
            builder.RegisterType<ERPConnectorClient>().As<IERPConnectorClient>().InstancePerLifetimeScope();
            builder.RegisterType<TagManagerClient>().As<ITagManagerClient>().InstancePerLifetimeScope();
            builder.RegisterType<BlogNewsClient>().As<IBlogNewsClient>().InstancePerLifetimeScope();
            builder.RegisterType<FormBuilderClient>().As<IFormBuilderClient>().InstancePerLifetimeScope();
            builder.RegisterType<FormSubmissionClient>().As<IFormSubmissionClient>().InstancePerLifetimeScope();
            builder.RegisterType<AddressClient>().As<IAddressClient>().InstancePerLifetimeScope();
            builder.RegisterType<CMSPageSearchClient>().As<ICMSPageSearchClient>().InstancePerLifetimeScope();
            builder.RegisterType<QuoteClient>().As<IQuoteClient>().InstancePerLifetimeScope();
            builder.RegisterType<RMAReturnClient>().As<IRMAReturnClient>().InstancePerLifetimeScope();
            builder.RegisterType<ContentContainerClient>().As<IContentContainerClient>().InstancePerLifetimeScope();
            builder.RegisterType<ContainerTemplateClient>().As<IContainerTemplateClient>().InstancePerLifetimeScope();
            builder.RegisterType<PortalSMSClient>().As<IPortalSMSClient>().InstancePerLifetimeScope();


            //PIM
            builder.RegisterType<ProductsClient>().As<IProductsClient>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryClient>().As<ICategoryClient>().InstancePerLifetimeScope();
            builder.RegisterType<CatalogClient>().As<ICatalogClient>().InstancePerLifetimeScope();
            builder.RegisterType<PIMAttributeClient>().As<IPIMAttributeClient>().InstancePerLifetimeScope();
            builder.RegisterType<PIMAttributeGroupClient>().As<IPIMAttributeGroupClient>().InstancePerLifetimeScope();
            builder.RegisterType<PIMAttributeFamilyClient>().As<IPIMAttributeFamilyClient>().InstancePerLifetimeScope();
            builder.RegisterType<BrandClient>().As<IBrandClient>().InstancePerLifetimeScope();
            builder.RegisterType<VendorClient>().As<IVendorClient>().InstancePerLifetimeScope();
            builder.RegisterType<LocaleClient>().As<ILocaleClient>().InstancePerLifetimeScope();
            builder.RegisterType<AddonGroupClient>().As<IAddonGroupClient>().InstancePerLifetimeScope();

            //GlobalAttribute
            builder.RegisterType<GlobalAttributeClient>().As<IGlobalAttributeClient>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalAttributeGroupClient>().As<IGlobalAttributeGroupClient>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalAttributeEntityClient>().As<IGlobalAttributeEntityClient>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalAttributeFamilyClient>().As<IGlobalAttributeFamilyClient>().InstancePerLifetimeScope();

            //Search Profile
            builder.RegisterType<SearchProfileClient>().As<ISearchProfileClient>().InstancePerLifetimeScope();
            builder.RegisterType<SearchBoostAndBuryClient>().As<ISearchBoostAndBuryClient>().InstancePerLifetimeScope();

            //Log Message
            builder.RegisterType<LogMessageClient>().As<ILogMessageClient>().InstancePerLifetimeScope();
            //Helper
            builder.RegisterType<HelperClient>().As<IHelperClient>().InstancePerLifetimeScope();
            builder.RegisterType<PublishHistoryClient>().As<IPublishHistoryClient>().InstancePerLifetimeScope();

            builder.RegisterType<DevExpressReportClient>().As<IDevExpressReportClient>().InstancePerLifetimeScope();
            builder.RegisterType<TypeaheadClient>().As<ITypeaheadClient>().InstancePerLifetimeScope();
            builder.RegisterType<SearchReportClient>().As<ISearchReportClient>().InstancePerLifetimeScope();

            
            //Maintenance
            builder.RegisterType<MaintenanceClient>().As<IMaintenanceClient>().InstancePerLifetimeScope();
            #endregion

            #region Register Class
            builder.RegisterType<AuthenticationHelper>().As<AuthenticationHelper>().InstancePerLifetimeScope();
            #endregion

            builder.RegisterType<ReportHelper>().As<IReportHelper>().InstancePerLifetimeScope();
            builder.RegisterType<ReportFilterHelper>().As<IReportFilterHelper>().InstancePerLifetimeScope();
            builder.RegisterType<AuthenticationHelper>().As<IAuthenticationHelper>().InstancePerLifetimeScope();
            builder.RegisterType<DependencyHelper>().As<IDependencyHelper>().InstancePerLifetimeScope();
            builder.RegisterType<PublishPopupHelper>().As<IPublishPopupHelper>().InstancePerLifetimeScope();
            builder.RegisterType<AdminHelper>().As<IAdminHelper>().InstancePerLifetimeScope();
            builder.RegisterType<ERPTaskSchedulerHelper>().As<IERPTaskSchedulerHelper>().InstancePerLifetimeScope();

            #region Klaviyo
            builder.RegisterType<KlaviyoClient>().As<IKlaviyoClient>().InstancePerLifetimeScope();

            builder.RegisterType<KlaviyoAgent>().As<IKlaviyoAgent>().InstancePerRequest();
            #endregion

 }

        public int Order
        {
            get { return 0; }
        }
    }
}
