CREATE TABLE [dbo].[ZnodePortalPaymentSetting] (
    [PortalPaymentId]    INT             IDENTITY (1, 1) NOT NULL,
    [PortalId]           INT             NULL,
    [PaymentSettingId]   INT             NULL,
    [CreatedBy]          INT             NOT NULL,
    [CreatedDate]        DATETIME        NOT NULL,
    [ModifiedBy]         INT             NOT NULL,
    [ModifiedDate]       DATETIME        NOT NULL,
    [PaymentDisplayName] NVARCHAR (1200) NULL,
    [PaymentExternalId]  NVARCHAR (1000) NULL,
    [IsApprovalRequired] BIT             CONSTRAINT [DF_ZnodePortalPaymentsetting_IsApprovalRequired] DEFAULT ((0)) NOT NULL,
    [IsOABRequired]      BIT             CONSTRAINT [DF_ZnodePortalPaymentsetting_IsOABRequired] DEFAULT ((0)) NOT NULL,
    [PublishStateId]     TINYINT         NULL,
    [IsUsedForWebStorePayment] BIT NULL, 
    [IsUsedForOfflinePayment] BIT NULL, 
    CONSTRAINT [PK_ZnodePortalPayment] PRIMARY KEY CLUSTERED ([PortalPaymentId] ASC),
    CONSTRAINT [FK_ZnodePortalPayment_ZnodePaymentSetting] FOREIGN KEY ([PaymentSettingId]) REFERENCES [dbo].[ZnodePaymentSetting] ([PaymentSettingId]),
    CONSTRAINT [FK_ZnodePortalPayment_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodePortalPaymentSetting_ZnodePaymentSetting] FOREIGN KEY ([PaymentSettingId]) REFERENCES [dbo].[ZnodePaymentSetting] ([PaymentSettingId]),
    CONSTRAINT [FK_ZnodePortalPaymentSetting_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
);









