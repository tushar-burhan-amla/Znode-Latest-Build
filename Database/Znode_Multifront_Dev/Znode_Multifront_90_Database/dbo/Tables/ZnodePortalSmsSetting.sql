CREATE TABLE [dbo].[ZnodePortalSmsSetting] (
    [PortalSmsSettingId]        INT             IDENTITY (1, 1) NOT NULL,
    [PortalId]         INT             NOT NULL,
    [SmsProviderId]        INT             NOT NULL,
    [SmsPortalAccountId]        NVARCHAR (300)   NULL,
    [AuthToken]              NVARCHAR (300)   NULL,
    [FromMobileNumber]            NVARCHAR (50)   NULL,
    [IsSMSSettingEnabled]      BIT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ZnodePortalSmsSetting] PRIMARY KEY CLUSTERED ([PortalSmsSettingId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePortalSmsSetting_ZnodeSmsProvider] FOREIGN KEY ([SmsProviderId]) REFERENCES [dbo].[ZnodeSmsProvider] ([SmsProviderId])
);