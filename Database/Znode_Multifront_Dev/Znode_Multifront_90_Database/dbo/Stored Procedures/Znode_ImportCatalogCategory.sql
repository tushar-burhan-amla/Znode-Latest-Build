CREATE PROCEDURE [dbo].[Znode_ImportCatalogCategory](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200), @PimCatalogId int= 0)
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import Catalog Category Product association
	
	-- Unit Testing : 
	--BEGIN TRANSACTION;
	--update ZnodeGlobalSetting set FeatureValues = '5' WHERE FeatureName = 'InventoryRoundOff' 
	--    DECLARE @status INT;
	--    EXEC [Znode_ImportInventory] @InventoryXML = '<ArrayOfImportInventoryModel>
	-- <ImportInventoryModel>
	--   <SKU>S1002</SKU>
	--   <Quantity>999998.33</Quantity>
	--   <ReOrderLevel>10</ReOrderLevel>
	--   <RowNumber>1</RowNumber>
	--   <ListCode>TestInventory</ListCode>
	--   <ListName>TestInventory</ListName>
	-- </ImportInventoryModel>
	--</ArrayOfImportInventoryModel>' , @status = @status OUT , @UserId = 2;
	--    SELECT @status;
	--    ROLLBACK TRANSACTION;
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max);
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		DECLARE @DefaultLocaleId INT=dbo.Fn_GetDefaultLocaleId()

		-- Retrive RoundOff Value from global setting 
		DECLARE @InsertCatalogCategory TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, SKU varchar(300), CategoryCode varchar(200), DisplayOrder int, IsActive varchar(10), GUID nvarchar(400)
			--,Index Ind_SKU1 (SKU),Index Ind_CategoryName (CategoryName)
		);

		DECLARE @CategoryAttributId int;

		SET @CategoryAttributId =
		(
			SELECT TOP 1 PimAttributeId
			FROM ZnodePimAttribute AS ZPA
			WHERE ZPA.AttributeCode = 'CategoryCode' AND 
				  ZPA.IsCategory = 1
		);

		DECLARE @InventoryListId int;

		SET @SSQL = 'Select RowNumber,trim(SKU),CategoryCode,DisplayOrder ,IsActive,GUID FROM '+@TableName;
		INSERT INTO @InsertCatalogCategory( RowNumber, SKU, CategoryCode, DisplayOrder, IsActive, GUID )
		EXEC sys.sp_sqlexec @SSQL;

		----Removing Duplicate data
		;with cte as
		(
			select SKU, CategoryCode,max(RowNumber) as RowNumber from @InsertCatalogCategory
			group by SKU, CategoryCode
		)
		delete a from @InsertCatalogCategory a
		where  not exists (select * from CTE b where a.RowNumber = b.RowNumber )

		--@MessageDisplay will use to display validate message for input inventory value  
		DECLARE @SKU TABLE
		( 
		   SKU nvarchar(300), PimProductId INT--, Index Ins_SKU (SKU)
		);
		INSERT INTO @SKU
			   SELECT b.AttributeValue, a.PimProductId
			   FROM ZnodePimAttributeValue AS a
					INNER JOIN
					ZnodePimAttributeValueLocale AS b
					ON a.PimAttributeId = dbo.Fn_GetProductSKUAttributeId() AND 
					   a.PimAttributeValueId = b.PimAttributeValueId AND b.LocaleId=@DefaultLocaleId;

		Declare @PimCategoryAttributeId int 
		set @PimCategoryAttributeId = (select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'CategoryCode')

		DECLARE @CategoryCode TABLE
		( 
			CategoryCode nvarchar(300), PimCategoryId int --index ind_101 (CategoryName)
		);
		INSERT INTO @CategoryCode
			   SELECT ZPCAL.CategoryValue, ZPCA.PimCategoryId
			   FROM ZnodePimCategoryAttributeValue AS ZPCA
					INNER JOIN
					ZnodePimCategoryAttributeValueLocale AS ZPCAL
					ON ZPCA.PimAttributeId = @PimCategoryAttributeId AND 
					ZPCA.PimCategoryAttributeValueId = ZPCAL.PimCategoryAttributeValueId;
					
		-- start Functional Validation 
		
		-----------------------------------------------
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '98', 'SKU', SKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertCatalogCategory AS ii
			   WHERE ii.SKU NOT in 
			   (
				   SELECT SKU FROM @SKU  where SKU IS NOT NULL 
			   );
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '105', 'CategoryCode', CategoryCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertCatalogCategory AS ii
			   WHERE ii.CategoryCode NOT IN 
			   (
				   SELECT CategoryCode FROM @CategoryCode  where CategoryCode IS NOT NULL 
			   );
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			SELECT '17', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM @InsertCatalogCategory AS ii
			WHERE (ii.DisplayOrder <> '' OR ii.DisplayOrder IS NOT NULL )AND  ii.DisplayOrder = 0

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			SELECT '115', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM @InsertCatalogCategory AS ii
			WHERE (ii.DisplayOrder <> '' OR ii.DisplayOrder IS NOT NULL )AND  ii.DisplayOrder > 999

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
		  SELECT '68', 'IsActive', IsActive, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
		  FROM @InsertCatalogCategory AS ii  
		  WHERE ii.IsActive not in ('True','1','Yes','FALSE','0','No')

		  
		
		UPDATE ZIL
			   SET ZIL.ColumnName =   ZIL.ColumnName + ' [ CategoryCode - ' + ISNULL(CategoryCode,'') + ' ] '
			   FROM ZnodeImportLog ZIL 
			   INNER JOIN @InsertCatalogCategory IPA ON (ZIL.RowNumber = IPA.RowNumber)
			   WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL


		-- End Function Validation 	
		-----------------------------------------------
		--- Delete Invalid Data after functional validatin  
		DELETE FROM @InsertCatalogCategory
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  AND RowNumber IS NOT NULL
		);

	
		
			  Declare @ZnodePimCategoryProduct TABLE (PimProductId int , PimCategoryId int , Status bit , DisplayOrder int) 
			  	
			  insert into @ZnodePimCategoryProduct (PimProductId , PimCategoryId , Status , DisplayOrder )
			  SELECT SKU.PimProductId, (Select top 1 PimCategoryId from @CategoryCode where ICC.CategoryCode = CategoryCode )  PimCategoryId
				 , CASE WHEN IsActive in ('True','1','Yes') Then 1 ELSE 0 END , DisplayOrder FROM @InsertCatalogCategory AS ICC INNER JOIN	 @SKU AS SKU ON ICC.SKU = SKU.SKU 
			
			  INSERT into ZnodePimCategoryProduct ( PimProductId, PimCategoryId, Status, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) 
			  Select TABL.PimProductId, TABL.PimCategoryId, TABL.Status, TABL.DisplayOrder,@UserId, @GetDate, @UserId, @GetDate   from @ZnodePimCategoryProduct TABL    
			  Where NOT EXISTS (Select top 1 1 from ZnodePimCategoryProduct ZPCP where ZPCP.PimProductId = TABL.PimProductId and  ZPCP.PimCategoryId = TABL.PimCategoryId)

		SET @GetDate = dbo.Fn_GetDate();
		-- Update Record count in log 
        DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM @InsertCatalogCategory
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount, TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
		WHERE ImportProcessLogId = @ImportProcessLogId;
		
		--Updating the import process status
		UPDATE ZnodeImportProcessLog
		SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
							WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
							WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
						END, 
			ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN A;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN A;

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();
 
		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportCatalogCategory @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@PimCatalogId='+CAST(@PimCatalogId AS VARCHAR(max));


		---Import process updating fail due to database error
		UPDATE ZnodeImportProcessLog
		SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		---Loging error for Import process due to database error
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '93', '', '', @NewGUId,  @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

		--Updating total and fail record count
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
		TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) Where ImportProcessLogId = @ImportProcessLogId)
		WHERE ImportProcessLogId = @ImportProcessLogId;

		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_ImportCatalogCategory',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
		
		
	END CATCH;
END;

