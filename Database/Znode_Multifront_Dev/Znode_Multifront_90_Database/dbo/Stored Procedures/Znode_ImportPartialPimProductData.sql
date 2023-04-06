CREATE PROCEDURE [dbo].[Znode_ImportPartialPimProductData]
(
	@TableName			VARCHAR(200),
	@NewGUID			NVARCHAR(200),
	@TemplateId			NVARCHAR(200),
	@ImportProcessLogId INT,
	@UserId				INT,
	@LocaleId			INT,
	@DefaultFamilyId	INT
)
AS 
/*
	Summary : Finally Import data into ZnodePimProduct, ZnodePimAttributeValue and ZnodePimAttributeValueLocale Table 
	Process : Flat global temporary table will split into cloumn wise and associted with Znode Attributecodes,
				Create group of product with their attribute code and values and inserted one by one products.
	
	SourceColumnName : CSV file column headers
	TargetColumnName : Attributecode from ZnodePimAttribute Table 

	*** Need to log error if transaction failed during insertion of records into table.
*/
BEGIN
	SET NOCOUNT ON
	BEGIN TRY
		--BEGIN TRAN ImportProducts;
		DECLARE @SQLQuery NVARCHAR(MAX);
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @AttributeTypeName NVARCHAR(10), @AttributeCode NVARCHAR(300), @AttributeId INT, @IsRequired BIT, @SourceColumnName NVARCHAR(600), @PimAttributeFamilyId INT, @NewProductId INT, @PimAttributeValueId INT, @status BIT= 0; 
		--Declare error Log Table 


		DECLARE @FamilyAttributeDetail TABLE
		( 
		PimAttributeId int, AttributeTypeName varchar(300), AttributeCode varchar(300), SourceColumnName nvarchar(600), IsRequired bit, PimAttributeFamilyId int
		);
		SET @DefaultFamilyId = 0 

		IF @DefaultFamilyId = 0
		BEGIN
			INSERT INTO @FamilyAttributeDetail( PimAttributeId, AttributeTypeName, AttributeCode, SourceColumnName, IsRequired, PimAttributeFamilyId )
			--Call Process to insert data of defeult family with cource column name and target column name 
			--EXEC Znode_ImportGetTemplateDetails @TemplateId = @TemplateId, @IsValidationRules = 0, @IsIncludeRespectiveFamily = 1,@DefaultFamilyId = @DefaultFamilyId;
			--UPDATE @FamilyAttributeDetail SET PimAttributeFamilyId = DBO.Fn_GetCategoryDefaultFamilyId();
			SELECT distinct zpa.PimAttributeId, zat.AttributeTypeName, zpa.AttributeCode, zitm.SourceColumnName, zpa.IsRequired ,0
			FROM dbo.ZnodePimAttribute AS zpa INNER JOIN dbo.ZnodeAttributeType AS zat ON zat.AttributeTypeId = zpa.AttributeTypeId 
			LEFT OUTER JOIN dbo.ZnodeImportTemplateMapping AS zitm
			ON zpa.AttributeCode = zitm.SourceColumnName AND zitm.ImportTemplateId = @TemplateId
			WHERE zpa.IsCategory = 0 
		END;
		--Read all attribute details with their datatype 
		IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '#DefaultAttributeValue')
		BEGIN
			CREATE TABLE #DefaultAttributeValue (AttributeTypeName VARCHAR(300),PimAttributeDefaultValueId INT,PimAttributeId INT,
			AttributeDefaultValueCode VARCHAR(100));
					
			INSERT INTO #DefaultAttributeValue(AttributeTypeName,PimAttributeDefaultValueId,PimAttributeId,AttributeDefaultValueCode)
			--Call Process to insert default data value 
			EXEC Znode_ImportGetPimAttributeDefaultValue;
		END;
		ELSE
		BEGIN
			DROP TABLE #DefaultAttributeValue;
		END;
		EXEC sys.sp_sqlexec @SQLQuery;

		DECLARE @PimProductDetail TABLE 
		(
			PimAttributeId INT, PimAttributeFamilyId INT,ProductAttributeCode VARCHAR(300) NULL,
			ProductAttributeDefaultValueId INT NULL,PimAttributeValueId INT NULL,LocaleId INT,
			PimProductId INT NULL,AttributeValue NVARCHAR(MAX) NULL,AssociatedProducts NVARCHAR(4000) NULL,ConfigureAttributeIds VARCHAR(2000) NULL,
			ConfigureFamilyIds VARCHAR(2000) NULL,RowNumber INT
		);

		-- Update Record count in log 
		DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND ImportProcessLogId = @ImportProcessLogId;
				
		SET @SQLQuery = ' insert into ZnodeImportSuccessLog (ImportedSku,ImportedProductId,ImportedGuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
		Select SKU, PimProductId , ''' + @NewGUID + ''', '+ Convert(nvarchar(100),@UserId) +',''' + 
		Convert(nvarchar(100),@GetDate) + ''', '+ Convert(nvarchar(100),@UserId) +',''' + 
		Convert(nvarchar(100),@GetDate) + ''' from ' + @TableName ;
		EXEC	sp_executesql @SQLQuery

		SET @SQLQuery = ' Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM '+ @TableName ;
		EXEC	sp_executesql @SQLQuery, N'@SuccessRecordCount BIGINT out' , @SuccessRecordCount=@SuccessRecordCount
		
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount ,
			TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- End

			
		-- Column wise split data from source table ( global temporary table ) and inserted into temporary table variable @PimProductDetail
		-- Add PimAttributeDefaultValue 
		DECLARE Cr_AttributeDetails CURSOR LOCAL FAST_FORWARD
		FOR SELECT PimAttributeId,AttributeTypeName,AttributeCode,IsRequired,SourceColumnName,PimAttributeFamilyId FROM @FamilyAttributeDetail WHERE ISNULL(SourceColumnName, '') <> '';
		OPEN Cr_AttributeDetails;
		FETCH NEXT FROM Cr_AttributeDetails INTO @AttributeId, @AttributeTypeName, @AttributeCode, @IsRequired, @SourceColumnName, @PimAttributeFamilyId;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @NewProductId = 0;
			SET @SQLQuery = ' SELECT '''+CONVERT(VARCHAR(100), @PimAttributeFamilyId)+''' PimAttributeFamilyId , PimProductId PimProductId ,'''+CONVERT(VARCHAR(100), @AttributeId)+''' AttributeId ,
							(SELECT TOP 1 PimAttributeDefaultValueId FROM #DefaultAttributeValue Where PimAttributeId = '
							+ CONVERT(VARCHAR(100), @AttributeId)+'AND AttributeDefaultValueCode = TN.['+@SourceColumnName+'] ) PimAttributeDefaultValueId ,['
							+ @SourceColumnName+'],'+CONVERT(VARCHAR(100), @LocaleId)+'LocaleId
									
							, RowNumber FROM '+@TableName+' TN';
			INSERT INTO @PimProductDetail( PimAttributeFamilyId, PimProductId, PimAttributeId, ProductAttributeDefaultValueId, AttributeValue, LocaleId, RowNumber )
			EXEC sys.sp_sqlexec @SQLQuery;
			FETCH NEXT FROM Cr_AttributeDetails INTO @AttributeId, @AttributeTypeName, @AttributeCode, @IsRequired, @SourceColumnName, @PimAttributeFamilyId;
		END;
		CLOSE Cr_AttributeDetails;
		DEALLOCATE Cr_AttributeDetails;

		SELECT * INTO #PimProductDetail FROM @PimProductDetail

		UPDATE a 
		SET ConfigureAttributeIds = SUBSTRING((SELECT ','+CAST(c.PimAttributeId As VARCHAR(100)) 
		FROM #PimProductDetail c 
		INNER JOIN ZnodePimAttribute b ON (b.PimAttributeId = c.PimAttributeId)
		WHERE IsConfigurable =1 AND c.RowNumber = a.RowNumber FOR XML PATH('')),2,4000) 
		FROM #PimProductDetail a 
		WHERE EXISTS (SELECT TOP 1 1 FROM #PimProductDetail ab WHERE ab.RowNumber = a.RowNumber AND	ab.ProductAttributeCode = 'ProductType' 
		AND ab.AttributeValue = 'ConfigurableProduct' )

		-- In case of Yes/No : If value is not TRUE OR 1 then it will be False else True
		--If default Value set not need of hard code for IsActive
		UPDATE ppdti SET ppdti.AttributeValue = CASE WHEN Upper(ISNULL(ppdti.AttributeValue, '')) in ( 'Yes','TRUE','1') THEN 'true' ELSE 'false' END FROM #PimProductDetail ppdti
		INNER JOIN #DefaultAttributeValue dav ON ppdti.PimAttributeId = dav.PimAttributeId WHERE dav.AttributeTypeName = 'Yes/No';

		-- Pass product records in bulk
		DECLARE @PimProductDetailToInsert PIMPRODUCTDETAIL; --User define table type to pass multiple records of product in single step

		INSERT INTO @PimProductDetailToInsert
			(PimAttributeId,PimAttributeFamilyId,ProductAttributeCode,ProductAttributeDefaultValueId,
				PimAttributeValueId,LocaleId,PimProductId,AttributeValue,AssociatedProducts,ConfigureAttributeIds,ConfigureFamilyIds)
		SELECT PimAttributeId,PimAttributeFamilyId,ProductAttributeCode,ProductAttributeDefaultValueId,PimAttributeValueId,LocaleId,
			PimProductId,AttributeValue,AssociatedProducts,ConfigureAttributeIds,ConfigureFamilyIds 
		FROM #PimProductDetail

		DELETE FROM @PimProductDetailToInsert WHERE RTRIM(LTRIM(AttributeValue)) = '';
	
		IF EXISTS (SELECT TOP 1 1 FROM @PimProductDetailToInsert)
		BEGIN
			EXEC Znode_ImportPartialInsertUpdateProductAttribute @PimProductDetail = @PimProductDetailToInsert,@UserID = @UserID,@status = @status OUT,@IsNotReturnOutput = 1;
		END
		/*
		-- Pass product records one by one 
		DECLARE @IncrementalId INT= 1;
		DECLARE @SequenceId INT=
		(
			SELECT MAX(RowNumber) FROM #PimProductDetail
		);

		DECLARE @PimProductDetailToInsert PIMPRODUCTDETAIL;  --User define table type to pass multiple records of product in single step
		
		WHILE @IncrementalId <= @SequenceId
		BEGIN
			INSERT INTO @PimProductDetailToInsert(PimAttributeId,PimAttributeFamilyId,ProductAttributeCode,ProductAttributeDefaultValueId,
			PimAttributeValueId,LocaleId,PimProductId,AttributeValue,AssociatedProducts,ConfigureAttributeIds,ConfigureFamilyIds)
			SELECT PimAttributeId,PimAttributeFamilyId,ProductAttributeCode,ProductAttributeDefaultValueId,PimAttributeValueId,LocaleId,
			PimProductId,AttributeValue,AssociatedProducts,ConfigureAttributeIds,ConfigureFamilyIds FROM #PimProductDetail
			WHERE [#PimProductDetail].RowNumber = @IncrementalId; --AND RTRIM(LTRIM(AttributeValue)) <> '';

			Delete from @PimProductDetailToInsert where RTRIM(LTRIM(AttributeValue)) = '';
			--ORDER BY [@PimProductDetail].RowNumber;
			----Call process to finally insert data into 
			----------------------------------------------------------
			--1. [dbo].[ZnodePimProduct]
			--2. [dbo].[ZnodePimAttributeValue]
			--3. [dbo].[ZnodePimAttributeValueLocale]
			if Exists (select TOP 1 1 from @PimProductDetailToInsert)
				EXEC [Znode_ImportPartialInsertUpdatePimProduct] @PimProductDetail = @PimProductDetailToInsert,@UserID = @UserID,@status = @status OUT,@IsNotReturnOutput = 1;
				
			DELETE FROM @PimProductDetailToInsert;
			SET @IncrementalId = @IncrementalId + 1;
		END;
		*/

		SET @GetDate = dbo.Fn_GetDate();
		--Updating the import process status
		UPDATE ZnodeImportProcessLog
		SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
							WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
							WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
						END, 
			ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- COMMIT TRAN ImportProducts;
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE(),ERROR_LINE(),ERROR_PROCEDURE();

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportPartialPimProductData @TableName = '+CAST(@TableName AS VARCHAR(max))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200)) +',@TemplateId='+CAST(@TemplateId AS VARCHAR(200)) +',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(10))+',@DefaultFamilyId='+CAST(@DefaultFamilyId AS VARCHAR(200));
			---Import process updating fail due to database error
		
		UPDATE ZnodeImportProcessLog
		SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		---Loging error for Import process due to database error
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '93', '', '', @NewGUId, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

		--Updating total and fail record count
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
			TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) Where ImportProcessLogId = @ImportProcessLogId)
		WHERE ImportProcessLogId = @ImportProcessLogId;

		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ImportPartialPimProductData',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;

		ROLLBACK TRAN ImportProducts;
	END CATCH;
END;