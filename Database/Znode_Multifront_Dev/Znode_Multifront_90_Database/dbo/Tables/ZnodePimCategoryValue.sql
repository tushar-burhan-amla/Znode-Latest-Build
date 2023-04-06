CREATE TABLE [dbo].[ZnodePimCategoryValue] (
    [PimCategoryValueId]         INT      IDENTITY (1, 1) NOT NULL,
    [PimCategoryId]              INT      NULL,
    [PimAttributeFamilyId]       INT      NULL,
    [PimAttributeId]             INT      NULL,
    [PimAttributeDefaultValueId] INT      NULL,
    [CreatedBy]                  INT      NOT NULL,
    [CreatedDate]                DATETIME NOT NULL,
    [ModifiedBy]                 INT      NOT NULL,
    [ModifiedDate]               DATETIME NOT NULL,
    CONSTRAINT [ZnodePimCategoryValue_PK] PRIMARY KEY CLUSTERED ([PimCategoryValueId] ASC),
    CONSTRAINT [FK_ZnodePimCategoryValue_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId]),
    CONSTRAINT [FK_ZnodePimCategoryValue_ZnodePimAttributeDefaultValue] FOREIGN KEY ([PimAttributeDefaultValueId]) REFERENCES [dbo].[ZnodePimAttributeDefaultValue] ([PimAttributeDefaultValueId]),
    CONSTRAINT [FK_ZnodePimCategoryValue_ZnodePimAttributeFamily] FOREIGN KEY ([PimAttributeFamilyId]) REFERENCES [dbo].[ZnodePimAttributeFamily] ([PimAttributeFamilyId]),
    CONSTRAINT [FK_ZnodePimCategoryValue_ZnodePimCategory] FOREIGN KEY ([PimCategoryId]) REFERENCES [dbo].[ZnodePimCategory] ([PimCategoryId])
);

