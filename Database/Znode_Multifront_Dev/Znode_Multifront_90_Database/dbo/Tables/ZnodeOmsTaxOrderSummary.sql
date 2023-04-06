CREATE TABLE [ZnodeOmsTaxOrderSummary] 
(
	[OmsTaxOrderSummaryId] INT IDENTITY,
	[OmsOrderDetailsId] INT,
	[Tax] NUMERIC(28,6),
	[Rate] NUMERIC(28,6),
	[TaxName] VARCHAR(600),
	[TaxTypeName] VARCHAR(600),
	CONSTRAINT [PK_ZnodeOmsTaxOrderSummary] PRIMARY KEY CLUSTERED ([OmsTaxOrderSummaryId]) ,
	CONSTRAINT [FK_ZnodeOmsTaxOrderSummary_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId])
)
GO
