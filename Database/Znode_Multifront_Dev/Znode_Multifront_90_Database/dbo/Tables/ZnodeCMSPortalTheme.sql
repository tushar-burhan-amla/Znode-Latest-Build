CREATE TABLE [dbo].[ZnodeCMSPortalTheme] (
    [CMSPortalThemeId]   INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]           INT            NOT NULL,
    [CMSThemeId]         INT            NOT NULL,
    [CMSThemeCSSId]      INT            NULL,
    [MediaId]            INT            NULL,
    [FavIconId]          INT            NULL,
    [WebsiteTitle]       NVARCHAR (200) NULL,
    [WebsiteDescription] NVARCHAR (MAX) NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSPortalTheme_1] PRIMARY KEY CLUSTERED ([CMSPortalThemeId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeCMSPortalTheme_ZnodeCMSTheme] FOREIGN KEY ([CMSThemeId]) REFERENCES [dbo].[ZnodeCMSTheme] ([CMSThemeId]),
    CONSTRAINT [FK_ZnodeCMSPortalTheme_ZnodeCMSThemeCSS] FOREIGN KEY ([CMSThemeCSSId]) REFERENCES [dbo].[ZnodeCMSThemeCSS] ([CMSThemeCSSId]),
    CONSTRAINT [FK_ZnodeCMSPortalTheme_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);









