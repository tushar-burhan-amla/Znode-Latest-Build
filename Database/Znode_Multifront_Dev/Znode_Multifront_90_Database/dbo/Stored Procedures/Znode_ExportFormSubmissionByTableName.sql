CREATE PROCEDURE [dbo].[Znode_ExportFormSubmissionByTableName]
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
		This Procedure is used to display exported FormSubmissions data from respective export table based on input values.

	Unit Testing :
		EXEC [Znode_ExportFormSubmissionByTableName] @TableName='ExportFormSubmissions_20220802_15_36_34',@WhereClause ='',@Rows=50,@PageNo=1,@Order_BY='',@RowsCount=0
		EXEC [Znode_ExportFormSubmissionByTableName] @TableName='ExportFormSubmissions_20220802_15_36_34',@WhereClause ='',@Rows=50,@PageNo=2,@Order_BY='',@RowsCount=0
*/
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @SSQLString NVARCHAR(MAX);

		IF OBJECT_ID('tempdb..[##TBL_FormBuilder]') IS NOT NULL
			DROP TABLE tempdb..[##TBL_FormBuilder];

		SET @SSQLString = '
		;With Cte_GetFormBuilderDetails 
		 AS (
			SELECT *
			FROM '+@TableName+' WITH (NOLOCK) 						
			)
			,Cte_GetFilterFormBuilder
			AS (
			SELECT *,ROW_NUMBER() OVER (ORDER BY FormBuilderSubmitId DESC) As RowId,				
			Count(*)Over() CountNo 
			FROM Cte_GetFormBuilderDetails CGPTL 
			WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'						
				)				
			SELECT *
			INTO ##TBL_FormBuilder
			FROM Cte_GetFilterFormBuilder
			'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows);
			
		--PRINT (@SSQLString)

		EXEC (@SSQLString);

		SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM ##TBL_FormBuilder ),0);
			
		ALTER TABLE ##TBL_FormBuilder 
		DROP COLUMN IF EXISTS RowId;

		ALTER TABLE ##TBL_FormBuilder
		DROP COLUMN IF EXISTS CountNo;

		SELECT * FROM ##TBL_FormBuilder;
		
		IF OBJECT_ID('tempdb..[##TBL_FormBuilder]') IS NOT NULL
			DROP TABLE tempdb..[##TBL_FormBuilder];
	END TRY
              			 
	BEGIN CATCH
		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= ' EXEC Znode_ExportFormSubmissionByTableName 
					@TableName = '+CAST(@TableName AS VARCHAR(100))+',
					@WhereClause = '+CAST(@WhereClause AS VARCHAR(MAX))+',
					@Rows='+CAST(@Rows AS VARCHAR(50))+',
					@PageNo='+CAST(@PageNo AS VARCHAR(50))+',
					@Order_BY='+CAST(@Order_BY AS VARCHAR(MAX))+',
					@RowsCount='+CAST(@RowsCount AS VARCHAR(50));
					
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ExportFormSubmissionByTableName',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END