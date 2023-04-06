CREATE PROCEDURE [dbo].[Znode_GetCMSContentFolder]
(@LocaleId INT = 1)
AS
   /*
    Summary:To get list of media filter associated with users      
    Unit Testing
    Exec Znode_GetCMSContentFolder 
	*/
     BEGIN
         BEGIN TRY
             SELECT zmp.CMSContentPageGroupId,zmp.[Code],zmpl.[Name],zmp.ParentCMSContentPageGroupId
             FROM ZnodeCMSContentPageGroup AS zmp
             INNER JOIN ZnodeCMSContentPageGroupLocale AS zmpl ON(zmp.CMSContentPageGroupId = zmpl.CMSContentPageGroupId)
             WHERE zmpl.LocaleId = @LocaleId;
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSContentFolder @LocaleId = '+CAST(@LocaleId AS VARCHAR(10))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSContentFolder',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;