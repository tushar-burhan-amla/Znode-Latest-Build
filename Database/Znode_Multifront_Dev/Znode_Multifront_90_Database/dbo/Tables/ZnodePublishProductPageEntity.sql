CREATE TABLE [dbo].[ZnodePublishProductPageEntity] (
    [PublishProductPageEntityId] INT           IDENTITY (1, 1) NOT NULL,
    [VersionId]                  INT           NOT NULL,
    [PublishStartTime]           DATETIME      NULL,
    [ProductPageId]              INT           NOT NULL,
    [PortalId]                   INT           NOT NULL,
    [ProductType]                VARCHAR (500) NULL,
    [TemplateName]               VARCHAR (500) NULL,
    CONSTRAINT [PK_ZnodePublishProductPageEntity] PRIMARY KEY CLUSTERED ([PublishProductPageEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishProductPageEntityVersionId]
    ON [dbo].[ZnodePublishProductPageEntity]([VersionId] ASC);

