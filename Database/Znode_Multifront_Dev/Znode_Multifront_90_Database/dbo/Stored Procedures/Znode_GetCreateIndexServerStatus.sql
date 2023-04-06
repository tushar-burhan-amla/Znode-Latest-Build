CREATE PROCEDURE [dbo].[Znode_GetCreateIndexServerStatus]
(   @WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT)
AS 
/*
     Summary :- This procedure is used to get the publish products details 
     Unit Testing 
     EXEC Znode_GetCreateIndexServerStatus '',@RowsCount=0
	*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_GetCreateIndexServerStatus TABLE (CatalogIndexId INT,SearchIndexMonitorId INT,SourceId	INT,SourceType NVARCHAR(500),SourceTransactionType NVARCHAR(500),
             AffectedType NVARCHAR(500),ServerName NVARCHAR(200),Status NVARCHAR(200),UserName NVARCHAR(512),CreatedBy INT,CreatedDate DATETIME,ModifiedBy INT,ModifiedDate DATETIME
             ,RowId INT,CountNo INT);

             SET @SQL = '
				;With Cte_GetCreateIndexServerStatusDetails AS 
				(
				SELECT zsim.CatalogIndexId,zsim.SearchIndexMonitorId, zsim.SourceId, zsim.SourceType, zsim.SourceTransactionType, zsim.AffectedType, zsiss.ServerName
					, zsiss.Status,zsiss.CreatedBy,zsiss.CreatedDate,zsiss.ModifiedBy,zsiss.ModifiedDate, IsNull(VIUSD.UserName,''Scheduled Task'') as UserName 
				FROM ZnodeSearchIndexMonitor zsim 
				LEFT OUTER JOIN ZnodeSearchIndexServerStatus zsiss ON zsim.SearchIndexMonitorId=zsiss.SearchIndexMonitorId
				LEFT JOIN View_GetUserDetails VIUSD ON (VIUSD.UserId = ZSIM.CreatedBy) 
				)
				, Cte_GetCreateIndexServerStatusDetailsData AS
				(
		
				SELECT *,'+dbo.Fn_GetPagingRowId(@Order_BY,'SearchIndexMonitorId ')+',Count(*)Over() CountNo
				FROM Cte_GetCreateIndexServerStatusDetails CTPC 
				WHERE 1=1 
						  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				)

				SELECT CatalogIndexId,SearchIndexMonitorId,SourceId,SourceType,SourceTransactionType,AffectedType,ServerName,Status,UserName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RowId,CountNo
				FROM Cte_GetCreateIndexServerStatusDetailsData 
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
            
             INSERT INTO @TBL_GetCreateIndexServerStatus
             EXEC (@SQL);
             SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_GetCreateIndexServerStatus), 0);

             SELECT CatalogIndexId,SearchIndexMonitorId,SourceId,SourceType,SourceTransactionType,AffectedType,ISNULL(ServerName,'') as ServerName,ISNULL(Status,'') as Status,
             ISNULL(CreatedBy,'')as CreatedBy,UserName,ISNULL(CreatedDate,'')as CreatedDate,ISNULL(ModifiedBy,'')as ModifiedBy,ISNULL(ModifiedDate,'')as ModifiedDate FROM @TBL_GetCreateIndexServerStatus;

         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCreateIndexServerStatus @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCreateIndexServerStatus',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;