CREATE TABLE [dbo].[ZnodeOmsTemplateLineItem] (
    [OmsTemplateLineItemId]           INT             IDENTITY (1, 1) NOT NULL,
    [ParentOmsTemplateLineItemId]     INT             NULL,
    [OmsTemplateId]                   INT             NOT NULL,
    [SKU]                             NVARCHAR (100)  NOT NULL,
    [Quantity]                        NUMERIC (28, 6) NULL,
    [CustomText]                      NVARCHAR (MAX)  NULL,
    [OrderLineItemRelationshipTypeId] INT             NULL,
    [CartAddOnDetails]                NVARCHAR (MAX)  NULL,
    [Sequence]                        INT             NOT NULL,
    [CreatedBy]                       INT             NOT NULL,
    [CreatedDate]                     DATETIME        NOT NULL,
    [ModifiedBy]                      INT             NOT NULL,
    [ModifiedDate]                    DATETIME        NOT NULL,
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
    [AutoAddon]                       NVARCHAR (200)  NULL,
    CONSTRAINT [PK_ZnodeOmsTemplateLineItem] PRIMARY KEY CLUSTERED ([OmsTemplateLineItemId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsTemplateLineItem_ZnodeOmsOrderLineItemRelationshipType] FOREIGN KEY ([OrderLineItemRelationshipTypeId]) REFERENCES [dbo].[ZnodeOmsOrderLineItemRelationshipType] ([OrderLineItemRelationshipTypeId]),
    CONSTRAINT [FK_ZnodeOmsTemplateLineItem_ZnodeOmsTemplate] FOREIGN KEY ([OmsTemplateId]) REFERENCES [dbo].[ZnodeOmsTemplate] ([OmsTemplateId]),
    CONSTRAINT [FK_ZnodeOmsTemplateLineItem_ZnodeOmsTemplateLineItem] FOREIGN KEY ([ParentOmsTemplateLineItemId]) REFERENCES [dbo].[ZnodeOmsTemplateLineItem] ([OmsTemplateLineItemId])
);



