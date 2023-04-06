CREATE TABLE [dbo].[ZnodeOmsOrderAttribute] (
    [OmsOrderAttributeId] INT            IDENTITY (1, 1) NOT NULL,
    [OmsOrderLineItemsId] INT            NULL,
    [AttributeCode]       NVARCHAR (600) NULL,
    [AttributeValue]      NVARCHAR (MAX) NULL,
    [CreatedBy]           INT            NULL,
    [CreatedDate]         DATETIME       NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL,
    [AttributeValueCode]  NVARCHAR (600) NULL,
    CONSTRAINT [PK_ZnodeOmsOrderAttribute] PRIMARY KEY CLUSTERED ([OmsOrderAttributeId] ASC),
    CONSTRAINT [FK_ZnodeOmsOrderAttribute_ZnodeOmsOrderLineItems] FOREIGN KEY ([OmsOrderLineItemsId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId])
);



