CREATE TABLE [dbo].[ZnodePortalPromotion] (
    [PortalPromotionId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]          INT      NOT NULL,
    [PromotionId]       INT      NOT NULL,
    [CreatedBy]         INT      NOT NULL,
    [CreatedDate]       DATETIME NOT NULL,
    [ModifiedBy]        INT      NOT NULL,
    [ModifiedDate]      DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePortalPromotion] PRIMARY KEY CLUSTERED ([PortalPromotionId] ASC),
    CONSTRAINT [FK_ZnodePortalPromotion_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId])
);

