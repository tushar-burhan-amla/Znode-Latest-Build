CREATE PROCEDURE [dbo].[Znode_SaveVariantsData]
(
	@LocaleId INT,
	@CMSContainerTemplateId INT,
	@CMSContainerProfileVariantId INT,
	@UserId INT=0,
	@IsActive BIT,
	@status BIT OUT
)
AS
--EXEC Znode_SaveVariantsData 0,0,0,0,0,0
BEGIN
	BEGIN TRY
	BEGIN TRAN SaveVariantsData
		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
		DECLARE @PublishStateId INT =(SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE StateName='Draft');

		IF EXISTS (SELECT TOP 1 1 FROM ZnodeCMSContainerProfileVariantLocale WHERE CMSContainerProfileVariantId=@CMSContainerProfileVariantId)
		BEGIN
			IF EXISTS (SELECT TOP 1 1 FROM ZnodeCMSContainerProfileVariantLocale
						WHERE CMSContainerProfileVariantId=@CMSContainerProfileVariantId AND LocaleId=@LocaleId)
			BEGIN
				UPDATE ZnodeCMSContainerProfileVariantLocale
				SET CMSContainerTemplateId=@CMSContainerTemplateId, IsActive=@IsActive
				WHERE CMSContainerProfileVariantId=@CMSContainerProfileVariantId AND LocaleId=@LocaleId
					--AND CMSContainerTemplateId<>@CMSContainerTemplateId;

				UPDATE ZnodeCMSContainerProfileVariant
				SET PublishStateId = @PublishStateId,ModifiedBy=@UserId, ModifiedDate=@GetDate
				WHERE CMSContainerProfileVariantId=@CMSContainerProfileVariantId;

				UPDATE ZnodeCMSContentContainer
				SET PublishStateId=@PublishStateId,ModifiedBy=@UserId, ModifiedDate=@GetDate
				WHERE CMSContentContainerId=(SELECT CMSContentContainerId FROM ZnodeCMSContainerProfileVariant
											WHERE CMSContainerProfileVariantId=@CMSContainerProfileVariantId);
			END
			ELSE
			BEGIN
				INSERT INTO ZnodeCMSContainerProfileVariantLocale
					(CMSContainerProfileVariantId,CMSContainerTemplateId,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsActive)
				SELECT @CMSContainerProfileVariantId,@CMSContainerTemplateId,@LocaleId,@UserId,@GetDate,@UserId,@GetDate,@IsActive;

				UPDATE ZnodeCMSContainerProfileVariant
				SET PublishStateId = @PublishStateId,ModifiedBy=@UserId, ModifiedDate=@GetDate
				WHERE CMSContainerProfileVariantId=@CMSContainerProfileVariantId;

				UPDATE ZnodeCMSContentContainer
				SET PublishStateId=@PublishStateId,ModifiedBy=@UserId, ModifiedDate=@GetDate
				WHERE CMSContentContainerId=(SELECT CMSContentContainerId FROM ZnodeCMSContainerProfileVariant
											WHERE CMSContainerProfileVariantId=@CMSContainerProfileVariantId);
			END
		END
		ELSE
		BEGIN
			INSERT INTO ZnodeCMSContainerProfileVariantLocale
					(CMSContainerProfileVariantId,CMSContainerTemplateId,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsActive)
			SELECT @CMSContainerProfileVariantId,@CMSContainerTemplateId,@LocaleId,@UserId,@GetDate,@UserId,@GetDate,@IsActive;

			UPDATE ZnodeCMSContainerProfileVariant
				SET PublishStateId = @PublishStateId,ModifiedBy=@UserId, ModifiedDate=@GetDate
				WHERE CMSContainerProfileVariantId=@CMSContainerProfileVariantId;

			UPDATE ZnodeCMSContentContainer
			SET PublishStateId=@PublishStateId,ModifiedBy=@UserId, ModifiedDate=@GetDate
			WHERE CMSContentContainerId=(SELECT CMSContentContainerId FROM ZnodeCMSContainerProfileVariant
										WHERE CMSContainerProfileVariantId=@CMSContainerProfileVariantId);
		END

		SET @Status = 1;
		SELECT 1 AS ID, CAST(1 AS BIT) AS [Status];

	COMMIT TRAN SaveVariantsData;
	END TRY

	BEGIN CATCH
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			@ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_SaveVariantsData 
			@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',
			@CMSContainerTemplateId='+CAST(@CMSContainerTemplateId AS VARCHAR(50))+',
			@CMSContainerProfileVariantId='+CAST(@CMSContainerProfileVariantId AS VARCHAR(50))+',
			@UserId='+CAST(@UserId AS VARCHAR(50))+',
			@Status='+CAST(@Status AS VARCHAR(50));

		SET @Status =0
		SELECT 1 AS ID,@Status AS Status;
		ROLLBACK TRAN SaveVariantsData;
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_SaveVariantsData',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

	END CATCH
END