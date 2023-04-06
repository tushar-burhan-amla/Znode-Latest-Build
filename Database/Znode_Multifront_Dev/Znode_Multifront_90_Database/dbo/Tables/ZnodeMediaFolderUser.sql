CREATE TABLE [dbo].[ZnodeMediaFolderUser] (
    [MediaFolderUsersId] INT      IDENTITY (1, 1) NOT NULL,
    [MediaPathId]        INT      NULL,
    [UserId]             INT      NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeMediafolderUsers] PRIMARY KEY CLUSTERED ([MediaFolderUsersId] ASC),
    CONSTRAINT [FK_ZnodeMediaFolderUser_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId]),
    CONSTRAINT [FK_ZnodeMediafolderUsers_ZnodeMediaPath] FOREIGN KEY ([MediaPathId]) REFERENCES [dbo].[ZnodeMediaPath] ([MediaPathId])
);



