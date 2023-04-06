CREATE TABLE [dbo].[ZnodeProductPromotion] (
    [ProductPromotionId]       INT      IDENTITY (1, 1) NOT NULL,
    [PublishProductId]         INT      NULL,
    [PromotionId]              INT      NOT NULL,
    [ReferralPublishProductId] INT      NULL,
    [PromotionProductQunatity] INT      NULL,
    [CreatedBy]                INT      NOT NULL,
    [CreatedDate]              DATETIME NOT NULL,
    [ModifiedBy]               INT      NOT NULL,
    [ModifiedDate]             DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeProductPromotion] PRIMARY KEY CLUSTERED ([ProductPromotionId] ASC),
    CONSTRAINT [FK_ZnodeProductPromotion_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId]),
    CONSTRAINT [FK_ZnodeProductPromotion_ZNodePublishProduct] FOREIGN KEY ([PublishProductId]) REFERENCES [dbo].[ZNodePublishProduct] ([PublishProductId])
);









