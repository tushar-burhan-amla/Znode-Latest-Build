CREATE TABLE [dbo].[ZnodePublishAddonEntity] (
    [PublishAddonEntityId]          INT           IDENTITY (1, 1) NOT NULL,
    [VersionId]                     INT           NOT NULL,
    [ZnodeProductId]                INT           NOT NULL,
    [ZnodeCatalogId]                INT           NOT NULL,
    [AssociatedZnodeProductId]      INT           NOT NULL,
    [AssociatedProductDisplayOrder] INT           NOT NULL,
    [LocaleId]                      INT           NOT NULL,
    [GroupName]                     VARCHAR (100) NOT NULL,
    [DisplayType]                   VARCHAR (100) NOT NULL,
    [DisplayOrder]                  INT           NOT NULL,
    [IsRequired]                    BIT           NOT NULL,
    [RequiredType]                  VARCHAR (100) NOT NULL,
    [IsDefault]                     BIT           NOT NULL,
	[ElasticSearchEvent]              INT,
    CONSTRAINT [PK_ZnodePublishAddonEntity] PRIMARY KEY CLUSTERED ([PublishAddonEntityId] ASC) WITH (FILLFACTOR = 90)
);






GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishAddOnEntity]
    ON [dbo].[ZnodePublishAddonEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishAddonEntity_CatalogId]
    ON [dbo].[ZnodePublishAddonEntity]([VersionId] ASC, [ZnodeCatalogId] ASC);


GO
CREATE NONCLUSTERED INDEX [Inx_ZnodePublishAddonEntity_LocaleId_ZnodeProductId_VersionId]
    ON [dbo].[ZnodePublishAddonEntity]([VersionId] ASC, [ZnodeProductId] ASC, [LocaleId] ASC);

