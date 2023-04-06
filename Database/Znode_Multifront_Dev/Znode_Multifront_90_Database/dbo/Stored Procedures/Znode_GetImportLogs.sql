CREATE PROCEDURE dbo.Znode_GetImportLogs
(
	@ImportProcessLogId INT
)
AS
/*
	Summary : Get import log details.
	Unit Testing  
	BEGIN TRAN
		EXEC Znode_GetImportLogs @ImportProcessLogId=1;
	ROLLBACK TRAN
*/
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		SELECT DISTINCT ImportProcessLogId,ImportTemplateId,Status,ProcessStartedDate,ProcessCompletedDate,CreatedDate,
			CreatedBy,ModifiedBy,ModifiedDate,ERPTaskSchedulerId,SuccessRecordCount,FailedRecordcount,
			TotalProcessedRecords,'' AS TemplateName,ImportName
		FROM
		(
			SELECT ZIP.ImportProcessLogId AS ImportProcessLogId,ZIP.ImportTemplateId AS ImportTemplateId,
				ZIP.[Status] AS [Status],ZIP.ProcessStartedDate AS ProcessStartedDate,
				ZIP.ProcessCompletedDate AS ProcessCompletedDate,ZIP.CreatedBy AS CreatedBy,
				ZIP.CreatedDate AS CreatedDate, ZIP.ModifiedBy AS ModifiedBy,ZIP.ModifiedDate AS ModifiedDate,
				ZIP.ERPTaskSchedulerId AS ERPTaskSchedulerId,ISNUll(ZIP.SuccessRecordCount,'0') AS SuccessRecordCount,
				ISNUll(ZIP.FailedRecordcount,'0') AS FailedRecordcount,ISNUll(ZIP.TotalProcessedRecords,'0') AS	TotalProcessedRecords,
				'' AS TemplateName,'' AS 'ImportName'
			FROM dbo.ZnodeImportProcessLog AS ZIP WITH(NOLOCK)
			--INNER JOIN ZnodeImportTemplate AS ZIT ON ZIP.ImportTemplateId = ZIT.ImportTemplateId
			--INNER JOIN ZnodeImportLog AS ZIL ON ZIP.ImportProcessLogId = ZIL.ImportProcessLogId
			WHERE ZIP.ImportProcessLogId=@ImportProcessLogId
		) logs
	END TRY
	BEGIN CATCH
		DECLARE @Status BIT;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetImportLogs @ImportProcessLogId = '+CAST(@ImportProcessLogId AS VARCHAR(50));
             
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetImportLogs',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;                              
	END CATCH;
END;