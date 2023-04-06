CREATE  PROCEDURE [dbo].[Znode_GetProductList]
(
 @IsAssociated BIT = 0
 ,@PimProductId Transferid Readonly ,
 @PimCatalogId INT = 0,
 @IsCatalogFilter   BIT            = 0
)
AS 
BEGIN 

 IF @IsAssociated = 0  --AND NOT EXISTS (SELECT Max(id) FROM @PimProductId WHERE id = 0)
 BEGIN 
  IF @PimCatalogId = 0 AND @IsCatalogFilter =1 -- filter for all catalog product except product which are not associated with any catalog
		 BEGIN
		 SELECT PimProductId 
		 FROM ZnodePimProduct ZPP 
		 WHERE NOT EXISTS (SELECT TOP 1  1 FROM @PimProductId WHERE id = ZPP.PimProductId )
		 AND EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryProduct ZPCP
		 inner join ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId
		 WHERE ZPP.PimProductId = ZPCP.PimProductId	)
		 AND PimProductId IS NOT NULL 
		 END
  ELSE IF @PimCatalogId = 0
         BEGIN
		 SELECT PimProductId 
		 FROM ZnodePimProduct ZPP 
		 WHERE NOT EXISTS (SELECT TOP 1  1 FROM @PimProductId WHERE id = ZPP.PimProductId )
		 AND PimProductId IS NOT NULL 
		 END
   ELSE IF @PimCatalogId = -1
		BEGIN
		SELECT PimProductId 
		FROM ZnodePimProduct ZPP 
		WHERE NOT EXISTS (SELECT TOP 1  1 FROM @PimProductId WHERE id = ZPP.PimProductId )
		AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryProduct ZPCP
		 inner join ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId 
										   WHERE ZPP.PimProductId = ZPCP.PimProductId)
		AND PimProductId IS NOT NULL
		END
   ELSE
		BEGIN
		SELECT PimProductId 
		FROM ZnodePimProduct ZPP 
		WHERE NOT EXISTS (SELECT TOP 1  1 FROM @PimProductId WHERE id = ZPP.PimProductId )
		AND EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryProduct ZPCP
		 inner join ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId 
		 WHERE ZPP.PimProductId = ZPCP.PimProductId AND ZPCH.PimCatalogId = @PimCatalogId)
		AND PimProductId IS NOT NULL
		END
 END 
 ELSE IF NOT EXISTS (SELECT TOP 1 1  FROM @PimProductId HAVING max(id) = 0)
 BEGIN

 SELECT PimProductId 
 FROM ZnodePimProduct ZPP 
 WHERE  EXISTS (SELECT TOP 1  1 FROM @PimProductId WHERE id = ZPP.PimProductId )
  AND PimProductId IS NOT NULL 
 END 
 ELSE 
 BEGIN 
  SELECT 0 PimProductId
END 
END