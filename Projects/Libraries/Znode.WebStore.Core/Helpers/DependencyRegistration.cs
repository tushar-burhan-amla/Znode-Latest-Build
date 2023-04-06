using Autofac;
using Autofac.Integration.Mvc;

using System;

using Znode.Engine.Api.Client;
using Znode.Engine.klaviyo.Client;
using Znode.Engine.Klaviyo.IClient;
using Znode.Engine.WebStore.Agents;
using Znode.Libraries.Framework.Business;
using Znode.WebStore.Core.Agents;
using Znode.WebStore.Helpers;

namespace Znode.Engine.WebStore
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public virtual void Register(ContainerBuilder builder)
        {
            //Register Assemblies Includes the Controller & Agent.
            builder.RegisterControllers(AppDomain.CurrentDomain.GetAssemblies());

            #region Register Agent
            builder.RegisterType<UserAgent>().As<IUserAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CartAgent>().As<ICartAgent>().InstancePerLifetimeScope();
            builder.RegisterType<BrandAgent>().As<IBrandAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryAgent>().As<ICategoryAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CaseRequestAgent>().As<ICaseRequestAgent>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutAgent>().As<ICheckoutAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentAgent>().As<IPaymentAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PortalAgent>().As<IPortalAgent>().InstancePerLifetimeScope();
            builder.RegisterType<StoreLocatorAgent>().As<IStoreLocatorAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAgent>().As<IProductAgent>().InstancePerLifetimeScope();
            builder.RegisterType<WidgetDataAgent>().As<IWidgetDataAgent>().InstancePerLifetimeScope();
            builder.RegisterType<MediaManagerAgent>().As<IMediaManagerAgent>().InstancePerLifetimeScope();
            builder.RegisterType<AttributeAgent>().As<IAttributeAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SearchAgent>().As<ISearchAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationSettingsAgent>().As<IApplicationSettingsAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ClearCacheAgent>().As<IClearCacheAgent>().InstancePerLifetimeScope();
            builder.RegisterType<BlogNewsAgent>().As<IBlogNewsAgent>().InstancePerLifetimeScope();
            builder.RegisterType<WSPromotionAgent>().As<IWSPromotionAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SiteMapAgent>().As<ISiteMapAgent>().InstancePerLifetimeScope();
            builder.RegisterType<FormBuilderAgent>().As<IFormBuilderAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ImportAgent>().As<IImportAgent>().InstancePerLifetimeScope();
            builder.RegisterType<MessageAgent>().As<IMessageAgent>().InstancePerLifetimeScope();
            builder.RegisterType<UrlRedirectAgent>().As<IUrlRedirectAgent>().InstancePerLifetimeScope();
            builder.RegisterType<AddressAgent>().As<IAddressAgent>().InstancePerLifetimeScope();
            builder.RegisterType<ECertAgent>().As<IECertAgent>().InstancePerLifetimeScope();
            builder.RegisterType<RecommendationAgent>().As<IRecommendationAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SearchReportAgent>().As<ISearchReportAgent>().InstancePerLifetimeScope();
            builder.RegisterType<QuoteAgent>().As<IQuoteAgent>().InstancePerLifetimeScope();
            builder.RegisterType<RMAReturnAgent>().As<IRMAReturnAgent>().InstancePerLifetimeScope();
            builder.RegisterType<TypeaheadAgent>().As<ITypeaheadAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PowerBIAgent>().As<IPowerBIAgent>().InstancePerLifetimeScope();
            builder.RegisterType<SaveForLaterAgent>().As<ISaveForLaterAgent>().InstancePerRequest();
            builder.RegisterType<SavedCartAgent>().As<ISavedCartAgent>().InstancePerRequest();
            #endregion

            #region Register Client
            builder.RegisterType<UserClient>().As<IUserClient>().InstancePerLifetimeScope();
          
            builder.RegisterType<PortalClient>().As<IPortalClient>().InstancePerLifetimeScope();
            builder.RegisterType<AccountClient>().As<IAccountClient>().InstancePerLifetimeScope();
            builder.RegisterType<RoleClient>().As<IRoleClient>().InstancePerLifetimeScope();
            builder.RegisterType<DomainClient>().As<IDomainClient>().InstancePerLifetimeScope();
            builder.RegisterType<ShippingClient>().As<IShippingClient>().InstancePerLifetimeScope();
            builder.RegisterType<ShippingTypeClient>().As<IShippingTypeClient>().InstancePerLifetimeScope();
            builder.RegisterType<StateClient>().As<IStateClient>().InstancePerLifetimeScope();
            builder.RegisterType<CityClient>().As<ICityClient>().InstancePerLifetimeScope();
            builder.RegisterType<ProductsClient>().As<IProductsClient>().InstancePerLifetimeScope();
            builder.RegisterType<BrandClient>().As<IBrandClient>().InstancePerLifetimeScope();
            builder.RegisterType<OrderClient>().As<IOrderClient>().InstancePerLifetimeScope();
            builder.RegisterType<EcommerceCatalogClient>().As<IEcommerceCatalogClient>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerClient>().As<ICustomerClient>().InstancePerLifetimeScope();
            builder.RegisterType<PublishProductClient>().As<IPublishProductClient>().InstancePerLifetimeScope();
            builder.RegisterType<MediaConfigurationClient>().As<IMediaConfigurationClient>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentClient>().As<IPaymentClient>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartClient>().As<IShoppingCartClient>().InstancePerLifetimeScope();
            builder.RegisterType<AccountQuoteClient>().As<IAccountQuoteClient>().InstancePerLifetimeScope();
            builder.RegisterType<OrderStateClient>().As<IOrderStateClient>().InstancePerLifetimeScope();
            builder.RegisterType<PIMAttributeClient>().As<IPIMAttributeClient>().InstancePerLifetimeScope();
            builder.RegisterType<CurrencyClient>().As<ICurrencyClient>().InstancePerLifetimeScope();
            builder.RegisterType<WebStoreCaseRequestClient>().As<IWebStoreCaseRequestClient>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationSettingsClient>().As<IApplicationSettingsClient>().InstancePerLifetimeScope();
            builder.RegisterType<WebStoreCategoryClient>().As<IWebStoreCategoryClient>().InstancePerLifetimeScope();
            builder.RegisterType<PublishCategoryClient>().As<IPublishCategoryClient>().InstancePerLifetimeScope();
            builder.RegisterType<IDynamicContentClient>().As<IDynamicContentClient>().InstancePerLifetimeScope();
            builder.RegisterType<PortalProfileClient>().As<IPortalProfileClient>().InstancePerLifetimeScope();
            builder.RegisterType<WebStoreUserClient>().As<IWebStoreUserClient>().InstancePerLifetimeScope();
            builder.RegisterType<WebStoreMessageClient>().As<IWebStoreMessageClient>().InstancePerLifetimeScope();
            builder.RegisterType<WebStorePortalClient>().As<IWebStorePortalClient>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerReviewClient>().As<ICustomerReviewClient>().InstancePerLifetimeScope();
            builder.RegisterType<WebStoreProductClient>().As<IWebStoreProductClient>().InstancePerLifetimeScope();
            builder.RegisterType<SearchClient>().As<ISearchClient>().InstancePerLifetimeScope();
            builder.RegisterType<HighlightClient>().As<IHighlightClient>().InstancePerLifetimeScope();
            builder.RegisterType<WebStoreLocatorClient>().As<IWebStoreLocatorClient>().InstancePerLifetimeScope();
            builder.RegisterType<UrlRedirectClient>().As<IUrlRedirectClient>().InstancePerLifetimeScope();
            builder.RegisterType<WebStoreWidgetClient>().As<IWebStoreWidgetClient>().InstancePerLifetimeScope();
            builder.RegisterType<MediaManagerClient>().As<IMediaManagerClient>().InstancePerLifetimeScope();
            builder.RegisterType<CountryClient>().As<ICountryClient>().InstancePerLifetimeScope();
            builder.RegisterType<WishListClient>().As<IWishListClient>().InstancePerLifetimeScope();
            builder.RegisterType<GiftCardClient>().As<IGiftCardClient>().InstancePerLifetimeScope();
            builder.RegisterType<PortalCountryClient>().As<IPortalCountryClient>().InstancePerLifetimeScope();
            builder.RegisterType<BlogNewsClient>().As<IBlogNewsClient>().InstancePerLifetimeScope();
            builder.RegisterType<PromotionClient>().As<IPromotionClient>().InstancePerLifetimeScope();
            builder.RegisterType<SiteMapClient>().As<ISiteMapClient>().InstancePerLifetimeScope();
            builder.RegisterType<FormBuilderClient>().As<IFormBuilderClient>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalAttributeEntityClient>().As<IGlobalAttributeEntityClient>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalAttributeClient>().As<IGlobalAttributeClient>().InstancePerLifetimeScope();
            builder.RegisterType<ImportClient>().As<IImportClient>().InstancePerLifetimeScope();
            builder.RegisterType<AddressClient>().As<IAddressClient>().InstancePerLifetimeScope();
            builder.RegisterType<ECertClient>().As<IECertClient>().InstancePerLifetimeScope();
            builder.RegisterType<ContentPageClient>().As<IContentPageClient>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultGlobalConfigClient>().As<IDefaultGlobalConfigClient>().InstancePerLifetimeScope();
            builder.RegisterType<RecommendationClient>().As<IRecommendationClient>().InstancePerLifetimeScope();
            builder.RegisterType<PublishBrandClient>().As<IPublishBrandClient>().InstancePerLifetimeScope();
            builder.RegisterType<SearchReportClient>().As<ISearchReportClient>().InstancePerLifetimeScope();
            builder.RegisterType<CMSPageSearchClient>().As<ICMSPageSearchClient>().InstancePerLifetimeScope();
            builder.RegisterType<RMAReturnClient>().As<IRMAReturnClient>().InstancePerLifetimeScope();
            builder.RegisterType<RMAConfigurationClient>().As<IRMAConfigurationClient>().InstancePerLifetimeScope();
            builder.RegisterType<TypeaheadClient>().As<ITypeaheadClient>().InstancePerLifetimeScope();
            builder.RegisterType<QuoteClient>().As<IQuoteClient>().InstancePerLifetimeScope();
            builder.RegisterType<GeneralSettingClient>().As<IGeneralSettingClient>().InstancePerLifetimeScope();
            builder.RegisterType<QuickOrderClient>().As<IQuickOrderClient>().InstancePerLifetimeScope();
            builder.RegisterType<SaveForLaterClient>().As<ISaveForLaterClient>().InstancePerRequest();
            builder.RegisterType<SavedCartClient>().As<ISavedCartClient>().InstancePerRequest();
            builder.RegisterType<KlaviyoClient>().As<IKlaviyoClient>().InstancePerLifetimeScope();

            #endregion

            #region Register Class

            builder.RegisterType<AuthenticationHelper>().As<IAuthenticationHelper>().InstancePerLifetimeScope();
            builder.RegisterType<SEOUrlRouteData>().As<ISEOUrlRouteData>().InstancePerLifetimeScope();
            builder.RegisterType<WebstoreHelper>().As<IWebstoreHelper>().InstancePerLifetimeScope();
            builder.RegisterType<DownloadHelper>().As<IDownloadHelper>().InstancePerLifetimeScope();

            #endregion

            #region TradeCentric
            builder.RegisterType<TradeCentricClient>().As<ITradeCentricClient>().InstancePerLifetimeScope();
            builder.RegisterType<TradeCentricAgent>().As<ITradeCentricAgent>().InstancePerLifetimeScope();
            #endregion
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
