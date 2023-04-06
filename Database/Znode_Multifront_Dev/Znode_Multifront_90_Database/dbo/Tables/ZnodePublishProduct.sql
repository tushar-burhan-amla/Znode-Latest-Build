CREATE TABLE [dbo].[ZnodePublishProduct] (
    [PublishProductId] INT      IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId] INT      NULL,
    [PimProductId]     INT      NULL,
    [CreatedBy]        INT      NOT NULL,
    [CreatedDate]      DATETIME NOT NULL,
    [ModifiedBy]       INT      NOT NULL,
    [ModifiedDate]     DATETIME NOT NULL,
    CONSTRAINT [PK_ZNodePublishProduct] PRIMARY KEY CLUSTERED ([PublishProductId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePublishProduct_ZnodePublishCatalog] FOREIGN KEY ([PublishCatalogId]) REFERENCES [dbo].[ZnodePublishCatalog] ([PublishCatalogId])
);




GO
CREATE NONCLUSTERED INDEX [IDX_ZnodePublishProduct_PimProductId_PublishCatalogId]
    ON [dbo].[ZnodePublishProduct]([PimProductId] ASC)
    INCLUDE([PublishCatalogId]);


GO
CREATE NONCLUSTERED INDEX [ID_ZnodePublishProduct_PimProductId]
    ON [dbo].[ZnodePublishProduct]([PimProductId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishProduct_PublishCatalogId]
    ON [dbo].[ZnodePublishProduct]([PublishCatalogId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishProduct_PublishCatalogId]
    ON [dbo].[ZnodePublishProduct]([PublishCatalogId] ASC)
    INCLUDE([PimProductId]);

