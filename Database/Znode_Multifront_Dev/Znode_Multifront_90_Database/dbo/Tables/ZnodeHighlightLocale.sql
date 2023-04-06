CREATE TABLE [dbo].[ZnodeHighlightLocale] (
    [HighlightLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]          INT            CONSTRAINT [DF_ZnodeHighlightLocale_LocaleId] DEFAULT ((43)) NOT NULL,
    [HighlightId]       INT            NOT NULL,
    [ImageAltTag]       NVARCHAR (200) NULL,
    [Name]              NVARCHAR (200) NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [ShortDescription]  NVARCHAR (MAX) NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeHighlightLocale] PRIMARY KEY CLUSTERED ([HighlightLocaleId] ASC),
    CONSTRAINT [FK_ZnodeHighlightLocale_ZnodeHighlight] FOREIGN KEY ([HighlightId]) REFERENCES [dbo].[ZnodeHighlight] ([HighlightId])
);

