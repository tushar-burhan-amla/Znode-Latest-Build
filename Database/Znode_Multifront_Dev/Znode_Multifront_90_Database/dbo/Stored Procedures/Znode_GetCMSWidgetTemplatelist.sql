CREATE PROCEDURE [dbo].[Znode_GetCMSWidgetTemplatelist]  
(    
	@WhereClause NVARCHAR(Max)       
	,@Rows INT = 100       
	,@PageNo INT = 1       
	,@Order_BY VARCHAR(1000) = ''    
	,@RowsCount INT OUT    
)    
AS    
--EXEC [Znode_GetCMSWidgetTemplatelist] @WhereClause = '',@RowsCount=0
BEGIN      
BEGIN TRY     
SET NOCOUNT ON  
	DECLARE @SQL NVARCHAR(MAX)  

	DECLARE @TBL_CMSWidgetTemplateList TABLE ([CMSWidgetTemplateId] INT, [Code] VARCHAR(200), [Name] VARCHAR(200),[FileName] NVARCHAR(2000),[MediaId] INT,[CreatedByName] NVARCHAR(600),[CreatedDate] DATETIME,[ModifiedByName] NVARCHAR(600),[ModifiedDate] DATETIME,[MediaPath] VARCHAR(300),RowId INT,CountNo INT) 

	IF OBJECT_ID('TEMPDB..#User') IS NOT NULL
		DROP TABLE #User

	IF OBJECT_ID('TEMPDB..#Media') IS NOT NULL
		DROP TABLE #Media

	SELECT ZU.UserId, ZU.[UserName]
	INTO #User
	FROM ZnodeUser ZU
	WHERE EXISTS(SELECT * FROM ZnodeCMSWidgetTemplate ZCW WHERE ZU.UserId = ZCW.CreatedBy OR ZU.UserId = ZCW.ModifiedBy)

	SELECT ZM.MediaId, ZM.Path as [MediaPath]
	INTO #Media
	FROM [ZnodeMedia] ZM
	WHERE EXISTS(SELECT * FROM [ZnodeCMSWidgetTemplate] CWT WHERE CWT.MediaId = ZM.MediaId) 

	SELECT 
		CWT.[CMSWidgetTemplateId] AS [CMSWidgetTemplateId], 
		CWT.[Code] AS [Code], 
		CWT.[Name] AS [Name], 
		CWT.[FileName] AS [FileName], 
		CWT.[MediaId] AS [MediaId], 
		U.[UserName] AS [CreatedByName], 
		CWT.[CreatedDate] AS [CreatedDate], 
		U1.[UserName] AS [ModifiedByName],  
		CWT.[ModifiedDate] AS [ModifiedDate],
		M.[MediaPath]
	INTO #CMSWidgetTemplate
	FROM [dbo].[ZnodeCMSWidgetTemplate] AS CWT
	INNER JOIN #User U ON U.UserId = CWT.CreatedBy
	INNER JOIN #User U1 ON U1.UserId = CWT.ModifiedBy
	LEFT JOIN #Media as M on CWT.MediaId = M.MediaId

	SET @SQL = '   
	;With Cte_CMSWidgetTemplate AS   
	(  
		SELECT DISTINCT [CMSWidgetTemplateId], [Code], [Name],[FileName],[MediaId],[CreatedByName],[CreatedDate],[ModifiedByName],[ModifiedDate],[MediaPath],
		'+dbo.Fn_GetPagingRowId(@Order_BY,' CMSWidgetTemplateId DESC')+',Count(*)Over() CountNo 
		FROM #CMSWidgetTemplate ZCW  
		WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
	)  
	SELECT [CMSWidgetTemplateId], [Code], [Name],[FileName],[MediaId],[CreatedByName],[CreatedDate],[ModifiedByName],[ModifiedDate],[MediaPath],RowId,CountNo    
	FROM Cte_CMSWidgetTemplate   
	'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  

	INSERT INTO @TBL_CMSWidgetTemplateList ([CMSWidgetTemplateId], [Code], [Name],[FileName],[MediaId],[CreatedByName],[CreatedDate],[ModifiedByName],[ModifiedDate],[MediaPath],RowId,CountNo)  
	EXEC (@SQL)  
	SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_CMSWidgetTemplateList ),0)  

	SELECT [CMSWidgetTemplateId], [Code], [Name],[FileName],[MediaId],[CreatedByName],[CreatedDate],[ModifiedByName],[ModifiedDate],[MediaPath]
	FROM @TBL_CMSWidgetTemplateList
	ORDER BY RowId

END TRY    
BEGIN CATCH    
	DECLARE @Status BIT ;    
	SET @Status = 0;    
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= 
	ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 
	'EXEC Znode_GetCMSWidgetTemplatelist @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',
	@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',
	@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                       
        
	EXEC Znode_InsertProcedureErrorLog    
	@ProcedureName = 'Znode_GetCMSWidgetTemplatelist',    
	@ErrorInProcedure = @Error_procedure,    
	@ErrorMessage = @ErrorMessage,    
	@ErrorLine = @ErrorLine,    
	@ErrorCall = @ErrorCall;                                
END CATCH; 
END;    