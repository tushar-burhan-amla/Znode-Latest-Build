CREATE PROCEDURE [dbo].[Znode_GetERPTaskSchedulerDetail]
( @ERPTaskSchedulerId INT)
AS
  /*
    
    Summary:  List of erp task scheduler details by ERPTaskSchedulerId    		              
    Unit Testing         	
       EXEC Znode_GetERPTaskSchedulerDetail 1   
   */ 
	 BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             SELECT ERPTaskSchedulerId, 
					ISNULL(ERPConfiguratorId, 0) ERPConfiguratorId,
					SchedulerName, 
					SchedulerType,
					TouchPointName,
					SchedulerFrequency,
					CASE WHEN ISNULL(StartDate,'1900-01-01 00:00:00.000')='1900-01-01 00:00:00.000' THEN null ELSE StartDate END AS StartDate,
					ZETS.CreatedBy,
					ZETS.CreatedDate,
					ZETS.ModifiedBy,
					ZETS.ModifiedDate,
					ZETS.IsEnabled,
					SchedulerCallFor,
					CronExpression,
					HangfireJobId
             FROM ZnodeERPTaskScheduler AS ZETS
             WHERE ZETS.ERPTaskSchedulerId = @ERPTaskSchedulerId;
         END TRY
         BEGIN CATCH
               DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetERPTaskSchedulerDetail @ERPTaskSchedulerId = '+CAST(@ERPTaskSchedulerId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetERPTaskSchedulerDetail',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;