CREATE PROCEDURE [dbo].[ZnodeReport_GetUserAssociatedPortal] 
(@LoginUserId INT )
AS

/*
Summary: Get associated portals from userId 
         
	   EXEC ZnodeReport_GetUserAssociatedPortal 1060;
	   EXEC ZnodeReport_GetUserAssociatedPortal 2;
	   EXEC ZnodeReport_GetUserAssociatedPortal 565766;

*/

     BEGIN
	 BEGIN TRY
	 SET NOCOUNT ON
         DECLARE @ZnodeUserPortal TABLE(PortalId INT);
         INSERT INTO @ZnodeUserPortal
                SELECT aup.PortalId
                FROM dbo.ZnodeUser AS zu
                     INNER JOIN ZnodeUserPortal AS AUP ON(AUP.UserId = ZU.UserId)
                WHERE zu.UserId = @LoginUserId;
         IF EXISTS
         (
             SELECT TOP 1 1
             FROM @ZnodeUserPortal
             WHERE PortalId IS NOT NULL
         )
             BEGIN
                 SELECT zup.PortalId,
                        zp.StoreName
                 FROM @ZnodeUserPortal AS zup
                      INNER JOIN ZnodePortal AS zp ON zup.PortalId = zp.PortalId
                 ORDER BY zp.StoreName;
             END;
         ELSE
             BEGIN
                 IF EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @ZnodeUserPortal
                     WHERE PortalId IS NULL
                 )
                     BEGIN
                         SELECT PortalId,
                                StoreName
                         FROM ZnodePortal
                         ORDER BY StoreName;
                     END;
                 ELSE
                     BEGIN
                         SELECT PortalId,
                                StoreName
                         FROM ZnodePortal
                         WHERE PortalId IN
                         (
                             SELECT PortalId
                             FROM @ZnodeUserPortal
                         );
                     END;
             END;

	END TRY
	BEGIN CATCH
	  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetUserAssociatedPortal @LoginUserId='+CAST(@LoginUserId AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetUserAssociatedPortal',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH
     END;