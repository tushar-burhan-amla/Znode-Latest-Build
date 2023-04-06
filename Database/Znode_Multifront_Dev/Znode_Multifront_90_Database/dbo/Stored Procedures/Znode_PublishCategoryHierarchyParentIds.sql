CREATE Procedure [dbo].[Znode_PublishCategoryHierarchyParentIds]
(@PimCatalogId int )
AS 
Begin
BEGIN TRY
		IF OBJECT_ID('tempdb..#PublishCategoryHierarchy') <> 0
			DROP TABLE #PublishCategoryHierarchy
		IF OBJECT_ID('tempdb..#ZnodePublishCategory') <> 0
			DROP TABLE #ZnodePublishCategory
		IF OBJECT_ID('tempdb..#PublishCategoryHierarchy') <> 0
			DROP TABLE #PublishCategoryHierarchy

		Create Table #PublishCategoryHierarchy (ZnodeCategoryId int ,ZnodeParentCategoryId Varchar(2000))
		Declare @TotalCategorycount int , @iCount int  =1,
		@PimCategoryId int ,@PublishCategoryId  int ,@PublishCatalogId INT

		SELECT @PublishCatalogId = PublishCatalogId FROM ZnodePublishCatalog AS ZPC WHERE ZPC.PimCatalogId = @PimCatalogId
		
		Select *, dense_Rank() Over (Order By PublishCategoryID) id  into #ZnodePublishCategory  from ZnodePublishCategory where PublishCatalogId =@PublishCatalogId
		Select @TotalCategorycount  = Max(id)  from #ZnodePublishCategory 
	
		while @iCount <=  @TotalCategorycount  
		Begin
			IF OBJECT_ID('tempdb..#ZnodePublishCategoryResult') <>0
			DROP TABLE #ZnodePublishCategoryResult
			--Select *From ZnodePimCategoryHierarchy
			Select @PimCategoryId = PimCategoryId , @PublishCategoryId = PublishCategoryId from #ZnodePublishCategory where id = @iCount
			;WITH CTE_ZnodePimCategoryHierarchy as 
			(
				SELECT PimCategoryHierarchyId,ParentPimCategoryHierarchyId,PimCategoryId,PimCatalogId
				FROM ZnodePimCategoryHierarchy 
				WHERE PimCatalogId=@PimCatalogId		)
			,Hierarchy AS
			(
			-- Anchor
			SELECT  A.PimCategoryHierarchyId,A.ParentPimCategoryHierarchyId,A.PimCategoryId,A.PimCatalogId 
			FROM CTE_ZnodePimCategoryHierarchy A
			WHERE A.ParentPimCategoryHierarchyId IS Not NULL AND  
			A.PimCategoryId =@PimCategoryId AND
			A.PimCatalogId=@PimCatalogId
			UNION ALL
			-- Recursive query
			SELECT  E.PimCategoryHierarchyId,E.ParentPimCategoryHierarchyId,E.PimCategoryId,E.PimCatalogId
			FROM CTE_ZnodePimCategoryHierarchy E
			JOIN Hierarchy H ON H.ParentPimCategoryHierarchyId = E.PimCategoryHierarchyId
			--WHERE  Exists(Select * From #SplitedCategoryHierarchy X Where E.PimCategoryId = X.PimCategoryId)
		)
		 Select @PublishCategoryId PublishCategoryId , B.PublishCategoryId PublishParentCategoryId into #ZnodePublishCategoryResult from Hierarchy A Inner join #ZnodePublishCategory  B on A.PimCategoryId = B.PimCategoryId
		 where B.PimCategoryId is not null 
		 if exists (Select TOP 1 1 from #ZnodePublishCategoryResult)
		 Begin
			insert into #PublishCategoryHierarchy  
			 Select A.PublishCategoryId, 
			 ISNULL(SUBSTRING((SELECT ','+CAST(PublishParentCategoryId AS VARCHAR(1000)) FROM #ZnodePublishCategoryResult AS ZPC 
			 where  A.PublishCategoryId =ZPC.PublishCategoryId
												  GROUP BY ZPC.PublishParentCategoryId FOR XML PATH('') ), 2, 4000), '') PublishParentCategoryId
												  from #ZnodePublishCategoryResult A  where  A.PublishCategoryId is not null 
												  Group by A.PublishCategoryId
		 End
		SET @iCount  = @iCount + 1 
		END
		--SELECT B.ZnodeCategoryIds, B.ZnodeParentCategoryIds, A.ZnodeParentCategoryId FROM #PublishCategoryHierarchy A INNER JOIN ZnodePublishProductEntity B ON 
		--A.ZnodeCategoryId = B.ZnodeCategoryIds WHERE B.ZnodeCatalogId =@PimCatalogId
		Update B Set B.ZnodeParentCategoryIds = A.ZnodeParentCategoryId from #PublishCategoryHierarchy A Inner join 
		ZnodePublishProductEntity B On A.ZnodeCategoryId = B.ZnodeCategoryIds 
		where B.ZnodeCatalogId = @PublishCatalogId

		UPDATE ZnodePublishProductEntity SET ZnodeParentCategoryIds = ZnodeCategoryIds WHERE ZnodeCatalogId = @PublishCatalogId
		AND  ZnodeParentCategoryIds IS NULL
		
END TRY   
BEGIN CATCH   
	
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),   
	@ErrorLine VARCHAR(100)= ERROR_LINE(),  
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PublishCategoryHierarchyParentIds   
	@PimCatalogId = '+CAST(@PimCatalogId  AS VARCHAR (max))                   
   
                        
	EXEC Znode_InsertProcedureErrorLog  
	@ProcedureName = 'Znode_PublishCategoryHierarchyParentIds',  
	@ErrorInProcedure = @Error_procedure,  
	@ErrorMessage = @ErrorMessage,  
	@ErrorLine = @ErrorLine,  
	@ErrorCall = @ErrorCall;  
END CATCH

End

