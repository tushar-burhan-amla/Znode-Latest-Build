CREATE PROCEDURE [dbo].[Znode_ExportImportErrorLogByTableName]
(
	@TableName	NVARCHAR(100),
	@WhereClause NVARCHAR(MAX),
	@Rows        INT           = 100,
	@PageNo      INT           = 1,
	@Order_BY    VARCHAR(100)  = '',
	@RowsCount   INT OUT
	--@Status		 BIT = 0 OUT
)
/*
	Summary: 
		This Procedure is used to display export import error log data from respective export table based on input values.

	Unit Testing :
		EXEC [Znode_ExportImportErrorLogByTableName] @TableName='ExportProduct_443',@WhereClause ='',@Rows=50,@PageNo=1,@Order_BY='',@RowsCount=0	
*/
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @SSQLString NVARCHAR(MAX);

		DROP TABLE IF EXISTS tempdb..##TBL_ImportErrorLog;

		SET @SSQLString = '
		;With Cte_GetImportErrorLogDetails 
		 AS 
		 (
			SELECT *
			FROM '+@TableName+' WITH (NOLOCK) 						
		)
		,Cte_GetFilterImportErrorLog
		AS 
		(
		SELECT *,ROW_NUMBER() OVER (ORDER BY ImportProcessLogId DESC) As RowId,				
		Count(*)Over() CountNo 
		FROM Cte_GetImportErrorLogDetails CGPTL 
		WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'						
		)				
		SELECT *
		INTO ##TBL_ImportErrorLog
		FROM Cte_GetFilterImportErrorLog
		'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows);
	

		EXEC (@SSQLString);

		SET @RowsCount =@@ROWCOUNT;
	
		ALTER TABLE ##TBL_ImportErrorLog 
		DROP COLUMN IF EXISTS RowId;

		ALTER TABLE ##TBL_ImportErrorLog
		DROP COLUMN IF EXISTS CountNo;

		SELECT * FROM ##TBL_ImportErrorLog;
		
		DROP TABLE IF EXISTS tempdb..##TBL_ImportErrorLog;
	END TRY
              			 
	BEGIN CATCH
		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= ' EXEC Znode_ExportImportErrorLogByTableName
					@TableName = '+CAST(@TableName AS VARCHAR(100))+',
					@WhereClause = '+CAST(@WhereClause AS VARCHAR(MAX))+',
					@Rows='+CAST(@Rows AS VARCHAR(50))+',
					@PageNo='+CAST(@PageNo AS VARCHAR(50))+',
					@Order_BY='+CAST(@Order_BY AS VARCHAR(MAX))+',
					@RowsCount='+CAST(@RowsCount AS VARCHAR(50));
					
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ExportImportErrorLogByTableName',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END
