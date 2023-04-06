CREATE TABLE [dbo].[ZnodePublishVersionEntity] (
    [PublishVersionEntityId] INT          IDENTITY (1, 1) NOT NULL,
    [VersionId]              INT          NOT NULL,
    [ZnodeCatalogId]         INT          NOT NULL,
    [RevisionType]           VARCHAR (50) NOT NULL,
    [LocaleId]               INT          NOT NULL,
    [IsPublishSuccess]       BIT          NOT NULL,
    CONSTRAINT [PK_ZnodePublishVersionEntity] PRIMARY KEY CLUSTERED ([PublishVersionEntityId] ASC)
);




GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishVersionEntity_VersionId_ZnodeCatalogId]
    ON [dbo].[ZnodePublishVersionEntity]([VersionId] ASC, [ZnodeCatalogId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishVersionEntity_VersionId]
    ON [dbo].[ZnodePublishVersionEntity]([VersionId] ASC);

