CREATE TABLE [dbo].[ZnodeCMSThemeWidgets] (
    [CMSThemeWidgetId] INT      IDENTITY (1, 1) NOT NULL,
    [CMSThemeId]       INT      NOT NULL,
    [CMSWidgetsId]     INT      NOT NULL,
    [CreatedBy]        INT      NOT NULL,
    [CreatedDate]      DATETIME NOT NULL,
    [ModifiedBy]       INT      NOT NULL,
    [ModifiedDate]     DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeCMSThemeWidgets] PRIMARY KEY CLUSTERED ([CMSThemeWidgetId] ASC),
    CONSTRAINT [FK_ZnodeCMSThemeWidgets_ZnodeCMSTheme] FOREIGN KEY ([CMSThemeId]) REFERENCES [dbo].[ZnodeCMSTheme] ([CMSThemeId]),
    CONSTRAINT [FK_ZnodeCMSThemeWidgets_ZnodeCMSWidgets] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId])
);

