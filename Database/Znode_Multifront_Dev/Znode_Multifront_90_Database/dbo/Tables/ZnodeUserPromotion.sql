CREATE TABLE [dbo].[ZnodeUserPromotion] (
    [UserPromotionId] INT      IDENTITY (1, 1) NOT NULL,
    [UserId]          INT      NOT NULL,
    [PromotionId]     INT      NOT NULL,
    [CreatedBy]       INT      NOT NULL,
    [CreatedDate]     DATETIME NOT NULL,
    [ModifiedBy]      INT      NOT NULL,
    [ModifiedDate]    DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeUserPromotion] PRIMARY KEY CLUSTERED ([UserPromotionId] ASC),
    CONSTRAINT [FK_ZnodeUserPromotion_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId]),
    CONSTRAINT [FK_ZnodeUserPromotion_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);



