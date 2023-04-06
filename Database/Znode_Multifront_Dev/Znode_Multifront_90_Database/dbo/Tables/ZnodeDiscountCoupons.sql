CREATE TABLE [dbo].[ZnodeDiscountCoupons] (
    [DiscountCouponId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (100) NULL,
    [CouponNumber]     NVARCHAR (40)  NOT NULL,
    [ExpirationDate]   DATETIME       NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeDiscountCoupons] PRIMARY KEY CLUSTERED ([DiscountCouponId] ASC)
);

