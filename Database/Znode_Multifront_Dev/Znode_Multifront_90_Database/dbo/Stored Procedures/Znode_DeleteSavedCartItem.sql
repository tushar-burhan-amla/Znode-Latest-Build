CREATE PROCEDURE [dbo].[Znode_DeleteSavedCartItem]  
(  
	@OmsCookieMappingId INT = 0,  
	@UserId INT = 0,
	@PortalId INT,
	@Status Bit OUT  
)  
/*  
 EXEC Znode_DeleteSavedCartItem  
 
*/  
 
AS  
BEGIN  
--BEGIN TRAN  
SET NOCOUNT ON;
BEGIN TRY
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

	IF(@OmsCookieMappingId < 1 AND @UserId > 0)
	BEGIN
		SET @OmsCookieMappingId =(SELECT TOP 1 OmsCookieMappingId FROM ZnodeOmsCookieMapping WITH (NOLOCK) WHERE UserId = @UserId AND ISNULL(PortalId,0) = @PortalId )
	END

    DECLARE @OmsSavedCartId INT =  ( SELECT TOP 1 OmsSavedCartId FROM ZnodeOmsSavedCart WITH (NOLOCK) WHERE OmsCookieMappingId = @OmsCookieMappingId )  
     
	 ----Adding dummy CookieMappingId if not present
	IF NOT EXISTS(SELECT * FROM ZnodeOmsCookieMapping WITH (NOLOCK) where OmsCookieMappingId = 1)
	BEGIN
		SET IDENTITY_INSERT ZnodeOmsCookieMapping ON
		INSERT INTO ZnodeOmsCookieMapping(OmsCookieMappingId,UserId,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT 1,null,(select top 1 PortalId from ZnodePortal order by 1 ASC),2,@GetDate,2,@GetDate
		SET IDENTITY_INSERT ZnodeOmsCookieMapping OFF
	END

	----geting dummy OmsSavedCartId on basis of OmsCookieMappingId = 1 if not present then add
	Declare @OmsSavedCartIdDummy int = 0
	SET @OmsSavedCartIdDummy = (Select Top 1 OmsSavedCartId  from ZnodeOmsSavedCart With(NoLock) where OmsCookieMappingId = 1)
	IF Isnull(@OmsSavedCartIdDummy ,0) = 0 
	BEGIN 
		INSERT INTO ZnodeOmsSavedCart(OmsCookieMappingId,SalesTax,RecurringSalesTax,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT  1,null,null,2,@GetDate,2,@GetDate
		SET @OmsSavedCartIdDummy  = @@IDENTITY
	END
   
	UPDATE ZnodeOmsSavedCartLineItem SET OmsSavedCartId = @OmsSavedCartIdDummy
	WHERE ZnodeOmsSavedCartLineItem.OmsSavedCartId = @OmsSavedCartId  
 
  SET @Status = 1  
 
  SELECT @OmsCookieMappingId Id , CAST(1 AS BIT ) Status  
 
--COMMIT TRAN  
 
END TRY  
BEGIN CATCH  
	--ROLLBACK TRAN
    SET @Status = 1  
  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteSavedCartItem @OmsCookieMappingId = '+CAST(@OmsCookieMappingId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
	SELECT @OmsCookieMappingId Id , CAST(0 AS BIT ) Status                  
		  
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_DeleteSavedCartItem',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall; 
    
END CATCH  
END