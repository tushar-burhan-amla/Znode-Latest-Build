CREATE TABLE [dbo].[ZnodePaymentMethods] (
    [PaymentGUID]             UNIQUEIDENTIFIER NOT NULL,
    [Token]                   VARCHAR (MAX)    NULL,
    [CreditCardLastFourDigit] VARCHAR (4)      NULL,
    [CreditCardExpMonth]      INT              NULL,
    [CreditCardExpYear]       INT              NULL,
    [CreditCardAddressId]     UNIQUEIDENTIFIER NULL,
    [PaymentSettingID]        INT              NULL,
    [CustomersGUID]           UNIQUEIDENTIFIER NULL,
    [CustomerProfileId]       NVARCHAR (64)    NULL,
    [CreditCardImageUrl]      NVARCHAR (200)   NULL,
    [IsSaveCreditCard]        BIT              CONSTRAINT [DF_ZnodePaymentMethods] DEFAULT ((0)) NOT NULL,
    [CardType]                VARCHAR (50)     NULL,
    [CreatedDate]             DATETIME         NOT NULL,
    [ModifiedDate]            DATETIME         NOT NULL,
    [UserId] INT NULL, 
    CONSTRAINT [PK_ZnodePaymentMethods] PRIMARY KEY CLUSTERED ([PaymentGUID] ASC),
    CONSTRAINT [FK_ZnodePaymentMethods_CreditCardAddressId] FOREIGN KEY ([CreditCardAddressId]) REFERENCES [dbo].[ZnodePaymentAddress] ([CreditCardAddressId]),
    CONSTRAINT [FK_ZnodePaymentMethods_CustomersGUID] FOREIGN KEY ([CustomersGUID]) REFERENCES [dbo].[ZnodePaymentCustomers] ([CustomersGUID]),
    CONSTRAINT [FK_ZnodePaymentMethods_PaymentSettingID] FOREIGN KEY ([PaymentSettingID]) REFERENCES [dbo].[ZNodePaymentSetting] ([PaymentSettingId])
);





