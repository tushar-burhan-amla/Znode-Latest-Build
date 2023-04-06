CREATE PROCEDURE [dbo].[Znode_ImportAttributes](
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
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max);
		DECLARE @GetDate datetime= dbo.Fn_GetDate(), @LocaleId int  ;
		SELECT @LocaleId = DBO.Fn_GetDefaultLocaleId();
		-- Retrive RoundOff Value from global setting 
		DECLARE @InsertPimAtrribute TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, AttributeName varchar(300), AttributeCode varchar(300), AttributeType varchar(300), DisplayOrder int, GUID nvarchar(400)
		
		);
		DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId int ,AttributeTypeId int,AttributeCode nvarchar(300))
		
		SET @SSQL = 'Select RowNumber,AttributeName,AttributeCode,AttributeType,DisplayOrder ,GUID FROM '+@TableName;
		INSERT INTO @InsertPimAtrribute( RowNumber,AttributeName,AttributeCode,AttributeType,DisplayOrder ,GUID)
		EXEC sys.sp_sqlexec @SSQL;


		--@MessageDisplay will use to display validate message for input inventory value  
		DECLARE @AttributeCode TABLE
		( 
		   AttributeCode nvarchar(300)
		);
		INSERT INTO @AttributeCode
			   SELECT AttributeCode
			   FROM ZnodePimAttribute 

		-- Start Functional Validation 
		-----------------------------------------------
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '10', 'AttributeCode', AttributeCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE ii.AttributeCode in 
			   (
				   SELECT AttributeCode FROM @AttributeCode  where AttributeCode is not null 
			   );
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '53', 'AttributeCode', AttributeCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE ii.AttributeCode in 
			   (
				   select AttributeCode  FROM @InsertPimAtrribute  Group BY AttributeCode  having count(*) > 1 
			   );

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '103', 'AttributeType', AttributeType, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE ii.AttributeType NOT in 
			   (
				   SELECT AttributeTypeName  FROM ZnodeAttributeType  where IsPimAttributeType = 1 
			   );

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '50', 'AttributeCode', AttributeCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE ltrim(rtrim(isnull(ii.AttributeCode,''))) like '%[^0-9A-Za-z]%' OR ltrim(rtrim(isnull(ii.AttributeCode,''))) NOT LIKE '[^0-9]%'
			   OR ltrim(rtrim(isnull(ii.AttributeCode,''))) like '% %'

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '50', 'AttributeCode', AttributeCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE Isnumeric(ltrim(rtrim(isnull(ii.AttributeCode,'')))) =1

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			SELECT '17', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM @InsertPimAtrribute AS ii
			WHERE (ii.DisplayOrder <> '' OR ii.DisplayOrder IS NOT NULL )AND  ii.DisplayOrder = 0

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '16', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE (ii.DisplayOrder <> '' OR ii.DisplayOrder IS NOT NULL )AND  ii.DisplayOrder > 99999


		UPDATE ZIL
			   SET ZIL.ColumnName =   ZIL.ColumnName + ' [ Attribute - ' + ISNULL(AttributeCode,'') + ' ] '
			   FROM ZnodeImportLog ZIL 
			   INNER JOIN @InsertPimAtrribute IPA ON (ZIL.RowNumber = IPA.RowNumber)
			   WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

		-- End Function Validation 	
		-----------------------------------------------
		-- Delete Invalid Data after functional validatin  
		DELETE FROM @InsertPimAtrribute
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
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM @InsertPimAtrribute
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount ,
		TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- End

		--- Insert data into base table ZnodePimatrribute with their validation 

		INSERT INTO ZnodePimAttribute (AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined
			,IsConfigurable,IsPersonalizable,IsShowOnGrid,DisplayOrder,HelpDescription,IsCategory,IsHidden,IsSwatch,
			CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		
		OUTPUT Inserted.PimAttributeId,Inserted.AttributeTypeId,Inserted.AttributeCode INTO @InsertedPimAttributeIds  
		
		SELECT ZAT.AttributeTypeId,AttributeCode, 0 IsRequired , 1 IsLocalizable,1 IsFilterable, 0 IsSystemDefined, 0 IsConfigurable,
		0 IsPersonalizable,  0 IsShowOnGrid , Case when Isnull(DisplayOrder,0) = 0 then  999 else DisplayOrder end  , '' HelpDescription ,0  IsCategory , 0 IsHidden , 0 IsSwatch,
		@UserId , @GetDate ,@UserId , @GetDate from @InsertPimAtrribute IPA INNER JOIN ZnodeAttributeType ZAT 
		ON IPA.AttributeType = ZAT.AttributeTypeName  
		
		INSERT INTO ZnodePimAttributeLocale (LocaleId,PimAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		Select @LocaleId ,IPAS.PimAttributeId, IPA.AttributeName, '', @UserId , @GetDate ,@UserId , @GetDate   
		 FROM @InsertedPimAttributeIds IPAS INNER JOIN @InsertPimAtrribute IPA ON IPAS.AttributeCode= IPA.AttributeCode 
		
		INSERT INTO ZnodePimAttributeValidation
		(PimAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT IPA.PimAttributeId,ZAIV.InputValidationId,NULL,null , @UserId , @GetDate ,@UserId , @GetDate  
		FROM @InsertedPimAttributeIds IPA INNER JOIN ZnodeAttributeInputValidation ZAIV ON IPA.AttributeTypeId = ZAIV.AttributeTypeId

		insert into ZnodePimFrontendProperties (PimAttributeId,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		Select PimAttributeId, 0 IsComparable, 0 IsUseInSearch,0 IsHtmlTags,0 IsFacets, @UserId CreatedBy,@GetDate CreatedDate, @UserId ModifiedBy, @GetDate ModifiedDate
		from  @InsertedPimAttributeIds
		--      SET @Status = 1;
		
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
		
		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportAttributes @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@PimCatalogId='+CAST(@PimCatalogId AS VARCHAR(max));


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
		@ProcedureName = 'Znode_ImportAttributes',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

	END CATCH;
END;