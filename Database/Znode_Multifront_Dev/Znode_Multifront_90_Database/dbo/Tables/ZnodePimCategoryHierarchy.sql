CREATE TABLE [dbo].[ZnodePimCategoryHierarchy] (
    [PimCategoryHierarchyId]       INT      IDENTITY (1, 1) NOT NULL,
    [PimCatalogId]                 INT      NULL,
    [ParentPimCategoryHierarchyId] INT      NULL,
    [PimCategoryId]                INT      NULL,
    [DisplayOrder]                 INT      NULL,
    [IsActive]                     BIT      NULL,
    [ActivationDate]               DATETIME NULL,
    [ExpirationDate]               DATETIME NULL,
    [CreatedBy]                    INT      NOT NULL,
    [CreatedDate]                  DATETIME NOT NULL,
    [ModifiedBy]                   INT      NOT NULL,
    [ModifiedDate]                 DATETIME NOT NULL,
    [PimParentCategoryId]          INT      NULL,
    CONSTRAINT [PK_ZnodePimCategoryHierarchy] PRIMARY KEY CLUSTERED ([PimCategoryHierarchyId] ASC),
    CONSTRAINT [FK_ZnodePimCategoryHierarchy_ZnodePimCatalog] FOREIGN KEY ([PimCatalogId]) REFERENCES [dbo].[ZnodePimCatalog] ([PimCatalogId]),
    CONSTRAINT [FK_ZnodePimCategoryHierarchy_ZnodePimCategory] FOREIGN KEY ([PimCategoryId]) REFERENCES [dbo].[ZnodePimCategory] ([PimCategoryId])
);







