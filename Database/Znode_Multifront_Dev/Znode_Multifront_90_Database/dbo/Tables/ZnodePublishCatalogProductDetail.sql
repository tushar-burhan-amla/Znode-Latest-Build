CREATE TABLE [dbo].[ZnodePublishCatalogProductDetail] (
    [PimCatalogProductDetailId] INT             IDENTITY (1, 1) NOT NULL,
    [PublishProductId]          INT             NULL,
    [PublishCatalogId]          INT             NULL,
    [PimCategoryHierarchyId]    INT             NULL,
    [SKU]                       NVARCHAR (2000) NOT NULL,
    [ProductName]               NVARCHAR (2000) NULL,
    [CategoryName]              NVARCHAR (2000) NULL,
    [CatalogName]               NVARCHAR (2000) NULL,
    [LocaleId]                  INT             NOT NULL,
    [IsActive]                  BIT             NULL,
    [ProfileIds]                VARCHAR (MAX)   NULL,
    [ProductIndex]              INT             NOT NULL,
    [CreatedBy]                 INT             NOT NULL,
    [CreatedDate]               DATETIME        NOT NULL,
    [ModifiedBy]                INT             NOT NULL,
    [ModifiedDate]              DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePimCatalogProductDetail] PRIMARY KEY CLUSTERED ([PimCatalogProductDetailId] ASC) WITH (FILLFACTOR = 90)
);




GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePublishCatalogProductDetail_PublishCatalogId_PublishProductId]
    ON [dbo].[ZnodePublishCatalogProductDetail]([PublishCatalogId] ASC, [PublishProductId] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePublishCatalogProductDetail_PublishProductId_PublishCatalogId_PimCategoryHierarchyId]
    ON [dbo].[ZnodePublishCatalogProductDetail]([PublishProductId] ASC, [PublishCatalogId] ASC, [PimCategoryHierarchyId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePublishCatalogProductDetail_PublishProductId]
    ON [dbo].[ZnodePublishCatalogProductDetail]([PublishProductId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePublishCatalogProductDetail_PublishCatalogId]
    ON [dbo].[ZnodePublishCatalogProductDetail]([PublishCatalogId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [ZnodePublishCatalogProductDetail_PimCategoryHierarchyId]
    ON [dbo].[ZnodePublishCatalogProductDetail]([PimCategoryHierarchyId] ASC)
    INCLUDE([PublishProductId], [PublishCatalogId]) WITH (FILLFACTOR = 90);
	GO
	CREATE NONCLUSTERED INDEX [Ind_ZnodePublishCatalogProductDetail_PublishCatalogId_LocaleId]
    ON [dbo].[ZnodePublishCatalogProductDetail]([PublishCatalogId] ASC, [LocaleId] ASC)
    INCLUDE([PublishProductId]);
GO

CREATE NONCLUSTERED INDEX [Ind_ZnodePublishCatalogProductDetail_PublishCatalogId_ModifiedDate]
    ON [dbo].[ZnodePublishCatalogProductDetail]([PublishCatalogId] ASC, [ModifiedDate] ASC);
GO

