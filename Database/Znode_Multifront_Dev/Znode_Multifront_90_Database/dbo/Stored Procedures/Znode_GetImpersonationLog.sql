CREATE PROCEDURE [dbo].[Znode_GetImpersonationLog]  
(   @WhereClause NVARCHAR(max),  
    @Rows        INT            = 100,  
    @PageNo      INT            = 1,  
    @Order_BY    VARCHAR(1000)  = '',  
    @RowsCount   INT  OUT  
   
  )    
AS   
/*  
 EXEC Znode_GetImpersonificationLog '',50,1,'',0
*/

 BEGIN  
    BEGIN TRY  
        SET NOCOUNT ON;  
        DECLARE @SQL NVARCHAR(MAX);
		 DECLARE @TBL_ImpersonificationLogList TABLE
		 (ImpersonationLogId INT,PortalId INT
		 ,StoreName NVARCHAR(200),CSRId INT,CSRName NVARCHAR(200),WebstoreuserId INT,
		 WebstoreUserName NVARCHAR(200),ActivityType VARCHAR(100),ActivityId INT,OperationType VARCHAR(20),
		 CreatedDate DATETIME,RowId INT, CountNo INT)

		  SET @SQL ='

		  ;WITH CTE_GetImpersonificationLog AS  
      (  
	  SELECT IL.ImpersonationLogId ,IL.PortalId, ZP.StoreName  , IL.CSRId,
	  ZU.FirstName + '' '' + ZU.LastName  AS CSRName,
	  IL.WebstoreuserId,ZUU.FirstName + '' '' + ZUU.LastName AS WebstoreUserName, IL.ActivityType,
	  IL.ActivityId ,IL.OperationType,IL.CreatedDate
	  FROM ZnodeImpersonationLog IL
	  INNER JOIN ZnodePortal ZP ON (ZP.PortalId = IL.PortalId)
	  INNER JOIN ZnodeUser ZU ON (ZU.UserId = IL.CSRID)
	  INNER JOIN ZnodeUser ZUU ON (ZUU.UserId = IL.WebstoreuserId)

	  )
	  ,CTE_GetImpersonificationLogList AS  
      (  
	  SELECT ImpersonationLogId ,PortalId, StoreName, CSRId,CSRName,WebstoreuserId,WebstoreUserName, ActivityType,
	  ActivityId ,OperationType,CreatedDate ,
      '+dbo.Fn_GetPagingRowId(@Order_BY,'ImpersonationLogId DESC')+',Count(*)Over() CountNo   
      FROM CTE_GetImpersonificationLog  
      WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+' 
	  )
		 SELECT ImpersonationLogId ,PortalId, StoreName, CSRId,CSRName,WebstoreuserId,WebstoreUserName,
		  ActivityType,  ActivityId ,OperationType,CreatedDate,RowId,CountNo  
      FROM CTE_GetImpersonificationLogList  
      '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows) 
		 
		
		INSERT INTO @TBL_ImpersonificationLogList (ImpersonationLogId ,PortalId,StoreName, 
		CSRId,CSRName,WebstoreuserId,WebstoreUserName, ActivityType,
		ActivityId ,OperationType,CreatedDate,RowId,CountNo)

		EXEC(@SQL)  
  
		SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_ImpersonificationLogList ),0)

		SELECT ImpersonationLogId ,PortalId, StoreName, CSRId,CSRName,WebstoreuserId,WebstoreUserName,
		  ActivityType,  ActivityId ,OperationType,CreatedDate
		  FROM @TBL_ImpersonificationLogList

	END TRY  
    BEGIN CATCH  
		DECLARE @Status BIT ;  
		SET @Status = 0;  
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetImpersonationLog @WhereClause = '+CAST(@WhereClause AS VARCHAR(MAX))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+'@Status='+CAST(@Status AS VARCHAR(10));  
                    
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
		EXEC Znode_InsertProcedureErrorLog  
		@ProcedureName = 'Znode_GetImpersonationLog',  
		@ErrorInProcedure = @Error_procedure,  
		@ErrorMessage = @ErrorMessage,  
		@ErrorLine = @ErrorLine,  
		@ErrorCall = @ErrorCall;  
	END CATCH  
 END