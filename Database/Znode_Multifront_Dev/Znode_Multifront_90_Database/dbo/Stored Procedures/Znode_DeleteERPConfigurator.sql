CREATE PROCEDURE [dbo].[Znode_DeleteERPConfigurator]
( @ERPConfiguratorId VARCHAR(2000),
  @Status            BIT OUT)
AS
   /* 
     Summary : Remove ERP Scheduler and Configurator details  
			   Here complete delete the ERP Scheduler and Configurator and their references without any check  
			   If passed @ERPConfiguratorId are matched with deleted count then data set return true other wise false 
			   dbo.Split function use to make comma seperated data in table rows 
			   1 ZnodeERPConfigurator
			   2 ZnodeERPTaskScheduler
     Unit Testing 
	 begin tran
     DEclare @Status bit = 1
     EXEC Znode_DeleteERPConfigurator  29 ,@Status =@Status OUT 
	 rollback tran
     SELECT @Status 
    
	*/
     BEGIN
         BEGIN TRAN DeleteERPConfigurator;
         BEGIN TRY
             SET NOCOUNT ON;
			  -- table hold the ERPConfigurator Id 
             DECLARE @TBL_DeletdERPConfiguratorId TABLE(ERPConfiguratorId INT);
			 
             INSERT INTO @TBL_DeletdERPConfiguratorId
			        -- dbo.Split function use to make comma separeted data in table rows
                    SELECT Item FROM dbo.split(@ERPConfiguratorId, ',') AS a;
             
             DELETE FROM ZnodeERPConfigurator
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletdERPConfiguratorId AS TBDEC
                 WHERE TBDEC.ERPConfiguratorId = ZnodeERPConfigurator.ERPConfiguratorId
             );
             
             IF (SELECT COUNT(1) FROM @TBL_DeletdERPConfiguratorId) =
             (SELECT COUNT(1) FROM dbo.split(@ERPConfiguratorId, ',') AS SP) -- if both count are equal then data set return true other wise return false
                 BEGIN
                     SELECT 1 AS ID, CAST(1 AS BIT) AS [Status];
                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID, CAST(0 AS BIT) AS [Status];
                     SET @Status = 0;
                 END;
             COMMIT TRAN DeleteERPConfigurator;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteERPConfigurator @ERPConfiguratorId = '+@ERPConfiguratorId+',@Status='+CAST(@Status AS VARCHAR(50));
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS [Status];
             SET @Status = 0;
             ROLLBACK TRAN DeleteERPConfigurator;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteERPConfigurator',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;