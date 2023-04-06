CREATE TABLE [dbo].[ZnodeCMSThemeCSS] (
    [CMSThemeCSSId] INT             IDENTITY (1, 1) NOT NULL,
    [CMSThemeId]    INT             NOT NULL,
    [CSSName]       NVARCHAR (1000) NOT NULL,
    [CreatedBy]     INT             NOT NULL,
    [CreatedDate]   DATETIME        NOT NULL,
    [ModifiedBy]    INT             NOT NULL,
    [ModifiedDate]  DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeCMSTemplateCSS] PRIMARY KEY CLUSTERED ([CMSThemeCSSId] ASC),
    CONSTRAINT [FK_ZnodeCMSThemeCSS_ZnodeCMSTheme] FOREIGN KEY ([CMSThemeId]) REFERENCES [dbo].[ZnodeCMSTheme] ([CMSThemeId])
);

