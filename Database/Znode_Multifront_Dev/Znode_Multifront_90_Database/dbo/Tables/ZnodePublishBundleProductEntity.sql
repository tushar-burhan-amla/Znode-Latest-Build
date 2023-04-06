CREATE TABLE [dbo].[ZnodePublishBundleProductEntity] (
    [PublishBundleProductEntityId]  INT IDENTITY (1, 1) NOT NULL,
    [VersionId]                     INT NOT NULL,
    [ZnodeProductId]                INT NOT NULL,
    [ZnodeCatalogId]                INT NOT NULL,
    [AssociatedZnodeProductId]      INT NOT NULL,
    [AssociatedProductDisplayOrder] INT NOT NULL,
	[AssociatedProductBundleQuantity] INT NULL DEFAULT 1,
	[ElasticSearchEvent]              INT,
    CONSTRAINT [PK_ZnodePublishBundleProductEntity] PRIMARY KEY CLUSTERED ([PublishBundleProductEntityId] ASC) WITH (FILLFACTOR = 90)
);




GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishBundleProductEntity]
    ON [dbo].[ZnodePublishBundleProductEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishBundleProductEntity_CatalogId]
    ON [dbo].[ZnodePublishBundleProductEntity]([VersionId] ASC, [ZnodeCatalogId] ASC);

