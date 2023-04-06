CREATE PROCEDURE [dbo].[Znode_GetImportProcessStatus]
	(@Status BIT = 0 OUT)
AS
 /*
  Summary :-  This Procedure is use to get the status of the last record.

  Unit Testing
  
  begin tran
  EXEC  [dbo].[Znode_GetImportProcessStatus]
  rollback tran


*/
BEGIN
	
	BEGIN TRY
		 BEGIN TRAN OrderRMAFlag
		 DECLARE @ImportStatus nvarchar(100);
		 SELECT @ImportStatus=Status FROM ZnodeImportProcessLog WHERE ImportProcessLogId = (SELECT  MAX(ImportProcessLogId) FROM ZnodeImportProcessLog)
		
		 IF (@ImportStatus = '' or @ImportStatus is null)
			BEGIN
				THROW 50001, 'No records found', 1;
			END
		 ELSE IF (@ImportStatus = 'Failed' OR @ImportStatus = 'Completed')
			BEGIN
				SELECT 1 ID, CAST(1 AS BIT) [Status];
			END
		 ELSE
			BEGIN
				SELECT 1 ID, CAST(0 AS BIT) [Status];
			END

		 COMMIT TRAN OrderRMAFlag;
    END TRY
    BEGIN CATCH

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetImportProcessStatus @Status='+CAST(@Status AS VARCHAR(50));
        SET @Status = 0;			 
        SELECT 0 AS ID,
            CAST(0 AS BIT) AS Status;
			ROLLBACK TRAN OrderRMAFlag;
        EXEC Znode_InsertProcedureErrorLog
            @ProcedureName = 'Znode_GetImportProcessStatus',
            @ErrorInProcedure = @Error_procedure,
            @ErrorMessage = @ErrorMessage,
            @ErrorLine = @ErrorLine,
            @ErrorCall = @ErrorCall;
			 
        END CATCH;
END;