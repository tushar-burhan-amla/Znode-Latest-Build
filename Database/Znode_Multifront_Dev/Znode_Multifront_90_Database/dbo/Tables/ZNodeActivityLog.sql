CREATE TABLE [dbo].[ZnodeActivityLog] (
    [ActivityLogId]      INT            IDENTITY (1, 1) NOT NULL,
    [ActivityLogTypeId]  INT            CONSTRAINT [DF_ZNodeActivityLog_ActivityLogTypeID] DEFAULT ((1)) NOT NULL,
    [PortalId]           INT            NULL,
    [ActivityCreateDate] DATETIME       CONSTRAINT [DF_ZNodeActivityLog_CreateDte] DEFAULT (getdate()) NOT NULL,
    [ActivityEndDate]    DATETIME       NULL,
    [URL]                NVARCHAR (MAX) NULL,
    [Data1]              NVARCHAR (MAX) NULL,
    [Data2]              NVARCHAR (MAX) NULL,
    [Data3]              NVARCHAR (MAX) NULL,
    [Status]             NVARCHAR (MAX) NULL,
    [LongData]           NVARCHAR (MAX) NULL,
    [Source]             NVARCHAR (255) NULL,
    [Target]             NVARCHAR (255) NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodeActivityLog] PRIMARY KEY CLUSTERED ([ActivityLogId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeActivityLog_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);










GO
CREATE NONCLUSTERED INDEX [IX_ZnodeActivityLog_ActivityLogTypeId]
    ON [dbo].[ZnodeActivityLog]([ActivityLogTypeId] ASC)
    INCLUDE([PortalId], [ActivityCreateDate], [Data1]);

