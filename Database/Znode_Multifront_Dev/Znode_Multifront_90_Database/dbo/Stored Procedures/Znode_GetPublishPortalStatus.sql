
CREATE PROCEDURE [dbo].[Znode_GetPublishPortalStatus]
(
	@WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT
)
AS 
/*
	 Summary :- This Procedure is used to get the publish status of the Portal 
	 Unit Testig 
	 EXEC  Znode_GetPublishPortalStatus '',10,1,'',0
*/
   BEGIN 
		BEGIN TRY 
		SET NOCOUNT ON 

		 DECLARE @SQL  NVARCHAR(max) 
		 DECLARE @TBL_PortalId TABLE (PortalId INT, PublishPortalLogId INT,PublishStatus VARCHAR(300),IsPortalPublished BIT,UserName NVARCHAR(512),PublishCategoryCount INT ,CreatedDate DATETIME ,ModifiedDate DATETIME ,RowId INT ,CountId INT)
	 
		 SET @SQL = '
		 ;With Cte_PortalLog AS
		 (
		 SELECT PublishPortalLogId,ZPS.DisplayName PublishStatus, IsPortalPublished, APZU.UserName ,(SELECT DISTINCT COUNT(Item) FROM dbo.split(ZPPL.PublishCategoryId,'','') SP
         ) PublishCategoryCount, ZPPL.CreatedDate,ZPPL.ModifiedDate ,ZPPL.PortalId PortalId
	     FROM ZnodePublishPortalLog  ZPPL LEFT JOIN ZnodeUser ZU ON (ZU.UserId = ZPPL.CreatedBy )
		 LEFT JOIN ZnodePublishState ZPS ON (ZPS.PublishStateId = ZPPL.PublishStateId)
	    
		 LEFT JOIN AspNetUsers APU ON (APU.Id = ZU.AspNetUserId) LEFT JOIN AspNetZnodeUser APZU ON (APZU.AspNetZnodeUserId = APU.UserName))
	 
	     ,Cte_PublishStatus 
		 AS (SELECT PortalId, PublishPortalLogId, PublishStatus, IsPortalPublished ,UserName , PublishCategoryCount, CreatedDate,ModifiedDate ,
		 '+[dbo].[Fn_GetPagingRowId](@Order_BY,'PublishPortalLogId DESC')+' , Count(*)Over() CountId FROM Cte_PortalLog
         WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' )
	 
		 SELECT PortalId, PublishPortalLogId,PublishStatus, IsPortalPublished,UserName,PublishCategoryCount,CreatedDate,ModifiedDate,RowId,CountId 
		 FROM Cte_PublishStatus 
		 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)+' '
	
	
		 INSERT INTO @TBL_PortalId
		 EXEC (@SQL)

		 SELECT  PortalId, PublishPortalLogId,PublishStatus, IsPortalPublished,UserName,PublishCategoryCount,CreatedDate,ModifiedDate
		 FROM @TBL_PortalId

		 SET @RowsCount = ISNULL((SELECT TOP 1 COUNTID FROM @TBL_PortalId),0)
	 
		 END TRY 
		 BEGIN CATCH 
			DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishPortalStatus @WhereClause = '+@WhereClause+',@Rows='+CAST(@Rows AS
 VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			EXEC Znode_InsertProcedureErrorLog
					@ProcedureName = 'Znode_GetPublishPortalStatus',
					@ErrorInProcedure = @Error_procedure,
					@ErrorMessage = @ErrorMessage,
					@ErrorLine = @ErrorLine,
					@ErrorCall = @ErrorCall;
		 END CATCH 
   END