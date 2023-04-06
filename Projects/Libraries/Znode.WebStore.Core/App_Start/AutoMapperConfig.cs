using Autofac;
using AutoMapper;
using System;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Core.ViewModels;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.WebStore.Core.ViewModels;
using Znode.WebStore.ViewModels;

namespace Znode.Engine.WebStore
{
    public static class AutoMapperConfig
    {
        public static void Execute()
        {
            Mapper.CreateMap<ZnodeTypedParameter, TypedParameter>();

            Mapper.CreateMap<ZnodeNamedParameter, NamedParameter>();

            Mapper.CreateMap<ProductReviewViewModel, CustomerReviewModel>();

            Mapper.CreateMap<CustomerReviewModel, ProductReviewViewModel>()
                 .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()));

            Mapper.CreateMap<CategoryHeaderViewModel, WebStoreCategoryModel>().ReverseMap()
        .ForMember(d => d.CategoryName, opt => opt.MapFrom(src => src.Name))
        .ForMember(d => d.CategoryId, opt => opt.MapFrom(src => src.PublishCategoryId))
        .ForMember(d => d.SEOPageName, opt => opt.MapFrom(src => src.SEODetails.SEOPageName))
        .ForMember(d => d.SubCategoryItems, opt => opt.MapFrom(src => src.SubCategories))
            .ForMember(d => d.Attributes, opt => opt.MapFrom(src => src.Attributes));

            Mapper.CreateMap<CategorySubHeaderViewModel, WebStoreCategoryModel>().ReverseMap()
         .ForMember(d => d.CategoryName, opt => opt.MapFrom(src => src.Name))
          .ForMember(d => d.CategoryId, opt => opt.MapFrom(src => src.PublishCategoryId))
          .ForMember(d => d.ChildCategoryItems, opt => opt.MapFrom(src => src.SubCategories))
           .ForMember(d => d.ParentCategories, opt => opt.MapFrom(src => src.ParentCategory))
         .ForMember(d => d.SEOPageName, opt => opt.MapFrom(src => src.SEODetails.SEOPageName))
          .ForMember(d => d.Attributes, opt => opt.MapFrom(src => src.Attributes));

            Mapper.CreateMap<BannerModel, SliderBannerViewModel>()
         .ForMember(d => d.SliderBannerId, opt => opt.MapFrom(src => src.CMSSliderBannerId))
         .ForMember(d => d.SliderBannerTitle, opt => opt.MapFrom(src => src.Title)).ReverseMap();

            Mapper.CreateMap<CMSWidgetConfigurationModel, WidgetSliderBannerViewModel>()
         .ForMember(d => d.WidgetSliderBannerId, opt => opt.MapFrom(src => src.CMSWidgetSliderBannerId))
         .ForMember(d => d.SliderId, opt => opt.MapFrom(src => src.CMSSliderId))
         .ForMember(d => d.MappingId, opt => opt.MapFrom(src => src.CMSMappingId)).ReverseMap();


            Mapper.CreateMap<CMSMediaWidgetConfigurationModel, WidgetMediaViewModel>()
         .ForMember(d => d.MediaPath, opt => opt.MapFrom(src => src.MediaPath))
         .ForMember(d => d.DisplayName, opt => opt.MapFrom(src => src.DisplayName));

            Mapper.CreateMap<PaymentHistoryModel, PaymentHistoryViewModel>()
         .ForMember(d => d.OrderDateWithTime, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat())).ReverseMap();                   

            Mapper.CreateMap<LocaleModel, LocaleModel>();

            Mapper.CreateMap<WebStorePortalModel, PortalViewModel>()
         .ForMember(d => d.Locales, opt => opt.MapFrom(src => src.PortalLocales))
         .ForMember(d => d.Css, opt => opt.MapFrom(src => src.CSSName))
         .ForMember(d => d.Theme, opt => opt.MapFrom(src => src.ThemeName))
         .ForMember(d => d.ParentTheme, opt => opt.MapFrom(src => src.ParentThemeName))
         .ForMember(d => d.Name, opt => opt.MapFrom(src => src.StoreName)).ReverseMap();

            Mapper.CreateMap<StoreLocatorViewModel, StoreLocatorDataModel>().ReverseMap();

            Mapper.CreateMap<AddressViewModel, AddressModel>()
                .ForMember(d => d.PostalCode, opt => opt.MapFrom(src => src.PostalCode.Trim())).ReverseMap();

            Mapper.CreateMap<StoreLocatorListModel, StoreLocatorViewModel>().ReverseMap();

            Mapper.CreateMap<WebStoreCaseRequestModel, CaseRequestViewModel>().ReverseMap();


            Mapper.CreateMap<CategoryViewModel, WebStoreCategoryModel>().ReverseMap()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(src => src.Name));

            Mapper.CreateMap<WebStoreSEOModel, SEOViewModel>()
                 .ForMember(d => d.SEOUrl, opt => opt.MapFrom(src => src.SEOPageName))
                .ReverseMap();

            Mapper.CreateMap<AttributesViewModel, PublishAttributeModel>().ReverseMap();

            Mapper.CreateMap<ProductViewModel, WebStoreProductModel>()
              .ForMember(d => d.RetailPrice, opt => opt.MapFrom(src => src.RetailPrice.ToPriceRoundOff()))
               .ForMember(d => d.SalesPrice, opt => opt.MapFrom(src => src.SalesPrice.ToPriceRoundOff()));

            Mapper.CreateMap<WebStoreProductModel, ProductViewModel>();

            Mapper.CreateMap<WebStoreWidgetProductModel, WidgetProductViewModel>()
                .ForMember(d => d.ProductViewModel, opt => opt.MapFrom(src => src.WebStoreProductModel));

            Mapper.CreateMap<LinkWidgetConfigurationModel, WidgetTitleViewModel>()
                  .ForMember(d => d.WidgetTitleConfigurationId, opt => opt.MapFrom(src => src.CMSWidgetTitleConfigurationId))
                   .ForMember(d => d.MappingId, opt => opt.MapFrom(src => src.CMSMappingId))
                    .ForMember(d => d.WidgetTitle, opt => opt.MapFrom(src => src.Title));

            Mapper.CreateMap<WebStoreWidgetCategoryModel, WidgetCategoryViewModel>()
                .ForMember(x => x.CategoryViewModel, opt => opt.MapFrom(model => model.PublishCategoryModel));

            Mapper.CreateMap<WebStoreWidgetBrandModel, WidgetBrandViewModel>()
               .ForMember(x => x.BrandViewModel, opt => opt.MapFrom(model => model.BrandModel));

            Mapper.CreateMap<WebStoreWidgetCategoryModel, CategoryViewModel>()
                .ForMember(x => x.Attributes, opt => opt.MapFrom(model => model.PublishCategoryModel.Attributes))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(model => model.PublishCategoryModel.Name));

            Mapper.CreateMap<WebStoreContentPageModel, ContentPageViewModel>().ReverseMap();
            Mapper.CreateMap<WebStoreContentPageModel, WidgetTextViewModel>().ReverseMap();
            Mapper.CreateMap<KeywordSearchModel, KeywordSearchViewModel>().ReverseMap();

            Mapper.CreateMap<ProductViewModel, PublishProductModel>().ReverseMap();
            Mapper.CreateMap<SearchProductModel, ProductViewModel>()
                 .ForMember(x => x.PublishProductId, opt => opt.MapFrom(model => model.ZnodeProductId));

            Mapper.CreateMap<ShoppingCartModel, CartViewModel>()
              .ForMember(d => d.SubTotal, opt => opt.MapFrom(src => src.SubTotal.ToPriceRoundOff()))
                .ForMember(d => d.Total, opt => opt.MapFrom(src => src.Total.ToPriceRoundOff()))
                .ForMember(d => d.Discount, opt => opt.MapFrom(src => src.Discount.ToPriceRoundOff()))
                .ForMember(d => d.ShippingResponseErrorMessage, opt => opt.MapFrom(src => src.Shipping.ResponseMessage))
                .ForMember(d => d.IsValidShippingSetting, opt => opt.MapFrom(src => src.Shipping.IsValidShippingSetting));

            Mapper.CreateMap<CartViewModel, ShoppingCartModel>()
           .ForMember(d => d.SubTotal, opt => opt.MapFrom(src => src.SubTotal.ToPriceRoundOff()))
             .ForMember(d => d.Total, opt => opt.MapFrom(src => src.Total.ToPriceRoundOff()))
             .ForMember(d => d.Discount, opt => opt.MapFrom(src => src.Discount.ToPriceRoundOff()))
               .ForMember(d => d.OrderTotalWithoutVoucher, opt => opt.MapFrom(src => src.OrderTotalWithoutVoucher.ToPriceRoundOff()));

            Mapper.CreateMap<ShoppingCartModel, AccountQuoteViewModel>();
            Mapper.CreateMap<AccountQuoteViewModel, ShoppingCartModel>();

            Mapper.CreateMap<ShoppingCartItemModel, CartItemViewModel>()
                  .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
                  .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.ToPriceRoundOff()))
                  .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity));

            Mapper.CreateMap<CartItemViewModel, ShoppingCartItemModel>()
                .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
                .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.ToPriceRoundOff()))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity));

            Mapper.CreateMap<TemplateCartItemViewModel, TemplateCartItemModel>()
              .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
              .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.ToPriceRoundOff()))
              .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()));

            Mapper.CreateMap<TemplateCartItemModel, TemplateCartItemViewModel>()
             .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
             .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.ToPriceRoundOff()))
             .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()));

            Mapper.CreateMap<TemplateCartItemModel, CartItemViewModel>()
            .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
            .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.ToPriceRoundOff()))
            .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()));

            Mapper.CreateMap<AccountTemplateModel, TemplateViewModel>()
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<SEOUrlModel, SEOUrlViewModel>().ReverseMap();

            Mapper.CreateMap<FacetViewModel, SearchFacetModel>().ReverseMap();

            Mapper.CreateMap<FacetValueViewModel, SearchFacetValueModel>().ReverseMap();

            Mapper.CreateMap<AddOnViewModel, WebStoreAddOnModel>().ReverseMap();

            Mapper.CreateMap<WebStoreAddOnValueModel, AddOnValuesViewModel>();

            Mapper.CreateMap<BundleProductViewModel, WebStoreBundleProductModel>().ReverseMap();

            Mapper.CreateMap<HighlightsViewModel, HighlightModel>().ReverseMap();

            Mapper.CreateMap<WishListViewModel, WishListModel>()
              .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat()));

            Mapper.CreateMap<WishListModel, WishListViewModel>()
                  .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat()));

            Mapper.CreateMap<UserModel, UserViewModel>()
               .ForMember(d => d.Email, opt => opt.MapFrom(src => Equals(src.User, null) ? string.Empty : src.User.Email))
               .ForMember(d => d.UserName, opt => opt.MapFrom(src => Equals(src.User, null) ? src.UserName : src.User.Username))
               .ForMember(d => d.AspNetUserId, opt => opt.MapFrom(src => Equals(src.User, null) ? src.AspNetUserId : src.User.UserId))
               .ForMember(d => d.WishList, opt => opt.MapFrom(src => src.WishList))
               .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat()));
               
             Mapper.CreateMap<UserViewModel, UserModel>()
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat()));
                
            Mapper.CreateMap<LoginViewModel, LoginUserModel>();

            Mapper.CreateMap<LoginViewModel, UserModel>()
               .ForMember(d => d.User, opt => opt.MapFrom(s => Mapper.Map<LoginViewModel, LoginUserModel>(s)));

            Mapper.CreateMap<ConfigurableAttributeModel, ProductAttributesViewModel>().ReverseMap();

            Mapper.CreateMap<WebStoreGroupProductModel, GroupProductViewModel>();

            Mapper.CreateMap<WebStoreConfigurableProductModel, ConfigurableProductViewModel>();

            Mapper.CreateMap<ConfigurableProductViewModel, ProductViewModel>();

            Mapper.CreateMap<CategorySubHeaderViewModel, CategoryHeaderViewModel>();

            Mapper.CreateMap<WebStoreAttributeValueSwatchModel, AttributeValueSwatchViewModel>();

            Mapper.CreateMap<WebStoreLinkProductModel, LinkProductViewModel>().ReverseMap();

            Mapper.CreateMap<SearchCategoryModel, SearchCategoryViewModel>().ReverseMap();

            Mapper.CreateMap<BrandViewModel, BrandModel>().ReverseMap();
            Mapper.CreateMap<PriceTierModel, TierPriceViewModel>();

            Mapper.CreateMap<OrderModel, OrdersViewModel>()
                 .ForMember(d => d.Total, opt => opt.MapFrom(src => src.Total.ToPriceRoundOff()))
                 .ForMember(d => d.OrderTotalWithoutVoucher, opt => opt.MapFrom(src => src.OrderTotalWithoutVoucher.ToPriceRoundOff()))
                 .ForMember(d => d.EmailAddress, opt => opt.MapFrom(src => src.BillingAddress.EmailAddress))
                 .ForMember(d => d.PODocumentPath, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.PODocumentPath) ? HelperUtility.GetFilePath(src.PODocumentPath.Replace("~", string.Empty)) : null))
                 .ForMember(d=> d.OrderDate,opt=>opt.MapFrom(src=> Convert.ToDateTime(src.OrderDate).ToGlobalTimeZone()))
                 .ForMember(d=> d.InHandDate, opt => opt.MapFrom(src=> Convert.ToDateTime(src.InHandDate).ToTimeZoneDateTimeFormat()))
                 .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                 .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<PaymentHistoryListViewModel, PaymentHistoryListModel>().ReverseMap();
            Mapper.CreateMap<OrdersViewModel, OrderModel>()
                 .ForMember(d => d.Email, opt => opt.MapFrom(src => src.EmailAddress))
                 .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                 .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName));


            Mapper.CreateMap<RMAReturnModel, RMAReturnViewModel>()
                .ForMember(d=> d.ReturnDate,opt=>opt.MapFrom(src => Convert.ToDateTime(src.ReturnDate).ToTimeZoneDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<OrderLineItemModel, OrderLineItemViewModel>()
                .ForMember(d => d.Image, opt => opt.MapFrom(src => src.ProductImagePath));

            Mapper.CreateMap<OrderLineItemViewModel, OrderLineItemModel>().ReverseMap();
            Mapper.CreateMap<AddressListViewModel, AddressListModel>().ReverseMap();

            Mapper.CreateMap<ProductAlterNateImageViewModel, ProductAlterNateImageModel>().ReverseMap();

            Mapper.CreateMap<SEODetailsModel, SEOViewModel>().ReverseMap();

            Mapper.CreateMap<GiftCardHistoryModel, GiftCardHistoryViewModel>()
                 .ForMember(d => d.TransactionAmount, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.TransactionAmount, src.CultureCode)));

            Mapper.CreateMap<GiftCardHistoryViewModel, GiftCardHistoryModel>();

            Mapper.CreateMap<PublishCategoryModel, CategoryViewModel>()
                .ForMember(d => d.CategoryId, opt => opt.MapFrom(src => src.PublishCategoryId))
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(src => src.Name));

            Mapper.CreateMap<BrandModel, BrandViewModel>().ReverseMap();

            Mapper.CreateMap<PublishProductModel, ProductReviewViewModel>()
                .ForMember(d => d.ProductName, opt => opt.MapFrom(src => src.Name));

            Mapper.CreateMap<CouponModel, CouponViewModel>().ReverseMap();

            Mapper.CreateMap<ProductCompareModel, ProductCompareViewModel>().ReverseMap();

            Mapper.CreateMap<EmailAFriendListModel, EmailAFriendViewModel>().ReverseMap();

            Mapper.CreateMap<AccountModel, AccountViewModel>().ReverseMap();

            Mapper.CreateMap<AccountQuoteModel, AccountQuoteViewModel>()
              .ForMember(d => d.BillingAddress, opt => opt.MapFrom(src => $"{src.BillingAddressModel.FirstName} {src.BillingAddressModel.LastName},{src.BillingAddressModel.Address1}, {src.BillingAddressModel.Address2}," + (string.IsNullOrEmpty(src.BillingAddressModel.Address3) ? "" : $"{src.BillingAddressModel.Address3} ,") + $" {src.BillingAddressModel.CityName}, {src.BillingAddressModel.StateName}, {src.BillingAddressModel.PostalCode}, {src.BillingAddressModel.CountryName}, PH NO. {src.BillingAddressModel.PhoneNumber}"))
             .ForMember(d => d.ShippingAddress, opt => opt.MapFrom(src => $"{src.ShippingAddressModel.FirstName} {src.ShippingAddressModel.LastName},{src.ShippingAddressModel.Address1}, {src.ShippingAddressModel.Address2}," + (string.IsNullOrEmpty(src.ShippingAddressModel.Address3) ? "" : $"{src.ShippingAddressModel.Address3} ,") + $" {src.ShippingAddressModel.CityName}, {src.ShippingAddressModel.StateName}, {src.ShippingAddressModel.PostalCode}, {src.ShippingAddressModel.CountryName}, PH NO. {src.ShippingAddressModel.PhoneNumber}"))
             .ForMember(d => d.TaxAmount, opt => opt.MapFrom(src => src.TaxCost.ToPriceRoundOff()))
             .ForMember(d => d.QuoteOrderTotal, opt => opt.MapFrom(src => @HelperMethods.FormatPriceWithCurrency(src.QuoteOrderTotal, src.CultureCode)))
             .ForMember(d => d.ShippingAmount, opt => opt.MapFrom(src => src.ShippingAmount.ToPriceRoundOff()))
             .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src =>Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
             .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src =>Convert.ToDateTime( src.ModifiedDate).ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<AccountQuoteViewModel, AccountQuoteModel>()
                 .ForMember(d => d.QuoteOrderTotal, opt => opt.MapFrom(src => Convert.ToDecimal(src.QuoteOrderTotal)));

            Mapper.CreateMap<AccountQuoteLineItemViewModel, AccountQuoteLineItemModel>().ReverseMap();

            Mapper.CreateMap<SearchProductModel, PublishProductModel>()
                .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ZnodeProductId));

            Mapper.CreateMap<TemplateViewModel, AccountTemplateModel>().ReverseMap();

            Mapper.CreateMap<PIMProductAttributeValuesModel, AttributeValidationViewModel>().ReverseMap();

            Mapper.CreateMap<PaymentSettingModel, PaymentSettingViewModel>().ReverseMap();

            Mapper.CreateMap<ShippingListModel, ShippingOptionListViewModel>().ReverseMap();
            Mapper.CreateMap<ShippingModel, ShippingOptionViewModel>().ReverseMap();

            Mapper.CreateMap<AttributesSelectValuesViewModel, AttributesSelectValuesModel>().ReverseMap();

            Mapper.CreateMap<ProductInventoryPriceModel, ProductPriceViewModel>();

            Mapper.CreateMap<UrlRedirectModel, UrlRedirectViewModel>().ReverseMap();

            Mapper.CreateMap<ProductPromotionModel, ProductPromotionViewModel>().ReverseMap();
            Mapper.CreateMap<PromotionModel, PromotionViewModel>().ReverseMap();

            Mapper.CreateMap<ReferralCommissionModel, ReferralCommissionViewModel>()
                .ForMember(d => d.ReferralCommission, opt => opt.MapFrom(src => Equals(src.ReferralCommissionType, WebStoreConstants.Percentage) ? src.ReferralCommission.ToPriceRoundOff() : @HelperMethods.FormatPriceWithCurrency(src.ReferralCommission, src.CultureCode)));

            Mapper.CreateMap<ReturnOrderLineItemViewModel, ReturnOrderLineItemModel>().ReverseMap();
            Mapper.CreateMap<PortalTrackingPixelViewModel, PortalTrackingPixelModel>().ReverseMap();
            Mapper.CreateMap<BlogNewsCommentViewModel, WebStoreBlogNewsCommentModel>().ReverseMap();
            Mapper.CreateMap<BlogNewsViewModel, WebStoreBlogNewsModel>()
                  .ForMember(d => d.Comments, opt => opt.MapFrom(src => src.Comments));
            Mapper.CreateMap<WebStoreBlogNewsModel, BlogNewsViewModel>()
              .ForMember(d => d.Comments, opt => opt.MapFrom(src => src.Comments));

            Mapper.CreateMap<RobotsTxtModel, RobotsTxtViewModel>().ReverseMap();

            Mapper.CreateMap<GlobalAttributeGroupModel, GlobalAttributeGroupViewModel>().ReverseMap();

            Mapper.CreateMap<FormBuilderAttributeGroupModel, WidgetFormBuilderAttributeViewModel>().ReverseMap();

            Mapper.CreateMap<GlobalAttributeValuesModel, GlobalAttributeValuesViewModel>().ReverseMap();

            Mapper.CreateMap<WebStoreWidgetFormParameters, WidgetFormConfigurationViewModel>().ReverseMap();

            Mapper.CreateMap<FormBuilderAttributeGroupModel, FormBuilderAttributeGroupViewModel>().ReverseMap();

            Mapper.CreateMap<FormSubmitModel, FormSubmitViewModel>().ReverseMap();

            Mapper.CreateMap<FormSubmitAttributeModel, FormSubmitAttributeViewModel>().ReverseMap();

            Mapper.CreateMap<UserModel, CustomerViewModel>()
                 .ForMember(d => d.BudgetAmount, opt => opt.MapFrom(src => !Equals(src.BudgetAmount, null) ? src.BudgetAmount.ToPriceRoundOff() : null))
                 .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                 .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()));
            
            Mapper.CreateMap<CustomerViewModel, UserModel>();
            Mapper.CreateMap<UserModel, CustomerViewModel>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(src => Equals(src.User, null) ? src.UserName : src.User.Username))
                .ForMember(dest => dest.IsSelectAllPortal, opt => opt.MapFrom(src => (!Equals(src.PortalIds, null) && src.PortalIds.Count() > 0) ? src.PortalIds.Contains("0") : false));

            Mapper.CreateMap<CustomerAccountViewModel, UserModel>();
            Mapper.CreateMap<UserModel, CustomerAccountViewModel>()
                .ForMember(d => d.BudgetAmount, opt => opt.MapFrom(src => !Equals(src.BudgetAmount, null) ? src.BudgetAmount.ToPriceRoundOff() : null));

            Mapper.CreateMap<AccountDepartmentViewModel, AccountDepartmentModel>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.DepartmentName) ? string.Empty : src.DepartmentName.Trim()));
            Mapper.CreateMap<AccountDepartmentModel, AccountDepartmentViewModel>();

            Mapper.CreateMap<ImportLogsModel, ImportProcessLogsViewModel>().ReverseMap();

            Mapper.CreateMap<ImportLogDetailsModel, ImportLogsViewModel>().ReverseMap();

            Mapper.CreateMap<AddToCartViewModel, ShoppingCartItemModel>().ReverseMap();

            Mapper.CreateMap<AddToCartModel, AddToCartViewModel>().ReverseMap();

            Mapper.CreateMap<WebStoreWidgetSearchModel, WidgetSearchDataViewModel>().ReverseMap();
            Mapper.CreateMap<OrderWarehouseModel, OrderWarehouseViewModel>().ReverseMap();
            Mapper.CreateMap<AssociatedProductsModel, AssociatedProductsViewModel>().ReverseMap();
            Mapper.CreateMap<UserApproverModel, UserApproverViewModel>().ReverseMap();

            Mapper.CreateMap<ECertTotalBalanceModel, ECertTotalBalanceViewModel>().ReverseMap();
            Mapper.CreateMap<ECertificateModel, ECertificateViewModel>().ReverseMap();
            Mapper.CreateMap<ECertificateListModel, ECertificateListViewModel>().ReverseMap();
            Mapper.CreateMap<QuoteApprovalModel, QuoteApprovalViewModel>().ReverseMap();

            Mapper.CreateMap<BarcodeReaderModel, BarcodeReaderViewModel>().ReverseMap();
            Mapper.CreateMap<SearchReportViewModel, SearchReportModel>().ReverseMap();

            Mapper.CreateMap<QuoteCreateModel, QuoteCreateViewModel>().ReverseMap();
            Mapper.CreateMap<QuoteResponseModel, QuoteResponseViewModel>()
                   .ForMember(d => d.TaxCost, opt => opt.MapFrom(src => src.TaxAmount))
                   .ForMember(d => d.QuoteNoteList, opt => opt.MapFrom(src => src.QuoteHistoryList))
                   .ForMember(d=> d.CreatedDate, opt => opt.MapFrom(src=> Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                   .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()))
                   .ForMember(d => d.QuoteExpirationDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.QuoteExpirationDate).ToTimeZoneDateTimeFormat())).ReverseMap();
            Mapper.CreateMap<QuoteLineItemModel, QuoteLineItemViewModel>().ReverseMap();
            Mapper.CreateMap<QuoteModel, QuoteViewModel>()
                  .ForMember(d => d.TotalAmount, opt => opt.MapFrom(src => @HelperMethods.FormatPriceWithCurrency(src.TotalAmount, src.CultureCode)))
                  .ForMember(d=> d.QuoteDate, opt=> opt.MapFrom(src=> src.QuoteDate.ToTimeZoneDateTimeFormat()));
            Mapper.CreateMap<ConvertQuoteToOrderModel, ConvertQuoteToOrderViewModel>()
                 .ForMember(d => d.PaymentDetails, opt => opt.MapFrom(src => src.PaymentDetails)).ReverseMap();

            Mapper.CreateMap<PayInvoiceModel, PayInvoiceViewModel>()
                  .ForMember(d => d.PaymentDetails, opt => opt.MapFrom(src => src.PaymentDetails)).ReverseMap();

            Mapper.CreateMap<ConvertQuoteToOrderModel, OrdersViewModel>();
            Mapper.CreateMap<OrderHistoryModel, QuoteNotesViewModel>();
            Mapper.CreateMap<SubmitPaymentDetailsModel, SubmitPaymentDetailsViewModel>().ReverseMap();

            Mapper.CreateMap<ShoppingCartItemModel, ProductDetailModel>()
                  .ForMember(d => d.Price, opt => opt.MapFrom(src => src.UnitPrice))
                  .ForMember(d => d.InitialShippingCost, opt => opt.MapFrom(src => src.ShippingCost));

            #region TradeCentric
            Mapper.CreateMap<TradeCentricUserModel, TradeCentricUserViewModel>().ReverseMap();
            #endregion

            #region PDP Lite mappings
            Mapper.CreateMap<PublishProductDTO, ShortProductViewModel>();
            Mapper.CreateMap<ProductImageDTO, ProductDefaultImageViewModel>();
            Mapper.CreateMap<ProductSeoDTO, ProductSEODetailsViewModel>();
            Mapper.CreateMap<ProductPricingDTO, ProductPricingViewModel>();
            Mapper.CreateMap<ProductInventoryDTO, ProductInventoryViewModel>();
            Mapper.CreateMap<InventorySKUModel, InventorySKUViewModel>();
            Mapper.CreateMap<ProductStoreSettingsDTO, ProductStoreSettingsViewModel>();
            Mapper.CreateMap<ProductMiscellaneousDetailsDTO, ProductMiscellaneousDetailsViewModel>();
            Mapper.CreateMap<ProductReviewsDTO, ProductReviewsViewModel>();
            Mapper.CreateMap<ProductBrandDTO, ProductBrandViewModel>();
            Mapper.CreateMap<ProductAssociationsDTO, ProductAssociationsViewModel>();
            #endregion

            #region CMS Search mappings
            Mapper.CreateMap<SearchCMSPageModel, CMSPageViewModel>().ReverseMap();
            #endregion

            Mapper.CreateMap<OrderModel, RMAReturnOrderDetailViewModel>()
                .ForMember(d => d.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToTimeZoneDateTimeFormat())).ReverseMap();
            Mapper.CreateMap<RMAReturnCartViewModel, ShoppingCartModel>().ReverseMap();

            Mapper.CreateMap<ShoppingCartItemModel, RMAReturnCartItemViewModel>()
                  .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
                  .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.ToPriceRoundOff()))
                  .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity));

            Mapper.CreateMap<RMAReturnCartItemViewModel, ShoppingCartItemModel>()
                .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
                .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.ToPriceRoundOff()))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity));

            Mapper.CreateMap<RMAReturnCalculateModel, RMAReturnCalculateViewModel>().ReverseMap();
            Mapper.CreateMap<RMAReturnCalculateLineItemModel, RMAReturnCalculateLineItemViewModel>().ReverseMap();
            Mapper.CreateMap<RMAReturnLineItemModel, RMAReturnLineItemViewModel>().ReverseMap();
            Mapper.CreateMap<RMAReturnNotesModel, RMAReturnNotesViewModel>().ReverseMap();
            Mapper.CreateMap<ProductInventoryDetailViewModel, ProductInventoryDetailModel>().ReverseMap();
          
            Mapper.CreateMap<PowerBISettingsViewModel, PowerBISettingsModel>().ReverseMap();
            Mapper.CreateMap<StockNotificationViewModel, StockNotificationModel>()
           .ForMember(d => d.ProductSKU, opt => opt.MapFrom(src => src.SKU));

            Mapper.CreateMap<GiftCardModel, VoucherViewModel>()
           .ForMember(d => d.VoucherAmount, opt => opt.MapFrom(src => src.Amount.ToPriceRoundOff()))
           .ForMember(d => d.VoucherBalanceAmount, opt => opt.MapFrom(src => src.RemainingAmount.ToPriceRoundOff()))
           .ForMember(d => d.Amount, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.Amount.GetValueOrDefault(), src.CultureCode)))
           .ForMember(d => d.RemainingAmount, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.RemainingAmount.GetValueOrDefault(), src.CultureCode)))
           .ForMember(d => d.VoucherId, opt => opt.MapFrom(src => src.GiftCardId));

            Mapper.CreateMap<VoucherViewModel, GiftCardModel>();

            Mapper.CreateMap<VoucherModel, VouchersViewModel>()
               .ForMember(d => d.VoucherAmountUsed, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.VoucherAmountUsed, src.CultureCode)))
               .ForMember(d => d.OrderVoucherAmount, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.OrderVoucherAmount, src.CultureCode)))
               .ForMember(d => d.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate.ToDateTimeFormat()));

            Mapper.CreateMap<VouchersViewModel, VoucherModel>();

            #region Quick Order
            Mapper.CreateMap<QuickOrderProductViewModel, QuickOrderProductModel>().ReverseMap();
            #endregion

            Mapper.CreateMap<ShippingOptionViewModel, ShippingOptionModel>().ReverseMap();

            #region Sitemap

            Mapper.CreateMap<SiteMapCategoryListViewModel, SiteMapCategoryListModel>().ReverseMap();
            Mapper.CreateMap<SiteMapCategoryViewModel, SiteMapCategoryModel>().ReverseMap()
            .ForMember(d => d.SubCategoryItems, opt => opt.MapFrom(src => src.SubCategoryItems));

            Mapper.CreateMap<SiteMapBrandListViewModel, SiteMapBrandListModel>().ReverseMap();
            Mapper.CreateMap<SiteMapBrandViewModel, SiteMapBrandModel>().ReverseMap();

            Mapper.CreateMap<SiteMapProductListModel, SiteMapProductListViewModel>().ReverseMap();
            Mapper.CreateMap<SiteMapProductModel, SiteMapProductViewModel>().ReverseMap();            

            #endregion

            Mapper.CreateMap<AssociatedPublishedBundleProductModel, AssociatedPublishBundleProductViewModel>();

            Mapper.CreateMap<TaxSummaryModel, TaxSummaryViewModel>().ReverseMap();

            Mapper.CreateMap<ContentContainerDataModel, ContentContainerDataViewModel>().ReverseMap();
            Mapper.CreateMap<PaymentTokenViewModel, PaymentGatewayTokenModel>().ReverseMap();

            Mapper.CreateMap<GlobalAttributeModel, GlobalAttributeViewModel>().ReverseMap();

            Mapper.CreateMap<CmsContainerWidgetConfigurationModel, CmsContainerWidgetConfigurationViewModel>().ReverseMap();

            #region Save for Later
            Mapper.CreateMap<ShoppingCartItemModel, TemplateCartItemModel>();
            Mapper.CreateMap<ShoppingCartItemModel, TemplateCartItemModel>().ReverseMap();
            #endregion

            #region Payment
            Mapper.CreateMap<PaymentTokenViewModel, PaymentGatewayTokenModel>().ReverseMap();
            #endregion
        }
    }
}