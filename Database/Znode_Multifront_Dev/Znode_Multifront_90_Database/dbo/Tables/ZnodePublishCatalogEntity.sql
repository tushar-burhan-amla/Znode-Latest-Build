CREATE TABLE [dbo].[ZnodePublishCatalogEntity] (
    [PublishCatalogEntityId] INT           IDENTITY (1, 1) NOT NULL,
    [VersionId]              INT           NOT NULL,
    [ZnodeCatalogId]         INT           NOT NULL,
    [CatalogName]            VARCHAR (300) NOT NULL,
    [RevisionType]           VARCHAR (50)  NOT NULL,
    [LocaleId]               INT           NOT NULL,
    [IsAllowIndexing]        BIT           NOT NULL,
    CONSTRAINT [PK_ZnodePublishCatalogEntity] PRIMARY KEY CLUSTERED ([PublishCatalogEntityId] ASC) WITH (FILLFACTOR = 90)
);




GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishCatalogEntity_VersionId_ZnodeCatalogId]
    ON [dbo].[ZnodePublishCatalogEntity]([VersionId] ASC, [ZnodeCatalogId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishCatalogEntity_VersionId]
    ON [dbo].[ZnodePublishCatalogEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishCatalogEntity_CatalogId]
    ON [dbo].[ZnodePublishCatalogEntity]([VersionId] ASC, [ZnodeCatalogId] ASC);

