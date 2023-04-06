CREATE TABLE [dbo].[ZnodeMenuActionsPermission] (
    [MenuActionsPermissionId] INT      IDENTITY (1, 1) NOT NULL,
    [MenuId]                  INT      NOT NULL,
    [ActionId]                INT      NOT NULL,
    [AccessPermissionId]      INT      NOT NULL,
    [CreatedBy]               INT      NOT NULL,
    [CreatedDate]             DATETIME NOT NULL,
    [ModifiedBy]              INT      NOT NULL,
    [ModifiedDate]            DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeMenuActionsPermission] PRIMARY KEY CLUSTERED ([MenuActionsPermissionId] ASC),
    CONSTRAINT [FK_ZnodeMenuActionsPermission_ZnodeAccessPermission] FOREIGN KEY ([AccessPermissionId]) REFERENCES [dbo].[ZnodeAccessPermission] ([AccessPermissionId]),
    CONSTRAINT [FK_ZnodeMenuActionsPermission_ZnodeActions] FOREIGN KEY ([ActionId]) REFERENCES [dbo].[ZnodeActions] ([ActionId]),
    CONSTRAINT [FK_ZnodeMenuActionsPermission_ZnodeMenu] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[ZnodeMenu] ([MenuId])
);

