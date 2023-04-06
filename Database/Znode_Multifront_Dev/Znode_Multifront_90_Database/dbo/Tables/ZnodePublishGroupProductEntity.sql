CREATE TABLE [dbo].[ZnodePublishGroupProductEntity] (
    [PublishGroupProductEntityId]   INT IDENTITY (1, 1) NOT NULL,
    [VersionId]                     INT NOT NULL,
    [ZnodeProductId]                INT NOT NULL,
    [ZnodeCatalogId]                INT NOT NULL,
    [AssociatedZnodeProductId]      INT NOT NULL,
    [AssociatedProductDisplayOrder] INT NOT NULL,
	[ElasticSearchEvent]              INT,
    CONSTRAINT [PK_ZnodePublishGroupProductEntity] PRIMARY KEY CLUSTERED ([PublishGroupProductEntityId] ASC) WITH (FILLFACTOR = 90)
);




GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishGroupProductEntity]
    ON [dbo].[ZnodePublishGroupProductEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishGroupProductEntity_CatalogId]
    ON [dbo].[ZnodePublishGroupProductEntity]([VersionId] ASC, [ZnodeCatalogId] ASC);

