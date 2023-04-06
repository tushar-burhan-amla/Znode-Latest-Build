CREATE TABLE [dbo].[ZnodePortalPaymentGroup] (
    [PortalPaymentGroupId] INT            IDENTITY (1, 1) NOT NULL,
    [PaymentGroupCode]     NVARCHAR (500) NULL,
    [PortalApprovalId]     INT            NOT NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    [isActive]             BIT            NULL,
    CONSTRAINT [PK_ZnodePortalPaymentGroup] PRIMARY KEY CLUSTERED ([PortalPaymentGroupId] ASC),
    CONSTRAINT [FK_ZnodePortalPaymentGroup_ZnodePortalApproval] FOREIGN KEY ([PortalApprovalId]) REFERENCES [dbo].[ZnodePortalApproval] ([PortalApprovalId])
);

