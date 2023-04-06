CREATE TABLE [dbo].[ZnodeCMSSearchIndexServerStatus] (
    [CMSSearchIndexServerStatusId] INT            IDENTITY (1, 1) NOT NULL,
    [ServerName]                   NVARCHAR (100) NOT NULL,
    [CMSSearchIndexMonitorId]      INT            NOT NULL,
    [Status]                       VARCHAR (50)   NOT NULL,
    [CreatedBy]                    INT            NOT NULL,
    [CreatedDate]                  DATETIME       NOT NULL,
    [ModifiedBy]                   INT            NOT NULL,
    [ModifiedDate]                 DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([CMSSearchIndexServerStatusId] ASC),
    CONSTRAINT [FK_ZnodeCMSSearchIndexServerStatus_ZnodeCMSSearchIndexMonitor] FOREIGN KEY ([CMSSearchIndexMonitorId]) REFERENCES [dbo].[ZnodeCMSSearchIndexMonitor] ([CMSSearchIndexMonitorId])
);

