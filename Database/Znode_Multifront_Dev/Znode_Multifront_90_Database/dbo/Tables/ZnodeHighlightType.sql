CREATE TABLE [dbo].[ZnodeHighlightType] (
    [HighlightTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (200) NOT NULL,
    [Description]     NVARCHAR (500) NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeHighlightType] PRIMARY KEY CLUSTERED ([HighlightTypeId] ASC)
);

