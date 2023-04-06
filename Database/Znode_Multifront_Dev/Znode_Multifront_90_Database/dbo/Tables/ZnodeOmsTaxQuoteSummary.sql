CREATE TABLE [ZnodeOmsTaxQuoteSummary] 
(
	[OmsTaxQuoteSummaryId] INT IDENTITY,
	[OmsQuoteId] INT,
	[Tax] NUMERIC(28,6),
	[Rate] NUMERIC(28,6),
	[TaxName] VARCHAR(600),
	[TaxTypeName] VARCHAR(600),
	CONSTRAINT [PK_ZnodeOmsTaxQuoteSummary] PRIMARY KEY CLUSTERED ([OmsTaxQuoteSummaryId]) ,
	CONSTRAINT [FK_ZnodeOmsTaxQuoteSummary_ZnodeOmsQuote] FOREIGN KEY ([OmsQuoteId]) REFERENCES [dbo].[ZnodeOmsQuote] ([OmsQuoteId])
)
GO