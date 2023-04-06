
create  FUNCTION [dbo].[Fn_GetRecurciveCategoryIds_PimCategoryHierarchyIdnew]
(
       @PimCategoryHierarchyId     VARCHAR(2000)
	   ,@PimCatalogId	   INT 
)
RETURNS  @ConvertTableData TABLE (PimCategoryId INT ,ParentPimCategoryId INT ,PimCategoryHierarchyId INT ,ParentPimCategoryHierarchyId INT  )
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
	       SELECT ZPCH.PimCategoryId ,ZPCI.PimCategoryId PimParentCategoryId,NULL ,NULL 
		   FROM ZnodePimCategoryHierarchy   ZPCH 
		   INNER JOIN ZnodePimCategoryHierarchy   ZPCI ON (ZPCI.PimCategoryHierarchyId = ZPCH.ParentPimCategoryHierarchyId)
		   WHERE ZPCH.PimCatalogId = @PimCatalogId
		END 
		ELSE 
		BEGIN 
	   ;With Cte_RecursiveAccountId AS
	    (
	       SELECT PimCategoryId ,PimCategoryHierarchyId ,ParentPimCategoryHierarchyId
		   FROM ZnodePimCategoryHierarchy   ZPCH 
		   WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PimCategoryId TBPC 
		   WHERE  ZPCH.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId OR @PimCategoryHierarchyId = '')
		   AND (ZPCH.PimCatalogId = @PimCatalogId)
		   UNION ALL 
		   SELECT ZPCH.PimCategoryId ,ZPCH.PimCategoryHierarchyId,ZPCH.ParentPimCategoryHierarchyId
		   FROM ZnodePimCategoryHierarchy   ZPCH 
		   INNER JOIN Cte_RecursiveAccountId CTRA ON (CTRA.ParentPimCategoryHierarchyId = ZPCH.PimCategoryHierarchyId )
	      WHERE  (ZPCH.PimCatalogId = @PimCatalogId)
		  )
  	   INSERT INTO @ConvertTableData
	   SELECT   a.PimCategoryId,t.PimCategoryId, a.PimCategoryHierarchyId,t.PimCategoryHierarchyId ParentPimCategoryHierarchyId
	   FROM Cte_RecursiveAccountId a 
	   LEFT JOIN ZnodePimCategoryHierarchy t ON (t.PimCategoryHierarchyId = a.ParentPimCategoryHierarchyId)
	   
	   END 
	     
     
		 RETURN 
     END;