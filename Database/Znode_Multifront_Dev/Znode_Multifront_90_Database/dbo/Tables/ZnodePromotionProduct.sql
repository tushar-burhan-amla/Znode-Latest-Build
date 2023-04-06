CREATE TABLE [dbo].[ZnodePromotionProduct] (
    [PromotionProductId] INT      IDENTITY (1, 1) NOT NULL,
    [PromotionId]        INT      NOT NULL,
    [PublishProductId]   INT      NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifedBy]          INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePromotionProduct] PRIMARY KEY CLUSTERED ([PromotionProductId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePromotionProduct_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId])
);



