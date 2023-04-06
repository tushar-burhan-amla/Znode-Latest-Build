CREATE TABLE [dbo].[ZnodeOmsQuoteTaxOrderDetails] (
    [OmsQuoteTaxOrderDetailsId] INT             IDENTITY (1, 1) NOT NULL,
    [OmsQuoteId]    INT             NOT NULL,
    [SalesTax]             NUMERIC (28, 6) NULL,
    [VAT]                  NUMERIC (28, 6) NULL,
    [GST]                  NUMERIC (28, 6) NULL,
    [PST]                  NUMERIC (28, 6) NULL,
    [HST]                  NUMERIC (28, 6) NULL,
    [CreatedBy]            INT             NOT NULL,
    [CreatedDate]          DATETIME        NOT NULL,
    [ModifiedBy]           INT             NOT NULL,
    [ModifiedDate]         DATETIME        NOT NULL,
    [ImportDuty] NUMERIC(28,6) NULL,
    CONSTRAINT [PK_ZnodeOmsQuoteTaxOrderDetails] PRIMARY KEY CLUSTERED ([OmsQuoteTaxOrderDetailsId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsQuoteTaxOrderDetails_ZnodeOmsQuote] 
	FOREIGN KEY ([OmsQuoteId]) REFERENCES [dbo].[ZnodeOmsQuote] ([OmsQuoteId])
);





