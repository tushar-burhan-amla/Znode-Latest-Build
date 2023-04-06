CREATE PROCEDURE [dbo].[Znode_CopyPortalEmailTemplate] 
(
   @PortalId		INT = 0 
  ,@CopyPortalId    INT = 0   
  ,@UserId			INT = 0 

 )
 AS 
 /*
	 Summary :- This procedure copy portal email template 	 
	 EXEC  Znode_CopyPortalEmailTemplate 
 
 */
 BEGIN 
  BEGIN TRANSACTION  CopyPortalEmailTemplate 
   BEGIN TRY 
    SET NOCOUNT ON 
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

	INSERT INTO ZnodeEmailTemplateMapper (EmailTemplateId,PortalId,EmailTemplateAreasId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	SELECT EmailTemplateId,@PortalId,EmailTemplateAreasId,IsActive,@UserId,@GetDate,@UserId,@GetDate FROM ZnodeEmailTemplateMapper ZETM 	
	WHERE PortalId = @CopyPortalId
	AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeEmailTemplateMapper ZETMI WHERE ZETMI.PortalId = @PortalId )

  COMMIT TRANSACTION CopyPortalEmailTemplate
   END TRY 
   BEGIN CATCH 
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_CopyPortalEmailTemplate @PortalId = '+CAST(@PortalId AS VARCHAR(200))+',@CopyPortalId='+CAST(@CopyPortalId AS VARCHAR(50))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRANSACTION CopyPortalEmailTemplate
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_CopyPortalEmailTemplate',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
   END CATCH 
 END