CREATE TABLE [dbo].[ZnodePimGlobalDisplaySetting] (
    [PimGlobalDisplaySettingsId]           INT      IDENTITY (1, 1) NOT NULL,
    [MediaId]                              INT      NULL,
    [MaxPimCatalogDisplayItems]            INT      NOT NULL,
    [MaxPimCatalogItemSmallThumbnailWidth] INT      NOT NULL,
    [MaxPimCatalogItemSmallWidth]          INT      NOT NULL,
    [MaxPimCatalogItemMediumWidth]         INT      NOT NULL,
    [MaxPimCatalogItemThumbnailWidth]      INT      NOT NULL,
    [MaxPimCatalogItemLargeWidth]          INT      NOT NULL,
    [MaxPimCatalogItemCrossSellWidth]      INT      NOT NULL,
    [CreatedBy]                            INT      NOT NULL,
    [CreatedDate]                          DATETIME NOT NULL,
    [ModifiedBy]                           INT      NOT NULL,
    [ModifiedDate]                         DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePimGlobalDisplaySetting] PRIMARY KEY CLUSTERED ([PimGlobalDisplaySettingsId] ASC),
    CONSTRAINT [FK_ZnodePimGlobalDisplaySetting_ZnodeMedia] FOREIGN KEY ([MediaId]) REFERENCES [dbo].[ZnodeMedia] ([MediaId])
);

