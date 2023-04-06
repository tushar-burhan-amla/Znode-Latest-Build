CREATE TABLE [dbo].[ZnodePortalAccount] (
    [PortalAccountId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]        INT      NULL,
    [AccountId]       INT      NULL,
    [CreatedBy]       INT      NOT NULL,
    [CreatedDate]     DATETIME NOT NULL,
    [ModifiedBy]      INT      NOT NULL,
    [ModifiedDate]    DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePortalAccount_PortalAccountId] PRIMARY KEY CLUSTERED ([PortalAccountId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePortalAccount_ZnodeAccount] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[ZnodeAccount] ([AccountId]),
    CONSTRAINT [FK_ZnodePortalAccount_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);





