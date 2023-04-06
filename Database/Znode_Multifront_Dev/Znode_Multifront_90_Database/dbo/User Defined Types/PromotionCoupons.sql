CREATE TYPE [dbo].[PromotionCoupons] AS TABLE(
	[Code] [varchar](50) NULL,
	[IsExistInOrder] [bit] NULL,
	[OmsOrderId] [int] NULL
)
GO