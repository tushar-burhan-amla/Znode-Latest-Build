CREATE PROCEDURE [dbo].[Znode_GetCMSWidgetlist]  
(    
  @WhereClause NVARCHAR(Max)       
 ,@Rows INT = 100       
 ,@PageNo INT = 1       
 ,@Order_BY VARCHAR(1000) = ''    
 ,@RowsCount INT OUT    
)    
AS    
--EXEC Znode_GetCMSWidgetlist @WhereClause = '',@RowsCount=0
BEGIN      
BEGIN TRY     
SET NOCOUNT ON        
	DECLARE @SQL NVARCHAR(MAX)    
	DECLARE @TBL_ContentWidget TABLE (ContentWidgetId INT,ContainerKey NVARCHAR(max),Tags NVARCHAR(max),CreatedByName NVARCHAR(600),CreatedDate DATETIME,ModifiedByName NVARCHAR(600),ModifiedDate DATETIME,RowId INT,CountNo INT)  
	
	IF OBJECT_ID('TEMPDB..#User') IS NOT NULL
		DROP TABLE #User

	SELECT ZU.UserId, ZU.UserName
	INTO #User
	FROM ZnodeUser ZU
	WHERE EXISTS(SELECT * FROM ZnodeCMSContentWidget ZCW WHERE ZU.UserId = ZCW.CreatedBy OR ZU.UserId = ZCW.ModifiedBy)

	
	SET @SQL = '   
	;With Cte_ContentWidget AS   
	(  
		SELECT DISTINCT ZCW.CMSContentWidgetId, ZCW.WidgetKey as ContainerKey, ZCW.Tags ,U.UserName AS CreatedByName,ZCW.CreatedDate,U1.UserName AS ModifiedByName,ZCW.ModifiedDate
		FROM ZnodeCMSContentWidget ZCW  
		INNER JOIN #User U ON U.UserId = ZCW.CreatedBy
		INNER JOIN #User U1 ON U1.UserId = ZCW.ModifiedBy
	)  
	,Cte_ContentWidgetList AS
	(
		SELECT CMSContentWidgetId, ContainerKey, Tags,CreatedByName,CreatedDate,ModifiedByName,ModifiedDate,
		'+dbo.Fn_GetPagingRowId(@Order_BY,'ContainerKey ASC')+',Count(*)Over() CountNo 
		FROM Cte_ContentWidget    
		WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'    
	)
	SELECT CMSContentWidgetId,ContainerKey,Tags,CreatedByName,CreatedDate,ModifiedByName,ModifiedDate,RowId,CountNo    
	FROM Cte_ContentWidgetList   
	'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  

	INSERT INTO @TBL_ContentWidget (ContentWidgetId,ContainerKey,Tags,CreatedByName,CreatedDate,ModifiedByName,ModifiedDate,RowId,CountNo)  
	EXEC (@SQL)  
	SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ContentWidget ),0)  

	SELECT ContentWidgetId,ContainerKey,Tags,CreatedByName,CreatedDate,ModifiedByName,ModifiedDate
	FROM @TBL_ContentWidget  
	ORDER BY RowId

END TRY    
BEGIN CATCH    
	DECLARE @Status BIT ;    
	SET @Status = 0;    
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= 
	ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 
	'EXEC Znode_GetCMSWidgetlist @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',
	@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',
	@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                       
        
	EXEC Znode_InsertProcedureErrorLog    
	@ProcedureName = 'Znode_GetCMSWidgetlist',    
	@ErrorInProcedure = @Error_procedure,    
	@ErrorMessage = @ErrorMessage,    
	@ErrorLine = @ErrorLine,    
	@ErrorCall = @ErrorCall;                                
END CATCH; 
END;    
    
    

