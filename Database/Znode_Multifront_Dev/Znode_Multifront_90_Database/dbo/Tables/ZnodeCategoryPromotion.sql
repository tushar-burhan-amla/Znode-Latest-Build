CREATE TABLE [dbo].[ZnodeCategoryPromotion] (
    [CategoryPromotionId] INT      IDENTITY (1, 1) NOT NULL,
    [PublishCategoryId]   INT      NULL,
    [PromotionId]         INT      NOT NULL,
    [CreatedBy]           INT      NOT NULL,
    [CreatedDate]         DATETIME NOT NULL,
    [ModifiedBy]          INT      NOT NULL,
    [ModifiedDate]        DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeCategoryPromotion] PRIMARY KEY CLUSTERED ([CategoryPromotionId] ASC),
    CONSTRAINT [FK_ZnodeCategoryPromotion_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId]),
    CONSTRAINT [FK_ZnodeCategoryPromotion_ZnodePublishCategory] FOREIGN KEY ([PublishCategoryId]) REFERENCES [dbo].[ZnodePublishCategory] ([PublishCategoryId])
);





