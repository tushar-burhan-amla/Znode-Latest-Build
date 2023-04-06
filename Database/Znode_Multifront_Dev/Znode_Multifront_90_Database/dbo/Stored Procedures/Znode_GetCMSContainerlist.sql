CREATE PROCEDURE [dbo].[Znode_GetCMSContainerlist]   
(    
  @WhereClause NVARCHAR(Max)       
 ,@Rows INT = 100       
 ,@PageNo INT = 1       
 ,@Order_BY VARCHAR(1000) = ''    
 ,@RowsCount INT OUT    
)    
AS    
--EXEC Znode_GetCMSContainerlist @WhereClause = '',@RowsCount=0
BEGIN      
BEGIN TRY     
SET NOCOUNT ON        
	DECLARE @SQL NVARCHAR(MAX)    
	DECLARE @TBL_ContentContainer TABLE (ContentContainerId INT,ContainerKey NVARCHAR(max),Name nvarchar(max),Tags NVARCHAR(max),PublishStatus VARCHAR(32),CreatedByName NVARCHAR(600),CreatedDate DATETIME,ModifiedByName NVARCHAR(600),ModifiedDate DATETIME,RowId INT,CountNo INT)  
	
	IF OBJECT_ID('TEMPDB..#User') IS NOT NULL
		DROP TABLE #User

	SELECT ZU.UserId, ZU.UserName
	INTO #User
	FROM ZnodeUser ZU
	WHERE EXISTS(SELECT * FROM ZnodeCMSContentContainer ZCW WHERE ZU.UserId = ZCW.CreatedBy OR ZU.UserId = ZCW.ModifiedBy)

	SET @SQL = '   
	;With Cte_ContentContainer AS   
	(  
		SELECT DISTINCT ZCW.CMSContentContainerId,ZCW.Name, ZCW.ContainerKey as ContainerKey, ZCW.Tags, ZPS.DisplayName As PublishStatus ,U.UserName AS CreatedByName,ZCW.CreatedDate,U1.UserName AS ModifiedByName,ZCW.ModifiedDate
		FROM ZnodeCMSContentContainer ZCW
		LEFT JOIN ZnodePublishState ZPS ON ZCW.PublishStateId=ZPS.PublishStateId
		LEFT JOIN #User U ON U.UserId = ZCW.CreatedBy
		LEFT JOIN #User U1 ON U1.UserId = ZCW.ModifiedBy
	)  
	,Cte_ContentContainerList AS
	(
		SELECT CMSContentContainerId, ContainerKey,Name, Tags,PublishStatus,CreatedByName,CreatedDate,ModifiedByName,ModifiedDate,
		'+dbo.Fn_GetPagingRowId(@Order_BY,'ContainerKey ASC')+',Count(*)Over() CountNo 
		FROM Cte_ContentContainer    
		WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'    
	)
	SELECT CMSContentContainerId,ContainerKey,Name,Tags,PublishStatus,CreatedByName,CreatedDate,ModifiedByName,ModifiedDate,RowId,CountNo    
	FROM Cte_ContentContainerList   
	'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
	  
	INSERT INTO @TBL_ContentContainer (ContentContainerId,ContainerKey,Name,Tags,PublishStatus,CreatedByName,CreatedDate,ModifiedByName,ModifiedDate,RowId,CountNo)  
	EXEC (@SQL)  
	SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ContentContainer ),0)  

	SELECT ContentContainerId,ContainerKey,Name,Tags,PublishStatus,CreatedByName,CreatedDate,ModifiedByName,ModifiedDate
	FROM @TBL_ContentContainer  
	ORDER BY RowId

END TRY    
BEGIN CATCH    
	DECLARE @Status BIT ;    
	SET @Status = 0;    
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= 
	ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 
	'EXEC Znode_GetCMSContainerlist @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',
	@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',
	@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                       
        
	EXEC Znode_InsertProcedureErrorLog    
	@ProcedureName = 'Znode_GetCMSContainerlist',    
	@ErrorInProcedure = @Error_procedure,    
	@ErrorMessage = @ErrorMessage,    
	@ErrorLine = @ErrorLine,    
	@ErrorCall = @ErrorCall;                                
END CATCH; 
END;