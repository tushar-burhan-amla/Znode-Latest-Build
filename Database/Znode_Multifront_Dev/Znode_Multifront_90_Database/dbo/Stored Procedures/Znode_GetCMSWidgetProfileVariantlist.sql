CREATE PROCEDURE [dbo].[Znode_GetCMSWidgetProfileVariantlist]  
(    
	@WhereClause NVARCHAR(MAX)
	,@WidgetKey NVARCHAR(MAX)
	,@Rows INT = 100       
	,@PageNo INT = 1       
	,@Order_BY VARCHAR(1000) = ''    
	,@RowsCount INT OUT    
)    
AS    
BEGIN      
BEGIN TRY     
SET NOCOUNT ON        
	DECLARE @SQL NVARCHAR(MAX)    
	CREATE TABLE #TBL_ContentWidget (CMSWidgetProfileVariantId INT, WidgetKey NVARCHAR(MAX), PortalId INT, StoreName NVARCHAR(MAX), ProfileName NVARCHAR(200),CreatedByName VARCHAR(500), CreatedDate DATETIME, ModifiedByName VARCHAR(500), ModifiedDate DATETIME, IsDefaultVarient BIT,RowId INT,CountNo INT)  
	
	SELECT CAST(UserName AS NVARCHAR(500)) AS UserName , ZU.UserId
	INTO #Users
	FROM ZnodeUser ZU WITH (NOLOCK)
	WHERE EXISTS( SELECT * FROM ZnodeCMSWidgetProfileVariant CWPV WHERE ZU.UserId = CWPV.CreatedBy )
	UNION
	SELECT UserName, ZU.UserId
	FROM ZnodeUser ZU
	WHERE EXISTS( SELECT * FROM ZnodeCMSWidgetProfileVariant CWPV WHERE ZU.UserId = CWPV.ModifiedBy )

	SELECT WPV.CMSWidgetProfileVariantId, ZCW.WidgetKey, WPV.PortalId, --ZCW.Tags,
		CASE WHEN WPV.PortalId IS NULL THEN  'Any Store' ELSE ZP.StoreName END StoreName  ,
		CASE WHEN WPV.ProfileId IS NULL THEN  'Any User Profile' ELSE ZPr.ProfileName END ProfileName, 
		U.UserName AS CreatedByName, WPV.CreatedDate, U1.UserName AS ModifiedByName, WPV.ModifiedDate,
		CAST(CASE WHEN WPV.ProfileId IS NULL AND WPV.PortalId IS NULL THEN 1 ELSE 0 END AS BIT) AS IsDefaultVarient
	INTO #Cte_ContentWidgetVarient
	FROM ZnodeCMSContentWidget ZCW  
	INNER JOIN ZnodeCMSWidgetProfileVariant WPV ON WPV.CMSContentWidgetId = ZCW.CMSContentWidgetId
	LEFT JOIN ZnodePortal ZP ON (WPV.PortalId = ZP.PortalId ) 
	LEFT JOIN ZnodeProfile ZPr ON WPV.ProfileId = ZPr.ProfileId
	INNER JOIN #Users U ON WPV.CreatedBy = U.UserId
	INNER JOIN #Users U1 ON WPV.ModifiedBy = U1.UserId
	WHERE ZCW.WidgetKey = @WidgetKey 

	SET @SQL = '   
	;With Cte_ContentWidgetVarientList AS
	(
		SELECT CMSWidgetProfileVariantId, WidgetKey, PortalId, StoreName, ProfileName,CreatedByName, CreatedDate, ModifiedByName, ModifiedDate, IsDefaultVarient,
		'+dbo.Fn_GetPagingRowId(@Order_BY,'IsDefaultVarient ASC,CMSWidgetProfileVariantId DESC')+',Count(*)Over() CountNo 
		FROM #Cte_ContentWidgetVarient    
		WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'    
	)
	INSERT INTO #TBL_ContentWidget 
	SELECT * 
	FROM Cte_ContentWidgetVarientList

	SELECT CMSWidgetProfileVariantId as WidgetProfileVariantId, WidgetKey, PortalId, StoreName, ProfileName,CreatedByName, CreatedDate, ModifiedByName, ModifiedDate, IsDefaultVarient  
	FROM #TBL_ContentWidget   
	'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows) +'
	 ORDER BY RowId'

	EXEC (@SQL)
	SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM #TBL_ContentWidget ),0)  

END TRY    
BEGIN CATCH    
	DECLARE @Status BIT ;    
	SET @Status = 0;    
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= 
	ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 
	'EXEC Znode_GetCMSWidgetProfileVariantlist @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',
	@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',
	@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                       
        
	EXEC Znode_InsertProcedureErrorLog    
	@ProcedureName = 'Znode_GetCMSWidgetProfileVariantlist',    
	@ErrorInProcedure = @Error_procedure,    
	@ErrorMessage = @ErrorMessage,    
	@ErrorLine = @ErrorLine,    
	@ErrorCall = @ErrorCall;                                
END CATCH; 
END;    