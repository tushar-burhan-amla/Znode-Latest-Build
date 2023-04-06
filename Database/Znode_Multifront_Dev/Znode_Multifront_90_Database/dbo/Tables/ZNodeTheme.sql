CREATE TABLE [dbo].[ZnodeTheme] (
    [ThemeId]      INT           IDENTITY (1, 1) NOT NULL,
    [ThemeName]    NVARCHAR (50) NOT NULL,
    [CreatedBy]    INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [ModifiedBy]   INT           NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    CONSTRAINT [PK_ZNodeTheme] PRIMARY KEY CLUSTERED ([ThemeId] ASC)
);



