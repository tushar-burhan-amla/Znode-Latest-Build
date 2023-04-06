CREATE TABLE [dbo].[ZNodeActivityLog] (
    [ActivityLogId]    INT            IDENTITY (1, 1) NOT NULL,
    [PaymentSettingId] INT            NULL,
    [PortalID]         INT            NULL,
    [URL]              NVARCHAR (MAX) NULL,
    [Data1]            NVARCHAR (255) NULL,
    [Data2]            NVARCHAR (255) NULL,
    [Data3]            NVARCHAR (255) NULL,
    [Status]           NVARCHAR (255) NULL,
    [LongData]         NVARCHAR (MAX) NULL,
    [Source]           NVARCHAR (255) NULL,
    [Target]           NVARCHAR (255) NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodeActivityLog] PRIMARY KEY CLUSTERED ([ActivityLogId] ASC)
);



