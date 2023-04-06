CREATE TABLE [dbo].[AIZnodeCartLineItemDesign] (
    [OmsOrderLineItemsDesignId] INT            IDENTITY (1, 1) NOT NULL,
    [OmsOrderLineItemsId]       INT            NULL,
    [OrderLineItemsDesignId]    INT            NULL,
    [OrderLineItemsImagePath]   NVARCHAR (MAX) NULL,
    [CreatedBy]                 INT            NULL,
    [CreatedDate]               DATETIME       NULL,
    [ModifiedBy]                INT            NULL,
    [ModifiedDate]              DATETIME       NULL,
	[Custom1]                 NVARCHAR (MAX)  NULL,
    [Custom2]                 NVARCHAR (MAX)  NULL,
    [Custom3]                 NVARCHAR (MAX)  NULL,
    [Custom4]                 NVARCHAR (MAX)  NULL,
    [Custom5]                 NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_AIZnodeLineItemDesign] PRIMARY KEY CLUSTERED ([OmsOrderLineItemsDesignId] ASC)
);

