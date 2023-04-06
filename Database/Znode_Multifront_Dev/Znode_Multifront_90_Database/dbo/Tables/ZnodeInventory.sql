CREATE TABLE [dbo].[ZnodeInventory] (
    [InventoryId]           INT             IDENTITY (1, 1) NOT NULL,
    [WarehouseId]           INT             NULL,
    [SKU]                   VARCHAR (300)   NULL,
    [Quantity]              NUMERIC (28, 6) NOT NULL,
    [ReOrderLevel]          NUMERIC (28, 6) CONSTRAINT [DF_ZnodeInventory_ReOrderLevel] DEFAULT ((0)) NOT NULL,
    [ExternalId]            NVARCHAR (1000) NULL,
    [CreatedBy]             INT             NOT NULL,
    [CreatedDate]           DATETIME        NOT NULL,
    [ModifiedBy]            INT             NOT NULL,
    [ModifiedDate]          DATETIME        NOT NULL,
    [BackOrderQuantity]     NUMERIC (28, 6) CONSTRAINT [DF_ZnodeInventory_BackOrderQuantity] DEFAULT ((0)) NOT NULL,
    [BackOrderExpectedDate] DATETIME        NULL,
    CONSTRAINT [PK_ZnodeInventory] PRIMARY KEY CLUSTERED ([InventoryId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeInventory_ZnodeWarehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[ZnodeWarehouse] ([WarehouseId])
);



















GO
CREATE NONCLUSTERED INDEX [IDX_ZnodeInventory_WarehouseId]
    ON [dbo].[ZnodeInventory]([WarehouseId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodeInventory_WarehouseId]
    ON [dbo].[ZnodeInventory]([WarehouseId] ASC)
    INCLUDE([SKU], [Quantity], [ReOrderLevel]);


GO
CREATE NONCLUSTERED INDEX [Idx_ZnodeInventory_SKU]
    ON [dbo].[ZnodeInventory]([SKU] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_ZnodeInventory_SKU_Quantity]
    ON [dbo].[ZnodeInventory]([WarehouseId] ASC)
    INCLUDE([SKU], [Quantity]);