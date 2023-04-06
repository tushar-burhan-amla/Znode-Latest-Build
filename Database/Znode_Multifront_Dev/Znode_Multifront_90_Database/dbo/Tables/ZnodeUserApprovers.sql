CREATE TABLE [dbo].[ZnodeUserApprovers] (
    [UserApproverId]       INT             IDENTITY (1, 1) NOT NULL,
    [UserId]               INT             NULL,
    [ApproverLevelId]      INT             NULL,
    [ApproverUserId]       INT             NOT NULL,
    [ApproverOrder]        INT             NOT NULL,
    [IsNotifyEmail]        BIT             CONSTRAINT [DF_ZnodeUserApprovers_IsNotifyEmail] DEFAULT ((1)) NOT NULL,
    [IsMandatory]          BIT             CONSTRAINT [DF_ZnodeUserApprovers_IsMandatory] DEFAULT ((0)) NOT NULL,
    [Custom1]              NVARCHAR (MAX)  NULL,
    [Custom2]              NVARCHAR (MAX)  NULL,
    [Custom3]              NVARCHAR (MAX)  NULL,
    [Custom4]              NVARCHAR (MAX)  NULL,
    [Custom5]              NVARCHAR (MAX)  NULL,
    [CreatedBy]            INT             NOT NULL,
    [CreatedDate]          DATETIME        NOT NULL,
    [ModifiedBy]           INT             NOT NULL,
    [ModifiedDate]         DATETIME        NOT NULL,
    [FromBudgetAmount]     NUMERIC (28, 8) NOT NULL,
    [ToBudgetAmount]       NUMERIC (28, 8) NULL,
    [IsNoLimit]            BIT             CONSTRAINT [DF_znodeuserapprovers_IsNoLimit] DEFAULT ((0)) NULL,
    [IsActive]             BIT             CONSTRAINT [DF_ZnodeUserApprovers_IsActive] DEFAULT ((1)) NOT NULL,
    [PortalApprovalId]     INT             NULL,
    [PortalPaymentGroupId] INT             NULL,
    CONSTRAINT [PK_ZnodeUserApprovers] PRIMARY KEY CLUSTERED ([UserApproverId] ASC),
    CONSTRAINT [FK_ZnodeUserApprovers_ZnodeApproverLevel] FOREIGN KEY ([ApproverLevelId]) REFERENCES [dbo].[ZnodeApproverLevel] ([ApproverLevelId]),
    CONSTRAINT [FK_ZnodeUserApprovers_ZnodePortalApproval] FOREIGN KEY ([PortalApprovalId]) REFERENCES [dbo].[ZnodePortalApproval] ([PortalApprovalId]),
    CONSTRAINT [FK_ZnodeUserApprovers_ZnodePortalPaymentGroup] FOREIGN KEY ([PortalPaymentGroupId]) REFERENCES [dbo].[ZnodePortalPaymentGroup] ([PortalPaymentGroupId]),
    CONSTRAINT [FK_ZnodeUserApprovers_ZnodeUser_ApproverUserId] FOREIGN KEY ([ApproverUserId]) REFERENCES [dbo].[ZnodeUser] ([UserId]),
    CONSTRAINT [FK_ZnodeUserApprovers_ZnodeUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);



