CREATE PROCEDURE [dbo].[Znode_ImportZipCode](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200), @CountryCode nvarchar(100))
AS 
	
/*
	----Summary:  Import Zip Code data List 
    */

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		DECLARE @sSql nvarchar(max);

		DECLARE @ZipCode TABLE
		( 
		   ZIPCode nvarchar(300)
		);
		INSERT INTO @ZipCode(ZIPCode)
			   SELECT ZIP
			   FROM ZnodeCity 


		DECLARE @Tlb_ZnodeCity TABLE
		( 
			[RowNumber] [int] NOT NULL, [CityName] [nvarchar](255) NULL, [CityType] [nvarchar](50) NULL, [ZIP] [nvarchar](50) NULL, [ZIPType] [nvarchar](50) NULL, [CountyCode] [varchar](255) NULL,  [StateCode] [nvarchar](255) NULL, [Latitude] [decimal](9, 6) NULL, [Longitude] [decimal](9, 6) NULL, [CountyFIPS] [varchar](50) NULL, [StateFIPS] [varchar](50) NULL, [MSACode] [varchar](50) NULL, [TimeZone] [varchar](50) NULL, [UTC] [decimal](3, 1) NULL, [DST] [char](1) NULL PRIMARY KEY([RowNumber])
		);
		SET @SSQL = 'Select CityName,CityType,ZIP,ZIPType,CountyCode,StateCode,Latitude,Longitude,CountyFIPS,StateFIPS,MSACode,TimeZone,UTC ,RowNumber FROM '+@TableName;
		INSERT INTO @Tlb_ZnodeCity( CityName, CityType, ZIP, ZIPType, CountyCode, StateCode, Latitude, Longitude, CountyFIPS, StateFIPS, MSACode, TimeZone, UTC, RowNumber )
		EXEC sys.sp_sqlexec @SSQL;
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '112', 'StateCode', StateCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @Tlb_ZnodeCity
			   WHERE StateCode NOT IN
			   (
				   SELECT StateCode
				   FROM ZnodeState
				   WHERE CountryCode = @CountryCode
			   );

	   INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '53', 'ZIPCode', ZIP, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			  FROM @Tlb_ZnodeCity AS ii
			   WHERE ii.ZIP IN 
			   (
				   SELECT ZIP  FROM @Tlb_ZnodeCity  Group BY ZIP  HAVING COUNT(*) > 1 
			   );

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '10', 'ZIPCode', ZIP, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			  FROM @Tlb_ZnodeCity AS ii
			   WHERE ii.ZIP in 
			   (
				   SELECT ZIPCode FROM @ZipCode  where ZIPCode IS NOT NULL 
			   );

		UPDATE ZIL
			   SET ZIL.ColumnName =   ZIL.ColumnName + ' [ ZIPCode - ' + ISNULL(ZIP,'') + ' ] '
			   FROM ZnodeImportLog ZIL 
			   INNER JOIN @Tlb_ZnodeCity IPA ON (ZIL.RowNumber = IPA.RowNumber)
			   WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

		DELETE FROM @Tlb_ZnodeCity
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber IS NOT NULL 
		);

		-- Update Record count in log 
        DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM @Tlb_ZnodeCity
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount ,
		TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
		WHERE ImportProcessLogId = @ImportProcessLogId;


		INSERT INTO ZnodeCity( CityName, CityType, ZIP, ZIPType, CountyCode, CountryCode, StateCode, Latitude, Longitude, CountyFIPS, StateFIPS, MSACode, TimeZone, UTC, DST, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
			   SELECT CityName, CityType, ZIP, ZIPType, [CountyCode], @CountryCode, StateCode, Latitude, Longitude, CountyFIPS, StateFIPS, MSACode, TimeZone, UTC, DST, 2, @GetDate, 2, @GetDate
			   FROM @Tlb_ZnodeCity AS tzc
			   WHERE NOT EXISTS
			   (
				   SELECT TOP 1 1
				   FROM ZnodeCity
				   WHERE  CountryCode = @CountryCode AND 
						  StateCode = tzc.StateCode AND 
						  CityName = TZC.CityName AND 
						  ZIP = TZC.ZIP
			   ) --AND 
					 --tzc.RowNumber =
			   --(
				  -- SELECT MAX(ii1.RowNumber)
				  -- FROM @Tlb_ZnodeCity AS ii1
				  -- WHERE ii1.StateCode = tzc.StateCode
			   --);
		UPDATE ZC
		  SET ZC.CityType = TZC.CityType, ZC.CountyCode = TZC.CountyCode, ZC.Latitude = TZC.Latitude, ZC.Longitude = TZC.Longitude, ZC.CountyFIPS = TZC.CountyFIPS, ZC.StateFIPS = TZC.StateFIPS, ZC.MSACode = TZC.MSACode, ZC.TimeZone = TZC.TimeZone, ZC.UTC = TZC.UTC, ZC.DST = TZC.DST, ZC.ModifiedBy = @UserId, ZC.ModifiedDate = @GetDate
		FROM ZnodeCity ZC
			 INNER JOIN
			 @Tlb_ZnodeCity TZC
			 ON ZC.CountryCode = @CountryCode AND 
				ZC.StateCode = TZC.StateCode AND 
				ZC.CityName = TZC.CityName AND 
				ZC.ZIP = TZC.ZIP;
		SET @Status = 1;
		
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
		COMMIT TRAN A;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN A;

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportZipCode @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@CountryCode='+CAST(@CountryCode AS VARCHAR(max));

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
		@ProcedureName = 'Znode_ImportZipCode',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
		
	END CATCH;
END;

