CREATE TABLE [dbo].[ZnodeMediaPathLocale] (
    [MediaPathLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]          INT            NULL,
    [MediaPathId]       INT            NULL,
    [PathName]          NVARCHAR (300) NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeMediaPathLocale] PRIMARY KEY CLUSTERED ([MediaPathLocaleId] ASC),
    CONSTRAINT [FK_ZnodeMediaPathLocale_ZnodeMediaPath] FOREIGN KEY ([MediaPathId]) REFERENCES [dbo].[ZnodeMediaPath] ([MediaPathId])
);







