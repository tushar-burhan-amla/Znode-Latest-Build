CREATE TABLE [dbo].[ZnodeDepartmentUser] (
    [DepartmentUserId] INT      IDENTITY (1, 1) NOT NULL,
    [UserId]           INT      NULL,
    [DepartmentId]     INT      NULL,
    [CreatedBy]        INT      NOT NULL,
    [CreatedDate]      DATETIME NOT NULL,
    [ModifiedBy]       INT      NOT NULL,
    [ModifiedDate]     DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeDepartmentUser] PRIMARY KEY CLUSTERED ([DepartmentUserId] ASC),
    CONSTRAINT [FK_ZnodeDepartmentUser_ZnodeDepartment] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[ZnodeDepartment] ([DepartmentId]),
    CONSTRAINT [fk_ZnodeDepartmentUser_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);

