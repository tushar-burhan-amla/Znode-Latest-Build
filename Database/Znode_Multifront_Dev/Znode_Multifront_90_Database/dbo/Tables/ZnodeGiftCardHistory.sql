CREATE TABLE [dbo].[ZnodeGiftCardHistory] (
    [GiftCardHistoryId] INT             IDENTITY (1, 1) NOT NULL,
    [GiftCardId]        INT             NOT NULL,
    [TransactionDate]   DATETIME        NOT NULL,
    [TransactionAmount] NUMERIC (28, 6) NOT NULL,
    [OmsOrderDetailsId] INT             NULL,
    [CreatedBy]         INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL,
    [ModifiedBy]        INT             NOT NULL,
    [ModifiedDate]      DATETIME        NOT NULL,
	[Notes]             NVARCHAR (MAX)  NULL,
    [RemainingAmount]      NUMERIC(26,6),
    CONSTRAINT [PK_ZnodeGiftCardHistory] PRIMARY KEY CLUSTERED ([GiftCardHistoryId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeGiftCardHistory_ZnodeGiftCard] FOREIGN KEY ([GiftCardId]) REFERENCES [dbo].[ZnodeGiftCard] ([GiftCardId]),
    CONSTRAINT [FK_ZnodeGiftCardHistory_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId])
);



