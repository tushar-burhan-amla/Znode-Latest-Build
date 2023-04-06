CREATE TABLE [dbo].[ZnodeOmsOrderLineItemsShipping] (
    [OmsOrderLineItemsShippingId] INT             IDENTITY (1, 1) NOT NULL,
    [OmsOrderId]                  INT             NULL,
    [OmsOrderLineItemsId]         INT             NULL,
    [OrderLineItemStateId]        INT             NULL,
    [Quantity]                    NUMERIC (28, 6) NULL,
    [ShippingId]                  INT             NULL,
    [ShippingCost]                NUMERIC (28, 6) NULL,
    [TrackingNumber]              VARCHAR (MAX)   NULL,
    [TaxCost]                     NUMERIC (28, 6) NULL,
    [Price]                       NUMERIC (28, 6) NULL,
    [CreatedBy]                   INT             NOT NULL,
    [CreatedDate]                 DATETIME        NOT NULL,
    [ModifiedBy]                  INT             NOT NULL,
    [ModifiedDate]                DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeOmsOrderLineItemsShipping] PRIMARY KEY CLUSTERED ([OmsOrderLineItemsShippingId] ASC),
    CONSTRAINT [FK_ZnodeOmsOrderLineItemsShipping_ZnodeOmsOrder] FOREIGN KEY ([OmsOrderId]) REFERENCES [dbo].[ZnodeOmsOrder] ([OmsOrderId]),
    CONSTRAINT [FK_ZnodeOmsOrderLineItemsShipping_ZnodeOmsOrderLineItems] FOREIGN KEY ([OmsOrderLineItemsId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId]),
    CONSTRAINT [FK_ZnodeOmsOrderLineItemsShipping_ZnodeOmsOrderState] FOREIGN KEY ([OrderLineItemStateId]) REFERENCES [dbo].[ZnodeOmsOrderState] ([OmsOrderStateId])
);



