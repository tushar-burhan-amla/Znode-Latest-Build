CREATE PROCEDURE [dbo].[Znode_GetActiveCatalogIds]
AS
BEGIN
BEGIN TRY 
with cte AS 
(
 SELECT PublishCatalogId    
 from ZnodePortalCatalog PC 
 left join ZnodePublishCatalogEntity  PCE on  PC.PublishCatalogId = PCE.ZnodeCatalogId 
 GROUP BY PublishCatalogId
 )
 SELECT STRING_AGG( PublishCatalogId , ',') PublishCatalogId  FROM cte

END TRY 
BEGIN CATCH 
 DECLARE @Status BIT ;    
  SET @Status = 0;    
  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetActiveCatalogIds  ';    
                      
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
        EXEC Znode_InsertProcedureErrorLog    
   @ProcedureName = 'Znode_GetActiveCatalogIds',    
   @ErrorInProcedure = @Error_procedure,    
   @ErrorMessage = @ErrorMessage,    
   @ErrorLine = @ErrorLine,    
   @ErrorCall = @ErrorCall;   

END CATCH 
END
