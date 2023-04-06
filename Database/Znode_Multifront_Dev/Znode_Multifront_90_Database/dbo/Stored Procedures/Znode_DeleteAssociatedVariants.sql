CREATE PROCEDURE [dbo].[Znode_DeleteAssociatedVariants]
(
	@ContainerProfileVariantId  VARCHAR(2000),
	@Status  BIT = 0 OUT
)
AS
BEGIN
BEGIN TRY


	DECLARE @TBL_ContainerProfileVariantId TABLE(CMSContainerProfileVariantId INT); 
             
	INSERT INTO @TBL_ContainerProfileVariantId(CMSContainerProfileVariantId)
	SELECT Item FROM  dbo.Split(@ContainerProfileVariantId, ',')
		
	IF ((SELECT COUNT(1) FROM @TBL_ContainerProfileVariantId) = 1
	AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariant 
			WHERE EXISTS(SELECT * FROM @TBL_ContainerProfileVariantId WP WHERE ZnodeCMSContainerProfileVariant.CMSContainerProfileVariantId = WP.CMSContainerProfileVariantId)
			AND ZnodeCMSContainerProfileVariant.ProfileId IS NULL AND ZnodeCMSContainerProfileVariant.PortalId IS NULL))
	BEGIN
		SET @Status = 0;
		SELECT 1 AS ID,
		CAST(0 AS BIT) AS [Status] ,'The default variant cannot be deleted.' AS Message;	
		RETURN
	END
	ELSE 
	BEGIN
		BEGIN TRAN DeleteAssociatedVariants
			DELETE WP
			FROM @TBL_ContainerProfileVariantId WP
			WHERE EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariant WHERE ZnodeCMSContainerProfileVariant.CMSContainerProfileVariantId = WP.CMSContainerProfileVariantId AND ProfileId IS NULL AND PortalId IS NULL)
		
			DELETE  L FROM ZnodeWidgetGlobalAttributeValueLocale L	
			INNER JOIN ZnodeWidgetGlobalAttributeValue V ON L.WidgetGlobalAttributeValueId = V.WidgetGlobalAttributeValueId
			WHERE EXISTS(SELECT * FROM @TBL_ContainerProfileVariantId WP WHERE V.CMSContainerProfileVariantId = WP.CMSContainerProfileVariantId)

			DELETE ZnodeWidgetGlobalAttributeValue 
			WHERE EXISTS(SELECT * FROM @TBL_ContainerProfileVariantId WP WHERE ZnodeWidgetGlobalAttributeValue.CMSContainerProfileVariantId = WP.CMSContainerProfileVariantId)

			DELETE WPVL
			FROM ZnodeCMSContainerProfileVariantLocale WPVL
			WHERE EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariant WPV
				WHERE EXISTS(SELECT * FROM @TBL_ContainerProfileVariantId WP WHERE WPV.CMSContainerProfileVariantId = WP.CMSContainerProfileVariantId)
				AND WPV.CMSContainerProfileVariantId = WPVL.CMSContainerProfileVariantId)

			DELETE ZnodeCMSContainerProfileVariant 
			WHERE EXISTS(SELECT * FROM @TBL_ContainerProfileVariantId WP WHERE ZnodeCMSContainerProfileVariant.CMSContainerProfileVariantId = WP.CMSContainerProfileVariantId)

			SET @Status = 1;
			SELECT 1 AS ID,
			CAST(1 AS BIT) AS [Status],'' as Message;	
		COMMIT TRAN DeleteAssociatedVariants;
	END
		

END TRY

BEGIN CATCH
	ROLLBACK TRAN DeleteAssociatedVariants;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
	@ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteAssociatedVariants 
	@WidgetProfileVariantId = '+@ContainerProfileVariantId+',@Status='+CAST(@Status AS VARCHAR(50));

	SET @Status =0  
	SELECT 1 AS ID,@Status AS Status,'' Message;  
	
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_DeleteAssociatedVariants',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	       
END CATCH

END