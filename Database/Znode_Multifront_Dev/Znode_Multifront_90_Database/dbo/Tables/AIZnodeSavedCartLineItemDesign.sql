CREATE TABLE [dbo].[AIZnodeSavedCartLineItemDesign] (
    [OmsSavedCartLineItemDesignId] INT            IDENTITY (1, 1) NOT NULL,
    [OmsSavedCartLineItemId]       INT            NULL,
    [AIDesignId]                   INT            NULL,
    [OmsCookieMappingId]           NVARCHAR (100) NULL,
    [SKU]                          NVARCHAR (100) NULL,
    [AISavedDesigns]               NVARCHAR (MAX) NULL,
    [IsConfigurableProduct]        BIT            CONSTRAINT [DF_AIZnodeSavedCartLineItemDesign_IsConfigurableProduct] DEFAULT ((0)) NULL,
    [IsGroupProduct]               BIT            CONSTRAINT [DF_AIZnodeSavedCartLineItemDesign_IsGroupProduct] DEFAULT ((0)) NULL,
    [SequenceNumber]               INT            NULL,
    [CreatedBy]                    INT            NULL,
    [CreatedDate]                  DATETIME       NULL,
    [ModifiedBy]                   INT            NULL,
    [ModifiedDate]                 DATETIME       NULL,
	[Custom1]                 NVARCHAR (MAX)  NULL,
    [Custom2]                 NVARCHAR (MAX)  NULL,
    [Custom3]                 NVARCHAR (MAX)  NULL,
    [Custom4]                 NVARCHAR (MAX)  NULL,
    [Custom5]                 NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_AIZnodeSavedCartLineItemDesign] PRIMARY KEY CLUSTERED ([OmsSavedCartLineItemDesignId] ASC)
);

