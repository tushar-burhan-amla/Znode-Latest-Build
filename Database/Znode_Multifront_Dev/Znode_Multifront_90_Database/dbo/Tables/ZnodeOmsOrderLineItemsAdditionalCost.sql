CREATE TABLE [dbo].[ZnodeOmsOrderLineItemsAdditionalCost] (
    [OmsOrderLineItemsAdditionalCostId] INT            IDENTITY (1, 1) NOT NULL,
    [OmsOrderLineItemsId]               INT            NULL,
    [KeyName]                           NVARCHAR (300) NOT NULL,
    [KeyValue]                          NUMERIC (18)   NOT NULL,
    [CreatedBy]                         INT            NOT NULL,
    [CreatedDate]                       DATETIME       NOT NULL,
    [ModifiedBy]                        INT            NOT NULL,
    [ModifiedDate]                      DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([OmsOrderLineItemsAdditionalCostId] ASC),
    CONSTRAINT [FK_ZnodeOmsOrderLineItemsAdditionalCost_ZnodeOmsOrderLineItems] FOREIGN KEY ([OmsOrderLineItemsId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId])
);

