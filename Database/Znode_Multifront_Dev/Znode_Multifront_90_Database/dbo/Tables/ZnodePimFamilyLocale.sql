CREATE TABLE [dbo].[ZnodePimFamilyLocale] (
    [PimFamilyLocaleId]    INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]             INT            NULL,
    [PimAttributeFamilyId] INT            NULL,
    [AttributeFamilyName]  NVARCHAR (MAX) NULL,
    [Label]                VARCHAR (300)  NULL,
    [Description]          VARCHAR (MAX)  NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimFamilyLocale] PRIMARY KEY CLUSTERED ([PimFamilyLocaleId] ASC),
    CONSTRAINT [FK_ZnodePimFamilyLocale_ZnodePimAttributeFamily] FOREIGN KEY ([PimAttributeFamilyId]) REFERENCES [dbo].[ZnodePimAttributeFamily] ([PimAttributeFamilyId])
);





