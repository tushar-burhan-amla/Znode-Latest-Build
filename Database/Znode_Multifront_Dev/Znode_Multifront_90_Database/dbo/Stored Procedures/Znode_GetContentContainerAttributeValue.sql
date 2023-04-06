CREATE PROCEDURE [dbo].[Znode_GetContentContainerAttributeValue]
(
	@EntityName	NVARCHAR(200) = 0,
	@ContainerKey	NVARCHAR(200),
	@LocalId	INT = 0,
	@PortalId	INT = 0,
	@ProfileId	INT = 0
)
AS
/*
	 Summary :- This procedure is used to get the Content and Container attribute value as per filter pass
	 Unit Testing 
	 BEGIN TRAN
	 EXEC [Znode_GetContentContainerAttributeValue] @EntityName='Content Containers',@ContainerKey='',@LocalId=0,@PortalId=0,@ProfileId=0
	 ROLLBACK TRAN
*/
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		-- Variable Declaration as per the code requirement
		DECLARE @LocaleCode NVARCHAR(50),
				@LocalIdInput INT,
				@LocaleCodeInput NVARCHAR(50),
				@GlobalEntityValueId INT,
				@GroupCode NVARCHAR(200) = NULL,
				@SelectedValue BIT = 0,
				@FamilyCode VARCHAR(200);

		SET @LocalIdInput = @LocalId

		SELECT @LocaleCode=Code FROM ZnodeLocale WITH (NOLOCK) WHERE LocaleId=@LocalId;
		SET @LocaleCodeInput = @LocaleCode

		--1. Specific Store, Specific User Profile, Selected Locale
		-- Based on input if count will 1 then this will take CMSContainerProfileVariantId from 1st loop else this will go for next loop.
		IF (SELECT COUNT(1) FROM ZnodeCMSContainerProfileVariant WPV WITH (NOLOCK) WHERE ISNULL(WPV.ProfileId,0)=@ProfileId 
			AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariantLocale WPVL WHERE WPV.CMSContainerProfileVariantId = WPVL.CMSContainerProfileVariantId AND WPVL.LocaleId=@LocalId ))=1
		BEGIN
			SELECT @GlobalEntityValueId=CMSContainerProfileVariantId 
			FROM ZnodeCMSContainerProfileVariant WPV WITH (NOLOCK)
			WHERE ISNULL(WPV.ProfileId,0)=@ProfileId 
			AND EXISTS(SELECT * FROM ZnodeCMSContentContainer CW WITH (NOLOCK) WHERE CW.ContainerKey=@ContainerKey AND WPV.CMSContentContainerId = CW.CMSContentContainerId)
			AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariantLocale WPVL WHERE WPV.CMSContainerProfileVariantId = WPVL.CMSContainerProfileVariantId AND WPVL.LocaleId=@LocalId );
		END
		ELSE
		BEGIN
			SELECT @GlobalEntityValueId=CMSContainerProfileVariantId 
			FROM ZnodeCMSContainerProfileVariant WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = @ProfileId ) AND (ISNULL(WPV.PortalId,0) = @PortalId )
			AND EXISTS(SELECT * FROM ZnodeCMSContentContainer CW WITH (NOLOCK) WHERE CW.ContainerKey=@ContainerKey AND WPV.CMSContentContainerId = CW.CMSContentContainerId)
			AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariantLocale WPVL WHERE WPV.CMSContainerProfileVariantId = WPVL.CMSContainerProfileVariantId AND WPVL.LocaleId=@LocalId );
		
		END

		--2. Specific Store, Specific User Profile, Default Locale
		IF ISNULL(@GlobalEntityValueId,0) = 0
		BEGIN
			SELECT @LocalId = LocaleId FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;
			SELECT @LocaleCode = Code FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;

			SELECT @GlobalEntityValueId=CMSContainerProfileVariantId 
			FROM ZnodeCMSContainerProfileVariant WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = @ProfileId ) AND (ISNULL(WPV.PortalId,0) = @PortalId )
			AND EXISTS(SELECT * FROM ZnodeCMSContentContainer CW WITH (NOLOCK) WHERE CW.ContainerKey=@ContainerKey AND WPV.CMSContentContainerId = CW.CMSContentContainerId)
			AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariantLocale WPVL WHERE WPV.CMSContainerProfileVariantId = WPVL.CMSContainerProfileVariantId AND WPVL.LocaleId=@LocalId );
			
			IF ISNULL(@GlobalEntityValueId,0) = 0
			BEGIN
				SET @LocalId = @LocalIdInput
				SET @LocaleCode = @LocaleCodeInput
			END
		
		END

		--3. Specific Store, Any User Profile, Selected Locale
		IF ISNULL(@GlobalEntityValueId,0) = 0
		BEGIN
			SELECT @GlobalEntityValueId=CMSContainerProfileVariantId 
			FROM ZnodeCMSContainerProfileVariant WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = 0 ) AND (ISNULL(WPV.PortalId,0) = @PortalId )
			AND EXISTS(SELECT * FROM ZnodeCMSContentContainer CW WITH (NOLOCK) WHERE CW.ContainerKey=@ContainerKey AND WPV.CMSContentContainerId = CW.CMSContentContainerId)
			AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariantLocale WPVL WHERE WPV.CMSContainerProfileVariantId = WPVL.CMSContainerProfileVariantId AND WPVL.LocaleId=@LocalId );			
		END

		--4. Specific Store, Any User Profile, Default Locale
		IF ISNULL(@GlobalEntityValueId,0) = 0
		BEGIN
			SELECT @LocalId = LocaleId FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;
			SELECT @LocaleCode = Code FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;

			SELECT @GlobalEntityValueId=CMSContainerProfileVariantId 
			FROM ZnodeCMSContainerProfileVariant WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = 0 ) AND (ISNULL(WPV.PortalId,0) = @PortalId )
			AND EXISTS(SELECT * FROM ZnodeCMSContentContainer CW WITH (NOLOCK) WHERE CW.ContainerKey=@ContainerKey AND WPV.CMSContentContainerId = CW.CMSContentContainerId)
			AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariantLocale WPVL WHERE WPV.CMSContainerProfileVariantId = WPVL.CMSContainerProfileVariantId AND WPVL.LocaleId=@LocalId );			
		
			IF ISNULL(@GlobalEntityValueId,0) = 0
			BEGIN
				SET @LocalId = @LocalIdInput
				SET @LocaleCode = @LocaleCodeInput
			END
		END
		
		--5. Any Store, Any User Profile, Selected Locale
		IF ISNULL(@GlobalEntityValueId,0) = 0
		BEGIN
			SELECT @GlobalEntityValueId=CMSContainerProfileVariantId 
			FROM ZnodeCMSContainerProfileVariant WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = 0 ) AND (ISNULL(WPV.PortalId,0) = 0 )
			AND EXISTS(SELECT * FROM ZnodeCMSContentContainer CW WITH (NOLOCK) WHERE CW.ContainerKey=@ContainerKey AND WPV.CMSContentContainerId = CW.CMSContentContainerId)
			AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariantLocale WPVL WHERE WPV.CMSContainerProfileVariantId = WPVL.CMSContainerProfileVariantId AND WPVL.LocaleId=@LocalId );			
		END

		--6. Any Store, Any User Profile, Default Locale
		IF ISNULL(@GlobalEntityValueId,0) = 0
		BEGIN
			SELECT @LocalId = LocaleId FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;
			SELECT @LocaleCode = Code FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;

			SELECT @GlobalEntityValueId=CMSContainerProfileVariantId 
			FROM ZnodeCMSContainerProfileVariant WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = 0 ) AND (ISNULL(WPV.PortalId,0) = 0 )
			AND EXISTS(SELECT * FROM ZnodeCMSContentContainer CW WITH (NOLOCK) WHERE CW.ContainerKey=@ContainerKey AND WPV.CMSContentContainerId = CW.CMSContentContainerId)
			AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariantLocale WPVL WHERE WPV.CMSContainerProfileVariantId = WPVL.CMSContainerProfileVariantId AND WPVL.LocaleId=@LocalId );			
		
			IF ISNULL(@GlobalEntityValueId,0) = 0
			BEGIN
				SET @LocalId = @LocalIdInput
				SET @LocaleCode = @LocaleCodeInput
			END
		END
		
		-- This will take FamilyCode based on the inputs.
		SELECT @FamilyCode=GAF.FamilyCode
		FROM ZnodeCMSContentContainer CCW WITH (NOLOCK)
		INNER JOIN ZnodeGlobalAttributeFamily GAF WITH (NOLOCK) ON CCW.FamilyId=GAF.GlobalAttributeFamilyId
		WHERE CCW.ContainerKey=@ContainerKey 
		AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariant CWPV WHERE CCW.CMSContentContainerId = CWPV.CMSContentContainerId AND (ISNULL(CWPV.PortalId,0)=@PortalId));

		-- Based on input @EntityName='Content Containers' as fixed value this will execute nested SP.
		-- Nested SP will give 2 Output (1 Result set & 1 Msg.
		IF @EntityName='Content Containers'
		begin
			 EXEC [dbo].[Znode_GetWidgetGlobalAttributeValue]
			 @EntityName=@EntityName,
			 @GlobalEntityValueId=@GlobalEntityValueId,
			 @LocaleCode=@LocaleCode,
			 @GroupCode =@GroupCode,
			 @SelectedValue = @SelectedValue;

		end
		
		-- This will take FamilyCode based on the inputs.
		SELECT @FamilyCode=GAF.FamilyCode
		FROM ZnodeCMSContentContainer CCW WITH (NOLOCK)
		INNER JOIN ZnodeGlobalAttributeFamily GAF WITH (NOLOCK) ON CCW.FamilyId=GAF.GlobalAttributeFamilyId
		WHERE CCW.ContainerKey=@ContainerKey 
		AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariant CWPV WHERE CCW.CMSContentContainerId = CWPV.CMSContentContainerId AND (ISNULL(CWPV.PortalId,0)=@PortalId));

		IF ISNULL(@FamilyCode,'') = ''
		BEGIN
			-- This will take FamilyCode based on the inputs.
			SELECT @FamilyCode=GAF.FamilyCode
			FROM ZnodeCMSContentContainer CCW WITH (NOLOCK)
			INNER JOIN ZnodeGlobalAttributeFamily GAF WITH (NOLOCK) ON CCW.FamilyId=GAF.GlobalAttributeFamilyId
			WHERE CCW.ContainerKey=@ContainerKey 
			AND EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariant CWPV WHERE CCW.CMSContentContainerId = CWPV.CMSContentContainerId AND (ISNULL(CWPV.PortalId,0)=0));
		END

		-- This will give 3rd result set of Attribute Groups Data.
		SELECT GE.GlobalEntityId, GE.EntityName, GE.IsActive, GE.TableName, GE.IsFamilyUnique
			, GAF.GlobalAttributeFamilyId, GAF.FamilyCode, GAF.IsSystemDefined, GAF.GlobalEntityId
			, GFGM.GlobalFamilyGroupMapperId, GFGM.GlobalAttributeFamilyId, GFGM.GlobalAttributeGroupId
			, GFGM.AttributeGroupDisplayOrder
			, GAG.GlobalAttributeGroupId, GAG.GroupCode, GAG.DisplayOrder, GAG.IsSystemDefined, GAG.GlobalEntityId
			, GAGL.GlobalAttributeGroupLocaleId, GAGL.LocaleId, GAGL.GlobalAttributeGroupId, GAGL.AttributeGroupName
			, GAGL.[Description]
		FROM ZnodeGlobalEntity GE WITH (NOLOCK)
		INNER JOIN ZnodeGlobalAttributeFamily GAF WITH (NOLOCK) ON GE.GlobalEntityId=GAF.GlobalEntityId
		INNER JOIN ZnodeGlobalFamilyGroupMapper GFGM WITH (NOLOCK) ON GAF.GlobalAttributeFamilyId=GFGM.GlobalAttributeFamilyId
		INNER JOIN ZnodeGlobalAttributeGroup GAG WITH (NOLOCK) ON GFGM.GlobalAttributeGroupId=GAG.GlobalAttributeGroupId
		INNER JOIN ZnodeGlobalAttributeGroupLocale GAGL WITH (NOLOCK) ON GAG.GlobalAttributeGroupId=GAGL.GlobalAttributeGroupId
		WHERE GE.EntityName=@EntityName AND GAGL.LocaleId=@LocalId AND GAF.FamilyCode=@FamilyCode
		ORDER BY GFGM.AttributeGroupDisplayOrder;

		-- This will give 4th result set of ContainerProfile & ContentContainer Data.
		SELECT CWPV.CMSContainerProfileVariantId, CWPV.CMSContentContainerId, CWPV.ProfileId, CWPVL.LocaleId, CWPVL.CMSContainerTemplateId
			,CCW.CMSContentContainerId, CCW.[Name], CCW.ContainerKey, CWPV.PortalId, CCW.FamilyId, CCW.Tags
		FROM ZnodeCMSContainerProfileVariant CWPV WITH (NOLOCK)
		INNER JOIN ZnodeCMSContainerProfileVariantLocale CWPVL WITH (NOLOCK) ON CWPV.CMSContainerProfileVariantId = CWPVL.CMSContainerProfileVariantId
		INNER JOIN ZnodeCMSContentContainer CCW WITH (NOLOCK) ON CWPV.CMSContentContainerId=CCW.CMSContentContainerId
		WHERE (ISNULL(CWPV.ProfileId,0) = @ProfileId OR ISNULL(CWPV.ProfileId,0) = 0) AND CWPVL.LocaleId=@LocalId AND CCW.ContainerKey=@ContainerKey AND ((ISNULL(CWPV.PortalId,0)=@PortalId OR ISNULL(CWPV.PortalId,0) = 0));

		-- This will give 5th result set of ContainerTemplate & ContentContainer Data.
		SELECT CWT.CMSContainerTemplateId, CWT.[FileName], CWT.MediaId, CCW.[Name], CCW.FamilyId, CCW.Tags
		FROM ZnodeCMSContentContainer CCW WITH (NOLOCK) 
		INNER JOIN ZnodeCMSContainerProfileVariant CWPV ON CCW.CMSContentContainerId = CWPV.CMSContentContainerId AND (ISNULL(CWPV.PortalId,0)=@PortalId OR ISNULL(CWPV.PortalId,0) = 0)
		INNER JOIN ZnodeCMSContainerProfileVariantLocale CWPVL ON CWPV.CMSContainerProfileVariantId = CWPVL.CMSContainerProfileVariantId
		INNER JOIN  ZnodeCMSContainerTemplate CWT WITH (NOLOCK) ON (CWPVL.CMSContainerTemplateId = CWT.CMSContainerTemplateId OR ISNULL(CWPVL.CMSContainerTemplateId,0) = 0)
		WHERE CCW.ContainerKey=@ContainerKey
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()
		DECLARE @Status BIT;
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetContentContainerAttributeValue
					@EntityName = '''+ISNULL(@EntityName,'''''')+
					''',@ContainerKey='+ISNULL(@ContainerKey,'''')+
					''',@LocalId='+ISNULL(CAST(@LocalId AS VARCHAR(20)),'''')+
					''',@PortalId='+ISNULL(CAST(@PortalId AS VARCHAR(20)),'''')+
					''',@ProfileId='+ISNULL(CAST(@ProfileId AS VARCHAR(20)),'''')
					
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetContentContainerAttributeValue',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH;
END;