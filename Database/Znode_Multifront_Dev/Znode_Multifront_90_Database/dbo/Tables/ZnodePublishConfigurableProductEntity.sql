CREATE TABLE [dbo].[ZnodePublishConfigurableProductEntity] (
    [PublishConfigurableProductEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                          INT            NOT NULL,
    [ZnodeProductId]                     INT            NOT NULL,
    [ZnodeCatalogId]                     INT            NOT NULL,
    [AssociatedZnodeProductId]           INT            NOT NULL,
    [AssociatedProductDisplayOrder]      INT            NOT NULL,
    [SelectValues]                       NVARCHAR (MAX) NOT NULL,
    [ConfigurableAttributeCodes]         NVARCHAR (MAX) NULL,
    [IsDefault]                          BIT            CONSTRAINT [DF_ZnodePublishConfigurableProductEntity_IsDefault] DEFAULT ((0)) NULL,
    [ElasticSearchEvent]                 INT            NULL,
    CONSTRAINT [PK_ZnodePublishConfigurableProductEntity] PRIMARY KEY CLUSTERED ([PublishConfigurableProductEntityId] ASC) WITH (FILLFACTOR = 90)
);








GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishConfigurableProductEntity]
    ON [dbo].[ZnodePublishConfigurableProductEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishConfigurableProductEntity_ZnodeProductId_1D1B4]
    ON [dbo].[ZnodePublishConfigurableProductEntity]([ZnodeProductId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishConfigurableProductEntity_VersionId_ZnodeProductId_7BE02]
    ON [dbo].[ZnodePublishConfigurableProductEntity]([VersionId] ASC, [ZnodeProductId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishConfigurableProductEntity_CatalogId]
    ON [dbo].[ZnodePublishConfigurableProductEntity]([VersionId] ASC, [ZnodeCatalogId] ASC);

