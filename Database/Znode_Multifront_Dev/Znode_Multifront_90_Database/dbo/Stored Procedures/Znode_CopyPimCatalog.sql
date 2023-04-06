CREATE PROCEDURE [dbo].[Znode_CopyPimCatalog]
( 
	@CatalogId int,
	@UserId int,
	@CatalogCode varchar(100),
	@CatalogName varchar(500),
	@CopyAllData bit= 0,
	@Status bit OUT
)
AS
/*
	 Summary: Create copy of existing catalog
	 Here copy all data of catalog on the basis of @CopyAllData bit parameter 
	 if true then copy all data other wise not copy all data 	   
	 Three tables are manipulated ZnodePimCatalog create new catalog, 
	 ZnodePimCategoryHierarchy copy the category and ZnodePimCatalogCategory Copy the products 	   
	 
	 Unit Testing   
	 begin tran
	 EXEC Znode_CopyPimCatalog @CatalogId=1,@UserId=2,@CatalogName='test',@CopyAllData=1,@Status=1
	 rollback tran
*/
BEGIN
	BEGIN TRAN CopyPimCatalog;
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @PimCatalogId int; -- hold the newly created pim catalog id 
		DECLARE @ZnodePimCategoryHierarchyTable AS TABLE (PimCategoryHierarchyId INT,PimCatalogId INT ,PimCategoryId INT)
		DECLARE @ParentPimCategoryHierarchyId INT;
		BEGIN
			INSERT INTO ZnodePimCatalog( CatalogName, IsActive, ExternalId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, CatalogCode )
			SELECT @CatalogName, 1, NULL, @UserId, @GetDate, @UserId, @GetDate, @CatalogCode;
		END;
		SET @PimCatalogId = SCOPE_IDENTITY();
		IF @CopyAllData = 1 -- copy all data 

		BEGIN
		    -- here copy the category Hierarchy
			INSERT INTO ZnodePimCategoryHierarchy( PimCatalogId, ParentPimCategoryHierarchyId, PimCategoryId, DisplayOrder, IsActive, ActivationDate, ExpirationDate, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
			OUTPUT INSERTED.PimCategoryHierarchyId, INSERTED.PimCatalogId,INSERTED.PimCategoryId
			INTO @ZnodePimCategoryHierarchyTable
			SELECT @PimCatalogId, ParentPimCategoryHierarchyId, PimCategoryId, DisplayOrder, IsActive, ActivationDate, ExpirationDate, @UserId, @GetDate, @UserId, @GetDate
			FROM ZnodePimCategoryHierarchy
			WHERE PimCatalogId = @CatalogId; 
			
			SELECT @ParentPimCategoryHierarchyId=PimCategoryHierarchyId FROM ZnodePimCategoryHierarchy
			WHERE PimCatalogId=@PimCatalogId and ParentPimCategoryHierarchyId IS NULL

			UPDATE pch
			SET pch.ParentPimCategoryHierarchyId=llpch.PimCategoryHierarchyId
			from ZnodePimCategoryHierarchy pch
			LEFT JOIN ZnodePimCategoryHierarchy lpch on lpch.PimCategoryHierarchyId=pch.ParentPimCategoryHierarchyId and lpch.PimCatalogId=@CatalogId
			LEFT JOIN ZnodePimCategoryHierarchy llpch on llpch.PimCategoryId=lpch.PimCategoryId AND ISNULL(llpch.ParentPimCategoryHierarchyId,0) =  ISNULL(lpch.ParentPimCategoryHierarchyId,0) and llpch.PimCatalogId=pch.PimCatalogId
			where pch.PimCatalogId=@PimCatalogId
			AND pch.ParentPimCategoryHierarchyId IS NOT NULL
 
		END;
		SELECT @PimCatalogId AS ID, CAST(CASE
										 WHEN @PimCatalogId IS NULL THEN 0
										 ELSE 1
										 END AS bit) AS [Status]; 
		SET @Status = 1;
	COMMIT TRAN CopyPimCatalog;
	END TRY
	BEGIN CATCH
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_CopyPimCatalog @CatalogId = '+CAST(@CatalogId AS varchar(100))+' ,@UserId='+CAST(@UserId AS varchar(100))+' ,@CatalogName= '+@CatalogName+',@CopyAllData='+CAST(@CopyAllData AS varchar(50))+',@Status='+CAST(@Status AS varchar(50));
		SELECT @CatalogId AS ID, CAST(0 AS bit) AS [Status];
		SET @Status = 0;
		ROLLBACK TRAN CopyPimCatalog;
		EXEC Znode_InsertProcedureErrorLog
			 @ProcedureName = 'Znode_CopyPimCatalog', 
			 @ErrorInProcedure = @Error_procedure,
			 @ErrorMessage = @ErrorMessage,
			 @ErrorLine = @ErrorLine, 
			 @ErrorCall = @ErrorCall;
	END CATCH;
END;
GO