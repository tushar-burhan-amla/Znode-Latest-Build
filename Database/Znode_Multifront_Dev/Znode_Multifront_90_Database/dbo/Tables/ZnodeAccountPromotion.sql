CREATE TABLE [dbo].[ZnodeAccountPromotion] (
    [AccountPromotionId] INT      IDENTITY (1, 1) NOT NULL,
    [AccountId]          INT      NOT NULL,
    [PromotionId]        INT      NOT NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeAccountPromotion] PRIMARY KEY CLUSTERED ([AccountPromotionId] ASC),
    CONSTRAINT [FK_ZnodeAccountPromotion_ZnodeAccount] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[ZnodeAccount] ([AccountId]),
    CONSTRAINT [FK_ZnodeAccountPromotion_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId])
);



