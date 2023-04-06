CREATE VIEW [dbo].[View_GetCatalogProduct]
AS
SELECT DISTINCT ZPCH.PimCatalogId, ZPP.PimProductId, ZPP.PublishStateId AS ProductPublishStateId, ZPP.PimAttributeFamilyId, ZPP.ExternalId
FROM dbo.ZnodePimCategoryProduct AS ZPCC 
INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId
INNER JOIN  dbo.ZnodePimProduct AS ZPP ON ZPP.PimProductId = ZPCC.PimProductId
GO