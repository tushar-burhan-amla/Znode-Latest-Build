CREATE TABLE [dbo].[ZnodeOMSQuoteApproval] (
    [OmsQuoteApprovalId]        INT            IDENTITY (1, 1) NOT NULL,
    [OmsQuoteId]                INT            NOT NULL,
    [ApproverLevelId]           INT            NOT NULL,
    [ApproverUserId]            INT            NOT NULL,
    [OmsOrderStateId]           INT            NULL,
    [UserId]                    INT            NOT NULL,
    [IsApprovalRoutingComplete] BIT            CONSTRAINT [DF_ZnodeOMSQuoteApproval_IsApprovalRoutingComplete] DEFAULT ((0)) NOT NULL,
    [Comments]                  NVARCHAR (MAX) NULL,
    [Custom1]                   NVARCHAR (MAX) NULL,
    [Custom2]                   NVARCHAR (MAX) NULL,
    [Custom3]                   NVARCHAR (MAX) NULL,
    [Custom4]                   NVARCHAR (MAX) NULL,
    [Custom5]                   NVARCHAR (MAX) NULL,
    [CreatedBy]                 INT            NOT NULL,
    [CreatedDate]               DATETIME       NOT NULL,
    [ModifiedBy]                INT            NOT NULL,
    [ModifiedDate]              DATETIME       NOT NULL,
    [ApproverOrder]             INT            NULL,
    [OmsQuoteCommentId]         INT            NULL,
    CONSTRAINT [PK_ZnodeOMSQuoteApproval] PRIMARY KEY CLUSTERED ([OmsQuoteApprovalId] ASC),
    CONSTRAINT [FK_ZnodeOMSQuoteApproval_ZnodeApproverLevel] FOREIGN KEY ([ApproverLevelId]) REFERENCES [dbo].[ZnodeApproverLevel] ([ApproverLevelId]),
    CONSTRAINT [FK_ZnodeOMSQuoteApproval_ZnodeOmsQuote] FOREIGN KEY ([OmsQuoteId]) REFERENCES [dbo].[ZnodeOmsQuote] ([OmsQuoteId]),
    CONSTRAINT [FK_ZnodeOMSQuoteApproval_ZnodeOmsQuoteComment] FOREIGN KEY ([OmsQuoteCommentId]) REFERENCES [dbo].[ZnodeOmsQuoteComment] ([OmsQuoteCommentId]),
    CONSTRAINT [FK_ZnodeOMSQuoteApproval_ZnodeUser_ApproverUserId] FOREIGN KEY ([ApproverUserId]) REFERENCES [dbo].[ZnodeUser] ([UserId]),
    CONSTRAINT [FK_ZnodeOMSQuoteApproval_ZnodeUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);




