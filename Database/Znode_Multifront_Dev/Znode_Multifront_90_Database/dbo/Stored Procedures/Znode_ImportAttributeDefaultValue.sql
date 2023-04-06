CREATE PROCEDURE [dbo].[Znode_ImportAttributeDefaultValue](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200))
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
			RowId int IDENTITY(1, 1) PRIMARY KEY,
			RowNumber int, AttributeCode varchar(300),AttributeDefaultValueCode varchar(300),AttributeDefaultValue varchar(1000),IsEditable varchar(10),DisplayOrder int, IsDefault varchar(10), SwatchText varchar(1000),SwatchImage varchar(500), SwatchImagePath varchar(500), GUID nvarchar(400)
		
		);
		DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId int ,PimAttributeDefaultValueId int,AttributeDefaultValueCode nvarchar(300))
		
		SET @SSQL = 'Select RowNumber,AttributeCode,AttributeDefaultValueCode,AttributeDefaultValue,IsEditable,DisplayOrder,IsDefault,SwatchText,SwatchImage, SwatchImagePath ,GUID FROM '+@TableName;
		INSERT INTO @InsertPimAtrribute( RowNumber,AttributeCode,AttributeDefaultValueCode,AttributeDefaultValue,IsEditable,DisplayOrder,IsDefault,SwatchText,SwatchImage, SwatchImagePath ,GUID)
		EXEC sys.sp_sqlexec @SSQL;

		UPDATE @InsertPimAtrribute  
		SET IsDefault=CASE ISNULL(IsDefault,0) WHEN 'YES' THEN 1 WHEN 'NO' THEN 0 END

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
			   SELECT '117', 'AttributeCode', AttributeCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE ii.AttributeCode not in 
			   (
				   SELECT AttributeCode FROM @AttributeCode
			   );

		WITH CTE_AttributeDefaultValueCode AS(
		SELECT AttributeDefaultValueCode , AttributeCode, RowNumber ,ROW_NUMBER ()OVER (PARTITION BY AttributeDefaultValueCode , AttributeCode ORDER BY AttributeDefaultValueCode , AttributeCode )  Row_Id
		FROM @InsertPimAtrribute Z
		
		
		)
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '53', 'AttributeDefaultValueCode', AttributeDefaultValueCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE EXISTS
			  ( SELECT TOP 1 1 FROM CTE_AttributeDefaultValueCode Z WHERE Z.RowNumber=ii.RowNumber AND Z.Row_Id >1
			   )
			  

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '79', 'AttributeDefaultValueCode', AttributeDefaultValueCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE ltrim(rtrim(isnull(ii.AttributeDefaultValueCode,''))) like '%[^0-9A-Za-z]%' OR ltrim(rtrim(isnull(ii.AttributeDefaultValueCode,''))) like '% %'

		

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '68', 'IsEditable', IsEditable, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertPimAtrribute AS ii  
			WHERE ii.IsEditable not in ('True','1','Yes','FALSE','0','No','')

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '68', 'IsDefault', IsDefault, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertPimAtrribute AS ii  
			WHERE ii.IsDefault not in ('True','1','Yes','FALSE','0','No','')

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			SELECT '16', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			FROM @InsertPimAtrribute AS ii
			WHERE (ii.DisplayOrder <> '' OR ii.DisplayOrder IS NOT NULL )AND  ii.DisplayOrder > 99999

		UPDATE ZIL
			SET ZIL.ColumnName =   ZIL.ColumnName + ' [ AttributeDefaultValueCode - ' + ISNULL(AttributeDefaultValueCode,'') + ' ] '
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
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount
		,	TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
		WHERE ImportProcessLogId = @ImportProcessLogId;

		DECLARE @MediaId INT
		SET @MediaId = (SELECT TOP 1 MediaId from @InsertPimAtrribute IPA INNER JOIN ZnodeMedia ZM ON IPA.SwatchImage = ZM.FileName and IPA.SwatchImagePath = ZM.Path)

		if (isnull(@MediaId,0)=0)
			SET @MediaId = (SELECT max(MediaId) from @InsertPimAtrribute IPA INNER JOIN ZnodeMedia ZM ON IPA.SwatchImage = ZM.FileName)

        update ZPADV set ZPADV.IsEditable = case when IPA.IsEditable in ('True','1','Yes') then 1 else 0 end,
		                 ZPADV.DisplayOrder = CASE WHEN ISNULL(IPA.DisplayOrder,'') <> '' then  IPA.DisplayOrder ELSE ZPADV.DisplayOrder END ,
		                 ZPADV.IsDefault = CASE WHEN ISNULL(IPA.IsDefault,'')<>'' THEN IPA.IsDefault ELSE ZPADV.IsDefault END,ZPADV.SwatchText = IPA.SwatchText ,
		                 ZPADV.MediaId = case when isnull(@MediaId,0)= 0 then ZPADV.MediaId else @MediaId end, ZPADV.ModifiedBy = @UserId, ZPADV.ModifiedDate = @GetDate 
		from @InsertPimAtrribute IPA 
		INNER JOIN ZnodePimAttribute ZPA ON IPA.AttributeCode = ZPA.AttributeCode 
		inner join ZnodePimAttributeDefaultValue ZPADV on ZPA.PimAttributeId = ZPADV.PimAttributeId and IPA.AttributeDefaultValueCode = ZPADV.AttributeDefaultValueCode

		update ZPADVL set ZPADVL.AttributeDefaultValue = IPA.AttributeDefaultValue, ZPADVL.ModifiedBy = @UserId, ZPADVL.ModifiedDate = @GetDate 
		from @InsertPimAtrribute IPA 
		INNER JOIN ZnodePimAttribute ZPA ON IPA.AttributeCode = ZPA.AttributeCode 
		inner join ZnodePimAttributeDefaultValue ZPADV on ZPA.PimAttributeId = ZPADV.PimAttributeId and IPA.AttributeDefaultValueCode = ZPADV.AttributeDefaultValueCode
		inner join ZnodePimAttributeDefaultValueLocale ZPADVL ON ( ZPADVL.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId)

		--- Insert data into base table ZnodePimatrribute with their validation 

		INSERT INTO ZnodePimAttributeDefaultValue (PimAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,IsDefault,SwatchText,MediaId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
		OUTPUT Inserted.PimAttributeId,Inserted.PimAttributeDefaultValueId,Inserted.AttributeDefaultValueCode INTO @InsertedPimAttributeIds  		
		SELECT ZPA.PimAttributeId,IPA.AttributeDefaultValueCode, case when IPA.IsEditable in ('True','1','Yes') then 1 else 0 end , Case when Isnull(IPA.DisplayOrder,0) = 0 then  99999 else IPA.DisplayOrder end  , 
		       case when IPA.IsDefault in ('True','1','Yes') then 1 else 0 end , IPA.SwatchText, @MediaId,@UserId , @GetDate ,@UserId , @GetDate 
		from @InsertPimAtrribute IPA 
		INNER JOIN ZnodePimAttribute ZPA ON IPA.AttributeCode = ZPA.AttributeCode  
		where not exists(select * from ZnodePimAttributeDefaultValue ZPADV where ZPA.PimAttributeId = ZPADV.PimAttributeId and IPA.AttributeDefaultValueCode = ZPADV.AttributeDefaultValueCode)
		
		INSERT INTO ZnodePimAttributeDefaultValueLocale(LocaleId,PimAttributeDefaultValueId,AttributeDefaultValue,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		Select @LocaleId ,IPAS.PimAttributeDefaultValueId, IPA.AttributeDefaultValue, '', @UserId , @GetDate ,@UserId , @GetDate   
		FROM @InsertedPimAttributeIds IPAS 
		INNER JOIN ZnodePimAttribute ZPA ON IPAS.PimAttributeId = ZPA.PimAttributeId  
		INNER JOIN @InsertPimAtrribute IPA ON ZPA.AttributeCode= IPA.AttributeCode and IPAS.AttributeDefaultValueCode = IPA.AttributeDefaultValueCode

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
	
		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportAttributeDefaultValue @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200));

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
		@ProcedureName = 'Znode_ImportAttributeDefaultValue',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

	END CATCH;
END;