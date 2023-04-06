CREATE TABLE [dbo].[ZnodeOmsOrderWarehouse] (
    [OmsOrderWarehouseId] INT          IDENTITY (1, 1) NOT NULL,
    [OmsOrderDetailsId]   INT          NULL,
    [OmsOrderLineItemsId] INT          NULL,
    [WarehouseId]         INT          NULL,
    [Quantity]            NUMERIC (13) NULL,
    [CreatedBy]           INT          NOT NULL,
    [CreatedDate]         DATETIME     NOT NULL,
    [ModifiedBy]          INT          NOT NULL,
    [ModifiedDate]        DATETIME     NOT NULL,
    CONSTRAINT [PK_ZnodeOmsOrderWarehouse] PRIMARY KEY CLUSTERED ([OmsOrderWarehouseId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsOrderWarehouse_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId]),
    CONSTRAINT [FK_ZnodeOmsOrderWarehouse_ZnodeOmsOrderLineItems] FOREIGN KEY ([OmsOrderLineItemsId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId]),
    CONSTRAINT [FK_ZnodeOmsOrderWarehouse_ZnodeWarehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[ZnodeWarehouse] ([WarehouseId])
);





