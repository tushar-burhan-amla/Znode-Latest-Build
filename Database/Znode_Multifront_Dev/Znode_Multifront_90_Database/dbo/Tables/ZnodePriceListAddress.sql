CREATE TABLE [dbo].[ZnodePriceListAddress] (
    [PriceListAddressId] INT      IDENTITY (1, 1) NOT NULL,
    [PriceListId]        INT      NOT NULL,
    [AddressId]          INT      NOT NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePriceListAddress] PRIMARY KEY CLUSTERED ([PriceListAddressId] ASC),
    CONSTRAINT [FK_ZnodePriceListAddress_ZnodeAddress] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[ZnodeAddress] ([AddressId]),
    CONSTRAINT [FK_ZnodePriceListAddress_ZnodePriceList] FOREIGN KEY ([PriceListId]) REFERENCES [dbo].[ZnodePriceList] ([PriceListId])
);



