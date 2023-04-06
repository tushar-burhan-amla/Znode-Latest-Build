

CREATE PROCEDURE [dbo].[Znode_AssociateBrandPortal] 
(
	@PortalId VARCHAR(MAX) = '',
	@BrandId  INT = 0,
	@IsUnAssociated BIT, -----0 = UnAssociate, 1 = Associate
	@UserId INT,
	@Status BIT OUT
)
AS
BEGIN
	SET NOCOUNT ON

 BEGIN TRY
	DECLARE @GetDate DATETIME= dbo.Fn_GetDate();

	IF ( @IsUnAssociated = 1 )
	BEGIN
		
		INSERT INTO ZnodeBrandPortal ( PortalId, BrandId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate ) 
		SELECT P.Item, @BrandId , @UserId, @GetDate, @UserId, @GetDate
		FROM dbo.Split ( @PortalId , ',' ) P
		WHERE EXISTS (SELECT * FROM ZnodePortal ZP WHERE ZP.PortalId = P.Item )
		AND NOT EXISTS ( SELECT * FROM ZnodeBrandPortal BP WHERE P.Item = BP.PortalId AND BP.BrandId = @BrandId )

	END
	ELSE IF ( @IsUnAssociated = 0 )
	BEGIN

		DELETE FROM ZnodeBrandPortal
		WHERE EXISTS( SELECT * FROM dbo.Split ( @PortalId , ',' ) P WHERE ZnodeBrandPortal.PortalId = P.Item AND ZnodeBrandPortal.BrandId = @BrandId )

	END

	SELECT 1 AS ID, CAST(1 AS bit) AS Status;

END TRY
         BEGIN CATCH

		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_AssociateBrandPortal @PortalId = '+CAST(@PortalId AS VARCHAR(max))+',@BrandId='+CAST(@BrandId AS VARCHAR(50))+',@IsUnAssociated='+CAST(@IsUnAssociated AS VARCHAR(50))+',@UserId='+CAST( @UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_AssociateBrandPortal',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH

END