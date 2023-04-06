CREATE TABLE [dbo].[ZnodePaymentAddress] (
    [CreditCardAddressId] UNIQUEIDENTIFIER NOT NULL,
    [CardHolderFirstName] VARCHAR (60)     NULL,
    [CardHolderLastName]  VARCHAR (60)     NULL,
    [AddressLine1]        VARCHAR (200)    NULL,
    [AddressLine2]        VARCHAR (200)    NULL,
    [City]                VARCHAR (100)    NULL,
    [State]               VARCHAR (100)    NULL,
    [Country]             VARCHAR (100)    NULL,
    [ZipCode]             VARCHAR (100)    NULL,
    [CreatedDate]         DATETIME         NOT NULL,
    [ModifiedDate]        DATETIME         NOT NULL,
    CONSTRAINT [PK_ZnodePaymentAddress] PRIMARY KEY CLUSTERED ([CreditCardAddressId] ASC)
);



