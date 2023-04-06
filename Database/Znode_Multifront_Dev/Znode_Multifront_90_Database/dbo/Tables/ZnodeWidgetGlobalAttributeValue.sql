CREATE TABLE [dbo].[ZnodeWidgetGlobalAttributeValue] (
    [WidgetGlobalAttributeValueId]  INT            IDENTITY (1, 1) NOT NULL,
    [CMSContentContainerId]				INT			   Not NULL,
	[CMSContainerProfileVariantId]        INT			   Not NULL,
    [GlobalAttributeId]             INT            NULL,
    [GlobalAttributeDefaultValueId] INT            NULL,
    [AttributeValue]                NVARCHAR (300) NULL,
    [CreatedBy]                     INT            NOT NULL,
    [CreatedDate]                   DATETIME       NOT NULL,
    [ModifiedBy]                    INT            NOT NULL,
    [ModifiedDate]                  DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeWidgetGlobalAttributeValue] PRIMARY KEY CLUSTERED ([WidgetGlobalAttributeValueId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeWidgetGlobalAttributeValue_ZnodeGlobalAttribute] FOREIGN KEY ([GlobalAttributeId]) REFERENCES [dbo].[ZnodeGlobalAttribute] ([GlobalAttributeId]),
    CONSTRAINT [FK_ZnodeWidgetGlobalAttributeValue_ZnodeGlobalAttributeDefaultValue] FOREIGN KEY ([GlobalAttributeDefaultValueId]) REFERENCES [dbo].[ZnodeGlobalAttributeDefaultValue] ([GlobalAttributeDefaultValueId])
);