CREATE TABLE [dbo].[ZnodeOmsQuoteLineItem] (
    [OmsQuoteLineItemId]              INT             IDENTITY (1, 1) NOT NULL,
    [ParentOmsQuoteLineItemId]        INT             NULL,
    [OmsQuoteId]                      INT             NOT NULL,
    [SKU]                             NVARCHAR (100)  NOT NULL,
    [Quantity]                        NUMERIC (28, 6) CONSTRAINT [DF_ZnodeOmsQuoteLineItem] DEFAULT ((0)) NULL,
    [OrderLineItemRelationshipTypeId] INT             NULL,
    [CustomText]                      NVARCHAR (MAX)  NULL,
    [CartAddOnDetails]                NVARCHAR (MAX)  NULL,
    [Sequence]                        INT             NOT NULL,
    [CreatedBy]                       INT             NOT NULL,
    [CreatedDate]                     DATETIME        NOT NULL,
    [ModifiedBy]                      INT             NOT NULL,
    [ModifiedDate]                    DATETIME        NOT NULL,
    [GroupId]                         NVARCHAR (MAX)  NULL,
    [ProductName]                     NVARCHAR (1000) NULL,
    [Description]                     NVARCHAR (MAX)  NULL,
    [Price]                           NUMERIC (28, 6) NULL,
    [ShipSeparately]                  BIT             NULL,
    [ShippingCost]                    NUMERIC (28, 6) NULL,
    [Custom1]                         NVARCHAR (MAX)  NULL,
    [Custom2]                         NVARCHAR (MAX)  NULL,
    [Custom3]                         NVARCHAR (MAX)  NULL,
    [Custom4]                         NVARCHAR (MAX)  NULL,
    [Custom5]                         NVARCHAR (MAX)  NULL,
    [InitialPrice]                    DECIMAL (28, 6) NULL,
    [InitialShippingCost]             DECIMAL (28, 6) NULL,
    [IsPriceEdit]                     BIT             NULL,
    CONSTRAINT [PK_ZnodeOmsQuoteLineItem] PRIMARY KEY CLUSTERED ([OmsQuoteLineItemId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsQuoteLineItem_ZnodeOmsOrderLineItemRelationshipType] FOREIGN KEY ([OrderLineItemRelationshipTypeId]) REFERENCES [dbo].[ZnodeOmsOrderLineItemRelationshipType] ([OrderLineItemRelationshipTypeId]),
    CONSTRAINT [FK_ZnodeOmsQuoteLineItem_ZnodeOmsQuote] FOREIGN KEY ([OmsQuoteId]) REFERENCES [dbo].[ZnodeOmsQuote] ([OmsQuoteId]),
    CONSTRAINT [FK_ZnodeOmsQuoteLineItem_ZnodeOmsQuoteLineItem] FOREIGN KEY ([ParentOmsQuoteLineItemId]) REFERENCES [dbo].[ZnodeOmsQuoteLineItem] ([OmsQuoteLineItemId])
);











