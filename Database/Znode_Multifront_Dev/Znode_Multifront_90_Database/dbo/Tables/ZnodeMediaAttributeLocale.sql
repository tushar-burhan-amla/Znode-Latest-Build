CREATE TABLE [dbo].[ZnodeMediaAttributeLocale] (
    [MediaAttributeLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]               INT            NULL,
    [MediaAttributeId]       INT            NULL,
    [AttributeName]          NVARCHAR (300) NULL,
    [Description]            VARCHAR (300)  NULL,
    [CreatedBy]              INT            NOT NULL,
    [CreatedDate]            DATETIME       NOT NULL,
    [ModifiedBy]             INT            NOT NULL,
    [ModifiedDate]           DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeMediaAttributeLocale] PRIMARY KEY CLUSTERED ([MediaAttributeLocaleId] ASC),
    CONSTRAINT [FK_ZnodeMediaAttributeLocale_ZnodeMediaAttribute] FOREIGN KEY ([MediaAttributeId]) REFERENCES [dbo].[ZnodeMediaAttribute] ([MediaAttributeId])
);





