CREATE TABLE [dbo].[ZnodeCMSAssetType] (
    [CMSAssetTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [AssetType]      VARCHAR (100) NOT NULL,
    [CreatedBy]      INT           NOT NULL,
    [CreatedDate]    DATETIME      NOT NULL,
    [ModifiedBy]     INT           NOT NULL,
    [ModifiedDate]   DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCMSThemeAssetType] PRIMARY KEY CLUSTERED ([CMSAssetTypeId] ASC)
);

