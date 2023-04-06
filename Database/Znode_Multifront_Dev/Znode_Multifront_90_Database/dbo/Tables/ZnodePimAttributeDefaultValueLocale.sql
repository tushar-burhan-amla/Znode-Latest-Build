CREATE TABLE [dbo].[ZnodePimAttributeDefaultValueLocale] (
    [PimAttributeDefaultValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                         INT            NULL,
    [PimAttributeDefaultValueId]       INT            NULL,
    [AttributeDefaultValue]            NVARCHAR (MAX) NULL,
    [Description]                      VARCHAR (300)  NULL,
    [CreatedBy]                        INT            NOT NULL,
    [CreatedDate]                      DATETIME       NOT NULL,
    [ModifiedBy]                       INT            NOT NULL,
    [ModifiedDate]                     DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeDefaultValueLocale] PRIMARY KEY CLUSTERED ([PimAttributeDefaultValueLocaleId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimAttributeDefaultValueLocale_ZNodePimDefaultAttributeValue] FOREIGN KEY ([PimAttributeDefaultValueId]) REFERENCES [dbo].[ZnodePimAttributeDefaultValue] ([PimAttributeDefaultValueId])
);












GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimAttributeDefaultValueLocale_PimAttributeDefaultValueId_LocaleId]
    ON [dbo].[ZnodePimAttributeDefaultValueLocale]([PimAttributeDefaultValueId] ASC, [LocaleId] ASC)
    INCLUDE([AttributeDefaultValue]);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimAttributeDefaultValueLocale_LocaleId_PimAttributeDefaultValueId]
    ON [dbo].[ZnodePimAttributeDefaultValueLocale]([LocaleId] ASC, [PimAttributeDefaultValueId] ASC)
    INCLUDE([AttributeDefaultValue]);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePimAttributeDefaultValueLocale_LocaleId_PimAttributeDefaultValueId]
    ON [dbo].[ZnodePimAttributeDefaultValueLocale]([LocaleId] ASC)
    INCLUDE([PimAttributeDefaultValueId]) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePimAttributeDefaultValueLocale_AttributeDefaultValue]
    ON [dbo].[ZnodePimAttributeDefaultValueLocale]([PimAttributeDefaultValueId] ASC, [LocaleId] ASC)
    INCLUDE([AttributeDefaultValue]) WITH (FILLFACTOR = 90);

