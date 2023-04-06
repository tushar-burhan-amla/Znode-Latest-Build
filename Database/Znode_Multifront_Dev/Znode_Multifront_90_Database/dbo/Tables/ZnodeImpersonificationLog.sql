CREATE TABLE [dbo].[ZnodeImpersonationLog] (
    [ImpersonationLogId] INT           IDENTITY (1, 1) NOT NULL,
    [PortalId]               INT           NOT NULL,
    [CSRId]                  INT           NULL,
    [WebstoreuserId]         INT           NULL,
    [ActivityType]           VARCHAR (100) NULL,
    [ActivityId]             INT           NULL,
    [OperationType]          VARCHAR (20)  NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeImpersonificationLog] PRIMARY KEY CLUSTERED ([ImpersonationLogId] ASC),
    CONSTRAINT [FK_ZnodeImpersonificationLog_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);



