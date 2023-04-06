CREATE TABLE [dbo].[ZnodeAccountPermissionAccess] (
    [AccountPermissionAccessId] INT      IDENTITY (1, 1) NOT NULL,
    [AccountPermissionId]       INT      NOT NULL,
    [AccessPermissionId]        INT      NOT NULL,
    [CreatedBy]                 INT      NOT NULL,
    [CreatedDate]               DATETIME NOT NULL,
    [ModifiedBy]                INT      NOT NULL,
    [ModifiedDate]              DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeAccountPermissionAccess] PRIMARY KEY CLUSTERED ([AccountPermissionAccessId] ASC),
    CONSTRAINT [FK_ZnodeAccountPermissionAccess_ZnodeAccessPermission] FOREIGN KEY ([AccessPermissionId]) REFERENCES [dbo].[ZnodeAccessPermission] ([AccessPermissionId]),
    CONSTRAINT [FK_ZnodeAccountPermissionAccess_ZnodeAccountPermission] FOREIGN KEY ([AccountPermissionId]) REFERENCES [dbo].[ZnodeAccountPermission] ([AccountPermissionId])
);





