CREATE PROCEDURE [dbo].[Znode_GetCategoryList]  
(  
 @IsAssociated BIT = 0 ,@PimCategoryId Transferid Readonly   
)  
AS   
BEGIN   
  IF @IsAssociated = 0 AND NOT EXISTS (SELECT Max(id) FROM @PimCategoryId WHERE id = 0)  
  BEGIN   
   SELECT PimCategoryId FROM znodePimCategory ZPP WHERE NOT EXISTS (SELECT TOP 1  1 FROM @PimCategoryId WHERE id = ZPP.PimCategoryId ) AND PimCategoryId IS NOT NULL   
  END   
  ELSE IF NOT EXISTS (SELECT TOP 1 1  FROM @PimCategoryId HAVING max(id) = 0)  
  BEGIN  
   SELECT PimCategoryId FROM znodePimCategory ZPP WHERE  EXISTS (SELECT TOP 1  1 FROM @PimCategoryId WHERE id = ZPP.PimCategoryId ) AND PimCategoryId IS NOT NULL   
  END   
  ELSE   
  BEGIN   
   SELECT -1 PimCategoryId  
  END   
END