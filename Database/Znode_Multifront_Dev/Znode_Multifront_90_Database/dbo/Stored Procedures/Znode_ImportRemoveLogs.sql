CREATE PROCEDURE [dbo].[Znode_ImportRemoveLogs]
(    @ImportProcessLogId VARCHAR(2000),
	 @Status  BIT OUT,
	 @IsDebug bit = 0 )
AS
   /*
     Summary: Delete Import log data with respective job present in sql server agent 
    
	 Unit Testing   
     EXEC Znode_RemoveImportLogs 17,1
*/
     BEGIN

         BEGIN TRAN DeleteProcessLog;
         BEGIN TRY
             SET NOCOUNT ON;

             DECLARE @TBL_DeleteImportProcessLogs TABLE(ImportProcessLogId INT); -- table use to hold the Process log id 
             
			 DECLARE @TBL_DeletedImportProcessLogs TABLE(ImportProcessLogId INT); -- After Delete Process log id 

             DECLARE @JobName NVarchar(400);
             
			 INSERT INTO @TBL_DeleteImportProcessLogs SELECT Item FROM dbo.split(@ImportProcessLogId, ',') AS SP;
             
			 SELECT TOP 1 @JobName = Guid FROM ZnodeImportLog ZIL WHERE 
			 EXISTS(SELECT TOP 1 1 FROM @TBL_DeleteImportProcessLogs  DIPL WHERE DIPL.ImportProcessLogId = ZIL.ImportProcessLogId);

			--Remove job 
			
			 
			 --Remove Log         
			 DELETE from ZnodeImportLog where  EXISTS (Select TOP 1 1 FROM @TBL_DeleteImportProcessLogs DIP WHERE  DIP.ImportProcessLogId = ZnodeImportLog.ImportProcessLogId)

			 --Remove Process             
			 DELETE FROM ZnodeImportProcessLog OUTPUT DELETED.ImportProcessLogId INTO @TBL_DeletedImportProcessLogs
             WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_DeleteImportProcessLogs AS TBDA WHERE TBDA.ImportProcessLogId = ZnodeImportProcessLog.ImportProcessLogId);
             
			 IF( SELECT COUNT(1) FROM @TBL_DeleteImportProcessLogs) = (SELECT COUNT(1) FROM dbo.split(@ImportProcessLogId, ',') AS a) -- check statement if count is equal the data set return true aother wise false 
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS Status;
                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS Status;
                     SET @Status = 0;
                 END;
             COMMIT TRAN DeleteProcessLog;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteProcessLog @ImportProcessLogId = '+@ImportProcessLogId+',@Status='+CAST(@Status AS VARCHAR(50));
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
             SET @Status = 0;
             ROLLBACK TRAN DeleteProcessLog;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteProcessLog',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;