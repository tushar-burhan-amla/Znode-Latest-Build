CREATE PROCEDURE [dbo].[Znode_ImportAddonAssociation](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200), @PimCatalogId int= 0)
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import Attribute Code Name and their default input validation rule other 
	--			  flag will be inserted as default we need to modify front end
	
	-- Unit Testing: 

	--------------------------------------------------------------------------------------
BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @SSQL nvarchar(max);
		DECLARE @GetDate DATETIME= dbo.Fn_GetDate(), @LocaleId int  ;
		SELECT @LocaleId = DBO.Fn_GetDefaultLocaleId();

		IF OBJECT_ID('TEMPDB..#InsertProductAddonAssociation') IS NOT NULL
			DROP TABLE #InsertProductAddonAssociation

		IF OBJECT_ID('TEMPDB..#SKU') IS NOT NULL
			DROP TABLE #SKU

		IF OBJECT_ID('TEMPDB..#InsertZnodePimAddOnProduct') IS NOT NULL
			DROP TABLE #InsertZnodePimAddOnProduct

		CREATE TABLE #InsertZnodePimAddOnProduct ( PimAddOnProductId int,PimAddonGroupId int,PimProductId int)

		CREATE TABLE #InsertProductAddonAssociation
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, SKU varchar(300),AddonGroupName varchar(300),AddOnSKU varchar(300),DisplayOrder int, IsDefault varchar(10), GUID nvarchar(400)
		
		);

		CREATE TABLE #SKU 
		( 
			SKU nvarchar(300), PimProductId int
		);

		----Get All SKU Data From DB
		INSERT INTO #SKU (SKU, PimProductId)
		SELECT ZPAVL.AttributeValue, ZPAV.PimProductId
		FROM ZnodePimAttributeValue AS ZPAV
		INNER JOIN ZnodePimAttributeValueLocale AS ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
		INNER JOIN ZnodePimAttribute ZPA ON ZPAV.PimAttributeId = ZPA.PimAttributeId
		WHERE ZPA.AttributeCode = 'SKU';

		SET @SSQL = 'Select RowNumber,SKU,AddonGroupName,AddOnSKU,DisplayOrder,IsDefault,GUID FROM '+@TableName;
		INSERT INTO #InsertProductAddonAssociation( RowNumber,SKU,AddonGroupName,AddOnSKU,DisplayOrder,IsDefault,GUID )
		EXEC sys.sp_sqlexec @SSQL;

		UPDATE #InsertProductAddonAssociation  
		SET IsDefault=CASE ISNULL(IsDefault,0) WHEN 'YES' THEN 1 WHEN 'NO' THEN 0 END

		SELECT SKU,AddonGroupName,AddOnSKU 
		INTO #DuplicateProductAddonAssociation 
		FROM #InsertProductAddonAssociation 
		Group BY SKU,AddonGroupName,AddOnSKU  having count(*) > 1

		----Checking AddonGroupName present in DB
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '99', 'AddonGroupName', AddonGroupName, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAddonAssociation AS ii
		WHERE NOT EXISTS ( SELECT * FROM ZnodePimAddonGroupLocale i where i.AddonGroupName = ii.AddonGroupName );

		----Checking SKU present in DB
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '98', 'SKU', SKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAddonAssociation AS ii
		WHERE NOT EXISTS( SELECT * FROM #SKU SKU WHERE ii.SKU = SKU.SKU)

		----Checking AddOnSKU present in DB
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '98', 'AddOnSKU', AddOnSKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAddonAssociation AS ii
		WHERE NOT EXISTS( SELECT * FROM #SKU SKU WHERE ii.AddOnSKU = SKU.SKU)
		
		----Duplicate Record (SKU \ AddOnSKU \ AddonGroupName)
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '53', 'SKU \ AddOnSKU \ AddonGroupName ', SKU+' \ '+AddOnSKU+' \ '+AddonGroupName, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAddonAssociation AS ii
		WHERE EXISTS ( select * FROM #DuplicateProductAddonAssociation i WHERE i.SKU = ii.SKU and i.AddOnSKU = ii.AddOnSKU and i.AddonGroupName = ii.AddonGroupName );

		----Checking is default value data validation
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
		SELECT '68', 'IsDefault', IsDefault, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
		FROM #InsertProductAddonAssociation AS ii  
		WHERE ISNULL(ii.IsDefault,0) not in ('True','1','Yes','FALSE','0','No','')

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			SELECT '115', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			FROM #InsertProductAddonAssociation AS ii
			WHERE (ii.DisplayOrder <> '' OR ii.DisplayOrder IS NOT NULL )AND  ii.DisplayOrder > 999

		UPDATE ZIL
			   SET ZIL.ColumnName =   ZIL.ColumnName + ' [ SKU - ' + isnull(SKU,'') + ' ] '
			   FROM ZnodeImportLog ZIL 
			   INNER JOIN #InsertProductAddonAssociation IPA ON (ZIL.RowNumber = IPA.RowNumber)
			   WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

		-- Delete Invalid Data after functional validatin  
		DELETE FROM #InsertProductAddonAssociation
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
		);
		
		-- Update Record count in log 
        DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM #InsertProductAddonAssociation
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount
		,TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))  
		WHERE ImportProcessLogId = @ImportProcessLogId;    
		
		UPDATE PAPD
		SET PAPD.ModifiedBy =@UserId,PAPD.ModifiedDate = @GetDate,
		PAPD.IsDefault = CASE WHEN ISNULL(IPAA.IsDefault,'') <>'' THEN IPAA.IsDefault ELSE PAPD.IsDefault END,
		PAPD.DisplayOrder = CASE WHEN ISNULL(IPAA.DisplayOrder,'') <>'' THEN IPAA.DisplayOrder ELSE PAPD.DisplayOrder END 
		FROM #InsertProductAddonAssociation IPAA   
		INNER JOIN #SKU SKU ON SKU.SKU = IPAA.SKU   
		INNER JOIN #SKU SKU1 ON SKU1.SKU = IPAA.AddOnSKU   
		INNER JOIN ZnodePimAddonGroupLocale ZPAGL on ZPAGL.AddonGroupName = IPAA.AddonGroupName   
		INNER JOIN ZnodePimAddOnProduct PAP ON (PAP.PimProductId = SKU.PimProductId AND PAP.PimAddonGroupId = ZPAGL.PimAddonGroupId)
		INNER JOIN ZnodePimAddOnProductDetail PAPD ON (PAPD.PimAddOnProductId =PAP.PimAddOnProductId AND SKU1.PimProductId = PAPD.PimChildProductId )
		
		-----Inserting records in ZnodePimAddOnProduct
		INSERT INTO ZnodePimAddOnProduct(PimAddonGroupId,PimProductId,DisplayOrder,RequiredType,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		OUTPUT inserted.PimAddOnProductId, inserted.PimAddonGroupId,inserted.PimProductId into #InsertZnodePimAddOnProduct (PimAddOnProductId,PimAddonGroupId,PimProductId)
		SELECT DISTINCT ZPAGL.PimAddonGroupId,SKU.PimProductId, 999 as DisplayOrder,
		'Required' RequiredType,@UserId as CreatedBy,@GetDate,@UserId as ModifiedBy,@GetDate
		FROM #InsertProductAddonAssociation IPAA
		INNER JOIN #SKU SKU ON SKU.SKU = IPAA.SKU
		INNER JOIN ZnodePimAddonGroupLocale ZPAGL on ZPAGL.AddonGroupName = IPAA.AddonGroupName
		WHERE NOT EXISTS(SELECT * FROM ZnodePimAddOnProduct ZPAP where ZPAP.PimAddonGroupId = ZPAGL.PimAddonGroupId and ZPAP.PimProductId = SKU.PimProductId )

		-----Inserting records in ZnodePimAddOnProductDetail
		INSERT INTO ZnodePimAddOnProductDetail(PimAddOnProductId,PimChildProductId,IsDefault,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT DISTINCT ZPAP.PimAddOnProductId, SKU.PimProductId as PimChildProductId, CASE WHEN isnull(IPAA.IsDefault,0) IN ('True','1','Yes') THEN 1 ELSE 0 END, ISNULL(IPAA.DisplayOrder,999),@UserId ,@GetDate,@UserId,@GetDate 
		FROM #InsertProductAddonAssociation IPAA
		INNER JOIN #SKU SKU1 ON SKU1.SKU = IPAA.SKU
		INNER JOIN #SKU SKU ON SKU.SKU = IPAA.AddOnSKU
		INNER JOIN ZnodePimAddonGroupLocale ZPAGL on IPAA.AddonGroupName = ZPAGL.AddonGroupName
		INNER JOIN ZnodePimAddOnProduct ZPAP on SKU1.PimProductId = ZPAP.PimProductId and ZPAP.PimAddonGroupId = ZPAGL.PimAddonGroupId
		WHERE NOT EXISTS(SELECT * FROM ZnodePimAddOnProductDetail ZPAPD WHERE ZPAPD.PimAddOnProductId = ZPAP.PimAddOnProductId and ZPAPD.PimChildProductId = SKU.PimProductId )

		SET @GetDate = dbo.Fn_GetDate()
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

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportAddonAssociation @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@PimCatalogId='+CAST(@PimCatalogId AS VARCHAR(max));

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
		@ProcedureName = 'Znode_ImportAddonAssociation',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
END;