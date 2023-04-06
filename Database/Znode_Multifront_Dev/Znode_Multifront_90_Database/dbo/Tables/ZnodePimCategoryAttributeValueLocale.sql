CREATE TABLE [dbo].[ZnodePimCategoryAttributeValueLocale] (
    [PimCategoryAttributeValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                          INT            NULL,
    [PimCategoryAttributeValueId]       INT            NULL,
    [CategoryValue]                     NVARCHAR (MAX) NULL,
    [CreatedBy]                         INT            NOT NULL,
    [CreatedDate]                       DATETIME       NOT NULL,
    [ModifiedBy]                        INT            NOT NULL,
    [ModifiedDate]                      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimCategoryAttributeValueLocale] PRIMARY KEY CLUSTERED ([PimCategoryAttributeValueLocaleId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimCategoryValueLocale_ZnodeLocale] FOREIGN KEY ([LocaleId]) REFERENCES [dbo].[ZnodeLocale] ([LocaleId]),
    CONSTRAINT [FK_ZnodePimCategoryValueLocale_ZnodePimCategoryValue] FOREIGN KEY ([PimCategoryAttributeValueId]) REFERENCES [dbo].[ZnodePimCategoryAttributeValue] ([PimCategoryAttributeValueId])
);










GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimCategoryAttributeValueLocale_LocaleId_PimCategoryAttributeValueId]
    ON [dbo].[ZnodePimCategoryAttributeValueLocale]([LocaleId] ASC, [PimCategoryAttributeValueId] ASC)
    INCLUDE([CategoryValue]);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimCategoryAttributeValueLocale_PimCategoryAttributeValueId_25359]
    ON [dbo].[ZnodePimCategoryAttributeValueLocale]([PimCategoryAttributeValueId] ASC)
    INCLUDE([CategoryValue]) WITH (FILLFACTOR = 90);

