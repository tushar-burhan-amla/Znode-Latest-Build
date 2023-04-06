CREATE TABLE [dbo].[ZnodePublishCatalog] (
    [PublishCatalogId] INT            IDENTITY (1, 1) NOT NULL,
    [PimCatalogId]     INT            NULL,
    [CatalogName]      NVARCHAR (MAX) NOT NULL,
    [ExternalId]       VARCHAR (50)   NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    [Token]            NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ZnodePublishCatalog] PRIMARY KEY CLUSTERED ([PublishCatalogId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170519-230210]
    ON [dbo].[ZnodePublishCatalog]([PimCatalogId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishCatalog_PublishCatalogId]
    ON [dbo].[ZnodePublishCatalog]([PublishCatalogId] ASC) WITH (FILLFACTOR = 90);

