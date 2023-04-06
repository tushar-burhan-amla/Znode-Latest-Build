CREATE TABLE [dbo].[ZnodePimAddonGroup] (
    [PimAddonGroupId] INT            IDENTITY (1, 1) NOT NULL,
    [DisplayType]     NVARCHAR (200) NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimAddonGroup] PRIMARY KEY CLUSTERED ([PimAddonGroupId] ASC)
);



