CREATE TABLE [dbo].[ZnodePublishProductEntity] (
    [PublishProductEntityId]        INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                     INT            NOT NULL,
    [IndexId]                       NVARCHAR (300) NOT NULL,
    [ZnodeProductId]                INT            NOT NULL,
    [ZnodeCatalogId]                INT            NOT NULL,
    [SKU]                           NVARCHAR (300)  NOT NULL,
    [LocaleId]                      INT            NOT NULL,
    [Name]                          NVARCHAR (600)  NOT NULL,
    [ZnodeCategoryIds]              INT            NOT NULL,
    [IsActive]                      BIT            NOT NULL,
    [Attributes]                    NVARCHAR (MAX) NOT NULL,
    [Brands]                        NVARCHAR (MAX) NOT NULL,
    [CategoryName]                  VARCHAR (300)  NOT NULL,
    [CatalogName]                   VARCHAR (300)  NOT NULL,
    [DisplayOrder]                  INT            NOT NULL,
    [RevisionType]                  VARCHAR (50)   NOT NULL,
    [AssociatedProductDisplayOrder] INT            NOT NULL,
    [ProductIndex]                  INT            NOT NULL,
    [SalesPrice]                    VARCHAR (50)   NULL,
    [RetailPrice]                   VARCHAR (50)   NULL,
    [CultureCode]                   VARCHAR (50)   NULL,
    [CurrencySuffix]                VARCHAR (50)   NULL,
    [CurrencyCode]                  VARCHAR (50)   NULL,
    [SeoDescription]                VARCHAR (1000) NULL,
    [SeoKeywords]                   VARCHAR (1000) NULL,
    [SeoTitle]                      VARCHAR (1000) NULL,
    [SeoUrl]                        VARCHAR (1000) NULL,
    [ImageSmallPath]                VARCHAR (500)  NOT NULL,
    [SKULower]                      NVARCHAR (300) NULL,
    [ElasticSearchEvent]            INT            NULL,
    [ZnodeParentCategoryIds]        VARCHAR (1000) NULL,
    [ModifiedDate]                  DATETIME       NULL,
    [IsSingleProductPublish]        BIT            NULL,
    [IsCacheClear]                  BIT            NULL,
    CONSTRAINT [PK_ZnodePublishProductEntity] PRIMARY KEY CLUSTERED ([PublishProductEntityId] ASC) WITH (FILLFACTOR = 90)
);










GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishProductEntity_VersionId]
    ON [dbo].[ZnodePublishProductEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishProductEntity_ZnodeCatalogId_SKU_LocaleId_C055E]
    ON [dbo].[ZnodePublishProductEntity]([ZnodeCatalogId] ASC, [SKU] ASC, [LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishProductEntity_VersionId_ZnodeProductId_ZnodeCatalogId_LocaleId_IsActive_B2EAF]
    ON [dbo].[ZnodePublishProductEntity]([VersionId] ASC, [ZnodeProductId] ASC, [ZnodeCatalogId] ASC, [LocaleId] ASC, [IsActive] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishProductEntity_VersionId_ZnodeProductId_LocaleId_ProductIndex_5D8F6]
    ON [dbo].[ZnodePublishProductEntity]([VersionId] ASC, [ZnodeProductId] ASC, [LocaleId] ASC, [ProductIndex] ASC);


GO



GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishProductEntity_VersionId_ZnodeCatalogId_IsActive_ZnodeCategoryIds_B590A]
    ON [dbo].[ZnodePublishProductEntity]([VersionId] ASC, [ZnodeCatalogId] ASC, [IsActive] ASC, [ZnodeCategoryIds] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishProductEntity_CatalogId]
    ON [dbo].[ZnodePublishProductEntity]([VersionId] ASC, [ZnodeCatalogId] ASC);


GO
CREATE NONCLUSTERED INDEX [Inx_ZnodePublishProductEntity_ZnodeProductId_LocaleId_IsActive_VersionId_ProductIndex]
    ON [dbo].[ZnodePublishProductEntity]([VersionId] ASC, [ZnodeProductId] ASC, [LocaleId] ASC, [IsActive] ASC, [ProductIndex] ASC);
GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePublishProductEntity_ElasticSearchEvent]
    ON [dbo].[ZnodePublishProductEntity]([ElasticSearchEvent] ASC);
GO

