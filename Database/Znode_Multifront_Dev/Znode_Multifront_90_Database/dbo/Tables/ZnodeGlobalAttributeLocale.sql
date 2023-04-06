CREATE TABLE [dbo].[ZnodeGlobalAttributeLocale] (
    [GlobalAttributeLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                INT            NULL,
    [GlobalAttributeId]       INT            NULL,
    [AttributeName]           NVARCHAR (300) NULL,
    [Description]             VARCHAR (300)  NULL,
    [CreatedBy]               INT            NOT NULL,
    [CreatedDate]             DATETIME       NOT NULL,
    [ModifiedBy]              INT            NOT NULL,
    [ModifiedDate]            DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalAttributeLocale] PRIMARY KEY CLUSTERED ([GlobalAttributeLocaleId] ASC),
    CONSTRAINT [FK_ZnodeGlobalAttributeLocale_ZnodeGlobalAttribute] FOREIGN KEY ([GlobalAttributeId]) REFERENCES [dbo].[ZnodeGlobalAttribute] ([GlobalAttributeId])
);

