CREATE TABLE [dbo].[ZnodeSearchIndexMonitor] (
    [SearchIndexMonitorId]  INT          IDENTITY (1, 1) NOT NULL,
    [CatalogIndexId]        INT          NOT NULL,
    [SourceId]              INT          NOT NULL,
    [SourceType]            VARCHAR (50) NOT NULL,
    [SourceTransactionType] VARCHAR (50) NOT NULL,
    [AffectedType]          VARCHAR (50) NOT NULL,
    [CreatedBy]             INT          NOT NULL,
    [CreatedDate]           DATETIME     NOT NULL,
    [ModifiedBy]            INT          NOT NULL,
    [ModifiedDate]          DATETIME     NOT NULL,
    CONSTRAINT [PK_ZnodeSearchIndexMonitor] PRIMARY KEY CLUSTERED ([SearchIndexMonitorId] ASC),
    CONSTRAINT [FK_ZnodeSearchIndexMonitor_ZnodeCatalogIndex] FOREIGN KEY ([CatalogIndexId]) REFERENCES [dbo].[ZnodeCatalogIndex] ([CatalogIndexId])
);





