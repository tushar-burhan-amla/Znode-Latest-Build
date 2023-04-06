CREATE TABLE [dbo].[ZnodeAccountPermission] (
    [AccountPermissionId]   INT           IDENTITY (1, 1) NOT NULL,
    [AccountId]             INT           NULL,
    [AccountPermissionName] VARCHAR (300) NOT NULL,
    [CreatedBy]             INT           NOT NULL,
    [CreatedDate]           DATETIME      NOT NULL,
    [ModifiedBy]            INT           NOT NULL,
    [ModifiedDate]          DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeAccountPermisson] PRIMARY KEY CLUSTERED ([AccountPermissionId] ASC),
    CONSTRAINT [FK_ZnodeAccountPermission_ZnodeAccount] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[ZnodeAccount] ([AccountId])
);





