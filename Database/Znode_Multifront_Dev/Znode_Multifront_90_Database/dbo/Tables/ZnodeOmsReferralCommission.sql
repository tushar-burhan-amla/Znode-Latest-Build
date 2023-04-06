CREATE TABLE [dbo].[ZnodeOmsReferralCommission] (
    [OmsReferralCommissionId]  INT             IDENTITY (1, 1) NOT NULL,
    [UserId]                   INT             NOT NULL,
    [OmsOrderDetailsId]        INT             NOT NULL,
    [OrderCommission]          NUMERIC (28, 6) NOT NULL,
    [TransactionId]            NVARCHAR (200)  NULL,
    [Description]              NVARCHAR (MAX)  NULL,
    [ReferralCommission]       NUMERIC (28, 6) NOT NULL,
    [ReferralCommissionTypeId] INT             NOT NULL,
    [CreatedBy]                INT             NOT NULL,
    [CreatedDate]              DATETIME        NOT NULL,
    [ModifiedBy]               INT             NOT NULL,
    [ModifiedDate]             DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeOmsReferralCommission] PRIMARY KEY CLUSTERED ([OmsReferralCommissionId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsReferralCommission_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId]),
    CONSTRAINT [FK_ZnodeOmsReferralCommission_ZnodeReferralCommissionType] FOREIGN KEY ([ReferralCommissionTypeId]) REFERENCES [dbo].[ZnodeReferralCommissionType] ([ReferralCommissionTypeId]),
    CONSTRAINT [FK_ZnodeOmsReferralCommission_ZnodeUser1] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);









