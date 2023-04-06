CREATE TABLE [dbo].[ZnodePromotionCategory] (
    [PromotionCategoryId] INT      IDENTITY (1, 1) NOT NULL,
    [PromotionId]         INT      NOT NULL,
    [PublishCategoryId]   INT      NULL,
    [CreatedBy]           INT      NOT NULL,
    [CreatedDate]         DATETIME NOT NULL,
    [ModifiedBy]          INT      NOT NULL,
    [ModifiedDate]        DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePromotionCategory] PRIMARY KEY CLUSTERED ([PromotionCategoryId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePromotionCategory_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId])
);





