CREATE TABLE [dbo].[ZnodeSearchIndexServerStatus] (
    [SearchIndexServerStatusId] INT           IDENTITY (1, 1) NOT NULL,
    [ServerName]                NVARCHAR (50) NOT NULL,
    [SearchIndexMonitorId]      INT           NOT NULL,
    [Status]                    VARCHAR (50)  NOT NULL,
    [CreatedBy]                 INT           NOT NULL,
    [CreatedDate]               DATETIME      NOT NULL,
    [ModifiedBy]                INT           NOT NULL,
    [ModifiedDate]              DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeSearchIndexServerStatus] PRIMARY KEY CLUSTERED ([SearchIndexServerStatusId] ASC),
    CONSTRAINT [FK_ZnodeSearchIndexServerStatus_ZnodeSearchIndexMonitor] FOREIGN KEY ([SearchIndexMonitorId]) REFERENCES [dbo].[ZnodeSearchIndexMonitor] ([SearchIndexMonitorId])
);

