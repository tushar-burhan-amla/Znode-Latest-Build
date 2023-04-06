CREATE TABLE [dbo].[ZnodeWidgetGlobalAttributeValueLocale] (
    [WidgetGlobalAttributeValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [WidgetGlobalAttributeValueId]       INT          NOT NULL,
    [LocaleId]                         INT            NOT NULL,
    [AttributeValue]                   NVARCHAR (MAX) NULL,
    [CreatedBy]                        INT            NOT NULL,
    [CreatedDate]                      DATETIME       NOT NULL,
    [ModifiedBy]                       INT            NOT NULL,
    [ModifiedDate]                     DATETIME       NOT NULL,
    [GlobalAttributeDefaultValueId]    INT            NULL,
    [MediaId]                          INT            NULL,
    [MediaPath]                        NVARCHAR (300) NULL,
    CONSTRAINT [PK_ZnodeWidgetGlobalAttributeValueLocale] PRIMARY KEY CLUSTERED ([WidgetGlobalAttributeValueLocaleId] ASC),
    CONSTRAINT [FK_ZnodeWidgetGlobalAttributeValueLocale_ZnodeMedia] FOREIGN KEY ([MediaId]) REFERENCES [dbo].[ZnodeMedia] ([MediaId]),
    CONSTRAINT [FK_ZnodeWidgetGlobalAttributeValueLocale_ZnodeWidgetGlobalAttributeValue] FOREIGN KEY ([WidgetGlobalAttributeValueId]) REFERENCES [dbo].[ZnodeWidgetGlobalAttributeValue] ([WidgetGlobalAttributeValueId])
);