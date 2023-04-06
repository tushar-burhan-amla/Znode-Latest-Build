CREATE TABLE [dbo].[ZnodeOmsNotes] (
    [OmsNotesId]        INT            IDENTITY (1, 1) NOT NULL,
    [OmsOrderDetailsId] INT            NULL,
    [OmsQuoteId]        INT            NULL,
    [Notes]             NVARCHAR (MAX) NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeOmsOrderNotes] PRIMARY KEY CLUSTERED ([OmsNotesId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsNotes_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId]),
    CONSTRAINT [FK_ZnodeOmsNotes_ZnodeOmsQuote] FOREIGN KEY ([OmsQuoteId]) REFERENCES [dbo].[ZnodeOmsQuote] ([OmsQuoteId])
);





