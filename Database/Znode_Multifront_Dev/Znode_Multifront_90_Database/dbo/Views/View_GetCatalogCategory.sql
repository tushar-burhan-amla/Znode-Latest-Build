CREATE VIEW [dbo].[View_GetCatalogCategory]
AS
SELECT DISTINCT ZPP.PimCategoryId, ZPP.PublishStateId AS CategoryPublishStateId, ZPP.ExternalId, ZPP.PimAttributeFamilyId, ZPCC.PimCatalogId
FROM            dbo.ZnodePimCategoryHierarchy AS ZPCC INNER JOIN
                         dbo.ZnodePimCategory AS ZPP ON ZPP.PimCategoryId = ZPCC.PimCategoryId

GO