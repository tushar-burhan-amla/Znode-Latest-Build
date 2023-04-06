CREATE TABLE [dbo].[ZnodeUserPortal] (
    [UserPortalId] INT      IDENTITY (1, 1) NOT NULL,
    [UserId]       INT      NULL,
    [PortalId]     INT      NULL,
    [CreatedBy]    INT      NOT NULL,
    [CreatedDate]  DATETIME NOT NULL,
    [ModifiedBy]   INT      NOT NULL,
    [ModifiedDate] DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeUserPortal] PRIMARY KEY CLUSTERED ([UserPortalId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeUserPortal_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodeUserportal_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);








GO
CREATE NONCLUSTERED INDEX [IX_ZnodeUserPortal_UserId_PortalId]
    ON [dbo].[ZnodeUserPortal]([UserId] ASC, [PortalId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodeUserPortal_UserId]
    ON [dbo].[ZnodeUserPortal]([UserId] ASC);

