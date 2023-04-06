CREATE PROCEDURE [dbo].[Znode_DeleteERPTaskScheduler]
( @ERPTaskSchedulerId VARCHAR(500),
  @Status             BIT OUT)
AS 
   /* 
    Summary:Remove enrty from ERP Scheduler details with setting     
    Unit Testing 
	begin tran  
    DECLARE	@return_value int,
    @Status bit
    EXEC	@return_value = [dbo].[Znode_DeleteERPTaskScheduler]
	rollback tran
    @ERPTaskSchedulerId = N'10',
    @Status = @Status OUTPUT
    SELECT	@Status as N'@Status'
    SELECT	'Return Value' = @return_value
     
	*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
             DECLARE @DeletdERPTaskSchedulerId TABLE(ERPTaskSchedulerId INT);
             DECLARE @DeleteERPTaskSchedulerSetting TABLE(TaskSchedulerSettingId INT);
             INSERT INTO @DeletdERPTaskSchedulerId
                    SELECT Item
                    FROM dbo.split(@ERPTaskSchedulerId, ',') AS a;
             
             DELETE FROM ZnodeERPTaskScheduler
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdERPTaskSchedulerId AS a
                 WHERE a.ERPTaskSchedulerId = ZnodeERPTaskScheduler.ERPTaskSchedulerId
             );
             
             IF
             (
                 SELECT COUNT(1)
                 FROM @DeletdERPTaskSchedulerId
             ) =
             (
                 SELECT COUNT(1)
                 FROM dbo.split(@ERPTaskSchedulerId, ',') AS a
             )
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS Status;
                 END;
             SET @Status = CAST(1 AS BIT);
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteERPTaskScheduler @ERPTaskSchedulerId = '+@ERPTaskSchedulerId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeleteERPTaskScheduler',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;