CREATE TABLE [dbo].[ZnodeRmaReturnHistory] (
    [RmaReturnHistoryId] INT             IDENTITY (1, 1) NOT NULL,
    [RmaReturnDetailsId] INT             NOT NULL,
    [TransactionId]      NVARCHAR (200)  NULL,
    [ReturnAmount]       NUMERIC (28, 6) NULL,
    [Message]            NVARCHAR (MAX)  NULL,
    [CreatedBy]          INT             NOT NULL,
    [CreatedDate]        DATETIME        NOT NULL,
    [ModifiedBy]         INT             NOT NULL,
    [ModifiedDate]       DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeRmaReturnHistory] PRIMARY KEY CLUSTERED ([RmaReturnHistoryId] ASC),
    CONSTRAINT [FK_ZnodeRmaReturnHistory_ZnodeRmaReturnDetails] FOREIGN KEY ([RmaReturnDetailsId]) REFERENCES [dbo].[ZnodeRmaReturnDetails] ([RmaReturnDetailsId])
);

