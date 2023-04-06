CREATE TABLE [dbo].[ZnodePimCategoryValueLocale] (
    [PimAttributeDefaultValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                         INT            NULL,
    [PimCategoryValueId]               INT            NULL,
    [CategoryValue]                    NVARCHAR (300) NULL,
    [CreatedBy]                        INT            NOT NULL,
    [CreatedDate]                      DATETIME       NOT NULL,
    [ModifiedBy]                       INT            NOT NULL,
    [ModifiedDate]                     DATETIME       NOT NULL,
    CONSTRAINT [ZnodePimCategoryValueLocale_PK] PRIMARY KEY CLUSTERED ([PimAttributeDefaultValueLocaleId] ASC),
    CONSTRAINT [FK_ZnodePimCategoryValueLocale_ZnodeLocale] FOREIGN KEY ([LocaleId]) REFERENCES [dbo].[ZnodeLocale] ([LocaleId]),
    CONSTRAINT [FK_ZnodePimCategoryValueLocale_ZnodePimCategoryValue] FOREIGN KEY ([PimCategoryValueId]) REFERENCES [dbo].[ZnodePimCategoryValue] ([PimCategoryValueId])
);

