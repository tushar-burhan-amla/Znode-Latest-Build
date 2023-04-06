CREATE PROCEDURE [dbo].[Znode_DeleteExportLogs]
(
	@DurationInDays			INT=0,
	@ExportProcessLogIds	NVARCHAR(MAX)=0,
	@Status					INT OUT
)
/*
	Summary: 
		This procedure is used to remove export logs/files and drop respective export tables.

	Unit Testing:
		EXEC dbo.Znode_DeleteExportLogs @DurationInDays=30, @ExportProcessLogIds=0, @Status=0
		EXEC dbo.Znode_DeleteExportLogs @DurationInDays=0, @ExportProcessLogIds='1,2,3,4', @Status=0
		EXEC dbo.Znode_DeleteExportLogs @DurationInDays=0, @ExportProcessLogIds='58', @Status=0
*/
AS
BEGIN
	BEGIN TRAN DeleteExportLogs;
	BEGIN TRY
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

		SET NOCOUNT ON;

		IF OBJECT_ID('tempdb..#TableName') IS NOT NULL
			DROP TABLE #TableName;

		IF OBJECT_ID('tempdb..#ExportProcessLog') IS NOT NULL
			DROP TABLE #ExportProcessLog;

		SELECT Item As ExportProcessLogId
		INTO #ExportProcessLog
		FROM dbo.Split(@ExportProcessLogIds,',');
		
		SELECT TableName
		INTO #TableName
		FROM ZnodeExportProcessLog
		WHERE ((@DurationInDays<>0 AND CONVERT(DATE,CreatedDate,121) < DATEADD(DAY,-@DurationInDays,CONVERT(DATE,@GetDate,121)))
			OR ExportProcessLogId IN (SELECT ExportProcessLogId FROM #ExportProcessLog))
			AND [Status] <> 'In Progress';

		--DECLARE @DropTable NVARCHAR(MAX) = (SELECT + STRING_AGG('DROP TABLE IF EXISTS '+TableName ,'; ') FROM #TableName);

		DECLARE @DropTable NVARCHAR(MAX);

		SELECT @DropTable = COALESCE(@DropTable + ' DROP TABLE IF EXISTS ', '') + TableName +';'
		FROM #TableName WHERE EXISTS (SELECT * FROM #TableName);

		IF LEN(@DropTable)>0
		BEGIN
			SET @DropTable = 'DROP TABLE IF EXISTS ' +  @DropTable;
			--,RIGHT(@DropTable,LEN(@DropTable)-2)

			--PRINT @DropTable
			EXEC (@DropTable);
		END

		DELETE
		FROM ZnodeExportProcessLog
		WHERE TableName IN (SELECT [TableName] FROM #TableName);

		SELECT TableName As [FileName]
		FROM #TableName;

		DROP TABLE #TableName;
		DROP TABLE #ExportProcessLog;

		SET @Status = 1;
	COMMIT TRAN DeleteExportLogs;
	END TRY
	BEGIN CATCH
		DECLARE @Error_Procedure VARCHAR(1000) = ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteExportLogs 
					@DurationInDays='+CAST(@DurationInDays AS VARCHAR(50))+',
					@ExportProcessLogIds='''+CAST(@ExportProcessLogIds AS NVARCHAR(MAX))+''',
					@Status='+CAST(@Status AS VARCHAR(50));
         
             SET @Status = 0;

             ROLLBACK TRAN DeleteExportLogs;

             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteExportLogs',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
		SELECT ERROR_MESSAGE()
	END CATCH
END