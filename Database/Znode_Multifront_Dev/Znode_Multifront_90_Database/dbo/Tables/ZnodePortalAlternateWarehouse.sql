CREATE TABLE [dbo].[ZnodePortalAlternateWarehouse] (
    [PortalAlternateWarehouseId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalWarehouseId]          INT      NOT NULL,
    [WarehouseId]                INT      NOT NULL,
    [Precedence]                 INT      NULL,
    [CreatedBy]                  INT      NOT NULL,
    [CreatedDate]                DATETIME NOT NULL,
    [ModifiedBy]                 INT      NOT NULL,
    [ModifiedDate]               DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePortalAlternateWarehouse_1] PRIMARY KEY CLUSTERED ([PortalAlternateWarehouseId] ASC),
    CONSTRAINT [FK_ZnodePortalAlternateWarehouse_ZnodePortalWarehouse] FOREIGN KEY ([PortalWarehouseId]) REFERENCES [dbo].[ZnodePortalWarehouse] ([PortalWarehouseId]),
    CONSTRAINT [FK_ZnodePortalAlternateWarehouse_ZnodeWarehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[ZnodeWarehouse] ([WarehouseId])
);



