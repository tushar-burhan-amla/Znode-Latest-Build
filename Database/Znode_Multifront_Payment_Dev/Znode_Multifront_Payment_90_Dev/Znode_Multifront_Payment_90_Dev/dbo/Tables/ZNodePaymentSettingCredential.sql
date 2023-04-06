CREATE TABLE [dbo].[ZNodePaymentSettingCredential] (
    [PaymentSettingCredentialId] INT            IDENTITY (1, 1) NOT NULL,
    [Partner]                    NVARCHAR (MAX) NULL,
    [Vendor]                     NVARCHAR (MAX) NULL,
    [TestMode]                   BIT            CONSTRAINT [DF_ZNodePaymentSettingCredential_TestMode] DEFAULT ((0)) NOT NULL,
    [PaymentSettingId]           INT            NULL,
    [GatewayUsername]            NVARCHAR (MAX) NULL,
    [GatewayPassword]            NVARCHAR (MAX) NULL,
    [TransactionKey]             NVARCHAR (MAX) NULL,
    [CreatedDate]                DATETIME       NOT NULL,
    [ModifiedDate]               DATETIME       NOT NULL,
    [Custom1] NVARCHAR(MAX) NULL, 
	[Custom2] NVARCHAR(MAX) NULL, 
	[Custom3] NVARCHAR(MAX) NULL, 
	[Custom4] NVARCHAR(MAX) NULL, 
	[Custom5] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_ZNodePaymentSettingCredential] PRIMARY KEY CLUSTERED ([PaymentSettingCredentialId] ASC),
    CONSTRAINT [FK_ZNodePaymentSettingCredential_ZNodePaymentSetting] FOREIGN KEY ([PaymentSettingId]) REFERENCES [dbo].[ZNodePaymentSetting] ([PaymentSettingId])
);



