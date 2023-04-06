CREATE TABLE [dbo].[ZnodeOrderPayment]
(
[OrderPaymentId]        INT             IDENTITY (1, 1) NOT NULL,
[OmsOrderId]        INT             NULL,
[TransactionReference]        NVARCHAR (50)   NULL,
[Amount]              NUMERIC(28, 6)   NULL,
[TransactionStatus]            NVARCHAR (50)   NULL,
[TransactionDate]            DATETIME   NULL,
[CreatedBy] INT NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[ModifiedBy] INT NOT NULL,
[ModifiedDate] [datetime] NOT NULL,
[PaymentSettingId]           INT    NULL,
[RemainingOrderAmount] NUMERIC(28, 6) NULL, 
    CONSTRAINT [PK_ZnodeOrderPayment] PRIMARY KEY CLUSTERED ([OrderPaymentId] ASC) WITH (FILLFACTOR = 90),
CONSTRAINT [FK_ZnodeOrderPayment_ZnodeOMSOrder] FOREIGN KEY ([OmsOrderId]) REFERENCES [dbo].[ZnodeOMSOrder] ([OMSOrderId]),
CONSTRAINT [FK_ZnodeOrderPayment_ZnodePaymentSetting] FOREIGN KEY ([PaymentSettingId]) REFERENCES [dbo].[ZnodePaymentSetting] ([PaymentSettingId])
)
