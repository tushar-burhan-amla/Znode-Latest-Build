CREATE TABLE [dbo].[ZnodePimCategory] (
    [PimCategoryId]        INT           IDENTITY (1, 1) NOT NULL,
    [PimAttributeFamilyId] INT           NULL,
    [IsActive]             BIT           CONSTRAINT [DF_ZnodePimCatagory_IsActive] DEFAULT ((1)) NULL,
    [ExternalId]           NVARCHAR (50) NULL,
    [CreatedBy]            INT           NOT NULL,
    [CreatedDate]          DATETIME      NOT NULL,
    [ModifiedBy]           INT           NOT NULL,
    [ModifiedDate]         DATETIME      NOT NULL,
    [IsCategoryPublish]    TINYINT       NULL,
    [PublishStateId]       TINYINT       NULL,
    CONSTRAINT [PK_ZnodePimCatagory] PRIMARY KEY CLUSTERED ([PimCategoryId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimCategory_ZnodePimAttributeFamily] FOREIGN KEY ([PimAttributeFamilyId]) REFERENCES [dbo].[ZnodePimAttributeFamily] ([PimAttributeFamilyId]),
    CONSTRAINT [FK_ZnodePimCategory_ZnodePimCategory] FOREIGN KEY ([PimCategoryId]) REFERENCES [dbo].[ZnodePimCategory] ([PimCategoryId]),
    CONSTRAINT [FK_ZnodePimCategory_ZnodePimCategory1] FOREIGN KEY ([PimCategoryId]) REFERENCES [dbo].[ZnodePimCategory] ([PimCategoryId]),
    CONSTRAINT [FK_ZnodePimCategory_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
);









