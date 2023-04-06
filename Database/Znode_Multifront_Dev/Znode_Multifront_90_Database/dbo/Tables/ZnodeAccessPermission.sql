CREATE TABLE [dbo].[ZnodeAccessPermission] (
    [AccessPermissionId] INT           IDENTITY (1, 1) NOT NULL,
    [PermissionsName]    VARCHAR (200) NULL,
    [PermissionCode]     VARCHAR (200) NULL,
    [TypeOfPermission]   VARCHAR (20)  NULL,
    [IsActive]           BIT           CONSTRAINT [DF__ZnodeAcce__IsAct__09DE7BCC] DEFAULT ((1)) NULL,
    [CreatedBy]          INT           NOT NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [ModifiedBy]         INT           NOT NULL,
    [ModifiedDate]       DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeAccessPermissions] PRIMARY KEY CLUSTERED ([AccessPermissionId] ASC)
);



