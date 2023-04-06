CREATE TABLE [dbo].[ZnodePortalLocale] (
    [PortalLocaleId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]       INT      NOT NULL,
    [LocaleId]       INT      NOT NULL,
    [IsDefault]      BIT      CONSTRAINT [DF_ZnodePortalLocale_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]      INT      NOT NULL,
    [CreatedDate]    DATETIME NOT NULL,
    [ModifiedBy]     INT      NOT NULL,
    [ModifiedDate]   DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePortalLocale] PRIMARY KEY CLUSTERED ([PortalLocaleId] ASC),
    CONSTRAINT [FK_ZnodePortalLocale_ZnodeLocale] FOREIGN KEY ([LocaleId]) REFERENCES [dbo].[ZnodeLocale] ([LocaleId]),
    CONSTRAINT [FK_ZnodePortalLocale_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);

