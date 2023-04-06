CREATE VIEW [dbo].[View_CategoryHierarchy]
AS
     SELECT a.PimCategoryHierarchyId,
            a.PimCatalogId,
            a.PimCategoryId,
            d.CategoryValue,
            a.ParentPimCategoryHierarchyId
     FROM dbo.ZnodePimCategoryHierarchy AS a
          INNER JOIN dbo.ZnodePimCategoryProduct AS s ON a.PimCategoryId = s.PimCategoryId
          INNER JOIN dbo.ZnodePimCategoryAttributeValue AS b ON a.PimCategoryId = b.PimCategoryId
          INNER JOIN dbo.ZnodePimAttribute AS c ON b.PimAttributeId = c.PimAttributeId
                                                   AND c.AttributeCode = 'CategoryName'
          INNER JOIN dbo.ZnodePimCategoryAttributeValueLocale AS d ON b.PimCategoryAttributeValueId = d.PimCategoryAttributeValueId
     GROUP BY a.PimCategoryHierarchyId,
              a.PimCatalogId,
              a.PimCategoryId,
              d.CategoryValue,
              a.ParentPimCategoryHierarchyId;
GO
