CREATE TABLE [dbo].[ZnodePortalCatalog] (
    [PortalCatalogId]  INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]         INT      NOT NULL,
    [PublishCatalogId] INT      NOT NULL,
    [CreatedBy]        INT      NOT NULL,
    [CreatedDate]      DATETIME NOT NULL,
    [ModifiedBy]       INT      NOT NULL,
    [ModifiedDate]     DATETIME NOT NULL,
    [OldPublishCatalogId] INT NULL, 
    CONSTRAINT [PK_ZnodePortalCatalog] PRIMARY KEY CLUSTERED ([PortalCatalogId] ASC),
    CONSTRAINT [FK_ZNodePortalCatalog_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);











