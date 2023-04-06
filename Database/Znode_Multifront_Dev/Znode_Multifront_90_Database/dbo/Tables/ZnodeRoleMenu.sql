CREATE TABLE [dbo].[ZnodeRoleMenu] (
    [RoleMenuId]   INT            IDENTITY (1, 1) NOT NULL,
    [RoleId]       NVARCHAR (128) NULL,
    [MenuId]       INT            NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [ModifiedBy]   INT            NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeRoleMenu] PRIMARY KEY CLUSTERED ([RoleMenuId] ASC),
    CONSTRAINT [FK_ZnodeRoleMenu_AspNetRoles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]),
    CONSTRAINT [FK_ZnodeRoleMenu_ZnodeMenu] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[ZnodeMenu] ([MenuId])
);





