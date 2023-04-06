CREATE PROCEDURE [dbo].[Znode_ImportSynonyms]
(
	@TableName nvarchar(100), 
	@Status bit OUT, 
	@UserId int, 
	@ImportProcessLogId int, 
	@NewGUId nvarchar(200)
)
AS
--------------------------------------------------------------------------------------
-- Summary :  This procedure is used to import the synonyms
	
-- Unit Testing: 

--------------------------------------------------------------------------------------
BEGIN
BEGIN TRAN A;
BEGIN TRY
	DECLARE @SSQL nvarchar(max);
	DECLARE @GetDate datetime= dbo.Fn_GetDate(), @LocaleId int  ;
	SELECT @LocaleId = DBO.Fn_GetDefaultLocaleId();

	DECLARE @InsertSearchSynonyms TABLE
	( 
		RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, CatalogCode varchar(max), SynonymCode varchar(max), RenameSynonymCode varchar(max), OriginalTerm varchar(max), ReplacedBy varchar(max),IsBidirectional varchar(100), GUID nvarchar(400)		
	);
	
	SET @SSQL = 'Select RowNumber,CatalogCode,SynonymCode,RenameSynonymCode,OriginalTerm,ReplacedBy,IsBidirectional ,GUID FROM '+@TableName;
	INSERT INTO @InsertSearchSynonyms( RowNumber,CatalogCode,SynonymCode,RenameSynonymCode,OriginalTerm,ReplacedBy,IsBidirectional ,GUID)
	EXEC sys.sp_sqlexec @SSQL;

	--Getting existing catalog
	DECLARE @CatalogCode TABLE 
	( 
		CatalogCode nvarchar(100), PublishCatalogId INT
	);
	INSERT INTO @CatalogCode
	SELECT CatalogCode,PublishCatalogId
	FROM ZnodePimCatalog a
	INNER JOIN ZnodePublishCatalog b on a.PimCatalogId = b.PimCatalogId

	-- Start Functional Validation on columns
	-----------------------------------------------
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '95', 'CatalogCode', CatalogCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE not exists(select * from @CatalogCode c where ii.CatalogCode = isnull(c.CatalogCode,'')) AND ISNULL(ii.RenameSynonymCode,'') = '' 
		AND NOT EXISTS(SELECT * FROM ZnodeSearchSynonyms i WHERE i.SynonymCode = ii.SynonymCode)

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '84', 'SynonymCode', SynonymCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE isnull(ii.SynonymCode,'') = ''

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '79', 'SynonymCode', SynonymCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE (isnull(ii.SynonymCode,'') LIKE '%[^a-zA-Z0-9]%')

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '78', 'SynonymCode', SynonymCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE len(ii.SynonymCode) > 100 AND isnull(ii.SynonymCode,'') <> ''

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '96', 'RenameSynonymCode', RenameSynonymCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE exists(select * from ZnodeSearchSynonyms i where i.SynonymCode = ii.RenameSynonymCode) AND ISNULL(ii.RenameSynonymCode,'') <> ''
	AND exists(select * from ZnodeSearchSynonyms i where i.SynonymCode = ii.SynonymCode)

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '78', 'RenameSynonymCode', RenameSynonymCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE len(ltrim(rtrim(ii.RenameSynonymCode))) > 100 AND ISNULL(ii.RenameSynonymCode,'') <> ''

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '79', 'RenameSynonymCode', RenameSynonymCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE (isnull(ii.RenameSynonymCode,'') LIKE '%[^a-zA-Z0-9]%') and isnull(ii.RenameSynonymCode,'') <> ''
		
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '84', 'OriginalTerm', OriginalTerm, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE isnull(ii.OriginalTerm,'') = '' AND  NOT EXISTS(SELECT * FROM ZnodeSearchSynonyms i WHERE i.SynonymCode = ii.SynonymCode)

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '78', 'OriginalTerm - '+Case when LEN(i.Item) >100 then i.Item else '' end, OriginalTerm, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	CROSS APPLY DBO.Split(OriginalTerm,'|') i
	WHERE isnull(ii.OriginalTerm,'') <> '' AND LEN(i.Item) > 100

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '97', 'OriginalTerm', OriginalTerm, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	CROSS APPLY DBO.Split(OriginalTerm,'|') i
	WHERE isnull(ii.OriginalTerm,'') <> ''
	GROUP BY SynonymCode, OriginalTerm, RowNumber
	HAVING COUNT(i.Item) > 20

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '84', 'ReplacedBy', ReplacedBy, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE isnull(ii.ReplacedBy,'') = '' AND  NOT EXISTS(SELECT * FROM ZnodeSearchSynonyms i WHERE i.SynonymCode = ii.SynonymCode)

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '78', 'ReplacedBy - '+Case when LEN(i.Item) >100 then i.Item else '' end, ReplacedBy, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	CROSS APPLY DBO.Split(ReplacedBy,'|') i
	WHERE isnull(ii.ReplacedBy,'') <> '' AND LEN(i.Item) > 100

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '97', 'ReplacedBy', ReplacedBy, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	CROSS APPLY DBO.Split(ReplacedBy,'|') i
	WHERE isnull(ii.ReplacedBy,'') <> ''
	GROUP BY SynonymCode, ReplacedBy, RowNumber
	HAVING COUNT(i.Item) > 20

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '84', 'IsBidirectional', IsBidirectional, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE isnull(ii.IsBidirectional,'') = '' AND  NOT EXISTS(SELECT * FROM ZnodeSearchSynonyms i WHERE i.SynonymCode = ii.SynonymCode)

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '68', 'IsBidirectional', IsBidirectional, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
	WHERE ii.IsBidirectional not in ('1','0','True','False','Yes','No') AND ii.IsBidirectional <> ''

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '53', 'SynonymCode', SynonymCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
			   WHERE ii.SynonymCode IN
			   (
				   select SynonymCode from @InsertSearchSynonyms
					group by SynonymCode
					having count(1)>1
			   );

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '53', 'RenameSynonymCode', RenameSynonymCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	FROM @InsertSearchSynonyms AS ii
			   WHERE ii.RenameSynonymCode IN
			   (
				   select RenameSynonymCode from @InsertSearchSynonyms
					group by RenameSynonymCode
					having count(1)>1
			   ) AND ISNULL(ii.RenameSynonymCode,'') <> ''

	UPDATE ZIL
	SET ZIL.ColumnName =   ZIL.ColumnName + ' [ Synonym - ' + ISNULL(SynonymCode,'') + ' ] '
	FROM ZnodeImportLog ZIL 
	INNER JOIN @InsertSearchSynonyms IPA ON (ZIL.RowNumber = IPA.RowNumber)
	WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

	-- End Function Validation 	
	-----------------------------------------------
	-- Delete Invalid Data after functional validatin  
	DELETE FROM @InsertSearchSynonyms
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
	Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM @InsertSearchSynonyms
	UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount ,
	TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
	WHERE ImportProcessLogId = @ImportProcessLogId;

	--Update existing search synonym 
	UPDATE ZSS 
	SET ZSS.SynonymCode = CASE WHEN ISNULL(IPS.RenameSynonymCode,'') <> '' THEN IPS.RenameSynonymCode ELSE ZSS.SynonymCode END , ZSS.OriginalTerm = CASE WHEN ISNULL(IPS.OriginalTerm,'') = '' THEN ZSS.OriginalTerm ELSE IPS.OriginalTerm END,
	ZSS.ReplacedBy =CASE WHEN ISNULL(IPS.ReplacedBy,'') = '' THEN ZSS.ReplacedBy ELSE IPS.ReplacedBy END,
	ZSS.IsBidirectional= CASE WHEN ISNULL (IPS.IsBidirectional,'') = '' THEN ZSS.IsBidirectional ELSE CASE WHEN IPS.IsBidirectional = 'YES' THEN '1' WHEN IPS.IsBidirectional = 'NO' THEN '0' ELSE IPS.IsBidirectional END END,
	ZSS.ModifiedBy = @UserId, ZSS.ModifiedDate = @GetDate
	FROM ZnodeSearchSynonyms ZSS
	INNER JOIN @InsertSearchSynonyms IPS ON ZSS.SynonymCode = IPS.SynonymCode
		
	--- Insert data into base table ZnodeSearchSynonyms
	INSERT INTO ZnodeSearchSynonyms (PublishCatalogId,OriginalTerm,ReplacedBy,IsBidirectional,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SynonymCode)
	SELECT B.PublishCatalogId, A.OriginalTerm, A.ReplacedBy, CASE WHEN A.IsBidirectional = 'YES' THEN '1' WHEN A.IsBidirectional = 'NO' THEN '0' ELSE A.IsBidirectional END, @LocaleId,@UserId,@GetDate,@UserId,@GetDate,a.SynonymCode
	FROM @InsertSearchSynonyms A
	INNER JOIN @CatalogCode B ON A.CatalogCode = B.CatalogCode
	WHERE NOT EXISTS(SELECT * FROM ZnodeSearchSynonyms ZSS WHERE A.SynonymCode = ZSS.SynonymCode OR (A.RenameSynonymCode = ZSS.SynonymCode))
		
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
		
	DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportAttributes @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200));


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
	@ProcedureName = 'Znode_ImportSynonyms',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;

END CATCH;
END;