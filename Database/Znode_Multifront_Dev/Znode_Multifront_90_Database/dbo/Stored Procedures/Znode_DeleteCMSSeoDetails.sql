CREATE PROCEDURE [dbo].[Znode_DeleteCMSSeoDetails]
(
	@SEOCode VARCHAR(600) ='',
	@CMSSEOTypeId INT,
	@PortalId INT,
	@Status INT = 0 OUT
	
)
AS
BEGIN
BEGIN TRY
BEGIN TRAN
	SET NOCOUNT ON;

	DELETE ZCSDL FROM ZnodeCMSSEODetailLocale ZCSDL
	where EXISTS(SELECT * FROM ZnodeCMSSEODetail ZCSD where ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId
				and ZCSD.SEOCode = @SEOCode And CMSSeoTypeId = @CMSSEOTypeId AND isnull(ZCSD.PortalId,0) = @PortalId)

	DELETE FROM ZnodeCMSSEODetail  
	where SEOCode = @SEOCode and isnull(PortalId,0) = @PortalId
	AND CMSSeoTypeId = @CMSSEOTypeId

	SET @Status = 1;
	SELECT 1 AS ID , CAST(@Status AS BIT) AS Status;
COMMIT TRAN
END TRY  
  
BEGIN CATCH  
    ROLLBACK TRAN
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),   
    @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteCMSSeoDetails   
    @CMSSEOTypeId = '+CAST(@CMSSEOTypeId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(50));  

     SET @Status =0    
     SELECT 1 AS ID,@Status AS Status;   
     EXEC Znode_InsertProcedureErrorLog  
                  @ProcedureName = 'Znode_DeleteCMSSeoDetails',  
                  @ErrorInProcedure = @Error_procedure,  
                  @ErrorMessage = @ErrorMessage,  
                  @ErrorLine = @ErrorLine,  
                  @ErrorCall = @ErrorCall;  
         
END CATCH  

END