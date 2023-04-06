CREATE PROCEDURE [dbo].[Znode_ImportInventory_Ver1](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200))
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import Inventory data 
	--		   Input data in XML format Validate data with all scenario 
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
		DECLARE @RoundOffValue int, @MessageDisplay nvarchar(100), @MessageDisplayForFloat nvarchar(100);
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		-- Retrive RoundOff Value from global setting 
		SELECT @RoundOffValue = FeatureValues
		FROM ZnodeGlobalSetting
		WHERE FeatureName = 'InventoryRoundOff';
		
		IF OBJECT_ID('tempdb.dbo.#InserInventoryForValidation', 'U') IS NOT NULL 
		DROP TABLE tempdb.dbo.#InserInventoryForValidation
		
		IF OBJECT_ID('tempdb.dbo.#InsertInventory ', 'U') IS NOT NULL 
		DROP TABLE tempdb.dbo.#InsertInventory 

		--@MessageDisplay will use to display validate message for input inventory value  

		DECLARE @sSql nvarchar(max);
		SET @sSql = ' Select @MessageDisplay_new = Convert(Numeric(28, '+CONVERT(nvarchar(200), @RoundOffValue)+'), 123.12345699 ) ';
		EXEC SP_EXecutesql @sSql, N'@MessageDisplay_new NVARCHAR(100) OUT', @MessageDisplay_new = @MessageDisplay OUT;
		SET @sSql = ' Select @MessageDisplay_new = Convert(Numeric(28, '+CONVERT(nvarchar(200), @RoundOffValue)+'), 0.999999 ) ';
		EXEC SP_EXecutesql @sSql, N'@MessageDisplay_new NVARCHAR(100) OUT', @MessageDisplay_new = @MessageDisplayForFloat OUT;
		Create TABLE tempdb..#InserInventoryForValidation 
		( 
				RowNumber int, SKU varchar(600), Quantity varchar(100), ReOrderLevel varchar(100), WarehouseCode varchar(300), GUID nvarchar(200)
		);

		CREATE INDEX IDX_#InserInventoryForValidation_SKU ON #InserInventoryForValidation(SKU)
		
		DECLARE @InventoryListId int;
		SET @SSQL = 'Select RowNumber,SKU,Quantity,ReOrderLevel,WarehouseCode ,GUID FROM '+@TableName;
		INSERT INTO tempdb..#InserInventoryForValidation( RowNumber, SKU, Quantity, ReOrderLevel, WarehouseCode, GUID )
		EXEC sys.sp_sqlexec @SSQL;
		

		UPDATE tempdb..#InserInventoryForValidation
		  SET ReOrderLevel = 0
		WHERE ISNULL(ReOrderLevel,'') = '';

		-- start Functional Validation 
		-----------------------------------------------
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '98', 'SKU', SKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM tempdb..#InserInventoryForValidation  AS ii
		WHERE NOT EXISTS( SELECT * FROM ZnodePimAttributeValue  AS a with(nolock)
			INNER JOIN ZnodePimAttributeValueLocale AS b with(nolock) ON a.PimAttributeValueId = b.PimAttributeValueId
			WHERE exists(Select top 1 PimAttributeId from ZnodePimAttribute zpa Where zpa.AttributeCode = 'SKU' and a.PimAttributeId = zpa.PimAttributeId)
			AND b.AttributeValue = ii.SKU);

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '107', 'WarehouseCode', WarehouseCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM tempdb..#InserInventoryForValidation  AS ii
		WHERE NOT EXISTS ( SELECT TOP 1 1 FROM ZnodeWarehouse AS zw WHERE zw.WarehouseCode = ii.WarehouseCode );

		UPDATE ZIL SET ZIL.ColumnName =   ZIL.ColumnName + ' [ SKU - ' + ISNULL(SKU,'') + ' ] '
		FROM ZnodeImportLog ZIL 
		INNER JOIN #InserInventoryForValidation IPA ON (ZIL.RowNumber = IPA.RowNumber)
		WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

		-----------------------------------------------
		--- Delete Invalid Data after functional validatin  
		DELETE FROM tempdb..#InserInventoryForValidation 
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId AND 
				  GUID = @NewGUID
		);
		
		DECLARE @TBL_ReadyToInsertInventory TABLE
		( 
			RowNumber int, SKU varchar(300), Quantity numeric(28, 6), ReOrderLevel numeric(28, 6), WarehouseId int
		);

		--deleting duplicate rows
		delete  ii
		FROM tempdb..#InserInventoryForValidation  ii
		where ii.RowNumber not IN
			   (
				   SELECT MAX(ii1.RowNumber)
				   FROM tempdb..#InserInventoryForValidation  AS ii1
				   WHERE ii1.WarehouseCode = ii.WarehouseCode AND 
						 ii1.SKU = ii.SKU
			   );

		-- Update Record count in log 
        DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM #InserInventoryForValidation
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount,
		TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- End

		--select 'update started'  
		UPDATE zi SET Quantity = rtii.Quantity, ReOrderLevel = ISNULL(rtii.ReOrderLevel, 0), ModifiedBy = @UserId, ModifiedDate = @GetDate
		FROM ZNodeInventory zi
		INNER JOIN #InserInventoryForValidation rtii ON( zi.SKU = rtii.SKU )
		INNER JOIN ZnodeWarehouse AS zw ON rtii.WarehouseCode = zw.WarehouseCode and zi.WarehouseId = zw.WarehouseId;
			   
		--select 'update End'                
		INSERT INTO ZnodeInventory( WarehouseId, SKU, Quantity, ReOrderLevel, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
		SELECT zw.WarehouseId, SKU, Quantity, ISNULL(ReOrderLevel, 0), @UserId, @GetDate, @UserId, @GetDate
		FROM #InserInventoryForValidation AS rtii
		INNER JOIN ZnodeWarehouse AS zw on rtii.WarehouseCode = zw.WarehouseCode
		WHERE NOT EXISTS ( SELECT TOP 1 1 FROM ZnodeInventory AS zi WHERE zi.WarehouseId = zw.WarehouseId AND zi.SKU = rtii.SKU );
		 
		SET @GetDate = dbo.Fn_GetDate();
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

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportInventory_Ver1 @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200));


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
		@ProcedureName = 'Znode_ImportInventory_Ver1',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
END;