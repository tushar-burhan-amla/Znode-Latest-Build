CREATE PROCEDURE [dbo].[Znode_GetCategoryHierarchy]
( @PimCatalogId     INT,
  @LocaleId         INT = NULL,
  @PimCategoryId    INT = NULL)
AS
/*
     Summary :- This Procedure is used to get category hierarchy 
     Unit Testing 
     EXEC [dbo].[Znode_GetCategoryHierarchy] @PimCatalogId=2,@LocaleId=1
	
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             

             DECLARE @TBL_PimCategoryHierarchy TABLE
             (PimCategoryHierarchyId INT,
              PimCatalogId           INT,
              PimCategoryId          INT,
              CategoryValue          NVARCHAR(600),
              ParentPimCategoryHierarchyId    INT,
              DisplayOrder           INT
             );
             INSERT INTO @TBL_PimCategoryHierarchy
                    SELECT a.PimCategoryHierarchyId,a.PimCatalogId,a.PimCategoryId,VIPCAV.CategoryValue,ParentPimCategoryHierarchyId,ISNULL(a.DisplayOrder, 0)
                    FROM ZnodePimCategoryHierarchy AS a
                    LEFT JOIN ZnodePimCategoryProduct AS s ON(a.PimCategoryId = s.PimCategoryId)
                    LEFT JOIN View_PimCategoryAttributeValue VIPCAV ON (VIPCAV.PimCategoryId = a.PimCategoryId
                                                                       AND VIPCAV.LocaleId = @LocaleId)
                    WHERE A.PimCatalogId = @PimCatalogId
                          AND VIPCAV.AttributeCode = 'CategoryName'
                    GROUP BY a.PimCategoryHierarchyId,a.PimCatalogId,a.PimCategoryId,VIPCAV.CategoryValue,ParentPimCategoryHierarchyId,a.DisplayOrder
                    ORDER BY a.PimCatalogId,a.PimCategoryId,a.DisplayOrder;

             IF @PimCategoryId IS NULL
                 BEGIN
                     SELECT Category.PimCategoryHierarchyId,Category.PimCatalogId,Category.PimCategoryId,Category.CategoryValue,Category.ParentPimCategoryHierarchyId,Category.DisplayOrder
                     FROM
                     (
                         SELECT 0 AS PimCategoryHierarchyId,a.PimCatalogId,0 AS PimCategoryId,a.CatalogName AS CategoryValue,-1 AS ParentPimCategoryHierarchyId,0 AS DisplayOrder
                         FROM ZnodePimCatalog AS a
                         WHERE a.PimCatalogId = @PimCatalogId
                         UNION ALL
                         SELECT a.PimCategoryHierarchyId ,a.PimCatalogId,a.PimCategoryId,CategoryValue,ParentPimCategoryHierarchyId,a.DisplayOrder     
                         FROM @TBL_PimCategoryHierarchy AS a
                              LEFT JOIN ZnodePimCategoryProduct AS s ON(a.PimCategoryId = s.PimCategoryId
                                                                        AND S.PimProductId IS NULL)
						GROUP BY a.PimCategoryHierarchyId ,a.PimCatalogId,a.PimCategoryId,CategoryValue,ParentPimCategoryHierarchyId,a.DisplayOrder     
                     ) AS Category 
                     ORDER BY Category.DisplayOrder, PimCategoryId;
                             
                 END;
             ELSE
                 BEGIN
				  
                     SELECT Category.PimCategoryHierarchyId,Category.PimCatalogId,Category.PimCategoryId,Category.CategoryValue,Category.ParentPimCategoryHierarchyId,Category.DisplayOrder
                     FROM
                     (
                         SELECT DISTINCT 0 AS PimCategoryHierarchyId,a.PimCatalogId,0 AS PimCategoryId,a.CatalogName AS CategoryValue,NULL AS ParentPimCategoryHierarchyId,0 AS DisplayOrder
                         FROM ZnodePimCatalog AS a
                         WHERE a.PimCatalogId = @PimCatalogId
                         UNION ALL
                         SELECT a.PimCategoryHierarchyId ,a.PimCatalogId,a.PimCategoryId,CategoryValue,ParentPimCategoryHierarchyId,a.DisplayOrder     
                         FROM @TBL_PimCategoryHierarchy AS a
                              LEFT JOIN ZnodePimCategoryProduct AS s ON(a.PimCategoryId = s.PimCategoryId AND S.PimProductId IS NULL)
						 GROUP BY a.PimCategoryHierarchyId ,a.PimCatalogId,a.PimCategoryId,CategoryValue,ParentPimCategoryHierarchyId,a.DisplayOrder
					) AS Category
                    WHERE Category.ParentPimCategoryHierarchyId = @PimCategoryId                    
                    ORDER BY Category.DisplayOrder,PimCategoryId;
                              
                 END;
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCategoryHierarchy @PimCatalogId = '+CAST(@PimCatalogId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@PimCategoryId='+CAST(@PimCategoryId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		   
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCategoryHierarchy',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;