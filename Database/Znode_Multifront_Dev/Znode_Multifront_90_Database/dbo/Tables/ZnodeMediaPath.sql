CREATE TABLE [dbo].[ZnodeMediaPath] (
    [MediaPathId]       INT        IDENTITY (1, 1) NOT NULL,
    [ParentMediaPathId] INT        NULL,
    [PathCode]          NCHAR (10) NULL,
    [IsActive]          BIT        NULL,
    [CreatedBy]         INT        NOT NULL,
    [CreatedDate]       DATETIME   NOT NULL,
    [ModifiedBy]        INT        NOT NULL,
    [ModifiedDate]      DATETIME   NOT NULL,
    CONSTRAINT [PK_ZnodeMediaPath] PRIMARY KEY CLUSTERED ([MediaPathId] ASC),
    CONSTRAINT [FK_ZnodeMediaPath_ZnodeMediaPath] FOREIGN KEY ([ParentMediaPathId]) REFERENCES [dbo].[ZnodeMediaPath] ([MediaPathId])
);









