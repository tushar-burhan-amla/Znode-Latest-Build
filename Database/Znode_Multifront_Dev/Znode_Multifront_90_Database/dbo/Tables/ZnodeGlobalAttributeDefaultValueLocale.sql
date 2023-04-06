CREATE TABLE [dbo].[ZnodeGlobalAttributeDefaultValueLocale] (
    [GlobalAttributeDefaultValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                            INT            NULL,
    [GlobalAttributeDefaultValueId]       INT            NULL,
    [AttributeDefaultValue]               NVARCHAR (MAX) NULL,
    [Description]                         VARCHAR (300)  NULL,
    [CreatedBy]                           INT            NOT NULL,
    [CreatedDate]                         DATETIME       NOT NULL,
    [ModifiedBy]                          INT            NOT NULL,
    [ModifiedDate]                        DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalAttributeDefaultValueLocale] PRIMARY KEY CLUSTERED ([GlobalAttributeDefaultValueLocaleId] ASC),
    CONSTRAINT [FK_ZnodeGlobalAttributeDefaultValueLocale_ZNodeGlobalDefaultAttributeValue] FOREIGN KEY ([GlobalAttributeDefaultValueId]) REFERENCES [dbo].[ZnodeGlobalAttributeDefaultValue] ([GlobalAttributeDefaultValueId])
);

