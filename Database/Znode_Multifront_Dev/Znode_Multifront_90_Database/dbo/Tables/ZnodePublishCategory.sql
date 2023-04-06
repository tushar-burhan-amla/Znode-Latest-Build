CREATE TABLE [dbo].[ZnodePublishCategory] (
    [PublishCategoryId]            INT             IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId]             INT             NULL,
    [PimCategoryId]                INT             NULL,
    [PimParentCategoryId]          NVARCHAR (4000) NULL,
    [PublishParentCategoryId]      NVARCHAR (4000) NULL,
    [CreatedBy]                    INT             NOT NULL,
    [CreatedDate]                  DATETIME        NOT NULL,
    [ModifiedBy]                   INT             NOT NULL,
    [ModifiedDate]                 DATETIME        NOT NULL,
    [PimCategoryHierarchyId]       INT             NULL,
    [ParentPimCategoryHierarchyId] INT             NULL,
    CONSTRAINT [PK_ZnodePublishCategory] PRIMARY KEY CLUSTERED ([PublishCategoryId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePublishCategory_ZnodePublishCatalog] FOREIGN KEY ([PublishCatalogId]) REFERENCES [dbo].[ZnodePublishCatalog] ([PublishCatalogId])
);




GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategory_PublishCatalogId_PimCategoryId_PimCategoryHierarchyId]
    ON [dbo].[ZnodePublishCategory]([PublishCatalogId] ASC, [PimCategoryId] ASC, [PimCategoryHierarchyId] ASC)
    INCLUDE([ParentPimCategoryHierarchyId]);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategory_PublishCatalogId_PimCategoryHierarchyId]
    ON [dbo].[ZnodePublishCategory]([PublishCatalogId] ASC, [PimCategoryHierarchyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategory_PublishCatalogId]
    ON [dbo].[ZnodePublishCategory]([PublishCatalogId] ASC)
    INCLUDE([PimCategoryId], [PimCategoryHierarchyId], [ParentPimCategoryHierarchyId]);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategory_PimCategoryId_ParentPimCategoryHierarchyId]
    ON [dbo].[ZnodePublishCategory]([PimCategoryId] ASC, [ParentPimCategoryHierarchyId] ASC);

