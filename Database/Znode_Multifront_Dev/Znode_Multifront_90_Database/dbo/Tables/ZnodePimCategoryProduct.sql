CREATE TABLE [dbo].[ZnodePimCategoryProduct] (
    [PimCategoryProductId] INT      IDENTITY (1, 1) NOT NULL,
    [PimCategoryId]        INT      NOT NULL,
    [PimProductId]         INT      NOT NULL,
    [DisplayOrder]         INT      NULL,
    [Status]               BIT      NOT NULL,
    [CreatedBy]            INT      NOT NULL,
    [CreatedDate]          DATETIME NOT NULL,
    [ModifiedBy]           INT      NOT NULL,
    [ModifiedDate]         DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePimCategoryProduct] PRIMARY KEY CLUSTERED ([PimCategoryProductId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimCategoryProduct_ZnodePimCategory] FOREIGN KEY ([PimCategoryId]) REFERENCES [dbo].[ZnodePimCategory] ([PimCategoryId]),
    CONSTRAINT [FK_ZnodePimCategoryProduct_ZnodePimProduct] FOREIGN KEY ([PimProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId])
);










GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePimCategoryProduct_CateProductId]
    ON [dbo].[ZnodePimCategoryProduct]([PimCategoryId] ASC, [PimProductId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimCategoryProduct_PimProductId_36136]
    ON [dbo].[ZnodePimCategoryProduct]([PimProductId] ASC)
    INCLUDE([PimCategoryId]) WITH (FILLFACTOR = 90);

