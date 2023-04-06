CREATE PROCEDURE [dbo].[Znode_DeleteProcedureErrorLog]
(   @ProcedureName  VARCHAR(1000) = '',
	@Status BIT OUT
)
AS
/*
EXEC Znode_DeleteProcedureErrorLog @ProcedureName = 'Znode_ManageProductList_XML',@Status =0

EXEC Znode_DeleteProcedureErrorLog @Status =0

EXEC Znode_DeleteProcedureErrorLog @ProcedureName = ''
*/
BEGIN
         
         BEGIN TRY
             SET NOCOUNT ON;

			 -- will delete all records
			 IF @ProcedureName = ''
				 BEGIN
				 TRUNCATE TABLE ZnodeProceduresErrorLog
				 END

			 ELSE
			 -- will delete records based on parameter (sp name)
				 BEGIN
				 DELETE FROM ZnodeProceduresErrorLog
				 WHERE ProcedureName = @ProcedureName
				 END

			 SELECT 1 AS ID,CAST(1 AS BIT) AS Status;
			  SET @Status = 1;
		 END TRY
         BEGIN CATCH
		    
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteProcedureErrorLog @ProcedureName = '''+ISNULL(@ProcedureName,'''''')+''',@Status = '+ISNULL(CAST(@Status AS varchar(10)),'''');
              			 
             --SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeleteProcedureErrorLog',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
			 
         END CATCH;
 
END;