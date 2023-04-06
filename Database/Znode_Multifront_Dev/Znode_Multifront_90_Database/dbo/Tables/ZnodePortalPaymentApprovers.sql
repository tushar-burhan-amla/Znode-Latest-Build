CREATE TABLE [dbo].[ZnodePortalPaymentApprovers] (
    [PortalPaymentApprovalId] INT      IDENTITY (1, 1) NOT NULL,
    [PaymentSettingId]        INT      NOT NULL,
    [PortalPaymentGroupId]    INT      NOT NULL,
    [CreatedBy]               INT      NOT NULL,
    [CreatedDate]             DATETIME NOT NULL,
    [ModifiedBy]              INT      NOT NULL,
    [ModifiedDate]            DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePortalPaymentApprovers] PRIMARY KEY CLUSTERED ([PortalPaymentApprovalId] ASC),
    CONSTRAINT [FK_ZnodePortalPaymentApprovers_ZnodePortalPaymentGroup] FOREIGN KEY ([PortalPaymentGroupId]) REFERENCES [dbo].[ZnodePortalPaymentGroup] ([PortalPaymentGroupId])
);

