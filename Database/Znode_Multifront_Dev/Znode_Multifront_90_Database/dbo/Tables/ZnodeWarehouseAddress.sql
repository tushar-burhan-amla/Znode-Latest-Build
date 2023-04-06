CREATE TABLE [dbo].[ZnodeWarehouseAddress] (
    [WarehouseAddressId] INT      IDENTITY (1, 1) NOT NULL,
    [WarehouseId]        INT      NOT NULL,
    [AddressId]          INT      NOT NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeWarehouseAddress] PRIMARY KEY CLUSTERED ([WarehouseAddressId] ASC),
    CONSTRAINT [FK_ZnodeWareHouseAddress_ZnodeAddress] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[ZnodeAddress] ([AddressId]),
    CONSTRAINT [FK_ZnodeWarehouseAddress_ZnodeWarehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[ZnodeWarehouse] ([WarehouseId])
);





