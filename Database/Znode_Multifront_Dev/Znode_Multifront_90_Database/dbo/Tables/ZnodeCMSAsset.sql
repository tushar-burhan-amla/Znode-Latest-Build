CREATE TABLE [dbo].[ZnodeCMSAsset] (
    [CMSAssetId]     INT           IDENTITY (1, 1) NOT NULL,
    [CMSAssetTypeId] INT           NOT NULL,
    [AssetName]      VARCHAR (100) NOT NULL,
    [DisplayName]    VARCHAR (100) NOT NULL,
    [IsDefault]      BIT           CONSTRAINT [DF_ZnodeCMSAsset_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]      INT           NOT NULL,
    [CreatedDate]    DATETIME      NOT NULL,
    [ModifiedBy]     INT           NOT NULL,
    [ModifiedDate]   DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCMSThemeAsset] PRIMARY KEY CLUSTERED ([CMSAssetId] ASC),
    CONSTRAINT [FK_ZnodeCMSAsset_ZnodeCMSAssetType] FOREIGN KEY ([CMSAssetTypeId]) REFERENCES [dbo].[ZnodeCMSAssetType] ([CMSAssetTypeId])
);





