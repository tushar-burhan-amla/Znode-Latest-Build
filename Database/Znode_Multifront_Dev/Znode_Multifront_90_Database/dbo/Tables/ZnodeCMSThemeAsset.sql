CREATE TABLE [dbo].[ZnodeCMSThemeAsset] (
    [CMSThemeAssetId] INT      IDENTITY (1, 1) NOT NULL,
    [CMSThemeId]      INT      NOT NULL,
    [CMSAssetId]      INT      NOT NULL,
    [ProductTypeId]   INT      NULL,
    [CreatedBy]       INT      NOT NULL,
    [CreatedDate]     DATETIME NOT NULL,
    [ModifiedBy]      INT      NOT NULL,
    [ModifiedDate]    DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeCMSThemeAsset_1] PRIMARY KEY CLUSTERED ([CMSThemeAssetId] ASC),
    CONSTRAINT [FK_ZnodeCMSThemeAsset_ZnodeCMSAsset] FOREIGN KEY ([CMSAssetId]) REFERENCES [dbo].[ZnodeCMSAsset] ([CMSAssetId]),
    CONSTRAINT [FK_ZnodeCMSThemeAsset_ZnodeCMSTheme] FOREIGN KEY ([CMSThemeId]) REFERENCES [dbo].[ZnodeCMSTheme] ([CMSThemeId])
);

