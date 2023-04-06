CREATE TABLE [dbo].[ZnodeMenu] (
    [MenuId]         INT            IDENTITY (1, 1) NOT NULL,
    [ParentMenuId]   INT            NULL,
    [MenuName]       VARCHAR (100)  NULL,
    [MenuSequence]   INT            NULL,
    [AreaName]       NVARCHAR (500) NULL,
    [ControllerName] VARCHAR (200)  NULL,
    [ActionName]     VARCHAR (200)  NULL,
    [CSSClassName]   VARCHAR (300)  NULL,
    [IsActive]       BIT            CONSTRAINT [DF__ZnodeMenu__IsAct__0425A276] DEFAULT ((1)) NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeMenu] PRIMARY KEY CLUSTERED ([MenuId] ASC),
    CONSTRAINT [FK_ZnodeMenu_ZnodeMenu] FOREIGN KEY ([ParentMenuId]) REFERENCES [dbo].[ZnodeMenu] ([MenuId])
);









