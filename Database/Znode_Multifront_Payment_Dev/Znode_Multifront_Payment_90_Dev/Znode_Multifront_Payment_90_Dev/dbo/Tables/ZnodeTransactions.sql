CREATE TABLE [dbo].[ZnodeTransactions] (
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
    [RefundTransactionId]    NVARCHAR (MAX)   NULL,
    [CaptureTransactionId]   NVARCHAR (MAX)   NULL,
    [Amount]                 MONEY            NULL,
    [PaymentSettingId]       INT              NULL,
    [CurrencyCode]           NVARCHAR (50)    NULL,
    [SubscriptionId]         NVARCHAR (MAX)   NULL,
    [PaymentStatusId]        INT              NULL,
    [RefundAmount]           NUMERIC (12, 6)  NULL,
    [CreatedDate]            DATETIME         NOT NULL,
    [ModifiedDate]           DATETIME         NOT NULL,
    CONSTRAINT [PK_Znode_Transactions] PRIMARY KEY CLUSTERED ([GUID] ASC),
    CONSTRAINT [FK_Znode_Transactions_ZNodePaymentSetting] FOREIGN KEY ([PaymentSettingId]) REFERENCES [dbo].[ZNodePaymentSetting] ([PaymentSettingId])
);





