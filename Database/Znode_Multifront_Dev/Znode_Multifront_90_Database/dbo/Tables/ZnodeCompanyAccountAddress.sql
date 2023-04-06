CREATE TABLE [dbo].[ZnodeCompanyAccountAddress] (
    [CompanyAccountAddressId] INT      IDENTITY (1, 1) NOT NULL,
    [CompanyAccountId]        INT      NOT NULL,
    [AddressId]               INT      NOT NULL,
    [CreatedBy]               INT      NOT NULL,
    [CreatedDate]             DATETIME NOT NULL,
    [ModifiedBy]              INT      NOT NULL,
    [ModifiedDate]            DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeCompanyAccountAddress] PRIMARY KEY CLUSTERED ([CompanyAccountAddressId] ASC),
    CONSTRAINT [FK_ZnodeCompanyAccountAddress_ZnodeAddress] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[ZnodeAddress] ([AddressId]),
    CONSTRAINT [FK_ZnodeCompanyAccountAddress_ZnodeCompanyAccount] FOREIGN KEY ([CompanyAccountId]) REFERENCES [dbo].[ZnodeCompanyAccount] ([CompanyAccountId])
);

