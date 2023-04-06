CREATE PROCEDURE [dbo].[Znode_ImportHighlight](
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
			RowId int IDENTITY(1, 1) PRIMARY KEY,
			RowNumber int, HighlightCode varchar(300), HighlightName varchar(1000),DisplayPopup varchar(300),Hyperlink varchar(1000),HighlightType varchar(300),IsActive varchar(100),DisplayOrder int,HighlightImage varchar(500),HighlightImagePath varchar(500), Description varchar(max), ShortDescription varchar(max), ImageAltTag varchar(400), GUID nvarchar(400)
		
		);
		DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId int ,PimAttributeDefaultValueId int,AttributeDefaultValueCode nvarchar(300))
		
		SET @SSQL = 'Select RowNumber,HighlightCode, HighlightName,DisplayPopup,Hyperlink,HighlightType,IsActive,DisplayOrder,HighlightImage,HighlightImagePath,Description,ShortDescription,ImageAltTag ,GUID FROM '+@TableName;
		INSERT INTO @InsertPimAtrribute( RowNumber,HighlightCode, HighlightName,DisplayPopup,Hyperlink,HighlightType,IsActive,DisplayOrder,HighlightImage,HighlightImagePath,Description,ShortDescription,ImageAltTag ,GUID)
		EXEC sys.sp_sqlexec @SSQL;

		UPDATE @InsertPimAtrribute  
		SET IsActive=CASE ISNULL(IsActive,0) WHEN 'YES' THEN 1 WHEN 'NO' THEN 0 END


		--@MessageDisplay will use to display validate message for input inventory value  
		DECLARE @HighlightAttributeDefaultCode TABLE
		( 
		   AttributeDefaultCode nvarchar(300)
		);
		INSERT INTO @HighlightAttributeDefaultCode
			   SELECT AttributeDefaultValueCode
			   FROM ZnodePimAttributeDefaultValue ZPADV
			   inner join ZnodePimAttribute ZPA on ZPADV.PimAttributeId = ZPA.PimAttributeId
			   where ZPA.AttributeCode = 'Highlights' 

		-- Start Functional Validation 
		-----------------------------------------------
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '118', 'HighlightCode', HighlightCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE ii.HighlightCode not in 
			   (
				   SELECT AttributeDefaultCode FROM @HighlightAttributeDefaultCode 
			   );
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '68', 'DisplayPopup', DisplayPopup, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertPimAtrribute AS ii  
			WHERE ii.DisplayPopup not in ('True','1','Yes','FALSE','0','No','')

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '68', 'IsActive', IsActive, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertPimAtrribute AS ii  
			WHERE ii.IsActive not in ('True','1','Yes','FALSE','0','No','')

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '103', 'HighlightType', HighlightType, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE ii.HighlightType NOT in 
			   (
				   SELECT HT.Name  FROM ZnodeHighlightType  HT
			   );

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			SELECT '115', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			FROM @InsertPimAtrribute AS ii
			WHERE (ii.DisplayOrder <> '' OR ii.DisplayOrder IS NOT NULL )AND  ii.DisplayOrder > 999

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '79', 'HighlightCode', HighlightCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE ltrim(rtrim(isnull(ii.HighlightCode,''))) like '% %' 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '53', 'HighlightCode', HighlightCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertPimAtrribute AS ii
			   WHERE ii.HighlightCode in 
			   (
				   select HighlightCode  FROM @InsertPimAtrribute  Group BY HighlightCode  having count(*) > 1 
			   );

		UPDATE ZIL
			   SET ZIL.ColumnName =   ZIL.ColumnName + ' [ Highlight - ' + ISNULL(HighlightCode,'') + ' ] '
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
		

		DECLARE @MediaId INT
		SET @MediaId = (SELECT TOP 1 MediaId from @InsertPimAtrribute IPA INNER JOIN ZnodeMedia ZM ON IPA.HighlightImage = ZM.FileName and IPA.HighlightImagePath = ZM.Path)

		if (isnull(@MediaId,0)=0)
			SET @MediaId = (SELECT max(MediaId) from @InsertPimAtrribute IPA INNER JOIN ZnodeMedia ZM ON IPA.HighlightImage = ZM.FileName)

		DECLARE @InsertedZnodeHighlight TABLE(HighlightId INT,HighlightCode VARCHAR(600)) 

			UPDATE ZH SET ZH.DisplayPopup = IPA.DisplayPopup, ZH.Hyperlink = IPA.Hyperlink, ZH.IsActive = CASE WHEN ISNULL(IPA.IsActive,'')<>''THEN IPA.IsActive ELSE ZH.IsActive END,
			       ZH.DisplayOrder = CASE WHEN ISNULL(IPA.DisplayOrder,'')<>'' THEN IPA.DisplayOrder ELSE ZH.DisplayOrder END,ZH.ModifiedBy = @UserId, ZH.ModifiedDate = @GetDate 
			FROM @InsertPimAtrribute IPA  
			INNER JOIN ZnodeHighlightType ZHT on IPA.HighlightType = ZHT.Name
			INNER JOIN ZnodeHighlight ZH ON IPA.HighlightCode = ZH.HighlightCode and ZHT.HighlightTypeId = ZH.HighlightTypeId

			

			UPDATE ZHL SET ZHL.Name = IPA.HighlightName, ZHL.Description = IPA.Description, ZHL.ShortDescription = IPA.ShortDescription, ZHL.ImageAltTag = IPA.ImageAltTag
			       , ZHL.ModifiedBy = @UserId, ZHL.ModifiedDate = @GetDate 
			FROM @InsertPimAtrribute IPA   
			INNER JOIN ZnodeHighlightType ZHT on IPA.HighlightType = ZHT.Name
			INNER JOIN ZnodeHighlight ZH ON IPA.HighlightCode = ZH.HighlightCode and ZHT.HighlightTypeId = ZH.HighlightTypeId
			INNER JOIN ZnodeHighlightLocale ZHL  ON ZH.HighlightId = ZHL.HighlightId 

			INSERT INTO ZnodeHighlight (MediaId,HighlightCode,DisplayPopup,Hyperlink,HighlightTypeId,IsActive,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			OUTPUT Inserted.HighlightId,Inserted.HighlightCode INTO @InsertedZnodeHighlight
			SELECT DISTINCT @MediaId,IPA.HighlightCode, IPA.DisplayPopup , IPA.Hyperlink, ZHT.HighlightTypeId,
				   case when IPA.IsActive in ('True','1','Yes') then 1 else 0 end, Case when Isnull(IPA.DisplayOrder,0) = 0 then  999 else IPA.DisplayOrder end  , @UserId , @GetDate ,@UserId , @GetDate 
			FROM @InsertPimAtrribute IPA   
			INNER JOIN ZnodeHighlightType ZHT on IPA.HighlightType = ZHT.Name
			WHERE NOT EXISTS(select * from ZnodeHighlight ZH where ZH.HighlightCode = IPA.HighlightCode and ZH.HighlightTypeId = ZHT.HighlightTypeId)

			INSERT INTO ZnodeHighlightLocale(LocaleId,HighlightId,ImageAltTag,Name,Description,ShortDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT DISTINCT @LocaleId, IZH.HighlightId, IPA.ImageAltTag, IPA.HighlightName, IPA.Description, IPA.ShortDescription, @UserId , @GetDate ,@UserId , @GetDate
			FROM @InsertPimAtrribute IPA 
			INNER JOIN @InsertedZnodeHighlight IZH on IPA.HighlightCode = IZH.HighlightCode 

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

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportHighlight @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@PimCatalogId='+CAST(@PimCatalogId AS VARCHAR(max));


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
		@ProcedureName = 'Znode_ImportHighlight',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
END;