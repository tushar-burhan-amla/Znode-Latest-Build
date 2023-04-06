CREATE TABLE [dbo].[ZnodeProfilePromotion] (
    [ProfilePromotionId] INT      IDENTITY (1, 1) NOT NULL,
    [ProfileId]          INT      NOT NULL,
    [PromotionId]        INT      NOT NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeProfilePromotion] PRIMARY KEY CLUSTERED ([ProfilePromotionId] ASC),
    CONSTRAINT [FK_ZnodeProfilePromotion_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId])
);

