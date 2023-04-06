namespace Znode.Libraries.ECommerce.Utilities
{
    public enum WebStoreEnum
    {
        ZnodeCategoryId,
        ZnodeParentCategoryIds,
        ZnodeCatalogId,
        ContentPageMapping,
        ZnodeProductId,
        ProfileIds,
        IsActive,
        VersionId,
        ParentProductId,
        ConfigurableProductSKU,
        ProductIndex,
        Required,
        Optional,
        Auto,
        ZnodeImportProcessLog,
        ZnodeUserImportProcessLog,
        ZnodeShippingAddressImportProcessLog,
        ZnodeImportLogDetails,
        LocaleId,
        IsGetParentCategory,
        IsBindImage,
        IsCallFromWebstore,
        CategoryCode,
        SKU,
        ProfileId,
        RevisionType,
        ZnodeCategoryIds
    }

    public enum MongoWebStoreEntityEnum
    {
        PortalId,
        VersionId,
        RevisionType
    }

    public enum ProductCompareErrorCode
    {
        CategoryChanged = 1,
        ProductExist = 2,
        MaxProductLimit = 3
    }

    public enum SortEnum
    {
        HighestRating = 1,
        MostReviewed,
        PriceHighToLow,
        PriceLowToHigh,
        NameAToZ,
        NameZToA,
        OutOfStock,
        InStock

    }

    public enum TemplateName
    {
        ImportShippingAddress,
        ImportB2BCustomer
    }

    public enum CurrentEnvironmentEnum
    {
        Prod,
        Qa,
        Uat
    }

    public enum ElasticEvent
    {
        ElasticSearchEvent
    }
}
