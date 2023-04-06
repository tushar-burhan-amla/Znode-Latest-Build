CREATE TABLE [dbo].[ZnodeOmsHistory] (
    [OmsHistoryId]      INT             IDENTITY (1, 1) NOT NULL,
    [OmsOrderDetailsId] INT             NULL,
    [OmsNotesId]        INT             NULL,
    [TransactionId]     NVARCHAR (200)  NULL,
    [OrderAmount]       NUMERIC (28, 6) NULL,
    [Message]           NVARCHAR (MAX)  NULL,
    [CreatedBy]         INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL,
    [ModifiedBy]        INT             NOT NULL,
    [ModifiedDate]      DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeOmsHistory] PRIMARY KEY CLUSTERED ([OmsHistoryId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsHistory_ZnodeOmsNotes] FOREIGN KEY ([OmsNotesId]) REFERENCES [dbo].[ZnodeOmsNotes] ([OmsNotesId]),
    CONSTRAINT [FK_ZnodeOmsHistory_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId])
);





