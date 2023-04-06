
	CREATE VIEW [dbo].[View_GetCatalogCategoryProduts]
	 As
	SELECT DISTINCT ISNULL(Null,1) RowId,  a.PimProductId , b.ProductName ,ZPCH.PimCatalogId,a.PimCategoryId,b.Localeid
	FROM ZnodePimCategoryProduct a 
	INNER JOIN ZnodePimCategoryHierarchy ZPCH ON a.PimCategoryId = ZPCH.PimCategoryId
	INNER JOIN View_ZnodePimAttributeValue b ON (a.PimProductId = b.PimProductId )
	inner JOIN ZnodePimAttribute c ON (b.PimAttributeId = c.PimAttributeId AND c.AttributeCode = 'ProductName' AND c.IsCategory = 0)
GO