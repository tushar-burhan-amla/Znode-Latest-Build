Declare @PimCategoryHierarchyId int, @PimCatalogId int

		  Declare Cur_Category_Hierarchy_Delete Cursor For 
		  select PimCategoryHierarchyId , PimCatalogId
			from ZnodePimCategoryHierarchy a where ParentPimCategoryHierarchyId is not null
			and not exists (
			select * from ZnodePimCategoryHierarchy b where b.PimCategoryHierarchyId = a.ParentPimCategoryHierarchyId
			)
		  OPEN Cur_Category_Hierarchy_Delete  
		  FETCH NEXT FROM Cur_Category_Hierarchy_Delete INTO @PimCategoryHierarchyId, @PimCatalogId 

		  WHILE (@@FETCH_STATUS = 0)
		  BEGIN

				EXEC [dbo].[Znode_DeletePimCategoryHierarchy] @PimCategoryHierarchyId = @PimCategoryHierarchyId, @PimCatalogId = @PimCatalogId

				FETCH NEXT FROM Cur_Category_Hierarchy_Delete INTO @PimCategoryHierarchyId, @PimCatalogId 

		  END
		  CLOSE Cur_Category_Hierarchy_Delete
		  DEALLOCATE Cur_Category_Hierarchy_Delete
go

IF NOT EXISTS (SELECT * FROM sys.objects o 
WHERE o.object_id = object_id(N'[dbo].[FK_ZnodePimCategoryHierarchy_ParentPimCategoryHierarchyId]') 
AND OBJECTPROPERTY(o.object_id, N'IsForeignKey') = 1)
BEGIN
ALTER TABLE ZnodePimCategoryHierarchy WITH CHECK ADD CONSTRAINT FK_ZnodePimCategoryHierarchy_ParentPimCategoryHierarchyId
FOREIGN KEY (ParentPimCategoryHierarchyId) REFERENCES ZnodePimCategoryHierarchy (PimCategoryHierarchyId)
END
