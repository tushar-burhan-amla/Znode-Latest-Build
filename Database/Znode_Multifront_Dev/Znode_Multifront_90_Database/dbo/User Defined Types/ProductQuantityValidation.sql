CREATE TYPE [dbo].[ProductQuantityValidation] AS TABLE(
	[SKU] [varchar](600) NULL,
	[MinimumQuantity] [numeric](28, 6) NULL,
	[MaximumQuantity] [numeric](28, 6) NULL,
	[Quantity] [numeric](28, 6) NULL,
	[IsEditCart] BIT NULL
)