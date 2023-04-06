

CREATE PROCEDURE [dbo].[Znode_GetProcedureErrorLog]
(   @WhereClause  NVARCHAR(MAX) = '',
	@Rows         INT           = 100,
	@PageNo       INT           = 1,
	@Order_BY     VARCHAR(100)  = '',
	@LogMessageId VARCHAR(100)  = '',										 
	@RowsCount    INT OUT)
AS 
/*
Summary: This procedure is used to fetch procedure error log details

EXEC Znode_GetProcedureErrorLog @WhereClause = 'LogMessageId = 74',@RowsCount = 0 , @LogMessageId =75
EXEC Znode_GetProcedureErrorLog @WhereClause = 'LogMessageId = 74',@RowsCount = 0
EXEC Znode_GetProcedureErrorLog @WhereClause = '',@RowsCount = 0 , @LogMessageId =75
EXEC Znode_GetProcedureErrorLog @WhereClause = '',@RowsCount = 0


*/


BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @LogId INT
			 DECLARE @TBL_ProcedureErrorLog TABLE (LogMessageId VARCHAR(100),LogMessage NVARCHAR(MAX),Component VARCHAR(1000),StackTraceMessage NVARCHAR(MAX),CreatedDate DATETIME,TraceLevel NVARCHAR(MAX),RowId INT,CountNo INT)


			 -- IF @LogMessageId is passed as parameter then only filter or else fetch all records
			 IF @LogMessageId <> 0 OR @LogMessageId <> '' 
			 BEGIN

			 --SET @WhereClause = ' LogMessageId = '+CAST(@LogMessageId AS nvarchar(100))
			 SET @WhereClause = CASE WHEN @WhereClause = '' THEN ' LogMessageId = '+@LogMessageId  ELSE @WhereClause + ' AND LogMessageId = '+@LogMessageId END
			 END
			 	 
			 SET @SQL = '


			 ;with Cte_GetProcedureErrorDetails AS 
				(

					SELECT PEL.ProcedureErrorLogId as LogMessageId,PEL.ErrorMessage as LogMessage,PEL.ProcedureName as Component,PEL.CreatedDate as CreatedDate,
					''Exception in  '' + ProcedureName + ''  ON  '' +ErrorCall + ''  IN  '' +ErrorInProcedure + ''  AT  '' +ErrorLine as StackTraceMessage,''DBError'' as TraceLevel
					FROM ZnodeProceduresErrorLog PEL
				  
				) 
			, Cte_GetProcedureError AS 
				(
					SELECT  LogMessageId,LogMessage,Component,CreatedDate,StackTraceMessage,TraceLevel ,'+dbo.Fn_GetPagingRowId(@Order_BY,'LogMessageId DESC')+',Count(*)Over() CountNo
					FROM Cte_GetProcedureErrorDetails
					WHERE 1=1 
					'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				)

				SELECT DISTINCT LogMessageId,LogMessage,Component,CreatedDate,StackTraceMessage,TraceLevel,RowId,CountNo
				FROM Cte_GetProcedureError
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
				print @sql

				INSERT INTO @TBL_ProcedureErrorLog(LogMessageId,LogMessage,Component,CreatedDate,StackTraceMessage,TraceLevel,RowId,CountNo)
				EXEC(@SQL)

				SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ProcedureErrorLog), 0);

				SELECT LogMessageId,LogMessage,Component,CreatedDate,StackTraceMessage,TraceLevel 
				FROM @TBL_ProcedureErrorLog
				

		 END TRY
         BEGIN CATCH
		 SELECT ERROR_MESSAGE();
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProcedureErrorLog @WhereClause = '''+ISNULL(CAST(@WhereClause AS VARCHAR(max)),'''''')+''',@Rows='+ISNULL(CAST(@Rows AS VARCHAR(50)),'''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',
			 @Order_BY='''+ISNULL(@Order_BY,'''''')+''',@LogMessageId = '''+ISNULL(CAST(@LogMessageId AS VARCHAR(50)),'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',@Status='+ISNULL(CAST(@Status AS VARCHAR(10)),'''');
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetProcedureErrorLog',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
END;