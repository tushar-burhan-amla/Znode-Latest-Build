CREATE TABLE [dbo].[ZnodePortalSmtpSetting] (
    [PortalSmtpSettingId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]            INT            NOT NULL,
    [ServerName]          VARCHAR (200)  NULL,
    [UserName]            VARCHAR (200)  NULL,
    [Password]            VARCHAR (200)  NULL,
    [Port]                INT            NULL,
    [IsEnableSsl]         BIT            CONSTRAINT [DF__ZnodeSMTP__Enabl__5F9E293D] DEFAULT ((1)) NOT NULL,
    [FromDisplayName]     VARCHAR (200)  NULL,
    [FromEmailAddress]    VARCHAR (200)  NULL,
    [BccEmailAddress]     NVARCHAR (MAX) NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    [DisableAllEmails]    BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ZnodeSMTPSettings] PRIMARY KEY CLUSTERED ([PortalSmtpSettingId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeSMTPSettings_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);








GO
CREATE TRIGGER [dbo].[ZnodePortalSmtpSetting_AspNet_SqlCacheNotification_Trigger] ON [dbo].[ZnodePortalSmtpSetting]
                       FOR INSERT, UPDATE, DELETE AS BEGIN
                       SET NOCOUNT ON
                       EXEC dbo.AspNet_SqlCacheUpdateChangeIdStoredProcedure N'ZnodePortalSmtpSetting'
                       END