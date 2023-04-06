CREATE  PROCEDURE [dbo].[Znode_GetImportERPConnectorLogs]
( @WhereClause NVARCHAR(max),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(1000)  = '',
  @RowsCount   INT OUT)
AS
    /*
	
    Summary : Get Import Template Log details and errorlog associated to it
    Unit Testing 
	BEGIN TRAN
    DECLARE @RowsCount INT;
    EXEC Znode_GetImportERPConnectorLogs @WhereClause = ' ',@Rows = 1000,@PageNo = 1,@Order_BY = NULL,@RowsCount = @RowsCount OUT;
	rollback tran
   
    */
	 BEGIN
        BEGIN TRY
          SET NOCOUNT ON;
		     DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_ErrorLog TABLE(ERPTaskSchedulerId INT, SchedulerName NVARCHAR(200),SchedulerType VARCHAR(20) ,
										TouchPointName NVARCHAR(200),
										[ImportStatus] VARCHAR(50) ,ProcessStartedDate DATETIME ,ProcessCompletedDate DATETIME
										,ImportProcessLogId INT,RowId INT,CountNo  INT,SuccessRecordCount int,FailedRecordcount INT)

             SET @SQL = ' 
			 	

					   ;With Cte_ErrorLog AS (
									   select zih.ERPTaskSchedulerId, 
									   Case when zih.SchedulerType = ''RealTime'' then zih.SchedulerName + '' - '' + ''RealTime''
									   else zih.SchedulerName end SchedulerName, 
									   zih.SchedulerType,
									   CASE when ZIT.ImportTemplateId is not null THEN 
									   zih.TouchPointName + '' - '' + ZIT.TemplateName
									   ELSE zih.TouchPointName END TouchPointName
									   ,zipl.Status ImportStatus ,
									   zipl.ProcessStartedDate,
									   zipl.ProcessCompletedDate,zipl.ImportProcessLogId,
									   zipl.SuccessRecordCount,zipl.FailedRecordcount
									   from ZnodeImportProcessLog zipl 
									   Inner join ZnodeERPTaskScheduler zih on zipl.ERPTaskSchedulerId = zih.ERPTaskSchedulerId
									   LEFT Outer JOIN ZnodeImportTemplate ZIT ON zipl.ImportTemplateId = ZIT.ImportTemplateId
									   Inner join ZnodeERPConfigurator ZEC ON zih.ERPConfiguratorId = ZEC.ERPConfiguratorId 
									   AND ZEC.IsActive = 1 
								     ) 
						,Cte_ErrorlogFilter AS
						(
					   SELECT ERPTaskSchedulerId,SchedulerName,SchedulerType,TouchPointName,ImportStatus,
						ProcessStartedDate,ProcessCompletedDate,ImportProcessLogId
						,'+dbo.Fn_GetPagingRowId(@Order_BY,'ImportProcessLogId DESC')+', Count(*)Over() CountNo
					   ,SuccessRecordCount,FailedRecordcount FROM Cte_ErrorLog
					   WHERE 1 = 1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
						) 
					   SELECT ERPTaskSchedulerId,SchedulerName,SchedulerType,TouchPointName,ImportStatus,
							ProcessStartedDate,ProcessCompletedDate,ImportProcessLogId
							,RowId,CountNo ,SuccessRecordCount,FailedRecordcount
					   FROM Cte_ErrorlogFilter 
					   '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
	        
			 INSERT INTO @TBL_ErrorLog (ERPTaskSchedulerId,SchedulerName,SchedulerType,TouchPointName,ImportStatus,ProcessStartedDate,ProcessCompletedDate,ImportProcessLogId,RowId,CountNo,SuccessRecordCount,FailedRecordcount )
			 EXEC(@SQl)												
             SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ErrorLog ), 0);

			 SELECT ERPTaskSchedulerId , SchedulerName ,SchedulerType  ,
										TouchPointName ,
										[ImportStatus] ,ProcessStartedDate ,ProcessCompletedDate 
										,ImportProcessLogId ,RowId ,CountNo  ,SuccessRecordCount,FailedRecordcount
			 FROM @TBL_ErrorLog
             
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetImportTemplateLogs @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status,ERROR_MESSAGE();                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetImportTemplateLogs',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                   
         END CATCH;
     END;