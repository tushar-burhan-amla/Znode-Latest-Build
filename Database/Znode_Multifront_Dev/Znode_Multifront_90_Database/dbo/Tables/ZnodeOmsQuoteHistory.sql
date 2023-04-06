CREATE TABLE [dbo].[ZnodeOmsQuoteHistory](
	[OmsQuoteHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[OMSQuoteId] [int] NULL,
	[OrderAmount] [numeric](28, 6) NULL,
	[Message] [nvarchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
   CONSTRAINT [PK_ZnodeOmsQuoteHistory] PRIMARY KEY CLUSTERED([OmsQuoteHistoryId] ASC) WITH (FILLFACTOR = 90),
   CONSTRAINT [FK_ZnodeOmsQuoteHistory_ZnodeOmsQuote] FOREIGN KEY([OMSQuoteId])REFERENCES [dbo].[ZnodeOmsQuote] ([OmsQuoteId])
);