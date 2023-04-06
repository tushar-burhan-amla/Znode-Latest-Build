CREATE TABLE [dbo].[ZnodePromotionShipping] (
    [PromotionShippingId] INT      IDENTITY (1, 1) NOT NULL,
    [PromotionId]         INT      NOT NULL,
    [ShippingId]          INT      NULL,
    [CreatedBy]           INT      NOT NULL,
    [CreatedDate]         DATETIME NOT NULL,
    [ModifedBy]           INT      NOT NULL,
    [ModifiedDate]        DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePromotionShipping] PRIMARY KEY CLUSTERED ([PromotionShippingId] ASC),
    CONSTRAINT [FK_ZnodePromotionShipping_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId]),
    CONSTRAINT [FK_ZnodePromotionShipping_ZnodeShipping] FOREIGN KEY ([ShippingId]) REFERENCES [dbo].[ZnodeShipping] ([ShippingId])
);

