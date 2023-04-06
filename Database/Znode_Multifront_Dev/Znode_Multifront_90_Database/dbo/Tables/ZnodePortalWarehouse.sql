CREATE TABLE [dbo].[ZnodePortalWarehouse] (
    [PortalWarehouseId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]          INT      NOT NULL,
    [WarehouseId]       INT      NOT NULL,
    [Precedence]        INT      NULL,
    [CreatedBy]         INT      NOT NULL,
    [CreatedDate]       DATETIME NOT NULL,
    [ModifiedBy]        INT      NOT NULL,
    [ModifiedDate]      DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeInventoryPortalWarehouse] PRIMARY KEY CLUSTERED ([PortalWarehouseId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePortalWarehouse_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodePortalWarehouse_ZnodeWareHouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[ZnodeWarehouse] ([WarehouseId])
);









