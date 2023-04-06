CREATE TABLE [dbo].[ZnodeCMSTheme] (
    [CMSThemeId]    INT           IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (200) NOT NULL,
    [CreatedBy]     INT           NOT NULL,
    [CreatedDate]   DATETIME      NOT NULL,
    [ModifiedBy]    INT           NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL,
    [ParentThemeId] INT           NULL,
    [IsParentTheme] BIT           DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ZnodeCMSTheme] PRIMARY KEY CLUSTERED ([CMSThemeId] ASC),
    CONSTRAINT [FK_ZnodeCMSTheme_ParentThemeId] FOREIGN KEY ([ParentThemeId]) REFERENCES [dbo].[ZnodeCMSTheme] ([CMSThemeId])
);



