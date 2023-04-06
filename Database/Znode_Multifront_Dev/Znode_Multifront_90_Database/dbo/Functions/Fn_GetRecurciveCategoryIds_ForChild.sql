CREATE FUNCTION [dbo].[Fn_GetRecurciveCategoryIds_ForChild]
(
       @PimCategoryHierarchyId     VARCHAR(2000)
	   ,@PimCatalogId	   INT 
)
RETURNS  @ConvertTableData TABLE (PimCategoryId INT ,PimCategoryHierarchyId INT,ParentPimCategoryHierarchyId INT   )
AS
	-- Summary :- This function is used to get the category id recursive 
	-- Unit Testing 
	-- EXEC [dbo].[Znode_SplitWhereClause] '',2
     BEGIN
        
		DECLARE @TBL_PimCategoryId TABLE (PimCategoryHierarchyId INT )

		INSERT INTO @TBL_PimCategoryId (PimCategoryHierarchyId )
		SELECT item FROM dbo.split(@PimCategoryHierarchyId,',') 

		IF @PimCategoryHierarchyId = '' 
		BEGIN 
		   INSERT INTO @ConvertTableData
	       SELECT ZPCH.PimCategoryId ,ZPCH.PimCategoryHierarchyId ,ZPCH.ParentPimCategoryHierarchyId
		   FROM ZnodePimCategoryHierarchy   ZPCH 
		   WHERE ZPCH.PimCatalogId = @PimCatalogId
		END 
		ELSE 
		BEGIN 
	   ;With Cte_RecursiveAccountId AS
	    (
	       SELECT PimCategoryId ,PimCategoryHierarchyId ,ParentPimCategoryHierarchyId
		   FROM ZnodePimCategoryHierarchy   ZPCH 
		   WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PimCategoryId TBPC WHERE  ZPCH.ParentPimCategoryHierarchyId = TBPC.PimCategoryHierarchyId OR @PimCategoryHierarchyId = '')
		   AND (ZPCH.PimCatalogId = @PimCatalogId)
		   UNION ALL 
		   SELECT ZPCH.PimCategoryId ,ZPCH.PimCategoryHierarchyId,ZPCH.ParentPimCategoryHierarchyId
		   FROM ZnodePimCategoryHierarchy   ZPCH 
		   INNER JOIN Cte_RecursiveAccountId CTRA ON (CTRA.PimCategoryHierarchyId = ZPCH.ParentPimCategoryHierarchyId )
	      WHERE  (ZPCH.PimCatalogId = @PimCatalogId)
		  )
  	   INSERT INTO @ConvertTableData
	   SELECT   a.PimCategoryId,PimCategoryHierarchyId ,ParentPimCategoryHierarchyId
	   FROM Cte_RecursiveAccountId a 
	   
	   
	   END 
	     
     
		 RETURN 
     END;