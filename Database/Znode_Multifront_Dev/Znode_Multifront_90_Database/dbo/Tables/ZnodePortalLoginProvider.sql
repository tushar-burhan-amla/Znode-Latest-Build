CREATE TABLE [dbo].[ZnodePortalLoginProvider] (
    [PortalLoginProviderId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]              INT            NOT NULL,
    [LoginProviderId]       INT            NOT NULL,
    [ProviderKey]           NVARCHAR (500) NULL,
    [SecretKey]             NVARCHAR (500) NULL,
    [DomainId]              INT            NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePortalLoginProvider] PRIMARY KEY CLUSTERED ([PortalLoginProviderId] ASC),
    CONSTRAINT [FK_ZnodePortalLoginProvider_ZnodeDomain] FOREIGN KEY ([DomainId]) REFERENCES [dbo].[ZnodeDomain] ([DomainId]),
    CONSTRAINT [FK_ZnodePortalLoginProvider_ZnodeLoginProvider] FOREIGN KEY ([LoginProviderId]) REFERENCES [dbo].[ZnodeLoginProvider] ([LoginProviderId]),
    CONSTRAINT [FK_ZnodePortalLoginProvider_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);



