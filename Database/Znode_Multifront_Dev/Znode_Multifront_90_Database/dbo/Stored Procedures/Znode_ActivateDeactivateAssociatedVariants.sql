CREATE PROCEDURE [dbo].[Znode_ActivateDeactivateAssociatedVariants]
(
	@ContainerProfileVariantIds VARCHAR(2000),
	@IsActivate BIT,
	@status BIT OUT
)
AS
--EXEC Znode_ActivateDeactivateAssociatedVariants '1019,1021,1027',0,0
BEGIN
	BEGIN TRY
	BEGIN TRAN ActDeactivateAssociatedVariant
		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();

		DECLARE @TBL_CMSContainerProfileVariant TABLE(CMSContainerProfileVariantId INT);

		DECLARE @VariantIdsCnt INT;

		INSERT INTO @TBL_CMSContainerProfileVariant (CMSContainerProfileVariantId)
		SELECT item FROM dbo.Split(@ContainerProfileVariantIds,',')

		SELECT @VariantIdsCnt=COUNT(1)
		FROM @TBL_CMSContainerProfileVariant;

		IF EXISTS (	SELECT TOP 1 1 FROM ZnodeCMSContainerProfileVariant CPV
					INNER JOIN @TBL_CMSContainerProfileVariant TCPV ON TCPV.CMSContainerProfileVariantId=CPV.CMSContainerProfileVariantId
					WHERE (CPV.ProfileId IS NULL AND CPV.PortalId IS NULL)
					) AND (@VariantIdsCnt=1)
		BEGIN
			IF (@IsActivate=1)
			BEGIN
				SET @Status = 1;
				SELECT 1 AS ID, CAST(1 AS BIT) AS [Status];
			END
			ELSE
			BEGIN
				SET @Status = 0;
				SELECT 1 AS ID, @Status AS [Status];
			END
		END

		ELSE
		BEGIN
			UPDATE CPVL
			SET CPVL.IsActive=@IsActivate, CPVL.ModifiedDate=@GetDate
			FROM ZnodeCMSContainerProfileVariant CPV
			INNER JOIN @TBL_CMSContainerProfileVariant TCPV ON TCPV.CMSContainerProfileVariantId=CPV.CMSContainerProfileVariantId
			INNER JOIN ZnodeCMSContainerProfileVariantLocale CPVL ON CPV.CMSContainerProfileVariantId=CPVL.CMSContainerProfileVariantId
			WHERE (CPV.ProfileId IS NOT NULL OR CPV.PortalId IS NOT NULL);

			SET @Status = 1;
			SELECT 1 AS ID, CAST(1 AS BIT) AS [Status];
		END
	COMMIT TRAN ActDeactivateAssociatedVariant;
	END TRY

	BEGIN CATCH
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			@ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ActivateDeactivateAssociatedVariants 
			@ContainerProfileVariantIds = '+@ContainerProfileVariantIds+',@IsActivate='+CAST(@IsActivate AS VARCHAR(50))+',
			@Status='+CAST(@Status AS VARCHAR(50));

		SET @Status =0
		SELECT 1 AS ID,@Status AS Status;
		ROLLBACK TRAN ActDeactivateAssociatedVariant;
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_ActivateDeactivateAssociatedVariants',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

	END CATCH
END