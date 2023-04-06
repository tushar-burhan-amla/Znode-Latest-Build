CREATE TABLE [dbo].[ZnodeActionMenu] (
    [ActionMenuId] INT      IDENTITY (1, 1) NOT NULL,
    [MenuId]       INT      NULL,
    [ActionId]     INT      NULL,
    [CreatedBy]    INT      NOT NULL,
    [CreatedDate]  DATETIME NOT NULL,
    [ModifiedBy]   INT      NOT NULL,
    [ModifiedDate] DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeActionMenu] PRIMARY KEY CLUSTERED ([ActionMenuId] ASC),
    CONSTRAINT [FK_ZnodeActionMenu_ZnodeAction] FOREIGN KEY ([ActionId]) REFERENCES [dbo].[ZnodeActions] ([ActionId]),
    CONSTRAINT [FK_ZnodeActionMenu_ZnodeMenu] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[ZnodeMenu] ([MenuId])
);

