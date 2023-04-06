CREATE TABLE [dbo].[ZnodeRoleMenuAccessMapper] (
    [RoleMenuAccessMapperId] INT      IDENTITY (1, 1) NOT NULL,
    [RoleMenuId]             INT      NOT NULL,
    [AccessPermissionId]     INT      NOT NULL,
    [CreatedBy]              INT      NOT NULL,
    [CreatedDate]            DATETIME NOT NULL,
    [ModifiedBy]             INT      NOT NULL,
    [ModifiedDate]           DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeRoleMenuAcessMapper] PRIMARY KEY CLUSTERED ([RoleMenuAccessMapperId] ASC),
    CONSTRAINT [FK_ZnodeRoleMenuAcessMapper_ZnodeAccessPermissions] FOREIGN KEY ([AccessPermissionId]) REFERENCES [dbo].[ZnodeAccessPermission] ([AccessPermissionId]),
    CONSTRAINT [FK_ZnodeRoleMenuAcessMapper_ZnodeRoleMenu] FOREIGN KEY ([RoleMenuId]) REFERENCES [dbo].[ZnodeRoleMenu] ([RoleMenuId])
);





