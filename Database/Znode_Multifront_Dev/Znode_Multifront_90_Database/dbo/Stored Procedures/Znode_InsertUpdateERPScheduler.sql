CREATE PROCEDURE [dbo].[Znode_InsertUpdateERPScheduler]
(   @SchedulerXML XML,
    @UserId       INT,
    @Status       BIT OUT)
AS
   /* 
    Summary: This procedure is used insert and update ERP scheduler data 
    		   Get average rating of products 
    		   Get Price / Inventory / SEO details .
    Unit Testing
	BEGIN TRAN   
     EXEC Znode_InsertUpdateERPScheduler 
	ROLLBACK TRAN

	*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @TBL_ERPTaskScheduler TABLE
             (ERPTaskSchedulerId    INT,
              SchedulerName         NVARCHAR(200),
			  SchedulerType			VARCHAR(20),	
              TouchPointName        NVARCHAR(200),
              SchedulerFrequency    NVARCHAR(100),
              StartDate             DATETIME,
              IsEnabled             BIT,
			  SchedulerCallFor      VARCHAR(200),
			  CronExpression		VARCHAR(100),
			  HangfireJobId			VARCHAR(100)
             );
             
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             DECLARE @ERPConfiguratorId INT=
             (
                 SELECT TOP 1 ERPConfiguratorId
                 FROM ZnodeERPConfigurator
                 WHERE IsActive = 1
             );
             DECLARE @ERPTaskShedulerId INT;
             INSERT INTO @TBL_ERPTaskScheduler
                    SELECT Tbl.Col.value('ERPTaskSchedulerId[1]', 'NVARCHAR(max)') AS ERPTaskSchedulerId,
                           Tbl.Col.value('SchedulerName[1]', 'VARCHAR(max)') AS SchedulerName,
						   Tbl.Col.value('SchedulerType[1]', 'NVARCHAR(max)') AS SchedulerType,
                           Tbl.Col.value('TouchPointName[1]', 'NVARCHAR(max)') AS TouchPointName,
                           Tbl.Col.value('SchedulerFrequency[1]', 'NVARCHAR(max)') AS SchedulerFrequency,
                           Tbl.Col.value('StartDate[1]', 'NVARCHAR(max)') AS StartDate,
                           Tbl.Col.value('IsEnabled[1]', 'NVARCHAR(max)') AS IsEnabled,
						   Tbl.Col.value('SchedulerCallFor[1]', 'NVARCHAR(max)') AS SchedulerCallFor,
						   Tbl.Col.value('CronExpression[1]', 'NVARCHAR(max)') AS CronExpression,
						   Tbl.Col.value('HangfireJobId[1]', 'NVARCHAR(max)') AS HangfireJobId
                    FROM @SchedulerXML.nodes('//ERPTaskSchedulerModel') AS Tbl(Col);

             UPDATE a
               SET
                   SchedulerName = b.SchedulerName,
				   SchedulerType = b.SchedulerType,
                   TouchPointName = b.TouchPointName,
                   SchedulerFrequency = b.SchedulerFrequency,
                   StartDate = b.StartDate,
                   IsEnabled = b.IsEnabled,
                   ModifiedBy = @UserId,
                   ModifiedDate = @GetDate,
				   CronExpression = b.CronExpression,
				   HangfireJobId = b.HangfireJobId
             FROM ZnodeERPTaskScheduler a
                  INNER JOIN @TBL_ERPTaskScheduler b ON(a.ERPTaskSchedulerId = b.ERPTaskSchedulerId);
             
             INSERT INTO ZnodeERPTaskScheduler
             (
              ERPConfiguratorId,
              SchedulerName,
			  SchedulerType,
              TouchPointName,
              SchedulerFrequency,
              StartDate,
              IsEnabled,
			  SchedulerCallFor,
              CreatedBy,
              CreatedDate,
              ModifiedBy,
              ModifiedDate,
			  CronExpression,
			  HangfireJobId
             )
                    SELECT @ERPConfiguratorId,
                           SchedulerName,
						   SchedulerType,
                           TouchPointName,
                           SchedulerFrequency,
                           StartDate,
                           IsEnabled,
						   SchedulerCallFor,
                           @UserId,
                           @GetDate,
                           @UserId,
                           @GetDate,
						   CronExpression,
						   HangfireJobId
                    FROM @TBL_ERPTaskScheduler
                    WHERE ERPTaskSchedulerId = 0;
             
			 SET @ERPTaskShedulerId = SCOPE_IDENTITY();

             SET @Status = CAST(1 AS BIT);
             SELECT
             (
                 SELECT TOP 1 CASE
                                  WHEN ISNULL(ERPTaskSchedulerId, 0) = 0
                                  THEN @ERPTaskShedulerId
                                  ELSE ERPTaskSchedulerId
                              END
                 FROM @TBL_ERPTaskScheduler
             ) AS Id,
             CAST(1 AS BIT) AS Status;
         END TRY
         BEGIN CATCH
            
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdateERPScheduler @SchedulerXML = '+CAST(@SchedulerXML AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  select ERROR_MESSAGE();
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_InsertUpdateERPScheduler',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;