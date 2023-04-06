CREATE TABLE [dbo].[AIZnodePortalSettings] (
    [AIPortalSettingId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]          INT            NULL,
    [WebsiteCode]       INT            NULL,
    [DomainName]        NVARCHAR (MAX) NULL,
    [WebApiclientKey]   NVARCHAR (MAX) NULL,
    [CreatedBy]         INT            NULL,
    [CreatedDate]       DATETIME       NULL,
    [ModifiedBy]        INT            NULL,
    [ModifiedDate]      DATETIME       NULL,
    [IsWebstorePreview] BIT            NULL,
    [PublishStateId]    TINYINT        NULL,
	[Custom1]                 NVARCHAR (MAX)  NULL,
    [Custom2]                 NVARCHAR (MAX)  NULL,
    [Custom3]                 NVARCHAR (MAX)  NULL,
    [Custom4]                 NVARCHAR (MAX)  NULL,
    [Custom5]                 NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_AIZnodePortalSettings] PRIMARY KEY CLUSTERED ([AIPortalSettingId] ASC)
);



