CREATE TABLE [dbo].[ZnodeFormBuilderGlobalAttributeValueLocale] (
    [FormBuilderGlobalAttributeValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [FormBuilderGlobalAttributeValueId]       INT            NOT NULL,
    [LocaleId]                                INT            NOT NULL,
    [AttributeValue]                          NVARCHAR (MAX) NULL,
    [CreatedBy]                               INT            NOT NULL,
    [CreatedDate]                             DATETIME       NOT NULL,
    [ModifiedBy]                              INT            NOT NULL,
    [ModifiedDate]                            DATETIME       NOT NULL,
    [GlobalAttributeDefaultValueId]           INT            NULL,
    [MediaId]                                 INT            NULL,
    [MediaPath]                               NVARCHAR (300) NULL,
    CONSTRAINT [PK_ZnodeFormBuilderGlobalAttributeValueLocale] PRIMARY KEY CLUSTERED ([FormBuilderGlobalAttributeValueLocaleId] ASC),
    CONSTRAINT [FK_ZnodeFormBuilderGlobalAttributeValueLocale_ZnodeFormBuilderGlobalAttributeValue] FOREIGN KEY ([FormBuilderGlobalAttributeValueId]) REFERENCES [dbo].[ZnodeFormBuilderGlobalAttributeValue] ([FormBuilderGlobalAttributeValueId]),
    CONSTRAINT [FK_ZnodeFormBuilderGlobalAttributeValueLocale_ZnodeMedia] FOREIGN KEY ([MediaId]) REFERENCES [dbo].[ZnodeMedia] ([MediaId])
);



