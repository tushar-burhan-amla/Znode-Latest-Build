CREATE TABLE [dbo].[ZnodeCatalogIndex] (
    [CatalogIndexId]   INT            IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId] INT            NOT NULL,
    [IndexName]        NVARCHAR (200) NOT NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePortalIndex] PRIMARY KEY CLUSTERED ([CatalogIndexId] ASC) WITH (FILLFACTOR = 90)
);



