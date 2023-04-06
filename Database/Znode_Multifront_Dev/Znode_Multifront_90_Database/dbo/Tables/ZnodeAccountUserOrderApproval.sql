CREATE TABLE [dbo].[ZnodeAccountUserOrderApproval] (
    [AccountUserOrderApprovalId] INT      IDENTITY (1, 1) NOT NULL,
    [UserId]                     INT      NOT NULL,
    [ApprovalUserId]             INT      NOT NULL,
    [CreatedBy]                  INT      NOT NULL,
    [CreatedDate]                DATETIME NOT NULL,
    [ModifiedBy]                 INT      NOT NULL,
    [ModifiedDate]               DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeAccountUserOrderApproval] PRIMARY KEY CLUSTERED ([AccountUserOrderApprovalId] ASC),
    CONSTRAINT [FK_ZnodeAccountUserOrderApproval_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId]),
    CONSTRAINT [FK_ZnodeAccountUserOrderApproval_ZnodeUser_ApprovalUserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);

