CREATE TABLE [dbo].[ZnodeInventoryWarehouse] (
    [InventoryWarehouseId] INT      IDENTITY (1, 1) NOT NULL,
    [WarehouseId]          INT      NOT NULL,
    [InventoryListId]      INT      NOT NULL,
    [IsActive]             BIT      NOT NULL,
    [CreatedBy]            INT      NOT NULL,
    [CreatedDate]          DATETIME NOT NULL,
    [ModifiedBy]           INT      NOT NULL,
    [ModifiedDate]         DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeInventoryWarehouseMapper] PRIMARY KEY CLUSTERED ([InventoryWarehouseId] ASC),
    CONSTRAINT [FK_ZnodeInventoryWarehouseMapper_ZnodeInventoryList] FOREIGN KEY ([InventoryListId]) REFERENCES [dbo].[ZnodeInventoryList] ([InventoryListId]),
    CONSTRAINT [FK_ZnodeInventoryWarehouseMapper_ZnodeInventoryWarehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[ZnodeWarehouse] ([WarehouseId])
);



