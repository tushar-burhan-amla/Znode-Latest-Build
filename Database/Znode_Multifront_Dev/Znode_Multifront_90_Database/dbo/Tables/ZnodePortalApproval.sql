CREATE TABLE [dbo].[ZnodePortalApproval] (
    [PortalApprovalId]         INT             IDENTITY (1, 1) NOT NULL,
    [EnableApprovalManagement] BIT             NOT NULL,
    [PortalApprovalTypeId]     INT             NOT NULL,
    [PortalApprovalLevelId]    INT             NOT NULL,
    [OrderLimit]               NUMERIC (28, 8) NOT NULL,
    [PortalId]                 INT             NOT NULL,
    [CreatedBy]                INT             NOT NULL,
    [CreatedDate]              DATETIME        NOT NULL,
    [ModifiedBy]               INT             NOT NULL,
    [ModifiedDate]             DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePortalApproval] PRIMARY KEY CLUSTERED ([PortalApprovalId] ASC),
    CONSTRAINT [FK_ZnodePortalApproval_ZnodePortalApprovalLevel] FOREIGN KEY ([PortalApprovalLevelId]) REFERENCES [dbo].[ZnodePortalApprovalLevel] ([PortalApprovalLevelId]),
    CONSTRAINT [FK_ZnodePortalApproval_ZnodePortalApprovalType] FOREIGN KEY ([PortalApprovalTypeId]) REFERENCES [dbo].[ZnodePortalApprovalType] ([PortalApprovalTypeId])
);

