CREATE TABLE [dbo].[Znode_Transactions] (
    [GUID]                   UNIQUEIDENTIFIER NOT NULL,
    [CustomerProfileId]      NVARCHAR (MAX)   NULL,
    [CustomerPaymentId]      NVARCHAR (MAX)   NULL,
    [TransactionId]          NVARCHAR (MAX)   NULL,
    [TransactionDate]        DATETIME         NULL,
    [CaptureTransactionDate] DATETIME         NULL,
    [RefundTransactionDate]  DATETIME         NULL,
    [ResponseText]           NVARCHAR (MAX)   NULL,
    [ResponseCode]           NVARCHAR (50)    NULL,
    [Custom1]                NVARCHAR (MAX)   NULL,
    [Custom2]                NVARCHAR (MAX)   NULL,
    [Custom3]                NVARCHAR (MAX)   NULL,
    [Amount]                 MONEY            NULL,
    [PaymentSettingId]       INT              NULL,
    [CurrencyCode]           NVARCHAR (50)    NULL,
    [SubscriptionId]         NVARCHAR (MAX)   NULL,
    [PaymentStatusId]        INT              NULL,
    CONSTRAINT [PK_Znode_Transactions] PRIMARY KEY CLUSTERED ([GUID] ASC),
    CONSTRAINT [FK_Znode_Transactions_ZNodePaymentSetting] FOREIGN KEY ([PaymentSettingId]) REFERENCES [dbo].[ZNodePaymentSetting] ([PaymentSettingID])
);

