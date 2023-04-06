CREATE TABLE [dbo].[AspNetZnodeUser] (
    [AspNetZnodeUserId] NVARCHAR (128) NOT NULL,
    [UserName]          NVARCHAR (256) NOT NULL,
    [PortalId]          INT            NULL,
    CONSTRAINT [PK_AspNetZnodeUser] PRIMARY KEY CLUSTERED ([AspNetZnodeUserId] ASC),
    CONSTRAINT [FK_AspNetZnodeUser_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);




GO
CREATE NONCLUSTERED INDEX [Idx_AspNetZnodeUser_UserName]
    ON [dbo].[AspNetZnodeUser]([UserName] ASC);

