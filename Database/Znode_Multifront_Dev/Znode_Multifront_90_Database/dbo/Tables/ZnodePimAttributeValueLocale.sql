CREATE TABLE [dbo].[ZnodePimAttributeValueLocale] (
    [ZnodePimAttributeValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [PimAttributeValueId]            INT            NOT NULL,
    [LocaleId]                       INT            NOT NULL,
    [AttributeValue]                 NVARCHAR (MAX) NULL,
    [CreatedBy]                      INT            NOT NULL,
    [CreatedDate]                    DATETIME       NOT NULL,
    [ModifiedBy]                     INT            NOT NULL,
    [ModifiedDate]                   DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeValueLocale] PRIMARY KEY CLUSTERED ([ZnodePimAttributeValueLocaleId] ASC),
    CONSTRAINT [FK_ZnodePimAttributeValueLocale_ZnodePimAttributeValue] FOREIGN KEY ([PimAttributeValueId]) REFERENCES [dbo].[ZnodePimAttributeValue] ([PimAttributeValueId])
);


















GO



GO
CREATE UNIQUE NONCLUSTERED INDEX [IDX_ZnodePimAttributeValuELocaleId]
    ON [dbo].[ZnodePimAttributeValueLocale]([PimAttributeValueId] ASC, [LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_ZnodePimAttributeValueLocale_PimAttributeValueId]
    ON [dbo].[ZnodePimAttributeValueLocale]([PimAttributeValueId] ASC)
    INCLUDE([ZnodePimAttributeValueLocaleId], [LocaleId]);




GO
CREATE NONCLUSTERED INDEX [IDX_ZnodePimAttributeValueLocale_LocaleId]
    ON [dbo].[ZnodePimAttributeValueLocale]([LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IND_ZnodePimAttributeValueLocale_localeId_PimAttributeValueId]
    ON [dbo].[ZnodePimAttributeValueLocale]([LocaleId] ASC)
    INCLUDE([ZnodePimAttributeValueLocaleId], [PimAttributeValueId]);

