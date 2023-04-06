CREATE TABLE [dbo].[ZnodeGlobalAttributeValueLocale] (
    [ZnodeGlobalAttributeValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [GlobalAttributeValueId]            INT            NOT NULL,
    [LocaleId]                          INT            NOT NULL,
    [AttributeValue]                    NVARCHAR (MAX) NULL,
    [CreatedBy]                         INT            NOT NULL,
    [CreatedDate]                       DATETIME       NOT NULL,
    [ModifiedBy]                        INT            NOT NULL,
    [ModifiedDate]                      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalAttributeValueLocale] PRIMARY KEY CLUSTERED ([ZnodeGlobalAttributeValueLocaleId] ASC),
    CONSTRAINT [FK_ZnodeGlobalAttributeValueLocale_ZnodeGlobalAttributeValue] FOREIGN KEY ([GlobalAttributeValueId]) REFERENCES [dbo].[ZnodeGlobalAttributeValue] ([GlobalAttributeValueId])
);

