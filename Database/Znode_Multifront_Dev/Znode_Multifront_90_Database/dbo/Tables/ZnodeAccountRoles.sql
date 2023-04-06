CREATE TABLE [dbo].[ZnodeAccountRoles] (
    [AccountId]    INT      NOT NULL,
    [RoleId]       INT      NOT NULL,
    [CreatedBy]    INT      NOT NULL,
    [CreatedDate]  DATETIME NOT NULL,
    [ModifiedBy]   INT      NOT NULL,
    [ModifiedDate] DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeAccountRoles] PRIMARY KEY CLUSTERED ([AccountId] ASC, [RoleId] ASC)
);





