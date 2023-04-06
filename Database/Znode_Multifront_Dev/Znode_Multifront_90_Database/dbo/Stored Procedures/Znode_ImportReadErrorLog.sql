CREATE PROCEDURE [dbo].[Znode_ImportReadErrorLog]
(   @ImportProcessLogId INT,
    @NewGUID            NVARCHAR(200),
    @ValidationValue    NVARCHAR(300) = '')
AS
/*
Summary: This Procedure is used to read error log for Import process
Unit Testing:
EXEC Znode_ImportReadErrorLog 
*/
     BEGIN
	 BEGIN TRY
	 SET NOCOUNT ON
         DECLARE @sSql NVARCHAR(MAX), @RoundOffValue INT, @MessageDisplay NVARCHAR(100), @ImportHead NVARCHAR(100);
         SELECT TOP 1 @ImportHead = zih.Name
         FROM ZnodeImportProcesslog zip
              INNER JOIN ZnodeImportLog zil ON zip.ImportProcessLogId = zil.ImportProcessLogId
              INNER JOIN ZnodeImportTemplate zit ON zip.ImportTemplateId = zit.ImportTemplateId
              INNER JOIN ZnodeImportHead zih ON zit.ImportHeadId = zih.ImportHeadId
         WHERE zip.ImportProcessLogId = @ImportProcessLogId;
         BEGIN
             SELECT zil.ImportLogId,
                    zil.ImportProcessLogId,
                    zil.ErrorDescription AS ErrorCode,
                    zil.RowNumber,
                    zil.ColumnName+': '+zm.MessageName+CASE
                                                           WHEN zm.MessageCode IN(16, 17, 41)
                                                           THEN+'  '+CASE
                                                                         WHEN @ImportHead = 'Inventory'
                                                                         THEN dbo.Fn_GetDefaultInventoryRoundOff(ISNULL(DefaultErrorValue, '0000000.00'))
                                                                         ELSE dbo.Fn_GetDefaultPriceRoundOff(ISNULL(DefaultErrorValue, '0000000.00'))
                                                                     END
                                                           ELSE ISNULL(DefaultErrorValue, '')
                                                       END,
                    zil.Data,
                    zil.Guid,
                    zil.CreatedBy,
                    zil.CreatedDate,
                    zil.ModifiedBy,
                    zil.ModifiedDate
             FROM ZnodeImportLog AS zil
                  INNER JOIN ZnodeMessage AS zm ON zil.ErrorDescription = CONVERT( VARCHAR(50), zm.MessageCode)
                                                   AND zil.GUID = @NewGUID
             WHERE zil.ImportProcessLogId = @ImportProcessLogId;
         END;
		 END TRY
		 BEGIN CATCH
			DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportReadErrorLog @ImportProcessLogId = '+CAST(@ImportProcessLogId AS VARCHAR(200))+',@NewGUID='+CAST(@NewGUID AS VARCHAR(50))+',@ValidationValue='+CAST(@ValidationValue AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportReadErrorLog',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		 END CATCH
     END;