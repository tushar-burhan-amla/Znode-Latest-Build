CREATE PROCEDURE [dbo].[Znode_GetPortalCatalog]
(@PortalIds VARCHAR(2000))
AS
/*
Summary:This Procedure is used to get information about portal catalog
Unit Testing:
EXEC Znode_GetPortalCatalog 6
*/
     BEGIN
	 BEGIN TRY
	 SET NOCOUNT ON
         SELECT PortalCatalogId,PortalCatalogId,PortalId,PublishCatalogId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
         FROM ZnodePortalCatalog;
     END TRY
	 BEGIN CATCH
	 DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPortalCatalog @PortalIds = '+CAST(@PortalIds AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPortalCatalog',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;

	 END CATCH
     END;