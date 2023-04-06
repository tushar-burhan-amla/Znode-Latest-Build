CREATE PROCEDURE [dbo].[Znode_GetCreateCMSPageIndexServerStatus]
(   @WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT)
AS 
/*
     Summary :- This procedure is used to get the publish products details 
     Unit Testing 
     EXEC Znode_GetCreateCMSPageIndexServerStatus '',@RowsCount=0

	*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_GetCreateCMSPageIndexServerStatus TABLE (CMSSearchIndexId INT,CMSSearchIndexMonitorId INT,SourceId	INT,SourceType NVARCHAR(500),SourceTransactionType NVARCHAR(500),
             AffectedType NVARCHAR(500),ServerName NVARCHAR(200),Status NVARCHAR(200),UserName NVARCHAR(512),CreatedBy INT,CreatedDate DATETIME,ModifiedBy INT,ModifiedDate DATETIME
             ,RowId INT,CountNo INT);

             SET @SQL = '
				;With Cte_GetCreateCMSPageIndexServerStatusDetails AS 
				(
				SELECT zsim.CMSSearchIndexId,zsim.CMSSearchIndexMonitorId, zsim.SourceId, zsim.SourceType, zsim.SourceTransactionType, zsim.AffectedType, zsiss.ServerName
					, zsiss.Status,zsiss.CreatedBy,zsiss.CreatedDate,zsiss.ModifiedBy,zsiss.ModifiedDate, IsNull(VIUSD.UserName,''Scheduled Task'') as UserName 
				FROM ZnodeCMSSearchIndexMonitor zsim 
				LEFT OUTER JOIN ZnodeCMSSearchIndexServerStatus zsiss ON zsim.CMSSearchIndexMonitorId=zsiss.CMSSearchIndexMonitorId
				LEFT JOIN View_GetUserDetails VIUSD ON (VIUSD.UserId = ZSIM.CreatedBy) 
				)
				, Cte_GetCreateCMSPageIndexServerStatusDetailsData AS
				(
		
				SELECT *,'+dbo.Fn_GetPagingRowId(@Order_BY,'CMSSearchIndexMonitorId ')+',Count(*)Over() CountNo
				FROM Cte_GetCreateCMSPageIndexServerStatusDetails CTPC 
				WHERE 1=1 
						  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				)

				SELECT CMSSearchIndexId,CMSSearchIndexMonitorId,SourceId,SourceType,SourceTransactionType,AffectedType,ServerName,Status,UserName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RowId,CountNo
				FROM Cte_GetCreateCMSPageIndexServerStatusDetailsData 
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
            
             INSERT INTO @TBL_GetCreateCMSPageIndexServerStatus
             EXEC (@SQL);
             SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_GetCreateCMSPageIndexServerStatus), 0);

             SELECT CMSSearchIndexId,CMSSearchIndexMonitorId,SourceId,SourceType,SourceTransactionType,AffectedType,ISNULL(ServerName,'') as ServerName,ISNULL(Status,'') as Status,
             ISNULL(CreatedBy,'')as CreatedBy,UserName,ISNULL(CreatedDate,'')as CreatedDate,ISNULL(ModifiedBy,'')as ModifiedBy,ISNULL(ModifiedDate,'')as ModifiedDate FROM @TBL_GetCreateCMSPageIndexServerStatus;

         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCreateCMSPageIndexServerStatus @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCreateCMSPageIndexServerStatus',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;