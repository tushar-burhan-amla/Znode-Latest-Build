CREATE PROCEDURE [dbo].[Znode_GetImportTemplates]
(
	@WhereClause	NVARCHAR(MAX),
	@Rows			INT	= 100,
	@PageNo			INT	= 1,
	@Order_BY		VARCHAR(1000) = '',
	@RowsCount		INT OUT
)
AS
/*
	Summary : Get Custom Import Template details and ImportHead associated to it.
		
	Unit Testing
		DECLARE @RowsCount INT;
		EXEC Znode_GetImportTemplates @WhereClause = '',@Rows = 50,@PageNo = 1,@Order_BY = NULL,@RowsCount = @RowsCount OUT;
*/
BEGIN
BEGIN TRY
	SET NOCOUNT ON;

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @SQL NVARCHAR(MAX);

	DECLARE @TBL_ImportTemplates TABLE
		(ImportHeadId INT, ImportName NVARCHAR(100), ImportTemplateId INT, TemplateName VARCHAR(300), RowId INT, CountNo INT);

	IF OBJECT_ID('Tempdb..[#ImportTemplates]') IS NOT NULL
		DROP TABLE Tempdb..[#ImportTemplates];

	CREATE TABLE #ImportTemplates
		(ImportHeadId INT, ImportName NVARCHAR(100), ImportTemplateId INT, TemplateName NVARCHAR(600));

	INSERT INTO #ImportTemplates
		(ImportHeadId,ImportName,ImportTemplateId,TemplateName)
	SELECT DISTINCT ImportHeadId, ImportName, ImportTemplateId, TemplateName
	FROM
	(
		SELECT ZIH.ImportHeadId, ZIH.NAME ImportName, ZIT.ImportTemplateId, ZIT.TemplateName
			,ROW_NUMBER() OVER (PARTITION BY ZIH.ImportHeadId ORDER BY ZIT.ImportTemplateId) As Rn
		FROM ZnodeImportHead ZIH
		INNER JOIN ZnodeImportTemplate ZIT ON ZIH.ImportHeadId = ZIT.ImportHeadId
		WHERE ZIH.ImportHeadId NOT IN (SELECT ImportHeadId FROM ZnodeImportHead WHERE Name='Promotions')
		UNION
		SELECT ZIH.ImportHeadId, ZIH.NAME ImportName, ZIT.ImportTemplateId, ZIT.TemplateName
			,ROW_NUMBER() OVER (PARTITION BY ZIH.ImportHeadId, ZIT.PromotionTypeId ORDER BY ZIT.ImportTemplateId) As Rn
		FROM ZnodeImportHead ZIH
		INNER JOIN ZnodeImportTemplate ZIT ON ZIH.ImportHeadId = ZIT.ImportHeadId
		WHERE ZIH.ImportHeadId IN (SELECT ImportHeadId FROM ZnodeImportHead WHERE Name='Promotions')
	) A
	WHERE A.Rn>1;
	
	SET @SQL = '
	;With Cte_ImportTemplates AS
	(
		SELECT ImportHeadId, ImportName, ImportTemplateId, TemplateName, 
			'+dbo.Fn_GetPagingRowId(@Order_BY,'ImportTemplateId DESC')+', Count(*) OVER() CountNo
		FROM #ImportTemplates
		WHERE 1 = 1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
	)
	SELECT ImportHeadId, ImportName, ImportTemplateId, TemplateName, RowId, CountNo
	FROM Cte_ImportTemplates
	'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows);

	INSERT INTO @TBL_ImportTemplates (ImportHeadId, ImportName, ImportTemplateId, TemplateName, RowId, CountNo)
	EXEC(@SQL);

	SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ImportTemplates ), 0);

	SELECT ImportName, ImportTemplateId, TemplateName, CASE A.Status WHEN 'Started' THEN 'true' ELSE 'false' END As Status
	FROM @TBL_ImportTemplates IT
	OUTER APPLY (SELECT TOP 1 Status FROM ZnodeImportProcessLog WHERE ImportTemplateId=IT.ImportTemplateId ORDER BY ImportProcessLogId DESC) A
	--ORDER BY ImportTemplateId DESC;

	IF OBJECT_ID('Tempdb..[#ImportTemplates]') IS NOT NULL
		DROP TABLE Tempdb..[#ImportTemplates];

	END TRY
	BEGIN CATCH
		DECLARE @Status BIT;
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetImportTemplates 
					@WhereClause = '''+ISNULL(@WhereClause,'''''')+''',
					@Rows='+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')+',
					@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',
					@Order_BY='''+ISNULL(@Order_BY,'''''')+''',
					@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''');

		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;
		
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetImportTemplates',
			@ErrorInProcedure = 'Znode_GetImportTemplates',
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH;
END;