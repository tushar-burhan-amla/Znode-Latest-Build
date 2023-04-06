CREATE PROCEDURE [dbo].[Znode_GetExportLogs]
( 
	@WhereClause NVARCHAR(MAX),
	@Rows        INT           = 100,
	@PageNo      INT           = 1,
	@Order_BY    VARCHAR(1000)  = '',
	@RowsCount   INT OUT
)
AS
/*
	Summary : Get Export Log details and errorlog associated to it
	Unit Testing 
	begin tran
		DECLARE @RowsCount INT;
		EXEC Znode_GetExportLogs @WhereClause = '',@Rows = 1000,@PageNo = 1,@Order_BY = NULL,@RowsCount = @RowsCount OUT;
		EXEC Znode_GetExportLogs @WhereClause = '',@Rows = 1000,@PageNo = 1,@Order_BY = NULL,@RowsCount = null;
	rollback tran
*/
BEGIN
	BEGIN TRY
    SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
		DECLARE @SQL NVARCHAR(MAX);
		DECLARE @TBL_ErrorLog TABLE (ExportProcessLogId INT, ExportType NVARCHAR(100), Status VARCHAR(50) , ProcessStartedDate DATETIME, ProcessCompletedDate DATETIME, RowId INT, CountNo INT, TableName NVARCHAR(100), FileType nvarchar(100))

		IF OBJECT_ID('Tempdb..[#ErrorLog]') IS NOT NULL
		DROP TABLE Tempdb..[#ErrorLog]

		CREATE TABLE #ErrorLog(ExportProcessLogId INT, ExportType NVARCHAR(100), Status NVARCHAR(100), ProcessStartedDate DATETIME, ProcessCompletedDate DATETIME, CreatedBy INT, TableName NVARCHAR(100), FileType nvarchar(100))

		INSERT INTO #ErrorLog(ExportProcessLogId,ExportType,Status,ProcessStartedDate, ProcessCompletedDate, TableName, FileType)				
		SELECT ExportProcessLogId,ExportType,Status,ProcessStartedDate, ProcessCompletedDate, TableName, FileType
		FROM ZnodeExportprocessLog

		SET @SQL = ';With Cte_ErrorlogFilter AS
		(

		SELECT ExportProcessLogId,ExportType,Status,ProcessStartedDate, ProcessCompletedDate, TableName, FileType
				,'+dbo.Fn_GetPagingRowId(@Order_BY,'ExportProcessLogId DESC')+', Count(*)Over() CountNo
		FROM #ErrorLog
		WHERE 1 = 1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
		) 
		SELECT ExportProcessLogId,ExportType,Status,ProcessStartedDate, ProcessCompletedDate,RowId,CountNo, TableName, FileType
		FROM Cte_ErrorlogFilter 
		'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
	       
		INSERT INTO @TBL_ErrorLog (ExportProcessLogId, ExportType,[Status],ProcessStartedDate, ProcessCompletedDate,RowId,CountNo,TableName,FileType)
		EXEC(@SQl)
		
		SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ErrorLog ), 0);

		SELECT ExportProcessLogId, ExportType, [Status], ProcessStartedDate, ProcessCompletedDate, TableName, FileType
		FROM @TBL_ErrorLog

		IF OBJECT_ID('Tempdb..[#ErrorLog]') IS NOT NULL
			DROP TABLE Tempdb..[#ErrorLog]
    END TRY
    BEGIN CATCH
        DECLARE @Status BIT;
		SET @Status = 0;
		
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetExportLogs 
					@WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS	VARCHAR(50)),'''''')+',
					@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',
					@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		
        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetExportLogs',
			@ErrorInProcedure = 'Znode_GetExportLogs',
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;                   
    END CATCH;
END;