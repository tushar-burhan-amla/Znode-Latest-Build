CREATE TABLE [dbo].[ZnodePimCustomFieldLocale] (
    [PimCustomFieldLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [PimCustomFieldId]       INT            NULL,
    [LocaleId]               INT            NULL,
    [CustomKey]              NVARCHAR (300) NULL,
    [CustomKeyValue]         NVARCHAR (300) NULL,
    [CreatedBy]              INT            NOT NULL,
    [CreatedDate]            DATETIME       NOT NULL,
    [ModifiedBy]             INT            NOT NULL,
    [ModifiedDate]           DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimCustomFieldLocale] PRIMARY KEY CLUSTERED ([PimCustomFieldLocaleId] ASC),
    CONSTRAINT [FK_ZnodePimCustomFieldLocale_ZnodePimCustomField] FOREIGN KEY ([PimCustomFieldId]) REFERENCES [dbo].[ZnodePimCustomField] ([PimCustomFieldId])
);







