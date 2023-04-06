CREATE TABLE [dbo].[ZnodeMediaFamilyLocale] (
    [MediaFamilyLocaleId]    INT           IDENTITY (1, 1) NOT NULL,
    [LocaleId]               INT           NULL,
    [MediaAttributeFamilyId] INT           NULL,
    [AttributeFamilyName]    VARCHAR (300) NULL,
    [Label]                  VARCHAR (300) NULL,
    [Description]            VARCHAR (300) NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeMediaFamilyLocale] PRIMARY KEY CLUSTERED ([MediaFamilyLocaleId] ASC),
    CONSTRAINT [FK_ZnodeMediaFamilyLocale_ZnodeMediaAttributeFamily] FOREIGN KEY ([MediaAttributeFamilyId]) REFERENCES [dbo].[ZnodeMediaAttributeFamily] ([MediaAttributeFamilyId])
);





