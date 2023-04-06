CREATE TABLE [dbo].[ZnodePromotionCatalogs] (
    [PromotionCatalogId] INT      IDENTITY (1, 1) NOT NULL,
    [PromotionId]        INT      NOT NULL,
    [PublishCatalogId]   INT      NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifedBy]          INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePromotionCatalog] PRIMARY KEY CLUSTERED ([PromotionCatalogId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePromotionCatalog_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId])
);



