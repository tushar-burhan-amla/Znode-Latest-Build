CREATE TABLE [dbo].[ZnodePromotionCoupon] (
    [PromotionCouponId] INT           IDENTITY (1, 1) NOT NULL,
    [PromotionId]       INT           NOT NULL,
    [Code]              VARCHAR (50)  NOT NULL,
    [InitialQuantity]   INT           NOT NULL,
    [AvailableQuantity] INT           NOT NULL,
    [IsActive]          BIT           NOT NULL,
    [IsCustomCoupon]    BIT           CONSTRAINT [DF_ZnodePromotionCoupon_IsCustomCoupon] DEFAULT ((0)) NOT NULL,
    [CustomCouponCode]  NVARCHAR (50) NULL,
    [CreatedBy]         INT           NOT NULL,
    [CreatedDate]       DATETIME      NOT NULL,
    [ModifiedBy]        INT           NOT NULL,
    [ModifiedDate]      DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePromotionCoupon] PRIMARY KEY CLUSTERED ([PromotionCouponId] ASC),
    CONSTRAINT [FK_ZnodePromotionCoupon_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId])
);











