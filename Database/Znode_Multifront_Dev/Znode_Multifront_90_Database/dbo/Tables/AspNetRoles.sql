CREATE TABLE [dbo].[AspNetRoles] (
    [Id]              NVARCHAR (128) NOT NULL,
    [Name]            NVARCHAR (256) NOT NULL,
    [IsActive]        BIT            CONSTRAINT [DF_AspNetRoles_IsActive] DEFAULT ((0)) NOT NULL,
    [TypeOfRole]      VARCHAR (50)   NULL,
    [IsSystemDefined] BIT            CONSTRAINT [DF_AspNetRoles_IsSystemDefined] DEFAULT ((0)) NOT NULL,
    [IsDefault]       BIT            CONSTRAINT [DF_AspNetRoles_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);












GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [dbo].[AspNetRoles]([Name] ASC);

