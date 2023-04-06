using Autofac;

using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

using Znode.Admin.Core.Areas.Search.ViewModels;
using Znode.Admin.Core.ViewModels;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Engine.klaviyo.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin
{
    public static class AutoMapperConfig
    {
        public static void Execute()
        {
            Mapper.CreateMap<UserModel, UserViewModel>()
                .ForMember(d => d.EmailAddress, opt => opt.MapFrom(src => Equals(src.User, null) ? string.Empty : src.User.Email))
                .ForMember(d => d.UserName, opt => opt.MapFrom(src => Equals(src.User, null) ? src.UserName : src.User.Username))
                .ForMember(d => d.AspNetUserId, opt => opt.MapFrom(src => Equals(src.User, null) ? src.AspNetUserId : src.User.UserId))
                .ForMember(d => d.RoleId, opt => opt.MapFrom(src => Equals(src.User, null) ? string.Empty : src.User.RoleId))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateTimeFormat()));
               
             Mapper.CreateMap<UserViewModel, UserModel>();

            Mapper.CreateMap<UserListModel, UsersListViewModel>()
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<LoginViewModel, LoginUserModel>();

            Mapper.CreateMap<LoginViewModel, UserModel>()
               .ForMember(d => d.User, opt => opt.MapFrom(s => Mapper.Map<LoginViewModel, LoginUserModel>(s)));

            Mapper.CreateMap<ProductDetailsModel, ProductDetailsViewModel>()
                .ForMember(d => d.ItemId, opt => opt.MapFrom(src => src.PublishProductId))
                .ForMember(d => d.ItemName, opt => opt.MapFrom(src => src.ProductName));

            Mapper.CreateMap<ProductDetailsViewModel, ProductDetailsModel>();
            
            Mapper.CreateMap<ProductDetailsListViewModel, ProductDetailsListModel>();

            Mapper.CreateMap<ProductDetailsListModel, ProductDetailsListViewModel>();

            Mapper.CreateMap<AttributeGroupModel, AttributeGroupViewModel>();

            Mapper.CreateMap<AttributeGroupViewModel, AttributeGroupModel>()
              .ForMember(d => d.IsHidden, opt => opt.MapFrom(src => false));

            Mapper.CreateMap<AttributeGroupMapperModel, AttributeGroupMapperViewModel>();

            Mapper.CreateMap<AttributeGroupMapperViewModel, AttributeGroupMapperModel>();

            Mapper.CreateMap<AttributeFamilyViewModel, AttributeFamilyModel>();

            Mapper.CreateMap<AttributeFamilyModel, AttributeFamilyViewModel>();

            Mapper.CreateMap<LinkProductDetailModel, LinkProductDetailViewModel>();

            Mapper.CreateMap<LinkProductDetailViewModel, LinkProductDetailModel>();

            Mapper.CreateMap<PIMProductAttributeValuesModel, PIMProductAttributeValuesViewModel>();

            Mapper.CreateMap<PIMAttributeGroupModel, PIMAttributeGroupViewModel>();

            Mapper.CreateMap<PIMAttributeGroupViewModel, PIMAttributeGroupModel>();

            Mapper.CreateMap<PIMAttributeModel, PIMAttributeViewModel>();

            Mapper.CreateMap<PIMAttributeValueListModel, PIMAttributeValueListViewModel>();

            Mapper.CreateMap<PIMAttributeValueModel, PIMAttributeValueViewModel>();

            Mapper.CreateMap<CatalogViewModel, CatalogModel>()
                .ForMember(d => d.CatalogName, opt => opt.MapFrom(src => src.CatalogName.Trim()));

            Mapper.CreateMap<CatalogModel, CatalogViewModel>()
              .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat()))
              .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateTimeFormat()))
              .ForMember(d => d.PublishCreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.PublishCreatedDate).ToTimeZoneDateTimeFormat()));

           Mapper.CreateMap<ProductTypeAssociationModel, ProductTypeAssociationViewModel>();

            Mapper.CreateMap<ProductTypeAssociationViewModel, ProductTypeAssociationModel>();

            Mapper.CreateMap<ProductTypeAssociationListModel, ProductTypeAssociationListViewModel>();

            Mapper.CreateMap<ProductTypeAssociationListViewModel, ProductTypeAssociationListModel>();

            Mapper.CreateMap<CatalogAssociationViewModel, CatalogAssociationModel>();

            Mapper.CreateMap<PIMAttributeFamilyViewModel, PIMAttributeFamilyModel>();

            Mapper.CreateMap<PIMAttributeFamilyModel, PIMAttributeFamilyViewModel>();

            Mapper.CreateMap<AttributesDataModel, AttributesViewModel>();

            Mapper.CreateMap<AttributesViewModel, AttributesDataModel>();

            Mapper.CreateMap<CustomFieldViewModel, CustomFieldModel>();

            Mapper.CreateMap<CustomFieldModel, CustomFieldViewModel>();

            Mapper.CreateMap<LocaleModel, LocaleViewModel>()
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<AttributeTypeDataModel, AttributeTypeModel>();

            Mapper.CreateMap<CategoryModel, CategoryViewModel>()
              .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()))
              .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat()))
                .ForMember(d => d.ItemId, opt => opt.MapFrom(src => src.PublishCategoryId))
                .ForMember(d => d.ItemName, opt => opt.MapFrom(src => src.CategoryName));

            Mapper.CreateMap<GlobalMediaDisplaySettingModel, GlobalMediaDisplaySettingViewModel>().ReverseMap();
            Mapper.CreateMap<GlobalSettingViewModel, GeneralSettingModel>();

            Mapper.CreateMap<CategoryTreeModel, CategoryTreeViewModel>();

            Mapper.CreateMap<ProductAttributeModel, ProductAttributeViewModel>();
            Mapper.CreateMap<ProductAttributeViewModel, ProductAttributeModel>();

            Mapper.CreateMap<ProductViewModel, ProductModel>();
            Mapper.CreateMap<ProductModel, ProductViewModel>();

            Mapper.CreateMap<PriceViewModel, PriceModel>()
                 .ForMember(d => d.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate.HasValue ? src.ExpirationDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : src.ExpirationDate));

            Mapper.CreateMap<PriceModel, PriceViewModel>()
                .ForMember(d => d.ActivationDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ActivationDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ExpirationDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ExpirationDate).ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<UserModel, AccountViewModel>();

            Mapper.CreateMap<UserModel, CustomerViewModel>()
                 .ForMember(d => d.BudgetAmount, opt => opt.MapFrom(src => !Equals(src.BudgetAmount, null) ? src.BudgetAmount.ToPriceRoundOff() : null));

            Mapper.CreateMap<AccountViewModel, UserModel>();
            Mapper.CreateMap<PriceSKUViewModel, PriceSKUModel>()
                 .ForMember(d => d.RetailPrice, opt => opt.MapFrom(src => src.RetailPrice.ToPriceRoundOff()))
                 .ForMember(d => d.SalesPrice, opt => opt.MapFrom(src => src.SalesPrice.ToPriceRoundOff()))
                 .ForMember(d => d.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate.HasValue ? src.ExpirationDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : src.ExpirationDate));

            Mapper.CreateMap<PriceSKUModel, PriceSKUViewModel>()
                 .ForMember(d => d.RetailPrice, opt => opt.MapFrom(src => src.RetailPrice.ToPriceRoundOff()))
                 .ForMember(d => d.SalesPrice, opt => opt.MapFrom(src => src.SalesPrice.ToPriceRoundOff()));

            Mapper.CreateMap<AddressModel, AddressViewModel>()
                 .ForMember(d => d.DisplayAddress, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName},"+(string.IsNullOrEmpty(src.CompanyName) ? "" : $"{src.CompanyName},")+ $"{src.Address1},"+ (string.IsNullOrEmpty(src.Address2) ? "" : $" {src.Address2},") + (string.IsNullOrEmpty(src.Address3) ? "" : $"{src.Address3} ,") + $" {src.CityName}, {src.StateName}, {src.PostalCode}, {src.CountryName}, PH NO. {src.PhoneNumber}"));
            Mapper.CreateMap<AddressViewModel, AddressModel>();

            Mapper.CreateMap<AccountModel, AccountViewModel>()
                 .ForMember(d => d.Address, opt => opt.MapFrom(s => Mapper.Map<AddressModel, AddressViewModel>(s.Address)));

            Mapper.CreateMap<AccountViewModel, AccountModel>()
              .ForMember(d => d.Name, opt => opt.MapFrom(s => string.IsNullOrEmpty(s.Name) ? string.Empty : s.Name.Trim()))
              .ForMember(d => d.ParentAccountId, opt => opt.MapFrom(s => s.ParentAccountId > 0 ? s.ParentAccountId : (int?)null))
              .ForMember(d => d.Address, opt => opt.MapFrom(s => Mapper.Map<AddressViewModel, AddressModel>(s.Address)));


            Mapper.CreateMap<PriceTierModel, PriceTierViewModel>()
                 .ForMember(d => d.Price, opt => opt.MapFrom(src => src.Price.ToPriceRoundOff()))
              .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()));

            Mapper.CreateMap<PriceTierViewModel, PriceTierModel>()
                  .ForMember(d => d.Price, opt => opt.MapFrom(src => src.Price.ToPriceRoundOff()))
                  .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()));

            Mapper.CreateMap<PriceTierModel, TierPriceViewModel>();

            Mapper.CreateMap<AccountDataModel, AccountDataViewModel>()
                  .ForMember(d => d.CompanyAccount, opt => opt.MapFrom(s => Mapper.Map<AccountModel, AccountViewModel>(s.CompanyAccount)));


            Mapper.CreateMap<AccountDataViewModel, AccountDataModel>()
                .ForMember(d => d.CompanyAccount, opt => opt.MapFrom(s => Mapper.Map<AccountViewModel, AccountModel>(s.CompanyAccount)));

            Mapper.CreateMap<InventoryViewModel, InventoryModel>();

            Mapper.CreateMap<InventoryModel, InventoryViewModel>();

            Mapper.CreateMap<InventorySKUModel, InventorySKUViewModel>()
                .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()))
                .ForMember(d => d.ReOrderLevel, opt => opt.MapFrom(src => src.ReOrderLevel.ToInventoryRoundOff()))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateTimeFormat()));
                
                Mapper.CreateMap<InventorySKUViewModel, InventorySKUModel>()
                .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()))
                .ForMember(d => d.ReOrderLevel, opt => opt.MapFrom(src => src.ReOrderLevel.ToInventoryRoundOff()));


            Mapper.CreateMap<PricePortalViewModel, PricePortalModel>();
            Mapper.CreateMap<PricePortalModel, PricePortalViewModel>()
              .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()))
              .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat()));

            Mapper.CreateMap<StoreViewModel, PortalModel>()
                 .ForMember(d => d.ReviewStatus, opt => opt.MapFrom(src => src.ReviewStatusId));
            Mapper.CreateMap<PortalModel, StoreViewModel>()
                .ForMember(d => d.ReviewStatusId, opt => opt.MapFrom(src => src.ReviewStatus));

            Mapper.CreateMap<PriceProfileViewModel, PriceProfileModel>();
            Mapper.CreateMap<PriceProfileModel, PriceProfileViewModel>()
              .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()))
              .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat()));

            Mapper.CreateMap<ProfileViewModel, ProfileModel>();
            Mapper.CreateMap<ProfileModel, ProfileViewModel>();

            Mapper.CreateMap<NoteModel, NoteViewModel>()
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateFormat()))
            .ForMember(d => d.NoteDateTime, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateFormat()))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateFormat())).ReverseMap();


            Mapper.CreateMap<AccountDepartmentViewModel, AccountDepartmentModel>()
                .ForMember(d => d.DepartmentName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.DepartmentName) ? string.Empty : src.DepartmentName.Trim()));
            Mapper.CreateMap<AccountDepartmentModel, AccountDepartmentViewModel>();

            Mapper.CreateMap<PriceUserViewModel, PriceUserModel>().ReverseMap();

            Mapper.CreateMap<EmailTemplateAreaMapperViewModel, EmailTemplateAreaMapperModel>().ReverseMap();


            Mapper.CreateMap<PriceAccountViewModel, PriceAccountModel>().ReverseMap();

            Mapper.CreateMap<WarehouseViewModel, WarehouseModel>();

            Mapper.CreateMap<WarehouseModel, WarehouseViewModel>()
              .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()))
              .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat()));

            Mapper.CreateMap<ImportPriceModel, ImportPriceViewModel>()
                .ForMember(d => d.SequenceNumber, opt => opt.MapFrom(src => src.RowNumber));

            Mapper.CreateMap<ImportPriceViewModel, ImportPriceModel>();

            Mapper.CreateMap<PIMProductAttributeValuesListModel, PIMProductAttributeValuesListViewModel>();

            Mapper.CreateMap<PIMProductAttributeValuesListViewModel, PIMProductAttributeValuesListModel>();

            Mapper.CreateMap<CategoryProductModel, CategoryProductViewModel>();
            Mapper.CreateMap<CategoryProductViewModel, CategoryProductModel>();

            Mapper.CreateMap<InventoryWarehouseMapperViewModel, InventoryWarehouseMapperModel>();

            Mapper.CreateMap<InventoryWarehouseMapperModel, InventoryWarehouseMapperViewModel>()
              .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()))
              .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat()))
              .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()))
              .ForMember(d => d.ReOrderLevel, opt => opt.MapFrom(src => src.ReOrderLevel.ToInventoryRoundOff()));

            Mapper.CreateMap<InventoryWarehouseMapperViewModel, InventoryWarehouseMapperModel>();

            Mapper.CreateMap<InventoryWarehouseMapperListModel, InventoryWarehouseMapperListViewModel>();

            Mapper.CreateMap<InventoryWarehouseMapperListViewModel, InventoryWarehouseMapperListModel>();

            Mapper.CreateMap<PortalWarehouseModel, PortalWarehouseViewModel>();

            Mapper.CreateMap<PortalWarehouseViewModel, PortalWarehouseModel>()
             .ForMember(d => d.WarehouseCode, opt => opt.MapFrom(src => src.WarehouseCode));


            Mapper.CreateMap<PortalAlternateWarehouseModel, PortalAlternateWarehouseViewModel>()
                 .ForMember(d => d.WarehouseCode, opt => opt.MapFrom(src => src.WarehouseCode));

            Mapper.CreateMap<PortalAlternateWarehouseViewModel, PortalAlternateWarehouseModel>();

            Mapper.CreateMap<CategoryValuesListModel, CategoryViewModel>();

            Mapper.CreateMap<ExportPriceModel, ExportPriceViewModel>()
                .ForMember(d => d.TierStartQuantity, opt => opt.MapFrom(src => src.TierStartQuantity.ToInventoryRoundOff()));

            Mapper.CreateMap<ShippingModel, ShippingViewModel>()
              .ForMember(d => d.HandlingCharge, opt => opt.MapFrom(src => src.HandlingCharge.ToPriceRoundOff()));

            Mapper.CreateMap<ShippingViewModel, ShippingModel>()
                   .ForMember(d => d.HandlingCharge, opt => opt.MapFrom(src => src.HandlingCharge.ToPriceRoundOff()));

            Mapper.CreateMap<OrderShippingModel, ShippingViewModel>()
              .ForMember(d => d.HandlingCharge, opt => opt.MapFrom(src => src.ShippingHandlingCharge.ToPriceRoundOff()))
              .ForMember(d => d.Description, opt => opt.MapFrom(src => src.ShippingDiscountDescription))
              .ForMember(d => d.DestinationCountryCode, opt => opt.MapFrom(src => src.ShippingCountryCode));


            Mapper.CreateMap<ShippingViewModel, OrderShippingModel>()
                   .ForMember(d => d.ShippingHandlingCharge, opt => opt.MapFrom(src => src.HandlingCharge.ToPriceRoundOff()))
                   .ForMember(d => d.ShippingDiscountDescription, opt => opt.MapFrom(src => src.Description))
                   .ForMember(d => d.ShippingCountryCode, opt => opt.MapFrom(src => src.DestinationCountryCode));

            Mapper.CreateMap<ShippingSKUModel, ShippingSKUViewModel>();

            Mapper.CreateMap<ShippingSKUViewModel, ShippingSKUModel>();

            Mapper.CreateMap<PortalCatalogModel, PortalCatalogViewModel>();

            Mapper.CreateMap<PortalCatalogViewModel, PortalCatalogModel>();

            Mapper.CreateMap<PublishCatalogModel, PublishCatalogViewModel>();
             
          Mapper.CreateMap<InventorySKUModel, ExportInventoryViewModel>();

            Mapper.CreateMap<ImportInventoryModel, ImportInventoryViewModel>();

            Mapper.CreateMap<ImportInventoryViewModel, ImportInventoryModel>();

            Mapper.CreateMap<TaxClassModel, TaxClassViewModel>();

            Mapper.CreateMap<TaxClassViewModel, TaxClassModel>();

            Mapper.CreateMap<TaxClassSKUModel, TaxClassSKUViewModel>();

            Mapper.CreateMap<TaxClassSKUViewModel, TaxClassSKUModel>();

            Mapper.CreateMap<AddonGroupModel, AddonGroupViewModel>();

            Mapper.CreateMap<AddonGroupViewModel, AddonGroupModel>();

            Mapper.CreateMap<AddonGroupLocaleListModel, AddonGroupLocaleListViewModel>();
            Mapper.CreateMap<AccountPermissionViewModel, AccessPermissionModel>()
                .ForMember(d => d.AccountPermissionName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.AccountPermissionName) ? string.Empty : src.AccountPermissionName.Trim()));
            Mapper.CreateMap<AccessPermissionModel, AccountPermissionViewModel>();

            Mapper.CreateMap<PermissionsModel, PermissionsViewModel>();

            Mapper.CreateMap<TaxRuleModel, TaxRuleViewModel>();

            Mapper.CreateMap<TaxRuleViewModel, TaxRuleModel>();

            Mapper.CreateMap<ThemeViewModel, ThemeModel>();

            Mapper.CreateMap<ThemeModel, ThemeViewModel>()
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<SliderViewModel, SliderModel>();

            Mapper.CreateMap<SliderModel, SliderViewModel>();

            Mapper.CreateMap<CustomerReviewViewModel, CustomerReviewModel>();

            Mapper.CreateMap<CustomerReviewModel, CustomerReviewViewModel>()
                   .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat()))
                   .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<WebSiteLogoViewModel, WebSiteLogoModel>();
            Mapper.CreateMap<WebSiteLogoModel, WebSiteLogoViewModel>();

            Mapper.CreateMap<DynamicContentModel, DynamicContentViewModel>().ReverseMap();

            Mapper.CreateMap<EditorFormatViewModel, EditorFormatModel>().ReverseMap();

            Mapper.CreateMap<EditorFormatListViewModel, EditorFormatListModel>().ReverseMap();

            Mapper.CreateMap<WebSiteThemeWidgetModel, WebSiteThemeWidgetViewModel>();

            Mapper.CreateMap<ThemeAssetModel, ThemeAssetViewModel>();

            Mapper.CreateMap<ThemeAssetViewModel, ThemeAssetModel>();
            Mapper.CreateMap<AddonGroupModel, AddonGroupViewModel>();
            Mapper.CreateMap<AddonGroupViewModel, AddonGroupModel>();

            Mapper.CreateMap<AddonGroupLocaleListViewModel, AddonGroupLocaleListModel>();
            Mapper.CreateMap<AddonGroupLocaleListModel, AddonGroupLocaleListViewModel>();

            Mapper.CreateMap<LinkWidgetConfigurationModel, LinkWidgetConfigurationViewModel>().ReverseMap();

            Mapper.CreateMap<LinkWidgetDataViewModel, LinkWidgetConfigurationModel>().ReverseMap();

            Mapper.CreateMap<ContentPageViewModel, ContentPageModel>()
                .ForMember(d => d.ProfileIds, opt => opt.MapFrom(src => src.IsSelectAllProfile ? null : string.Join(",", src.ProfileIds)));

            Mapper.CreateMap<ContentPageModel, ContentPageViewModel>()
                .ForMember(d => d.ItemId, opt => opt.MapFrom(src => src.CMSContentPagesId))
                .ForMember(d => d.ProfileIds, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.ProfileIds) ? src.ProfileIds.Split(',') : null))
                .ForMember(d => d.ItemName, opt => opt.MapFrom(src => src.PageName))
                .ForMember(d => d.IsSelectAllProfile, opt => opt.MapFrom(src => (string.IsNullOrEmpty(src.ProfileIds)) ? true : false));

            Mapper.CreateMap<StateViewModel, StateModel>();

            Mapper.CreateMap<StateModel, StateViewModel>();

            Mapper.CreateMap<CityViewModel, CityModel>();

            Mapper.CreateMap<CityModel, CityViewModel>();

            Mapper.CreateMap<AddonGroupViewModel, AddonGroupModel>();
            Mapper.CreateMap<AddonGroupModel, AddonGroupViewModel>();

            Mapper.CreateMap<AddonGroupLocaleViewModel, AddonGroupLocaleModel>();
            Mapper.CreateMap<AddonGroupLocaleModel, AddonGroupLocaleViewModel>();

            Mapper.CreateMap<AddOnProductViewModel, AddOnProductModel>();
            Mapper.CreateMap<AddonGroupLocaleModel, AddOnProductViewModel>();

            Mapper.CreateMap<CustomerAccountViewModel, UserModel>();
            Mapper.CreateMap<UserModel, CustomerAccountViewModel>()
                .ForMember(d => d.BudgetAmount, opt => opt.MapFrom(src => !Equals(src.BudgetAmount, null) ? src.BudgetAmount.ToPriceRoundOff() : null));

            Mapper.CreateMap<CMSWidgetConfigurationViewModel, CMSWidgetConfigurationModel>();

            Mapper.CreateMap<CMSWidgetConfigurationModel, CMSWidgetConfigurationViewModel>();

            Mapper.CreateMap<BannerViewModel, BannerModel>();

            Mapper.CreateMap<BannerModel, BannerViewModel>();

            Mapper.CreateMap<CMSAreaWidgetsDataModel, CMSAreaWidgetsDataViewModel>();

            Mapper.CreateMap<CMSWidgetsModel, CMSWidgetsViewModel>();
            Mapper.CreateMap<CMSWidgetsViewModel, CMSWidgetsModel>();

            Mapper.CreateMap<ShippingRuleModel, ShippingRuleViewModel>()
                .ForMember(d => d.BaseCost, opt => opt.MapFrom(src => src.BaseCost.ToPriceRoundOff()))
            .ForMember(d => d.PerItemCost, opt => opt.MapFrom(src => src.PerItemCost.ToPriceRoundOff()))
             .ForMember(d => d.LowerLimit, opt => opt.MapFrom(src => src.LowerLimit.ToInventoryRoundOff()))
             .ForMember(d => d.UpperLimit, opt => opt.MapFrom(src => src.UpperLimit.ToInventoryRoundOff()));

            Mapper.CreateMap<ShippingRuleViewModel, ShippingRuleModel>()
                .ForMember(d => d.BaseCost, opt => opt.MapFrom(src => src.BaseCost.ToPriceRoundOff()))
            .ForMember(d => d.PerItemCost, opt => opt.MapFrom(src => src.PerItemCost.ToPriceRoundOff()))
             .ForMember(d => d.LowerLimit, opt => opt.MapFrom(src => src.LowerLimit.ToInventoryRoundOff()))
             .ForMember(d => d.UpperLimit, opt => opt.MapFrom(src => src.UpperLimit.ToInventoryRoundOff()));

            Mapper.CreateMap<ManageMessageViewModel, ManageMessageModel>();

            Mapper.CreateMap<ManageMessageModel, ManageMessageViewModel>();
            Mapper.CreateMap<ManageMessageModel, ManageMessageViewModel>();

            Mapper.CreateMap<ThemeViewModel, ThemeModel>();

            Mapper.CreateMap<PortalProductPageModel, PortalProductPageViewModel>().ReverseMap();

            Mapper.CreateMap<PortalSEOSettingModel, PortalSEOSettingViewModel>();

            Mapper.CreateMap<PortalSEOSettingViewModel, PortalSEOSettingModel>();

            Mapper.CreateMap<TaxClassPortalModel, TaxClassPortalViewModel>();

            Mapper.CreateMap<TaxClassPortalViewModel, TaxClassPortalModel>();

            Mapper.CreateMap<TaxClassPortalListViewModel, TaxClassPortalListModel>();

            Mapper.CreateMap<ShippingPortalModel, ShippingPortalViewModel>();

            Mapper.CreateMap<ShippingPortalViewModel, ShippingPortalModel>();

            Mapper.CreateMap<ShippingPortalListViewModel, ShippingPortalListModel>();

            Mapper.CreateMap<CMSWidgetProductModel, CMSWidgetProductViewModel>();
            Mapper.CreateMap<CMSWidgetProductCategoryModel, CMSWidgetProductViewModel>().ReverseMap();
            Mapper.CreateMap<CMSWidgetProductCategoryModel, CategoryViewModel>().ReverseMap();

            Mapper.CreateMap<ProductDetailsModel, ProductDetailsViewModel>();

            Mapper.CreateMap<UrlRedirectViewModel, UrlRedirectModel>();
            Mapper.CreateMap<UrlRedirectModel, UrlRedirectViewModel>();

            Mapper.CreateMap<CategoryListModel, CategoryListViewModel>();

            Mapper.CreateMap<SEODetailsModel, SEODetailsViewModel>();
            Mapper.CreateMap<SEODetailsViewModel, SEODetailsModel>();

            Mapper.CreateMap<PromotionModel, PromotionViewModel>()
                .ForMember(p => p.Discount, opt => opt.MapFrom(src => src.Discount.ToPriceRoundOff()))
                .ForMember(p => p.OrderMinimum, opt => opt.MapFrom(src => src.OrderMinimum.ToPriceRoundOff()))
                .ForMember(p => p.QuantityMinimum, opt => opt.MapFrom(src => src.QuantityMinimum.ToInventoryRoundOff()))
                .ForMember(p => p.StartDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.StartDate).ToTimeZoneDateTimeFormat()))
                .ForMember(p => p.EndDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.EndDate).ToTimeZoneDateTimeFormat()));
                
                Mapper.CreateMap<PromotionViewModel, PromotionModel>()
                .ForMember(p => p.Discount, opt => opt.MapFrom(src => src.Discount.GetValueOrDefault().ToPriceRoundOff()))
                .ForMember(p => p.OrderMinimum, opt => opt.MapFrom(src => src.OrderMinimum.GetValueOrDefault().ToPriceRoundOff()))
                .ForMember(p => p.QuantityMinimum, opt => opt.MapFrom(src => src.QuantityMinimum.GetValueOrDefault().ToInventoryRoundOff()));

            Mapper.CreateMap<CouponViewModel, CouponModel>();
            Mapper.CreateMap<CouponModel, CouponViewModel>();

            Mapper.CreateMap<CouponListViewModel, CouponListModel>().ReverseMap();

            Mapper.CreateMap<ProductPromotionViewModel, ProductPromotionModel>().ReverseMap();

            Mapper.CreateMap<StoreLocatorDataViewModel, StoreLocatorDataModel>();
            Mapper.CreateMap<StoreLocatorDataModel, StoreLocatorDataViewModel>();

            Mapper.CreateMap<CatalogAssociateCategoryViewModel, CatalogAssociateCategoryModel>();
            Mapper.CreateMap<CatalogAssociateCategoryModel, CatalogAssociateCategoryViewModel>();

            Mapper.CreateMap<CustomerViewModel, UserModel>();
            Mapper.CreateMap<UserModel, CustomerViewModel>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(src => Equals(src.User, null) ? src.UserName : src.User.Username))
                .ForMember(dest => dest.IsSelectAllPortal, opt => opt.MapFrom(src => (!Equals(src.PortalIds, null) && src.PortalIds.Count() > 0) ? src.PortalIds.Contains("0") : false))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<MenuModel, MenuViewModel>();
            Mapper.CreateMap<MenuViewModel, MenuModel>();

            Mapper.CreateMap<AddOnProductModel, AddOnProductViewModel>();
            Mapper.CreateMap<AddOnProductViewModel, AddOnProductModel>();

            Mapper.CreateMap<UserPortalModel, UserPortalViewModel>()
                .ForMember(d => d.IsSelectAllPortal, opt => opt.MapFrom(src => (!Equals(src.PortalIds, null) && src.PortalIds.Count() > 0) ? src.PortalIds.Contains("0") : false))
                .ForMember(d => d.PortalIds, opt => opt.MapFrom(src => (!Equals(src.PortalIds, null) && src.PortalIds.Count() > 0 && src.PortalIds.Contains("0")) ? new string[] { } : src.PortalIds));
            Mapper.CreateMap<UserPortalViewModel, UserPortalModel>()
                 .ForMember(dest => dest.PortalIds, opt => opt.MapFrom(src => src.IsSelectAllPortal ? new string[] { "0" } : src.PortalIds.Where(x => !string.IsNullOrEmpty(x)).ToArray()));

            Mapper.CreateMap<DomainModel, DomainViewModel>();
            Mapper.CreateMap<DomainViewModel, DomainModel>();

            Mapper.CreateMap<Api.Models.ActionModel, ActionViewModel>();
            Mapper.CreateMap<ActionViewModel, Api.Models.ActionModel>();

            Mapper.CreateMap<ActionPermissionMapperModel, ActionPermissionMapperViewModel>();
            Mapper.CreateMap<ActionPermissionMapperViewModel, ActionPermissionMapperModel>();

            Mapper.CreateMap<MenuActionsPermissionViewModel, MenuActionsPermissionModel>();
            Mapper.CreateMap<MenuActionsPermissionModel, MenuActionsPermissionViewModel>();

            Mapper.CreateMap<UsersViewModel, UserModel>()
                .ForMember(d => d.User, opt => opt.MapFrom(src => new LoginUserModel() { Email = src.UserName, UserId = src.AspNetUserId, Username = src.UserName }))
                .ForMember(d => d.PortalIds, opt => opt.MapFrom(src => src.IsSelectAllPortal ? new string[] { "0" } : src.PortalIds));

            Mapper.CreateMap<UserModel, UsersViewModel>()
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.UserName, opt => opt.MapFrom(src => Equals(src.User, null) ? src.UserName : src.User.Username))
                .ForMember(d => d.IsSelectAllPortal, opt => opt.MapFrom(src => Equals(src.PortalIds, null)))
                .AfterMap((src, dest) => dest.PortalIds = !Equals(src.PortalIds, null) ? src.PortalIds : null);

            Mapper.CreateMap<GiftCardViewModel, GiftCardModel>()
               .ForMember(d => d.Amount, opt => opt.MapFrom(src => src.GiftCardAmount));
               
          Mapper.CreateMap<GiftCardModel, GiftCardViewModel>()
            .ForMember(d => d.GiftCardAmount, opt => opt.MapFrom(src => src.Amount.ToPriceRoundOff()))
            .ForMember(d => d.Amount, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.Amount.GetValueOrDefault(), src.CultureCode, string.Empty)))
            .ForMember(d => d.RemainingAmount, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(Math.Round(src.RemainingAmount.GetValueOrDefault(), Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff)), src.CultureCode, string.Empty)))
            .ForMember(d => d.OwedAmount, opt => opt.MapFrom(src => Math.Round(src.OwedAmount.GetValueOrDefault(), Convert.ToInt32(DefaultSettingHelper.DefaultPriceRoundOff))))
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()))
            .ForMember(d => d.ExpirationDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ExpirationDate).ToTimeZoneDateTimeFormat()))
            .ForMember(d => d.StartDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.StartDate).ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<VoucherHistoryViewModel, GiftCardHistoryModel>();

            Mapper.CreateMap<GiftCardHistoryModel, VoucherHistoryViewModel>()
                 .ForMember(d => d.TransactionAmount, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.TransactionAmount, src.CultureCode, string.Empty)));

            Mapper.CreateMap<VoucherModel, VouchersViewModel>()
               .ForMember(d => d.VoucherAmountUsed, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.VoucherAmountUsed, src.CultureCode, string.Empty)))
               .ForMember(d => d.OrderVoucherAmount, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.OrderVoucherAmount, src.CultureCode, string.Empty)))
               .ForMember(d => d.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate.ToDateTimeFormat()));

            Mapper.CreateMap<VouchersViewModel, VoucherModel>();


            Mapper.CreateMap<RolePermissionModel, RolePermissionViewModel>();

            Mapper.CreateMap<PaymentSettingViewModel, PaymentSettingModel>()
                .ForMember(d => d.EnableAmex, opt => opt.MapFrom(src => src.EnableAmericanExpress));

            Mapper.CreateMap<PaymentSettingModel, PaymentSettingViewModel>()
                .ForMember(d => d.EnableAmericanExpress, opt => opt.MapFrom(src => src.EnableAmex))
                .ForMember(d => d.IsCallPaymentAPI, opt => opt.MapFrom(src => src.IsCallToPaymentAPI)).ReverseMap();

            Mapper.CreateMap<PaymentSettingValidationModel, PaymentSettingValidationViewModel>();
            Mapper.CreateMap<PaymentSettingValidationViewModel, PaymentSettingValidationModel>();

            Mapper.CreateMap<CSSModel, CSSViewModel>();
            Mapper.CreateMap<CSSViewModel, CSSModel>();

            Mapper.CreateMap<PortalSortSettingModel, PortalSortSettingViewModel>();
            Mapper.CreateMap<PortalSortSettingViewModel, PortalSortSettingModel>();

            Mapper.CreateMap<PortalPageSettingModel, PortalPageSettingViewModel>();
            Mapper.CreateMap<PortalPageSettingViewModel, PortalPageSettingModel>();


            Mapper.CreateMap<TemplateModel, TemplateViewModel>()
                .ForMember(d => d.MediaPath, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.MediaPath) ? ZnodeAdminSettings.DefaultImagePath : src.MediaPath));
            Mapper.CreateMap<TemplateViewModel, TemplateModel>();

            Mapper.CreateMap<CMSTextWidgetConfigurationModel, CMSTextWidgetConfigurationViewModel>();
            Mapper.CreateMap<CMSTextWidgetConfigurationViewModel, CMSTextWidgetConfigurationModel>();

            Mapper.CreateMap<CMSMediaWidgetConfigurationModel, CMSMediaWidgetConfigurationViewModel>();
            Mapper.CreateMap<CMSMediaWidgetConfigurationViewModel, CMSMediaWidgetConfigurationModel>();

            Mapper.CreateMap<ERPConfiguratorModel, ERPConfiguratorViewModel>().ReverseMap();

            Mapper.CreateMap<TouchPointConfigurationModel, TouchPointConfigurationViewModel>()
            .ForMember(d => d.ProcessStartedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ProcessStartedDate).ToTimeZoneDateTimeFormat()))
            .ForMember(d => d.ProcessCompletedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ProcessCompletedDate).ToTimeZoneDateTimeFormat()))
            .ForMember(d => d.SchedulerCreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.SchedulerCreatedDate).ToTimeZoneDateTimeFormat())).ReverseMap(); 

          Mapper.CreateMap<ERPTaskSchedulerModel, ERPTaskSchedulerViewModel>().ReverseMap();

            Mapper.CreateMap<ReferralCommissionModel, CustomerAffiliateViewModel>()
               .ForMember(dest => dest.ReferralCommission, opt => opt.MapFrom(src => src.ReferralCommission.GetValueOrDefault().ToPriceRoundOff()))
               .ForMember(dest => dest.OwedAmount, opt => opt.MapFrom(src => src.OwedAmount.GetValueOrDefault().ToPriceRoundOff()));

            Mapper.CreateMap<CustomerAffiliateViewModel, ReferralCommissionModel>()
               .ForMember(dest => dest.ReferralCommission, opt => opt.MapFrom(src => src.ReferralCommission.GetValueOrDefault().ToPriceRoundOff()))
               .ForMember(dest => dest.OwedAmount, opt => opt.MapFrom(src => src.OwedAmount.ToPriceRoundOff()));

            Mapper.CreateMap<UserAddressDataViewModel, UserAddressModel>().ReverseMap();

            Mapper.CreateMap<PublishProductModel, PublishProductsViewModel>()
                .ForMember(d => d.RetailPrice, opt => opt.MapFrom(src => src.RetailPrice.ToPriceRoundOff()))
                .ForMember(d => d.SalesPrice, opt => opt.MapFrom(src => src.SalesPrice.ToPriceRoundOff()))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(src => Equals(src.Quantity, null) ? null : Convert.ToDecimal(src.Quantity).ToInventoryRoundOff())).ReverseMap();

            Mapper.CreateMap<PublishAttributeModel, PublishAttributeViewModel>().ReverseMap();

            Mapper.CreateMap<EmailTemplateModel, EmailTemplateViewModel>().ReverseMap();

            Mapper.CreateMap<RMAConfigurationModel, RMAConfigurationViewModel>().ReverseMap();

            Mapper.CreateMap<RequestStatusModel, RequestStatusViewModel>().ReverseMap();


            Mapper.CreateMap<CountryModel, CountryViewModel>()
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<CurrencyModel, CurrencyViewModel>()
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToDateTimeFormat()))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<WebStoreCaseRequestModel, CaseRequestViewModel>()
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src =>Convert.ToDateTime( src.CreatedDate).ToTimeZoneDateTimeFormat()))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime( src.ModifiedDate).ToTimeZoneDateTimeFormat())).ReverseMap();
            
             Mapper.CreateMap<PortalShippingModel, PortalShippingViewModel>()
                  .ForMember(d => d.PackageWeightLimit, opt => opt.MapFrom(src => Convert.ToDouble(src.PackageWeightLimit)));

            Mapper.CreateMap<PortalShippingViewModel, PortalShippingModel>()
                 .ForMember(d => d.PackageWeightLimit, opt => opt.MapFrom(src => Convert.ToDouble(src.PackageWeightLimit)));

            Mapper.CreateMap<TaxPortalModel, TaxPortalViewModel>();

            Mapper.CreateMap<TaxPortalViewModel, TaxPortalModel>();

            Mapper.CreateMap<CartItemViewModel, ShoppingCartItemModel>()
               .ForMember(d => d.ProductId, opt => opt.MapFrom(src => src.PublishProductId))
               .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
               .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.ToPriceRoundOff()))
               .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()))
               .ForMember(d => d.OmsOrderStatusId, opt => opt.MapFrom(src => src.OrderLineItemStatusId));

            Mapper.CreateMap<ShoppingCartItemModel, CartItemViewModel>()
                  .ForMember(d => d.PublishProductId, opt => opt.MapFrom(src => src.ProductId))
                  .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
                  .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.ToPriceRoundOff()))
                  .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()))
                  .ForMember(d => d.OrderLineItemStatusId, opt => opt.MapFrom(src => src.OmsOrderStatusId));

            Mapper.CreateMap<AddToCartViewModel, ShoppingCartItemModel>()
             .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()));


            Mapper.CreateMap<EditCartItemViewModel, ShoppingCartItemModel>()
             .ForMember(d => d.OmsSavedcartLineItemId, opt => opt.MapFrom(src => src.OmsSavedCartLineItemId.GetValueOrDefault()));

            Mapper.CreateMap<AddToCartModel, AddToCartViewModel>().ReverseMap();

            Mapper.CreateMap<CartViewModel, ShoppingCartModel>()
                .ForMember(d => d.SubTotal, opt => opt.MapFrom(src => src.SubTotal.ToPriceRoundOff()))
                .ForMember(d => d.Total, opt => opt.MapFrom(src => src.Total.ToPriceRoundOff()))
                .ForMember(d => d.TaxCost, opt => opt.MapFrom(src => src.TaxCost.ToPriceRoundOff()))
                .ForMember(d => d.Discount, opt => opt.MapFrom(src => src.Discount.ToPriceRoundOff()))
                .ForMember(d => d.OrderTotalWithoutVoucher, opt => opt.MapFrom(src => src.OrderTotalWithoutVoucher.ToPriceRoundOff()));

            Mapper.CreateMap<ShoppingCartModel, CartViewModel>()
                .ForMember(d => d.ShippingCost, opt => opt.MapFrom(src => src.ShippingCost.ToPriceRoundOff()))
                .ForMember(d => d.CSRDiscountAmount, opt => opt.MapFrom(src => src.CSRDiscountAmount.ToPriceRoundOff()));

            Mapper.CreateMap<HighlightModel, HighlightViewModel>().ReverseMap();

            Mapper.CreateMap<HighlightProductModel, HighlightProductViewModel>();
            Mapper.CreateMap<HighlightProductViewModel, HighlightProductModel>();

            Mapper.CreateMap<SearchDocumentMappingModel, SearchDocumentMappingViewModel>()
                 .ForMember(d => d.ID, opt => opt.MapFrom(src => src.SearchDocumentMappingId))
                 .ForMember(d => d.Boost, opt => opt.MapFrom(src => src.Boost.ToPriceRoundOff())).ReverseMap();

            Mapper.CreateMap<SearchGlobalProductBoostModel, SearchGlobalProductBoostViewModel>()
                 .ForMember(d => d.ID, opt => opt.MapFrom(src => src.SearchGlobalProductBoostId))
                 .ForMember(d => d.Boost, opt => opt.MapFrom(src => src.Boost.ToPriceRoundOff()));

            Mapper.CreateMap<SearchGlobalProductBoostViewModel, SearchGlobalProductBoostModel>()
                 .ForMember(d => d.SearchGlobalProductBoostId, opt => opt.MapFrom(src => src.ID))
                 .ForMember(d => d.Boost, opt => opt.MapFrom(src => src.Boost.ToPriceRoundOff()));

            Mapper.CreateMap<SearchGlobalProductCategoryBoostModel, SearchGlobalProductCategoryBoostViewModel>()
                 .ForMember(d => d.ID, opt => opt.MapFrom(src => src.SearchGlobalProductCategoryBoostId))
                 .ForMember(d => d.Boost, opt => opt.MapFrom(src => src.Boost.ToPriceRoundOff()));

            Mapper.CreateMap<SearchGlobalProductCategoryBoostViewModel, SearchGlobalProductCategoryBoostModel>()
                 .ForMember(d => d.SearchGlobalProductCategoryBoostId, opt => opt.MapFrom(src => src.ID))
                 .ForMember(d => d.Boost, opt => opt.MapFrom(src => src.Boost.ToPriceRoundOff()));

            Mapper.CreateMap<BoostDataModel, BoostDataViewModel>().ReverseMap();
            Mapper.CreateMap<WebStoreAddOnModel, AddOnViewModel>();
            Mapper.CreateMap<WebStoreAddOnValueModel, AddOnValuesViewModel>();
            Mapper.CreateMap<WebStoreBundleProductModel, BundleProductViewModel>();
            Mapper.CreateMap<OrderModel, OrderViewModel>()
                .ForMember(d => d.PODocumentName, opt => opt.MapFrom(src => src.PoDocument))
                .ForMember(d => d.Total, opt => opt.MapFrom(src => src.Total.ToPriceRoundOff()))
                .ForMember(d => d.ImportDuty, opt => opt.MapFrom(src => src.ImportDuty))
                .ForMember(d => d.TaxSummaryList, opt => opt.MapFrom(src => src.TaxSummaryList))
                .ForMember(d => d.OrderTotalWithoutVoucher, opt => opt.MapFrom(src => src.OrderTotalWithoutVoucher.ToPriceRoundOff()))
                .ForMember(d => d.OrderDateWithTime, opt => opt.MapFrom(src => src.OrderDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src =>Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                 .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<FailedOrderTransactionModel, FailedOrderTransactionViewModel>().ReverseMap();

            Mapper.CreateMap<ConfigurableAttributeModel, ProductAttributesViewModel>();

            Mapper.CreateMap<StoreListViewModel, PortalListModel>().ReverseMap();
            Mapper.CreateMap<ProductFeedViewModel, ProductFeedModel>().ReverseMap();

            Mapper.CreateMap<WebStoreGroupProductModel, GroupProductViewModel>();

            Mapper.CreateMap<PublishAttributeModel, AttributesViewModel>();

            Mapper.CreateMap<MediaConfigurationModel, MediaConfigurationViewModel>().ReverseMap();

            Mapper.CreateMap<CmsContainerWidgetConfigurationModel, CmsContainerWidgetConfigurationViewModel>().ReverseMap();

            Mapper.CreateMap<BrandModel, BrandViewModel>().ReverseMap();

            Mapper.CreateMap<PortalProfileViewModel, PortalProfileModel>().ReverseMap();

            Mapper.CreateMap<VendorModel, VendorViewModel>().ReverseMap();

            //Mapper.CreateMap<ReportViewModel, ReportModel>().ReverseMap();

            Mapper.CreateMap<PortalIndexModel, PortalIndexViewModel>().ReverseMap();

            Mapper.CreateMap<PublishCategoryModel, CategoryViewModel>()
              .ForMember(d => d.PublishCategoryId, opt => opt.MapFrom(src => src.PublishCategoryId))
              .ForMember(d => d.CategoryName, opt => opt.MapFrom(src => src.Name));

            Mapper.CreateMap<PublishCatalogModel, PortalCatalogViewModel>()
              .ForMember(d => d.PublishCatalogId, opt => opt.MapFrom(src => src.PublishCatalogId))
              .ForMember(d => d.CatalogName, opt => opt.MapFrom(src => src.CatalogName))
              .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
              .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<PortalUnitViewModel, PortalUnitModel>().ReverseMap();

            Mapper.CreateMap<SearchIndexMonitorModel, SearchIndexMonitorViewModel>()
                .ForMember(d => d.CreatedDateWithTime, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDateWithTime, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateTimeFormat())).ReverseMap(); 

            Mapper.CreateMap<ImportLogsViewModel, ImportLogsModel>().ReverseMap();
            Mapper.CreateMap<ImportLogsListViewModel, ImportLogsListModel>().ReverseMap();
            Mapper.CreateMap<ExportLogsModel, ExportProcessLogsViewModel>()
               .ForMember(d => d.ProcessStartedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ProcessStartedDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ProcessCompletedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ProcessCompletedDate).ToTimeZoneDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<PublishProductModel, PublishProductsViewModel>().ReverseMap();

            Mapper.CreateMap<OrderLineItemModel, OrderLineItemViewModel>()
                .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToPriceRoundOff())).ReverseMap();

            Mapper.CreateMap<AccountQuoteModel, AccountQuoteViewModel>()
                .ForMember(d => d.BillingAddress, opt => opt.MapFrom(src => $"{src.BillingAddressModel.FirstName}{" "}{src.BillingAddressModel.LastName}{"<br />"}{src.BillingAddressModel.CompanyName}{"<br />"}{src.BillingAddressModel.Address1}{"<br />"}{src.BillingAddressModel.Address2}{"<br />"}" + (string.IsNullOrEmpty(src.BillingAddressModel.Address3) ? "" : $"{src.BillingAddressModel.Address3} {"<br />"}") + $" {src.BillingAddressModel.CityName}{", "} {src.BillingAddressModel.StateName}{", "} {src.BillingAddressModel.CountryName}{" "} {src.BillingAddressModel.PostalCode}{"<br />"} PH NO. {src.BillingAddressModel.PhoneNumber}"))
                .ForMember(d => d.ShippingAddress, opt => opt.MapFrom(src => $"{src.ShippingAddressModel.FirstName}{" "}{src.ShippingAddressModel.LastName}{"<br />"}{src.ShippingAddressModel.CompanyName}{"<br />"}{src.ShippingAddressModel.Address1}{"<br />"} {src.ShippingAddressModel.Address2}{"<br />"}" + (string.IsNullOrEmpty(src.ShippingAddressModel.Address3) ? "" : $"{src.ShippingAddressModel.Address3} {"<br />"}") + $" {src.ShippingAddressModel.CityName}{", "} {src.ShippingAddressModel.StateName}{", "} {src.ShippingAddressModel.CountryName}{" "} {src.ShippingAddressModel.PostalCode}{"<br />"}  PH NO. {src.ShippingAddressModel.PhoneNumber}"))
                .ForMember(d => d.ShippingAmount, opt => opt.MapFrom(src => src.ShippingAmount.ToPriceRoundOff()))
                .ForMember(d => d.TaxAmount, opt => opt.MapFrom(src => src.TaxCost.ToPriceRoundOff()))
                .ForMember(d => d.QuoteOrderTotal, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.QuoteOrderTotal, src.CultureCode, string.Empty)))
                .ForMember(d=>d.CreatedDate,opt => opt.MapFrom(src=> src.CreatedDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<AccountQuoteViewModel, AccountQuoteModel>()
                .ForMember(d => d.ShippingCountryCode, opt => opt.MapFrom(src => src.CountryCode))
                .ForMember(d => d.QuoteOrderTotal, opt => opt.MapFrom(src => Convert.ToDecimal(src.QuoteOrderTotal)));

            Mapper.CreateMap<AccountQuoteLineItemViewModel, AccountQuoteLineItemModel>().ReverseMap();

            Mapper.CreateMap<ImportLogsModel, ImportProcessLogsViewModel>()
                .ForMember(d => d.ProcessStartedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ProcessStartedDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ProcessCompletedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ProcessCompletedDate).ToTimeZoneDateTimeFormat())).ReverseMap();
            Mapper.CreateMap<ImportMappingModel, ImportMappingsViewModel>().ReverseMap();

            Mapper.CreateMap<SearchIndexServerStatusModel, SearchIndexServerStatusViewModel>().ReverseMap();
            Mapper.CreateMap<RefundPaymentModel, RefundPaymentViewModel>()
                .ForMember(d => d.RefundableAmountLeft, opt => opt.MapFrom(src => src.RefundableAmountLeft.ToPriceRoundOff()))
                .ForMember(d => d.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.ToPriceRoundOff())).ReverseMap();
            Mapper.CreateMap<OrderItemsRefundModel, OrderItemsRefundViewModel>().ReverseMap();

            Mapper.CreateMap<ImportLogDetailsModel, ImportLogsViewModel>().ReverseMap();
            Mapper.CreateMap<ImportLogDetailsModel, ImportLogDetailsDownloadViewModel>().ReverseMap();

            Mapper.CreateMap<OrderStateParameterViewModel, OrderModel>().ReverseMap();
            Mapper.CreateMap<OrderStateParameterModel, OrderStateParameterViewModel>().ReverseMap();

            Mapper.CreateMap<ReferralCommissionModel, ReferralCommissionViewModel>()
                 .ForMember(d => d.ReferralCommission, opt => opt.MapFrom(src => Equals(src.Name, AdminConstants.Percentage) ? $"{src.ReferralCommission.ToPriceRoundOff()}%" : HelperMethods.FormatPriceWithCurrency(src.ReferralCommission.GetValueOrDefault(), src.CultureCode, string.Empty)))
                 .ForMember(d => d.OwedAmount, opt => opt.MapFrom(src => src.OwedAmount.GetValueOrDefault().ToPriceRoundOff()))
                .ForMember(d => d.OrderCommission, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.OrderCommission, src.CultureCode, string.Empty)));

            Mapper.CreateMap<ReferralCommissionViewModel, ReferralCommissionModel>()
            .ForMember(d => d.ReferralCommission, opt => opt.MapFrom(src => Convert.ToDecimal(src.ReferralCommission).ToPriceRoundOff()))
            .ForMember(d => d.OwedAmount, opt => opt.MapFrom(src => src.OwedAmount.ToPriceRoundOff()))
            .ForMember(d => d.OrderCommission, opt => opt.MapFrom(src => Convert.ToDecimal(src.OrderCommission).ToPriceRoundOff()));

            Mapper.CreateMap<ProfileCatalogModel, ProfileCatalogViewModel>().ReverseMap();

            Mapper.CreateMap<AddressModel, WarehouseAddressViewModel>().ReverseMap();

            Mapper.CreateMap<RMARequestItemModel, RMARequestItemViewModel>()
                .ForMember(d => d.Price, opt => opt.MapFrom(src => src.Price.GetValueOrDefault().ToPriceRoundOff()))
                .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
                .ForMember(d => d.RMAMaxQuantity, opt => opt.MapFrom(src => src.RMAMaxQuantity.ToPriceRoundOff()));

            Mapper.CreateMap<RMARequestItemViewModel, RMARequestItemModel>()
                .ForMember(d => d.Price, opt => opt.MapFrom(src => src.Price.GetValueOrDefault().ToPriceRoundOff()))
                .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
                .ForMember(d => d.RMAMaxQuantity, opt => opt.MapFrom(src => src.RMAMaxQuantity.ToPriceRoundOff()));

            Mapper.CreateMap<OrderNotesModel, OrderNotesViewModel>()
               .ForMember(d => d.UserName, opt => opt.MapFrom(src => HelperUtility.IsNull(src.UserName) ? "Guest User" : src.UserName));
            Mapper.CreateMap<OrderNotesViewModel, OrderNotesModel>();

            Mapper.CreateMap<RMARequestModel, RMARequestViewModel>().ReverseMap();

            Mapper.CreateMap<RMARequestItemListModel, RMARequestItemListViewModel>().ReverseMap();

            Mapper.CreateMap<IssuedGiftCardListModel, IssuedGiftCardListViewModel>().ReverseMap();

            Mapper.CreateMap<IssuedGiftCardViewModel, IssuedGiftCardModel>().ReverseMap();

            Mapper.CreateMap<LicenceInfoViewModel, LicenceInfoModel>().ReverseMap();

            Mapper.CreateMap<PublishCatalogLogModel, PublishCatalogLogViewModel>()
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<PublishPortalLogModel, PublishPortalLogViewModel>()
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat())).ReverseMap();
            Mapper.CreateMap<AttributesSelectValuesViewModel, AttributesSelectValuesModel>().ReverseMap();

            Mapper.CreateMap<RoleModel, RoleViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.RoleId));

            Mapper.CreateMap<RoleViewModel, RoleModel>()
                .ForMember(d => d.RoleId, opt => opt.MapFrom(src => src.Id));

            Mapper.CreateMap<CurrencyListViewModel, CurrencyListModel>().ReverseMap();

            Mapper.CreateMap<AddonProductDetailViewModel, AddOnProductDetailModel>().ReverseMap();

            Mapper.CreateMap<PortalProfileShippingViewModel, PortalProfileShippingModel>().ReverseMap();

            Mapper.CreateMap<PromotionExportViewModel, PromotionModel>().ReverseMap();
            Mapper.CreateMap<TaxRuleTypeModel, ProviderEngineViewModel>()
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.TaxRuleTypeId)).ReverseMap();
            Mapper.CreateMap<PromotionTypeModel, ProviderEngineViewModel>()
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PromotionTypeId)).ReverseMap();
            Mapper.CreateMap<ShippingTypeModel, ProviderEngineViewModel>()
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ShippingTypeId)).ReverseMap();

            Mapper.CreateMap<CouponExportViewModel, CouponModel>().ReverseMap();
            Mapper.CreateMap<ReportModel, ViewModels.ReportViewModel>().ReverseMap();
            Mapper.CreateMap<AssociatedProductModel, AssociatedProductModel>().ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()));

            Mapper.CreateMap<PIMAttributeGroupViewModel, PIMAttributeGroupModel>().ReverseMap();

            Mapper.CreateMap<OrderHistoryModel, OrderHistoryViewModel>()
                .ForMember(d => d.OrderDateWithTime, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<PaymentHistoryModel, PaymentHistoryViewModel>()
                .ForMember(d => d.OrderDateWithTime, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat())).ReverseMap();
           
            Mapper.CreateMap<DashboardTopItemsModel, DashboardTopItemsViewModel>()
               .ForMember(d => d.TotalSales, opt => opt.MapFrom(src => Equals(src.TotalSales, null) ? string.Empty : src.Symbol + src.TotalSales))
               .ForMember(d => d.Sales, opt => opt.MapFrom(src => Equals(src.Sales, null) ? string.Empty : src.Symbol + src.Sales));

            Mapper.CreateMap<DashboardItemsModel, DashboardItemsViewModel>()
                .ForMember(d=> d.Date, opt => opt.MapFrom(src=> Equals(src.Date,null)?string.Empty :Convert.ToDateTime(src.Date).ToTimeZoneDateTimeFormat())); 
            Mapper.CreateMap<DashboardItemsViewModel, DashboardItemsModel>(); 

            Mapper.CreateMap<DashboardTopItemsViewModel, DashboardTopItemsModel>();
            Mapper.CreateMap<ReturnOrderLineItemModel, ReturnOrderLineItemViewModel>()
                .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity.ToInventoryRoundOff()))
                .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.ToPriceRoundOff()))
                .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.ToPriceRoundOff())).ReverseMap();

            Mapper.CreateMap<ERPConnectorControlModel, Property>().ReverseMap();
            Mapper.CreateMap<ERPConnectorControlModel, ERPConnectorViewModel>().ReverseMap();

            Mapper.CreateMap<TagManagerModel, TagManagerViewModel>().ReverseMap();
            Mapper.CreateMap<PortalTrackingPixelModel, PortalTrackingPixelViewModel>().ReverseMap();
            Mapper.CreateMap<SMTPModel, SMTPViewModel>().ReverseMap();
            Mapper.CreateMap<KlaviyoModel, KlaviyoViewModel>().ReverseMap();
            Mapper.CreateMap<EmailModel, EmailViewModel>().ReverseMap();
            Mapper.CreateMap<BlogNewsModel, BlogNewsViewModel>()
                .ForMember(d => d.ActivationDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ActivationDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ExpirationDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ExpirationDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat())).ReverseMap();
            Mapper.CreateMap<BlogNewsCommentModel, BlogNewsCommentViewModel>().ReverseMap();

            Mapper.CreateMap<RobotsTxtModel, RobotsTxtViewModel>().ReverseMap();

            Mapper.CreateMap<SearchKeywordsRedirectModel, SearchKeywordsRedirectViewModel>().ReverseMap();

            Mapper.CreateMap<SearchSynonymsModel, SearchSynonymsViewModel>()
                .ForMember(d=>d.OriginalTerm, opt=>opt.MapFrom(src=>src.OriginalTerm.Replace(ZnodeConstant.PipeSeparator,ZnodeConstant.CommaSeparator)))
                .ForMember(d => d.ReplacedBy, opt => opt.MapFrom(src => src.ReplacedBy.Replace(ZnodeConstant.PipeSeparator, ZnodeConstant.CommaSeparator))).ReverseMap();


            Mapper.CreateMap<SendInvoiceModel, SendInvoiceViewModel>().ReverseMap();

            Mapper.CreateMap<CacheModel, CacheViewModel>()
                .ForMember(d => d.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString())).ReverseMap();

            Mapper.CreateMap<CloudflareErrorResponseModel, CloudflareErrorViewModel>().ReverseMap();

            Mapper.CreateMap<GlobalAttributeModel, GlobalAttributeViewModel>().ReverseMap();

            Mapper.CreateMap<GlobalAttributeGroupModel, GlobalAttributeGroupViewModel>().ReverseMap();
            Mapper.CreateMap<GlobalAttributeGroupMapperModel, GlobalAttributeGroupMapperViewModel>().ReverseMap();

            Mapper.CreateMap<GlobalAttributeGroupModel, AssignedEntityGroupViewModel>().ReverseMap();

            Mapper.CreateMap<GlobalAttributeEntityDetailsModel, GlobalAttributeEntityDetailsViewModel>().ReverseMap();

            Mapper.CreateMap<GlobalAttributeEntityDetailsModel, GlobalAttributeEntityDetailsViewModel>();

            Mapper.CreateMap<GlobalAttributeFamilyModel, GlobalAttributeFamilyViewModel>().ReverseMap();

            Mapper.CreateMap<GlobalAttributeFamilyListModel, GlobalAttributeFamilyListViewModel>().ReverseMap();

            Mapper.CreateMap<GlobalAttributeValuesModel, GlobalAttributeValuesViewModel>().ReverseMap();

            Mapper.CreateMap<SearchProfileModel, SearchProfileViewModel>()
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                 .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<SearchQueryTypeModel, SearchQueryTypeViewModel>().ReverseMap();

            Mapper.CreateMap<SearchFeatureModel, SearchFeatureViewModel>().ReverseMap();

            Mapper.CreateMap<DownloadableProductKeyModel, DownloadableProductKeyViewModel>().ReverseMap();

            Mapper.CreateMap<EntityAttributeModel, EntityAttributeViewModel>().ReverseMap();
            Mapper.CreateMap<EntityAttributeDetailsModel, EntityAttributeDetailsViewModel>().ReverseMap();

            Mapper.CreateMap<SearchAttributesModel, SearchAttributesViewModel>().ReverseMap();
            Mapper.CreateMap<SearchProductModel, SearchProfileProductViewModel>().ReverseMap();

            Mapper.CreateMap<SearchAttributesViewModel, SearchAttributesModel>().ReverseMap();

            Mapper.CreateMap<SearchProfilePortalViewModel, SearchProfilePortalModel>().ReverseMap();

            Mapper.CreateMap<SearchTriggersViewModel, SearchTriggersModel>().ReverseMap();

            Mapper.CreateMap<FormBuilderViewModel, FormBuilderModel>().ReverseMap();

            Mapper.CreateMap<PortalSearchProfileViewModel, PortalSearchProfileModel>().ReverseMap();

            Mapper.CreateMap<CMSFormWidgetConfigurationViewModel, CMSFormWidgetConfigrationModel>().ReverseMap();

            Mapper.CreateMap<FormSubmissionViewModel, FormSubmissionModel>().ReverseMap();

            Mapper.CreateMap<FormSubmissionModel, FormSubmissionViewModel>()
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
            .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<FormBuilderAttributeGroupModel, FormBuilderAttributeGroupViewModel>().ReverseMap();

            Mapper.CreateMap<FormWidgetEmailConfigurationViewModel, FormWidgetEmailConfigurationModel>().ReverseMap();

            Mapper.CreateMap<LogMessageModel, LogMessageViewModel>()
            .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<LogMessageConfigurationModel, LogConfigurationViewModel>().ReverseMap();

            Mapper.CreateMap<CMSSearchWidgetConfigurationViewModel, CMSSearchWidgetConfigurationModel>().ReverseMap();

            Mapper.CreateMap<UserApproverModel, UserApproverViewModel>().ReverseMap();

            Mapper.CreateMap<UserModel, UserApproverViewModel>();

            Mapper.CreateMap<PermissionCodeModel, PermissionCodeViewModel>().ReverseMap();
            Mapper.CreateMap<FieldValueViewModel, FieldValueModel>().ReverseMap();

            Mapper.CreateMap<SearchBoostAndBuryRuleViewModel, SearchBoostAndBuryRuleModel>().ReverseMap();

            Mapper.CreateMap<SearchBoostAndBuryRuleModel, SearchBoostAndBuryRuleViewModel>()
                .ForMember(d => d.StartDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.StartDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.EndDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.EndDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()));

            
            Mapper.CreateMap<SearchTriggerRuleViewModel, SearchTriggerRuleModel>().ReverseMap();

            Mapper.CreateMap<SearchItemRuleViewModel, SearchItemRuleModel>().ReverseMap();
            Mapper.CreateMap<OrderWarehouseModel, OrderWarehouseViewModel>().ReverseMap();

            Mapper.CreateMap<PublishStateMappingModel, PublishStateMappingViewModel>();
            Mapper.CreateMap<PublishStateMappingViewModel, PublishStateMappingModel>();
            Mapper.CreateMap<PublishHistoryModel, PublishHistoryViewModel>()
                 .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                 .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()));
          
            Mapper.CreateMap<CategoryViewModel, CategoryModel>().ReverseMap();
            Mapper.CreateMap<PortalApprovalModel, PortalApprovalViewModel>().ReverseMap();
            Mapper.CreateMap<PortalPaymentApproverModel, PortalPaymentApproverViewModel>().ReverseMap();
            Mapper.CreateMap<DiagnosticsModel, DiagnosticViewModel>()
                 .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                 .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat())).ReverseMap();
               

            Mapper.CreateMap<LogMessageViewModel, LogMessageModel>().ReverseMap();

            Mapper.CreateMap<ImpersonationActivityLogModel, ImpersonationLogViewModel>()
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<ImpersonationLogViewModel, ImpersonationActivityLogModel>().ReverseMap();
            Mapper.CreateMap<PortalBrandDetailViewModel, PortalBrandDetailModel>().ReverseMap();
            Mapper.CreateMap<UserAccountViewModel, UserAccountModel>().ReverseMap();
            Mapper.CreateMap<SearchReportModel, SearchReportViewModel>().ReverseMap();
            Mapper.CreateMap<ConfigurationSettingModel, ConfigurationSettingViewModel>().ReverseMap();
            Mapper.CreateMap<StockNoticeSettingsModel, StockNoticeSettingsViewModel>().ReverseMap();

            Mapper.CreateMap<ImportManageTemplateModel, ImportManageTemplateMappingViewModel>().ReverseMap();

            #region Content Containers
            Mapper.CreateMap<ContentContainerCreateModel, ContentContainerViewModel>().ReverseMap();
            Mapper.CreateMap<ContentContainerUpdateModel, ContentContainerViewModel>().ReverseMap();
            Mapper.CreateMap<AssociatedVariantModel, ContainerVariantViewModel>().ReverseMap();
          
            
            Mapper.CreateMap<ContentContainerResponseModel, ContentContainerViewModel>()
               .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
               .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat()));

            Mapper.CreateMap<ContainerTemplateModel, ContainerTemplateViewModel>()
                .ForMember(d => d.MediaPath, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.MediaPath) ? ZnodeAdminSettings.DefaultImagePath : src.MediaPath))
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<ContainerTemplateViewModel, ContainerTemplateModel>();


            Mapper.CreateMap<ContainerTemplateCreateModel, ContainerTemplateViewModel>().ReverseMap();

            Mapper.CreateMap<ContainerTemplateUpdateModel, ContainerTemplateViewModel>().ReverseMap();

            #endregion

            Mapper.CreateMap<SalesRepUserModel, SalesRepUsersViewModel>().ReverseMap();

            #region Quote
            Mapper.CreateMap<QuoteModel, QuoteViewModel>()
                 .ForMember(d => d.TotalAmount, opt => opt.MapFrom(src => HelperMethods.FormatPriceWithCurrency(src.TotalAmount, src.CultureCode, string.Empty)))
                 .ForMember(d => d.QuoteDate, opt => opt.MapFrom(src => src.QuoteDate.ToTimeZoneDateTimeFormat()))
                 .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat()))
                 .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateTimeFormat()));


            Mapper.CreateMap<QuoteResponseModel, QuoteResponseViewModel>();
              
            Mapper.CreateMap<QuoteResponseModel, QuoteCartViewModel>()
                .ForMember(d => d.TaxCost, opt => opt.MapFrom(src => src.TaxAmount))
                .ForMember(d => d.Total, opt => opt.MapFrom(src => src.QuoteTotal.ToPriceRoundOff()));

            Mapper.CreateMap<QuoteResponseViewModel, QuoteSessionModel>()
                .ForMember(d => d.ShippingAddressModel, opt => opt.MapFrom(src => src.CustomerInformation.ShippingAddress))
                .ForMember(d => d.BillingAddressModel, opt => opt.MapFrom(src => src.CustomerInformation.BillingAddress))
                .ForMember(d => d.ShippingId, opt => opt.MapFrom(src => src.CartInformation.ShippingId))
                .ForMember(d => d.QuoteExpirationDate, opt => opt.MapFrom(src => src.QuoteInformation.QuoteExpirationDate))
                .ForMember(d => d.InHandDate, opt => opt.MapFrom(src => src.QuoteInformation.InHandDate));


             Mapper.CreateMap<QuoteSessionModel, ShoppingCartModel>()
                .ForMember(d => d.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddressModel))
                .ForMember(d => d.BillingAddress, opt => opt.MapFrom(src => src.BillingAddressModel))
                .ForMember(d => d.PublishedCatalogId, opt => opt.MapFrom(src => src.PublishCatalogId))
                .AfterMap((src, dest) => dest.Shipping.ShippingCountryCode = src.ShippingAddressModel.CountryName)
                .AfterMap((src, dest) => dest.Shipping.ShippingId = src.CartInformation.ShippingId);

            Mapper.CreateMap<QuoteSessionModel, QuoteCartViewModel>()
                 .ForMember(d => d.TaxCost, opt => opt.MapFrom(src => src.TaxAmount))
                 .ForMember(d => d.Total, opt => opt.MapFrom(src => src.QuoteTotal.ToPriceRoundOff()));

            Mapper.CreateMap<ShoppingCartModel, QuoteCartViewModel>()
                .ForMember(d => d.TaxCost, opt => opt.MapFrom(src => src.TaxCost))
                .ForMember(d => d.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
                .ForMember(d => d.Total, opt => opt.MapFrom(src => src.Total.ToPriceRoundOff()))
                .ForMember(d => d.IsTaxExempt, opt => opt.MapFrom(src => Convert.ToBoolean(src.CustomTaxCost != null)));
                
            Mapper.CreateMap<QuoteCartViewModel, ShoppingCartModel>();
            Mapper.CreateMap<QuoteSessionModel, QuoteResponseViewModel>();
            Mapper.CreateMap<ShoppingCartItemModel, QuoteLineItemModel>()
                  .ForMember(d => d.Price, opt => opt.MapFrom(src => src.UnitPrice));
            Mapper.CreateMap<ShoppingCartItemModel, ProductDetailModel>()
                  .ForMember(d => d.Price, opt => opt.MapFrom(src => src.UnitPrice))
                  .ForMember(d => d.InitialShippingCost, opt => opt.MapFrom(src => src.ShippingCost));
            Mapper.CreateMap<QuoteSessionModel, UpdateQuoteModel>()
                 .ForMember(d => d.QuoteTotal, opt => opt.MapFrom(src => src.CartInformation.Total))
                 .ForMember(d => d.TaxCost, opt => opt.MapFrom(src => src.CartInformation.TaxCost))
                 .ForMember(d => d.ShippingCost, opt => opt.MapFrom(src => src.CartInformation.ShippingCost))
                 .ForMember(d => d.ShippingAddressId, opt => opt.MapFrom(src => src.ShippingAddressModel.AddressId))
                 .ForMember(d => d.BillingAddressId, opt => opt.MapFrom(src => src.BillingAddressModel.AddressId))
                 .ForMember(d => d.QuoteLineItems, opt => opt.MapFrom(src => src.CartInformation.ShoppingCartItems))
                 .ForMember(d => d.IsTaxExempt, opt => opt.MapFrom(src => src.CartInformation.IsTaxExempt))
                 .ForMember(d => d.ImportDuty, opt => opt.MapFrom(src => src.CartInformation.ImportDuty))
                 .ForMember(d => d.TaxSummaryList, opt => opt.MapFrom(src => src.CartInformation.TaxSummaryList));

            Mapper.CreateMap<UpdateQuoteModel, QuoteResponseViewModel>();
            Mapper.CreateMap<QuoteCreateViewModel, CreateOrderViewModel>();
            Mapper.CreateMap<QuoteCreateModel, QuoteCreateViewModel>().ReverseMap();

            Mapper.CreateMap<ConvertQuoteToOrderViewModel, ConvertQuoteToOrderModel>();
            Mapper.CreateMap<SubmitPaymentDetailsViewModel, SubmitPaymentDetailsModel>();
            Mapper.CreateMap<OrderModel, OrdersViewModel>()
                .ForMember(d => d.OrderId, opt => opt.MapFrom(src => src.OmsOrderId));
            #endregion

            Mapper.CreateMap<PowerBISettingsModel, PowerBISettingsViewModel>().ReverseMap();

            #region Recommendation Engine
            Mapper.CreateMap<RecommendationSettingModel, RecommendationSettingViewModel>().ReverseMap();
            Mapper.CreateMap<RecommendationDataGenerateModel, RecommendationDataGenerateViewModel>().ReverseMap();
            Mapper.CreateMap<RecommendationGeneratedDataModel, RecommendationGeneratedDataViewModel>().ReverseMap();
            #endregion

            #region CMS index
            Mapper.CreateMap<CMSPortalContentPageIndexModel, CMSPortalContentPageIndexViewModel>()
                .ForMember(d => d.CreatedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.CreatedDate).ToTimeZoneDateTimeFormat()))
                 .ForMember(d => d.ModifiedDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ModifiedDate).ToTimeZoneDateTimeFormat())).ReverseMap();

            Mapper.CreateMap<CMSSearchIndexMonitorModel, CMSSearchIndexMonitorViewModel>()
                .ForMember(d => d.CreatedDateWithTime, opt => opt.MapFrom(src => src.CreatedDate.ToTimeZoneDateTimeFormat()))
                .ForMember(d => d.ModifiedDateWithTime, opt => opt.MapFrom(src => src.ModifiedDate.ToTimeZoneDateTimeFormat())).ReverseMap();
            #endregion

            #region RMA Return
            Mapper.CreateMap<RMAReturnModel, RMAReturnViewModel>()
                .ForMember(d => d.TotalReturnAmount, opt => opt.MapFrom(src => src.TotalReturnAmount.ToPriceRoundOff()))
                .ForMember(d => d.ReturnDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ReturnDate).ToTimeZoneDateTimeFormat())).ReverseMap();


            Mapper.CreateMap<RMAReturnLineItemModel, RMAReturnLineItemViewModel>().ReverseMap();
            Mapper.CreateMap<RMAReturnHistoryModel, RMAReturnHistoryViewModel>().ReverseMap();

            Mapper.CreateMap<RMAReturnCalculateModel, RMAReturnViewModel>()
                .ForMember(d => d.ReturnLineItems, opt => opt.MapFrom(src => src.ReturnCalculateLineItemList)).ReverseMap();

            Mapper.CreateMap<RMAReturnViewModel, RMAReturnCalculateModel>()
                .ForMember(d => d.ReturnCalculateLineItemList, opt => opt.MapFrom(src => src.ReturnLineItems)).ReverseMap();

            Mapper.CreateMap<RMAReturnCalculateLineItemModel, RMAReturnLineItemViewModel>()
                .ForMember(d => d.Total, opt => opt.MapFrom(src => src.TotalLineItemPrice)).ReverseMap();

            Mapper.CreateMap< RMAReturnLineItemViewModel, RMAReturnCalculateLineItemModel>()
                 .ForMember(d => d.TotalLineItemPrice, opt => opt.MapFrom(src => src.Total)).ReverseMap();
            Mapper.CreateMap<OrderModel, RMAReturnOrderDetailViewModel>()
                 .ForMember(d => d.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToTimeZoneDateTimeFormat())).ReverseMap();
            
            Mapper.CreateMap<RMAReturnCartViewModel, ShoppingCartModel>().ReverseMap();
            Mapper.CreateMap<RMAReturnCalculateModel, RMAReturnCalculateViewModel>().ReverseMap();
            Mapper.CreateMap<RMAReturnCalculateLineItemModel, RMAReturnCalculateLineItemViewModel>().ReverseMap();
            Mapper.CreateMap<ShoppingCartItemModel, RMAReturnCartItemViewModel>()
              .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
              .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice))
             .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity));

            Mapper.CreateMap<RMAReturnCartItemViewModel, ShoppingCartItemModel>()
                .ForMember(d => d.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(d => d.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(src => src.Quantity));

            #endregion

            Mapper.CreateMap<ParentAccountModel, ParentAccountViewModel>().ReverseMap();  

            Mapper.CreateMap<OrderModel, OrderPaymentCreateModel>().ReverseMap();

            Mapper.CreateMap<AssociatedPublishedBundleProductModel, AssociatedPublishedBundleProductViewModel>();

            Mapper.CreateMap<TaxSummaryModel, TaxSummaryViewModel>().ReverseMap();

            Mapper.CreateMap<ContentContainerResponseModel, ContainerVariantDataViewModel>();

            Mapper.CreateMap<SMSModel, PortalSMSViewModel>().ReverseMap();

            Mapper.CreateMap<UserDetailsViewModel, UserDetailsModel>().ReverseMap();

            Mapper.CreateMap<PaymentTokenViewModel, PaymentGatewayTokenModel>().ReverseMap();

        }
    }
}