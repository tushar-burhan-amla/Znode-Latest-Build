CREATE TABLE [dbo].[ZnodeMediaCategory] (
    [MediaCategoryId]        INT           IDENTITY (1, 1) NOT NULL,
    [MediaPathId]            INT           NULL,
    [MediaId]                INT           NULL,
    [MediaAttributeFamilyId] INT           NULL,
    [Path]                   VARCHAR (300) NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeMediaCategory] PRIMARY KEY CLUSTERED ([MediaCategoryId] ASC),
    CONSTRAINT [FK_ZnodeMediaCategory_ZnodeMedia] FOREIGN KEY ([MediaId]) REFERENCES [dbo].[ZnodeMedia] ([MediaId]),
    CONSTRAINT [FK_ZnodeMediaCategory_ZnodeMediaAttributeFamily] FOREIGN KEY ([MediaAttributeFamilyId]) REFERENCES [dbo].[ZnodeMediaAttributeFamily] ([MediaAttributeFamilyId]),
    CONSTRAINT [FK_ZnodeMediaCategory_ZnodeMediaPath] FOREIGN KEY ([MediaPathId]) REFERENCES [dbo].[ZnodeMediaPath] ([MediaPathId])
);





