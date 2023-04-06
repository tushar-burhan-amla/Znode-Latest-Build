CREATE TABLE [dbo].[ZnodePublishCategoryProduct] (
    [PublishCategoryProductId] INT      IDENTITY (1, 1) NOT NULL,
    [PublishProductId]         INT      NULL,
    [PublishCategoryId]        INT      NULL,
    [PublishCatalogId]         INT      NULL,
    [CreatedBy]                INT      NOT NULL,
    [CreatedDate]              DATETIME NOT NULL,
    [ModifiedBy]               INT      NOT NULL,
    [ModifiedDate]             DATETIME NOT NULL,
    [PimCategoryHierarchyId]   INT      NULL,
    [ProductIndex]             INT      NULL,
    CONSTRAINT [PK_ZnodePublishCategoryProduct] PRIMARY KEY CLUSTERED ([PublishCategoryProductId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePublishCategoryProduct_ZnodePublishCatalog] FOREIGN KEY ([PublishCatalogId]) REFERENCES [dbo].[ZnodePublishCatalog] ([PublishCatalogId]),
    CONSTRAINT [FK_ZnodePublishCategoryProduct_ZnodePublishCategory] FOREIGN KEY ([PublishCategoryId]) REFERENCES [dbo].[ZnodePublishCategory] ([PublishCategoryId]),
    CONSTRAINT [FK_ZnodePublishCategoryProduct_ZnodePublishProduct] FOREIGN KEY ([PublishProductId]) REFERENCES [dbo].[ZnodePublishProduct] ([PublishProductId])
);
















GO



GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryProduct_PublishCategoryId]
    ON [dbo].[ZnodePublishCategoryProduct]([PublishCategoryId] ASC)
    INCLUDE([PublishProductId], [PublishCatalogId]);


GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePublishCategoryProduct_PublishCatalogId]
    ON [dbo].[ZnodePublishCategoryProduct]([PublishCatalogId] ASC)
    INCLUDE([PublishProductId], [PimCategoryHierarchyId]);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryProduct_PublishProductId_PublishCatalogId_A5436]
    ON [dbo].[ZnodePublishCategoryProduct]([PublishProductId] ASC, [PublishCatalogId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryProduct_PublishProductId_PublishCatalogId_74840]
    ON [dbo].[ZnodePublishCategoryProduct]([PublishProductId] ASC, [PublishCatalogId] ASC)
    INCLUDE([PublishCategoryId]) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryProduct_PublishProductId_PublishCatalogId_3E771]
    ON [dbo].[ZnodePublishCategoryProduct]([PublishProductId] ASC, [PublishCatalogId] ASC)
    INCLUDE([PimCategoryHierarchyId]) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryProduct_PublishProductId_0BEBF]
    ON [dbo].[ZnodePublishCategoryProduct]([PublishProductId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryProduct_PublishCategoryId_PublishCatalogId_PimCategoryHierarchyId_A2899]
    ON [dbo].[ZnodePublishCategoryProduct]([PublishCategoryId] ASC, [PublishCatalogId] ASC, [PimCategoryHierarchyId] ASC)
    INCLUDE([PublishProductId]) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryProduct_PublishCatalogId_PublishCategoryId_E8266]
    ON [dbo].[ZnodePublishCategoryProduct]([PublishCatalogId] ASC, [PublishCategoryId] ASC)
    INCLUDE([PublishProductId]) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryProduct_PublishCatalogId_1B166]
    ON [dbo].[ZnodePublishCategoryProduct]([PublishCatalogId] ASC)
    INCLUDE([PublishProductId], [PublishCategoryId]) WITH (FILLFACTOR = 90);

