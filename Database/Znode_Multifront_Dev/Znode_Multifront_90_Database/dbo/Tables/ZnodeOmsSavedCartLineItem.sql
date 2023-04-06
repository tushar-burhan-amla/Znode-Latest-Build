CREATE TABLE [dbo].[ZnodeOmsSavedCartLineItem] (
    [OmsSavedCartLineItemId]          INT             IDENTITY (1, 1) NOT NULL,
    [ParentOmsSavedCartLineItemId]    INT             NULL,
    [OmsSavedCartId]                  INT             NOT NULL,
    [SKU]                             NVARCHAR (100)  NOT NULL,
    [Quantity]                        NUMERIC (28, 6) CONSTRAINT [DF_ZnodeOmsSavedCartLineItem_Quantity] DEFAULT ((0)) NULL,
    [OrderLineItemRelationshipTypeId] INT             NULL,
    [CustomText]                      NVARCHAR (MAX)  NULL,
    [CartAddOnDetails]                NVARCHAR (MAX)  NULL,
    [Sequence]                        INT             NOT NULL,
    [CreatedBy]                       INT             NOT NULL,
    [CreatedDate]                     DATETIME        NOT NULL,
    [ModifiedBy]                      INT             NOT NULL,
    [ModifiedDate]                    DATETIME        NOT NULL,
    [AutoAddon]                       NVARCHAR (200)  NULL,
    [OmsOrderId]                      INT             NULL,
    [Custom1]                         NVARCHAR (MAX)  NULL,
    [Custom2]                         NVARCHAR (MAX)  NULL,
    [Custom3]                         NVARCHAR (MAX)  NULL,
    [Custom4]                         NVARCHAR (MAX)  NULL,
    [Custom5]                         NVARCHAR (MAX)  NULL,
    [GroupId]                         NVARCHAR (MAX)  NULL,
    [ProductName]                     NVARCHAR (1000) NULL,
    [Description]                     NVARCHAR (MAX)  NULL,
    [CustomUnitPrice] NUMERIC(28, 6) NULL, 
    CONSTRAINT [PK_ZnodeOmsSavedCartLineItem] PRIMARY KEY CLUSTERED ([OmsSavedCartLineItemId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsSavedCartLineItem_ZnodeOmsSavedCart] FOREIGN KEY ([OmsSavedCartId]) REFERENCES [dbo].[ZnodeOmsSavedCart] ([OmsSavedCartId]),
    CONSTRAINT [FK_ZnodeOmsSavedCartLineItem_ZnodeOmsSavedCartLineItem] FOREIGN KEY ([ParentOmsSavedCartLineItemId]) REFERENCES [dbo].[ZnodeOmsSavedCartLineItem] ([OmsSavedCartLineItemId]),
    CONSTRAINT [FK_ZnodeOmsSavedCartLineItem_ZnodeOrderLineItemRelationshipType] FOREIGN KEY ([OrderLineItemRelationshipTypeId]) REFERENCES [dbo].[ZnodeOmsOrderLineItemRelationshipType] ([OrderLineItemRelationshipTypeId])
);


















GO
CREATE NONCLUSTERED INDEX [idx_ZnodeOmsSavedCartLineItem_OrderLineItemRelationshipTypeID]
    ON [dbo].[ZnodeOmsSavedCartLineItem]([OrderLineItemRelationshipTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodeOmsSavedCartLineItem_OrderLineItemRelationshipTypeId]
    ON [dbo].[ZnodeOmsSavedCartLineItem]([OrderLineItemRelationshipTypeId] ASC)
    INCLUDE([OmsSavedCartId], [Quantity]);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodeOmsSavedCartLineItem_OmsSavedCartId_OrderLineItemRelationshipTypeId]
    ON [dbo].[ZnodeOmsSavedCartLineItem]([OmsSavedCartId] ASC, [OrderLineItemRelationshipTypeId] ASC)
    INCLUDE([Quantity]);

