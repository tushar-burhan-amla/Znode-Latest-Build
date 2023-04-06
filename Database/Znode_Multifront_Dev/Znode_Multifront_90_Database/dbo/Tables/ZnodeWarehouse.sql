CREATE TABLE [dbo].[ZnodeWarehouse] (
    [WarehouseId]   INT           IDENTITY (1, 1) NOT NULL,
    [WarehouseCode] VARCHAR (100) NULL,
    [WarehouseName] VARCHAR (100) NOT NULL,
    [CreatedBy]     INT           NOT NULL,
    [CreatedDate]   DATETIME      NOT NULL,
    [ModifiedBy]    INT           NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeInventoryWarehouse] PRIMARY KEY CLUSTERED ([WarehouseId] ASC)
);



