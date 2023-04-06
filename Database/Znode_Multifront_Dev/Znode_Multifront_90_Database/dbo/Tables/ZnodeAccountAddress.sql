CREATE TABLE [dbo].[ZnodeAccountAddress] (
    [AccountAddressId] INT      IDENTITY (1, 1) NOT NULL,
    [AccountId]        INT      NOT NULL,
    [AddressId]        INT      NOT NULL,
    [CreatedBy]        INT      NOT NULL,
    [CreatedDate]      DATETIME NOT NULL,
    [ModifiedBy]       INT      NOT NULL,
    [ModifiedDate]     DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeAccountAddress] PRIMARY KEY CLUSTERED ([AccountAddressId] ASC),
    CONSTRAINT [FK_ZnodeAccountAddress_ZnodeAccount] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[ZnodeAccount] ([AccountId]),
    CONSTRAINT [FK_ZnodeAccountAddress_ZnodeAddress] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[ZnodeAddress] ([AddressId])
);




GO
CREATE NONCLUSTERED INDEX [IDX_ZnodeAccountAddress_AccountId]
    ON [dbo].[ZnodeAccountAddress]([AccountId] ASC)
    INCLUDE([AddressId]);
Go
CREATE NONCLUSTERED INDEX Ind_ZnodeAccountAddress_AddressId
ON [dbo].[ZnodeAccountAddress] ([AddressId])
INCLUDE ([AccountId])
