CREATE TABLE [dbo].[ZnodeAccountUserPermission] (
    [AccountUserPermissionId]   INT      IDENTITY (1, 1) NOT NULL,
    [UserId]                    INT      NULL,
    [AccountPermissionAccessId] INT      NOT NULL,
    [CreatedBy]                 INT      NOT NULL,
    [CreatedDate]               DATETIME NOT NULL,
    [ModifiedBy]                INT      NOT NULL,
    [ModifiedDate]              DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeAccountUserPermission] PRIMARY KEY CLUSTERED ([AccountUserPermissionId] ASC),
    CONSTRAINT [FK_ZnodeAccountUserPermission_ZnodeAccountPermissionAccess] FOREIGN KEY ([AccountPermissionAccessId]) REFERENCES [dbo].[ZnodeAccountPermissionAccess] ([AccountPermissionAccessId]),
    CONSTRAINT [FK_ZnodeAccountUserPermission_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);

