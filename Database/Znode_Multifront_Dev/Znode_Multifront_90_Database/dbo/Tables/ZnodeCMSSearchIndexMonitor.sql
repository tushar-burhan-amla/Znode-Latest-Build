CREATE TABLE [dbo].[ZnodeCMSSearchIndexMonitor] (
    [CMSSearchIndexMonitorId] INT          IDENTITY (1, 1) NOT NULL,
    [CMSSearchIndexId]        INT          NOT NULL,
    [SourceId]                INT          NOT NULL,
    [SourceType]              VARCHAR (50) NOT NULL,
    [SourceTransactionType]   VARCHAR (50) NOT NULL,
    [AffectedType]            VARCHAR (50) NOT NULL,
    [CreatedBy]               INT          NOT NULL,
    [CreatedDate]             DATETIME     NOT NULL,
    [ModifiedBy]              INT          NOT NULL,
    [ModifiedDate]            DATETIME     NOT NULL,
    PRIMARY KEY CLUSTERED ([CMSSearchIndexMonitorId] ASC),
    CONSTRAINT [FK_ZnodeCMSSearchIndexMonitor_ZnodeCMSSearchIndex] FOREIGN KEY ([CMSSearchIndexId]) REFERENCES [dbo].[ZnodeCMSSearchIndex] ([CMSSearchIndexId])
);

