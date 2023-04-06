﻿CREATE PROCEDURE [dbo].[Znode_GetPublishContentContainerAttributeValue]
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
	 EXEC [Znode_GetPublishContentContainerAttributeValue] @EntityName='Content Containers',@ContainerKey='',@LocalId=0,@PortalId=0,@ProfileId=0
	 ROLLBACK TRAN
*/
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		-- Variable Declaration as per the code requirement
		DECLARE @LocaleCode NVARCHAR(50),
				@LocalIdInput INT,
				@LocaleCodeInput NVARCHAR(50),
				@CMSContainerProfileVariantId INT,
				@GroupCode NVARCHAR(200) = NULL,
				@SelectedValue BIT = 0,
				@FamilyCode VARCHAR(200);

		SET @LocalIdInput = @LocalId

		SELECT @LocaleCode=Code FROM ZnodeLocale WITH (NOLOCK) WHERE LocaleId=@LocalId;
		SET @LocaleCodeInput = @LocaleCode

		--1. Specific Store, Specific User Profile, Selected Locale
		-- Based on input if count will 1 then this will take CMSContainerProfileVariantId from 1st loop else this will go for next loop.
		IF (SELECT COUNT(1) FROM ZnodePublishContentContainerVariantEntity WPV WITH (NOLOCK) WHERE ISNULL(WPV.PortalId,0)=@PortalId AND ISNULL(WPV.ProfileId,0)=@ProfileId  AND WPV.LocaleId=@LocalId)=1
		BEGIN
			SELECT @CMSContainerProfileVariantId=WPV.CMSContainerProfileVariantId 
			FROM ZnodePublishContentContainerVariantEntity WPV WITH (NOLOCK)
			WHERE ISNULL(WPV.ProfileId,0)=@ProfileId AND WPV.ContainerKey=@ContainerKey
			AND WPV.LocaleId=@LocalId
			AND WPV.IsActive=1
		END
		ELSE
		BEGIN
			SELECT @CMSContainerProfileVariantId=WPV.CMSContainerProfileVariantId 
			FROM ZnodePublishContentContainerVariantEntity WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = @ProfileId ) AND (ISNULL(WPV.PortalId,0) = @PortalId )
			AND WPV.ContainerKey=@ContainerKey AND WPV.LocaleId=@LocalId
			AND WPV.IsActive=1
		END

		--2. Specific Store, Specific User Profile, Default Locale
		IF ISNULL(@CMSContainerProfileVariantId,0) = 0
		BEGIN
			SELECT @LocalId = LocaleId FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;
			SELECT @LocaleCode = Code FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;

			SELECT @CMSContainerProfileVariantId= WPV.CMSContainerProfileVariantId 
			FROM ZnodePublishContentContainerVariantEntity WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = @ProfileId ) AND (ISNULL(WPV.PortalId,0) = @PortalId )
			AND WPV.ContainerKey=@ContainerKey AND WPV.LocaleId=@LocalId
			AND WPV.IsActive=1

			IF ISNULL(@CMSContainerProfileVariantId,0) = 0
			BEGIN
				SET @LocalId = @LocalIdInput
				SET @LocaleCode = @LocaleCodeInput
			END
		
		END

		--3. Specific Store, Any User Profile, Selected Locale
		IF ISNULL(@CMSContainerProfileVariantId,0) = 0
		BEGIN
			SELECT @CMSContainerProfileVariantId= WPV.CMSContainerProfileVariantId 
			FROM ZnodePublishContentContainerVariantEntity WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = 0 ) AND (ISNULL(WPV.PortalId,0) = @PortalId )
			AND WPV.ContainerKey=@ContainerKey AND WPV.LocaleId=@LocalId
			AND WPV.IsActive=1
		END

		--4. Specific Store, Any User Profile, Default Locale
		IF ISNULL(@CMSContainerProfileVariantId,0) = 0
		BEGIN
			SELECT @LocalId = LocaleId FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;
			SELECT @LocaleCode = Code FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;

			SELECT @CMSContainerProfileVariantId= WPV.CMSContainerProfileVariantId 
			FROM ZnodePublishContentContainerVariantEntity WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = 0 ) AND (ISNULL(WPV.PortalId,0) = @PortalId )
			AND WPV.ContainerKey=@ContainerKey AND WPV.LocaleId=@LocalId
			AND WPV.IsActive=1

			IF ISNULL(@CMSContainerProfileVariantId,0) = 0
			BEGIN
				SET @LocalId = @LocalIdInput
				SET @LocaleCode = @LocaleCodeInput
			END
		END
		
		--5. Any Store, Any User Profile, Selected Locale
		IF ISNULL(@CMSContainerProfileVariantId,0) = 0
		BEGIN
			SELECT @CMSContainerProfileVariantId= WPV.CMSContainerProfileVariantId 
			FROM ZnodePublishContentContainerVariantEntity WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = 0 ) AND (ISNULL(WPV.PortalId,0) = 0 )
			AND WPV.ContainerKey=@ContainerKey AND WPV.LocaleId=@LocalId
			AND WPV.IsActive=1
		END

		--6. Any Store, Any User Profile, Default Locale
		IF ISNULL(@CMSContainerProfileVariantId,0) = 0
		BEGIN
			SELECT @LocalId = LocaleId FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;
			SELECT @LocaleCode = Code FROM ZnodeLocale WITH (NOLOCK) WHERE IsDefault = 1;

			SELECT @CMSContainerProfileVariantId= WPV.CMSContainerProfileVariantId 
			FROM ZnodePublishContentContainerVariantEntity WPV WITH (NOLOCK)
			WHERE (ISNULL(WPV.ProfileId,0) = 0 ) AND (ISNULL(WPV.PortalId,0) = 0 )
			AND WPV.ContainerKey=@ContainerKey AND WPV.LocaleId=@LocalId
			AND WPV.IsActive=1

			IF ISNULL(@CMSContainerProfileVariantId,0) = 0
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
		AND EXISTS(SELECT * FROM ZnodePublishContentContainerVariantEntity CWPV WITH (NOLOCK) WHERE CCW.CMSContentContainerId = CWPV.CMSContentContainerId AND (ISNULL(CWPV.PortalId,0)=@PortalId));

		-- Based on input @EntityName='Content Containers' as fixed value this will execute nested SP.
		-- Nested SP will give 2 Output (1 Result set & 1 Msg.

		-- Declared table variable to combined nested sp data with the output of this sp.
		DECLARE	@PublishContainerGlobalAttributeValue As TABLE
		(CMSContainerProfileVariantId INT, ContentContainerName NVARCHAR(100), ContainerKey NVARCHAR(50), GlobalAttributes NVARCHAR(MAX), ContainerTemplateName NVARCHAR(100))

		IF @EntityName='Content Containers'
		begin
			INSERT INTO @PublishContainerGlobalAttributeValue
				(CMSContainerProfileVariantId,ContentContainerName, ContainerKey, GlobalAttributes, ContainerTemplateName)
			EXEC [dbo].[Znode_GetPublishContainerGlobalAttributeValue]
				@CMSContainerProfileVariantId=@CMSContainerProfileVariantId, @LocaleCode= @LocaleCode
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
		--SELECT GE.GlobalEntityId, GE.EntityName, GE.IsActive, GE.TableName, GE.IsFamilyUnique
		--	, GAF.GlobalAttributeFamilyId, GAF.FamilyCode, GAF.IsSystemDefined, GAF.GlobalEntityId
		--	, GFGM.GlobalFamilyGroupMapperId, GFGM.GlobalAttributeFamilyId, GFGM.GlobalAttributeGroupId
		--	, GFGM.AttributeGroupDisplayOrder
		--	, GAG.GlobalAttributeGroupId, GAG.GroupCode, GAG.DisplayOrder, GAG.IsSystemDefined, GAG.GlobalEntityId
		--	, GAGL.GlobalAttributeGroupLocaleId, GAGL.LocaleId, GAGL.GlobalAttributeGroupId, GAGL.AttributeGroupName
		--	, GAGL.[Description]
		--FROM ZnodeGlobalEntity GE WITH (NOLOCK)
		--INNER JOIN ZnodeGlobalAttributeFamily GAF WITH (NOLOCK) ON GE.GlobalEntityId=GAF.GlobalEntityId
		--INNER JOIN ZnodeGlobalFamilyGroupMapper GFGM WITH (NOLOCK) ON GAF.GlobalAttributeFamilyId=GFGM.GlobalAttributeFamilyId
		--INNER JOIN ZnodeGlobalAttributeGroup GAG WITH (NOLOCK) ON GFGM.GlobalAttributeGroupId=GAG.GlobalAttributeGroupId
		--INNER JOIN ZnodeGlobalAttributeGroupLocale GAGL WITH (NOLOCK) ON GAG.GlobalAttributeGroupId=GAGL.GlobalAttributeGroupId
		--WHERE GE.EntityName=@EntityName AND GAGL.LocaleId=@LocalId AND GAF.FamilyCode=@FamilyCode
		--ORDER BY GFGM.AttributeGroupDisplayOrder;

		-- This will give 4th result set of ContainerProfile & ContentContainer Data.

		;WITH Cte_CMSContainerProfileVariant AS
		(
		SELECT DISTINCT CWPV.PublishContentContainerVariantEntityId,CWPV.CMSContainerProfileVariantId, CWPV.CMSContentContainerId, CGAV.ContentContainerName, CWPV.ProfileId
			, CWPV.LocaleId, CWPV.CMSContainerTemplateId, CGAV.ContainerTemplateName
			, CCW.[Name], CCW.ContainerKey, CWPV.PortalId, CCW.FamilyId, CCW.Tags
			, CGAV.GlobalAttributes
		FROM ZnodePublishContentContainerVariantEntity CWPV WITH (NOLOCK)
		INNER JOIN ZnodeCMSContentContainer CCW WITH (NOLOCK) ON CWPV.CMSContentContainerId=CCW.CMSContentContainerId
		INNER JOIN @PublishContainerGlobalAttributeValue CGAV ON CCW.ContainerKey=CGAV.ContainerKey
			AND CWPV.CMSContainerProfileVariantId=CGAV.CMSContainerProfileVariantId
		WHERE CWPV.IsActive=1 AND ( (ISNULL(CWPV.ProfileId,0) = @ProfileId OR ISNULL(CWPV.ProfileId,0) = 0) AND CWPV.LocaleId=@LocalId AND CCW.ContainerKey=@ContainerKey
			AND ((ISNULL(CWPV.PortalId,0)=@PortalId OR ISNULL(CWPV.PortalId,0) = 0)))
		)
		,Cte_GetFirstFilterData AS
		(
		SELECT DISTINCT CWPV.PublishContentContainerVariantEntityId,CWPV.CMSContainerProfileVariantId, CWPV.CMSContentContainerId, CGAV.ContentContainerName, CWPV.ProfileId
			, CWPV.LocaleId, CWPV.CMSContainerTemplateId, CGAV.ContainerTemplateName
			, CCW.[Name], CCW.ContainerKey, CWPV.PortalId, CCW.FamilyId, CCW.Tags
			, CGAV.GlobalAttributes
		FROM ZnodePublishContentContainerVariantEntity CWPV WITH (NOLOCK)
		INNER JOIN ZnodeCMSContentContainer CCW WITH (NOLOCK) ON CWPV.CMSContentContainerId=CCW.CMSContentContainerId
		INNER JOIN @PublishContainerGlobalAttributeValue CGAV ON CCW.ContainerKey=CGAV.ContainerKey
		WHERE CWPV.ProfileId IS NULL AND CCW.ContainerKey=@ContainerKey AND CWPV.PortalId IS NULL AND CWPV.LocaleId=@LocalId
		)

		SELECT DISTINCT CMSContainerProfileVariantId, CMSContentContainerId, ContentContainerName, ProfileId
			, LocaleId, CMSContainerTemplateId, ContainerTemplateName
			, [Name], ContainerKey, PortalId, FamilyId, Tags
			, GlobalAttributes
		FROM Cte_CMSContainerProfileVariant
		UNION
		SELECT DISTINCT CMSContainerProfileVariantId, CMSContentContainerId, ContentContainerName, ProfileId
			, LocaleId, CMSContainerTemplateId, ContainerTemplateName
			, [Name], ContainerKey, PortalId, FamilyId, Tags
			, GlobalAttributes
		FROM Cte_GetFirstFilterData CWPV
		WHERE NOT EXISTS (SELECT * FROM Cte_CMSContainerProfileVariant WHERE PublishContentContainerVariantEntityId=CWPV.PublishContentContainerVariantEntityId)
			AND NOT EXISTS (SELECT * FROM Cte_CMSContainerProfileVariant)

		-- This will give 5th result set of ContainerTemplate & ContentContainer Data.
		--SELECT CWT.CMSContainerTemplateId, CWT.[FileName], CWT.MediaId, CCW.[Name], CCW.FamilyId, CCW.Tags
		--FROM ZnodeCMSContentContainer CCW WITH (NOLOCK) 
		--INNER JOIN ZnodeCMSContainerProfileVariant CWPV ON CCW.CMSContentContainerId = CWPV.CMSContentContainerId AND (ISNULL(CWPV.PortalId,0)=@PortalId OR ISNULL(CWPV.PortalId,0) = 0)
		--INNER JOIN ZnodeCMSContainerProfileVariantLocale CWPVL ON CWPV.CMSContainerProfileVariantId = CWPVL.CMSContainerProfileVariantId
		--INNER JOIN  ZnodeCMSContainerTemplate CWT WITH (NOLOCK) ON (CWPVL.CMSContainerTemplateId = CWT.CMSContainerTemplateId OR ISNULL(CWPVL.CMSContainerTemplateId,0) = 0)
		--WHERE CCW.ContainerKey=@ContainerKey
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()
		DECLARE @Status BIT;
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishContentContainerAttributeValue
					@EntityName = '''+ISNULL(@EntityName,'''''')+
					''',@ContainerKey='+ISNULL(@ContainerKey,'''')+
					''',@LocalId='+ISNULL(CAST(@LocalId AS VARCHAR(20)),'''')+
					''',@PortalId='+ISNULL(CAST(@PortalId AS VARCHAR(20)),'''')+
					''',@ProfileId='+ISNULL(CAST(@ProfileId AS VARCHAR(20)),'''')
					
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetPublishContentContainerAttributeValue',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH;
END;