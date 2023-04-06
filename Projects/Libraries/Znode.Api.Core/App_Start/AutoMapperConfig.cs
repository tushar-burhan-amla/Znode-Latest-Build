using Autofac;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;
using Znode.Engine.Recommendations.DataModel;
using Znode.Engine.Recommendations.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.MongoDB.Data;
using Znode.Libraries.Search;

namespace Znode.Engine.Api
{
    public static class AutoMapperConfig
    {
        public static void Execute()
        {
            RegisterV1Maps();
            RegisterV2Maps();
        }

        static void RegisterV1Maps()
        {
            Mapper.CreateMap<ZnodeTypedParameter, TypedParameter>();

            Mapper.CreateMap<ZnodeNamedParameter, NamedParameter>();

            Mapper.CreateMap<ZnodeMediaAttributeFamily, AttributeFamilyModel>();

            Mapper.CreateMap<AttributeFamilyModel, ZnodeMediaAttributeFamily>();

            Mapper.CreateMap<ZnodeGlobalSetting, DefaultGlobalConfigModel>();


            Mapper.CreateMap<FamilyLocaleModel, ZnodeMediaFamilyLocale>()
                .ForMember(d => d.MediaAttributeFamilyId, opt => opt.MapFrom(src => src.AttributeFamilyId));

            Mapper.CreateMap<ZnodeMediaAttributeGroupMapper, AttributeGroupMapperModel>();


            Mapper.CreateMap<CustomFieldModel, ZnodePimCustomField>()
                .ForMember(d => d.PimCustomFieldId, opt => opt.MapFrom(src => src.CustomFieldId))
                .ForMember(d => d.PimProductId, opt => opt.MapFrom(src => src.ProductId));

            Mapper.CreateMap<ZnodePimCustomField, CustomFieldModel>()
              .ForMember(d => d.CustomFieldId, opt => opt.MapFrom(src => src.PimCustomFieldId))
              .ForMember(d => d.ProductId, opt => opt.MapFrom(src => src.PimProductId))
              .ForMember(d => d.CustomFieldLocales, opt => opt.MapFrom(src => src.ZnodePimCustomFieldLocales));

            Mapper.CreateMap<CustomFieldLocaleModel, ZnodePimCustomFieldLocale>()
               .ForMember(d => d.PimCustomFieldId, opt => opt.MapFrom(src => src.CustomFieldId))
               .ForMember(d => d.PimCustomFieldLocaleId, opt => opt.MapFrom(src => src.CustomFieldLocaleId));

            Mapper.CreateMap<ZnodePimCustomFieldLocale, CustomFieldLocaleModel>()
              .ForMember(d => d.CustomFieldId, opt => opt.MapFrom(src => src.PimCustomFieldId))
              .ForMember(d => d.CustomFieldLocaleId, opt => opt.MapFrom(src => src.PimCustomFieldLocaleId));
            Mapper.CreateMap<AttributeGroupMapperModel, ZnodeMediaAttributeGroupMapper>();

            Mapper.CreateMap<ZnodeMediaAttributeGroupLocale, AttributeGroupLocaleModel>();

            Mapper.CreateMap<AttributeGroupLocaleModel, ZnodeMediaAttributeGroupLocale>();

            Mapper.CreateMap<View_PimAttributeGroupbyFamily, PIMAttributeGroupModel>();

            Mapper.CreateMap<View_GetCatalogProduct, ZnodePimProduct>();

            Mapper.CreateMap<View_GetCatalogCategory, ZnodePimCategory>();

            Mapper.CreateMap<View_PimAttributeValues, PIMProductAttributeValuesModel>();

            Mapper.CreateMap<ZnodePimAttributeValue, PIMProductAttributeValuesModel>().ReverseMap();

            Mapper.CreateMap<PIMProductAttributeValuesLocalModel, ZnodePimAttributeValueLocale>().ReverseMap();

            Mapper.CreateMap<ZnodePimAttributeFamily, PIMAttributeFamilyModel>();

            Mapper.CreateMap<ZnodeMediaAttributeGroup, AttributeGroupModel>().ReverseMap();

            Mapper.CreateMap<PortalModel, ZnodePortalCatalog>();

            Mapper.CreateMap<ProfileModel, ZnodeAccountProfile>();

            Mapper.CreateMap<ZnodeAccountProfile, ProfileModel>()
                 .ForMember(d => d.Name, opt => opt.MapFrom(src => src.ZnodeAccount.Name))
               .ForMember(d => d.ProfileName, opt => opt.MapFrom(src => src.ZnodeProfile.ProfileName));

            Mapper.CreateMap<FilterTuple, FilterDataTuple>();

            Mapper.CreateMap<View_CMSWidgetsConfigurationList, LinkWidgetConfigurationModel>()
                   .ForMember(d => d.MediaPath, opt => opt.MapFrom(src => src.Image));

            Mapper.CreateMap<ZnodePimAttributeValue, PIMAttributeValueModel>();

            Mapper.CreateMap<PIMAttributeValueModel, ZnodePimAttributeValue>();

            Mapper.CreateMap<ZnodePimProductTypeAssociation, ProductTypeAssociationModel>();

            Mapper.CreateMap<ProductTypeAssociationModel, ZnodePimProductTypeAssociation>();

            Mapper.CreateMap<CatalogModel, ZnodePimCatalog>();

            Mapper.CreateMap<ZnodePimCatalog, CatalogModel>();

            Mapper.CreateMap<ZnodePimAttributeGroup, PIMAttributeGroupModel>();

            Mapper.CreateMap<PIMAttributeGroupModel, ZnodePimAttributeGroup>();

            Mapper.CreateMap<PIMAttributeFamilyModel, ZnodePimAttributeFamily>();

            Mapper.CreateMap<AttributesValidationModel, ZnodeMediaAttributeValidation>();

            Mapper.CreateMap<ZnodeMediaAttribute, AttributesDataModel>();

            Mapper.CreateMap<AttributesDataModel, ZnodeMediaAttribute>();

            Mapper.CreateMap<DefaultAttributeValueLocaleModel, ZnodeMediaAttributeDefaultValueLocale>();

            Mapper.CreateMap<AttributesLocaleModel, ZnodeMediaAttributeLocale>();

            Mapper.CreateMap<ZnodeAttributeType, AttributeTypeDataModel>();

            Mapper.CreateMap<AttributeTypeDataModel, ZnodeAttributeType>();

            Mapper.CreateMap<ZnodeMediaAttributeLocale, AttributeLocalDataModel>();

            Mapper.CreateMap<AttributeLocalDataModel, ZnodeMediaAttributeLocale>();

            Mapper.CreateMap<DefaultAttributeValueLocaleModel, ZnodeMediaAttributeDefaultValueLocale>();

            Mapper.CreateMap<PIMAttributeGroupLocaleModel, ZnodePimAttributeGroupLocale>();

            Mapper.CreateMap<ZnodePimAttributeGroupLocale, PIMAttributeGroupLocaleModel>();

            Mapper.CreateMap<ZnodePimAttributeGroupMapper, PIMAttributeGroupMapperModel>();

            Mapper.CreateMap<PIMAttributeGroupMapperModel, ZnodePimAttributeGroupMapper>();

            Mapper.CreateMap<PIMFamilyLocaleModel, ZnodePimFamilyLocale>();

            Mapper.CreateMap<ZnodePimFamilyLocale, PIMFamilyLocaleModel>();

            Mapper.CreateMap<DisplayUnitModel, ZnodeDisplayUnit>();

            Mapper.CreateMap<ZnodeDisplayUnit, DisplayUnitModel>();

            Mapper.CreateMap<WeightUnitModel, ZnodeWeightUnit>();

            Mapper.CreateMap<ZnodeWeightUnit, WeightUnitModel>();

            Mapper.CreateMap<DateFormatModel, ZnodeDateFormat>();

            Mapper.CreateMap<ZnodeDateFormat, DateFormatModel>();

            Mapper.CreateMap<TimeFormatModel, ZnodeTimeFormat>();

            Mapper.CreateMap<ZnodeTimeFormat, TimeFormatModel>();

            Mapper.CreateMap<TimeZoneModel, ZnodeTimeZone>();

            Mapper.CreateMap<ZnodeTimeZone, TimeZoneModel>();

            Mapper.CreateMap<View_PimCategoryDetail, CategoryModel>();

            Mapper.CreateMap<GlobalMediaDisplaySettingModel, ZnodeGlobalMediaDisplaySetting>().ReverseMap();

            Mapper.CreateMap<PIMAttributeValidationModel, ZnodePimAttributeValidation>();
            Mapper.CreateMap<ZnodePriceList, PriceModel>();

            Mapper.CreateMap<PriceModel, ZnodePriceList>();

            Mapper.CreateMap<ZnodeAccount, UserModel>();
            Mapper.CreateMap<UserModel, ZnodeAccount>();

            Mapper.CreateMap<ZnodeNote, NoteModel>();
            Mapper.CreateMap<NoteModel, ZnodeNote>();

            Mapper.CreateMap<ZnodeDepartment, AccountDepartmentModel>();
            Mapper.CreateMap<AccountDepartmentModel, ZnodeDepartment>();

            Mapper.CreateMap<ZnodeAccount, AccountModel>();
            Mapper.CreateMap<AccountModel, ZnodeAccount>();

            Mapper.CreateMap<ZnodeAddress, AddressModel>();
            Mapper.CreateMap<AddressModel, ZnodeAddress>();

            Mapper.CreateMap<ZnodePrice, PriceSKUModel>();

            Mapper.CreateMap<PriceSKUModel, ZnodePrice>();

            Mapper.CreateMap<View_PimCategoryAttributeValues, PIMProductAttributeValuesModel>();

            Mapper.CreateMap<View_ManageProductList, ProductDetailsModel>();

            Mapper.CreateMap<ZnodeInventory, InventorySKUModel>();

            Mapper.CreateMap<InventorySKUModel, ZnodeInventory>();

            Mapper.CreateMap<PriceTierModel, ZnodePriceTier>();
            Mapper.CreateMap<ZnodePriceTier, PriceTierModel>();

            Mapper.CreateMap<View_GetAssociatedPortalToPriceList, PricePortalModel>();

            Mapper.CreateMap<PricePortalModel, View_GetAssociatedPortalToPriceList>();

            Mapper.CreateMap<ZnodePortal, PortalModel>();

            Mapper.CreateMap<ZnodePortalCustomCss, DynamicContentModel>().ReverseMap();

            Mapper.CreateMap<PortalModel, ZnodePortal>()
                .ForMember(d => d.AdminEmail, opt => opt.MapFrom(src => src.AdministratorEmail))
                .ForMember(d => d.UseSSL, opt => opt.MapFrom(src => src.IsEnableSSL))
                .ForMember(d => d.DefaultOrderStateID, opt => opt.MapFrom(src => src.OrderStatusId))
                .ForMember(d => d.DefaultReviewStatus, opt => opt.MapFrom(src => src.ReviewStatus))
                .ForMember(d => d.CopyContentBasedOnPortalId, opt => opt.MapFrom(src => src.CopyContentPortalId))
                .ForMember(d => d.UserVerificationType, opt => opt.MapFrom(src => src.UserVerificationTypeCode.ToString()));

            Mapper.CreateMap<ZnodePortal, PortalModel>()
                .ForMember(d => d.AdministratorEmail, opt => opt.MapFrom(src => src.AdminEmail))
                .ForMember(d => d.IsEnableSSL, opt => opt.MapFrom(src => src.UseSSL))
                .ForMember(d => d.OrderStatusId, opt => opt.MapFrom(src => src.DefaultOrderStateID))
                .ForMember(d => d.ReviewStatus, opt => opt.MapFrom(src => src.DefaultReviewStatus))
                .ForMember(d => d.CopyContentPortalId, opt => opt.MapFrom(src => src.CopyContentBasedOnPortalId))
                .ForMember(d => d.UserVerificationTypeCode, opt => opt.MapFrom(src => Enum.Parse(typeof(UserVerificationTypeEnum), src.UserVerificationType)));

            Mapper.CreateMap<View_GetPriceListUsers, PriceUserModel>()
             .ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FullName));
            Mapper.CreateMap<View_GetAssociatedProfileToPriceList, PriceProfileModel>();

            Mapper.CreateMap<PriceUserModel, View_GetPriceListUsers>();

            Mapper.CreateMap<View_UserRoles, UserModel>();
            Mapper.CreateMap<PriceProfileModel, View_GetAssociatedProfileToPriceList>();

            Mapper.CreateMap<ProfileModel, ZnodeProfile>().ReverseMap();

            Mapper.CreateMap<ZnodeWarehouse, WarehouseModel>();

            Mapper.CreateMap<WarehouseModel, ZnodeWarehouse>();

            Mapper.CreateMap<PIMAttributeModel, ZnodePimAttribute>();

            Mapper.CreateMap<ZnodePimAttribute, PIMAttributeModel>();

            Mapper.CreateMap<PriceUserModel, ZnodePriceListUser>();

            Mapper.CreateMap<ZnodePriceListUser, PriceUserModel>()
                 .ForMember(d => d.FullName, opt => opt.MapFrom(src => $"{src.ZnodeUser.FirstName} {src.ZnodeUser.LastName}"));

            Mapper.CreateMap<PriceAccountModel, ZnodePriceListAccount>();

            Mapper.CreateMap<ZnodePriceListAccount, PriceAccountModel>()
                   .ForMember(d => d.AccountName, opt => opt.MapFrom(src => src.ZnodeAccount.Name));

            Mapper.CreateMap<ZnodePimAttributeLocale, PIMAttributeLocaleModel>();

            Mapper.CreateMap<PIMAttributeLocaleModel, ZnodePimAttributeLocale>();

            Mapper.CreateMap<ZnodePimCategoryProduct, CategoryProductModel>();

            Mapper.CreateMap<CategoryProductModel, ZnodePimCategoryProduct>();

            Mapper.CreateMap<View_CategoryAssociatedProduct, CategoryProductModel>();

            Mapper.CreateMap<ZnodeUom, UomModel>();

            Mapper.CreateMap<View_PimPersonalisedAttributeValues, PIMProductAttributeValuesModel>();

            Mapper.CreateMap<ZnodePortalWarehouse, PortalWarehouseModel>()
                .ForMember(d => d.WarehouseName, opt => opt.MapFrom(src => src.ZnodeWarehouse.WarehouseName))
                .ForMember(d => d.WarehouseCode, opt => opt.MapFrom(src => src.ZnodeWarehouse.WarehouseCode));

            Mapper.CreateMap<PortalWarehouseModel, ZnodePortalWarehouse>();

            Mapper.CreateMap<ZnodePortalAlternateWarehouse, PortalAlternateWarehouseModel>()
             .ForMember(d => d.WarehouseName, opt => opt.MapFrom(src => src.ZnodeWarehouse.WarehouseName))
             .ForMember(d => d.WarehouseCode, opt => opt.MapFrom(src => src.ZnodeWarehouse.WarehouseCode));

            Mapper.CreateMap<PortalAlternateWarehouseModel, ZnodePortalAlternateWarehouse>();

            Mapper.CreateMap<View_ManageProductTypeAssociationList, ProductDetailsModel>();


            Mapper.CreateMap<ZnodePimLinkProductDetail, LinkProductDetailModel>();

            Mapper.CreateMap<LinkProductDetailModel, ZnodePimLinkProductDetail>();

            Mapper.CreateMap<ZnodePimProduct, ProductModel>();

            Mapper.CreateMap<ProductModel, ZnodePimProduct>();

            Mapper.CreateMap<View_ManageLinkProductList, ProductDetailsModel>();

            Mapper.CreateMap<ZnodePriceListPortal, PricePortalModel>();

            Mapper.CreateMap<PricePortalModel, ZnodePriceListPortal>()
                 .ForMember(d => d.Precedence, opt => opt.MapFrom(src => Equals(Convert.ToInt32(src.Precedence), 0) ? ZnodeConstant.DefaultPrecedence : src.Precedence));

            Mapper.CreateMap<ZnodePriceListProfile, PricePortalModel>().ReverseMap();

            Mapper.CreateMap<View_GetAssociatedPortalToPriceList, PortalModel>();

            Mapper.CreateMap<PriceProfileModel, ZnodePriceListProfile>()
                  .ForMember(d => d.Precedence, opt => opt.MapFrom(src => Equals(Convert.ToInt32(src.Precedence), 0) ? ZnodeConstant.DefaultPrecedence : src.Precedence));

            Mapper.CreateMap<ZnodePriceListProfile, PriceProfileModel>();

            Mapper.CreateMap<View_GetAssociatedProfileToPriceList, ProfileModel>();

            Mapper.CreateMap<ShippingModel, ZnodeShipping>();

            Mapper.CreateMap<ZnodeShipping, ShippingModel>();

            Mapper.CreateMap<ShippingServiceCodeModel, ZnodeShippingServiceCode>();

            Mapper.CreateMap<ZnodeShippingServiceCode, ShippingServiceCodeModel>();

            Mapper.CreateMap<ZnodeAttributeType, PIMAttributeTypeModel>();

            Mapper.CreateMap<PIMAttributeTypeModel, ZnodeAttributeType>();

            Mapper.CreateMap<ShippingSKUModel, ZnodeShippingSKU>();

            Mapper.CreateMap<ZnodeShippingSKU, ShippingSKUModel>();

            Mapper.CreateMap<ZnodePortalCatalog, PortalCatalogModel>();
         
            Mapper.CreateMap<PortalCatalogModel, ZnodePortalCatalog>();



            Mapper.CreateMap<TaxClassModel, ZnodeTaxClass>();

            Mapper.CreateMap<ZnodeTaxClass, TaxClassModel>();

            Mapper.CreateMap<TaxClassSKUModel, ZnodeTaxClassSKU>();

            Mapper.CreateMap<ZnodeTaxClassSKU, TaxClassSKUModel>();

            Mapper.CreateMap<View_GetConfigureAttributeDetail, PIMProductAttributeValuesModel>();

            Mapper.CreateMap<AccessPermissionModel, ZnodeAccessPermission>();
            Mapper.CreateMap<ZnodeAccessPermission, AccessPermissionModel>();

            Mapper.CreateMap<View_GetAccountAccessPermission, AccessPermissionModel>();

            Mapper.CreateMap<AccessPermissionModel, ZnodeAccountPermission>();
            Mapper.CreateMap<ZnodeAccountPermission, AccessPermissionModel>();

            Mapper.CreateMap<AccessPermissionModel, ZnodeAccountPermissionAccess>();

            Mapper.CreateMap<ZnodeCMSPortalTheme, PricePortalModel>()
             .ForMember(d => d.PriceListId, opt => opt.MapFrom(src => src.CMSThemeId))
             .ForMember(d => d.PriceListPortalId, opt => opt.MapFrom(src => src.CMSPortalThemeId));

            Mapper.CreateMap<PricePortalModel, ZnodeCMSPortalTheme>()
             .ForMember(d => d.CMSThemeId, opt => opt.MapFrom(src => src.PriceListId))
             .ForMember(d => d.CMSPortalThemeId, opt => opt.MapFrom(src => src.PriceListPortalId));

            Mapper.CreateMap<View_GetAssociatedCMSThemeToPortal, PortalModel>();

            Mapper.CreateMap<View_GetAssociatedCMSThemeToPortal, PricePortalModel>()
             .ForMember(d => d.PriceListId, opt => opt.MapFrom(src => src.CMSThemeId))
             .ForMember(d => d.PriceListPortalId, opt => opt.MapFrom(src => src.CMSPortalThemeId));

            Mapper.CreateMap<ThemeModel, ZnodeCMSTheme>();

            Mapper.CreateMap<RecommendationSettingModel, ZnodePortalRecommendationSetting>().ReverseMap();

            Mapper.CreateMap<ZnodeCMSTheme, ThemeModel>()
                .ForMember(d => d.ParentThemeName, opt => opt.MapFrom(src => HelperUtility.IsNotNull(src.ZnodeCMSTheme2) ? src.ZnodeCMSTheme2.Name : null));

            Mapper.CreateMap<ZnodeAccountPermissionAccess, AccountPermissionAccessModel>();

            Mapper.CreateMap<TaxRuleModel, ZnodeTaxRule>();

            Mapper.CreateMap<ZnodeTaxRule, TaxRuleModel>()
                .ForMember(d => d.DestinationCountryCode, opt => opt.MapFrom(src => Equals(src.DestinationCountryCode, null) ? "All" : src.DestinationCountryCode));

            Mapper.CreateMap<SliderModel, ZnodeCMSSlider>();

            Mapper.CreateMap<ZnodeCMSSlider, SliderModel>();

            Mapper.CreateMap<CustomerReviewModel, ZnodeCMSCustomerReview>();
            Mapper.CreateMap<ZnodeCMSCustomerReview, CustomerReviewModel>();
            Mapper.CreateMap<ZnodeCMSPortalTheme, WebSiteLogoModel>();
            Mapper.CreateMap<WebSiteLogoModel, ZnodeCMSPortalTheme>();

            Mapper.CreateMap<ZnodePimAddonGroup, AddonGroupModel>();
            Mapper.CreateMap<AddonGroupModel, ZnodePimAddonGroup>();

            Mapper.CreateMap<AddonGroupLocaleListModel, ICollection<ZnodePimAddonGroupLocale>>();
            Mapper.CreateMap<ICollection<ZnodePimAddonGroupLocale>, AddonGroupLocaleListModel>();

            Mapper.CreateMap<ZnodeCMSWidgetTitleConfiguration, LinkWidgetConfigurationModel>().ReverseMap();

            Mapper.CreateMap<ZnodeCMSWidgetTitleConfigurationLocale, LinkWidgetConfigurationModel>().ReverseMap();

            Mapper.CreateMap<ZnodePublishWidgetTitleEntity, LinkWidgetConfigurationModel>();

            Mapper.CreateMap<ContentPageModel, ZnodeCMSContentPage>();

            Mapper.CreateMap<ZnodeCMSContentPage, ContentPageModel>();

            Mapper.CreateMap<StateModel, ZnodeState>();

            Mapper.CreateMap<ZnodeState, StateModel>();

            Mapper.CreateMap<CityModel, ZnodeCity>();

            Mapper.CreateMap<ZnodeCity, CityModel>();

            Mapper.CreateMap<WeightUnitModel, ZnodeWeightUnit>().ReverseMap();
            Mapper.CreateMap<ZnodePimAddonGroup, AddonGroupModel>();
            Mapper.CreateMap<AddonGroupModel, ZnodePimAddonGroup>();

            Mapper.CreateMap<ZnodePimAddonGroupLocale, AddonGroupLocaleModel>();
            Mapper.CreateMap<AddonGroupLocaleModel, ZnodePimAddonGroupLocale>();

            Mapper.CreateMap<ZnodePimAddOnProduct, AddOnProductModel>();
            Mapper.CreateMap<AddOnProductModel, ZnodePimAddOnProduct>();

            Mapper.CreateMap<AddonGroupLocaleModel, ZnodePimAddOnProduct>();

            Mapper.CreateMap<View_GetPimAddonGroups, AddonGroupModel>();

            Mapper.CreateMap<UserModel, ZnodeUser>();
            Mapper.CreateMap<ZnodeUser, UserModel>();

            Mapper.CreateMap<CMSWidgetConfigurationModel, ZnodeCMSWidgetSliderBanner>().ReverseMap();

            Mapper.CreateMap<BannerModel, ZnodeCMSSliderBanner>();

            Mapper.CreateMap<ZnodeCMSSliderBanner, BannerModel>();
            Mapper.CreateMap<ZnodeCMSArea, CMSAreaModel>();

            Mapper.CreateMap<ZnodeCMSWidget, CMSWidgetsModel>();

            Mapper.CreateMap<ZnodeCMSArea, CMSAreaModel>();

            Mapper.CreateMap<ZnodePublishSeoEntity, SEOUrlModel>().ForMember(d => d.Name, opt => opt.MapFrom(src => src.SEOTypeName));

            Mapper.CreateMap<ShippingRuleModel, ZnodeShippingRule>().ReverseMap();

            Mapper.CreateMap<ZnodePublishMessageEntity, ManageMessageModel>();
            Mapper.CreateMap<ShippingRuleTypeModel, ZnodeShippingRuleType>();

            Mapper.CreateMap<ZnodeShippingRuleType, ShippingRuleTypeModel>();

            Mapper.CreateMap<View_GetManageMessageForEdit, ManageMessageModel>();

            Mapper.CreateMap<ManageMessageModel, View_GetManageMessageForEdit>();

            Mapper.CreateMap<ZnodeAccountAddress, AddressModel>();

            Mapper.CreateMap<CMSContentPageTemplateModel, ZnodeCMSTemplate>();

            Mapper.CreateMap<ZnodeCMSTemplate, CMSContentPageTemplateModel>();

            Mapper.CreateMap<ZnodePimAddOnProductDetail, AddOnProductDetailModel>();

            Mapper.CreateMap<AddOnProductDetailModel, ZnodePimAddOnProductDetail>();

            Mapper.CreateMap<PortalModel, ZnodeCMSPortalTheme>();

            Mapper.CreateMap<ZnodeCMSPortalSEOSetting, PortalSEOSettingModel>();

            Mapper.CreateMap<PortalSEOSettingModel, ZnodeCMSPortalSEOSetting>();


            Mapper.CreateMap<View_GetListOfPimAttributeValues, PIMProductAttributeValuesModel>();

            Mapper.CreateMap<ZnodePortalTaxClass, TaxClassPortalModel>();

            Mapper.CreateMap<TaxClassPortalModel, ZnodePortalTaxClass>();

            Mapper.CreateMap<View_GetAssociatedPortalToTaxClass, PortalModel>();

            Mapper.CreateMap<View_GetAssociatedPortalToTaxClass, TaxClassPortalModel>();

            Mapper.CreateMap<ZnodeCMSUrlRedirect, UrlRedirectModel>();
            Mapper.CreateMap<UrlRedirectModel, ZnodeCMSUrlRedirect>();

            Mapper.CreateMap<ZnodeCMSWidgetCategory, CategoryModel>();

            Mapper.CreateMap<ZnodeCMSWidgetBrand, BrandModel>();

            Mapper.CreateMap<ShippingPortalModel, ZnodeShippingPortal>();

            Mapper.CreateMap<View_GetAssociatedPortalToShipping, PortalModel>();

            Mapper.CreateMap<View_GetAssociatedPortalToShipping, ShippingPortalModel>();


            Mapper.CreateMap<CMSWidgetProductModel, ZnodeCMSWidgetProduct>();
            Mapper.CreateMap<ZnodeCMSWidgetProduct, CMSWidgetProductModel>();

            Mapper.CreateMap<CMSWidgetProductListModel, ZnodeCMSWidgetProduct>().ReverseMap();

            Mapper.CreateMap<SEODetailsModel, ZnodeCMSSEODetail>().ReverseMap();

            Mapper.CreateMap<PromotionModel, ZnodePromotion>();

            Mapper.CreateMap<ZnodePromotion, PromotionModel>()
                .ForMember(d => d.PromotionType, opt => opt.MapFrom(src => src.ZnodePromotionType));
            Mapper.CreateMap<ZnodePromotionType, PromotionTypeModel>();

            Mapper.CreateMap<CouponModel, ZnodePromotionCoupon>();

            Mapper.CreateMap<ZnodePromotionCoupon, CouponModel>();

            Mapper.CreateMap<ZnodePortalAddress, StoreLocatorDataModel>();

            Mapper.CreateMap<StoreLocatorDataModel, ZnodePortalAddress>();

            Mapper.CreateMap<StoreLocatorDataModel, ZnodeAddress>();

            Mapper.CreateMap<CatalogAssociateCategoryModel, ZnodePimCategoryHierarchy>();
            Mapper.CreateMap<ZnodePimCategoryHierarchy, CatalogAssociateCategoryModel>();

            Mapper.CreateMap<AspNetRole, RoleModel>()
                .ForMember(d => d.RoleId, opt => opt.MapFrom(src => src.Id));
            Mapper.CreateMap<RoleModel, AspNetRole>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Name) ? src.Name : src.Name.Trim()));

            Mapper.CreateMap<MenuModel, ZnodeMenu>();
            Mapper.CreateMap<ZnodeMenu, MenuModel>();

            Mapper.CreateMap<AddOnProductModel, ZnodePimAddOnProduct>();
            Mapper.CreateMap<ZnodePimAddOnProduct, AddOnProductModel>();

            Mapper.CreateMap<DomainModel, ZnodeDomain>();
            Mapper.CreateMap<ZnodeDomain, DomainModel>()
                 .ForMember(d => d.DomainName, opt => opt.MapFrom(src => src.DomainName.ToLower().Contains("http") ? src.DomainName : (src.ZnodePortal.UseSSL ? "https://" + src.DomainName : "http://" + src.DomainName)));

            Mapper.CreateMap<ActionPermissionMapperModel, ZnodeMenuActionsPermission>();
            Mapper.CreateMap<ZnodeMenuActionsPermission, ActionPermissionMapperModel>();

            Mapper.CreateMap<ActionModel, ZnodeAction>();
            Mapper.CreateMap<ZnodeAction, ActionModel>();

            Mapper.CreateMap<ZnodeAddress, WarehouseModel>();

            Mapper.CreateMap<WarehouseModel, ZnodeAddress>();

            Mapper.CreateMap<ZnodePaymentSetting, PaymentSettingModel>()
                .ForMember(d => d.PaymentTypeName, opt => opt.MapFrom(src => HelperUtility.IsNotNull(src.ZnodePaymentType) ? src.ZnodePaymentType.Name : null))
            .ForMember(d => d.IsCallToPaymentAPI, opt => opt.MapFrom(src => HelperUtility.IsNotNull(src.ZnodePaymentType) ? src.ZnodePaymentType.IsCallToPaymentAPI : false))
                .ForMember(d => d.PaymentTypeCode, opt => opt.MapFrom(src => HelperUtility.IsNotNull(src.ZnodePaymentType) ? src.ZnodePaymentType.Code : null))
                .ForMember(d => d.TestMode, opt => opt.MapFrom(src => src.IsTestMode));

            Mapper.CreateMap<PaymentSettingModel, ZnodePaymentSetting>().ForMember(d => d.IsTestMode, opt => opt.MapFrom(src => src.TestMode));

            Mapper.CreateMap<CSSModel, ZnodeCMSThemeCSS>();
            Mapper.CreateMap<ZnodeCMSThemeCSS, CSSModel>();

            Mapper.CreateMap<TemplateModel, ZnodeCMSTemplate>();
            Mapper.CreateMap<ZnodeCMSTemplate, TemplateModel>();

            Mapper.CreateMap<ContentPageFolderModel, ZnodeCMSContentPageGroup>();
            Mapper.CreateMap<ContentPageFolderModel, ZnodeCMSContentPageGroupLocale>()
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Code));

            Mapper.CreateMap<CMSTextWidgetConfigurationModel, ZnodeCMSTextWidgetConfiguration>();
            Mapper.CreateMap<ZnodeCMSTextWidgetConfiguration, CMSTextWidgetConfigurationModel>();


            Mapper.CreateMap<CMSMediaWidgetConfigurationModel, ZnodeCMSMediaConfiguration>();
            Mapper.CreateMap<ZnodeCMSMediaConfiguration, CMSMediaWidgetConfigurationModel>();
            Mapper.CreateMap<CmsContainerWidgetConfigurationModel, ZnodeCMSContainerConfiguration>();
            Mapper.CreateMap<ZnodeCMSContainerConfiguration, CmsContainerWidgetConfigurationModel>();

            Mapper.CreateMap<ZnodeLocale, LocaleModel>();

            Mapper.CreateMap<ZnodePortal, WebStorePortalModel>();

            Mapper.CreateMap<WebStorePortalModel, ZnodePortal>();

            Mapper.CreateMap<ZnodeCMSPortalMessage, ManageMessageModel>();

            Mapper.CreateMap<ManageMessageModel, ZnodeCMSPortalMessage>();
            Mapper.CreateMap<View_GetShippingList, ShippingModel>()
                  .ForMember(d => d.ShippingTypeName, opt => opt.MapFrom(src => src.ShippingType))
               .ForMember(d => d.DestinationCountryCode, opt => opt.MapFrom(src => src.CountryCode));

            Mapper.CreateMap<ZnodePortalLocale, DefaultGlobalConfigModel>().ReverseMap();

            Mapper.CreateMap<ZnodeBrandDetail, BrandModel>();

            Mapper.CreateMap<ZnodePublishWebstoreEntity, WebStorePortalModel>()
                .ForMember(d => d.CMSThemeId, opt => opt.MapFrom(src => src.ThemeId));

            Mapper.CreateMap<ERPConfiguratorModel, ZnodeERPConfigurator>().ReverseMap();

            Mapper.CreateMap<ZnodePublishWidgetSliderBannerEntity, CMSWidgetConfigurationModel>()
               .ForMember(d => d.SliderBanners, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<BannerModel>>(src.SliderBanners)))              
               .ForMember(d => d.CMSSliderId, opt => opt.MapFrom(src => src.SliderId))
               .ForMember(d => d.CMSMappingId, opt => opt.MapFrom(src => src.MappingId))
               .ForMember(d => d.CMSWidgetSliderBannerId, opt => opt.MapFrom(src => src.WidgetSliderBannerId));

            Mapper.CreateMap<ZnodePublishContainerWidgetEntity, CmsContainerWidgetConfigurationModel>()
               .ForMember(d => d.WidgetKey, opt => opt.MapFrom(src => src.WidgetsKey));

            Mapper.CreateMap<ZnodePublishBlogNewsEntity, WebStoreBlogNewsModel>();

            Mapper.CreateMap<ZnodeCaseRequest, WebStoreCaseRequestModel>().ReverseMap();

            Mapper.CreateMap<ZnodeCasePriority, CasePriorityModel>().ReverseMap();

            Mapper.CreateMap<ZnodeCaseStatu, CaseStatusModel>().ReverseMap();

            Mapper.CreateMap<ZnodeCaseType, CaseTypeModel>().ReverseMap();


            Mapper.CreateMap<ZnodePublishWidgetBrandEntity, WebStoreWidgetBrandModel>();

            Mapper.CreateMap<ZnodeCMSWidgetBrand, WebStoreWidgetBrandModel>();


            Mapper.CreateMap<ERPTaskSchedulerModel, ZnodeERPTaskScheduler>().ReverseMap();

            Mapper.CreateMap<ZnodeReferralCommissionType, ReferralCommissionTypeModel>();

            Mapper.CreateMap<UserModel, UserAddressModel>();


            Mapper.CreateMap<ZnodeAccountUserOrderApproval, UserModel>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(src => $"{src.ZnodeUser.FirstName} {src.ZnodeUser.LastName}"));

            Mapper.CreateMap<ZnodeEmailTemplate, EmailTemplateModel>().ReverseMap();

            Mapper.CreateMap<ZnodeEmailTemplateArea, EmailTemplateAreaModel>().ReverseMap();

            Mapper.CreateMap<ZnodeEmailTemplateMapper, EmailTemplateAreaMapperModel>().ReverseMap();

            Mapper.CreateMap<ZnodeEmailTemplateLocale, EmailTemplateModel>()
                 .ForMember(d => d.Html, opt => opt.MapFrom(src => src.Content));

            Mapper.CreateMap<EmailTemplateModel, ZnodeEmailTemplateLocale>()
                 .ForMember(d => d.Content, opt => opt.MapFrom(src => src.Html));
            Mapper.CreateMap<GiftCardModel, ZnodeGiftCard>().ReverseMap();

            Mapper.CreateMap<ZnodePortalCountry, CountryModel>();

            Mapper.CreateMap<ZnodeCountry, CountryModel>().ReverseMap();

            Mapper.CreateMap<ZnodeUserWishList, WishListModel>().ForMember(d => d.AddOnProductSKUs, opt => opt.MapFrom(src => src.AddOnSKUs));

            Mapper.CreateMap<WishListModel, ZnodeUserWishList>().ForMember(d => d.AddOnSKUs, opt => opt.MapFrom(src => src.AddOnProductSKUs));

            Mapper.CreateMap<ZnodeUserAddress, AddressModel>().ReverseMap();

            Mapper.CreateMap<ZnodeUserProfile, ProfileModel>().ReverseMap();

            Mapper.CreateMap<ZnodeRmaConfiguration, RMAConfigurationModel>().ReverseMap();

            Mapper.CreateMap<ZnodeRmaRequestStatu, RequestStatusModel>().ReverseMap();

            Mapper.CreateMap<PortalPageSettingModel, ZnodePortalPageSetting>().ReverseMap();
            Mapper.CreateMap<PortalPageSettingModel, ZnodePageSetting>().ReverseMap();

            Mapper.CreateMap<PortalSortSettingModel, ZnodePortalSortSetting>().ReverseMap();
            Mapper.CreateMap<PortalSortSettingModel, ZnodeSortSetting>().ReverseMap();


            Mapper.CreateMap<ZnodeRmaReasonForReturn, RequestStatusModel>()
                .ForMember(d => d.Reason, opt => opt.MapFrom(src => src.Name));

            Mapper.CreateMap<RequestStatusModel, ZnodeRmaReasonForReturn>()
               .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Reason));

            Mapper.CreateMap<ParameterModelForPortalCountries, ZnodePortalCountry>();

            Mapper.CreateMap<ZnodeMedia, MediaManagerModel>()
                .ForMember(d => d.MediaType, opt => opt.MapFrom(src => src.Type))
                 .ForMember(d => d.MediaPathId, opt => opt.MapFrom(src => (HelperUtility.IsNotNull(src.ZnodeMediaCategories) && HelperUtility.IsNotNull(src.ZnodeMediaCategories.FirstOrDefault())) ? src.ZnodeMediaCategories.FirstOrDefault().MediaPathId : 0));

            Mapper.CreateMap<ZnodeMedia, MediaDetailModel>().ReverseMap();

            Mapper.CreateMap<ZnodeShippingPortal, PortalShippingModel>().ReverseMap();

            Mapper.CreateMap<ZnodeTaxPortal, TaxPortalModel>().ReverseMap();

            Mapper.CreateMap<ZnodeHighlight, HighlightModel>().ReverseMap();

            Mapper.CreateMap<ZnodeHighlightLocale, HighlightModel>()
             .ForMember(d => d.HighlightName, opt => opt.MapFrom(src => src.ZnodeHighlight.ZnodeHighlightType.Name))
              .ForMember(d => d.Description, opt => opt.MapFrom(src => src.ZnodeHighlight.ZnodeHighlightType.Description));

            Mapper.CreateMap<HighlightModel, ZnodeHighlightLocale>()
             .ForMember(d => d.Name, opt => opt.MapFrom(src => src.HighlightName))
              .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Description));

            Mapper.CreateMap<HighlightTypeModel, ZnodeHighlightType>();

            Mapper.CreateMap<ZnodeHighlightType, HighlightTypeModel>();

            Mapper.CreateMap<SearchGlobalProductBoostModel, ZnodeSearchGlobalProductBoost>().ReverseMap();

            Mapper.CreateMap<SearchGlobalProductCategoryBoostModel, ZnodeSearchGlobalProductCategoryBoost>().ReverseMap();

            Mapper.CreateMap<SearchDocumentMappingModel, ZnodeSearchDocumentMapping>().ReverseMap();

            Mapper.CreateMap<PriceUserModel, ZnodePriceListAccount>().ReverseMap();

            Mapper.CreateMap<ZnodePortalProfile, PortalProfileModel>()
            .ForMember(d => d.ProfileName, opt => opt.MapFrom(src => src.ZnodeProfile.ProfileName));

            Mapper.CreateMap<ZnodeOmsOrder, OrderModel>().ReverseMap();

            Mapper.CreateMap<ZnodeOmsOrderDetail, OrderModel>()
                .ForMember(d => d.PaymentStatus, opt => opt.MapFrom(src => src.ZnodeOmsPaymentState.Name))
                .ForMember(d => d.OrderState, opt => opt.MapFrom(src => src.ZnodeOmsOrderState.Description))
                .ForMember(d => d.PaymentType, opt => opt.MapFrom(src => src.ZnodePaymentType.Name))
                .ForMember(d => d.ShippingTypeName, opt => opt.MapFrom(src => src.ZnodeShipping.Description))
                .ForMember(d => d.OrderNotes, opt => opt.MapFrom(src => src.ZnodeOmsNotes));

            Mapper.CreateMap<ZnodeOmsOrderDetail, AddressModel>()
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.BillingFirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.BillingLastName))
                .ForMember(d => d.CountryName, opt => opt.MapFrom(src => src.BillingCountry))
                .ForMember(d => d.StateName, opt => opt.MapFrom(src => src.BillingStateCode))
                .ForMember(d => d.CityName, opt => opt.MapFrom(src => src.BillingCity))
                .ForMember(d => d.PostalCode, opt => opt.MapFrom(src => src.BillingPostalCode))
                .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(src => src.BillingPhoneNumber))
                .ForMember(d => d.EmailAddress, opt => opt.MapFrom(src => src.BillingEmailId))
                .ForMember(d => d.Address1, opt => opt.MapFrom(src => src.BillingStreet1))
                .ForMember(d => d.CompanyName, opt => opt.MapFrom(src => src.BillingCompanyName))
                .ForMember(d => d.Address2, opt => opt.MapFrom(src => src.BillingStreet2));

            Mapper.CreateMap<ZnodeOmsOrderShipment, AddressModel>()
               .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.ShipToFirstName))
               .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.ShipToLastName))
               .ForMember(d => d.CountryName, opt => opt.MapFrom(src => src.ShipToCountry))
               .ForMember(d => d.StateName, opt => opt.MapFrom(src => src.ShipToStateCode))
               .ForMember(d => d.CityName, opt => opt.MapFrom(src => src.ShipToCity))
               .ForMember(d => d.PostalCode, opt => opt.MapFrom(src => src.ShipToPostalCode))
               .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(src => src.ShipToPhoneNumber))
               .ForMember(d => d.EmailAddress, opt => opt.MapFrom(src => src.ShipToEmailId))
               .ForMember(d => d.Address1, opt => opt.MapFrom(src => src.ShipToStreet1))
               .ForMember(d => d.CompanyName, opt => opt.MapFrom(src => src.ShipToCompanyName))
               .ForMember(d => d.Address2, opt => opt.MapFrom(src => src.ShipToStreet2));

            Mapper.CreateMap<OrderModel, ZnodeOmsOrderDetail>();

            Mapper.CreateMap<ZnodeOmsOrderLineItem, OrderLineItemModel>()
                .ForMember(d => d.OrderLineItemStateId, opt => opt.MapFrom(src => src.ZnodeOmsOrderState.OmsOrderStateId))
                .ForMember(d => d.IsShowToCustomer, opt => opt.MapFrom(src => src.ZnodeOmsOrderState.IsShowToCustomer))
                .ForMember(d => d.OrderLineItemState, opt => opt.MapFrom(src => src.ZnodeOmsOrderState.Description))
                .ForMember(d => d.Attributes, opt => opt.MapFrom(src => src.ZnodeOmsOrderAttributes))
                .ForMember(d => d.TaxTransactionNumber, opt => opt.MapFrom(src => HelperUtility.IsNotNull(src.ZnodeOmsTaxOrderLineDetails.FirstOrDefault(x => !string.IsNullOrEmpty(x.TaxTransactionNumber))) ? src.ZnodeOmsTaxOrderLineDetails.FirstOrDefault(x => !string.IsNullOrEmpty(x.TaxTransactionNumber)).TaxTransactionNumber : null))
                .ForMember(d => d.SalesTax, opt => opt.MapFrom(src => src.ZnodeOmsTaxOrderLineDetails.Count > 0 ? (src.ZnodeOmsTaxOrderLineDetails.FirstOrDefault().GST + src.ZnodeOmsTaxOrderLineDetails.FirstOrDefault().PST + src.ZnodeOmsTaxOrderLineDetails.FirstOrDefault().HST + src.ZnodeOmsTaxOrderLineDetails.FirstOrDefault().VAT + src.ZnodeOmsTaxOrderLineDetails.FirstOrDefault().SalesTax) : 0)).ReverseMap();

            Mapper.CreateMap<ZnodeMedia, MediaManagerModel>()
            .ForMember(d => d.MediaType, opt => opt.MapFrom(src => src.Type));

            Mapper.CreateMap<MediaManagerModel, ZnodeMedia>()
            .ForMember(d => d.Type, opt => opt.MapFrom(src => src.MediaType));

            Mapper.CreateMap<MediaAttributeValuesModel, ZnodeMediaAttributeValue>()
            .ForMember(d => d.MediaAttributeId, opt => opt.MapFrom(src => src.AttributeId))
            .ForMember(d => d.MediaAttributeDefaultValueId, opt => opt.MapFrom(src => (src.DefaultAttributeValueId < 1) ? null : src.DefaultAttributeValueId))
            .ForMember(d => d.AttributeValue, opt => opt.MapFrom(src => src.MediaAttributeValue))
            .ForMember(d => d.MediaAttributeValueId, opt => opt.MapFrom(src => src.MediaAttributeValueId.GetValueOrDefault()))
            .ForMember(d => d.MediaCategoryId, opt => opt.MapFrom(src => (src.MediaCategoryId < 1) ? null : src.MediaCategoryId));

            Mapper.CreateMap<ZnodeMediaAttributeValue, MediaAttributeValuesModel>()
           .ForMember(d => d.AttributeId, opt => opt.MapFrom(src => src.MediaAttributeId))
           .ForMember(d => d.DefaultAttributeValueId, opt => opt.MapFrom(src => src.MediaAttributeDefaultValueId))
           .ForMember(d => d.MediaAttributeValue, opt => opt.MapFrom(src => src.AttributeValue));

            Mapper.CreateMap<View_GetAttributeFamilyByName, MediaAttributeFamily>()
            .ForMember(d => d.AttributeFamilyId, opt => opt.MapFrom(src => src.MediaAttributeFamilyId))
            .ForMember(d => d.FamilyLocaleId, opt => opt.MapFrom(src => src.MediaFamilyLocaleId))
            .ForMember(d => d.MaxFileSize, opt => opt.MapFrom(src => src.Name));

            Mapper.CreateMap<ZnodeMediaConfiguration, MediaConfigurationModel>()
            .ForMember(d => d.MediaServerMasterId, opt => opt.MapFrom(src => src.MediaServerMasterId))
            .ForMember(d => d.MediaServer, opt => opt.MapFrom(src => src.ZnodeMediaServerMaster))
            .ForMember(d => d.ThumbnailFolderName, opt => opt.MapFrom(src => !Equals(src.ZnodeMediaServerMaster, null) ? src.ZnodeMediaServerMaster.ThumbnailFolderName : string.Empty));

            Mapper.CreateMap<ZnodeMediaServerMaster, MediaServerModel>()
            .ForMember(d => d.MediaServerMasterId, opt => opt.MapFrom(src => src.MediaServerMasterId));

            Mapper.CreateMap<ZnodeOmsOrderLineItem, ShoppingCartItemModel>()
            .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.Price))         
            .ForMember(d => d.ProductDiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
            .ForMember(d => d.CartDescription, opt => opt.MapFrom(src => src.Description));

            Mapper.CreateMap<ZnodeOmsSavedCartLineItem, ShoppingCartItemModel>()
                .ForMember(d => d.AutoAddonSKUs, opt => opt.MapFrom(src => src.AutoAddon))
                .ReverseMap();

            Mapper.CreateMap<ZnodeOmsOrderLineItem, AssociatedProductModel>().ReverseMap();

            Mapper.CreateMap<ZnodeOmsOrderDetail, ShoppingCartModel>()
                 .ForMember(d => d.Discount, opt => opt.MapFrom(src => src.DiscountAmount));

            Mapper.CreateMap<ZnodeMediaServerMaster, MediaServerModel>();

            Mapper.CreateMap<ZnodeMediaConfiguration, MediaConfigurationModel>()
               .ForMember(d => d.ThumbnailFolderName, opt => opt.MapFrom(src => !Equals(src.ZnodeMediaServerMaster, null) ? src.ZnodeMediaServerMaster.ThumbnailFolderName : string.Empty))
            .ForMember(d => d.NetworkUrl, opt => opt.MapFrom(src => src.Custom1));
            Mapper.CreateMap<MediaConfigurationModel, ZnodeMediaConfiguration>()
            .ForMember(d => d.Custom1, opt => opt.MapFrom(src => src.NetworkUrl));

            Mapper.CreateMap<View_ManageProductList, CategoryProductModel>()
             .ForMember(d => d.PimProductId, opt => opt.MapFrom(src => src.ProductId));

            Mapper.CreateMap<CategoryProductModel, View_ManageProductList>()
            .ForMember(d => d.ProductId, opt => opt.MapFrom(src => src.PimProductId));

            Mapper.CreateMap<PIMAttributeDefaultValueModel, ZnodePimAttributeDefaultValue>().ReverseMap();

            Mapper.CreateMap<BrandModel, ZnodeBrandDetail>().ReverseMap();

            Mapper.CreateMap<ZnodePublishPortalBrandEntity, BrandModel>();


            Mapper.CreateMap<PortalProfileModel, ZnodePortalProfile>().ReverseMap();

            Mapper.CreateMap<VendorModel, ZnodePimVendor>().ReverseMap();

            Mapper.CreateMap<ZnodeOmsOrderShipment, OrderShipmentModel>().ReverseMap();

            Mapper.CreateMap<PortalIndexModel, ZnodeCatalogIndex>();

            Mapper.CreateMap<ZnodeCatalogIndex, PortalIndexModel>();                

            Mapper.CreateMap<ImportTypeModel, ZnodeImportHead>().ReverseMap();
            Mapper.CreateMap<ImportTemplateModel, ZnodeImportTemplate>().ReverseMap();
            Mapper.CreateMap<ImportTemplateMappingModel, ZnodeImportTemplateMapping>().ReverseMap();

            Mapper.CreateMap<PortalUnitModel, ZnodePortalUnit>()
                 .ForMember(d => d.CurrencyId, opt => opt.MapFrom(src => src.CurrencyTypeID));

            Mapper.CreateMap<ZnodePortalUnit, PortalUnitModel>()
                 .ForMember(d => d.CurrencyTypeID, opt => opt.MapFrom(src => src.CurrencyId));

            Mapper.CreateMap<ZnodeSearchIndexServerStatu, SearchIndexServerStatusModel>().ReverseMap();

            Mapper.CreateMap<ZnodeSearchIndexMonitor, SearchIndexMonitorModel>().ReverseMap();

            Mapper.CreateMap<ZnodeImportLog, ImportLogsModel>()
                .ForMember(d => d.TemplateName, opt => opt.MapFrom(src => src.ZnodeImportProcessLog.ZnodeImportTemplate.TemplateName))
                .ReverseMap();

            Mapper.CreateMap<ZnodeImportProcessLog, ImportLogsModel>()
                .ForMember(d => d.TemplateName, opt => opt.MapFrom(src => src.ZnodeImportTemplate.TemplateName))
                .ReverseMap();

            Mapper.CreateMap<AccountQuoteModel, ZnodeOmsQuote>();

            Mapper.CreateMap<ZnodeOmsQuote, AccountQuoteModel>()
            .ForMember(d => d.OrderNotes, opt => opt.MapFrom(src => src.ZnodeOmsNotes))
            .ForMember(d => d.AccountQuoteLineItemList, opt => opt.MapFrom(src => src.ZnodeOmsQuoteLineItems));

            Mapper.CreateMap<AccountQuoteLineItemModel, ZnodeOmsQuoteLineItem>().ReverseMap();

            Mapper.CreateMap<ZnodeOmsSavedCartLineItem, AccountQuoteLineItemModel>();

            Mapper.CreateMap<ContentPageModel, View_GetContentPageDetails>()
             .ForMember(d => d.StoreName, opt => opt.MapFrom(src => src.PortalName));

            Mapper.CreateMap<View_GetContentPageDetails, ContentPageModel>()
                .ForMember(d => d.PortalName, opt => opt.MapFrom(src => src.StoreName));

            Mapper.CreateMap<SearchIndexServerStatusModel, ZnodeSearchIndexServerStatu>().ReverseMap();

            Mapper.CreateMap<RefundPaymentModel, ZnodeOmsPaymentRefund>().ReverseMap();

            Mapper.CreateMap<TaxOrderDetailsModel, ZnodeOmsTaxOrderDetail>().ReverseMap();

            Mapper.CreateMap<TaxOrderLineDetailsModel, ZnodeOmsTaxOrderLineDetail>().ReverseMap();

            Mapper.CreateMap<View_CustomerReferralCommissionDetail, ReferralCommissionModel>().ReverseMap();

            Mapper.CreateMap<View_GetProfileCatalog, ProfileCatalogModel>().ReverseMap();

            Mapper.CreateMap<ZnodeOmsOrderDiscount, OrderDiscountModel>().ReverseMap();

            Mapper.CreateMap<ZnodeOmsOrderAttribute, OrderAttributeModel>().ReverseMap();

            Mapper.CreateMap<View_AccountProfileList, ProfileModel>().ReverseMap();

            Mapper.CreateMap<View_GetNotes, NoteModel>().ReverseMap();

            Mapper.CreateMap<ZnodeCaseRequestHistory, WebStoreCaseRequestModel>().ReverseMap();
            Mapper.CreateMap<ZnodeOmsNote, OrderNotesModel>().ReverseMap();
            Mapper.CreateMap<View_GetOmsOrderNotes, OrderNotesModel>();
            Mapper.CreateMap<ReferralCommissionModel, ZnodeOmsReferralCommission>().ReverseMap();
            Mapper.CreateMap<View_GetRMASearchRequest, RMARequestModel>().ReverseMap();
            Mapper.CreateMap<ZnodeRmaRequest, RMARequestModel>().ReverseMap();
            Mapper.CreateMap<ZnodeRmaRequestItem, RMARequestItemModel>().ReverseMap();

            Mapper.CreateMap<ZnodeOmsTemplate, AccountTemplateModel>().ReverseMap();
            Mapper.CreateMap<TemplateCartItemModel, ZnodeOmsTemplateLineItem>().ReverseMap();
            Mapper.CreateMap<ZnodePimAttributeFamily, ImportProductFamilyModel>().ReverseMap();
            Mapper.CreateMap<ZnodeGiftCard, IssuedGiftCardModel>().ReverseMap();
            Mapper.CreateMap<View_QuoteOrderTemplateDetail, AccountTemplateModel>().ReverseMap();
            Mapper.CreateMap<ZnodePublishCatalogLog, PublishCatalogLogModel>();

            Mapper.CreateMap<CurrencyModel, ZnodeCurrency>().ReverseMap();
            Mapper.CreateMap<OrderPaymentStateModel, ZnodeOmsPaymentState>().ReverseMap();

            Mapper.CreateMap<ZnodeProfileShipping, PortalProfileShippingModel>().ReverseMap();
            Mapper.CreateMap<ZnodeCustomReportTemplate, ReportModel>()
              .ForMember(d => d.Name, opt => opt.MapFrom(src => src.ReportName))
             .ForMember(d => d.Path, opt => opt.MapFrom(src => "/" + ZnodeApiSettings.ReportServerDynamicReportFolderName + "/" + src.ReportName + ".rdl"))
            .ForMember(d => d.FolderName, opt => opt.MapFrom(src => ZnodeApiSettings.ReportServerDynamicReportFolderName));

            Mapper.CreateMap<AddonGroupProductModel, ZnodePimAddonGroupProduct>().ReverseMap();
            Mapper.CreateMap<OrderHistoryModel, ZnodeOmsHistory>().ReverseMap();
            Mapper.CreateMap<ZnodeOmsNote, OrderHistoryModel>()
                .ForMember(d => d.Message, opt => opt.MapFrom(src => src.Notes));

            Mapper.CreateMap<ZnodeOmsOrderState, OrderStateModel>().ReverseMap();

            Mapper.CreateMap<AssociatedProductModel, ZnodeOmsTemplateLineItem>().ReverseMap();
            Mapper.CreateMap<ZnodeOmsTemplateLineItem, AssociatedProductModel>().ReverseMap();

            Mapper.CreateMap<ZnodeGoogleTagManager, TagManagerModel>().ReverseMap();
            Mapper.CreateMap<PortalTrackingPixelModel, ZnodePortalPixelTracking>();

            Mapper.CreateMap<ZnodePortalPixelTracking, PortalTrackingPixelModel>()
                  .ForMember(d => d.StoreName, opt => opt.MapFrom(src => src.ZnodePortal.StoreName));

            Mapper.CreateMap<ReturnOrderLineItemModel, ZnodeOmsOrderLineItem>()
                .ForMember(d => d.OmsOrderDetailsId, opt => opt.MapFrom(src => src.OrderDetailId))
                .ForMember(d => d.OrderLineItemStateId, opt => opt.MapFrom(src => src.OmsOrderStatusId))
                .ForMember(d => d.RmaReasonForReturnId, opt => opt.MapFrom(src => src.ReasonForReturnId))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.CartDescription))
                .ForMember(d => d.Price, opt => opt.MapFrom(src => src.UnitPrice)).ReverseMap();

            Mapper.CreateMap<ZnodeProductFeed, ProductFeedModel>()
                .ForMember(d => d.ProductFeedTypeCode, opt => opt.MapFrom(src => src.ZnodeProductFeedType.ProductFeedTypeCode))
                .ForMember(d => d.StoreName, opt => opt.MapFrom(src => src.ZnodePortal.StoreName))
                .ForMember(d => d.ProductFeedTypeName, opt => opt.MapFrom(src => src.ZnodeProductFeedType.ProductFeedTypeName))
                .ForMember(d => d.ProductFeedSiteMapTypeCode, opt => opt.MapFrom(src => src.ZnodeProductFeedSiteMapType.ProductFeedSiteMapTypeCode)).ReverseMap();

            Mapper.CreateMap<ZnodeProductFeedType, ProductFeedTypeModel>().ReverseMap();
            Mapper.CreateMap<ZnodeProductFeedSiteMapType, ProductFeedSiteMapTypeModel>().ReverseMap();

            Mapper.CreateMap<ZnodeBlogNew, BlogNewsModel>().ReverseMap();
            Mapper.CreateMap<ZnodeBlogNewsLocale, BlogNewsModel>().ReverseMap();
            Mapper.CreateMap<ZnodeCMSSEODetail, BlogNewsModel>().ReverseMap();
            Mapper.CreateMap<ZnodeCMSSEODetailLocale, BlogNewsModel>().ReverseMap();
            Mapper.CreateMap<ZnodeBlogNewsContent, BlogNewsModel>().ReverseMap();
            Mapper.CreateMap<ZnodeBlogNewsComment, WebStoreBlogNewsCommentModel>().ReverseMap();
            Mapper.CreateMap<WebStoreBlogNewsCommentModel, ZnodeBlogNewsCommentLocale>()
                 .ForMember(d => d.BlogComment, opt => opt.MapFrom(src => src.BlogNewsComment)).ReverseMap();

            Mapper.CreateMap<ZnodeBlogNewsComment, BlogNewsCommentModel>().ReverseMap();

            Mapper.CreateMap<ZnodeRobotsTxt, RobotsTxtModel>().ReverseMap();

            Mapper.CreateMap<View_GetLocaleDetails, LocaleModel>().ReverseMap();

            Mapper.CreateMap<ZnodeSearchSynonym, SearchSynonymsModel>().ReverseMap();

            Mapper.CreateMap<ZnodeSearchKeywordsRedirect, SearchKeywordsRedirectModel>().ReverseMap();

            Mapper.CreateMap<SearchKeywordsRedirectModel, ZnodeSearchKeywordsRedirect>().ReverseMap();

            Mapper.CreateMap<OrderShippingModel, ZnodeOmsCustomerShipping>().ReverseMap();

            Mapper.CreateMap<ZnodeApplicationCache, CacheModel>().ReverseMap();

            Mapper.CreateMap<ZnodeGlobalAttribute, GlobalAttributeModel>().ReverseMap();
            Mapper.CreateMap<ZnodeGlobalAttributeLocale, GlobalAttributeLocaleModel>().ReverseMap();
            Mapper.CreateMap<ZnodeGlobalAttributeDefaultValue, GlobalAttributeDefaultValueModel>().ReverseMap();


            Mapper.CreateMap<ZnodeGlobalAttributeGroup, GlobalAttributeGroupModel>().ReverseMap();
            Mapper.CreateMap<ZnodeGlobalAttributeGroupLocale, GlobalAttributeGroupLocaleModel>().ReverseMap();
            Mapper.CreateMap<ZnodeGlobalAttributeGroupMapper, GlobalAttributeGroupMapperModel>().ReverseMap();

            Mapper.CreateMap<ZnodeGlobalAttributeFamily, GlobalAttributeFamilyModel>().ReverseMap();
            Mapper.CreateMap<ZnodeGlobalAttributeFamily, GlobalAttributeFamilyCreateModel > ().ReverseMap();
            Mapper.CreateMap<ZnodeGlobalAttributeFamilyLocale, GlobalAttributeFamilyLocaleModel>().ReverseMap();

            Mapper.CreateMap<ZnodeGlobalEntity, GlobalEntityModel>().ReverseMap();

            Mapper.CreateMap<ZnodeSearchProfile, SearchProfileModel>().ReverseMap();

            Mapper.CreateMap<ZnodeSearchQueryType, SearchQueryTypeModel>().ReverseMap();

            Mapper.CreateMap<ZnodeSearchFeature, SearchFeatureModel>().ReverseMap();

            Mapper.CreateMap<ZnodePimDownloadableProductKey, DownloadableProductKeyModel>().ReverseMap();


            Mapper.CreateMap<ZnodeSearchProfileAttributeMapping, SearchAttributesModel>().ReverseMap();

            Mapper.CreateMap<SearchFeatureModel, ZnodeSearchProfileFeatureMapping>();




            Mapper.CreateMap<ZnodeSearchProfileAttributeMapping, SearchAttributesModel>().ReverseMap();


            Mapper.CreateMap<ZnodeSearchProfileTrigger, SearchTriggersModel>()
                 .ForMember(d => d.UserProfile, opt => opt.MapFrom(src => src.ZnodeProfile.ProfileName));

            Mapper.CreateMap<SearchTriggersModel, ZnodeSearchProfileTrigger>();

            Mapper.CreateMap<ZnodeFormBuilder, FormBuilderModel>().ReverseMap();

            Mapper.CreateMap<ZnodeCMSFormWidgetConfiguration, CMSFormWidgetConfigrationModel>().ReverseMap();

            Mapper.CreateMap<ZnodePortalSearchProfile, PortalSearchProfileModel>().ReverseMap();
            Mapper.CreateMap<ZnodeFormWidgetEmailConfiguration, FormWidgetEmailConfigurationModel>().ReverseMap();

            Mapper.CreateMap<LogMessageEntity, LogMessageModel>().ReverseMap();

            Mapper.CreateMap<FilterTuple, FilterMongoTuple>();
            Mapper.CreateMap<ZnodeAddress, AddressModel>().ReverseMap();

            Mapper.CreateMap<ZnodeCMSSearchWidget, CMSSearchWidgetConfigurationModel>().ReverseMap();

            Mapper.CreateMap<ZnodeApproverLevel, ApproverLevelModel>().ReverseMap();
            Mapper.CreateMap<ZnodeUserApprover, UserApproverModel>()
               .ForMember(d => d.ApproverName, opt => opt.MapFrom(src => src.ZnodeUser.Email));
            Mapper.CreateMap<UserApproverModel, ZnodeUserApprover>();

            Mapper.CreateMap<ZnodePortalPaymentGroup, PortalPaymentApproverModel>().ReverseMap();


            Mapper.CreateMap<ZnodeCMSSearchIndex, CMSPortalContentPageIndexModel>().ReverseMap();

            Mapper.CreateMap<ZnodeCMSSearchIndexMonitor, CMSSearchIndexMonitorModel>().ReverseMap();

            Mapper.CreateMap<ZnodeCMSSearchIndexServerStatu, CMSSearchIndexServerStatusModel>().ReverseMap();

            // Added two entries as reverse map is not working in this case.
            Mapper.CreateMap<ZnodeRmaReturnDetail, RMAReturnModel>()
                .ForMember(d => d.ReturnImportDuty, opt => opt.MapFrom(src => src.ImportDuty)).ReverseMap();

            Mapper.CreateMap<RMAReturnModel, ZnodeRmaReturnDetail>()
               .ForMember(d => d.ImportDuty, opt => opt.MapFrom(src => src.ReturnImportDuty)).ReverseMap();

            Mapper.CreateMap<ZnodeRmaReturnLineItem, RMAReturnLineItemModel>().ReverseMap();
            Mapper.CreateMap<ZnodeRmaReturnNote, RMAReturnNotesModel>().ReverseMap();
            Mapper.CreateMap<ZnodeRmaReturnState, RMAReturnStateModel>().ReverseMap();
            Mapper.CreateMap<ZnodeRmaReturnHistory, RMAReturnHistoryModel>().ReverseMap();

            Mapper.CreateMap<QuoteTaxDetailsModel, ZnodeOmsQuoteTaxOrderDetail>().ReverseMap();
            Mapper.CreateMap<ZnodeOmsQuoteOrderDiscount, QuoteDiscountModel>().ReverseMap();
        }

        static void RegisterV2Maps()
        {
            Mapper.CreateMap<PublishProductModel, CategoryProductModelV2>()
                .ForMember(d => d.ZnodeCategoryId, opt => opt.MapFrom(src => src.ZnodeCategoryIds)).ReverseMap();
            Mapper.CreateMap<PublishProductModel, PublishProductModelV2>().ReverseMap();
            Mapper.CreateMap<CreateUserModelV2, UserModel>().ReverseMap();
            Mapper.CreateMap<ZnodeUser, GuestUserModelV2>().ReverseMap();
            Mapper.CreateMap<AddressModel, UserAddressV2>().ReverseMap();
            Mapper.CreateMap<CreateOrderModelV2, ShoppingCartModel>().ReverseMap();
            Mapper.CreateMap<OrderShippingModel, ShippingModel>().ReverseMap();
            Mapper.CreateMap<AddressModelV2, ZnodeAddress>().ReverseMap();
            Mapper.CreateMap<ZnodeOmsOrderWarehouse, OrderWarehouseModel>().ReverseMap();

            Mapper.CreateMap<ZnodeOmsOrderLineItemsAdditionalCost, OrderLineItemAdditionalCostModel>().ReverseMap();

            Mapper.CreateMap<PublishStateMappingModel, ZnodePublishStateApplicationTypeMapping>()
               .ForMember(d => d.ApplicationType, opt => opt.MapFrom(src => src.ApplicationType))
               .ForMember(d => d.IsEnabled, opt => opt.MapFrom(src => src.IsEnabled))
               .ForMember(d => d.PublishStateId, opt => opt.MapFrom(src => src.PublishStateId))
               .ForMember(d => d.PublishStateMappingId, opt => opt.MapFrom(src => src.PublishStateMappingId));

            Mapper.CreateMap<ZnodePublishStateApplicationTypeMapping, PublishStateMappingModel>()
               .ForMember(d => d.ApplicationType, opt => opt.MapFrom(src => src.ApplicationType))
               .ForMember(d => d.IsEnabled, opt => opt.MapFrom(src => src.IsEnabled))
               .ForMember(d => d.PublishStateId, opt => opt.MapFrom(src => src.PublishStateId))
               .ForMember(d => d.PublishState, opt => opt.MapFrom(src => HelperUtility.IsNotNull(src.ZnodePublishState) ? src.ZnodePublishState.DisplayName : null))
               .ForMember(d => d.PublishStateMappingId, opt => opt.MapFrom(src => src.PublishStateMappingId));

            Mapper.CreateMap<ZnodePublishPreviewLogEntity, PublishHistoryModel>()
                .ForMember(d => d.SourcePublishState, opt => opt.MapFrom(src => src.SourcePublishState));

            Mapper.CreateMap<ZnodePublishProgressNotifierEntity, ProgressNotificationModel>().ReverseMap();
          Mapper.CreateMap<StockNotificationModel, ZnodeStockNotice>()
           .ForMember(d => d.SKU, opt => opt.MapFrom(src => src.ProductSKU));

            Mapper.CreateMap<StockNotificationResponseModel, ZnodeStockNotice>()
         .ForMember(d => d.SKU, opt => opt.MapFrom(src => src.ProductSKU));


            Mapper.CreateMap<CultureModel, ZnodeCulture>().ReverseMap();
            Mapper.CreateMap<PortalApprovalModel, ZnodePortalApproval>().ReverseMap();
            Mapper.CreateMap<ZnodePublishCatalogLog, PublishCatalogLogModel>().ReverseMap();

            #region Content Containers
            Mapper.CreateMap<ContentContainerCreateModel, ZnodeCMSContentContainer>()
                .ForMember(d => d.ContainerKey, opt => opt.MapFrom(src => src.ContainerKey));

            Mapper.CreateMap<ZnodeCMSContentContainer, ContentContainerCreateModel>()
                .ForMember(d => d.ContainerKey, opt => opt.MapFrom(src => src.ContainerKey));

            Mapper.CreateMap<OrderModel, OrderPaymentCreateModel>().ReverseMap();

            Mapper.CreateMap<ContentContainerResponseModel, ZnodeCMSContentContainer>().ReverseMap()
                .ForMember(d => d.ContentContainerId, opt => opt.MapFrom(src => src.CMSContentContainerId))
                .ForMember(d => d.ContainerKey, opt => opt.MapFrom(src => src.ContainerKey));
        

            Mapper.CreateMap<AssociatedVariantModel, ZnodeCMSContainerProfileVariant>()
                .ForMember(d => d.CMSContainerProfileVariantId, opt => opt.MapFrom(src => src.ContainerProfileVariantId))
                .ForMember(d => d.CMSContentContainerId, opt => opt.MapFrom(src => src.ContentContainerId));

            Mapper.CreateMap<AssociatedVariantModel, ZnodeCMSContainerProfileVariantLocale>()
                .ForMember(d => d.CMSContainerProfileVariantId, opt => opt.MapFrom(src => src.ContainerProfileVariantId))
                .ForMember(d => d.CMSContainerTemplateId, opt => opt.MapFrom(src => src.ContainerTemplateId));

            Mapper.CreateMap<ContentContainerResponseModel, ZnodeGlobalEntityFamilyMapper>()
                .ForMember(d => d.GlobalAttributeFamilyId, opt => opt.MapFrom(src => src.FamilyId))
                .ForMember(d => d.GlobalEntityValueId, opt => opt.MapFrom(src => src.ContentContainerId))
                .ForMember(d => d.GlobalEntityId, opt => opt.MapFrom(src => src.GlobalEntityId));

            Mapper.CreateMap<ContainerTemplateCreateModel, ZnodeCMSContainerTemplate>().ReverseMap();

            Mapper.CreateMap<OrderModel, PlaceOrderModel>();

            Mapper.CreateMap<OrderLineItemModel, PlaceOrderLineItemModel>();

            Mapper.CreateMap<OrderLineItemModel, PlaceOrderlineItemCollection>();

            Mapper.CreateMap<OrderDiscountModel, PlaceOrderDiscountModel>();

            Mapper.CreateMap<OrderLineItemModel, PlaceOrderlineItemCollection>();

            Mapper.CreateMap<ContainerTemplateModel, ZnodeCMSContainerTemplate>().ReverseMap()
                .ForMember(d => d.ContainerTemplateId, opt => opt.MapFrom(src => src.CMSContainerTemplateId));

            #endregion



            #region Shopping cart mappers
            Mapper.CreateMap<ShoppingCartModel, ShoppingCartModelV2>().ReverseMap();
            Mapper.CreateMap<ShoppingCartModel, ShoppingCartCalculateRequestModelV2>().ReverseMap();
            Mapper.CreateMap<ShoppingCartItemModelV2, ShoppingCartItemModel>()
                .ForMember(d => d.ConfigurableProductSKUs, opt => opt.MapFrom(src => src.ChildProductSKU));
            Mapper.CreateMap<ShoppingCartItemModel, ShoppingCartItemModelV2>()
                .ForMember(d => d.ChildProductSKU, opt => opt.MapFrom(src => src.ConfigurableProductSKUs));
            Mapper.CreateMap<CartParameterModel, RemoveCartItemModelV2>().ReverseMap();

            Mapper.CreateMap<ZnodeECertificate, ECertificateModel>().ReverseMap();
            Mapper.CreateMap<PortalApprovalLevelModel, ZnodePortalApprovalLevel>().ReverseMap();
            Mapper.CreateMap<PortalApprovalTypeModel, ZnodePortalApprovalType>().ReverseMap();
            #endregion

            #region PDP Lite mappers

            //This mapping should not be tried in reverse direction until a reverse AfterMap() is defined.
            Mapper.CreateMap<PublishProductModel, PublishProductDTO>()
              .AfterMap((src, dest) =>
              {
                  //Product Images
                  if (Equals(dest.ProductImage, null)) dest.ProductImage = new ProductImageDTO();
                  dest.ProductImage.AlternateImages = src.AlternateImages ?? new List<ProductAlterNateImageModel>();
                  dest.ProductImage.ImageLargePath = src.ImageLargePath;
                  dest.ProductImage.ImageMediumPath = src.ImageMediumPath;
                  dest.ProductImage.ImageSmallPath = src.ImageSmallPath;
                  dest.ProductImage.ImageSmallThumbnailPath = src.ImageSmallThumbnailPath;
                  dest.ProductImage.ImageThumbNailPath = src.ImageThumbNailPath;
                  dest.ProductImage.OriginalImagepath = src.OriginalImagepath;
                  dest.ProductImage.ProductImagePath = src.ProductImagePath;

                  //SEO
                  if (Equals(dest.SEO, null)) dest.SEO = new ProductSeoDTO();
                  dest.SEO.SEODescription = src.SEODescription;
                  dest.SEO.SEOKeywords = src.SEOKeywords;
                  dest.SEO.SEOTitle = src.SEOTitle;
                  dest.SEO.SEOUrl = src.SEOUrl;
                  dest.SEO.ParentSEOCode = src.ParentSEOCode;
                  dest.SEO.SEOCode = src.SEOCode;

                  //Pricing
                  if (Equals(dest.Pricing, null)) dest.Pricing = new ProductPricingDTO();
                  dest.Pricing.CultureCode = src.CultureCode;
                  dest.Pricing.CurrencyCode = src.CurrencyCode;
                  dest.Pricing.ProductPrice = src.ProductPrice;
                  dest.Pricing.PromotionalPrice = src.PromotionalPrice;
                  dest.Pricing.RetailPrice = src.RetailPrice;
                  dest.Pricing.SalesPrice = src.SalesPrice;
                  dest.Pricing.TierPriceList = src.TierPriceList ?? new List<PriceTierModel>();
                  dest.Pricing.AdditionalCost = src.AdditionalCost ?? new Dictionary<string, decimal>();
                  dest.Pricing.ShippingCost = src.ShippingCost;

                  //Inventory
                  if (Equals(dest.InventoryDetails, null)) dest.InventoryDetails = new ProductInventoryDTO();
                  dest.InventoryDetails.Inventory = src.Inventory ?? new List<InventorySKUModel>();
                  dest.InventoryDetails.InventoryMessage = src.InventoryMessage;
                  dest.InventoryDetails.Quantity = src.Quantity;
                  dest.InventoryDetails.ReOrderLevel = src.ReOrderLevel;
                  dest.InventoryDetails.ShowAddToCart = src.ShowAddToCart;
                  dest.InventoryDetails.GroupProductPriceMessage = src.GroupProductPriceMessage;

                  //Product related store settings.
                  if (Equals(dest.StoreSettings, null)) dest.StoreSettings = new ProductStoreSettingsDTO();
                  dest.StoreSettings.BackOrderMessage = src.BackOrderMessage;
                  dest.StoreSettings.InStockMessage = src.InStockMessage;
                  dest.StoreSettings.OutOfStockMessage = src.OutOfStockMessage;

                  //Product Brand.
                  if (Equals(dest.Brand, null)) dest.Brand = new ProductBrandDTO();
                  dest.Brand.BrandId = src.BrandId;
                  dest.Brand.BrandSeoUrl = src.BrandSeoUrl;
                  dest.Brand.IsBrandActive = src.IsBrandActive;

                  //Product miscellaneous details
                  if (Equals(dest.MiscellaneousDetails, null)) dest.MiscellaneousDetails = new ProductMiscellaneousDetailsDTO();
                  dest.MiscellaneousDetails.CatalogName = src.CatalogName;
                  dest.MiscellaneousDetails.CategoryName = src.CategoryName;
                  dest.MiscellaneousDetails.ConfigurableProductId = src.ConfigurableProductId;
                  dest.MiscellaneousDetails.IsActive = src.IsActive;
                  dest.MiscellaneousDetails.IsConfigurableProduct = src.IsConfigurableProduct;
                  dest.MiscellaneousDetails.IsDefaultConfigurableProduct = src.IsDefaultConfigurableProduct;
                  dest.MiscellaneousDetails.IsPublish = src.IsPublish;
                  dest.MiscellaneousDetails.IsSimpleProduct = src.IsSimpleProduct;
                  dest.MiscellaneousDetails.ParentConfiguarableProductName = src.ParentConfiguarableProductName;
                  dest.MiscellaneousDetails.PimProductId = src.PimProductId;
                  dest.MiscellaneousDetails.PortalId = src.PortalId;
                  dest.MiscellaneousDetails.ProductName = src.ProductName;
                  dest.MiscellaneousDetails.ProductType = src.ProductType;
                  dest.MiscellaneousDetails.PublishStatus = src.PublishStatus;

                  //Customer Reviews
                  if (Equals(dest.CustomerReviews, null)) dest.CustomerReviews = new ProductReviewsDTO();
                  dest.CustomerReviews.ProductReviews = src.ProductReviews ?? new List<CustomerReviewModel>();
                  dest.CustomerReviews.Rating = src.Rating;
                  dest.CustomerReviews.TotalReviews = src.TotalReviews;

                  //Product associations.
                  if (Equals(dest.ProductAssociations, null)) dest.ProductAssociations = new ProductAssociationsDTO();
                  dest.ProductAssociations.AssociatedAddOnProducts = src.AssociatedAddOnProducts ?? new List<AssociatedProductModel>();
                  dest.ProductAssociations.AssociatedGroupProducts = src.AssociatedGroupProducts ?? new List<WebStoreGroupProductModel>();
                  dest.ProductAssociations.AssociatedProducts = src.AssociatedProducts ?? new List<AssociatedProductsModel>();
                  dest.ProductAssociations.GroupProductSKUs = src.GroupProductSKUs ?? new List<AssociatedProductModel>();
                  dest.ProductAssociations.BundleProductSKUs = src.BundleProductSKUs;
                  dest.ProductAssociations.AddonProductSKUs = src.AddonProductSKUs;
                  dest.ProductAssociations.ConfigurableProductSKUs = src.ConfigurableProductSKUs;
              });

            #endregion

            #region Impersonation
            Mapper.CreateMap<ImpersonationActivityLogModel, ZnodeImpersonationLog>();

            Mapper.CreateMap<ZnodeImpersonationLog, ImpersonationActivityLogModel>();
            #endregion

            #region Recommendation engine
            Mapper.CreateMap<RecommendationContext, RecommendationRequestModel>().ReverseMap();
            Mapper.CreateMap<RecommendationProcessingLogModel, ZnodeRecommendationProcessingLog>().ReverseMap();
            Mapper.CreateMap<RecommendedProductModel, ZnodeRecommendedProduct>().ReverseMap();

            Mapper.CreateMap<RecommendationBaseProductModel, ZnodeRecommendationBaseProduct>()
                .ForMember(d => d.ZnodeRecommendedProducts, opt => opt.MapFrom(src => src.RecommendedProducts));

            Mapper.CreateMap<ZnodeRecommendationBaseProduct, RecommendationBaseProductModel>()
               .ForMember(d => d.RecommendedProducts, opt => opt.MapFrom(src => src.ZnodeRecommendedProducts));
            #endregion

            Mapper.CreateMap<ZnodeSearchActivity, SearchReportModel>().ReverseMap();

            #region Quote mappers
            Mapper.CreateMap<ZnodeOmsQuote, QuoteResponseModel>()
            .ForMember(d => d.OmsQuoteStateId, opt => opt.MapFrom(src => src.OmsOrderStateId))
            .ForMember(d => d.TaxAmount, opt => opt.MapFrom(src => src.TaxCost))
            .ForMember(d => d.QuoteTotal, opt => opt.MapFrom(src => src.QuoteOrderTotal));

            Mapper.CreateMap<ZnodeOmsQuote, ShoppingCartModel>();

            Mapper.CreateMap<ZnodeOmsQuoteLineItem, ShoppingCartItemModel>()
                .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(d => d.CustomUnitPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(d => d.CartDescription, opt => opt.MapFrom(src => src.Description));

            Mapper.CreateMap<ZnodeOmsQuote, QuoteCreateModel>().ReverseMap();

            Mapper.CreateMap<ZnodeOmsQuote, QuoteResponseModel>()
                  .ForMember(d => d.OmsQuoteStateId, opt => opt.MapFrom(src => src.OmsOrderStateId))
                  .ForMember(d=>d.QuoteTotal, opt => opt.MapFrom(src => src.QuoteOrderTotal))
                  .ForMember(d => d.PublishCatalogId, opt => opt.MapFrom(src => src.PublishStateId)).ReverseMap();
            Mapper.CreateMap<ZnodeOmsQuoteLineItem, QuoteLineItemModel>()
                .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(d => d.Price, opt => opt.MapFrom(src => src.Price));

            Mapper.CreateMap<ZnodeOmsQuote, AddressModel>();
            Mapper.CreateMap<OrderHistoryModel, ZnodeOmsQuoteHistory>().ReverseMap();
            #endregion


            #region Publish Entity mappers

            //This mapping is to convert json product entity data into published product entity model
            Mapper.CreateMap<ZnodePublishProductEntity, PublishedProductEntityModel>()
             .ForMember(d => d.Brands, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<PublishedBrandEntityModel>>(src.Brands)))
             .ForMember(d => d.Attributes, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<PublishedAttributeEntityModel>>(src.Attributes)));

            //This mapping is to convert json product entity data into searchproduct model.
            Mapper.CreateMap<ZnodePublishProductEntity, SearchProduct>()
             .ForMember(d => d.brands, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<ElasticBrands>>(src.Brands)))
             .ForMember(d => d.attributes, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<SearchAttributes>>(src.Attributes)))
             .ForMember(d => d.categoryid, opt => opt.MapFrom(src => src.ZnodeCategoryIds))
             .ForMember(d => d.rawname, opt => opt.MapFrom(src => src.Name.ToLower()))
             .ForMember(d => d.rawsku, opt => opt.MapFrom(src => src.SKULower))
             .ForMember(d => d.version, opt => opt.MapFrom(src => src.VersionId))
             .ForMember(d => d.indexid, opt => opt.MapFrom(src => src.IndexId))
             .ForMember(d => d.catalogid, opt => opt.MapFrom(src => src.ZnodeCatalogId))
             .ForMember(d => d.parentcategoryids, opt => opt.MapFrom(src => HelperUtility.IsNotNull(src.ZnodeParentCategoryIds.Split(',').Select(int.Parse)) ? src.ZnodeParentCategoryIds.Split(',').Select(int.Parse).ToList() : null));

            //This mapping is to convert json category entity data into published category entity model.
            Mapper.CreateMap<ZnodePublishCategoryEntity, PublishedCategoryEntityModel>()
             .ForMember(d => d.Attributes, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<PublishedAttributeEntityModel>>(src.Attributes)))
             .ForMember(d => d.ZnodeParentCategoryIds, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ZnodeParentCategoryIds) ? null :
             JsonConvert.DeserializeObject<int[]>(src.ZnodeParentCategoryIds)))
             .ForMember(d => d.ProductIds, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ProductIds) ? null :
             JsonConvert.DeserializeObject<int[]>(src.ProductIds)));

            Mapper.CreateMap<ZnodePublishCategoryEntity, WebStoreCategoryModel>()
             .ForMember(d => d.Attributes, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<PublishAttributeModel>>(src.Attributes)))
             .ForMember(d => d.PublishCategoryId, opt => opt.MapFrom(src => src.ZnodeCategoryId))
             .ForMember(d => d.ZnodeParentCategoryIds, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ZnodeParentCategoryIds) ? null :
             JsonConvert.DeserializeObject<int[]>(src.ZnodeParentCategoryIds)))
             .ForMember(d => d.ProductIds, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ProductIds) ? null :
             JsonConvert.DeserializeObject<int[]>(src.ProductIds)));

            Mapper.CreateMap<ZnodePublishCategoryEntity, PublishCategoryModel>()
             .ForMember(d => d.Attributes, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<PublishAttributeModel>>(src.Attributes)))
             .ForMember(d => d.PublishCategoryId, opt => opt.MapFrom(src => src.ZnodeCategoryId))
             .ForMember(d => d.ZnodeParentCategoryIds, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ZnodeParentCategoryIds) ? null :
             JsonConvert.DeserializeObject<int[]>(src.ZnodeParentCategoryIds)))
             .ForMember(d => d.ProductIds, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ProductIds) ? null :
             JsonConvert.DeserializeObject<int[]>(src.ProductIds)));

            Mapper.CreateMap<ZnodePublishCatalogAttributeEntity, SearchAttributes>()
             .ForMember(d => d.selectvalues, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<ElasticSelectValues>>(src.SelectValues)));

            Mapper.CreateMap<ZnodePublishProductEntity, WebStoreAddOnValueModel>()
             .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ZnodeProductId))
             .ForMember(d => d.Attributes, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Attributes) ? null :
             JsonConvert.DeserializeObject<List<PublishAttributeModel>>(src.Attributes)));

            Mapper.CreateMap<ZnodePublishProductEntity, WebStoreBundleProductModel>()
              .ForMember(d => d.Attributes, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Attributes) ? null :
               JsonConvert.DeserializeObject<List<PublishAttributeModel>>(src.Attributes)));

            Mapper.CreateMap<ZnodePublishProductEntity, WebStoreGroupProductModel>()
               .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ZnodeProductId))
              .ForMember(d => d.Attributes, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Attributes) ? null :
              JsonConvert.DeserializeObject<List<PublishAttributeModel>>(src.Attributes)));

            Mapper.CreateMap<BrandModel, ZnodePublishPortalBrandEntity>().ReverseMap();

            //This mapping is to convert json configurable product entity data into published configurable product entity model.
            Mapper.CreateMap<ZnodePublishConfigurableProductEntity, PublishedConfigurableProductEntityModel>()
             .ForMember(d => d.SelectValues, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.SelectValues) ? null :
             JsonConvert.DeserializeObject<List<PublishedSelectValuesEntityModel>>(src.SelectValues)))
             .ForMember(d => d.ConfigurableAttributeCodes, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ConfigurableAttributeCodes) ? null :
             JsonConvert.DeserializeObject<List<string>>(src.ConfigurableAttributeCodes)));


            //This mapping is to convert json catalog attribute product entity data into published catalog attribute entity model.
            Mapper.CreateMap<ZnodePublishCatalogAttributeEntity, PublishedCatalogAttributeEntityModel>()
             .ForMember(d => d.SelectValues, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<PublishedSelectValuesEntityModel>>(src.SelectValues)));

            Mapper.CreateMap<ZnodePublishProductEntity, PublishProductModelV2>()
              .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ZnodeProductId))
              .ForMember(d => d.Attributes, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Attributes) ? null :
              JsonConvert.DeserializeObject<List<PublishAttributeModel>>(src.Attributes)));

            Mapper.CreateMap<ZnodePublishContentPageConfigEntity, WebStoreContentPageModel>()
             .ForMember(d => d.ProfileId, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ProfileId) ? null :
              JsonConvert.DeserializeObject<int[]>(src.ProfileId)));

            Mapper.CreateMap<ZnodePublishCatalogAttributeEntity, PublishAttributeModel>()
               .ForMember(d => d.SelectValues, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.SelectValues) ? null : JsonConvert.DeserializeObject<List<AttributesSelectValuesModel>>(src.SelectValues)));

            Mapper.CreateMap<ZnodePublishCatalogEntity, PublishCatalogModel>()
              .ForMember(d => d.PublishCatalogId, opt => opt.MapFrom(src => src.ZnodeCatalogId));

            Mapper.CreateMap<SearchAttributes, ZnodePublishCatalogAttributeEntity>();

            Mapper.CreateMap<ZnodePublishCatalogAttributeEntity, SearchAttributesModel>().ReverseMap();

            Mapper.CreateMap<ZnodePublishWebstoreEntity, PortalModel>()
             .ForMember(d => d.StoreName, opt => opt.MapFrom(src => src.WebsiteTitle));

            Mapper.CreateMap<ZnodePublishWidgetCategoryEntity, WebStoreWidgetCategoryModel>();

            Mapper.CreateMap<ZnodePublishProductEntity, PublishProductModel>()
             .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ZnodeProductId))
             .ForMember(d => d.Attributes, opt => opt.MapFrom(src =>
             JsonConvert.DeserializeObject<List<PublishAttributeModel>>(src.Attributes)));

            Mapper.CreateMap<ZnodePublishProductEntity, WebStoreAddOnValueModel>()
             .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ZnodeProductId))
             .ForMember(d => d.Attributes, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Attributes) ? null :
             JsonConvert.DeserializeObject<List<PublishAttributeModel>>(src.Attributes)));

            Mapper.CreateMap<ZnodePublishProductEntity, ProductDetailsModel>()
             .ForMember(d => d.ProductName, opt => opt.MapFrom(src => src.Name))
             .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ZnodeProductId));

            Mapper.CreateMap<ZnodePublishProductEntity, WebStoreProductModel>()
             .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ZnodeProductId))
             .ForMember(d => d.Attributes, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Attributes) ? null :
             JsonConvert.DeserializeObject<List<PublishAttributeModel>>(src.Attributes)));

            Mapper.CreateMap<ZnodePublishCategoryEntity, CategoryModel>()
              .ForMember(d => d.PublishCategoryId, opt => opt.MapFrom(src => src.ZnodeCategoryId))
              .ForMember(d => d.CategoryName, opt => opt.MapFrom(src => src.Name));

            Mapper.CreateMap<ZnodePublishCategoryEntity, PublishCategoryModel>()
             .ForMember(d => d.PublishCategoryId, opt => opt.MapFrom(src => src.ZnodeCategoryId));

            Mapper.CreateMap<SEODetailsModel, ZnodePublishSeoEntity>().ReverseMap();

            Mapper.CreateMap<ZnodePublishProductEntity, AssociatedPublishedBundleProductModel>()
             .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ZnodeProductId))
             .ForMember(d => d.Attributes, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Attributes) ? null :
             JsonConvert.DeserializeObject<List<PublishAttributeModel>>(src.Attributes)));

            Mapper.CreateMap<ZnodePublishProductEntity, WebStoreConfigurableProductModel>()
            .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ZnodeProductId));

            Mapper.CreateMap<ZnodePublishSearchProfileEntity, PublishSearchProfileModel>().ReverseMap();
            #endregion 

            #region Account Attribute

            Mapper.CreateMap<EntityAttributeModel, GlobalAttributeEntityDetailsModel>().ReverseMap();

            Mapper.CreateMap<EntityAttributeDetailsModel, GlobalAttributeValuesModel>().ReverseMap()
            .ForMember(d => d.AttributeValue, opt => opt.MapFrom(src => src.AttributeDefaultValue))
            .ForMember(d => d.GlobalAttributeValueId, opt => opt.MapFrom(src => HelperUtility.IsNotNull(src.GlobalAttributeValueId) ? src.GlobalAttributeValueId : 0))
            .ForMember(d => d.GlobalAttributeDefaultValueId, opt => opt.MapFrom(src => HelperUtility.IsNotNull(src.GlobalAttributeDefaultValueId) ? src.GlobalAttributeDefaultValueId : 0));

            #endregion


            Mapper.CreateMap<ZnodePublishCategoryEntity, SiteMapCategoryModel>()
            .ForMember(d => d.ZnodeParentCategoryIds, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ZnodeParentCategoryIds) ? null :
            JsonConvert.DeserializeObject<int[]>(src.ZnodeParentCategoryIds)));

            #region TradeCentric mapper
            Mapper.CreateMap<TradeCentricUserModel, ZnodeTradeCentricUser>().ReverseMap();
            #endregion

        }
    }
}
