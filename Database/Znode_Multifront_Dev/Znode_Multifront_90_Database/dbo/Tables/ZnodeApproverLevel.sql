CREATE TABLE [dbo].[ZnodeApproverLevel] (
    [ApproverLevelId] INT            IDENTITY (1, 1) NOT NULL,
    [LevelCode]       VARCHAR (200)  NOT NULL,
    [LevelName]       NVARCHAR (300) NULL,
    [Description]     NVARCHAR (MAX) NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeApproverLevel] PRIMARY KEY CLUSTERED ([ApproverLevelId] ASC),
    CONSTRAINT [UK_ZnodeApproverLevel_LevelCode] UNIQUE NONCLUSTERED ([LevelCode] ASC)
);

