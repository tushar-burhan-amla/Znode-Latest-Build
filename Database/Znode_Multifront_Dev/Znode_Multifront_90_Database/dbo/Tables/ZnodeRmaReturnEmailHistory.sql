CREATE TABLE [dbo].[ZnodeRmaReturnEmailHistory] (
    [RmaReturnEmailHistoryId] INT      IDENTITY (1, 1) NOT NULL,
    [RmaReturnDetailsId]       INT      NOT NULL,
    [SentDate]                 DATETIME NOT NULL,
    [IsSuccess]                BIT      NOT NULL,
    [CreatedBy]                INT      NOT NULL,
    [CreatedDate]              DATETIME NOT NULL,
    [ModifiedBy]               INT      NOT NULL,
    [ModifiedDate]             DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeRmaReturnEmailHistory] PRIMARY KEY CLUSTERED ([RmaReturnEmailHistoryId] ASC),
    CONSTRAINT [FK_ZnodeRmaReturnEmailHistory_ZnodeRmaReturnDetails] FOREIGN KEY ([RmaReturnDetailsId]) REFERENCES [dbo].[ZnodeRmaReturnDetails] ([RmaReturnDetailsId])
);

