CREATE TABLE [dbo].[ZnodeCatalogPromotion] (
    [CatalogPromotionId] INT      IDENTITY (1, 1) NOT NULL,
    [PublishCatalalogId] INT      NULL,
    [PromotionId]        INT      NOT NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeCatalogPromotion] PRIMARY KEY CLUSTERED ([CatalogPromotionId] ASC),
    CONSTRAINT [FK_ZnodeCatalogPromotion_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId]),
    CONSTRAINT [FK_ZnodeCatalogPromotion_ZnodePublishCatalog] FOREIGN KEY ([PublishCatalalogId]) REFERENCES [dbo].[ZnodePublishCatalog] ([PublishCatalogId])
);





