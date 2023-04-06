CREATE TABLE [dbo].[ZnodeOmsQuoteComment] (
    [OmsQuoteCommentId] INT            IDENTITY (1, 1) NOT NULL,
    [OmsQuoteId]        INT            NULL,
    [Comments]          NVARCHAR (MAX) NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeOmsQuoteComment] PRIMARY KEY CLUSTERED ([OmsQuoteCommentId] ASC),
    CONSTRAINT [FK_ZnodeOmsQuoteComment_ZnodeOmsQuote] FOREIGN KEY ([OmsQuoteId]) REFERENCES [dbo].[ZnodeOmsQuote] ([OmsQuoteId])
);

