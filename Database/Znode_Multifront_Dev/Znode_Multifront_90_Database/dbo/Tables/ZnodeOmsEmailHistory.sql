CREATE TABLE [dbo].[ZnodeOmsEmailHistory] (
    [OmsEmailHistoryId] INT      IDENTITY (1, 1) NOT NULL,
    [OmsOrderDetailsId] INT      NOT NULL,
    [SentDate]          DATETIME NOT NULL,
    [IsSuccess]         BIT      CONSTRAINT [DF_ZnodeOmsEmailHistory_IsSuccess] DEFAULT ((0)) NOT NULL,
    [CreatedBy]         INT      NOT NULL,
    [CreatedDate]       DATETIME NOT NULL,
    [ModifiedBy]        INT      NOT NULL,
    [ModifiedDate]      DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeOmsEmailHistory] PRIMARY KEY CLUSTERED ([OmsEmailHistoryId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsEmailHistory_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId])
);



